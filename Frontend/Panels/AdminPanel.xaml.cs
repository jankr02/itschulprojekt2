using MesseauftrittDatenerfassung_UI.Dtos.ProductGroupDtos;
using MesseauftrittDatenerfassung_UI.Dtos.BusinessDtos;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerDtos;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerProductGroupDto;
using MesseauftrittDatenerfassung_UI.Dtos.PictureDtos;
using MesseauftrittDatenerfassung_UI.Enums;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;

namespace MesseauftrittDatenerfassung_UI
{
    /// <summary>
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        private CustomerApiClient _localApiClient = null;
        private CustomerApiClient _remoteApiClient = null;
        private List<AddCustomerProductGroupDto> _productGroups = new List<AddCustomerProductGroupDto>();
        private List<int> _businessIds = new List<int>();

        public AdminPanel()
        {
            InitializeComponent();
            if (Task.Run(() => InitializeApiClientsAsync(1, DatabaseType.LocalDatabase)).GetAwaiter().GetResult())
            {
                CustomersDataGridLocal.ItemsSource = Task.Run(() => _localApiClient.GetAllCustomersAsync().GetAwaiter().GetResult()).Result;
            }
            if (Task.Run(() => InitializeApiClientsAsync(1, DatabaseType.RemoteDatabase)).GetAwaiter().GetResult())
            {
                CustomersDataGridRemote.ItemsSource = Task.Run(() => _remoteApiClient.GetAllCustomersAsync().GetAwaiter().GetResult()).Result;
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToMainWindow();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(!Task.Run(() => _remoteApiClient.IsInternetAvailableAsync()).GetAwaiter().GetResult())
            {
                MessageBox.Show("Es ist keine Internetverbindung vorhanden.");
                return;
            }

            try
            {
                await _remoteApiClient.GetAllCustomersAsync();
            }
            catch (Exception)
            {

                MessageBox.Show("Es besteht kein Anschluss zur Remote Datenbank.");
                return;
            }

            List<GetCustomerDto> allLocalCustomers = Task.Run(() => _localApiClient.GetAllCustomersAsync().GetAwaiter().GetResult()).Result;

            if (allLocalCustomers == null || allLocalCustomers.Count <= 0)
            {
                MessageBox.Show("Es sind keine zu speichernden Daten vorhanden.");
                return;
            }

            List<AddCompleteCustomerDto> newCustomers = new List<AddCompleteCustomerDto>();

            foreach (var customer in allLocalCustomers)
            {
                newCustomers.Add(ConvertToAddCustomerDto(customer));
            }

            await _remoteApiClient.CreateMultipleCustomersAsync(newCustomers);
            await _localApiClient.TruncateAllTablesAsync();

            //_productGroups.Clear();
            //_businessIds.Clear();

            //foreach(var customer in allLocalCustomers)
            //{
            //    var savedCustomer = Task.Run(() => _remoteApiClient.CreateCustomerAsync(ConvertToAddCustomerDto(customer)).GetAwaiter().GetResult()).Result;

            //    if(customer.Business != null)
            //    {
            //        await _remoteApiClient.AddBusinessToCustomerAsync(savedCustomer.Id, ConvertToAddBusinessDto(customer.Business));
            //        AddToListOfBusinesses(customer.Business.Id);
            //    }

            //    if(customer.Picture != null)
            //    {
            //        await _remoteApiClient.AddPictureToCustomerAsync(savedCustomer.Id, ConvertToAddPictureDto(customer.Picture));
            //    }

            //    if(customer.ProductGroups != null)
            //    {
            //        AddToListOfCustomerProductGroups(savedCustomer.Id, customer.ProductGroups);
            //    }

            //    await _localApiClient.DeleteCustomerAsync(customer.Id);
            //}

            //await _remoteApiClient.AddProductGroupsToCustomerAsync(_productGroups);

            //foreach(var customer in allLocalCustomers)
            //{
            //    await _localApiClient.DeleteCustomerAsync(customer.Id);
            //}

            //foreach(var id in _businessIds)
            //{
            //    await _localApiClient.DeleteBusinessAsync(id);
            //}

            //_productGroups.Clear();
            //_businessIds.Clear();

            CustomersDataGridLocal.ItemsSource = Task.Run(() => _localApiClient.GetAllCustomersAsync().GetAwaiter().GetResult()).Result;
            CustomersDataGridRemote.ItemsSource = Task.Run(() => _remoteApiClient.GetAllCustomersAsync().GetAwaiter().GetResult()).Result;

            MessageBox.Show("Die Daten wurden erfolgreich in die Remote Datenbank gespeichert.");
        }

        private AddCompleteCustomerDto ConvertToAddCustomerDto(GetCustomerDto customer)
        {
            var addCustomerDto = new AddCompleteCustomerDto();
            if (customer != null)
            {
                addCustomerDto.FirstName = customer.FirstName;
                addCustomerDto.LastName = customer.LastName;
                addCustomerDto.Street = customer.Street;
                addCustomerDto.HouseNumber = customer.HouseNumber;
                addCustomerDto.PostalCode = customer.PostalCode;
                addCustomerDto.City = customer.City;
                addCustomerDto.Picture = ConvertToAddPictureDto(customer.Picture);
                addCustomerDto.ProductGroups = customer.ProductGroups;
                addCustomerDto.Business = ConvertToAddBusinessDto(customer.Business);
            }
            return addCustomerDto;
        }

        private AddBusinessDto ConvertToAddBusinessDto(GetBusinessDto business)
        {
            var addBusinessDto = new AddBusinessDto();
            if (business != null)
            {
                addBusinessDto.Name = business.Name;
                addBusinessDto.Street = business.Street;
                addBusinessDto.HouseNumber = business.HouseNumber;
                addBusinessDto.PostalCode = business.PostalCode;
                addBusinessDto.City = business.City;
            }
            return addBusinessDto;
        }

        private AddPictureDto ConvertToAddPictureDto(GetPictureDto picture)
        {
            var addPictureDto = new AddPictureDto();
            if (picture != null)
            {
                addPictureDto.Name = picture.Name;
                addPictureDto.Image = picture.Image;
            }
            return addPictureDto;
        }

        //private void AddToListOfCustomerProductGroups(int customerId, List<GetProductGroupDto> productGroups)
        //{
        //    foreach (var productGroup in productGroups)
        //    {
        //        _productGroups.Add(new AddCustomerProductGroupDto { CustomerId = customerId, ProductGroupId = productGroup.Id });
        //    }
        //}

        //private void AddToListOfBusinesses(int businessId)
        //{
        //    if (!_businessIds.Contains(businessId))
        //    {
        //        _businessIds.Add(businessId);
        //    }
        //}

        private void ReturnToMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            Close();
            mainWindow.Show();
        }

