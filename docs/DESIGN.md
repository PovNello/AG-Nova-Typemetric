# UI & Design System

Typemetric's user interface is built heavily on WPF grids and vectors, specifically engineered to avoid hardcoded constraints where possible to allow dynamic resizing.

## Responsive Design

- The `MainWindow.xaml` uses `SizeToContent="WidthAndHeight"` coupled with a `MinWidth` and `MinHeight`.
- This ensures the window precisely encapsulates the 100% scale `VisualKeyboard` without clipping or infinitely expanding via `Viewbox`.

## Heatmap & Thermal Interpolation

The application features a 100% physical scale representation of a standard 104-key layout (`VisualKeyboard.xaml`) and a 3-button mouse (`VisualMouse.xaml`).

### Logarithmic Scale
Instead of calculating a linear percentage, `HeatmapRenderer.cs` uses a logarithmic formula:
`ratio = Math.Log(count + 1) / Math.Log(maxCount + 1)`
This normalizes outliers like `Spacebar` and `Enter`, which would otherwise dwarf regular alphabet keys and render the rest of the board visually "cold".

### Continuous RGB Gradient (Thermal Tech)
Colors are not discrete buckets. The `HeatmapRenderer` linearly interpolates RGB channels across 5 segments based on the log ratio, producing thousands of potential shades:

1. **Base (0.0)**: Deep Blue (`#142437`)
2. **Low (0.25)**: Muted Blue Transition (`#314B63`)
3. **Moderate (0.50)**: Cyan (`#8FD8E0`)
4. **High (0.75)**: Ivory / Pale Yellow (`#DCCFAA`)
5. **Very High (0.85)**: Orange (`#E6822F`)
6. **Peak (1.00)**: Danger Red (`#D56756`)

This creates an extremely granular and organic heat distribution visually representing peripheral wear over time.
