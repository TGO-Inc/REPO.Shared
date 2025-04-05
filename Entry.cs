using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Sentry;
using Shared.Internal;
using UnityEngine;

namespace Shared;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
internal sealed class Entry : BaseUnityPlugin
{
    private const string PluginGuid = "tgo.shared";
    private const string PluginName = "Shared";
    private const string PluginVersion = "1.0.2.0";
    private static readonly Harmony Harmony = new(PluginGuid);
    
    internal static ManualLogSource LogSource { get; } = BepInEx.Logging.Logger.CreateLogSource(PluginGuid);

    internal Entry()
    {
        Application.logMessageReceived += API.Log; 
        AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.AssemblyResolve; 
        
        SentrySdk.Init(options =>
        {
            options.Dsn = "https://6a470a1f74d44dd3dfc24e520cae407d@devsentry.theguy920.dev/5";
            options.Debug = false;
            options.AutoSessionTracking = true;
            options.IsGlobalModeEnabled = true;
            options.AttachStacktrace = false;
            options.DisableFileWrite = true;
            options.StackTraceMode = StackTraceMode.Enhanced;
#if !DEBUG 
            options.Release = $"{PluginName}@{PluginVersion}";
#endif
        });

        SentrySdk.ConfigureScope(scope => { scope.Level = SentryLevel.Warning; });
        API.OnException += OnException;
    }

    private static string AsmRefName = typeof(Entry).Namespace!.ToLowerInvariant();
    private static void OnException(Exception obj, LogType logType)
    {
        var message = $"{obj.Message}{obj.Source}{obj.StackTrace}";
        if (!message.ToLowerInvariant().Contains(AsmRefName)) return;
        SentrySdk.CaptureException(obj);
    }
    
    private void Awake()
    {
        Harmony.PatchAll();
    }
}