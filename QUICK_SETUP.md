# Quick Setup Guide - Arkanoid 3D

## Szybka Konfiguracja (15 minut)

### 1. Otworzyć Unity z tym projektem

### 2. Utworzyć Materiały (5 min)
W folderze Materials:
1. Prawym → Create → Material → "RedBrick"
   - Shader: Custom/BrickGlowShader
   - Color: Czerwony
   - Emission Color: Ciemny czerwony
   - Emission Strength: 1.0

2. Powtórz dla: BlueBrick, GreenBrick, YellowBrick, PurpleBrick

### 3. Konfiguracja Sceny (5 min)

**Main Camera:**
- Add Component: CameraController
- Position: (0, 0, -10)
- Orthographic: Yes
- Size: 10

**Directional Light:**
- Add Component: DynamicLighting
- Rotation: (50, -30, 0)

**Paddle:**
- GameObject → 3D Object → Cube
- Add: PaddleController, ProceduralPaddle, Rigidbody (Kinematic)
- Position: (0, -4, 0), Scale: (2, 0.3, 0.5)
- Tag: "Paddle"
- Input Actions: przeciągnij InputSystem_Actions

**Ball:**
- GameObject → 3D Object → Sphere
- Add: BallController, Rigidbody, TrailRenderer
- Position: (0, -3, 0), Scale: (0.3, 0.3, 0.3)
- Rigidbody: Use Gravity = OFF, Continuous Collision
- Tag: "Ball"

**Walls:** (3 cubes)
- Left: Position (-9, 0, 0), Scale (0.5, 20, 1), Add WallController
- Right: Position (9, 0, 0), Scale (0.5, 20, 1)
- Top: Position (0, 10, 0), Scale (20, 0.5, 1)

**DeadZone:**
- Cube, Position (0, -6, 0), Scale (20, 1, 1)
- Add DeadZone, BoxCollider (Trigger = ON)
- Remove MeshRenderer
- Tag: "DeadZone"

**Brick Prefab:**
- Cube → Add: BrickController, ProceduralBrick
- Scale: (1, 0.4, 0.5), Tag: "Brick"
- Przeciągnij do Prefabs, usuń ze sceny

**LevelGenerator:**
- Empty GameObject
- Add: LevelGenerator
- Rows: 5, Columns: 10
- Brick Prefab: twój prefab
- Brick Materials: przeciągnij wszystkie materiały

**GameManager:**
- Empty GameObject
- Add: GameManager, AudioManager, ParticleController

### 4. UI (5 min)

**Canvas:**
- UI → Canvas
- Add: UIManager

**HUD:**
- Panel → 2x TextMeshPro (Score, Lives)
- Ustawić na górze ekranu

**Pause/GameOver/Victory Panels:**
- 3 panele z tekstami i przyciskami
- Ukryć na start

**Połączyć referencje w UIManager**

### 5. Tags
Project Settings → Tags:
- Paddle
- Ball
- Brick
- DeadZone
- Wall

### 6. PLAY!

## Troubleshooting

**Piłka nie odbija się:**
- Sprawdź czy Ball ma Rigidbody + SphereCollider
- Sprawdź czy bloki mają BoxCollider
- Physics Material na piłce: Bounciness = 1, Friction = 0

**Input nie działa:**
- Sprawdź czy PaddleController ma przypisany InputSystem_Actions
- Window → Package Manager → Input System zainstalowany?

**Bloki nie generują się:**
- Sprawdź czy LevelGenerator ma brick prefab
- Sprawdź czy materiały są przypisane

**Brak dźwięków:**
- Normalne - trzeba dodać własne audio do folderu Audio
- Przypisać w AudioManager

## Gotowe!
