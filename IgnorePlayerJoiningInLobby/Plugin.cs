using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;

namespace IgnorePlayerJoiningInLobby;

[BepInPlugin("com.kacpermarcinkiewicz.repo.IgnorePlayerJoiningInLobby", "Ignore Player Joining In Lobby", "1.0.2")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    
    public static string GUID => "com.kacpermarcinkiewicz.repo.IgnorePlayerJoiningInLobby";
        
    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Loaded!");
        
        var harmony = new HarmonyLib.Harmony(GUID);
        harmony.PatchAll();
    }

    [HarmonyPatch]
    public static class UpdatePatch
    {
        [HarmonyPatch(typeof(MenuPageLobby), "Update")]
        [HarmonyPostfix]
        public static void Postfix(MenuPageLobby __instance)
        {
            if (__instance.startButton != null)
            {
                // find private field 'disabled' in MenuPageLobby.startButton
                var field = AccessTools.Field(__instance.startButton.GetType(), "disabled");

                if (field != null)
                {
                    field.SetValue(__instance.startButton, false); // force unlock button
                }
                else
                {
                    Logger.LogError("Private field 'disabled' not found for btn!");
                }
            }
            else
            {
                if (__instance == null)
                {
                    Logger.LogError("MenuPageLobby instance is null!");
                }
                else if (__instance.startButton == null)
                {
                    Logger.LogError("Start button is null!");
                }
            }
        }
    }

    [HarmonyPatch]
    public static class LobbyMenuPatch
    {
        [HarmonyPatch(typeof(MenuPageLobby), "ButtonStart")]
        [HarmonyPrefix]
        public static void Prefix(MenuPageLobby __instance)
        {
            // find private field 'joiningPlayer' in MenuPageLobby
            var field = AccessTools.Field(typeof(MenuPageLobby), "joiningPlayer");

            if (field != null)
            {
                field.SetValue(__instance, false); // disable "Player joining..." lock
                Logger.LogInfo("I'm disabling 'Player joining...' btn lock");
            }
            else
            {
                Logger.LogError("Private field 'joiningPlayer' not found!");
            }
        }
    }
}
