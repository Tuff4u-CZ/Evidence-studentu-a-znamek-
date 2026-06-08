using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StudentEvidence.Models;

namespace StudentEvidence.Services
{
    public class DataService
    {
        // Cesta do AppData složky aplikace
        private readonly string _appDataPath;
        private readonly string _dataSoubor;

        public DataService()
        {
            // Vytvoří složku v AppData/Roaming/StudentEvidence
            _appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "StudentEvidence"
            );
            Directory.CreateDirectory(_appDataPath);
            _dataSoubor = Path.Combine(_appDataPath, "studenti.txt");
        }

        // Uloží seznam studentů do TXT souboru v AppData
        public void UlozStudenty(List<Student> studenti)
        {
            var sb = new StringBuilder();
            foreach (var s in studenti)
            {
                // Formát řádku: ID|Jmeno|Prijmeni|Trida
                sb.AppendLine($"STUDENT|{s.Id}|{s.Jmeno}|{s.Prijmeni}|{s.Trida}");
                // Každý předmět na nový řádek: PREDMET|StudentId|NazevPredmetu|1,2,3,4
                foreach (var kv in s.Znamky)
                {
                    string znamkyStr = string.Join(",", kv.Value);
                    sb.AppendLine($"PREDMET|{s.Id}|{kv.Key}|{znamkyStr}");
                }
            }
            File.WriteAllText(_dataSoubor, sb.ToString(), Encoding.UTF8);
        }

        // Načte studenty z TXT souboru
        public List<Student> NactiStudenty()
        {
            var studenti = new List<Student>();
            if (!File.Exists(_dataSoubor)) return studenti;

            var radky = File.ReadAllLines(_dataSoubor, Encoding.UTF8);
            foreach (var radek in radky)
            {
                if (string.IsNullOrWhiteSpace(radek)) continue;

                var casti = radek.Split('|');

                if (casti[0] == "STUDENT" && casti.Length == 5)
                {
                    studenti.Add(new Student
                    {
                        Id = int.Parse(casti[1]),
                        Jmeno = casti[2],
                        Prijmeni = casti[3],
                        Trida = casti[4]
                    });
                }
                else if (casti[0] == "PREDMET" && casti.Length == 4)
                {
                    int studentId = int.Parse(casti[1]);
                    string predmet = casti[2];
                    var student = studenti.Find(s => s.Id == studentId);
                    if (student != null && !string.IsNullOrEmpty(casti[3]))
                    {
                        student.Znamky[predmet] = new List<int>();
                        foreach (var z in casti[3].Split(','))
                        {
                            if (int.TryParse(z, out int znamka))
                                student.Znamky[predmet].Add(znamka);
                        }
                    }
                }
            }
            return studenti;
        }

        // Exportuje seznam do uživatelem zvoleného TXT souboru
        public void ExportujDoTxt(List<Student> studenti, string cesta)
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("       EVIDENCE STUDENTŮ A ZNÁMEK      ");
            sb.AppendLine($"  Exportováno: {DateTime.Now:dd.MM.yyyy HH:mm}  ");
            sb.AppendLine("========================================");
            sb.AppendLine();

            foreach (var s in studenti)
            {
                sb.AppendLine($"Jméno: {s.CeleJmeno}  |  Třída: {s.Trida}");
                if (s.Znamky.Count == 0)
                {
                    sb.AppendLine("  Žádné známky");
                }
                else
                {
                    foreach (var kv in s.Znamky)
                    {
                        string znamky = string.Join(", ", kv.Value);
                        sb.AppendLine($"  {kv.Key}: {znamky}  (průměr: {s.VypoctiPrumerPredmetu(kv.Key):F2})");
                    }
                    sb.AppendLine($"  Celkový průměr: {s.VypoctiPrumer():F2}");
                }
                sb.AppendLine("----------------------------------------");
            }
            File.WriteAllText(cesta, sb.ToString(), Encoding.UTF8);
        }

        public string AppDataCesta => _appDataPath;
    }
}
