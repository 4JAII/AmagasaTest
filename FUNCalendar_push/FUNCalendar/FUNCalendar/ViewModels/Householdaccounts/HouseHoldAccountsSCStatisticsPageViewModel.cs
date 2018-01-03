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
    public class HouseHoldAccountsSCStatisticsPageViewModel : BindableBase,INavigationAware,IDisposable
    {
        public ReactiveCommand BackPageCommand { get; private set; }

        private IHouseHoldAccounts _householdaccounts;
        private INavigationService _inavigationservice;

        public static readonly string InputKey = "InputKey";

        public ReadOnlyReactiveCollection<VMHouseholdaccountDcStatisticsItem> DisplayScategoryItems { get; private set; }
        public ReactiveProperty<string> DisplayScategoryTotal { get; private set; }

        public HouseholdaccountNavigationItem NavigationItem { get; set; }
        private DateTime CurrentDate { get; set; }
        private string CurrentBalanceType { get; set; }
        private Range CurrentRange { get; set; }
        private string CurrentSCategory { get; set; }

        private PlotModel _plotmodel { get; set; } = new PlotModel();
        public PlotModel DisplayPlotModel { get; private set; }
        public ReadOnlyReactiveCollection<HouseholdaccountPieSliceItem> Slices { get; private set; }
        public List<PieSlice> DisplaySlices { get; private set; }
        private PieSeries pieseries = new PieSeries()
        {
            StrokeThickness = 2.0,
            InsideLabelPosition = 0.5,
            AngleSpan = 360,
            StartAngle = 270,
            InnerDiameter = 0.8
        };
        public ReadOnlyReactiveCollection<VMHouseholdaccountLegendItem> DisplayLegend { get; set; }

        /* Picker用のアイテム */
        public HouseholdaccoutRangeItem[] RangeNames { get; private set; }

        public ReactiveProperty<DateTime> SelectedDate { get; private set; }    /* 日付を格納 */
        public ReactiveProperty<HouseholdaccoutRangeItem> SelectedRange { get; private set; }   /* 範囲を格納 */



        private CompositeDisposable disposable { get; } = new CompositeDisposable();


        public HouseHoldAccountsSCStatisticsPageViewModel(IHouseHoldAccounts ihouseholdaccounts, INavigationService navigationService)
        {
            this._householdaccounts = ihouseholdaccounts;
            this._inavigationservice = navigationService;
            this.BackPageCommand = new ReactiveCommand();
            this.DisplayScategoryItems = _householdaccounts.ScategoryItems.ToReadOnlyReactiveCollection(x => new VMHouseholdaccountDcStatisticsItem(x)).AddTo(disposable);
            this.DisplayScategoryTotal = _householdaccounts.ObserveProperty(h => h.SCategoryTotal).ToReactiveProperty().AddTo(disposable);

            /* ReactiveProperty化(グラフ) */
            this.Slices = _householdaccounts.PieSlice.ToReadOnlyReactiveCollection().AddTo(disposable);

            /* RectiveProperty化(グラフ凡例) */
            this.DisplayLegend = _householdaccounts.Legend.ToReadOnlyReactiveCollection(x => new VMHouseholdaccountLegendItem(x)).AddTo(disposable);


            _householdaccounts.SetSCategoryStatics(CurrentRange, (BalanceTypes)Enum.Parse(typeof(BalanceTypes), CurrentBalanceType), CurrentDate, (SCategorys)Enum.Parse(typeof(SCategorys), CurrentSCategory));
            _householdaccounts.SetSCategoryStatisticsPie(CurrentRange, CurrentDate, (SCategorys)Enum.Parse(typeof(SCategorys), CurrentSCategory));

            BackPageCommand.Subscribe(_ =>
            {
                _inavigationservice.NavigateAsync("/RootPage/NavigationPage/HouseHoldAccountsStatisticsPage");
            }).AddTo(disposable);
        }
        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(InputKey))
            {
                NavigationItem = (HouseholdaccountNavigationItem)parameters[InputKey];
                this.CurrentDate = NavigationItem.CurrentDate;
                this.CurrentBalanceType = NavigationItem.SCItem.Balancetype;
                this.CurrentRange = NavigationItem.CurrentRange;
                this.CurrentSCategory = NavigationItem.SCItem.Scategory;
                _householdaccounts.SetSCategoryStatics(CurrentRange, (BalanceTypes)Enum.Parse(typeof(BalanceTypes),CurrentBalanceType), CurrentDate, (SCategorys)Enum.Parse(typeof(SCategorys),CurrentSCategory));
                _householdaccounts.SetSCategoryStatisticsPie(CurrentRange, CurrentDate, (SCategorys)Enum.Parse(typeof(SCategorys), CurrentSCategory));
            }
        }

        /* 購読解除 */
        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
