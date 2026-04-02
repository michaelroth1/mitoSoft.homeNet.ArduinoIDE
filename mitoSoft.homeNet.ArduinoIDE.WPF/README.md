# mitoSoft.homeNet.ArduinoIDE.WPF

Dies ist die WPF-Version des mitoSoft homeNet Arduino IDE, das das Windows Forms Frontend ersetzt.

## Funktionen

- **YAML-Editor**: Bearbeiten Sie YAML-Konfigurationsdateien mit Syntax-Highlighting
- **Controller-Verwaltung**: Wählen Sie und verwalten Sie HomeNet-Controller
- **Fehlerprüfung**: Überprüfen Sie Ihre Konfiguration auf Fehler und Warnungen
- **Code-Generierung**: Generieren Sie Arduino-Code aus YAML-Konfigurationen
- **Kommentierung**: Kommentieren/Dekommentieren von Zeilen
- **Suchen**: Suchen Sie Text in der YAML-Datei (Strg+F)
- **Zoom**: Passen Sie die Schriftgröße an
- **Persistente Einstellungen**: Fenster-Größe, Zoom und Inhalt werden gespeichert

## Hauptfenster (MainWindow)

Das Hauptfenster bietet:
- Menüleiste mit Datei- und Bearbeitungsoptionen
- Toolbar mit Icon-Buttons und schnellem Zugriff auf häufig verwendete Funktionen:
  - ✓ Check (Grün): Überprüft Fehler und Warnungen
  - 🔧 Build (Orange): Generiert Arduino-Code
  - ➕ Comment (Blau): Kommentiert ausgewählte Zeilen
  - ➖ Uncomment (Rot): Entfernt Kommentare von ausgewählten Zeilen
- YAML-Editor im Hauptbereich
- Fehler-/Ausgabebereich unten
- Statusleiste mit Dateiinformationen

## Ausgabefenster (OutputWindow)

Zeigt generierten Arduino-Code und erlaubt:
- Kopieren in die Zwischenablage
- Speichern als .ino-Datei

## Tastenkombinationen

- **Strg+F**: Suchen

## Projektstruktur

- `MainWindow.xaml/cs`: Hauptfenster
- `OutputWindow.xaml/cs`: Ausgabefenster für generierten Code
- `FindDialog.xaml/cs`: Dialog für die Suchfunktion
- `RelayCommand.cs`: Hilfsklasse für WPF Commands
- `App.xaml/cs`: Application-Einstiegspunkt mit Icon-Ressourcen
- `Properties/Settings.settings`: Benutzereinstellungen

### Icon-Ressourcen (App.xaml)

Die Anwendung verwendet PNG-Icon-Dateien aus dem Resources-Ordner:
- `Check.png` (CheckBoxChecked.png): Icon für Fehlerprüfung
- `Build.png` (PlayVideo.png): Icon für Code-Generierung
- `Comment.png` (CommentCode.png): Icon zum Kommentieren
- `Uncomment.png` (UncommentCode.png): Icon zum Entfernen von Kommentaren

Diese Icons werden als BitmapImage-Ressourcen geladen und in den Toolbar-Buttons verwendet.
Die PNG-Dateien sind im Projekt als Ressourcen verlinkt und stammen aus dem Forms-Projekt.

## Abhängigkeiten

- .NET 9.0 Windows
- mitoSoft.homeNet.ArduinoIDE.ProgramParser (Projekt-Referenz)

## Migration von Windows Forms

Diese WPF-Anwendung ersetzt vollständig das Windows Forms Frontend (`mitoSoft.homeNet.ArduinoIDE`) 
und bietet eine modernere, flexiblere Benutzeroberfläche mit:
- Besserer Skalierung und DPI-Unterstützung
- Moderneres Look & Feel
- Verbesserte Datenbindung
- MVVM-fähige Architektur
