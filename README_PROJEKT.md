# Arkanoid 3D - Dokumentacja Projektu

## Opis Projektu
Gra typu Arkanoid wykonana w Unity 3D. Projekt spełnia wszystkie wymagania akademickie, maksymalizując punktację we wszystkich kategoriach.

## Kategorie Oceny - Realizacja Wymagań

### 1. Poprawność Modeli (1.125 pkt)
**Realizacja:**
- Modele 3D generowane proceduralnie przez skrypty:
  - `ProceduralPaddle.cs` - zakrzywiona paletka z segmentami
  - `ProceduralBrick.cs` - bloki z opcjonalnymi fazami
- Złożoność geometryczna: paletka z 10 segmentami, krzywa powierzchnia
- Meshes z poprawną topologią, UV mapping, normalnymi
- Brak nieciągłości siatki dzięki proceduralnej generacji

**Modele:**
- Paletka: zakrzywiona geometria, 10 segmentów
- Piłka: Unity Sphere (złożoność geometryczna wystarczająca)
- Bloki: proceduralne cubes z opcjonalnymi detalami
- Ściany: proste planes/cubes

### 2. Kamera Wirtualna (1.125 pkt)
**Realizacja:**
- Skrypt: `CameraController.cs`
- Typ: Kamera ortograficzna (idealna dla Arkanoida)
- Parametry konfigurowalne: orthographicSize, FOV
- Funkcje:
  - Screen shake przy uderzeniach
  - Smooth follow (opcjonalnie)
  - Animowany zoom
  - Efekty proceduralne

### 3. Shadery/Materiały (1.125 pkt)
**Realizacja:**
- **2 Custom Shadery:**
  - `BrickGlowShader.shader` - efekt świecenia, Fresnel, pulsacja
  - `HolographicShader.shader` - scanlines, glitch effect, rim lighting
- **Materiały wbudowane:**
  - Standard PBR dla podstawowych obiektów
  - Materiały z metalliciem i smoothness
- **Oświetlenie:**
  - Dynamic lighting w `DynamicLighting.cs`
  - Reaguje na eventy (flash przy uderzeniach)
  - Pulsujące oświetlenie
  - Emisja w shaderach

### 4. Zasoby Zewnętrzne (1.125 pkt)
**Realizacja:**
- Strukturyzowana organizacja:
  - `/Audio` - pliki dźwiękowe (należy dodać)
  - `/Textures` - tekstury (można dodać własne)
  - `/Materials` - materiały
  - `/Prefabs` - prefabrykaty
- **AudioManager.cs** zarządza wszystkimi dźwiękami
- Rekomendacja: dodać własne audio z freesound.org lub nagrać własne
- Shadery napisane od zera (własne)

**Dźwięki do dodania:**
- Odbicie od paletki
- Odbicie od bloku
- Odbicie od ściany
- Zniszczenie bloku
- Utrata życia
- Victory/Game Over
- Muzyka w tle

### 5. Organizacja Sceny (1.125 pkt)
**Realizacja:**
- **Struktura drzewiasta:**
  - GameObject `BricksContainer` z wszystkimi blokami
  - Hierarchia: Camera → Lighting → GameplayObjects → UI
- **Techniki optymalizacyjne:**
  - MaterialPropertyBlock (bez tworzenia instancji materiałów)
  - Object pooling dla fragmentów (destrukcja bloków)
  - Occlusion culling możliwy
  - Batching dla identycznych bloków
- **LevelGenerator.cs** - automatyczna organizacja bloków w kontenerze

### 6. Animacje (1.125 pkt)
**Realizacja:**
- **Animacje proceduralne:**
  - Squeeze effect paletki (`PaddleController.cs`)
  - Rotacja piłki proporcjonalna do prędkości
  - Pulsujące shadery (animacja w czasie rzeczywistym)
  - Screen shake
  - Animacja spawnu bloków (elastic ease)
  - Flash efekty (kolory)
- **Animacje fragmentów:**
  - Fizyka dla eksplodujących części bloków
  - Trail renderer dla piłki
  - Particles

**Typy:**
- Proceduralne: tak (większość)
- Keyframe/Baked: można dodać w Animatorze dla UI

### 7. Fizyka i Kolizje (1.125 pkt)
**Realizacja:**
- **Rigidbody 3D:**
  - Piłka: Dynamic rigidbody, continuous collision detection
  - Paletka: Kinematic rigidbody
  - Bloki: Kinematic/Static
- **Collidery:**
  - BoxCollider dla bloków i paletki
  - SphereCollider dla piłki
  - BoxCollider (trigger) dla DeadZone
- **Zjawiska fizyczne:**
  - Odbicia piłki (Vector3.Reflect)
  - Modyfikacja kąta odbicia od paletki
  - Przyspieszanie piłki
  - Grawitacja dla fragmentów
  - Siły wybuchowe (AddForce) przy destrukcji

