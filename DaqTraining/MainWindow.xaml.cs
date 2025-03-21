using BMZ.Daq;
using BMZ.Daq.Keysight970A;
using BMZ.PowerSupply;
using System;
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
        private bool charged = true;
        public double measuredVoltage;

        public MainWindow()
        {
            InitializeComponent();
            CompositionRoot.Instance.Init();

            _Daq = CompositionRoot.Instance.DIContainer.Resolve<IDaq>();
            _PowerSupply = CompositionRoot.Instance.DIContainer.Resolve<IPowerSupply>();

            //if (charged)
            //{
            //    PowerSupply(37, 0);
            //    StatusTextBlock.Text = $"{measuredVoltage} - Charged";
            //}

            //else if(!charged)
            //{
            //    PowerSupply(37, 5);
            //}


            // Getting Daq reading real time
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Daq);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            DispatcherTimer timer2 = new DispatcherTimer();
            timer2.Tick += new EventHandler(VoltageCheck);
            timer2.Interval = new TimeSpan(0, 0, 1);
            timer2.Start();
        }

        private void PowerSupply(int voltage, int current)
        {
            _PowerSupply.General.Connection.Connect();
            _PowerSupply.Commons.PerformReset();
            Thread.Sleep(500);
            _PowerSupply.Voltage.SetVoltage(voltage * 1000);
            _PowerSupply.Current.SetCurrent(current * 1000);
            Thread.Sleep(500);
            _PowerSupply.Output.SetOutput(true);
            StatusTextBlock.Text = "Charging";
            _PowerSupply.General.Connection.Disconnect();
        }

        private void Daq(object sender, EventArgs e)
        {
            _Daq.General.Connection.Connect();
            _Daq.General.Configure("C:\\Users\\Harun.Shaban\\Downloads\\DaqSettings.xml");
            measuredVoltage =  Math.Round(_Daq.Measurement.Measure("BatteryVoltage")/1000, 4);
            DaqReadingTextBox.Text = measuredVoltage.ToString();
        }

        private void VoltageCheck(object sender, EventArgs e)
        {
            Debug.WriteLine($"VoltageCheck: measuredVoltage = {(int)measuredVoltage}");
            if((int)measuredVoltage == 37)
            {
                _PowerSupply.General.Connection.Connect();
                _PowerSupply.Current.SetCurrent(0);
                Thread.Sleep(500);
                _PowerSupply.Output.SetOutput(true);
                Thread.Sleep(500);
                StatusTextBlock.Text = "Checking";
                Thread.Sleep(60000);
                if((int)measuredVoltage != 36)
                {
                    _PowerSupply.General.Connection.Disconnect();
                    PowerSupply(38, 5);
                }
                else
                {
                    _PowerSupply.General.Connection.Disconnect();
                }
            }
            //_PowerSupply.Output.SetOutput(false);
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            PowerSupply(38, 5);

            ////checking for numeric input
            //int number;
            //bool successVoltage = int.TryParse(VoltageTextBox.Text, out number);
            //bool successCurrent = int.TryParse(CurrentTextBox.Text, out number);

            //if (successVoltage && successCurrent)
            //{
            //    int setVoltage = int.Parse(VoltageTextBox.Text);
            //    int setCurrent = int.Parse(CurrentTextBox.Text);
            //    // checking for correct input
            //    if ((setVoltage < 0 || setVoltage > 40) && (setCurrent < 0 || setCurrent > 5))
            //    {
            //        StatusTextBlock.Text = "Voltage should be max 40 and Current max 5";
            //    }
            //    PowerSupply(setVoltage, setCurrent);
            //}
            //else
            //{
            //    StatusTextBlock.Text = "Entered Value must be number";
            //}
        }

    }
}