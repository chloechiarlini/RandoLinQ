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
    Console.WriteLine("=== Toutes les randonnées ===");
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

    var resultats = (from r in randos
                     where string.Equals(r.difficulte, diff, StringComparison.OrdinalIgnoreCase)
                     select r).ToList();

    Console.WriteLine($"Randonnées avec difficulté \"{diff}\" : ");

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
    var longues = (from r in randos
                   where r.distance_km > 20
                   select r).ToList();

    Console.WriteLine("=== Randonnées de plus de 20 km === ");
    foreach (var r in longues)
        Console.WriteLine($"{r.nom} - {r.distance_km} km");

    DemanderExport(longues);
}

void TrierParDistance()
{
    var tri = (from r in randos
               orderby r.distance_km
               select r).ToList();

    Console.WriteLine("=== Randonnées triées par distance (croissante) === ");
    foreach (var r in tri)
        Console.WriteLine($"{r.nom} - {r.distance_km} km");

    DemanderExport(tri);
}

void GrouperParDifficulte()
{
    var groupes = from r in randos
                  group r by r.difficulte into g
                  orderby g.Key
                  select g;

    Console.WriteLine("=== Groupement par difficulté === ");
    foreach (var g in groupes)
    {
        Console.WriteLine($"Difficulté : {g.Key} ({g.Count()} randos)");
        foreach (var r in g)
            Console.WriteLine($" - {r.nom} ({r.distance_km} km)");
        Console.WriteLine();
    }

    var liste = (from g in groupes
                 from r in g
                 select r).ToList();
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
    try
    {
        Console.WriteLine("Quels champs voulez-vous inclure dans l'export ?");
        Console.WriteLine("Tapez oui/non pour chaque champ :");

        Console.Write("Inclure le nom ? (oui/non) : ");
        bool Nom = Console.ReadLine()?.Trim().ToLower() == "oui";

        Console.Write("Inclure la distance (km) ? (oui/non) : ");
        bool Distance = Console.ReadLine()?.Trim().ToLower() == "oui";

        Console.Write("Inclure la difficulté ? (oui/non) : ");
        bool Difficulte = Console.ReadLine()?.Trim().ToLower() == "oui";

        Console.Write("Inclure les coordonnées ? (oui/non) : ");
        bool Coord = Console.ReadLine()?.Trim().ToLower() == "oui";

        string fileName = $"export-{DateTime.Now:yyyyMMdd-HHmmss}.csv";
        string currentDir = Directory.GetCurrentDirectory();
        Console.WriteLine($"Dossier actuel: {currentDir}");

        string exportPath = Path.Combine(currentDir, "Exports");
        Console.WriteLine($"Création du dossier d'export : {exportPath}");

        if (!Directory.Exists(exportPath))
        {
            Directory.CreateDirectory(exportPath);
            Console.WriteLine("Dossier Exports créé");
        }

        string fullPath = Path.Combine(exportPath, fileName);
        Console.WriteLine($"Chemin complet du fichier : {fullPath}");

        using (var writer = new StreamWriter(fullPath))
        {
            var header = new List<string>();
            if (Nom) header.Add("Nom");
            if (Distance) header.Add("Distance_km");
            if (Difficulte) header.Add("Difficulté");
            if (Coord) header.Add("Latitude,Longitude");

            writer.WriteLine(string.Join(";", header));

            foreach (var r in liste)
            {
                var row = new List<string>();
                if (Nom) row.Add(r.nom);
                if (Distance) row.Add(r.distance_km.ToString());
                if (Difficulte) row.Add(r.difficulte);
                if (Coord) row.Add($"{r.geo_point_2d?.lat},{r.geo_point_2d?.lon}");

                writer.WriteLine(string.Join(";", row));
            }
            Console.WriteLine("Données écrites");
        }

        Console.WriteLine($"Export terminé : {fullPath}");
        if (File.Exists(fullPath))
        {
            Console.WriteLine("Le fichier a bien été créé");
        }
        else
        {
            Console.WriteLine("ERREUR : Le fichier n'a pas été créé");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERREUR lors de l'export : {ex.Message}");
    }
}
