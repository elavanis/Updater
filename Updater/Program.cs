﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            string programToExecute = AppContext.GetData("ProgramToExecute").ToString();
            string executionFlags = AppContext.GetData("ExecutionFlags").ToString();
            try
            {
                HashSet<string> filesToExclude = new HashSet<string>(AppContext.GetData("FilesToExclude").ToString().Split(','));
                string directoryToCheckForUpdates = AppContext.GetData("DirectoryToCheckForUpdates").ToString();
                bool copyAlways = bool.Parse(AppContext.GetData("AlwaysCopy").ToString());

                string[] files = Directory.GetFiles(directoryToCheckForUpdates);
                foreach (string remoteFile in files)
                {
                    if (copyAlways || CheckIfNeedToUpdate(filesToExclude, remoteFile))
                    {
                        CopyRemoteFileLocal(remoteFile);
                    }
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
                Process.Start(programToExecute, executionFlags);
            }
        }

        private static void CopyRemoteFileLocal(string remoteFile)
        {
            string localFile = Path.GetFileName(remoteFile);
            Console.WriteLine($"Copying {remoteFile}");
            File.Copy(remoteFile, localFile, true);
        }

        private static bool CheckIfNeedToUpdate(HashSet<string> filesToExclude, string remoteFile)
        {
            string localFile = Path.GetFileName(remoteFile);

            if (filesToExclude.Contains(localFile))
            {
                return false;
            }

            if (!File.Exists(localFile))
            {
                return true;
            }

            FileInfo localFileInfo = new FileInfo(localFile);
            FileInfo remoteFileInfo = new FileInfo(remoteFile);

            if (remoteFileInfo.LastWriteTimeUtc > localFileInfo.LastWriteTimeUtc)
            {
                return true;
            }

            return false;
        }
    }
}
