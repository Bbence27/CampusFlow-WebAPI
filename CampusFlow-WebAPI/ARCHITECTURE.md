# Architecture Overview: CampusFlow Modular API

This document details the architectural decisions, structural breakdown, encapsulated volatilities, and trade-offs (pros and cons) for each of the 11 projects comprising the **CampusFlow-WebAPI** system.

------------------------------------------------------------------------

## 1. Architectural Rationale & Volatility Encapsulation

The system was refactored from a monolithic application into a modular architecture to isolate change. In software design, *volatility* refers to the likelihood and reasons why a part of a system might change. By separating code based on volatility, changes to database providers, business logic, rules, or web frameworks are localized and do not ripple through the entire codebase.

------------------------------------------------------------------------

## 2. Detailed Module Breakdown (The 11 Projects)

### 1. `CampusFlow.Domain.Models`

- **Why it was created:** To serve as the foundational anchor for core enterprise entities and database records (e.g., Student, Course, Enrollment).
- **Volatility Encapsulated:** Core business entity schemas and relational data structures.
- **Pros:**
  - Establishes a single source of truth for enterprise domain concepts.
  - Completely independent of external frameworks, databases, or UI.
- **Cons:**
  - Requires mapping layers when transforming database entities to external DTOs.

### 2. `CampusFlow.Contracts.Enrollments`

- **Why it was created:** To encapsulate data transfer objects (DTOs), requests, responses, errors, and enums specifically for student enrollments.
- **Volatility Encapsulated:** External API contracts and data shapes for enrollment operations.
- **Pros:**
  - Prevents tight coupling between API request payloads and internal database schemas.
  - Allows API data formats to evolve independently of storage models.
- **Cons:**
  - Adds boilerplate code via data duplication across different project contracts.

### 3. `CampusFlow.Contracts.Students`

- **Why it was created:** To define data contracts and payloads specific to student management features.
- **Volatility Encapsulated:** Student-specific data transfer interfaces.
- **Pros:**
  - Highly targeted contracts make student endpoints self-contained and easy to modify.
- **Cons:**
  - Proliferation of multiple small projects/assemblies to manage.

### 4. `CampusFlow.Contracts.Courses`

- **Why it was created:** To manage data contracts, requests, and responses for course management.
- **Volatility Encapsulated:** Course-specific interface contracts and schemas.
- **Pros:**
  - Isolate course data definitions so changes don't affect other domains.
- **Cons:**
  - Increases project navigation complexity within the solution workspace.

### 5. `CampusFlow.Contracts.Statistics`

- **Why it was created:** To handle specialized reporting and analytics contracts.
- **Volatility Encapsulated:** Metrics, reporting payloads, and statistical data schemas.
- **Pros:**
  - Decouples reporting output formats from core CRUD operations.
- **Cons:**
  - Fragmented contract definitions require referencing multiple minor projects.

### 6. `CampusFlow.Rules.Enrollment`

- **Why it was created:** To encapsulate pure academic and business validation logic (e.g., capacity checks, max course limits, duplicate enrollment rules).
- **Volatility Encapsulated:** Business policy changes and academic constraints.
- **Pros:**
  - Rules are pure, decoupled functions that can be unit-tested in complete isolation without a database or web server.
  - Easy to update business logic when university policies change.
- **Cons:**
  - Introduces indirection, requiring services to coordinate multiple discrete rule objects.

### 7. `CampusFlow.Services.Enrollment`

- **Why it was created:** To orchestrate core workflows, combining business rules, data access, and transaction management for enrollments.
- **Volatility Encapsulated:** Application workflow and orchestration logic.
- **Pros:**
  - Centralizes use-case execution flow into dedicated service classes.
- **Cons:**
  - Risk of application logic scattering if boundaries between rules and services blur.

### 8. `CampusFlow.ResourceAccess.Sqlite`

- **Why it was created:** To manage Entity Framework Core configurations, `CampusFlowDbContext`, and SQLite persistence.
- **Volatility Encapsulated:** Database vendor technology, schema migrations, and persistence mechanisms.
- **Pros:**
  - Swapping or upgrading databases (e.g., from SQLite to PostgreSQL) can be done with minimal changes outside this project.
- **Cons:**
  - Tightly binds data access to EF Core concepts within this specific boundary.

### 9. `CampusFlow.Endpoints`

- **Why it was created:** To define API routing handlers (FastEndpoints / controllers) mapping incoming HTTP requests to underlying services.
- **Volatility Encapsulated:** HTTP protocol handling, routing paths, and status code mapping.
- **Pros:**
  - Keeps routing and presentation concerns separate from business logic.
- **Cons:**
  - Requires explicit mapping code between HTTP requests, DTOs, and domain services.

### 10. `CampusFlow.Util.Logging`

- **Why it was created:** To supply custom logging extensions, file writers, sinks, and log formatting options across the application.
- **Volatility Encapsulated:** Cross-cutting logging infrastructure and formatting policies.
- **Pros:**
  - Reusable logging utility across all solution layers without duplicating code.
- **Cons:**
  - Introduces custom infrastructure code that must be maintained alongside built-in framework logging.

### 11. `CampusFlow.WebAPI`

- **Why it was created:** To act as the sole composition root, bootstrapping the application, wiring dependency injection containers, and hosting the web server.
- **Volatility Encapsulated:** Hosting framework, application startup configuration, and middleware pipeline wiring.
- **Pros:**
  - Centralizes runtime configuration and DI setup.
- **Cons:**
  - Serves as a point of high coupling where all modules converge, requiring strict adherence to interfaces to prevent leakage.

------------------------------------------------------------------------

## 3. Dependency Flow & Architecture Summary

- **Inward Flow:** Dependencies flow strictly from outer projects (`WebAPI`, `Endpoints`) inward toward core business rules, services, contracts, and domain models.
- **Zero Circular Dependencies:** Verified via clean, zero-error compilation passes across all 11 projects in .NET 10.0.

