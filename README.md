# README #

### What is this repository for? ###

* This is console based amqp client builded on [AMQP.NET lite library](https://github.com/Azure/amqpnetlite)
* Version: 1.0.0

### How do I get set up? ###

1. Summary of set up
    * Clone repo on Windows system (ws2012r2, ws2012, win 8.1, win 10)
    * Open `cli-netlite.sln` in Visual Studio and build it, or build using msbuild.exe in cmd

2. Dependencies
    * This client depends on .net 4.5 and nuget packages of AMQPNetlite and NDesk.Options type `nuget restore` to download packages

### Using

Using cmd client

```cmd
> cli-netlite-sender.exe --broker "username:password@localhost:5672" --address "queue_test" --count 2 --msg-content "text message" --log-msgs dict
> cli-netlite-receiver.exe --broker "username:password@localhost:5672" --address "queue_test" --count 2 --log-msgs dict
```

Contributors
----
* David Kornel <david.kornel@outook.com>, <dkornel@redhat.com>

License
----

Apache v2