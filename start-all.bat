@echo off
echo =======================================================
echo   Iniciando E-Commerce Microservices Architecture...
echo =======================================================

echo Iniciando Servicio de Autenticacion...
start "API Auth" cmd /k "cd src\Services\Api.Auth\Api.Auth.WebAPI && title Api.Auth && dotnet run"

echo Iniciando Servicio de Inventario...
start "API Inventario" cmd /k "cd src\Services\Api.Inventario\Api.Inventario.WebAPI && title Api.Inventario && dotnet run"

echo Iniciando Servicio de Pedidos...
start "API Pedidos" cmd /k "cd src\Services\Api.Pedidos\Api.Pedidos.WebAPI && title Api.Pedidos && dotnet run"

echo Iniciando Servicio de Notificaciones...
start "API Notificaciones" cmd /k "cd src\Services\Api.Notificaciones\Api.Notificaciones.WebAPI && title Api.Notificaciones && dotnet run"

:: Esperamos 2 segundos para que las APIs levanten antes de iniciar el Gateway
timeout /t 2 /nobreak > NUL

echo Iniciando API Gateway (YARP)...
start "API Gateway" cmd /k "cd src\ApiGateways\Api.Gateway && title Api.Gateway && dotnet run"

echo.
echo =======================================================
echo  ¡Todas las terminales han sido lanzadas con exito!
echo  Revisa las ventanas para confirmar que no haya errores.
echo =======================================================
pause