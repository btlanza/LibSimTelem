# LibSimTelem
An easy-to-use, extendable, multi-simulator realtime racing telemetry interfacing library for C# .NET 4.0+

## Excuse me?
This is a reasonable response to the project desciption. Basically, many modern racing simulator games have
features that allow information about various aspects of the game to be sent, processed, displayed and/or
recorded by external applications. This library is meant to simplify access to this data across multiple
racing simulators.

## What games are supported?
Currently the library supports F1 2017, F1 2018 and Assetto Corsa. Support for Project CARS 2 is also in
the works!

## Features
* Simplifies access to common data across multiple racing simulator games
* Provides full game-specific access to data not provided in across other supported titles
* Uses the UDP interfacing option where available to allow access to data on machines not hosting the game
* Extendability allows relatively easy implementation of advanced features
* Callback-driven approach allows for prompt updates and flexibility in choosing which data should be exposed 

## Code examples
View my demo project at the [LibSimTelemDemo repository](https://github.com/btlanza/LibSimTelemDemo)!
##### F1 2018
