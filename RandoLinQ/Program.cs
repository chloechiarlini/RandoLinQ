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

//1. Rechercher les randos avec une distance > 20 km :
var resultatsDistance = from r in randos
                        where r.distance_km > 20
                        select r;

Console.WriteLine("Randonnées de plus de 20 km: ");
foreach (var r in resultatsDistance)
{
    Console.WriteLine($"{r.nom} - {r.distance_km} km - Difficulté: {r.difficulte}");
}

//2. Trier les randos par distance croissante :
var resultatsDistanceCroissante = from r in randos
                                  orderby r.distance_km
                                  select r;

Console.WriteLine("Randonnées triées par distance (croissant): ");
foreach (var r in resultatsDistanceCroissante)
{
    Console.WriteLine($"{r.nom} - {r.distance_km} km");
}

//3. Grouper par difficulté :
var groupParDifficulte = from r in randos
                         group r by r.difficulte into g
                         select g;

Console.WriteLine("Randonnées groupées par difficulté :");
foreach (var groupe in groupParDifficulte)
{
    Console.WriteLine($"Difficulté : {groupe.Key} ({groupe.Count()} randonnées)");
    foreach (var r in groupe)
    {
        Console.WriteLine($" - {r.nom} ({r.distance_km} km)");
    }
    Console.WriteLine();
}





