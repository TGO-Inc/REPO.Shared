using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Shared.Internal;
using UnityEngine;

namespace Shared;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
internal sealed class Entry : BaseUnityPlugin
{
    private const string PluginGuid = "tgo.shared";
    private const string PluginName = "Shared";
    private const string PluginVersion = "1.0.0.0";
    private static readonly Harmony Harmony = new(PluginGuid);
    
    internal static ManualLogSource LogSource { get; } = BepInEx.Logging.Logger.CreateLogSource(PluginGuid);

    internal Entry()
    {
        Application.logMessageReceived += API.Log; 
        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.AssemblyResolve; 
    }
    
    private void Awake()
    {
        Harmony.PatchAll();
    }
}