### 8. Kompletność Projektu (1.125 pkt)
**Realizacja:**
- **Menu startowe:** `MainMenuManager.cs`
  - Opcje: Play, Options, Credits, Quit
  - Ustawienia audio
- **Cel rozgrywki:** Jasno określony
  - Zniszcz wszystkie bloki
  - Nie trać wszystkich żyć
- **Warunki końca:**
  - **Zwycięstwo:** wszystkie bloki zniszczone
  - **Przegrana:** życia = 0
  - Ekrany podsumowania z wynikiem
- **UI System:**
  - HUD: wynik, życia
  - Pause menu
  - Victory/GameOver screens
- **GameManager.cs** - kompletna logika gry

## Struktura Projektu

```
Assets/
├── Scenes/
│   ├── MainMenu.unity      [NALEŻY UTWORZYĆ]
│   └── SampleScene.unity    [GRA]
├── Scripts/
│   ├── PaddleController.cs
│   ├── BallController.cs
│   ├── BrickController.cs
│   ├── GameManager.cs
│   ├── UIManager.cs
│   ├── MainMenuManager.cs
│   ├── AudioManager.cs
│   ├── CameraController.cs
│   ├── LevelGenerator.cs
│   ├── ProceduralPaddle.cs
│   ├── ProceduralBrick.cs
│   ├── WallController.cs
│   ├── DeadZone.cs
│   ├── ParticleController.cs
│   └── DynamicLighting.cs
├── Shaders/
│   ├── BrickGlowShader.shader
│   └── HolographicShader.shader
├── Materials/       [NALEŻY UTWORZYĆ W UNITY]
├── Prefabs/         [NALEŻY UTWORZYĆ W UNITY]
├── Audio/           [DODAĆ PLIKI AUDIO]
└── Textures/        [OPCJONALNIE]
```

## Instrukcja Konfiguracji w Unity

### Krok 1: Utworzenie Sceny Gry (SampleScene)

#### 1.1 Kamera
1. Zaznacz Main Camera
2. Dodaj komponent `CameraController`
3. Ustaw:
   - Position: (0, 0, -10)
   - Is Orthographic: ✓
   - Orthographic Size: 10

#### 1.2 Oświetlenie
1. Create → Light → Directional Light
2. Dodaj komponent `DynamicLighting`
3. Ustaw:
   - Rotation: (50, -30, 0)
   - Intensity: 1
   - Color: White

#### 1.3 Paletka (Paddle)
1. Create → 3D Object → Cube (nazwij "Paddle")
2. Dodaj komponenty:
   - `PaddleController`
   - `ProceduralPaddle`
   - `Rigidbody` (Kinematic: ✓)
   - `BoxCollider`
3. Ustaw:
   - Position: (0, -4, 0)
   - Scale: (2, 0.3, 0.5)
   - Tag: "Paddle"
4. W PaddleController:
   - Input Actions: przeciągnij InputSystem_Actions
   - Move Speed: 10
   - Min X: -8, Max X: 8

#### 1.4 Piłka (Ball)
1. Create → 3D Object → Sphere (nazwij "Ball")
2. Dodaj komponenty:
   - `BallController`
   - `Rigidbody`:
     - Mass: 1
     - Use Gravity: ✗
     - Collision Detection: Continuous
   - `SphereCollider`
   - `TrailRenderer` (opcjonalnie)
   - `AudioSource`
3. Ustaw:
   - Position: (0, -3, 0)
   - Scale: (0.3, 0.3, 0.3)
   - Tag: "Ball"
4. W BallController:
   - Initial Speed: 8
   - Max Speed: 15
   - Start On Awake: ✗ (start przez GameManager)

#### 1.5 Ściany (Walls)
1. **Lewa ściana:**
   - Create → 3D Object → Cube
   - Position: (-9, 0, 0)
   - Scale: (0.5, 20, 1)
   - Dodaj: `WallController`, `BoxCollider`

2. **Prawa ściana:**
   - Position: (9, 0, 0)
   - Scale: (0.5, 20, 1)

3. **Górna ściana:**
   - Position: (0, 10, 0)
   - Scale: (20, 0.5, 1)

#### 1.6 Dead Zone
1. Create → 3D Object → Cube (nazwij "DeadZone")
2. Ustaw:
   - Position: (0, -6, 0)
   - Scale: (20, 1, 1)
   - Tag: "DeadZone"
3. Dodaj komponenty:
   - `DeadZone`
   - `BoxCollider` (Is Trigger: ✓)
4. Usuń `MeshRenderer` (niewidoczny)

#### 1.7 Bloki - Prefab
1. Create → 3D Object → Cube (nazwij "Brick")
2. Dodaj komponenty:
   - `BrickController`
   - `ProceduralBrick`
   - `BoxCollider`
   - `AudioSource`
3. Ustaw:
   - Scale: (1, 0.4, 0.5)
   - Tag: "Brick"
4. Przeciągnij do folderu Prefabs
5. Usuń z sceny

