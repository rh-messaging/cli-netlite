# README #
[![Build Status](https://travis-ci.org/rh-messaging/cli-netlite.svg?branch=master)](https://travis-ci.org/rh-messaging/cli-netlite)



## Description

* This is console based amqp client builded on [AMQP.NET lite library](https://github.com/Azure/amqpnetlite)

## Compilation

### With .NET Core:

1. Open a browser and log in to the Red Hat Customer Portal Product Downloads page at [access.redhat.com/downloads](https://access.redhat.com/downloads).
2. Locate the Red Hat AMQ Clients entry in the INTEGRATION AND AUTOMATION category.
3. Click Red Hat AMQ Clients. The Software Downloads page opens.
4. Download latest AMQ Clients .NET Core .zip file and unzip it.
5. Clone cli-netlite repo.
6. Create `DLLs` folder inside cli-netlite repo.
7. Locate `AMQP.dll` file inside unzipped .NET Core client and copy it inside DLLs folder.
8. Run `build-netcore.sh`.
9. Built binary files are located inside `dist/netcore`.

### With .NET Framework:

1. Open a browser and log in to the Red Hat Customer Portal Product Downloads page at [access.redhat.com/downloads](https://access.redhat.com/downloads).
2. Locate the Red Hat AMQ Clients entry in the INTEGRATION AND AUTOMATION category.
3. Click Red Hat AMQ Clients. The Software Downloads page opens.
4. Download latest AMQ Clients .NET .zip file and unzip it.
5. Clone cli-netlite repo.
6. Create `DLLs` folder inside cli-netlite repo.
7. Locate `Amqp.net.dll` file inside unzipped .NET client and copy it inside DLLs folder.
8. Run `build-netlite.sh`.
9. Built binary files are located inside `dist/netlite`.

## Using

Using cmd client

```cmd
> cli-netlite-sender.exe --broker "username:password@localhost:5672" --address "queue_test" --count 2 --msg-content "text message" --log-msgs dict
> cli-netlite-receiver.exe --broker "username:password@localhost:5672" --address "queue_test" --count 2 --log-msgs dict
```

## Contributors

* David Kornel <david.kornel@outook.com>, <dkornel@redhat.com>

## License

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)