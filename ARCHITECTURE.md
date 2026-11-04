# Architecture Overview: CampusFlow Modular API

This document explains the architectural decisions, module breakdown, volatilities, and trade-offs (pros and cons) for the 11 projects making up the **CampusFlow-WebAPI** system.

------------------------------------------------------------------------

## 1. Architectural Approach & Volatility

We refactored the application from a monolith into 11 separate projects to isolate **volatility**—meaning we separated parts of the code based on *why* and *how often* they change. For example, changing the database or updating a business rule no longer requires changing the web server or API routing code.

------------------------------------------------------------------------

## 2. Module Breakdown, Volatility, and Pros & Cons

### 1. `CampusFlow.Domain.Models`

- **Why it was created:** To hold the core database entities and enterprise data structures (like Student, Course, and Enrollment).
- **Volatility:** Core enterprise data shapes and database entity fields.
- **Pros:**
  - Acts as a single source of truth for domain concepts.
  - Completely independent of web frameworks or databases.
- **Cons:**
  - Requires mapping code to convert database models into API response objects.

### 2. `CampusFlow.Contracts.Enrollments`

- **Why it was created:** To define data transfer objects (DTOs), requests, responses, and errors for enrollments.
- **Volatility:** External API request/response structures.
- **Pros:**
  - Keeps API endpoints decoupled from internal database models.
  - Allows API data formats to change safely without breaking database tables.
- **Cons:**
  - Adds extra code files for managing separate data transfer objects.

### 3. `CampusFlow.Contracts.Students`

- **Why it was created:** To house data contracts and payloads specific to student management.
- **Volatility:** Student-specific API input and output formats.
- **Pros:**
  - Keeps student-related data shapes clean and self-contained.
- **Cons:**
  - Results in a higher number of small contract projects in the solution.

### 4. `CampusFlow.Contracts.Courses`

- **Why it was created:** To manage data contracts, requests, and responses for courses.
- **Volatility:** Course-specific interface contracts and schemas.
- **Pros:**
  - Isolates course data definitions so they don't impact other features.
- **Cons:**
  - Requires navigating across multiple small contract projects.

### 5. `CampusFlow.Contracts.Statistics`

- **Why it was created:** To handle specialized reporting and analytics contracts.
- **Volatility:** Metrics, reporting payloads, and statistical data formats.
- **Pros:**
  - Decouples reporting outputs from standard database records.
- **Cons:**
  - Fragmented contract definitions spread across multiple minor projects.

### 6. `CampusFlow.Rules.Enrollment`

- **Why it was created:** To encapsulate academic and business validation logic (like capacity checks, duplicate enrollments, and student limits).
- **Volatility:** Business policies and university rules.
- **Pros:**
  - Rules are pure code units that can be tested easily without a database or web server.
  - Easy to update when university policies change.
- **Cons:**
  - Adds layers of indirection between services and validation logic.

### 7. `CampusFlow.Services.Enrollment`

- **Why it was created:** To coordinate core application workflows, combining rules, data access, and transactions for enrollments.
- **Volatility:** Application use-cases and process workflows.
- **Pros:**
  - Centralizes use-case logic into dedicated service classes.
- **Cons:**
  - Risk of application logic blurring if boundaries between rules and services aren't maintained.

### 8. `CampusFlow.ResourceAccess.Sqlite`

- **Why it was created:** To manage Entity Framework Core, database contexts, and SQLite persistence.
- **Volatility:** Database technology, connection strings, and data storage mechanisms.
- **Pros:**
  - Swapping to a different database (like PostgreSQL or SQL Server) only requires changes inside this module.
- **Cons:**
  - Tightly couples data storage logic to Entity Framework Core tools.

### 9. `CampusFlow.Endpoints`

- **Why it was created:** To define API routing and map incoming HTTP requests to underlying business services.
- **Volatility:** HTTP transport protocols, routing paths, and web endpoints.
- **Pros:**
  - Keeps web routing code completely separate from core business logic.