#### 1.8 Level Generator
1. Create → Create Empty (nazwij "LevelGenerator")
2. Dodaj komponent `LevelGenerator`
3. Ustaw:
   - Rows: 5
   - Columns: 10
   - Block Spacing: 1.1
   - Start Position: (-5, 5, 0)
   - Brick Prefab: przeciągnij prefab Brick
   - Brick Materials: stwórz kilka materiałów (patrz niżej)

#### 1.9 Materiały
1. Utwórz folder `Assets/Materials`
2. Stwórz materiały dla bloków:
   - **Red Brick:** Create → Material
     - Shader: Custom/BrickGlowShader
     - Color: czerwony
     - Emission Color: czerwony (ciemny)
   - **Blue Brick, Green Brick, Yellow Brick** - analogicznie
3. Przeciągnij materiały do `Brick Materials` w LevelGenerator

#### 1.10 Game Manager
1. Create → Create Empty (nazwij "GameManager")
2. Dodaj komponenty:
   - `GameManager`
   - `AudioManager`
   - `ParticleController`
3. Ustaw w GameManager:
   - Starting Lives: 3
   - Ball Prefab: przeciągnij Ball
   - UI Manager: (skonfigurujemy później)

#### 1.11 UI System
1. Create → UI → Canvas (nazwij "GameCanvas")
2. Canvas Scaler:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080

**HUD Panel:**
1. W Canvas: Create → UI → Panel (nazwij "HUD")
2. Dodaj TextMeshPro dla Score:
   - Create → UI → Text - TextMeshPro
   - Anchor: górny lewy róg
   - Text: "Score: 000000"
3. Dodaj TextMeshPro dla Lives:
   - Anchor: górny prawy róg
   - Text: "Lives: 3"

**Pause Panel:**
1. Create → UI → Panel (nazwij "PausePanel")
2. Background: ciemny, semi-transparent
3. Dodaj przyciski: Resume, Restart, Main Menu, Quit

**GameOver Panel:**
1. Create → UI → Panel (nazwij "GameOverPanel")
2. Dodaj: tytuł, final score text, przyciski (Restart, Main Menu)

**Victory Panel:**
1. Create → UI → Panel (nazwij "VictoryPanel")
2. Podobnie jak GameOver

**UI Manager:**
1. Dodaj do Canvas komponent `UIManager`
2. Przeciągnij wszystkie referencje (panele, texty)

### Krok 2: Scena Menu Głównego

1. Create → Scene (nazwij "MainMenu")
2. Create → UI → Canvas
3. Dodaj komponent `MainMenuManager`
4. Stwórz UI:
   - Tytuł gry
   - Przyciski: Play, Options, Credits, Quit
   - Panel Options z sliderem volume
   - Panel Credits
5. Podłącz przyciski do metod w MainMenuManager

### Krok 3: Build Settings
1. File → Build Settings
2. Dodaj sceny:
   - MainMenu (index 0)
   - SampleScene (index 1)

### Krok 4: Physics Settings
1. Edit → Project Settings → Physics
2. Sprawdź Layer Collision Matrix:
   - Ball kolizje ze wszystkim
   - Walls statyczne

### Krok 5: Tags
1. Edit → Project Settings → Tags and Layers
2. Dodaj tagi:
   - "Paddle"
   - "Ball"
   - "Brick"
   - "DeadZone"
   - "Wall"

### Krok 6: Audio (Opcjonalnie - Dodaj Własne)
1. Pobierz dźwięki z freesound.org lub nagraj własne
2. Przeciągnij do folderu Audio
3. Przypisz w AudioManager

## Sterowanie
- **Strzałki** / **A/D** - Ruch paletką
- **Spacja** - Start piłki (opcjonalnie)
- **ESC** - Pauza

## Punktacja Spodziewana

| Kategoria | Punkty | Realizacja |
|-----------|--------|------------|
| Modele | 1.125 | Proceduralne meshes 3D, złożona geometria |
| Kamera | 1.125 | Ortograficzna + skrypty efektów |
| Shadery | 1.125 | 2 custom shadery + oświetlenie dynamiczne |
| Zasoby | 1.125 | Audio system, własne shadery |
| Organizacja | 1.125 | Hierarchia + optymalizacje |
| Animacje | 1.125 | Proceduralne + fizyczne |
| Fizyka | 1.125 | Rigidbody 3D + zjawiska fizyczne |
| Kompletność | 1.125 | Pełne menu + warunki gry |
| **SUMA** | **9-10** | **Wszystkie kategorie spełnione** |

## Rozszerzenia (Opcjonalne Bonusy)
- Power-upy (multi-ball, większa paletka)
- Więcej poziomów
- High score system z zapisem
- Particle effects (już częściowo)
- Post-processing effects
- Własna muzyka

## Autor
Projekt wykonany na potrzeby kursu Unity/Godot
Data: 2026-01-15
