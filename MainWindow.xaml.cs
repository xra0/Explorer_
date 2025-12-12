using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_Object_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBox_Object.Text != "")
                Button_Add.IsEnabled = true;
            else Button_Add.IsEnabled = false;
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}