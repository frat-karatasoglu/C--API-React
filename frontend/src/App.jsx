import { useState, useEffect } from 'react';

function App() {
  const [data, setData] = useState([]); // Gelen veriyi tutacak state
  const [error, setError] = useState(null); // Hata olursa tutacak state

  const fetchData = () => {
    {
      // C# API'mıza istek atıyoruz
      fetch('http://localhost:5191/WeatherForecast')
        .then(response => {
          if (!response.ok) {
            throw new Error('Network response was not ok');
          }
          return response.json();
        })
        .then(json => {

          setData(json); // Başarılıysa veriyi state'e yaz
        })
        .catch(error => {
          setError(error); // Başarısızsa hatayı state'e yaz
          console.error('Fetch error:', error);
        });
    }
  };

  const handlePost = () => {
    // 1. C#'a göndereceğimiz veriyi hazırla.
    const newForecast = {
      date: new Date().toISOString().split('T')[0], // "2025-10-24" formatı verir
      temperatureC: Math.floor(Math.random() * 50) - 10, // -10 ile 40 arası rastgele
      summary: "React'ten Geldi"
    };

    // 2. fetch'i POST için ayarla
    fetch('http://localhost:5191/WeatherForecast', { // Aynı adrese gidiyoruz
      method: 'POST', // Ama metodu 'POST' olarak belirtiyoruz
      headers: {
        'Content-Type': 'application/json', // "Sana JSON yolluyorum" diyoruz
      },
      body: JSON.stringify(newForecast), // Hazırladığımız objeyi JSON metnine çevirip yolluyoruz
    })
      .then(response => {
        if (!response.ok) { // Cevap 200 (OK) değilse (örn: 400, 500 hatası)
          throw new Error('POST isteği başarısız oldu');
        }
        return response.json(); // C#'ın yolladığı "Başarıyla eklendi" cevabını al
      })
      .then(postedData => {
        // 3. EN ÖNEMLİ ADIM: Başarılıysa listeyi yenile!
        console.log('Başarıyla gönderildi:', postedData);
        fetchData(); // GET'i tekrar çalıştır ki yeni veri ekrana gelsin.
      })
      .catch(error => {
        // Bir hata olursa yakala
        setError(error);
        console.error('Post error:', error);
      });
  }

  useEffect(() => {
    fetchData();
  }, []);
  if (error) {
    return <div>Hata oluştu: {error.message}</div>;
  }

  if (!data) {
    return <div>Yükleniyor...</div>;
  }

  // Veri başarıyla geldiyse, JSON'u ekrana bas
  return (
    <div>
      {/* 1. Butonumuz: Tıklandığında 'handlePost'u çalıştıracak */}
      <button onClick={handlePost}>
        Sunucuya Yeni Veri Gönder (POST)
      </button>

      <h1>Sunucudaki Veri:</h1>

      {/* 2. Görüntüleme Mantığı (Koşullu Render) */}
      <pre>
        {
          error ? (
            // Eğer 'error' state'i doluysa, hatayı göster
            `Hata oluştu: ${error.message}`
          ) : data.length === 0 ? (
            // 'error' yoksa VE 'data' listesi boşsa, bunu göster
            "Sunucuda veri yok."
          ) : (
            // 'error' yoksa VE 'data' listesinde veri varsa, JSON'u göster
            JSON.stringify(data, null, 2)
          )
        }
      </pre>
    </div>
  );
}

export default App;