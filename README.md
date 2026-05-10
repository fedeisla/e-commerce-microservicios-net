
# E-Commerce Microservices (.NET & Docker)

Este proyecto es una implementación de una arquitectura de microservicios orientada a eventos, diseñada para ser escalable, desacoplada y de alta disponibilidad. Utiliza el ecosistema de **.NET** junto con herramientas de mensajería, caché y contenedores para resolver los desafíos de un sistema de comercio electrónico moderno.

##  Arquitectura del Sistema

El sistema está diseñado bajo el patrón de **Arquitectura Orientada a Eventos (EDA)**. El flujo principal se basa en el desacople de procesos pesados (como la actualización de inventario y notificaciones) del hilo principal de ejecución del cliente.

### Modelo Inicial
A continuación se presenta el esquema arquitectónico que rige el diseño de este sistema:

<img width="945" height="1026" alt="Microservicio" src="https://github.com/user-attachments/assets/935682cf-72ee-4ab6-ac6a-fd2320942caa" />


### Componentes Principales:
- **API E-Commerce (User):** El carril rápido para el cliente final. Implementa caché con **Redis** para lecturas de stock ultrarrápidas y publica eventos en **RabbitMQ**.
- **API E-Commerce Admin:** Panel de gestión síncrono para administradores.
- **Microservicio de Inventario:** Worker Service que consume eventos de compra y gestiona el stock físico.
- **Microservicio de Identidad:** Proveedor de identidad basado en **JWT** (JSON Web Tokens).
- **Microservicio de Notificaciones:** Sistema de alertas al usuario final.

## Tecnologías Utilizadas
- **Backend:** .NET 8 (C#)
- **Mensajería:** RabbitMQ (Broker)
- **Caché:** Redis
- **Base de Datos:** PostgreSQL
- **Contenedores:** Docker & Docker Compose
- **ORM:** Entity Framework Core

## Cómo Levantar el Proyecto

Este proyecto utiliza Docker para garantizar que el entorno de desarrollo sea idéntico en cualquier máquina.

### Requisitos previos:
- Docker Desktop instalado.
- .NET SDK 8 instalado (para desarrollo).

### Pasos para levantar el proyecto:

1. Clonar el repositorio

 ```Bash
git clone https://github.com/fedeisla/e-commerce-microservicios-net.git
```
2. Acceder a la carpeta del proyecto:

 ```Bash
cd e-commerce-microservicios-net
```

3. Levantar los servicios con Docker:
Asegúrate de tener Docker Desktop iniciado y ejecuta:

 ```Bash
docker-compose up -d
```
