using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP500
{
    public class Preset
    {
        public static bool IsCalibrating { get; set; }
        public static DateTime FromDate { get; set; }
        public static DateTime ToDate { get; set; }
        public static double LastTradePrice { get; set; }
        public static double Up { get; set; }
        public static double Down { get; set; }
        public static double CashAmount { get; set; }
        public static double SharesAmount { get; set; }
        public static double Top { get; set; }
        public static double Bottom { get; set; }
        public static double CashMonthlyAdd { get; set; }
        public static double SharesMonthlyAdd { get; set; }
        public Preset()
        {
            IsCalibrating = false;
            Top = 0;
            Bottom = double.PositiveInfinity;
            CashMonthlyAdd = 0;
            SharesMonthlyAdd = 0;
        }
    }
}
