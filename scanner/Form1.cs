using System.ComponentModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using scanner.Data.Entities;

namespace scanner
{
    using SQLite;
    using Extensions;
    public partial class Form1 : Form
    {
        private BluetoothLEAdvertisementWatcher advertiseWatcher;
        private DeviceWatcher deviceWatcher;
        private BindingList<AdvertiseEntity> advertise;
        private BindingList<DeviceEntity> device;
        private SQLiteAsyncConnection? connection = null;
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
                add_data(new DeviceEntity(memo.Text, "Removed", btdev), device);
            }
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (await BluetoothLEDevice.FromIdAsync(args.Id) is BluetoothLEDevice btdev)
            {
                add_data(new DeviceEntity(memo.Text, "Updated", btdev), device);
            }
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (await BluetoothLEDevice.FromIdAsync(args.Id) is BluetoothLEDevice btdev)
            {
                add_data(new DeviceEntity(memo.Text, "Added", btdev), device);
            }
        }

        private void OnReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs ev)
        {
            add_data(new AdvertiseEntity(memo.Text, ev), advertise);
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
        
        private  void add_data<Entity>(Entity entity, BindingList<Entity> db) where Entity : notnull
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke((Action)(() => { add_data(entity, db); }));
                }
                catch
                {
                    // do nothing 
                }
                return;
            }
            else
            {
                connection?.InsertAsync(entity);
                if (db.Count >= 100)
                {
                    db.RemoveAt(0);
                }
                db.Add(entity);
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            IsStarted = !IsStarted;
        }
        private async void start_watcher()
        {
            if (IsStarted) return;

            string? path = ":memory:";
            if (logpath.Text != String.Empty)
            {
                path = logpath.Text;

            }
            connection = new SQLiteAsyncConnection(path, storeDateTimeAsTicks:false) ;
            var createadv = connection.CreateTableAsync<AdvertiseEntity>();
            var createdev = connection.CreateTableAsync<DeviceEntity>();
            await Task.WhenAll(createadv, createdev);
            advertiseWatcher.Start();
            deviceWatcher.Start();
        }

        private void stop_watcher()
        {
            if (!IsStarted) return;
            advertiseWatcher.Stop();
            deviceWatcher.Stop();

        }

        private async  void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            IsStarted = false;
            
            if (connection is SQLiteAsyncConnection)
            {
                await connection.CloseAsync();
                connection = null;
            }
        }
    }
}