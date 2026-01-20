# DAVET NOTU VE GÖRÜNÜRLÜK TEST KILAVUZU

## ✅ YENİ ÖZELLİKLER

### 1. **Davet Notu Ekleme**
- Davet gönderirken mesaj yazabilme
- Katılma isteği gönderirken mesaj yazabilme
- Not opsiyonel (zorunlu değil)

### 2. **Davet Görünürlüğü**
- **Bana gönderilen davetler**: Sadece benim davetlerimde görünür
- **Projelerime gelen istekler**: Sadece proje sahibinin davetlerinde görünür

---

## 🧪 TEST SENARYOLARI

### SENARYO 1: Proje Sahibi Developer'a Davet Gönderir

**Adımlar:**
1. **6353** kullanıcısı ile giriş yap
2. Bir proje oluştur (örn: "E-Ticaret Sitesi")
3. Proje detay sayfasına git
4. Sağ tarafta "Geliştiricilere Davet Gönder" bölümünde **m** kullanıcısını bul
5. **"Davet Gönder"** butonuna tıkla
6. Modal açılır → Not yaz:
   ```
   Merhaba, projenize katılmanızı isteriz. 
   React konusunda deneyiminizden faydalanmak isteriz.
   ```
7. **"Davet Gönder"** butonuna tıkla

**Beklenen Sonuç:**
- ✅ Davet başarıyla gönderildi mesajı
- ✅ **m** kullanıcısı "Davetlerim" sayfasında daveti görür
- ✅ Davet kartında **not görünür**
- ❌ **6353** kullanıcısı "Davetlerim" sayfasında bu daveti GÖRMEZ

---

### SENARYO 2: Developer Projeye Katılma İsteği Gönderir

**Adımlar:**
1. **m** kullanıcısı ile giriş yap
2. **6353**'ün projesine git (Proje Havuzu veya Proje Detay)
3. **"Projeye Katılmak İstiyorum"** butonuna tıkla
4. Modal açılır → Not yaz:
   ```
   Merhaba, projenize katkıda bulunmak isterim. 
   React ve Node.js konusunda 3 yıl deneyimim var.
   ```
5. **"Katılma İsteği Gönder"** butonuna tıkla

**Beklenen Sonuç:**
- ✅ Katılma isteği gönderildi mesajı
- ✅ **6353** kullanıcısı "Davetlerim" → "Projelerime Gelen Katılma İstekleri" bölümünde görür
- ✅ İstek kartında **m'nin mesajı görünür**
- ❌ **m** kullanıcısı "Davetlerim" sayfasında bu isteği GÖRMEZ (kendi gönderdiği için)

---

### SENARYO 3: Proje Sahibi Katılma İsteğini Kabul Eder

**Adımlar:**
1. **6353** kullanıcısı ile giriş yap
2. **"Davetlerim"** sayfasına git
3. **"Projelerime Gelen Katılma İstekleri"** bölümünde **m**'nin isteğini gör
4. **m**'nin mesajını oku
5. **"Kabul Et"** butonuna tıkla

**Beklenen Sonuç:**
- ✅ Davet kabul edildi mesajı
- ✅ **m** "Davetlerim" → "Kabul Edilen" bölümünde görür
- ✅ **m** artık proje ekibinde
- ✅ Proje detay sayfasında "Proje Ekibi" bölümünde **m** görünür

---

### SENARYO 4: Not Yazmadan Davet Gönderme

**Adımlar:**
1. Davet gönderirken modal'da not alanını **boş bırak**
2. **"Davet Gönder"** butonuna tıkla

**Beklenen Sonuç:**
- ✅ Davet başarıyla gönderilir
- ✅ Davet kartında not bölümü görünmez
- ✅ Sistem normal çalışır

---

## 🔍 KONTROL LİSTESİ

