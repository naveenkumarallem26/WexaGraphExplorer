# Wexa Graph Explorer

Wexa Graph Explorer is a web application built for the Wexa AI CognoDB take-home assignment. The application uses a graph database to represent developers, projects, skills, technologies, and companies, and provides a simple interface for exploring the relationships between them.

The application has two main features. The first is **Talent Finder**, which finds developers whose skills match the requirements of a selected project and excludes developers who have already worked on that project. The second is **Project Dependencies**, which finds relationships between projects through shared developers and skills.

The backend is implemented using ASP.NET Core 8 and the official Neo4j .NET Driver. The frontend is implemented using Angular 20. CognoDB is used as the graph database and communicates with the backend through the Bolt protocol.

---

## 1. Application Overview

The application represents an engineering organization as a graph.

A developer can have multiple skills, work for a company, and work on multiple projects. A project can require several skills and use different technologies.

For example, the following relationship exists in the application:

```text
Developer
    |
    | HAS_SKILL
    v
  Skill
```

A developer can also have a relationship with a project:

```text
Developer
    |
    | WORKED_ON
    v
 Project
```

Projects have their own relationships with skills and technologies:

```text
Project
   |                    |
   | USES_SKILL        | USES_TECHNOLOGY
   v                    v
 Skill              Technology
```

This structure allows the application to answer questions by traversing the graph instead of treating each entity as an isolated record.

---

## 2. Live Deployment

The application is deployed and available online.

### Production Frontend

https://wexa-graph-web.onrender.com

### Production Backend API

https://wexa-graph-api.onrender.com

### Production API Health Check

https://wexa-graph-api.onrender.com/api/graph/health

The health endpoint verifies that the API can successfully communicate with CognoDB over the Bolt protocol.

### Production Graph Summary

https://wexa-graph-api.onrender.com/api/graph/summary

The production API currently returns the following graph summary:

```text
Company       3
Developer     4
Project       4
Skill         8
Technology    5
```

The Angular frontend communicates with the deployed ASP.NET Core API in the production environment.

---

## 3. Why I Chose a Graph Database

The main reason for choosing a graph database is that the useful information in this application comes from the relationships between entities.

For example, Talent Finder needs to start with a project, find the skills required by that project, find developers who have those skills, check their project history, and exclude developers who have already worked on the selected project.

The relationship can be represented as:

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

This type of traversal can certainly be implemented in a relational database, but it would require several tables and joins between project, skill, developer, and project-history tables.

The Project Dependencies feature is another example. It looks for projects that are connected through developers and shared skills. These relationships are naturally represented by nodes and edges in a graph.

For this use case, the graph model makes the relationships explicit and the Cypher queries easier to understand.

---

## 4. Technology Used

### Backend

* ASP.NET Core 8
* C#
* Official Neo4j .NET Driver
* OpenCypher
* Swagger / OpenAPI
* Docker
* Render

### Frontend

* Angular 20
* TypeScript
* SCSS
* Angular Router
* Angular HTTP Client
* Angular SSR

### Database

* CognoDB Cloud
* Bolt protocol
* OpenCypher

---

## 5. Project Structure

The solution is divided into separate projects so that the database access, application logic, API, and frontend are kept separate.

```text
Assessment
│
├── scripts
│   ├── seed.cypher
│   └── queries.cypher
│
├── docs
│   └── screenshots
│       ├── dashboard.png
│       ├── talent-finder.png
│       ├── project-dependencies.png
│       ├── swagger-health.png
│       ├── swagger-summary.png
│       ├── swagger-dependencies.png
│       ├── swagger-dependencies-response.png
│       └── README_SCREENSHOTS_SECTION.md
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
│   ├── Configuration
│   │   └── CognoDbSettings.cs
│   ├── Graph
│   │   └── CognoDbGraphExplorerRepository.cs
│   └── Seeding
│       └── CognoDbSeeder.cs
│
├── WexaGraphExplorer.Web
│
├── Dockerfile
│
└── WexaGraphExplorer.slnx
```

The API project is responsible for exposing the HTTP endpoints.

The Application project contains the graph-related application logic and DTOs.

