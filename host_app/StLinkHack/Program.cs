using Device.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using Usb.Net;
using Usb.Net.Windows;

namespace StLinkHack
{
    class Program
    {
        static List<FilterDeviceDefinition> deviceFilters;

        static async Task<int> Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: StLinkHack <payload> <serial_port> <stack_offset> <output_path>");
                return 1;
            }

            string payloadPath = args[0];
            string serialPortName = args[1];
            uint stackOffset;
            try
            {
                stackOffset = (uint)new UInt32Converter().ConvertFromString(args[2]);
            }
            catch
            {
                Console.WriteLine("Cannot parse stack offset.");
                return 2;
            }
            string outputPath = args[3];

            try
            {
                DeviceManager.Current.DeviceFactories.Add(new WindowsUsbDeviceFactory(null, null));
                deviceFilters = new List<FilterDeviceDefinition>
                {
                    // ST-Link/V2-1
                    new FilterDeviceDefinition { DeviceType = DeviceType.Usb, VendorId = 0x0483, ProductId = 0x374b },
                    // ST-Link/V2-1 bootloader
                    new FilterDeviceDefinition { DeviceType = DeviceType.Usb, VendorId = 0x0483, ProductId = 0x3748 }
                };

                Console.WriteLine("Checking device...");
                using (var device = await GetDevice())
                {
                    // Get mode
                    var readResult = await device.WriteAndReadAsync(MakeSimpleCommand(0xf5));
                    ushort result = (ushort)(readResult.Data[0] | (readResult.Data[1] << 8));

                    if (result < 0x200)
                    {
                        Console.WriteLine("Rebooting to bootloader");
                        // Go to bootloader mode
                        await device.WriteAsync(MakeSimpleCommand(0xf9, 0x01));
                    }
                }

                await Task.Delay(1000);

                // Open serial
                using var serial = new SerialPort(serialPortName, 115200, Parity.None, 8, StopBits.One);
                serial.Open();

                using (var device = await GetDevice())
                {
                    Console.WriteLine("Sending payload...");
                    byte[] payload = File.ReadAllBytes(payloadPath);
                    await DfuSetAddress(device, stackOffset);
                    await DownloadData(device, payload, 1);
                }

                Console.WriteLine("Waiting for start sequence...");
                serial.ReadTo("start");

                Console.WriteLine("Dumping!");
                byte[] firmware = new byte[0x20000];
                int read = 0;
                while (read < firmware.Length)
                {
                    read += serial.Read(firmware, read, firmware.Length - read);
                }
                File.WriteAllBytes(outputPath, firmware);

                Console.WriteLine("Done!");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong: " + ex);
                return -1;
            }
        }

        static async Task DownloadData(UsbDevice device, byte[] data, ushort blockNum)
        {
            await device.WriteAsync(MakeDownloadCommand(blockNum, (ushort)data.Length, 0));
            for (int i = 0; i < data.Length;)
            {
                byte[] curr = new byte[Math.Min(0x40, data.Length - i)];
                Buffer.BlockCopy(data, i, curr, 0, curr.Length);
                await device.WriteAsync(curr);
                i += curr.Length;
            }
            var result = await device.WriteAndReadAsync(MakeSimpleCommand(0xf3, 0x03));
            var resultBytes = result.Data;
            int timeout = resultBytes[1] | (resultBytes[2] << 8) | (resultBytes[2] << 16);
            await Task.Delay(timeout);
            await device.WriteAndReadAsync(MakeSimpleCommand(0xf3, 0x03));
        }

        static byte[] MakeDownloadCommand(ushort blockNum, ushort length, ushort checksum)
        {
            return MakeSimpleCommand(0xf3, 0x01, (byte)blockNum, (byte)(blockNum >> 8),
                (byte)checksum, (byte)(checksum >> 8), (byte)length, (byte)(length >> 8));
        }

        static async Task DfuSetAddress(UsbDevice device, uint address)
        {
            byte[] command = new byte[] { 0x21, (byte)address, (byte)(address >> 8), (byte)(address >> 16), (byte)(address >> 24) };
            await DownloadData(device, command, 0);
        }

        static byte[] MakeSimpleCommand(params byte[] cmd)
        {
            if (cmd.Length > 0x10) throw new ArgumentException("Command is too long", nameof(cmd));
            byte[] outBytes = new byte[16];
            for (int i = 0; i < cmd.Length; ++i)
            {
                outBytes[i] = cmd[i];
            }
            return outBytes;
        }

        static async Task<UsbDevice> GetDevice()
        {
            var devices = await DeviceManager.Current.GetDevicesAsync(deviceFilters);
            if (devices.Count != 1) throw new Exception("No devices or more than 1 device");
            if (!(devices[0] is UsbDevice device)) throw new Exception("Device is not USB?");
            await device.InitializeAsync();
            return device;
        }
    }
}
