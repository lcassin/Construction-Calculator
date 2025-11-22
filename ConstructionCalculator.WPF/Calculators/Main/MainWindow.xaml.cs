using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ConstructionCalculator.WPF.Calculators.UnitConverter;
using ConstructionCalculator.WPF.Calculators.Construction.Concrete;
using ConstructionCalculator.WPF.Calculators.Construction.Stair;
using ConstructionCalculator.WPF.Calculators.Construction.Ramp;
using ConstructionCalculator.WPF.Calculators.Construction.Drywall;
using ConstructionCalculator.WPF.Calculators.Construction.Grading;
using ConstructionCalculator.WPF.Calculators.Construction.HVAC;
using ConstructionCalculator.WPF.Calculators.Construction.Plumbing;
using ConstructionCalculator.WPF.Calculators.Geometry.Area;
using ConstructionCalculator.WPF.Calculators.Geometry.Angle;
using ConstructionCalculator.WPF.Calculators.Geometry.Roofing;
using ConstructionCalculator.WPF.Calculators.Materials.Paint;
using ConstructionCalculator.WPF.Calculators.Materials.BoardFeet;
using ConstructionCalculator.WPF.Calculators.Materials.Flooring;
using ConstructionCalculator.WPF.Calculators.Survey;
using ConstructionCalculator.WPF.Calculators.SeatingLayout;
using ConstructionCalculator.WPF.Shared.HelpSystem;
using ConstructionCalculator.WPF.Launcher;
using ConstructionCalculator.WPF.About;

namespace ConstructionCalculator.WPF.Calculators.Main;

/// <summary>
/// </summary>
public partial class MainWindow : Window
{
    private bool isDecimalMode = false;
    private string currentOperation = "";
    private Measurement? storedValue = null;
    private bool shouldClearDisplay = false;
    private readonly List<string> calculationChain = new();
    private Measurement? memoryValue = null;

