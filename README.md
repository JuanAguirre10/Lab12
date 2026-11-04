# GimnasioApi

Web API REST para administrar las membresías y los socios de un gimnasio. Está hecha con ASP.NET Core (.NET 10) y Entity Framework Core con **base de datos primero**: la base `GimnasioDB` se crea con el script `db/GimnasioDB.sql` y el modelo se genera con Scaffold. Los controladores usan directamente el `DbContext` y todo el acceso a datos se hace con LINQ.

Swagger publicado: (pendiente)

## Entidades

| Entidad | Campos |
|---|---|
| `Membresia` | Id, Nombre (único, máx. 50), Precio (decimal(10,2)), DuracionDias (1 a 365) |
| `Socio` | Id, Nombres (máx. 100), Dni (8 dígitos, único), Telefono (opcional, 9 dígitos que empiezan con 9), FechaInscripcion, MembresiaId |

Relación 1:N: una membresía tiene muchos socios y cada socio pertenece a una membresía (`FK_Socios_Membresias`).

- `Socio.Membresia` admite nulo, así que en el POST solo se envía `membresiaId`.
- `Membresia.Socios` tiene `[JsonIgnore]` para evitar el ciclo en el JSON.
- Las validaciones están en los Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, `[RegularExpression]`) con mensajes en español. Con `[ApiController]`, si los datos no son válidos la API responde 400 automáticamente.

## Reglas de negocio (409)

- No se puede registrar ni renombrar una membresía con un nombre que ya existe.
- No se puede eliminar una membresía que tiene socios.
- No se puede registrar ni actualizar un socio con un DNI que ya existe.

Si `membresiaId` no existe, el POST y el PUT de socios responden 400 con el error en `MembresiaId`.

## Endpoints

| Verbo | Ruta | Respuesta |
|---|---|---|
| GET | `/api/membresias` | 200 |
| GET | `/api/membresias/{id}` | 200, 404 |
| GET | `/api/membresias/{id}/socios` | 200, 404 |
| POST | `/api/membresias` | 201, 400, 409 |
| PUT | `/api/membresias/{id}` | 204, 400, 404, 409 |
| DELETE | `/api/membresias/{id}` | 204, 404, 409 |
| GET | `/api/socios?membresiaId=&buscar=` | 200 |
| GET | `/api/socios/{id}` | 200, 404 |
| POST | `/api/socios` | 201, 400, 409 |
| PUT | `/api/socios/{id}` | 204, 400, 404, 409 |
| DELETE | `/api/socios/{id}` | 204, 404 |

`buscar` filtra por nombres o DNI. Las pruebas de cada endpoint y de cada código están en `GimnasioApi/GimnasioApi.http`.

## Ejecutar en local (Visual Studio)

1. En SSMS, conectarse a `.\SQLEXPRESS`, crear la base `GimnasioDB` y ejecutar `db/GimnasioDB.sql` sobre ella.
2. Abrir `Gimnasio.slnx`.
3. Clic derecho en `GimnasioApi` > **Administrar secretos de usuario** y pegar:

   ```json
   {
     "ConnectionStrings": {
       "Gimnasio": "Server=.\\SQLEXPRESS;Database=GimnasioDB;Integrated Security=True;TrustServerCertificate=True"
     }
   }
   ```

4. Ejecutar con F5 (perfil `http`). Se abre Swagger en http://localhost:5206/swagger.

Desde la terminal:

```bash
cd GimnasioApi
dotnet user-secrets set "ConnectionStrings:Gimnasio" "Server=.\SQLEXPRESS;Database=GimnasioDB;Integrated Security=True;TrustServerCertificate=True"
dotnet run --launch-profile http
```

La cadena de conexión no está en `appsettings.json`. En local se lee de los secretos de usuario y en la nube de la variable de entorno `ConnectionStrings__Gimnasio`.

## Scaffold

Consola del Administrador de paquetes (proyecto predeterminado `GimnasioApi`):

```
Scaffold-DbContext "Server=.\SQLEXPRESS;Database=GimnasioDB;Integrated Security=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Context GimnasioContext -NoOnConfiguring
```

Terminal (`dotnet-ef` está en el manifiesto local `dotnet-tools.json`):

```bash
dotnet tool restore
cd GimnasioApi
dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS;Database=GimnasioDB;Integrated Security=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models -c GimnasioContext --no-onconfiguring
```

Después del Scaffold se agregaron a mano los Data Annotations, el `[JsonIgnore]` de `Membresia.Socios` y el `?` de `Socio.Membresia`. Si se vuelve a generar, hay que agregarlos otra vez.

## Estructura

```
Lab12/
├── Gimnasio.slnx
├── dotnet-tools.json
├── db/GimnasioDB.sql
└── GimnasioApi/
    ├── Models/        GimnasioContext, Membresia, Socio
    ├── Controllers/   MembresiasController, SociosController
    ├── Program.cs     EF Core, Swagger y CORS
    └── GimnasioApi.http
```

CORS solo permite el origen `http://localhost:5173`.
