﻿namespace RemoteDatabase.Dtos.BusinessDtos
{
    public class GetBusinessDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Street { get; set; }
        public required string HouseNumber { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
    }
}