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
generic_proto_path ?= ../../d1-service-generic/protobuf
storage_proto_path ?= ../../d1-service-storage/protobuf

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
.PHONY: protos
protos: ## Copy protos to build directory and cleanup files to avoid .NET build warnings
	mkdir -p ./src/protos
	cp $(generic_proto_path)/*.proto ./src/protos
	cp $(storage_proto_path)/*.proto ./src/protos
	# For some reason the documentation comments doesn't like the comments in the protobuf files
	# that starts with '///', instead it causes a lot of compilation warnings. Changing comments to '//'
	# fixes the issue
	grep -r -l "///" ./src/protos | xargs sed -i 's/\/\/\//\/\//g'

.PHONY: build
build: protos ## Build
	dotnet build

.PHONY: tests
tests: docker-core-tests docker-objects-tests ## Run all tests

.PHONY: docker-core-tests
docker-core-tests: protos docker-core-tests-up ## Run Core tests
	USER_INFO=$$(docker exec d1-service-generic /d1-service-generic create-user rcudiom  | tail -n 1) && \
		export E2E_TEST_UID=$$(echo $$USER_INFO | jq -r ".user_id") && \
		export E2E_TEST_PASS=$$(echo $$USER_INFO | jq -r ".password") && \
		dotnet test --filter Category!=Objects
	@make docker-core-tests-down

.PHONY: docker-core-tests-up
docker-core-tests-up: ## Start docker Core test environment
	cd tests && \
		docker-compose --profile core up -d

.PHONY: docker-core-tests-down
docker-core-tests-down: ## Stop docker Core test environment
	docker-compose --profile core -f tests/compose.yaml down -v

.PHONY: docker-objects-tests
docker-objects-tests: protos docker-objects-tests-up ## Run objects tests
	USER_INFO=$$(docker exec d1-service-storage /d1-service-storage create-user rcudiom  | tail -n 1) && \
		export E2E_TEST_UID=$$(echo $$USER_INFO | jq -r ".user_id") && \
		export E2E_TEST_PASS=$$(echo $$USER_INFO | jq -r ".password") && \
		dotnet test --filter Category!=Core
	@make docker-objects-tests-down

.PHONY: docker-objects-tests-up
docker-objects-tests-up: ## Start docker Objects test environment
	cd tests && \
		docker-compose --profile objects up -d

.PHONY: docker-objects-test-down
docker-objects-tests-down: ## Stop docker Objects test environment
	docker-compose --profile objects -f tests/compose.yaml down -v

.PHONY: nuget-pack
nuget-pack: protos ## Pack the library
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
	rm -rf ./src/protos