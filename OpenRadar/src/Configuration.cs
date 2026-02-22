using Dalamud.Configuration;
using ECommons.Configuration;
using System;

namespace OpenRadar;

public partial class Configuration
{
    public bool FirstInstalled { get; set;} = true;
    public bool RequestPackets { get; set;} = false;
    public bool PlayerTrackReader { get; set;} = false;
    public bool QueryTomestone { get; set;} = true;


    public void Save()
    {
        EzConfig.Save();
    }
}
