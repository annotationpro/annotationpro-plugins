# Edit#Find-and-Replace

## Opis
Plugin `Edit#Find-and-Replace` to narzędzie dla programu Annotation Pro, pozwalające na masową zamianę ciągów znaków (znajdź i zamień) lub ich całkowite usunięcie (poprzez zamianę na ciąg pusty) wewnątrz etykiet anotacyjnych.

## Funkcjonalność
* **Filtrowanie warstw:** Skrypt automatycznie pomija wybrane warstwy (np. `Word`, `Syllable`, `Phone`, `Expressive Movement Unit`), dzięki czemu chroni dane pierwotne i surowe transkrypcje przed przypadkową modyfikacją.
* **Filtrowanie etykiet:** Możliwość wykluczenia specyficznych etykiet z procesu zamiany.
* **Zasady zamiany (ReplacePatterns):** Użytkownik definiuje w kodzie listę wzorców `ReplacePattern(szukany_tekst, zamiennik)`. Plugin automatycznie iteruje po wszystkich dopuszczonych segmentach i wykonuje operacje na tekście.

## Zastosowanie w kodzie
Aby dostosować plugin do własnych potrzeb analizy, edytuj poniższe listy w pliku `.cs`:
- `ignoreLayers` - zbiór nazw warstw, które nie będą sprawdzane.
- `ignoreLabels` - zbiór etykiet, które mają pozostać nietknięte.
- `replacePatterns` - reguły podmiany tekstu. Aby usunąć ciąg znaków, jako argument "zamień" podaj `string.Empty`.

## Autorzy
Wojciech Klessa & Katarzyna Klessa (2016)
