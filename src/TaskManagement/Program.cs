using Microsoft.Extensions.Configuration;
using Serilog;
using TaskManagement.API;
using TaskManagement.API.Middlawares;
using TaskManagement.Application;
using TaskManagement.Application.Common;
using TaskManagement.Application.Features.Tasks.Queries;
using TaskManagement.Application.Helpers;
using TaskManagement.Application.Helpers.GenerateJwt;
using TaskManagement.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
var config = builder.Configuration;

builder.Services.AddHttpContextAccessor(); // Faqat bir marta ro'yxatdan o'tkaziladi
builder.Services.Configure<EmailConfiguration>(config.GetSection("EmailConfiguration"));
builder.Services.Configure<JwtOption>(config.GetSection("JwtOption"));

builder.Services.AddAuth(config);


Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/app-log.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Seq("http://localhost:5341")
            .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetAllTasksQueryHandler).Assembly);
});

builder.Services.AddApplicationServices(config);
builder.Services.AddPersistenceServices(config);

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Secure Login API v1");
//    });
//}

app.UseSwagger(options => options.OpenApiVersion =
Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0);
app.UseSwaggerUI();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
