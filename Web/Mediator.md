# MediatR
____
This system allows us to decouple parts of our architecture.

Idea is define a variety of requests/commands that our system requires. For instance, you may find 
folder structures with 3 subdirectories of `Commands`, `Notifications`, and `Queries`. 

Within our `ServiceCollectionExtensions` or `Startup` files you'll notice we add `MediatR` from the assembly.
This grabs all of the things we defined via reflection. Meaning, we are free to add to our architecture
without having to manually add it elsewhere for consumption / availability.

## Commands
Basically everything that isn't a `get` request. You'll notice within [Channels](./../Application/Channels/Commands)
We have `Create`, `Delete`, and `Update` commands. Fairly obvious what they're meant to do.
Within each of those files contains the request and it's handler.

## Notifications
We utilize `DomainEvents` which represent the variety of actions in our architecture. These simply check
to see if an event has occurred. Like `ChannelCreatedEvent`, and when it sees it - do something. Technically
we can do more than just `Printing` something to logs. Think of it as an event which originates in one part of our 
architecture. Somewhere else, we have something we want to happen whenever that event occurs. 

## Queries

Fairly straightforward. Based on parameters provide some set of data back. Whether it's searching for something by ID, or whatever.