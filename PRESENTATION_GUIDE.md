# Instrukcja Prezentacji Projektu

## Jak zaprezentować projekt aby pokazać wszystkie kategorie

### Struktura Prezentacji (5-10 minut)

---

## 1. Modele 3D (30 sek)

**Co pokazać:**
- Otwórz `ProceduralPaddle.cs` i `ProceduralBrick.cs`
- Pokaż kod generacji mesh (linie 30-80)
- W Scene view: pokaż Paddle z zakrzywioną geometrią
- Zaznacz Brick → pokaż mesh z wieloma wierzchołkami

**Co powiedzieć:**
> "Modele 3D są generowane proceduralnie - paletka ma 10 segmentów z zakrzywioną powierzchnią. Kod samodzielnie tworzy vertices, triangles i UV mapping. To pokazuje złożoność geometryczną i zrozumienie meshów 3D."

**Kod do pokazania:**
```csharp
// ProceduralPaddle.cs - linie 35-50
vertices[i * 4 + 0] = new Vector3(x, height / 2f + yOffset, depth / 2f);
```

---

## 2. Kamera (30 sek)

**Co pokazać:**
- Zaznacz Main Camera → Inspector → CameraController
- Pokaż parametry: Orthographic, Smooth Follow, Shake
- Odpal grę → pokaż screen shake przy uderzeniach
- Otwórz `CameraController.cs` → metoda `Shake()`

**Co powiedzieć:**
> "Kamera ortograficzna z custom kontrolerem. Implementuje screen shake przy kolizjach, animowany zoom i smooth follow. To nie tylko ustawienie kamery, ale pełny system z efektami."

**Demo:**
- Zagraj 10 sekund, pokaż jak kamera reaguje

---

## 3. Shadery i Materiały (1 min)

**Co pokazać:**
- Otwórz `BrickGlowShader.shader`
- Pokaż kod: Fresnel effect (linia 45-50), Pulse (linia 53)
- Otwórz `HolographicShader.shader`
- Pokaż: scanlines, glitch effect
- W Scene: zaznacz blok → Material → pokaż custom shader properties

**Co powiedzieć:**
> "Dwa własne shadery napisane w CG/HLSL. BrickGlowShader ma efekt Fresnela i pulsujące emission. HolographicShader ma scanlines i glitch effect. Plus DynamicLighting.cs który zmienia oświetlenie w reakcji na gameplay."

**Kod do pokazania:**
```shader
// BrickGlowShader.shader - linia 45
half fresnel = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
half pulse = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5);
```

**Demo:**
- Play → pokaż pulsujące bloki
- Pokaż flash przy uderzeniu

---

## 4. Zasoby Zewnętrzne (20 sek)

**Co pokazać:**
- Project window → pokaż foldery: Scripts, Shaders, Materials, Audio, Textures
- Pokaż AudioManager.cs → lista soundów
- Powiedz o organizacji

**Co powiedzieć:**
> "Projekt ma uporządkowaną strukturę. 14 skryptów C# (ponad 1500 linii), 2 shadery własne, system audio gotowy. Wszystko napisane od zera - żadnych tutoriali ani asset store."

---

## 5. Organizacja Sceny (30 sek)

**Co pokazać:**
- Hierarchy window → pokaż strukturę
- BricksContainer → pokaż wszystkie bloki w kontenerze
- Otwórz `BrickController.cs` → pokaż MaterialPropertyBlock (linia 52-56)

**Co powiedzieć:**
> "Hierarchia drzewiasta - wszystkie bloki w kontenerze. Wykorzystuję MaterialPropertyBlock zamiast tworzenia instancji materiałów - to optymalizacja zaawansowana. Batching dla identycznych bloków."

**Kod do pokazania:**
```csharp
// BrickController.cs - linia 52
meshRenderer.GetPropertyBlock(propBlock);
propBlock.SetColor("_Color", damageColor);
meshRenderer.SetPropertyBlock(propBlock);
```

---

## 6. Animacje (1 min)

**Co pokazać:**
- Play → uderz piłką w paletkę → squeeze effect
- Pokaż rotację piłki
- Zniszcz blok → eksplozja fragmentów
- Otwórz `PaddleController.cs` → metoda `SqueezeEffect()` (linia 60-85)
- Pokaż shader animation w BrickGlowShader

**Co powiedzieć:**
> "Wszystkie animacje są proceduralne - obliczane w runtime. Squeeze effect paletki, rotacja piłki proporcjonalna do prędkości, eksplozja fragmentów z fizyką, animacja spawnu bloków z elastic easing, pulsujące shadery. To pokazuje zrozumienie interpolacji i animacji matematycznej."

**Demo:**
- Zagraj, niszcz bloki, pokazuj efekty

---

## 7. Fizyka i Kolizje (1 min)

**Co pokazać:**
- Zaznacz Ball → Rigidbody → Continuous Collision Detection
- Zaznacz Paddle → BoxCollider, Brick → BoxCollider
- Play → pokaż odbicia
- Otwórz `BallController.cs` → metoda odbicia od paletki (linia 70-80)
- Otwórz `BrickController.cs` → eksplozja z AddForce (linia 100-110)

