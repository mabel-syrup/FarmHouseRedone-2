using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Maps
{
    public class Wall : Facade
    {
        public Wall(Rectangle rect)
        {
            this.region = rect;
        }

        public override void Apply(Map map, int index)
        {
            for (int x = region.X; x < region.Right; x++)
            {
                for (int y = region.Y; y < region.Bottom; y++)
                {
                    MapUtilities.SetWallMapTileIndexForAnyLayer(map, x, y, index % 16 + index / 16 * 48);
                }
            }
        }
    }
}
