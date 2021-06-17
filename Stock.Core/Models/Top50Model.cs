using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Models
{
    /// <summary>
    /// 買賣超前50資料
    /// </summary>
    public class Top50Model
    {
        /// <summary>
        /// 股票名稱
        /// </summary>
        public string StockName { get; set; }

        /// <summary>
        /// 券商名稱
        /// </summary>
        public string SecBrokerName { get; set; }

        /// <summary>
        /// 買賣超數量
        /// </summary>
        public int? BuyCellOver { get; set; }
    }
}
