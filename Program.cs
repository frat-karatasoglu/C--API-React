using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyReactApp",
        builder =>
        {
            // Buraya React'in çalıştığı adresi yazıyoruz.

            builder.WithOrigins("http://localhost:5173")
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

app.MapPost("/WeatherForecast", async (WeatherForecast forecast, WeatherDb db) =>
{
    // Gelen veriyi "veritabanımıza" (listemize) ekle
    db.WeatherForecasts.Add(forecast);
    await db.SaveChangesAsync();

    // Başarılı olduğunu söyle ve eklenen veriyi geri döndür
    return Results.CreatedAtRoute("GetWeatherForecast", null, forecast);
})
.WithName("PostWeatherForecast");



app.Run();
