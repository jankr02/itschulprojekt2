using System.Text.Json.Serialization;

namespace MesseauftrittDatenerfassung.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductGroups
    {
        Testgruppe1 = 1,
        Testgruppe2 = 2,
        Testgruppe3 = 3,
        Testgruppe4 = 4,
    }
}
