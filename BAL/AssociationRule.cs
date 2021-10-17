using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public class AssociationRule
    {
        public List<string> Label { get; set; }
        public List<string> Label1 { get; set; }
        public double Confidance { get; set; }
        public double Support { get; set; }
    }
}
