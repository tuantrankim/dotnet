using System;
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
        public DateTime Date {
            get { return CurrentIndex.Date; }
        }
        public Double Qty { get; set; }
       
        public Double Price {
            get { return CurrentIndex.Close; }
        }
       
        Double previousPrice = 0;
        public Double PreviousPrice
        {
            get
            {
                if (previousPrice == 0) previousPrice = Price;
                return previousPrice;
            }
            set { previousPrice = value; }
        }
        Double top = 0;
        public Double Top
        {
            get
            {
                if (top == 0) top = Price;
                return top;
            }
            set { top = value; }
        }
        Double bottom = 0;
        public Double Bottom
        {
            get
            {
                if (bottom == 0) bottom = Price;
                return bottom;
            }
            set { bottom = value; }
        }
        public Double Total {
            get
            {
                return Price * Qty;
            }
        }
        public bool IsBuy { get; set; }
        public Index CurrentIndex { get; set; }

        public double PriceChange
        {
            get { return PreviousPrice - Price; }
        }
        public double PriceChangePercentage
        {
            get { return PriceChange * 100 / PreviousPrice; }
        }
        public double Up
        {
            get{
                return PriceChange > 0 ? PriceChange : 0;
            }
        }
        public double UpPecentage
        {
            get
            {
                return Up * 100 / PreviousPrice;
            }
        }

        public double Down
        {
            get
            {
                return PriceChange < 0 ? PriceChange : 0;
            }
        }

        public double DownPercentage
        {
            get
            {
                return Down *100 / PreviousPrice;
            }
        }
        public String Action
        {
            get
            {
                return IsBuy ? "Buy" : "Sell";
            }
        }
    }
}
