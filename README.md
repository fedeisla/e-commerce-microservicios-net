# E-Commerce Microservices Architecture

Proyecto de E-Commerce desarrollado bajo una arquitectura de microservicios, diseñado para ser escalable, desacoplado y resiliente. El sistema utiliza mensajería asíncrona para gestionar transacciones distribuidas y asegurar la integridad de los datos, demostrando patrones de diseño orientados a entornos corporativos (Cloud-Native).

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2CA5E0?style=for-the-badge&logo=docker&logoColor=white)

---

## Documentación Técnica (Wiki)

Para comprender en profundidad las decisiones de diseño, los patrones implementados y el flujo de los datos, te invito a leer la documentación detallada del proyecto:

1. [Introducción y Objetivos Arquitectónicos](./docs/01-introduccion.md)
2. [Arquitectura Orientada a Eventos y Patrón Saga](./docs/02-saga-rabbitmq.md)
3. [API Gateway (YARP) y Seguridad](./docs/03-componentes.md)
4. [Microservicios y Dominios](./docs/04-microservicios.md)
5. [Infraestructura y Estrategia de Despliegue](./docs/05-despliegue-e-infraestructura.md)


---

## Arquitectura del Sistema

El proyecto sigue un enfoque de **Microservicios Coreografiados** mediante el patrón **Saga** y cuenta con un único punto de entrada:

* **API Gateway (YARP):** Enrutador central, balanceador de carga, Rate Limiter distribuido (Redis) y manejador de caché.
* **Api.Auth:** Gestión de usuarios y emisión de identidad (JWT).
* **Api.Pedidos:** Gestión transaccional de carritos de compras y órdenes de pedido.
* **Api.Inventario:** Catálogo y control de disponibilidad de stock (Unit of Work).
* **Api.Notificaciones:** Servicio Worker (Stateless) para la emisión asíncrona de alertas.

### Flujo de Mensajería
Se utiliza **MassTransit** sobre **RabbitMQ** para la comunicación entre servicios. Cuando un pedido es creado, los dominios de Inventario y Pedidos ejecutan una coreografía de eventos para confirmar o rechazar la transacción, garantizando la **Consistencia Eventual** y la **Alta Disponibilidad** del sistema sin acoplamiento temporal.

---

## Cómo ejecutar el proyecto

Este proyecto cuenta con su capa de persistencia y mensajería totalmente contenedorizada. Es necesario tener [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado.

### 1. Clonar el repositorio
```bash
git clone [https://github.com/tu-usuario-github/e-commerce-microservicios-net.git](https://github.com/tu-usuario-github/e-commerce-microservicios-net.git)
cd e-commerce-microservicios-net
```

### 2. Levantar la infraestructura (Bases de datos y Message Broker)
Ejecuta el siguiente comando en la raíz para iniciar PostgreSQL, RabbitMQ y Redis en contenedores aislados:
```bash
docker-compose up -d
```

### 3. Iniciar los Microservicios
Para facilitar la evaluación técnica, el proyecto incluye un script de automatización. Simplemente ejecuta el archivo `start-services.bat` ubicado en la raíz del proyecto (puedes hacerle doble clic o ejecutarlo desde la terminal).

Este script abrirá automáticamente una ventana de terminal para cada dominio y el API Gateway, permitiéndote observar el flujo asíncrono y los logs en tiempo real.

---

## Pruebas (Postman)

En la carpeta raíz del repositorio encontrarás el archivo `ECommerce-Saga-Collection.json`. Puedes importarlo directamente en Postman o Insomnia para tener listos todos los endpoints configurados (Registro, Login, Gestión de Catálogo y Flujo de Checkout) y probar la arquitectura localmente.

###  Exploración de APIs (Swagger UI)

Además de la colección de Postman, todos los microservicios tienen **Swagger** configurado e integrado. 

Si deseas explorar la totalidad de los endpoints disponibles, verificar los esquemas de datos (DTOs) o probar peticiones individuales de forma visual, puedes acceder a la interfaz de Swagger navegando a la ruta `/swagger` de cada API mientras se encuentran en ejecución (por ejemplo: `http://localhost:5000/swagger` para el Gateway).

> Para probar los endpoints protegidos desde Swagger, haz clic en el botón **"Authorize"** en la parte superior de la página e ingresa tu token con el prefijo Bearer (ejemplo: `Bearer eyJhbGci...`).