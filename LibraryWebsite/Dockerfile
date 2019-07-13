FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_6.x | bash - && \
    apt-get install -y build-essential nodejs
# End setup

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_6.x | bash - && \
    apt-get install -y build-essential nodejs
# End setup
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