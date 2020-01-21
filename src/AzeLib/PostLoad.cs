﻿using Harmony;
using KMod;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AzeLib
{
    public abstract class PostLoad
    {
        protected abstract IEnumerable<MethodBase> PostLoadTargetMethods();

        [HarmonyPatch(typeof(Manager), nameof(Manager.Load))]
        private class Load_Patch
        {
            static void Postfix(Content content)
            {
                if (content != Content.Strings)
                    return;

                HarmonyInstance harmonyInstance = HarmonyInstance.Create("AzePostLoad");

                var instances = ReflectionHelpers.GetChildInstanceForType<PostLoad>();
                Debug.Log("Aze Instances: " + instances.Count() + ", from: " + Assembly.GetExecutingAssembly().GetName());

                foreach (var instance in instances)
                {
                    foreach (var method in instance.PostLoadTargetMethods())
                    {
                        HarmonyMethod harmonyPrefix = null;
                        HarmonyMethod harmonyPostfix = null;
                        HarmonyMethod harmonyTranspiler = null;

                        // GetCustomAttributes on MethodInfo
                        if (AccessTools.Method(instance.GetType(), "Prefix") != null)
                            harmonyPrefix = new HarmonyMethod(instance.GetType(), "Prefix");
                        if (AccessTools.Method(instance.GetType(), "Postfix") != null)
                            harmonyPostfix = new HarmonyMethod(instance.GetType(), "Postfix");
                        if (AccessTools.Method(instance.GetType(), "Transpiler") != null)
                            harmonyTranspiler = new HarmonyMethod(instance.GetType(), "Transpiler");

                        Debug.Log("METHOD: " + method + "; patched with: (" + harmonyPrefix + ", " + harmonyPostfix + ", " + harmonyTranspiler + ")");
                        harmonyInstance.Patch(method, harmonyPrefix, harmonyPostfix, harmonyTranspiler);
                    }
                }
            }
        }
    }
}