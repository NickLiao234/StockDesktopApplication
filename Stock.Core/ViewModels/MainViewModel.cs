using Stock.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Library.Extentions;
using System.Collections.ObjectModel;
using System.Collections;

namespace Stock.Core.ViewModels
{
    /// <summary>
    /// 主畫面 ViewModel
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// stockID與HashTableID對應表
        /// </summary>
        private Dictionary<string, List<int>> StockIDMapTable;

        /// <summary>
        /// 原始個股資料
        /// </summary>
        public Hashtable Datas { get; set; }

        /// <summary>
        /// 篩選後個股資料
        /// </summary>
        public List<StockInfoModel> FilterDatas { get; set; }

        /// <summary>
        /// 統計資料
        /// </summary>
        public List<StatisticsData> StatisticsDatas { get; set; }

        /// <summary>
        /// 買賣超前50名資料
        /// </summary>
        public List<Top50Model> Top50Datas { get; set; }

        /// <summary>
        /// 下拉選單Items
        /// </summary>
        public Dictionary<string, StockInfoModel> ComboBoxItems { get; set; }

        /// <summary>
        /// 初始化原始資料屬性
        /// </summary>
        public MainViewModel()
        {
            Datas = new Hashtable();
        }

        /// <summary>
        /// 讀取指定路徑的csv檔，並將內容轉為集合資料，並將結果指派給Datas屬性
        /// </summary>
        /// <param name="filePath">csv檔案路徑</param>
        /// <returns></returns>
        public async Task RetriveDatasAsync(string filePath)
        {
            var result = new Hashtable();
            var mapTable = new Dictionary<string, List<int>>();

            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {
                var index = 0;
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();

                    if (line.Contains("日期"))
                    {
                        continue;
                    }

                    var values = line.Split(',');
                    var dealDate = values[0];
                    var stockID = values[1];
                    var stockName = values[2];
                    var secBrokerID = values[3];
                    var secBrokerName = values[4];
                    var price = Convert.ToDouble(values[5]);
                    var buyQty = Convert.ToInt32(values[6]);
                    var cellQty = Convert.ToInt32(values[7]);

                    result.Add(index, new StockInfoModel()
                    {
                        DealDate = dealDate,
                        StockID = stockID,
                        StockName = stockName,
                        SecBrokerID = secBrokerID,
                        SecBrokerName = secBrokerName,
                        Price = Convert.ToDouble(price),
                        BuyQty = Convert.ToInt32(buyQty),
                        CellQty = Convert.ToInt32(cellQty)
                    });

                    if (mapTable.ContainsKey(stockID))
                    {
                        mapTable[stockID].Add(index);  
                    }
                    else
                    {
                        var list = new List<int>();
                        list.Add(index);
                        mapTable.Add(stockID, list);
                    }

                    index += 1;
                }             
            }

            Datas = result;
            StockIDMapTable = mapTable;

            var comboBoxDatas = new Dictionary<string, StockInfoModel>();
            comboBoxDatas.Add(
                "All",
                new StockInfoModel()
                {
                    StockID = "All"
                });
            
