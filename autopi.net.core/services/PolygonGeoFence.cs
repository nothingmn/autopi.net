using System.Collections.Generic;
using autopi.net.core.Models;

namespace autopi.net.core.services
{
    //shamelessly stolen from
    //https://stackoverflow.com/questions/924171/geo-fencing-point-inside-outside-polygon/924179#924179
    public class PolygonGeoFence
    {

        public bool PolyContainsPoint(Boundary boundary, Location p)
        {
            bool inside = false;

            // An imaginary closing segment is implied,
            // so begin testing with that.
            var v1 = boundary.Bounds[boundary.Bounds.Count - 1];

            foreach (var v0 in boundary.Bounds)
            {
                double d1 = (p.Lon - v0.Lon) * (v1.Lat - v0.Lat);
                double d2 = (p.Lat - v0.Lat) * (v1.Lon - v0.Lon);

                if (p.Lon < v1.Lon)
                {
                    // V1 below ray
                    if (v0.Lon <= p.Lon)
                    {
                        // V0 on or above ray
                        // Perform intersection test
                        if (d1 > d2)
                        {
                            inside = !inside; // Toggle state
                        }
                    }
                }
                else if (p.Lon < v0.Lon)
                {
                    // V1 is on or above ray, V0 is below ray
                    // Perform intersection test
                    if (d1 < d2)
                    {
                        inside = !inside; // Toggle state
                    }
                }

                v1 = v0; //Store previous endpoint as next startpoint
            }

            return inside;
        }
    }
}