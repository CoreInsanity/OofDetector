using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace OofDetector.Helpers
{
    class Audio
    {
        private SoundPlayer KillPlayer;
        private SoundPlayer BootPlayer;

        public Audio()
        {
            try
            {
                KillPlayer = new SoundPlayer(@"C:\\Program Files (x86)\CorpInsanity\OofDetector\media\killsound.wav");
                KillPlayer.Load();
            }
            catch { }
            try
            {
                BootPlayer = new SoundPlayer(@"C:\\Program Files (x86)\CorpInsanity\OofDetector\media\boot.wav");
                BootPlayer.Load();
            }
            catch { }
            
        }
        public void PlayAudio(AudioSelect type)
        {
            try
            {
                switch (type)
                {
                    case AudioSelect.OOF:
                        KillPlayer.Play();
                        break;
                    case AudioSelect.WAKEUP:
                        BootPlayer.Play();
                        break;
                }
            }
            catch
            { 
                //User probably removed the audio file, ignore any errors
            }
        }
    }
    enum AudioSelect
    {
        OOF, WAKEUP
    }
}
