using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using autopi.net.core;
using autopi.net.core.API;
using autopi.net.core.auth.API;
using autopi.net.core.Models;
using autopi.net.core.services.export;
using Newtonsoft.Json;

namespace autopi.net.console
{
    public class ExportConsole
    {
        public async Task Main(string[] args)
        {
            var startup = new Startup();
            await startup.Initialize();
            var logger = startup.Logger;

            var credentialsFile = "../../autopi.net.credentials..json";

            Credentials credentials = null;
            try
            {
                if (System.IO.File.Exists(credentialsFile))
                {
                    credentials = JsonConvert.DeserializeObject<Credentials>(await System.IO.File.ReadAllTextAsync(credentialsFile));
                    var auth = new AuthManager(AutoPiApiClient.Client, logger);
                    var loginResult = await auth.CreateLogin(credentials);
                    if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
                    {
                        credentials = null;
                    }

                }
            }
            catch
            {
                credentials = null;

            }
            if (credentials == null)
            {
                LoginResponse loginResponse = null;
                do
                {
                    try
                    {
                        Console.Write("Email Address:");
                        var email = Console.ReadLine();
                        Console.Write("Password:");
                        var password = Console.ReadLine();
                        credentials = new Credentials()
                        {
                            Email = email,
                            Password = password
                        };

                        var auth1 = new AuthManager(AutoPiApiClient.Client, logger);
                        loginResponse = await auth1.CreateLogin(credentials);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Login Failed, try again.");
                    }
                } while (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token));
                System.IO.File.WriteAllText(credentialsFile, JsonConvert.SerializeObject(credentials));
                Console.WriteLine("Login successfull!");

            }

            var logBookManager = new LogBookManager(AutoPiApiClient.Client, logger);
            var dm = new DongleManager(AutoPiApiClient.Client, logger);

            string input = "";

            while (input != "q")
            {
                var dongle = await ShowDongles(dm);
                if (dongle != null)
                {
                    Console.WriteLine($"Selected Dongle: {dongle.Id} - {dongle.CallName} - {dongle.Display}");

                    var trips = await ShowTrips(dongle, logBookManager);
                    if (trips != null)
                    {
                        var exporter = SelectTripExporter();
                        foreach (var trip in trips)
                        {
                            Console.WriteLine($"Selected Trip: {trip.Id} - {trip.StartDisplay} to {trip.EndDisplay} at {trip.StartTimeUtc.ToLocalTime()}");
                            Console.WriteLine("Preparing data for export");
                            var aligned = await logBookManager.GetTripDataAligned(dongle.Id,
                                     PrimitiveDataPoints.CoolantTemp | PrimitiveDataPoints.EngineLoad |
                                     PrimitiveDataPoints.FuelLevel | PrimitiveDataPoints.FuelRate |
                                     PrimitiveDataPoints.IntakeTemp | PrimitiveDataPoints.Position |
                                     PrimitiveDataPoints.RpiTemperature | PrimitiveDataPoints.Speed |
                                     PrimitiveDataPoints.Voltage
                                     , trip.StartTimeUtc, trip.EndTimeUtc, "1s");

                            var path = System.IO.Path.Combine(System.Environment.CurrentDirectory, $"{trip.Id}.{exporter.Extension}");
                            exporter.ExportAlignedTripData(path, trip, dongle, aligned);
                            Console.WriteLine($"Trip data exported to:{path}");
                        }
                    }
                }
                Console.WriteLine("Hit enter to go again, 'q' to quit");
                input = Console.ReadLine();
            }
        }

        private ITripExport SelectTripExporter()
        {
            ITripExport exporter = null;
            do
            {
                Console.WriteLine("Choose an exporter:");
                Console.WriteLine("1) KML (Google Earth, maps, etc.)");
                Console.WriteLine("2) CSV (Excel, etc.)");
                var input = Console.ReadLine();
                if (input.Equals("1"))
                {
                    exporter = new SimpleKMLExporter();
                }
                if (input.Equals("2"))
                {
                    exporter = new CSVExporter();
                }
            } while (exporter == null);
            return exporter;
        }

        private async Task<IList<GetTripsResponse>> ShowTrips(GetDongleResponse dongle, LogBookManager logBookManager)
        {
            Console.WriteLine("Attempting to retreive the list of trips");
            var trips = await logBookManager.GetTrips(dongle.Id, start: DateTime.UtcNow.AddDays(-365));
            if (trips == null)
            {
                Console.WriteLine("No trips were found for this device.");
                return null;
            }
            foreach (var trip in trips)
            {
                Console.WriteLine($"{trip.Id} - {trip.StartDisplay} to {trip.EndDisplay} at {trip.StartTimeUtc.ToLocalTime()}");
            }
            Console.Write("Choose a Trip (use 'all' for all):");
            var input = Console.ReadLine();

            if (string.Equals(input, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                return trips.ToList();
            }

            foreach (var t in trips)
            {
                if (t.Id.ToString().StartsWith(input))
                {
                    return new List<GetTripsResponse>() { t };
                }
            }
            return null;
        }

        private async Task<GetDongleResponse> ShowDongles(DongleManager dm)
        {
            Console.WriteLine("Attempting to retreive the list of dongles");
            var dongles = await dm.GetDongleDevices();

            if (dongles == null)
            {
                Console.WriteLine("No dongles found.");
                return null;
            }
            foreach (var dongle in dongles)
            {
                Console.WriteLine($"{dongle.Id} - {dongle.CallName} - {dongle.Display}");
            }
            Console.Write("Choose a dongle:");
            var input = Console.ReadLine();
            foreach (var d in dongles)
            {
                if (d.Id.ToString().StartsWith(input) || d.CallName.StartsWith(input) || d.Display.StartsWith(input))
                {
                    return d;
                }
            }
            return null;
        }
    }
}
