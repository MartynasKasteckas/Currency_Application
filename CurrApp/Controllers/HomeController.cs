using CurrApp.Models;
using CurrApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace CurrApp.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        public ActionResult Index()
        {
            return View(EmptyObject());
        }

        [HttpPost]
        public ActionResult Index(DateTime? InputedDate)
        {
            var DummyDate = new DateTime();
            var MaxDate = new DateTime(2014, 12, 31);
            var InputedDateforCheck = InputedDate ?? DummyDate;

            if (InputedDateforCheck != DummyDate && InputedDateforCheck <= MaxDate)
            {
                //Set Criteria
                var FromDate = InputedDateforCheck.AddDays(-1);
                var STRFromDate = FromDate.ToShortDateString();
                var STRToDate = InputedDateforCheck.ToShortDateString();

                //Set Data
                var FirstCurrSet = GetData(STRFromDate);
                var SecondCurrSet = GetData(STRToDate);

                //Compare Data and Create final List with Rate Difference
                var DifData = FinalList(FirstCurrSet, SecondCurrSet);

                //Add Citeria to Final Result
                DifData.SearchParameters = STRFromDate + " - " + STRToDate;

                return View(DifData);
            }
            else
            {
                return View(EmptyObject());
            }
        }

        //Creates Empty Model
        private HomeControllerViewModel EmptyObject()
        {
            HomeControllerViewModel AllDataEmpty = new HomeControllerViewModel(new CurrencyModel("", 0));

            return AllDataEmpty;
        }

        //Compare Data and Create final List with Rate Difference
        private HomeControllerViewModel FinalList(HomeControllerViewModel FromDateCurrencyDataList, HomeControllerViewModel ToDateCurrencyDataList)
        {
            HomeControllerViewModel FinalResult = new HomeControllerViewModel();
            foreach (CurrencyModel SingleFromDateCurrencyData in FromDateCurrencyDataList.AllCurrencies)
            {
                var s = ToDateCurrencyDataList.AllCurrencies.Where(SingleToDateCurrencyData => SingleToDateCurrencyData.CurrName == SingleFromDateCurrencyData.CurrName).Single();
                var DifName = SingleFromDateCurrencyData.CurrName;
                var DifRate = Math.Round(SingleFromDateCurrencyData.CurrRate - s.CurrRate, 4);
                FinalResult.AllCurrencies.Add(new CurrencyModel(DifName, DifRate));
            }

            return FinalResult;
        }

        //Recieving and Seting DATA
        private HomeControllerViewModel GetData(String InputedDate)
        {
            HomeControllerViewModel Alldata = new HomeControllerViewModel();
            RecieveDataAndFill(InputedDate, Alldata);

            return Alldata;
        }

        //Request
        private static void RecieveDataAndFill(string DateParameter, HomeControllerViewModel obj)
        {
            var url = "http://www.lb.lt/webservices/ExchangeRates/ExchangeRates.asmx/getExchangeRatesByDate?Date=" + DateParameter;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var xdoc = XDocument.Parse(response.Content.ReadAsStringAsync().Result);

                    var res = xdoc.Descendants("item");
                    foreach (var element in res)
                    {
                        var currency = element.Element("currency").Value;
                        var rate = element.Element("rate").Value.Replace(".", ",");
                        var doubleRate = Math.Round(Convert.ToDouble(rate), 4);

                        obj.AllCurrencies.Add(new CurrencyModel(currency, doubleRate));
                    }
                }
            }
        }
    }
}