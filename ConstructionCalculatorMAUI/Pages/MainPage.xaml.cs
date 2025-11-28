using ConstructionCalculator.Core;

namespace ConstructionCalculatorMAUI.Pages;

public partial class MainPage : ContentPage
{
    private bool _isDecimalMode = false;
    private string _currentOperation = "";
    private Measurement? _storedValue = null;
    private bool _shouldClearDisplay = false;
    private readonly List<string> _calculationChain = new();
    private Measurement? _memoryValue = null;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
#if WINDOWS
        var window = this.Window?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (window != null)
        {
            window.Content.KeyDown += OnWindowKeyDown;
        }
#endif
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
#if WINDOWS
        var window = this.Window?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (window != null)
        {
            window.Content.KeyDown -= OnWindowKeyDown;
        }
#endif
    }

#if WINDOWS
    private void OnWindowKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        var key = e.Key;
        bool handled = true;

        switch (key)
        {
            case Windows.System.VirtualKey.Number0:
            case Windows.System.VirtualKey.NumberPad0:
                AppendToDisplay("0");
                break;
            case Windows.System.VirtualKey.Number1:
            case Windows.System.VirtualKey.NumberPad1:
                AppendToDisplay("1");
                break;
            case Windows.System.VirtualKey.Number2:
            case Windows.System.VirtualKey.NumberPad2:
                AppendToDisplay("2");
                break;
            case Windows.System.VirtualKey.Number3:
            case Windows.System.VirtualKey.NumberPad3:
                AppendToDisplay("3");
                break;
            case Windows.System.VirtualKey.Number4:
            case Windows.System.VirtualKey.NumberPad4:
                AppendToDisplay("4");
                break;
            case Windows.System.VirtualKey.Number5:
            case Windows.System.VirtualKey.NumberPad5:
                AppendToDisplay("5");
                break;
            case Windows.System.VirtualKey.Number6:
            case Windows.System.VirtualKey.NumberPad6:
                AppendToDisplay("6");
                break;
            case Windows.System.VirtualKey.Number7:
            case Windows.System.VirtualKey.NumberPad7:
                AppendToDisplay("7");
                break;
            case Windows.System.VirtualKey.Number8:
            case Windows.System.VirtualKey.NumberPad8:
                AppendToDisplay("8");
                break;
            case Windows.System.VirtualKey.Number9:
            case Windows.System.VirtualKey.NumberPad9:
                AppendToDisplay("9");
                break;

            case Windows.System.VirtualKey.Add:
                SetOperation("+");
                break;
            case Windows.System.VirtualKey.Subtract:
                SetOperation("-");
                break;
            case Windows.System.VirtualKey.Multiply:
                SetOperation("*");
                break;
            case Windows.System.VirtualKey.Divide:
                SetOperation("/");
                break;

            case Windows.System.VirtualKey.Decimal:
            case (Windows.System.VirtualKey)190: // Period key
                AppendToDisplay(".");
                break;
            case Windows.System.VirtualKey.Space:
                AppendToDisplay(" ");
                break;
            case (Windows.System.VirtualKey)222: // Quote key for ' and "
                {
                    var shift = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
                    var leftShift = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.LeftShift);
                    var rightShift = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.RightShift);

                    bool isShiftDown =
                        shift.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down) ||
                        leftShift.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down) ||
                        rightShift.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

                    AppendToDisplay(isShiftDown ? "\"" : "'");
                    break;
                }
            case (Windows.System.VirtualKey)191: // Forward slash key
                AppendToDisplay("/");
                break;

            case Windows.System.VirtualKey.Enter:
                PerformCalculation();
                DisplayEntry.Focus();
                DisplayEntry.CursorPosition = 0;
                DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
                break;
            case Windows.System.VirtualKey.Escape:
                Clear();
                break;
            case Windows.System.VirtualKey.Back:
                ClearEntry();
                break;

            default:
                handled = false;
                break;
        }

        e.Handled = handled;
    }
