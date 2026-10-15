FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Shared/SharedContracts/SharedContracts.csproj", "src/Shared/SharedContracts/"]
COPY ["src/Services/Api.Auth/Api.Auth.Domain/Api.Auth.Domain.csproj", "src/Services/Api.Auth/Api.Auth.Domain/"]
COPY ["src/Services/Api.Auth/Api.Auth.Application/Api.Auth.Application.csproj", "src/Services/Api.Auth/Api.Auth.Application/"]
COPY ["src/Services/Api.Auth/Api.Auth.Infrastructure/Api.Auth.Infrastructure.csproj", "src/Services/Api.Auth/Api.Auth.Infrastructure/"]
COPY ["src/Services/Api.Auth/Api.Auth.WebAPI/Api.Auth.WebAPI.csproj", "src/Services/Api.Auth/Api.Auth.WebAPI/"]

RUN dotnet restore "src/Services/Api.Auth/Api.Auth.WebAPI/Api.Auth.WebAPI.csproj"

COPY src/Shared/SharedContracts/ src/Shared/SharedContracts/
COPY src/Services/Api.Auth/ src/Services/Api.Auth/

WORKDIR "/src/src/Services/Api.Auth/Api.Auth.WebAPI"
RUN dotnet publish "Api.Auth.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Api.Auth.WebAPI.dll"]

