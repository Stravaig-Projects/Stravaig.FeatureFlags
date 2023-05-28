#!/bin/sh

set -e

git config --global --add safe.directory /workspaces/Stravaig.FeatureFlags

gem install bundler
bundle install

SCRIPT_DIR=$( dirname "$0" )
