
namespace KeepAlive
{
    using Microsoft.Win32;
    using System;
    using System.Diagnostics;
    using System.Threading;

    public class CurrentApplication
    {
        private static string UniqueBackgroundEnvName = "Env-C72EDBDD-0109-4989-8A08-FDC8E90079EB";
        private static string UniqueBackgroundEnvValue = "65BAF29A-4E4A-498D-A0A3-F5091CB71104";

        /// <summary>
        /// Calling this routine ensures that this program is automatically run when the current user logs back in.
        /// This will thus not require the user to re-run the program.
        /// </summary>
        public static void MakeProgramAutoRun()
        {
            RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            add.SetValue("Keep Machine Alive", "\"" + CurrentApplication.GetCurrentApplication() + "\"");
        }
        
        /// <summary>
        /// Calling this routine ensures that the currently running application (with the provided applicationId)
        /// is the only one running. If another application with the same unique id is already running, 
        /// this application will exit.
        /// </summary>
        /// <param name="uniqueApplicationId"></param>
        public static void EnsureSingleInstance(string uniqueApplicationId)
        {
            bool createdNew;

            Mutex mutex = new Mutex(
                true /* intially owned */,
                uniqueApplicationId,
                out createdNew);

            if (!createdNew)
            {
                // If this app was not the one that created the mutex, 
                // another instance of this app was already running. 
                // Exit silently.
                Console.WriteLine("App already running on this machine.");
                Environment.Exit(0);
            }

            // Let the mutext not be GC'ed away.
            GC.KeepAlive(mutex);
        }
        
        /// <summary>
        /// A background worker here is defined as an executable that doesnt consume up a cmd window or a console.
        /// Its precesence is available for inspection only in the proc list.
        /// </summary>
        public static void EnsureBackgroundWorker()
        {
            if (Environment.GetEnvironmentVariable(UniqueBackgroundEnvName) != UniqueBackgroundEnvValue)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(CurrentApplication.GetCurrentApplication());
                startInfo.EnvironmentVariables.Add(UniqueBackgroundEnvName, UniqueBackgroundEnvValue);
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = false;

                // start the background process and exit
                Process.Start(startInfo);

                Environment.Exit(0);
            }
        }

        private static string GetCurrentApplication()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }
    }
}
