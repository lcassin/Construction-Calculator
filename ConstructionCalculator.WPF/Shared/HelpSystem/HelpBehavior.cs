using System.Windows;
using System.Windows.Input;

namespace ConstructionCalculator.WPF.Shared.HelpSystem;

public static class HelpBehavior
{
    public static readonly DependencyProperty CalculatorKindProperty =
        DependencyProperty.RegisterAttached(
            "CalculatorKind",
            typeof(CalculatorKind?),
            typeof(HelpBehavior),
            new PropertyMetadata(null, OnCalculatorKindChanged));

    public static CalculatorKind? GetCalculatorKind(DependencyObject obj)
    {
        return (CalculatorKind?)obj.GetValue(CalculatorKindProperty);
    }

    public static void SetCalculatorKind(DependencyObject obj, CalculatorKind? value)
    {
        obj.SetValue(CalculatorKindProperty, value);
    }

    private static void OnCalculatorKindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Window window && e.NewValue is CalculatorKind kind)
        {
            var keyBinding = new KeyBinding(
                new RelayCommand(() => ShowHelp(window, kind)),
                Key.F1,
                ModifierKeys.None);

            window.InputBindings.Add(keyBinding);
        }
    }

    private static void ShowHelp(Window owner, CalculatorKind kind)
    {
        var helpWindow = new HelpWindow(kind)
        {
            Owner = owner
        };
        helpWindow.Show();
    }

    private class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }
}
