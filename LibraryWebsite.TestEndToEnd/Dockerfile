FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview7-buster AS base

# Cache NuGet packages
WORKDIR /src
COPY ["LibraryWebsite.TestEndToEnd/*.csproj", "LibraryWebsite.TestEndToEnd/"]
RUN dotnet restore "LibraryWebsite.TestEndToEnd"

# copy remaining application files
COPY . .

# build tests 
RUN dotnet build "LibraryWebsite.TestEndToEnd" -c Release
ENTRYPOINT ["bash", "./LibraryWebsite.TestEndToEnd/run_in_docker.sh"]