# Renewable Asset Work Orders API

A beginner-friendly .NET 8 CRUD application for renewable-energy maintenance work orders. It includes an ASP.NET Core API, SQLite storage, and a responsive jQuery interface for managing work orders across wind, solar, battery-storage, and substation assets.

## What it does

- Lists and filters work orders by status
- Creates a maintenance work order
- Edits a work order
- Deletes a work order
- Stores data in a local SQLite database
- Seeds three sample renewable-asset work orders on first run

## Technology

- .NET 8 / ASP.NET Core minimal API
- Entity Framework Core with SQLite
- jQuery 3.7.1
- HTML and CSS

## Run it locally

1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
2. Open a terminal in this project folder.
3. Run:

   ```powershell
   dotnet restore
   dotnet run
   ```

4. Open the address shown in the terminal, usually `http://localhost:5000` or `https://localhost:5001`.

The first run creates `workorders.db` automatically.

## API endpoints

| Method | Endpoint | Purpose |
| --- | --- | --- |
| GET | `/api/workorders` | List all work orders |
| GET | `/api/workorders?status=Open` | Filter by status |
| GET | `/api/workorders/{id}` | Get one work order |
| POST | `/api/workorders` | Create a work order |
| PUT | `/api/workorders/{id}` | Update a work order |
| DELETE | `/api/workorders/{id}` | Delete a work order |

## Production note

This demo intentionally keeps authentication and user roles out of scope. A real energy-operations system should add identity, role-based authorization, audit history, server-side logging, validation policy, secure configuration, and a managed database before deployment.
