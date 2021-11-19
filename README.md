# ping-identity-playground

See [Deploy a simple stack with PingFederate and PingDirectory](https://devops.pingidentity.com/deployment/deployCompose/) for more information.

See [pingone-sample-dotnet](https://github.com/pingidentity/pingone-sample-dotnet) for more information.

## Prerequisites

- You should have a [Ping Identity DevOps Registration](https://devops.pingidentity.com/get-started/devopsRegistration/);

- [.NET 5.0](https://dotnet.microsoft.com/download/)

## Running the `infra` locally

First, you need to replace the credentials in the [docker-compose](docker-compose.yaml) environment to your own (replace in `pingaccess`, `pingfederate` and `pingdirectory`):

```bash
      - PING_IDENTITY_DEVOPS_USER=<your-devops-user-here>
      - PING_IDENTITY_DEVOPS_KEY=<your-devops-key-here>
```

To open the `PingFederate` locally, open the following URL in your browser:

```
https://localhost:9999/pingfederate/app#/
```

When asked for the username and password, enter the following:

```
username: administrator
password: 2FederateM0re   
```

To open the `PingAccess` locally, open the following URL in your browser:

```
https://localhost:9000/
```

When asked for the username and password, enter the following:

```
username: administrator
password: 2FederateM0re   
```

Finally, to open the `PingDataConsole` (PingData Administrative Console) locally, open the following URL in your browser:

```
https://localhost:8443/
```

When asked for the credentials, enter the following:

```
server: pingdirectory:1636
username: administrator
password: 2FederateM0re   
```

## Creating a new OIDC application

Follow [this stpes](https://docs.pingidentity.com/bundle/solution-guides/page/ywg1598030491145.html) to create a new OIDC application in `PingFederate`.
