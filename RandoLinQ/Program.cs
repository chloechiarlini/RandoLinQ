using System.IO;
using Newtonsoft.Json;
using System.Linq;
using RandoLinQ;

string json = File.ReadAllText(@$"{Directory.GetCurrentDirectory()}/Data/randonnees-velos.json");

var randos = JsonConvert.DeserializeObject<List<Trek>>(json) ?? new List<Trek>();

foreach (var r in randos)
{
    Console.WriteLine($"{r.nom} - {r.distance_km} km - Difficulté : {r.difficulte}");
    Console.WriteLine($"Coordonnées : {r.geo_point_2d.lat}, {r.geo_point_2d.lon}");
    Console.WriteLine("-----------------------------------");
    Console.WriteLine("");
}