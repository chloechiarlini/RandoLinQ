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
var resultatsDistance = randos
    .Where(r => r.distance_km > 20)
    .ToList();

Console.WriteLine("Randonnées de plus de 20 km: ");
foreach (var r in resultatsDistance)
{
    Console.WriteLine($"{r.nom} - {r.distance_km} km - Difficulté: {r.difficulte}");
}

//3. Trier les randos par distance croissante :
var resultatsDistanceCroissante = randos
    .OrderBy(r => r.distance_km)
    .ToList();

Console.WriteLine("Randonnées triées par distance (croissant): ");
foreach (var r in resultatsDistanceCroissante)
{
    Console.WriteLine($"{r.nom} - {r.distance_km} km");
}

//4. Trier par date :
var triDate = randos
    .OrderBy(r => r.date)
    .ToList();

Console.WriteLine("Randonnées triées par date : ");
foreach (var r in triDate)
{
    Console.WriteLine($"{r.nom} - {r.date} - {r.distance_km} km");
}

//5. Grouper par difficulté :
var groupParDifficulte = randos
    .GroupBy(r => r.difficulte);

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





