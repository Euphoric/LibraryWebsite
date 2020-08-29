FROM mcr.microsoft.com/dotnet/core/sdk:3.1.302-alpine3.12 AS publish
# Setup NodeJs
RUN apk update
RUN apk add --update npm=12.18.3-r0

# Cache NuGet packages
WORKDIR /src
COPY ["LibraryWebsite/LibraryWebsite.csproj", "LibraryWebsite/"]
RUN dotnet restore "LibraryWebsite/LibraryWebsite.csproj"

# Cache node_modules
WORKDIR /src
COPY ["LibraryWebsite/ClientApp/package.json","LibraryWebsite/ClientApp/package-lock.json", "LibraryWebsite/ClientApp/"]
RUN cd /src/LibraryWebsite/ClientApp; npm install

# copy remaining application files
COPY . .

# release app
WORKDIR "/src/LibraryWebsite"
RUN dotnet publish "LibraryWebsite.csproj" -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.6-alpine3.12 AS final
# Setup NodeJs
RUN apk update
RUN apk add --update npm=12.18.3-r0

# enable globalization support
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

EXPOSE 80
EXPOSE 443

COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LibraryWebsite.dll"]