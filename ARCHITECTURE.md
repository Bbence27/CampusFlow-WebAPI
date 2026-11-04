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

```mermaid
flowchart TD
    classDef client fill:#f3f4f6,stroke:#374151,stroke-width:2px,color:#111827;
    classDef project fill:#eff6ff,stroke:#1d4ed8,stroke-width:2px,color:#1e3a8a;
    classDef db fill:#fef3c7,stroke:#d97706,stroke-width:2px,color:#78350f;

    Client["API Client<br/>(Web / Mobile / Swagger)"] --> WebAPI["CampusFlow.WebAPI<br/>(Host / DI Root)"]

    subgraph Solution ["CampusFlow Solution (11 Projects)"]
        WebAPI --> Endpoints["CampusFlow.Endpoints<br/>(Routing & Handlers)"]
        
        Endpoints --> ServicesEnrollment["CampusFlow.Services.Enrollment<br/>(Business Logic)"]
        
        ServicesEnrollment --> RulesEnrollment["CampusFlow.Rules.Enrollment<br/>(Business Rules)"]
        ServicesEnrollment --> ResourceAccess["CampusFlow.ResourceAccess.Sqlite<br/>(EF Core Data Access)"]
        
        Endpoints --> ContractsEnrollments["CampusFlow.Contracts.Enrollments"]
        Endpoints --> ContractsStudents["CampusFlow.Contracts.Students"]
        Endpoints --> ContractsCourses["CampusFlow.Contracts.Courses"]
        Endpoints --> ContractsStatistics["CampusFlow.Contracts.Statistics"]
        
        ServicesEnrollment -.-> ContractsEnrollments
        ResourceAccess --> DomainModels["CampusFlow.Domain.Models<br/>(Core Domain Entities)"]
        WebAPI --> UtilLogging["CampusFlow.Util.Logging<br/>(Infrastructure Logging)"]
    end

    ResourceAccess --> SQLite["SQLite Database"]

    class Client client;
    class WebAPI,Endpoints,ServicesEnrollment,RulesEnrollment,ResourceAccess,ContractsEnrollments,ContractsStudents,ContractsCourses,ContractsStatistics,DomainModels,UtilLogging project;
    class SQLite db;
