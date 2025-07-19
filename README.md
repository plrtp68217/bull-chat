# Deploy

```bash
sudo apt-get update
sudo apt install -y postgresql
sudo apt-get install -y dotnet-sdk-9.0
dotnet tool install --global dotnet-ef
psql -h postgres -p 5432 -U postgres -c "CREATE DATABASE bull-chat;"
cd .\bull-chat-backend
dotnet ef database update
```

# Создать миграцию
dotnet ef migrations add `Name`

### 1. Перенаправление пользователя
- **Описание**: Если у пользователя есть токен, необходимо автоматически перенаправить его на страницу `/chat`.
  - Проверить наличие токена при загрузке приложения.
  - Если токен существует, выполнить перенаправление на `/chat`.

### 2. Сохранение данных пользователя
- **Описание**: Сохранять идентификатор пользователя и его имя в `localStorage` для случая, когда есть токен и пользователь перенаправляется сразу в чат.
  - При успешной аутентификации пользователя получить его ID и имя.
  - Сохранить эти данные в `localStorage` с соответствующими ключами.
