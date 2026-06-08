namespace StudentEvidence.Services
{
    public static class Validace
    {
        // Zkontroluje jméno - nesmí být prázdné, max 50 znaků
        public static (bool Ok, string Chyba) OverJmeno(string jmeno)
        {
            if (string.IsNullOrWhiteSpace(jmeno))
                return (false, "Jméno nesmí být prázdné.");
            if (jmeno.Trim().Length < 2)
                return (false, "Jméno musí mít aspoň 2 znaky.");
            if (jmeno.Length > 50)
                return (false, "Jméno může mít max 50 znaků.");
            return (true, "");
        }

        // Zkontroluje třídu - nesmí být prázdná
        public static (bool Ok, string Chyba) OverTridu(string trida)
        {
            if (string.IsNullOrWhiteSpace(trida))
                return (false, "Třída nesmí být prázdná.");
            if (trida.Length > 10)
                return (false, "Název třídy může mít max 10 znaků.");
            return (true, "");
        }

        // Zkontroluje předmět - nesmí být prázdný
        public static (bool Ok, string Chyba) OverPredmet(string predmet)
        {
            if (string.IsNullOrWhiteSpace(predmet))
                return (false, "Název předmětu nesmí být prázdný.");
            if (predmet.Length > 30)
                return (false, "Název předmětu může mít max 30 znaků.");
            return (true, "");
        }

        // Zkontroluje, zda je zadaná hodnota platná známka (1–5)
        public static (bool Ok, string Chyba) OverZnamku(string vstup)
        {
            if (!int.TryParse(vstup, out int znamka))
                return (false, "Zadej číslo (1–5).");
            if (znamka < 1 || znamka > 5)
                return (false, "Známka musí být mezi 1 a 5.");
            return (true, "");
        }
    }
}
