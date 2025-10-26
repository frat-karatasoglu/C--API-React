# C# API, React & PostgreSQL Full-Stack Projesi (Docker & Render ile Deploy Edildi)

Bu proje, C# (.NET 9) ile oluşturulmuş bir "Beyin" (Backend API), React ile oluşturulmuş bir "Yüz" (Frontend) ve PostgreSQL ile oluşturulmuş bir "Hafıza"dan (Veritabanı) oluşan tam yığın (Full-Stack) bir uygulamadır.

Tüm servisler (API, Frontend ve Veritabanı) **Render** üzerinde, **DevSecOps** prensiplerine uygun olarak "headless" (birbirinden bağımsız) bir yapıda deploy edilmiştir.

## 🚀 Canlı Linkler

* **Frontend (Yüz):** [https://c-api-react-1.onrender.com](https://c-api-react-1.onrender.com)
* **Backend (Beyin) API Endpoint'i:** [https://c-api-react.onrender.com/WeatherForecast](https://c-api-react.onrender.com/WeatherForecast)

## 🏛️ Proje Mimarisi

Bu proje, 3 bağımsız servisten oluşur:

1.  **Backend (API):**
    * `https://c-api-react.onrender.com`
    * C# .NET 9 Minimal API kullanılarak oluşturulmuştur.
    * Proje, bir `Dockerfile` kullanılarak "konteynerize" edilmiş ve Render'a "Web Service" olarak deploy edilmiştir.
    * Entity Framework Core (EF Core) kullanarak veritabanı işlemleri (CRUD) yapar.

2.  **Frontend (Yüz):**
    * `https://c-api-react-1.onrender.com`
    * React (Vite) kullanılarak oluşturulmuştur.
    * Render'a "Static Site" (Statik Site) olarak deploy edilmiştir.
    * API (Beyin) ile `fetch` kullanarak konuşur.

3.  **Database (Hafıza):**
    * Render üzerinde host edilen *gerçek* bir PostgreSQL sunucusudur.
    * **Kritik Not:** API (Beyin) ve Veritabanı (Hafıza), güvenlik ve hız için Render'ın *iç ağı* (Internal Database URL) üzerinden konuşur. Frontend (Yüz) ise API'a *dış ağdan* (External URL) erişir.

## 🔒 Güvenlik & DevSecOps Prensibi (En Önemli Kısım)

Bu proje, "production" (canlı) ortam güvenliğini ciddiye alır.

* **Sır Yönetimi (Secret Management):** `appsettings.json` ve `appsettings.Development.json` dosyaları `.gitignore` içine eklenerek *asla* GitHub'a gönderilmez. Bu, API anahtarları veya veritabanı şifreleri gibi gizli bilgilerin sızmasını engeller.
* **Ortam Ayrımı (Separation of Environments):** Tüm gizli veritabanı bağlantı bilgileri (`ConnectionStrings`), *sadece* Render sunucusunda **"Environment Variables" (Ortam Değişkenleri)** olarak saklanır. Kodun kendisi (`Program.cs`) temizdir ve hiçbir sır içermez.
* **CORS Politikası:** `Program.cs` içindeki CORS (Cross-Origin Resource Sharing) politikası, API'ın *sadece* `localhost`'tan (geliştirme için) ve `https://c-api-react-1.onrender.com` (canlı frontend) adresinden gelen isteklere cevap vermesine izin verecek şekilde yapılandırılmıştır.

## 🛠️ Kullanılan Teknolojiler

* **Backend:** C# (.NET 9.0), Entity Framework Core, Npgsql
* **Frontend:** React (Vite), JavaScript
* **Database:** PostgreSQL
* **DevOps:** Docker, Render (CI/CD & Hosting), Git & GitHub

## 📝 Gelecekteki Bana Not: Yerelde (Localhost) Veritabanını İnşa Etmek

Canlı (Render) veritabanı şeması (`WeatherForecasts` tablosu) güncellenirse, şu adımlar izlenmelidir:

1.  `appsettings.json` dosyasına *geçici* olarak `ConnectionStrings` bloğu eklenir.
2.  Bu bloğa, Render'daki veritabanının **"External Database URL"** (Dış Adres) bilgisi, `Host=...;Port=...;` formatına çevrilerek yapıştırılır.
3.  Yerel terminalde `dotnet ef database update` komutu çalıştırılarak buluttaki veritabanı "inşa edilir".
4.  İşlem bittikten sonra, `appsettings.json` dosyasındaki o `ConnectionStrings` bloğu **tamamen silinir.**
5.  Temizlenmiş `appsettings.json` dosyası `git commit` yapılır.
