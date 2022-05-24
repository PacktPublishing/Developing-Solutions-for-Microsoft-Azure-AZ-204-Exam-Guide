using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consumer
{
    //Model class for messaging
    public class Booking
    {
        public class AirBooking
        {
            public string To { get; set; }
            public string From { get; set; }
            public DateTime FlighDate { get; set; }
        }

        public class HotelBooking
        {
            public string City { get; set; }
            public DateTime CheckinDate { get; set; }
            public DateTime LeaveDate { get; set; }
        }

        public AirBooking[] AirBookings { get; set; }
        public HotelBooking[] HotelBookings { get; set; }

        public override string ToString()
        {
            var air = AirBookings == null ? new[] { "No flights" } : AirBookings.Select(x => $"Flight: {x.To}=>{x.From} {x.FlighDate.Date}");
            var hotel = HotelBookings == null ? new[] { "No hotels" } : HotelBookings.Select(x => $"Hotel: {x.City} {x.CheckinDate.Date}").ToArray();
            return
                string.Join("\r\n", air.Union(hotel));

        }
    }
}
