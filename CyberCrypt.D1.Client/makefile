# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Config #####
# Change shell to bash to allow sourcing of env variables
SHELL := /bin/bash

publishDir = ./artifacts/publish
library = "$(publishDir)/CyberCrypt.D1.Client.dll"
apiDocsDir = ../documentation/api

##### Files #####

# Check that given variables are set and all have non-empty values,
# die with an error otherwise.
#
# Params:
#   1. Variable name(s) to test.
#   2. (optional) Error message to print.
check_defined = \
    $(strip $(foreach 1,$1, \
        $(call __check_defined,$1,$(strip $(value 2)))))
__check_defined = \
    $(if $(value $1),, \
      $(error Undefined $1$(if $2, ($2))))

##### Build targets #####
.PHONY: build
build: ## Build
	dotnet build

.PHONY: tests
tests: docker-generic-tests docker-storage-tests ## Run all tests

.PHONY: docker-generic-tests
docker-generic-tests: docker-generic-tests-up ## Run Generic tests
	USER_INFO=$$(docker exec d1-service-generic /d1-service-generic create-user | tail -n 1) && \
		export E2E_TEST_UID=$$(echo $$USER_INFO | jq -r ".user_id") && \
		export E2E_TEST_PASS=$$(echo $$USER_INFO | jq -r ".password") && \
		dotnet test --filter Category!=Storage
	@make docker-generic-tests-down

.PHONY: docker-generic-tests-up
docker-generic-tests-up: ## Start docker Generic test environment
	cd tests && \
		docker-compose --profile generic up -d

.PHONY: docker-generic-tests-down
docker-generic-tests-down: ## Stop docker Generic test environment
	docker-compose --profile generic -f tests/compose.yaml down -v

.PHONY: docker-storage-tests
docker-storage-tests: docker-storage-tests-up ## Run Storage tests
	USER_INFO=$$(docker exec d1-service-storage /d1-service-storage create-user | tail -n 1) && \
		export E2E_TEST_UID=$$(echo $$USER_INFO | jq -r ".user_id") && \
		export E2E_TEST_PASS=$$(echo $$USER_INFO | jq -r ".password") && \
		dotnet test --filter Category!=Generic
	@make docker-storage-tests-down

.PHONY: docker-storage-tests-up
docker-storage-tests-up: ## Start docker Storage test environment
	cd tests && \
		docker-compose --profile storage up -d

.PHONY: docker-storage-test-down
docker-storage-tests-down: ## Stop docker Storage test environment
	docker-compose --profile storage -f tests/compose.yaml down -v

.PHONY: nuget-pack
nuget-pack: ## Pack the library
	$(call check_defined, VERSION)
	dotnet pack --output ./artifacts/ --configuration Release /p:Version=${VERSION}

.PHONY: nuget-publish
nuget-publish: nuget-pack ## Publish the NuGet package
	$(call check_defined, PACKAGE_SOURCE, API_KEY)
	dotnet nuget push ./artifacts/CyberCrypt.D1.Client.${VERSION}.nupkg --source "${PACKAGE_SOURCE}" --api-key "${API_KEY}"

.PHONY: publish
publish: ## Publish the library and its dependencies to a local directory
	dotnet publish -o $(publishDir) --configuration Release ./src/

.PHONY: apidocs
apidocs: publish ## Generate API documentation
	xmldocmd $(library) $(apiDocsDir)

.PHONY: apidocs-verify
apidocs-verify: publish ## Verify API documentation is up-to-date
	git ls-files --other --modified --deleted --exclude-standard -- $(apiDocsDir)  | sed q1

##### Cleanup targets #####
.PHONY: clean
clean: ## Remove build artifacts
	dotnet clean
	rm -rf ./artifacts/
