using System;

enum Periode { Dagelijks, Wekelijks, Maandelijks }

class Boek
{
    public string Isbn { get; set; }
    public string Naam { get; set; }
    public string Uitgever { get; set; }
    private decimal prijs;
    public decimal Prijs
    {
        get => prijs;
        set => prijs = value < 5 ? 5 : value > 50 ? 50 : value;
    }

    public Boek(string isbn, string naam, string uitgever, decimal prijs)
        => (Isbn, Naam, Uitgever, Prijs) = (isbn, naam, uitgever, prijs);

    public virtual void Lees()
    {
        Console.Write("ISBN: "); Isbn = Console.ReadLine();
        Console.Write("Naam: "); Naam = Console.ReadLine();
        Console.Write("Uitgever: "); Uitgever = Console.ReadLine();
        Console.Write("Prijs: "); Prijs = decimal.Parse(Console.ReadLine());
    }

    public override string ToString()
        => $"{Naam} (ISBN: {Isbn}), Uitgever: {Uitgever}, Prijs: €{Prijs}";
}

class Tijdschrift : Boek
{
    public Periode Verschijningsperiode { get; set; }
    public Tijdschrift(string isbn, string naam, string uitgever, decimal prijs, Periode periode)
        : base(isbn, naam, uitgever, prijs) => Verschijningsperiode = periode;

    public override void Lees()
    {
        base.Lees();
        Console.Write("Periode (0=Dagelijks, 1=Wekelijks, 2=Maandelijks): ");
        Verschijningsperiode = (Periode)int.Parse(Console.ReadLine());
    }

    public override string ToString()
        => $"{base.ToString()}, Periode: {Verschijningsperiode}";
}

class Bestelling<T>
{
    private static int teller;
    public int Id { get; }
    public T Item { get; }
    public DateTime Datum { get; }
    public int Aantal { get; }
    public Periode? AbonnementPeriode { get; }
    public event Action<string> Besteld;

    public Bestelling(T item, int aantal, Periode? abonnementPeriode = null)
        => (Id, Item, Aantal, Datum, AbonnementPeriode) = (++teller, item, aantal, DateTime.Now, abonnementPeriode);

    public (string isbn, int aantal, decimal totaal) Bestel()
    {
        if (Item is Boek b)
        {
            var totaal = b.Prijs * Aantal;
            string extra = "";
            if (AbonnementPeriode.HasValue)
                extra = $" (Abonnement: {AbonnementPeriode.Value})";
            Besteld?.Invoke($"EVENT: Bestelling {Id}: {Aantal}x {b.Naam} besteld (€{totaal}){extra}");
            return (b.Isbn, Aantal, totaal);
        }
        return ("", Aantal, 0);
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var b1 = new Boek("123", "C# Basics", "Uitgever A", 20);
        var t1 = new Tijdschrift("456", "Tech Mag", "Uitgever B", 5, Periode.Maandelijks);

        Console.WriteLine(b1);
        Console.WriteLine(t1);
        Console.WriteLine("\n--- Bestellingen ---");

        var bestelling1 = new Bestelling<Boek>(b1, 2);
        bestelling1.Besteld += Console.WriteLine;
        var tuple1 = bestelling1.Bestel();
        Console.WriteLine($"Datum: {bestelling1.Datum}");
        Console.WriteLine($"Tuple: ISBN={tuple1.isbn}, Aantal={tuple1.aantal}, Totaal=€{tuple1.totaal}\n");

        var bestelling2 = new Bestelling<Tijdschrift>(t1, 1, Periode.Maandelijks);
        bestelling2.Besteld += Console.WriteLine;
        bestelling2.Bestel();
        Console.WriteLine($"Datum: {bestelling2.Datum}");

        Console.WriteLine("\nDruk op Enter om af te sluiten...");
        Console.ReadLine();
    }
}