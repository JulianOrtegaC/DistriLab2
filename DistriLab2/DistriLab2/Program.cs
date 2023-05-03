using Azure.Storage.Blobs;
using DistriLab2.Models.DB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<dblab2Context>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conexion"));
});
builder.Services.AddCors(options => options.AddPolicy("AllowAngularOrigins",
                                    builder => builder.AllowAnyOrigin()
                                                    .WithOrigins("https://black-bay-05044b510.3.azurestaticapps.net")
                                                    .AllowAnyHeader()
                                                    .AllowAnyMethod()));

builder.Services.AddScoped(_ => {
    return new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage"));

});


builder.Configuration.AddJsonFile("appsettings.json");
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var secretKey = builder.Configuration.GetSection("settings").GetSection("secretKey").ToString();
var keyByte = Encoding.UTF8.GetBytes(secretKey);
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyByte),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
    };
});

var app = builder.Build();
app.UseCors("AllowAngularOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
