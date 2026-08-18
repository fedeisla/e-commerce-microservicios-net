# 2. Detalle de Componentes

En esta sección se desglosan los elementos principales que conforman la topología de la red y el ecosistema de microservicios, comenzando por el punto de entrada principal del sistema.



## 2.1 API Gateway (Puerta de Enlace)

En una arquitectura de microservicios, exponer cada servicio directamente al cliente (Frontend o Aplicación Móvil) genera múltiples problemas: acoplamiento de URLs, exposición de la red interna y dificultad para implementar políticas de seguridad globales. 

Para resolver esto, el sistema implementa un **API Gateway** (configurado mediante **YARP - Yet Another Reverse Proxy**), que actúa como el único punto de entrada público para todo el tráfico externo y centraliza las reglas de seguridad, caché y enrutamiento.

### Responsabilidades del Gateway:

1. **Enrutamiento Inverso (Reverse Proxy):** Recibe todas las peticiones HTTP del cliente y las redirige al microservicio correspondiente dentro de la red interna de Docker. El cliente solo conoce la URL del Gateway, ignorando por completo la topología interna (puertos e IPs de los microservicios).
2. **Autenticación y Autorización Centralizada:** Actúa como el primer gran filtro de seguridad del sistema. Se comunica con las reglas de negocio verificando el token JWT emitido por `Api.Auth`. Valida los roles del usuario antes de dejar pasar la petición; por ejemplo, restringe los endpoints de mutación de productos (POST, PUT, DELETE) exclusivamente a la política `AdminOnly`.
3. **Seguridad y Rate Limiting:** Cuenta con un middleware global apoyado en **Redis** para gestionar el límite de peticiones (Rate Limiting). Esto protege a los microservicios internos contra bots, ataques de denegación de servicio (DDoS) y fuerza bruta.
4. **Caché de Salida (Output Caching):** Mejora drásticamente los tiempos de respuesta interceptando peticiones frecuentes. Las consultas públicas al catálogo de productos (`GET /api/productos`) son servidas directamente desde la memoria caché configurada en el Gateway, evitando sobrecargar el microservicio subyacente y la base de datos.

<details>
<summary><b>Ver Configuración de YARP (appsettings.json)</b></summary>

> **Nota Arquitectónica:** En este archivo de configuración se evidencia cómo el Gateway aplica las políticas de caché (`CatalogoCache`), los requisitos de autorización (`AdminOnly` vs `Default`) y el mapeo hacia los clusters correspondientes.

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Key": "EstaEsUnaClaveSuperSecretaYLargaParaFirmarElToken123!",
    "Issuer": "Api.Auth",
    "Audience": "ECommerceClients",
    "ExpireMinutes": 60
  },
  "ReverseProxy": {
    "Routes": {
      "auth-route": {
        "ClusterId": "auth-cluster",
        "Match": {
          "Path": "/api/auth/{**catch-all}"
        }
      },
      "productos-public-get": {
        "ClusterId": "productos-cluster",
        "OutputCachePolicy": "CatalogoCache",
        "Match": {
          "Path": "/api/productos/{**catch-all}",
          "Methods": [ "GET" ]
        }
      },
      "productos-admin-mutations": {
        "ClusterId": "productos-cluster",
        "Match": {
          "Path": "/api/productos/{**catch-all}",
          "Methods": [ "POST", "PUT", "DELETE" ]
        },
        "AuthorizationPolicy": "AdminOnly"
      },
      "pedidos-route": {
        "ClusterId": "pedidos-cluster",
        "Match": {
          "Path": "/api/pedidos/{**catch-all}"
        },
        "AuthorizationPolicy": "Default" 
      }
    },
    "Clusters": {
      "auth-cluster": {
        "Destinations": {
          "destination1": { "Address": "http://localhost:5259" }
        }
      },
      "productos-cluster": {
        "Destinations": {
          "destination1": { "Address": "http://localhost:5069" }
        }
      },
      "pedidos-cluster": {
        "Destinations": {
          "destination1": { "Address": "http://localhost:5141" }
        }
      }
    }
  }
}
```
</details>


---

| <b><a href="./01-introduccion.md">Anterior: Introducción</a></b> | <b><a href="./03-microservicios.md">Siguiente: Microservicios y Dominios</a></b> |
| :--- | ---: |

