#!/bin/sh

# Exit if any of the commands fail
set -e

case "$1" in

  "database")
    sh ./IntegrationTest/wait_for_it.sh $DATABASE_SERVER_WAIT -t 30
    dotnet test LibraryWebsite.TestDatabase -c Release --no-build --logger "trx;LogFileName=../../TestResults/DatabaseTestResult.trx"
    ;;

  "endToEnd")
    sh ./IntegrationTest/wait_for_it.sh $WEB_ADDRESS -t 30
    dotnet test LibraryWebsite.TestEndToEnd -c Release --no-build --logger "trx;LogFileName=../../TestResults/EndToEndTestResult.trx"
    ;;

  "unit_dotnet")
    dotnet test LibraryWebsite.Test -c Release --no-build --logger "trx;LogFileName=../../TestResults/UnitTestResult.trx"
    ;;
	
  *)
    exit 1
    ;;
esac

trx2junit ./TestResults/*.trx --output ./TestResults