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
            input = input.Trim().Replace("\"", "").Replace("'", " ");
            
            var feetInchesMatch = Regex.Match(input, @"^(\d+)\s+(\d+)(?:[\s\-]+(\d+)/(\d+))?$");
            if (feetInchesMatch.Success)
            {
                int feet = int.Parse(feetInchesMatch.Groups[1].Value);
                int inches = int.Parse(feetInchesMatch.Groups[2].Value);
                int numerator = feetInchesMatch.Groups[3].Success ? int.Parse(feetInchesMatch.Groups[3].Value) : 0;
                int denominator = feetInchesMatch.Groups[4].Success ? int.Parse(feetInchesMatch.Groups[4].Value) : 16;
                return new Measurement(feet, inches, numerator, denominator);
            }
            
            var inchesOnlyMatch = Regex.Match(input, @"^(\d+)(?:[\s\-]+(\d+)/(\d+))?$");
            if (inchesOnlyMatch.Success)
            {
                int inches = int.Parse(inchesOnlyMatch.Groups[1].Value);
                int numerator = inchesOnlyMatch.Groups[2].Success ? int.Parse(inchesOnlyMatch.Groups[2].Value) : 0;
                int denominator = inchesOnlyMatch.Groups[3].Success ? int.Parse(inchesOnlyMatch.Groups[3].Value) : 16;
                return new Measurement(0, inches, numerator, denominator);
            }
            
            var fractionOnlyMatch = Regex.Match(input, @"^(\d+)/(\d+)$");
            if (fractionOnlyMatch.Success)
            {
                int numerator = int.Parse(fractionOnlyMatch.Groups[1].Value);
                int denominator = int.Parse(fractionOnlyMatch.Groups[2].Value);
                return new Measurement(0, 0, numerator, denominator);
            }
            
            if (double.TryParse(input, out double decimalValue))
            {
                return FromDecimalInches(decimalValue);
            }
            
            throw new FormatException($"Unable to parse measurement: {input}");
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
            string result = "";
            
            if (Feet != 0)
            {
                result = $"{Feet}'";
            }
            
            if (Inches != 0 || Numerator != 0 || Feet == 0)
            {
                if (Feet != 0) result += " ";
                result += Inches.ToString();
                
                if (Numerator != 0)
                {
                    result += $"-{Numerator}/{Denominator}";
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
