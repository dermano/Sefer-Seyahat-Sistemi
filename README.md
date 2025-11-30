🚀 Kullanılan Teknolojiler
🖥️ Uygulama Geliştirme

C# .NET Framework – Windows Forms

Visual Studio 2022

Ado.NET (SQL bağlantı işlemleri)

🗄️ Veritabanı

Microsoft SQL Server

Tablolar:

TBLYOLCUBILGI → Yolcu bilgileri (Ad, Soyad, Telefon, TC, Cinsiyet, Mail)

TBLKAPTAN → Kaptan no, ad soyad, telefon

TBLSEFERBILGI → Sefer kalkış-varış, tarih, saat, kaptan, fiyat

TBLSEFERDETAY → Sefer rezervasyonları (SeferNo, Yolcu TC, Koltuk No)

🚌 Özellikler

✔️ Sefer oluşturma

✔️ Kaptan ekleme ve seferlere kaptan atama

✔️ Yolcu kaydı

✔️ Dinamik koltuk tasarımı (2+1 Travego düzeninde)

✔️ Kaptan koltuğu (K)

✔️ Dolu koltukların otomatik renklendirilmesi

Erkek → Mavi

Kadın → Pembe

✔️ Sefere göre dolu koltukların otomatik işaretlenmesi

✔️ TC ile yolcuya göre koltuk rezervasyonu

✔️ Gerçek zamanlı koltuk numarası seçimi

📌 Teknik Notlar

Koltukların durumu (dolu/boş) TBLSEFERDETAY üzerinden kontrol edilir.

Cinsiyete göre renklendirme için TBLYOLCUBILGI tablosu ile JOIN yapılır.

Veriler ADO.NET ile yönetilir (SqlConnection, SqlCommand, SqlDataReader, DataAdapter).

UI tamamen WinForms üzerinde dinamik olarak oluşturulmuştur.
