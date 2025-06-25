using Rrs.Tasks;
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

namespace WpfApp1;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TaskCache<string> _tc = new();
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Run_Click(object sender, RoutedEventArgs e)
    {
        var context = new object();
        Output.Text = $"{DateTime.Now} - Run {context.GetHashCode()}{Environment.NewLine}" + Output.Text;
        _ = _tc.RunOrStart("Run", async () =>
        {
            await Task.Delay(10_000);
            _ = Dispatcher.BeginInvoke(() =>
            {
                Output.Text = $"{DateTime.Now} - Run {context.GetHashCode()} Complete{Environment.NewLine}" + Output.Text;
            });
        });
    }

    private async void Get_Click(object sender, RoutedEventArgs e)
    {
        var context = new object();
        Output.Text = $"{DateTime.Now} - Get {context.GetHashCode()}{Environment.NewLine}" + Output.Text;
        var winner = await _tc.GetOrStart("Get", async () =>
        {
            await Task.Delay(10_000);
            return context;
        });
        if (winner == context)
        {
            Output.Text = $"{DateTime.Now} - Get {context.GetHashCode()} Complete{Environment.NewLine}" + Output.Text;
        }
    }
}