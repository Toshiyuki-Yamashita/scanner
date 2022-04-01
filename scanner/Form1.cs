using System.ComponentModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;

namespace scanner
{
    using SQLite;
    using Util;
    public partial class Form1 : Form
    {
        private BluetoothLEAdvertisementWatcher advertiseWatcher;
        private DeviceWatcher deviceWatcher;
        private BindingList<AdvertiseEntity> advertise;
        private BindingList<DeviceEntity> device;
        private SQLiteConnection? connection = null;
        bool IsStarted
        {
            get
            {
                return advertiseWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Started;
            }
            set
            {
                if (value)
                {
                    start_watcher();
                }
                else
                {
                    stop_watcher();
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            advertiseWatcher = new BluetoothLEAdvertisementWatcher();
            advertiseWatcher.ScanningMode = BluetoothLEScanningMode.Passive;
            advertiseWatcher.Received += OnReceived;
            advertise = new BindingList<AdvertiseEntity>();
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = advertise;
            dataGridView1.SetProperty("DoubleBuffered", true);

            deviceWatcher = DeviceInformation.CreateWatcher(
                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" },
                DeviceInformationKind.AssociationEndpoint);
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            device = new BindingList<DeviceEntity>();
            dataGridView2.AutoGenerateColumns = true;
            dataGridView2.DataSource = device;
            dataGridView2.SetProperty("DoubleBuffered", true);

        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (await BluetoothLEDevice.FromIdAsync(args.Id) is BluetoothLEDevice btdev)
            {
                add_data(new DeviceEntity("Removed", btdev));
            }
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (await BluetoothLEDevice.FromIdAsync(args.Id) is BluetoothLEDevice btdev)
            {
                add_data(new DeviceEntity("Updated", btdev));
            }
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (await BluetoothLEDevice.FromIdAsync(args.Id) is BluetoothLEDevice btdev)
            {
                add_data(new DeviceEntity("Added", btdev));
            }
        }

        private void OnReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs ev)
        {
            add_data(new AdvertiseEntity(ev));
        }

        private void log_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "db file(*.sqlite)|*.sqlite| All files (*.*)|*.*";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                logpath.Text = dialog.FileName;
            }
        }
        private void add_data(DeviceEntity entity)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                try
                {
                    Invoke((Action)(() => { add_data(entity); }));
                }
                catch
                {
                    // do nothing 
                }
            }
            else
            {
                if (device.Count >= 100)
                {
                    advertise.RemoveAt(0);
                }
                device.Add(entity);
                if (connection is SQLiteConnection)
                {
                    connection.Insert(entity);
                }
            }

        }
        private void add_data(AdvertiseEntity entity)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                try
                {
                    Invoke((Action)(() => { add_data(entity); }));
                }
                catch
                {
                    // do nothing 
                }
            }
            else
            {
                if (advertise.Count >= 100)
                {
                    advertise.RemoveAt(0);
                }
                advertise.Add(entity);
                if (connection is SQLiteConnection)
                {
                    connection.Insert(entity);
                }
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            IsStarted = !IsStarted;
        }
        private void start_watcher()
        {
            if (IsStarted) return;

            string? path = ":memory:";
            if (logpath.Text != String.Empty)
            {
                path = logpath.Text;

            }
            connection = new SQLiteConnection(path);
            connection.CreateTable<AdvertiseEntity>();
            connection.CreateTable<DeviceEntity>();
            advertiseWatcher.Start();
            deviceWatcher.Start();
        }

        private void stop_watcher()
        {
            if (!IsStarted) return;
            advertiseWatcher.Stop();
            deviceWatcher.Stop();
            if (connection is SQLiteConnection)
            {
                connection.Close();
                connection = null;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            IsStarted = false;
        }
    }
}