#!/bin/bash
set -e

# Add dotnet tools to PATH
export PATH="$PATH:/root/.dotnet/tools"

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== BACKEND CLIENTS (Service-to-Service) ===${NC}"
echo ""

declare -A BACKEND_SERVICES=(
  ["SignalR"]="http://ecommerceddd-signalr/swagger/v2/swagger.json|SignalR"
  ["IdentityServer"]="http://ecommerceddd-identityserver/swagger/v2/swagger.json|IdentityServer"
  ["ProductCatalog"]="http://ecommerceddd-product-catalog/swagger/v2/swagger.json|ProductCatalog"
  ["InventoryManagement"]="http://ecommerceddd-inventory-management/swagger/v2/swagger.json|InventoryManagement"
  ["CustomerManagement"]="http://ecommerceddd-customer-management/swagger/v2/swagger.json|CustomerManagement"
  ["QuoteManagement"]="http://ecommerceddd-quote-management/swagger/v2/swagger.json|QuoteManagement"
  ["PaymentProcessing"]="http://ecommerceddd-payment-processing/swagger/v2/swagger.json|PaymentProcessing"
  ["ShipmentProcessing"]="http://ecommerceddd-shipment-processing/swagger/v2/swagger.json|ShipmentProcessing"
)

for service in "${!BACKEND_SERVICES[@]}"; do
  IFS='|' read -r url namespace <<< "${BACKEND_SERVICES[$service]}"
  echo -e "→ Generating ${GREEN}${service}Client${NC}..."
  kiota generate -l CSharp \
    -c "${service}Client" \
    -n "EcommerceDDD.ServiceClients.${namespace}" \
    -d "$url" \
    -o "/src/Crosscutting/EcommerceDDD.ServiceClients/Kiota/${service}" \
    --clean-output
  echo ""
done

echo -e "${BLUE}=== FRONTEND CLIENT (Unified via Gateway + Koalesce) ===${NC}"
echo ""
echo -e "→ Generating ${GREEN}TypeScript apiClient${NC}..."
kiota generate -l TypeScript \
  -c apiClient \
  -d http://ecommerceddd-apigateway/swagger/v2/apigateway.yaml \
  -o /src/EcommerceDDD.Spa/src/app/clients \
  --clean-output

echo ""
echo -e "${GREEN}✓ All clients regenerated successfully${NC}"
