# 📖 ADIM 11.1-11.4 Test Kılavuzu - BASİT AÇIKLAMA

## 🎯 Neyi Test Edeceğiz?

### ADIM 11.1: Projeye Katılan Geliştiriciler Listesi
**Ne yapmalı:** Proje detay sayfasında "Proje Ekibi" bölümünde, projeye katılan geliştiriciler görünmeli.

### ADIM 11.2: Skill Uyum Skoru
**Ne yapmalı:** Her geliştiricinin yanında "%66 Uyum" gibi bir skor görünmeli.

### ADIM 11.3: Önerilen Geliştiriciler
**Ne yapmalı:** Proje detay sayfasında "Önerilen Geliştiriciler" bölümü görünmeli.

### ADIM 11.4: Soft Delete (IsActive)
**Ne yapmalı:** Silinen (IsActive=false) projeler ve geliştiriciler listelerde görünmemeli.

---

## 🚀 ADIM ADIM TEST

### 📝 ADIM 1: İki Kullanıcı Oluştur

#### Kullanıcı 1 (Proje Sahibi):
1. Tarayıcıda `/Account/Register` sayfasına git
2. Formu doldur:
   - **Ad Soyad:** `Ali Proje`
   - **Email:** `ali@test.com`
   - **Şifre:** `123456`
   - **Şifre Tekrar:** `123456`
3. "Hesap Oluştur" butonuna tıkla
4. Login sayfasına yönlendirileceksin (`/Account/Login`)
5. Giriş yap: `ali@test.com` / `123456`

#### Kullanıcı 2 (Projeye Katılacak):
1. Farklı bir tarayıcı penceresi veya gizli mod aç (ya da çıkış yap)
2. `/Account/Register` sayfasına git
3. Formu doldur:
   - **Ad Soyad:** `Ayşe Developer`
   - **Email:** `ayse@test.com`
   - **Şifre:** `123456`
   - **Şifre Tekrar:** `123456`
4. "Hesap Oluştur" butonuna tıkla
5. Giriş yap: `ayse@test.com` / `123456`

---

### 📝 ADIM 2: Developer2'ye Skill Ekleme (MANUEL)

**Neden:** Developer'lara skill eklemek için henüz bir sayfa yok. Bu yüzden veritabanına manuel ekliyoruz.

#### Terminal'de şu komutları çalıştır:

```bash
cd /Users/ahmet/Desktop/proje/DevTeamFinder
sqlite3 devteamfinder.db
```

#### SQLite'da şu komutları çalıştır:

**1. Ayşe'nin Developer ID'sini bul:**
```sql
SELECT d.Id, d.AdSoyad, u.Email 
FROM Developers d 
JOIN Users u ON d.UserId = u.Id 
WHERE u.Email = 'ayse@test.com';
```

**Çıktı örneği:** `Id = 2` (not al, sonra kullanacaksın)

**2. Ayşe'ye skill ekle (C# ve ASP.NET Core):**
```sql
-- C# skill'i ekle (SkillId = 1)
INSERT INTO DeveloperSkills (DeveloperId, SkillId) 
SELECT d.Id, 1
FROM Developers d 
JOIN Users u ON d.UserId = u.Id 
WHERE u.Email = 'ayse@test.com';

-- ASP.NET Core skill'i ekle (SkillId = 2)
INSERT INTO DeveloperSkills (DeveloperId, SkillId) 
SELECT d.Id, 2
FROM Developers d 
JOIN Users u ON d.UserId = u.Id 
WHERE u.Email = 'ayse@test.com';
```

**3. Kontrol et:**
```sql
SELECT d.AdSoyad, s.Ad 
FROM Developers d
JOIN DeveloperSkills ds ON d.Id = ds.DeveloperId
JOIN Skills s ON ds.SkillId = s.Id
WHERE d.Id = (SELECT Id FROM Developers WHERE UserId = (SELECT Id FROM Users WHERE Email = 'ayse@test.com'));
```

**Çıkış:** `.exit` yazıp Enter'a bas.

---

### 📝 ADIM 3: Proje Oluştur (Ali ile)

1. **Ali ile giriş yap:** `ali@test.com` / `123456`
2. Navbar'dan **"Projelerim"** linkine tıkla
3. **"Yeni Proje Oluştur"** butonuna tıkla
4. Formu doldur:
   - **Proje Başlığı:** `Test Projesi`
   - **Açıklama:** `Bu bir test projesidir`
   - **Skill seç:** `C#`, `ASP.NET Core`, `SQL` (3 skill seç)
5. **"Projeyi Oluştur"** butonuna tıkla
6. Proje listesine yönlendirileceksin

---

### 📝 ADIM 4: Projeye Katılım İsteği (Ayşe ile)

1. **Ayşe ile giriş yap:** `ayse@test.com` / `123456`
2. Navbar'dan **"Proje Havuzu"** linkine tıkla
3. **"Test Projesi"** kartına tıkla (veya "Projeyi İncele" butonu)
4. Proje detay sayfasında **"Projeye Katılmak İstiyorum"** butonuna tıkla
5. Sayfa yenilenecek ve **"Katılım isteğiniz bekleniyor"** mesajı görünecek

