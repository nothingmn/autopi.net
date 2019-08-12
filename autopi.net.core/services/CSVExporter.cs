using System.Collections.Generic;
using autopi.net.core.API;
using autopi.net.core.Models;
using System.Linq;

namespace autopi.net.core.services
{
    public class CSVExporter
    {
        public void ExportAlignedTripData(string filename, GetTripsResponse trip, GetDongleResponse dongle, AlignedTripData alignedTripData)
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Timestamp,CoolantTemp,EngineLoad,FuelLevel,FuelRate,IntakeTemp,Lat,Lon,RpiTemperature,Speed,Voltage\r\n");
            foreach (var a in alignedTripData.AlignedDataPoints.OrderBy(f => f.Timestamp))
            {
                sb.Append(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}\r\n", a.Timestamp, a.CoolantTemp, a.EngineLoad, a.FuelLevel, a.FuelRate, a.IntakeTemp, a.Position?.Lat, a.Position?.Lon, a.RpiTemperature, a.Speed, a.Voltage));
            }

            System.IO.File.WriteAllText(filename, sb.ToString());
        }

    }
}