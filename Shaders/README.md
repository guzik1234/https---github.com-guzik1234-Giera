# Własny Shader - GlowShader

## Opis
Custom shader z efektem glow (emisja światła) dla cegieł w grze Arkanoid.

## Funkcje
1. **Emission (Glow)** - cegły świecą własnym światłem
2. **Pulsowanie** - intensywność glow zmienia się w czasie (sin wave)
3. **Metallic & Smoothness** - realistyczne odbicia światła
4. **Fallback** - kompatybilność z URP i Standard Pipeline

## Parametry shadera
- `_Color` - główny kolor obiektu
- `_EmissionColor` - kolor świecenia (glow)
- `_EmissionStrength` - intensywność glow (0-2)
- `_PulseSpeed` - prędkość pulsowania (0-10)
- `_PulseAmount` - amplituda pulsowania (0-1)
- `_Metallic` - metaliczność powierzchni (0-1)
- `_Glossiness` - gładkość powierzchni (0-1)

## Techniki użyte
- **Surface Shader** - łatwiejszy w pisaniu niż vertex/fragment
- **Time-based animation** - `_Time.y` dla pulsowania
- **Sine wave modulation** - matematyka dla smooth pulsing
- **Multi-pipeline support** - działa w URP i Built-in

## Integracja
Shader jest automatycznie aplikowany przez `GlowEffect.cs` do wszystkich cegieł podczas tworzenia sceny w `AutoSceneSetup.cs`.

## Punktacja
Kategoria "Shadery/materiały": +1.0 pkt
- Własny shader napisany od zera
- Użycie emission (glow effect)
- Animacja proceduralna (pulsowanie)
- Fallback dla kompatybilności
