﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using webszolg2.Entities;
using webszolg2.MnbServiceReference;

namespace webszolg2
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        public Form1()
        {
            InitializeComponent();
            GetWebservice();
            dataGridView1.DataSource = Rates.ToList();
            ProcessXml(GetWebservice());
        }

        private string GetWebservice()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();
            var request = new GetExchangeRatesRequestBody()
            {
                currencyNames = "EUR",
                startDate = "2020-01-01",
                endDate = "2020-06-30",
            };

            var response = mnbService.GetExchangeRates(request);
            var result = response.GetExchangeRatesResult;

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
                rate.Currency = childElement.GetAttribute("curr");

                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0)
                {
                    rate.Value = value / unit;
                }
            }
        }
    }
}
