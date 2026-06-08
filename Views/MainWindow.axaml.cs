using Avalonia.Controls;
using Avalonia.Interactivity;
using StudentEvidence.Models;
using StudentEvidence.Services;
using System.Collections.Generic;
using System.Linq;

namespace StudentEvidence.Views
{
    public partial class MainWindow : Window
    {
        private readonly StudentService _service;
        private List<Student> _zobrazeni = new();

        public MainWindow()
        {
            InitializeComponent();
            var dataService = new DataService();
            _service = new StudentService(dataService);

            // Napojení událostí
            HledaniBox.TextChanged += (_, _) => ObnovSeznam();
            BtnPridat.Click += BtnPridat_Click;
            BtnStatistiky.Click += BtnStatistiky_Click;
            BtnExport.Click += BtnExport_Click;

            ObnovSeznam();
        }

        // Obnoví seznam studentů (filtruje podle hledaného textu)
        private void ObnovSeznam()
        {
            string hledani = HledaniBox.Text ?? "";
            _zobrazeni = _service.Filtruj(hledani);

            // Vytvoří vizuální položky pro každého studenta
            var polozky = _zobrazeni.Select(student => CreateStudentItem(student)).ToList();
            StudentiList.ItemsSource = null;
            StudentiList.ItemsSource = polozky;

            // Aktualizace info textu
            var vsichni = _service.VratStudenty();
            InfoText.Text = $"AppData: {_service.AppDataCesta}";
            StatusText.Text = $"Zobrazeno {_zobrazeni.Count} z {vsichni.Count} studentů";
        }

        // Vytvoří panel pro jednoho studenta
        private Border CreateStudentItem(Student student)
        {
            double prumer = student.VypoctiPrumer();
            string prumerText = prumer > 0 ? prumer.ToString("F1") : "—";
            string barvaPromer = prumer > 0 && prumer <= 2.0 ? "#A6E3A1" : prumer <= 3.5 ? "#F9E2AF" : "#F38BA8";

            var border = new Border
            {
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#313244")),
                CornerRadius = new Avalonia.CornerRadius(8),
                Padding = new Avalonia.Thickness(16, 12),
                Margin = new Avalonia.Thickness(0, 0, 0, 8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            // Levý sloupec - jméno a třída
            var infoStack = new StackPanel();
            infoStack.Children.Add(new TextBlock
            {
                Text = student.CeleJmeno,
                FontSize = 15,
                FontWeight = Avalonia.Media.FontWeight.SemiBold,
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#CDD6F4"))
            });
            infoStack.Children.Add(new TextBlock
            {
                Text = $"Třída: {student.Trida}",
                FontSize = 12,
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#89B4FA")),
                Margin = new Avalonia.Thickness(0, 2, 0, 0)
            });
            Grid.SetColumn(infoStack, 0);

            // Střední sloupec - průměr
            var prumerStack = new StackPanel
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Margin = new Avalonia.Thickness(0, 0, 20, 0)
            };
            prumerStack.Children.Add(new TextBlock
            {
                Text = prumerText,
                FontSize = 22,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(barvaPromer)),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            });
            prumerStack.Children.Add(new TextBlock
            {
                Text = "průměr",
                FontSize = 10,
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#6C7086")),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            });
            Grid.SetColumn(prumerStack, 1);

            // Pravý sloupec - tlačítka
            var btnStack = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Spacing = 6
            };

            var btnZnamky = new Button
            {
                Content = "📝 Známky",
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#CBA6F7")),
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#1E1E2E")),
                FontSize = 12,
                Padding = new Avalonia.Thickness(10, 6),
                CornerRadius = new Avalonia.CornerRadius(5)
            };
            btnZnamky.Click += async (_, _) =>
            {
                var okno = new ZnamkyWindow(_service, student.Id);
                await okno.ShowDialog(this);
                ObnovSeznam();
            };

            var btnUpravit = new Button
            {
                Content = "✏️",
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#45475A")),
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#CDD6F4")),
                FontSize = 14,
                Padding = new Avalonia.Thickness(8, 6),
                CornerRadius = new Avalonia.CornerRadius(5)
            };
            btnUpravit.Click += async (_, _) =>
            {
                var okno = new StudentFormWindow(_service, student);
                await okno.ShowDialog(this);
                ObnovSeznam();
            };

            var btnSmazat = new Button
            {
                Content = "🗑️",
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#F38BA8")),
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#1E1E2E")),
                FontSize = 14,
                Padding = new Avalonia.Thickness(8, 6),
                CornerRadius = new Avalonia.CornerRadius(5)
            };
            btnSmazat.Click += async (_, _) =>
            {
                var dialog = new PotvrzeniWindow($"Opravdu smazat studenta {student.CeleJmeno}?");
                bool potvrzen = await dialog.ShowDialog<bool>(this);
                if (potvrzen)
                {
                    _service.SmazStudenta(student.Id);
                    ObnovSeznam();
                }
            };

            btnStack.Children.Add(btnZnamky);
            btnStack.Children.Add(btnUpravit);
            btnStack.Children.Add(btnSmazat);
            Grid.SetColumn(btnStack, 2);

            grid.Children.Add(infoStack);
            grid.Children.Add(prumerStack);
            grid.Children.Add(btnStack);

            border.Child = grid;
            return border;
        }

        private async void BtnPridat_Click(object? sender, RoutedEventArgs e)
        {
            var okno = new StudentFormWindow(_service, null);
            await okno.ShowDialog(this);
            ObnovSeznam();
        }

        private async void BtnStatistiky_Click(object? sender, RoutedEventArgs e)
        {
            var okno = new StatistikyWindow(_service);
            await okno.ShowDialog(this);
        }

        private async void BtnExport_Click(object? sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Exportovat seznam studentů",
                DefaultExtension = "txt",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Textový soubor", Extensions = { "txt" } }
                },
                InitialFileName = "studenti_export.txt"
            };
            var cesta = await dialog.ShowAsync(this);
            if (!string.IsNullOrEmpty(cesta))
            {
                _service.ExportujTxt(cesta);
                StatusText.Text = $"✅ Exportováno do: {cesta}";
            }
        }
    }
}
