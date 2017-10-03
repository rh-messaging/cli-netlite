# README #
[![Build Status](https://travis-ci.org/rh-messaging/cli-netlite.svg?branch=master)](https://travis-ci.org/rh-messaging/cli-netlite)



## Description

* This is console based amqp client builded on [AMQP.NET lite library](https://github.com/Azure/amqpnetlite)

## Compilation

1. Clone repo on Windows system (ws2012r2, ws2012, win 8.1, win 10)
1. Open `cli-netlite.sln` in Visual Studio and build it, or build using msbuild.exe in cmd

## Installation

1. Download cli-netlite-[version].msi from releases
2. Install cli-netlite-[version].msi

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