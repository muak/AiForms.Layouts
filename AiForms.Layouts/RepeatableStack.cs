using System;
using System.Collections;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace AiForms.Layouts
{
    /// <summary>
    /// RepeatableStack
    /// StackLayout corresponding to ItemsSource and ItemTemplate
    /// </summary>
    public class RepeatableStack : StackLayout
    {
        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(RepeatableStack),
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
                typeof(RepeatableStack),
                default(DataTemplate),
                propertyChanged: (bindable, oldValue, newValue) => {
                    var control = (RepeatableStack)bindable;
                    //when to occur propertychanged earlier ItemsSource than ItemTemplate, raise ItemsChanged manually
                    if (newValue != null && control.ItemsSource != null && !control.doneItemSourceChanged) {
                        ItemsChanged(bindable, null, control.ItemsSource);
                    }
                }
            );

        public DataTemplate ItemTemplate {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private bool doneItemSourceChanged = false;

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (RepeatableStack)bindable;
            // when to occur propertychanged earlier ItemsSource than ItemTemplate, do nothing.
            if (control.ItemTemplate == null) {
                control.doneItemSourceChanged = false;
                return;
            }

            control.doneItemSourceChanged = true;

            IEnumerable newValueAsEnumerable;
            try {
                newValueAsEnumerable = newValue as IEnumerable;
            }
            catch (Exception e) {
                throw e;
            }

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
                    var view = CreateChildViewFor(control.ItemTemplate, item, control);

                    control.Children.Add(view);
                }
            }

            control.UpdateChildrenLayout();
            control.InvalidateLayout();
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace) {

                this.Children.RemoveAt(e.OldStartingIndex);

                var item = e.NewItems[e.NewStartingIndex];
                var view = CreateChildViewFor(this.ItemTemplate, item, this);

                this.Children.Insert(e.NewStartingIndex, view);
            }

            else if (e.Action == NotifyCollectionChangedAction.Add) {
                if (e.NewItems != null) {
                    for (var i = 0; i < e.NewItems.Count; ++i) {
                        var item = e.NewItems[i];
                        var view = CreateChildViewFor(this.ItemTemplate, item, this);

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

        }

        private View CreateChildViewFor(object item)
        {
            this.ItemTemplate.SetValue(BindableObject.BindingContextProperty, item);
            return (View)this.ItemTemplate.CreateContent();
        }

        private static View CreateChildViewFor(DataTemplate template, object item, BindableObject container)
        {
            var selector = template as DataTemplateSelector;

            if (selector != null)
            {
                template = selector.SelectTemplate(item, container);
            }

            //Binding context
            template.SetValue(BindableObject.BindingContextProperty, item);

            return (View)template.CreateContent();
        }
    }
}
