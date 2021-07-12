using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmHouseRedone.IO
{
    public class DecoratableModel
    {
        public Dictionary<string, int[]> rooms { get; set; }

        public DecoratableModel()
        {
            rooms = new Dictionary<string, int[]>();
        }
    }
}
