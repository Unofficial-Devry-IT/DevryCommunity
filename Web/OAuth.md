# OAuth

Discord OAuth is leveraged as an alternative login method. 

- User is required to be part of the Discord Community (at least at time of registration)
- Email is pulled from linked information
- Can easily identify who this user is from the community (i.e - admin, moderator, tutor, professor, etc)

## Production
Encrypted versions of `AppId`, and `AppSecret` are stored using GitHub secrets (encrypted yet again there).

During the docker build stage these values are injected using docker-secrets into the
docker container. This prevents the values from appearing within one of the layers

The application first checks to determine if these two values are available
as environment variables.

## Development
Unlike above, the application will leverage user-secrets to store the `AppId` and `AppSecret`

If you cloned/pulled/forked this project odds are -- you do not have these values
on your machine. You will need to be given access to the `Discord Team` for `Devry Bot`. 
From there you can easily retrieve the values you need under `OAuth2` page.

Since the project already contains a `UserSecretsId` entry within the `Web.csproj` file
you won't have to initialize the `user-secrets` link.

However: for your reference for other projects this is how you initialize user secrets via CLI
**Make sure your current working directory is this project (Web)**
```
dotnet user-secrets init
```

Get with another contributor, or Badger2-3 to figure out the method of encryption for these values. 
Neither the AppId, nor AppSecret are stored via plainText.

Once you encrypt those values run the following commands to add them to your local
secrets file.

Note: The user-secrets file is accessed via Configuration from within the app.
It's essentially an extension of `appsettings.json` - just for values you don't
want to commit in there for security purposes

```
dotnet user-secrets set "Discord:AppId" "encrypted value for app id here"

dotnet user-secrets set "Discord:AppSecret" "encrypted value for app secret here"
```