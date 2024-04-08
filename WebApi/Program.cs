using Application;
using Infrastructure;
using Serilog;
using WebApi;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddIdentitySettings();

builder.Services.AddApplicationServices();

builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSettings(builder.Configuration));

builder.Services.AddInfrastructureServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterSwagger();

builder.Services.AddInfrastructureDependencies();

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.SeedDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
