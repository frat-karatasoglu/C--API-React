using Microsoft.EntityFrameworkCore;

// 1. ADIM: "VERİTABANI PLANI" (DbContext)
// Bizim veritabanımızı temsil eden class.
// EF Core'a "Benim veritabanımla böyle konuş" diyoruz.
public class WeatherDb : DbContext
{
    // Bu constructor (yapıcı metot) EF Core'un çalışması için gerekli.
    // Veritabanı ayarlarını (nerede, nasıl bağlanacak) almasını sağlar.
    public WeatherDb(DbContextOptions<WeatherDb> options) : base(options) { }

    // 2. ADIM: "TABLO" TANIMI
    // EF Core'a "Bu veritabanında 'WeatherForecasts' adında bir tablo olsun" diyoruz.
    // "Bu tablo, 'WeatherForecast' tipindeki verileri tutacak."
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
}


// 3. ADIM: "MODEL" (VERİNİN ŞEKLİ)
// Bu, bizim 'record WeatherForecast' tanımımızın daha esnek bir hali (class).
// EF Core, veritabanına bir "Id" (kimlik) kolonu eklemeyi sever.
// Bu yüzden 'record' yerine bunu kullanacağız.
public class WeatherForecast
{
    public int Id { get; set; } // Veritabanındaki her satır için benzersiz kimlik (Primary Key)
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}