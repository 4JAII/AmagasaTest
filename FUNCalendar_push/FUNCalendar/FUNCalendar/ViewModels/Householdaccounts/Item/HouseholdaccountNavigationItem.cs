using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNCalendar.Models;


namespace FUNCalendar.ViewModels
{
    public class HouseholdaccountNavigationItem
    {
        public VMHouseholdaccountScStatisticsItem SCItem { get; set; }
        public DateTime CurrentDate { get; set; }
        public Range CurrentRange { get; set; }

        public HouseholdaccountNavigationItem(VMHouseholdaccountScStatisticsItem scitem, DateTime date, Range range)
        {
            this.SCItem = scitem;
            this.CurrentDate = date;
            this.CurrentRange = range;
        }

        public HouseholdaccountNavigationItem(DateTime date, Range range)
        {
            this.SCItem = null;
            this.CurrentRange = range;
            this.CurrentDate = date;
        }

    }
}
