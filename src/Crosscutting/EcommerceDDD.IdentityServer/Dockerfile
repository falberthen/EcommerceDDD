#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Crosscutting/EcommerceDDD.IdentityServer/EcommerceDDD.IdentityServer.csproj", "src/Crosscutting/EcommerceDDD.IdentityServer/"]
COPY ["src/Core/EcommerceDDD.Core/EcommerceDDD.Core.csproj", "src/Core/EcommerceDDD.Core/"]
RUN dotnet restore "src/Crosscutting/EcommerceDDD.IdentityServer/EcommerceDDD.IdentityServer.csproj"
COPY . .
WORKDIR "/src/src/Crosscutting/EcommerceDDD.IdentityServer"
RUN dotnet build "EcommerceDDD.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EcommerceDDD.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceDDD.IdentityServer.dll"]