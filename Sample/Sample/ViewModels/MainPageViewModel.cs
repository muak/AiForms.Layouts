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

namespace Sample.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {

        private IPageDialogService _pageDialog;
        private INavigationService _navi;
        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDlg)
        {
            _navi = navigationService;
            _pageDialog = pageDlg;
            Visible = false;

            BoxList = new ObservableCollection<Hoge>(new List<Hoge> {
                new Hoge{Name="ABC",Color=Color.Red},
                new Hoge{Name="Defsd",Color=Color.Blue},
                new Hoge{Name="あｓｄｆさ",Color=Color.Yellow},
                new Hoge{Name="ｔｊｊｇｆｆｄ",Color=Color.Green},
                new Hoge{Name="なむ",Color=Color.Lime},
                new Hoge{Name="XYZ",Color=Color.Pink},
                new Hoge{Name="DEF",Color=Color.Aqua},
                new Hoge{Name="KISD",Color=Color.Olive},
            });
        }

        private bool _Visible;
        public bool Visible {
            get { return _Visible; }
            set { SetProperty(ref _Visible, value); }
        }

        public ObservableCollection<Hoge> BoxList { get; set; }

        private DelegateCommand _VisibleChangeCommand;
        public DelegateCommand VisibleChangeCommand {
            get {
                return _VisibleChangeCommand = _VisibleChangeCommand ?? new DelegateCommand(() => {
                    Visible = !Visible;
                });
            }
        }

        private DelegateCommand _ClearCommand;
        public DelegateCommand ClearCommand {
            get { return _ClearCommand = _ClearCommand ?? new DelegateCommand(() => {
                BoxList.Clear();
            }); }
        }

        private DelegateCommand _ReloadCommand;
        public DelegateCommand ReloadCommand {
            get { return _ReloadCommand = _ReloadCommand ?? new DelegateCommand(() => {
                var list = new List<Hoge> {
                    new Hoge{Name="afsf",Color=Color.Orange},
                    new Hoge{Name="Desfsffsd",Color=Color.Blue},
                    new Hoge{Name="dsfs",Color=Color.Yellow},
                    new Hoge{Name="ｔｊｊｇｆｆｄ",Color=Color.Green},
                    new Hoge{Name="なむ",Color=Color.Lime},
                    new Hoge{Name="XYZ",Color=Color.Pink},
                    new Hoge{Name="DEF",Color=Color.Aqua},
                    new Hoge{Name="KISD",Color=Color.Maroon},
                    };

                foreach (var c in list) {
                    BoxList.Add(c);
                }

            }); }
        }

        private DelegateCommand _DeleteCommand;
        public DelegateCommand DeleteCommand {
            get { return _DeleteCommand = _DeleteCommand ?? new DelegateCommand(() => {
                BoxList.RemoveAt(0);
            }); }
        }

        private DelegateCommand _ReplaceCommand;
        public DelegateCommand ReplaceCommand {
            get { return _ReplaceCommand = _ReplaceCommand ?? new DelegateCommand(() => {
                BoxList[0] = new Hoge {
                    Name="ReplaceItem",Color = Color.Teal
                };
                //BoxList[0].Name = "Replace";
                //BoxList[0].Color = Color.Teal;
            }); }
        }

        private DelegateCommand<object> _TapCommand;
        public DelegateCommand<object> TapCommand {
            get { return _TapCommand = _TapCommand ?? new DelegateCommand<object>((x) => {
                var item = x as Hoge;
                _pageDialog.DisplayAlertAsync("",item.Name,"OK");
            }); }
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

        public class Hoge:BindableBase
        {
           private string _Name;
            public string Name {
                get { return _Name; }
                set { SetProperty(ref _Name, value); }
            }

            private Color _Color;
            public Color Color {
                get { return _Color; }
                set { SetProperty(ref _Color, value); }
            }
        }
    }
}

