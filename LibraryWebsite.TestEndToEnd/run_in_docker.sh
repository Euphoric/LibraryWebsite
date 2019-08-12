#!/bin/bash

# Exit if any of the commands fail
set -e 

IntegrationTest/wait_for_it.sh $WEB_ADDRESS -t 30
dotnet test LibraryWebsite.TestEndToEnd -c Release --logger:trx