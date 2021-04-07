# Database

Since we leverage `EntityFrameworkCore` we can technically shift around to any `DbProvider` without modifying our code. Minus where we say `UseMySql` in our Infrastructure. We just need to swap it out for the designated provider.

## Development
In Development, utilizing MySQL. 

If you make any changes to a database model located within [Domain.Entities](./../Domain/Entities), or implement a new table -- requirements defined later, you'll need to create a new migration

### Connection String
The default connection string must utilize the `string.format` method (already implemented). Both the `user` and `password` values are stored as user secrets. Obviously, for security reasons. The application will be looking for the following values
at startup

> `Database:User`

> `Database:Password`
 
Verify your local MySQL connection settings and set values by using the commands below

```
dotnet user-secrets set "Database:User" "YourUserGoesHere"
```

```
dotnet user-secrets set "Database:Password" "YourPasswordGoesHere"
```

### Create Migration
Migrations are essentially git-like source control for your data layer. They have up/down functionality that allows you to upgrade to the latest version of your code. Or revert back your application doesn't work as needed.

The name of your migration should be a short descriptor of what you did. i.e. `AddedConfigModel`

Make sure you run this from within the `Web` folder

```
dotnet ef migrations add <name of your migration>
```

Once done, you should see your migration added to the [Migrations](./Migrations/) folder.

To apply your changes to the database using the `ConnectionString` provided in [appsettings.json](./appsettings.json) run the following 

```
dotnet ef database update
```

## Production
A separate docker image is utilized. This should match the database type we're using! So if we are using MySQL our image should be MySQL. Primary reason
MySQL was chosen was it has a much smaller footprint than SQLServer (~3.5 GB of RAM)

Within the build-pipeline we shall populate/create the necessary images (web | db).

These images are defined within our [docker-compose](./docker-compose.yml) file.

## How to add to database (new entities)
Define the class within the [Domain](./../Domain) project. Specifically the [Domain.Entities](./../Domain/Entities/) folder. Granted, you can have subfolders in there. Just define the data you'd like to store. Utilize exiting entities
for reference.

This object will then need to be defined in two places <br/>
[IApplicationDbContext](./../Application/Common/Interfaces/IApplicationDbContext.cs) - add a `DbSet` of your new type

With this update you may notice an error indicating ApplicationDbContext doesn't implement your new `DbSet`. <br/>
[ApplicationDbContext](./../Infrastructure/Persistence/ApplicationDbContext.cs)

Now you should be free to use your new type across the application.

## New Service/Controller
If you need a reference to the database - our architecture leverages Dependency Injection (DI). This means within the constructor of your object you can specify all the things you need! Just make sure one of those items is [IApplicationDbContext](./../Application/Common/Interfaces/IApplicationDbContext.cs)

Please reference other items within the solution to see how this is done. An example is as follows:

```
using Application.Common.Interfaces;

public class MyNewService
{
    private readonly IApplicationDbContext _context;
    
    public MyNewService(IApplicationDbContext context)
    {
        _context = context;
    }
}
```

If this is a service you'll need to register it to the application in order for DI to work.

Either add this to the `ServiceCollectionExtensions.cs` file located at the root of each project directory, or the [Startup.cs](./Startup.cs)'s `ConfigureServices` method. There are a variety of scopes. Please reference the [Microsoft documentation](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)
as to which scope/lifetime fits your service

## Docker Image
> mysql

Environment Variables that must be set during build

As an alternative to passing sensitive information via environment variables, `_FILE` can be appended to
variables for them to get loaded via secret file

Docker secrets are stored in `/run/secrets/secretname`
Only works for the following items

> MYSQL_ROOT_PASSWORD

> MYSQL_ROOT_HOST

> MYSQL_DATABASE

> MYSQL_USER

> MYSQL_PASSWORD
```
docker run --name nameOfImage -e MYSQL_ROOT_PASSWORD_FILE=/run/secrets/<secret_name> -d mysql

docker run --name nameOfImage -e MYSQL_ROOT_PASSWORD_FILE=/run/secrets/mysql-root -d mysql
```

Of course, we want to ensure the data is persistent. If the container goes down, or system reboots the container's
data will reset if we don't mount it

Ensure we have the following volume mounted out

- v ${PWD}/data:/var/lib/mysql

