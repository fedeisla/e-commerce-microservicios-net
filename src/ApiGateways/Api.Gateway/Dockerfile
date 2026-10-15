FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/ApiGateways/Api.Gateway/Api.Gateway.csproj", "src/ApiGateways/Api.Gateway/"]
RUN dotnet restore "src/ApiGateways/Api.Gateway/Api.Gateway.csproj"

COPY src/ApiGateways/Api.Gateway/ src/ApiGateways/Api.Gateway/
WORKDIR "/src/src/ApiGateways/Api.Gateway"
RUN dotnet publish "Api.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Api.Gateway.dll"]

