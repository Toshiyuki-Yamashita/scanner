using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;

namespace scanner
{
    using Util;
    internal class DeviceEntity
    {
        [SQLite.AutoIncrement, SQLite.PrimaryKey]
        public int id { get; set; }
        public string Name { get; set; } = string.Empty;
        public BluetoothAddressType BluetoothAddressType { get; set; } = BluetoothAddressType.Unspecified;
        public string BluetoothAddress { get; set; } = "";
        public BluetoothConnectionStatus ConnectionStatus { get; set; }
        public DeviceAccessStatus AccessStatus { get; set; }
        public string Operation { get; set; } = string.Empty;

        public DeviceEntity()
        {

        }

        public DeviceEntity(string operation , BluetoothLEDevice bt)
        {
            Name = bt.Name;
            BluetoothAddressType = bt.BluetoothAddressType;
            BluetoothAddress = bt.BluetoothAddress.ToMacAddressString();
            ConnectionStatus = bt.ConnectionStatus;
            AccessStatus = bt.DeviceAccessInformation.CurrentStatus;
            Operation = operation;
        }
    }
}
