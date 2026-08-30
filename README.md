# Wexa Graph Explorer

Wexa Graph Explorer is a web application built for the **Wexa AI CognoDB take-home assignment**.

The application models an engineering organization as a graph containing developers, projects, skills, technologies, and companies. It provides a simple interface for exploring relationships within the engineering ecosystem.

The application has two primary graph-powered features:

* **Talent Finder** — Finds developers whose skills match the requirements of a selected project while excluding developers who have already worked on that project.
* **Project Dependencies** — Finds relationships between projects through shared developers and shared skills.

The backend is implemented using **ASP.NET Core 8** and the official **Neo4j .NET Driver**. The frontend is implemented using **Angular 20**. **CognoDB Cloud** is used as the graph database and communicates with the backend through the **Bolt protocol**.

---

## 1. Live Application

### Frontend

**Wexa Graph Explorer**

https://wexa-graph-web.onrender.com

### Backend API

**Wexa Graph Explorer API**

https://wexa-graph-api.onrender.com

### API Health Check

https://wexa-graph-api.onrender.com/api/graph/health

The production health endpoint verifies that the API is running and successfully connected to CognoDB.

Example response:

```json
{
  "status": "Healthy",
  "message": "Successfully connected to CognoDB over Bolt."
}
```

---

## 2. Demo Video

A short walkthrough of the completed application is included in the repository.

[Watch the Wexa Graph Explorer Demo](docs/demo/Wexa-Graph-Explorer-Demo.mp4)

The demonstration covers the dashboard, Talent Finder, Project Dependencies, and backend API functionality.

---

## 3. Application Overview

The application represents an engineering organization using a graph.

A developer can:

* Have multiple skills
* Work for a company
* Work on multiple projects

A project can:

* Require several skills
* Use different technologies

The graph allows the application to answer relationship-oriented questions by traversing connected nodes instead of treating each entity as an isolated record.

For example:

```text
Developer
    |
    | HAS_SKILL
    v
  Skill
```

A developer can also be connected to projects:

```text
Developer
    |
    | WORKED_ON
    v
  Project
```

Projects can be connected to skills and technologies:

```text
Project
   |                     |
   | USES_SKILL         | USES_TECHNOLOGY
   v                     v
 Skill               Technology
```

---

## 4. Why a Graph Database?

The main reason for using a graph database is that the important information in this application comes from the **relationships between entities**.

For example, Talent Finder needs to:

1. Start with a project.
2. Find the skills required by that project.
3. Find developers who have those skills.
4. Check the projects those developers have previously worked on.
5. Exclude developers who already worked on the selected project.

This relationship can be represented as:

```text
Project
   |
   | USES_SKILL
   v
 Skill
   ^
   | HAS_SKILL
   |
Developer
   |
   | WORKED_ON
   v
Project History
```

This type of traversal can also be implemented using a relational database, but it would require multiple tables and joins between projects, developers, skills, and project history.

The Project Dependencies feature provides another graph-oriented use case. It finds projects connected through:

* Shared developers
* Shared skills

These relationships are naturally represented as nodes and edges in a graph.

For this application, the graph model makes the relationships explicit and allows the required Cypher queries to remain focused on graph traversal.

---

## 5. Technology Stack

### Backend

* ASP.NET Core 8
* C#
* Official Neo4j .NET Driver
* OpenCypher
* Swagger / OpenAPI

### Frontend

* Angular 20
* TypeScript
* SCSS
* Angular Router
* Angular HTTP Client

### Database

* CognoDB Cloud
* Bolt protocol
* OpenCypher

### Deployment

* Render
* GitHub

---

## 6. Architecture

The application follows a simple layered architecture:

```text
                    Angular 20
                       |
                       | HTTP
                       v
              ASP.NET Core 8 API
                       |
                       v
            Application / Graph Logic
                       |
                       v
              Infrastructure Layer
                       |
                       | Neo4j .NET Driver
                       | Bolt
                       v
                  CognoDB Cloud
```

The responsibilities are separated as follows:

### API

Responsible for:

* HTTP endpoints
* CORS configuration
* Swagger/OpenAPI
* Request handling

### Application

Responsible for:

* Graph application logic
* DTOs
* Repository abstraction
* Talent Finder logic
* Project Dependencies logic

