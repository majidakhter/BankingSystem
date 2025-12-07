\## Welcome to Banking App DDD

This project is an experimental full-stack application where still I am working on frontend and I have combined several cutting-edge technologies and architectural patterns. Thanks for getting here! please <b>give a ⭐</b> if you liked the project. It motivates me to keep improving it.

<br><br>



<a href="images/ecommerceddd-1.gif" target="\_blank">

<img src="images/ecommerceddd-1.gif" width="600px"/>

</a>



<a href="images/ecommerceddd-2.gif" target="\_blank">

<img src="images/ecommerceddd-2.gif" width="600px"/>

</a>



<a href="images/ecommerceddd-3.gif" target="\_blank">

<img src="images/ecommerceddd-3.gif" width="600px"/>

</a>



---



\## Architecture



\### High-Level System Architecture

<a href="images/BankingAppDDD-hl-architecture.png" target="\_blank">

<img src="images/BankingAppDDD-hl-architecture.png"/>

</a>





\### Detailed Architecture

<a href="images/BankingAppDDD-detailed-architecture.png" target="\_blank">

<img src="images/BankingAppDDD-detailed-architecture.png"/>

</a>





```

├── Core

│   ├── Applications

│   ├── Common

│   ├── Domains

│   └── Infrastructures



├── Crosscutting

│   ├── ServiceClients

│   ├── ApiGateway

│   ├── IdentityServer

│   

│

├── Services

│   ├── CustomerManagement

│   ├── AccountManagement

│   ├── LoanManagement

|      └─ BankingApp.LoanManagement

│         ├── API

│         ├── Application

│         ├── Core(Domain)

│         └── Infrastructure

├── SPA

└── docker-compose

```



\- \*\*Domains\*\* <br/>

It defines the building blocks and abstractions used on all underlying projects. Its nature is very abstract, with no implementations.



\- \*\*Infrastructure\*\* <br/>

It holds some abstractions and implementation for infrastructure to be used by all microservices and underlying dependencies.



\- \*\*Applications\*\* <br/>

It defines the building blocks and abstractions used on all underlying projects and implementation for application to be used by all the services.



\- \*\*Common\*\* <br/>

It defines the building blocks and abstractions used on all underlying projects and implementation for application to be used by all the services.



\- \*\*Crosscutting\*\* <br/>

It contains building blocks and implementation to be used by all the services`.



\- \*\*Services\*\* <br/>

The microservices composing the backend are built to be as simple as possible, structured as a vertically sliced structure with  `API`, `Application`, `Core,` and `Infrastructure.`



&nbsp;   ```

&nbsp;     ├── BankingApp.CustomerManagement

&nbsp;     │   ├── Controller(API)

&nbsp;     │   ├── Application

&nbsp;     │   ├── Core(Domain)

&nbsp;     │   └── Infrastructure

&nbsp;   ```



&nbsp; - \*\*API\*\* <br/>

&nbsp; RESTful API for enabling communication between client and server.



&nbsp; - \*\*Application\*\* <br/> 

&nbsp; It orchestrates the interactions between the external world and the domain to perform application tasks through use cases by `handling commands and queries`. 



&nbsp; - \*\*Domain\*\* <br/>

&nbsp; A structured implementation of the domain through aggregates, commands, value objects, domain services, repository definitions, and domain events.



&nbsp; - \*\*Infrastructure\*\* <br/>

&nbsp; It is a supporting library for upper layers, handling infrastructural matters such as data persistence with \*implementing repositories\*, database mapping, and external integrations.



&nbsp; - \*\*SPA (Single Page Application)\*\* <br/>

&nbsp; A lightweight Angular-based `SPA` providing a functional and user-friendly UI on which still work is going on.



---



\## Technologies used

<ul>

&nbsp; <li>

&nbsp;   <a href='https://get.asp.net' target="\_blank">ASP.NET Core API</a> and <a href='https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-12' target="\_blank">C# 12</a>

&nbsp;   for cross-platform backend with:

&nbsp;   <ul>

&nbsp;     <li>.NET 8</li>

&nbsp;     <li><b>Koalesce.OpenAPI 0.1.1-alpha.2</b></li>

&nbsp;     <li>Ocelot 23.4.3</li>

&nbsp;     <li>RabbitMq</li>

&nbsp;     <li>Redis</li>

&nbsp;     <li>Seq</li>

&nbsp;     <li>seq-input-gelf</li>

&nbsp;     <li>Mongodb</li>

&nbsp;     <li>Entity Framework Core 8.0.15</li>

&nbsp;     <li>EF Core Migrations</li>

&nbsp;     <li>Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11</li>

&nbsp;     <li>ASP.NET Core Authentication JwtBearer 8.0.15</li>

&nbsp;     <li>Keycloak 26.1.2</li>

&nbsp;     <li>SwaggerGen/SwaggerUI 8.1.1</li>

&nbsp;     <li>Microsoft.Kiota.\* 1.17.2</li>

&nbsp;   </ul>

&nbsp; </li>

&nbsp; <li>

&nbsp;   <a href='https://angular.io/' target="\_blank">Angular v19.2.7</a> and <a href='http://www.typescriptlang.org/' target="\_blank">TypeScript 5.5.4</a> for the frontend with:

&nbsp;   <ul>

&nbsp;     <li>Kiota 1.0.2</li>

&nbsp;     <li>NgBootstrap 18.0.0/ Bootstrap 5.3.5</li>

&nbsp;     <li>Font Awesome 6.7.2</li>

&nbsp;   </ul>

&nbsp; </li>

</ul>



---



\## What do you need to run it 



\#### Running the microservices using Docker



The project was designed to be easily run within docker containers, hence all you need is 1 command line to up everything. Make sure you have `Docker` installed and have fun!





\- Download Docker: <a href="https://docs.docker.com/docker-for-windows/wsl/" target="\_blank">Docker Desktop with support for WLS 2</a>

&nbsp;   

<br/>



Using a terminal, run:



```console

&nbsp;$ docker-compose up or docker-compose up --build

``` 



You can also set the `docker-compose.dcproj` as a Startup project on Visual Studio if you want to run it while debugging. 







