# EDIC
Elite:Dangerous Inara connector

Connect your data to Inara with this app.

# How to use
1. Open Elite dangerous
2. Choose your gamemode
3. Open EDIC
4. Enjoy

# Plugins help
Entrypoint method in dll is Main, args: name: "Event" type: "dynamic"

Code example(used nuget packages: System.Speech, Dynamitey, ElitAPI):
```c#
using System;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using Dynamitey;

namespace EDcombatHeler
{
    public class MainClass
    {
        private SpeechSynthesizer speech = new SpeechSynthesizer();
        public void Main(dynamic Event)
        {
            speech.SelectVoice("some SpeechSynthesizer");
            speech.Rate = 2;
            if((string)GetPropertyFromDynamic(Event, "event") == "ShipTargeted")
            {
                if(GetPropertyFromDynamic(Event, "ScanStage") != null && (int)GetPropertyFromDynamic(Event, "ScanStage") > 2)
                {
                    if((string)GetPropertyFromDynamic(Event, "LegalStatus") == "Clean")
                    {
                        speech.Speak("Ship is clear");
                    }
                    if((string)GetPropertyFromDynamic(Event, "LegalStatus") == "Wanted")
                    {
                        speech.Speak("Ship has bounty, count " + (string)GetPropertyFromDynamic(Event, "Bounty") + " кр");
                    }
                    if((string)GetPropertyFromDynamic(Event, "LegalStatus") == "Hunter")
                    {
                        speech.Speak("Ship has bouty in other sys, count " + (string)GetPropertyFromDynamic(Event, "Bounty") + " кр");
                    }
                }
            }
        }
        private object GetPropertyFromDynamic(dynamic d, string val)
        {
            var PropetyValue = Dynamic.InvokeGet(d, val);
            return PropetyValue;
        }
    }
}
```