### Infrastructure

Responsible for:

* CognoDB configuration
* Neo4j driver
* Database connection
* Repository implementation
* Database seeding

### Angular

Responsible for:

* Dashboard
* Talent Finder UI
* Project Dependencies UI
* API communication
* Loading states
* Error states

---

## 7. Project Structure

```text
Assessment
│
├── docs
│   ├── demo
│   │   └── Wexa-Graph-Explorer-Demo.mp4
│   │
│   └── screenshots
│       ├── dashboard.png
│       ├── project-dependencies.png
│       ├── talent-finder.png
│       ├── swagger-dependencies.png
│       ├── swagger-dependencies-response.png
│       ├── swagger-health.png
│       ├── swagger-summary.png
│       └── README_SCREENSHOTS_SECTION.md
│
├── scripts
│   ├── seed.cypher
│   └── queries.cypher
│
├── WexaGraphExplorer.Api
│   ├── Endpoints
│   │   └── GraphExplorerEndpoints.cs
│   ├── Properties
│   │   └── launchSettings.json
│   └── Program.cs
│
├── WexaGraphExplorer.Application
│   └── Graph
│       ├── GraphDtos.cs
│       ├── GraphExplorerService.cs
│       └── IGraphExplorerRepository.cs
│
├── WexaGraphExplorer.Domain
│
├── WexaGraphExplorer.Infrastructure
│   ├── CognoDb
│   │   ├── CognoDbConnectionTest.cs
│   │   └── CognoDbDriverFactory.cs
│   │
│   ├── Configuration
│   │   └── CognoDbSettings.cs
│   │
│   ├── Graph
│   │   └── CognoDbGraphExplorerRepository.cs
│   │
│   └── Seeding
│       └── CognoDbSeeder.cs
│
├── WexaGraphExplorer.Web
│
├── WexaGraphExplorer.slnx
└── README.md
```

---

## 8. Graph Data Model

The graph contains five types of nodes.

### Company

Represents an organization where a developer has worked.

Properties:

```text
name
location
industry
```

### Developer

Represents a developer in the engineering organization.

Properties:

```text
name
email
experienceYears
location
```

### Project

Represents an engineering project.

Properties:

```text
name
description
```

### Skill

Represents a technical skill that a developer can have or a project can require.

Properties:

```text
name
category
```

### Technology

Represents a technology used by a project.

Properties:

```text
name
category
```

---

## 9. Graph Relationships

The graph uses the following relationships:

```text
Developer ── HAS_SKILL ──> Skill

Developer ── WORKED_AT ──> Company

Developer ── WORKED_ON ──> Project

Project ── USES_SKILL ──> Skill

Project ── USES_TECHNOLOGY ──> Technology
```

A simplified representation is:

```text
                    Company
                       ^
                       |
                   WORKED_AT
                       |
                       |
Developer ── HAS_SKILL ──> Skill
   |
   |
WORKED_ON
   |
   v
Project ── USES_SKILL ──> Skill
   |
   |
   └── USES_TECHNOLOGY ──> Technology
```

---

## 10. Seed Data

The initial graph data is stored in:

```text
scripts/seed.cypher
```

The seed script creates:

* Companies
* Developers
* Projects
* Skills
* Technologies
* Relationships between these entities

The current graph contains:

| Type                    |  Count |
| ----------------------- | -----: |
| Companies               |      3 |
| Developers              |      4 |
| Projects                |      4 |
| Skills                  |      8 |
| Technologies            |      5 |
| **Total Nodes**         | **24** |
| **Total Relationships** | **57** |

The seed process runs when the API starts successfully and the required CognoDB environment variables are configured.

---

## 11. Main Features

### 11.1 Dashboard

The dashboard displays the current graph statistics.

The summary includes:

* Companies
* Developers
* Projects
* Skills
* Technologies

It also displays the CognoDB connection status.

The production API currently returns:

```json
[
  {
    "label": "Company",
    "count": 3
  },
  {
    "label": "Developer",
    "count": 4
  },
  {
    "label": "Project",
    "count": 4
  },
  {
    "label": "Skill",
    "count": 8
  },
  {
    "label": "Technology",
    "count": 5
  }
]
```