#endif

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
            string operation = button.CommandParameter?.ToString() ?? button.Text;
            SetOperation(operation);
        }
    }

    private void OnSpaceClicked(object sender, EventArgs e)
    {
        AppendToDisplay(" ");
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

    private void OnMemoryClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            string buttonText = button.Text;
            
            switch (buttonText)
            {
                case "MC":
                    MemoryClear();
                    break;
                case "MR":
                    MemoryRecall();
                    break;
                case "M+":
                    MemoryAdd();
                    break;
                case "M-":
                    MemorySubtract();
                    break;
            }
        }
    }

    private void OnMathFunctionClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            string buttonText = button.Text;
            
            switch (buttonText)
            {
                case "√":
                    PerformSquareRoot();
                    break;
                case "x²":
                    PerformSquared();
                    break;
                case "%":
                    PerformPercent();
                    break;
                case "+/-":
                    ToggleSign();
                    break;
            }
        }
    }

    private void MemoryClear()
    {
        _memoryValue = null;
        UpdateMemoryIndicator();
    }

    private void MemoryRecall()
    {
        if (_memoryValue != null)
        {
            UpdateDisplay(_memoryValue);
            _shouldClearDisplay = true;
            DisplayEntry.Focus();
            DisplayEntry.CursorPosition = 0;
            DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
        }
    }

    private async void MemoryAdd()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            if (_memoryValue == null)
            {
                _memoryValue = current;
            }
            else
            {
                _memoryValue = _memoryValue + current;
            }
            UpdateMemoryIndicator();
            DisplayEntry.Focus();
            DisplayEntry.CursorPosition = 0;
            DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Memory add error: {ex.Message}", "OK");
        }
    }

    private async void MemorySubtract()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            if (_memoryValue == null)
            {
                _memoryValue = Measurement.FromDecimalInches(-current.ToTotalInches());
            }
            else
            {
                _memoryValue = _memoryValue - current;
            }
            UpdateMemoryIndicator();
            DisplayEntry.Focus();
            DisplayEntry.CursorPosition = 0;
            DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Memory subtract error: {ex.Message}", "OK");
        }
    }

    private void UpdateMemoryIndicator()
    {
        if (_memoryValue != null)
        {
            MemoryLabel.IsVisible = true;
            MemoryLabel.Text = "Memory: " + (_isDecimalMode ? _memoryValue.ToDecimalString() : _memoryValue.ToFractionString());
        }
        else
        {
            MemoryLabel.IsVisible = false;
            MemoryLabel.Text = "";
        }
    }

    private async void PerformSquareRoot()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            double totalInches = current.ToTotalInches();
            
            if (totalInches < 0)
            {
                await DisplayAlert("Error", "Cannot take square root of negative number", "OK");
                return;
            }
            
            double result = Math.Sqrt(totalInches);
            Measurement resultMeasurement = Measurement.FromDecimalInches(result);
            UpdateDisplay(resultMeasurement);
            _shouldClearDisplay = true;
            
            DisplayEntry.Focus();
            DisplayEntry.CursorPosition = 0;
            DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Square root error: {ex.Message}", "OK");
        }
    }

    private async void PerformSquared()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            double totalInches = current.ToTotalInches();
            double result = totalInches * totalInches;
            
            Measurement resultMeasurement = Measurement.FromDecimalInches(result);
            UpdateDisplay(resultMeasurement);
            _shouldClearDisplay = true;
            
            DisplayEntry.Focus();
            DisplayEntry.CursorPosition = 0;
            DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Squared error: {ex.Message}", "OK");
        }
    }

    private async void PerformPercent()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            
            if (_storedValue != null && !string.IsNullOrEmpty(_currentOperation))
            {
                double baseValue = _storedValue.ToTotalInches();
                double percentValue = current.ToTotalInches();
                double result;
                
                switch (_currentOperation)
                {
                    case "+":
                    case "-":
                        result = baseValue * (percentValue / 100.0);
                        break;
                    case "*":
                    case "/":
                        result = percentValue / 100.0;
                        break;
                    default:
                        result = percentValue / 100.0;
                        break;
                }
                
                Measurement resultMeasurement = Measurement.FromDecimalInches(result);
                UpdateDisplay(resultMeasurement);
                _shouldClearDisplay = true;
            }
            else
            {
                double totalInches = current.ToTotalInches();
                double result = totalInches / 100.0;
                Measurement resultMeasurement = Measurement.FromDecimalInches(result);
                UpdateDisplay(resultMeasurement);
                _shouldClearDisplay = true;
            }
            
            DisplayEntry.Focus();
            DisplayEntry.CursorPosition = 0;
            DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Percent error: {ex.Message}", "OK");
        }
    }

    private async void ToggleSign()
    {
        try
        {
            Measurement current = ParseCurrentDisplay();
            double totalInches = current.ToTotalInches();
            Measurement result = Measurement.FromDecimalInches(-totalInches);
            UpdateDisplay(result);
            
            DisplayEntry.Focus();
            DisplayEntry.CursorPosition = 0;
            DisplayEntry.SelectionLength = DisplayEntry.Text?.Length ?? 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Toggle sign error: {ex.Message}", "OK");
        }
    }
}
