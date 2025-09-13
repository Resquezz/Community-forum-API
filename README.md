# Community Forum API

Це навчальний **pet-проєкт** для вивчення ASP.NET Core, JWT-автентифікації, авторизації та роботи з SignalR.  
API надає базовий функціонал форуму: створення тем, постів, коментарів, голосувань, а також управління користувачами.

---

## Технології
- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **JWT Authentication & Authorization**
- **SignalR**
- **Serilog**
- **NUnit**

---

## Структура проєкту
- `CommunityForum.API` – Web API (контролери, налаштування).
- `CommunityForum.Application` – бізнес-логіка (сервіси, DTO).
- `CommunityForum.Domain` – сутності та інтерфейси.
- `CommunityForum.Infrastructure` – доступ до БД (репозиторії, EF Core).
- `CommunityForum.Tests` – модульні тести (NUnit).

---

## Environment variables
Перед запуском проекту необхідно встановити змінну середовища CONNECTION_STRING:

Windows:
setx CONNECTION_STRING "Server=localhost;Database=CommunityForum;User Id=YOUR_USER;Password=YOUR_PASSWORD;"

Linux/macOS:
export CONNECTION_STRING="Server=localhost;Database=CommunityForum;User Id=YOUR_USER;Password=YOUR_PASSWORD;"

Замість YOUR_USER та YOUR_PASSWORD використовуйте власні дані для доступу до бази.

У коді підключення до бази буде виглядати так:
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

---

## Запуск
1. Клонуйте репозиторій:

git clone https://github.com/YOUR_USERNAME/CommunityForum.git

2. Встановіть змінну середовища (як описано вище).

3. Відкрийте проект у Visual Studio або VS Code і запустіть:

dotnet run --project CommunityForum.API

4. API буде доступне на https://localhost:7074 або http://localhost:5074.

---

## Авторизація
- Реєстрація користувача → отримання JWT токена.
- Додавання токена в `Authorization: Bearer {token}` для доступу до захищених ендпоінтів.
- Ролі:
  - **User** – стандартний користувач.
  - **Admin**

---

## Тести
Юніт-тести знаходяться у окремому проєкті CommunityForum.Tests.

Використовуйте:
dotnet test

Модульні тести написані для:
- Сервісів (логіка створення постів, коментарів, голосувань).
- Перевірки обробки винятків та валідації даних.

---

## Logging

Використовується Serilog.

Логи зберігаються у файл logs/log.txt.

---

## Автор
**Dmytro Ilchenko**
