using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using StardewModdingAPI;
using HarmonyLib;

namespace FarmHouseRedone
{
    public static class Patcher
    {
        public static Harmony harmony = new Harmony("mabelsyrup.farmhouse");
        public static IReflectionHelper reflector;
    }
}
