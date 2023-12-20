using COMProject.Enums;
using COMProject.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace COMProject
{
    public partial class MainForm : Form
    {
        private SendDataSettings sendDataSettings;
        public MainForm()
        {
            InitializeComponent();
            Initialize();
        }
        private void Initialize()
        {
            sendDataSettings = new SendDataSettings();
            sendDataSettings.SendDataType = SendDataTypeEnum.Char;
            sendDataSettings.RTSEnable = false;
            sendDataSettings.DTREnable = false;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            COMPortComboBox.Items.Clear();
            COMPortComboBox.Items.AddRange(ports);

            ParityBitsComboBox.Items.Clear();
            ParityBitsComboBox.DataSource = Enum.GetNames(typeof(Parity));

            StopBitsComboBox.Items.Clear();
            StopBitsComboBox.DataSource = Enum.GetNames(typeof(StopBits));

            BaudRateComboBox.SelectedIndex = 2;
            DataBitsComboBox.SelectedIndex = 2;
            StopBitsComboBox.SelectedIndex = 0;
            ParityBitsComboBox.SelectedIndex = 0;
            DTRCheckBox.Checked = false;
            RTSCheckBox.Checked = false;
            CharRadioButton.Checked = true;
        }
        private void OpenPortButton_Click(object sender, EventArgs e)
        {
            try
            {
                sendDataSettings.SerialPort = new SerialPort
                (
                sendDataSettings.ComPortName,
                sendDataSettings.BaudRate,
                sendDataSettings.ParityBits,
                sendDataSettings.DataBits,
                sendDataSettings.StopBits
                );
                sendDataSettings.SerialPort.DtrEnable = sendDataSettings.DTREnable;
                sendDataSettings.SerialPort.RtsEnable = sendDataSettings.RTSEnable;
                sendDataSettings.SerialPort.DataReceived += SerialPort_DataReceived;

                sendDataSettings.SerialPort.Open();
                PortStatusLabel.Text = $"Status : Serial Port has been opened from {sendDataSettings.ComPortName}";
                PortStatusLabel.ForeColor = Color.Green;
                sendDataSettings.ISPortOpen = true;
            }
            catch (Exception ex)
            {
                PortStatusLabel.Text = $"Status : {ex.Message}";
                PortStatusLabel.ForeColor = Color.Red;
                sendDataSettings.ISPortOpen = false;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string receivedData = sendDataSettings.SerialPort.ReadLine();
            if (ReceiveDataTextBox.InvokeRequired)
                ReceiveDataTextBox.Invoke((Action)(() => ReceivedDataTextHandler(receivedData)));
            else
                ReceivedDataTextHandler(receivedData);

            if (DataOutLengthLabel.InvokeRequired)
                DataOutLengthLabel.Invoke((Action)(() => ReceivedDataOutHandler(receivedData)));
            else
                ReceivedDataOutHandler(receivedData);
        }

        private void ReceivedDataTextHandler(string text)
        {
            ReceiveDataTextBox.Text += text + Environment.NewLine;
        }
        private void ReceivedDataOutHandler(string text)
        {
            DataOutLengthLabel.Text = $"Data Out Length : {text.Length}";
        }

        private void ClosePortButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (sendDataSettings.ISPortOpen)
                {
                    sendDataSettings.SerialPort.Close();
                    PortStatusLabel.Text = $"Status : Serial Port has been closed from {sendDataSettings.ComPortName}";
                    PortStatusLabel.ForeColor = Color.Orange;
                    sendDataSettings.ISPortOpen = false;
                }
            }
            catch (Exception ex)
            {
                PortStatusLabel.Text = $"Status : {ex.Message}";
                PortStatusLabel.ForeColor = Color.Red;
            }

        }
        private void COMPortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            sendDataSettings.ComPortName = COMPortComboBox.Text;
            ClosePortButton_Click(sender, e);
        }
        private void BaudRateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            sendDataSettings.BaudRate = String.IsNullOrEmpty(BaudRateComboBox.SelectedText) ? 9600 : Convert.ToInt32(BaudRateComboBox.Text);
            ClosePortButton_Click(sender, e);
        }
        private void DataBitsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            sendDataSettings.DataBits = String.IsNullOrEmpty(DataBitsComboBox.SelectedText) ? 8 : Convert.ToInt32(DataBitsComboBox.Text);
            ClosePortButton_Click(sender, e);
        }
        private void StopBitsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            sendDataSettings.StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBitsComboBox.Text);
            ClosePortButton_Click(sender, e);
        }
        private void ParityBitsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            sendDataSettings.ParityBits = (Parity)Enum.Parse(typeof(Parity), ParityBitsComboBox.Text);
            ClosePortButton_Click(sender, e);
        }
        private void DTRCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            sendDataSettings.DTREnable = DTRCheckBox.Checked;
            ClosePortButton_Click(sender, e);
        }
        private void RTSCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            sendDataSettings.RTSEnable = RTSCheckBox.Checked;
            ClosePortButton_Click(sender, e);
        }
        private void RadioButtons_CheckedChanged(object sender, EventArgs e)
        {
            var checkedButton = SendDataRadioButtonsContainer.Controls.OfType<RadioButton>()
                                      .FirstOrDefault(r => r.Checked);
            sendDataSettings.SendDataType = (SendDataTypeEnum)Enum.Parse(typeof(SendDataTypeEnum), checkedButton.Text);
            SendDataTextBox.Text = String.Empty;
        }
        private void SendDataTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)8)
                return;

            switch (sendDataSettings.SendDataType)
            {
                case SendDataTypeEnum.Char:
                    break;
                case SendDataTypeEnum.Hex:
                    if (!HelperMethods.IsHex(e.KeyChar))
                        e.Handled = true;
                    break;
                case SendDataTypeEnum.Decimal:
                    if (!HelperMethods.IsDecimal(e.KeyChar))
                        e.Handled = true;
                    break;
                case SendDataTypeEnum.Binary:
                    if (!HelperMethods.IsBinary(e.KeyChar))
                        e.Handled= true;
                    break;
                default:
                    break;
            }
        }
        private void SendDataTextBox_TextChanged(object sender, EventArgs e)
        {
            sendDataSettings.SendDataText = SendDataTextBox.Text;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (sendDataSettings.ISPortOpen && !String.IsNullOrEmpty(sendDataSettings.SendDataText))
            {
                sendDataSettings.SerialPort.WriteLine(sendDataSettings.SendDataText);
                DataInLengthLabel.Text = $"Data In Length : {sendDataSettings.SendDataText.Length}";
            }
        }

        private void SingleButton_Click(object sender, EventArgs e)
        {
            //Tekli atış serisi için kullandığımız sendDataSettings classının SendDataText parametresini mauel girerek
            //send butonunu tetikledik.
            sendDataSettings.SendDataText = "FF0454D5349AF";
            SendButton_Click(sender, e);
        }
    }
}