    public MainWindow()
    {
        InitializeComponent();
        DisplayTextBox.Focus();
        DisplayTextBox.SelectAll();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        string buttonText = btn.Content.ToString() ?? "";

        try
        {
            if (buttonText == "C")
            {
                Clear();
            }
            else if (buttonText == "CE")
            {
                ClearEntry();
            }
            else if (buttonText == "Mode")
            {
                ToggleMode();
            }
            else if (buttonText == "Copy")
            {
                Clipboard.SetText(DisplayTextBox.Text);
                FocusDisplayAtEnd();
            }
            else if (buttonText == "Space")
            {
                AppendToDisplay(" ");
            }
            else if (buttonText == "=")
            {
                PerformCalculation();
                FocusDisplayAtEnd();
            }
            else if (buttonText == "√")
            {
                PerformSquareRoot();
            }
            else if (buttonText == "x²")
            {
                PerformSquared();
            }
            else if (buttonText == "%")
            {
                PerformPercent();
            }
            else if (buttonText == "+/-")
            {
                ToggleSign();
            }
            else if (buttonText == "MC")
            {
                MemoryClear();
            }
            else if (buttonText == "MR")
            {
                MemoryRecall();
            }
            else if (buttonText == "M+")
            {
                MemoryAdd();
            }
            else if (buttonText == "M-")
            {
                MemorySubtract();
            }
            else if (buttonText == "+" || buttonText == "-" || buttonText == "*" || buttonText == "×" || buttonText == "÷")
            {
                string operation = buttonText;
                if (buttonText == "×") operation = "*";
                if (buttonText == "÷") operation = "/";
                SetOperation(operation);
            }
            else
            {
                AppendToDisplay(buttonText);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Clear();
        }
    }

    private void Clear()
    {
        DisplayTextBox.Text = isDecimalMode ? "0" : "0\"";
        currentOperation = "";
        storedValue = null;
        shouldClearDisplay = false;
        calculationChain.Clear();
        UpdateChainDisplay();
        FocusDisplayAndSelectAll();
    }

    private void ClearEntry()
    {
        if (calculationChain.Count > 0)
        {
            calculationChain.RemoveAt(calculationChain.Count - 1);
            UpdateChainDisplay();

            if (calculationChain.Count > 0)
            {
                try
                {
                    ReplayCalculationChain();
                }
                catch
                {
                    DisplayTextBox.Text = isDecimalMode ? "0" : "0\"";
                    storedValue = null;
                    currentOperation = "";
                }
            }
            else
            {
                DisplayTextBox.Text = isDecimalMode ? "0" : "0\"";
                storedValue = null;
                currentOperation = "";
            }
        }
        else
        {
            DisplayTextBox.Text = isDecimalMode ? "0" : "0\"";
        }
        shouldClearDisplay = false;
        FocusDisplayAndSelectAll();
    }

    private void UpdateChainDisplay()
    {
        ChainLabel.Text = calculationChain.Count > 0 ? string.Join(" ", calculationChain) : "";
    }

    private void ReplayCalculationChain()
    {
        storedValue = null;
        currentOperation = "";

        for (int i = 0; i < calculationChain.Count; i++)
        {
            string item = calculationChain[i];

            if (item == "+" || item == "-" || item == "*" || item == "/" || item == "%")
            {
                currentOperation = item;
            }
            else
            {
                Measurement value = Measurement.Parse(item);

                if (storedValue == null)
                {
                    storedValue = value;
                }
                else if (!string.IsNullOrEmpty(currentOperation))
                {
                    switch (currentOperation)
                    {
                        case "+":
                            storedValue += value;
                            break;
                        case "-":
                            storedValue -= value;
                            break;
                        case "*":
                            storedValue *= value.ToTotalInches();
                            break;
                        case "/":
                            storedValue /= value.ToTotalInches();
                            break;
                        case "%":
                            storedValue *= (value.ToTotalInches() / 100.0);
                            break;
                    }
                    currentOperation = "";
                }
            }
        }

        if (storedValue != null)
        {
            UpdateDisplay(storedValue);
        }
    }

    private void ToggleMode()
    {
        isDecimalMode = !isDecimalMode;
        ModeLabel.Text = isDecimalMode ? "Mode: Decimal (inches)" : "Mode: Fractions (1/16\")";

        try
        {
            if (!string.IsNullOrEmpty(DisplayTextBox.Text) && DisplayTextBox.Text != "0" && DisplayTextBox.Text != "0\"")
            {
                Measurement current = ParseCurrentDisplay();
                UpdateDisplay(current);
            }
            else
            {
                DisplayTextBox.Text = isDecimalMode ? "0" : "0\"";
            }
        }
        catch
        {
            DisplayTextBox.Text = isDecimalMode ? "0" : "0\"";
        }

        FocusDisplayAtEnd();
    }

    private void SetOperation(string operation)
    {
        try
        {
            bool calculationPerformed = false;

            if (storedValue != null && !string.IsNullOrEmpty(DisplayTextBox.Text.Trim()) && !string.IsNullOrEmpty(currentOperation))
            {
                PerformCalculation();
                calculationPerformed = true;
            }

            Measurement currentValue = ParseCurrentDisplay();
            string displayText = DisplayTextBox.Text.Trim();

            if (storedValue == null || calculationChain.Count == 0)
            {
                calculationChain.Add(displayText);
            }
            else if (!shouldClearDisplay)
            {
                calculationChain.Add(displayText);
            }

            calculationChain.Add(operation);
            UpdateChainDisplay();

            if (!calculationPerformed)
            {
                storedValue = currentValue;
            }

            currentOperation = operation;
            shouldClearDisplay = true;
            DisplayTextBox.Text = "";

            FocusDisplayAtEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error parsing value: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Clear();
        }
    }

    private void PerformCalculation()
    {
        try
        {
            string displayText = DisplayTextBox.Text.Trim();

            if (displayText.EndsWith("%"))
            {
                string numStr = displayText.TrimEnd('%').Trim();
                double pctVal = Measurement.Parse(numStr).ToTotalInches() / 100.0;
                Measurement pctResult;

                if (storedValue != null && !string.IsNullOrEmpty(currentOperation))
                {
                    if (!shouldClearDisplay && calculationChain.Count > 0 &&
                        calculationChain[^1] != "+" &&
                        calculationChain[^1] != "-" &&
                        calculationChain[^1] != "*" &&
                        calculationChain[^1] != "/")
                    {
                        calculationChain.Add(displayText);
                    }
                    else if (calculationChain.Count > 0 &&
                             (calculationChain[^1] == "+" ||
                              calculationChain[^1] == "-" ||
                              calculationChain[^1] == "*" ||
                              calculationChain[^1] == "/"))
                    {
                        calculationChain.Add(displayText);
                    }
                    UpdateChainDisplay();

                    switch (currentOperation)
                    {
                        case "*":
                            pctResult = storedValue * pctVal;
                            break;
                        case "/":
                            if (pctVal == 0)
                            {
                                MessageBox.Show("Cannot divide by zero percent", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            pctResult = storedValue / pctVal;
                            break;
                        case "+":
                            pctResult = storedValue + (storedValue * pctVal);
                            break;
                        case "-":
                            pctResult = storedValue - (storedValue * pctVal);
                            break;
                        default:
                            pctResult = Measurement.FromDecimalInches(pctVal);
                            break;
                    }

                    UpdateDisplay(pctResult);
                    storedValue = pctResult;
                    currentOperation = "";
                    shouldClearDisplay = true;
                    FocusDisplayAtEnd();
                    return;
                }
                else
                {
                    pctResult = Measurement.FromDecimalInches(pctVal);
                    calculationChain.Clear();
                    calculationChain.Add(displayText);
                    UpdateChainDisplay();
                    UpdateDisplay(pctResult);
                    storedValue = pctResult;
                    currentOperation = "";
                    shouldClearDisplay = true;
                    FocusDisplayAtEnd();
                    return;
                }
            }

            if (ContainsOperator(displayText))
            {
                EvaluateExpression(displayText);
                return;
            }

            if (storedValue == null || string.IsNullOrEmpty(currentOperation))
                return;

            Measurement current = ParseCurrentDisplay();

            if (!shouldClearDisplay && calculationChain.Count > 0 &&
                calculationChain[^1] != "+" &&
                calculationChain[^1] != "-" &&
                calculationChain[^1] != "*" &&
                calculationChain[^1] != "/")
            {
                calculationChain.Add(displayText);
            }
            else if (calculationChain.Count > 0 &&
                     (calculationChain[^1] == "+" ||
                      calculationChain[^1] == "-" ||
                      calculationChain[^1] == "*" ||
                      calculationChain[^1] == "/"))
            {
                calculationChain.Add(displayText);
            }

            UpdateChainDisplay();

            Measurement result;

            switch (currentOperation)
            {
                case "+":
                    result = storedValue + current;
                    break;
                case "-":
                    result = storedValue - current;
                    break;
                case "*":
                    result = storedValue * current.ToTotalInches();
                    break;
                case "/":
                    if (current.ToTotalInches() == 0)
                    {
                        MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    result = storedValue / current.ToTotalInches();
                    break;
                case "%":
                    result = storedValue * (current.ToTotalInches() / 100.0);
                    break;
                default:
                    return;
            }

            UpdateDisplay(result);
            storedValue = result;
            currentOperation = "";
            shouldClearDisplay = true;

            FocusDisplayAtEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Calculation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Clear();
        }
    }

    private void AppendToDisplay(string text)
    {
        if (shouldClearDisplay)
        {
            DisplayTextBox.Text = "";
            shouldClearDisplay = false;
        }

        if (DisplayTextBox.Text == "0" || DisplayTextBox.Text == "0\"")
        {
            DisplayTextBox.Text = text;
        }
        else
        {
            DisplayTextBox.Text += text;
        }

        FocusDisplayAtEnd();
    }

    private void FocusDisplayAtEnd()
    {
        DisplayTextBox.Focus();
        DisplayTextBox.CaretIndex = DisplayTextBox.Text.Length;
        DisplayTextBox.SelectionLength = 0;
    }

    private void FocusDisplayAndSelectAll()
    {
        DisplayTextBox.Focus();
        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, new Action(() =>
        {
            DisplayTextBox.SelectAll();
        }));
    }

    private Measurement ParseCurrentDisplay()
    {
        string text = DisplayTextBox.Text.Trim();
        if (string.IsNullOrEmpty(text) || text == "0" || text == "0\"")
        {
            return new Measurement(0, 0, 0, 16);
        }

        return Measurement.Parse(text);
    }

    private void UpdateDisplay(Measurement measurement)
    {
        DisplayTextBox.Text = isDecimalMode ? measurement.ToDecimalString() : measurement.ToFractionString();
    }

    private bool ContainsOperator(string text)
    {
        bool hasQuotes = text.Contains("'") || text.Contains("\"");

        if (hasQuotes)
        {
            bool lastWasQuote = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '"' || c == '\'')
                {
                    lastWasQuote = true;
                    continue;
                }

                if (lastWasQuote && char.IsWhiteSpace(c))
                {
                    continue;
                }

                if (lastWasQuote && (c == '+' || c == '-' || c == '*' || c == '/'))
                {
                    return true;
                }

                lastWasQuote = false;
            }
            
            if (text.Contains('%'))
                return true;
                
            return false;
        }

        return text.Contains('+') || text.Contains('-') || text.Contains('*') || text.Contains('/') || text.Contains('%');
    }

    private bool IsFractionNotDivision(string text, int slashIndex)
    {
        if (slashIndex <= 0 || slashIndex >= text.Length - 1)
            return false;

        bool hasDigitBefore = false;
        bool hasDigitAfter = false;

        for (int i = slashIndex - 1; i >= 0; i--)
        {
            if (char.IsDigit(text[i]))
            {
                hasDigitBefore = true;
                break;
            }
            if (!char.IsWhiteSpace(text[i]) && text[i] != '-')
                break;
        }

        for (int i = slashIndex + 1; i < text.Length; i++)
        {
            if (char.IsDigit(text[i]))
            {
                hasDigitAfter = true;
                break;
            }
            if (!char.IsWhiteSpace(text[i]))
                break;
        }

        return hasDigitBefore && hasDigitAfter;
    }

    private void EvaluateExpression(string expression)
    {
        try
        {
            string[] operators = { "+", "-", "*", "/" };
            List<string> parts = new();
            string currentPart = "";

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if ((c == '+' || c == '-' || c == '*' || c == '/'))
                {
                    if (c == '/' && IsFractionNotDivision(expression, i))
                    {
                        currentPart += c;
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(currentPart))
                    {
                        parts.Add(currentPart.Trim());
                        parts.Add(c.ToString());
                        currentPart = "";
                    }
                }
                else
                {
                    currentPart += c;
                }
            }

            if (!string.IsNullOrWhiteSpace(currentPart))
            {
                parts.Add(currentPart.Trim());
            }

            if (parts.Count == 0)
                return;

            Measurement result = Measurement.Parse(parts[0]);

            for (int i = 1; i < parts.Count; i += 2)
            {
                if (i + 1 >= parts.Count)
                    break;

                string op = parts[i];
                string nextValueStr = parts[i + 1];
                
                bool isPercent = nextValueStr.TrimEnd().EndsWith("%");
                if (isPercent)
                {
                    nextValueStr = nextValueStr.TrimEnd().TrimEnd('%').Trim();
                }
                
                Measurement nextValue = Measurement.Parse(nextValueStr);
                double nextValueInches = nextValue.ToTotalInches();
                
                if (isPercent)
                {
                    double pct = nextValueInches / 100.0;
                    
                    switch (op)
                    {
                        case "+":
                            result = result + (result * pct);
                            break;
                        case "-":
                            result = result - (result * pct);
                            break;
                        case "*":
                            result = result * pct;
                            break;
                        case "/":
                            if (pct == 0)
                            {
                                MessageBox.Show("Cannot divide by zero percent", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            result = result / pct;
                            break;
                    }
                }
                else
                {
                    switch (op)
                    {
                        case "+":
                            result += nextValue;
                            break;
                        case "-":
                            result -= nextValue;
                            break;
                        case "*":
                            result *= nextValueInches;
                            break;
                        case "/":
                            if (nextValueInches == 0)
                            {
                                MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            result /= nextValueInches;
                            break;
                    }
                }
            }

            calculationChain.Clear();
            calculationChain.Add(expression);
            UpdateChainDisplay();

            UpdateDisplay(result);
            storedValue = result;
            currentOperation = "";
            shouldClearDisplay = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Expression evaluation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Clear();
        }
    }

    private void DisplayTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            e.Handled = true;
            PerformCalculation();
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
        {
            Clipboard.SetText(DisplayTextBox.Text);
            e.Handled = true;
        }
        else if (e.Key == Key.Add || (e.Key == Key.OemPlus && Keyboard.Modifiers == ModifierKeys.Control))
        {
            SetOperation("+");
            e.Handled = true;
        }
        else if (e.Key == Key.Subtract || (e.Key == Key.OemMinus && Keyboard.Modifiers == ModifierKeys.Control))
        {
            SetOperation("-");
            e.Handled = true;
        }
        else if (e.Key == Key.Multiply || (e.Key == Key.D8 && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift)))
        {
            SetOperation("*");
            e.Handled = true;
        }
        else if (e.Key == Key.Divide || (e.Key == Key.OemQuestion && Keyboard.Modifiers == ModifierKeys.Control))
        {
            SetOperation("/");
            e.Handled = true;
        }
        else if (e.Key == Key.M && Keyboard.Modifiers == ModifierKeys.Control)
        {
            ToggleMode();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            Clear();
            e.Handled = true;
        }
        else if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
        {
            ClearEntry();
            e.Handled = true;
        }
    }

    private void AngleCalculator_Click(object sender, RoutedEventArgs e)
    {
        var angleCalculator = new AngleCalculatorWindow { Owner = this };
        angleCalculator.ShowDialog();
    }

    private void StairCalculator_Click(object sender, RoutedEventArgs e)
    {
        var stairCalculator = new StairCalculatorWindow { Owner = this };
        stairCalculator.ShowDialog();
    }

    private void RampCalculator_Click(object sender, RoutedEventArgs e)
    {
        var rampCalculator = new RampCalculatorWindow { Owner = this };
        rampCalculator.ShowDialog();
    }

    private void SurveyCalculator_Click(object sender, RoutedEventArgs e)
    {
        var surveyCalculator = new SurveyCalculatorWindow { Owner = this };
        surveyCalculator.ShowDialog();
    }

    private void SeatingLayoutCalculator_Click(object sender, RoutedEventArgs e)
    {
        var seatingLayoutCalculator = new SeatingLayoutCalculatorWindow { Owner = this };
        seatingLayoutCalculator.ShowDialog();
    }

    private void AreaCalculator_Click(object sender, RoutedEventArgs e)
    {
        var areaCalculator = new AreaCalculatorWindow { Owner = this };
        areaCalculator.ShowDialog();
    }

    private void UnitConverter_Click(object sender, RoutedEventArgs e)
    {
        var unitConverter = new UnitConverterWindow { Owner = this };
        unitConverter.ShowDialog();
    }

    private void CalculatorLauncher_Click(object sender, RoutedEventArgs e)
    {
        var launcher = new CalculatorLauncherWindow { Owner = this };
        launcher.ShowDialog();
    }

    private void ConcreteCalculator_Click(object sender, RoutedEventArgs e)
    {
        var concreteCalculator = new ConcreteCalculatorWindow { Owner = this };
        concreteCalculator.ShowDialog();
    }

    private void RoofingCalculator_Click(object sender, RoutedEventArgs e)
    {
        var roofingCalculator = new RoofingCalculatorWindow { Owner = this };
        roofingCalculator.ShowDialog();
    }

    private void PaintCalculator_Click(object sender, RoutedEventArgs e)
    {
        var paintCalculator = new PaintCalculatorWindow { Owner = this };
        paintCalculator.ShowDialog();
    }

    private void BoardFeetCalculator_Click(object sender, RoutedEventArgs e)
    {
        var boardFeetCalculator = new BoardFeetCalculatorWindow { Owner = this };
        boardFeetCalculator.ShowDialog();
    }

    private void DrywallCalculator_Click(object sender, RoutedEventArgs e)
    {
        var drywallCalculator = new DrywallCalculatorWindow { Owner = this };
        drywallCalculator.ShowDialog();
    }

    private void GradingCalculator_Click(object sender, RoutedEventArgs e)
    {
        var gradingCalculator = new GradingCalculatorWindow { Owner = this };
        gradingCalculator.ShowDialog();
    }

    private void HVACCalculator_Click(object sender, RoutedEventArgs e)
    {
        var hvacCalculator = new HVACCalculatorWindow { Owner = this };
        hvacCalculator.ShowDialog();
    }

    private void PlumbingCalculator_Click(object sender, RoutedEventArgs e)
    {
        var plumbingCalculator = new PlumbingCalculatorWindow { Owner = this };
        plumbingCalculator.ShowDialog();
    }

    private void FlooringCalculator_Click(object sender, RoutedEventArgs e)
    {
        var flooringCalculator = new FlooringCalculatorWindow { Owner = this };
        flooringCalculator.ShowDialog();
    }

    private void LightTheme_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.ThemeMode = ThemeMode.Light;
    }

