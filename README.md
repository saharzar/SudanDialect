# [sudandialect.com](sudandialect.com)

This project aims to preserve Sudan’s linguistic richness and cultural diversity through a modern digital platform. We believe the Sudanese dialect is more than a way of speaking; it is a record of long-standing cultural and linguistic exchange that deserves to be documented for future generations.

The project began as an effort to connect linguistic heritage with modern technology. Thousands of printed and scanned pages were transformed into a structured digital database, with programmatic data processing to improve search accuracy and make browsing easier.

This dictionary is an ongoing effort, and we welcome contributions and corrections to keep improving the content.



## Tech Stack

- Backend: C#, ASP.NET (.NET 10), Entity Framework Core, PostgreSQL
- Frontend: Angular 21

## Features

- Public dictionary search and word details
- Browse words by letter

## Prerequisites

- .NET SDK 10.x
- Node.js 20+
- npm 11+
- PostgreSQL 14+

## Backend Setup

1. Configure secrets for local development.

```bash
cd backend/SudanDialect.Api

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=sudan_dialect;Username=postgres;Password=postgres"
dotnet user-secrets set "Jwt:SigningKey" "replace-with-a-long-random-secret-at-least-32-characters"
dotnet user-secrets set "AdminSeed:Users:0:Username" "admin"
dotnet user-secrets set "AdminSeed:Users:0:Password" "ChangeMe123!"
```

2. Apply database migrations.

```bash
dotnet ef database update
```

3. Run the API.

```bash
dotnet run
```

Default local API URL is `http://localhost:5038`.

## Frontend Setup

1. Install dependencies.

```bash
cd frontend
npm install
```

2. Run the Angular app.

```bash
npm start
```

Default local frontend URL is `http://localhost:4200`.

The frontend currently points to `http://localhost:5038` in environment files.

## API Overview

Public endpoints:

- `GET /api/words/search`
- `GET /api/words/{id}`
- `GET /api/words/browse`

## License

This project is licensed under the GNU General Public License v3.0 (AGPL-3.0).
See the LICENSE file for full terms.
