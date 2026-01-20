# Co-Code – Final Projesi

## Proje Tanımı
Co-Code, yazılım geliştiriciler ile proje sahiplerini teknik yetkinliklere göre eşleştiren ASP.NET Core MVC tabanlı bir web uygulamasıdır. Sistem, proje gereksinimleri ile geliştiricilerin sahip olduğu becerileri karşılaştırarak objektif bir uyum puanı hesaplar ve en uygun eşleşmeleri sunar.

## Projenin Amacı
Bu projenin amacı, MVC mimarisini doğru şekilde uygulayarak veritabanı destekli, gerçek hayata yönelik ve işlevsel bir web uygulaması geliştirmektir. Proje kapsamında yazılım mimarisi, veri yönetimi ve kullanıcı etkileşimi bütüncül olarak ele alınmıştır.

## Kullanılan Teknolojiler
- ASP.NET Core MVC
- .NET 9.0
- C#
- Entity Framework Core
- SQLite
- HTML / CSS / Bootstrap

## Mimari Yapı
Uygulama Model–View–Controller (MVC) mimarisi kullanılarak geliştirilmiştir.
- Model katmanı: Veritabanı tabloları ve iş kuralları
- View katmanı: Kullanıcı arayüzleri
- Controller katmanı: İş mantığı ve veri akışı

## Veritabanı Yönetimi
Veritabanı olarak SQLite kullanılmıştır. Veritabanı işlemleri Entity Framework Core aracılığıyla gerçekleştirilmiş ve Code First yaklaşımı uygulanmıştır.

## Uyum Puanı Hesaplama
Proje gereksinimleri ile geliştirici yetkinlikleri karşılaştırılarak aşağıdaki formül ile uyum puanı hesaplanmaktadır:

Uyum Puanı = (Ortak Skill Sayısı / Gerekli Skill Sayısı) × 100

Bu yapı sayesinde yanlış eşleşmeler azaltılmıştır.

## Sistem İşleyişi
- Kullanıcılar sisteme kayıt olur
- Geliştiriciler teknik yetkinliklerini tanımlar
- Proje sahipleri proje gereksinimlerini belirler
- Sistem otomatik olarak uyum puanlarını hesaplar
- En uygun geliştiriciler listelenir

## Güvenlik ve Gizlilik
Kullanıcı iletişim bilgileri doğrudan paylaşılmamaktadır. Sistem, kontrollü iletişim mantığına sahiptir.

## Proje Tanıtım Videosu
https://www.youtube.com/watch?v=1Lcz0bft4r8

## Geliştirici Bilgileri
Melike Kumanlı 

Uludağ Üniversitesi  

Yönetim Bilişim Sistemleri



