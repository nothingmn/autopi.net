using System.Collections.Generic;
using autopi.net.core.API;
using autopi.net.core.Models;
using System.Linq;

namespace autopi.net.core.services.export
{
    public class CSVExporter : ITripExport
    {
        public void ExportAlignedTripData(string filename, GetTripsResponse trip, GetDongleResponse dongle, AlignedTripData alignedTripData)
        {

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Timestamp,CoolantTemp,EngineLoad,FuelLevel,FuelRate,IntakeTemp,Lat,Lon,Altitude,RpiTemperature,Speed,Voltage,AccelerometerX,AccelerometerY,AccelerometerZ\r\n");
            foreach (var a in alignedTripData.AlignedDataPoints.OrderBy(f => f.Timestamp))
            {
                sb.Append($"{a.Timestamp},{a.CoolantTemp},{a.EngineLoad},{a.FuelLevel},{a.FuelRate},{a.IntakeTemp},{a.Position?.Lat},{a.Position?.Lon},{a.Altitude},{a.RpiTemperature},{a.Speed},{a.Voltage},{a.AccelerometerX},{a.AccelerometerY},{a.AccelerometerZ}\r\n");
            }

            System.IO.File.WriteAllText(filename, sb.ToString());
        }
        public string Extension { get; set; } = "csv";

    }
}