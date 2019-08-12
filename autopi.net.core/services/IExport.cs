using System.Collections.Generic;
using autopi.net.core.API;
using autopi.net.core.Models;

namespace autopi.net.core.services
{
    public interface IExport
    {
        void ExportAlignedTripData(string filename, GetTripsResponse trip, GetDongleResponse dongle, AlignedTripData alignedTripData);

    }
}