using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class RepeatableFlexPageViewModel:BindableBase
    {

        public ReactiveCommand ClearCommand { get; } = new ReactiveCommand();
        public ReactiveCommand DeleteCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ReplaceCommand { get; } = new ReactiveCommand();
        public ReactiveCommand AddCommand { get; } = new ReactiveCommand();
        public ReactiveCommand<string> DirectionCommand { get; } = new ReactiveCommand<string>();
        public ReactiveCommand<string> AlignItemsCommand { get; } = new ReactiveCommand<string>();
        public ReactiveCommand<string> JustifyContentCommand { get; } = new ReactiveCommand<string>();
        public ReactiveCommand<string> WrapCommand { get; } = new ReactiveCommand<string>();
        public ReactiveProperty<Color>[] DirectionColor { get; } = new ReactiveProperty<Color>[3];
        public ReactiveProperty<Color>[] AColor { get; } = new ReactiveProperty<Color>[5];
        public ReactiveProperty<Color>[] JColor { get; } = new ReactiveProperty<Color>[7];
        public ReactiveProperty<Color>[] WrapColor { get; } = new ReactiveProperty<Color>[2];
        public ObservableCollection<Hoge> BoxList { get; }

        public ReactiveProperty<FlexDirection> FlexDirection { get; } = new ReactiveProperty<FlexDirection>();
        public ReactiveProperty<FlexAlignItems> FlexAlignItems { get; } = new ReactiveProperty<FlexAlignItems>();
        public ReactiveProperty<FlexJustify> FlexJustify { get; } = new ReactiveProperty<FlexJustify>();
        public ReactiveProperty<FlexWrap> FlexWrap { get; } = new ReactiveProperty<FlexWrap>();
        public ReactiveProperty<ScrollOrientation> ScrollDirection { get; } = new ReactiveProperty<ScrollOrientation>();

        public RepeatableFlexPageViewModel(INavigationService navigationService, IPageDialogService pageDlg)
        {



            FlexDirection.Value = Xamarin.Forms.FlexDirection.Row;
            FlexAlignItems.Value = Xamarin.Forms.FlexAlignItems.Start;
            FlexJustify.Value = Xamarin.Forms.FlexJustify.Start;
            FlexWrap.Value = Xamarin.Forms.FlexWrap.NoWrap;
            ScrollDirection.Value = ScrollOrientation.Horizontal;

            SetColors(DirectionColor,0);
            SetColors(AColor,3);
            SetColors(JColor,3);
            SetColors(WrapColor,0);         

            DirectionCommand.Subscribe(x => {
                var idx = int.Parse(x);
                FlexDirection.Value = (Xamarin.Forms.FlexDirection)idx;
                SetScrollDirection();
                SetColors(DirectionColor, idx);
            });
            AlignItemsCommand.Subscribe(x => {
                var idx = int.Parse(x);
                FlexAlignItems.Value = (Xamarin.Forms.FlexAlignItems)idx;
                SetColors(AColor, idx);
            });
            JustifyContentCommand.Subscribe(x => {
                var idx = int.Parse(x);
                FlexJustify.Value = (Xamarin.Forms.FlexJustify)idx;
                SetColors(JColor, idx);
            });
            WrapCommand.Subscribe(x => {
                var idx = int.Parse(x);
                FlexWrap.Value = (Xamarin.Forms.FlexWrap)idx;
                SetScrollDirection();
                SetColors(WrapColor, idx);
            });


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

        void SetScrollDirection()
        {
            if (FlexDirection.Value == Xamarin.Forms.FlexDirection.Row && FlexWrap.Value == Xamarin.Forms.FlexWrap.NoWrap ||
                   FlexDirection.Value == Xamarin.Forms.FlexDirection.Column && FlexWrap.Value == Xamarin.Forms.FlexWrap.Wrap) {
                ScrollDirection.Value = ScrollOrientation.Horizontal;
            }
            else {
                ScrollDirection.Value = ScrollOrientation.Vertical;
            }
        }

        void SetColors(ReactiveProperty<Color>[] reactiveProperties,int target = -1) 
        {
            for (var i = 0; i < reactiveProperties.Length; i++){
                if(reactiveProperties[i] == null){
                    reactiveProperties[i] = new ReactiveProperty<Color>();
                }
                reactiveProperties[i].Value = Color.Blue;
            }
            if (target >= 0) {
                reactiveProperties[target].Value = Color.Red;
            }
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
