﻿namespace FibergCarRental.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
