using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Phoneword.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string _translatedNumber;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void translateButton_Click(object sender, RoutedEventArgs e)
        {
            var textNumber = numberTextBox.Text;
            _translatedNumber = Core.PhonewordTranslator.ToNumber(textNumber);
            if (string.IsNullOrWhiteSpace(_translatedNumber))
            {
                callButton.IsEnabled = false;
                callButton.Content = "Call";
            }
            else
            {
                callButton.IsEnabled = true;
                callButton.Content = "Call - " + _translatedNumber;
            }
        }

        private async void callButton_Click(object sender, RoutedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(
                "Windows.ApplicationModel.Calls.PhoneLine"))
            {
                var store = await PhoneCallManager.RequestStoreAsync();
                var phoneLineId = await store.GetDefaultLineAsync();
                var phoneLine = await PhoneLine.FromIdAsync(phoneLineId);
                phoneLine.Dial(_translatedNumber, "");
            }
            else
            {
                var uriSkype = new Uri($"Skype:{Regex.Replace(_translatedNumber, @"[-\s]", "")}?call");
                //var uriSkype = new Uri($"Skype:{_translatedNumber}?call");

                // Set the option to show a warning
                var promptOptions = new Windows.System.LauncherOptions {TreatAsUntrusted = false};

                // Launch the URI
                await Windows.System.Launcher.LaunchUriAsync(uriSkype, promptOptions);
            }
        }

    }
}
