using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmHouseRedone.IO
{
    public class DecoratableModel
    {
        public Dictionary<string, int[]> Rooms { get; set; }

        public DecoratableModel()
        {
            Rooms = new Dictionary<string, int[]>();
        }
    }
}
