using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using xTile.Display;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Maps
{
    public static class Display
    {
        public static IDisplayDevice display;

        public static void Init()
        {
            display = new XnaDisplayDevice(Game1.content, Game1.graphics.GraphicsDevice);
        }
    }
}
