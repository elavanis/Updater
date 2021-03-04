using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            string programToExecute = AppContext.GetData("ProgramToExecute").ToString();

            try
            {
                string directoryToCheckForUpdates = AppContext.GetData("DirectoryToCheckForUpdates").ToString();

                bool newer = CheckIfNeedToUpdate(programToExecute, directoryToCheckForUpdates);

                if (newer == true)
                {
                    UpdateFiles(directoryToCheckForUpdates);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
            }

            finally
            {
                Process.Start(programToExecute);
            }
        }

        private static void UpdateFiles(string directoryToCheckForUpdates)
        {
            HashSet<string> filesToExclude = new HashSet<string>(AppContext.GetData("FilesToExclude").ToString().Split(','));

            string[] files = Directory.GetFiles(directoryToCheckForUpdates);
            foreach (string remoteFile in files)
            {
                string localFile = Path.GetFileName(remoteFile);
                if (!filesToExclude.Contains(localFile))
                {
                    File.Copy(remoteFile, localFile, true);
                }
            }
        }

        private static bool CheckIfNeedToUpdate(string programToExecute, string directoryToCheckForUpdates)
        {
            //if the program to update isn't local return true so its copied
            if (!File.Exists(programToExecute))
            {
                return true;
            }

            bool newer = false;
            string remoteFilePath = Path.Combine(directoryToCheckForUpdates, programToExecute);

            FileInfo existingFile = new FileInfo(programToExecute);
            FileInfo remoteFile = new FileInfo(remoteFilePath);

            if (remoteFile.CreationTimeUtc > existingFile.CreationTimeUtc)
            {
                newer = true;
            }

            return newer;
        }
    }
}
