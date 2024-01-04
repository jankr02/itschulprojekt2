using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        public MainWindow()
        {
            SplashScreen splashScreen = new SplashScreen("Die Anwendung wird geladen ...");
            splashScreen.Show();
            if (!Task.Run(() => InitializeApiClientAsync(10)).GetAwaiter().GetResult())
            {
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

        //private void productGroup_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (productGroup_ComboBox.SelectedItem != null)
        //    {
        //        string selectedItem = productGroup_ComboBox.SelectedItem.ToString();
        //        string[] parts = selectedItem.Split(' ');

        //        if (parts.Length >= 2 && int.TryParse(parts[parts.Length - 1], out int productGroupId))
        //        {
        //            _productGroupId = productGroupId;
        //        }
        //    }
        //}

        private async Task<bool> InitializeApiClientAsync(int numberOfConnectionTries)
        {
            try
            {
                _apiClient = CustomerApiClient.CreateOrGetClient(DatabaseType.LocalDatabase);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler bei der Initialisierung: " + ex.Message);
                return false;
            }

            for (int i = 0; i < numberOfConnectionTries; i++)
            {
                try
                {
                    await _apiClient.GetCustomerAsync();
                }
                catch (Exception ex)
                {
                    if(i == (numberOfConnectionTries - 1))
                    {
                        MessageBox.Show("Fehler bei der Initialisierung: " + ex.Message);
                        return false;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    continue;
                }
                return true;
            }
            return true;
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
            _capturedImageBytes = ToByteArray();
            //_capturedImageBytes = cameraApi.CaptureImage();

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

        private static byte[] ToByteArray()
        {
            var image = System.Drawing.Image.FromFile("C:\\Users\\Matth\\Desktop\\Testbild.jpg");
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return memoryStream.ToArray();
            }
        }

        private void OpenAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            if (adminName_TextBox.Text == "Admin" && passwordBox.Password == "Admin123")
            {
                AdminPanel adminPanelWindow = new AdminPanel();
                Close();
                adminPanelWindow.Show();
            }
        }

        private void SendData_Button_Click(object sender, RoutedEventArgs e)
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

            var customerId = AddCustomerToDatabase(customerData);

            if(customerId == 0)
            {
                return;
            }

            if (_capturedImageBytes != null)
            {
                AddPictureDto pictureData = new AddPictureDto()
                {
                    Name = personalImage.Name.ToString(),
                    Image = _capturedImageBytes
                };

                if(!Task.Run(() => AddPictureToCustomerAsync(customerId, pictureData)).GetAwaiter().GetResult())
                {
                    return;
                }
            }

            if (productGroup_ListBox.SelectedIndex != -1)
            {
                var customerProductGroups = new List<AddCustomerProductGroupDto>();
                foreach (var item in productGroup_ListBox.SelectedItems)
                {
                    if(Enum.TryParse(item.ToString(), out ProductGroupName productGroupName))
                    {
                        customerProductGroups.Add(
                            new AddCustomerProductGroupDto()
                            {
                                CustomerId = customerId,
                                ProductGroupId = (int)productGroupName
                            }) ;
                    }
                }

                if (!Task.Run(() => AddProductGroupsToCustomerAsync(customerProductGroups)).GetAwaiter().GetResult())
                {
                    return;
                }
            }

            if ((company_CheckBox.IsChecked != null) && (bool)company_CheckBox.IsChecked)
            {
                AddBusinessDto businessData = new AddBusinessDto()
                {
                    Name = companyName_TextBox.Text.ToString(),
                    Street = companyStreet_TextBox.Text.ToString(),
                    HouseNumber = companyHouseNr_TextBox.Text.ToString(),
                    City = companyCity_TextBox.Text.ToString(),
                    PostalCode = companyPLZ_TextBox.Text.ToString(),
                };

                if(!Task.Run(() => AddBusinessToCustomerAsync(customerId, businessData)).GetAwaiter().GetResult())
                {
                    return;
                }
            }

            MessageBox.Show("Ihre Daten wurden erfolgreich gespeichert.");
        }

        //private void productGroup_CheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    CheckBox checkBox = sender as CheckBox;
        //    if (checkBox != null)
        //    {
        //        string productGroupName = "";
        //        switch (checkBox.Name)
        //        {
        //            case "productGroup_Checkbox1":
        //                productGroupName = productGroup_Label1.Content.ToString();
        //                break;
        //            case "productGroup_Checkbox2":
        //                productGroupName = productGroup_Label2.Content.ToString();
        //                break;
        //            case "productGroup_Checkbox3":
        //                productGroupName = productGroup_Label3.Content.ToString();
        //                break;
        //        }

        //        if (!ProductGroups.Contains(productGroupName))
        //        {
        //            ProductGroups.Add(productGroupName);
        //        }
        //    }
        //}

        //private void productGroup_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    CheckBox checkBox = sender as CheckBox;
        //    if (checkBox != null)
        //    {
        //        string productGroupName = "";
        //        switch (checkBox.Name)
        //        {
        //            case "productGroup_Checkbox1":
        //                productGroupName = productGroup_Label1.Content.ToString();
        //                break;
        //            case "productGroup_Checkbox2":
        //                productGroupName = productGroup_Label2.Content.ToString();
        //                break;
        //            case "productGroup_Checkbox3":
        //                productGroupName = productGroup_Label3.Content.ToString();
        //                break;
        //        }

        //        ProductGroups.Remove(productGroupName);
        //    }
        //}

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

        private int AddCustomerToDatabase(AddCustomerDto customerData)
        {
            int id = 0;
            try
            {
                id = _apiClient.CreateCustomerAsync(customerData).GetAwaiter().GetResult().Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Anlegen des Benutzers: " + ex.Message);
            }
            return id;
        }

        private async Task<bool> AddPictureToCustomerAsync(int customerId, AddPictureDto pictureData)
        {
            try
            {
                await _apiClient.AddPictureToCustomerAsync(customerId, pictureData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Senden des Bildes: " + ex.Message);
                return false;
            }
            return true;
        }

        private async Task<bool> AddProductGroupsToCustomerAsync(List<AddCustomerProductGroupDto> customerProductGroups)
        {
            try
            {
                await _apiClient.AddProductGroupsToCustomerAsync(customerProductGroups);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Hinzufügen der Produktgruppe(n): " + ex.Message);
                return false;
            }
            return true;
        }


        private async Task<bool> AddBusinessToCustomerAsync(int customerId, AddBusinessDto businessData)
        {
            try
            {
                await _apiClient.AddBusinessToCustomerAsync(customerId, businessData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Hinzufügen der Unternehmensdaten: " + ex.Message);
                return false;
            }
            return true;
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
