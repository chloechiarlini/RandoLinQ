using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandoLinQ
{
    public class GeoPoint
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Randonnee
    {
        public GeoPoint geo_point_2d { get; set; }
        public string nom { get; set; }
        public string date { get; set; }
        public double distance_km { get; set; }
        public string difficulte { get; set; }
        public string plus_infos { get; set; }
    }

}
