#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0.16-bullseye-slim-arm64v8 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0.408-bullseye-slim-arm64v8 AS build
WORKDIR /src
COPY . .
RUN dotnet restore --runtime linux-arm64
RUN dotnet build --runtime linux-arm64
RUN dotnet test --runtime linux-arm64

FROM build AS publish
RUN dotnet publish --runtime linux-arm64 -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.dll"]