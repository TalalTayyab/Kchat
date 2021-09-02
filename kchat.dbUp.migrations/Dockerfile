#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["kchat.dbUp.migrations.csproj", "kchat.dbUp.migrations/"]
RUN dotnet restore "kchat.dbUp.migrations/kchat.dbUp.migrations.csproj"

WORKDIR "/src/kchat.dbUp.migrations"
COPY . .
RUN dotnet build "kchat.dbUp.migrations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "kchat.dbUp.migrations.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "kchat.dbUp.migrations.dll"]
CMD [\"${DB_CONNECTION}\"]