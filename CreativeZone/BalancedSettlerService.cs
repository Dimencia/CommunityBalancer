using ECS;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBalancer
{

    public class BalancedSettlerService : CreativeService<BalancedSettlerService, Service.Settler.ISettlerService>
    {

        [HarmonyReplace]
        public UID CreateSettler(SettlerDataComponent.Profession profession, SettlerDataComponent.Gender gender, float age, GridPosition position)
        {
            var uid = this.Vanilla.CreateSettler(profession, gender, age, position);
            var component = BalancedEntityManager.Instance.Vanilla.GetComponent<SettlerDataComponent>(uid);
            component.speed = 3.5f; // Default 2.5f
            this.Vanilla.UpdateSettlerRenderComponent(uid);
            BalancedEntityManager.Instance.Vanilla.EntityModified(uid);
            return uid;
        }

    }
}
