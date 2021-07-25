# Devry-Service-Bot

This bot is utilized in an academic setting for students and professors.

It provides the Unofficial Devry Discord Community an 
enhanced user experience compared to vanilla discord.

#### Status
[![Latest Build](https://github.com/Unofficial-Devry-IT/DevryCommunity/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Unofficial-Devry-IT/DevryCommunity/actions/workflows/dotnet.yml)

#### Commit Activities
![GitHub last commit](https://img.shields.io/github/last-commit/Unofficial-Devry-IT/DevryCommunity?label=Last%20Commit)
![GitHub commit activity](https://img.shields.io/github/commit-activity/m/Unofficial-Devry-IT/DevryCommunity?label=Commit%20Activity)

#### Stats
![GitHub issues](https://img.shields.io/github/issues/Unofficial-Devry-IT/DevryCommunity?label=Issues)
![GitHub pull requests](https://img.shields.io/github/issues-pr/Unofficial-Devry-IT/DevryCommunity?label=PRs)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/Unofficial-Devry-IT/DevryCommunity?label=Size)
![GitHub contributors](https://img.shields.io/github/contributors/Unofficial-Devry-IT/DevryCommunity)
![GitHub Repo stars](https://img.shields.io/github/stars/Unofficial-Devry-IT/DevryCommunity?label=Stars)



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

#### /lecture-invite
   Are you sharing an invite during lecture? Help your fellow classmates by telling the bot to expect an in-flux of traffic from a given class, or classes.
   
   1. Displays invite
   2. Select role(s) from your current roleset
   3. As people join they will be greeted by the bot. Selected role(s) will be attached as buttons to the welcome message -- allowing new members to click on them to join. Thus expediting the entire process. 

----
#### /code-review

**Work in progress feature**
Currently supports Python files ( .py ) extension. The bot will prompt users for a single file. Once done it'll generate an HTML report. User will be given a button to go view the report.

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

