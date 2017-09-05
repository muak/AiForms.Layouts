using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Sample.Resources;
using Xamarin.Forms;
using System.Net.Http;
using Prism.Services;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
    public class WrapLayoutWithSelectorViewModel:BindableBase, INavigationAware
    {
        private IPageDialogService _pageDialog;
        private INavigationService _navi;
        public WrapLayoutWithSelectorViewModel(INavigationService navigationService, IPageDialogService pageDlg)
        {
            _navi = navigationService;
            _pageDialog = pageDlg;
            IsSquare = false;
            UniformColumns = 0;
            Title = "WrapLayout(Variable)";

            BoxList = new ObservableCollection<Hoge>(Shuffle());
        }

        List<Hoge> Shuffle()
        {
            var list = new List<Hoge>();

            var rand = new Random();
            for (var i = 0; i < 8; i++) {

                var r = rand.Next(10, 245);
                var g = rand.Next(10, 245);
                var b = rand.Next(10, 245);
                var color = Color.FromRgb(r, g, b);
                var w = rand.Next(30, 100);
                var h = rand.Next(30, 60);

                list.Add(new Hoge {
                    Name = $"#{r:X2}{g:X2}{b:X2}",
                    Color = color,
                    Width = w,
                    Height = h,
                });
            }

            return list;
        }

        Hoge GetNextItem()
        {
            var rand = new Random();
            var r = rand.Next(10, 245);
            var g = rand.Next(10, 245);
            var b = rand.Next(10, 245);
            var color = Color.FromRgb(r, g, b);
            var w = rand.Next(30, 100);
            var h = rand.Next(30, 60);

            return new Hoge {
                Name = $"#{r:X2}{g:X2}{b:X2}",
                Color = color,
                Width = w,
                Height = h,
            };
        }

        private string _Title;
        public string Title {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }

        private int _UniformColumns;
        public int UniformColumns {
            get { return _UniformColumns; }
            set { SetProperty(ref _UniformColumns, value); }
        }

        private bool _IsSquare;
        public bool IsSquare {
            get { return _IsSquare; }
            set { SetProperty(ref _IsSquare, value); }
        }


        public ObservableCollection<Hoge> BoxList { get; set; }

        private DelegateCommand _AddCommand;
        public DelegateCommand AddCommand {
            get {
                return _AddCommand = _AddCommand ?? new DelegateCommand(() => {
                    BoxList.Add(GetNextItem());
                });
            }
        }

        private DelegateCommand _ToggleUniCommand;
        public DelegateCommand ToggleUniCommand {
            get {
                return _ToggleUniCommand = _ToggleUniCommand ?? new DelegateCommand(() => {
                    if (UniformColumns == 0) {
                        UniformColumns = 3;
                        Title = "WrapLayout(Uniform)";
                    }
                    else {
                        UniformColumns = 0;
                        Title = "WrapLayout(Variable)";
                    }


                });
            }
        }

        private DelegateCommand _ToggleSquareCommand;
        public DelegateCommand ToggleSquareCommand {
            get {
                return _ToggleSquareCommand = _ToggleSquareCommand ?? new DelegateCommand(() => {
                    IsSquare = !IsSquare;
                    if (IsSquare) {
                        Title += "(Square)";
                    }

                });
            }
        }

        private DelegateCommand _CheckCommand;
        public DelegateCommand CheckCommand {
            get {
                return _CheckCommand = _CheckCommand ?? new DelegateCommand(() => {
                    var chk = BoxList;
                    var bak = BoxList.ToList();

                    BoxList.Clear();

                    //BoxList = new ObservableCollection<Hoge>(bak);
                    //OnPropertyChanged(() => BoxList);
                    foreach (var hoge in bak) {
                        BoxList.Add(hoge);
                    }


                });
            }
        }

        private DelegateCommand _ShuffleCommand;
        public DelegateCommand ShuffleCommand {
            get {
                return _ShuffleCommand = _ShuffleCommand ?? new DelegateCommand(() => {

                    BoxList.Clear();
                    var list = Shuffle();
                    BoxList = new ObservableCollection<Hoge>(list);
                    OnPropertyChanged(() => BoxList);
                    //foreach (var hoge in list) {
                    //    BoxList.Add(hoge);
                    //}

                });
            }
        }

        private DelegateCommand _ClearCommand;
        public DelegateCommand ClearCommand {
            get {
                return _ClearCommand = _ClearCommand ?? new DelegateCommand(() => {
                    BoxList.Clear();
                });
            }
        }


        private DelegateCommand _DeleteCommand;
        public DelegateCommand DeleteCommand {
            get {
                return _DeleteCommand = _DeleteCommand ?? new DelegateCommand(() => {
                    BoxList.Remove(BoxList.Last());
                });
            }
        }

        private DelegateCommand _ReplaceCommand;
        public DelegateCommand ReplaceCommand {
            get {
                return _ReplaceCommand = _ReplaceCommand ?? new DelegateCommand(() => {
                    BoxList[0] = GetNextItem();
                });
            }
        }

        private DelegateCommand<object> _TapCommand;
        public DelegateCommand<object> TapCommand {
            get {
                return _TapCommand = _TapCommand ?? new DelegateCommand<object>((x) => {
                    var item = x as Hoge;
                    _pageDialog.DisplayAlertAsync("", item.Name, "OK");
                });
            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }


    }

}
