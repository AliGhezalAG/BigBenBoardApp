using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using log4net;
using RestWCFServiceLibrary.WiiMote.Model;
using WiimoteLib;

namespace RestWCFServiceLibrary.WiiMote
{
    public class WiiBoardGetDatas : IDisposable
    {
        private static readonly ILog LOG = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly DataStore dataStore = new DataStore();
        private static readonly DataMapper dataMapper = new DataMapper();
        private static readonly ProxyStatiqueServices proxyStatiqueServices = new ProxyStatiqueServices();

        private DateTime acquisitionStartedAt;

        public SynchronizedCollection<WiiBoardData> WiiBoardDatas { get; } = new SynchronizedCollection<WiiBoardData>();
 
        public bool AcquisitionActive { get; private set; } = false;
        public IList<string> YeuxDone { get { return dataStore.YeuxDone(); } }

        private readonly DeviceAccess wiiAccess;

        public WiiBoardGetDatas(DeviceAccess wiiAccess)
        {
            this.wiiAccess = wiiAccess;
        }

        public string StartAcquisition()
        {
            // RAZ des données précédentes
            LOG.DebugFormat("<<< StartAcquisition()");

            WiiBoardDatas.Clear();
            this.acquisitionStartedAt = DateTime.UtcNow;
            // Start acquisition
            var response = StartLowLevelAcquisition();

            AcquisitionActive = true;

            LOG.DebugFormat(">>> StartAcquisition() (OK)");
            return response;
        }

        public string StopAcquisition(bool keepDatas)
        {
            LOG.DebugFormat("<<< StopAcquisition(): keepDatas={0}", keepDatas);
            string response;
            try
            {
                response = StopLowLevelAcquisition();
            }
            finally
            {
                if (!keepDatas)
                {
                    //RAZ des données acquises
                    WiiBoardDatas.Clear();
                }
                AcquisitionActive = false;
            }
            
            response = "Acquisition stopped";
            LOG.Debug(">>> StopAcquisition(bool) (OK)");
            return response;
        }

        public string StopAcquisition(string guid, string yeux, string date)
        {
            LOG.DebugFormat("<<< StopAcquisition(): guid={0}, yeux={1}, date={2}", guid, yeux, date);
            string response;
            try
            {
                dataStore.Clear(yeux);
                response = StopLowLevelAcquisition();

                var file = dataMapper.MapToAcquisitionFile(WiiBoardDatas, date, yeux);
                dataStore.Put(yeux, file);
                response = "Raw data generated";
            }
            finally
            {
                AcquisitionActive = false;
            }

            LOG.Debug(">>> StopAcquisition(string,string,string) (OK)");
            return response;
        }

        public async Task<string> PushAcquisitionAsync(string guid, string yeux, string date, string authorizationHeader)
        {
            LOG.DebugFormat("PushAcquisition: guid={0}, yeux={1}, date={2}", guid, yeux, date);

            var response = await proxyStatiqueServices.CreateTempFolderAsync(guid, authorizationHeader);

            var file = dataStore.Get(yeux);
            if (null == file)
            {
                var message = string.Format("No acquisition file available for given parameters: guid={0}, yeux={1}, date={2}", guid, yeux, date);
                LOG.WarnFormat(message);
            }

            response = await proxyStatiqueServices.PushAsync(guid, file, authorizationHeader);

            dataStore.Clear(yeux);

            LOG.Debug(">>> PushAcquisition(string,string,string) (OK)");
            return response;
        }

        private string StartLowLevelAcquisition()
        {
            LOG.Debug("<<< startLowLevelAcquisition()");
            var wiiboard = wiiAccess.WiiBoard;

            if (wiiboard == null)
            {
                LOG.WarnFormat("Wiiboard not found");
            }

            wiiboard.WiimoteChanged += WiimoteChanged;
            wiiboard.WiimoteExtensionChanged += WiimoteExtensionChanged;

            wiiboard.Connect();
            if (wiiboard.Connected)
                wiiboard.SetLEDs(1);// Pas utile ici à mon avis !
            else
            {
                // add a retry
                wiiboard.Connect();
                // An ugly sleep.
                Task.Delay(1000).Wait();
                wiiboard.SetLEDs(1);// Pas utile ici à mon avis !
            }
            var msg = "Acquisition de la WiiBoard " + wiiboard.HIDDevicePath + " activée";
            LOG.DebugFormat(">>> startLowLevelAcquisition(): {0}", msg);

            return msg;
        }

        private string StopLowLevelAcquisition()
        {
            LOG.Debug("<<< stopLowLevelAcquisition()");

            var wiiboard = wiiAccess.WiiBoard;

            if (wiiboard == null)
            {
                LOG.WarnFormat("Wiiboard not found");
            }

            if(wiiboard.Connected)
                wiiboard.SetLEDs(0);// Pas utile ici à mon avis !
            wiiboard.Disconnect();

            wiiboard.WiimoteChanged -= WiimoteChanged;
            wiiboard.WiimoteExtensionChanged -= WiimoteExtensionChanged;

            var msg = "Acquisition de la WiiBoard " + wiiboard.HIDDevicePath + " désactivée";

            LOG.DebugFormat(">>> stopLowLevelAcquisition(): {0}", msg);

            return msg;
        }

        private void WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs e)
        {
            LOG.Debug("NotImplemented: wiimoteExtensionChanged()");
        }

        private void WiimoteChanged(object sender, WiimoteChangedEventArgs e)
        {
            WiiBoardData singleDataPoint = dataMapper.MapFromWiiFormat(e.WiimoteState, this.acquisitionStartedAt);

            LOG.DebugFormat("Current value: topLeftKg={0}, topRightKg={1}, bottomLeftKg={2}, bottomRightKg={3}, weightKg={4}, gravity.X={5}, gravity.Y={6}, Horodate={7}, TIMESTAMP={8}", singleDataPoint.topLeftKg, singleDataPoint.topRightKg, singleDataPoint.bottomLeftKg, singleDataPoint.bottomRightKg, singleDataPoint.weightKg, singleDataPoint.gravity.X, singleDataPoint.gravity.Y, singleDataPoint.Horodate, singleDataPoint.TIMESTAMP);

            if (IsValidData(singleDataPoint))
            {
                WiiBoardDatas.Add(singleDataPoint);
                #if DEBUG
                LOG.DebugFormat("Wiimote callback from ID={0}: data was collected.", ((Wiimote)sender).ID);
                #endif
            }
            else
            {
                LOG.DebugFormat("Wiimote callback from ID={0}: no valid data.", ((Wiimote)sender).ID);
            }
        }

        /// <summary>
        /// Filtrer les NaN sinon plantage à la sérialisation.
        /// Filter les valeurs à 0 car inutiles.
        /// </summary>
        /// <param name="singleDataPoint"></param>
        /// <returns></returns>
        private static bool IsValidData(WiiBoardData singleDataPoint)
        {
            return !float.IsNaN(singleDataPoint.gravity.X) &
                            singleDataPoint.bottomLeftKg != singleDataPoint.bottomRightKg &
                            singleDataPoint.topLeftKg != singleDataPoint.topRightKg;
        }


#region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopLowLevelAcquisition();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
#endregion
    }
}