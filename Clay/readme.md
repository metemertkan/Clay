Project Scope
-Admin API for lock and access management.
-Login mechanism was implemented with JWT authentication.
-Audit trail for every Lock/Unlock action
-User API for individual's lock and access management.

Features
-Pagination
-Caching
-Authorization

Following technologies was used:
-Asp.net Core 2.1
-Entity Framework
-MSSQL
-JWT

Asp.net core choosed because it is flexible and fast. MSSQL and Entity Framework was choosed because I am most familiar with them. Jwt was choosed to avoid time consumption reasons.

Architecture
Application follows MVC architecture patterns and follows most of the Domain Design Principle patterns.
-Domain folder holds the domain objects
-Repository folder holds data access layer
-Service folder is for holding business logic
-Controllers is for user interactions 

Improvements
-Searching and Filtering implementation
	-Currently there is only pagination, searching and sorting is a nice improvement. I started an implementation for sort but didn't used it.
-Message Queuing 
	-Currently it is accepting only requests directly. If we consider 1m locks, It may requires queuing implementation. I wanted to see real life example of the project.
-Unit testing covers get actions and only checks for create/update actions executed or not. Maybe an idea to implement dummy database and do create/update action and check if really worked. But this becomes integration tests. So more tests can be implemented.
-Sorting wasn't used, from a user input like "name|desc" we can implement a sorting that sorts according to name and descending. Strategy pattern would be applied here.


How to run and install
Configure the connection string in appsettings.json and "dotnet watch run" is enough to run.
It will create the database and start the application.