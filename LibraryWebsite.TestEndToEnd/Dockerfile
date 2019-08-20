FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview7-buster AS base

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