# SubscriptionManager

A full stack subscription management app built with ASP.NET Core, React, and PostgreSQL. Users can register, log in, and manage their personal subscriptions with a clean, responsive UI that supports dark mode.

---

## Live Demo
- **Frontend:** https://subscription-manager-api-azure.vercel.app
- **API:** https://subscriptionmanagerapi-production.up.railway.app

## Features

### Backend
- JWT-based authentication (register, login)
- User-scoped subscriptions — each user only sees their own data
- Full CRUD for subscriptions (create, read, update, delete)
- Subscription renewal logic with billing cycle support (Weekly, Monthly, Yearly)
- Filtering by category and active status
- Sorting by name, price, or next renewal date
- Pagination support
- Global exception handling middleware
- Proper HTTP status codes (401 for unauthorized, 404 for not found, 400 for bad requests)

### Frontend
- Login and registration pages
- View all subscriptions with next renewal date
- Add, edit, and delete subscriptions
- Renew subscriptions (only shown for active subscriptions)
- Dark mode toggle across all pages
- Billing cycle dropdown to prevent invalid input
- Responsive layout with Tailwind CSS

---

## Tech Stack

### Backend
- **ASP.NET Core 9** — Web API
- **Entity Framework Core** — ORM
- **PostgreSQL 16** — Database (via Docker)
- **BCrypt.Net** — Password hashing
- **JWT Bearer** — Authentication
- **Swagger / Swashbuckle** — API documentation

### Frontend
- **React 19** — UI framework
- **Vite** — Build tool
- **Tailwind CSS** — Styling
- **React Router** — Client-side routing

---

## Project Structure

```
SubscriptionManager/
├── SubscriptionManager.api/        # ASP.NET Core Web API
│   └── SubscriptionManager.Api/
│       ├── Controllers/            # API endpoints
│       ├── DTO/                    # Request and response models
│       ├── Entities/               # Database models
│       ├── Exceptions/             # Custom exception classes
│       ├── Middleware/             # Global exception handling
│       ├── Migrations/             # EF Core migrations
│       └── Services/               # Business logic
└── SubscriptionManager.client/     # React frontend
    └── src/
        └── pages/                  # Login, Register, Subscriptions
```

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js 22+](https://nodejs.org)

### 1. Clone the repository

```bash
git clone https://github.com/Scriblezz/SubscriptionManager.api.git
cd SubscriptionManager.api
```

### 2. Set up the database

Create a `.env` file in the root of the repository:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_password
POSTGRES_DB=subscription_manager
```

Then start the PostgreSQL container:

```bash
docker-compose up -d
```

### 3. Configure API user secrets

Navigate to the API project folder and set the required JWT secrets:

```bash
cd SubscriptionManager.api/SubscriptionManager.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-secret-key-at-least-32-characters-long"
dotnet user-secrets set "Jwt:Issuer" "SubscriptionManagerApi"
dotnet user-secrets set "Jwt:Audience" "SubscriptionManagerClient"
```

### 4. Update the connection string

In `SubscriptionManager.api/SubscriptionManager.Api/appsettings.json`, update the connection string to match your `.env` values:

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

### 7. Run the frontend

In a new terminal:

```bash
cd SubscriptionManager.client
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`.

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

## Security Notes

- Passwords are hashed using BCrypt before being stored
- JWT secrets are stored using .NET User Secrets (never committed to source control)
- All subscription endpoints are user-scoped — users cannot access each other's data
- Invalid login attempts return 401 Unauthorized without revealing whether the email exists
