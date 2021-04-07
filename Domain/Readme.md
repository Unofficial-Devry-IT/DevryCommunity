# Domain
____

Within this project we define our domain. 

- Models that we'll need within the database
- Events based on our models (create, deleted, updated, etc)
- Exceptions that could occur within our domain (NotFoundException for instance)


# DO NOT:

> Add domain-logic within here. That is reserved for the Application project.

>Add definitions of infrastructure related code - such as DbContext implementation. This is reserved for the Infrastructure Project