The Infrastructure project contains the CognoDB connection, Neo4j driver configuration, repository implementation, and database seeding.

The Web project contains the Angular application.

The `scripts` directory contains the Cypher seed data and example queries used by the application.

The `docs/screenshots` directory contains screenshots demonstrating the working application and API responses.

The Dockerfile provides the container configuration used for deployment.

---

## 6. Graph Data Model

The database contains five types of nodes.

### Company

A company represents an organization where a developer has worked.

Properties:

```text
name
location
industry
```

### Developer

A developer represents a person in the engineering organization.

Properties:

```text
name
email
experienceYears
location
```

### Project

A project represents an engineering project.

Properties:

```text
name
description
```

### Skill

A skill represents a technical skill that a developer can have or a project can require.

Properties:

```text
name
category
```

### Technology

A technology represents a technology used by a project.

Properties:

```text
name
category
```

The relationships are:

```text
Developer ── HAS_SKILL ──> Skill

Developer ── WORKED_AT ──> Company

Developer ── WORKED_ON ──> Project

Project ── USES_SKILL ──> Skill

Project ── USES_TECHNOLOGY ──> Technology
```

A simplified view of the complete graph is:

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

## 7. Seed Data

The initial graph data is stored in:

```text
scripts/seed.cypher
```

The seed script first removes the existing graph:

```cypher
MATCH (n)
DETACH DELETE n;
```

It then creates the companies, technologies, skills, developers, and projects followed by their relationships.

The current seeded graph contains:

| Type                | Count |
| ------------------- | ----: |
| Companies           |     3 |
| Developers          |     4 |
| Projects            |     4 |
| Skills              |     8 |
| Technologies        |     5 |
| Total Nodes         |    24 |
| Total Relationships |    57 |

The seed script is executed automatically when the API starts successfully and the required CognoDB environment variables are configured.

---

## 8. Main Queries

The application uses Cypher queries to retrieve information from CognoDB.

### Graph Summary

The dashboard uses a query similar to:

```cypher
MATCH (n)

RETURN
    labels(n)[0] AS Label,
    count(n) AS Count

ORDER BY Label;
```

This query counts the nodes belonging to each label.

It is used to display the number of projects, developers, skills, companies, and technologies on the dashboard.

---

### Talent Finder

Talent Finder starts with the selected project and finds the skills required by that project.

It then checks developers who have those skills.

The query also checks whether the developer has already worked on the selected project. Developers who have already worked on the project are excluded from the result.

The important part of the traversal is:

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
```

The project name is passed to the query as a parameter:

```text
$projectName
```

The application does not concatenate the project name into the Cypher query.

---

### Project Dependencies

The Dependencies page looks for projects that are connected through developers and shared skills.

For example:

```text
Employee Management Portal
          |
          | USES_SKILL
          v
         C#
          ^
          | USES_SKILL
          |
Digital Banking Platform
```

A developer can also connect the two projects:

```text
Employee Management Portal
          ^
          |
      WORKED_ON
          |
      Developer
          |
      WORKED_ON
          |
          v
Digital Banking Platform
```

This is a multi-hop relationship and is one of the main reasons a graph database is suitable for this application.

The dependency query also receives the project name as a parameter.

---

## 9. Parameterized Queries

User input is not directly inserted into Cypher strings.

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

This keeps query structure separate from user input and follows the requirement to use parameterized queries.

---

## 10. CognoDB Setup

Before running the application locally, a CognoDB Cloud instance needs to be created.

Create a CognoDB Cloud account and create a free C0 instance.

After the instance is created, CognoDB provides a connection URI similar to:

```text
bolt+s://<instance-id>.databases.cognodb.cloud
```

The username for the provided instance is:

```text
cognodb
```

Save the generated password because it is required by the application.

---

## 11. Environment Variables

The application reads the CognoDB connection details from environment variables.

The following variables are required:

```text
COGNODB_URI
COGNODB_USERNAME
COGNODB_PASSWORD
```

For example:

```text
COGNODB_URI=bolt+s://<instance-id>.databases.cognodb.cloud
COGNODB_USERNAME=cognodb
COGNODB_PASSWORD=<your-password>
```

The actual password should not be added to the source code or committed to GitHub.

For the deployed Render API, these values are configured as environment variables in the Render service.

---

## 12. Running the Backend Locally

Open PowerShell in the repository root.

First build the complete solution:

```powershell
dotnet build .\WexaGraphExplorer.slnx
```

If the build succeeds, start the API:

```powershell
dotnet run --project .\WexaGraphExplorer.Api
```

The API runs on:

```text
http://localhost:5021
```

When the application starts, it verifies the CognoDB connection and executes the seed process when the required environment variables are configured.

---

## 13. Swagger

Swagger is available locally at:

```text
http://localhost:5021/swagger
```

Swagger can be used to test the backend endpoints independently from the Angular application.

The main graph endpoints are:

```text
GET /api/graph/health

