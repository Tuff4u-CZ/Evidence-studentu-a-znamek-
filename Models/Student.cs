using System.Collections.Generic;
using System.Linq;

namespace StudentEvidence.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Jmeno { get; set; } = "";
        public string Prijmeni { get; set; } = "";
        public string Trida { get; set; } = "";

        // Klíč = název předmětu, hodnota = seznam známek
        public Dictionary<string, List<int>> Znamky { get; set; } = new();

        // Vypočítá průměr ze všech předmětů
        public double VypoctiPrumer()
        {
            var vsechnyZnamky = Znamky.Values.SelectMany(z => z).ToList();
            if (vsechnyZnamky.Count == 0) return 0;
            return vsechnyZnamky.Average();
        }

        // Vypočítá průměr pro konkrétní předmět
        public double VypoctiPrumerPredmetu(string predmet)
        {
            if (!Znamky.ContainsKey(predmet) || Znamky[predmet].Count == 0)
                return 0;
            return Znamky[predmet].Average();
        }

        // Přidá známku k předmětu
        public void PridejZnamku(string predmet, int znamka)
        {
            if (!Znamky.ContainsKey(predmet))
                Znamky[predmet] = new List<int>();
            Znamky[predmet].Add(znamka);
        }

        // Vrátí jméno + příjmení
        public string CeleJmeno => $"{Jmeno} {Prijmeni}";

        public override string ToString() => CeleJmeno;
    }
}
