using System.Text;
using System.IO.Ports;
using static System.Windows.Forms.AxHost;

namespace Virtual_DM400
{
    public partial class DM400 : Form
    {
        int rowCount = -1;
        List<SerialPortEmulator> serialPortEmulators = new();

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

            dataGridView_PortConfiguration.Rows.Add("FirmwareController", "COM6", 115200, (int)System.IO.Ports.Parity.None, 8, (int)System.IO.Ports.StopBits.One);
            dataGridView_PortConfiguration.Rows.Add("WaterLevelChecker", "COM4", 38400, (int)System.IO.Ports.Parity.None, 8, (int)System.IO.Ports.StopBits.One);
        }

        private void DM400_Load(object sender, EventArgs e)
        {
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
            if (isConnected)
            {
                buttonPortOpen.Enabled = false;
                buttonPortClose.Enabled = true;
            }
            else
            {
                buttonPortOpen.Enabled = true;
                buttonPortClose.Enabled = false;
            }
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

            rowCount = -1;

            for (int i = 0; i < dataGridView_PortConfiguration.RowCount; i++)
            {
                if ((string)dataGridView_PortConfiguration.Rows[i].Cells[1].Value != "")
                    rowCount++;
            }

            // ��� ����
            for (int i = 0; i < rowCount; i++)
            {
                string portName = (string)dataGridView_PortConfiguration.Rows[i].Cells[1].Value;
                int baudRate = (int)dataGridView_PortConfiguration.Rows[i].Cells[2].Value;
                Parity parity = (Parity)dataGridView_PortConfiguration.Rows[i].Cells[3].Value;
                int dataBits = (int)dataGridView_PortConfiguration.Rows[i].Cells[4].Value;
                StopBits stopBits = (StopBits)dataGridView_PortConfiguration.Rows[i].Cells[5].Value;

                SerialPortEmulator newEmulator = new(this, portName, baudRate, parity, dataBits, stopBits);

                Thread newThread = new Thread(newEmulator.Start);
                newThread.IsBackground = true;
                newThread.Start();

                serialPortEmulators.Add(newEmulator);
            }
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

        int rowCount_PortConfiguration = -1;

        private SerialPort serialPort;
        private bool isRunning = false;
        private string foundDeviceName;

        public SerialPortEmulator(DM400 mainForm, string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            this.mainForm = mainForm;
            serialPort = new(portName, baudRate, parity, dataBits, stopBits);
        }

        public void SetDataGridView(DataGridView dataGridView_PortConfiguration)
        {
            for (int i = 0; i < dataGridView_PortConfiguration.RowCount; i++)
            {
                if ((string)dataGridView_PortConfiguration.Rows[i].Cells[1].Value != "")
                    rowCount_PortConfiguration++;
            }

            // serialPort.PortName�� ���� DeviceName�� ã��
            for (int i = 0; i < rowCount_PortConfiguration; i++)
            {
                if ((string)dataGridView_PortConfiguration.Rows[i].Cells[1].Value == serialPort.PortName)
                    foundDeviceName = (string)dataGridView_PortConfiguration.Rows[i].Cells[0].Value;
            }
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

        private static StringBuilder buffer = new StringBuilder();

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort.BytesToRead > 0)
            {
                string receivedData = serialPort.ReadExisting();
                buffer.Append(receivedData);

                if (receivedData.EndsWith("\r") || receivedData.EndsWith("\n") || receivedData.EndsWith("\r\n") || receivedData.EndsWith("\0"))
                {
                    string message = buffer.ToString().Trim().ToUpper();

                    // ���� �����͸� ������� ���� ����
                    string responseData = ProcessReceivedData(message);

                    // ���� ����
                    serialPort.Write(responseData + "\r\n");

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

        private string ProcessReceivedData(string receivedData)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// ���� ����
            // �ǹ�: sensor 01���� RMD ����� ������ ���� ���� ���� �˷���
            if (receivedData.Trim() == "%01#RMD**")
            {
                // ����: "%01$RMD-0123456**" : -12.3456mm
                double lower = -10.0;
                double upper = -1.0;

                Random rand = new Random();
                double randomValue = rand.NextDouble() * (upper - lower) + lower;

                // ��: -9.8765 -> -98765
                int intValue = (int)(randomValue * 10000);

                // 4. 8�ڸ� ���ڿ��� ������
                // ����: ��ȣ(1�ڸ�) + 0���� ä�� ����(7�ڸ�)
                // ��: -98765 -> "-0098765"
                string formattedValue = string.Format("{0:+0000000;-0000000}", intValue);

                // 5. ���� ���� ���ڿ� ����
                string responseData = $"%01$RMD{formattedValue}**";

                // ...
                mainForm.TextBoxCurrentWaterLevel.Text = $"{randomValue:F4}"; // ���� ���� ǥ��

                return responseData;
            }

            //public int GetLightAmount()
            //{
            //    if (this.serialPort.IsOpen == false) this.serialPort.Open();
            //    this.serialPort.Write("%01#RID**\r");
            //    Thread.Sleep(50);

            //    string receivedMessage = this.serialPort.ReadExisting();
            //    int returnLightAmount = Convert.ToInt32(receivedMessage.Substring(7, 6));

            //    return returnLightAmount;
            //}


            //// ���ø� �ֱ⸦ ������ (enum Sampling �� ����)
            //public string SetSamplingRate(int samplingRate)
            //{
            //    string[] messageList = ["%01#WSP+00000**\r", "%01#WSP+00001**\r", "%01#WSP+00002**\r", "%01#WSP+00003**\r"];    // Rate_200us, Rate_500us, Rate_1ms, Rate_2ms

            //    if (this.serialPort.IsOpen == false) this.serialPort.Open();
            //    this.serialPort.Write(messageList[samplingRate]);
            //    Thread.Sleep(50);

            //    string returnString = this.serialPort.ReadExisting();

            //    return returnString;
            //}

            //// �ʱ�ȭ�� ������ (enum Initialization �� ����)
            //public void SetSamplingRateValue(int samplingRate) { this.samplingRateValue = samplingRate; }

            //public string SetInitialization(int command)
            //{
            //    string[] messageList = ["%01#WIN+00000**\r", "%01#WIN+00001**\r"];      // Command_None, Command_Initialize

            //    if (this.serialPort.IsOpen == false) this.serialPort.Open();
            //    this.serialPort.Write(messageList[command]);
            //    Thread.Sleep(50);

            //    string returnString = this.serialPort.ReadExisting();

            //    return returnString;
            //}

            //// ���� ������ ������
            //public string SetZeroSet()
            //{
            //    if (this.serialPort.IsOpen == false) this.serialPort.Open();
            //    this.serialPort.Write("%01#WZS+00001**\r");
            //    Thread.Sleep(50);

            //    string returnString = this.serialPort.ReadExisting();

            //    return returnString;
            //}

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// ������
            // ���� ��ũ �̵�
            if (receivedData.Trim().StartsWith("7E03"))
            {
                // string.Format("7E03{0:X6}{1}", (int)(Position * 6400), Feed)
                string hexValue = receivedData.Substring(4, 6);
                int decimalValue = Convert.ToInt32(hexValue, 16);

                LevelTankPosition = decimalValue;

                // ���� ��ũ ��ư�� ��ġ �� ǥ��
                mainForm.ButtonLevelTank.Text = $"Level Tank\n({(LevelTankPosition / 6400).ToString()})";

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

                mainForm.ButtonLevelTank.Location = new Point(mainForm.ButtonLevelTank.Location.X, (int)Math.Round(newY));

                Thread.Sleep(500);

                return "succ";
            }
            // ���� ��ũ ��ġ ��û
            if (receivedData.Trim().StartsWith("7E05"))
            {
                // LevelTankPosition �״�� ����
                // DM400 SW ������ 6400�� ������ ��
                // 10���� ���·� ���� ��
                return Convert.ToInt32(LevelTankPosition).ToString();
            }
            // ���� ��ũ ����
            if (receivedData.Trim().StartsWith("7E06"))
            {
                return "succ";
            }
            // ���� ��ũ �ӵ� ����
            if (receivedData.Trim().StartsWith("7E07"))
            {
                // 7E07{0:X5}: ���� ������ �ʿ䰡 �����Ƿ� �� ������

                return "succ";
            }
            // ������ �̵�
            if (receivedData.Trim().StartsWith("7E10"))
            {
                // string.Format("7E10{0:X6}{1}", (int)(Position * 2560), Feed)
                string hexValue = receivedData.Substring(4, 6);
                int decimalValue = Convert.ToInt32(hexValue, 16);

                BuildPlatformPosition = decimalValue;

                // ������ ��ư�� ��ġ �� ǥ��
                mainForm.ButtonBuildPlatform.Text = $"Build Platform\n({(BuildPlatformPosition / 2560).ToString("0.00")})";

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
                mainForm.ButtonBuildPlatform.Location = new Point(mainForm.ButtonBuildPlatform.Location.X, newY);
                BuildPlatformPosition = decimalValue;

                Thread.Sleep(500);

                return "succ";
            }
            // ������ ��ġ ��û
            if (receivedData.Trim().StartsWith("7E11"))
            {
                // BuildPlatformPosition �״�� ����
                // DM400 SW ������ 2560�� ������ ��
                // 10���� ���·� ���� ��
                return Convert.ToInt32(BuildPlatformPosition).ToString();
            }
            // ������ ����
            if (receivedData.Trim().StartsWith("7E12"))
            {
                return "0";
            }
            // ������ �ӵ� ����
            if (receivedData.Trim().StartsWith("7E13"))
            {
                // string.Format("7E13{0:X5}", (int)(Speed * 5363.464))
                return "succ";
            }

            // ����Ʈ ���̵� �ӵ� ����
            if (receivedData.Trim().StartsWith("7E24"))
            {
                // string.Format("7E24{0:X1}", (int)(Speed))
                return "2";
            }
            // ����Ʈ ���̵� ������ �̵�
            if (receivedData.Trim().StartsWith("7E25"))
            {
                // string.Format("7E25{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeLimit.Location.X, mainForm.ButtonPrintBlade.Location.Y);

                Thread.Sleep(500);

                return "0";
            }
            // ����Ʈ ���̵� ������ �̵�
            if (receivedData.Trim().StartsWith("7E27"))
            {
                // string.Format("7E27{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeLimit.Location.X, mainForm.ButtonPrintBlade.Location.Y);

                Thread.Sleep(500);

                return "0";
            }
            // ����Ʈ ���̵� �ڷ� �̵�
            if (receivedData.Trim().StartsWith("7E26"))
            {
                // string.Format("7E26{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeZero.Location.X, mainForm.ButtonPrintBlade.Location.Y);

                Thread.Sleep(500);

                return "1";
            }
            // ����Ʈ ���̵� �ڷ� �̵�
            if (receivedData.Trim().StartsWith("7E28"))
            {
                // string.Format("7E28{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeZero.Location.X, mainForm.ButtonPrintBlade.Location.Y);

                Thread.Sleep(500);

                return "1";
            }

            // �÷�Ʈ ���̵� �ӵ� ����
            if (receivedData.Trim().StartsWith("7E34"))
            {
                // string.Format("7E34{0:X1}", (int)(Speed))
                return "2";
            }
            // �÷�Ʈ ���̵� ������ �̵�
            if (receivedData.Trim().StartsWith("7E35"))
            {
                // string.Format("7E35{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeLimit.Location.X, mainForm.ButtonCollectBlade.Location.Y);

                Thread.Sleep(500);

                return "0";
            }
            // �÷�Ʈ ���̵� ������ �̵�
            if (receivedData.Trim().StartsWith("7E37"))
            {
                // string.Format("7E37{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeLimit.Location.X, mainForm.ButtonCollectBlade.Location.Y);

                Thread.Sleep(500);

                return "0";
            }
            // �÷�Ʈ ���̵� �ڷ� �̵�
            if (receivedData.Trim().StartsWith("7E36"))
            {
                // string.Format("7E36{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeZero.Location.X, mainForm.ButtonCollectBlade.Location.Y);

                Thread.Sleep(500);

                return "1";
            }
            // �÷�Ʈ ���̵� �ڷ� �̵�
            if (receivedData.Trim().StartsWith("7E38"))
            {
                // string.Format("7E38{0:X6}", (int)(Step))

                // ����Ʈ ���̵� ��ư�� ��ġ ����
                mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeZero.Location.X, mainForm.ButtonCollectBlade.Location.Y);

                Thread.Sleep(500);

                return "1";
            }

            return "ERROR";
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
