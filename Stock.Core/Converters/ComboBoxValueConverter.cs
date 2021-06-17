using Stock.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Stock.Core.Converters
{
    /// <summary>
    /// ComboBox資料轉換器
    /// </summary>
    public class ComboBoxValueConverter : IValueConverter
    {
        /// <summary>
        /// 轉換方法
        /// </summary>
        /// <param name="value">欲轉換物件</param>
        /// <param name="targetType">轉換物件型別</param>
        /// <param name="parameter">參數</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return value;
            }

            var datas = (ObservableCollection<StockInfoModel>)value;

            foreach (var data in datas)
            {
                data.StockName = $"{data.StockID} - {data.StockName}";
            }
            return datas;
        }

        /// <summary>
        /// 回復轉換方法
        /// </summary>
        /// <param name="value">欲轉換物件</param>
        /// <param name="targetType">轉換物件型別</param>
        /// <param name="parameter">參數</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
