using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TheCloudShopsAI.Models
{
    public class Client
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClientID { get; set; }
        public string Name { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
