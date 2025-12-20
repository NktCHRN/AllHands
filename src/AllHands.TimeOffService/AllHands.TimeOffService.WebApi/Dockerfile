# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /source

ARG SERVICE_NAME=AllHands.TimeOffService

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ${SERVICE_NAME}.WebApi/*.csproj ./${SERVICE_NAME}.WebApi/
COPY ${SERVICE_NAME}.Infrastructure/*.csproj ./${SERVICE_NAME}.Infrastructure/
COPY ${SERVICE_NAME}.Application/*.csproj ./${SERVICE_NAME}.Application/
COPY ${SERVICE_NAME}.Domain/*.csproj ./${SERVICE_NAME}.Domain/
RUN dotnet restore ${SERVICE_NAME}.WebApi/*.csproj

# copy everything else and build app
COPY ${SERVICE_NAME}.WebApi/. ./${SERVICE_NAME}.WebApi/
COPY ${SERVICE_NAME}.Infrastructure/. ./${SERVICE_NAME}.Infrastructure/
COPY ${SERVICE_NAME}.Application/. ./${SERVICE_NAME}.Application/
COPY ${SERVICE_NAME}.Domain/. ./${SERVICE_NAME}.Domain/
WORKDIR /source/${SERVICE_NAME}.WebApi
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
ENV RUN_MIGRATIONS=true
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "AllHands.TimeOffService.WebApi.dll"]
