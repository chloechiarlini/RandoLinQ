using System.IO;
using Newtonsoft.Json;
using System.Linq;
using RandoLinQ;

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
    string choix = Console.ReadLine();

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

static void ListerToutesLesRandos()
{
    Console.WriteLine("=== Toutes les randonnées ===\n");
    foreach (var r in randos)
    {
        Console.WriteLine($"{r.nom} - {r.distance_km} km - Difficulté : {r.difficulte}");
    }
}

static void RechercherParDifficulte()
{
    Console.WriteLine("Entrez la difficulté à rechercher (ex: *, **, ***, Facile, Moyen, Difficile) : ");
    string diff = Console.ReadLine();

    var resultats = from r in randos
                    where r.difficulte?.ToLower() == diff?.ToLower()
                    select r;

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
}

static void AfficherRandosLongues()
{
    var longues = from r in randos
                  where r.distance_km > 20
                  select r;

    Console.WriteLine("=== Randonnées de plus de 20 km ===\n");
    foreach (var r in longues)
        Console.WriteLine($"{r.nom} - {r.distance_km} km");
}

static void TrierParDistance()
{
    var tri = from r in randos
              orderby r.distance_km
              select r;

    Console.WriteLine("=== Randonnées triées par distance (croissante) ===\n");
    foreach (var r in tri)
        Console.WriteLine($"{r.nom} - {r.distance_km} km");
}

static void GrouperParDifficulte()
{
    var groupes = from r in randos
                  group r by r.difficulte into g
                  orderby g.Key
                  select g;

    Console.WriteLine("=== Groupement par difficulté ===\n");
    foreach (var g in groupes)
    {
        Console.WriteLine($"Difficulté : {g.Key} ({g.Count()} randos)");
        foreach (var r in g)
            Console.WriteLine($" - {r.nom} ({r.distance_km} km)");
        Console.WriteLine();
    }
}
