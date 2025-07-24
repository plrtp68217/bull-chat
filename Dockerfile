# Frontend build
FROM node:20-alpine AS frontend-builder
WORKDIR /frontend
COPY bull-chat-frontend/package*.json ./
RUN npm install
COPY bull-chat-frontend/ .
RUN npm run build

# Backend build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-builder
WORKDIR /bull-chat-backend
COPY bull-chat-backend/ .
#RUN dotnet restore bull-chat-backend.csproj
RUN dotnet publish bull-chat-backend.csproj -c Release -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=backend-builder /app .
COPY --from=frontend-builder /frontend/dist ./wwwroot

# Добавим скрипт запуска
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh
ENTRYPOINT ["./entrypoint.sh"]

