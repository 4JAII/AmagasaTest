using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Xamarin.Forms;
using OxyPlot.Series;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using FUNCalendar.Models;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using Prism.Navigation;
using Prism.Services;

namespace FUNCalendar.ViewModels
{
    public class HouseholdaccountsHistoryPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        private IHouseHoldAccounts _householdaccounts;
        private INavigationService _navigationservice;

        public static readonly string InputKey = "InputKey";

        /* 履歴 */
        public ReadOnlyReactiveCollection<VMHouseHoldAccountsItem> DisplayHistoryCollection { get; private set; }

        public HouseholdaccountNavigationItem NavigatedItem { get; set; }
        private BalanceTypes CurrentBalanceType { get; set; }
        private SCategorys _currentSCategory;
        public SCategorys CurrentSCategory
        {
            get { return this._currentSCategory; }
            set { this.SetProperty(ref this._currentSCategory, value); }
        }
        private DCategorys _currentDCategory;
        public DCategorys CurrentDCategory
        {
            get { return this._currentDCategory; }
            set { this.SetProperty(ref this._currentDCategory, value); }
        }

        public HouseholdaccountRangeItem[] RangeNames { get; private set; }

        public ReactiveProperty<DateTime> SelectedDate { get; private set; }
        public ReactiveProperty<HouseholdaccountRangeItem> SelectedRange { get; private set; }

        public ReactiveCommand ResistCommand { get; private set; }

        private CompositeDisposable disposable { get; } = new CompositeDisposable();

        /* 統計画面移行用 */
        public ReactiveCommand StatisticsCommand { get; private set; }

        /* 残高画面移行用 */
        public ReactiveCommand BalanceCommand { get; private set; }

        /* 編集用 */
        public ReactiveCommand EditCommand { get; private set; } = new ReactiveCommand();


        public HouseholdaccountsHistoryPageViewModel(IHouseHoldAccounts householdaccounts, INavigationService navigationService)
        {
            this._householdaccounts = householdaccounts;
            this._navigationservice = navigationService;
            this.ResistCommand = new ReactiveCommand();

            DisplayHistoryCollection = _householdaccounts.DisplayHouseholdaccountList.ToReadOnlyReactiveCollection(x => new VMHouseHoldAccountsItem(x)).AddTo(disposable);

            /* インスタンス化 */
            SelectedRange = new ReactiveProperty<HouseholdaccountRangeItem>();
            SelectedDate = new ReactiveProperty<DateTime>();
            BalanceCommand = new ReactiveCommand();
            StatisticsCommand = new ReactiveCommand();

            /* ピッカー用のアイテムの作成 */
            RangeNames = new[]
            {
                new HouseholdaccountRangeItem
                {
                    RangeName = "統計:日単位",
                    RangeData = Range.Day
                },
                new HouseholdaccountRangeItem
                {
                    RangeName = "統計:月単位" ,
                    RangeData = Range.Month
                },
                new HouseholdaccountRangeItem
                {
                    RangeName = "統計:年単位",
                    RangeData = Range.Year
                }
            };

            /* アイテム追加ボタンが押された時の処理 */
            ResistCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdaccountNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdaccountsRegisterPageViewModel.InputKey, navigationitem }
                };
                await _navigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdaccountsRegisterPage", navigationparameter);
            }).AddTo(disposable);


            /* アイテムを編集するときの処理 */
            EditCommand.Subscribe(async (obj) =>
            {
                _householdaccounts.SetHouseholdaccountsItem(VMHouseHoldAccountsItem.ToHouseholdaccountsItem(obj as VMHouseHoldAccountsItem));
                var navigationitem = new HouseholdaccountNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdaccountsRegisterPageViewModel.EditKey, navigationitem }
                };
                await _navigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdaccountsRegisterPage", navigationparameter);
            });


            /* 統計ボタンが押されたときの処理 */
            StatisticsCommand.Subscribe(async _ =>
            {
                var navigationitem = new HouseholdaccountNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdaccountBalancePageViewModel.InputKey, navigationitem }
                };
                await _navigationservice.NavigateAsync("/RootPage/NavigationPage/HouseHoldAccountsStatisticsPage", navigationparameter);
            }).AddTo(disposable);


            /* 残高ボタンが押されたときの処理 */
            BalanceCommand.Subscribe(_ =>
            {
                var navigationitem = new HouseholdaccountNavigationItem(SelectedDate.Value, SelectedRange.Value.RangeData);
                var navigationparameter = new NavigationParameters()
                {
                    {HouseholdaccountBalancePageViewModel.InputKey, navigationitem }
                };
                _navigationservice.NavigateAsync("/RootPage/NavigationPage/HouseholdaccountBalancePage", navigationparameter);
            }).AddTo(disposable);

        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            this.Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(InputKey))
            {
                NavigatedItem = (HouseholdaccountNavigationItem)parameters[InputKey];

                this.SelectedDate.Value = NavigatedItem.CurrentDate;
                this.CurrentBalanceType = NavigatedItem.CurrentBalanceType;
                this.SelectedRange.Value = (NavigatedItem.CurrentRange == Range.Day) ? RangeNames[0] :
                    (NavigatedItem.CurrentRange == Range.Month) ? RangeNames[1] :
                    (NavigatedItem.CurrentRange == Range.Year) ? RangeNames[2] : null;
                this.CurrentSCategory = NavigatedItem.CurrentSCategory;
                this.CurrentDCategory = NavigatedItem.CurrentDCategory;
                _householdaccounts.SetAllHistory(SelectedRange.Value.RangeData, SelectedDate.Value);

                /* 日付が変更された時の処理 */
                SelectedDate.Subscribe(_ =>
                {
                    if (_ != null)
                    {
                        _householdaccounts.SetAllHistory(SelectedRange.Value.RangeData, SelectedDate.Value);
                    }
                })
                .AddTo(disposable);

                /* レンジが変更された時の処理 */
                SelectedRange.Subscribe(_ =>
                {
                    if (_ != null)
                    {
                        _householdaccounts.SetAllHistory(SelectedRange.Value.RangeData, SelectedDate.Value);
                    }
                })
                .AddTo(disposable);

            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        public void Dispose()
        {
            disposable.Dispose();
        }

    }
}
