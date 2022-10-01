#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Crosscutting/EcommerceDDD.ApiGateway/EcommerceDDD.ApiGateway.csproj", "src/Crosscutting/EcommerceDDD.ApiGateway/"]
RUN dotnet restore "src/Crosscutting/EcommerceDDD.ApiGateway/EcommerceDDD.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/Crosscutting/EcommerceDDD.ApiGateway"
RUN dotnet build "EcommerceDDD.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EcommerceDDD.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceDDD.ApiGateway.dll"]