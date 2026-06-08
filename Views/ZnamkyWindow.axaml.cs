using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Layout;
using StudentEvidence.Models;
using StudentEvidence.Services;
using System.Linq;

namespace StudentEvidence.Views
{
    public partial class ZnamkyWindow : Window
    {
        private readonly StudentService _service;
        private readonly int _studentId;
        private Student _student;

        public ZnamkyWindow(StudentService service, int studentId)
        {
            InitializeComponent();
            _service = service;
            _studentId = studentId;
            _student = _service.VratStudenty().First(s => s.Id == studentId);

            BtnPridatZnamku.Click += BtnPridat_Click;
            BtnZavrit.Click += (_, _) => Close();

            ObnovZobrazeni();
        }

        private void ObnovZobrazeni()
        {
            // Znovu načti aktuální data studenta
            _student = _service.VratStudenty().First(s => s.Id == _studentId);

            JmenoLabel.Text = $"📝 {_student.CeleJmeno} — Třída: {_student.Trida}";
            double prumer = _student.VypoctiPrumer();
            PrumerLabel.Text = prumer > 0 ? $"Celkový průměr: {prumer:F2}" : "Žádné známky";

            // Sestavíme seznam předmětů
            var panely = _student.Znamky.Keys.Select(predmet => VytvoriPredmetPanel(predmet)).ToList();
            PredmetyList.ItemsSource = panely;

            int pocetZnamek = _student.Znamky.Values.Sum(z => z.Count);
            InfoText.Text = $"Předmětů: {_student.Znamky.Count}  |  Celkem známek: {pocetZnamek}";
        }

        // Vytvoří vizuální panel pro jeden předmět se všemi jeho známkami
        private Border VytvoriPredmetPanel(string predmet)
        {
            var znamky = _student.Znamky[predmet];
            double prumerPredmet = _student.VypoctiPrumerPredmetu(predmet);

            var border = new Border
            {
                Background = new SolidColorBrush(Color.Parse("#313244")),
                CornerRadius = new Avalonia.CornerRadius(8),
                Padding = new Avalonia.Thickness(16, 12),
                Margin = new Avalonia.Thickness(0, 0, 0, 8)
            };

            var stack = new StackPanel { Spacing = 8 };

            // Řádek s názvem předmětu a průměrem
            var hlavicka = new Grid();
            hlavicka.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            hlavicka.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            hlavicka.Children.Add(new TextBlock
            {
                Text = predmet,
                FontSize = 14,
                FontWeight = FontWeight.SemiBold,
                Foreground = new SolidColorBrush(Color.Parse("#CDD6F4"))
            });

            var prumerText = new TextBlock
            {
                Text = $"⌀ {prumerPredmet:F2}",
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.Parse("#F9E2AF")),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(prumerText, 1);
            hlavicka.Children.Add(prumerText);
            stack.Children.Add(hlavicka);

            // Řádek se známkami jako tlačítka (kliknutím smažeš)
            var znamkyPanel = new WrapPanel { Spacing = 6 };
            for (int i = 0; i < znamky.Count; i++)
            {
                int index = i; // Zachycení pro lambda
                int hodnota = znamky[i];

                string barva = hodnota <= 2 ? "#A6E3A1" : hodnota <= 3 ? "#F9E2AF" : "#F38BA8";

                var btnZnamka = new Button
                {
                    Content = hodnota.ToString(),
                    Background = new SolidColorBrush(Color.Parse(barva)),
                    Foreground = new SolidColorBrush(Color.Parse("#1E1E2E")),
                    FontSize = 15,
                    FontWeight = FontWeight.Bold,
                    Width = 38,
                    Height = 38,
                    CornerRadius = new Avalonia.CornerRadius(6),
                    ToolTip = { }
                };
                ToolTip.SetTip(btnZnamka, "Klikni pro smazání");
                btnZnamka.Click += (_, _) =>
                {
                    _service.SmazZnamku(_studentId, predmet, index);
                    ObnovZobrazeni();
                };
                znamkyPanel.Children.Add(btnZnamka);
            }
            stack.Children.Add(znamkyPanel);

            if (znamky.Count == 0)
            {
                stack.Children.Add(new TextBlock
                {
                    Text = "Žádné známky",
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Color.Parse("#6C7086"))
                });
            }

            border.Child = stack;
            return border;
        }

        private void BtnPridat_Click(object? sender, RoutedEventArgs e)
        {
            ChybaText.Text = "";

            // Validace předmětu
            var (predmetOk, predmetChyba) = Validace.OverPredmet(PredmetBox.Text ?? "");
            if (!predmetOk) { ChybaText.Text = predmetChyba; return; }

            // Validace známky
            var (znamkaOk, znamkaChyba) = Validace.OverZnamku(ZnamkaBox.Text ?? "");
            if (!znamkaOk) { ChybaText.Text = znamkaChyba; return; }

            int hodnota = int.Parse(ZnamkaBox.Text!);
            _service.PridejZnamku(_studentId, PredmetBox.Text!.Trim(), hodnota);

            ZnamkaBox.Text = "";
            ObnovZobrazeni();
        }
    }
}
