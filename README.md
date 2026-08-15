# Welcome to Banking App DDD

This project is an experimental full-stack banking application combining cutting-edge technologies and modern architectural patterns. Thanks for visiting! Please **give a ⭐** if you like this project — it motivates me to keep improving it!

---

## Features Provided

This system provides a comprehensive suite of banking features to clients:

- **Account Registration & Opening**: Open a new bank account by providing Full Name, Username, Account Password, Account Type, Branch, and Initial Deposit. A unique Account ID is automatically generated for each user.
  
  [![Open Account](image/openaccount.png)](image/openaccount.png)

- **Authentication & Login**: Secure login using unique Account ID / Email and Account Password. Full handling for invalid credentials and session token validation.
  
  [![Login Page](image/login.png)](image/login.png)

- **User Profile Management**: View and edit user profile details, SSN, contact details, address, and profile photo.
  
  [![User Profile](image/userprofile.png)](image/userprofile.png)

- **Balance & Cash Operations**: Real-time balance check, cash deposit, and cash withdrawal with validation against negative amounts or insufficient funds.
- **Internal & External Fund Transfer**: Transfer money to accounts within the same bank or across external banks with account validation and balance checks.
  
  [![User Dashboard](image/userdashboard1.png)](image/userdashboard1.png)

- **Payment Gateway Processing**: Fund transfers using multiple payment methods including **NEFT**, **RTGS**, and **UPI**.
  
  [![Fund Transfer](image/fundtransfer.png)](image/fundtransfer.png)

- **Transaction History**: Comprehensive transaction history with filtering and details.
  
  [![Transaction History](image/userdashboard2.png)](image/userdashboard2.png)

- **Account Summary**: View detailed balances, account numbers, statuses, and branch info.
  
  [![Account Summary](image/accountsummary.png)](image/accountsummary.png)

- **Beneficiary / Payee Management**: Add and manage payees for quick and safe transfers.
  
  [![Add Payee](image/addpayee.png)](image/addpayee.png)

- **Loan Application & Evaluation**: Submit loan applications, evaluate applications, search loan applications by ID, and process manager decisions.
  
  [![Loan Application](image/loanapplication.png)](image/loanapplication.png)
  
  [![Loan Decision](image/loandecision.png)](image/loandecision.png)
  
  [![Loan Search](image/loansearch.png)](image/loansearch.png)

- **Operator Hub**: Dedicated operator dashboard for bank staff to monitor operations and customer requests.
  
  [![Operator Hub](image/operatorhub.png)](image/operatorhub.png)

---

## System Architecture

### High-Level Architecture

[![High-Level System Architecture](image/BankingAppDDD-hl-architecture.png)](image/BankingAppDDD-hl-architecture.png)

### Detailed Architecture

[![Detailed System Architecture](image/BankingAppDDD-detailed-architecture.png)](image/BankingAppDDD-detailed-architecture.png)

---

## Project Directory Structure

```
├── Core
│   ├── Applications
│   ├── Common
│   ├── Domains
│   └── Infrastructures
│
├── Crosscutting
│   ├── ApiGateway
│   ├── IdentityServer
│   └── ServiceClients
│
├── Services
│   ├── AccountManagement
│   ├── CustomerManagement
│   ├── LoanManagement
│   │   └── BankingApp.LoanManagement
│   │       ├── API
│   │       ├── Application
│   │       ├── Core (Domain)
│   │       └── Infrastructure
│   └── PaymentProcessing
│
├── SPA (BankingAppDDDSpa)
└── docker-compose
```

---

## Architectural Patterns Implemented

- **Microservices Architecture**: Vertically sliced services built for scalability and isolation.
- **Clean Architecture & Domain-Driven Design (DDD)**: Aggregate roots, entity boundaries, value objects, domain services, repository abstractions, and domain events.
- **CQRS (Command Query Responsibility Segregation)**: Separate read and write models powered by MediatR.
- **Outbox Pattern**: Reliable domain event publishing across service boundaries.
- **Saga Pattern**: Distributed transactions implemented using choreography and orchestration.
- **Circuit Breaker**: Fault-tolerance mechanisms for cross-service calls.
- **Cross-Cutting Concerns**: Centralized logging with Seq, global exception handling, Polly resilience, and ASP.NET Core Health Checks.
- **Machine Learning & LLM Model**: Implemented Real time fraud detection system by introducing a 2-layer ensemble workflow: Layer 1 (ML Quantitative Engine) + Layer 2 (Microsoft Semantic Kernel & OpenAI Semantic Analyzer), merged by an Ensemble Workflow Coordinator using weighted voting and conditional routing.
### Layer Responsibilities

- **Core / Domain**: Building blocks and abstractions used across projects (Aggregates, Value Objects, Domain Events). Pure domain logic without infrastructure dependencies.
- **Infrastructure**: Persistence implementations, Entity Framework Core mappings, database contexts, and external integrations.
- **Application**: Application use cases, command/query handlers, DTOs, and event consumers.
- **Common**: Shared utilities, helper classes, base controllers, and common middleware.
- **Services (Vertically Sliced)**:
  ```
  ├── Services
  │   ├── BankingApp.AccountManagement
  │   │   ├── API
  │   │   ├── Application
  │   │   ├── Core (Domain)
  │   │   └── Infrastructure
  ```
- **SPA (Single Page Application)**: Modern Angular frontend built with Angular Standalone components, Reactive Forms, NgBootstrap, and FontAwesome.

---

## Technologies Used

