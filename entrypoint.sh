RUN dotnet restore bull-chat-backend.csproj
RUN dotnet publish bull-chat-backend.csproj -c Release -o /app

