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
            
            SplashScreenForm splash = new SplashScreenForm();
            splash.Show();
            Application.DoEvents();
            
            CalculatorForm mainForm = new CalculatorForm();
            splash.Close();
            
            Application.Run(mainForm);
        }
    }
}
