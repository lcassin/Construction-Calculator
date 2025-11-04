# Construction Calculator

A Windows Forms calculator application designed for construction and carpentry work, supporting feet-inches measurements with fractions down to 1/16 inch precision.

## Recent Updates (October 2025)

### Installer Available:
- **Windows Installer**: Easy-to-use Inno Setup installer script included
- One-click installation with Start Menu shortcuts
- Proper uninstall support through Windows "Add or Remove Programs"
- See `BUILD-INSTALLER.md` for instructions on creating the installer

### UI/UX Improvements:
- **Fixed Dialog Layouts**: Angle and Stair calculator dialogs now display correctly without overlap with MaterialSkin title bar
- **Theme Inheritance Fixed**: System Default theme now works correctly across all calculator windows
- **Improved Button Visibility**: Numeric buttons (0-9, .) now have distinctive light blue background for better differentiation
- **Larger Display Text**: Display field font increased from 20pt to 24pt for improved readability

## Features

- **Modern Material Design Theme:**
  - Clean, professional Material Design interface using MaterialSkin
  - Consistent styling across all calculator tools
  - **Theme Selection:** Choose between Light, Dark, or System Default themes
  - **System Default:** Automatically matches your Windows theme preference
  - Access theme options from Tools → Theme menu
  - BlueGrey color scheme with professional styling

- **Measurement Input Formats:**
  - Feet and inches with fractions: `23' 6 1/2"`
  - Inches only with fractions: `6 1/2"`
  - Fractions only: `1/2`
  - Decimal inches: `6.5`
  - **Full expressions:** `3' 0 7/8"+16' 11 13/16"` (type the entire expression and press Enter)

- **Display Modes:**
  - Fraction mode (default): Shows results as feet-inches-fractions (e.g., `20' 1 1/4"`)
  - Decimal mode: Shows results as decimal inches (e.g., `241.2500`)
  - Toggle between modes using the "Mode" button

- **Operations:**
  - Addition (+)
  - Subtraction (-)
  - Multiplication (*)
  - Division (/)
  - Chainable calculations (e.g., 23' 6 1/2" + 3' 5 1/4" + 5' 2")

- **Buttons:**
  - **C**: Clear all (clears display, chain, and stored values)
  - **CE**: Clear Error (removes the last entry from the calculation chain)
  - **Mode**: Toggle between fraction and decimal display
  - **Copy**: Copy the current display value to clipboard
  - **=**: Calculate result (or press Enter)

- **Keyboard Input:**
  - Type measurements directly into the display
  - Press Enter to calculate
  - All number keys and operators work (+, -, *, /, =)
  - **Type full expressions:** You can type complete expressions like `3' 0 7/8"+16' 11 13/16"` directly in the display and press Enter to calculate
  - Expressions are evaluated left-to-right (no operator precedence)

