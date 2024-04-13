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
        // Belirli bir süre içinde izin verilen maksimum istek sayýsý
        options.PermitLimit = 10;

        // Pencere boyunca sürekli olarak kayan zaman dilimi
        options.Window = TimeSpan.FromSeconds(10);

        // Pencere içindeki segment sayýsý (sürekli kayan pencere bölünmüþ olabilir)
        options.SegmentsPerWindow = 2;

        // Kuyruk iþleme sýrasý (en eski isteklerden baþlayarak iþleme almak için)
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

        // Pencere içinde iþleme alýnacak maksimum kuyruk uzunluðu
        options.QueueLimit = 5;
    });

    options.AddTokenBucketLimiter("token", options =>
    {
        // Belirli bir süre içinde maksimum token sayýsý
        options.TokenLimit = 20;

        // Kuyruk iþleme sýrasý (en eski isteklerden baþlayarak iþleme almak için)
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

        // Tokenlerin yenilendiði periyot
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);

        // Her yenileme periyodunda eklenen token sayýsý
        options.TokensPerPeriod = 20;

        // Otomatik olarak tokenleri yenileme özelliði
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
