FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["backend/SudanDialect.Api/SudanDialect.Api.csproj", "backend/SudanDialect.Api/"]
RUN dotnet restore "backend/SudanDialect.Api/SudanDialect.Api.csproj"

COPY . .

WORKDIR "/src/backend/SudanDialect.Api"
RUN dotnet publish "SudanDialect.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "SudanDialect.Api.dll"]