    private void DarkTheme_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.ThemeMode = ThemeMode.Dark;
    }

    private void SystemTheme_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.ThemeMode = ThemeMode.System;
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow { Owner = this };
        aboutWindow.ShowDialog();
    }

    private void HelpThisCalculator_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Main) { Owner = this };
        helpWindow.Show();
    }

    private void HelpMain_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Main) { Owner = this };
        helpWindow.Show();
    }

    private void HelpUnitConverter_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.UnitConverter) { Owner = this };
        helpWindow.Show();
    }

    private void HelpArea_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Area) { Owner = this };
        helpWindow.Show();
    }

    private void HelpAngle_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Angle) { Owner = this };
        helpWindow.Show();
    }

    private void HelpSurvey_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Survey) { Owner = this };
        helpWindow.Show();
    }

    private void HelpSeatingLayout_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.SeatingLayout) { Owner = this };
        helpWindow.Show();
    }

    private void HelpConcrete_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Concrete) { Owner = this };
        helpWindow.Show();
    }

    private void HelpStair_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Stair) { Owner = this };
        helpWindow.Show();
    }

    private void HelpRamp_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Ramp) { Owner = this };
        helpWindow.Show();
    }

    private void HelpRoofing_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Roofing) { Owner = this };
        helpWindow.Show();
    }

    private void HelpPaint_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Paint) { Owner = this };
        helpWindow.Show();
    }

    private void HelpBoardFeet_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.BoardFeet) { Owner = this };
        helpWindow.Show();
    }

    private void HelpDrywall_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Drywall) { Owner = this };
        helpWindow.Show();
    }

    private void HelpGrading_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Grading) { Owner = this };
        helpWindow.Show();
    }

    private void HelpHVAC_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.HVAC) { Owner = this };
        helpWindow.Show();
    }

    private void HelpPlumbing_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Plumbing) { Owner = this };
        helpWindow.Show();
    }

    private void HelpFlooring_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new HelpWindow(CalculatorKind.Flooring) { Owner = this };
        helpWindow.Show();
    }

    private void PerformSquareRoot()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            double totalInches = current.ToTotalInches();

            if (totalInches < 0)
            {
                MessageBox.Show("Cannot calculate square root of negative value", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double sqrtInches = Math.Sqrt(totalInches);
            Measurement result = Measurement.FromDecimalInches(sqrtInches);

            calculationChain.Clear();
            calculationChain.Add($"√({DisplayTextBox.Text.Trim()})");
            UpdateChainDisplay();

            UpdateDisplay(result);
            storedValue = result;
            currentOperation = "";
            shouldClearDisplay = true;

            FocusDisplayAtEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Square root error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Clear();
        }
    }

    private void PerformSquared()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            double totalInches = current.ToTotalInches();
            double squaredInches = totalInches * totalInches;
            Measurement result = Measurement.FromDecimalInches(squaredInches);

            calculationChain.Clear();
            calculationChain.Add($"({DisplayTextBox.Text.Trim()})²");
            UpdateChainDisplay();

            UpdateDisplay(result);
            storedValue = result;
            currentOperation = "";
            shouldClearDisplay = true;

            FocusDisplayAtEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Squared error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Clear();
        }
    }

    private void PerformPercent()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            double currentValue = current.ToTotalInches();
            Measurement result;

            if (storedValue != null && !string.IsNullOrEmpty(currentOperation))
            {
                switch (currentOperation)
                {
                    case "*":
                        result = storedValue * (currentValue / 100.0);
                        break;
                    case "/":
                        if (currentValue == 0)
                        {
                            MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        result = storedValue / (currentValue / 100.0);
                        break;
                    case "+":
                        result = storedValue + (storedValue * (currentValue / 100.0));
                        break;
                    case "-":
                        result = storedValue - (storedValue * (currentValue / 100.0));
                        break;
                    default:
                        result = Measurement.FromDecimalInches(currentValue / 100.0);
                        break;
                }

                string displayText = DisplayTextBox.Text.Trim();
                if (!shouldClearDisplay && calculationChain.Count > 0 &&
                    calculationChain[^1] != "+" &&
                    calculationChain[^1] != "-" &&
                    calculationChain[^1] != "*" &&
                    calculationChain[^1] != "/")
                {
                    calculationChain.Add(displayText + "%");
                }
                else if (calculationChain.Count > 0 &&
                         (calculationChain[^1] == "+" ||
                          calculationChain[^1] == "-" ||
                          calculationChain[^1] == "*" ||
                          calculationChain[^1] == "/"))
                {
                    calculationChain.Add(displayText + "%");
                }
                UpdateChainDisplay();

                UpdateDisplay(result);
                storedValue = result;
                currentOperation = "";
                shouldClearDisplay = true;
            }
            else
            {
                result = Measurement.FromDecimalInches(currentValue / 100.0);
                
                calculationChain.Clear();
                calculationChain.Add($"{DisplayTextBox.Text.Trim()}%");
                UpdateChainDisplay();

                UpdateDisplay(result);
                storedValue = result;
                currentOperation = "";
                shouldClearDisplay = true;
            }

            FocusDisplayAtEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Percentage error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Clear();
        }
    }

    private void ToggleSign()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            double totalInches = current.ToTotalInches();
            Measurement negated = Measurement.FromDecimalInches(-totalInches);
            
            UpdateDisplay(negated);
            FocusDisplayAtEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Toggle sign error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MemoryClear()
    {
        memoryValue = null;
        UpdateMemoryIndicator();
    }

    private void MemoryRecall()
    {
        if (memoryValue != null)
        {
            UpdateDisplay(memoryValue);
            shouldClearDisplay = true;
            FocusDisplayAtEnd();
        }
    }

    private void MemoryAdd()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            if (memoryValue == null)
            {
                memoryValue = current;
            }
            else
            {
                memoryValue = memoryValue + current;
            }
            UpdateMemoryIndicator();
            FocusDisplayAtEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Memory add error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MemorySubtract()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            if (memoryValue == null)
            {
                memoryValue = Measurement.FromDecimalInches(-current.ToTotalInches());
            }
            else
            {
                memoryValue = memoryValue - current;
            }
            UpdateMemoryIndicator();
            FocusDisplayAtEnd();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Memory subtract error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateMemoryIndicator()
    {
        if (memoryValue != null)
        {
            Title = "Construction Calculator [M]";
        }
        else
        {
            Title = "Construction Calculator";
        }
    }
}
