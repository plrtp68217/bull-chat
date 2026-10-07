# Bull Chat 🐂

![.NET](https://img.shields.io/badge/ASP.NET_Core_9-512BD4?logo=dotnet&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-512BD4?logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?logo=postgresql&logoColor=white)
![Vue](https://img.shields.io/badge/Vue_3-4FC08D?logo=vuedotjs&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?logo=typescript&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)

Веб-чат с обменом сообщениями в реальном времени. Backend на ASP.NET Core с SignalR, frontend на Vue 3. Приложение собирается в один Docker-образ и разворачивается одной командой.

<!-- Cкриншот или GIF чата: ![Bull Chat](docs/screenshot.png) -->

## Возможности

- Регистрация, вход и выход, аутентификация по JWT
- Обмен сообщениями в реальном времени через WebSocket (SignalR) с проверкой токена при подключении к хабу
- Отправка изображений
- Подгрузка истории сообщений постранично
- Документация API в Swagger

## Стек

| Часть | Технологии |
|---|---|
| Backend | C#, ASP.NET Core 9, SignalR, Entity Framework Core, JWT |
| База данных | PostgreSQL (Npgsql), миграции EF Core |
| Frontend | Vue 3, TypeScript, Pinia, Vue Router, Axios, Naive UI, @microsoft/signalr, Vite |
| Тесты | xUnit, Moq, EF Core InMemory |
| Деплой | Docker (multi-stage), Docker Compose |

## Архитектура

```
bull-chat-backend/
├── Controllers/     # REST API: аутентификация, сообщения, загрузка изображений
├── Hubs/            # SignalR-хаб чата, фильтр аутентификации
├── Repository/      # Репозитории для работы с данными
├── Services/        # JWT, сессии, хеширование паролей
├── Models/          # Сущности БД и DTO
├── Migrations/      # Миграции EF Core
└── Extensions/      # Настройка Swagger, JWT, PostgreSQL
bull-chat-backend.tests/   # Unit-тесты репозиториев
bull-chat-frontend/
├── src/api/         # HTTP-клиент
├── src/hubs/        # Подключение к SignalR
├── src/stores/      # Pinia-хранилища
└── src/components/  # Экраны авторизации и чата
```

Docker-образ собирается в три этапа: сборка frontend на Node.js, сборка backend на .NET SDK, итоговый образ на ASP.NET Runtime, где backend раздаёт собранный frontend как статику. Миграции применяются отдельным сервисом в Docker Compose до запуска приложения.

### REST API

| Метод | Маршрут | Описание |
|---|---|---|
| `POST` | `/api/Authentication/register` | Регистрация |
| `POST` | `/api/Authentication/login` | Вход, выдача JWT |
| `POST` | `/api/Authentication/logout` | Выход |
| `POST` | `/api/Authentication/validate` | Проверка токена |
| `POST` | `/api/Message/next-message-page-index` | История сообщений (по странице) |
| `POST` | `/api/Message/next-message-page-date` | История сообщений (по дате) |
| `POST` | `/api/Message/media/upload-image` | Загрузка изображения |

## Запуск

Требуется Docker и Docker Compose.

```bash
git clone https://github.com/plrtp68217/bull-chat.git
cd bull-chat
docker compose up --build
```

Приложение будет доступно на `http://localhost:30000/бойня` (базовый путь задаётся аргументом `VITE_BASE_PATH` в `docker-compose.yml`).

### Локальная разработка

```bash
# Backend (требуется .NET SDK 9 и PostgreSQL)
dotnet tool install --global dotnet-ef
cd bull-chat-backend
dotnet ef database update
dotnet run

# Frontend
cd bull-chat-frontend
npm install
npm run dev
```

Создание новой миграции:

```bash
dotnet ef migrations add <Name>
```

Запуск тестов:

```bash
dotnet test bull-chat-backend.tests
```

## Команда

Командный проект:
- [plrtp68217](https://github.com/plrtp68217) — frontend, часть backend
- Pavel Dudin — backend
