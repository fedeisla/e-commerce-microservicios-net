@echo off
echo Iniciando todos los microservicios del E-Commerce...

:: Levantar API Gateway
start "API Gateway" cmd /k "cd src\ApiGateways\Api.Gateway && dotnet run"

:: Levantar Servicio de Autenticacion
start "API Auth" cmd /k "cd src\Services\Api.Auth\Api.Auth.WebAPI && dotnet run"

:: Levantar Servicio de Inventario
start "API Inventario" cmd /k "cd src\Services\Api.Inventario\Api.Inventario.WebAPI && dotnet run"

:: Levantar Servicio de Pedidos
start "API Pedidos" cmd /k "cd src\Services\Api.Pedidos\Api.Pedidos.WebAPI && dotnet run"

echo ¡Todas las terminales han sido lanzadas!