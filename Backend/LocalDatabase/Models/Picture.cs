﻿namespace LocalDatabase.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required byte[]? Data { get; set; }
        public Customer? Customer { get; set; }
    }
}
