# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Config #####
# Change shell to bash to allow sourcing of env variables
SHELL := /bin/bash

##### Files #####
# TODO Update
proto_path ?= ../../encryptonize-core/integrations/protobuf

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
protos: ## Copy protos to build directory
	mkdir -p ./src/protos
	cp $(proto_path)/*.proto ./src/protos

.PHONY: build
build: protos ## Build
	dotnet build

.PHONY: test
test: docker-eaas-test docker-objects-test ## Run all tests

.PHONY: docker-eaas-test
docker-eaas-test: docker-eaas-test-up## Run EAAS tests
	USER_INFO=$$(docker exec encryptonize-eaas /eaas create-user rcudiom  | tail -n 1) && \
		export E2E_TEST_UID=$$(echo $$USER_INFO | jq -r ".user_id") && \
		export E2E_TEST_PASS=$$(echo $$USER_INFO | jq -r ".password") && \
		dotnet test --no-build --filter Category!=Objects
	@make docker-eaas-test-down

.PHONY: docker-eaas-test-up
docker-eaas-test-up: ## Start docker EAAS test environment
	cd tests && \
		docker-compose --profile eaas up -d

.PHONY: docker-eaas-test-down
docker-eaas-test-down: ## Stop docker EAAS test environment
	docker-compose --profile eaas -f tests/compose.yaml down

.PHONY: docker-objects-test
docker-objects-test: docker-objects-test-up ## Run objects tests
	USER_INFO=$$(docker exec encryptonize-objects /objects create-user rcudiom  | tail -n 1) && \
		export E2E_TEST_UID=$$(echo $$USER_INFO | jq -r ".user_id") && \
		export E2E_TEST_PASS=$$(echo $$USER_INFO | jq -r ".password") && \
		dotnet test --no-build --filter Category!=EAAS
	@make docker-objects-test-down

.PHONY: docker-objects-test-up
docker-objects-test-up: ## Start docker Objects test environment
	cd tests && \
		docker-compose --profile objects up -d

.PHONY: docker-objects-test-down
docker-objects-test-down: ## Stop docker Objects test environment
	docker-compose --profile objects -f tests/compose.yaml down

.PHONY: pack
pack: protos ## Pack the library
	$(call check_defined, VERSION)
	dotnet pack --output ./artifacts/ --configuration Release /p:Version=${VERSION}

.PHONY: publish
publish: pack ## Publish the NuGet package
	$(call check_defined, PACKAGE_SOURCE, API_KEY)
	dotnet nuget push ./artifacts/Encryptonize.Client.${VERSION}.nupkg --source "${PACKAGE_SOURCE}" --api-key "${API_KEY}"

##### Cleanup targets #####
.PHONY: clean  ## Remove build artifacts
clean :
	dotnet clean
	rm -rf ./artifacts/
	rm -rf ./src/protos
