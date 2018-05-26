using System.Linq;
using System.Reflection;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Xamarin.Forms.Internals;

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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var types = this.GetType().GetTypeInfo().Assembly.DefinedTypes;
            this.GetType().GetTypeInfo().Assembly.DefinedTypes
                .Where(t => t.Namespace?.EndsWith(".Views", System.StringComparison.Ordinal) ?? false)
                .ForEach(t => {
                    containerRegistry.RegisterForNavigation(t.AsType(), t.Name);
                });
        }
    }
}