- **Cons:**
  - Requires explicit mapping code to translate web requests into service calls.

### 10. `CampusFlow.Util.Logging`

- **Why it was created:** To supply custom logging extensions, file writers, and formatting options across the application.
- **Volatility:** Cross-cutting logging infrastructure and output formatting.
- **Pros:**
  - Reusable logging logic that can be shared across all modules without code duplication.
- **Cons:**
  - Introduces custom logging code that must be maintained alongside built-in framework features.

### 11. `CampusFlow.WebAPI`

- **Why it was created:** To act as the composition root, bootstrapping the application, configuring dependency injection, and hosting the web server.
- **Volatility:** Hosting framework and application startup configurations.
- **Pros:**
  - Centralizes runtime setup and dependency wiring in one place.
- **Cons:**
  - Serves as a high-coupling convergence point where all modules come together.

------------------------------------------------------------------------

## 3. C4 Component Diagram

The diagram below illustrates how the 11 modular projects and the database interact with each other at runtime:

\`\`\`mermaid C4Component title C4 Component Diagram for CampusFlow (11-Project Architecture)

```         
Person(client, "API Client", "Web client, mobile app, or Swagger UI")

Enterprise_Boundary(b0, "CampusFlow Solution (11 Projects)") {
    Component(webApi, "CampusFlow.WebAPI", "Host / Program.cs", "Composition root. Bootstraps the application and wires DI.")
    Component(endpoints, "CampusFlow.Endpoints", "FastEndpoints / Routing", "Defines HTTP endpoints and maps requests to services.")
    Component(servicesEnrollment, "CampusFlow.Services.Enrollment", "Business Logic", "Orchestrates enrollment workflows and rule execution.")
    Component(rulesEnrollment, "CampusFlow.Rules.Enrollment", "Business Rules", "Encapsulates validation logic (capacity, course limits).")
    Component(resourceAccess, "CampusFlow.ResourceAccess.Sqlite", "Data Access", "Manages EF Core DbContext and SQLite persistence.")
    Component(contractsEnrollments, "CampusFlow.Contracts.Enrollments", "Contracts", "DTOs, requests, and responses for enrollments.")
    Component(contractsStudents, "CampusFlow.Contracts.Students", "Contracts", "DTOs and request payloads for students.")
    Component(contractsCourses, "CampusFlow.Contracts.Courses", "Contracts", "DTOs and request payloads for courses.")
    Component(contractsStatistics, "CampusFlow.Contracts.Statistics", "Contracts", "DTOs and request payloads for system metrics.")
    Component(domainModels, "CampusFlow.Domain.Models", "Core Domain", "Enterprise domain entities and database models.")
    Component(utilLogging, "CampusFlow.Util.Logging", "Infrastructure", "Provides custom logging extensions and file writers.")
}

System_Db(sqliteDb, "SQLite Database", "Relational database persisting entity state via EF Core.")

Rel(client, webApi, "Sends HTTP Requests", "HTTPS/JSON")
Rel(webApi, endpoints, "Dispatches traffic")
Rel(endpoints, servicesEnrollment, "Invokes service methods")
Rel(servicesEnrollment, rulesEnrollment, "Evaluates rules")
Rel(servicesEnrollment, resourceAccess, "Queries / Persists data")
Rel(resourceAccess, sqliteDb, "Reads/Writes SQL", "EF Core")

Rel(endpoints, contractsEnrollments, "Uses DTOs")
Rel(endpoints, contractsStudents, "Uses DTOs")
Rel(endpoints, contractsCourses, "Uses DTOs")
Rel(endpoints, contractsStatistics, "Uses DTOs")
Rel(servicesEnrollment, contractsEnrollments, "Returns contracts")
Rel(resourceAccess, domainModels, "Uses entities")
Rel(webApi, utilLogging, "Configures logging")

UpdateLayoutConfig($c4ShapeInRow="4", $c4BoundaryInRow="1")
```
