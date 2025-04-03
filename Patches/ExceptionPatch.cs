using System;
using HarmonyLib;
using Shared.Internal;

namespace Shared.Patches;

[HarmonyPatch(typeof(Exception))]
internal class ExceptionPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("GetStackTrace", typeof(bool))]
    private static void GetStackTrace(Exception __instance, string __result)
    {
        var unityStr = ExceptionStackTraceStringConverter.ConvertToUnityStackTraceString(__result);
        API.Exceptions.TryAdd(unityStr, __instance);
    }
}