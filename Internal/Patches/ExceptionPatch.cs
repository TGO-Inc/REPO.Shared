using HarmonyLib;
using Shared.Core.Converters;
using Shared.Internal.Services;

namespace Shared.Internal.Patches;

[HarmonyPatch(typeof(Exception))]
internal class ExceptionPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("GetStackTrace", typeof(bool))]
    private static void GetStackTrace(Exception __instance, string? __result)
    {
        var unityStr = ExceptionStackTraceStringConverter.ConvertToUnityStackTraceString(__result ?? __instance.Message);
        ExceptionTracker.RegisterException(unityStr, __instance);
    }
}