GET /api/graph/summary

GET /api/graph/projects/{projectName}/missing-talent

GET /api/graph/projects/{projectName}/dependencies
```

Swagger is useful when debugging the API because it allows the graph endpoints to be tested directly without depending on the Angular frontend.

---

## 14. Running the Angular Application

Open a second PowerShell window.

Move to the Angular project:

```powershell
cd .\WexaGraphExplorer.Web
```

Install the required packages:

```powershell
npm install
```

Start the Angular development server:

```powershell
npm start
```

The frontend will be available at:

```text
http://localhost:4200
```

The Angular application uses the development proxy configuration in:

```text
WexaGraphExplorer.Web/proxy.conf.json
```

API requests beginning with `/api` are forwarded to the ASP.NET Core API running on port `5021`.

For production, the Angular application uses the deployed API:

```text
https://wexa-graph-api.onrender.com/api/graph
```

---

## 15. Using the Application

After starting both applications locally, open:

```text
http://localhost:4200/dashboard
```

The Dashboard displays the current graph statistics and CognoDB connection status.

The production application is available at:

```text
https://wexa-graph-web.onrender.com
```

### Talent Finder

From the navigation menu, open **Talent Finder**.

Enter or select a project such as:

```text
Employee Management Portal
```

The application returns developers who have matching skills and have not already worked on that project.

The application displays their company, matching skills, and work history information returned by the API.

### Project Dependencies

Open **Dependencies**.

Select:

```text
Employee Management Portal
```

The application displays connected projects and the dependency paths between them.

For example:

```text
Project: Employee Management Portal
       ↓
Skill: C#
       ↓
Project: Digital Banking Platform
```

---

## 16. Error Handling

The application verifies the CognoDB connection when the API starts.

If the database is unavailable, the API reports the database initialization or connectivity problem instead of silently assuming that the database is available.

The API endpoints also return appropriate HTTP error responses when graph operations fail.

The Angular application handles API connection failures.

For example, if the API is unavailable while Angular is running, the dashboard displays an error state indicating that the Graph API is unavailable and provides a retry option.

This makes the failure visible to the user rather than leaving the page in a loading state indefinitely.

---

## 17. Production Deployment

The application is deployed using Docker and Render.

### Frontend

The Angular application is containerized and deployed as a production application.

Production frontend:

```text
https://wexa-graph-web.onrender.com
```

### Backend

The ASP.NET Core 8 Web API is containerized and deployed separately.

Production API:

```text
https://wexa-graph-api.onrender.com
```

### Production API Verification

Health check:

```text
GET https://wexa-graph-api.onrender.com/api/graph/health
```

Expected response:

```json
{
  "status": "Healthy",
  "message": "Successfully connected to CognoDB over Bolt."
}
```

Graph summary:

```text
GET https://wexa-graph-api.onrender.com/api/graph/summary
```

Expected seeded summary:

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

The production frontend successfully communicates with the production API and retrieves graph data from CognoDB.

---

## 18. Docker

The project includes Docker configuration for deployment.

The Dockerfile is located at:

```text
WexaGraphExplorer.Web/Dockerfile
```

The deployment process builds the Angular application inside the container and runs the generated production application using the configured server.

The backend also uses Docker-based deployment through the Render service configuration.

---

## 19. Verifying the Database

The graph can be verified directly using Cypher.

To check the number of nodes:

```cypher
MATCH (n)

