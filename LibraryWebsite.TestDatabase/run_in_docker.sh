#!/bin/bash

# Exit if any of the commands fail
set -e 

IntegrationTest/wait_for_it.sh $DATABASE_SERVER_WAIT -t 30

dotnet test LibraryWebsite.TestDatabase -c Release --logger "trx;LogFileName=../../TestResults/DatabaseTestResult.trx"
trx2junit ./TestResults/*.trx --output ./TestResults