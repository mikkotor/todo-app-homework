FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TodoApi.csproj", "."]
RUN dotnet restore "./TodoApi.csproj"
COPY . .
RUN dotnet build "TodoApi.csproj" -c Release -o /app
RUN dotnet publish "TodoApi.csproj" -c Release -o /app
# Generate self-signed certificate for HTTPS
RUN mkdir -p /app/https && \
    openssl req -x509 -newkey rsa:4096 -keyout /app/https/key.pem -out /app/https/cert.pem -days 365 -nodes -subj "/CN=localhost" && \
    openssl pkcs12 -export -out /app/https/cert.pfx -inkey /app/https/key.pem -in /app/https/cert.pem -passout pass:

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 7105
COPY --from=build /app .
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/https/cert.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=""
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://+:7105
ENTRYPOINT ["dotnet", "TodoApi.dll"]
