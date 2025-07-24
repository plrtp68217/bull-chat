#!/bin/sh
dotnet ef database update
dotnet bull-chat-backend.dll --urls "http://0.0.0.0:80"