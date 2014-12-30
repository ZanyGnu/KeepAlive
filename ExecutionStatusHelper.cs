
namespace KeepAlive
{
    using System;
    using System.Runtime.InteropServices;

    /*
     * This setting is per thread/application not global, so if you go to ES_CONTINUOUS 
     * and another app/thread is still setting ES_DISPLAY the display will be kept on.
     * Setting a flag without also using ES_CONTINUOUS merely resets the idle timer, 
     * so it does not prevent sleep for an extended period.
    */
    class ExecutionStatusHelper
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);


        public static void PreventMonitorPowerdown()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }

        public static void AllowMonitorPowerdown()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        public static void PreventSleep()
        {
            // Prevent Idle-to-Sleep (monitor not affected) (see note above)
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
        }

        public static void KeepSystemAwake()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED);
        }

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }
    }
}
