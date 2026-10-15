FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Shared/SharedContracts/SharedContracts.csproj", "src/Shared/SharedContracts/"]
COPY ["src/Services/Api.Inventario/Api.Inventario.Domain/Api.Inventario.Domain.csproj", "src/Services/Api.Inventario/Api.Inventario.Domain/"]
COPY ["src/Services/Api.Inventario/Api.Inventario.Application/Api.Inventario.Application.csproj", "src/Services/Api.Inventario/Api.Inventario.Application/"]
COPY ["src/Services/Api.Inventario/Api.Inventario.Infrastructure/Api.Inventario.Infrastructure.csproj", "src/Services/Api.Inventario/Api.Inventario.Infrastructure/"]
COPY ["src/Services/Api.Inventario/Api.Inventario.WebAPI/Api.Inventario.WebAPI.csproj", "src/Services/Api.Inventario/Api.Inventario.WebAPI/"]

RUN dotnet restore "src/Services/Api.Inventario/Api.Inventario.WebAPI/Api.Inventario.WebAPI.csproj"

COPY src/Shared/SharedContracts/ src/Shared/SharedContracts/
COPY src/Services/Api.Inventario/ src/Services/Api.Inventario/

WORKDIR "/src/src/Services/Api.Inventario/Api.Inventario.WebAPI"
RUN dotnet publish "Api.Inventario.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Api.Inventario.WebAPI.dll"]

