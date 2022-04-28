# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Config #####
SHELL := /bin/bash

##### Files #####
# TODO Update
export proto_path = ../../encryptonize-core/integrations/protobuf

##### Build targets #####
.PHONY: build
build: # Build all clients
	@make build-client

.PHONY: build-client
build-client: ## Build Encryptonize client
	@make -C Encryptonize.Client build

.PHONY: test
test: ## Run all tests
	@make test-client

.PHONY: test-client
test-client: ## Run Encryptonize client tests
	@make -C Encryptonize.Client test

.PHONY: publish
publish: ## Publish packages
	@make -C Encryptonize.Client publish

##### Cleanup targets #####
.PHONY: clean  ## Remove build artifacts
clean :
	@make -C Encryptonize.Client clean
