using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Shared;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
internal sealed class Entry : BaseUnityPlugin
{
    private const string PluginGuid = "tgo.shared";
    private const string PluginName = "Shared";
    private const string PluginVersion = "1.0.0.0";
    private static readonly Harmony Harmony = new(PluginGuid);
    
    internal static ManualLogSource LogSource { get; } = BepInEx.Logging.Logger.CreateLogSource(PluginGuid);
    
    private void Awake()
    {
        Harmony.PatchAll();
    }
}