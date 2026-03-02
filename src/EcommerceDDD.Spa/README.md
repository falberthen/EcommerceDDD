### Running the Angular SPA

Using a terminal, navigate to `EcommerceDDD.Spa` and run for the following commands the node packages and serving the SPA on `http://localhost:4200` respectively:

```console
 $ npm install #first time only
 $ npm run start #or ng serve 
```

### E2E Testing with Cypress

Cypress runs full browser-based end-to-end tests against the SPA, validating real user flows (for example login, product selection, cart management, and checkout) across routing, UI behavior, and backend integration.

Use the commands below from `src/EcommerceDDD.Spa`:

```console
 $ npm run e2e
```

Runs Cypress through Angular CLI (`ng e2e`), using the project e2e target from `angular.json`. In this project, that target starts the Angular dev server and launches Cypress in interactive watch mode, which is best for local development and debugging.

```console
 $ npm run cypress:run
```

Runs Cypress headlessly for automation/CI use cases.
