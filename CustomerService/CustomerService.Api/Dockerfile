FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8081
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Commons/Commons.csproj", "Commons/"]
COPY ["CustomerService/CustomerService.Api/CustomerService.Api.csproj", "CustomerService/CustomerService.Api/"]
COPY ["CustomerService/CustomerService.Application/CustomerService.Application.csproj", "CustomerService/CustomerService.Application/"]
COPY ["CustomerService/CustomerService.Domain/CustomerService.Domain.csproj", "CustomerService/CustomerService.Domain/"]
COPY ["CustomerService/CustomerService.Infra/CustomerService.Infra.csproj", "CustomerService/CustomerService.Infra/"]
COPY ["CustomerService/CustomerService.Shared/CustomerService.Shared.csproj", "CustomerService/CustomerService.Shared/"]
COPY ["CustomerService/CustomerService.Infra.CrossCutting/CustomerService.Infra.CrossCutting.csproj", "CustomerService/CustomerService.Infra.CrossCutting/"]

RUN dotnet restore "CustomerService/CustomerService.Api/CustomerService.Api.csproj"

# Copia tudo agora
COPY . .

WORKDIR "/src/CustomerService/CustomerService.Api"
RUN dotnet build "CustomerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "CustomerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomerService.Api.dll"]
