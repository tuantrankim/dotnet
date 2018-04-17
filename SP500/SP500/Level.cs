﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP500
{
    public class Trade
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public DateTime Date { get; set; }
        public Double Qty { get; set; }
        public Double Price { get; set; }
        public Double Total { get; set; }
        public bool IsBuy { get; set; }
    }
}
