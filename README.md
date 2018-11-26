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
#### F1 2018
```C#
using LibSimTelem;
using static LibSimTelem.EventDefs;
using static LibSimTelem.F118Defs;

/* ... */

static void Main(string[] args){

    /* Create an instance of and launch the program */
    Program p = new Program();
    p.Run();

}

private void Run()
{

    /* This code will work for all supported games by only changing the TelemReader
       and TelemData implementations to the right type. So, while here we have an
       F118Reader and F118Data object, we could just as easily declare them as
       F117Reader/ACReader and F117Data/ACData objects. Remember to include the
       correct F117Defs or ACDefs static classes in your usings at the top of the
       file! */
       
    /* Create the F1 2018 reader and data objects */
    F118Reader reader = new F118Reader();
    F118Data data = new F118Data();

    /* Connect the data object to the reader to access telemetry through
       the simplified TelemeData interface functions */
    data.SubscribeToReader(reader);

    /* Subscribe to the higher-level powertrain data callback to get speed,
       RPM, and gear information */
    data.PowertrainReceivedEvent += OnGotPowertrain;

    /* Start reading packets from the game */
    reader.Start();

    /* Wait for the user to press a key before cleaning up and exiting */
    while (!Console.KeyAvailable) { }

    /* Clean up and finish */
    reader.Stop();

}

private void OnGotPowertrain(object sender, PowertrainEventArgs args)
{
    /* Clear the console */
    Console.Clear();

    /* Write received data to console */
    Console.WriteLine("Speed in kmh: {0}\nSpeed in mph: {1}\nEngine RPM: {2}\nGear: {3}",
        args.kmh, args.mph, args.rpm, args.gear);       

}
```
