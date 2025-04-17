using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RandoLinQ;

List<Trek> randos = new();

string json = File.ReadAllText(@$"{Directory.GetCurrentDirectory()}/Data/randonnees-velos.json");
randos = JsonConvert.DeserializeObject<List<Trek>>(json) ?? new List<Trek>();

while (true)
{
    Console.Clear();
    Console.WriteLine("=== MENU RANDONNÉES ===");
    Console.WriteLine("1. Lister toutes les randonnées");
    Console.WriteLine("2. Rechercher par difficulté");
    Console.WriteLine("3. Afficher les randonnées > 20 km");
    Console.WriteLine("4. Trier par distance croissante");
    Console.WriteLine("5. Grouper par difficulté");
    Console.WriteLine("0. Quitter");
    Console.Write("\nVotre choix : ");
    string choix = Console.ReadLine()?.Trim();

    Console.Clear();

    switch (choix)
    {
        case "1":
            ListerToutesLesRandos();
            break;
        case "2":
            RechercherParDifficulte();
            break;
        case "3":
            AfficherRandosLongues();
            break;
        case "4":
            TrierParDistance();
            break;
        case "5":
            GrouperParDifficulte();
            break;
        case "0":
            Console.WriteLine("À bientôt !");
            return;
        default:
            Console.WriteLine("Choix invalide.");
            break;
    }

    Console.WriteLine("\nAppuyez sur une touche pour revenir au menu...");
    Console.ReadKey();
}

void ListerToutesLesRandos()
{
    Console.WriteLine("=== Toutes les randonnées ===\n");
    foreach (var r in randos)
    {
        Console.WriteLine($"{r.nom} - {r.distance_km} km - Difficulté : {r.difficulte}");
    }

    DemanderExport(randos);
}

void RechercherParDifficulte()
{
    Console.Write("Entrez la difficulté à rechercher (ex: *, **, ***, Facile, Moyen, Difficile) : ");
    string? diff = Console.ReadLine()?.Trim();

    var resultats = randos
        .Where(r => string.Equals(r.difficulte, diff, StringComparison.OrdinalIgnoreCase))
        .ToList();

    Console.WriteLine($"\nRandonnées avec difficulté \"{diff}\" :\n");

    if (!resultats.Any())
    {
        Console.WriteLine("Aucune randonnée trouvée pour cette difficulté.");
    }
    else
    {
        foreach (var r in resultats)
            Console.WriteLine($"{r.nom} - {r.distance_km} km");
    }

    DemanderExport(resultats);
}

void AfficherRandosLongues()
{
    var longues = randos.Where(r => r.distance_km > 20).ToList();

    Console.WriteLine("=== Randonnées de plus de 20 km ===\n");
    foreach (var r in longues)
        Console.WriteLine($"{r.nom} - {r.distance_km} km");

    DemanderExport(longues);
}

void TrierParDistance()
{
    var tri = randos.OrderBy(r => r.distance_km).ToList();

    Console.WriteLine("=== Randonnées triées par distance (croissante) ===\n");
    foreach (var r in tri)
        Console.WriteLine($"{r.nom} - {r.distance_km} km");

    DemanderExport(tri);
}

void GrouperParDifficulte()
{
    var groupes = randos
        .GroupBy(r => r.difficulte)
        .OrderBy(g => g.Key);

    Console.WriteLine("=== Groupement par difficulté ===\n");
    foreach (var g in groupes)
    {
        Console.WriteLine($"Difficulté : {g.Key} ({g.Count()} randos)");
        foreach (var r in g)
            Console.WriteLine($" - {r.nom} ({r.distance_km} km)");
        Console.WriteLine();
    }

    var liste = groupes.SelectMany(g => g).ToList();
    DemanderExport(liste);
}

void DemanderExport(List<Trek> liste)
{
    if (liste == null || !liste.Any())
    {
        Console.WriteLine("Aucune donnée à exporter.");
        return;
    }

    Console.WriteLine("\nSouhaitez-vous exporter ces randonnées ? (oui/non) : ");
    if (Console.ReadLine()?.Trim().ToLower() == "oui")
    {
        ProposerExport(liste);
    }
    else
    {
        Console.WriteLine("Export annulé.");
    }
}

void ProposerExport(List<Trek> liste)
{
    Console.WriteLine("\nQuels champs voulez-vous inclure dans l’export ?");
    Console.WriteLine("Tapez oui/non pour chaque champ :");

    Console.Write("Inclure le nom ? (oui/non) : ");
    bool inclNom = Console.ReadLine()?.Trim().ToLower() == "oui";

    Console.Write("Inclure la distance (km) ? (oui/non) : ");
    bool inclDistance = Console.ReadLine()?.Trim().ToLower() == "oui";

    Console.Write("Inclure la difficulté ? (oui/non) : ");
    bool inclDifficulte = Console.ReadLine()?.Trim().ToLower() == "oui";

    Console.Write("Inclure les coordonnées ? (oui/non) : ");
    bool inclCoord = Console.ReadLine()?.Trim().ToLower() == "oui";

    string fileName = $"export-{DateTime.Now:yyyyMMdd-HHmmss}.csv";
    string exportPath = Path.Combine(Directory.GetCurrentDirectory(), "Exports");

    Directory.CreateDirectory(exportPath);
    string fullPath = Path.Combine(exportPath, fileName);

    using (var writer = new StreamWriter(fullPath))
    {
        var header = new List<string>();
        if (inclNom) header.Add("Nom");
        if (inclDistance) header.Add("Distance_km");
        if (inclDifficulte) header.Add("Difficulté");
        if (inclCoord) header.Add("Latitude,Longitude");

        writer.WriteLine(string.Join(";", header));

        foreach (var r in liste)
        {
            var row = new List<string>();
            if (inclNom) row.Add(r.nom);
            if (inclDistance) row.Add(r.distance_km.ToString());
            if (inclDifficulte) row.Add(r.difficulte);
            if (inclCoord) row.Add($"{r.geo_point_2d?.lat},{r.geo_point_2d?.lon}");

            writer.WriteLine(string.Join(";", row));
        }
    }

    Console.WriteLine($"\nExport terminé : {Path.Combine("Exports", fileName)}");
}
