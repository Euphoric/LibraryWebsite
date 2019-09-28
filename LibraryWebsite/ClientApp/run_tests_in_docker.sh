#!/bin/sh

# Exit if any of the commands fail
set -e

npm run test -- --no-watch --no-progress --browsers=ChromeHeadlessCI