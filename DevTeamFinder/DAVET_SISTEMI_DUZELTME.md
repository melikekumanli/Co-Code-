# Davet Sistemi Düzeltme Raporu

## Sorun

Zeynep "deneme" adında bir proje oluşturdu ve Beyza katılma isteği gönderdi. Ancak bu davet hem Zeynep'in hem de Beyza'nın "Davetlerim" sayfasında görünüyordu.

## Kök Neden

`Invitation` modelinde "kim gönderdi" bilgisi yoktu. Bu yüzden:
- **Proje sahibinin developer'a gönderdiği davet** ile
- **Developer'ın proje sahibine gönderdiği katılma isteği**

arasında ayrım yapılamıyordu.

Her iki durumda da `DeveloperId` farklı anlamda kullanılıyordu:
- **Create (Davet)**: DeveloperId = davet edilen kişi
- **RequestToJoin (İstek)**: DeveloperId = isteği gönderen kişi

Bu karışıklık yüzünden filtreleme mantığı yanlış çalışıyordu.

## Çözüm

### 1. Model Değişikliği

`Invitation` modeline `SenderId` alanı eklendi:

```csharp
public class Invitation
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int DeveloperId { get; set; }
    public int SenderId { get; set; } // YENİ: Daveti/İsteği gönderen kişinin DeveloperId'si
    public string Durum { get; set; } = "Beklemede";
    public string? Not { get; set; }

    // Navigation Properties
    public Project Project { get; set; }
    public Developer Developer { get; set; }
    public Developer Sender { get; set; } // YENİ: Gönderen developer
}
```

### 2. Davet Türleri Netleştirildi

Artık iki farklı senaryo net bir şekilde ayrılıyor:

#### A. Proje Sahibi Developer'a Davet Gönderir
```csharp
var invitation = new Invitation
{
    ProjectId = projectId,
    DeveloperId = developerId,      // Davet edilen developer
    SenderId = currentDeveloper.Id, // Daveti gönderen (proje sahibi)
    Durum = "Beklemede"
};
```

#### B. Developer Proje Sahibine Katılma İsteği Gönderir
```csharp
var invitation = new Invitation
{
    ProjectId = projectId,
    DeveloperId = currentDeveloper.Id, // İstek gönderen developer
    SenderId = currentDeveloper.Id,    // İsteği gönderen (kendisi)
    Durum = "Beklemede"
};
```

### 3. Filtreleme Mantığı Düzeltildi

`InvitationsController.Index` metodunda üç farklı liste oluşturuluyor:

#### A. Bana Gönderilen Davetler
```csharp
var receivedInvitations = myInvitations
    .Where(i => i.DeveloperId == developer.Id && i.SenderId != developer.Id)
    .ToList();
```
- `DeveloperId == benim`: Ben davet edildim
- `SenderId != benim`: Başkası gönderdi (proje sahibi)

#### B. Benim Gönderdiğim Katılma İstekleri
```csharp
var sentRequests = myInvitations
    .Where(i => i.DeveloperId == developer.Id && i.SenderId == developer.Id)
    .ToList();
```
- `DeveloperId == benim`: Ben istek gönderdim
- `SenderId == benim`: Ben gönderdim

#### C. Projelerime Gelen Katılma İstekleri
```csharp
var projectRequests = myInvitations
    .Where(i => i.Project.DeveloperId == developer.Id 
             && i.SenderId != developer.Id 
             && i.DeveloperId != developer.Id)
    .ToList();
```
- `Project.DeveloperId == benim`: Benim projem
- `SenderId != benim`: Başkası gönderdi
- `DeveloperId != benim`: Başkası katılmak istiyor

### 4. View Güncellemeleri

"Davetlerim" sayfasına yeni bir sekme eklendi:

1. **Bekleyen Davetler**: Bana gönderilen davetler (proje sahipleri tarafından)
2. **Kabul Edilenler**: Kabul ettiğim davetler
3. **Reddedilenler**: Reddettiğim davetler
4. **Gönderdiğim İstekler**: Benim gönderdiğim katılma istekleri (YENİ)
5. **Projeme Gelen İstekler**: Projelerime gelen katılma istekleri (YENİ - ayrı sekme)

### 5. Migration

Yeni migration dosyası oluşturuldu: `20260118000000_AddInvitationSender.cs`

Bu migration:
- `SenderId` kolonunu ekler
- Mevcut kayıtlar için `SenderId`'yi proje sahibinin ID'si olarak ayarlar (varsayılan)
- Foreign key ilişkilerini günceller (Cascade yerine Restrict)

## Test Senaryosu

### Senaryo 1: Proje Sahibi Davet Gönderir

1. **Zeynep** "deneme" projesini oluşturur
2. **Zeynep** proje detay sayfasından **Beyza**'ya davet gönderir
3. **Beklenen Sonuç:**
   - Zeynep'in "Davetlerim" sayfasında bu davet GÖRÜNMEZ
   - Beyza'nın "Davetlerim" → "Bekleyen Davetler" sekmesinde bu davet GÖRÜNÜR
   - Beyza daveti kabul edebilir veya reddedebilir

### Senaryo 2: Developer Katılma İsteği Gönderir

1. **Beyza** "Proje Havuzu"ndan Zeynep'in projesini bulur
2. **Beyza** "Katılma İsteği Gönder" butonuna tıklar
3. **Beklenen Sonuç:**
   - Beyza'nın "Davetlerim" → "Gönderdiğim İstekler" sekmesinde bu istek GÖRÜNÜR
   - Zeynep'in "Davetlerim" → "Projeme Gelen İstekler" sekmesinde bu istek GÖRÜNÜR
   - Zeynep isteği kabul edebilir veya reddedebilir

## Önemli Notlar

1. **Program.cs** dosyasında `EnsureCreated()` yerine `Migrate()` kullanılıyor. Bu sayede migration'lar otomatik olarak uygulanır.

2. Mevcut veritabanındaki kayıtlar için `SenderId` otomatik olarak proje sahibinin ID'si olarak ayarlanır. Eğer mevcut kayıtlar "katılma isteği" ise, bunları manuel olarak düzeltmeniz gerekebilir.

3. Yeni eklenen `Sender` navigation property sayesinde davet/istek gönderen kişinin bilgilerine kolayca erişilebilir.

## Sonraki Adımlar

1. Uygulamayı çalıştırın: `dotnet run`
2. Migration otomatik olarak uygulanacak
3. Test senaryolarını uygulayın
4. Eğer mevcut veritabanında yanlış kayıtlar varsa, veritabanını sıfırlayın:
   ```bash
   rm devteamfinder.db
   dotnet run
   ```

## Değişiklik Özeti

- ✅ `Invitation` modeline `SenderId` ve `Sender` eklendi
- ✅ `AppDbContext` güncellendi (foreign key ilişkileri)
- ✅ Migration oluşturuldu
- ✅ `InvitationsController.Create` güncellendi
- ✅ `InvitationsController.RequestToJoin` güncellendi
- ✅ `InvitationsController.Index` filtreleme mantığı düzeltildi
- ✅ `Views/Invitations/Index.cshtml` güncellendi (yeni sekmeler eklendi)
- ✅ `Program.cs` güncellendi (EnsureCreated → Migrate)
