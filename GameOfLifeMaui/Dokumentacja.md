<h1>Pages</h1>

<h2>```MainPage```</h2>
Klasa ```MainPage```, jak nazwa wskazuje, stanowi główną stronę aplikacji z widoczną planszą gry.
Zawiera ona publiczny konstruktor inicjalizujący sam obiekt gry, tworzący główną pętlę działania gry i wczytującej zapisane ustawienia.

<h2>```SettingsPage```</h2>
Klasa ```SettingsPage``` odpowiada za wyświetlanie strony z ustawieniami, jak i za wyświetlanie pop upów odpowiadających za wprowadzanie lub modyfikację ustawień.
Klasa posiada publiczny konstruktor inicjalizujący teksty pól wyświetlających aktualne ustawienia.
Publiczna metoda ```BuildColors``` odpowiada za przebudowanie listy kolorów po aktualizacji tego ustawienia.
Metody ```UpdateRulestring```, ```TappedAgeChange```, ```OnSizeChanged``` odpowiadają za aktualizacje odpowiadających im pól, ze specyficznymi ustawieniami.
Metody ```ToggleGyroscope``` i ```ToggleAccelerometer``` włączają (lub wyłączają) odczyty z żyroskopu lub akcelerometru.
Metoda ```GetTime``` wysyła request REST do API worldtimeapi.org, odczytując zwracany czas i datę, a następnie wyświetla go na UI.

<h1>Entities</h1>

<h2>```ColorAgeEntity```</h2>
Klasa posiada publiczne atrybuty Id, Age (wiek komórki mającej ten kolor), a także 4 atrybuty Red, Green, Blue i Alpha odpowiadających za utworzenie koloru RGBA.
Odpowiada za zapis ustawień kolorów do bazy danych, a także ponowny odczyt kolorów.

<h2>```SettingsEntity```</h2>
KLasa posiada publiczne atrybuty Id, FieldName (nazwa pola ustawienia) i Value (wartość zapisana w string).
Odpowiada za zapis pojedyńczego ustawienia do bazy danych.

<h1>Popups</h1>

<h2>```CellAgePopup```</h2>
Klasa odpowiada za wyświetlanie pop upu, zapisanie i wysłanie sygnału uaktualniającego wartość ustawienia wieku dotkniętej komórki.

<h2>```CellSizePopup```</h2>
Klasa odpowiada za wyświetlenie pop upu, zapisanie nowej wartości i asynchroniczną aktualizację planszy, na postawie podanej wartości rozmiaru komórki w px.

<h2>```ColorPopup```</h2>
Klasa odpowiada za wyświetlenie pop upu tworzącym (lub edytującym istniejacy) kolor, następnie zapis ustawienia i aktualizację UI.

<h2>```RulestringPupup```</h2>
Klasa odpowiada za wyświetlenie pop upu umożliwiającego użytkownikowi ustawienia nowych zasad gry.

<h1>Inne ViewModele</h1>

<h2>```App```</h2>
Klasa startowa inicjalizująca aplikację. Może być nadpisywana w zależności od platformy.

<h2>```AppShell```</h2>
Aplikacja inicjalizująca Shell.

<h2>```ColorHolder```</h2>
Klasa wyświetla pojedyńczy kolor, wraz z podanych wiekem w ustawieniach. Dodatkowo przechwytuje naciśniecie i otwiera pop up.

<h1>Logika</h1>

<h2>```MauiProgram```<h2>
Klasa startowa całej aplikacji. Inicjalizuje biblioteki i rejestruje serwisy.

