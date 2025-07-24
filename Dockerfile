# Frontend build
FROM node:20-alpine AS frontend-builder
ARG VITE_BASE_PATH=/
ENV VITE_BASE_PATH=$VITE_BASE_PATH
WORKDIR /frontend
COPY bull-chat-frontend/package*.json ./
RUN npm install
COPY bull-chat-frontend/ .
RUN npm run build

# Backend build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-builder
WORKDIR /backend

COPY bull-chat-backend/*.csproj ./

RUN dotnet restore && \
    dotnet tool install --global dotnet-ef

# Копируем остальные исходники
COPY bull-chat-backend/ ./

# Собираем и публикуем приложение в одном шаге без лишнего вывода
RUN dotnet publish -c Release -o /output \
    --no-restore \
    -p:DebugType=None \
    -p:DebugSymbols=false \
    -p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=backend-builder /output .
COPY --from=frontend-builder /frontend/dist ./wwwroot

COPY entrypoint.sh .
RUN chmod +x entrypoint.sh
ENTRYPOINT ["./entrypoint.sh"]
