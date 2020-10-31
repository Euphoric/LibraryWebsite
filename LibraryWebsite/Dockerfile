FROM mcr.microsoft.com/dotnet/core/sdk:3.1.302-alpine3.12 AS publish

# Cache NuGet packages
WORKDIR /src
COPY ["LibraryWebsite.sln", "."]
COPY ["LibraryWebsite/LibraryWebsite.csproj", "LibraryWebsite/"]
COPY ["LibraryWebsite.Client/LibraryWebsite.Client.csproj", "LibraryWebsite.Client/"]
COPY ["LibraryWebsite.Shared/LibraryWebsite.Shared.csproj", "LibraryWebsite.Shared/"]
COPY ["LibraryWebsite.Test/LibraryWebsite.Test.csproj", "LibraryWebsite.Test/"]
COPY ["LibraryWebsite.TestEndToEnd/LibraryWebsite.TestEndToEnd.csproj", "LibraryWebsite.TestEndToEnd/"]
COPY ["LibraryWebsite.TestDatabase/LibraryWebsite.TestDatabase.csproj", "LibraryWebsite.TestDatabase/"]
RUN dotnet restore

# copy remaining application files
COPY . .

# release app
WORKDIR "/src/LibraryWebsite"
RUN dotnet publish "LibraryWebsite.csproj" -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.6-alpine3.12 AS final

# enable globalization support
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

EXPOSE 80
EXPOSE 443

COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LibraryWebsite.dll"]