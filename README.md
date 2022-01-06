# The RelationalAI Software Development Kit for C#

The RelationalAI (RAI) SDK for C# enables developers to access the RAI
REST APIs from C#.

* You can find RelationalAI C# SDK documentation at <https://docs.relational.ai/rkgms/sdk/csharp-sdk>
* You can find RelationalAI product documentation at <https://docs.relational.ai>
* You can learn more about RelationalAI at <https://relational.ai>

## Getting started

### Requirements

==todo==

### Installing the SDK

==todo==

### Create a configuration file

In order to run the examples and, you will need to create an SDK config file.
The default location for the file is `$HOME/.rai/config` and the file should
include the following:

```conf
[default]
host = azure.relationalai.com
port = <api-port>      # optional, default: 443
scheme = <scheme>      # optional, default: https
client_id = <your client_id>
client_secret = <your client secret>
client_credentials_url = <account login URL>  # optional
# default: https://login.relationalai.com/oauth/token
```

Client credentials can be created using the RAI console at https://console.relationalai.com/login

## Examples

==todo==

## Support

You can reach the RAI developer support team at `support@relational.ai`

## Contributing

We value feedback and contributions from our developer community. Feel free
to submit an issue or a PR here.

## License

The RelationalAI Software Development Kit for C# is licensed under the
Apache License 2.0. See:
https://github.com/RelationalAI/rai-sdk-csharp/blob/master/LICENSE
