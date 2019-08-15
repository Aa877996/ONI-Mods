﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;

namespace SuppressNotifications
{
    [HarmonyPatch(typeof(Capturable), "OnPrefabInit")]
    class Patch_Capturable_OnPrefabInit
    {
        static void Postfix(Capturable __instance)
        {
            __instance.gameObject.AddComponent<SuppressionButton>();
        }
    }
}