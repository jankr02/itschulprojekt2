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
using MesseauftrittDatenerfassung_UI.Dtos.User;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace MesseauftrittDatenerfassung_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class MainWindow
    {
        private CameraAPI _cameraApi;
        private CustomerApiClient _localApiClient;
        private CustomerApiClient _remoteApiClient;
        private bool _imageTaken;
        private byte[] _capturedImageBytes;

        public MainWindow()
        {
            var splashScreen = new SplashScreen("Die Anwendung wird geladen ...");
            splashScreen.Show();
            InitializeApiClient(DatabaseType.LocalDatabase);
            InitializeApiClient(DatabaseType.RemoteDatabase);
            if (!Task.Run(() => _localApiClient.TestConnection(2)).GetAwaiter().GetResult())
            {
                MessageBox.Show("Es konnte keine Verbindung zur lokalen Datenbank hergestellt werden. Die Anwendung wird geschlossen.");
                Close();
            }
            splashScreen.Close();
            InitializeComponent();
            SetCompanyGridEnabled(false);
            SetCompanyGridVisibility(Visibility.Visible, 0.5);
            PopulateProductGroupListBox();
            _cameraApi = new CameraAPI();
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
            _capturedImageBytes = _cameraApi.CaptureImage();

            if (_capturedImageBytes != null)
            {
                personalImage.Source = CustomImageConverter.ConvertByteArrayToBitmapImage(_capturedImageBytes);
                _imageTaken = true;
            }
        }

        private async void OpenAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            UserLoginDto userLogin = new UserLoginDto()
            {
                Username = adminName_TextBox.Text.ToString(),
                Password = passwordBox.Password.ToString()
            };

            var responseLocal = await _localApiClient.Login(userLogin);
            if (!responseLocal.Success)
            {
                MessageBox.Show("Fehler beim Anmelden an der lokalen Datenbank: " + responseLocal.Message);
                return;
            }

            var responseRemote = await _remoteApiClient.Login(userLogin);
            if (!responseRemote.Success)
            {
                MessageBox.Show("Fehler beim Anmelden an der remote Datenbank: " + responseRemote.Message);
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
            if (!ValidateRequiredInput(this)) 
            {
                MessageBox.Show("Bitte füllen Sie alle erforderlichen Felder aus.");
                return; 
            }
            var errorMessage = ValidateInputParameters(this);
            if (errorMessage != String.Empty)
            {
                MessageBox.Show(errorMessage);
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
            var json = File.ReadAllText("FormPlaceholders.json");
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            name_TextBox.Text = data["name"];
            surname_TextBox.Text = data["surname"];
            street_TextBox.Text = data["street"];
            houseNr_TextBox.Text = data["houseNr"];
            city_TextBox.Text = data["city"];
            postalCode_TextBox.Text = data["postalCode"];

            productGroup_ListBox.SelectedIndex = -1;

            companyName_TextBox.Text = data["companyName"];
            companyStreet_TextBox.Text = data["companyStreet"];
            companyHouseNr_TextBox.Text = data["companyHouseNr"];
            companyCity_TextBox.Text = data["companyCity"];
            companyPLZ_TextBox.Text = data["companyPLZ"];

            personalImage.Source = new BitmapImage(new Uri("Resources/defaultImage.jpg", UriKind.Relative));
            _imageTaken = false;
        }

        private static bool ValidateRequiredInput(DependencyObject container)
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
                        if (!ValidateRequiredInput(dependencyObject))
                        {
                            isValid = false;
                        }
                        break;
                    }
                }
            }
            return isValid;
        }

        private string ValidateInputParameters(DependencyObject container)
        {
            var json = File.ReadAllText("FormValidation.json");
            var data = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

            var errorMessage = String.Empty;
            foreach (var child in LogicalTreeHelper.GetChildren(container))
            {
                switch (child)
                {
                    case TextBox textBox:
                        var textBoxName = textBox.Name.Split('_')[0];
                        if (textBoxName == "adminName") break;
                        foreach (var value in data[textBoxName])
                        {
                            switch (textBoxName)
                            {
                                case "name":
                                    if (value.ToLower() == textBox.Text.ToLower() || textBox.Text.Length > 50)
                                    {
                                        errorMessage = "Bitte geben Sie einen gültigen Vornamen ein.";
                                        return errorMessage;
                                    }
                                    break;
                                case "surname":
                                    if (value.ToLower() == textBox.Text.ToLower() || textBox.Text.Length > 50)
                                    {
                                        errorMessage = "Bitte geben Sie einen gültigen Nachnamen ein.";
                                        return errorMessage;
                                    }
                                    break;
                                case "street":
                                    if (value.ToLower() == textBox.Text.ToLower() || textBox.Text.Length > 50)
                                    {
                                        errorMessage = "Bitte geben Sie eine gültige Straße ein.";
                                        return errorMessage;
                                    }
                                    break;
                                case "houseNr":
                                    if (textBox.Text.Length > 4 || !(new Regex("^[0-9]+$").IsMatch(textBox.Text)))
                                    {
                                        errorMessage = "Bitte geben Sie eine gültige Hausnummer ein.";
                                        return errorMessage;
                                    }
                                    break;
                                case "city":
                                    if (value.ToLower() == textBox.Text.ToLower() || textBox.Text.Length > 50)
                                    {
                                        errorMessage = "Bitte geben Sie eine gültige Stadt ein.";
                                        return errorMessage;
                                    }
                                    break;
                                case "postalCode":
                                    if (textBox.Text.Length != 5 || !(new Regex("^[0-9]+$").IsMatch(textBox.Text)))
                                    {
                                        errorMessage = "Bitte geben Sie eine gültige Postleitzahl ein.";
                                        return errorMessage;
                                    }
                                    break;
                                case "companyName":
                                    if (!(bool)company_CheckBox.IsChecked)
                                    {
                                        break;
                                    }
                                    if (value.ToLower() == textBox.Text.ToLower() || textBox.Text.Length > 50)
                                    {
                                        errorMessage = "Bitte geben Sie einen gültigen Unternehmensnamen ein.";
                                        return errorMessage;
                                    }
                                    break;
                                case "companyStreet":
                                    if (!(bool)company_CheckBox.IsChecked)
                                    {
                                        break;
                                    }
                                    if (value.ToLower() == textBox.Text.ToLower() || textBox.Text.Length > 50)
                                    {
                                        errorMessage = "Bitte geben Sie eine gültige Unternehmensstraße ein.";
                                        return errorMessage;
                                    }
                                    break;
                                case "companyHouseNr":
                                    if (!(bool)company_CheckBox.IsChecked)
                                    {
                                        break;
                                    }
                                    if (textBox.Text.Length > 4 || !(new Regex("^[0-9]+$").IsMatch(textBox.Text)))
                                     {
                                        errorMessage = "Bitte geben Sie eine gültige Unternehmenshausnummer ein.";
                                        return errorMessage;
                                    }
                                    break;
                                case "companyCity":
                                    if (!(bool)company_CheckBox.IsChecked)
                                    {
                                        break;
                                    }
                                    if (value.ToLower() == textBox.Text.ToLower() || textBox.Text.Length > 50)
                                    {
                                        errorMessage = "Bitte geben Sie eine gültige Unternehmensstadt ein.";
                                        return errorMessage;
                                    }
                                    break;
                                case "companyPLZ":
                                    if (!(bool)company_CheckBox.IsChecked)
                                    {
                                        break;
                                    }
                                    if (textBox.Text.Length != 5 || !(new Regex("^[0-9]+$").IsMatch(textBox.Text)))
                                    {
                                        errorMessage = "Bitte geben Sie eine gültige Unternehmenspostleitzahl ein.";
                                        return errorMessage;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case ListBox listBox:
                        if (listBox.SelectedIndex == -1)
                        {
                            errorMessage = "Bitte wählen Sie mindestens eine Produktgruppe aus.";
                            return errorMessage;
                        }
                        break;
                    case Image image:
                        if(!_imageTaken)
                        {
                            errorMessage = "Bitte nehmen Sie ein Bild auf.";
                            return errorMessage;
                        }
                        break;
                    case DependencyObject dependencyObject:
                    {
                        var insideErrorMessage = ValidateInputParameters(dependencyObject);
                        if (insideErrorMessage != String.Empty)
                        {
                           errorMessage = insideErrorMessage;
                        }
                        break;
                    }
                }
            }
            return errorMessage;
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
