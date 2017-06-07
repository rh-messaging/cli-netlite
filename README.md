# README #

### What is this repository for? ###

* This is console based amqp client builded on [AMQP.NET lite library](https://github.com/Azure/amqpnetlite)
* Version: 1.0.0

### How do I get set up? ###

1. Summary of set up
    * Clone repo on Windows system (ws2012r2, ws2012, win 8.1, win 10)
    * Download `Amqp.Net.dll` or clone [AMQP.NET lite library](https://github.com/Azure/amqpnetlite) and build `Amqp.Net.dll` with .net 4.5
    * Copy `Amqp.Net.dll` to `<project-folder>\dotnetlite\dlls` folder
    * Open `DotNetLiteClient.sln` in Visual Studio and build it, or build using msbuild.exe in cmd

2. Dependencies
    * This client depends on .net 4.5 and Amqp.Net.Dll

### Contributors ###
* David Kornel <david.kornel@outook.com>