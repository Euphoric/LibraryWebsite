FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["LibraryWebsite/LibraryWebsite.csproj", "LibraryWebsite/"]
RUN dotnet restore "LibraryWebsite/LibraryWebsite.csproj"
COPY . .
WORKDIR "/src/LibraryWebsite"
RUN dotnet build "LibraryWebsite.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LibraryWebsite.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LibraryWebsite.dll"]