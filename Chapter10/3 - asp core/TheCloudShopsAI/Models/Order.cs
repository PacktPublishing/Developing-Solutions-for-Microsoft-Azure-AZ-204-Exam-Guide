using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TheCloudShopsAI.Models
{
        public enum Size
        {
            S,M,L,XL
        }

        public class Order
        {
            public int ID { get; set; }
            public int ClientID { get; set; }
            public string ProductName { get; set; }
            public Size Size { get; set; }
            public Client Client { get; set; }
            [NotMapped]
            public string Description { get; set; }

        }
}
