#!/bin/sh

set -e

git config --global --add safe.directory /workspaces/Stravaig.FeatureFlags

gem install bundler
bundle install --gemfile=/workspaces/Stravaig.FeatureFlags/docs/Gemfile

SCRIPT_DIR=$( dirname "$0" )

bash --version
pwsh --version

echo Gem Version:
gem --version

jekyll --version
bundle --version