---

### 11.2 Talent Finder

Talent Finder identifies developers who are suitable for a selected project.

The process is:

```text
Selected Project
       |
       | USES_SKILL
       v
Required Skills
       ^
       | HAS_SKILL
       |
Developers
       |
       | WORKED_ON
       v
Previous Projects
```

The query:

1. Finds the selected project.
2. Finds the skills required by the project.
3. Finds developers with matching skills.
4. Determines the projects previously worked on by each developer.
5. Excludes developers who already worked on the selected project.
6. Returns matching skills, company information, and work count.

For example, selecting:

```text
Employee Management Portal
```

can return developers such as:

```text
Arjun Mehta
Priya Reddy
```

depending on the seeded graph data.

---

### 11.3 Project Dependencies

Project Dependencies finds projects that are connected through shared developers and shared skills.

A simplified relationship can be represented as:

```text
Project A
   |
   | WORKED_ON
   v
Developer
   |
   | WORKED_ON
   v
Project B
```

Projects can also be connected through shared skills:

```text
Project A
   |
   | USES_SKILL
   v
  Skill
   ^
   | USES_SKILL
   |
Project B
```

The application combines these relationships to identify project dependencies.

The returned dependency chain can look like:

```text
Project: Employee Management Portal
        ↓
Skill: C#
        ↓
Project: Digital Banking Platform
```

---

## 12. Graph Summary Query

The dashboard uses a Cypher query similar to:

```cypher
MATCH (n)

RETURN
    labels(n)[0] AS Label,
    count(n) AS Count

ORDER BY Label;
```

This query:

1. Matches graph nodes.
2. Reads their labels.
3. Groups nodes by label.
4. Counts each group.
5. Sorts the results.

---

## 13. Parameterized Cypher Queries

User input is not directly concatenated into Cypher queries.

For example, the repository creates a parameter:

```csharp
var parameters = new Dictionary<string, object>
{
    ["projectName"] = projectName
};
```

The Cypher query then uses:

```cypher
$projectName
```

This keeps query structure separate from user input and avoids directly inserting user-provided values into the query string.

The same approach is used by both:

* Talent Finder
* Project Dependencies

---

## 14. Multi-Hop Graph Traversal

The application demonstrates multi-hop graph traversal rather than simple single-node lookups.

For example, Project Dependencies can traverse:

```text
Project
   ↓
Developer
   ↓
Project
```

and:

```text
Project
   ↓
Skill
   ↑
Project
```

Talent Finder also traverses relationships between:

```text
Project
   ↓
Skill
   ↑
Developer
   ↓
Project History
```

These traversal patterns are central to the purpose of the application.

---

## 15. CognoDB Setup

The application requires a CognoDB Cloud instance.

Create a CognoDB Cloud instance and obtain the connection information required by the application.

A connection URI has a format similar to:

```text
bolt+s://<instance-id>.databases.cognodb.cloud
```

The username for the provided CognoDB setup is:

```text
cognodb
```

The generated password should be stored securely.

**Do not commit the database password to GitHub.**

---

## 16. Environment Variables

The backend reads CognoDB configuration from environment variables.

Required variables:

```text
COGNODB_URI
COGNODB_USERNAME
COGNODB_PASSWORD
```

Example:

```text
COGNODB_URI=bolt+s://<instance-id>.databases.cognodb.cloud
COGNODB_USERNAME=cognodb
COGNODB_PASSWORD=<your-password>
```

The actual password must not be stored in source code.

For production deployment, these values are configured through the hosting environment rather than committed to the repository.

---

## 17. Running the Backend Locally

Open PowerShell in the repository root.

Build the solution:

```powershell
dotnet build .\WexaGraphExplorer.slnx
```

Run the API:

```powershell
dotnet run --project .\WexaGraphExplorer.Api
```

The API runs locally at:

```text
http://localhost:5021
```

### Health Check

```powershell
curl.exe -i http://localhost:5021/api/graph/health
```

Expected response:

```json
{
  "status": "Healthy",
  "message": "Successfully connected to CognoDB over Bolt."
}
```

### Graph Summary

```powershell
curl.exe -i http://localhost:5021/api/graph/summary
```

Expected result:

