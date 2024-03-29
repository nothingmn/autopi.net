using System.Collections.Generic;
using autopi.net.core.API;
using autopi.net.core.Models;
using System.Linq;
using System.Text;

namespace autopi.net.core.services.export
{
    public class SimpleKMLExporter : ITripExport
    {
        public string Extension { get; set; } = "kml";

        public void ExportAlignedTripData(string filename, GetTripsResponse trip, GetDongleResponse dongle, AlignedTripData alignedTripData)
        {
            if (alignedTripData.AlignedDataPoints.Count <= 0) return;

            string fileTemplate = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><kml xmlns=\"http://www.opengis.net/kml/2.2\"><Document><name>{name}</name><description>{description}</description><Style id=\"lineColor\"><LineStyle><color>7dff0000</color><width>4</width></LineStyle><PolyStyle><color>7dff0000</color></PolyStyle></Style>{Placemarks}</Document></kml>";
            string placemarkTemplate = "<Placemark><styleUrl>#lineColor</styleUrl><LineString><extrude>1</extrude><tessellate>1</tessellate><coordinates>{LOCATIONS}</coordinates></LineString></Placemark>";

            var item = placemarkTemplate;
            var sb = new StringBuilder();
            foreach (var a in alignedTripData.AlignedDataPoints.OrderBy(f => f.Timestamp))
            {
                if (a.Position != null && a.Position.Lat != 0 && a.Position.Lon != 0)
                {
                    sb.Append($"{a.Position?.Lon},{a.Position?.Lat},{a.Altitude}\n");
                }
            }
            item = item.Replace("{LOCATIONS}", sb.ToString());

            var full = fileTemplate.Replace("{name}", $"{trip.StartDisplay} to {trip.EndDisplay} at {trip.StartTimeUtc.ToLocalTime().ToString()}");
            full = full.Replace("{description}", $"Distance:{trip.DistanceKm.ToString("00.00")}km<br />Duration:{trip.Duration}<br />Start:{trip.StartDisplay} at {trip.StartTimeUtc.ToLocalTime()}<br />End:{trip.EndDisplay} at {trip.EndTimeUtc.ToLocalTime()}<br />Duration:{trip.Duration}<br />");
            full = full.Replace("{Placemarks}", item);

            System.IO.File.WriteAllText(filename, full);

        }

    }
}