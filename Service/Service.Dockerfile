FROM mcr.microsoft.com/dotnet/runtime:6.0.16-bullseye-slim-arm64v8 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0.408-bullseye-slim-arm64v8 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build
RUN dotnet test

## We need this step to remove windows line endings from the entrypoint script. Else we get the fault "/usr/bin/env: 'bash\r': No such file or directory"
RUN apt-get update && apt-get install -y dos2unix
RUN dos2unix /src/entrypoint.sh

FROM build AS publish
RUN dotnet publish --runtime linux-arm64 --no-self-contained -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /src/entrypoint.sh .

RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]