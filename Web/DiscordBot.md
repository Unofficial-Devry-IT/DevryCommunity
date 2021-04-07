# Discord Bot
___

Discord bot provides numerous administrative capabilities, along with quality of life features 
for users.

This bot shares a common architecture to make it easier for other systems to leverage events
which happen. For instance, chats appearing being transposed to other locations. Or an event created on the site 
is displayed in Discord as well.

## Production
The bot relies on a `token` in order to work. This token is stored within Github as a secret, similar to our OAuth system.
The docker image will utilize docker-secrets to store this value as an environment variable. In the event
someone were to inspect this image - it won't appear.

## Development
Once again, similar to OAuth - we're utilizing `AppSecrets` to store this value. Reference [OAuth](OAuth.md) to figure out how to
initialize `AppSecrets` for a new project. In our case - we already have `user secrets` defined within our `Web.csproj`

You'll require access to the Discord Bot on the Discord Developer portal. Get with a contributor if you need access. This value
is stored as encrypted text, just like our OAuth values.

Once these values are encrypted (ask a contributor) -- you can update your local store by

```
dotnet user-secrets set "Discord:Token" "encrypted token value here"
```