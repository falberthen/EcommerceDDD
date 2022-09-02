![Build](https://github.com/falberthen/ecommerceddd/actions/workflows/ecommerceddd.yml/badge.svg)
[![License](https://img.shields.io/github/license/falberthen/ecommerceddd.svg)](LICENSE)

## Welcome to Ecommerce DDD
This project is an experimental full-stack application I use to combine several cutting-edge technologies and architectural patterns.\
Thanks for getting here! please <b>give a ⭐</b> if you liked the project. It motivates me to keep improving it.
<br><br>

<a href="images/ecommerceddd-1.gif" target="_blank">
<img src="images/ecommerceddd-1.gif" width="600px"/>
</a>

<a href="images/ecommerceddd-2.gif" target="_blank">
<img src="images/ecommerceddd-2.gif" width="600px"/>
</a>

<br/><br/>


The overall architecture is organized with `Core`, `Crosscutting` and `Services`.

<br/>

## Core
It defines all the building blocks and abstractions to be used on every underlying project.

## Core.Infrastructure
It implements infrastructure matters to be used by microservices. Also, it centralizes third-party packages.

<br/>

## Crosscutting
It contains projects with logic needed to cross over the microservices, such as `IdentityServer4` token logic, `API gateway` and `Integration services` to allow microservices to communicate to each other through http.

<br/>

## Services
The microservices composing the back-end, are built to be as compact as possible, meaning they're structured to have not only the business logic related to the domain but also to expose it through a self-contained API.

It is structured with: `Domain`, `Application`, `API`, `Infrastructure` (when apply).

#### - Domain
This is where the business logic resides, with a structured implementation of the domain through aggregates, entities, value objects, domain services, repository definitions, and domain events.

#### - Application
It orchestrates the interactions between the external world (API/SPA) and the domain. It is concerned with defining the jobs needed to be done to accomplish a certain application task. Since the project is based on CQRS/EventSourcing architecture, it defines and handles commands, queries and events.

#### - Infrastructure
It handles infrastructure matters that are not related to the business itself.

<br/>

## Presentation
A lightweight Angular-based `SPA` providing a functional and user-friendly UI.

<br/>

## Technologies used

<ul>
  <li>
    <a href='https://get.asp.net' target="_blank">ASP.NET</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx' target="_blank">C# 10</a>
    for cross-platform back-end with:
    <ul>
      <li>.NET 6</li>
      <li>ASP.NET Core Minimal API</li>
      <li>Ocelot</li>
      <li>Marten</li>
      <li>Postgres</li>
      <li>Entity Framework Core 6</li>
      <li>ASP.NET Core Identity</li>
      <li>JWT Bearer Authentication</li>
      <li>IdentityServer4</li>
      <li>SignalR Core</li>
      <li>MediatR</li>
      <li>Fluent Validation</li>
      <li>XUnit / Mock</li>
      <li>Swagger</li>
      <li>Kafka</li>
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

<br/>

## What do you need to run 

### Running the back-end

The project was designed to be easily run within docker containers, hence all you need is 1 command line to up everything. Make sure you have installed `Docker` and have fun!

- Docker: <a href="https://docs.docker.com/docker-for-windows/wsl/" target="_blank">Docker Desktop with support for WLS 2</a>
    
```console
 $ docker-compose up
``` 

You can also set the `docker-compose` project as startup on Visual Studio if you want to run it while debugging. 

<br/>

### Running the Angular SPA
    
Using a terminal, navigate to `EcommerceDDD.Spa` and run for the node packages and serving the SPA on `http://localhost:4200` respectively:

```console
 $ npm install
 $ ng serve
```
