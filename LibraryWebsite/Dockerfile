FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview8-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_10.x | bash - && \
    apt-get install -y nodejs
# End setup

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview8-buster AS publish
# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_10.x | bash - && \
    apt-get install -y build-essential nodejs
# End setup

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

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LibraryWebsite.dll"]