using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VeevaDelete
{
    public class LimitArgs : EventArgs
    {
        string burstLimit {get; set;}
        string dailyLimit { get; set;}

        public LimitArgs(string burstLimit, string dailyLimit)
        {
            // TODO: Complete member initialization
            this.burstLimit = burstLimit;
            this.dailyLimit = dailyLimit;
        }

        public string GetBurstLimit()
        {
            return burstLimit;
        }

        public string GetDailyLimit()
        {
            return dailyLimit;
        }
    }
}
