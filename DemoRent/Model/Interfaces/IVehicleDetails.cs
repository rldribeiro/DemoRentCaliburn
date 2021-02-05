using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    interface IVehicleDetails
    {
        string Brand { get; set; }
        string Model { get; set; }
        string PhotoUrl { get; set; }
        int Year { get; set; }       
        int Kms { get; set; }
    }
}
