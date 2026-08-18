# 🛒 E-Commerce Microservices Architecture

Proyecto de E-Commerce desarrollado bajo arquitectura de microservicios, diseñado para ser escalable, desacoplado y resiliente. El sistema utiliza mensajería asíncrona para gestionar transacciones distribuidas y asegurar la integridad de los datos.

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)

---

## 🏗️ Arquitectura del Sistema

El proyecto sigue un enfoque de **Microservicios Coreografiados** mediante el patrón **Saga**:

*   **Api.Auth:** Gestión de usuarios y autenticación (JWT).
*   **Api.Pedidos:** Gestión de carrito de compras y órdenes de pedido.
*   **Api.Inventario:** Control de stock de productos.
*   **Api.Notificaciones:** Envío de notificaciones asíncronas vía eventos.

### Flujo de Mensajería
Utilizamos **MassTransit** sobre **RabbitMQ** para la comunicación entre servicios. Cuando un pedido es creado, los servicios de Inventario y Pedidos ejecutan una coreografía de eventos para confirmar o rechazar la transacción, garantizando la consistencia eventual.

---

## 🚀 Cómo ejecutar el proyecto

Este proyecto está totalmente contenedorizado. Solo necesitás tener [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado.

### 1. Clonar el repositorio
```bash
git clone [https://github.com/tu-usuario-github/e-commerce-microservicios-net.git](https://github.com/tu-usuario-github/e-commerce-microservicios-net.git)
cd e-commerce-microservicios-net
```
### 2. Levantar la infraestructura
Ejecuta el siguiente comando en la raíz para iniciar Postgres, RabbitMQ y Redis:
```bash
docker-compose up -d
```
