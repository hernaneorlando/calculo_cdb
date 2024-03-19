FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./backend ./
RUN dotnet publish "WebApi.csproj" -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
RUN apt-get update && apt-get install -y curl
ENTRYPOINT ["dotnet", "WebApi.dll"]
EXPOSE 8080