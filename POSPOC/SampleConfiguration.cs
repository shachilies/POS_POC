using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.PointOfService;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace POSPOC
{
    public partial class DeviceHelpers
    {
        public static async Task<BarcodeScanner> GetFirstBarcodeScannerAsync(PosConnectionTypes connectionTypes = PosConnectionTypes.All)
        {
            return await DeviceHelpers.GetFirstDeviceAsync(BarcodeScanner.GetDeviceSelector(connectionTypes), async (id) => await BarcodeScanner.FromIdAsync(id));
        }
    }

    public partial class DataHelpers
    {
        public static string GetDataString(IBuffer data)
        {
            if (data == null)
            {
                return "No data";
            }

            // Just to show that we have the raw data, we'll print the value of the bytes.
            // Arbitrarily limit the number of bytes printed to 20 so the UI isn't overloaded.
            string result = CryptographicBuffer.EncodeToHexString(data);
            if (result.Length > 40)
            {
                result = result.Substring(0, 40) + "...";
            }
            return result;
        }

        public static string GetDataLabelString(IBuffer data, uint scanDataType)
        {
            // Only certain data types contain encoded text.
            // To keep this simple, we'll just decode a few of them.
            if (data == null)
            {
                return "No data";
            }

            // This is not an exhaustive list of symbologies that can be converted to a string.
            if (scanDataType == BarcodeSymbologies.Upca ||
                scanDataType == BarcodeSymbologies.UpcaAdd2 ||
                scanDataType == BarcodeSymbologies.UpcaAdd5 ||
                scanDataType == BarcodeSymbologies.Upce ||
                scanDataType == BarcodeSymbologies.UpceAdd2 ||
                scanDataType == BarcodeSymbologies.UpceAdd5 ||
                scanDataType == BarcodeSymbologies.Ean8 ||
                scanDataType == BarcodeSymbologies.TfStd ||
                scanDataType == BarcodeSymbologies.OcrA ||
                scanDataType == BarcodeSymbologies.OcrB)
            {
                // The UPC, EAN8, and 2 of 5 families encode the digits 0..9
                // which are then sent to the app in a UTF8 string (like "01234").
                return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, data);
            }

            // Some other symbologies (typically 2-D symbologies) contain binary data that
            // should not be converted to text.
            return string.Format("Decoded data unavailable. Raw label data: {0}", DataHelpers.GetDataString(data));
        }
    }

    public partial class BindingHelpers
    {
        // Inverters for binding.
        public static bool Not(bool value) => !value;
        public static Visibility CollapsedIf(bool value) => value ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <summary>
    /// This class is used for data-binding.
    /// </summary>
    public class BarcodeScannerInfo
    {
        public BarcodeScannerInfo(String deviceName, String deviceId)
        {
            DeviceName = deviceName;
            DeviceId = deviceId;
        }

        public String Name => $"{DeviceName} ({DeviceId})";
        public String DeviceId { get; private set; }
        private string DeviceName;
    }

    /// <summary>
    /// This class is used for data-binding.
    /// </summary>
    public class SymbologyListEntry
    {
        public SymbologyListEntry(uint symbologyId, bool symbologyEnabled = true)
        {
            Id = symbologyId;
            IsEnabled = symbologyEnabled;
        }

        public uint Id { get; private set; }
        public bool IsEnabled { get; set; }
        public String Name => BarcodeSymbologies.GetName(Id);
    }
}
