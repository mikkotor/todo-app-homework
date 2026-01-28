FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TodoApi.csproj", "."]
RUN dotnet restore "./TodoApi.csproj"
COPY . .
RUN dotnet build "TodoApi.csproj" -c Release -o /app
RUN dotnet publish "TodoApi.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 3001
COPY --from=build /app .
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:3001
ENTRYPOINT ["dotnet", "TodoApi.dll"]
