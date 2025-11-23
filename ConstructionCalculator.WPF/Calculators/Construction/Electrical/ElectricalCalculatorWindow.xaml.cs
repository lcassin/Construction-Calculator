using System.Windows;
using System.Windows.Controls;

namespace ConstructionCalculator.WPF.Calculators.Construction.Electrical;

public partial class ElectricalCalculatorWindow : Window
{
    private readonly List<CircuitData> circuits = new();

    public ElectricalCalculatorWindow()
    {
        InitializeComponent();
    }

    private void CircuitTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CircuitTypeComboBox == null) return;

        int selectedIndex = CircuitTypeComboBox.SelectedIndex;

        // Show/hide input panels based on circuit type
        if (selectedIndex == 0) // General Lighting
        {
            LightingInputPanel.Visibility = Visibility.Visible;
            ApplianceInputPanel.Visibility = Visibility.Collapsed;
            CountInputPanel.Visibility = Visibility.Collapsed;
        }
        else if (selectedIndex == 1 || selectedIndex == 2) // Small Appliance or Laundry
        {
            LightingInputPanel.Visibility = Visibility.Collapsed;
            ApplianceInputPanel.Visibility = Visibility.Collapsed;
            CountInputPanel.Visibility = Visibility.Visible;
        }
        else // All other appliances
        {
            LightingInputPanel.Visibility = Visibility.Collapsed;
            ApplianceInputPanel.Visibility = Visibility.Visible;
            CountInputPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void AddCircuitButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string circuitName = string.IsNullOrWhiteSpace(CircuitNameTextBox.Text) 
                ? CircuitTypeComboBox.Text 
                : CircuitNameTextBox.Text;
            
            int circuitTypeIndex = CircuitTypeComboBox.SelectedIndex;
            string circuitType = CircuitTypeComboBox.Text;
            
            CircuitData circuit = circuitTypeIndex switch
            {
                0 => CreateLightingCircuit(circuitName),
                1 => CreateSmallApplianceCircuit(circuitName),
                2 => CreateLaundryCircuit(circuitName),
                _ => CreateApplianceCircuit(circuitName, circuitType)
            };

            circuits.Add(circuit);

            string displayText = $"{circuit.Name}: {circuit.Load:F0} VA | {circuit.Voltage}V | {circuit.Amperage:F1}A | Wire: {circuit.WireSize} | Breaker: {circuit.BreakerSize}A";
            CircuitsListBox.Items.Add(displayText);

            UpdatePanelLoadSummary();

            CircuitNameTextBox.Clear();
            SquareFootageTextBox.Clear();
            LoadWattsTextBox.Clear();
            CircuitCountTextBox.Text = "2";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private CircuitData CreateLightingCircuit(string name)
    {
        if (!double.TryParse(SquareFootageTextBox.Text, out double sqft) || sqft <= 0)
        {
            throw new ArgumentException("Please enter valid square footage.");
        }

        // NEC 220.12: General lighting load is 3 VA per square foot for dwelling units
        double load = sqft * 3.0;
        int voltage = 120;
        double amperage = load / voltage;
        
        // Determine wire size and breaker based on amperage
        (string wireSize, int breakerSize) = DetermineWireAndBreaker(amperage, voltage, 50, true);

        return new CircuitData
        {
            Name = name,
            Type = "General Lighting",
            Load = load,
            Voltage = voltage,
            Amperage = amperage,
            WireSize = wireSize,
            BreakerSize = breakerSize,
            DemandFactor = 1.0 // Will be applied in total calculation
        };
    }

    private CircuitData CreateSmallApplianceCircuit(string name)
    {
        if (!int.TryParse(CircuitCountTextBox.Text, out int count) || count <= 0)
        {
            throw new ArgumentException("Please enter valid number of circuits.");
        }

        // NEC 220.52(A): Small appliance branch circuits - minimum 1500 VA each
        double load = 1500.0 * count;
        int voltage = 120;
        double amperage = load / voltage;

        return new CircuitData
        {
            Name = $"{name} ({count} circuits)",
            Type = "Small Appliance",
            Load = load,
            Voltage = voltage,
            Amperage = amperage,
            WireSize = "12 AWG",
            BreakerSize = 20,
            DemandFactor = 1.0
        };
    }

    private CircuitData CreateLaundryCircuit(string name)
    {
        if (!int.TryParse(CircuitCountTextBox.Text, out int count) || count <= 0)
        {
            throw new ArgumentException("Please enter valid number of circuits.");
        }

        // NEC 220.52(B): Laundry branch circuit - minimum 1500 VA
        double load = 1500.0 * count;
        int voltage = 120;
        double amperage = load / voltage;

        return new CircuitData
        {
            Name = $"{name} ({count} circuits)",
            Type = "Laundry",
            Load = load,
            Voltage = voltage,
            Amperage = amperage,
            WireSize = "12 AWG",
            BreakerSize = 20,
            DemandFactor = 1.0
        };
    }

    private CircuitData CreateApplianceCircuit(string name, string type)
    {
        if (!double.TryParse(LoadWattsTextBox.Text, out double load) || load <= 0)
        {
            throw new ArgumentException("Please enter valid load in watts or VA.");
        }

        int voltage = VoltageComboBox.SelectedIndex == 0 ? 120 : 240;
        
        // Special handling for EV Charger per NEC 2023
        if (type.Contains("EV Charger"))
        {
            // NEC 220.57: EVSE load must be larger of 7200 VA or nameplate rating
            load = Math.Max(load, 7200);
        }

        double amperage = load / voltage;
        (string wireSize, int breakerSize) = DetermineWireAndBreaker(amperage, voltage, 50, true);

        // Determine demand factor based on appliance type
        double demandFactor = type switch
        {
            string s when s.Contains("Range") || s.Contains("Oven") => 0.8, // NEC Table 220.55
            string s when s.Contains("Dryer") => 1.0, // NEC 220.54
            string s when s.Contains("Water Heater") => 1.0,
            string s when s.Contains("HVAC") || s.Contains("Heat Pump") => 1.0,
            string s when s.Contains("EV Charger") => 1.0,
            _ => 1.0
        };

        return new CircuitData
        {
            Name = name,
            Type = type,
            Load = load,
            Voltage = voltage,
            Amperage = amperage,
            WireSize = wireSize,
            BreakerSize = breakerSize,
            DemandFactor = demandFactor
        };
    }

    private (string wireSize, int breakerSize) DetermineWireAndBreaker(double amperage, int voltage, double length, bool isCopper)
    {
        // Add 25% for continuous loads per NEC 210.19(A)(1)
        double adjustedAmperage = amperage * 1.25;

        // Determine breaker size (round up to standard sizes)
        int[] standardBreakers = { 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 110, 125, 150, 175, 200, 225, 250, 300, 350, 400 };
        int breakerSize = standardBreakers.FirstOrDefault(b => b >= adjustedAmperage, 400);

        // Determine wire size based on ampacity (NEC Table 310.16 for copper at 75°C)
        string wireSize = isCopper ? GetCopperWireSize(breakerSize) : GetAluminumWireSize(breakerSize);

        return (wireSize, breakerSize);
    }

    private string GetCopperWireSize(int breakerSize)
    {
        // Based on NEC Table 310.16 - Copper conductors at 75°C
        return breakerSize switch
        {
            <= 15 => "14 AWG",
            <= 20 => "12 AWG",
            <= 30 => "10 AWG",
            <= 40 => "8 AWG",
            <= 55 => "6 AWG",
            <= 70 => "4 AWG",
            <= 85 => "3 AWG",
            <= 95 => "2 AWG",
            <= 110 => "1 AWG",
            <= 125 => "1/0 AWG",
            <= 150 => "2/0 AWG",
            <= 175 => "3/0 AWG",
            <= 200 => "4/0 AWG",
            <= 230 => "250 kcmil",
            <= 255 => "300 kcmil",
            <= 285 => "350 kcmil",
            <= 310 => "400 kcmil",
            <= 335 => "500 kcmil",
            _ => "600 kcmil"
        };
    }

    private string GetAluminumWireSize(int breakerSize)
    {
        // Based on NEC Table 310.16 - Aluminum conductors at 75°C
        return breakerSize switch
        {
            <= 15 => "12 AWG",
            <= 25 => "10 AWG",
            <= 35 => "8 AWG",
            <= 45 => "6 AWG",
            <= 55 => "4 AWG",
            <= 65 => "3 AWG",
            <= 75 => "2 AWG",
            <= 85 => "1 AWG",
            <= 100 => "1/0 AWG",
            <= 115 => "2/0 AWG",
            <= 135 => "3/0 AWG",
            <= 155 => "4/0 AWG",
            <= 180 => "250 kcmil",
            <= 205 => "300 kcmil",
            <= 230 => "350 kcmil",
            <= 255 => "400 kcmil",
            <= 280 => "500 kcmil",
            _ => "600 kcmil"
        };
    }

    private void UpdatePanelLoadSummary()
    {
        if (circuits.Count == 0)
        {
            TotalConnectedLoadLabel.Text = "Total Connected Load: 0 VA";
            DemandLoadLabel.Text = "Demand Load (with NEC factors): 0 VA";
            RequiredAmpsLabel.Text = "Required Amperage: 0 A";
            RecommendedPanelLabel.Text = "Recommended Panel: Not calculated";
            MainBreakerLabel.Text = "Main Breaker Size: Not calculated";
            DemandFactorsLabel.Text = "No circuits added yet";
            return;
        }

        double totalConnectedLoad = circuits.Sum(c => c.Load);

        // Apply NEC demand factors per Article 220
        double demandLoad = CalculateDemandLoad();

        // Calculate required amperage at 240V (standard service voltage)
        double requiredAmps = demandLoad / 240.0;

        // Determine recommended panel size
        int[] standardPanelSizes = { 100, 125, 150, 200, 225, 300, 400 };
        int recommendedPanel = standardPanelSizes.FirstOrDefault(p => p >= requiredAmps, 400);

        // Main breaker is typically same as panel rating
        int mainBreaker = recommendedPanel;

        TotalConnectedLoadLabel.Text = $"Total Connected Load: {totalConnectedLoad:F0} VA";
        DemandLoadLabel.Text = $"Demand Load (with NEC factors): {demandLoad:F0} VA";
        RequiredAmpsLabel.Text = $"Required Amperage: {requiredAmps:F1} A @ 240V";
        RecommendedPanelLabel.Text = $"Recommended Panel: {recommendedPanel}A Service Panel";
        MainBreakerLabel.Text = $"Main Breaker Size: {mainBreaker}A (2-pole)";

        // Show demand factors applied
        DemandFactorsLabel.Text = GetDemandFactorsDescription();
    }

    private double CalculateDemandLoad()
    {
        double demandLoad = 0;

        // Separate loads by type for proper demand factor application
        double lightingLoad = circuits.Where(c => c.Type == "General Lighting").Sum(c => c.Load);
        double smallApplianceLoad = circuits.Where(c => c.Type == "Small Appliance").Sum(c => c.Load);
        double laundryLoad = circuits.Where(c => c.Type == "Laundry").Sum(c => c.Load);
        double applianceLoad = circuits.Where(c => c.Type != "General Lighting" && c.Type != "Small Appliance" && c.Type != "Laundry").Sum(c => c.Load * c.DemandFactor);

        // NEC 220.42: Apply demand factors to general lighting and receptacles
        double generalLoad = lightingLoad + smallApplianceLoad + laundryLoad;
        
        if (generalLoad <= 3000)
        {
            demandLoad += generalLoad;
        }
        else if (generalLoad <= 120000)
        {
            demandLoad += 3000 + (generalLoad - 3000) * 0.35; // 35% demand factor for load over 3000 VA
        }
        else
        {
            demandLoad += 3000 + (117000 * 0.35) + (generalLoad - 120000) * 0.25; // 25% for load over 120000 VA
        }

        // Add appliance loads (already have demand factors applied)
        demandLoad += applianceLoad;

        return demandLoad;
    }

    private string GetDemandFactorsDescription()
    {
        var factors = new List<string>();

        if (circuits.Any(c => c.Type == "General Lighting" || c.Type == "Small Appliance" || c.Type == "Laundry"))
        {
            factors.Add("General lighting/receptacles: 100% first 3kVA, 35% next 117kVA (NEC 220.42)");
        }

        if (circuits.Any(c => c.Type.Contains("Range") || c.Type.Contains("Oven")))
        {
            factors.Add("Electric range: 80% demand factor (NEC Table 220.55)");
        }

        if (circuits.Any(c => c.Type.Contains("EV Charger")))
        {
            factors.Add("EV Charger: Minimum 7200 VA or nameplate rating (NEC 220.57)");
        }

        return factors.Count > 0 ? string.Join("\n", factors) : "Standard NEC demand factors applied per Article 220";
    }

    private void RemoveCircuitButton_Click(object sender, RoutedEventArgs e)
    {
        if (CircuitsListBox.SelectedIndex >= 0)
        {
            int index = CircuitsListBox.SelectedIndex;
            circuits.RemoveAt(index);
            CircuitsListBox.Items.RemoveAt(index);
            UpdatePanelLoadSummary();
        }
        else
        {
            MessageBox.Show("Please select a circuit to remove.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ClearAllCircuitsButton_Click(object sender, RoutedEventArgs e)
    {
        circuits.Clear();
        CircuitsListBox.Items.Clear();
        UpdatePanelLoadSummary();
    }

    private void CalculateWireSizeButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            double load;
            double amperage;
            
            if (WireSizingUnitComboBox.SelectedIndex == 0) // Watts
            {
                if (!double.TryParse(WireSizingLoadTextBox.Text, out load) || load <= 0)
                {
                    MessageBox.Show("Please enter a valid load in watts.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                int voltage = WireSizingVoltageComboBox.SelectedIndex == 0 ? 120 : 240;
                amperage = load / voltage;
            }
            else // Amps
            {
                if (!double.TryParse(WireSizingLoadTextBox.Text, out amperage) || amperage <= 0)
                {
                    MessageBox.Show("Please enter a valid load in amps.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (!double.TryParse(WireLengthTextBox.Text, out double length) || length <= 0)
            {
                MessageBox.Show("Please enter a valid wire length.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int voltage = WireSizingVoltageComboBox.SelectedIndex == 0 ? 120 : 240;
            bool isCopper = WireTypeComboBox.SelectedIndex == 0;

            (string wireSize, int breakerSize) = DetermineWireAndBreaker(amperage, voltage, length, isCopper);

            // Calculate voltage drop
            double voltageDrop = CalculateVoltageDrop(amperage, length, wireSize, voltage, isCopper);
            double voltageDropPercent = (voltageDrop / voltage) * 100;

            WireSizeResultLabel.Text = $"Wire Size: {wireSize} ({WireTypeComboBox.Text})";
            BreakerSizeResultLabel.Text = $"Breaker Size: {breakerSize}A";
            
            string voltageDropWarning = voltageDropPercent > 3 ? " (⚠ Exceeds 3% NEC recommendation)" : " (✓ Within NEC 3% recommendation)";
            VoltageDropLabel.Text = $"Voltage Drop: {voltageDrop:F2}V ({voltageDropPercent:F2}%){voltageDropWarning}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private double CalculateVoltageDrop(double amperage, double length, string wireSize, int voltage, bool isCopper)
    {
        // Simplified voltage drop calculation: VD = 2 × K × I × L / CM
        // K = 12.9 for copper, 21.2 for aluminum (ohms per circular mil foot)
        // 2 accounts for round trip (hot and neutral)
        
        double k = isCopper ? 12.9 : 21.2;
        double circularMils = GetCircularMils(wireSize);
        
        // Voltage drop = 2 × K × I × L / CM
        double voltageDrop = (2 * k * amperage * length) / circularMils;
        
        return voltageDrop;
    }

    private double GetCircularMils(string wireSize)
    {
        // Circular mil values for common wire sizes
        return wireSize switch
        {
            "14 AWG" => 4110,
            "12 AWG" => 6530,
            "10 AWG" => 10380,
            "8 AWG" => 16510,
            "6 AWG" => 26240,
            "4 AWG" => 41740,
            "3 AWG" => 52620,
            "2 AWG" => 66360,
            "1 AWG" => 83690,
            "1/0 AWG" => 105600,
            "2/0 AWG" => 133100,
            "3/0 AWG" => 167800,
            "4/0 AWG" => 211600,
            "250 kcmil" => 250000,
            "300 kcmil" => 300000,
            "350 kcmil" => 350000,
            "400 kcmil" => 400000,
            "500 kcmil" => 500000,
            "600 kcmil" => 600000,
            _ => 10380 // Default to 10 AWG
        };
    }

    private void CopyResultsButton_Click(object sender, RoutedEventArgs e)
    {
        if (circuits.Count == 0)
        {
            MessageBox.Show("Please add at least one circuit first.", "No Circuits", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        string results = "Electrical Load Calculation Results (NEC 2023)\n";
        results += "==============================================\n\n";

        results += "CIRCUITS:\n";
        foreach (var circuit in circuits)
        {
            results += $"{circuit.Name}:\n";
            results += $"  Type: {circuit.Type}\n";
            results += $"  Load: {circuit.Load:F0} VA\n";
            results += $"  Voltage: {circuit.Voltage}V\n";
            results += $"  Amperage: {circuit.Amperage:F1}A\n";
            results += $"  Wire Size: {circuit.WireSize}\n";
            results += $"  Breaker: {circuit.BreakerSize}A\n\n";
        }

        results += "PANEL LOAD SUMMARY:\n";
        results += $"{TotalConnectedLoadLabel.Text}\n";
        results += $"{DemandLoadLabel.Text}\n";
        results += $"{RequiredAmpsLabel.Text}\n";
        results += $"{RecommendedPanelLabel.Text}\n";
        results += $"{MainBreakerLabel.Text}\n\n";

        results += "NEC DEMAND FACTORS:\n";
        results += $"{DemandFactorsLabel.Text}\n\n";

        results += "Note: Calculations based on NEC 2023. Always consult with a licensed electrician and local codes.";

        Clipboard.SetText(results);
        MessageBox.Show("Results copied to clipboard!", "Copied", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private class CircuitData
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public double Load { get; set; }
        public int Voltage { get; set; }
        public double Amperage { get; set; }
        public string WireSize { get; set; } = "";
        public int BreakerSize { get; set; }
        public double DemandFactor { get; set; }
    }
}
