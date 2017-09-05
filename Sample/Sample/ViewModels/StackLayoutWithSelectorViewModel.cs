using System;
using System.Collections.Generic;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;
using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sample.ViewModels
{
    public class StackLayoutWithSelectorViewModel
    {
        public ReactiveCommand ClearCommand { get; } = new ReactiveCommand();
        public ReactiveCommand DeleteCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ReplaceCommand { get; } = new ReactiveCommand();
        public ReactiveCommand AddCommand { get; } = new ReactiveCommand();
        public ObservableCollection<Hoge> BoxList { get; }

        public StackLayoutWithSelectorViewModel(INavigationService navigationService, IPageDialogService pageDlg)
        {
            BoxList = new ObservableCollection<Hoge>(Shuffle());

            AddCommand.Subscribe(_ => {
                BoxList.Add(GetNextItem());
            });

            DeleteCommand.Subscribe(_ => {
                BoxList.Remove(BoxList.Last());
            });

            ReplaceCommand.Subscribe(__ => {
                BoxList[0] = GetNextItem();
            });

            ClearCommand.Subscribe(__ => {
                BoxList.Clear();
            });
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
    }
}
