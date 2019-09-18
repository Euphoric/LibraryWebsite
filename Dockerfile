################################### Project build ###########################################

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview8-buster-slim AS runtime_image
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

################################## Database test #####################################

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview8-buster AS database_test

# install trx2junit
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install -g trx2junit

# Cache NuGet packages
WORKDIR /src
COPY ["LibraryWebsite/*.csproj", "LibraryWebsite/"]
COPY ["LibraryWebsite.TestDatabase/*.csproj", "LibraryWebsite.TestDatabase/"]
RUN dotnet restore "LibraryWebsite.TestDatabase"

# copy remaining application files
COPY . .

# build test project
RUN dotnet build "LibraryWebsite.TestDatabase" -c Release

# enable execution permission for shell files
RUN ["chmod", "+x", "./LibraryWebsite.TestDatabase/run_in_docker.sh"]
RUN ["chmod", "+x", "./IntegrationTest/wait_for_it.sh"]

ENTRYPOINT ["bash", "./LibraryWebsite.TestDatabase/run_in_docker.sh"]

################################## End to end tests #####################################

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview8-buster AS end_to_end

# install trx2junit
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install -g trx2junit

# Cache NuGet packages
WORKDIR /src
COPY ["LibraryWebsite.TestEndToEnd/*.csproj", "LibraryWebsite.TestEndToEnd/"]
RUN dotnet restore "LibraryWebsite.TestEndToEnd"

# copy remaining application files
COPY . .

# build tests 
RUN dotnet build "LibraryWebsite.TestEndToEnd" -c Release

# enable execution permission for shell files
RUN ["chmod", "+x", "./LibraryWebsite.TestEndToEnd/run_in_docker.sh"]
RUN ["chmod", "+x", "./IntegrationTest/wait_for_it.sh"]

ENTRYPOINT ["bash", "./LibraryWebsite.TestEndToEnd/run_in_docker.sh"]


################################## Release image #########################################
# Build runtime image
FROM runtime_image AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LibraryWebsite.dll"]