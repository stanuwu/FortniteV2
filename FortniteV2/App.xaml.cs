using System.Windows;

namespace FortniteV2
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            FortniteV2.MainWindow.Cheat.Dispose();
        }
    }
}