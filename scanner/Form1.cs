using Windows.Devices.Bluetooth.Advertisement;
using System.Reflection;
namespace scanner
{
    public partial class Form1 : Form
    {
        private BluetoothLEAdvertisementWatcher watcher;
        private System.ComponentModel.BindingList<BluetoothLEAdvertisementReceivedEventArgs> db;
        bool IsStarted
        {
            get
            {
                return watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started;
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
            watcher = new BluetoothLEAdvertisementWatcher();
            watcher.ScanningMode = BluetoothLEScanningMode.Passive;
            watcher.Received += OnReceived;
            db = new System.ComponentModel.BindingList<BluetoothLEAdvertisementReceivedEventArgs>();
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = db;
            dataGridView1.GetType().InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                dataGridView1,
                new object[] { true});

        }

        private async void OnReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs ev)
        {
            var data = await Task.Run(() => {
                return ev;
            });
            add_data(data);
        }

        private void log_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                logpath.Text = dialog.SelectedPath;
            }
        }
        private void add_data(BluetoothLEAdvertisementReceivedEventArgs data)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                Invoke((Action)(() => { add_data(data); }));
            }
            else
            {
                if (db.Count >= 100)
                {
                    db.RemoveAt(0);
                }
                db.Add(data);
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            IsStarted = !IsStarted;
        }
        private void stop_watcher()
        {
            if (!IsStarted) return;
            watcher.Stop();
        }
        private void start_watcher()
        {
            if (IsStarted) return;
            watcher.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            IsStarted = false;
        }
    }
}