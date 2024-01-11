using System.Windows.Media.Imaging;

namespace MesseauftrittDatenerfassung_UI.Models
{
    public class DataGridRecordSet
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public BitmapImage ImageData { get; set; }
    public string ProductGroups { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string BusinessName { get; set; }
    public string BusinessStreet { get; set; }
    public string BusinessHouseNumber { get; set; }
    public string BusinessPostalCode { get; set; }
    public string BusinessCity { get; set; }
  }
}