- **Keyboard Shortcuts:**
  - **Enter** or **=**: Calculate result
  - **Escape**: Clear all (C button)
  - **Ctrl+Z**: Clear last entry (CE button/undo)
  - **Ctrl+M**: Toggle Mode (fraction/decimal)
  - **Ctrl+C**: Copy current value to clipboard
  - **Ctrl++**: Addition operator
  - **Ctrl+-**: Subtraction operator
  - **Ctrl+***: Multiplication operator
  - **Ctrl+/**: Division operator

- **Automatic Fraction Simplification:**
  - 2/16 automatically displays as 1/8
  - 8/16 displays as 1/2
  - All fractions reduced to simplest form

## Building the Application

### Requirements
- .NET 8.0 SDK or later
- Windows operating system (to run the application)

### Build Instructions

1. Open a command prompt or PowerShell window
2. Navigate to the ConstructionCalculator directory:
   ```
   cd ConstructionCalculator
   ```
3. Build the application:
   ```
   dotnet build -c Release
   ```
4. The compiled executable will be in:
   ```
   bin\Release\net8.0-windows\ConstructionCalculator.exe
   ```

### Running the Application

Simply double-click `ConstructionCalculator.exe` or run it from the command line:
```
.\bin\Release\net8.0-windows\ConstructionCalculator.exe
```

## Usage Examples

### Example 1: Simple Addition
```
Input: 23' 6 1/2"
Press: +
Input: 3' 5 1/4"
Press: =
Result: 26' 11 3/4"
```

### Example 2: Subtraction
```
Input: 23' 6 1/2"
Press: -
Input: 3' 5 1/4"
Press: =
Result: 20' 1 1/4"
```

### Example 3: Chain Calculation
```
Input: 10' 6"
Press: +
Input: 5' 3 1/2"
Press: +
Input: 2' 8 1/4"
Press: =
Result: 18' 5 3/4"
```

### Example 4: Typing a Full Expression
```
Type in display: 3' 0 7/8"+16' 11 13/16"
Press: Enter (or click =)
Result: 19' 11 11/16"
```

You can type the entire calculation in one go! The calculator will automatically parse and evaluate the expression.

### Example 5: Using CE (Clear Error)
```
Input: 10'
Press: +
Input: 5'
Press: +
Input: 3' (oops, wrong value!)
Press: CE (clears the last "3'" from the chain)
Input: 8'
Press: =
Result: 23'
```

### Example 6: Decimal Mode
```
Input: 10' 6"
Press: Mode (switch to decimal)
Display shows: 126.0000 (inches)
Press: Mode (switch back to fraction)
Display shows: 10' 6"
```

### Example 7: Copy to Clipboard
```
Input: 23' 6 1/2"
Press: + 
Input: 3' 5 1/4"
Press: =
Result: 26' 11 3/4"
Press: Copy (or Ctrl+C)
The value "26' 11 3/4"" is now in your clipboard
```

## Tips

- You can type measurements directly into the display field or use keyboard shortcuts for operators
- **NEW:** Type full expressions like `3' 0 7/8"+16' 11 13/16"` and press Enter to calculate instantly
- **NEW:** Use Ctrl+operator shortcuts (Ctrl++, Ctrl+-, Ctrl+*, Ctrl+/) to fire calculation buttons without using the mouse
- The calculation chain is shown above the main display
- Fractions are automatically rounded to the nearest 1/16"
- Use CE (Ctrl+Z) to remove the last entry if you make a mistake in a chain
- Use C (Escape) to start completely over
- Switch to decimal mode (Ctrl+M) when you need precise inch measurements
- Use Copy button or Ctrl+C to copy the current total to clipboard
- **Keyboard-only workflow:** All operations can be performed entirely from the keyboard for maximum efficiency
- Expressions are evaluated left-to-right (e.g., `10+5*2` = `30`, not `20`)
- **Chaining calculations:** After getting a result, you can immediately use operators to continue calculating (e.g., result + 3' 4" + 4' 3 1/4")

## Technical Details

- Built with .NET 8.0 and Windows Forms
- Measurement precision: 1/16 inch
- Supports negative measurements
- Automatic fraction reduction using GCD (Greatest Common Divisor)
- Chainable operations maintain precision throughout calculation

## Tools Menu

### Theme Selection
Choose your preferred visual theme:
- **Light:** Clean, bright interface ideal for well-lit environments
- **Dark:** Reduced eye strain in low-light conditions, modern dark mode styling
- **System Default:** Automatically matches your Windows theme preference (Windows 10+)

Access theme options from: **Tools → Theme** and select your preferred theme.

The theme applies to all calculator windows including the main calculator, Angle Calculator, and Stair Calculator.

### Angle Calculator
Convert between rise/run ratios and angles for construction calculations:
- **Rise/Run to Angle**: Enter rise and run measurements to calculate the angle
- **Angle to Rise/Run**: Enter an angle to see the equivalent rise/run ratio (based on 12" run)
- Toggle between degrees and radians
- Useful for roof pitch, ramp slopes, and stair pitch calculations

Example: A 4/12 roof pitch = 18.43° angle

### Stair Calculator
Calculate riser heights for stairway construction:
- Enter total rise (e.g., 9' or 108")
- Enter number of steps
- See calculated riser height
- Check code compliance (typical residential: 7-7.75")

Example: 108" total rise with 15 steps = 7.2" risers (within code)

### Survey Calculator
Professional surveying and coordinate geometry tools for CAD drafters:

#### Bearing / Azimuth Converter
Convert between bearing notation (surveying) and azimuth notation (CAD/civil engineering):
- **Bearing to Azimuth**: Convert bearing format (e.g., "N 45° 30' E") to azimuth format (e.g., "045° 30' 00"")
- **Azimuth to Bearing**: Convert azimuth format (e.g., "045° 30'") to bearing format (e.g., "N 45° 30' 00" E")
- Supports all quadrants: NE, SE, SW, NW
- Accepts multiple input formats:
  - Bearing: "N 45° 30' E", "N45°30'E", "N45.5E"
  - Azimuth: "045° 30'", "45.5" (decimal degrees)

**Conversion Rules:**
- NE quadrant (0-90°): Azimuth = Bearing
- SE quadrant (90-180°): Azimuth = 180° - Bearing
- SW quadrant (180-270°): Azimuth = 180° + Bearing
- NW quadrant (270-360°): Azimuth = 360° - Bearing

**Example:** Bearing "N 45° 30' E" converts to Azimuth "045° 30' 00""

#### Coordinate Geometry
Calculate endpoint coordinates from starting point, distance, and direction (for traverse calculations and plotting survey points):
- Enter starting coordinates (Northing, Easting)
- Enter distance (supports feet-inches format like "100' 6"" or decimal feet like "100.5")
- Enter direction (azimuth like "045°" or bearing like "N45°E")
- Calculate endpoint coordinates automatically

**Calculation Method:**
- ΔNorthing = Distance × cos(azimuth)
- ΔEasting = Distance × sin(azimuth)
- End Northing = Start Northing + ΔNorthing
- End Easting = Start Easting + ΔEasting

**Example:** Starting at (5000.00 N, 2000.00 E), traveling 100.5' at azimuth 045° results in endpoint (5071.07 N, 2071.07 E)

**Benefits for CAD Drafters:**
- No need to know trigonometry formulas (sin/cos/tan)
- Quick bearing/azimuth conversions for civil drawings
- Accurate coordinate calculations for survey point locations
- All calculations formatted properly for civil engineering work
- Integrated with Construction Calculator's measurement system

### Seating Layout Calculator
Professional auditorium seating layout tool for radial seating arrangements with complex constraints:

#### Overview
Calculate precise chair positions for auditorium-style radial seating with A-B alternation pattern (chairs with/without arms) and code-compliant aisle spacing. Generates CAD-ready coordinate data for accurate construction drawings.

#### Features
- **Radial Geometry**: Calculates chair positions along curved rows at specified radii
- **A-B Pattern**: Alternates between Type A chairs (with arms) and Type B chairs (without arms)
- **Aisle Constraints**: Ensures aisle seats always have arms for safety compliance
- **Code Compliance**: Validates aisle width and maximum seats between aisles
- **CSV Export**: Generates coordinate data ready for CAD import

#### Input Parameters

**Row Configuration:**
- Number of Rows: Total rows in the seating layout
- Starting Radius (ft): Radius of the first row from center point
- Row Spacing (ft): Distance between consecutive rows
- Arc Span (degrees): Total angular coverage of seating area (e.g., 180° for semicircle)
- Center Point: Northing and Easting coordinates of the arc center

**Chair Specifications:**
- Type A Width (inches): Width of chairs with arms (typically 24")
- Type A Depth (inches): Depth of chairs with arms (typically 20")
- Type B Width (inches): Width of chairs without arms (typically 20")
- Type B Depth (inches): Depth of chairs without arms (typically 20")

**Aisle Configuration:**
- Number of Aisles: How many aisles divide the seating sections
- Aisle Width (inches): Width of each aisle (typically 36" minimum)
- Aisle Spacing: Evenly Spaced or Custom Positions

**Code Compliance:**
- Min Aisle Width (inches): Minimum allowed aisle width (building code requirement)
- Max Seats/Aisle: Maximum seats between aisles (typically 14 for exits)

#### Calculation Method

1. **Arc Geometry**: `Arc Length = radius × arc_span_radians`
2. **Available Width**: `Arc Length - (Number of Aisles × Aisle Width)`
3. **Chair Pattern Generation**:
   - Start each section with Type A (arms) at aisle
   - Alternate A-B-A-B through middle of section
   - End each section with Type A (arms) at aisle
   - Verify maximum seats per section not exceeded
4. **Coordinate Calculation**:
   - Angular position: `θ = cumulative_width / radius`
   - X coordinate: `Center_X + radius × cos(θ)`
   - Y coordinate: `Center_Y + radius × sin(θ)`
   - Rotation angle: `θ + 90°` (chairs face center)

#### Output Format (CSV)
```
Row,Seat,Type,X_Coordinate,Y_Coordinate,Rotation_Angle,Notes
1,1,A,100.234,50.123,45.00,Aisle seat
1,2,B,100.456,50.345,46.50,
1,3,A,100.678,50.567,48.00,
...
```

#### Benefits for CAD Drafters
- **Time Savings**: Eliminates manual calculation of hundreds of chair positions
- **Accuracy**: Precise coordinates ensure perfect radial alignment
- **Code Compliance**: Built-in validation prevents non-compliant layouts
- **Flexibility**: Adjustable parameters for different venue requirements
- **Export Ready**: CSV format imports directly into AutoCAD, Revit, etc.
- **Documentation**: Complete record of layout decisions and measurements

#### Example Usage
**Small Theater (10 rows, 180° arc):**
- Starting Radius: 20 ft
- Row Spacing: 3 ft
- Chair A Width: 24", Chair B Width: 20"
- 2 Aisles at 36" width
- Result: ~200 seats with optimal sightlines and code-compliant aisles

**Large Auditorium (20 rows, 120° arc):**
- Starting Radius: 30 ft
- Row Spacing: 3.5 ft
- Chair A Width: 24", Chair B Width: 20"
- 3 Aisles at 42" width
- Result: ~400 seats with multiple exit paths

## License

Free to use for any purpose.
