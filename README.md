# Devry-Service-Bot
[![Build & Push Image](https://github.com/JBraunsmaJr/Devry-Service-Bot/actions/workflows/dotnet.yml/badge.svg)](https://github.com/JBraunsmaJr/Devry-Service-Bot/actions/workflows/dotnet.yml)

This bot is utilized in an academic setting for students and professors.

It provides the Unofficial Devry Discord Community an 
enhanced user experience compared to vanilla discord.


## Slash Commands

#### /join `search`
  
  Uses a fuzzy search feature. In the event of an exact match - you automatically receive the role.
  If a role was not found it removes a character from the end until it finds something. Must have 
  at least 3 characters in length.

----

#### /leave `search`
  
  Behaves the same way as `/join`. If you rather view the list of roles you have and go from there use 
  `/leave list`
  
----

#### /invite
  
  Displays the server's invitation message

----

#### /create-class
  
  Allows moderators to create classes on the fly.
  
----

#### /create-reminder
  
  Allows moderators to add a reminder to the current channel

----

#### /show-reminders
  
  Allows moderators to view reminders in the current channel
 
 ----
 
#### /delete-reminder
  
  Allows moderators to delete reminders from the current channel

----

#### /archive
  
  Any class section that hasn't been used in a configurable amount of time will be deleted, along with the associated class-role.
