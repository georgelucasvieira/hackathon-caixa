FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["API_Simulacao.csproj", "API_Simulacao/"]
WORKDIR "/app/API_Simulacao"
RUN dotnet restore "API_Simulacao.csproj"

COPY . .
RUN dotnet publish "API_Simulacao.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/API_Simulacao/out .
ENTRYPOINT ["dotnet", "API_Simulacao.dll"]
