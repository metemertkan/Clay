<b>Project Scope</b>

-Admin API for lock and access management.<br>
-Login mechanism was implemented with JWT authentication.<br>
-Audit trail for every Lock/Unlock action<br>
-User API for individual's lock and access management.<br>

<b>Features</b>

-Pagination<br>
-Caching<br>
-Authorization<br>

<b>Following technologies was used:</b>

-Asp.net Core 2.1<br>
-Entity Framework<br>
-MSSQL<br>
-JWT<br>

Asp.net core choosed because it is flexible and fast. MSSQL and Entity Framework was choosed because I am most familiar with them. Jwt was choosed to avoid time consumption reasons.<br>

<b>Architecture</b>

Application follows MVC architecture patterns and follows most of the Domain Design Principle patterns.<br>
-Domain folder holds the domain objects<br>
-Repository folder holds data access layer<br>
-Service folder is for holding business logic<br>
-Controllers is for user interactions <br>

<b>Improvements</b>

-Searching and Filtering implementation<br>
<nbsp><nbsp>-Currently there is only pagination, searching and sorting is a nice improvement. I started an implementation for sort but didn't used it.<br>
-Message Queuing <br><emsp>-Currently it is accepting only requests directly. If we consider 1m locks, It may requires queuing implementation. I wanted to see real life example of the project.<br>
-Unit testing covers get actions and only checks for create/update actions executed or not. Maybe an idea to implement dummy database and do create/update action and check if really worked. But this becomes integration tests. So more tests can be implemented.<br>
-Sorting wasn't used, from a user input like "name|desc" we can implement a sorting that sorts according to name and descending. Strategy pattern would be applied here.<br>


<b>How to run and install</b>

Configure the connection string in appsettings.json and "dotnet watch run" is enough to run.
It will create the database and start the application.<br>
