using Microsoft.Extensions.Configuration;

namespace ImportCvsLibrary
{
    public  class DataBaseBackup : IDataBaseBackup
    {
        string _filesRootFolder;

        public DataBaseBackup(IConfiguration config)
        {
            _filesRootFolder = config["GlobalSettings:CvUpFilesRootFolder"];
        }

        public void BackupDataBase()
        {
            try
            {
                string databaseName = "cvup00001";
                string backUpDir = $"{_filesRootFolder}\\dataBaseBackupFiles";

                DirectoryInfo backupDirInfo = new DirectoryInfo(backUpDir); 
                FileInfo[] dirFiles = backupDirInfo.GetFiles("*.gz");

                foreach (var fil in dirFiles)
                {
                    if (fil.CreationTime.Date == DateTime.Now.Date)
                    {
                        return;
                    }
                }

                var backUpDirObj = Directory.CreateDirectory(backUpDir);
                string newBackUpFileName = databaseName + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".sql";
                string batchFileName = backUpDir + @"\DbBackup.bat";

                if (File.Exists(batchFileName))
                {
                    File.Delete(batchFileName);
                }

                using (StreamWriter sw = File.CreateText(batchFileName))
                {
                    sw.WriteLine(@"@REM *** PARAMETERS/VARIABLES ***");
                    sw.WriteLine(@"SET BackupDir=""" + backUpDirObj.FullName + @"\""");
                    sw.WriteLine(@"SET BackupFile=" + newBackUpFileName);

                    sw.WriteLine(@"SET mysqldir=C:\Program Files\MySQL\MySQL Server 8.0\bin");

                    sw.WriteLine(@"SET mysqlpassword=!Shalot5");
                    sw.WriteLine(@"SET mysqluser=root");
                    sw.WriteLine(@"SET databaseName=" + databaseName);
                    sw.WriteLine(@"set zip=""C:\Program Files\7-Zip\7z.exe""");
                    sw.WriteLine(@"@REM *** EXECUTION ***");
                    sw.WriteLine(@"@REM Change to mysqldir");
                    sw.WriteLine(@"CD %mysqldir%");
                    sw.WriteLine(@"@REM dump/backup");
                    sw.WriteLine(@"mysqldump -u %mysqluser% -p%mysqlpassword% --databases %databaseName% >%BackupDir%%BackupFile%");
                    sw.WriteLine(@"@REM zip backup file");
                    sw.WriteLine(@"%zip% a -tgzip %BackupDir%%BackupFile%.gz %BackupDir%%BackupFile%");
                    sw.WriteLine(@"@REM delete backup file");
                    sw.WriteLine(@"del %BackupDir%%BackupFile%");
                }

                //execute the batch file
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = batchFileName;
                proc.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
                proc.Start();

                //delete files older than last 1 month
                string[] files = Directory.GetFiles(backUpDir);

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < DateTime.Now.AddMonths(-1))
                        fi.Delete();
                }
            }
            catch (Exception ex)
            {
                string errorDetail = "error: " + ex.Message;
            }
        }
    }
}
