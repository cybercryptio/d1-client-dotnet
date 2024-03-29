# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Config #####
SHELL := /bin/bash

##### Build targets #####
.PHONY: build
build: # Build all clients
	@make build-client
	@make apidocs

.PHONY: build-client
build-client: ## Build D1 client
	@make -C CyberCrypt.D1.Client build

.PHONY: tests
tests: ## Run all tests
	@make client-tests

.PHONY: client-tests
client-tests: ## Run D1 client tests
	@make -C CyberCrypt.D1.Client tests

.PHONY: nuget-publish
nuget-publish: ## Publish packages
	@make -C CyberCrypt.D1.Client nuget-publish

.PHONY: apidocs
apidocs: ## Generate API documentation
	@make -C CyberCrypt.D1.Client apidocs

.PHONY: apidocs-verify
apidocs-verify: ## Verify API documentation is up-to-date
	@make -C CyberCrypt.D1.Client apidocs-verify

##### Cleanup targets #####
.PHONY: clean
clean: ## Remove build artifacts
	@make -C CyberCrypt.D1.Client clean
