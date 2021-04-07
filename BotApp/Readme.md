# BotApp
____

All the functionality of our Bot resides here

## Configuration
> Discord:Token   -- Reference [DiscordBot](./../Web/DiscordBot.md) for more information

## Dependencies
> DSharpPlus (API For Discord)

## Project Structure
Since we're utilizing `MediatR` - the structure may not make sense right away. Perhaps it could be refactored a bit.

1. Commands
    - All Discord Invokable Commands
        - Join
        - Leave
        - Help
        - etc
2. Classes
    - MediatR directory. This is where we defined notifications/commands/queries related to classes

3. Common:
    - Self explanatory, common stuff shared across this library/assembly

4. Extensions
    - Useful extension methods that help us developers do things.
    - These must be reusable items
   
5. Helpers
   - Items that don't make the cut for an "extension" method, but are still useful
   
6. Interaction
   - In the past - referred to as "Wizard(s)"
   - Chat-Based interaction
   
7. Notification Handlers
   - Tap into noficiations from within the architecture to do things
   - Or simply add your own notification
   
8. Services
   - Anything this bot brings to the table that can be used by the bot itself and potentially other parts of the architecture
      - So make sure they're reusable... if you add something here