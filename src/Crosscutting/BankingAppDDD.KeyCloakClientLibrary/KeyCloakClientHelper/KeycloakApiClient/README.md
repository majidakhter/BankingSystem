# AzureKeycloak
This repo contains the files to create an ApiClient for Keycloak.

## Files
- **KeycloakApiClient.cs**: Generated file from Keycloak OpenAPI spec.
- **KeycloakApiClientFactory.cs**: Provides a factory method to create and configure instances of the KeycloakApiClient.
- **source/keycloak_openapi.json**: Contains the OpenAPI (Swagger) specification for the Keycloak API, used for client code generation. Downloaded from https://www.keycloak.org/docs-api/latest/rest-api/index.html
- **source/nswag.nswag**: NSwag configuration file, used by the NSwagStudio tool.
- **source/NSwagStudio.msi**: Installer for NSwagStudio, a graphical tool for working with OpenAPI/Swagger documents and generating client code.
