using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ConstructionCalculator.WPF.Calculators.Materials.BoardFeet;

public partial class BoardFeetCalculatorWindow : Window
{
    private List<LumberPiece> pieces = new List<LumberPiece>();

    public BoardFeetCalculatorWindow()
    {
        InitializeComponent();
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            double thickness = double.Parse(ThicknessTextBox.Text);
            double width = double.Parse(WidthTextBox.Text);
            double length = double.Parse(LengthTextBox.Text);

            double boardFeet = (thickness * width * length) / 12.0;

            var piece = new LumberPiece
            {
                Thickness = thickness,
                Width = width,
                Length = length,
                BoardFeet = boardFeet
            };

            pieces.Add(piece);
            AddPieceToDisplay(piece, pieces.Count - 1);
            UpdateTotal();

            ThicknessTextBox.Clear();
            WidthTextBox.Clear();
            LengthTextBox.Clear();
            ThicknessTextBox.Focus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}\n\nPlease enter valid numbers.", 
                          "Input Error", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Error);
        }
    }

    private void AddPieceToDisplay(LumberPiece piece, int index)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 5)
        };

        var textBlock = new TextBlock
        {
            Text = $"{piece.Thickness}\" × {piece.Width}\" × {piece.Length}' = {piece.BoardFeet:F2} bd ft",
            FontSize = 13,
            VerticalAlignment = VerticalAlignment.Center,
            Width = 350
        };

        var removeButton = new Button
        {
            Content = "Remove",
            Width = 70,
            Height = 25,
            Margin = new Thickness(10, 0, 0, 0),
            Tag = index
        };
        removeButton.Click += RemovePiece_Click;

        panel.Children.Add(textBlock);
        panel.Children.Add(removeButton);
        PiecesPanel.Children.Add(panel);
    }

    private void RemovePiece_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int index)
        {
            if (index >= 0 && index < pieces.Count)
            {
                pieces.RemoveAt(index);
                RefreshDisplay();
            }
        }
    }

    private void ClearAll_Click(object sender, RoutedEventArgs e)
    {
        pieces.Clear();
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        PiecesPanel.Children.Clear();
        for (int i = 0; i < pieces.Count; i++)
        {
            AddPieceToDisplay(pieces[i], i);
        }
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        double total = 0;
        foreach (var piece in pieces)
        {
            total += piece.BoardFeet;
        }
        TotalTextBlock.Text = $"Total Board Feet: {total:F2}";
    }
}

public class LumberPiece
{
    public double Thickness { get; set; }
    public double Width { get; set; }
    public double Length { get; set; }
    public double BoardFeet { get; set; }
}
