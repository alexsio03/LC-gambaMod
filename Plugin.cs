using BepInEx;
using BepInEx.Logging;
using gamba.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gamba
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class GambaBase : BaseUnityPlugin
    {
        private const string modGUID = "Alexsio.GambaMod";
        private const string modName = "GambaMod";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static GambaBase Instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("Gamba mod up and awake");

            harmony.PatchAll(typeof(GambaBase));
            harmony.PatchAll(typeof(TerminalPatch));
        }
    }
}
