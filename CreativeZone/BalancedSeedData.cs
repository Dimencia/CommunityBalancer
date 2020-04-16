using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBalancer
{
    [HarmonyPatch(typeof(Globals.SeedData), "Initialize")]
    public class BalancedSeedData
    {

        //static AccessTools.FieldRef<Globals.SeedData, List<MonoSeedComponent>> fieldSeedsRef = AccessTools.FieldRefAccess<Globals.SeedData, List<MonoSeedComponent>>("fieldSeeds");
        // I think we can't access public fields...?  
        // Which means we need to do this as a Postfix and adjust our LUT and _fieldSeeds

        static AccessTools.FieldRef<Globals.SeedData, List<Globals.SeedData.Seed>> fieldSeedsRef = AccessTools.FieldRefAccess<Globals.SeedData, List<Globals.SeedData.Seed>>("_fieldSeeds");
        static AccessTools.FieldRef<Globals.SeedData, Dictionary<int, Globals.SeedData.Seed>> LUTRef = AccessTools.FieldRefAccess<Globals.SeedData, Dictionary<int, Globals.SeedData.Seed>>("seedLUT");

        // , ref Dictionary<int, Globals.SeedData.Seed> ___seedLUT
        public static void Postfix(Globals.SeedData __instance)
        {
            FileLog.Log("Pre-initialize...");
            
            foreach(var component in fieldSeedsRef(__instance))
            {
                float newValue = 0;
                    switch (component.seedComponent.seedType)
                    {
                        case(ResourceComponent.ResourceType.Cabbage):
                            newValue = 2.25f;
                            break;
                        case (ResourceComponent.ResourceType.Beans):
                            newValue = 1.6f; // This hits both wheat and beans, wheat was supposed to be slower but oh well
                            break;
                        case (ResourceComponent.ResourceType.Potatoe):
                            newValue = 3.6f;
                            break;
                        case (ResourceComponent.ResourceType.Corn):
                            newValue = 1f;
                            break;
                        case (ResourceComponent.ResourceType.Pepper):
                            newValue = 0.5f;
                            break;
                        case (ResourceComponent.ResourceType.Wheat):
                            newValue = 1.75f;
                            break;
                        case (ResourceComponent.ResourceType.Squash):
                            newValue = 2.875f;
                            break;
                    }
                if (newValue != 0)
                {
                    FileLog.Log("Editing seed " + component.seedComponent.seedType + " and setting new value to " + newValue);
                    component.seedComponent.growTime = newValue;
                    LUTRef(__instance)[(int)component.seedComponent.seedType] = component;
                }
            }
            
        }

        //[HarmonyReplace]
        //public void SetGrowTime(float value, bool addToLuaReplay = false, string luaValue = null)
        //{
        //    // I have some specific values I want here so IDK how else to best do this... 
        //    // I hope this.seedType gets set before this
        //
        //    float newValue = 0;
        //    switch (this.Vanilla.seedType)
        //    {
        //        case(ResourceComponent.ResourceType.Cabbage):
        //            newValue = 2.25f;
        //            break;
        //        case (ResourceComponent.ResourceType.Beans):
        //            newValue = 1.6f; // This hits both wheat and beans, wheat was supposed to be slower but oh well
        //            break;
        //        case (ResourceComponent.ResourceType.Potatoe):
        //            newValue = 3.6f;
        //            break;
        //        case (ResourceComponent.ResourceType.Corn):
        //            newValue = 1f;
        //            break;
        //        case (ResourceComponent.ResourceType.Pepper):
        //            newValue = 0.5f;
        //            break;
        //        case (ResourceComponent.ResourceType.Wheat):
        //            newValue = 1.75f;
        //            break;
        //        case (ResourceComponent.ResourceType.Squash):
        //            newValue = 2.875f;
        //            break;
        //    }
        //    if (newValue != 0)
        //        this.Vanilla.SetGrowTime(newValue, addToLuaReplay, luaValue);
        //    else
        //        this.Vanilla.SetGrowTime(value, addToLuaReplay, luaValue);
        //}
    }
}
