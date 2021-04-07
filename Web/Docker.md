# Docker Info
____

Anything that happens within a container after it starts running is lost if the container goes down. (except when restarting).
This is why mounting is a thing. Mounting allows you to persist data between docker sessions. From database
to configuration files.

Important to note: You want to mount a configuration file `appsettings.json`

> To mount `appsettings.json` you're required to have this file on the host system
> Otherwise it will mount as a directory which just... breaks things.

In the event changes are made to a container during runtime, and you want to `save` it... luckily
docker has you covered.

Obtain the container ID via
> docker ps

Figure out what you want to save the image as (TAG) and use that ID from above.
> docker commit CONTAINER_ID NEW_TAG 