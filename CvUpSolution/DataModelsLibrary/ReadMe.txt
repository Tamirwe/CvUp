
Update DataModelLibrary from data base

1. git commit and push 
2. make sure solution compile successfully
3. set ""DataModelsLibrary"" as startup project
4. open package manager console and select default project "DataModelsLibrary" from the drop down 
5. run the command
Scaffold-DbContext "Host=localhost;Database=cvupdb;Username=postgres;Password=!Shalot5" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir DBModels -Namespace Database.models -UseDatabaseNames -Force -Project DataModelsLibrary 

6. set CvUpApi as startup project
7. build






Old Command
	Scaffold-DbContext "server=localhost;port=3306;user=root;password=!Shalot5;database=cvup00001" Pomelo.EntityFrameworkCore.MySql -OutputDir DBModels -namespace Database.models -UseDatabaseNames -f 


