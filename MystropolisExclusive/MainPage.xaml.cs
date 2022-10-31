using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MystropolisExclusive
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string AdminCode = "Mystifax1997";

        public MainPage()
        {
            this.InitializeComponent();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var code = CodeInput.Text;

            ErrorMessage.Visibility = Visibility.Collapsed;

            await CheckCode(code);
        }

        private async Task CheckCode(string code)
        {
            if (code == AdminCode)
            {
                Frame.Navigate(typeof(AdminPage));
                return;
            }

            var mysticlusiveCode = DataAccess.DataAccess.CheckCode(code);
            if (mysticlusiveCode != null)
            {
                if (mysticlusiveCode.Used && mysticlusiveCode.OneTimeUse)
                {
                    Frame.Navigate(typeof(CodeUsed));
                }
                else
                {
                    Frame.Navigate(typeof(VideoPlayer), mysticlusiveCode);
                }
            }
            else
            {
                ErrorMessage.Visibility = Visibility.Visible;
                ErrorMessage.Text = "Koden er ikke gyldig";
            }
        }
    }
}
