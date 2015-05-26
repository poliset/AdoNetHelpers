using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONETHelpers.Ado
{
    public class AdoNetConnectionStatistics
    {
        public long ExecutionTime { get; set; }
        public long BytesReceived { get; set; }

        public AdoNetConnectionStatistics(IDictionary status)
        {
            if (status.Contains("ExecutionTime"))
                ExecutionTime = long.Parse(status["ExecutionTime"].ToString());
            if (status.Contains("BytesReceived"))
                ExecutionTime = long.Parse(status["BytesReceived"].ToString());
        }
    }
}
