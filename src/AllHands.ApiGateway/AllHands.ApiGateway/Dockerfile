# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /source

ARG SERVICE_NAME=AllHands.ApiGateway

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ${SERVICE_NAME}/*.csproj ./${SERVICE_NAME}/
RUN dotnet restore ${SERVICE_NAME}/*.csproj

# copy everything else and build app
COPY ${SERVICE_NAME}/. ./${SERVICE_NAME}/
WORKDIR /source/${SERVICE_NAME}
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "AllHands.ApiGateway.dll"]
