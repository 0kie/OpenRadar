using Dalamud.Configuration;
using ECommons.Configuration;
using System;

namespace OpenRadar;

public partial class Configuration
{
    public int Version { get; set; } = 0;

    public void Save()
    {
        EzConfig.Save();
    }
}
