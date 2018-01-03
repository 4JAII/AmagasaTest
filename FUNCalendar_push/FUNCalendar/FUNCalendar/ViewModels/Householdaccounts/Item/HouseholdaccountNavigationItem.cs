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

        public HouseholdaccountNavigationItem(VMHouseholdaccountScStatisticsItem item, DateTime date,Range range)
        {
            this.SCItem = item;
            this.CurrentDate = date;
            this.CurrentRange = range;
        }
    }
}
