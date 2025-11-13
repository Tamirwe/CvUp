using System;
using System.Collections.Generic;

namespace Database.models
{
    public partial class futures_ohlc
    {
        public int id { get; set; }
        public DateTime statistic_date { get; set; }
        public int open { get; set; }
        public int high { get; set; }
        public int low { get; set; }
        public int close { get; set; }
        public int day_points { get; set; }
    }
}
