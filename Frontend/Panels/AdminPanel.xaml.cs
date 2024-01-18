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
using System.Collections;
using System.Windows.Controls;

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

        public bool IsClosed { get; private set; }

        public AdminPanel(CustomerApiClient localApiClient, CustomerApiClient remoteApiClient)
        {
            InitializeComponent();
            _localApiClient = localApiClient;
            _remoteApiClient = remoteApiClient;
            PopulateProductGroupFilterListBox();

            if (!Task.Run(() => _localApiClient.TestConnection(1)).GetAwaiter().GetResult())
            {
                MessageBox.Show("Es konnte keine Verbindung zur lokalen Datenbank hergestellt werden. Die Anwendung wird geschlossen.");
                Close();
                IsClosed = true;
                return;
            }

            LoadCustomers(DatabaseType.LocalDatabase);

            if (!Task.Run(() => _remoteApiClient.TestConnection(2)).GetAwaiter().GetResult())
            {
                MessageBox.Show("Es konnte keine Verbindung zur remote Datenbank hergestellt werden.");
                return;
            }

            LoadCustomers(DatabaseType.RemoteDatabase);
        }

        private void PopulateProductGroupFilterListBox()
        {
            Filter_ProcuctGroup.Items.Add("[Auswahl aufheben]");

            foreach (var item in Enum.GetValues(typeof(ProductGroupName)))
            {
                Filter_ProcuctGroup.Items.Add(item);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToMainWindow();
        }

        private void LoadCustomers(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.LocalDatabase:
                    var localCustomers = Task.Run(() => _localApiClient.GetAllCustomersAsync()).GetAwaiter().GetResult();
                    _localEntries = ConvertListOfGetCustomerDtoToDataGridData(localCustomers);
                    CustomersDataGridLocal.ItemsSource = _localEntries;
                    break;
                case DatabaseType.RemoteDatabase:
                    var remoteCustomers = Task.Run(() => _remoteApiClient.GetAllCustomersAsync()).GetAwaiter().GetResult();
                    _remoteEntries = ConvertListOfGetCustomerDtoToDataGridData(remoteCustomers);
                    CustomersDataGridRemote.ItemsSource = _remoteEntries;
                    break;
                default:
                    break;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Task.Run(() => CustomerApiClient.IsInternetAvailableAsync()).GetAwaiter().GetResult())
            {
                MessageBox.Show("Es ist keine Internetverbindung vorhanden. Bitte versuchen Sie es später nochmal.");
                return;
            }

            if (!Task.Run(() => _remoteApiClient.TestConnection(2)).GetAwaiter().GetResult())
            {
                MessageBox.Show("Es konnte keine Verbindung zur remote Datenbank hergestellt werden. Bitte versuchen Sie es später nochmal.");
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
            _remoteEntries = ConvertListOfGetCustomerDtoToDataGridData(remoteCustomers);
            DisplayTables();

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
            IsClosed = true;
            mainWindow.Show();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayTables();
        }

        private void Filter_ProcuctGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] == listBox.Items[0])
                {
                    listBox.SelectedItems.Clear();
                }

                if (e.AddedItems[0] != listBox.Items[0])
                {
                    listBox.SelectedItems.Remove(listBox.Items[0]);
                }
            }
            DisplayTables();
        }

        private void DisplayTables()
        {
            var customerProductGroups = new List<string>();
            if (Filter_ProcuctGroup.SelectedIndex != -1)
            {
                foreach (var item in Filter_ProcuctGroup.SelectedItems)
                {
                    if (Enum.TryParse(item.ToString(), out ProductGroupName productGroupName))
                    {
                        customerProductGroups.Add(item.ToString());
                    }
                }
            }

            CustomersDataGridLocal.ItemsSource = _localEntries
                .Where(l => l.LastName.ToLower().Contains(Filter_LastName.Text.ToLower()))
                .Where(l => l.FirstName.ToLower().Contains(Filter_FirstName.Text.ToLower()))
                .Where(l => l.BusinessName.ToLower().Contains(Filter_Business.Text.ToLower()))
                .Where(l => customerProductGroups.All(x => l.ProductGroups.Contains(x)));
            CustomersDataGridRemote.ItemsSource = _remoteEntries
                .Where(r => r.LastName.ToLower().Contains(Filter_LastName.Text.ToLower()))
                .Where(r => r.FirstName.ToLower().Contains(Filter_FirstName.Text.ToLower()))
                .Where(r => r.BusinessName.ToLower().Contains(Filter_Business.Text.ToLower()))
                .Where(r => customerProductGroups.All(x => r.ProductGroups.Contains(x)));
        }
    }
}
