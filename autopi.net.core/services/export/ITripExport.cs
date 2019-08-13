using System.Collections.Generic;
using autopi.net.core.API;
using autopi.net.core.Models;

namespace autopi.net.core.services.export
{
    public interface ITripExport
    {
        void ExportAlignedTripData(string filename, GetTripsResponse trip, GetDongleResponse dongle, AlignedTripData alignedTripData);
        string Extension { get; set; }
    }
}