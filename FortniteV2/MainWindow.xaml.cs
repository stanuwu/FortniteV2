using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FortniteV2.Utils;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace FortniteV2
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            PreviewKeyDown += (sender, args) =>
            {
                var key = args.Key;
                switch (args.Key)
                {
                    case Key.System:
                        key = args.SystemKey;
                        break;
                    case Key.Enter:
                    case Key.Space:
                        args.Handled = true;
                        break;
                }

                if (ReferenceEquals(TriggerBotKeyPicker.Content, "..."))
                {
                    TriggerKey = key;
                    TriggerBotKeyPicker.Content = key;
                    if (key == Key.Escape)
                    {
                        TriggerKey = Key.None;
                        TriggerBotKeyPicker.Content = "";
                    }
                }

                if (ReferenceEquals(AimBotKeyPicker.Content, "..."))
                {
                    AimKey = key;
                    AimBotKeyPicker.Content = key;
                    if (key == Key.Escape)
                    {
                        AimKey = Key.None;
                        AimBotKeyPicker.Content = "";
                    }
                }
            };
            LoadFromMemory();
            Cheat = new Fortnite();
            Task.Run(RunCheat);
        }

        public static Fortnite Cheat { get; private set; }
        private Key TriggerKey { get; set; } = Key.None;
        private Key AimKey { get; set; } = Key.None;

        private void TriggerBotKeyPicker_OnClick(object sender, RoutedEventArgs e)
        {
            TriggerBotKeyPicker.Content = "...";
        }

        private void AimBotKeyPicker_OnClick(object sender, RoutedEventArgs e)
        {
            AimBotKeyPicker.Content = "...";
        }

        private void SaveToFile()
        {
            var jsonSave = new JObject();
            jsonSave["watermark"] = WatermarkTextBox.Text;

            jsonSave["recoilcrosshair"] = RecoilCrosshairCheckBox.IsChecked ?? false;

            jsonSave["skeletonesp"] = SkeletonEspCheckBox.IsChecked ?? false;

            jsonSave["triggerbot"] = TriggerBotCheckBox.IsChecked ?? false;
            jsonSave["triggerkey"] = TriggerKey.ToString();
            jsonSave["triggerdelay"] = (int)TriggerBotDelaySlider.Value;
            jsonSave["triggerrandom"] = (int)TriggerBotRandomSlider.Value;

            jsonSave["aimbot"] = AimBotCheckBox.IsChecked ?? false;
            jsonSave["aimkey"] = AimKey.ToString();
            jsonSave["aimspeed"] = (int)AimBotSpeedSlider.Value;
            jsonSave["aimfov"] = (int)AimBotFovSlider.Value;
            jsonSave["aimdelay"] = (int)AimBotDelaySlider.Value;

            var cdir = Directory.GetCurrentDirectory() + "\\config";
            if (!Directory.Exists(cdir)) Directory.CreateDirectory(cdir);

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "json files (*.json)|*.json",
                InitialDirectory = cdir
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                if (!saveFileDialog.CheckPathExists) File.Create(saveFileDialog.FileName);
                File.WriteAllText(saveFileDialog.FileName, jsonSave.ToString());
            }
        }

        private void LoadFromFile()
        {
            var cdir = Directory.GetCurrentDirectory() + "\\config";
            if (!Directory.Exists(cdir)) Directory.CreateDirectory(cdir);

            var openFileDialog = new OpenFileDialog
            {
                Filter = "json files (*.json)|*.json",
                InitialDirectory = cdir,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (!openFileDialog.CheckFileExists) return;
                var jsonSave = JObject.Parse(File.ReadAllText(openFileDialog.FileName));

                try
                {
                    WatermarkTextBox.Text = (string)jsonSave["watermark"] ?? throw new InvalidOperationException();

                    RecoilCrosshairCheckBox.IsChecked = (bool)jsonSave["recoilcrosshair"];

                    SkeletonEspCheckBox.IsChecked = (bool)jsonSave["skeletonesp"];

                    TriggerBotCheckBox.IsChecked = (bool)jsonSave["triggerbot"];
                    TriggerKey = (Key)Enum.Parse(typeof(Key), (string)jsonSave["triggerkey"] ?? throw new InvalidOperationException());
                    TriggerBotKeyPicker.Content = (string)jsonSave["triggerkey"] ?? throw new InvalidOperationException();
                    TriggerBotDelaySlider.Value = (int)jsonSave["triggerdelay"];
                    TriggerBotRandomSlider.Value = (int)jsonSave["triggerrandom"];

                    AimBotCheckBox.IsChecked = (bool)jsonSave["aimbot"];
                    AimKey = (Key)Enum.Parse(typeof(Key), (string)jsonSave["aimkey"] ?? throw new InvalidOperationException());
                    AimBotKeyPicker.Content = (string)jsonSave["aimkey"] ?? throw new InvalidOperationException();
                    AimBotSpeedSlider.Value = (int)jsonSave["aimspeed"];
                    AimBotFovSlider.Value = (int)jsonSave["aimfov"];
                    AimBotDelaySlider.Value = (int)jsonSave["aimdelay"];
                }
                catch (Exception)
                {
                    MessageBox.Show("Error: Invalid Config File", "Fortnite", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                }
            }
        }

        private void LoadFromMemory()
        {
            WatermarkTextBox.Text = Config.Watermark;

            RecoilCrosshairCheckBox.IsChecked = Config.EnableRecoilCrosshair;

            SkeletonEspCheckBox.IsChecked = Config.EnableSkeletonEsp;

            TriggerBotCheckBox.IsChecked = Config.EnableTriggerBot;
            TriggerKey = Config.TriggerBotKey;
            TriggerBotKeyPicker.Content = Config.TriggerBotKey;
            TriggerBotDelaySlider.Value = Config.TriggerBotDelayMs;
            TriggerBotRandomSlider.Value = Config.TriggerBotDelayRandomMs;

            AimBotCheckBox.IsChecked = Config.EnableAimBot;
            AimKey = Config.AimBotKey;
            AimBotKeyPicker.Content = Config.AimBotKey;
            AimBotSpeedSlider.Value = Config.AimSpeed;
            AimBotFovSlider.Value = Config.AimPixelFov;
            AimBotDelaySlider.Value = Config.AimSwitchDelayMs;
        }

        private void SaveToMemory()
        {
            Config.Watermark = WatermarkTextBox.Text;

            Config.EnableRecoilCrosshair = RecoilCrosshairCheckBox.IsChecked ?? false;

            Config.EnableSkeletonEsp = SkeletonEspCheckBox.IsChecked ?? false;

            Config.EnableTriggerBot = TriggerBotCheckBox.IsChecked ?? false;
            Config.TriggerBotKey = TriggerKey;
            Config.TriggerBotDelayMs = (int)TriggerBotDelaySlider.Value;
            Config.TriggerBotDelayRandomMs = (int)TriggerBotRandomSlider.Value;

            Config.EnableAimBot = AimBotCheckBox.IsChecked ?? false;
            Config.AimBotKey = AimKey;
            Config.AimSpeed = (int)AimBotSpeedSlider.Value;
            Config.AimPixelFov = (int)AimBotFovSlider.Value;
            Config.AimSwitchDelayMs = (int)AimBotDelaySlider.Value;
        }

        private void UpdateStatus(string status)
        {
            Dispatcher.Invoke(() => { Status.Content = status; }, DispatcherPriority.Normal);
        }

        [STAThread]
        private void RunCheat()
        {
            UpdateStatus("Starting");

            var runDir = Directory.GetCurrentDirectory();

            // run offset dumper
            UpdateStatus("Dumping Offsets");
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(runDir + "/offset_dumper/cs2-dumper.exe")
            {
                WorkingDirectory = runDir + "/offset_dumper",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            p.Start();

            // wait for dump to finish
            while (true)
            {
                Thread.Sleep(100);
                if (p.HasExited) break;
            }

            // read offsets
            UpdateStatus("Reading Offsets");
            Offsets.ReadOffsets(runDir);

            // start cheat
            UpdateStatus("Starting Cheat");
            Cheat.OnStartup();
            UpdateStatus("Started Cheat");
        }

        private void Apply_OnClick(object sender, RoutedEventArgs e)
        {
            SaveToMemory();
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            SaveToFile();
        }

        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
            LoadFromFile();
        }
    }
}