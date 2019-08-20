#!/bin/bash

# Exit if any of the commands fail
set -e 

IntegrationTest/wait_for_it.sh $WEB_ADDRESS -t 30

dotnet test LibraryWebsite.TestEndToEnd -c Release --logger "trx;LogFileName=../../TestResults/EndToEndTestResult.trx"
trx2junit ./TestResults/*.trx --output ./TestResults