# EDIC
Elite:Dangerous Inara connector

Connect your data to Inara and EDDN with this app.

# How to use
1. Open Elite dangerous
2. Choose your gamemode
3. Open EDIC
4. Enjoy

# Plugins help
> Entrypoint method in dll is Main, event provided as same as in game, code looks like
```c#
using System;
namespace MyPlugin
{
    public class MyPluginClass
    {
        public void Main(string Event)
        {
            
        }
    }
}
```
> If some dll librarys needed, then they're should putten in EDIC directory, not plugins directory.
