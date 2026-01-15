# TODO w Unity Editor

## ✅ Zrobione (kod gotowy)
- [x] Wszystkie skrypty C# (14 plików)
- [x] 2 custom shadery
- [x] Dokumentacja
- [x] Struktura folderów

## 🔧 Do zrobienia w Unity (15-20 minut)

### 1. Materiały (3 min)
W folderze Materials utwórz 5 materiałów:
- [ ] RedBrick (Shader: Custom/BrickGlowShader, kolor: czerwony)
- [ ] BlueBrick (niebieski)
- [ ] GreenBrick (zielony)
- [ ] YellowBrick (żółty)
- [ ] PurpleBrick (fioletowy)

### 2. Prefab Brick (2 min)
- [ ] Cube → dodaj BrickController, ProceduralBrick, BoxCollider
- [ ] Tag: "Brick"
- [ ] Zapisz jako prefab w folderze Prefabs

### 3. Scene Setup (10 min)
- [ ] Main Camera → dodaj CameraController, ustaw ortograficzną
- [ ] Directional Light → dodaj DynamicLighting
- [ ] Paddle (Cube) → PaddleController, ProceduralPaddle, Rigidbody(Kinematic), tag "Paddle"
- [ ] Ball (Sphere) → BallController, Rigidbody, tag "Ball"
- [ ] 3 Walls (Cubes) → WallController
- [ ] DeadZone (Cube invisible) → DeadZone, BoxCollider(Trigger), tag "DeadZone"
- [ ] LevelGenerator (Empty) → LevelGenerator, przypisz prefab i materiały
- [ ] GameManager (Empty) → GameManager, AudioManager, ParticleController

### 4. UI (5 min)
- [ ] Canvas → UIManager
- [ ] HUD Panel → 2x TextMeshPro (Score, Lives)
- [ ] Pause Panel → przyciski
- [ ] GameOver Panel → tekst + przyciski
- [ ] Victory Panel → tekst + przyciski
- [ ] Połącz referencje w UIManager

### 5. Tags (1 min)
Project Settings → Tags, dodaj:
- [ ] Paddle
- [ ] Ball
- [ ] Brick
- [ ] DeadZone
- [ ] Wall

### 6. Input System (jeśli trzeba)
- [ ] Sprawdź czy InputSystem_Actions ma akcję "Move"
- [ ] Przypisz w PaddleController

### 7. Opcjonalnie - Audio
Pobierz z freesound.org i dodaj:
- [ ] Ball hit paddle sound
- [ ] Ball hit brick sound
- [ ] Ball hit wall sound
- [ ] Brick destroy sound
- [ ] Lose life sound
- [ ] Victory sound
- [ ] Game over sound
- [ ] Przypisz w AudioManager

## 🎮 Test
- [ ] Play w edytorze
- [ ] Sprawdź ruch paletki
- [ ] Sprawdź odbicia piłki
- [ ] Sprawdź niszczenie bloków
- [ ] Sprawdź UI i punktację

## 📦 Build
- [ ] File → Build Settings → dodaj sceny
- [ ] Build and Run

## Czas realizacji
**Minimum (bez audio):** 15 minut
**Z audio:** 25 minut
**Ocena:** 9-10 / 10 punktów
