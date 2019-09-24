# LibraryWebsite
Website implementing simple Library application. Demonstrates lots of infrastructure concepts of using ASP.NET Core 3.0, Angular, CI/CD, etc..
Or it should, but not yet.. as it is heavily in work in progress.

[![CircleCI](https://circleci.com/gh/Euphoric/LibraryWebsite/tree/master.svg?style=svg)](https://circleci.com/gh/Euphoric/LibraryWebsite/tree/master)

## Dependencies

Requires .NET Core 3.0

## Running

Simplest way to run whole application is to use docker-compose.
Running below command will start up whole application environment.

    docker-compose -f docker-compose.runlocal.yml up --build

The application can be run outside docker as long as you have an SQL Server available and change connection string in `appsettings.json` appropriately.
Setting `MigrateOnStartup` in `appsettings.json` will create a database and migrate to newest schema.

### .NET Unit tests

.NET Unit tests are in `LibraryWebsite.Test`

    dotnet test LibraryWebsite.Test

### Angular tests

Angular client is in `LibraryWebsite/ClientApp` directory. 

	cd LibraryWebsite/ClientApp/
	npm run test

### Database tests

Database tests test Entity Framework and it's migrations

    .\run_database_test.ps1

### End to End tests

EndToEnd tests are in .NET in `LibraryWebsite.TestEndToEnd`.

To run these tests, whole application environment must be running.
Running tests directly can be simply done using

	.\run_endToEnd_test.ps1

To debug the tests individually, start up the environment using 

    docker-compose -f docker-compose.runlocal.yml up --build

and then run the individual tests from IDE

## Continuous Integration

Docker image published to Docker Hub : https://hub.docker.com/r/radekfalhar/librarywebsite
