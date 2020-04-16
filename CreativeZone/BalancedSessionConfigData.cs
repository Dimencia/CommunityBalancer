using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBalancer
{
    [HarmonyPatch(typeof(SessionConfigData),"Initialize")]
    class BalancedSessionConfigData
    {
        public static void Postfix(ref SessionConfigData __instance)
        {
            FileLog.Log("Adjusting carry weight in SessionConfigData");
            __instance.algorithm.defaultCarryWeightWorker = 50; // Default 100
            FileLog.Log("Adjusting forester chance to skip cutting in favor of planting");
            __instance.task.foresterProbabilityToSkipCut = 0.3f; // Default 0.1f
        }
    }
}
