using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevTools.Objects
{
    public class RevFilterRule
    {
        public string FilterCriteria { get; set; }
        public FilterOptions RevFilterOption { get; set; } // Use the enum type
        public string RevFilterCriteria { get; set; }

    }
}
