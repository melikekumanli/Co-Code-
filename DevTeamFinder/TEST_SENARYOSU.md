# ADIM 11.1-11.4 Test Senaryosu

## 📋 Test Öncesi Hazırlık

### 1️⃣ İki Kullanıcı Oluştur

**Developer 1 (Proje Sahibi):**
- Email: `proje1@test.com`
- Şifre: `test123`
- Ad Soyad: `Ali Proje Sahibi`

**Developer 2 (Projeye Katılacak):**
- Email: `developer2@test.com`
- Şifre: `test123`
- Ad Soyad: `Ayşe Developer`

### 2️⃣ Developer2'ye Skill Ekleme (MANUEL - Veritabanı)

Developer'lara skill eklemek için UI henüz yok. Bu yüzden veritabanına manuel ekleme yapmalısınız:

**SQLite veritabanını aç:**
```bash
sqlite3 devteamfinder.db
```

**Mevcut Skill ID'lerini kontrol et:**
```sql
SELECT * FROM Skills;
```

**Developer2'nin ID'sini bul:**
```sql
SELECT d.Id, d.AdSoyad, u.Email 
FROM Developers d 
JOIN Users u ON d.UserId = u.Id 
WHERE u.Email = 'developer2@test.com';
```

**Developer2'ye skill ekle (örnek):**
```sql
-- Developer2'ye C# (Id=1) ve ASP.NET Core (Id=2) skill'lerini ekle
INSERT INTO DeveloperSkills (DeveloperId, SkillId) 
VALUES (
  (SELECT Id FROM Developers WHERE UserId = (SELECT Id FROM Users WHERE Email = 'developer2@test.com')),
  1
);

INSERT INTO DeveloperSkills (DeveloperId, SkillId) 
VALUES (
  (SELECT Id FROM Developers WHERE UserId = (SELECT Id FROM Users WHERE Email = 'developer2@test.com')),
  2
);
```

**Kontrol et:**
```sql
SELECT ds.*, d.AdSoyad, s.Ad 
FROM DeveloperSkills ds
JOIN Developers d ON ds.DeveloperId = d.Id
JOIN Skills s ON ds.SkillId = s.Id
WHERE d.Id = (SELECT Id FROM Developers WHERE UserId = (SELECT Id FROM Users WHERE Email = 'developer2@test.com'));
```

---

## 🧪 Test Adımları

### ✅ ADIM 11.1: Projeye Katılan Geliştiriciler Listesi

1. **Developer1 ile giriş yap** (`proje1@test.com` / `test123`)

2. **Yeni Proje Oluştur:**
   - "Projelerim" → "Yeni Proje Oluştur"
   - Başlık: `Test Projesi`
   - Açıklama: `Bu bir test projesidir`
   - **Skill seç:** `C#`, `ASP.NET Core`, `SQL` (3 skill seç - uyum skorunu test etmek için)

3. **Developer2'ye davet gönder veya Developer2 projeye katılmak istesin:**
   - Developer1: Proje detay sayfasında Developer2'ye "Davet Gönder" butonu
   - VEYA Developer2 giriş yapıp projeye "Projeye Katılmak İstiyorum" butonu

4. **Developer2 ile giriş yap** (`developer2@test.com` / `test123`)

5. **Daveti kabul et:**
   - "Davetlerim" sayfasına git
   - Beklemede olan daveti bul
   - "Kabul Et" butonuna tıkla

6. **Developer1 ile tekrar giriş yap ve Proje Detay sayfasına git:**
   - "Projelerim" → Projeye tıkla
   - **"Proje Ekibi" bölümünde Developer2 görünmeli** ✅ (ADIM 11.1)

---

### ✅ ADIM 11.2: Skill Uyum Skoru Hesaplama

**Uyum Skoru Hesabı:**
- Proje Skill'leri: `C#`, `ASP.NET Core`, `SQL` (3 skill)
- Developer2 Skill'leri: `C#`, `ASP.NET Core` (2 skill)
- **Uyum = (2 / 3) * 100 = %66** ✅

**Test:**
1. Developer1 ile proje detay sayfasına git
2. "Proje Ekibi" bölümünde Developer2'nin yanında **"%66 Uyum"** görünmeli ✅ (ADIM 11.2)

---

### ✅ ADIM 11.3: Önerilen Geliştiriciler Listesi

**Önerilen Geliştiriciler Kriterleri:**
- Bu projeye daveti olmayan
- IsActive == true
- Skill uyumu >= %50

