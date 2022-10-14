1. git commit and push 
2. make sure solution comile successfully
3. set Database as startup project
4. open package manager console and select default project from the drop down "Database"
5. run the command
	Scaffold-DbContext "server=localhost;port=3306;user=root;password=!Shalot5;database=cvup00001" Pomelo.EntityFrameworkCore.MySql -OutputDir DBModels -namespace Database.models -UseDatabaseNames -f
6. set CvUpApi as startup project
7. build