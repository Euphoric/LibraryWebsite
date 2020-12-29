FROM mcr.microsoft.com/dotnet/sdk:5.0.100-alpine3.12 AS publish

# Cache NuGet packages
WORKDIR /src
COPY ["LibraryWebsite.sln", "."]
COPY ["LibraryWebsite/LibraryWebsite.csproj", "LibraryWebsite/"]
COPY ["LibraryWebsite.Client/LibraryWebsite.Client.csproj", "LibraryWebsite.Client/"]
COPY ["LibraryWebsite.Shared/LibraryWebsite.Shared.csproj", "LibraryWebsite.Shared/"]
COPY ["Euphoric.EventModel/Euphoric.EventModel.csproj", "Euphoric.EventModel/"]
COPY ["LibraryWebsite.Test/LibraryWebsite.Test.csproj", "LibraryWebsite.Test/"]
COPY ["LibraryWebsite.TestEndToEnd/LibraryWebsite.TestEndToEnd.csproj", "LibraryWebsite.TestEndToEnd/"]
RUN dotnet restore

# copy remaining application files
COPY . .

# release app
WORKDIR "/src/LibraryWebsite"
RUN dotnet publish "LibraryWebsite.csproj" -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0.0-alpine3.12 AS final

# enable globalization support
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

EXPOSE 80
EXPOSE 443

COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LibraryWebsite.dll"]