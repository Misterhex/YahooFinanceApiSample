using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using YahooFinanceApi;

namespace YahooFinanceApi
{
    public class YahooFinance
    {
        private const string _historicalPricesFormat =
            @"http://ichart.finance.yahoo.com/table.csv?s={0}&d={1}&e={2}&f={3}&g=d&a={4}&b={5}&c={6}&ignore=.csv";

        public IEnumerable<HistoricalPriceData> GetHistoricalPrices(string stockQuote, DateTime from, DateTime to)
        {
            string requestUrl =
                string.Format(_historicalPricesFormat, stockQuote,
                to.Month - 1, to.Day.ToString().PadLeft(2, '0'), to.Year,
                from.Month - 1, from.Day.ToString().PadLeft(2, '0'), from.Year);

            //WebRequest webRequest = HttpWebRequest.CreateHttp(requestUrl);
            //var webResponse = webRequest.GetResponseAsync().Result;

            HttpClient httpClient = new HttpClient();
            var stream = httpClient.GetStreamAsync(requestUrl).Result;

            using (var csvReader = new CsvReader(new StreamReader(stream)))
            {
                while (csvReader.Read())
                {
                    //Date,Open,High,Low,Close,Volume,Adj Close
                    DateTime date = csvReader.GetField<DateTime>("Date");
                    decimal open = csvReader.GetField<decimal>("Open");
                    decimal high = csvReader.GetField<decimal>("High");
                    decimal low = csvReader.GetField<decimal>("Low");
                    decimal close = csvReader.GetField<decimal>("Close");
                    int volume = csvReader.GetField<int>("Volume");
                    decimal adjClose = csvReader.GetField<decimal>("Adj Close");

                    yield return new HistoricalPriceData()
                    {
                        Date = date,
                        Open = open,
                        High = high,
                        Low = low,
                        Close = close,
                        Volume = volume,
                        Adj_Close = adjClose
                    };
                }
            }
        }
    }
}