        //private void CheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    CheckBox selectedCheckBox = sender as CheckBox;
        //    UpdateCheckBoxes(selectedCheckBox, true);
        //    UpdateTextBoxVisibility(selectedCheckBox, true);
        //}

        //private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    CheckBox selectedCheckBox = sender as CheckBox;
        //    UpdateCheckBoxes(selectedCheckBox, false);
        //    UpdateTextBoxVisibility(selectedCheckBox, false);
        //}

        //private void UpdateCheckBoxes(CheckBox selectedCheckBox, bool isChecked)
        //{
        //    foreach (ComboBoxItem item in FilterComboBox.Items)
        //    {
        //        CheckBox checkBox = (item.Content as CheckBox);
        //        if (checkBox != null && checkBox != selectedCheckBox)
        //        {
        //            checkBox.IsChecked = false;
        //        }
        //    }
        //}

        //private void UpdateTextBoxVisibility(CheckBox selectedCheckBox, bool isChecked)
        //{
        //    // Setze alle Textfelder auf unsichtbar
        //    FilterIdTextBox.Visibility = Visibility.Collapsed;
        //    FilterIdBorder.Visibility = Visibility.Collapsed;
        //    FilterNameTextBox.Visibility = Visibility.Collapsed;
        //    FilterNameBorder.Visibility = Visibility.Collapsed;

        //    // Wenn isChecked, mache das ausgewählte Textfeld sichtbar
        //    if (isChecked)
        //    {
        //        if (selectedCheckBox.Tag.ToString() == "IdCheckBox")
        //        {
        //            FilterIdTextBox.Visibility = Visibility.Visible;
        //            FilterIdBorder.Visibility = Visibility.Visible;
        //        }
        //        else if (selectedCheckBox.Tag.ToString() == "NameCheckBox")
        //        {
        //            FilterNameTextBox.Visibility = Visibility.Visible;
        //            FilterNameBorder.Visibility = Visibility.Visible;
        //        }
        //        else if (selectedCheckBox.Tag.ToString() == "BothCheckBox")
        //        {
        //            FilterIdTextBox.Visibility = Visibility.Visible;
        //            FilterIdBorder.Visibility = Visibility.Visible;
        //            FilterNameTextBox.Visibility = Visibility.Visible;
        //            FilterNameBorder.Visibility = Visibility.Visible;
        //        }
        //    }
        //}

        private async Task<bool> InitializeApiClientsAsync(int numberOfConnectionTries, DatabaseType databaseType)
        {
            if (databaseType == DatabaseType.LocalDatabase)
            {
                _localApiClient = CustomerApiClient.CreateOrGetClient(DatabaseType.LocalDatabase);
            }
            else if (databaseType == DatabaseType.RemoteDatabase)
            {
                _remoteApiClient = CustomerApiClient.CreateOrGetClient(DatabaseType.RemoteDatabase);
            }
            else
            {
                MessageBox.Show("Fehler bei der Initialisierung: Nicht existenter Datenbanktyp");
                return false;
            }

            for (int i = 0; i < numberOfConnectionTries; i++)
            {
                try
                {
                    if (databaseType == DatabaseType.LocalDatabase)
                    {
                        await _localApiClient.GetAllCustomersAsync();
                    }
                    else if (databaseType == DatabaseType.RemoteDatabase)
                    {
                        await _remoteApiClient.GetAllCustomersAsync();
                    }
                }
                catch (Exception ex)
                {
                    if (i == (numberOfConnectionTries - 1))
                    {
                        if (databaseType == DatabaseType.LocalDatabase)
                        {
                            MessageBox.Show("Fehler bei der Initialisierung der lokalen Datenbank: " + ex.Message);
                        }
                        else if (databaseType == DatabaseType.RemoteDatabase)
                        {
                            MessageBox.Show("Fehler bei der Initialisierung der remote Datenbank: " + ex.Message);
                        }
                        return false;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    continue;
                }
                return true;
            }
            return true;
        }
    }
}
