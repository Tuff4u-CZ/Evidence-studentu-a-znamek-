# 📚 Evidence studentů a známek

Školní aplikace pro správu studentů a jejich známek, vytvořená v .NET 8 + Avalonia UI.

## Funkce

- ✅ Seznam studentů s filtrováním a vyhledáváním
- ✅ Přidávání, editace a mazání studentů
- ✅ Zadávání a mazání známek po předmětech
- ✅ Výpočet průměru celkového i per předmět
- ✅ Statistiky třídy (nejlepší/nejhorší student, průměr třídy)
- ✅ Export seznamu do TXT souboru
- ✅ Automatické ukládání dat do AppData
- ✅ Validace vstupů

## Struktura projektu

```
StudentEvidence/
├── Models/
│   └── Student.cs          # Datový model studenta
├── Services/
│   ├── DataService.cs      # Čtení/zápis do AppData + TXT export
│   ├── StudentService.cs   # Aplikační logika (CRUD, filtrování, statistiky)
│   └── Validace.cs         # Validační metody
└── Views/
    ├── MainWindow           # Hlavní okno - seznam studentů
    ├── StudentFormWindow    # Formulář přidat/upravit studenta
    ├── ZnamkyWindow         # Správa známek studenta
    ├── StatistikyWindow     # Statistiky třídy
    └── PotvrzeniWindow      # Potvrzovací dialog
```

## Jak spustit

```bash
cd StudentEvidence
dotnet run
```

## Data

Data se automaticky ukládají do:
`%AppData%\StudentEvidence\studenti.txt`

## Technologie

- .NET 8
- Avalonia UI 11.2
- C#


