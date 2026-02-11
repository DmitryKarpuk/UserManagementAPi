# User Management API
**Capstone Project — Coursera / Microsoft**

**Back-End Development with .NET Specialization**

This repository contains a **User Management Web API** built using ASP.NET Core Minimal API.
It serves as the capstone project for the Coursera program:

👉 ***Microsoft — Back-End Development with .NET***

The project demonstrates core backend development skills such as routing, data processing, middleware pipelines, authentication, request validation, logging, and error handling.

## 🚀 Features Overview
✔ User CRUD Endpoints
- GET /users — retrieve all users
- GET /users/{id} — retrieve user by ID
- POST /users — create new user
- PUT /users/{id} — update user
- DELETE /users/{id} — delete user

All user data is stored in an in-memory ConcurrentDictionary.

## 🧠 Custom Middleware Pipeline
This project includes three fully custom middlewares, implemented manually to demonstrate deep understanding of ASP.NET Core’s pipeline mechanics.

🔹 **1. ErrorHandlingMiddleware**

Global exception handler that:

- Captures all unhandled exceptions
- Logs detailed error information
- Returns a clean JSON response:

```JSON
{ "error": "Internal server error." }
```

This prevents API crashes and keeps clients safe from internal implementation details.

🔹 **2. LoggingMiddleware**

Logs each HTTP request with:

- HTTP method
- Request path
- Response status code

Example log entry:
```Shell
HTTP request GET /users, response status: 200
```


🔹 **3. TokenValidationMiddleware**

Implements a simple authentication layer that:

- Reads the Authorization: Bearer <token> header
- Validates tokens through ITokenValidator
- Rejects missing/invalid tokens with:

```JSON
{ "error": "Invalid or expired token." }
```

Allows processing to continue only if the token is valid

This adds lightweight security and demonstrates middleware-based authentication.

## 🔒 Token Validator Service
The project includes a custom ITokenValidator interface and a simple implementation (SimpleTokenValidator).
The service is registered via Dependency Injection and used by the authentication middleware.
You can later replace this with:

## 📘 Swagger Integration
Swagger UI is enabled and automatically generated via:

- AddEndpointsApiExplorer
- AddSwaggerGen

After running the application, navigate to:

`/swagger`

This interactive documentation allows testing all endpoints directly in the browser.

## 🛠 Technology Stack

- **ASP.NET Core 8 Minimal API**
- **C#**
- **Thread-safe collections (ConcurrentDictionary)**
- **Custom Middleware**
- **Dependency Injection**
- **Swagger (Swashbuckle)**


## 📦 Project Highlights

- Clean and simple architecture
- Modular middleware building
- Stateless authentication design
- Production-like error handling
- Structured JSON responses
- Clear separation of responsibilities


## ▶️ Running the Project
From the project directory:
```Shell
dotnet run
```
Then open:

Swagger UI → http://localhost:<port>/swagger

Main endpoint → http://localhost:<port>/