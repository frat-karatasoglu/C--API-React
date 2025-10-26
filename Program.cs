using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyReactApp",
        builder =>
        {
            // Buraya React'in çalıştığı adresi yazıyoruz.

            builder.WithOrigins("http://localhost:5173", "https://c-api-react-1.onrender.com")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
// Veritabanı "planımızı" (WeatherDb) projeye bir servis olarak ekliyoruz.
builder.Services.AddDbContext<WeatherDb>(options =>
{
    // Ve EF Core'a "SQLite kullan" diyoruz.
    // Veritabanı dosyası olarak da "weather.db" adını veriyoruz.
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowMyReactApp");


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/WeatherForecast", async (WeatherDb db) =>
{

    return await db.WeatherForecasts.ToListAsync();
})
.WithName("GetWeatherForecast");

app.MapPost("/WeatherForecast", async (HttpContext context, WeatherForecast forecast, WeatherDb db, IConfiguration config) =>
{

    // 1. "Anahtar Kasası"ndan (Environment) "ApiKey"i oku
    var apiKey = config["ApiKey"];

    // 2. Gelen isteğin başlığından (Header) "X-API-Key"i oku
    if (!context.Request.Headers.TryGetValue("X-API-Key", out var gelenAnahtar))
    {
        // Eğer "X-API-Key" başlığı HİÇ YOKSA, reddet.
        return Results.Problem(
    detail: "API Key Gerekli (Header: X-API-Key)",
    statusCode: StatusCodes.Status401Unauthorized
);
    }

    // 3. İki anahtarı karşılaştır
    if (apiKey != gelenAnahtar)
    {
        // Eğer anahtarlar EŞLEŞMİYORSA, reddet.
        return Results.Problem(
    detail: "API Key Gecersiz",
    statusCode: StatusCodes.Status401Unauthorized
);
    }

    // 4. GÜVENLİK GEÇİLDİ. Artık normal işe devam et.
    db.WeatherForecasts.Add(forecast);
    await db.SaveChangesAsync();

    // Başarılı olduğunu söyle ve eklenen veriyi geri döndür
    return Results.CreatedAtRoute("GetWeatherForecast", null, forecast);

})
.WithName("PostWeatherForecast");

app.Run();