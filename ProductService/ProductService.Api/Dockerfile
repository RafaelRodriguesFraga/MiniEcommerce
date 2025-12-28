FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8082
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Commons
COPY ["Commons/Commons.csproj", "Commons/"]

# ProductService
COPY ["ProductService/ProductService.Api/ProductService.Api.csproj", "ProductService/ProductService.Api/"]
COPY ["ProductService/ProductService.Application/ProductService.Application.csproj", "ProductService/ProductService.Application/"]
COPY ["ProductService/ProductService.Domain/ProductService.Domain.csproj", "ProductService/ProductService.Domain/"]
COPY ["ProductService/ProductService.Infra/ProductService.Infra.csproj", "ProductService/ProductService.Infra/"]

RUN dotnet restore "ProductService/ProductService.Api/ProductService.Api.csproj"

COPY . .

WORKDIR "/src/ProductService/ProductService.Api"
RUN dotnet build "ProductService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProductService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.Api.dll"]
