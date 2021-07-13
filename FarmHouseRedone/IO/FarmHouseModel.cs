using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmHouseRedone.IO
{
    public class FarmHouseModel
    {
        public string PackID { get; set; }
        public string UpgradeID { get; set; }
        public int DaysUntilUpgrade { get; set; }
        public List<string> AppliedUpgrades { get; set; }
        public int[] Offset { get; set; }

    }
}
