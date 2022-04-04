# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Config #####
# Change shell to bash to allow sourcing of env variables
SHELL := /bin/bash

##### Files #####
test_env = ./env/test_env

##### Build targets #####
.PHONY: build
build:
	dotnet build ./src

.PHONY: test
test: build
# TODO: Setup env variables in script
	source $(test_env) && \
	dotnet test ./tests/Encryptonize.Client.Tests --no-build

.PHONY: pack
pack: build
	dotnet pack ./src --output ./artifacts/ --configuration Release

##### Cleanup targets #####
.PHONY: clean  ## Remove build artifacts
clean :
	dotnet clean ./src
	dotnet clean ./tests/Encryptonize.Client.Tests
	rm -rf ./artifacts/