<h2>```GameOfLife```<h2>
Klasa realizująca logikę gry. Odpowiada za renderowanie gry, inicjalizację gry, a także wylicza kolejne stany komórek.
Publiczna właściwość ```Layout``` wskazuje na widok, którym osadzona jest gra.
Metoda ```DrawNext``` kalkuluje następny stan planszy i następnie go wyświetla.
Metoda ```GetLivingCells``` pozwala dostać ilość żyjących komórek wokół podanych X i Y.
Metoda ```Seed``` ustawia komórki losowo (na podstawie cyfry lub pseudolosowo) na różne stany.
Metoda ```Clear``` czyści planszę, ustawiając wszystkie komórki na martwe.
Metoda ```ChangeCellSize``` zmienia rozmiar komórki i odświeża widok, przerysowując planszę.
Metoda ```CalculateNewCellNumber``` przerysowuje planszę w przypadku zmiany rozmiaru widoku, którym osadzona jest gra, lub rozmiaru komórki.
Metoda ```UpdateColors``` wywoływana jest przy zmianie kolorów i zmienia kolory komórek na nowe.
Metoda ```GetNearestChild``` zwraca nam instancję komórki położonej najbliżej podanego X i Y.


<h2>```Cell```<h2>
Klasa realizuje logikę pojedyńczej komórki na polu gry. Opiera się o obiekt ```Frame``` trzymający w sobie ```BoxView``` wyświetlający kolor.
Publiczne właściwości ```IndeX``` i ```IndexY``` są odpowiednikiem położenia komórki w tablicy komórek w logice gry.
Metoda ```ProcessLife``` ustawia kolejną wartość komórki w oparciu o aktualne zasady gry.
Metoda ```SetCurrentState``` ustawia wiek komórki i kolor w oparciu o jej stan.
Metoda ```UpdateColor``` ustawia kolor.
Metoda ```Freeze``` zamraża komórkę, robiąc ją niewrażliwą na zmiany koloru.
Metoda ```UnFreeze``` odmraża komórkę.
Metoda ```OnClick``` zmienia stan komórki, jak i jej kolor i wiek.

<h2>```GoLDatabase``` i ```IDatabase```<h2>
Klasy te przechowują logikę i interfejs użytej bazy danych.
Metoda ```GetSettingsAsync``` wczytuje zapisane ustawienia asynchronicznie.
Metoda ```SaveSettingAsync``` zapisuje pojedyńcze ustawienie asynchronicznie.
Metoda ```GetColorsAsync``` wczytuje aktualne ustawienia kolorów asynchronicznie.
Metoda ``SaveColorAsync```` zapisuje pojedyńczy kolor asynchronicznie.
Metoda ```RemoveColorAsync``` usuwa zapisany kolor o podanym wieku asynchronicznie.
Metoda ``````
Metoda ``````

<h2>```SettingsManager```<h2>
Klasa statyczna gromadząca aktualne ustawienia. Powiadamia stronę z ustawieniami jak i samą logikę gry o ewentualnych zmianach ustawień i zapisuje je do bazy danych. 
Publiczne właściwości ```Game``` przechowuje logikę gry, ```CurrentSize``` aktulny rozmiar komórki, ```TappedAge``` wiek naciśniętej komórki, ```BArg``` i ```SArg``` aktualne zasady, ```Database``` obiekt łączący się z bazą danych.
Metoda ```ChangeCellSize``` zmienia rozmiar komórki, aktualizuje UI i planszę, zapisuje ustawienie do bazy danych.
Metoda ```ChangeTappedCellAge``` zmnienia ustawienie wieku naciśniętej komórki.
Metoda ```GetColors``` zwraca wszystkie kolory.
Metoda ```TryRemoveColor``` usuwa pojedyńczy kolor i przebudowuje kolory.
Metoda ```GetRuleString``` zwraca aktualne zasady w formie string.
Metoda ```GetColor``` zwraca pojedyńczy kolor dla danej wartości Age.
Metoda ```LoadSettings``` ładuje zapisane wartości ustawień z bazy danych.

<h2>```PanGestureHandler```<h2>
Statyczna klasa odpowiadająca za logikę przeciągania po ekranie.
Metoda ```GetInstance``` zwraca instancję obiektu.
Metoda ```OnPanUpdated``` zawiera logikę interakcji użytkownika z planszą za pomocą pisania po ekranie. Zamraża tymczasowo komórki, wyszukuje ich pozycję i na koniec aktualizuje ich kolory i stany.