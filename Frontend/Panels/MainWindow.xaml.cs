using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gu.Wpf.Adorners;
using MesseauftrittDatenerfassung.MesseauftrittDatenerfassung;
using MesseauftrittDatenerfassung_UI.Dtos.BusinessDtos;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerDtos;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerProductGroupDto;
using MesseauftrittDatenerfassung_UI.Dtos.PictureDtos;
using MesseauftrittDatenerfassung_UI.Enums;

namespace MesseauftrittDatenerfassung_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CameraAPI cameraApi = new CameraAPI();
        private CustomerApiClient _apiClient;
        private byte[] _capturedImageBytes;
        private int _productGroupId;
        public MainWindow()
        {
            InitializeComponent();
            SplashScreen splashScreen = new SplashScreen("Die Anwendung wird geladen ...");
            splashScreen.Show();
            InitializeApiClientAsync(splashScreen);
            PopulateProductGroupComboBox();
            SetCompanyGridEnabled(false);
            SetCompanyGridVisibility(Visibility.Visible, 0.5);
        }

        private void PopulateProductGroupComboBox()
        {
            productGroup_ComboBox.ItemsSource = Enum.GetValues(typeof(ProductGroupName))
                                                       .Cast<ProductGroupName>();
        }

        private void productGroup_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productGroup_ComboBox.SelectedItem != null)
            {
                string selectedItem = productGroup_ComboBox.SelectedItem.ToString();
                string[] parts = selectedItem.Split(' ');

                if (parts.Length >= 2 && int.TryParse(parts[parts.Length - 1], out int productGroupId))
                {
                    _productGroupId = productGroupId;
                }
            }
        }

        private async void InitializeApiClientAsync(SplashScreen splashScreen)
        {
            try
            {
                _apiClient = await CustomerApiClient.CreateAsync();
                await Task.Delay(TimeSpan.FromSeconds(0.15));
                splashScreen.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler bei der Initialisierung: " + ex.Message);
                Close(); 
            }

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            watermarkTextBlock.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Image_Button_Click(object sender, RoutedEventArgs e)
        {
            _capturedImageBytes = cameraApi.CaptureImage();

            if (_capturedImageBytes != null)
            {
                using (MemoryStream ms = new MemoryStream(_capturedImageBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    personalImage.Source = image;
                }
            }
        }

        private void OpenAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            if (adminName_TextBox.Text == "Admin" && passwordBox.Password == "Admin123")
            {
                AdminPanel adminPanelWindow = new AdminPanel();
                this.Close();
                adminPanelWindow.Show();
            }
        }

        private void sendData_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(this)) 
            {
                MessageBox.Show("Bitte füllen Sie alle erforderlichen Felder aus.");
                return; 
            }

            AddCustomerDto customerData = new AddCustomerDto()
            {
                FirstName = name_TextBox.Text.ToString(),
                LastName = surname_TextBox.Text.ToString(),
                Street = street_TextBox.Text.ToString(),
                HouseNumber = houseNr_TextBox.Text.ToString(),
                PostalCode = postalCode_TextBox.Text.ToString(),
                City = city_TextBox.Text.ToString()
            };

            AddBusinessDto businessData = new AddBusinessDto()
            {
                Name = companyName_TextBox.Text.ToString(),
                Street = companyStreet_TextBox.Text.ToString(),
                HouseNumber = companyHouseNr_TextBox.Text.ToString(),
                City = companyCity_TextBox.Text.ToString(),
                PostalCode = companyPLZ_TextBox.Text.ToString(),
            };

            AddPictureDto pictureData = new AddPictureDto()
            {
                Name = personalImage.Name.ToString(),
                Image = _capturedImageBytes
            };

            AddCustomerProductGroupDto customerProductGroupData = new AddCustomerProductGroupDto()
            {
            };

            SendDataToBackend(customerData, businessData, pictureData, customerProductGroupData);
        }

        private bool ValidateInput(DependencyObject container)
        {
            bool isValid = true;
            foreach (var child in LogicalTreeHelper.GetChildren(container))
            {
                if (child is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.BorderBrush = Brushes.Red;
                    isValid = false;
                }
                else if (child is DependencyObject dependencyObject)
                {
                    if (!ValidateInput(dependencyObject))
                    {
                        isValid = false;
                    }
                }
            }
            return isValid;
        }

        private async void SendDataToBackend(AddCustomerDto customerData, AddBusinessDto businessData, 
            AddPictureDto pictureData, AddCustomerProductGroupDto customerProductGroupData)
        {
            try
            {
                int id = _apiClient.GetCustomerAsync().Result.Id;

                await _apiClient.CreateCustomerAsync(customerData);
                await _apiClient.AddBusinessToCustomerAsync(id, businessData);
                await _apiClient.AddPictureToCustomerAsync(id, pictureData);
                await _apiClient.AddProductGroupToCustomerAsync(id, customerProductGroupData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Senden der Daten: " + ex.Message);
            }
        }

        private void company_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetCompanyGridEnabled(true);
            SetCompanyGridVisibility(Visibility.Visible, 1);
        }

        private void company_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetCompanyGridEnabled(false);
            SetCompanyGridVisibility(Visibility.Visible, 0.5);
        }

        private void SetCompanyGridVisibility(Visibility visibility, double opacity)
        {
            // Stellt sicher, dass alle Elemente im Company_Grid betroffen sind
            foreach (var child in Company_Grid.Children)
            {
                if (child is UIElement uiElement)
                {
                    uiElement.Visibility = visibility;
                    uiElement.Opacity = opacity;
                }
            }
        }

        private void SetCompanyGridEnabled(bool isEnabled)
        {
            foreach (var child in Company_Grid.Children)
            {
                if (child is UIElement uiElement)
                {
                    uiElement.IsEnabled = isEnabled;
                }
            }
        }
    }

    public class PasswordLengthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null && value is int && (int)value > 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
