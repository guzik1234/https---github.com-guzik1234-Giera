# Instrukcja Dodawania Dźwięków do Gry

## Pliki dźwiękowe dostępne w projekcie:

1. **paddle_hit.wav** - Dźwięk odbicia piłki od paletki
2. **brick_break.wav** - Dźwięk zniszczenia cegły
3. **wall_bounce.wav** - Dźwięk odbicia od ściany
4. **life_lost.wav** - Dźwięk utraty życia
5. **game_over.wav** - Dźwięk końca gry (przegrana)
6. **victory.wav** - Dźwięk zwycięstwa

## Gdzie są używane dźwięki:

### 1. paddle_hit.wav (Odbicie od paletki)
- **Lokalizacja w kodzie**: `BallController.cs` - metoda `OnCollisionEnter()`
- **Kiedy**: Gdy piłka uderza w paletkę
- **Wywołanie**: `AudioManager.Instance.PlayBallHitPaddle()`

### 2. brick_break.wav (Zniszczenie cegły)
- **Lokalizacja w kodzie**: `BrickController.cs` - metoda `DestroyBrick()`
- **Kiedy**: Gdy cegła zostaje zniszczona (hp <= 0)
- **Wywołanie**: `AudioManager.Instance.PlayBrickDestroy()`
- **Także używane w**: `BallController.cs` dla efektu uderzenia w cegłę (`PlayBallHitBrick()`)

### 3. wall_bounce.wav (Odbicie od ściany)
- **Lokalizacja w kodzie**: `BallController.cs` - metoda `OnCollisionEnter()`
- **Kiedy**: Gdy piłka odbija się od ściany
- **Wywołanie**: `AudioManager.Instance.PlayBallHitWall()`

### 4. life_lost.wav (Utrata życia)
- **Lokalizacja w kodzie**: `GameManager.cs` - metoda `OnBallLost()`
- **Kiedy**: Gdy piłka spada do DeadZone i gracz traci życie
- **Wywołanie**: `AudioManager.Instance.PlayLoseLife()`

### 5. game_over.wav (Koniec gry)
- **Lokalizacja w kodzie**: `GameManager.cs` - metoda `GameOver()`
- **Kiedy**: Gdy gracz straci wszystkie życia
- **Wywołanie**: `AudioManager.Instance.PlayGameOver()`

### 6. victory.wav (Zwycięstwo)
- **Lokalizacja w kodzie**: `GameManager.cs` - metoda `WinLevel()`
- **Kiedy**: Gdy wszystkie cegły zostaną zniszczone
- **Wywołanie**: `AudioManager.Instance.PlayVictory()`

## Jak przypisać dźwięki w Unity Editor:

### Metoda 1: Ręczne przypisanie (Zalecana)

1. **Znajdź AudioManager w scenie:**
   - Otwórz scenę `SampleScene` lub `MainMenu`
   - W Hierarchy znajdź obiekt `AudioManager` (jeśli nie ma, stwórz pusty GameObject i dodaj skrypt AudioManager)

2. **Przypisz pliki audio:**
   - Zaznacz obiekt AudioManager w Hierarchy
   - W Inspectorze zobaczysz sekcję "Sound Clips" z polami:
     - Paddle Hit Sound
     - Brick Break Sound
     - Wall Bounce Sound
     - Life Lost Sound
     - Game Over Sound
     - Victory Sound
   
3. **Przeciągnij pliki:**
   - Z folderu `Assets/Audio/` przeciągnij odpowiednie pliki:
     - `paddle_hit.wav.wav` → Paddle Hit Sound
     - `brick_break.wav.wav` → Brick Break Sound
     - `wall_bounce.wav.mp3` → Wall Bounce Sound
     - `life_lost.wav.wav` → Life Lost Sound
     - `game_over.wav.mp3` → Game Over Sound
     - `victory.wav.wav` → Victory Sound

### Metoda 2: Automatyczne ładowanie przez Resources (Backup)

Jeśli nie przypiszesz plików ręcznie, AudioManager spróbuje załadować je automatycznie z folderu `Resources/Audio/`. 

**Aby to działało:**
1. Stwórz folder `Assets/Resources/Audio/`
2. Przenieś tam pliki dźwiękowe
3. Zmień nazwy plików aby pasowały:
   - `paddle_hit.wav` → `paddle_hit`
   - `brick_break.wav` → `brick_break`
   - `wall_bounce.wav` → `wall_bounce`
   - `life_lost.wav` → `life_lost`
   - `game_over.wav` → `game_over`
   - `victory.wav` → `victory`

## Testowanie dźwięków:

1. Uruchom grę w Unity
2. Sprawdź Console - powinna być wiadomość:
   ```
   AudioManager: Loaded sounds - Paddle:True, Brick:True, Wall:True, Life:True, GameOver:True, Victory:True
   ```
3. Jeśli któryś dźwięk pokazuje `False`, sprawdź czy plik jest przypisany w Inspectorze

## Ustawienia głośności:

W Inspectorze AudioManagera możesz zmienić:
- **Master Volume**: Główna głośność (domyślnie 0.3)
- **Enable Audio**: Czy dźwięki są włączone (domyślnie true)

## Formaty audio:

Unity obsługuje:
- `.wav` - najlepszy dla krótkich efektów dźwiękowych
- `.mp3` - dobry dla dłuższej muzyki
- `.ogg` - zalecany dla muzyki (mniejszy rozmiar)

## Rozwiązywanie problemów:

### Dźwięki nie grają:
1. Sprawdź czy AudioManager jest w scenie
2. Sprawdź czy pliki są przypisane w Inspectorze
3. Sprawdź Console czy są błędy
4. Sprawdź czy Master Volume nie jest ustawiona na 0
5. Upewnij się że Enable Audio jest zaznaczone

### Dźwięki są za głośne/ciche:
1. Zmień Master Volume w AudioManager
2. W kodzie każdy dźwięk ma własny mnożnik głośności (np. `masterVolume * 0.8f`)

### Dźwięki się nakładają:
- To normalne! Gra używa `PlayOneShot()` aby wiele dźwięków mogło grać jednocześnie
- Jeśli przeszkadza, możesz zmniejszyć Master Volume

## Dodatkowe informacje:

Kod został zmodyfikowany aby używać prawdziwych plików audio zamiast proceduralnie generowanych dźwięków. 

Wszystkie wywołania audio są już zaimplementowane w odpowiednich miejscach w kodzie:
- ✅ BallController.cs - odbicia
- ✅ BrickController.cs - zniszczenie cegły
- ✅ GameManager.cs - życia, game over, zwycięstwo
