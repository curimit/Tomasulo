using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomasulo.Core
{
    public class LoadItem
    {
        public string Header { get; set; }

        public bool IsBusy { get; set; }
        public int Address { get; set; }
    }
}
