using System.Windows;

namespace JocDeDame
{
    public partial class AboutWindow : Window
    {
        public AboutWindow(string aboutText)
        {
            InitializeComponent();
            txtAbout.Text = aboutText;
        }
    }
}
