using MesseauftrittDatenerfassung_UI.Dtos.BusinessDtos;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerDtos;
using MesseauftrittDatenerfassung_UI.Dtos.PictureDtos;
using MesseauftrittDatenerfassung_UI.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MesseauftrittDatenerfassung_UI.Dtos.ProductGroupDtos;
using MesseauftrittDatenerfassung_UI.Models;
using static MesseauftrittDatenerfassung_UI.Converters.CustomImageConverter;
using System.Windows.Media.Imaging;

namespace MesseauftrittDatenerfassung_UI
{
    /// <summary>
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel
  {
        private CustomerApiClient _localApiClient;
        private CustomerApiClient _remoteApiClient;

        private ObservableCollection<DataGridRecordSet> _localEntries;
        private ObservableCollection<DataGridRecordSet> _remoteEntries;

        public AdminPanel()
        {
          InitializeComponent();
            if (Task.Run(() => InitializeApiClientsAsync(1, DatabaseType.LocalDatabase)).GetAwaiter().GetResult()){}
            {
              var localCustomers = Task.Run(() => _localApiClient.GetAllCustomersAsync()).GetAwaiter().GetResult();
              _localEntries = ConvertListOfGetCustomerDtoToDataGridData(localCustomers);
              CustomersDataGridLocal.ItemsSource = ConvertListOfGetCustomerDtoToDataGridData(localCustomers);
            }
            if (Task.Run(() => InitializeApiClientsAsync(1, DatabaseType.RemoteDatabase)).GetAwaiter().GetResult())
            {
              var remoteCustomers = Task.Run(() => _remoteApiClient.GetAllCustomersAsync()).GetAwaiter().GetResult();
              _remoteEntries = ConvertListOfGetCustomerDtoToDataGridData(remoteCustomers);
              CustomersDataGridRemote.ItemsSource = ConvertListOfGetCustomerDtoToDataGridData(remoteCustomers);
            }
        }

        //private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;
        //    if (textBox != null && textBox.Text == textBox.Tag.ToString())
        //    {
        //        textBox.Text = string.Empty;
        //    }
        //}

        //private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;
        //    if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
        //    {
        //        textBox.Text = textBox.Tag.ToString();
        //    }
        //}

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToMainWindow();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //if(!Task.Run(() => _remoteApiClient.IsInternetAvailableAsync()).GetAwaiter().GetResult())
            //{
            //    MessageBox.Show("Es ist keine Internetverbindung vorhanden.");
            //    return;
            //}

            try
            {
                await _remoteApiClient.GetAllCustomersAsync();
            }
            catch (Exception)
            {

                MessageBox.Show("Es besteht kein Anschluss zur Remote Datenbank.");
                return;
            }

            var allLocalCustomers = Task.Run(() => _localApiClient.GetAllCustomersAsync().GetAwaiter().GetResult()).Result;

            if ((allLocalCustomers == null) || (allLocalCustomers.Count <= 0))
            {
                MessageBox.Show("Es sind keine zu speichernden Daten vorhanden.");
                return;
            }

            var newCustomers = allLocalCustomers.Select(ConvertToAddCustomerDto).ToList();

            
            var remoteCustomers = Task.Run(() => _remoteApiClient.CreateMultipleCompleteCustomersAsync(newCustomers)).GetAwaiter().GetResult();
            CustomersDataGridRemote.ItemsSource = ConvertListOfGetCustomerDtoToDataGridData(remoteCustomers);
            var localCustomers = Task.Run(() => _localApiClient.TruncateAllTablesAsync()).GetAwaiter().GetResult();
            CustomersDataGridLocal.ItemsSource = ConvertListOfGetCustomerDtoToDataGridData(localCustomers);

            MessageBox.Show("Die Daten wurden erfolgreich in die Remote Datenbank gespeichert.");
        }

        private static AddCompleteCustomerDto ConvertToAddCustomerDto(GetCustomerDto customer)
        {
            var addCustomerDto = new AddCompleteCustomerDto();
            if (customer == null)
            {
              return addCustomerDto;
            }

            addCustomerDto.FirstName = customer.FirstName;
            addCustomerDto.LastName = customer.LastName;
            addCustomerDto.Street = customer.Street;
            addCustomerDto.HouseNumber = customer.HouseNumber;
            addCustomerDto.PostalCode = customer.PostalCode;
            addCustomerDto.City = customer.City;
            addCustomerDto.Picture = ConvertToAddPictureDto(customer.Picture);
            addCustomerDto.ProductGroups = ConvertToAddProductGroupDto(customer.ProductGroups);
            addCustomerDto.Business = ConvertToAddBusinessDto(customer.Business);

            return addCustomerDto;
        }

