#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["kchat.UI/kchat.UI.csproj", "kchat.UI/"]
COPY ["kchat.kafka/kchat.kafka.csproj", "kchat.kafka/"]
RUN dotnet restore "kchat.UI/kchat.UI.csproj"
COPY . .
WORKDIR "/src/kchat.UI"
RUN dotnet build "kchat.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "kchat.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "kchat.UI.dll"]