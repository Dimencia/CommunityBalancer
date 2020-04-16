using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CommunityBalancer.Utils;
using ECS;
using HarmonyLib;
using Service;
using Service.Achievement;
using Service.Building;
using Service.Localization;
using Service.Street;
using Service.TaskService2;
using Service.UserWorldTasks;
using Zenject;

namespace CommunityBalancer
{
    public class ServiceInjector
    {
        private static DiContainer Core => global::Kernel.Instance.Container;
        internal static Harmony Harmony;

        private static bool ServicesInstalled = false;
        private static bool PatchesInstalled = false;
        private static bool WaitForDebugger = false;

        public static void InstallModServices()
        {
            try
            {
                InstallModServicesImpl();
            }
            catch (Exception e)
            {
                FileLog.Log(e.ToString());
                throw;
            }
        }

        public static void InstallModPatches()
        {
            try
            {
                InstallModPatchesImpl();
            }
            catch (Exception e)
            {
                FileLog.Log(e.ToString());
                throw;
            }
        }

        public static void InstallModServicesImpl()
        {
            if (ServicesInstalled)
                return;

            ServicesInstalled = true;

            while (WaitForDebugger && Debugger.IsAttached == false)
            { }

            try
            {

                using (Log.Debug.OpenScope("Installing services", string.Empty))
                {
                    Install<IEntityManager, BalancedEntityManager>();
                    Install<Service.Settler.ISettlerService, BalancedSettlerService>();
                    Install<ITaskService, BalancedTaskService>();
                }


                FileLog.Log("Install completed");
            }
            catch (Exception e)
            {
                FileLog.Log("Exception during service install:");
                FileLog.Log(e.ToString());
                throw;
            }
        }

        public static void InstallModPatchesImpl()
        {
            if (PatchesInstalled)
                return;

            PatchesInstalled = true;

            while (WaitForDebugger && Debugger.IsAttached == false)
            { }

            try
            {

                FileLog.Log("Installing patches");

                Harmony.DEBUG = true;
                Harmony.PatchAll();

                FileLog.Log("Install completed");
            }
            catch (Exception e)
            {
                FileLog.Log("Exception during service install:");
                FileLog.Log(e.ToString());
                throw;
            }
        }

        private static void Install<TService, TImpl>()
        {
            using (Log.Debug.OpenScope($"Installing {typeof(TImpl)} -> {typeof(TService)} ... ", "Done"))
            {
                var vanilla = Core.Resolve<TService>();
                var installMethod = typeof(TImpl).GetMethod("Install", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                installMethod.Invoke(null, new object[] { Harmony, vanilla, typeof(TService) });
            }
        }

    }
}
