using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace ConstructionCalculator.WPF;

public partial class SeatingLayoutCalculatorWindow : Window
{
    private StringBuilder calculationResults = new();

    public SeatingLayoutCalculatorWindow()
    {
        InitializeComponent();
    }

    private void CalculateLayoutButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            int numberOfRows = int.Parse(NumberOfRowsTextBox.Text.Trim());
            double startingRadius = double.Parse(StartingRadiusTextBox.Text.Trim());
            double rowSpacing = double.Parse(RowSpacingTextBox.Text.Trim());
            double arcSpan = double.Parse(ArcSpanTextBox.Text.Trim());
            double centerNorthing = double.Parse(CenterNorthingTextBox.Text.Trim());
            double centerEasting = double.Parse(CenterEastingTextBox.Text.Trim());

            double chairAWidth = double.Parse(ChairAWidthTextBox.Text.Trim()) / 12.0;
            double chairBWidth = double.Parse(ChairBWidthTextBox.Text.Trim()) / 12.0;

            int numberOfAisles = int.Parse(NumberOfAislesTextBox.Text.Trim());
            double aisleWidth = double.Parse(AisleWidthTextBox.Text.Trim()) / 12.0;

            double minAisleWidth = double.Parse(MinAisleWidthTextBox.Text.Trim()) / 12.0;
            int maxSeatsPerAisle = int.Parse(MaxSeatsTextBox.Text.Trim());

            if (aisleWidth < minAisleWidth)
            {
                MessageBox.Show($"Aisle width ({aisleWidth * 12:F1}\") is less than minimum ({minAisleWidth * 12:F1}\")!",
                    "Code Compliance Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            calculationResults.Clear();
            calculationResults.AppendLine("Row,Seat,Type,X_Coordinate,Y_Coordinate,Rotation_Angle,Notes");

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
                    MessageBox.Show($"Cannot fit any chairs in row {row}. Radius too small or too many aisles.",
                        "Layout Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double currentAngle = startAngle;
                int seatNumber = 1;
                int seatsInSection = 0;

                foreach (char chairType in chairPattern)
                {
                    if (chairType == '|')
                    {
                        currentAngle += aisleWidth / currentRadius;
                        seatsInSection = 0;
                        continue;
                    }

                    double chairWidth = (chairType == 'A') ? chairAWidth : chairBWidth;
                    currentAngle += chairWidth / (2.0 * currentRadius);

                    double x = centerEasting + (currentRadius * Math.Cos(currentAngle));
                    double y = centerNorthing + (currentRadius * Math.Sin(currentAngle));
                    double rotationDegrees = (currentAngle * 180.0 / Math.PI) + 90.0;

                    string notes = "";
                    seatsInSection++;

                    calculationResults.AppendLine($"{row},{seatNumber},{chairType},{x:F3},{y:F3},{rotationDegrees:F2},{notes}");

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
            MessageBox.Show($"Error: {ex.Message}", "Calculation Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                pattern.Add('|');
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

    private void ExportToCSVButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (calculationResults.Length == 0 || calculationResults.ToString().Split('\n').Length <= 2)
            {
                MessageBox.Show("Please calculate layout first before exporting.",
                    "Export Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"SeatingLayout_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, calculationResults.ToString());
                MessageBox.Show($"Layout exported successfully to:\n{saveFileDialog.FileName}",
                    "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting file: {ex.Message}", "Export Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
