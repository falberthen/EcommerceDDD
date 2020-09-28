using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceDDD.WebApp.BackgroundServices
{
    public class MessageProcessorTaskOptions
    {
        public TimeSpan Interval { get; }
        public int BatchSize { get; }

        public MessageProcessorTaskOptions(TimeSpan interval, int batchSize)
        {
            Interval = interval;
            BatchSize = batchSize;
        }
    }
}
