
CvUp Installation on a new pc
---------------------------------

1) Import CV's
---------------
	1. Install Drive for desktop.
	2. Connect to jobs.lomda@gmail.com account.
	3. Mirror files to  "C:\JobsLomda-GDrive" folder.

2) Add GitHub Desktop
---------------------
	1. Install GitHub Desktop.
	2. Connect to your account tamir.we@gmail.com.
	3. clone the repository "Tamirwe\CvUp" to "C:\GitHub\CvUp"

3) Install CvUp Back-End 
------------------------
	1. Open ""Visual Studio 2022".
	2. Open the project "C:\github\CvUp\CvUpSolution\CvUpSolution.sln".

	3. Install CvUp - Web API
	-------------------------
		a. Build the solution and install missing packages if needed.
		b. Add folder C:\inetpub\wwwroot\CvUpAPI and add security group "Everyone" with permission "Full control"
		c. Setting API Website  
			1. Open "IIS Manager" and add "CvUpAPI" website and set physical path to "C:\inetpub\wwwroot\CvUpAPI" 
			2. Set the binding: 
				1. select the pc Ip address and set port like 8070.
				2. add one more binding just with port like 8070.
		d. Publish CvUpAPI project (publish is already configured just Check "publish server: localhost and IIS application: CvUpAPI")


	4. Install CvUp - Windows Service for importing CV's and backup DB
	------------------------------------------------------------------
		a.  goto ReadMe.txt on CvsWorkerService project.

4) Install CvUp Front-End
-------------------------
	1. Open "C:\github\CvUp\cvup-ui-app" with VS code
	2. Install node js LTS
	2. Open vs code terminal and run "yarn" for installing "node_modules".
	3. Add folder "C:\inetpub\wwwroot\CvUpUI" and add security group "Everyone" with permission "Full control"
	4. Add website "CvUpUI" with "IIS Manager"
		1. set physical path to "C:\inetpub\wwwroot\CvUpUI" 
		2. Set the binding: 
			1. select the pc Ip address and set port like 8075.
			2. add one more binding just with port like 8075.
	5. run "yarn build:deploy"
	6. Install  "URL Rewrite"
	7. install "hosting bundle https://dotnet.microsoft.com/en-us/download/dotnet/3.1" (click the "Hosting Bundle" link)

5) Add port Forwording on cellcom router
----------------------------------------
	1. Open http://10.100.102.1/2.0/gui/#/
	2. Go to "Access Control" -> "Port Forwording"
	3. Add port name: CvUpUI-3UJCVDH2, port:8075, Internal host: 10.100.102.30
	3. Add port name: CvUpAPI-3UJCVDH2, port:8070, Internal host: 10.100.102.30

6) Import Database
------------------
	1. copy the last file from "C:\JobsLomda-GDrive\CvUp\dataBaseBackupFiles" and un zip it.
	2. Import DB

7) Lucene Indexing
------------------
	1. Run IndexCvsConsoleApp project


