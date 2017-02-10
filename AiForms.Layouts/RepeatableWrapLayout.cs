using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiForms.Layouts
{

    /// <summary>
    /// WrapLayout corresponding to DataTemplate
    /// Base Code ->  https://forums.xamarin.com/discussion/21635/xforms-needs-an-itemscontrol/p2
    /// </summary>
    public class RepeatableWrapLayout : WrapLayout
    {
        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(RepeatableWrapLayout),
                null,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: ItemsChanged
            );

        public IEnumerable ItemsSource {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static BindableProperty ItemTemplateProperty =
            BindableProperty.Create(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(RepeatableWrapLayout),
                default(DataTemplate)
            );

        public DataTemplate ItemTemplate {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static BindableProperty ItemTapCommandProperty =
            BindableProperty.Create(
                nameof(ItemTapCommand),
                typeof(ICommand),
                typeof(RepeatableWrapLayout),
                default(ICommand),
                defaultBindingMode: BindingMode.OneWay
            );
        /// <summary>
        /// Command invoked when it tapped a item.
        /// </summary>
        public ICommand ItemTapCommand {
            get { return (ICommand)GetValue(ItemTapCommandProperty); }
            set { SetValue(ItemTapCommandProperty, value); }
        }

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            IEnumerable newValueAsEnumerable;
            try {
                newValueAsEnumerable = newValue as IEnumerable;
            }
            catch (Exception e) {
                throw e;
            }

            var control = (RepeatableWrapLayout)bindable;
            var oldObservableCollection = oldValue as INotifyCollectionChanged;

            if (oldObservableCollection != null) {
                oldObservableCollection.CollectionChanged -= control.OnItemsSourceCollectionChanged;
            }

            var newObservableCollection = newValue as INotifyCollectionChanged;

            if (newObservableCollection != null) {
                newObservableCollection.CollectionChanged += control.OnItemsSourceCollectionChanged;
            }

            control.Children.Clear();

            if (newValueAsEnumerable != null) {
                foreach (var item in newValueAsEnumerable) {
                    var view = control.CreateChildViewFor(item);
                    if (control.ItemTapCommand != null) {
                        view.GestureRecognizers.Add(new TapGestureRecognizer {
                            Command = control.ItemTapCommand,
                            CommandParameter = item,
                        });
                    }
                    control.Children.Add(view);
                }
            }

            control.UpdateChildrenLayout();
            control.InvalidateLayout();
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var invalidate = false;

            if (e.Action == NotifyCollectionChangedAction.Replace) {

                this.Children.RemoveAt(e.OldStartingIndex);

                var item = e.NewItems[e.NewStartingIndex];
                var view = CreateChildViewFor(item);

                if (ItemTapCommand != null) {
                    view.GestureRecognizers.Add(new TapGestureRecognizer {
                        Command = ItemTapCommand,
                        CommandParameter = item,
                    });
                }

                this.Children.Insert(e.NewStartingIndex, view);
            }

            else if (e.Action == NotifyCollectionChangedAction.Add) {
                if (e.NewItems != null) {
                    for (var i = 0; i < e.NewItems.Count; ++i) {
                        var item = e.NewItems[i];
                        var view = this.CreateChildViewFor(item);

                        if (ItemTapCommand != null) {
                            view.GestureRecognizers.Add(new TapGestureRecognizer {
                                Command = ItemTapCommand,
                                CommandParameter = item,
                            });
                        }

                        this.Children.Insert(i + e.NewStartingIndex, view);
                    }
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                if (e.OldItems != null) {
                    this.Children.RemoveAt(e.OldStartingIndex);
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Reset) {
                this.Children.Clear();
            }

            else {
                return;
            }

            if (invalidate) {
                this.UpdateChildrenLayout();
                this.InvalidateLayout();
            }

        }

        private View CreateChildViewFor(object item)
        {
            this.ItemTemplate.SetValue(BindableObject.BindingContextProperty, item);
            return (View)this.ItemTemplate.CreateContent();
        }
    }

}

