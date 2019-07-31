using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using RestWCFServiceLibrary.WiiMote.Model;
using WiimoteLib;

namespace RestWCFServiceLibrary.WiiMote
{
    public class DataMapper
    {
        private static readonly ILog LOG = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public WiiBoardData MapFromWiiFormat(WiimoteState wiimoteState, DateTime acquisitionStartedAt)
        {

#if DEBUG
            // too verbose for release

            if (LOG.IsDebugEnabled)
            {
                var value = JsonConvert.SerializeObject(wiimoteState, Formatting.Indented);
                LOG.Debug(value);
            }
#endif

            var balanceBoardState = wiimoteState.BalanceBoardState;

            return new WiiBoardData()
            {
                bottomLeftKg = balanceBoardState.SensorValuesKg.BottomLeft,
                bottomRightKg = balanceBoardState.SensorValuesKg.BottomRight,
                topLeftKg = balanceBoardState.SensorValuesKg.TopLeft,
                topRightKg = balanceBoardState.SensorValuesKg.TopRight,
                gravity = new PointGravity()
                {
                    X = balanceBoardState.CenterOfGravity.X,
                    Y = balanceBoardState.CenterOfGravity.Y
                },
                weightKg = balanceBoardState.WeightKg,
                TIMESTAMP = (DateTime.UtcNow - acquisitionStartedAt).TotalSeconds,
                Horodate = JsonConvert.SerializeObject(DateTime.UtcNow, Formatting.Indented, new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                })
            };
        }

        public AcquisitionFile MapToAcquisitionFile(IList<WiiBoardData> wiiBoardDatas, string date, string yeux)
        {
            var rawFile = this.ConvertWiiDatasToRawFile(wiiBoardDatas);
            var fileName = string.Concat(date, "_", yeux.ToUpper());

            LOG.DebugFormat("Génération du fichier de données pour date={0} et yeux={1}: {2}", date, yeux, fileName);

            using (var memoryStream = new MemoryStream())
            {
                using (var sw = new StreamWriter(memoryStream))
                {
                    foreach (var line in CsvBuilder.ToCsv(rawFile, "\t"))
                    {
                        sw.WriteLine(line);
                    }
                    sw.Flush();
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var blob = memoryStream.ToArray();

                    return new AcquisitionFile()
                    {
                        Blob = blob,
                        Name = fileName
                    };
                }
            }
        }

        private List<RawOutputModel> ConvertWiiDatasToRawFile(IList<WiiBoardData> wiiBoardDatas)
        {
            var output = new List<RawOutputModel>();

            foreach (var data in wiiBoardDatas)
            {
                output.Add(new RawOutputModel()
                {
                    TIMESTAMP = data.TIMESTAMP,
                    BottomLeftCalcul_SensorsKG = data.bottomLeftKg,
                    BottomRightCalcul_SensorsKG = data.bottomRightKg,
                    TopLeftCalcul_SensorsKG = data.topLeftKg,
                    TopRightCalcul_SensorsKG = data.topRightKg
                });
            }

            LOG.InfoFormat("nb lignes : {0}", output.Count());
            return output;
        }
    }
}
