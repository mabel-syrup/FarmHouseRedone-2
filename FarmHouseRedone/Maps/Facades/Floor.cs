using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Maps
{
    public class Floor : Facade
    {

        public Floor(Rectangle rect)
        {
            this.region = rect;
        }

        public override void Apply(Map map, int index)
        {
            for (int x = region.X; x < region.Right; x++)
            {
                for (int y = region.Y; y < region.Bottom; y++)
                {
                    MapUtilities.SetFloorMapTileIndexForAnyLayer(map, x, y, 336 + index % 8 * 2 + index / 8 * 32);
                }
            }
        }
    }
}
