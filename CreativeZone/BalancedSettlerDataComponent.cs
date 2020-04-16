using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBalancer
{
    // Doesn't seem to work.  We can get log messages about it firing (a lot), but doesn't affect their speeds at prefix or postfix
    //[HarmonyPatch(typeof(SettlerDataComponent), "OnConstruct")] 
    class BalancedSettlerDataComponent
    {
        //static AccessTools.FieldRef<SettlerDataComponent, float> speedRef = AccessTools.FieldRefAccess<SettlerDataComponent, float>("speed");
        public static void Prefix(SettlerDataComponent __instance)
        {
            __instance.speed = 5.0f; // 2.5f default
        }
    }
}
