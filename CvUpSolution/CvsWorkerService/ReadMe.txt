 1. publish the project to folder "C:\github\CvUp\CvUpSolution\CvsWorkerService\bin\Release\net6.0\publish\"
 2. open Command Prompt (in administration mode) and run the following command
 
 sc create CvsWorkerService binpath="C:\GitHub\CvUp\CvUpSolution\CvsWorkerService\bin\Release\net6.0\publish\" 

 sc start CvsWorkerService

 sc stop  CvsWorkerService

 CvsWorkerService 