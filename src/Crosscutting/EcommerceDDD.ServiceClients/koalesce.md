## Generating API gateway single OpenAPI definition with Koalesce

## Install Koalesce.OpenAPI.CLI 
dotnet tool install --global Koalesce.OpenAPI.CLI --version 0.1.1-alpha.1

### Execute it while running the solution if you want to update this apigateway.yaml to generate new Kiota clients
koalesce --config .your-path\EcommerceDDD\src\Crosscutting\EcommerceDDD.ApiGateway\appsettings.json --output .your-path\EcommerceDDD\src\Crosscutting\EcommerceDDD.ServiceClients\apigateway.yaml
