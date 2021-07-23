﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModManager
{
    [HarmonyPatch(typeof(ModsScreen), nameof(ModsScreen.OnActivate))]
    [HarmonyPriority(Priority.Last)]
    public class OverrideModsScreen
    {
        public static List<ModUIExtract> ModUIExtractions { get; set; }

        static void Postfix(ModsScreen __instance)
        {
            ModUIExtractions = ExtractFromMods(__instance.displayedMods);
            LogDefaultScreen(__instance);

            // Instantiate the custom mods screen.
            new AModsScreen().GetDialog().Activate();

            // This is allowed to activate so that necessary information can be extracted.
            // But it must be deactivated so that there aren't two UIs.
            __instance.Deactivate();
        }

        private static List<ModUIExtract> ExtractFromMods(List<ModsScreen.DisplayedMod> mods) =>
            mods.Select(x => new ModUIExtract(x)).ToList();

        private static void LogDefaultScreen(ModsScreen modsScreen)
        {
            LogGO(modsScreen.entryPrefab);
            foreach (var displayedMod in modsScreen.displayedMods)
                LogGO(displayedMod.rect_transform.gameObject);
        }

        private static void LogGO(GameObject go)
        {
            Debug.Log(go.name);
            foreach (Transform child in go.transform)
                Debug.Log("   " + child.name);
        }
    }
}