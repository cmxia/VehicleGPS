using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;

namespace VehicleGPS.Services
{
   public  static class SpeechHelper
    {
       public static void speechContent(string str)
       {
           if (str.Length >0)
           {
               SpeechSynthesizer synth = new SpeechSynthesizer();
               string test = str;
               synth.Speak(test);
           }
       }
    }
}
