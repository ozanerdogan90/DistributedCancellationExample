# Distributed Cancellation Sample 

> .Net Core 3.1, Redis, DistributedCancellation, DistributedLocking , Polly

## Description
This projects is a sample of distributed cancellation usage. The study case is ;
We have a remove device that we can send commands (in my case its turn on , hibernate, sleep , shutdown ) and we want these commands to be executed without redundant calls. 
Distributed cancellation uses redis to acquire a lock and cancel other calls with same key to execute only single command per time.

## Features
##### Framework
- .Net Core
 
## Requirements
- .Net Core >= 3.1
- Redis

## Running the API
### Development
To start the application in development mode, run:

```cmd
dotnet build
cd src\DistributedCancellationExample.Console
dotnet run
```
Application will be showing the logs: 

