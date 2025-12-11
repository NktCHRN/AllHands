# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY AllHands.WebApi/*.csproj ./AllHands.WebApi/
COPY AllHands.Infrastructure/*.csproj ./AllHands.Infrastructure/
COPY AllHands.Application/*.csproj ./AllHands.Application/
COPY AllHands.Domain/*.csproj ./AllHands.Domain/
RUN dotnet restore

# copy everything else and build app
COPY AllHands.WebApi/. ./AllHands.WebApi/
COPY AllHands.Infrastructure/. ./AllHands.Infrastructure/
COPY AllHands.Application/. ./AllHands.Application/
COPY AllHands.Domain/. ./AllHands.Domain/
WORKDIR /source/AllHands.WebApi
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
ENV RUN_MIGRATIONS=true
ENV FEATURE_TIMEOFFBALANCE_AUTOUPDATER=true
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "AllHands.WebApi.dll"]
