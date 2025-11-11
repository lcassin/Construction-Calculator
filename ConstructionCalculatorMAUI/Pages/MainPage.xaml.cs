using ConstructionCalculator.Core;

namespace ConstructionCalculatorMAUI.Pages;

public partial class MainPage : ContentPage
{
    private bool _isDecimalMode = false;
    private string _currentOperation = "";
    private Measurement? _storedValue = null;
    private bool _shouldClearDisplay = false;
    private readonly List<string> _calculationChain = new();

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnNumberClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            AppendToDisplay(button.Text);
        }
    }

    private void OnOperatorClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            SetOperation(button.Text);
        }
    }

    private void OnEqualsClicked(object sender, EventArgs e)
    {
        PerformCalculation();
        DisplayEntry.Focus();
        DisplayEntry.CursorPosition = 0;
        DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        Clear();
    }

    private void OnClearEntryClicked(object sender, EventArgs e)
    {
        ClearEntry();
    }

    private async void OnCopyClicked(object sender, EventArgs e)
    {
        await Clipboard.SetTextAsync(DisplayEntry.Text);
        DisplayEntry.Focus();
        DisplayEntry.CursorPosition = 0;
        DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
    }

    private void OnModeClicked(object sender, EventArgs e)
    {
        ToggleMode();
    }

    private void OnDisplayCompleted(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_currentOperation))
        {
            PerformCalculation();
        }
    }

    private void OnDisplayFocused(object sender, FocusEventArgs e)
    {
        DisplayEntry.CursorPosition = 0;
        DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
    }

    private void AppendToDisplay(string text)
    {
        if (_shouldClearDisplay)
        {
            DisplayEntry.Text = "";
            _shouldClearDisplay = false;
        }

        if (DisplayEntry.Text == "0" || DisplayEntry.Text == "0\"")
        {
            DisplayEntry.Text = text;
        }
        else
        {
            DisplayEntry.Text += text;
        }
    }

    private void Clear()
    {
        DisplayEntry.Text = _isDecimalMode ? "0" : "0\"";
        _currentOperation = "";
        _storedValue = null;
        _shouldClearDisplay = false;
        _calculationChain.Clear();
        UpdateChainDisplay();

        DisplayEntry.Focus();
        DisplayEntry.CursorPosition = 0;
        DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
    }

    private void ClearEntry()
    {
        if (_calculationChain.Count > 0)
        {
            _calculationChain.RemoveAt(_calculationChain.Count - 1);
            UpdateChainDisplay();

            if (_calculationChain.Count > 0)
            {
                try
                {
                    ReplayCalculationChain();
                }
                catch
                {
                    DisplayEntry.Text = _isDecimalMode ? "0" : "0\"";
                    _storedValue = null;
                    _currentOperation = "";
                }
            }
            else
            {
                DisplayEntry.Text = _isDecimalMode ? "0" : "0\"";
                _storedValue = null;
                _currentOperation = "";
            }
        }
        else
        {
            DisplayEntry.Text = _isDecimalMode ? "0" : "0\"";
        }
        _shouldClearDisplay = false;

        DisplayEntry.Focus();
        DisplayEntry.CursorPosition = 0;
        DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
    }

    private void UpdateChainDisplay()
    {
        if (_calculationChain.Count > 0)
        {
            ChainLabel.Text = string.Join(" ", _calculationChain);
        }
        else
        {
            ChainLabel.Text = "";
        }
    }

    private void ReplayCalculationChain()
    {
        _storedValue = null;
        _currentOperation = "";

        for (int i = 0; i < _calculationChain.Count; i++)
        {
            string item = _calculationChain[i];

            if (item == "+" || item == "-" || item == "*" || item == "/")
            {
                _currentOperation = item;
            }
            else
            {
                Measurement value = Measurement.Parse(item);

                if (_storedValue == null)
                {
                    _storedValue = value;
                }
                else if (!string.IsNullOrEmpty(_currentOperation))
                {
                    switch (_currentOperation)
                    {
                        case "+":
                            _storedValue += value;
                            break;
                        case "-":
                            _storedValue -= value;
                            break;
                        case "*":
                            _storedValue *= value.ToTotalInches();
                            break;
                        case "/":
                            _storedValue /= value.ToTotalInches();
                            break;
                    }
                    _currentOperation = "";
                }
            }
        }

        if (_storedValue != null)
        {
            UpdateDisplay(_storedValue);
        }
    }

    private void ToggleMode()
    {
        _isDecimalMode = !_isDecimalMode;
        ModeLabel.Text = _isDecimalMode ? "Mode: Decimal (inches)" : "Mode: Fractions (1/16\")";

        try
        {
            if (!string.IsNullOrEmpty(DisplayEntry.Text) && DisplayEntry.Text != "0" && DisplayEntry.Text != "0\"")
            {
                Measurement current = ParseCurrentDisplay();
                UpdateDisplay(current);
            }
            else
            {
                DisplayEntry.Text = _isDecimalMode ? "0" : "0\"";
            }
        }
        catch
        {
            DisplayEntry.Text = _isDecimalMode ? "0" : "0\"";
        }

        DisplayEntry.Focus();
        DisplayEntry.CursorPosition = 0;
        DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
    }

    private void SetOperation(string operation)
    {
        try
        {
            bool calculationPerformed = false;

            if (_storedValue != null && !string.IsNullOrEmpty(DisplayEntry.Text.Trim()) && !string.IsNullOrEmpty(_currentOperation))
            {
                PerformCalculation();
                calculationPerformed = true;
            }

            Measurement currentValue = ParseCurrentDisplay();
            string displayText = DisplayEntry.Text.Trim();

            if (_storedValue == null || _calculationChain.Count == 0)
            {
                _calculationChain.Add(displayText);
            }
            else if (!_shouldClearDisplay)
            {
                _calculationChain.Add(displayText);
            }

            _calculationChain.Add(operation);
            UpdateChainDisplay();

            if (!calculationPerformed)
            {
                _storedValue = currentValue;
            }

            _currentOperation = operation;
            _shouldClearDisplay = true;
            DisplayEntry.Text = "";

            DisplayEntry.Focus();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Error parsing value: {ex.Message}", "OK");
            Clear();
        }
    }

    private void PerformCalculation()
    {
        try
        {
            string displayText = DisplayEntry.Text.Trim();

            if (_storedValue == null || string.IsNullOrEmpty(_currentOperation))
                return;

            Measurement current = ParseCurrentDisplay();

            if (!_shouldClearDisplay && _calculationChain.Count > 0 &&
                _calculationChain[^1] != "+" &&
                _calculationChain[^1] != "-" &&
                _calculationChain[^1] != "*" &&
                _calculationChain[^1] != "/")
            {
                _calculationChain.Add(displayText);
            }
            else if (_calculationChain.Count > 0 &&
                     (_calculationChain[^1] == "+" ||
                      _calculationChain[^1] == "-" ||
                      _calculationChain[^1] == "*" ||
                      _calculationChain[^1] == "/"))
            {
                _calculationChain.Add(displayText);
            }

            UpdateChainDisplay();

            Measurement result;

            switch (_currentOperation)
            {
                case "+":
                    result = _storedValue + current;
                    break;
                case "-":
                    result = _storedValue - current;
                    break;
                case "*":
                    result = _storedValue * current.ToTotalInches();
                    break;
                case "/":
                    if (current.ToTotalInches() == 0)
                    {
                        DisplayAlert("Error", "Cannot divide by zero", "OK");
                        return;
                    }
                    result = _storedValue / current.ToTotalInches();
                    break;
                default:
                    return;
            }

            UpdateDisplay(result);
            _storedValue = result;
            _currentOperation = "";
            _shouldClearDisplay = true;

            DisplayEntry.Focus();
            DisplayEntry.CursorPosition = 0;
            DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Calculation error: {ex.Message}", "OK");
            Clear();
        }
    }

    private Measurement ParseCurrentDisplay()
    {
        string text = DisplayEntry.Text.Trim();
        if (string.IsNullOrEmpty(text) || text == "0" || text == "0\"")
        {
            return new Measurement(0, 0, 0, 16);
        }

        return Measurement.Parse(text);
    }

    private void UpdateDisplay(Measurement measurement)
    {
        DisplayEntry.Text = _isDecimalMode ? measurement.ToDecimalString() : measurement.ToFractionString();
    }
}
