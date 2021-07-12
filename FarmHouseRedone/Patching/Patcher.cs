using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using StardewModdingAPI;
using Harmony;

namespace FarmHouseRedone
{
    public static class Patcher
    {
        public static HarmonyInstance harmony = HarmonyInstance.Create("mabelsyrup.farmhouse");
        public static IReflectionHelper reflector;
    }
}
