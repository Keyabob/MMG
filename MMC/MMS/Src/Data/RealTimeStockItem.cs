using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Src.Data
{
    public class RealTimeStockItem
    {
        public RealTimeStockItem()
        {
            this.BuyCounts = new long[5];
            this.SaleCounts = new long[5];
            this.BuyPrices = new float[5];
            this.SalePrices = new float[5];
        }

        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 代码类型
        /// </summary>
        public string CodeType { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
        public string StockName { get; set; }
        
        /// <summary>
        /// 行情时间
        /// </summary>
        public DateTime CurrentTimestamp { get; set; }
        
        /// <summary>
        /// 最新价
        /// </summary>
        public float CurrentPrice { get; set; }
        
        /// <summary>
        /// 昨日收盘价
        /// </summary>
        public float YesterdayClose { get; set; }

        /// <summary>
        /// 今开盘价
        /// </summary>
        public float TodayOpen { get; set; }

        /// <summary>
        /// 最低价
        /// </summary>
        public float TodayLow { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        public float TodayHigh { get; set; }

        /// <summary>
        /// 成交量(手)
        /// </summary>
        public long Volume { get; set; }

        /// <summary>
        /// 成交金额
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// 买盘成交量
        /// </summary>
        public long[] BuyCounts { get; private set; }

        /// <summary>
        /// 买盘价格
        /// </summary>
        public float[] BuyPrices { get; private set; }

        /// <summary>
        /// 卖盘成交量
        /// </summary>
        public long[] SaleCounts { get; private set; }

        /// <summary>
        /// 卖盘价格
        /// </summary>
        public float[] SalePrices { get; private set; }
    }
}
