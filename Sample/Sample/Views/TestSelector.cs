using System;
using Xamarin.Forms;
using Sample.ViewModels;
namespace Sample.Views
{
    public class TestSelector:DataTemplateSelector
    {
        public DataTemplate TemplateA { get; set; }
        public DataTemplate TemplateB { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var hoge = item as Hoge;
            return hoge.Color.Luminosity < 0.5d ? TemplateA : TemplateB;
        }
    }
}
