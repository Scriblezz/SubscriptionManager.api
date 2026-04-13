# SubscriptionManager.Api

A RESTful API for managing personal subscriptions, built with ASP.NET Core and PostgreSQL. Users can register, log in, and manage their own subscriptions — with full filtering, sorting, and pagination support.

---

## Features

- JWT-based authentication (register, login)
- User-scoped subscriptions — each user only sees their own data
- Full CRUD for subscriptions
- Filtering by category and active status
- Sorting by name, price, or next renewal date
- Pagination support
- Subscription renewal logic with billing cycle support (Weekly, Monthly, Yearly)
- Global exception handling middleware

---

## Tech Stack

- **ASP.NET Core 9** — Web API
- **Entity Framework Core** — ORM
- **PostgreSQL 16** — Database (via Docker)
- **BCrypt.Net** — Password hashing
- **JWT Bearer** — Authentication
- **Swagger / Swashbuckle** — API documentation

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### 1. Clone the repository

```bash
git clone https://github.com/Scriblezz/SubscriptionManager.api.git
cd SubscriptionManager.api
```

### 2. Set up the database

Create a `.env` file in the root of the repository with the following values:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_password
POSTGRES_DB=subscription_manager
```

Then start the PostgreSQL container:

```bash
docker-compose up -d
```

### 3. Configure user secrets

Navigate to the API project folder and set the required JWT secrets:

```bash
cd SubscriptionManager.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-secret-key-at-least-32-characters-long"
dotnet user-secrets set "Jwt:Issuer" "SubscriptionManagerApi"
dotnet user-secrets set "Jwt:Audience" "SubscriptionManagerClient"
```

### 4. Update the connection string

In `appsettings.json`, update the connection string to match your `.env` values:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5433;Username=postgres;Password=your_password;Database=subscription_manager;"
}
```

### 5. Apply database migrations

```bash
dotnet ef database update
```

### 6. Run the API

```bash
dotnet run
```

The API will be available at `http://localhost:5001`. Swagger UI is available at `http://localhost:5001/swagger`.

---

## Authentication

This API uses JWT Bearer authentication.

1. Register a new account via `POST /api/auth/register`
2. Log in via `POST /api/auth/login` to receive a token
3. In Swagger, click the **Authorize** button and enter your token
4. All subscription endpoints require a valid token

---

## API Endpoints

### Auth

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register a new user | No |
| POST | `/api/auth/login` | Login and receive JWT token | No |

### Subscriptions

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/subscriptions` | Get all subscriptions (paginated) | Yes |
| GET | `/api/subscriptions/{id}` | Get subscription by ID | Yes |
| POST | `/api/subscriptions` | Create a new subscription | Yes |
| PUT | `/api/subscriptions/{id}` | Update a subscription | Yes |
| DELETE | `/api/subscriptions/{id}` | Delete a subscription | Yes |
| POST | `/api/subscriptions/{id}/renew` | Renew a subscription | Yes |

### Query Parameters (GET /api/subscriptions)

| Parameter | Type | Description |
|-----------|------|-------------|
| `category` | string | Filter by category |
| `isActive` | bool | Filter by active status |
| `sortBy` | string | Sort by `name`, `price`, or `nextRenewalDate` |
| `sortDirection` | string | `asc` or `desc` |
| `page` | int | Page number (default: 1) |
| `pageSize` | int | Results per page (default: 10, max: 100) |

---

## Project Structure

```
SubscriptionManager.Api/
├── Controllers/        # API endpoints
├── DTO/                # Request and response models
├── Entities/           # Database models
├── Exceptions/         # Custom exception classes
├── Middleware/         # Global exception handling
├── Migrations/         # EF Core migrations
└── Services/           # Business logic
```

---

## Security Notes

- Passwords are hashed using BCrypt before being stored
- JWT secrets are stored using .NET User Secrets (never committed to source control)
- All subscription endpoints are user-scoped — users cannot access each other's data
