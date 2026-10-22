# WeddingAppV3 – Wedding Website Platform V3

Version 3 of the wedding website for Richmond & Keziah, built for **Programming in C#, III – Assignment 3** at Malmö University.

It builds on the ASP.NET Core MVC and Entity Framework Core app from Assignment 2 and adds **Web APIs**, **ASP.NET Core Identity** and **authorization with roles and claims**. Visitors can still read the wedding information, register as guests and answer the RSVP. Admins manage the site from a protected admin area. The same data is also available as JSON through REST endpoints.

![Home page](WeddingAppV3/wwwroot/images/home.png)

## Features

### Public pages
- Home page with the wedding information, edited from the admin area
- Programme of the day (events)
- Photo gallery
- Guest registration followed by the RSVP form

### User accounts (ASP.NET Core Identity)
- Register, log in and log out
- The menu changes depending on whether the user is logged in
- A "My account" page for managing the account, e.g. changing the password
- The Identity pages are **scaffolded** into the project (`Areas/Identity/Pages/Account`), so their code can be read and changed
- The Identity pages use the same layout as the rest of the site (`Views/Shared/_Layout.cshtml`)

### Admin area (protected)
- Dashboard with RSVP statistics: attendance percentage, response rate, average party size and guests grouped by RSVP answer
- Guest management: search, filter, sort, edit and delete
- Event and wedding information management (create, edit, delete)
- Users and roles page for giving or removing the roles and the `CanManageGuests` claim

![Admin dashboard](WeddingAppV3/wwwroot/images/admin.png)

### Web API (JSON)
| Endpoint | Description | Access |
|---|---|---|
| `GET /api/events` | All events (`?location=` to search) | Public |
| `GET /api/events/{id}` | One event | Public |
| `GET /api/events/overview` | Short overview (anonymous type) | Public |
| `GET /api/guests/attending` | Names of attending guests (anonymous type) | Public |
| `GET /api/guests` | All guests (`?status=attending\|declined\|noresponse`, `?sort=name\|name_desc\|newest`) | `CanManageGuests` claim |
| `GET /api/guests/{id}` | One guest | `CanManageGuests` claim |
| `GET /api/statistics` | Totals, percentages and averages | Admin role |
| `GET /api/statistics/attendance` | Guests grouped by RSVP answer | Admin role |
| `GET /api/statistics/registrations?page=1&pageSize=5` | Newest registrations, page by page | Admin role |
| `GET /api/statistics/events` | Events grouped by date | Admin role |

The API returns `200 OK`, `400 Bad Request`, `401 Unauthorized`, `403 Forbidden` or `404 Not Found`, with a message where it helps. All endpoints are also listed with clickable links on the page `/Home/Api`.

## Security

| Account type | Can do |
|---|---|
| Visitor (not logged in) | Public pages, guest registration, RSVP, public API endpoints |
| **User** role | Same as a visitor, plus a personal account |
| **CanManageGuests** claim (policy `GuestManager`) | Guest list and the guest API, even without the Admin role |
| **Admin** role | Everything: dashboard, events, wedding information, users and roles, statistics API |

- Role-based authorization: `[Authorize(Roles = "Admin")]`
- Claims-based authorization: `[Authorize(Policy = "GuestManager")]`
- Protected pages redirect to the login page. Protected API endpoints answer `401` or `403` instead of redirecting.
- The roles and test accounts are created automatically when the app starts (`Data/SeedData.cs`).

## Technologies

- ASP.NET Core MVC (.NET 9)
- Entity Framework Core 9 with SQLite
- ASP.NET Core Identity with scaffolded Razor Pages (roles and claims)
- Web API controllers (`ControllerBase`, `[ApiController]`)
- LINQ: `Where`, `Select`, `OrderBy`, `OrderByDescending`, `Count`, `Sum`, `Average`, `GroupBy`, `Skip`, `Take`
- ViewModels and anonymous types instead of exposing entities
- A service layer (`StatisticsService`) shared by the MVC and API controllers
- Bootstrap 5

### NuGet packages
| Package | Version |
|---|---|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 9.0.21 |
| Microsoft.AspNetCore.Identity.UI | 9.0.21 |
| Microsoft.EntityFrameworkCore | 9.0.21 |
| Microsoft.EntityFrameworkCore.Sqlite | 9.0.21 |
| Microsoft.EntityFrameworkCore.Tools | 9.0.21 |
| Microsoft.VisualStudio.Web.CodeGeneration.Design (used for scaffolding) | 9.0.12 |

## Project structure

```
WeddingAppV3/
├── Areas/Identity/Pages/
│   ├── Account/            Scaffolded Identity pages (Login, Register, Logout, Manage, ...)
│   ├── _Layout.cshtml      Passes the Identity pages on to the site layout
│   └── _ViewStart.cshtml   Layout = "/Views/Shared/_Layout.cshtml"
├── Controllers/            MVC controllers (Home, Guest, Event, Admin, WeddingInfo, Users)
│   └── Api/                API controllers (GuestsApi, EventsApi, StatisticsApi)
├── Data/                   ApplicationDbContext and SeedData
├── Migrations/             EF Core migrations (InitialCreate, AddIdentity)
├── Models/                 Entities: Guest, Event, WeddingInfo, ApplicationUser
├── Services/               StatisticsService
├── ViewModels/             Guest, GuestDetails, Event, Dashboard, Statistics, ...
├── Views/                  Razor views, the shared layout and _LoginPartial
└── wwwroot/                CSS, images and libraries
```

## Getting started

1. Open `WeddingAppV3.sln` in Visual Studio 2022.
2. Open **Tools → NuGet Package Manager → Package Manager Console** and run:
   ```
   Update-Database
   ```
   If the project has no `Migrations` folder, run `Add-Migration InitialCreate` first.
3. Press **F5** to run the application.

The SQLite database `WeddingV3.db` is created in the project folder and filled with sample guests, events and wedding information.

### Test accounts

| Email | Password | Role | Claim |
|---|---|---|---|
| admin@wedding.se | Admin123! | Admin | CanManageGuests |
| planner@wedding.se | Planner123! | User | CanManageGuests |
| user@wedding.se | User123! | User | – |

Roles and claims are stored in the login cookie. After changing a user's roles or claims, that user has to log out and log in again before the change takes effect.

## Notes on the Identity scaffolding

Problems that came up while scaffolding the Identity pages, and how they were solved:

- **"Package restore failed. Rolling back package changes."** The scaffolder installs a newer package than the EF Core and Identity packages in the project. Fixed by updating all EF Core and Identity packages to the same version (9.0.21) and installing `Microsoft.VisualStudio.Web.CodeGeneration.Design` manually before scaffolding again.
- **"'ApplicationUser' is an ambiguous reference."** The scaffolder created a second `ApplicationUser` class in `Data`. Fixed by deleting it and keeping `Models/ApplicationUser.cs`.
- **"The layout view '/Pages/Shared/_Layout.cshtml' could not be located."** The scaffolder points the Identity pages to a Razor Pages layout. Fixed by setting `Layout = "/Views/Shared/_Layout.cshtml";` in `Areas/Identity/Pages/_ViewStart.cshtml`.

## Author

**Richmond** – Programming in C#, III, Malmö University, 2026
Developed by [RHB Digital Solutions](https://rhbdigital.se)
