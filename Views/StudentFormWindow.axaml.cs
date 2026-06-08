using Avalonia.Controls;
using Avalonia.Interactivity;
using StudentEvidence.Models;
using StudentEvidence.Services;

namespace StudentEvidence.Views
{
    public partial class StudentFormWindow : Window
    {
        private readonly StudentService _service;
        private readonly Student? _editovany; // null = přidáváme nového

        public StudentFormWindow(StudentService service, Student? editovany)
        {
            InitializeComponent();
            _service = service;
            _editovany = editovany;

            BtnUlozit.Click += BtnUlozit_Click;
            BtnZrusit.Click += (_, _) => Close();

            // Pokud editujeme, předvyplníme pole
            if (editovany != null)
            {
                Titulek.Text = "Upravit studenta";
                JmenoBox.Text = editovany.Jmeno;
                PrijmeniBox.Text = editovany.Prijmeni;
                TridaBox.Text = editovany.Trida;
            }
        }

        private void BtnUlozit_Click(object? sender, RoutedEventArgs e)
        {
            // Validace - zkontroluje každé pole
            bool ok = true;

            var (jmenoOk, jmenoChyba) = Validace.OverJmeno(JmenoBox.Text ?? "");
            JmenoChyba.Text = jmenoChyba;
            if (!jmenoOk) ok = false;

            var (prijmeniOk, prijmeniChyba) = Validace.OverJmeno(PrijmeniBox.Text ?? "");
            PrijmeniChyba.Text = prijmeniChyba;
            if (!prijmeniOk) ok = false;

            var (tridaOk, tridaChyba) = Validace.OverTridu(TridaBox.Text ?? "");
            TridaChyba.Text = tridaChyba;
            if (!tridaOk) ok = false;

            if (!ok) return; // Zastav, pokud je chyba

            if (_editovany == null)
            {
                // Přidáme nového studenta
                var novy = new Student
                {
                    Jmeno = JmenoBox.Text!.Trim(),
                    Prijmeni = PrijmeniBox.Text!.Trim(),
                    Trida = TridaBox.Text!.Trim()
                };
                _service.PridejStudenta(novy);
            }
            else
            {
                // Upravíme existujícího
                _editovany.Jmeno = JmenoBox.Text!.Trim();
                _editovany.Prijmeni = PrijmeniBox.Text!.Trim();
                _editovany.Trida = TridaBox.Text!.Trim();
                _service.UpravStudenta(_editovany);
            }

            Close();
        }
    }
}
