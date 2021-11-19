# ping-identity-playground

See [Deploy a simple stack with PingFederate and PingDirectory](https://devops.pingidentity.com/deployment/deployCompose/) for more information.

See [pingone-sample-dotnet](https://github.com/pingidentity/pingone-sample-dotnet) for more information.

## Prerequisites

- You should have a [Ping Identity DevOps Registration](https://devops.pingidentity.com/get-started/devopsRegistration/);

- [.NET 5.0](https://dotnet.microsoft.com/download/)

## Running it locally

First, you need to replace the credentials in the [docker-compose](docker-compose.yaml) environment to your own (replace in `pingaccess`, `pingfederate` and `pingdirectory`):

```bash
      - PING_IDENTITY_DEVOPS_USER=<your-devops-user-here>
      - PING_IDENTITY_DEVOPS_KEY=<your-devops-key-here>
```
