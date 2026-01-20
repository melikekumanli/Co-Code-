-- HIZLI TEST İÇİN ÖRNEK VERİLER
-- Bu SQL'i çalıştırarak hızlıca test edebilirsiniz

-- 1. Kullanıcılar oluştur
INSERT INTO Users (Email, PasswordHash, CreatedAt) VALUES 
('proje@test.com', '$2a$11$xQKvPZZxKZZxKZZxKZZxKOqKqKqKqKqKqKqKqKqKqKqKqKqKqKqKq', datetime('now')),
('dev1@test.com', '$2a$11$xQKvPZZxKZZxKZZxKZZxKOqKqKqKqKqKqKqKqKqKqKqKqKqKqKqKq', datetime('now')),
('dev2@test.com', '$2a$11$xQKvPZZxKZZxKZZxKZZxKOqKqKqKqKqKqKqKqKqKqKqKqKqKqKqKq', datetime('now'));

-- 2. Developer profilleri oluştur
INSERT INTO Developers (UserId, AdSoyad, Hakkinda, DeneyimSeviyesi, IsActive) VALUES 
(1, 'Proje Sahibi', 'Full Stack Developer', 'Senior', 1),
(2, 'Developer 1', 'Frontend Developer', 'Mid-Level', 1),
(3, 'Developer 2', 'Backend Developer', 'Junior', 1);

-- 3. Developer Skill'leri ekle
-- Proje Sahibi: C#, ASP.NET Core, React, SQL, Docker
INSERT INTO DeveloperSkills (DeveloperId, SkillId) VALUES 
(1, 1),  -- C#
(1, 2),  -- ASP.NET Core
(1, 13), -- React
(1, 28), -- SQL
(1, 33); -- Docker

-- Developer 1: C#, ASP.NET Core, React, JavaScript (4/5 ortak = %80 uyum)
INSERT INTO DeveloperSkills (DeveloperId, SkillId) VALUES 
(2, 1),  -- C#
(2, 2),  -- ASP.NET Core
(2, 13), -- React
(2, 11); -- JavaScript

-- Developer 2: C#, React, Node.js, MongoDB (2/5 ortak = %40 uyum)
INSERT INTO DeveloperSkills (DeveloperId, SkillId) VALUES 
(3, 1),  -- C#
(3, 13), -- React
(3, 16), -- Node.js
(3, 31); -- MongoDB

-- 4. Proje oluştur
INSERT INTO Projects (Baslik, Aciklama, Durum, DeveloperId, IsActive) VALUES 
('E-Ticaret Platformu', 'Modern bir e-ticaret sitesi geliştiriyoruz. ASP.NET Core backend ve React frontend kullanıyoruz.', 'Aktif', 1, 1);

-- 5. Proje Skill'leri ekle
INSERT INTO ProjectSkills (ProjectId, SkillId) VALUES 
(1, 1),  -- C#
(1, 2),  -- ASP.NET Core
(1, 13), -- React
(1, 28), -- SQL
(1, 33); -- Docker

-- BEKLENEN SONUÇLAR:
-- Developer 1: 4/5 ortak skill = %80 uyum ✅ (Önerilenlerde görünür)
-- Developer 2: 2/5 ortak skill = %40 uyum ❌ (Önerilenlerde görünmez, %50 altı)

-- TEST ADIMLARI:
-- 1. Bu SQL'i çalıştır: sqlite3 devteamfinder.db < HIZLI_TEST.sql
-- 2. Tarayıcıda http://localhost:5000 aç
-- 3. proje@test.com / Test123! ile giriş yap
-- 4. "Projelerim" → "E-Ticaret Platformu" → Detay
-- 5. Sağ tarafta "Geliştiricilere Davet Gönder" bölümünde uyum puanlarını gör
-- 6. "Önerilen Geliştiriciler" bölümünde sadece Developer 1'i gör (%80 uyum)
