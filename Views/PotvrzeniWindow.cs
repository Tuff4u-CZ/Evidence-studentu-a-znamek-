using Avalonia.Controls;
using Avalonia.Interactivity;

namespace StudentEvidence.Views
{
    // Jednoduchý potvrzovací dialog - vrátí true nebo false
    public class PotvrzeniWindow : Window
    {
        public PotvrzeniWindow(string zprava)
        {
            Title = "Potvrzení";
            Width = 380;
            Height = 160;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Background = Avalonia.Media.Brushes.Transparent;

            var grid = new Grid { Margin = new Avalonia.Thickness(24) };
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            var border = new Border
            {
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#313244")),
                CornerRadius = new Avalonia.CornerRadius(10),
                Padding = new Avalonia.Thickness(24)
            };

            var innerStack = new StackPanel { Spacing = 16 };
            innerStack.Children.Add(new TextBlock
            {
                Text = zprava,
                FontSize = 14,
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#CDD6F4")),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            });

            var btnPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Spacing = 8
            };

            var btnAno = new Button
            {
                Content = "✅ Ano, smazat",
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#F38BA8")),
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#1E1E2E")),
                FontWeight = Avalonia.Media.FontWeight.SemiBold,
                Padding = new Avalonia.Thickness(16, 8),
                CornerRadius = new Avalonia.CornerRadius(6)
            };
            btnAno.Click += (_, _) => Close(true);

            var btnNe = new Button
            {
                Content = "Zrušit",
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#45475A")),
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#CDD6F4")),
                Padding = new Avalonia.Thickness(16, 8),
                CornerRadius = new Avalonia.CornerRadius(6)
            };
            btnNe.Click += (_, _) => Close(false);

            btnPanel.Children.Add(btnAno);
            btnPanel.Children.Add(btnNe);
            innerStack.Children.Add(btnPanel);
            border.Child = innerStack;

            var outerGrid = new Grid();
            outerGrid.Children.Add(border);
            Content = outerGrid;
        }
    }
}
