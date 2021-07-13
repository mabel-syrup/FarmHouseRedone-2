using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace FarmHouseRedone.UI
{
    public static class Translation
    {
        public static ITranslationHelper translation;

        public static string Translate(string key)
        {
            return translation.Get(key);
        }

        public static string Translate(string key, params object[] args)
        {
            try
            {
                return string.Format(Translate(key), args);
            }
            catch (FormatException)
            {
                return Translate(key);
            }
        }
    }
}
