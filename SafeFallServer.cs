using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace SafeFallServer
{
    [BepInPlugin(
        "com.lucas.safeFallServer",
        "Safe Fall Server",
        "1.0.0"
    )]
    public class SafeFallPlugin : BaseUnityPlugin
    {
        public static float MaxFallTime;

        private Harmony harmony;

        private void Awake()
        {
            MaxFallTime = Config.Bind(
                "General",
                "MaxFallSeconds",
                5f,
                "Seconds before teleporting falling players"
            ).Value;

            harmony = new Harmony("com.lucas.safeFallServer");
            harmony.PatchAll();

            Logger.LogInfo("SafeFallServer loaded");
        }
    }

    public class FallData
    {
        public float FallTime = 0f;
        public Vector3 LastGroundedPosition;
    }

    [HarmonyPatch(typeof(Player), "Update")]
    public static class PlayerUpdatePatch
    {
        private static readonly Dictionary<long, FallData> PlayerData
            = new Dictionary<long, FallData>();

        private static bool IsAdmin(Player player)
        {
            if (ZNet.instance == null)
                return false;

            return ZNet.instance.IsAdmin(player.GetPlayerID());
        }

        static void Postfix(Player __instance)
        {
            // Server only
            if (!ZNet.instance || !ZNet.instance.IsServer())
                return;

            // Admin immunity
            if (IsAdmin(__instance))
                return;

            long id = __instance.GetPlayerID();

            if (!PlayerData.TryGetValue(id, out FallData data))
            {
                data = new FallData();
                PlayerData[id] = data;
            }

            bool grounded = __instance.IsOnGround();

            if (grounded)
            {
                data.FallTime = 0f;
                data.LastGroundedPosition = __instance.transform.position;
            }
            else
            {
                data.FallTime += Time.deltaTime;

                if (data.FallTime >= SafeFallPlugin.MaxFallTime)
                {
                    __instance.TeleportTo(
                        data.LastGroundedPosition,
                        __instance.transform.rotation,
                        true
                    );

                    data.FallTime = 0f;
                }
            }
        }
    }
}