**Test için:**
1. Developer1 ile proje detay sayfasına git
2. **"Önerilen Geliştiriciler" bölümü** görünmeli ✅
3. Developer2'ye davet gönderilmediyse ve uyumu >= %50 ise Developer2 listelenmeli ✅ (ADIM 11.3)

**Ekstra Test:**
- Developer3 oluştur (uyum < %50 olan skill'lerle)
- Developer3 listelenmemeli ✅

---

### ✅ ADIM 11.4: Soft Delete (IsActive)

**Test Senaryosu 1: Developer Soft Delete**

1. Developer2'yi soft delete yap (IsActive = false):
   ```sql
   UPDATE Developers 
   SET IsActive = 0 
   WHERE UserId = (SELECT Id FROM Users WHERE Email = 'developer2@test.com');
   ```

2. Developer1 ile "Proje Havuzu" sayfasına git:
   - Developer2 artık önerilen geliştiriciler listesinde görünmemeli ✅

3. Developer2 ile giriş yapmayı dene:
   - Developer2 hala giriş yapabilir (soft delete, login'i engellemez)
   - Ancak proje listelerinde görünmemeli ✅

**Test Senaryosu 2: Project Soft Delete**

1. Developer1 ile giriş yap

2. Projeyi soft delete yap:
   ```sql
   UPDATE Projects 
   SET IsActive = 0 
   WHERE Baslik = 'Test Projesi';
   ```

3. "Projelerim" sayfasına git:
   - Proje artık listelenmemeli ✅

4. "Proje Havuzu" sayfasına git:
   - Proje artık listelenmemeli ✅

5. Proje detay sayfasına direkt URL ile git:
   - 404 veya hata alınmalı ✅ (ADIM 11.4)

---

## 🎯 Beklenen Sonuçlar

### ADIM 11.1 ✅
- Proje detay sayfasında "Proje Ekibi" bölümü görünür
- Proje sahibi listelenir
- Kabul edilen davetlerle katılan geliştiriciler listelenir

### ADIM 11.2 ✅
- Her geliştirici için uyum skoru hesaplanır
- Format: `%66 Uyum` şeklinde gösterilir
- Skor: `(Ortak Skill Sayısı / Proje Skill Sayısı) * 100`

### ADIM 11.3 ✅
- "Önerilen Geliştiriciler" bölümü görünür (proje sahibi için)
- Sadece uyum >= %50 olan geliştiriciler listelenir
- Daveti olmayan geliştiriciler listelenir
- IsActive == false olanlar listelenmez

### ADIM 11.4 ✅
- IsActive == false olan projeler listelerde görünmez
- IsActive == false olan geliştiriciler listelerde görünmez
- Soft delete = hard delete değil, sadece görünmezlik
- Veritabanında kayıt hala var

---

## 💡 Hızlı SQL Kontrolleri

```sql
-- Developer Skill'lerini kontrol et
SELECT d.AdSoyad, s.Ad 
FROM Developers d
JOIN DeveloperSkills ds ON d.Id = ds.DeveloperId
JOIN Skills s ON ds.SkillId = s.Id
ORDER BY d.Id, s.Id;

-- Proje Skill'lerini kontrol et
SELECT p.Baslik, s.Ad 
FROM Projects p
JOIN ProjectSkills ps ON p.Id = ps.ProjectId
JOIN Skills s ON ps.SkillId = s.Id
ORDER BY p.Id, s.Id;

-- Invitation durumlarını kontrol et
SELECT 
  p.Baslik AS Proje,
  d.AdSoyad AS Developer,
  i.Durum
FROM Invitations i
JOIN Projects p ON i.ProjectId = p.Id
JOIN Developers d ON i.DeveloperId = d.Id;

-- IsActive durumlarını kontrol et
SELECT AdSoyad, IsActive FROM Developers;
SELECT Baslik, IsActive FROM Projects;
```

---

## ⚠️ Notlar

1. **Developer Skill Ekleme:** Şu an UI yok, manuel SQL ile ekleme yapılmalı
2. **Test Verileri:** Test sonrası veritabanını temizlemek isterseniz:
   ```sql
   DELETE FROM Invitations;
   DELETE FROM ProjectSkills;
   DELETE FROM DeveloperSkills;
   DELETE FROM Projects;
   DELETE FROM Developers;
   DELETE FROM Users;
   ```
3. **IsActive:** Soft delete için kullanılır, hard delete yapılmaz
