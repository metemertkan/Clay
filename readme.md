<b>Project Scope</b>

-Admin API for lock and access management.<br>
-Login mechanism was implemented with JWT authentication.<br>
-Audit trail for every Lock/Unlock action<br>
-User API for individual's lock and access management.<br>

<b>Following technologies was used:</b>

-Asp.net Core 2.1<br>
-Entity Framework<br>
-MSSQL<br>
-JWT<br>
-Swagger<br>

Asp.net core choosed because it is flexible and fast. MSSQL and Entity Framework was choosed because I am most familiar with them. Jwt was choosed to avoid time consumption reasons.<br>

<b>Architecture</b>

Application follows MVC architecture patterns and follows most of the Domain Design Principle patterns.<br><br>
-Constants folder holds classes for constant strings<br><br>
-Contollers holds 1 base controller called "ClayControllerBase" and 3 children controller "Admin","User","Account"<br><br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-Admin Controller has actions for Admin<br><br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-User Controller has actions for User<br><br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-Account Controller used for login.<br><br>
-Data Folder holds files about data manipulation. DbContext, DbContext Factory and Seeding are database interactions. Pagination folder has related to pagination.<br><br>
-Filters folder holds action filters that has been in the project. "ExceptionFilter" is for handling Server exceptions. "PaginationCorrection" is for allowing user to send null object for the pagination item. null object converted into default, that way we ensure pagination all the time."ValidateViewModel" is for validation purposes.<br><br>
-Logs folder holds logs files but it is being ignored.<br><br>
-Managers folder holds classes that act like manager. In our case there was a interaction between User and Lock. This interaction was assign/unassign. In order to keep Single responsibility we used a manager for manipulation.<br><br>
-Migration folder holds classes for Entityframework migrations and database snapshot.<br><br>
-Models folder holds Domain, Response and Input level classes. Domain classes are the ones has been used from every level of the project. Input models are used as action parameters. Response Models is for returning response from actions.<br><br>
-Repositories is for Data access. There is an interface IBaseRepository and BaseRepository for holding general actions. There are interfaces that requires special actions. This extension achieved by the interfaces like ILockRepository<br><br>
-Services folder holds one class TokenService. This is used for creating token after login <br><br>
-UnitOfWork folder holds classes for enabling transactional behaviour on repositories.<br><br>
<b>Improvements</b>
<br>
-Sorting<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-Currently pagination and searching in place. Sorting is a nice improvement. It can be integrated into base repository interface and class<br><br>
-Message Queuing <br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-Currently it is accepting only requests directly. If we consider 1m locks, It may requires queuing implementation. I wanted to see real life example of the project.<br><br>
-Pagination correction via action filter supports one level. This means for the method "GetLockHistory" I had to manipulate on the action. Supporting deeper level of correction via reflection would be nice. Also need to check for performance with the refrection.<br><br>
-Incorrect password with n number of tries can be blocked. Currently there is no support, could be nice feature.<br><br>
-As far as I know Asp.net core does not support for getting the list of the cache keys (https://github.com/aspnet/Caching/issues/93). So I implemented a workaround with invalidation that goes like follows:
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1- cache the get request with pagination (if pagination is null then use default one.)<br>
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2-If there is a Create/Update/Lock/Unlock action on the locks remove the cache.<br>
Same thing for the attempts. However controller become dirty with this implementaiton. So as an improvement we can try Redis and see how it goes.<br><br>
<b>How to run and install</b>

Configure the connection string in appsettings.json and "dotnet watch run" is enough to run.
It will create the database and start the application.<br>
