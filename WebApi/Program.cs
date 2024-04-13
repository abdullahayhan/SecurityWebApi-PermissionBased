using Application;
using Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System.Threading.RateLimiting;
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

builder.Services.AddCors(o =>
     o.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader())
    );

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddSlidingWindowLimiter("sliding", options =>
    {
        // Belirli bir s�re i�inde izin verilen maksimum istek say�s�
        options.PermitLimit = 10;

        // Pencere boyunca s�rekli olarak kayan zaman dilimi
        options.Window = TimeSpan.FromSeconds(10);

        // Pencere i�indeki segment say�s� (s�rekli kayan pencere b�l�nm�� olabilir)
        options.SegmentsPerWindow = 2;

        // Kuyruk i�leme s�ras� (en eski isteklerden ba�layarak i�leme almak i�in)
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

        // Pencere i�inde i�leme al�nacak maksimum kuyruk uzunlu�u
        options.QueueLimit = 5;
    });

    options.AddTokenBucketLimiter("token", options =>
    {
        // Belirli bir s�re i�inde maksimum token say�s�
        options.TokenLimit = 20;

        // Kuyruk i�leme s�ras� (en eski isteklerden ba�layarak i�leme almak i�in)
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

        // Tokenlerin yenilendi�i periyot
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);

        // Her yenileme periyodunda eklenen token say�s�
        options.TokensPerPeriod = 20;

        // Otomatik olarak tokenleri yenileme �zelli�i
        options.AutoReplenishment = true;
    });

});


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

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
