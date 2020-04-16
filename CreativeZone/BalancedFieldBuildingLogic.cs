using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Systems;
using Systems.BuildingLogic;

namespace CommunityBalancer
{

    class FieldHelper
    {
        public static bool HasRipeness(FieldComponent field)
        {
            bool anyRipe = false;
            for (int x = 0; x < field.ripeness.XLength; x++)
            {
                for (int y = 0; y < field.ripeness.YLength; y++)
                {
                    if (field.ripeness[x, y] >= 1)
                    {
                        anyRipe = true;
                        break;
                    }
                }
                if (anyRipe)
                    break;
            }
            return anyRipe;
        }
    }

    [HarmonyPatch(typeof(FieldBuildingLogic), "CreateTasks")]
    class BalancedFieldCreateTasks
    {
        public static bool Prefix(ref FieldBuildingLogic __instance, ref BuildingTaskSystemComponents buildingComponents)
        {

            // If no ripe, and if we don't need to plant and aren't paused, and a seed is selected
            if (!buildingComponents.field.needsSeedPlanting && buildingComponents.field.seed.HasValue && buildingComponents.field.seedAmount >= buildingComponents.field.MaxSeedAmount)
            {
                bool anyRipe = FieldHelper.HasRipeness(buildingComponents.field); // This is heavy to put here, but... gotta do what we gotta do
                if (!anyRipe)
                {
                    var taskService = BalancedTaskService.Instance.Vanilla;

                    taskService.ChangeBuildingTaskPriority(buildingComponents.Entity, Service.TaskService2.TaskPriority.Default);
                    taskService.KillAllTasksForBuilding(buildingComponents.Entity); // If this is the actual TaskService we should be good.  If not we might have to override for a ref to it

                    buildingComponents.buildingRuntime.SetAssignedProfessionals(0); // Still doesn't reassign them, hope the prev thing did that
                    // Right so this seems mostly great... but then they reassign here immediately, so now we pause it
                    buildingComponents.buildingRuntime.SetDeactivateBuilding(true);


                    return false;
                }
            }
            //}
            return true;
        }

    }

    [HarmonyPatch(typeof(FieldBuildingLogic), "NeedsNewTasks")]
    class BalancedFieldNeedsNewTasks
    {
        public static void Postfix(ref FieldBuildingLogic __instance, ref BuildingTaskSystemComponents buildingComponents, ref bool __result)
        {
            if (!__result)
            {
                // If it didn't need new tasks, but it was paused...
                if (buildingComponents.buildingRuntime.deactivateBuilding)
                {
                    // See if it actually needs to be unpaused

                    // Unfortunately, we're iterating all of them... every time... but it's the only surefire way
                    bool anyRipe = FieldHelper.HasRipeness(buildingComponents.field);
                    if (anyRipe || buildingComponents.field.needsSeedPlanting || buildingComponents.field.seedAmount < buildingComponents.field.MaxSeedAmount)
                    {
                        // Then we just unpause it and return the new result of the vanilla function
                        // Because we can't easily check MaxCapReached or InventoryFull from here since those are protected
                        //if (anyRipe)
                        //    FileLog.Log("Found at least one ripe thing.  Overall ripeness: " + buildingComponents.field.overallRipeness); // This seems to be an average and unreliable for what we want
                        buildingComponents.buildingRuntime.SetDeactivateBuilding(false);
                        __result = __instance.NeedsNewTasks(buildingComponents);
                    }

                }
            }
        }
    }
}
