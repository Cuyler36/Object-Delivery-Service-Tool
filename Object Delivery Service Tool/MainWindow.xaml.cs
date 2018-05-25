using System.Windows;

namespace Object_Delivery_Service_Tool
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

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            float DiscountPercentage = discountComboBox.SelectedIndex > -1 ? discountComboBox.SelectedIndex * 0.1f : 0.0f;
            int ObjectType = decorationComboBox.SelectedIndex > -1 ? decorationComboBox.SelectedIndex : 0;
            int X_Acre = xAcreComboBox.SelectedIndex > -1 ? xAcreComboBox.SelectedIndex + 1 : 1;
            int Y_Acre = yAcreComboBox.SelectedIndex > -1 ? yAcreComboBox.SelectedIndex + 2 : 2;
            string EncodedPlayerName = ObjectDeliveryService.EncodeString(playerNameTextBox.Text);
            string EncodedTownName = ObjectDeliveryService.EncodeString(townNameTextBox.Text);

            passwordTextBlock.Text = ObjectDeliveryService.GetPasswordString(DiscountPercentage, ObjectType, X_Acre, Y_Acre, EncodedPlayerName, EncodedTownName);
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(passwordTextBlock.Text))
            {
                Clipboard.SetText(passwordTextBlock.Text);
                MessageBox.Show("The Password was copied to the clipboard!", "Password Copied", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
