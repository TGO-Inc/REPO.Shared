using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Sentry;
using Sentry.Unity;
using Shared.Core;
using Shared.Core.Converters;

namespace Shared;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
internal sealed class Entry : BaseUnityPlugin
{
    private const string PluginGuid = "tgo.shared";
    private const string PluginName = "Shared";
    private const string PluginVersion = "1.1.4.0";
    private static readonly Harmony Harmony = new(PluginGuid);
    
    internal static ManualLogSource LogSource { get; } = BepInEx.Logging.Logger.CreateLogSource(PluginGuid);
    
#if GH_RELEASE || GH_DEBUG
    internal static object? SentryLifetimeObject;
    internal static object? SentrySDK => ((SentryUnitySdk?)SentryLifetimeObject)?.SentrySdk;
    private static bool _isAwake;

    internal static void CaptureException(Exception ex)
    {
        if (!_isAwake)
        {
            LogSource.LogWarning(ex);
            return;
        }

        ((SentrySdk?)SentrySDK)?.CaptureException(ex);
    }
#else
    internal static SentryUnitySdk? SentryLifetimeObject;
    internal static SentrySdk? SentrySDK => SentryLifetimeObject?.SentrySdk;
    internal static void CaptureException(Exception ex) => SentrySDK?.CaptureException(ex);
#endif

    internal Entry()
    {
        API.Init();
    }

    
    
    private void Awake()
    {
#if GH_RELEASE || GH_DEBUG
        _isAwake = true;
#endif
        
        SentryLifetimeObject = SentryUnity.Init(options =>
        {
            options.Dsn = "https://6a470a1f74d44dd3dfc24e520cae407d@devsentry.theguy920.dev/5";
            options.DiagnosticLogger = new BepLog2SenLog(LogSource, SentryLevel.Debug);
            options.AutoSessionTracking = true;
#if GH_RELEASE
            options.Release = $"GH-{PluginName}@{PluginVersion}";
            options.Environment = "production";
#elif MEGA_DEBUG
            options.Debug = true;
            options.Environment = "development";
            options.DiagnosticLevel = SentryLevel.Debug;
#elif !DEBUG
            options.Release = $"{PluginName}@{PluginVersion}";
            options.Environment = "production";
#endif
        });
        
        Harmony.PatchAll();
    }
}