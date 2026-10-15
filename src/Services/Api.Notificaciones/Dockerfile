FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Shared/SharedContracts/SharedContracts.csproj", "src/Shared/SharedContracts/"]
COPY ["src/Services/Api.Notificaciones/Api.Notificaciones.csproj", "src/Services/Api.Notificaciones/"]

RUN dotnet restore "src/Services/Api.Notificaciones/Api.Notificaciones.csproj"

COPY src/Shared/SharedContracts/ src/Shared/SharedContracts/
COPY src/Services/Api.Notificaciones/ src/Services/Api.Notificaciones/

WORKDIR "/src/src/Services/Api.Notificaciones"
RUN dotnet publish "Api.Notificaciones.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Api.Notificaciones.dll"]

