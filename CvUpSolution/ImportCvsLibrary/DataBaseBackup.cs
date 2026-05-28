using Microsoft.Extensions.Configuration;

namespace ImportCvsLibrary
{
    public  class DataBaseBackup : IDataBaseBackup
    {
        string _filesRootFolder;
        string _localRootFolder;

        public DataBaseBackup(IConfiguration configuration)
        {
            _filesRootFolder = configuration["CVS_ROOT_FOLDER"];
            _localRootFolder = configuration["APP_LOCAL_ROOT_FOLDER"];

        }

        public void BackupDataBase()
        {
            try
            {
                string databaseName = "cvupdb";
                string backUpDir = $"{_filesRootFolder}\\dataBaseBackupFiles";
                string tempBackUpDir = $"{_localRootFolder}\\temp";

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
                var tempBackUpDirObj = Directory.CreateDirectory(tempBackUpDir);
                string newBackUpFileName = databaseName + "_" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm") + ".sql";
                string batchFileName = tempBackUpDirObj.FullName + @"\DbBackup.bat";

                if (File.Exists(batchFileName))
                {
                    File.Delete(batchFileName);
                }

                using (StreamWriter sw = File.CreateText(batchFileName))
                {
                    sw.WriteLine(@"@REM *** PARAMETERS/VARIABLES ***");
                    sw.WriteLine(@"SET BackupDir=" + backUpDirObj.FullName);
                    sw.WriteLine(@"SET TempBackupDir=" + tempBackUpDirObj.FullName);
                    sw.WriteLine(@"SET BackupFile=" + newBackUpFileName);

                    sw.WriteLine(@"SET postgresqldir=C:\Program Files\PostgreSQL\18\bin");

                    sw.WriteLine(@"SET pgpassword=!Shalot5");
                    sw.WriteLine(@"SET pguser=postgres");
                    sw.WriteLine(@"SET databaseName=" + databaseName);
                    sw.WriteLine(@"set zip=""C:\Program Files\7-Zip\7z.exe""");
                    sw.WriteLine(@"@REM *** EXECUTION ***");
                    sw.WriteLine(@"@REM Change to postgresqldir");
                    sw.WriteLine(@"CD %postgresqldir%");
                    sw.WriteLine(@"@REM dump/backup to temp directory");
                    sw.WriteLine(@"pg_dump -c -U %pguser% -d %databaseName% >""%TempBackupDir%\%BackupFile%""");
                    sw.WriteLine(@"@REM wait for file to be fully written");
                    sw.WriteLine(@"timeout /t 2 /nobreak");
                    sw.WriteLine(@"@REM zip backup file in temp directory");
                    sw.WriteLine(@"%zip% a -tgzip ""%TempBackupDir%\%BackupFile%.gz"" ""%TempBackupDir%\%BackupFile%""");
                    sw.WriteLine(@"@REM copy zipped file to backup directory");
                    sw.WriteLine(@"copy ""%TempBackupDir%\%BackupFile%.gz"" ""%BackupDir%\%BackupFile%.gz""");
                    sw.WriteLine(@"@REM delete files in temp directory");
                    sw.WriteLine(@"del ""%TempBackupDir%\%BackupFile%""");
                    sw.WriteLine(@"del ""%TempBackupDir%\%BackupFile%.gz""");
                }

                //execute the batch file
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = batchFileName;
                proc.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory;
                proc.Start();

                // keep only the newest 20 files in the backup directory
                DirectoryInfo backupDir = new DirectoryInfo(backUpDir);
                FileInfo[] allFiles = backupDir.GetFiles();
                // sort by last write time descending (newest first)
                Array.Sort(allFiles, (a, b) => b.LastWriteTime.CompareTo(a.LastWriteTime));
                for (int i = 20; i < allFiles.Length; i++)
                {
                    try
                    {
                        allFiles[i].Delete();
                    }
                    catch
                    {
                        // ignore individual delete errors
                    }
                }
            }
            catch (Exception ex)
            {
                string errorDetail = "error: " + ex.Message;
            }
        }
    }
}
