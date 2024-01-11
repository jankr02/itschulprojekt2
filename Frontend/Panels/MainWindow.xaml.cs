using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MesseauftrittDatenerfassung.MesseauftrittDatenerfassung;
using MesseauftrittDatenerfassung_UI.Converters;
using MesseauftrittDatenerfassung_UI.Dtos.BusinessDtos;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerDtos;
using MesseauftrittDatenerfassung_UI.Dtos.PictureDtos;
using MesseauftrittDatenerfassung_UI.Dtos.ProductGroupDtos;
using MesseauftrittDatenerfassung_UI.Enums;
using Brushes = System.Windows.Media.Brushes;

namespace MesseauftrittDatenerfassung_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private CameraAPI _cameraApi = new CameraAPI();
        private CustomerApiClient _localApiClient;
        private CustomerApiClient _remoteApiClient;
        private byte[] _capturedImageBytes;

        public MainWindow()
        {
            var splashScreen = new SplashScreen("Die Anwendung wird geladen ...");
            splashScreen.Show();
            InitializeApiClient(DatabaseType.LocalDatabase);
            InitializeApiClient(DatabaseType.RemoteDatabase);
            if (!Task.Run(() => _localApiClient.TestConnection(1)).GetAwaiter().GetResult())
            {
                MessageBox.Show("Es konnte keine Verbindung zur lokalen Datenbank hergestellt werden. Die Anwendung wird geschlossen.");
                Close();
            }
            splashScreen.Close();
            InitializeComponent();
            SetCompanyGridEnabled(false);
            SetCompanyGridVisibility(Visibility.Visible, 0.5);
            PopulateProductGroupListBox();
        }

        private void PopulateProductGroupListBox()
        {
            foreach (var item in Enum.GetValues(typeof(ProductGroupName)))
            {
                productGroup_ListBox.Items.Add(item);
            }
        }

        private void InitializeApiClient(DatabaseType databaseType)
        {
            var apiClient = CustomerApiClient.CreateOrGetClient(databaseType);
            switch (databaseType)
            {
                case DatabaseType.LocalDatabase:
                    _localApiClient = apiClient;
                    break;
                case DatabaseType.RemoteDatabase:
                    _remoteApiClient = apiClient;
                    break;
                default:
                    break;
            }            
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if ((textBox != null) && (textBox.Text == textBox.Tag.ToString()))
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if ((textBox != null) && string.IsNullOrWhiteSpace(textBox.Text))
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
            _capturedImageBytes = ToByteArray();
            //_capturedImageBytes = _cameraApi.CaptureImage();

            if (_capturedImageBytes != null)
            {
                personalImage.Source = CustomImageConverter.ConvertByteArrayToBitmapImage(_capturedImageBytes);
            }
        }

        private static byte[] ToByteArray()
        {
            var image = Properties.Resources.testImage;
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return memoryStream.ToArray();
            }
        }

        private void OpenAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            if ((adminName_TextBox.Text != "Admin") || (passwordBox.Password != "Admin123"))
            {
                return;
            }

            var adminPanelWindow = new AdminPanel(_localApiClient, _remoteApiClient);

            Close();

            if (adminPanelWindow.IsClosed == true)
            {
                return;
            }

            adminPanelWindow.Show();
        }

        private void SendData_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput(this)) 
            {
                MessageBox.Show("Bitte füllen Sie alle erforderlichen Felder aus.");
                return; 
            }
            
            var customerData = new AddCompleteCustomerDto()
            {
                FirstName = name_TextBox.Text,
                LastName = surname_TextBox.Text,
                Street = street_TextBox.Text,
                HouseNumber = houseNr_TextBox.Text,
                PostalCode = postalCode_TextBox.Text,
                City = city_TextBox.Text
            };

            if (_capturedImageBytes != null)
            {
                var pictureData = new AddPictureDto
                {
                    Name = personalImage.Name,
                    Data = Convert.ToBase64String(_capturedImageBytes)
                };

                customerData.Picture = pictureData;
            }

            if (productGroup_ListBox.SelectedIndex != -1)
            {
                var customerProductGroups = new List<AddProductGroupDto>();
                foreach (var item in productGroup_ListBox.SelectedItems)
                {
                    if(Enum.TryParse(item.ToString(), out ProductGroupName productGroupName))
                    {
                        customerProductGroups.Add(new AddProductGroupDto{ Name = productGroupName}) ;
                    }
                }

                customerData.ProductGroups = customerProductGroups;
            }

            if ((company_CheckBox.IsChecked != null) && (bool)company_CheckBox.IsChecked)
            {
                var businessData = new AddBusinessDto
                {
                    Name = companyName_TextBox.Text,
                    Street = companyStreet_TextBox.Text,
                    HouseNumber = companyHouseNr_TextBox.Text,
                    City = companyCity_TextBox.Text,
                    PostalCode = companyPLZ_TextBox.Text,
                };

                customerData.Business = businessData;
            }

            _localApiClient.CreateCompleteCustomerAsync(customerData).GetAwaiter().GetResult();

            ResetDataInWindow();

            MessageBox.Show("Ihre Daten wurden erfolgreich gespeichert.");
        }

        private void ResetDataInWindow()
        {
            name_TextBox.Text = "Vorname";
            surname_TextBox.Text = "Nachname";
            street_TextBox.Text = "Straßenname";
            houseNr_TextBox.Text = "Hausnr.";
            city_TextBox.Text = "Ortsname";
            postalCode_TextBox.Text = "PLZ";

            productGroup_ListBox.SelectedIndex = -1;

            companyName_TextBox.Text = "Firmenname";
            companyStreet_TextBox.Text = "Straßenname";
            companyHouseNr_TextBox.Text = "Hausnr.";
            companyCity_TextBox.Text = "Ortsname";
            companyPLZ_TextBox.Text = "PLZ";

            personalImage.Source = new BitmapImage(new Uri("Resources/defaultImage.jpg", UriKind.Relative));
        }

        private static bool ValidateInput(DependencyObject container)
        {
            var isValid = true;
            foreach (var child in LogicalTreeHelper.GetChildren(container))
            {
              switch (child)
              {
                case TextBox textBox when string.IsNullOrWhiteSpace(textBox.Text):
                  textBox.BorderBrush = Brushes.Red;
                  isValid = false;
                  break;
                case DependencyObject dependencyObject:
                {
                  if (!ValidateInput(dependencyObject))
                  {
                    isValid = false;
                  }

                  break;
                }
              }
            }
            return isValid;
        }
    
        private void Company_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetCompanyGridEnabled(true);
            SetCompanyGridVisibility(Visibility.Visible, 1);
        }

        private void Company_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetCompanyGridEnabled(false);
            SetCompanyGridVisibility(Visibility.Visible, 0.5);
        }

        private void SetCompanyGridVisibility(Visibility visibility, double opacity)
        {
            // Stellt sicher, dass alle Elemente im Company_Grid betroffen sind
            foreach (var child in Company_Grid.Children)
            {
              if (!(child is UIElement uiElement))
              {
                continue;
              }

              uiElement.Visibility = visibility;
              uiElement.Opacity = opacity;
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
}
