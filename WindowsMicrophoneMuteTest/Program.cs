using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsMicrophoneMuteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Preparing Windows Microphone Mute Library...");
            WindowsMicrophoneMuteLibrary.WindowsMicMute micMute = new WindowsMicrophoneMuteLibrary.WindowsMicMute();
            Console.WriteLine("We will now mute the microphone, press enter to mute.");
            Console.ReadLine();
            micMute.MuteMic();
            Console.WriteLine("Microphone should now be muted, press enter to unmute.");
            Console.ReadLine();
            micMute.UnMuteMic();
            Console.WriteLine("Microphone should now be unmuted, press enter to quit.");
            Console.ReadLine();
        }
    }
}
