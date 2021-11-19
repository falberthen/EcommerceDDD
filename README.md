![Build](https://github.com/falberthen/ecommerceddd/actions/workflows/ecommerceddd.yml/badge.svg)
[![License](https://img.shields.io/github/license/falberthen/ecommerceddd.svg)](LICENSE)

## Welcome to Ecommerce DDD
This project is a showcase of a full-stack application I use to combine several cutting-edge technologies and architectural patterns. It is based on Domain-Driven Design at its core for implementing a clean-architectured API that powers up an Angular-based SPA application.
<br><br>
Thanks for getting here! please <b>give a ⭐</b> if you liked the project. It motivates me to keep improving it.
<br><br>

## Screenshots

<a href="images/login.png" target="_blank">
<img src="images/login.png"/>
</a>

<br/><a href="images/products.png" target="_blank">
<img src="images/products.png" />
</a>

<br/><a href="images/orders.png" target="_blank">
<img src="images/orders.png" />
</a>

<br/><a href="images/events.png" target="_blank">
<img src="images/events.png" />
</a>

<br/><a href="images/order-events.png" target="_blank">
<img src="images/order-events.png" />
</a>

<br>

## Architecture 
    
### Domain
This is where the business logic resides, with a structured implementation of the domain through aggregates, entities, value objects, domain services, repository definitions, and domain events.
<br/>

### Domain SeedWork
It defines the domain building blocks, such as entities, value objects, aggregate root, repositories, services and so on.
<br/>

### Application
It orchestrates the interactions between the external world (API/SPA) and the domain. It is concerned with defining the jobs needed to be done to accomplish a certain application task. Since the project is based on CQRS/EventSourcing architecture, it defines and handles commands, queries and events.
<br/>

### Infrastructure
It takes care of the application's infrastructure and issues not related to the business itself. It is responsible for database mapping (ORM), domain repository implementation, identity authentication and user claims, JWT authentication, tooling for processing and publishing messages, Inversion of Control and everything needed to support the upper layers.
<br/>

### Presentation
- <b>Web API</b>: A restful API providing endpoints with secured routes based on user claims. 
  It also implements and host a <b>Background Service</b> for processing and publishing stored events.
- <b>SPA</b>: A lightweight Angular-based application providing a functional and user-friendly UI.

<br>

## Technologies used

<ul>
  <li>
    <a href='https://get.asp.net' target="_blank">ASP.NET Core</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx' target="_blank">C# 10</a>
    for cross-platform back-end with:
    <ul>
      <li>.NET 6</li>
      <li>Entity Framework Core 6</li>
      <li>ASP.NET Core Web API</li>
      <li>ASP.NET Core Identity</li>
      <li>SignalR Core</li>
      <li>JWT Bearer Authentication</li>
      <li>MediatR</li> 
      <li>Fluent Validation</li>
      <li>Automapper</li>
      <li>NSubstitute</li>
      <li>Swagger</li>
      <li>Docker Compose</li>
    </ul>
  </li>
  <li>
    <a href='https://angular.io/' target="_blank">Angular 12</a> and <a href='http://www.typescriptlang.org/' target="_blank">TypeScript</a> for the front-end with:
    <ul>
      <li>NgBootstrap</li>
      <li>Font Awesome</li>
      <li>Toastr</li>
      <li>Angular JWT</li>
    </ul>
  </li>
</ul>

<br>

## What do you need to run 

- The latest <a href="https://dotnet.microsoft.com/download" target="_blank">.NET Core SDK</a> and <a href="https://www.microsoft.com/en-us/sql-server/sql-server-downloads" target="_blank">SQL Server</a> for the database.
- <a href='https://nodejs.org' target="_blank">NodeJs</a> for the front-end.
- Optional: <a href="https://docs.docker.com/docker-for-windows/wsl/" target="_blank">Docker Desktop with support for WLS 2</a>

#### Running the WebAPI
    
Set `EcommerceDDD.Api` as the `Startup project` and run
```console
 $ docker-compose up --build
``` 

#### Running the Angular SPA
    
Using a terminal, navigate to `EcommerceDDD.Spa` and run for the node packages and serving the SPA on `http://localhost:4200` respectively:

```console
 $ npm install
 $ ng serve
```

#### Notes:
- The `docker-compose.yml` is targeting the OS to `Linux` and setting up the with `SQL Server 2019 for Ubuntu`
- When registering your first customer, it will create the database structure automatically. DataSeeder will add some products for you if the Products table is empty.