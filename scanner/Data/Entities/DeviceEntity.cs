using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using scanner.Extensions;


namespace scanner.Data.Entities
{

    internal class DeviceEntity : scanner.Data.Entities.BaseEntity
    {
        [SQLite.AutoIncrement, SQLite.PrimaryKey]
        public int id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public BluetoothAddressType BluetoothAddressType { get; set; } = BluetoothAddressType.Unspecified;
        public string BluetoothAddress { get; set; } = "";
        public BluetoothConnectionStatus ConnectionStatus { get; set; }
        public DeviceAccessStatus AccessStatus { get; set; }
        public string Operation { get; set; } = string.Empty;

        public DeviceEntity()
        {

        }

        public DeviceEntity(string memo, string operation, BluetoothLEDevice bt)
        {
            Name = bt.Name;
            Memo = memo;
            TimeStamp = DateTime.Now;
            BluetoothAddressType = bt.BluetoothAddressType;
            BluetoothAddress = bt.BluetoothAddress.ToMacAddressString();
            ConnectionStatus = bt.ConnectionStatus;
            AccessStatus = bt.DeviceAccessInformation.CurrentStatus;
            Operation = operation;
        }
    }
}
