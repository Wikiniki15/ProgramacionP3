using ProgramacionP3.VistaModelo;
namespace ProgramacionP3
{
    public partial class App : Application
    {
        public static BuscarPais BuscarPaisViewModel { get; private set; }
        public App()
        {
            InitializeComponent(); 

            BuscarPaisViewModel = new BuscarPais(); 
            MainPage = new AppShell(); 
        } 
    }
}
