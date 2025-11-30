using System.Collections.ObjectModel;

namespace ConstructionCalculatorMAUI.Pages.Calculators.Materials;

public partial class BoardFeetCalculatorPage : ContentPage
{
    private readonly ObservableCollection<LumberPiece> pieces = new();

    public BoardFeetCalculatorPage()
    {
        InitializeComponent();
        PiecesCollectionView.ItemsSource = pieces;
    }

    private async void Add_Clicked(object sender, EventArgs e)
    {
        try
        {
            double thickness = double.Parse(ThicknessEntry.Text);
            double width = double.Parse(WidthEntry.Text);
            double length = double.Parse(LengthEntry.Text);

            double boardFeet = (thickness * width * length) / 12.0;

            var piece = new LumberPiece
            {
                Thickness = thickness,
                Width = width,
                Length = length,
                BoardFeet = boardFeet
            };

            pieces.Add(piece);
            UpdateTotal();

            ThicknessEntry.Text = "";
            WidthEntry.Text = "";
            LengthEntry.Text = "";
            ThicknessEntry.Focus();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error: {ex.Message}\n\nPlease enter valid numbers.", "OK");
        }
    }

    private void RemovePiece_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is LumberPiece piece)
        {
            pieces.Remove(piece);
            UpdateTotal();
        }
    }

    private void ClearAll_Clicked(object sender, EventArgs e)
    {
        pieces.Clear();
        UpdateTotal();
    }

    private void UpdateTotal()
    {
        double total = pieces.Sum(p => p.BoardFeet);
        TotalLabel.Text = $"Total Board Feet: {total:F2}";
    }

    public class LumberPiece
    {
        public double Thickness { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double BoardFeet { get; set; }
        
        public string DisplayText => $"{Thickness}\" × {Width}\" × {Length}' = {BoardFeet:F2} bd ft";
    }
}
