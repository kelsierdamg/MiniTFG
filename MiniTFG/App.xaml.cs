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
            var shell = new AppShell();

            int savedUserId = Preferences.Get("userId", 0);
            if (savedUserId != 0)
            {
                UsuarioActual = new Usuario
                {
                    Id = savedUserId,
                    Nombre = Preferences.Get("userName", string.Empty),
                    Correo = Preferences.Get("userCorreo", string.Empty)
                };

                shell.Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.GoToAsync("//home");
                });
            }

            return new Window(shell);
        }
    }
}