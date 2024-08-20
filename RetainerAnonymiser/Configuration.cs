using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using ECommons.DalamudServices;
using Newtonsoft.Json.Linq;
using System.IO;

namespace RetainerAnonymiser;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public bool HideRetainerNames = true;
    public bool HideRetainerGil = true;
    public bool AutomaticallyEnableOnLogin = true;

    public string RetainerAnonymisedName = "Anonymous";

    public void Save()
    {
        Svc.PluginInterface.SavePluginConfig(this);
    }

    public static Configuration Load()
    {
        try
        {
            var contents = File.ReadAllText(Svc.PluginInterface.ConfigFile.FullName);
            var json = JObject.Parse(contents);
            var version = (int?)json["Version"] ?? 0;
            return json.ToObject<Configuration>() ?? new();
        }
        catch (Exception e)
        {
            Svc.Log.Error($"Failed to load config from {Svc.PluginInterface.ConfigFile.FullName}: {e}");
            return new();
        }
    }
}
