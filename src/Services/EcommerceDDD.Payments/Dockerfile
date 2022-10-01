#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/EcommerceDDD.Payments/EcommerceDDD.Payments.csproj", "src/Services/EcommerceDDD.Payments/"]
COPY ["src/Core/EcommerceDDD.Core.Infrastructure/EcommerceDDD.Core.Infrastructure.csproj", "src/Core/EcommerceDDD.Core.Infrastructure/"]
COPY ["src/Core/EcommerceDDD.Core/EcommerceDDD.Core.csproj", "src/Core/EcommerceDDD.Core/"]
RUN dotnet restore "src/Services/EcommerceDDD.Payments/EcommerceDDD.Payments.csproj"
COPY . .
WORKDIR "/src/src/Services/EcommerceDDD.Payments"
RUN dotnet build "EcommerceDDD.Payments.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EcommerceDDD.Payments.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceDDD.Payments.dll"]