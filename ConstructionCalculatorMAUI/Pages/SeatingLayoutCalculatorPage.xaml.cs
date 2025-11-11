using System.Text;

namespace ConstructionCalculatorMAUI.Pages;

public partial class SeatingLayoutCalculatorPage : ContentPage
{
    private StringBuilder _calculationResults = new();

    public SeatingLayoutCalculatorPage()
    {
        InitializeComponent();
    }

    private async void OnCalculateLayoutClicked(object sender, EventArgs e)
    {
        try
        {
            int numberOfRows = int.Parse(NumberOfRowsEntry.Text.Trim());
            double startingRadius = double.Parse(StartingRadiusEntry.Text.Trim());
            double rowSpacing = double.Parse(RowSpacingEntry.Text.Trim());
            double arcSpan = double.Parse(ArcSpanEntry.Text.Trim());
            double centerNorthing = double.Parse(CenterNorthingEntry.Text.Trim());
            double centerEasting = double.Parse(CenterEastingEntry.Text.Trim());

            double chairAWidth = double.Parse(ChairAWidthEntry.Text.Trim()) / 12.0; // Convert inches to feet
            double chairBWidth = double.Parse(ChairBWidthEntry.Text.Trim()) / 12.0;

            int numberOfAisles = int.Parse(NumberOfAislesEntry.Text.Trim());
            double aisleWidth = double.Parse(AisleWidthEntry.Text.Trim()) / 12.0;

            double minAisleWidth = double.Parse(MinAisleWidthEntry.Text.Trim()) / 12.0;
            int maxSeatsPerAisle = int.Parse(MaxSeatsEntry.Text.Trim());

            if (aisleWidth < minAisleWidth)
            {
                await DisplayAlert("Code Compliance Warning",
                    $"Aisle width ({aisleWidth * 12:F1}\") is less than minimum ({minAisleWidth * 12:F1}\")!",
                    "OK");
            }

            _calculationResults.Clear();
            _calculationResults.AppendLine("Row,Seat,Type,X_Coordinate,Y_Coordinate,Rotation_Angle,Notes");

            int totalSeats = 0;
            double arcSpanRadians = arcSpan * (Math.PI / 180.0);
            double startAngle = -(arcSpanRadians / 2.0);

            for (int row = 1; row <= numberOfRows; row++)
            {
                double currentRadius = startingRadius + ((row - 1) * rowSpacing);
                double arcLength = currentRadius * arcSpanRadians;
                double availableWidth = arcLength - (numberOfAisles * aisleWidth);

                List<char> chairPattern = GenerateChairPattern(availableWidth, chairAWidth, chairBWidth, numberOfAisles, maxSeatsPerAisle);

                if (chairPattern.Count == 0)
                {
                    await DisplayAlert("Layout Error",
                        $"Cannot fit any chairs in row {row}. Radius too small or too many aisles.",
                        "OK");
                    return;
                }

                double currentAngle = startAngle;
                int seatNumber = 1;

                foreach (char chairType in chairPattern)
                {
                    if (chairType == '|')
                    {
                        currentAngle += aisleWidth / currentRadius;
                        continue;
                    }

                    double chairWidth = (chairType == 'A') ? chairAWidth : chairBWidth;
                    currentAngle += chairWidth / (2.0 * currentRadius);

                    double x = centerEasting + (currentRadius * Math.Cos(currentAngle));
                    double y = centerNorthing + (currentRadius * Math.Sin(currentAngle));
                    double rotationDegrees = (currentAngle * 180.0 / Math.PI) + 90.0;

                    _calculationResults.AppendLine($"{row},{seatNumber},{chairType},{x:F3},{y:F3},{rotationDegrees:F2},");

                    currentAngle += chairWidth / (2.0 * currentRadius);
                    seatNumber++;
                    totalSeats++;
                }
            }

            ResultLabel.Text = $"Layout calculated successfully!\n" +
                              $"Total Seats: {totalSeats}\n" +
                              $"Rows: {numberOfRows}\n" +
                              $"Click 'Export to CSV' to save.";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Calculation Error", $"Error: {ex.Message}", "OK");
        }
    }

    private List<char> GenerateChairPattern(double availableWidth, double chairAWidth, double chairBWidth,
                                            int numberOfAisles, int maxSeatsPerAisle)
    {
        List<char> pattern = new List<char>();
        int sections = numberOfAisles + 1;
        double widthPerSection = availableWidth / sections;

        for (int section = 0; section < sections; section++)
        {
            if (section > 0)
            {
                pattern.Add('|'); // Aisle marker
            }

            List<char> sectionPattern = new List<char>();
            double remainingWidth = widthPerSection;
            int seatCount = 0;
            bool useTypeA = true;

            sectionPattern.Add('A');
            remainingWidth -= chairAWidth;
            seatCount++;

            while (remainingWidth > 0 && seatCount < maxSeatsPerAisle)
            {
                double nextWidth = useTypeA ? chairAWidth : chairBWidth;

                if (remainingWidth < nextWidth)
                {
                    break;
                }

                sectionPattern.Add(useTypeA ? 'A' : 'B');
                remainingWidth -= nextWidth;
                seatCount++;
                useTypeA = !useTypeA;
            }

            if (sectionPattern.Count > 0 && sectionPattern[^1] != 'A')
            {
                sectionPattern[^1] = 'A';
            }

            pattern.AddRange(sectionPattern);
        }

        return pattern;
    }

    private async void OnExportToCSVClicked(object sender, EventArgs e)
    {
        try
        {
            if (_calculationResults.Length == 0 || _calculationResults.ToString().Split('\n').Length <= 2)
            {
                await DisplayAlert("Export Error", "Please calculate layout first before exporting.", "OK");
                return;
            }

            string fileName = $"SeatingLayout_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            await File.WriteAllTextAsync(filePath, _calculationResults.ToString());

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Export Seating Layout",
                File = new ShareFile(filePath)
            });

            await DisplayAlert("Export Successful", $"Layout exported to:\n{fileName}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Export Error", $"Error exporting file: {ex.Message}", "OK");
        }
    }
}
