using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using webszolg2.Entities;
using webszolg2.MnbServiceReference;

namespace webszolg2
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        BindingList<string> Currencies = new BindingList<string>();
        public Form1()
        {
            InitializeComponent();
            GetCurrencies();
            ListCurrencies(GetCurrencies());
            comboBox1.DataSource = Currencies;
            RefreshData();           
        }

        private void RefreshData()
        {
            Rates.Clear();
            GetWebservice();
            
            ProcessXml(GetWebservice());
            dataGridView1.DataSource = Rates;
            ShowData();
        }

        private string GetWebservice()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();
            var request = new GetExchangeRatesRequestBody()
            {
                currencyNames = (string)comboBox1.SelectedItem,
                startDate = dateTimePicker1.Value.ToString(),
                endDate = dateTimePicker2.Value.ToString()
            };

            var response = mnbService.GetExchangeRates(request);
            var result = response.GetExchangeRatesResult;
            Console.WriteLine(result);
            return result;
        }
        
        private void ProcessXml(string xmlString)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlString);

            foreach (XmlElement element in xml.DocumentElement)
            {
                var rate = new RateData();
                Rates.Add(rate);

                rate.Date = DateTime.Parse(element.GetAttribute("date"));

                var childElement = (XmlElement)element.ChildNodes[0];
                if (childElement == null) continue;
                rate.Currency = childElement.GetAttribute("curr");

                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0)
                {
                    rate.Value = value / unit;
                }
            }
        }

        private void ShowData()
        {
            chartRateData.DataSource = Rates;

            var series = chartRateData.Series[0];
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date";
            series.YValueMembers = "Value";
            series.BorderWidth = 2;

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsStartedFromZero = false;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private string GetCurrencies()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();

            var request = new GetCurrenciesRequestBody();
            var response = mnbService.GetCurrencies(request);
            var result = response.GetCurrenciesResult;
            Console.WriteLine(result);
            return result;
        }

        private void ListCurrencies(string xmlString)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlString);
            foreach (XmlElement element in xml.DocumentElement)
            {
                var currency = new RateData();
                Currencies.Add(currency.ToString());

                var childElement = (XmlElement)element.ChildNodes[0];
                currency.Currency = childElement.InnerText;
            }
        }
    }
}
