using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using Stock.Core.Models;
using Stock.Core.ViewModels;
using Stock.Library.Enums;

namespace Stock
{    
    /// <summary>
    /// WPF template
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 效能監控物件
        /// </summary>
        private Stopwatch sw = new Stopwatch();

        /// <summary>
        /// initial component
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 讀取檔案按鈕事件
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            long total = 0;

            var openFileDialog = new OpenFileDialog()
            {
                Filter = "CSV documents (.csv)|*.txt|All files (*.*)|*.*"
            };

            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                sw.Reset();
                sw.Start();
                PathText.Text = openFileDialog.FileName;
                ChangeStatus(MainPageStatus.ReadingFile);

                var vm = (MainViewModel)this.DataContext;

                ThreadPool.QueueUserWorkItem(o =>
                {
                    Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await vm.RetriveDatasAsync(openFileDialog.FileName);
                    }));
                });

                sw.Stop();
                total += sw.ElapsedMilliseconds;
                var time = TimeSpan.FromMilliseconds(total);

                var readFileTime = time.ToString(@"hh\:mm\:ss\.fff");

                MessagesText.Text = $"ReadFile Time = {readFileTime} \n Generate ComboBox Time = ";

                ChangeStatus(MainPageStatus.ReadFileSuccess);
            }
            else
            {
                ChangeStatus(MainPageStatus.ReadFileFail);
            }
        }

        /// <summary>
        /// 變更主頁狀態
        /// </summary>
        /// <param name="status">主頁狀態enum</param>
        private void ChangeStatus(MainPageStatus status)
        {
            switch (status)
            {
                case MainPageStatus.ReadingFile:
                    ReadFileStatusText.Text = "讀檔中";
                    ReadFileButton.IsEnabled = false;
                    break;
                case MainPageStatus.ReadFileSuccess:
                    ReadFileButton.IsEnabled = true;
                    ReadFileStatusText.Text = "讀檔完成";
                    PathText.Text = "";
                    break;
                case MainPageStatus.ReadFileFail:
                    PathText.Text = "";
                    ReadFileStatusText.Text = "讀檔失敗";
                    ReadFileButton.IsEnabled = true;
                    break;
                case MainPageStatus.QueryStock:
                    break;
                case MainPageStatus.QuerySuccess:
                    break;
                case MainPageStatus.QueryFail:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 查詢個股資訊按鈕事件
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void QueryStockButton_Click(object sender, RoutedEventArgs e)
        {
            long total = 0;
            sw.Reset();
            sw.Start();
            var it = (StockInfoModel)StocksComboBox.SelectedItem;

            ((MainViewModel)this.DataContext).GetFilterData(new List<string>() { it.StockID });

            sw.Stop();
            total += sw.ElapsedMilliseconds;
            var time = TimeSpan.FromMilliseconds(total);

            var queryTime = time.ToString(@"hh\:mm\:ss\.fff");

            MessagesText.Text = $"查詢股票資訊共花費 {queryTime}";
        }

        /// <summary>
        /// 前50買賣超查詢按鈕事件
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void Top50Button_Click(object sender, RoutedEventArgs e)
        {
            long total = 0;
            sw.Reset();
            sw.Start();

            var it = (StockInfoModel)StocksComboBox.SelectedItem;

            ((MainViewModel)this.DataContext).GetTop50Datas(new List<string>() { it.StockID });

            sw.Stop();
            total += sw.ElapsedMilliseconds;
            var time = TimeSpan.FromMilliseconds(total);

            var queryTime = time.ToString(@"hh\:mm\:ss\.fff");

            MessagesText.Text = $"查詢股票買賣超前50資訊共花費 {queryTime}";
        }
    }
}
