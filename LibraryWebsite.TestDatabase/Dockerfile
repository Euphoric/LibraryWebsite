FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview8-buster AS base

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