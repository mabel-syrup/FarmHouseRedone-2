using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.UI
{
    public class KeyFrame
    {
        public float time;
        public Vector2 destination;
        public int animationStyle;

        public KeyFrame(float time, Vector2 destination, int animationStyle = 0)
        {
            this.time = time;
            this.destination = destination;
            this.animationStyle = animationStyle;
        }
    }
}
