using System;

namespace EcommerceDDD.WebApp.BackgroundServices
{
    public class MessageProcessorServiceOptions
    {
        public double IntervalOnSeconds { get; set; }
        public int BatchSize { get; set; }
    }
}
