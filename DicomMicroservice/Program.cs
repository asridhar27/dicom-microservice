
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Split(" ").Last();
                context.Token = token;
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dicom API", Version = "v1" });
    c.EnableAnnotations();
});

builder.Configuration.AddJsonFile("appsettings.json", optional: false);

builder.Services.AddSingleton<IDicomService, DicomService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var uploadDirectory = configuration["UploadDirectory"];
    var sampleFilePath = configuration["DicomDirectory"];
    var pngDirectory = configuration["UploadPNGDirectory"];
    return new DicomService(uploadDirectory, sampleFilePath, pngDirectory);
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dicom API v1"));
}

app.UseHttpsRedirection();

// Add authorization middleware
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();


