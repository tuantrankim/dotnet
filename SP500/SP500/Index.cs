using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP500
{
    public class Index
    {
        public DateTime Date { get; set; }
        public double Volume { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double AdjClose { get; set; }

        public static Index FromHistoricalQuotesCsv(string csvLine)
        {
            string[] values = csvLine.Replace("\"", "").Split(',');
            Index dailyValues = new Index();
            dailyValues.Date = Convert.ToDateTime(values[0]);
            dailyValues.Close = Convert.ToDouble(values[1]);
            dailyValues.Volume = Convert.ToDouble(values[2]);
            dailyValues.Open = Convert.ToDouble(values[3]);
            dailyValues.High = Convert.ToDouble(values[4]);
            dailyValues.Low = Convert.ToDouble(values[5]);
            return dailyValues;
        }
        public static Index FromSPYCsv(string csvLine)
        {
            string[] values = csvLine.Replace("\"", "").Split(',');
            Index dailyValues = new Index();
            dailyValues.Date = Convert.ToDateTime(values[0]);
            dailyValues.Open = Convert.ToDouble(values[1]);
            dailyValues.High = Convert.ToDouble(values[2]);
            dailyValues.Low = Convert.ToDouble(values[3]);
            dailyValues.Close = Convert.ToDouble(values[4]);
            dailyValues.AdjClose = Convert.ToDouble(values[5]);
            dailyValues.Volume = Convert.ToDouble(values[6]);
            return dailyValues;
        }
    }
}
