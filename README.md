# Kchat

A chat application using Kafka.

![Alt text](kchat.png?raw=true "kchat")

This is a tutorial to be used in hands-on-workshop. Each branch only contains part of the code.

|Branch|Description|
|-|-|
[Main](https://github.com/TalalTayyab/Kchat/tree/main)|Readme file that describes how to get a free Kafka instance
[First](https://github.com/TalalTayyab/Kchat/tree/first)|Layout of the Blazor app
[Second](https://github.com/TalalTayyab/Kchat/tree/second)|Add Kafka code
[Third](https://github.com/TalalTayyab/Kchat/tree/third)|Add web project into docker compose to simulate multiple client instances

## How to install Kafka

This section will discuss options on how to install Kafka.

### Locally using Docker-compose

Note: Requires Docker with support for linux containers.

1. Run `docker-compose up -d` in the `\kchat` folder and wait for it to complete.
1. Download the Kafka GUI [tool](https://kafkatool.com/download.html) aka Offset explorer
1. Open the Offset Explorer tool and Add a new connection `File -> Add New Connection`
1. Enter Cluster name example localhost - `Properties -> Cluster Name`
1. Enter bootstrap server `localhost:29092` (this is where Kafka is running) - `Advanced -> Bootstrap servers`
1. Press Test Connection and Yes to add connection.

### Free Cloud Kafka instance 

1. Browse to [CloudKafka](https://www.cloudkarafka.com/plans.html)
1. Scroll all the way down until you see Developer Duck Free account
1. Follow through the process to get a free instance of Kafka.

