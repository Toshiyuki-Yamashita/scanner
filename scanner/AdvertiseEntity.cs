﻿using System;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;

namespace scanner
{
    using Util;
    internal class AdvertiseEntity
    {
        [SQLite.AutoIncrement, SQLite.PrimaryKey]
        public int id { get; set; }
        public DateTimeOffset TimeStamp;
        public string Name { get; set; } = "";
        public string CompanyId { get; set; } = "";
        public BluetoothLEAdvertisementType AdvertisementType { get; set; } = BluetoothLEAdvertisementType.ConnectableDirected;
        public BluetoothAddressType BluetoothAddressType { get; set; } = BluetoothAddressType.Unspecified;
        public string BluetoothAddress { get; set; } = "";
        public short RawSignalStrengthInDBm {get; set;} = 0;
        public short TransmitPowerLevelInDBm { get; set; } = 127;
        public bool IsAnonymous { get; set; } = false;
        public bool IsConnectable {get; set;} = false;
        public bool IsScannable  {get; set;} = false;
        public bool IsDirected {get; set;} = false;
        public bool IsScanResponse  { get; set;} = false;

        public AdvertiseEntity()
        {

        }

        public AdvertiseEntity(BluetoothLEAdvertisementReceivedEventArgs data)
        {
            TimeStamp = data.Timestamp;
            Name = data.Advertisement.LocalName;
            if ( data.Advertisement.ManufacturerData.FirstOrDefault() is BluetoothLEManufacturerData md)
            {
                CompanyId = md.CompanyId.ToString("X4");
            }
            AdvertisementType = data.AdvertisementType;
            BluetoothAddressType = data.BluetoothAddressType;
            BluetoothAddress = data.BluetoothAddress.ToMacAddressString();
            RawSignalStrengthInDBm =  data.RawSignalStrengthInDBm;
            TransmitPowerLevelInDBm = data.TransmitPowerLevelInDBm ?? 127;
            IsAnonymous = data.IsAnonymous;
            IsConnectable =  data.IsConnectable;
            IsScannable = data.IsScannable;
            IsDirected= data.IsDirected;
            IsScanResponse =data.IsScanResponse;
        }
    }
}