```json
[
  {
    "label": "Company",
    "count": 3
  },
  {
    "label": "Developer",
    "count": 4
  },
  {
    "label": "Project",
    "count": 4
  },
  {
    "label": "Skill",
    "count": 8
  },
  {
    "label": "Technology",
    "count": 5
  }
]
```

---

## 18. Swagger / OpenAPI

When running locally, Swagger is available at:

```text
http://localhost:5021/swagger
```

Swagger can be used to test the backend independently from Angular.

The API exposes graph-related endpoints including:

```text
GET /api/graph/health

GET /api/graph/summary

GET /api/graph/projects/{projectName}/missing-talent

GET /api/graph/projects/{projectName}/dependencies
```

Swagger is useful for verifying API functionality independently from the frontend.

---

## 19. Running the Angular Application

Open a second PowerShell window.

Move to the Angular project:

```powershell
cd .\WexaGraphExplorer.Web
```

Install dependencies:

```powershell
npm install
```

Start Angular:

```powershell
npm start
```

The frontend will be available at:

```text
http://localhost:4200
```

Open:

```text
http://localhost:4200/dashboard
```

---

## 20. Angular Development Proxy

Local Angular development uses the proxy configuration:

```text
WexaGraphExplorer.Web/proxy.conf.json
```

API requests beginning with:

```text
/api
```

are forwarded to the local ASP.NET Core API running on port `5021`.

This allows the Angular application to communicate with the local API without hardcoding the local API host throughout the frontend.

For production, the Angular application uses the deployed API:

```text
https://wexa-graph-api.onrender.com/api/graph
```

---

## 21. CORS Configuration

The API allows requests from the Angular frontend.

Configured origins include:

```text
https://wexa-graph-web.onrender.com
http://localhost:4200
```

The API allows:

* GET
* Other required HTTP methods
* Request headers required by the Angular client

The production CORS configuration was verified using an HTTP request containing the production frontend origin.

---

## 22. API Endpoints

### Health

```text
GET /api/graph/health
```

Purpose:

Verifies that the API can communicate successfully with CognoDB.

---

### Summary

```text
GET /api/graph/summary
```

Purpose:

Returns node counts grouped by graph label.

Example:

```json
[
  {
    "label": "Company",
    "count": 3
  },
  {
    "label": "Developer",
    "count": 4
  },
  {
    "label": "Project",
    "count": 4
  },
  {
    "label": "Skill",
    "count": 8
  },
  {
    "label": "Technology",
    "count": 5
  }
]
```

---

### Talent Finder

```text
GET /api/graph/projects/{projectName}/missing-talent
```

Purpose:

Finds developers with matching skills who have not already worked on the selected project.

---

### Project Dependencies

```text
GET /api/graph/projects/{projectName}/dependencies
```

Purpose:

Finds connected projects through shared developers and skills.

---

## 23. Error Handling

The backend handles database failures and reports an appropriate API error rather than silently assuming the database is available.

For example, database-related failures return a `503 Service Unavailable` response.

The Angular application also handles API connection failures.

If the API is unavailable, the dashboard displays an error state indicating that the Graph API is unavailable and provides a retry option.

This prevents the application from remaining indefinitely in a loading state.

---

## 24. Verifying the Database

The graph can be verified directly using Cypher.

### Total Nodes

```cypher
MATCH (n)

RETURN count(n) AS TotalNodes;
```

Expected:

```text
24
```

### Total Relationships

```cypher
MATCH ()-[r]->()

RETURN count(r) AS TotalRelationships;
```

Expected:

```text
57
```

### Node Types

```cypher
MATCH (n)

RETURN
    labels(n)[0] AS Label,
    count(n) AS Count

ORDER BY Label;
```

Expected:

```text
Company       3
Developer     4
Project       4
Skill         8
Technology    5
```

---

## 25. Screenshots

Screenshots demonstrating the working application are included in:

```text
docs/screenshots/
```

### Dashboard

![Dashboard](docs/screenshots/dashboard.png)

The dashboard displays graph statistics and the CognoDB connection status.

### Talent Finder

![Talent Finder](docs/screenshots/talent-finder.png)

The Talent Finder demonstrates skill-based developer matching.

### Project Dependencies

