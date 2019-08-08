using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using autopi.net.core;
using autopi.net.core.API;
using autopi.net.core.auth;
using autopi.net.core.auth.API;
using autopi.net.core.Models;
using autopi.net.core.services;
using autopi.net.core.tags;
using Newtonsoft.Json;

namespace autopi.net.console
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var geoFences = TestPoly();
            var fenceService = new PolygonGeoFence();
            var startup = new Startup();
            await startup.Initialize();
            var metaDataStorage = startup.MetaDataStorage;
            var logger = startup.Logger;


            Credentials credentials = null;
            try
            {
                var credentialsFile = "../../autopi.net.credentials.json";
                if (System.IO.File.Exists(credentialsFile))
                {
                    credentials = JsonConvert.DeserializeObject<Credentials>(await System.IO.File.ReadAllTextAsync(credentialsFile));
                }
            }
            catch
            {

            }
            var auth = new AuthManager(AutoPiApiClient.Client, logger);
            var loginResult = await auth.CreateLogin(credentials);

            if (loginResult != null && !string.IsNullOrEmpty(loginResult.Token))
            {
                Console.WriteLine("Login successfull!");
                var logBookManager = new LogBookManager(AutoPiApiClient.Client, logger);

                Console.WriteLine("Attempting to retreive the list logbook fields");
                var fields = await logBookManager.GetStorageFields();
                foreach (var field in fields)
                {
                    Console.WriteLine("{1} - {0}", field.Field, field.Type);
                }

                Console.WriteLine("Attempting to retreive the list of dongles");

                var dm = new DongleManager(AutoPiApiClient.Client, logger);

                var dongles = await dm.GetDongleDevices();

                if (dongles == null)
                {
                    Console.WriteLine("No dongles found.");
                    return;
                }
                foreach (var dongle in dongles)
                {

                    Console.WriteLine("Call Name:{0}, Display:{1}, Last Communication:{2}", dongle.CallName, dongle.Display, dongle.LastCommunication);
                    if (dongle.Vehicle != null)
                    {
                        Console.WriteLine("\tVehicle Call Name:{0}, Vin:{1}", dongle.Vehicle.CallName, dongle.Vehicle.Vin);
                    }
                    Console.WriteLine("Attempting to retreive the recent stats");
                    var recentStats = await logBookManager.GetRecentStats(dongle.Id);
                    if (recentStats != null)
                    {
                        Console.WriteLine("Recent Status:\n\tLast Comm:{0}\n\tVoltage:{1} at {2}\n\tVoltage Level:{3} at {4}\n\t",
                        recentStats.LatestCom,
                        recentStats.Voltage,
                        recentStats.VoltageTs,
                        recentStats.VoltageLevel,
                        recentStats.VoltageLevelTs
                        );
                    }

                    Console.WriteLine("Attempting to retreive the list of trips");
                    var trips = await logBookManager.GetTrips(dongle.Id);
                    if (trips == null)
                    {
                        Console.WriteLine("No trips were found for this device.");
                        continue;
                    }
                    foreach (var trip in trips)
                    {
                        Console.WriteLine("\t\tStart:{2} at {0}, End:{3} at {1}, Distance (km):{4:0.0}, Duration:{5}", trip.StartTimeUtc, trip.EndTimeUtc, trip.StartDisplay, trip.EndDisplay, trip.DistanceKm, trip.DurationTS);
                        var tags = await metaDataStorage.GetTagsForEntity(trip.Id);
                        if (tags != null)
                        {
                            Console.WriteLine("\t\t\tTags:{0}", string.Join(',', tags));
                        }

                        var tripDataPoints = await logBookManager.GetStorageData(dongle.Id, trip.StartTimeUtc, trip.EndTimeUtc);
                        var existingTags = await metaDataStorage.GetTagsForEntity(trip.Id);
                        if (existingTags == null) existingTags = new List<string>();
                        if (tripDataPoints != null)
                        {
                            foreach (var point in tripDataPoints)
                            {
                                foreach (var fence in geoFences)
                                {
                                    if (!existingTags.Contains(fence.Name))
                                    {
                                        if (fenceService.PolyContainsPoint(fence, point.Location))
                                        {
                                            Console.WriteLine("--->Geofence triggered: Where:{0} (From:{1} To:{2})", fence.Name, trip.StartDisplay, trip.EndDisplay);
                                            await metaDataStorage.TagEntity(trip.Id, fence.Name);
                                            existingTags.Add(fence.Name);
                                        }
                                    }
                                }
                            }
                        }

                    }

                    //var readFuelLevel = await logBookManager.GetStorageRead(dongle.Id, DateTime.Now.AddHours(-24), field: "obd.fuel_level.value");
                    //var raw = await logBookManager.GetStorageRaw(dongle.Id, DateTime.Now.AddHours(-24));
                    //var position = await logBookManager.GetStorageRaw(dongle.Id, DateTime.Now.AddHours(-24), field: "track.pos.loc");

                }



            }
        }
        static IReadOnlyCollection<Boundary> TestPoly()
        {
            var fence = new Boundary()
            {
                Bounds = new List<Location>() {
                new Location { Lat = 0, Lon = 0},
                new Location { Lat = 10, Lon = 0},
                new Location { Lat = 10, Lon = 10},
                new Location { Lat = 0, Lon = 10},
            }
            };
            var pointInFence = new core.Models.Location() { Lat = 5, Lon = 5 };
            var pointOutOfFence = new Location() { Lat = 50, Lon = 5 };

            var geofenceService = new PolygonGeoFence();
            var contains = geofenceService.PolyContainsPoint(fence, pointInFence);
            var NotContains = geofenceService.PolyContainsPoint(fence, pointOutOfFence);


            try
            {
                IReadOnlyCollection<Boundary> geoFences = null;
                var geoFenceFile = "../../autopi.net.boundaries.json";
                if (System.IO.File.Exists(geoFenceFile))
                {
                    geoFences = JsonConvert.DeserializeObject<IReadOnlyCollection<Boundary>>(System.IO.File.ReadAllText(geoFenceFile));
                    return geoFences;
                }
            }
            catch
            {

            }
            return null;

        }

    }
}
