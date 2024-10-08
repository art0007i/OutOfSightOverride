using HarmonyLib;
using ResoniteModLoader;
using System.Collections.Generic;
using FrooxEngine;
using System.Reflection.Emit;

namespace OutOfSightOverride;

public class OutOfSightOverride : ResoniteMod
{
    public override string Name => "OutOfSightOverride";
    public override string Author => "art0007i";
    public override string Version => "1.0.0";
    public override string Link => "https://github.com/art0007i/OutOfSightOverride/";
    public static ModConfiguration config;
    [AutoRegisterConfigKey]
    public static ModConfigurationKey<bool> KEY_ENABLED = new("enabled", "Whether the mod is enabled.", () => true);
    public override void OnEngineInit()
    {
        config = GetConfiguration();
        Harmony harmony = new Harmony("me.art0007i.OutOfSightOverride");
        harmony.PatchAll();
    }
    [HarmonyPatch(typeof(TouchSource), "UpdateCurrentTouchable")]
    class OutOfSightOverridePatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
        {
            var lookfor = typeof(ITouchable).GetProperty(nameof(ITouchable.CanTouchOutOfSight)).GetMethod;
            foreach (var code in codes)
            {
                if (code.Calls(lookfor))
                {
                    Debug("Transpiler ran successfully");
                    yield return new(OpCodes.Call, typeof(OutOfSightOverridePatch).GetMethod(nameof(OutOfSightProxy)));
                }
                else
                {
                    yield return code;
                }
            }
        }

        public static bool OutOfSightProxy(ITouchable touchable)
        {
            if (config.GetValue(KEY_ENABLED)) return true;
            return touchable.CanTouchOutOfSight;
        }
    }
}