![Project Dependencies](docs/screenshots/project-dependencies.png)

The Project Dependencies page demonstrates relationships between projects.

### Swagger — Health

![Swagger Health](docs/screenshots/swagger-health.png)

### Swagger — Summary

![Swagger Summary](docs/screenshots/swagger-summary.png)

### Swagger — Dependencies

![Swagger Dependencies](docs/screenshots/swagger-dependencies.png)

### Swagger — Dependencies Response

![Swagger Dependencies Response](docs/screenshots/swagger-dependencies-response.png)

---

## 26. Complete Local Application Flow

The complete local application requires two terminals.

### Terminal 1 — Backend

From the repository root:

```powershell
dotnet run --project .\WexaGraphExplorer.Api
```

API:

```text
http://localhost:5021
```

### Terminal 2 — Angular

From the repository root:

```powershell
cd .\WexaGraphExplorer.Web
npm start
```

Frontend:

```text
http://localhost:4200
```

Open:

```text
http://localhost:4200/dashboard
```

The complete request flow is:

```text
Angular
   |
   | HTTP
   v
ASP.NET Core API
   |
   | Neo4j .NET Driver / Bolt
   v
CognoDB Cloud
```

---

## 27. Production Deployment

The application is deployed using Render.

### Frontend

```text
https://wexa-graph-web.onrender.com
```

### Backend

```text
https://wexa-graph-api.onrender.com
```

The Angular production application communicates with the deployed ASP.NET Core API.

The production API has been verified using:

```text
GET /api/graph/health
```

and:

```text
GET /api/graph/summary
```

Both endpoints return successful responses from the deployed API.

---

## 28. GitHub Repository

The complete source code, Cypher scripts, screenshots, documentation, and demo video are maintained in the GitHub repository:

```text
https://github.com/naveenkumarallem26/WexaGraphExplorer
```

The repository contains the complete implementation required to build and run the application.

---

## 29. Assignment Requirements

The implementation covers the primary requirements of the assignment:

* CognoDB graph database
* Official Neo4j .NET Driver
* OpenCypher queries
* Labeled graph nodes
* Typed relationships
* Node properties
* Realistic seed data
* Seed script included in the repository
* Example Cypher queries included
* Parameterized Cypher queries
* Multi-hop graph traversal
* Relationship-oriented graph queries
* ASP.NET Core Web API
* Angular web application
* Swagger / OpenAPI
* CognoDB connectivity handling
* API error handling
* Angular loading states
* Angular error states
* Retry functionality
* Production deployment
* Screenshots
* Demo video

---

## 30. Key Design Decisions

### Graph-oriented data model

The data is modeled around relationships between developers, projects, skills, technologies, and companies.

### Repository abstraction

The application layer depends on:

```text
IGraphExplorerRepository
```

while the infrastructure layer provides:

```text
CognoDbGraphExplorerRepository
```

This keeps graph access separate from application logic.

### Parameterized Cypher

Project names are passed as parameters using:

```text
$projectName
```

rather than concatenating user input into Cypher queries.

### Separation of concerns

The solution separates:

```text
API
Application
Domain
Infrastructure
Frontend
```

This keeps responsibilities clear and makes the application easier to maintain.

### Production and local environments

The application supports:

```text
Local Angular
        ↓
Local ASP.NET Core API
        ↓
CognoDB
```

and:

```text
Production Angular
        ↓
Production ASP.NET Core API
        ↓
CognoDB
```

---

## 31. Conclusion

Wexa Graph Explorer demonstrates how a graph database can be used to model and explore an engineering ecosystem.

The application focuses on relationship-oriented queries rather than simple CRUD operations.

The two primary use cases demonstrate this directly:

```text
Talent Finder
Project → Skill → Developer → Project History
```

and:

```text
Project Dependencies
Project → Developer → Project
Project → Skill → Project
```

The completed solution provides:

* A graph-based backend
* Parameterized Cypher queries
* Multi-hop graph traversal
* A functional Angular interface
* API documentation through Swagger
* CognoDB connectivity
* Production deployment
* Error handling
* Screenshots
* A demonstration video

---

## 32. Author

**Naveen Kumar**

Wexa AI — CognoDB Take-Home Assignment
