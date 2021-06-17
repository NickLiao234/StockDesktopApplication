using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Models
{
    /// <summary>
    /// 個股統計資料
    /// </summary>
    public class StatisticsData
    {
        /// <summary>
        /// 各股代號
        /// </summary>
        public string StockID { get; set; }

        /// <summary>
        /// 各股名稱
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 買進數量
        /// </summary>
        public int? BuyTotal { get; set; } = 0;

        /// <summary>
        /// 賣出數量
        /// </summary>
        public int? CellTotal { get; set; } = 0;

        /// <summary>
        /// 加權平均價格
        /// </summary>
        public double? AvgPrice { get; set; } = 0;

        /// <summary>
        /// 買賣超數量
        /// </summary>
        public int? BuyCellOver { get; set; } = 0;

        /// <summary>
        /// 券商數量
        /// </summary>
        public int? SecBrokerCnt { get; set; } = 0;
    }
}
