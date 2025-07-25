# DevPhone

Este es un proyecto web solución a una problematica que presenta la empresa PhonePlace que utiliza ASP.NET MVC + Alpine.js y SQL Server, a continuación se muestra los pasos para ejecutar el proyecto con exito.

## Configuración del servidor de base de datos

Se debe dirigir al archivo `appsettings.json` y cambiar la cadena conexión con sus datos en este caso solo cambiar el nombre del servidor:
```bash
"DefaultConnection": "Server=[NOMBRE_SERVIDOR];Database=DevPhoneDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

## Migraciones

Se debe realizar las migraciones para que la BD tenga las tablas, para aquello debe abrir la Consola del Administrador de paquetes

<img width="833" height="165" alt="image" src="https://github.com/user-attachments/assets/16350438-4828-414c-9b5b-66f873d16f12" />

Una vez abierta debe escribir `update-database` eso creara las tablas, en caso de error revise el paso anterior.

## Ejecutar el proyecto
Ejecute el proyecto o presione F5, la primera vez demorara un poco. Finalmente para ingresar y probar las funciones puede ingresar con las credenciales:

Usuario: `admin`
Contraseña: `Admin123!`

