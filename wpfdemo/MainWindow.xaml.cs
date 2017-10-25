using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Data;
using Newtonsoft.Json.Linq;


namespace wpfdemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetData();
            getTabItem2();
        }
        public MainWindow(string sd)
        {
            InitializeComponent();
            //label1.Content = sd.ToString();
        }
        public void GetData()
        {
            WebClient webclient = new WebClient();
            string uri = "http://www.test2.com/phpquery/demo3.php";
            //webclient.DownloadData
            try
            {
                Stream st = webclient.OpenRead(uri);
                StreamReader sr = new StreamReader(st);
                string res = sr.ReadToEnd();
                sr.Close();
                st.Close();

                DataTable dt = new DataTable();
                dt.Columns.Add("title");
                dt.Columns.Add("date");
                dt.Columns.Add("url");
                JArray jay = JArray.Parse(res);
                for (int i = 0; i < jay.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["title"] = jay[i]["title"].ToString();
                    dr["date"] = jay[i]["date"].ToString();
                    dr["url"] = jay[i]["url"].ToString();
                    dt.Rows.Add(dr);
                }
                dataGrid1.ItemsSource = dt.DefaultView;

            }
            catch (Exception ex)
            {

            }


        }

        private DateTime StampToDateTime(string timeStamp)
        {
            DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }


        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string url = btn.CommandParameter.ToString();
            //获取到url参数 传递到本地获取内容页内容
            WebClient webclient = new WebClient();
            string uri = "http://www.test2.com/phpquery/demo4.php?url=" + url;

            Stream st = webclient.OpenRead(uri);
            StreamReader sr = new StreamReader(st, Encoding.Default);
            string res = sr.ReadToEnd();
            sr.Close();
            st.Close();

            MessageBox.Show(res);


            
        }

        private void getTabItem2()
        {
            string sql = "select * from hd_user";

            DataTable dt = SQLHelper.ExecuteDataTable(sql);

            dataGrid2.ItemsSource = dt.DefaultView;
        }



    }
}
