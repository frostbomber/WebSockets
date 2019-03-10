using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WebSocketCommon.Services.Timer
{
    public class MMTimer : IDisposable
    {
        #region Imports
        //Lib API declarations
        [DllImport("Winmm.dll", CharSet = CharSet.Auto)]
        static extern uint timeSetEvent(uint uDelay, uint uResolution, TimerCallback lpTimeProc, UIntPtr dwUser, uint fuEvent);

        [DllImport("Winmm.dll", CharSet = CharSet.Auto)]
        static extern uint timeKillEvent(uint uTimerID);

        [DllImport("Winmm.dll", CharSet = CharSet.Auto)]
        static extern uint timeGetTime();

        [DllImport("Winmm.dll", CharSet = CharSet.Auto)]
        static extern uint timeBeginPeriod(uint uPeriod);

        [DllImport("Winmm.dll", CharSet = CharSet.Auto)]
        static extern uint timeEndPeriod(uint uPeriod);
        #endregion

        #region Flags, Delegates
        //Timer type definitions
        [Flags]
        public enum fuEvent : uint
        {
            TIME_ONESHOT = 0,      //Event occurs once, after uDelay milliseconds. 
            TIME_PERIODIC = 1,
            TIME_CALLBACK_FUNCTION = 0x0000,  /* callback is function */
                                              //TIME_CALLBACK_EVENT_SET = 0x0010, /* callback is event - use SetEvent */
                                              //TIME_CALLBACK_EVENT_PULSE = 0x0020  /* callback is event - use PulseEvent */
        }

        //Delegate definition for the API callback
        delegate void TimerCallback(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2);
        #endregion

        #region IDisposable
        //IDisposable code
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Stop();
                }
            }
            disposed = true;
        }

        ~MMTimer()
        {
            Dispose(false);
        }
        #endregion

        /// <summary>
        /// The current timer instance ID
        /// </summary>
        uint id = 0;

        /// <summary>
        /// The callback used by the the API
        /// </summary>
        TimerCallback thisCB { get; set; }

        /// <summary>
        /// The timer elapsed event 
        /// </summary>
        public event EventHandler Timer;
        protected virtual void OnTimer(EventArgs e)
        {
            Timer?.Invoke(this, e);
        }

        public MMTimer()
        {
            //Initialize the API callback
            thisCB = CBFunc;
            Timer += MMTimer_TimerCompleted;
        }

        private void MMTimer_TimerCompleted(object sender, EventArgs e)
        {
            Console.WriteLine($"MMTimer {id.ToString()} completed at {DateTime.Now.Ticks}");
        }

        /// <summary>
        /// Stop the current timer instance (if any)
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                if (id != 0)
                {
                    timeKillEvent(id);
                    Console.WriteLine("MMTimer " + id.ToString() + " stopped");
                    id = 0;
                }
            }
        }

        /// <summary>
        /// Start a timer instance
        /// </summary>
        /// <param name="ms">Timer interval in milliseconds</param>
        /// <param name="repeat">If true sets a repetitive event, otherwise sets a one-shot</param>
        public void Start(uint ms, bool repeat)
        {
            //Kill any existing timer
            Stop();

            //Set the timer type flags
            fuEvent f = fuEvent.TIME_CALLBACK_FUNCTION | (repeat ? fuEvent.TIME_PERIODIC : fuEvent.TIME_ONESHOT);

            lock (this)
            {
                id = timeSetEvent(ms, 0, thisCB, UIntPtr.Zero, (uint)f);
                if (id == 0)
                    throw new Exception("timeSetEvent error");
                Console.WriteLine($"MMTimer {id.ToString()} started at {DateTime.Now.Ticks}");
            }
        }

        void CBFunc(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2)
        {
            //Callback from the MMTimer API that fires the Timer event. Note we are in a different thread here
            OnTimer(new EventArgs());
        }
    }
}
