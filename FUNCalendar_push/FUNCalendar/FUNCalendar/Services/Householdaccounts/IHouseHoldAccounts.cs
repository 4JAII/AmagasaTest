using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.ComponentModel;

namespace FUNCalendar.Models
{
    public interface IHouseHoldAccounts:INotifyPropertyChanged
    {
        string TotalIncome { get; }
        string TotalOutgoing { get; }
        string Difference { get; }
        int TotalBalance { get; }
        string SCategoryTotal { get; }
        ObservableCollection<HouseholdaccountScStatisticsItem> SIncomes { get; }
        ObservableCollection<HouseholdaccountScStatisticsItem> SOutgoings { get; }
        ObservableCollection<HouseholdaccountPieSliceItem> PieSlice { get; }
        ObservableCollection<HouseholdaccountBalanceItem> Balances { get; }
        ObservableCollection<HouseholdaccountLegendItem> Legend { get; }
        ObservableCollection<HouseholdaccountDcStatisticsItem> ScategoryItems { get; }

        void SetAllStatics(Range r, DateTime date);
        void SetAllStaticsPie(Range r, BalanceTypes b, DateTime date);
        void SetSCategoryStatics(Range r, BalanceTypes b, DateTime date, SCategorys sc);
        void SetSCategoryStatisticsPie(Range r, DateTime date, SCategorys sc);
        void AddHouseHoldAccountsItem(string name, int count, int price, DateTime date, DCategorys detailcategory, SCategorys summarycategory, StorageTypes storagetype, bool isoutgoings);
        void SetBalance();
        void IncrementBalancePrice(StorageTypes st, int price);
        void EditHouseholdaccountBalance(StorageTypes st, int price);
    }
}
