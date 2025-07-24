# Frontend build
FROM node:20-alpine AS frontend-builder
WORKDIR /frontend
COPY bull-chat-frontend/package*.json ./
RUN npm install
COPY bull-chat-frontend/ .
RUN npm run build

# Backend build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-builder
WORKDIR /backend
COPY bull-chat-backend/ .
RUN dotnet restore -v diag && \
    dotnet build -c Release --no-restore -v diag && \
    dotnet publish -c Release -o /output \
        --no-build \
        --no-restore \
        -p:DebugType=None \
        -p:DebugSymbols=false \
        -p:UseAppHost=false -v diag

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=backend-builder /output .
COPY --from=frontend-builder /frontend/dist ./wwwroot

# Startup
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh
ENTRYPOINT ["./entrypoint.sh"]
