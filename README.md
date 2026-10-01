# Rivel 🚢

A two-sided freight logistics marketplace digitizing Nile river shipping — connecting **Cargo Owners** who need to move goods with **Carriers** who operate vessels.

This repository is the **backend API**. The Angular frontend lives in a separate repository (`Rivel_Fr`).

🌐 **Live app:** [rivel.site](https://www.rivel.site/) &nbsp;|&nbsp; 🔌 **API/Swagger:** [rivel.runasp.net/swagger](https://rivel.runasp.net/swagger)

---

## Table of contents

- [What it does](#what-it-does)
- [Roles](#roles)
- [Tech stack](#tech-stack)
- [Core domain flow](#core-domain-flow)
- [Architecture](#architecture)
- [Project structure](#project-structure)
- [Domain model](#domain-model)
- [API reference](#api-reference)
- [Cross-cutting concerns](#cross-cutting-concerns)
- [Configuration](#configuration)
- [Local development](#local-development)
- [Deployment](#deployment)
- [Roadmap](#roadmap)

---

## What it does

Cargo owners post shipment requests with cargo type, weight, origin/destination berths and a requested date. Carriers declare the routes they run, get notified when a matching request appears, and submit offers (price + proposed pickup date) using one of their own vessels. The owner accepts one offer — which rejects the competing offers, withdraws the vessel's other pending offers, and creates a shipment. The carrier then advances the shipment through `Matched → PickedUp → InTransit → Delivered`, after which the owner leaves a rating that updates the carrier's public score.

The whole matching loop is driven off a seeded dataset of **48 Nile berths** grouped into three navigation axes (Cairo–Aswan, Cairo–Damietta, Aswan–Wadi Halfa).

---

## Roles

| Role | Capabilities |
|---|---|
| **Cargo Owner** | Post shipment requests · Review and accept offers · Track shipments · Rate carriers |
| **Carrier** | Manage company profile and logo · Register and maintain vessels · Declare active routes · Browse and accept suggested requests · Submit offers · Advance shipment status |

Role is baked into the JWT as a `role` claim and enforced with `[Authorize(Roles = ...)]` per controller or action. Carrier registration also creates a `CarrierProfile` row (company name, bio, logo, rolling rating).

---

## Tech stack

**Backend — .NET 10 / ASP.NET Core**

| Concern | Choice |
|---|---|
| Framework | ASP.NET Core on `net10.0`, minimal hosting (`Program.cs`) |
| ORM | Entity Framework Core 10 + Npgsql |
| Database | PostgreSQL 17 (Supabase in production, `postgres:17` in Docker Compose) |
| Identity | ASP.NET Core Identity with `Guid` keys, EF Core stores |
| Auth | JWT bearer (HS256) + rotating refresh tokens stored on the user |
| Validation | FluentValidation 12 via an `IAsyncActionFilter` |
| Errors | RFC 7807 ProblemDetails + `IExceptionHandler` implementations |
| Rate limiting | Built-in `AddRateLimiter`, fixed-window, per-IP and per-user policies |
| Realtime | SignalR `NotificationHub` |
| Email | Resend, with in-process HTML templating (Arabic, RTL) |
| Storage | Supabase Storage over HTTP (carrier logos) |
| API docs | Swashbuckle / OpenAPI with a bearer security definition |
| Token crypto | Data Protection persisted to the database via `IDataProtectionKeyContext` |
| Queries | `System.Linq.Dynamic.Core` available for dynamic filters |

**Frontend — Angular 22** (separate repository)

Standalone components, Angular signals for local state, Taiga UI, Tailwind CSS 4, `@microsoft/signalr` client, `ngx-translate` for AR/EN, route guards by role, auth + error interceptors. Deployed on Vercel.

---

## Core domain flow

```
Carrier registers → declares active routes → vessels registered as Available
        │
Cargo owner posts shipment request (Open)
        │  └──► background worker notifies carriers whose routes match
        │      origin→destination AND who have a bookable vessel with enough capacity
        ▼
Carrier submits offer (Pending) — vessel must be owned, Available, capacity ≥ cargo weight
        │
Owner accepts one offer ─┬─► accepted offer → Accepted, vessel → OnTrip, request → Matched
                          ├─► sibling offers on that request → Rejected (owner notified)
                          ├─► vessel's other pending offers → Withdrawn (owner notified)
                          └─► Shipment created in Matched state
        ▼
Carrier advances: Matched → PickedUp → InTransit → Delivered
        │  (strictly sequential; vessel returns to Available on Delivered)
        ▼
Owner rates the shipment once (1–5, optional comment)
        └──► carrier's OverallRating + RatingCount recomputed
```

**Vessel state** is derived, not free-form: the carrier may only set `Available` or `Maintenance`. `OnTrip` is assigned by the system on offer acceptance and released on delivery. Switching a vessel to maintenance, or archiving it, withdraws its pending offers.

**Shipment status transitions** are enforced by a static transition map in `ShipmentService`; anything out of order returns `409 Conflict`.

---

## Architecture

```
Program.cs
  └─ DependencyInjection.cs   one extension per concern
       ├─ AddControllers          MVC + JSON/XML formatters + enum-as-string + FluentValidationFilter
       ├─ AddErrorHandling        ProblemDetails + two IExceptionHandlers
       ├─ AddDatabase             ApplicationDbContext on Npgsql
       ├─ AddApplicationServices  all domain services + mappers, scoped
       ├─ AddFilesUploadServices  IFileStorage → Supabase, options validated at startup
       ├─ AddAuthenticationServices  Identity + JWT bearer + Data Protection + MemoryCache
       ├─ AddCors                 "RiverLinePolicy" from Cors:AllowedOrigins
       ├─ AddRateLimiting         "public" and "sensitive" fixed-window policies
       └─ AddNotificationServices SignalR + Resend + AppUrls + MatchNotificationWorker
```

Request path: `Controller → service → DbContext`. Controllers stay thin — they validate (or rely on the global filter), call one service method, and map a `Result<T>` to `Ok`/`NotFound`/`Conflict`/`Forbidden`/`BadRequest` through `ControllerResultExtensions.ToActionResult`.

Layered by feature, not by technical type:

```
Controllers/<Domain>Controller.cs     HTTP surface, auth attributes, rate-limit attributes
Services/<Domain>/                    business rules, transactions, persistence, notifications
Models/Entities · Enums · Dtos        domain, persisted enums, transport contracts
Mappers/                              entity → DTO projections (registered scoped)
Validators/                           FluentValidation validators discovered from the assembly
Middleware/                           global exception + validation filters
Common/ Extensions/ Settings/         Result, phone/image helpers, query extensions, config POCOs
```

`global.cs` holds every `global using` so individual files stay import-free.

**Error convention.** Services return `Result<T>` with an `OperationError` and a message. Some messages are dotted keys (`"email.unconfirmed"`, `"carrier.not_found"`, `"password.current_incorrect"`) and some are human sentences — the frontend resolves the keys and falls back to the raw string.

---

## Project structure

```
Rivel/
├── Program.cs                  minimal host, middleware pipeline
├── DependencyInjection.cs      service registration, one method per concern
├── global.cs                   global usings
├── appsettings.json            config with empty secrets (supply via user-secrets/env)
├── compose.yaml                local PostgreSQL 17 on 127.0.0.1:5433
├── Dockerfile                  3-stage build, expects RiverLine.Api/ under the build context
├── dotnet-tools.json           dotnet-ef 10.0.12
│
├── Common/                     Result<T>, controller mapping, rate-limit policy names,
│                               phone normalisation, image signature sniffing, token codec
├── Configurations/             AppUrls — builds deep links into the frontend
├── Controllers/                10 controllers
├── Data/
│   ├── ApplicationDbContext.cs entity configuration, indexes, relationships
│   └── Seeds/NileBerthSeed.cs  48 berths seeded via HasData
├── Extensions/                 ClaimsPrincipal, matching (Bookable / Matching), email-confirmed
├── Hubs/                       NotificationHub
├── Mappers/                    entity → DTO
├── Middleware/                 GlobalExceptionHandler, ValidationExceptionHandler,
│                               FluentValidationFilter
├── Migrations/                 15 EF migrations (initial → carrier logo)
├── Models/
│   ├── Entities/               User, CarrierProfile, Vessel, CarrierRoute, NileBerth,
│   │                           ShipmentRequest, Offer, Shipment, Rating, Notification, RefreshToken
│   ├── Enums/                  roles, statuses, vessel types, berth types, notification types
│   └── Dtos/                   request/response contracts grouped per feature
├── Properties/launchSettings.json
├── Providers/                  Data Protection email-confirmation token provider (3-day lifespan),
│                               NameIdentifier SignalR user-id provider
├── Services/
│   ├── Auth/                   register, login, refresh rotation, logout, email confirmation,
│   │                           forgot/reset password
│   ├── CarrierRoutes/          route CRUD + suggested-request matching query
│   ├── Carriers/               public carrier profile aggregation
│   ├── Email/                  Resend client wrapper, template registry, renderer
│   ├── NileBerths/             berth lookup
│   ├── Notifications/          persistence + SignalR push + email fan-out, match worker/queue
│   ├── Offers/                 offer creation and the transactional accept flow
│   ├── Profiles/               profile read/update, password change, logo upload/delete
│   ├── Ratings/                rating creation, carrier rating rollup, distribution
│   ├── ShipmentRequests/       request creation and listing
│   ├── Shipments/              status transitions and queries
│   ├── Storage/                IFileStorage + Supabase implementation
│   └── Vessels/                vessel CRUD, status, archive
└── Settings/                   JwtSettings, EmailSettings, SupabaseStorageSettings
```

---

## Domain model

```
User (Identity, Guid)
 └─ CarrierProfile?          CompanyName, Bio, LogoPath, OverallRating, RatingCount
     ├─ Vessels              Name, Type, RegistrationNumber (unique), Capacity (t),
     │                       Status, YearBuilt, IsArchived
     └─ CarrierRoutes        Origin berth → Destination berth, IsActive

NileBerth                    Name/ArabicName, Governorate, Lat/Lng,
                             Type, Axis, CoordinateAccuracy

ShipmentRequest              CargoType, Weight, Origin→Destination, RequestedDate,
                             Status: Open → Matched → Closed

Offer                        Request, Carrier, Vessel, Price, ProposedPickupDate,
                             Status: Pending → Accepted | Rejected | Withdrawn

Shipment                     Request + Offer + Vessel,
                             Status: Matched → PickedUp → InTransit → Delivered,
                             IsRated, optional Rating

Notification                 UserId, Type, EntityId, DataJson (jsonb), IsRead, CreatedAt

RefreshToken                 [Owned] on User — Token, ExpireTime, RevokedAt
```

Relationships worth knowing: `ShipmentRequest → Shipment` and `Offer → Shipment` are one-to-one (delete restricted on the request side); `Shipment → Rating` is one-to-one. `Vessel.RegistrationNumber` is uniquely indexed, and notifications are indexed on `(UserId, IsRead, CreatedAt)` as `IX_Notifications_User_Unread`.

**Enums**

| Enum | Values |
|---|---|
| `UserRole` | `CargoOwner`, `Carrier` |
| `ShipmentRequestStatus` | `Open`, `Matched`, `Closed` |
| `OfferStatus` | `Pending`, `Accepted`, `Rejected`, `Withdrawn` |
| `ShipmentStatus` | `Matched`, `PickedUp`, `InTransit`, `Delivered` |
| `VesselStatus` | `Available`, `OnTrip`, `Maintenance` |
| `VesselType` | `Barge`, `SelfPropelledBarge`, `Tugboat`, `PushBoat`, `CargoVessel`, `BulkCarrier`, `ContainerBarge`, `TankBarge`, `RoRo` |
| `BerthType` | `Port`, `Terminal`, `Dock`, `Pier`, `LandingSite` |
| `NavigationAxis` | `CairoAswan`, `CairoDamietta`, `AswanWadiHalfa` |
| `NotificationType` | `OfferReceived`, `OfferAccepted`, `OfferRejected`, `OfferWithdrawn`, `RequestMatched`, `RequestExpired`, `ShipmentStatusChanged`, `ShipmentCancelled`, `RatingReceived` |

Enums serialise as strings (a `JsonStringEnumConverter` is registered globally).

---

## API reference

Swagger UI is served at `/swagger` in every environment. Full request/response schemas live there; this is the surface map.

### Auth — `/api/auth`

| Method | Route | Access | Notes |
|---|---|---|---|
| POST | `/register` | Anonymous | 5/15min per user+IP. Requires `CompanyName` when role is `Carrier`. Returns tokens immediately and sends a confirmation email. |
| POST | `/login` | Anonymous | Lockout-aware (`CheckPasswordSignInAsync`), 5 attempts then 5-minute lockout |
| POST | `/refresh` | Anonymous | Rotates the refresh token; the presented one is revoked |
| POST | `/confirm-email` | Anonymous | `{ userId, token }` from the emailed link |
| POST | `/resend-confirmation` | Authenticated | Rate limited; 5-minute in-memory cooldown per user |
| POST | `/forgot-password` | Anonymous | Always `204`, regardless of whether the email exists |
| POST | `/reset-password` | Anonymous | Also confirms the email, clears lockout, revokes all refresh tokens |
| POST | `/logout` | Anonymous | Revokes the presented refresh token, `204` either way |

### Shipment requests — `/api/shipment-requests` (authenticated)

| Method | Route | Access | Notes |
|---|---|---|---|
| POST | `/` | CargoOwner | Blocked until the email is confirmed. Queues a match notification for carriers |
| GET | `/open` | Carrier | All open requests |
| GET | `/mine` | CargoOwner | Requests belonging to the caller |
| GET | `/{id}` | Authenticated | Single request with offer count |

### Offers — `/api`

| Method | Route | Access | Notes |
|---|---|---|---|
| POST | `/shipment-requests/{id}/offers` | Carrier | Confirmed email required. One active offer per carrier per request; vessel must be owned, `Available`, non-archived, capacity ≥ cargo weight |
| GET | `/shipment-requests/{id}/offers` | CargoOwner | Sorted by price then pickup date; 404 unless you own the request |
| GET | `/offers/mine` | Carrier | Caller's offers ordered by pickup date |
| POST | `/offers/{id}/accept` | CargoOwner | Transactional — see the flow diagram |

### Shipments — `/api/shipments` (authenticated)

| Method | Route | Access | Notes |
|---|---|---|---|
| PATCH | `/{id}/status` | Carrier | Only the vessel owner; strictly the next status in sequence |
| GET | `/` | Authenticated | Shipments where you are owner or carrier. `?rated=true` / `?rated=false` filters (`false` = delivered and unrated) |
| GET | `/{id}` | Authenticated | Detail with nested rating |
| GET | `/{id}/rating` | Authenticated | The shipment's rating |

### Ratings — `/api/ratings`

| Method | Route | Access | Notes |
|---|---|---|---|
| POST | `/create` | CargoOwner | Only your own shipment, only after `Delivered`, only once. Score 1–5. Recomputes the carrier average |
| GET | `/{carrierId}` | Anonymous | Paged: overall average, count, 1–5 distribution, and recent reviews with first name only |

### Carrier routes and suggestions — `/api/routes` (Carrier)

| Method | Route | Notes |
|---|---|---|
| GET | `/` | Caller's routes |
| POST | `/` | Origin ≠ destination, both berths must exist, no duplicates |
| DELETE | `/{id}` | Own routes only |
| GET | `/suggested-requests` | Open requests matching the caller's active routes, excluding ones already offered on. Query: `routeId`, `fittingOnly` (≤ largest bookable capacity), `sort` = `Newest` \| `PickupSoonest` \| `WeightAsc` \| `WeightDesc`, `page`, `pageSize` (max 50). Returns per-route counts as filter chips, `isNew` for requests under 24h, lowest pending offer price, and how many vessels can carry the weight |

### Vessels — `/api/vessels` (Carrier)

| Method | Route | Notes |
|---|---|---|
| GET | `/mine` | Non-archived vessels with current assignment and pending offer count |
| POST | `/` | Registration number must be unique; capacity ≤ 20,000 t |
| PUT | `/{id}` | Own vessels only. Shrinking capacity below the heaviest active shipment or pending offer is rejected |
| PATCH | `/{id}/status` | Only `Available` or `Maintenance`. `Maintenance` withdraws pending offers; `OnTrip` vessels are locked |
| DELETE | `/{id}` | Archives. Rejected while the vessel has an active shipment; withdraws pending offers |

### Profile — `/api/profile` (authenticated)

| Method | Route | Notes |
|---|---|---|
| GET | `/me` | Profile with resolved public logo URL |
| PUT | `/me` | Name, phone (normalised, resets confirmation on change), and company/bio for carriers |
| POST | `/change-password` | Revokes all active refresh tokens |
| POST | `/logo` | Carrier only, multipart, 5 MB request limit / 2 MB file limit. Magic-byte check for PNG, JPEG, WebP |
| DELETE | `/logo` | Carrier only. Deletes the stored object |

### Carriers, berths, notifications

| Method | Route | Access | Notes |
|---|---|---|---|
| GET | `/api/carriers/{userId}` | Anonymous | Public profile: company, bio, rating, completed shipments, vessel count, active routes, logo. 60 req/min per IP |
| GET | `/api/berths` | Anonymous | Active berths ordered by axis then latitude |
| GET | `/api/notifications` | Authenticated | Paged, `unreadOnly`, returns unread count and `hasMore` |
| GET | `/api/notifications/unread-count` | Authenticated | |
| POST | `/api/notifications/{id}/read` | Authenticated | |
| POST | `/api/notifications/read-all` | Authenticated | |

### Realtime

`GET /hubs/notifications` — SignalR. Authorised via the `access_token` query parameter; the user identifier is resolved from the `NameIdentifier` claim. The server sends a `notification` event with the `NotificationDto` payload.

---

## Cross-cutting concerns

**Authentication.** Access tokens are HS256 JWTs carrying `NameIdentifier`, `Role`, and `Email`, valid for 30 minutes by default. Refresh tokens are 32 random bytes (base64), valid 7 days, stored as an owned collection on the user, and rotated on every refresh. Stale inactive tokens older than 7 days are pruned on issue. Email-confirmation tokens use a Data Protection provider with a 3-day lifespan; reset tokens use the default provider (2 hours). Data Protection keys are persisted in the database under the application name `RiverLine`, so tokens survive restarts and stay valid across instances.

**Concurrency.** `POST /offers/{id}/accept` runs inside a transaction and takes `SELECT … FOR UPDATE` row locks on the offer, the shipment request, and the vessel before validating state. This makes double-accept and accept-vs-withdraw races resolve deterministically.

**Validation.** A global `FluentValidationFilter` resolves `IValidator<T>` for every action argument and throws on failure, so endpoints do not need per-action validation code. `ValidationExceptionHandler` renders errors grouped by property (lowercased) into a ProblemDetails `errors` extension. Password rules live in one place (`PasswordRules.StrongPassword`) to stay in sync with Identity's options and the frontend's `password.validator.ts`.

**Rate limiting.** `public` = 60 requests/minute per IP. `sensitive` = 5 requests per 15 minutes per user (falling back to IP), applied to register, resend confirmation, forgot/reset password, change password, and logo upload. Rejections return `429` with a plain-text `rate_limited` body.

**Notifications.** `INotificationService.CreateManyAsync` persists a row per recipient, pushes over SignalR, then fans out email where the notification type has a template mapping. Each channel is individually try/caught so a failing email never rolls back the write or blocks the push. Payload lives in a `jsonb` column so the frontend can switch on `type` and read `data.actionUrl` for deep linking.

**Match notifications.** Creating a request writes its ID to a bounded channel (1000, drop-write) drained by `MatchNotificationWorker`. The worker finds carriers with an active route on the exact origin→destination pair *and* at least one bookable vessel with capacity ≥ cargo weight, then notifies each once. It runs outside the request path, so posting a request stays fast.

**Email.** Templates are Arabic and RTL with an inline HTML email layout. `EmailRenderer` fills `{{placeholders}}`, HTML-encodes every value, formats dates with `ar-EG` culture and long-form month names, thousands-separates numbers, and derives a plain-text alternative by stripping tags. Sending is wrapped so a Resend outage cannot fail an API call — errors are logged instead.

**File uploads.** Logos go to Supabase Storage under `carriers/{userId}/{guid}.{ext}`. The content type comes from magic-byte detection rather than the client-declared type, uploads are rolled back if the database write fails, and the previous object is deleted after a successful swap.

**Response conventions.** `ReturnHttpNotAcceptable` is on, so clients get `406` rather than a coerced format. JSON and XML formatters are both enabled. Payload DTOs are records with positional constructors, and many list queries project directly into DTOs in the database rather than materialising entities.

---

## Configuration

`appsettings.json` ships with empty secrets — supply them via user-secrets, environment variables, or a local override.

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=rivel;Username=..."
dotnet user-secrets set "Jwt:Key" "<at-least-32-bytes>"
dotnet user-secrets set "Resend:ApiKey" "re_..."
dotnet user-secrets set "Resend:FromEmail" "noreply@yourdomain.com"
dotnet user-secrets set "Supabase:Url" "https://<project>.supabase.co"
dotnet user-secrets set "Supabase:ServiceKey" "..."
dotnet user-secrets set "App:BaseUrl" "http://localhost:4200"
```

| Key | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | Npgsql connection string |
| `Jwt:Key` / `Issuer` / `Audience` | Signing key and token validation |
| `Jwt:DurationInMinutes` / `ExpiryDays` | Access token 30 min, refresh token 7 days |
| `Cors:AllowedOrigins` | Array of allowed frontend origins, e.g. `http://localhost:4200` |
| `Resend:ApiKey` / `FromName` / `FromEmail` | Email provider |
| `Supabase:Url` / `ServiceKey` / `Bucket` | Storage; `Url` is validated and required at startup |
| `App:BaseUrl` | Frontend base URL for deep links; validated as absolute http/https at startup |
| `ApiSettings:BaseUrl` | Swagger "Try it out" server URL (defaults to `http://localhost:5132`) |

Supabase and App settings use `ValidateOnStart`, so the app fails fast at boot with a clear message rather than on the first request.

---

## Local development

Prerequisites: .NET 10 SDK, Docker, and the `dotnet-ef` tool (restored automatically from `dotnet-tools.json`).

**1. Database**

```bash
docker compose up -d db          # PostgreSQL 17 on 127.0.0.1:5433
```

`compose.yaml` reads `POSTGRES_DB` and `POSTGRES_USER` from a `.env` file next to it.

**2. Secrets** — set the keys from the table above. At minimum you need the connection string, `Jwt:Key`, `Resend:*`, `Supabase:*`, and `App:BaseUrl`, because Supabase and App options validate at startup.

**3. Migrations**

```bash
dotnet tool restore
dotnet ef database update
```

The 48 berths are seeded by `NileBerthSeed` through `HasData`, so they land with the migration — no manual data entry. To add a migration after changing entities:

```bash
dotnet ef migrations add <Name>
```

**4. Run**

```bash
dotnet run
```

The API listens on `http://localhost:5132` (https profile adds `https://localhost:7238`). Swagger UI is at `/swagger`, and is served in every environment.

**Frontend** (separate repository) proxies `/api` to `localhost:5132`:

```bash
cd ../Rivel_Fr
npm install
npm start          # ng serve on http://localhost:4200
```

---

## Deployment

Two paths, both live:

- **Azure** — published to `rivel.runasp.net`; publish profiles are referenced in `RiverLine.Api.csproj` (`Properties/PublishProfiles/rivel.runasp.net-WebDeploy`).
- **Docker** — the multi-stage `Dockerfile` builds on `mcr.microsoft.com/dotnet/sdk:10.0` and runs on `aspnet:10.0` as a non-root user, exposing 8080/8081. Note the build context is the **parent** directory, since the Dockerfile copies `RiverLine.Api/RiverLine.Api.csproj` and expects the project folder inside it.

Secrets in production come from the platform's configuration, not from `appsettings.json`. JWT and Data Protection keys must be identical across instances for tokens to survive a scale-out.

---

## Roadmap

| Phase | Status |
|---|---|
| v1 — Core marketplace (auth, requests, offers, shipments, ratings, vessels) | ✅ Done |
| v1.1 — Production deployment (Azure + Docker), email confirmation, password reset | ✅ Done |
| v1.2 — Carrier routes, auto-suggest matching, public carrier profiles, logo upload | ✅ Done |
| v1.3 — Notifications: persistence, SignalR push, match worker, email fan-out | ✅ Done |
| v2 — In-app messaging, pricing history, cancellations, fleet utilisation analytics | 🔜 Planned |