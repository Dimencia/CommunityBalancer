using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBalancer
{
    [HarmonyPatch(typeof(BuildingWaterTowerComponent), "OnConstruct")] // I think this fires as if it's initialize
    class BalancedBuildingWaterTowerComponent
    {
        public static void Postfix(ref BuildingWaterTowerComponent __instance)
        {
            FileLog.Log("Setting water per gather task");
            __instance.SetWaterPerGatherTask(12); // Note, at current version, this doesn't work.  They're setting it later somewhere
        }
    }


    [HarmonyPatch(typeof(Systems.BuildingLogic.WaterTowerBuildingLogic), "CreateTasks")]
    class BalancedWaterTowerBuildingLogic
    {
        public static void Prefix(ref Systems.BuildingLogic.WaterTowerBuildingLogic __instance, ref Systems.BuildingTaskSystemComponents buildingComponents)
        {
            buildingComponents.waterTower.waterPerGatherTask = 12; // This is here until hopefully eventually they change it in a better spot
            //FileLog.Log("Creating task - water per gather task: " + buildingComponents.waterTower.waterPerGatherTask);
        }
    }
}
