using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using log4net;
using Microsoft.Win32;
using WiimoteLib;
using System.Threading.Tasks;

namespace RestWCFServiceLibrary.WiiMote
{
    public class DeviceAccess
    {
        private static readonly ILog LOG = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string WII_BOARD_NAME = "Nintendo RVL-WBC-01";


        Wiimote wiiboard = null;

        public Wiimote WiiBoard
        {
            get
            {
                if (wiiboard == null)
                    SearchWiiBoard();
                return wiiboard;
            }
        }


        public void SearchWiiBoard()
        {
            Console.WriteLine("searching...");
            WiimoteCollection wiimoteCollection = new WiimoteCollection();
            try
            {
                wiimoteCollection.FindAllWiimotes();
            }
            catch (WiimoteNotFoundException ex)
            {
                LOG.Error("Wiimote not found error", ex);
                return;
            }
            catch (WiimoteException ex)
            {
                LOG.Error("Wiimote error", ex);
                return;
            }
            catch (Exception ex)
            {
                LOG.Error("Unknown error", ex);
                return;
            }

            Console.WriteLine("found " + wiimoteCollection.ToList().Count);
            foreach (Wiimote current in wiimoteCollection)
            {
                try
                {
                    current.Connect();
                    current.SetLEDs(true, false, false, false);
                    var founded = current.WiimoteState.ExtensionType == ExtensionType.BalanceBoard;
                    current.Disconnect();
                    if (founded)
                    {
                        Console.WriteLine("Hola");
                        wiiboard = current;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error(string.Format("Error durring access to Wiimote {0}.",current.HIDDevicePath ) , ex);
                }
                
            }
        }

        public string GetBluetoothState()
        {
            string state = string.Empty;
            var radios = BluetoothRadio.PrimaryRadio;
            if (radios != null)
            {
                state = radios.Mode.ToString();
            }
            else
            {
                state = "Not Enabled";
            }
            LOG.DebugFormat("GetBluetoothState : {0}", state);
            return state;
        }

        public string GetWiiBoardState()
        {
            string state = "WiiBoard not discovered";

            if (BluetoothRadio.PrimaryRadio == null)
            {
                LOG.Info("Bluetooth not enabled");
                return "Bluetooth not enabled";
            }

            using (BluetoothClient bluetoothClient = new BluetoothClient())
            {
                try
                {
                    List<string> lstaddrStr = GetBluetoothRegistryName();
                    foreach (string addrStr in lstaddrStr)
                    {
                        BluetoothAddress addr = new BluetoothAddress(Convert.ToInt64(addrStr, 16));
                        BluetoothDeviceInfo deviceInfo = new BluetoothDeviceInfo(addr);
                        if (deviceInfo.Connected)
                        {
                            LOG.DebugFormat("WiiBoard connected : {0}", addrStr);
                            state = "WiiBoard connected";
                        }
                        else if (state != "WiiBoard connected")
                        {
                            LOG.DebugFormat("WiiBoard not connected : {0}", addrStr);
                            state = "WiiBoard not connected";
                        }
                    }
                }
                catch (Exception ex)
                {
                    LOG.Warn(ex);
                }
            }
            LOG.DebugFormat("GetWiiBoardState : {0}", state);
            return state;
        }

        private List<string> GetBluetoothRegistryName()
        {
            List<string> deviceAdrr = new List<string>();
            string registryPath = @"SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
            {
                if (key != null)
                {
                    string[] valueNames = key.GetSubKeyNames();
                    foreach (string currSubKey in valueNames)
                    {
                        string devicePath = String.Format(@"{0}\{1}", registryPath, currSubKey);
                        using (RegistryKey device = Registry.LocalMachine.OpenSubKey(devicePath))
                        {
                            Object o = device.GetValue("Name");
                            byte[] raw = o as byte[];
                            if (raw != null)
                            {
                                if (Encoding.ASCII.GetString(raw) == string.Concat(WII_BOARD_NAME, "\0"))
                                {
                                    deviceAdrr.Add(currSubKey);
                                }
                            }
                        }
                    }
                }
            }
            return deviceAdrr;
        }

        public int TryDeviceSearchAndServiceConnect()
        {
            int wiiConnectstatus = -1;

            using (BluetoothClient bluetoothClient = new BluetoothClient())
            {
                try
                {
                    BluetoothDeviceInfo[] btDevicesDiscovered = bluetoothClient.DiscoverDevices();
                    for (int i = 0; i < btDevicesDiscovered.Length; i++)
                    {
                        BluetoothDeviceInfo bluetoothDeviceInfo = btDevicesDiscovered[i];
                        LOG.InfoFormat("TryDeviceSearchAndServiceConnect - Device name : {0} ", bluetoothDeviceInfo.DeviceName);
                        if (bluetoothDeviceInfo.DeviceName.Contains(WII_BOARD_NAME))
                        {
                            BluetoothDeviceInfo bluetoothWiiBoardDeviceInfo = bluetoothDeviceInfo;
                            //PermanentSync
                            if (!bluetoothDeviceInfo.Remembered)
                            {
                                #region PermanentSync
                                string pin = this.AddressToWiiPin(BluetoothRadio.PrimaryRadio.LocalAddress.ToString());
                                LOG.InfoFormat("code pin : {0}", pin);
                                new BluetoothWin32Authentication(bluetoothDeviceInfo.DeviceAddress, pin);
                                BluetoothSecurity.PairRequest(bluetoothDeviceInfo.DeviceAddress, pin);
                                #endregion
                            }
                            // Enable service !
                            bluetoothDeviceInfo.SetServiceState(BluetoothService.HumanInterfaceDevice, true);
                            wiiConnectstatus = 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LOG.Warn(ex);
                    wiiConnectstatus = 99;
                }
            }

            // reinit wiiboard to null to force a search at next aquisition.
            wiiboard = null;

            return wiiConnectstatus;
        }

        public int ConnectToWiiBoard()
        {
            int wiiConnectstatus = -1;
            WiimoteCollection wiimoteCollection = new WiimoteCollection();
            wiimoteCollection.FindAllWiimotes();
            try
            {
                foreach (Wiimote current in wiimoteCollection)
                {
                    current.Connect();
                    current.SetLEDs(true, false, false, false);
                    current.Disconnect();
                    wiiConnectstatus = 1; // connecté 
                }
            }
            catch (Exception ex)
            {
                LOG.Warn(ex);
                wiiConnectstatus = 99;
            }

            // reinit wiiboard to null to force a search at next aquisition.
            wiiboard = null;
            return wiiConnectstatus;
        }

        private string AddressToWiiPin(string bluetoothAddress)
        {
            if (bluetoothAddress.Length != 12)
            {
                throw new Exception("Invalid Bluetooth Address: " + bluetoothAddress);
            }
            string text = "";
            for (int i = bluetoothAddress.Length - 2; i >= 0; i -= 2)
            {
                string value = bluetoothAddress.Substring(i, 2);
                text += (char)Convert.ToInt32(value, 16);
            }
            return text;
        }

        public bool ChangeStateWiiBoard(bool state)
        {
            bool found = false;
            using (BluetoothClient bluetoothClient = new BluetoothClient())
            {
                BluetoothDeviceInfo[] btDevicesDiscovered = bluetoothClient.DiscoverDevices(2048, true, true, false);
                foreach(var device in btDevicesDiscovered.Where(e => e.DeviceName.Contains(WII_BOARD_NAME)))
                {
                    device.SetServiceState(BluetoothService.HumanInterfaceDevice, state);
                    found = true;

                    var toto = device.InstalledServices;
                }
            }
            return found;
        }

        public int TryDeviceDisconnect(int codeForce)
        {
            int wiiConnectstatus = -1;
            bool withoutService = false;
            using (BluetoothClient bluetoothClient = new BluetoothClient())
            {
                BluetoothDeviceInfo[] btDevicesDiscovered = null;
                switch (codeForce)
                {
                    case 0:
                        {
                            btDevicesDiscovered = bluetoothClient.DiscoverDevices(255, true, true, false); // un connu !
                            break;
                        }
                    case 1:
                        {
                            btDevicesDiscovered = bluetoothClient.DiscoverDevices(255, true, true, false); // un connu !
                            withoutService = true;
                            break;
                        }
                    case 99:
                        {
                            btDevicesDiscovered = bluetoothClient.DiscoverDevices();
                            break;
                        }
                    default:
                        {
                            LOG.Warn("Pour faire plaisir à S.M.");
                            break;
                        }
                }
                for (int i = 0; i < btDevicesDiscovered.Length; i++)
                {
                    BluetoothDeviceInfo bluetoothDeviceInfo = btDevicesDiscovered[i];
                    LOG.DebugFormat("device trouvé : {0} - {1} - {2}", bluetoothDeviceInfo.DeviceName, bluetoothDeviceInfo.DeviceAddress, bluetoothDeviceInfo.ClassOfDevice);
                    if (bluetoothDeviceInfo.DeviceName.Contains(WII_BOARD_NAME))
                    {
                        try
                        {
                            WiimoteCollection wiimoteCollection = new WiimoteCollection();
                            wiimoteCollection.FindAllWiimotes();
                            foreach (Wiimote current in wiimoteCollection)
                            {
                                current.Connect();
                                current.SetLEDs(false, false, false, false);
                                current.Disconnect();
                                wiiConnectstatus = 1;
                            }
                            if (!withoutService)
                            {
                                LOG.DebugFormat("Suppression du servie BT pour le device {0}", bluetoothDeviceInfo.DeviceName);
                                BluetoothSecurity.RemoveDevice(bluetoothDeviceInfo.DeviceAddress);
                                bluetoothDeviceInfo.SetServiceState(BluetoothService.HumanInterfaceDevice, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            LOG.Warn(ex);
                            wiiConnectstatus = 99;
                        }
                        finally
                        {
                            if (codeForce == 99)
                            {
                                LOG.WarnFormat("Forçage de suppression du service BT {0} - {1}", bluetoothDeviceInfo.DeviceName, bluetoothDeviceInfo.DeviceAddress);
                                BluetoothSecurity.RemoveDevice(bluetoothDeviceInfo.DeviceAddress);
                                bluetoothDeviceInfo.SetServiceState(BluetoothService.HumanInterfaceDevice, false);
                                wiiConnectstatus = 1;
                            }
                        }
                    }
                }
            }
            wiiboard = null;
            return wiiConnectstatus;
        }
    }
}
