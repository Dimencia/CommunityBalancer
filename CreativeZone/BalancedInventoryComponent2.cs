using Globals;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CommunityBalancer
{
    // This wasn't enough, it's referenced in far too many places
    //[HarmonyPatch(typeof(InventoryComponent2), "GetMaxPossibleSettlerAmount")]
    class BalancedInventoryComponent2
    {
        public static bool Prefix(ResourceComponent.ResourceType resource, ref int __result)
        {
            FileLog.Log("Clamping: " + Globals.Data.SessionConfigData.algorithm.defaultCarryWeightWorker);
            __result = Mathf.FloorToInt((float)((Data.SessionConfigData.algorithm.defaultCarryWeightWorker) / Data.ResourceConfigData.GetResourceInfo(resource).weight)); // Halve worker carry weight
            FileLog.Log("Result: " + __result);
            return false; // Skip original and use our result
        }
    }
}
