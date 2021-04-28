## Welcome to Ecommerce DDD

My main goal with this project is to demonstrate how to build a full-stack application using principles of domain-driven design and SOLID to provide an expressive and cohesive code.
Besides, provide an easy-to-use and scalable API to power a SPA application. Last but not least, I also demonstrate the powerful combination of the CQRS pattern and Event Sourcing. 

<br>The chosen domain was e-commerce, although this project is not intended to be a real or much less ideal implementation of e-commerce solution for production, it is capable of demonstrating a purchasing cycle that goes from product selection to final payment through established business rules.

<br>Thanks for getting here, and <b>let your star</b> if you liked it! I will always keep working to improve the project with optimizations, tests, and refactorings. 

<hr>

### Screenshots 

<a href="https://raw.githubusercontent.com/falberthen/EcommerceDDD/master/Screenshots/login.png" target="_blank">
<img src="https://raw.githubusercontent.com/falberthen/EcommerceDDD/master/Screenshots/login.png"/>
</a>
<br/><br/><br/>
<a href="https://raw.githubusercontent.com/falberthen/EcommerceDDD/master/Screenshots/products.png" target="_blank">
<img src="https://raw.githubusercontent.com/falberthen/EcommerceDDD/master/Screenshots/products.png" />
</a>

<br/><br/><br/>
<a href="https://raw.githubusercontent.com/falberthen/EcommerceDDD/master/Screenshots/orders.png" target="_blank">
<img src="https://raw.githubusercontent.com/falberthen/EcommerceDDD/master/Screenshots/orders.png" />
</a>

<br/><br/>
<a href="https://raw.githubusercontent.com/falberthen/EcommerceDDD/master/Screenshots/events.png" target="_blank">
<img src="https://raw.githubusercontent.com/falberthen/EcommerceDDD/master/Screenshots/events.png" />
</a>

<hr>

### Architecture diagram
<img src="https://raw.githubusercontent.com/falberthen/EcommerceDDD/master/Screenshots/Diagram.PNG"/>

Following a typical clean architecture, each layer was designed to play a specific role, allowing low coupling and testability. You will find a quick summary about each layer below.
<hr/>

### Domain Core
The Domain Core layer establishes basic abstractions to be implemented in the lower layers. It forms the basis of the building blocks for the implementation of DDD, with abstract classes for Entity, Aggregate Root, Value Object and, interfaces for Repository, Unit of Work.
<br/><br/>

### Domain
The Domain layer is where the business lies. It has a modular (organized by folders) implementation of the classes that make up the e-commerce through <b>concrete</b> Aggregates, Entities, Value objects, Domain Services and Repository interfaces and Domain Events. All the business rules and invariants are written in the domain models or services to ensure domain integrity and proper flow.
<br/><br/>

### Application
The Application layer orchestrates the interactions between the external world (API/SPA) and domain. It does not contain business rules or knowledge. It does not have state reflecting the business situation, but it can have state that reflects the progress of a task for the user or the program. Since the project is based on CQRS/EventSourcing architecture, there are Commands, CommandHandlers, Queries, QueryHandlers, EventHandlers and some primary validation.
<br/><br/>

### Infrastructure
The Infrastructure layer takes care of the application's infrastructure and issues not related to the business itself. It is responsible for Database (ORM) creation, Domain Repository implementation, Identity authentication and User Claims, JWT Authentication, Tooling for processing and publishing messages and, Inversion of Control and all and everything we need to support the other layers.
<br/><br/>

### Presentation
- <b>API</b>: This API provides all endpoints necessary to perform commands/queries to the Application. The routes are secured using policies that match to the user claims. 
- <b>SPA</b>: Built using the ASPNET Core Web Application with Angular template. This SPA is a simple and lightweight application using modular components to provide a functional workflow and usability. It contains Angular/Typescript Models, Services, Pipes, Guards, Interceptors and secured routes. 
- <b>Background Service</b>: Last but not least there's a background service listening to the Stored Events, processing and publishing them asynchronously. 
<hr>

### Mainly used technologies:
<ul>
  <li>
    <a href='https://get.asp.net' target="_blank">ASP.NET Core</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx' target="_blank">C#</a>
    for cross-platform back-end with:
    <ul>
      <li>.NET 5</li>
      <li>Entity Framework Core 5.0.5</li>
      <li>ASP.NET Core with JWT Bearer Authentication</li>
      <li>ASP.NET Identity Core</li>
      <li><a href='https://github.com/falberthen/BuildingBlocks.CQRS' target="_blank">BuildingBlocks.CQRS 2.0.0</a></li>
      <li>MediatR</li> 
      <li>Fluent Validation</li>
      <li>Automapper</li>
      <li>NSubstitute</li>
      <li>Swagger</li>
      <li>HealthChecks</li>      
    </ul>
  </li>
  <li>
    <a href='https://angular.io/' target="_blank">Angular 9</a> and <a href='http://www.typescriptlang.org/' target="_blank">TypeScript</a> for the front-end with:
    <ul>
      <li>Bootstrap</li>
      <li>NgBootstrap</li>
      <li>Font Awesome</li>
      <li>Toastr</li>
      <li>Angular JWT</li>
    </ul>
  </li>
</ul>

<hr/>

### What do you need to run it:

<ul>
  <li>The latest <a href="https://dotnet.microsoft.com/download" target="_blank">.NET Core SDK</a> and <a href="https://www.microsoft.com/en-us/sql-server/sql-server-downloads" target="_blank">SQL Server</a> for the back-end, and change the connection string values in appsettings.json if you need. <br>Of course you can use other database technologies, but you will have to setup Entity Framework to it.</li>
  <li><a href='https://nodejs.org' target="_blank">NodeJs</a> for the front-end in case you need to install packages.</li>
  <li>Set EcommerceDDD.DataSeed as Startup project and run it once to add some products.</li>
  <li>Set EcommerceDDD.WebApp as Startup project to use the SPA WebApp.</li>
</ul>

<b>When registering your customer for the first time, it will create the Identity tables. No database update / migration commands are needed.</b>
