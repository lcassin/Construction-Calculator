using System;
using System.Windows.Forms;

namespace ConstructionCalculator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            using (SplashScreenForm splash = new SplashScreenForm())
            {
                splash.ShowDialog();
            }
            
            Application.Run(new CalculatorForm());
        }
    }
}