            foreach (DictionaryEntry data in Datas)
            {
                comboBoxDatas[((StockInfoModel)data.Value).StockID] = (StockInfoModel)data.Value;
            }
            ComboBoxItems = comboBoxDatas;
        }

        /// <summary>
        /// 取得依股票代號篩選出來的個股資訊，並將結果指派給FilterDatas屬性
        /// </summary>
        /// <param name="StockIDList">股票代號集合</param>
        public void GetFilterData(IEnumerable<string> StockIDList)
        {
            if (Datas is null) 
            {
                return;
            }

            var resultOfFilterDatas = new List<StockInfoModel>();
            var resultOfStatisticsDatas = new List<StatisticsData>();

            if (StockIDList.Contains("All"))
            {
                var strList = ComboBoxItems.Select(d => d.Value.StockID).ToList();
                strList.RemoveAt(0);
                StockIDList = strList;
            }

            foreach (var stockID in StockIDList)
            {
                var mapIDs = StockIDMapTable[stockID];
                var eachStockDatas = new List<StockInfoModel>();

                foreach (var hashTableID in mapIDs)
                {
                    eachStockDatas.Add((StockInfoModel)Datas[hashTableID]);
                }

                resultOfFilterDatas.AddRange(eachStockDatas);

                var statisticsData = new StatisticsData();
                statisticsData.StockID = stockID;
                statisticsData.StockName = eachStockDatas.First().StockName;
                statisticsData.BuyTotal = eachStockDatas.Sum(d => d.BuyQty);
                statisticsData.CellTotal = eachStockDatas.Sum(d => d.CellQty);
                statisticsData.BuyCellOver = statisticsData.BuyTotal - statisticsData.CellTotal;
                statisticsData.SecBrokerCnt = eachStockDatas.Distinct(d => d.SecBrokerID).Count();
                var sumPrice = eachStockDatas.Sum(d => d.Price * (d.BuyQty + d.CellQty));
                statisticsData.AvgPrice = sumPrice / (statisticsData.BuyTotal + statisticsData.CellTotal);

                resultOfStatisticsDatas.Add(statisticsData);
            }
            FilterDatas = resultOfFilterDatas.OrderBy(d => d.StockID).ToList();
            StatisticsDatas = resultOfStatisticsDatas;           
        }

        /// <summary>
        /// 取得買賣超前50資料，並將結果指派給Top50Datas屬性
        /// </summary>
        /// <param name="StockIDList">股票代號集合</param>
        public void GetTop50Datas(IEnumerable<string> StockIDList)
        {
            if (Datas is null)
            {
                return;
            }

            var resultOfTop50Datas = new List<Top50Model>();

            if (StockIDList.Contains("All"))
            {
                var strList = ComboBoxItems.Select(d => d.Value.StockID).ToList();
                strList.RemoveAt(0);
                StockIDList = strList;
            }

            foreach (var stockID in StockIDList)
            {
                var mergedData = GetMergedData(stockID);

                var BuyQtyTop50Datas = mergedData
                    .Where(d => (d.BuyQty - d.CellQty) >= 0)
                    .Select(d => new Top50Model()
                    {
                        StockName = d.StockName,
                        SecBrokerName = d.SecBrokerName,
                        BuyCellOver = d.BuyQty - d.CellQty
                    })
                    .OrderByDescending(d => d.BuyCellOver)
                    .Take(50)
                    .ToList();

                resultOfTop50Datas.AddRange(BuyQtyTop50Datas);

                var CellQtyTop50Datas = mergedData
                    .Where(d => (d.BuyQty - d.CellQty) <= 0)
                    .Select(d => new Top50Model()
                    {
                        StockName = d.StockName,
                        SecBrokerName = d.SecBrokerName,
                        BuyCellOver = d.BuyQty - d.CellQty
                    })
                    .OrderBy(d => d.BuyCellOver)
                    .Take(50)
                    .ToList();

                resultOfTop50Datas.AddRange(CellQtyTop50Datas);
            }

            Top50Datas = resultOfTop50Datas;
        }

        /// <summary>
        /// 取得將個股原始資料合併同一家券商買賣資料
        /// </summary>
        /// <param name="stockID">股票代號</param>
        /// <returns></returns>
        private List<StockInfoModel> GetMergedData(string stockID)
        {
            var result = new List<StockInfoModel>();

            var datasByID = Datas.Values.Cast<StockInfoModel>().Where(d => d.StockID == stockID).ToList();
            var secBrokerIDs = datasByID.Distinct(d => d.SecBrokerID).Select(d => d.SecBrokerID).ToList();
            foreach (var secBrokerID in secBrokerIDs)
            {                
                var datasByBrokerID = datasByID.Where(d => d.SecBrokerID == secBrokerID).ToList();

                var stockInfo = new StockInfoModel()
                {
                    StockID = datasByBrokerID[0].StockID,
                    StockName = datasByBrokerID[0].StockName,
                    DealDate = datasByBrokerID[0].DealDate,
                    SecBrokerID = secBrokerID,
                    SecBrokerName = datasByBrokerID[0].SecBrokerName,
                    Price = datasByBrokerID[0].Price,
                    BuyQty = datasByBrokerID.Sum(d => d.BuyQty),
                    CellQty = datasByBrokerID.Sum(d => d.CellQty)
                };

                result.Add(stockInfo);
            }

            return result;
        }

        /// <summary>
        /// 實作INotifyPropertyChanged必要方法
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;       
    }
}
