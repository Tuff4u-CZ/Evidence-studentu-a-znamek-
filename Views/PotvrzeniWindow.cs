using Avalonia.Controls;
using Avalonia.Interactivity;

namespace StudentEvidence.Views
{
    // Potvrzovací dialog - vrátí true (Ano) nebo false (Zrušit)
    public partial class PotvrzeniWindow : Window
    {
        public PotvrzeniWindow(string zprava)
        {
            InitializeComponent();
            ZpravaText.Text = zprava;
            BtnAno.Click += (_, _) => Close(true);
            BtnNe.Click += (_, _) => Close(false);
        }
    }
}