        private static AddBusinessDto ConvertToAddBusinessDto(GetBusinessDto business)
        {
            var addBusinessDto = new AddBusinessDto();
            if (business == null)
            {
              return addBusinessDto;
            }

            addBusinessDto.Name = business.Name;
            addBusinessDto.Street = business.Street;
            addBusinessDto.HouseNumber = business.HouseNumber;
            addBusinessDto.PostalCode = business.PostalCode;
            addBusinessDto.City = business.City;

            return addBusinessDto;
        }

        private static AddPictureDto ConvertToAddPictureDto(GetPictureDto picture)
        {
            var addPictureDto = new AddPictureDto();
            if (picture == null)
            {
                return addPictureDto;
            }

            addPictureDto.Data = picture.Data;
            addPictureDto.Name = picture.Name;

            return addPictureDto;
        }

        private static List<AddProductGroupDto> ConvertToAddProductGroupDto(List<GetProductGroupDto> productGroups)
        {
            return (from productGroup in productGroups
                    where productGroup != null
                    select new AddProductGroupDto { Name = productGroup.Name }).ToList();
        }

        private static ObservableCollection<DataGridRecordSet> ConvertListOfGetCustomerDtoToDataGridData(List<GetCustomerDto> customers)
        {
          var dataGridData = new ObservableCollection<DataGridRecordSet>();
          foreach (var customer in customers)
            dataGridData.Add(new DataGridRecordSet
            {
              Id = customer.Id,
              FirstName = customer.FirstName ?? string.Empty,
              LastName = customer.LastName ?? string.Empty,
              ImageData = ConvertStringToBitmapImage(customer.Picture != null ? (customer.Picture.Data ?? string.Empty) : string.Empty) ?? new BitmapImage(new Uri("Resources/defaultImage.jpg", UriKind.Relative)),
              ProductGroups = ConvertProductGroupsToString(customer.ProductGroups) ?? string.Empty,
              Street = customer.Street ?? string.Empty,
              HouseNumber = customer.HouseNumber ?? string.Empty,
              PostalCode = customer.PostalCode ?? string.Empty,
              City = customer.City ?? string.Empty,
              BusinessName = customer.Business != null ? (customer.Business.Name ?? string.Empty) : string.Empty,
              BusinessStreet = customer.Business != null ? (customer.Business.Street ?? string.Empty) : string.Empty,
              BusinessHouseNumber = customer.Business != null ? (customer.Business.HouseNumber ?? string.Empty) : string.Empty,
              BusinessPostalCode = customer.Business != null ? (customer.Business.PostalCode ?? string.Empty) : string.Empty,
              BusinessCity = customer.Business != null ? (customer.Business.City ?? string.Empty) : string.Empty,
            });

          return dataGridData;
        }

        private static string ConvertProductGroupsToString(List<GetProductGroupDto> productGroups)
        { 
            var productGroupString = string.Empty;

            if ((productGroups == null) || !productGroups.Any())
            {
              return productGroupString;
            }

            productGroupString = productGroups.Aggregate(productGroupString, (current, group) => current + $"{group.Name.ToString()}, ");
            productGroupString = productGroupString.Substring(0, productGroupString.Length - 2);
            return productGroupString;
        }
    
        private void ReturnToMainWindow()
        {
            var mainWindow = new MainWindow();
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
            switch (databaseType)
            {
              case DatabaseType.LocalDatabase:
                _localApiClient = CustomerApiClient.CreateOrGetClient(DatabaseType.LocalDatabase);
                break;
              case DatabaseType.RemoteDatabase:
                _remoteApiClient = CustomerApiClient.CreateOrGetClient(DatabaseType.RemoteDatabase);
                break;
              default:
                MessageBox.Show("Fehler bei der Initialisierung: Nicht existenter Datenbanktyp");
                return false;
            }

            for (var i = 0; i < numberOfConnectionTries; i++)
            {
                try
                {
                  switch (databaseType)
                  {
                    case DatabaseType.LocalDatabase:
                      await _localApiClient.GetAllCustomersAsync();
                      break;
                    case DatabaseType.RemoteDatabase:
                      await _remoteApiClient.GetAllCustomersAsync();
                      break;
                    default:
                      throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
                  }
                }
                catch (Exception ex)
                {
                    if (i == (numberOfConnectionTries - 1))
                    {
                      switch (databaseType)
                      {
                        case DatabaseType.LocalDatabase:
                          MessageBox.Show("Fehler bei der Initialisierung der lokalen Datenbank: " + ex.Message);
                          break;
                        case DatabaseType.RemoteDatabase:
                          MessageBox.Show("Fehler bei der Initialisierung der remote Datenbank: " + ex.Message);
                          break;
                        default:
                          throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
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
