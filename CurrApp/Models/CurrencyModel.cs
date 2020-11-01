using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurrApp.Models
{
    public class CurrencyModel
    {
        public string CurrName { get; set; }
        public double CurrRate { get; set; }

        public CurrencyModel ()
        {

        }

        public CurrencyModel ( string name, double rate)
        {
            CurrName = name;
            CurrRate = rate;
        }
    }
}