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
using System.Speech.Synthesis;
using EliteAPI.Events;
using EliteAPI;
using Dynamitey;

namespace EDcombatHeler
{
    public class MainClass
    {
        private SpeechSynthesizer speech = new SpeechSynthesizer();
        public void Main(dynamic Event)
        {
            speech.SelectVoice("Microsoft Irina Desktop");
            if((string)GetPropertyFromDynamic(Event, "event") == "ShipTargeted")
            {
                if((int)GetPropertyFromDynamic(Event, "ScanStage") > 2)
                {
                    if((string)GetPropertyFromDynamic(Event, "LegalStatus") == "Clean")
                    {
                        speech.Speak("Этот чист");
                    }
                    if((string)GetPropertyFromDynamic(Event, "LegalStatus") == "Wanted")
                    {
                        speech.Speak("За этого есть награда, размер " + (string)GetPropertyFromDynamic(Event, "Bounty"));
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
