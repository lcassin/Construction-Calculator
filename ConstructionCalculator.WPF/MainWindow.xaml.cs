using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConstructionCalculator.WPF;

/// <summary>
/// </summary>
public partial class MainWindow : Window
{
    private bool isDecimalMode = false;
    private string currentOperation = "";
    private Measurement? storedValue = null;
    private bool shouldClearDisplay = false;
    private readonly List<string> calculationChain = new();

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

            if (item == "+" || item == "-" || item == "*" || item == "/")
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
            return false;
        }

        return text.Contains('+') || text.Contains('-') || text.Contains('*') || text.Contains('/');
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
                Measurement nextValue = Measurement.Parse(parts[i + 1]);

                switch (op)
                {
                    case "+":
                        result += nextValue;
                        break;
                    case "-":
                        result -= nextValue;
                        break;
                    case "*":
                        result *= nextValue.ToTotalInches();
                        break;
                    case "/":
                        if (nextValue.ToTotalInches() == 0)
                        {
                            MessageBox.Show("Cannot divide by zero", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        result /= nextValue.ToTotalInches();
                        break;
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
        MessageBox.Show("Construction Calculator\nVersion 2.0 (WPF with Fluent Theme)\n\n© 2025", "About", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
