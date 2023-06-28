# General

| Workflow | Status |
| --------------------------- | ---------------------------------------------------------------------- |
| Continuous Integration (CI) | ![build](https://github.com/RelationalAI/rai-sdk-csharp/actions/workflows/dotnet-build.yaml/badge.svg) |
| [Publish to Nuget](https://www.nuget.org/packages/RAI) | ![publish](https://github.com/RelationalAI/rai-sdk-csharp/actions/workflows/nuget-pack.yaml/badge.svg) |

# The RelationalAI Software Development Kit for C#

The RelationalAI (RAI) SDK for C# enables developers to access the RAI
REST APIs from C#.

* You can find RelationalAI C# SDK documentation at <https://docs.relational.ai/rkgms/sdk/csharp-sdk>
* You can find RelationalAI product documentation at <https://docs.relational.ai>
* You can learn more about RelationalAI at <https://relational.ai>

## Getting started
Building:
```shell
cd rai-sdk-csharp
dotnet build
```

Running Examples:
```shell
cd rai-sdk-csharp/RelationalAI.Examples
dotnet run ListUsers --profile latest
dotnet run CreateEngine --engine csharp-sdk-test --profile latest
dotnet run DeleteEngine --engine csharp-sdk-test --profile latest
```

### Requirements

.Net Core 3.1

### Installing the SDK

```shell
dotnet build
```

### Create a configuration file

In order to run the examples and, you will need to create an SDK config file.
The default location for the file is `$HOME/.rai/config` and the file should
include the following:

```conf
[default]
host = azure.relationalai.com
client_id = <your client_id>
client_secret = <your client secret>

# the following are all optional, with default values shown
# port = 443
# scheme = https
# client_credentials_url = https://login.relationalai.com/oauth/token
```

Client credentials can be created using the RAI console at https://console.relationalai.com/login

You can copy `config.spec` from the root of this repo and modify as needed.

## Examples

```shell
cd rai-sdk-csharp/RelationalAI.Examples
dotnet run ListUsers --profile latest
```

## Support

You can reach the RAI developer support team at `support@relational.ai`

## Contributing

We value feedback and contributions from our developer community. Feel free
to submit an issue or a PR here.

## License

The RelationalAI Software Development Kit for C# is licensed under the
Apache License 2.0. See:
https://github.com/RelationalAI/rai-sdk-csharp/blob/master/LICENSE
