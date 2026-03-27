using Microsoft.Extensions.DependencyInjection;

namespace MiniTFG
{
    public partial class App : Application
    {
        public static Usuario UsuarioTemporal { get; set; }
        public static Usuario UsuarioActual { get; set; }

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}