



 
 1) *** create new windows service ****
 --------------------------------------
	 1. publish this project to folder C:\github\CvUp\CvUpSolution\SchedulerWorkerService\bin\Release\net9.0\
	 2. open Command Prompt (in administration mode) and run the following command
	 3. sc.exe create "a Cvs Scheduler Service" binPath="C:\github\CvUp\CvUpSolution\SchedulerWorkerService\bin\Release\net9.0\SchedulerWorkerService.exe"
 

 2) *** update service ***
 -------------------------
	 1. stop service
	 2. publish CvsWorkerService
	 3. start service



 *** service commands ****
 sc.exe start SchedulerWorkerService

 sc.exe stop  SchedulerWorkerService

 sc.exe delete  SchedulerWorkerService