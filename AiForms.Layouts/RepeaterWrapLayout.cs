using System;
using System.Collections;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace AiForms.Layouts
{
    public delegate void RepeaterWrapLayoutItemAddedEventHandler(object sender, RepeaterWrapLayoutItemAddedEventArgs args);

    /// <summary>
    /// code from https://forums.xamarin.com/discussion/21635/xforms-needs-an-itemscontrol/p2
    /// </summary>
    public class RepeaterWrapLayout : WrapLayout
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: "ItemsSource",
            returnType: typeof(IEnumerable),
            declaringType: typeof(RepeaterWrapLayout),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: ItemsChanged);

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            propertyName: "ItemTemplate",
            returnType: typeof(DataTemplate),
            declaringType: typeof(RepeaterWrapLayout),
            defaultValue: default(DataTemplate));

        public event RepeaterWrapLayoutItemAddedEventHandler ItemCreated;

        public IEnumerable ItemsSource {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue) {
            IEnumerable newValueAsEnumerable;
            try {
                newValueAsEnumerable = newValue as IEnumerable;
            }
            catch (Exception e) {
                throw e;
            }

            var control = (RepeaterWrapLayout)bindable;
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
                    control.Children.Add(view);
                    control.OnItemCreated(view);
                }
            }

            control.UpdateChildrenLayout();
            control.InvalidateLayout();
        }

        protected virtual void OnItemCreated(View view) =>
            this.ItemCreated?.Invoke(this, new RepeaterWrapLayoutItemAddedEventArgs(view, view.BindingContext));

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            var invalidate = false;

            if (e.OldItems != null) {
                this.Children.RemoveAt(e.OldStartingIndex);
                invalidate = true;
            }

            if (e.NewItems != null) {
                for (var i = 0; i < e.NewItems.Count; ++i) {
                    var item = e.NewItems[i];
                    var view = this.CreateChildViewFor(item);

                    this.Children.Insert(i + e.NewStartingIndex, view);
                    OnItemCreated(view);
                }

                invalidate = true;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset) {
                this.Children.Clear();
            }

            if (invalidate) {
                this.UpdateChildrenLayout();
                this.InvalidateLayout();
            }


        }

        private View CreateChildViewFor(object item) {
            this.ItemTemplate.SetValue(BindableObject.BindingContextProperty, item);
            return (View)this.ItemTemplate.CreateContent();
        }
    }

    public class RepeaterWrapLayoutItemAddedEventArgs : EventArgs
    {
        private readonly View view;
        private readonly object model;

        public RepeaterWrapLayoutItemAddedEventArgs(View view, object model) {
            this.view = view;
            this.model = model;
        }

        public View View => this.view;

        public object Model => this.model;
    }
}

