using BMZ.Daq;
using BMZ.Daq.Keysight970A;
using BMZ.PowerSupply;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Unity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DaqTraining
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDaq _Daq;
        private readonly IPowerSupply _PowerSupply;
        public MainWindow()
        {
            InitializeComponent();
            CompositionRoot.Instance.Init();

            _Daq = CompositionRoot.Instance.DIContainer.Resolve<IDaq>();
            _PowerSupply = CompositionRoot.Instance.DIContainer.Resolve<IPowerSupply>();

            // Getting Daq reading real time
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Daq);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void PowerSupply(int voltage)
        {
            _PowerSupply.General.Connection.Connect();
            _PowerSupply.Commons.PerformReset();
            Thread.Sleep(500);
            _PowerSupply.Voltage.GetVoltage();
            _PowerSupply.Voltage.SetVoltage(voltage * 1000);
            StatusTextBlock.Text = $"Voltage initialized to {VoltageTextBox.Text}";
            Thread.Sleep(500);
            _PowerSupply.Output.SetOutput(true);
            StatusTextBlock.Text = $"Voltage set to {VoltageTextBox.Text}";
            _PowerSupply.General.Connection.Disconnect();
        }

        private void Daq(object sender, EventArgs e)
        {
            _Daq.General.Connection.Connect();
            _Daq.General.Configure("C:\\Users\\Harun.Shaban\\Downloads\\DaqSettings.xml");
            DaqReadingTextBox.Text = _Daq.Measurement.Measure("BatteryVoltage").ToString();
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            //checking for numeric input
            bool success = int.TryParse(VoltageTextBox.Text, out int number);
            if (success)
            {
                int setVoltage = int.Parse(VoltageTextBox.Text);
                // checking for correct input
                if (setVoltage < 0 || setVoltage > 30)
                {
                    StatusTextBlock.Text = "Entered Value must be between 0 and 30";
                }
                PowerSupply(setVoltage);
            }
            else
            {
                StatusTextBlock.Text = "Entered Value must be number";
            }
        }
    }
}