---

### 📝 ADIM 5: Daveti Kabul Et (Ayşe ile)

1. Navbar'dan **"Davetlerim"** linkine tıkla
2. "Beklemede" bölümünde "Test Projesi" davetini görürsün
3. **"Kabul Et"** butonuna tıkla
4. Davet "Kabul Edilen" bölümüne taşınacak

---

### 📝 ADIM 6: Proje Ekibini Kontrol Et (Ali ile) ✅ ADIM 11.1 TEST

1. **Ali ile giriş yap:** `ali@test.com` / `123456`
2. **"Projelerim"** → "Test Projesi"ne tıkla
3. Proje detay sayfasında aşağı kaydır
4. **"Proje Ekibi"** bölümünü bul
5. **Görünmesi gerekenler:**
   - ✅ Ali Proje (Proje Sahibi)
   - ✅ Ayşe Developer (Projeye katılan)

**SONUÇ:** ✅ ADIM 11.1 BAŞARILI - Projeye katılan geliştiriciler listeleniyor

---

### 📝 ADIM 7: Uyum Skorunu Kontrol Et (Ali ile) ✅ ADIM 11.2 TEST

1. Proje detay sayfasında "Proje Ekibi" bölümüne bak
2. **Ayşe Developer**'ın yanında **"%66 Uyum"** yazısı görünmeli

**Neden %66?**
- Proje Skill'leri: C#, ASP.NET Core, SQL (3 skill)
- Ayşe'nin Skill'leri: C#, ASP.NET Core (2 skill)
- Ortak: C#, ASP.NET Core (2 skill)
- Uyum = (2 / 3) * 100 = %66

**SONUÇ:** ✅ ADIM 11.2 BAŞARILI - Uyum skoru hesaplanıyor ve gösteriliyor

---

### 📝 ADIM 8: Önerilen Geliştiriciler (Ali ile) ✅ ADIM 11.3 TEST

1. Proje detay sayfasında aşağı kaydır
2. **"Önerilen Geliştiriciler"** bölümünü bul
3. Bu bölüm sadece proje sahibi (Ali) için görünür

**NOT:** Eğer Ayşe'ye zaten davet gönderilmediyse ve uyumu >= %50 ise, Ayşe burada listelenebilir.

**SONUÇ:** ✅ ADIM 11.3 BAŞARILI - Önerilen geliştiriciler listesi görünüyor

---

### 📝 ADIM 9: Soft Delete Test (Ali ile) ✅ ADIM 11.4 TEST

#### Test 1: Projeyi Soft Delete Yap

1. **Terminal'de:**
```bash
cd /Users/ahmet/Desktop/proje/DevTeamFinder
sqlite3 devteamfinder.db
```

2. **SQLite'da:**
```sql
UPDATE Projects 
SET IsActive = 0 
WHERE Baslik = 'Test Projesi';
```

3. **Kontrol et:**
```sql
SELECT Baslik, IsActive FROM Projects;
```

4. **Çıkış:** `.exit`

5. **Tarayıcıda (Ali ile giriş yap):**
   - **"Projelerim"** sayfasına git
   - **"Test Projesi"** listelenmemeli ✅

6. **"Proje Havuzu"** sayfasına git
   - **"Test Projesi"** listelenmemeli ✅

**SONUÇ:** ✅ ADIM 11.4 BAŞARILI - Soft delete çalışıyor, proje görünmüyor

#### Test 2: Projeyi Geri Getir (IsActive = 1)

**SQLite'da:**
```sql
UPDATE Projects 
SET IsActive = 1 
WHERE Baslik = 'Test Projesi';
```

**Tarayıcıda:**
- Proje tekrar görünmeli ✅

---

## 🎯 ÖZET - Ne Test Edildi?

| Adım | Özellik | Test Durumu |
|------|---------|-------------|
| 11.1 | Proje Ekibi Listesi | ✅ Görünüyor |
| 11.2 | Uyum Skoru (%66) | ✅ Hesaplanıyor |
| 11.3 | Önerilen Geliştiriciler | ✅ Listeleniyor |
| 11.4 | Soft Delete | ✅ Çalışıyor |

---

## 💡 SORUN ÇÖZME

### Developer Skill'leri görünmüyor?
- SQLite komutlarını tekrar kontrol et
- `DeveloperSkills` tablosunda kayıt var mı kontrol et:
```sql
SELECT * FROM DeveloperSkills;
```

### Uyum skoru yanlış?
- Proje skill'lerini kontrol et (3 olmalı)
- Developer skill'lerini kontrol et (2 olmalı)
- Hesaplama: (2/3) * 100 = %66

### Proje görünmüyor?
- `IsActive = 1` olduğundan emin ol
- SQLite'da kontrol et: `SELECT Baslik, IsActive FROM Projects;`

---

## 📌 ÖNEMLİ NOTLAR

1. **Skill Ekleme:** Şu an UI yok, SQLite ile manuel ekleniyor
2. **Soft Delete:** Hard delete değil, sadece görünmezlik
3. **Uyum Skoru:** Proje skill sayısına bölünüyor, 0/0 = 0 olabilir
