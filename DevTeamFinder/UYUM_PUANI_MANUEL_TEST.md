# UYUM PUANI MANUEL TEST KILAVUZU

## 🚀 HIZLI TEST (5 Dakika)

### ADIM 1: Uygulamayı Başlat
```bash
cd /Users/ahmet/Desktop/proje/DevTeamFinder
dotnet run
```
Tarayıcıda: `http://localhost:5000`

---

### ADIM 2: İlk Kullanıcı Oluştur (Proje Sahibi)

1. **Kayıt Ol** butonuna tıkla
2. Bilgileri gir:
   - **Email:** proje@test.com
   - **Şifre:** Test123!
   - **Ad Soyad:** Proje Sahibi
   - **Hakkında:** Full Stack Developer
   - **Deneyim:** Senior
   - **Skill'ler Seç:** 
     - ✅ C#
     - ✅ ASP.NET Core
     - ✅ React
     - ✅ SQL
     - ✅ Docker
3. **Kayıt Ol** butonuna tıkla

---

### ADIM 3: İkinci Kullanıcı Oluştur (Developer 1)

1. **Çıkış Yap**
2. **Kayıt Ol** butonuna tıkla
3. Bilgileri gir:
   - **Email:** dev1@test.com
   - **Şifre:** Test123!
   - **Ad Soyad:** Developer 1
   - **Hakkında:** Frontend Developer
   - **Deneyim:** Mid-Level
   - **Skill'ler Seç:** 
     - ✅ C#
     - ✅ ASP.NET Core
     - ✅ React
     - ✅ JavaScript
4. **Kayıt Ol** butonuna tıkla

**BEKLENEN UYUM:** 3/5 = %60 ✅ (Önerilenlerde görünür)

---

### ADIM 4: Üçüncü Kullanıcı Oluştur (Developer 2)

1. **Çıkış Yap**
2. **Kayıt Ol** butonuna tıkla
3. Bilgileri gir:
   - **Email:** dev2@test.com
   - **Şifre:** Test123!
   - **Ad Soyad:** Developer 2
   - **Hakkında:** Backend Developer
   - **Deneyim:** Junior
   - **Skill'ler Seç:** 
     - ✅ C#
     - ✅ React
     - ✅ Node.js
     - ✅ MongoDB
4. **Kayıt Ol** butonuna tıkla

**BEKLENEN UYUM:** 2/5 = %40 ❌ (Önerilenlerde görünmez)

---

### ADIM 5: Proje Oluştur

1. **Çıkış Yap**
2. **proje@test.com** ile giriş yap (Şifre: Test123!)
3. **Yeni Proje Oluştur** butonuna tıkla
4. Proje bilgilerini gir:
   - **Başlık:** E-Ticaret Platformu
   - **Açıklama:** Modern bir e-ticaret sitesi geliştiriyoruz. ASP.NET Core backend ve React frontend kullanıyoruz.
   - **Skill'ler Seç:**
     - ✅ C#
     - ✅ ASP.NET Core
     - ✅ React
     - ✅ SQL
     - ✅ Docker
5. **Oluştur** butonuna tıkla

---

### ADIM 6: Uyum Puanlarını Gör

1. **Projelerim** sayfasında "E-Ticaret Platformu" projesine tıkla
2. Proje detay sayfasında **sağ tarafta** "Geliştiricilere Davet Gönder" bölümünü gör
3. **BEKLENEN SONUÇLAR:**

```
┌─────────────────────────────────────┐
│ Geliştiricilere Davet Gönder       │
├─────────────────────────────────────┤
│ Developer 1                         │
│ Mid-Level                           │
│ %60 Uyum                           │ ← BURADA GÖRÜNMELI
│ [Davet Gönder]                     │
├─────────────────────────────────────┤
│ Developer 2                         │
│ Junior                              │
│ %40 Uyum                           │ ← BURADA GÖRÜNMELI
│ [Davet Gönder]                     │
└─────────────────────────────────────┘
```

4. Aşağıda **"Önerilen Geliştiriciler"** bölümünü gör
5. **BEKLENEN SONUÇ:**
   - ✅ **Developer 1** görünmeli (%60 uyum, %50+ eşiği geçiyor)
   - ❌ **Developer 2** görünmemeli (%40 uyum, %50 altı)

