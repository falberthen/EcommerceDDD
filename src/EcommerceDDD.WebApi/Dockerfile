#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/EcommerceDDD.WebApi/EcommerceDDD.WebApi.csproj", "src/EcommerceDDD.WebApi/"]
COPY ["src/EcommerceDDD.Infrastructure.IoC/EcommerceDDD.Infrastructure.IoC.csproj", "src/EcommerceDDD.Infrastructure.IoC/"]
COPY ["src/EcommerceDDD.Infrastructure/EcommerceDDD.Infrastructure.csproj", "src/EcommerceDDD.Infrastructure/"]
COPY ["src/EcommerceDDD.Domain/EcommerceDDD.Domain.csproj", "src/EcommerceDDD.Domain/"]
COPY ["src/EcommerceDDD.Domain.SeedWork/EcommerceDDD.Domain.SeedWork.csproj", "src/EcommerceDDD.Domain.SeedWork/"]
COPY ["src/EcommerceDDD.Application/EcommerceDDD.Application.csproj", "src/EcommerceDDD.Application/"]
RUN dotnet restore "src/EcommerceDDD.WebApi/EcommerceDDD.WebApi.csproj"
COPY . .
WORKDIR "/src/src/EcommerceDDD.WebApi"
RUN dotnet build "EcommerceDDD.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EcommerceDDD.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceDDD.WebApi.dll"]
