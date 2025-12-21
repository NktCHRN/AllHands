# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

ARG SERVICE_NAME=AllHands.AuthService

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ${SERVICE_NAME}.ConsumersWorker/*.csproj ./${SERVICE_NAME}.ConsumersWorker/
COPY ${SERVICE_NAME}.Infrastructure/*.csproj ./${SERVICE_NAME}.Infrastructure/
COPY ${SERVICE_NAME}.Application/*.csproj ./${SERVICE_NAME}.Application/
COPY ${SERVICE_NAME}.Contracts/*.csproj ./${SERVICE_NAME}.Contracts/
COPY ${SERVICE_NAME}.Domain/*.csproj ./${SERVICE_NAME}.Domain/
RUN dotnet restore ${SERVICE_NAME}.ConsumersWorker/*.csproj

# copy everything else and build app
COPY ${SERVICE_NAME}.ConsumersWorker/. ./${SERVICE_NAME}.ConsumersWorker/
COPY ${SERVICE_NAME}.Infrastructure/. ./${SERVICE_NAME}.Infrastructure/
COPY ${SERVICE_NAME}.Application/. ./${SERVICE_NAME}.Application/
COPY ${SERVICE_NAME}.Contracts/. ./${SERVICE_NAME}.Contracts/
COPY ${SERVICE_NAME}.Domain/. ./${SERVICE_NAME}.Domain/
WORKDIR /source/${SERVICE_NAME}.ConsumersWorker
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "AllHands.AuthService.ConsumersWorker.dll"]
