using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Unity;

[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
namespace Sample
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected async override void OnInitialized()
        {
            InitializeComponent();

           await NavigationService.NavigateAsync("MyNavigationPage/SelectPage");
        }

        protected override void RegisterTypes()
        {
            var types = this.GetType().GetTypeInfo().Assembly.DefinedTypes;
            this.GetType().GetTypeInfo().Assembly.DefinedTypes
                .Where(t => t.Namespace?.EndsWith(".Views", System.StringComparison.Ordinal) ?? false)
                .ForEach(t => {
                    Container.RegisterTypeForNavigation(t.AsType(), t.Name);
                });
        }

    }
}

