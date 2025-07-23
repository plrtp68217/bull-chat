FROM node:20-alpine AS frontend-builder
WORKDIR /frontend
COPY bull-chat-frontend/package*.json ./
RUN npm install
COPY bull-chat-frontend/ .
RUN npm run build  # Результат в /frontend/dist

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-builder
WORKDIR /bull-chat-backend
COPY bull-chat-backend/ .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=backend-builder /app .

# Копируем собранный фронтенд в wwwroot
COPY --from=frontend-builder /frontend/dist ./wwwroot

CMD ["dotnet", "bull-chat-backend.dll" "--urls=http://localhost:80/"]