using System.Collections.Generic;
using System.Linq;
using StudentEvidence.Models;

namespace StudentEvidence.Services
{
    public class StudentService
    {
        private List<Student> _studenti = new();
        private readonly DataService _dataService;
        private int _nextId = 1;

        public StudentService(DataService dataService)
        {
            _dataService = dataService;
            // Načte studenty ze souboru při spuštění
            _studenti = _dataService.NactiStudenty();
            if (_studenti.Count > 0)
                _nextId = _studenti.Max(s => s.Id) + 1;
        }

        // Vrátí kopii seznamu všech studentů
        public List<Student> VratStudenty() => new(_studenti);

        // Přidá nového studenta + uloží
        public void PridejStudenta(Student student)
        {
            student.Id = _nextId++;
            _studenti.Add(student);
            Uloz();
        }

        // Smaže studenta podle ID + uloží
        public void SmazStudenta(int id)
        {
            _studenti.RemoveAll(s => s.Id == id);
            Uloz();
        }

        // Upraví existujícího studenta + uloží
        public void UpravStudenta(Student upraveny)
        {
            var index = _studenti.FindIndex(s => s.Id == upraveny.Id);
            if (index >= 0)
            {
                _studenti[index] = upraveny;
                Uloz();
            }
        }

        // Přidá známku studentovi + uloží
        public void PridejZnamku(int studentId, string predmet, int znamka)
        {
            var student = _studenti.FirstOrDefault(s => s.Id == studentId);
            student?.PridejZnamku(predmet, znamka);
            Uloz();
        }

        // Smaže konkrétní známku (podle indexu v seznamu)
        public void SmazZnamku(int studentId, string predmet, int indexZnamky)
        {
            var student = _studenti.FirstOrDefault(s => s.Id == studentId);
            if (student != null && student.Znamky.ContainsKey(predmet))
            {
                student.Znamky[predmet].RemoveAt(indexZnamky);
                if (student.Znamky[predmet].Count == 0)
                    student.Znamky.Remove(predmet);
                Uloz();
            }
        }

        // Filtruje studenty podle jména nebo třídy
        public List<Student> Filtruj(string hledani)
        {
            if (string.IsNullOrWhiteSpace(hledani)) return VratStudenty();
            string h = hledani.ToLower();
            return _studenti
                .Where(s =>
                    s.CeleJmeno.ToLower().Contains(h) ||
                    s.Trida.ToLower().Contains(h))
                .ToList();
        }

        // Statistiky třídy (bonus)
        public (double Prumer, Student? Nejlepsi, Student? Nejhorsi) StatistikySkolni()
        {
            var sZnamkami = _studenti.Where(s => s.Znamky.Count > 0).ToList();
            if (sZnamkami.Count == 0) return (0, null, null);

            double prumer = sZnamkami.Average(s => s.VypoctiPrumer());
            var nejlepsi = sZnamkami.MinBy(s => s.VypoctiPrumer());
            var nejhorsi = sZnamkami.MaxBy(s => s.VypoctiPrumer());
            return (prumer, nejlepsi, nejhorsi);
        }

        // Uloží vše do AppData
        public void Uloz() => _dataService.UlozStudenty(_studenti);

        // Export do TXT
        public void ExportujTxt(string cesta) =>
            _dataService.ExportujDoTxt(_studenti, cesta);

        public string AppDataCesta => _dataService.AppDataCesta;
    }
}