### Backend Stack

- **Framework**: .NET 8 / C# 12
- **API Gateway**: Ocelot 23.4.3 & Koalesce.OpenAPI
- **Databases & ORM**: PostgreSQL, MongoDB, Redis, Entity Framework Core 8.0.15, Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11
- **Messaging & Event Bus**: RabbitMQ with MassTransit
- **Identity & Auth**: Keycloak 26.1.2 with ASP.NET Core JwtBearer 8.0.15
- **Logging & Monitoring**: Seq & GELF logging input (`seq-input-gelf`)
- **Documentation & Kiota**: SwaggerGen / SwaggerUI 8.1.1, Microsoft.Kiota

### Frontend Stack

- **Framework**: Angular v19.2 / TypeScript 5.5
- **UI Components**: Bootstrap 5.3.5 / NgBootstrap 18.0.0
- **Icons & Styling**: Font Awesome 6.7.2, Vanilla CSS

---

## Prerequisites & How to Run

### 1. Requirements

- Install **Docker Desktop** with WSL 2 support: [Docker Desktop Download](https://docs.docker.com/docker-for-windows/wsl/)
- **.NET 8 SDK** & **Node.js** (for local development)

Running the SPA locally
If you prefer running the frontend outside Docker for development, start the backend with docker compose up, then:

cd src/BankingAppDDDSpa
npm install
ng serve
The app will be available at http://localhost:4200.

### 2. Database & Identity Setup

#### Keycloak PostgreSQL Setup
Execute the commands below from your terminal:

```bash
# Connect to PostgreSQL container
docker exec -it <container_id_of_postgres> /bin/bash

# Log into PostgreSQL prompt
psql -U keycloak -d keycloak_db

# Create User and Roles
CREATE ROLE TestUser WITH LOGIN PASSWORD 'TestUser1';
ALTER ROLE TestUser WITH CREATEDB LOGIN REPLICATION;
```

#### MongoDB Setup
```bash
# Connect to MongoDB container
docker exec -it <container_id_of_mongodb> /bin/bash

# Open MongoDB shell
mongosh -u root -p root123 --authenticationDatabase admin

# Create user database
use myUserInfo
db.createUser({
  user: "TestUser",
  pwd: "TestUser1",
  roles: [ { role: "readWrite", db: "myUserInfo" } ]
})
```

Connect using **Studio 3T** or **MongoDB Compass**:
```
mongodb://TestUser:TestUser1@localhost:27017/myUserInfo?authSource=myUserInfo
```

[![MongoDB Studio 3T](image/mongostudio3t.png)](image/mongostudio3t.png)

#### Local DNS Hosts Configuration
Add the following entries to your `/etc/hosts` file (or `C:\Windows\System32\drivers\etc\hosts` on Windows):

```text
127.0.0.1 keycloak
127.0.0.1 seq
```

### 3. Keycloak Realm & Client Configuration

1. Open Keycloak Admin Console at [http://keycloak:8080](http://keycloak:8080) (or `http://localhost:8080`).
2. Log in using `admin` / `admin`.
3. Create Realm: `bankaccount`.
4. Create Client: `customermanagementclient`.
5. Configure client settings according to the screenshots below:

   [![Keycloak Setup 1](image/keycloak1.png)](image/keycloak1.png)
   
   [![Keycloak Setup 2](image/keycloak2.png)](image/keycloak2.png)
   
   [![Keycloak Setup 3](image/keycloak3.png)](image/keycloak3.png)
   
   [![Keycloak Setup 4](image/keycloak4.png)](image/keycloak4.png)
   
   [![Keycloak Setup 5](image/keycloak5.png)](image/keycloak5.png)

6. Configure Web API clients for endpoints:
   - `http://localhost:5263`
   - `http://localhost:5157`
   - `http://localhost:5210`
   - `http://localhost:5273`
   - `http://localhost:5000` (API Gateway Client)
7. Copy the client secret from the **Credentials** tab and paste it into the respective `appsettings.json` files for your Web API services.
8. In the **Roles** tab, create roles: `Admin`, `Customer`, `Operator`, `Underwriter`.
9. In **Client Scopes**, select `customermanagementclient-dedicated` and add a new mapper for `UserClient`.
10. In `customermanagementclient` -> **Service Account Roles**, click **Assign Roles**, filter by clients, and assign `realm-management -> manage-users` to allow user creation via Keycloak REST API.

---

## Running with Docker Compose

Run the following command from the repository root:

```bash
docker-compose up --build
```

You can also set `docker-compose.dcproj` as the Startup Project in Visual Studio for debugging.

---

## Service Endpoints & Dashboards

- **Angular Frontend SPA**: [http://localhost:4200](http://localhost:4200)
- **API Gateway**: [http://localhost:5000](http://localhost:5000)
- **Seq Log Server**: [http://localhost:8081/#/events](http://localhost:8081/#/events)
  
  [![Seq Log Server](image/seqlog.png)](image/seqlog.png)

- **RabbitMQ Dashboard**: [http://localhost:15672](http://localhost:15672) (`guest` / `guest`)
  
  [![RabbitMQ Dashboard](image/rabbitmq.png)](image/rabbitmq.png)

- **PgAdmin Database Manager**: [http://localhost:5050](http://localhost:5050)
  
  [![PgAdmin Interface 1](image/pgadmininterface.png)](image/pgadmininterface.png)
  
  [![PgAdmin Interface 2](image/pgadminin1terface.png)](image/pgadminin1terface.png)