### Davet Notu
- [ ] Proje sahibi davet gönderirken not yazabiliyor
- [ ] Developer katılma isteği gönderirken not yazabiliyor
- [ ] Not opsiyonel (boş bırakılabiliyor)
- [ ] Not davet kartında görünüyor
- [ ] Not 4 satır textarea ile yazılabiliyor

### Davet Görünürlüğü
- [ ] **m** projeye katılma isteği attığında:
  - ✅ **6353** "Projelerime Gelen Katılma İstekleri" bölümünde görüyor
  - ❌ **m** "Davetlerim" sayfasında GÖRM İYOR (kendi gönderdiği)
  
- [ ] **6353** developer'a davet attığında:
  - ✅ Developer "Davetlerim" sayfasında görüyor
  - ❌ **6353** "Davetlerim" sayfasında GÖRMÜYOR (kendi gönderdiği)

- [ ] **6353** katılma isteğini kabul ettiğinde:
  - ✅ **m** "Kabul Edilen" bölümünde görüyor
  - ✅ **6353** "Kabul Edilen" bölümünde görüyor

---

## 📊 GÖRSEL GÖSTERIM

### Davet Modal'ı:
```
┌─────────────────────────────────────┐
│ Developer Adı - Davet Gönder        │
├─────────────────────────────────────┤
│ Davet Mesajı (Opsiyonel)            │
│ ┌─────────────────────────────────┐ │
│ │ Merhaba, projenize katılmanızı │ │
│ │ isteriz. React konusunda...    │ │
│ │                                 │ │
│ │                                 │ │
│ └─────────────────────────────────┘ │
│ Örnek: Merhaba, projenize...        │
│                                     │
│ [İptal]  [Davet Gönder]            │
└─────────────────────────────────────┘
```

### Davet Kartı (Not ile):
```
┌─────────────────────────────────────┐
│ E-Ticaret Platformu                 │
│ Proje Sahibi: 6353                  │
│ Modern bir e-ticaret sitesi...      │
│ [C#] [React] [SQL]                  │
│                                     │
│ ┌─────────────────────────────────┐ │
│ │ Mesaj:                          │ │
│ │ Merhaba, projenize katılmanızı │ │
│ │ isteriz. React konusunda...    │ │
│ └─────────────────────────────────┘ │
│                                     │
│ [Kabul Et] [Reddet] [Proje Detayı] │
└─────────────────────────────────────┘
```

---

## 🐛 SORUN GİDERME

### Not görünmüyor
**Çözüm:**
- Tarayıcıyı yenile (Ctrl+F5)
- Veritabanını kontrol et:
```bash
sqlite3 devteamfinder.db "SELECT Id, Not FROM Invitations WHERE Not IS NOT NULL;"
```

### Davet yanlış yerde görünüyor
**Çözüm:**
- Hangi kullanıcı ile giriş yaptığını kontrol et
- "Davetlerim" ve "Projelerime Gelen Katılma İstekleri" bölümlerini kontrol et

### Modal açılmıyor
**Çözüm:**
- Bootstrap JS yüklenmiş mi kontrol et
- Tarayıcı konsolunda hata var mı kontrol et
- Sayfayı yenile

---

## ✅ BAŞARILI TEST

Eğer aşağıdakiler çalışıyorsa test başarılı:

1. ✅ Davet gönderirken not yazılabiliyor
2. ✅ Not davet kartında görünüyor
3. ✅ Not opsiyonel (boş bırakılabiliyor)
4. ✅ **m** katılma isteği attığında sadece **6353** görüyor
5. ✅ **6353** davet attığında sadece davet edilen görüyor
6. ✅ Kabul edilen davetler her iki tarafta da görünüyor

---

## 🎉 SONUÇ

Artık:
- ✅ Davet gönderirken mesaj yazılabiliyor
- ✅ Davet görünürlüğü doğru çalışıyor
- ✅ Sadece ilgili kişi daveti görüyor
- ✅ Proje sahibi katılma isteklerini onaylayabiliyor

**Keyifli testler! 🚀**
