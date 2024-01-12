namespace RemoteDatabase.Dtos.BusinessDtos
{
    public class AddBusinessDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? Street { get; set; } = string.Empty;
        public string? HouseNumber { get; set; } = string.Empty;
        public string? PostalCode { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
    }
}