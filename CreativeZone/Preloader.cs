using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommunityBalancer.Utils;
using HarmonyLib;
using Zenject;

namespace CommunityBalancer
{
    public static class Preloader
    {
        /// <summary>
        /// Invoked by Doorstop
        /// </summary>
        public static void Main()
        {
            try
            {
                Harmony.DEBUG = true;

                FileLog.Log(DateTime.Now.ToString());
                FileLog.Log("Loading CommunityBalancer");

                ServiceInjector.Harmony = new Harmony("CommunityBalancer");

                LoadGameAssembly();
                PatchGameStartup();

                //ServiceInjector.Harmony.PatchAll();
                //ServiceInjector.Harmony.Patch(typeof(Globals.SeedData).GetMethod("Initialize"), postfix: new HarmonyMethod(typeof(BalancedSeedData).GetMethod(nameof(BalancedSeedData.Postfix))));

                FileLog.Log("Startup inject done");

                // Let's see if we can't... traverse the structure for anything else in this dir with a valid entry point

                // So, this was a neat idea, but I think windows defender prevents it from running silently when this is here
                // It makes it take a very long time, the exe never starts so I can't kill the process, but steam won't let it stop either
                // Something's checking it out imo

                /*


                var targetDir = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Endzone_Data" + Path.DirectorySeparatorChar + "Managed";
                string[] ignoreList = new string[] { "0Harmony.dll", "AfterTheEnd.dll" }; // IDK if these have them yet, may be unnecessary

                foreach(string filepath in Directory.GetFiles(targetDir, "*.dll"))
                {
                    //FileLog.Log("Checking " + filepath + " for entry point");
                    var dll = Assembly.LoadFile(filepath);
                    foreach(Type t in dll.ExportedTypes)
                    {
                        if (t.Name == "Preloader")
                        {
                            FileLog.Log("Checking type " + t.FullName);
                            foreach (var m in t.GetRuntimeMethods())
                            {
                                FileLog.Log("Checking Func " + m.Name);
                                if (m.Name == "Main")
                                {
                                    FileLog.Log("Found enterable file: " + filepath);
                                    FileLog.Log("Loading mod...");
                                    object result = m.Invoke(null,null);
                                    FileLog.Log("Finished with result " + result);
                                }
                            }
                        }
                    }
                        
                }
                */
            }
            catch(Exception e)
            {
                FileLog.Log("Exception during init:");
                FileLog.Log(e.ToString());
                
            }
        }

        private static void LoadGameAssembly()
        {
            var t = typeof(AfterTheEndKernel);
        }

        private static void PatchGameStartup()
        {
            var method = typeof(Zenject.ProjectContext).GetMethod("EnsureIsInitialized"); // Patches need to go in ASAP in case they override initialize
            ServiceInjector.Harmony.Patch(method, null, new HarmonyMethod(typeof(ServiceInjector).GetMethod(nameof(ServiceInjector.InstallModPatches))));
            method = typeof(SceneContext).GetMethod("Awake"); // Services may need to be installed later after a scene starts
            ServiceInjector.Harmony.Patch(method, null, new HarmonyMethod(typeof(ServiceInjector).GetMethod(nameof(ServiceInjector.InstallModServices))));

        }
    }
}
