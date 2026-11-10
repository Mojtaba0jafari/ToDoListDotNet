# ToDoList API

A small, production-shaped REST API for managing a to-do list, built with **ASP.NET Core (.NET 10)**, **Entity Framework Core** and **PostgreSQL**.

The project is meant to demonstrate backend fundamentals rather than features: a clean controller → service → data layering, DTOs kept separate from the persistence model, consistent HTTP semantics, RFC 9457 error responses, and a documented OpenAPI contract.

---

## Features

- Full CRUD: list, read, create, replace, partially update and delete to-dos
- Pagination with a hard page-size cap
- Filtering by completion status
- Sorting by title, creation date or deadline, in both directions, with a deterministic tiebreaker
- Request DTO validation with per-field error messages
- RFC 9457 (`application/problem+json`) error responses for `400`, `404`, `405`, `415` and `500`
- OpenAPI 3.1 document plus Swagger UI in Development
- UTC normalisation of incoming timestamps and a computed `isOverdue` flag on responses
- EF Core migrations

---

## Requirements

| Tool | Version |
| --- | --- |
| .NET SDK | 10.0 or later |
| PostgreSQL | 14 or later |
| dotnet-ef (optional, for migrations) | 10.x |

```bash
dotnet tool install --global dotnet-ef
```

---

## Getting started

### 1. Restore and build

Restore the NuGet packages:

```bash
dotnet restore
```

Compile the project:

```bash
dotnet build
```

### 2. Configure the database connection

The connection string is **not** stored in `appsettings.json`; it is read from configuration key `ConnectionStrings:DefaultConnection`. For local development use [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets):

**Windows PowerShell**

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=todolist;Username=postgres;Password=your-password"
```

Keep the command on a single line. PowerShell continues a line with a backtick, not a backslash, so ending the line with a backslash breaks it.

**bash / zsh**

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=todolist;Username=postgres;Password=your-password"
```

An environment variable works as well.

**Windows PowerShell**

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Database=todolist;Username=postgres;Password=your-password"
```

**bash / zsh**

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Database=todolist;Username=postgres;Password=your-password"
```

### 3. Apply migrations

```bash
dotnet ef database update
```

### 4. Run

```bash
dotnet run --launch-profile http
```

- API: <http://localhost:5112>
- Swagger UI: <http://localhost:5112/swagger>
- OpenAPI document: <http://localhost:5112/openapi/v1.json>

The `https` launch profile serves both HTTP and HTTPS when a development certificate is trusted (`dotnet dev-certs https --trust`).

---

## API reference

All resources live under `/api/Todos`. Responses are JSON; errors are `application/problem+json`.

| Method | Path | Description | Success | Errors |
| --- | --- | --- | --- | --- |
| `GET` | `/api/Todos` | List to-dos (paged, filtered, sorted) | `200` | `400` |
| `GET` | `/api/Todos/{id}` | Read a single to-do | `200` | `404` |
| `POST` | `/api/Todos` | Create a to-do | `201` + `Location` | `400` |
| `PUT` | `/api/Todos/{id}` | Replace a to-do | `200` | `400`, `404` |
| `PATCH` | `/api/Todos/{id}` | Partially update a to-do | `200` | `400`, `404` |
| `DELETE` | `/api/Todos/{id}` | Delete a to-do | `204` | `404` |

### Query parameters (`GET /api/Todos`)

| Name | Type | Default | Notes |
| --- | --- | --- | --- |
| `page` | integer | `1` | Must be `>= 1` |
| `pageSize` | integer | `10` | Between `1` and `100` |
| `isCompleted` | boolean | – | Omit to return both states |
| `sortBy` | `title` \| `createdAt` \| `deadline` | `createdAt` | Case-insensitive |
| `sortOrder` | `asc` \| `desc` | `desc` | Case-insensitive |

Invalid values are rejected with `400` and a `ValidationProblemDetails` body.

Responses use an envelope:

```json
{
  "items": [
    {
      "id": 1,
      "title": "Learn LINQ",
      "description": "Practice filtering",
      "isCompleted": false,
      "createdAt": "2026-11-01T14:29:03.430527Z",
      "deadline": "2026-11-01T00:00:00Z",
      "isOverdue": true
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

Sorting is deterministic: every ordering is followed by `id DESC`, so records that share a sort value keep a stable position across pages.

### Request bodies

`POST /api/Todos`

```json
{
  "title": "Learn LINQ",
  "description": "Practice filtering",
  "deadline": "2026-11-01T00:00:00Z"
}
```

`PUT /api/Todos/{id}` – full replacement; `title` and `isCompleted` are required, `description` and `deadline` may be `null`.

```json
{
  "title": "Learn LINQ",
  "description": "Practice filtering",
  "isCompleted": true,
  "deadline": "2026-11-01T00:00:00Z"
}
```

`PATCH /api/Todos/{id}` – send only what changes. A field that is absent keeps its value; an explicit `null` clears a nullable field.

Mark a to-do as completed:

```json
{
  "isCompleted": true
}
```

Clear its deadline:

```json
{
  "deadline": null
}
```

### Errors

Validation failure (`400`):

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": { "Title": ["Title cannot exceed 100 characters."] },
  "traceId": "00-8c5fd3d9248fe79fb1d2b11b1e779d0f-390e36ba1c900a76-00"
}
```

Unhandled server errors return `500` with a generic message; the exception is logged with the same `traceId`, and no stack trace or internal detail is sent to the client.

---

## Behaviour worth knowing

- **PUT replaces, PATCH merges.** `PUT` requires `title` and `isCompleted`; omitting one is a `400` rather than an implicit reset. `PATCH` only touches the fields present in the payload.
- **Timestamps are UTC.** Offsets are converted; a timestamp without an offset is treated as UTC.
- **`isOverdue` is computed**, not stored: a to-do is overdue when it is not completed and its deadline is in the past.
- **Unknown JSON members are rejected.** A typo such as `{"titel": "..."}` returns `400` instead of being silently ignored. This also means a `GET` response body cannot be sent straight back to `PUT`: strip `id`, `createdAt` and `isOverdue` first.
- **CORS** is enabled for `http://localhost:5173` and `http://localhost:3000` so a local front end can call the API during development.

---

## Project structure

```
ToDoList.Api/
├── Controllers/          HTTP endpoints, status codes, response metadata
├── Services/             Business logic, query composition, entity → DTO mapping
├── DTOs/                 Request and response contracts
├── Domain/               Entity and shared constraints
├── Data/                 DbContext and EF Core configuration
├── Migrations/           EF Core migrations
├── Program.cs            Composition root and HTTP pipeline
├── GlobalExceptionHandler.cs
└── ToDoList.Api.http     Ready-to-run example requests
```

Dependencies point inward: `Controllers → Services → Data/Domain`. DTOs never contain EF Core types, and the entity never leaks out of the service layer.

---

## Migrations

Create a migration after changing the model:

```bash
dotnet ef migrations add <Name>
```

Apply all pending migrations to the database:

```bash
dotnet ef database update
```

List the migrations and their applied / pending state:

```bash
dotnet ef migrations list
```

Check whether the model has drifted away from the last migration:

```bash
dotnet ef migrations has-pending-model-changes
```

---

## Tests

The unit and integration tests live in a separate repository: [ToDoList.Tests](https://github.com/Mojtaba0jafari/ToDoList.Tests). That repository contains the unit and integration tests for this API — xUnit, an in-memory SQLite database and `WebApplicationFactory` for the HTTP pipeline.
