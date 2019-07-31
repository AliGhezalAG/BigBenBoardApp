using RestWCFServiceLibrary.WiiMote;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using WiimoteLib;
using Newtonsoft.Json;

namespace WiiBoardApp
{
    public partial class WiiBoardAppForm : Form
    {
        DeviceAccess _WiiAccess = new DeviceAccess();
        Wiimote wiiboard = null;
        WiiBoardGetDatas _WiiBoardDatas = null;
        SynchronizedCollection<WiiBoardData> results = null;


        string targetResults;
        string targetResultsJson;
        string targetResultsCsv;
        string targetResultsTxt;

        public WiiBoardAppForm()
        {
            targetResults = @"C:/BoardAppResults";
            targetResultsJson = targetResults + "/json";
            targetResultsCsv = targetResults + "/csv";
            targetResultsTxt = targetResults + "/txt";

            InitializeComponent();
        }

        private void acquisitionTypeLabel_Click(object sender, EventArgs e)
        {

        }

        private void acquisitionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void jsonOutputButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void csvOutputButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtOutputButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void startAcquisitionButton_Click(object sender, EventArgs e)
        {
            stateLabel.Text = "Acquisition started...";
            _WiiBoardDatas.StartAcquisition();

            if (acquisitionTypeComboBox.SelectedItem.ToString().Equals("Timed acquisition"))
            {
                System.Threading.Thread.Sleep(int.Parse(acquisitionLength.Text)*1000);
                stopAcquisition();
            }
        }

        private void stopAcquisitionButton_Click(object sender, EventArgs e)
        {
            stopAcquisition();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            _WiiAccess.SearchWiiBoard();
            wiiboard = _WiiAccess.WiiBoard;
            _WiiBoardDatas = new WiiBoardGetDatas(_WiiAccess);
            stateLabel.Text = "Board connected";
        }

        private void stopAcquisition()
        {
            _WiiBoardDatas.StopAcquisition(true);
            results = _WiiBoardDatas.WiiBoardDatas;
            processDawData();
            stateLabel.Text = "Acquisition finished";
        }

        private void processDawData()
        {
            initDirectories();

            if (jsonOutputButton.Checked) {
                generateJsonOutput();
            } else if (csvOutputButton.Checked) {
                generateCsvOutput();
            } else {
                generateTxtOutput();
            }
        }

        private void generateJsonOutput()
        {
            using (StreamWriter file = File.CreateText(Path.Combine(targetResultsJson, DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "-result.json")))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, results);
            }
        }

        private void generateCsvOutput()
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(targetResultsCsv, DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "-result.csv")))
            {
                outputFile.WriteLine("TIMESTAMP, bottomLeftKg, bottomRightKg, topLeftKg, topRightKg");

                foreach (WiiBoardData element in results)
                {
                    var line = element.TIMESTAMP.ToString() + ","
                                + element.bottomLeftKg.ToString() + ","
                                + element.bottomRightKg.ToString() + ","
                                + element.topLeftKg.ToString() + ","
                                + element.topRightKg.ToString();
                    outputFile.WriteLine(line);
                }
            }
        }

        private void generateTxtOutput()
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(targetResultsTxt, DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "-result.txt")))
            {
                outputFile.WriteLine("TIMESTAMP \t bottomLeftKg \t bottomRightKg \t topLeftKg \t topRightKg");

                foreach (WiiBoardData element in results)
                {
                    var line = element.TIMESTAMP.ToString() + "\t"
                                + element.bottomLeftKg.ToString() + "\t"
                                + element.bottomRightKg.ToString() + "\t"
                                + element.topLeftKg.ToString() + "\t"
                                + element.topRightKg.ToString();
                    outputFile.WriteLine(line);
                }
            }
        }

        private void initDirectories() {
            try
            {
                if (!Directory.Exists(targetResults)) {
                    Directory.CreateDirectory(targetResults);
                    Directory.CreateDirectory(targetResultsJson);
                    Directory.CreateDirectory(targetResultsCsv);
                    Directory.CreateDirectory(targetResultsTxt);
                } else {
                    if (!Directory.Exists(targetResultsJson)) {
                        Directory.CreateDirectory(targetResultsJson);
                    }

                    if (!Directory.Exists(targetResultsCsv))  {
                        Directory.CreateDirectory(targetResultsCsv);
                    }

                    if (!Directory.Exists(targetResultsTxt)) {
                        Directory.CreateDirectory(targetResultsTxt);
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
    }
}
