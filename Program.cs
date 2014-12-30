
namespace KeepAlive
{
    using System;
    using System.Threading;

    class Program
    {
        const string UniqueApplicationId = "89627B5E-BE19-4951-B037-07A127054D2E";

        static void Main(string[] args)
        {
            CurrentApplication.EnsureSingleInstance(UniqueApplicationId);
            CurrentApplication.EnsureBackgroundWorker();
            CurrentApplication.MakeProgramAutoRun();

            // prevent the machine from sleeping
            ExecutionStatusHelper.PreventSleep();

            Console.WriteLine("Machine configured to not sleep.");
            
            // Sleep forever. 
            // The thread execution state is only valid if the thread is still in an executable state.
            Thread.Sleep(Timeout.Infinite);
        }

    }
}
