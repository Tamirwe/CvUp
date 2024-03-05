 
 *** create new windows service ****
 1. publish the project to folder "C:\github\CvUp\CvUpSolution\CvsWorkerService\bin\Release\net6.0\publish\"
 2. open Command Prompt (in administration mode) and run the following command
 3. sc.exe create CvsWorkerService binpath="c:\github\cvup\cvupsolution\cvsworkerservice\bin\release\net6.0\publish\CvsWorkerService.exe"
 

 *** update service ***
 1. stop service
 2. publish CvsWorkerService
 3. start service



 *** service commands ****
 sc.exe start CvsWorkerService

 sc.exe stop  CvsWorkerService

 sc.exe delete  CvsWorkerService