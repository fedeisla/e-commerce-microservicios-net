FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Shared/SharedContracts/SharedContracts.csproj", "src/Shared/SharedContracts/"]
COPY ["src/Services/Api.Pedidos/Api.Pedidos.Domain/Api.Pedidos.Domain.csproj", "src/Services/Api.Pedidos/Api.Pedidos.Domain/"]
COPY ["src/Services/Api.Pedidos/Api.Pedidos.Application/Api.Pedidos.Application.csproj", "src/Services/Api.Pedidos/Api.Pedidos.Application/"]
COPY ["src/Services/Api.Pedidos/Api.Pedidos.Infrastructure/Api.Pedidos.Infrastructure.csproj", "src/Services/Api.Pedidos/Api.Pedidos.Infrastructure/"]
COPY ["src/Services/Api.Pedidos/Api.Pedidos.WebAPI/Api.Pedidos.WebAPI.csproj", "src/Services/Api.Pedidos/Api.Pedidos.WebAPI/"]

RUN dotnet restore "src/Services/Api.Pedidos/Api.Pedidos.WebAPI/Api.Pedidos.WebAPI.csproj"

COPY src/Shared/SharedContracts/ src/Shared/SharedContracts/
COPY src/Services/Api.Pedidos/ src/Services/Api.Pedidos/

WORKDIR "/src/src/Services/Api.Pedidos/Api.Pedidos.WebAPI"
RUN dotnet publish "Api.Pedidos.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Api.Pedidos.WebAPI.dll"]

