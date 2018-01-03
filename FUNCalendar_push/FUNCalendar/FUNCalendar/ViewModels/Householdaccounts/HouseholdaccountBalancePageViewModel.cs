using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using FUNCalendar.Models;
using System.Reactive.Disposables;
using System.Reactive.Linq;



namespace FUNCalendar.ViewModels
{
    public class HouseholdaccountBalancePageViewModel : BindableBase,IDisposable
    {
        private IHouseHoldAccounts _householdaccounts;

        public ReadOnlyReactiveCollection<VMHouseholdaccountBalanceItem> DisplayBalances { get; private set; }
        public ReactiveProperty<int> DisplayTotalBalance { get; private set; }

        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        public HouseholdaccountBalancePageViewModel(IHouseHoldAccounts ihouseholdaccounts)
        {
            _householdaccounts = ihouseholdaccounts;
            this.DisplayBalances = _householdaccounts.Balances.ToReadOnlyReactiveCollection(x => new VMHouseholdaccountBalanceItem(x)).AddTo(disposable);
            this.DisplayTotalBalance = _householdaccounts.ObserveProperty(h => h.TotalBalance).ToReactiveProperty().AddTo(disposable);

            _householdaccounts.SetBalance();
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