RETURN count(n) AS TotalNodes;
```

Expected result:

```text
24
```

To check relationships:

```cypher
MATCH ()-[r]->()

RETURN count(r) AS TotalRelationships;
```

Expected result:

```text
57
```

To check the different node types:

```cypher
MATCH (n)

RETURN
    labels(n)[0] AS Label,
    count(n) AS Count

ORDER BY Label;
```

Expected result:

```text
Company       3
Developer     4
Project       4
Skill         8
Technology    5
```

---

## 20. Screenshots

Screenshots of the working application and API responses are included in:

```text
docs/screenshots/
```

### Dashboard

![Dashboard](docs/screenshots/dashboard.png)

### Talent Finder

![Talent Finder](docs/screenshots/talent-finder.png)

### Project Dependencies

![Project Dependencies](docs/screenshots/project-dependencies.png)

### Swagger — Health

![Swagger Health](docs/screenshots/swagger-health.png)

### Swagger — Summary

![Swagger Summary](docs/screenshots/swagger-summary.png)

### Swagger — Dependencies

![Swagger Dependencies](docs/screenshots/swagger-dependencies.png)

### Swagger — Dependencies Response

![Swagger Dependencies Response](docs/screenshots/swagger-dependencies-response.png)

---

## 21. Running the Complete Application Locally

The complete local setup requires two terminals.

### Terminal 1 — API

From the repository root:

```powershell
dotnet run --project .\WexaGraphExplorer.Api
```

The API should be available at:

```text
http://localhost:5021
```

### Terminal 2 — Angular

From the repository root:

```powershell
cd .\WexaGraphExplorer.Web

npm install

npm start
```

The Angular application should be available at:

```text
http://localhost:4200
```

Then open:

```text
http://localhost:4200/dashboard
```

The request flow is:

```text
Angular
   |
   | HTTP
   v
ASP.NET Core API
   |
   | Neo4j .NET Driver / Bolt
   v
CognoDB
```

---

## 22. API Endpoints

The application exposes the following graph endpoints:

### Health

```text
GET /api/graph/health
```

Checks the connection between the API and CognoDB.

### Graph Summary

```text
GET /api/graph/summary
```

Returns node counts grouped by graph label.

### Talent Finder

```text
GET /api/graph/projects/{projectName}/missing-talent
```

Returns developers whose skills match the requirements of the specified project while excluding developers who have already worked on that project.

### Project Dependencies

```text
GET /api/graph/projects/{projectName}/dependencies
```

Returns connected projects, shared developers, shared skills, and dependency chains.

---

## 23. Assignment Requirements

The implementation covers the main requirements of the assignment:

* CognoDB graph database
* Official Neo4j .NET Driver
* Labeled graph nodes
* Typed relationships
* Node properties
* Realistic seed data
* Seed script included in the repository
* Cypher queries included in the repository
* Parameterized Cypher queries
* Multi-hop graph traversal
* Relationship-oriented graph queries
* Angular web application
* ASP.NET Core Web API
* Swagger / OpenAPI
* Database connectivity handling
* API error handling
* Loading and error states in the UI
* Docker deployment
* Production deployment using Render
* Production frontend and backend separation

The submission includes:

* GitHub repository
* Hosted application
* Hosted API
* Screenshots
* Source code
* Seed and query scripts
* Docker configuration
* Project documentation
* Screen recording, where required

---

## 24. Final Verification

The application has been verified locally and in production.

### Local Verification

```text
GET http://localhost:5021/api/graph/health
→ 200 OK

GET http://localhost:5021/api/graph/summary
→ 200 OK
```

### Production Verification

```text
GET https://wexa-graph-api.onrender.com/api/graph/health
→ 200 OK

GET https://wexa-graph-api.onrender.com/api/graph/summary
→ 200 OK
```

### Frontend Verification

```text
Dashboard
→ Working

Talent Finder
→ Working

Project Dependencies
→ Working
```

### Git Verification

The repository is synchronized with the remote `main` branch and the working tree is clean.

---

## 25. Author

**Naveen Kumar**

Wexa AI — CognoDB Take-Home Assignment
