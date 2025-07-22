# Deploy 🐂

```bash
sudo apt-get update
sudo apt install -y postgresql
sudo apt-get install -y dotnet-sdk-9.0
dotnet tool install --global dotnet-ef
psql -h postgres -p 5432 -U postgres -c "CREATE DATABASE bull-chat;"
cd .\bull-chat-backend
dotnet ef database update
```

# Создать миграцию 🐂
dotnet ef migrations add `Name`
