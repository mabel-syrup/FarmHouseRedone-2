using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using xTile;

namespace FarmHouseRedone.Maps
{
    public abstract class Facade
    {
        public Rectangle region;

        public Facade()
        {
            region = Rectangle.Empty;
        }

        public Facade(Rectangle region)
        {
            this.region = region;
        }

        public abstract void Apply(Map map, int index);

        public override string ToString()
        {
            return region.ToString();
        }
    }
}
