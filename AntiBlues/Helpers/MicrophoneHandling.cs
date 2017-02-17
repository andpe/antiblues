using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AntiBlues.Helpers
{
    class MicrophoneHandling
    {
        private WindowsMicrophoneMuteLibrary.WindowsMicMute mMicrophoneMuter;
        private Timer mTimer;
        private int mCooldown;

        public MicrophoneHandling(int cooldown)
        {
            mMicrophoneMuter = new WindowsMicrophoneMuteLibrary.WindowsMicMute();
            mCooldown = cooldown;

            // TODO: Make this configurable
            mTimer = new Timer();
            mTimer.Interval = mCooldown;
            mTimer.Elapsed += UnmuteMic;
            mTimer.AutoReset = false;
        }

        public void TriggeredLimit()
        {
            mMicrophoneMuter.MuteMic();

            // Reset the timer if we trigger it again while it's still ongoing.
            mTimer.Stop();
            mTimer.Interval = mCooldown;
            mTimer.Start();
        }

        public void UnmuteMic(object sender, ElapsedEventArgs e)
        {
            mMicrophoneMuter.UnMuteMic();
        }
    }
}
