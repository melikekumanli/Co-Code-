# UYUM PUANI TEST KILAVUZU

## 🎯 Uyum Puanı Nasıl Çalışır?

**Formül:**
```
Uyum Puanı = (Ortak Skill Sayısı / Proje Skill Sayısı) × 100
```

**Örnek:**
- Proje skill'leri: React, Node.js, MongoDB (3 skill)
- Developer skill'leri: React, Node.js, Python, Docker (4 skill)
- Ortak skill'ler: React, Node.js (2 skill)
- **Uyum Puanı: (2 / 3) × 100 = %66**

---

## 📝 TEST SENARYOSU

### ADIM 1: İki Kullanıcı Oluştur

**Kullanıcı 1 (Proje Sahibi):**
- Email: proje@test.com
- Şifre: Test123!
- Ad Soyad: Proje Sahibi
- Deneyim: Senior

**Kullanıcı 2 (Developer):**
- Email: dev@test.com
- Şifre: Test123!
- Ad Soyad: Test Developer
- Deneyim: Mid-Level

---

### ADIM 2: Developer Skill'leri Ekle

**Developer 1 (proje@test.com) - Profil Sayfasında:**
- C#
- ASP.NET Core
- React
- SQL
- Docker

**Developer 2 (dev@test.com) - Profil Sayfasında:**
- C#
- ASP.NET Core
- React
- JavaScript
- Node.js
- MongoDB

---

### ADIM 3: Proje Oluştur (proje@test.com ile)

**Proje 1: "E-Ticaret Platformu"**
- Açıklama: Modern e-ticaret sitesi
- Skill'ler: C#, ASP.NET Core, React, SQL
- Durum: Aktif

**Beklenen Uyum Puanı:**
- Developer 2 için: (4/4) × 100 = **%100** ✅
  - Ortak: C#, ASP.NET Core, React, SQL (hepsi var ama SQL yok, 3/4 = %75)

---

### ADIM 4: Uyum Puanını Gör

**Yöntem 1: Proje Detay Sayfası**
1. proje@test.com ile giriş yap
2. "Projelerim" → "E-Ticaret Platformu" → Detay
3. Sağ tarafta "Geliştiricilere Davet Gönder" bölümünde
4. Developer 2'nin yanında **"%75 Uyum"** yazmalı

**Yöntem 2: Önerilen Geliştiriciler**
1. Aynı proje detay sayfasında
2. "Önerilen Geliştiriciler" bölümünde
3. Uyum puanı %50+ olanlar gösterilir
4. Developer 2 burada görünmeli

---

## 🧪 FARKLI SENARYOLAR

### Senaryo 1: %100 Uyum
**Proje Skill'leri:** React, Node.js
**Developer Skill'leri:** React, Node.js, Python, Docker
**Sonuç:** 2/2 = **%100**

### Senaryo 2: %50 Uyum
**Proje Skill'leri:** React, Node.js, MongoDB, Docker
**Developer Skill'leri:** React, Node.js, Python, Java
**Sonuç:** 2/4 = **%50**

### Senaryo 3: %0 Uyum
**Proje Skill'leri:** Java, Spring Boot
**Developer Skill'leri:** React, Node.js
**Sonuç:** 0/2 = **%0** (Önerilen geliştiricilerde görünmez)

### Senaryo 4: %66 Uyum
**Proje Skill'leri:** C#, ASP.NET Core, SQL
**Developer Skill'leri:** C#, ASP.NET Core, React, Docker
**Sonuç:** 2/3 = **%66**

---

## 🎨 GÖRSEL GÖSTERIMLER

### Proje Detay Sayfasında:
```
┌─────────────────────────────────────┐
│ Geliştiricilere Davet Gönder       │
├─────────────────────────────────────┤
│ Test Developer                      │
│ Mid-Level                           │
│ %75 Uyum                           │
│ [Davet Gönder]                     │
└─────────────────────────────────────┘
```

### Önerilen Geliştiriciler:
```
┌─────────────────────────────────────┐
│ Önerilen Geliştiriciler             │
│ (Projenize en uygun geliştiriciler) │
├─────────────────────────────────────┤
│ Test Developer                      │
│ Mid-Level                           │
│ %75 Uyum                           │
│ [Projeme Katıl]                    │
└─────────────────────────────────────┘
```

---

## 🔍 NEREDE GÖRÜNÜR?

1. **Proje Detay Sayfası (Proje Sahibi Görünümü)**
   - Sağ tarafta "Geliştiricilere Davet Gönder" bölümü
   - Her developer'ın yanında uyum puanı

2. **Önerilen Geliştiriciler Bölümü**
   - Uyum puanı %50+ olanlar
   - En yüksek uyumdan düşüğe sıralı
   - Maksimum 5 öneri

3. **Proje Ekibi Bölümü**
   - Projeye katılan developer'lar
   - Uyum puanları gösterilir

---

## 💡 İPUÇLARI

1. **Daha Fazla Skill Ekle:**
   - Developer profilinde daha fazla skill ekle
   - Uyum puanı artar

2. **Farklı Projeler Oluştur:**
   - Farklı skill kombinasyonları dene
   - Hangi developer'ların önerildiğini gör

3. **%50 Eşiği:**
   - Önerilen geliştiriciler bölümünde sadece %50+ uyum gösterilir
   - Bu eşik değiştirilebilir (kod içinde)

4. **Sıralama:**
   - Önerilen geliştiriciler en yüksek uyumdan düşüğe sıralı
   - İlk 5 gösterilir

---

## 🐛 SORUN GİDERME

**Uyum puanı görünmüyor:**
- Developer'ın skill'leri var mı kontrol et
- Projenin skill'leri var mı kontrol et
- Proje aktif mi kontrol et

**Önerilen geliştiriciler boş:**
- Hiçbir developer %50+ uyum sağlamıyor olabilir
- Daha fazla developer ekle veya skill'leri güncelle

**Yanlış uyum puanı:**
- Developer skill'lerini kontrol et
- Proje skill'lerini kontrol et
- Tarayıcıyı yenile (cache sorunu olabilir)

---

## ✅ BAŞARILI TEST KRİTERLERİ

- [ ] İki kullanıcı oluşturuldu
- [ ] Her kullanıcıya skill'ler eklendi
- [ ] Proje oluşturuldu ve skill'ler seçildi
- [ ] Proje detay sayfasında uyum puanı görünüyor
- [ ] Önerilen geliştiriciler bölümünde %50+ olanlar görünüyor
- [ ] Uyum puanı doğru hesaplanıyor
- [ ] Proje ekibinde uyum puanı görünüyor

---

## 🎉 SONUÇ

Uyum puanı sistemi çalışıyor! Artık proje sahipleri en uygun developer'ları kolayca bulabilir.
