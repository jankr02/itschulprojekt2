using System.Windows;

namespace MesseauftrittDatenerfassung_UI
{
    /// <summary>
    /// Interaktionslogik für SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen(string message)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
        }
    }
}