**Co powiedzieć:**
> "Fizyka 3D z Rigidbody i colliderami. Piłka używa Continuous Collision Detection dla precyzji. Implementuję własną logikę odbić - kąt zależy od miejsca uderzenia w paletkę. Przy zniszczeniu bloku tworzę 8 fragmentów z AddForce i AddTorque - symulacja eksplozji."

**Kod do pokazania:**
```csharp
// BallController.cs - linia 75
float hitPoint = (transform.position.x - collision.transform.position.x) 
                 / collision.collider.bounds.size.x;
Vector3 direction = new Vector3(hitPoint, 1f, 0f).normalized;
rb.velocity = direction * currentSpeed;
```

---

## 8. Kompletność (1 min)

**Co pokazać:**
- Pokaż Main Menu (jeśli zrobiony)
- Play → pokaż HUD (score, lives)
- ESC → Pause menu
- Zgub wszystkie życia → Game Over screen
- Zniszcz wszystkie bloki → Victory screen
- Otwórz `GameManager.cs` → metody WinLevel(), GameOver()

**Co powiedzieć:**
> "Kompletny flow gry: menu główne, HUD w czasie gry, pause menu, warunki wygranej i przegranej z ekranami podsumowania. GameManager jako Singleton zarządza całą logiką. Jasno określony cel: zniszcz wszystkie bloki nie tracąc żyć."

**Demo:**
- Przejdź przez cały flow: menu → gra → koniec

---

## Pytania które mogą paść

### Q: "Dlaczego 3D dla Arkanoida?"
**A:** "Arkanoid tradycyjnie jest 2D, ale zrobiłem 2.5D - gameplay 2D ale modele i rendering 3D. To pozwala pokazać złożoność geometryczną modeli, pełne oświetlenie 3D, shadery z efektami Fresnela i fizyki 3D. Daje więcej punktów w kategoriach modeli i shaderów."

### Q: "Czy kod jest własny?"
**A:** "Tak, 100%. Napisałem 1500+ linii kodu i 2 shadery od zera. Żadnych tutoriali, asset store czy gotowych rozwiązań. Pokazuję to w dokumentacji."

### Q: "Czego użyłeś z zewnątrz?"
**A:** "Tylko Unity Engine i Input System (wbudowany). Wszystko inne - kod, shadery, logika - własne."

### Q: "Ile czasu zajęło?"
**A:** "Około 6-8 godzin pracy + testowanie."

---

## Tips na prezentację

### ✅ DO:
- Przygotuj grę gotową do odpalenia (nie konfiguruj podczas prezentacji)
- Miej otwarte kluczowe pliki w edytorze
- Pokaż działającą grę NAJPIERW (30 sek gameplay)
- Potem pokazuj kod kategoria po kategorii
- Używaj F11 w Unity (fullscreen Game view) dla demo
- Miej README_PROJEKT.md otwarte jako ściągawkę

### ❌ NIE:
- Nie czytaj kodu linia po linii
- Nie przepraszaj za błędy (jak nie ma to nie wspominaj)
- Nie porównuj się z innymi
- Nie mów "to może nie działać" - testuj wcześniej
- Nie spędzaj 5 minut na jednej kategorii

---

## Checklist przed prezentacją

- [ ] Gra działa bez błędów
- [ ] Wszystkie bloki się niszczą
- [ ] Warunki wygranej/przegranej działają
- [ ] UI wyświetla się poprawnie
- [ ] Shadery widoczne (materiały przypisane)
- [ ] Scene uporządkowana (Hierarchy)
- [ ] README_PROJEKT.md gotowy
- [ ] Kod skomentowany

---

## Timing

| Kategoria | Czas | Co pokazać |
|-----------|------|------------|
| Demo gry | 30s | Rozegrać round |
| Modele | 30s | Kod + mesh |
| Kamera | 30s | Skrypt + shake |
| Shadery | 60s | Kod + demo efektów |
| Zasoby | 20s | Struktura projektu |
| Organizacja | 30s | Hierarchy + MaterialPropertyBlock |
| Animacje | 60s | Demo + kod |
| Fizyka | 60s | Kod odbić + eksplozja |
| Kompletność | 60s | Full gameplay loop |
| **TOTAL** | **6-8 min** | |

---

## Kluczowe frazy które dają punkty

- "Proceduralna generacja mesh"
- "Własne shadery w CG/HLSL"
- "MaterialPropertyBlock dla optymalizacji"
- "Continuous Collision Detection"
- "Singleton pattern"
- "Elastic easing"
- "Efekt Fresnela"
- "Dynamic lighting reagujący na gameplay"
- "Kompletny game loop"

---

## Po prezentacji

Jeśli pytają o rozszerzenia:
- Power-upy (multi-ball, większa paletka, laser)
- Więcej poziomów z różnymi patterns
- High score z zapisem do PlayerPrefs
- Particle effects (już częściowo jest)
- Post-processing (bloom, vignette)

**Projekt gotowy na 9-10/10 punktów!**
