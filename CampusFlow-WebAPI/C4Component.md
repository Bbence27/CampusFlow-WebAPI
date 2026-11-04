C4Component
    title C4 Component Diagram for CampusFlow (11-Project Modular Architecture)

    Person(client, "API Client", "Web client, mobile app, or Swagger UI")

    Enterprise_Boundary(b0, "CampusFlow Solution (11 Projects)") {
        Component(webApi, "CampusFlow.WebAPI", "Host / Program.cs", "Composition root. Bootstraps the application and wires dependency injection.")
        Component(endpoints, "CampusFlow.Endpoints", "FastEndpoints / Routing", "Defines HTTP endpoints and maps requests to application services.")
        Component(servicesEnrollment, "CampusFlow.Services.Enrollment", "Business Logic", "Orchestrates enrollment workflows, transactions, and rule execution.")
        Component(rulesEnrollment, "CampusFlow.Rules.Enrollment", "Business Rules", "Encapsulates pure validation logic (capacity, course limits, conflicts).")
        Component(resourceAccess, "CampusFlow.ResourceAccess.Sqlite", "Data Access", "Manages Entity Framework Core DbContext and SQLite persistence.")
        Component(contractsEnrollments, "CampusFlow.Contracts.Enrollments", "Contracts", "DTOs, requests, responses, and errors for enrollments.")
        Component(contractsStudents, "CampusFlow.Contracts.Students", "Contracts", "DTOs and request/response payloads for students.")
        Component(contractsCourses, "CampusFlow.Contracts.Courses", "Contracts", "DTOs and request/response payloads for courses.")
        Component(contractsStatistics, "CampusFlow.Contracts.Statistics", "Contracts", "DTOs and request/response payloads for system metrics.")
        Component(domainModels, "CampusFlow.Domain.Models", "Core Domain", "Enterprise domain entities and database models.")
        Component(utilLogging, "CampusFlow.Util.Logging", "Infrastructure", "Provides custom logging extensions, file writers, and sinks.")
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