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

            decorationComboBox.SelectionChanged += (object sender, System.Windows.Controls.SelectionChangedEventArgs e) => SetDecorationPrice();
            discountComboBox.SelectionChanged += (object sender, System.Windows.Controls.SelectionChangedEventArgs e) => SetDecorationPrice();
        }

        private void SetDecorationPrice()
        {
            if (decorationComboBox.SelectedIndex > -1 && decorationComboBox.SelectedIndex < 15
                && discountComboBox.SelectedIndex > -1 && discountComboBox.SelectedIndex < 4)
            {
                priceLabel.Text = string.Format("{0:n0}", ObjectDeliveryService.GetPriceWithDiscount(discountComboBox.SelectedIndex * 0.1f,
                    decorationComboBox.SelectedIndex));
            }
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            float DiscountPercentage = discountComboBox.SelectedIndex > -1 ? discountComboBox.SelectedIndex * 0.1f : 0.0f;
            int ObjectType = decorationComboBox.SelectedIndex > -1 ? decorationComboBox.SelectedIndex : 0;
            int X_Acre = xAcreComboBox.SelectedIndex > -1 ? xAcreComboBox.SelectedIndex + 1 : 1;
            int Y_Acre = yAcreComboBox.SelectedIndex > -1 ? yAcreComboBox.SelectedIndex + 2 : 2;

            passwordTextBlock.Text = ObjectDeliveryService.GetPasswordString(DiscountPercentage, ObjectType, X_Acre, Y_Acre,
                playerNameTextBox.Text, townNameTextBox.Text);
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
