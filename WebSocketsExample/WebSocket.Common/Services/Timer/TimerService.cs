using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketCommon.Services.Timer
{
    public class TimerService : ITimerService
    {
        public void TestTimer()
        {
            //default ctor has callback function that just logs out the time
            //using (
            var timer = new MMTimer();
                //)
            //{
                //one shot, to test
                timer.Start(1, false);
                //shut it off
                //timer.Stop();
            //}
        }
    }
}
