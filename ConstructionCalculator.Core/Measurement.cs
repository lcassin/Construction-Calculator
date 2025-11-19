using System;
using System.Text.RegularExpressions;

namespace ConstructionCalculator
{
    public class Measurement
    {
        public int Feet { get; set; }
        public int Inches { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }

        public Measurement(int feet = 0, int inches = 0, int numerator = 0, int denominator = 16)
        {
            Feet = feet;
            Inches = inches;
            Numerator = numerator;
            Denominator = denominator;
            Normalize();
        }

        public static Measurement FromDecimalInches(double totalInches)
        {
            int feet = (int)(totalInches / 12);
            double remainingInches = totalInches - (feet * 12);
            int inches = (int)remainingInches;
            double fraction = remainingInches - inches;
            
            int numerator = (int)Math.Round(fraction * 16);
            
            return new Measurement(feet, inches, numerator, 16);
        }

        public static Measurement Parse(string input)
        {
            input = input.Trim();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new FormatException("Measurement cannot be empty.\n\nAccepted formats:\n  3' 4-1/2\"\n  3'4\"\n  4-1/2\"\n  4.5\n\nNote: Inside a measurement, \"-\" means \"and\" (e.g., 4-1/2\" = 4.5 inches)");
            }
            
            bool isNegative = false;
            if (input.StartsWith("-"))
            {
                isNegative = true;
                input = input.Substring(1).Trim();
                
                if (input.StartsWith("(") && input.EndsWith(")"))
                {
                    input = input.Substring(1, input.Length - 2).Trim();
                }
            }
            
            input = NormalizeUnicode(input);
            
            bool hasFeetMarker = input.Contains("'");
            
            string cleanInput = input.Replace("\"", "").Replace("'", " ").Trim();
            
            var feetInchesMatch = Regex.Match(cleanInput, @"^(\d+)\s*(\d+)(?:[\s\-]+(\d+)/(\d+))?$");
            if (feetInchesMatch.Success)
            {
                int feet = int.Parse(feetInchesMatch.Groups[1].Value);
                int inches = int.Parse(feetInchesMatch.Groups[2].Value);
                int numerator = feetInchesMatch.Groups[3].Success ? int.Parse(feetInchesMatch.Groups[3].Value) : 0;
                int denominator = feetInchesMatch.Groups[4].Success ? int.Parse(feetInchesMatch.Groups[4].Value) : 16;
                var m = new Measurement(feet, inches, numerator, denominator);
                return isNegative ? FromDecimalInches(-m.ToTotalInches()) : m;
            }
            
            var singleNumberMatch = Regex.Match(cleanInput, @"^(\d+)(?:[\s\-]+(\d+)/(\d+))?$");
            if (singleNumberMatch.Success)
            {
                int value = int.Parse(singleNumberMatch.Groups[1].Value);
                int numerator = singleNumberMatch.Groups[2].Success ? int.Parse(singleNumberMatch.Groups[2].Value) : 0;
                int denominator = singleNumberMatch.Groups[3].Success ? int.Parse(singleNumberMatch.Groups[3].Value) : 16;
                
                Measurement m;
                if (hasFeetMarker)
                {
                    m = new Measurement(value, 0, numerator, denominator);
                }
                else
                {
                    m = new Measurement(0, value, numerator, denominator);
                }
                return isNegative ? FromDecimalInches(-m.ToTotalInches()) : m;
            }
            
            var fractionOnlyMatch = Regex.Match(cleanInput, @"^(\d+)/(\d+)$");
            if (fractionOnlyMatch.Success)
            {
                int numerator = int.Parse(fractionOnlyMatch.Groups[1].Value);
                int denominator = int.Parse(fractionOnlyMatch.Groups[2].Value);
                var m = new Measurement(0, 0, numerator, denominator);
                return isNegative ? FromDecimalInches(-m.ToTotalInches()) : m;
            }
            
            if (double.TryParse(cleanInput, System.Globalization.CultureInfo.InvariantCulture, out double decimalValue))
            {
                return FromDecimalInches(isNegative ? -decimalValue : decimalValue);
            }
            
            throw new FormatException($"Unable to parse measurement: {input}\n\nAccepted formats:\n  3' 4-1/2\"\n  3'4\"\n  4-1/2\"\n  4.5\n\nNote: Inside a measurement, \"-\" means \"and\" (e.g., 4-1/2\" = 4.5 inches)\nFor subtraction, use separate measurements: 4\" - 1/2\"");
        }

        private static string NormalizeUnicode(string input)
        {
            return input
                .Replace('\u2032', '\'')
                .Replace('\u2033', '"')
                .Replace('\u2018', '\'')
                .Replace('\u2019', '\'')
                .Replace('\u201C', '"')
                .Replace('\u201D', '"')
                .Replace('\u2013', '-')
                .Replace('\u2014', '-')
                .Replace('\u2212', '-');
        }

        public double ToTotalInches()
        {
            return Feet * 12.0 + Inches + (double)Numerator / Denominator;
        }

        private void Normalize()
        {
            if (Denominator == 0) Denominator = 16;
            
            int gcd = GCD(Math.Abs(Numerator), Denominator);
            if (gcd > 1)
            {
                Numerator /= gcd;
                Denominator /= gcd;
            }
            
            while (Numerator >= Denominator)
            {
                Inches++;
                Numerator -= Denominator;
            }
            
            while (Numerator < 0)
            {
                Inches--;
                Numerator += Denominator;
            }
            
            while (Inches >= 12)
            {
                Feet++;
                Inches -= 12;
            }
            
            while (Inches < 0)
            {
                Feet--;
                Inches += 12;
            }
        }

        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public string ToFractionString()
        {
            Normalize();
            
            double totalInches = ToTotalInches();
            bool isNegative = totalInches < 0;
            
            if (isNegative)
            {
                Measurement positive = FromDecimalInches(Math.Abs(totalInches));
                return "-" + positive.ToFractionString();
            }
            
            string result = "";
            
            if (Feet != 0)
            {
                result = $"{Math.Abs(Feet)}'";
            }
            
            if (Inches != 0 || Numerator != 0 || Feet == 0)
            {
                if (Feet != 0) result += " ";
                result += Math.Abs(Inches).ToString();
                
                if (Numerator != 0)
                {
                    result += $"-{Math.Abs(Numerator)}/{Math.Abs(Denominator)}";
                }
                
                result += "\"";
            }
            
            return result.Trim();
        }

        public string ToDecimalString()
        {
            return ToTotalInches().ToString("F4");
        }

        public static Measurement operator +(Measurement a, Measurement b)
        {
            double total = a.ToTotalInches() + b.ToTotalInches();
            return FromDecimalInches(total);
        }

        public static Measurement operator -(Measurement a, Measurement b)
        {
            double total = a.ToTotalInches() - b.ToTotalInches();
            return FromDecimalInches(total);
        }

        public static Measurement operator *(Measurement a, double multiplier)
        {
            double total = a.ToTotalInches() * multiplier;
            return FromDecimalInches(total);
        }

        public static Measurement operator /(Measurement a, double divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException("Cannot divide by zero");
            double total = a.ToTotalInches() / divisor;
            return FromDecimalInches(total);
        }
    }
}