```
┌─────────────────────────────────────┐
│ Önerilen Geliştiriciler             │
│ (Projenize en uygun geliştiriciler) │
├─────────────────────────────────────┤
│ Developer 1                         │
│ Mid-Level                           │
│ %60 Uyum                           │ ← SADECE BU GÖRÜNMELI
│ [Projeme Katıl]                    │
└─────────────────────────────────────┘
```

---

## 🎯 FARKLI SENARYOLAR TEST ET

### Senaryo 1: %100 Uyum

**Yeni Developer Oluştur:**
- Email: dev3@test.com
- Skill'ler: C#, ASP.NET Core, React, SQL, Docker (AYNI skill'ler)
- **Beklenen:** %100 uyum, önerilenlerde en üstte

### Senaryo 2: %20 Uyum

**Yeni Developer Oluştur:**
- Email: dev4@test.com
- Skill'ler: Python, Django, Flask, PostgreSQL
- Ortak: Sadece SQL varsa %20 (1/5)
- **Beklenen:** Önerilenlerde görünmez

### Senaryo 3: Skill Ekle/Çıkar

1. **dev2@test.com** ile giriş yap
2. **Profilim** sayfasına git
3. Skill ekle: SQL, Docker
4. **Çıkış Yap** ve **proje@test.com** ile giriş yap
5. Proje detayına git
6. **Beklenen:** Developer 2 artık %80 uyum ile önerilenlerde görünmeli

---

## 🔍 KONTROL LİSTESİ

Test sırasında bunları kontrol et:

- [ ] Uyum puanı doğru hesaplanıyor mu?
- [ ] %50+ uyum olanlar önerilenlerde görünüyor mu?
- [ ] %50 altı olanlar önerilenlerde görünmüyor mu?
- [ ] Uyum puanı en yüksekten düşüğe sıralı mı?
- [ ] Skill eklediğinde uyum puanı artıyor mu?
- [ ] Skill çıkardığında uyum puanı azalıyor mu?
- [ ] Maksimum 5 öneri gösteriliyor mu?

---

## 📊 HESAPLAMA ÖRNEKLERİ

### Örnek 1: Developer 1
```
Proje Skill'leri:     C#, ASP.NET Core, React, SQL, Docker (5 skill)
Developer 1 Skill'leri: C#, ASP.NET Core, React, JavaScript (4 skill)
Ortak Skill'ler:      C#, ASP.NET Core, React (3 skill)

Uyum Puanı = (3 / 5) × 100 = %60
```

### Örnek 2: Developer 2
```
Proje Skill'leri:     C#, ASP.NET Core, React, SQL, Docker (5 skill)
Developer 2 Skill'leri: C#, React, Node.js, MongoDB (4 skill)
Ortak Skill'ler:      C#, React (2 skill)

Uyum Puanı = (2 / 5) × 100 = %40
```

---

## 🐛 SORUN GİDERME

### Uyum puanı görünmüyor
**Çözüm:**
1. Tarayıcıyı yenile (Ctrl+F5)
2. Developer'ın skill'leri var mı kontrol et
3. Projenin skill'leri var mı kontrol et

### Yanlış uyum puanı
**Çözüm:**
1. Developer skill'lerini kontrol et (Profilim sayfası)
2. Proje skill'lerini kontrol et (Proje düzenle)
3. Veritabanını kontrol et:
```bash
sqlite3 devteamfinder.db "SELECT * FROM DeveloperSkills WHERE DeveloperId = 2;"
sqlite3 devteamfinder.db "SELECT * FROM ProjectSkills WHERE ProjectId = 1;"
```

### Önerilen geliştiriciler boş
**Çözüm:**
- Hiçbir developer %50+ uyum sağlamıyor
- Daha fazla developer ekle veya skill'leri güncelle

---

## ✅ BAŞARILI TEST

Eğer aşağıdakiler çalışıyorsa test başarılı:

1. ✅ Developer 1 (%60 uyum) önerilenlerde görünüyor
2. ✅ Developer 2 (%40 uyum) önerilenlerde görünmüyor
3. ✅ Her iki developer'ın uyum puanı "Geliştiricilere Davet Gönder" bölümünde görünüyor
4. ✅ Uyum puanları doğru hesaplanıyor
5. ✅ Skill eklendiğinde uyum puanı artıyor

---

## 🎉 SONUÇ

Uyum puanı sistemi çalışıyor! Artık:
- Proje sahipleri en uygun developer'ları kolayca bulabilir
- Developer'lar kendilerine uygun projeleri görebilir
- Akıllı eşleştirme sistemi çalışıyor

**Keyifli testler! 🚀**
