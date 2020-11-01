using CurrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrApp.ViewModels
{
    public class HomeControllerViewModel
    {
        public List<CurrencyModel> AllCurrencies { get; set; }
        public string SearchParameters { get; set; }
        public HomeControllerViewModel()
        {
            AllCurrencies = new List<CurrencyModel>();
        }

        public HomeControllerViewModel(CurrencyModel EmptyList)
        {
            AllCurrencies = new List<CurrencyModel>();
            AllCurrencies.Add(EmptyList);
        }

    }
}