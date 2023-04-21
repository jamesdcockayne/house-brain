#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0.16-bullseye-slim-arm64v8 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0.408-bullseye-slim-arm64v8 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Service/Service.csproj --runtime linux-arm64
RUN dotnet restore ServiceTests/ServiceTests.csproj --runtime linux-arm64

RUN dotnet build Service/Service.csproj --runtime linux-arm64 --no-self-contained
RUN dotnet build ServiceTests/ServiceTests.csproj

RUN dotnet test ServiceTests/ServiceTests.csproj 

FROM build AS publish
RUN dotnet publish --runtime linux-arm64 --no-self-contained -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.dll"]