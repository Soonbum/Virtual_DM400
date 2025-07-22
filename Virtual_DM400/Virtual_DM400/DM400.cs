using System.Text;
using System.IO.Ports;
using static System.Windows.Forms.AxHost;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace Virtual_DM400
{
    public partial class DM400 : Form
    {
        int rowCount = -1;
        List<SerialPortEmulator> serialPortEmulators = [];

        public DM400()
        {
            InitializeComponent();

            RestrictButtons(false);

            dataGridView_PortConfiguration.Columns["DeviceName"].ValueType = typeof(string);
            dataGridView_PortConfiguration.Columns["PortName"].ValueType = typeof(string);
            dataGridView_PortConfiguration.Columns["BaudRate"].ValueType = typeof(int);
            dataGridView_PortConfiguration.Columns["Parity"].ValueType = typeof(int);
            dataGridView_PortConfiguration.Columns["DataBits"].ValueType = typeof(int);
            dataGridView_PortConfiguration.Columns["StopBits"].ValueType = typeof(int);

            dataGridView_PortConfiguration.Rows.Add("FirmwareController", "COM5", 115200, (int)System.IO.Ports.Parity.None, 8, (int)System.IO.Ports.StopBits.One);
            dataGridView_PortConfiguration.Rows.Add("WaterLevelChecker", "COM3", 38400, (int)System.IO.Ports.Parity.None, 8, (int)System.IO.Ports.StopBits.One);
        }

        private void DM400_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ���� �ø��� ��Ʈ ���� ����
            if (buttonPortClose.Enabled)
            {
                buttonPortClose_Click(sender, e);
            }
        }

        public void RestrictButtons(bool isConnected)
        {
            buttonPortOpen.Enabled = !isConnected;
            buttonPortClose.Enabled = isConnected;

            TextBoxLevelTankZero.Enabled = !isConnected;
            TextBoxLevelTankLimit.Enabled = !isConnected;
            TextBoxCollectBladeZero.Enabled = !isConnected;
            TextBoxCollectBladeLimit.Enabled = !isConnected;
            TextBoxPrintBladeZero.Enabled = !isConnected;
            TextBoxPrintBladeLimit.Enabled = !isConnected;
            TextBoxBuildPlatformPositionTop.Enabled = !isConnected;
            TextBoxBuildPlatformPositionOrigin.Enabled = !isConnected;
            TextBoxBuildPlatformPositionLimitA.Enabled = !isConnected;
            TextBoxBuildPlatformPositionLimitB.Enabled = !isConnected;

            TextBoxCurrentWaterLevelMin.Enabled = !isConnected;
            TextBoxCurrentWaterLevelMax.Enabled = !isConnected;
        }

        public void LogWriteLine(string message)
        {
            logText.Text += message + "\n";

            // Ŀ���� �ؽ�Ʈ �� ������ �̵�
            logText.SelectionStart = logText.TextLength;

            // ��ũ���� �� �Ʒ���
            logText.ScrollToCaret();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            logText.Text = string.Empty;
        }

        private void buttonPortOpen_Click(object sender, EventArgs e)
        {
            RestrictButtons(true);

            foreach (DataGridViewRow row in dataGridView_PortConfiguration.Rows)
            {
                if (row.IsNewRow || row.Cells["PortName"].Value == null || string.IsNullOrEmpty(row.Cells["PortName"].Value.ToString()))
                {
                    continue;
                }

                string deviceName = (string)row.Cells["DeviceName"].Value;
                string portName = (string)row.Cells["PortName"].Value;
                int baudRate = (int)row.Cells["BaudRate"].Value;
                Parity parity = (Parity)row.Cells["Parity"].Value;
                int dataBits = (int)row.Cells["DataBits"].Value;
                StopBits stopBits = (StopBits)row.Cells["StopBits"].Value;

                SerialPortEmulator newEmulator = new(this, deviceName, portName, baudRate, parity, dataBits, stopBits);

                Thread newThread = new(newEmulator.Start)
                {
                    IsBackground = true
                };
                newThread.Start();

                serialPortEmulators.Add(newEmulator);
            }

            // ��� ���� �� ��Ȯ�� ������ŭ �ݱ� ���� rowCount�� �ٽ� �����մϴ�.
            rowCount = serialPortEmulators.Count;
        }

        private void buttonPortClose_Click(object sender, EventArgs e)
        {
            RestrictButtons(false);

            // ��� ����
            for (int i = 0; i < rowCount; i++)
            {
                serialPortEmulators[i].Stop();
            }

            serialPortEmulators.Clear();
        }
    }

    public class SerialPortEmulator
    {
        private DM400 mainForm;

        private readonly SerialPort serialPort;
        private readonly string deviceName;
        private bool isRunning = false;

        public SerialPortEmulator(DM400 mainForm, string deviceName, string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            this.mainForm = mainForm;
            this.serialPort = new(portName, baudRate, parity, dataBits, stopBits);
            this.deviceName = deviceName;
        }

        public void Start()
        {
            serialPort.DataReceived += SerialPort_DataReceived;

            serialPort.Open();
            isRunning = true;

            while (isRunning)
            {
                Thread.Sleep(1000);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort.BytesToRead > 0)
            {
                StringBuilder buffer = new();
                string receivedData = serialPort.ReadExisting();
                buffer.Append(receivedData);

                if (receivedData.EndsWith("\r") || receivedData.EndsWith("\n") || receivedData.EndsWith("\r\n") || receivedData.EndsWith("\0"))
                {
                    // ����, Ư������ �����ϰ� �빮��ȭ
                    string message = buffer.ToString().Trim().ToUpper();

                    // ���� �����͸� ������� ���� ����
                    string responseData = ProcessReceivedData(message);

                    // ���� ����
                    serialPort.Write(responseData + "\r\n");

                    // ��� ������
                    //Thread.Sleep(100);

                    // ��� �ۼ��� �� ǥ��
                    mainForm.Invoke(new Action(() =>
                    {
                        mainForm.LogWriteLine($"--> {message}\n<-- {responseData}");
                    }));

                    // ���� �ʱ�ȭ
                    buffer.Clear();
                }
            }
        }

        public int LevelTankPosition = 0;      // DM400 SW�κ��� �� ���� 6400�� ���� ���� ������
        public int BuildPlatformPosition = 0;  // DM400 SW�κ��� �� ���� 2560�� ���� ���� ������

        private string ProcessReceivedData(string message)
        {
            // !!! �� �κк��� Debug ���� ���� �� ���� �ʿ���
            if (this.deviceName == "FirmwareController")
                return ProcessFirmwareControllerData(message);
            else if (this.deviceName == "WaterLevelChecker")
                return ProcessWaterLevelCheckerData(message);
            else
                return "UNKNOWN_DEVICE_ERROR";
        }

        private string ProcessFirmwareControllerData(string message)
        {
            // ���� ��ũ �̵�
            if (message.Trim().StartsWith("7E03"))
            {
                // string.Format("7E03{0:X6}{1}", (int)(Position * 6400), Feed)
                string hexValue = message.Substring(4, 6);
                int decimalValue = Convert.ToInt32(hexValue, 16);

                LevelTankPosition = decimalValue;

                // ���� ��ũ ��ư�� ��ġ �� ǥ��
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonLevelTank.Text = $"Level Tank\n({LevelTankPosition / 6400})";
                }));

                // ���� ��ũ ��ư�� ��ġ ����
                double.TryParse(mainForm.TextBoxLevelTankLimit.Text, out double maxValue);  // ���� ��ũ ��ġ �ִ�ġ
                double.TryParse(mainForm.TextBoxLevelTankZero.Text, out double minValue);   // ���� ��ũ ��ġ �ּ�ġ

                int startY = mainForm.TextBoxLevelTankLimit.Location.Y;                     // ��ư ��ġ ���� ���� ��
                int endY = mainForm.TextBoxLevelTankZero.Location.Y;                        // ��ư ��ġ ���� ���� ��

                double currentValue = LevelTankPosition / 6400.0;                           // ��ġ ��
                currentValue = Math.Clamp(currentValue, minValue, maxValue);

                double valueRange = maxValue - minValue;                                    // ���� ���
                double valueRatio = (valueRange > 0) ? ((currentValue - minValue) / valueRange) : 0;
                double positionRatio = 1.0 - valueRatio;                                    // ���� Ŭ���� ���� ������ ���� ������Ŵ
                double newY = startY + ((endY - startY) * positionRatio);                   // ��ư�� Y ��ǥ ���

                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonLevelTank.Location = new Point(mainForm.ButtonLevelTank.Location.X, (int)Math.Round(newY));
                }));

                return "succ";
            }
            // ���� ��ũ ��ġ ��û
            else if (message.Trim().StartsWith("7E05"))
            {
                // LevelTankPosition �״�� ����
                // DM400 SW ������ 6400�� ������ ��
                // 10���� ���·� ���� ��
                return Convert.ToInt32(LevelTankPosition).ToString();
            }
            // ���� ��ũ ����
            else if (message.Trim().StartsWith("7E06"))
            {
                return "succ";
            }
            // ���� ��ũ �ӵ� ����
            else if (message.Trim().StartsWith("7E07"))
            {
                // 7E07{0:X5}: ���� ������ �ʿ䰡 �����Ƿ� �� ������

                return "succ";
            }
            // ������ �̵�
            else if (message.Trim().StartsWith("7E10"))
            {
                // string.Format("7E10{0:X6}{1}", (int)(Position * 2560), Feed)
                string hexValue = message.Substring(4, 6);
                int decimalValue = Convert.ToInt32(hexValue, 16);

                BuildPlatformPosition = decimalValue;

                // ������ ��ư�� ��ġ �� ǥ��
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonBuildPlatform.Text = $"Build Platform\n({BuildPlatformPosition / 2560:0.00})";
                }));

                // ������ ��ư�� ��ġ ����
                // 1. ���� ���� ��ǥ���� ��� �����ɴϴ�.
                double.TryParse(mainForm.TextBoxBuildPlatformPositionTop.Text, out double topVal);
                double.TryParse(mainForm.TextBoxBuildPlatformPositionOrigin.Text, out double originVal);
                double.TryParse(mainForm.TextBoxBuildPlatformPositionLimitA.Text, out double limitAVal);
                double.TryParse(mainForm.TextBoxBuildPlatformPositionLimitB.Text, out double limitBVal);

                int topY = mainForm.TextBoxBuildPlatformPositionTop.Location.Y;
                int originY = mainForm.TextBoxBuildPlatformPositionOrigin.Location.Y;
                int limitAY = mainForm.TextBoxBuildPlatformPositionLimitA.Location.Y;
                int limitBY = mainForm.TextBoxBuildPlatformPositionLimitB.Location.Y;

                // 2. ���� ���� ����մϴ�.
                double currentValue = BuildPlatformPosition / 2560.0;
                int newY;

                // 3. ���� ���� ��� ����(Segment)�� ���ϴ��� �Ǵ��Ͽ� �ش� ���� �������� ������ �����մϴ�.
                if (currentValue <= topVal)
                {
                    // �ֻ�� ������ �۰ų� ������, �ֻ�� ��ġ�� �����մϴ�.
                    newY = topY;
                }
                else if (currentValue <= originVal)
                {
                    // ���� 1: Top ~ Origin ����
                    newY = InterpolateForBuildPlatform(currentValue, topVal, originVal, topY, originY);
                }
                else if (currentValue <= limitAVal)
                {
                    // ���� 2: Origin ~ LimitA ����
                    newY = InterpolateForBuildPlatform(currentValue, originVal, limitAVal, originY, limitAY);
                }
                else if (currentValue <= limitBVal)
                {
                    // ���� 3: LimitA ~ LimitB ����
                    newY = InterpolateForBuildPlatform(currentValue, limitAVal, limitBVal, limitAY, limitBY);
                }
                else
                {
                    // ���ϴ� ������ ũ��, ���ϴ� ��ġ�� �����մϴ�.
                    newY = limitBY;
                }

                // 4. ��ư�� ��ġ�� ���������� ������Ʈ�մϴ�.
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonBuildPlatform.Location = new Point(mainForm.ButtonBuildPlatform.Location.X, newY);
                }));
                BuildPlatformPosition = decimalValue;

                return "succ";
            }
            // ������ ��ġ ��û
            else if (message.Trim().StartsWith("7E11"))
            {
                // BuildPlatformPosition �״�� ����
                // DM400 SW ������ 2560�� ������ ��
                // 10���� ���·� ���� ��
                return Convert.ToInt32(BuildPlatformPosition).ToString();
            }
            // ������ ����
            else if (message.Trim().StartsWith("7E12"))
            {
                return "0";
            }
            // ������ �ӵ� ����
            else if (message.Trim().StartsWith("7E13"))
            {
                // string.Format("7E13{0:X5}", (int)(Speed * 5363.464))
                return "succ";
            }

            // ����Ʈ ���̵� �ӵ� ����
            else if (message.Trim().StartsWith("7E24"))
            {
                // string.Format("7E24{0:X1}", (int)(Speed))
                return "2";
            }
            // ����Ʈ ���̵� ������ �̵�
            else if (message.Trim().StartsWith("7E25"))
            {
                // string.Format("7E25{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeLimit.Location.X, mainForm.ButtonPrintBlade.Location.Y);
                }));

                return "0";
            }
            // ����Ʈ ���̵� ������ �̵�
            else if (message.Trim().StartsWith("7E27"))
            {
                // string.Format("7E27{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeLimit.Location.X, mainForm.ButtonPrintBlade.Location.Y);
                }));

                return "0";
            }
            // ����Ʈ ���̵� �ڷ� �̵�
            else if (message.Trim().StartsWith("7E26"))
            {
                // string.Format("7E26{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeZero.Location.X, mainForm.ButtonPrintBlade.Location.Y);
                }));

                return "1";
            }
            // ����Ʈ ���̵� �ڷ� �̵�
            else if (message.Trim().StartsWith("7E28"))
            {
                // string.Format("7E28{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeZero.Location.X, mainForm.ButtonPrintBlade.Location.Y);
                }));

                return "1";
            }

            // �÷�Ʈ ���̵� �ӵ� ����
            else if (message.Trim().StartsWith("7E34"))
            {
                // string.Format("7E34{0:X1}", (int)(Speed))
                return "2";
            }
            // �÷�Ʈ ���̵� ������ �̵�
            else if (message.Trim().StartsWith("7E35"))
            {
                // string.Format("7E35{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeLimit.Location.X, mainForm.ButtonCollectBlade.Location.Y);
                }));

                return "0";
            }
            // �÷�Ʈ ���̵� ������ �̵�
            else if (message.Trim().StartsWith("7E37"))
            {
                // string.Format("7E37{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeLimit.Location.X, mainForm.ButtonCollectBlade.Location.Y);
                }));

                return "0";
            }
            // �÷�Ʈ ���̵� �ڷ� �̵�
            else if (message.Trim().StartsWith("7E36"))
            {
                // string.Format("7E36{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeZero.Location.X, mainForm.ButtonCollectBlade.Location.Y);
                }));

                return "1";
            }
            // �÷�Ʈ ���̵� �ڷ� �̵�
            else if (message.Trim().StartsWith("7E38"))
            {
                // string.Format("7E38{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeZero.Location.X, mainForm.ButtonCollectBlade.Location.Y);
                }));

                return "1";
            }

            return "FIRMWARE_CONTROLLER_ERROR";
        }

        private string ProcessWaterLevelCheckerData(string message)
        {
            // �ǹ�: sensor 01���� RMD ����� ������ ���� ���� ���� �˷���
            if (message.Trim().StartsWith("%01#RMD**"))
            {
                // ����: "%01$RMD-0123456**" : -12.3456mm
                double lower = Convert.ToDouble(mainForm.TextBoxCurrentWaterLevelMin.Text);
                double upper = Convert.ToDouble(mainForm.TextBoxCurrentWaterLevelMax.Text);

                Random rand = new();
                double randomValue = rand.NextDouble() * (upper - lower) + lower;

                // ��: -9.8765 -> -98765
                int intValue = (int)(randomValue * 10000);

                // 4. 8�ڸ� ���ڿ��� ������
                // ����: ��ȣ(1�ڸ�) + 0���� ä�� ����(7�ڸ�)
                // ��: -98765 -> "-0098765"
                string formattedValue = string.Format("{0:+0000000;-0000000}", intValue);

                // 5. ���� ���� ���ڿ� ����
                string responseData = $"%01$RMD{formattedValue}**";

                mainForm.Invoke(new Action(() =>
                {
                    mainForm.TextBoxCurrentWaterLevel.Text = $"{randomValue:F4}"; // ���� ���� ǥ��
                }));

                return responseData;
            }

            //// �ǹ�: sensor 01���� RID ����� ������ ���� ������ �˷���
            //else if (receivedData.Trim().StartsWith("%01#RID**\r"))
            //{
            //    // ... int returnLightAmount = Convert.ToInt32(receivedMessage.Substring(7, 6)); <-- �ݼ� ��
            //}

            //// �ǹ�: sensor 01���� WSP ����� ������ ���ø� �ֱ⸦ ������ (200us)
            //else if (receivedData.Trim().StartsWith("%01#WSP+00000**\r"))
            //{
            //    // ... Rate_200us --> ������ �ݼ��ؾ� �ϳ�? (0)
            //}
            //// �ǹ�: sensor 01���� WSP ����� ������ ���ø� �ֱ⸦ ������ (500us)
            //else if (receivedData.Trim().StartsWith("%01#WSP+00001**\r"))
            //{
            //    // ... Rate_500us --> ������ �ݼ��ؾ� �ϳ�? (1)
            //}
            //// �ǹ�: sensor 01���� WSP ����� ������ ���ø� �ֱ⸦ ������ (1ms)
            //else if (receivedData.Trim().StartsWith("%01#WSP+00002**\r"))
            //{
            //    // ... Rate_1ms --> ������ �ݼ��ؾ� �ϳ�? (2)
            //}
            //// �ǹ�: sensor 01���� WSP ����� ������ ���ø� �ֱ⸦ ������ (2ms)
            //else if (receivedData.Trim().StartsWith("%01#WSP+00003**\r"))
            //{
            //    // ... Rate_2ms --> ������ �ݼ��ؾ� �ϳ�? (3)
            //}

            //// �ǹ�: sensor 01���� WIN ����� ������ �ʱ�ȭ��
            //else if (receivedData.Trim().StartsWith("%01#WIN+00001**\r"))
            //{
            //    // ... ������ �ݼ��ؾ� �ϳ�?
            //}

            //// �ǹ�: sensor 01���� WZS ����� ������ ���� ������ ������\
            //else if (receivedData.Trim().StartsWith("%01#WZS+00001**\r"))
            //{
            //    // ... ������ �ݼ��ؾ� �ϳ�?
            //}

            return "WATER_LEVEL_CHECKER_ERROR";
        }

        private int InterpolateForBuildPlatform(double currentValue, double startVal, double endVal, int startY, int endY)
        {
            double valueRange = endVal - startVal;
            // 0���� ������ ���� ����
            if (valueRange <= 0) return startY;

            // ���� ���� ���� ������ �����ϴ� ������ ��� (0.0 ~ 1.0)
            double ratio = (currentValue - startVal) / valueRange;

            // Y ��ǥ ���� ������ ������ �ش��ϴ� ���� Y ��ǥ�� ���
            double newY = startY + ((endY - startY) * ratio);

            return (int)Math.Round(newY);
        }

        public void Stop()
        {
            isRunning = false;

            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Dispose();
            }
        }
    }
}
