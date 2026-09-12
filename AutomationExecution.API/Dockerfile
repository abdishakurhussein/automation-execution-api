FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["AutomationExecution.API.csproj", "./"]

RUN dotnet restore "AutomationExecution.API.csproj"

COPY . .

RUN dotnet publish "AutomationExecution.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "AutomationExecution.API.dll"]