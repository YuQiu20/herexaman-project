using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Demo_COM_port_5
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;                
        private readonly byte[] dmxData = new byte[513]; // startcode + 512 DMX-kanalen
        private readonly DispatcherTimer dmxTimer;

        // kleurcyclus groen ↔ rood
        private readonly DispatcherTimer colorCycleTimer = new DispatcherTimer();
        private bool cycleGreen = true;           // true = groen, false = rood
        private bool _programmaticUpdate = false; // voorkomt dubbele slider-events bij set via timer

        public MainWindow()
        {
            InitializeComponent();
            cbxPortName.ItemsSource = SerialPort.GetPortNames();

            // DMX refresher (40Hz)
            dmxTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(25) };
            dmxTimer.Tick += (_, __) => SendDmxData();

            // Kleurcyclus timer 
            colorCycleTimer.Interval = TimeSpan.FromMilliseconds(1000);
            colorCycleTimer.Tick += ColorCycleTimer_Tick;

            dmxData[0] = 0; // startcode altijd 0
        }

        private void cbxPortName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (serialPort != null)
                {
                    dmxTimer.Stop();
                    colorCycleTimer.Stop();
                    if (serialPort.IsOpen) serialPort.Close();
                    serialPort.Dispose();
                }

                if (cbxPortName.SelectedItem == null) return;

                serialPort = new SerialPort(
                    cbxPortName.SelectedItem.ToString(),
                    250000, Parity.None, 8, StopBits.Two);

                serialPort.Open();

                // Sliders activeren
                sldrRed.IsEnabled = sldrGreen.IsEnabled = sldrBlue.IsEnabled = true;

                // Alles uit sturen en master correct zetten
                Array.Clear(dmxData, 1, 512);
                UpdateMasterDimmer();
                SendDmxData();

                dmxTimer.Start();
                lblDebug.Content = "Verbonden. Alle LEDs uit.";
            }
            catch (Exception ex)
            {
                lblDebug.Content = "Fout: " + ex.Message;
                MessageBox.Show("Kon de COM-poort niet openen.\n" + ex.Message,
                                "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                dmxTimer.Stop();
                colorCycleTimer.Stop();
                if (serialPort != null)
                {
                    if (serialPort.IsOpen) serialPort.Close();
                    serialPort.Dispose();
                }
            }
            catch { }
        }

        private void SendDmxData()
        {
            if (serialPort == null || !serialPort.IsOpen) return;

            try
            {
                
                serialPort.BreakState = true; Thread.Sleep(1);
                serialPort.BreakState = false; Thread.Sleep(1);

                serialPort.Write(dmxData, 0, dmxData.Length);

                lblDebug.Content = $"DMX: Dimmer={dmxData[1]} R={dmxData[4]} G={dmxData[2]} B={dmxData[3]}";
            }
            catch
            {
            }
        }

        // ---- Nieuw: master dimmer automatisch aan/uit
        private void UpdateMasterDimmer()
        {
            if (dmxData[2] > 0 || dmxData[3] > 0 || dmxData[4] > 0)
                dmxData[1] = 255; // aan
            else
                dmxData[1] = 0;   // uit
        }

        // Slider rood → roodkanaal (R = 4 + 3*i)
        private void sldrRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_programmaticUpdate) return;
            for (int i = 0; i < 16; i++)
                dmxData[4 + (i * 3)] = (byte)sldrRed.Value;
            UpdateMasterDimmer();
        }

        // Slider groen → groenkanaal (G = 2 + 3*i)
        private void sldrGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_programmaticUpdate) return;
            for (int i = 0; i < 16; i++)
                dmxData[2 + (i * 3)] = (byte)sldrGreen.Value;
            UpdateMasterDimmer();
        }

        // Slider blauw → blauwkanaal (B = 3 + 3*i)
        private void sldrBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_programmaticUpdate) return;
            for (int i = 0; i < 16; i++)
                dmxData[3 + (i * 3)] = (byte)sldrBlue.Value;
            UpdateMasterDimmer();
        }

        // Alles uit
        private void btnAllOff_Click(object sender, RoutedEventArgs e)
        {
            colorCycleTimer.Stop();
            Array.Clear(dmxData, 1, 512);

            _programmaticUpdate = true;
            sldrRed.Value = sldrGreen.Value = sldrBlue.Value = 0;
            _programmaticUpdate = false;

            UpdateMasterDimmer();
            SendDmxData();
            lblDebug.Content = "Alles uit";
        }

        // Alles aan
        private void btnAllOn_Click(object sender, RoutedEventArgs e)
        {
            colorCycleTimer.Stop();
            for (int i = 1; i <= 512; i++)
                dmxData[i] = 255;

            _programmaticUpdate = true;
            sldrRed.Value = sldrGreen.Value = sldrBlue.Value = 255;
            _programmaticUpdate = false;

            UpdateMasterDimmer();
            SendDmxData();
            lblDebug.Content = "Alles aan";
        }

        // Startknop 
        private void btnStartCycle_Click(object sender, RoutedEventArgs e)
        {
            // Lees interval (ms)
            if (int.TryParse(txtCycleMs.Text, out int ms) && ms >= 100)
                colorCycleTimer.Interval = TimeSpan.FromMilliseconds(ms);
            else
                colorCycleTimer.Interval = TimeSpan.FromMilliseconds(1000);

            cycleGreen = true;   // begin met groen
            ApplyCycleStep();    // direct zetten
            colorCycleTimer.Start();
            lblDebug.Content = $"Kleurcyclus gestart ({colorCycleTimer.Interval.TotalMilliseconds} ms).";
        }

        private void ColorCycleTimer_Tick(object sender, EventArgs e)
        {
            cycleGreen = !cycleGreen; // toggle groen ↔ rood
            ApplyCycleStep();
        }

        private void ApplyCycleStep()
        {
            byte r = (byte)(cycleGreen ? 0 : 255);
            byte g = (byte)(cycleGreen ? 255 : 0);
            byte b = 0;

            for (int i = 0; i < 16; i++)
            {
                dmxData[2 + (i * 3)] = g; // Groen
                dmxData[3 + (i * 3)] = b; // Blauw
                dmxData[4 + (i * 3)] = r; // Rood
            }

            // Sliders mee laten bewegen zonder hun events uit te voeren
            _programmaticUpdate = true;
            sldrGreen.Value = g;
            sldrBlue.Value = b;
            sldrRed.Value = r;
            _programmaticUpdate = false;

            UpdateMasterDimmer();
            
        }
    }
}
