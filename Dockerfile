FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["iCarus.csproj", "./"]
RUN dotnet restore "iCarus.csproj"

COPY . .
RUN dotnet publish "iCarus.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "dotnet iCarus.dll --urls http://0.0.0.0:${PORT:-8080}"]
