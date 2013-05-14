using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomasulo.Core
{
    public class StoreQueueItem
    {
        public string Header { get; set; }

        public string Busy { get; set; }
        public string Address { get; set; }
    }
}
