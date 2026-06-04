

 ** how to import a SQL backup file in pgAdmin 4 **
---------------------------------------------------


OPTION 1

first: click cvupdb
In pgAdmin → Tools → PSQL Tool
Run the command 

\i 'C:/CvUp/temp/cvupdb_2026-05-26-07-32.sql'




OPTION 2

Right-click the target database → Restore...
Set Format to match your file:

Plain → .sql files

Click the folder icon next to Filename and select your file
Click Restore