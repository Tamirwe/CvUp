 1. publish the project to folder "C:\github\CvUp\CvUpSolution\CvsWorkerService\bin\Release\net6.0\publish\"
 2. open Command Prompt (in administration mode) and run the following command
 
 sc.exe create CvsWorkerService binpath="c:\github\cvup\cvupsolution\cvsworkerservice\bin\release\net6.0\publish\CvsWorkerService.exe"
 
 sc.exe start CvsWorkerService

 sc.exe stop  CvsWorkerService

 sc.exe delete  CvsWorkerService