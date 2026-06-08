using Avalonia.Controls;
using Avalonia.Media;
using StudentEvidence.Services;
using System.Linq;

namespace StudentEvidence.Views
{
    public partial class StatistikyWindow : Window
    {
        public StatistikyWindow(StudentService service)
        {
            InitializeComponent();
            BtnZavrit.Click += (_, _) => Close();

            var studenti = service.VratStudenty();
            var (prumer, nejlepsi, nejhorsi) = service.StatistikySkolni();

            // Celkový počet studentů
            PridejKartu("👥 Počet studentů", studenti.Count.ToString(), "#89B4FA");

            // Celkový průměr
            PridejKartu("📈 Průměr třídy",
                prumer > 0 ? prumer.ToString("F2") : "Žádná data", "#A6E3A1");

            // Nejlepší student
            PridejKartu("🏆 Nejlepší student",
                nejlepsi != null ? $"{nejlepsi.CeleJmeno} ({nejlepsi.VypoctiPrumer():F2})" : "—",
                "#F9E2AF");

            // Nejhorší student
            PridejKartu("📉 Nejslabší student",
                nejhorsi != null ? $"{nejhorsi.CeleJmeno} ({nejhorsi.VypoctiPrumer():F2})" : "—",
                "#F38BA8");

            // Počet předmětů celkem
            var predmety = studenti
                .SelectMany(s => s.Znamky.Keys)
                .Distinct()
                .OrderBy(p => p)
                .ToList();
            PridejKartu("📚 Předměty", predmety.Count > 0 ? string.Join(", ", predmety) : "Žádné", "#CBA6F7");

            // Rozložení průměrů
            int vybornych = studenti.Count(s => s.VypoctiPrumer() > 0 && s.VypoctiPrumer() <= 1.5);
            int dobrych = studenti.Count(s => s.VypoctiPrumer() > 1.5 && s.VypoctiPrumer() <= 3.0);
            int slabych = studenti.Count(s => s.VypoctiPrumer() > 3.0);
            PridejKartu("🎯 Výborní (≤1.5) / Dobří / Slabí",
                $"{vybornych} / {dobrych} / {slabych}", "#94E2D5");
        }

        // Přidá jednu statistickou kartu do panelu
        private void PridejKartu(string nazev, string hodnota, string barva)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.Parse("#313244")),
                CornerRadius = new Avalonia.CornerRadius(8),
                Padding = new Avalonia.Thickness(16, 12)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            grid.Children.Add(new TextBlock
            {
                Text = nazev,
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.Parse("#A6ADC8")),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            });

            var hodnotaText = new TextBlock
            {
                Text = hodnota,
                FontSize = 15,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Foreground = new SolidColorBrush(Color.Parse(barva)),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                TextAlignment = Avalonia.Media.TextAlignment.Right
            };
            Grid.SetColumn(hodnotaText, 1);
            grid.Children.Add(hodnotaText);

            border.Child = grid;
            StatsPanel.Children.Add(border);
        }
    }
}
