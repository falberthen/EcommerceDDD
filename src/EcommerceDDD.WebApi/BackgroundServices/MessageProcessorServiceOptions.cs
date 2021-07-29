using System;

namespace EcommerceDDD.WebApi.BackgroundServices
{
    public class MessageProcessorServiceOptions
    {
        public double IntervalOnSeconds { get; set; }
        public int BatchSize { get; set; }
    }
}
