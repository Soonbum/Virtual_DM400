using System.Text;
using System.IO.Ports;
using static System.Windows.Forms.AxHost;
using System.Windows.Forms;

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

        private void DM400_Load(object sender, EventArgs e)
        {
        }

        private void DM400_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 기존 시리얼 포트 종료 로직
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

            // 커서를 텍스트 맨 끝으로 이동
            logText.SelectionStart = logText.TextLength;

            // 스크롤을 맨 아래로
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

            // 통신 연결
            for (int i = 0; i < rowCount; i++)
            {
                string portName = (string)dataGridView_PortConfiguration.Rows[i].Cells[1].Value;
                int baudRate = (int)dataGridView_PortConfiguration.Rows[i].Cells[2].Value;
                Parity parity = (Parity)dataGridView_PortConfiguration.Rows[i].Cells[3].Value;
                int dataBits = (int)dataGridView_PortConfiguration.Rows[i].Cells[4].Value;
                StopBits stopBits = (StopBits)dataGridView_PortConfiguration.Rows[i].Cells[5].Value;

                SerialPortEmulator newEmulator = new(this, portName, baudRate, parity, dataBits, stopBits);

                Thread newThread = new(newEmulator.Start)
                {
                    IsBackground = true
                };
                newThread.Start();

                serialPortEmulators.Add(newEmulator);
            }
        }

        private void buttonPortClose_Click(object sender, EventArgs e)
        {
            RestrictButtons(false);

            // 통신 해제
            for (int i = 0; i < rowCount; i++)
            {
                serialPortEmulators[i].Stop();
            }

            serialPortEmulators.Clear();
        }
    }

    public class SerialPortEmulator(DM400 mainForm, string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
    {
        private DM400 mainForm = mainForm;

        int rowCount_PortConfiguration = -1;

        private SerialPort serialPort = new(portName, baudRate, parity, dataBits, stopBits);
        private bool isRunning = false;

        public void SetDataGridView(DataGridView dataGridView_PortConfiguration)
        {
            for (int i = 0; i < dataGridView_PortConfiguration.RowCount; i++)
            {
                if ((string)dataGridView_PortConfiguration.Rows[i].Cells[1].Value != "")
                    rowCount_PortConfiguration++;
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

        private static StringBuilder buffer = new();

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort.BytesToRead > 0)
            {
                string receivedData = serialPort.ReadExisting();
                buffer.Append(receivedData);
                string receivedDataComplete = buffer.ToString();

                if (receivedDataComplete.EndsWith('\r') || receivedDataComplete.EndsWith('\n'))
                {
                    string message = buffer.ToString().Trim().ToUpper();

                    // 받은 데이터를 기반으로 응답 생성
                    string responseData = ProcessReceivedData(message);

                    // 응답 전송
                    serialPort.Write(responseData + "\r\n");

                    mainForm.Invoke(new Action(() =>
                    {
                        mainForm.LogWriteLine($"--> {message}\n<-- {responseData}");
                    }));

                    // 버퍼 초기화
                    buffer.Clear();
                }
            }
        }

        public int LevelTankPosition = 0;      // DM400 SW로부터 온 값의 6400을 곱한 값을 저장함
        public int BuildPlatformPosition = 0;  // DM400 SW로부터 온 값의 2560을 곱한 값을 저장함

        private string ProcessReceivedData(string receivedData)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 온도 컨트롤러
            // ...
            
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 레벨 센서
            // 의미: sensor 01에게 RMD 명령을 보내면 현재 측정 값을 알려줌
            if (receivedData.Trim().StartsWith("%01#RMD**"))
            {
                // 예제: "%01$RMD-0123456**" : -12.3456mm
                double lower = -10.0;
                double upper = -11.0;

                Random rand = new();
                double randomValue = rand.NextDouble() * (upper - lower) + lower;

                // 예: -9.8765 -> -98765
                int intValue = (int)(randomValue * 10000);

                // 4. 8자리 문자열로 포매팅
                // 형식: 부호(1자리) + 0으로 채운 숫자(7자리)
                // 예: -98765 -> "-0098765"
                string formattedValue = string.Format("{0:+0000000;-0000000}", intValue);

                // 5. 최종 응답 문자열 조합
                string responseData = $"%01$RMD{formattedValue}**";

                mainForm.Invoke(new Action(() =>
                {
                    mainForm.TextBoxCurrentWaterLevel.Text = $"{randomValue:F4}"; // 현재 수위 표시
                }));

                Thread.Sleep(500);

                return responseData;
            }

            // 의미: sensor 01에게 RID 명령을 보내면 현재 광량을 알려줌
            if (receivedData.Trim().StartsWith("%01#RID**\r"))
            {
                // ... int returnLightAmount = Convert.ToInt32(receivedMessage.Substring(7, 6)); <-- 반송 값
            }

            // 의미: sensor 01에게 WSP 명령을 보내면 샘플링 주기를 설정함 (200us)
            if (receivedData.Trim().StartsWith("%01#WSP+00000**\r"))
            {
                // ... Rate_200us --> 무엇을 반송해야 하나? (0)
            }
            // 의미: sensor 01에게 WSP 명령을 보내면 샘플링 주기를 설정함 (500us)
            if (receivedData.Trim().StartsWith("%01#WSP+00001**\r"))
            {
                // ... Rate_500us --> 무엇을 반송해야 하나? (1)
            }
            // 의미: sensor 01에게 WSP 명령을 보내면 샘플링 주기를 설정함 (1ms)
            if (receivedData.Trim().StartsWith("%01#WSP+00002**\r"))
            {
                // ... Rate_1ms --> 무엇을 반송해야 하나? (2)
            }
            // 의미: sensor 01에게 WSP 명령을 보내면 샘플링 주기를 설정함 (2ms)
            if (receivedData.Trim().StartsWith("%01#WSP+00003**\r"))
            {
                // ... Rate_2ms --> 무엇을 반송해야 하나? (3)
            }

            // 의미: sensor 01에게 WIN 명령을 보내면 초기화함
            if (receivedData.Trim().StartsWith("%01#WIN+00001**\r"))
            {
                // ... 무엇을 반송해야 하나?
            }

            // 의미: sensor 01에게 WZS 명령을 보내면 영점 기준을 설정함\
            if (receivedData.Trim().StartsWith("%01#WZS+00001**\r"))
            {
                // ... 무엇을 반송해야 하나?
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 원보드
            // 레벨 탱크 이동
            if (receivedData.Trim().StartsWith("7E03"))
            {
                // string.Format("7E03{0:X6}{1}", (int)(Position * 6400), Feed)
                string hexValue = receivedData.Substring(4, 6);
                int decimalValue = Convert.ToInt32(hexValue, 16);

                LevelTankPosition = decimalValue;

                // 레벨 탱크 버튼에 위치 값 표시
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonLevelTank.Text = $"Level Tank\n({LevelTankPosition / 6400})";
                }));

                // 레벨 탱크 버튼의 위치 변경
                double.TryParse(mainForm.TextBoxLevelTankLimit.Text, out double maxValue);  // 레벨 탱크 위치 최대치
                double.TryParse(mainForm.TextBoxLevelTankZero.Text, out double minValue);   // 레벨 탱크 위치 최소치

                int startY = mainForm.TextBoxLevelTankLimit.Location.Y;                     // 버튼 위치 가장 낮은 곳
                int endY = mainForm.TextBoxLevelTankZero.Location.Y;                        // 버튼 위치 가장 높은 곳

                double currentValue = LevelTankPosition / 6400.0;                           // 위치 값
                currentValue = Math.Clamp(currentValue, minValue, maxValue);

                double valueRange = maxValue - minValue;                                    // 비율 계산
                double valueRatio = (valueRange > 0) ? ((currentValue - minValue) / valueRange) : 0;
                double positionRatio = 1.0 - valueRatio;                                    // 값이 클수록 위로 가도록 비율 반전시킴
                double newY = startY + ((endY - startY) * positionRatio);                   // 버튼의 Y 좌표 계산

                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonLevelTank.Location = new Point(mainForm.ButtonLevelTank.Location.X, (int)Math.Round(newY));
                }));

                Thread.Sleep(500);

                return "succ";
            }
            // 레벨 탱크 위치 요청
            if (receivedData.Trim().StartsWith("7E05"))
            {
                // LevelTankPosition 그대로 전송
                // DM400 SW 측에서 6400을 나누게 됨
                // 10진수 형태로 보낼 것
                return Convert.ToInt32(LevelTankPosition).ToString();
            }
            // 레벨 탱크 리셋
            if (receivedData.Trim().StartsWith("7E06"))
            {
                return "succ";
            }
            // 레벨 탱크 속도 설정
            if (receivedData.Trim().StartsWith("7E07"))
            {
                // 7E07{0:X5}: 따로 설정할 필요가 없으므로 값 무시함

                return "succ";
            }
            // 조형판 이동
            if (receivedData.Trim().StartsWith("7E10"))
            {
                // string.Format("7E10{0:X6}{1}", (int)(Position * 2560), Feed)
                string hexValue = receivedData.Substring(4, 6);
                int decimalValue = Convert.ToInt32(hexValue, 16);

                BuildPlatformPosition = decimalValue;

                // 조형판 버튼에 위치 값 표시
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonBuildPlatform.Text = $"Build Platform\n({BuildPlatformPosition / 2560:0.00})";
                }));

                // 조형판 버튼의 위치 변경
                // 1. 기준 값과 좌표들을 모두 가져옵니다.
                double.TryParse(mainForm.TextBoxBuildPlatformPositionTop.Text, out double topVal);
                double.TryParse(mainForm.TextBoxBuildPlatformPositionOrigin.Text, out double originVal);
                double.TryParse(mainForm.TextBoxBuildPlatformPositionLimitA.Text, out double limitAVal);
                double.TryParse(mainForm.TextBoxBuildPlatformPositionLimitB.Text, out double limitBVal);

                int topY = mainForm.TextBoxBuildPlatformPositionTop.Location.Y;
                int originY = mainForm.TextBoxBuildPlatformPositionOrigin.Location.Y;
                int limitAY = mainForm.TextBoxBuildPlatformPositionLimitA.Location.Y;
                int limitBY = mainForm.TextBoxBuildPlatformPositionLimitB.Location.Y;

                // 2. 현재 값을 계산합니다.
                double currentValue = BuildPlatformPosition / 2560.0;
                int newY;

                // 3. 현재 값이 어느 구간(Segment)에 속하는지 판단하여 해당 구간 내에서만 보간을 수행합니다.
                if (currentValue <= topVal)
                {
                    // 최상단 값보다 작거나 같으면, 최상단 위치에 고정합니다.
                    newY = topY;
                }
                else if (currentValue <= originVal)
                {
                    // 구간 1: Top ~ Origin 사이
                    newY = InterpolateForBuildPlatform(currentValue, topVal, originVal, topY, originY);
                }
                else if (currentValue <= limitAVal)
                {
                    // 구간 2: Origin ~ LimitA 사이
                    newY = InterpolateForBuildPlatform(currentValue, originVal, limitAVal, originY, limitAY);
                }
                else if (currentValue <= limitBVal)
                {
                    // 구간 3: LimitA ~ LimitB 사이
                    newY = InterpolateForBuildPlatform(currentValue, limitAVal, limitBVal, limitAY, limitBY);
                }
                else
                {
                    // 최하단 값보다 크면, 최하단 위치에 고정합니다.
                    newY = limitBY;
                }

                // 4. 버튼의 위치를 최종적으로 업데이트합니다.
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonBuildPlatform.Location = new Point(mainForm.ButtonBuildPlatform.Location.X, newY);
                }));
                BuildPlatformPosition = decimalValue;

                Thread.Sleep(500);

                return "succ";
            }
            // 조형판 위치 요청
            if (receivedData.Trim().StartsWith("7E11"))
            {
                // BuildPlatformPosition 그대로 전송
                // DM400 SW 측에서 2560을 나누게 됨
                // 10진수 형태로 보낼 것
                return Convert.ToInt32(BuildPlatformPosition).ToString();
            }
            // 조형판 리셋
            if (receivedData.Trim().StartsWith("7E12"))
            {
                return "0";
            }
            // 조형판 속도 설정
            if (receivedData.Trim().StartsWith("7E13"))
            {
                // string.Format("7E13{0:X5}", (int)(Speed * 5363.464))
                return "succ";
            }

            // 프린트 블레이드 속도 설정
            if (receivedData.Trim().StartsWith("7E24"))
            {
                // string.Format("7E24{0:X1}", (int)(Speed))
                return "2";
            }
            // 프린트 블레이드 앞으로 이동
            if (receivedData.Trim().StartsWith("7E25"))
            {
                // string.Format("7E25{0:X6}", (int)(Step))

                // 프린트 블레이드 버튼의 위치 변경
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeLimit.Location.X, mainForm.ButtonPrintBlade.Location.Y);
                }));

                Thread.Sleep(500);

                return "0";
            }
            // 프린트 블레이드 앞으로 이동
            if (receivedData.Trim().StartsWith("7E27"))
            {
                // string.Format("7E27{0:X6}", (int)(Step))

                // 프린트 블레이드 버튼의 위치 변경
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeLimit.Location.X, mainForm.ButtonPrintBlade.Location.Y);
                }));

                Thread.Sleep(500);

                return "0";
            }
            // 프린트 블레이드 뒤로 이동
            if (receivedData.Trim().StartsWith("7E26"))
            {
                // string.Format("7E26{0:X6}", (int)(Step))

                // 프린트 블레이드 버튼의 위치 변경
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeZero.Location.X, mainForm.ButtonPrintBlade.Location.Y);
                }));

                Thread.Sleep(500);

                return "1";
            }
            // 프린트 블레이드 뒤로 이동
            if (receivedData.Trim().StartsWith("7E28"))
            {
                // string.Format("7E28{0:X6}", (int)(Step))

                // 프린트 블레이드 버튼의 위치 변경
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonPrintBlade.Location = new Point(mainForm.TextBoxPrintBladeZero.Location.X, mainForm.ButtonPrintBlade.Location.Y);
                }));

                Thread.Sleep(500);

                return "1";
            }

            // 컬렉트 블레이드 속도 설정
            if (receivedData.Trim().StartsWith("7E34"))
            {
                // string.Format("7E34{0:X1}", (int)(Speed))
                return "2";
            }
            // 컬렉트 블레이드 앞으로 이동
            if (receivedData.Trim().StartsWith("7E35"))
            {
                // string.Format("7E35{0:X6}", (int)(Step))

                // 프린트 블레이드 버튼의 위치 변경
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeLimit.Location.X, mainForm.ButtonCollectBlade.Location.Y);
                }));

                Thread.Sleep(500);

                return "0";
            }
            // 컬렉트 블레이드 앞으로 이동
            if (receivedData.Trim().StartsWith("7E37"))
            {
                // string.Format("7E37{0:X6}", (int)(Step))

                // 프린트 블레이드 버튼의 위치 변경
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeLimit.Location.X, mainForm.ButtonCollectBlade.Location.Y);
                }));

                Thread.Sleep(500);

                return "0";
            }
            // 컬렉트 블레이드 뒤로 이동
            if (receivedData.Trim().StartsWith("7E36"))
            {
                // string.Format("7E36{0:X6}", (int)(Step))

                // 프린트 블레이드 버튼의 위치 변경
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeZero.Location.X, mainForm.ButtonCollectBlade.Location.Y);
                }));

                Thread.Sleep(500);

                return "1";
            }
            // 컬렉트 블레이드 뒤로 이동
            if (receivedData.Trim().StartsWith("7E38"))
            {
                // string.Format("7E38{0:X6}", (int)(Step))

                // 프린트 블레이드 버튼의 위치 변경
                mainForm.Invoke(new Action(() =>
                {
                    mainForm.ButtonCollectBlade.Location = new Point(mainForm.TextBoxCollectBladeZero.Location.X, mainForm.ButtonCollectBlade.Location.Y);
                }));

                Thread.Sleep(500);

                return "1";
            }

            return "ERROR";
        }

        private int InterpolateForBuildPlatform(double currentValue, double startVal, double endVal, int startY, int endY)
        {
            double valueRange = endVal - startVal;
            // 0으로 나누는 것을 방지
            if (valueRange <= 0) return startY;

            // 현재 값이 구간 내에서 차지하는 비율을 계산 (0.0 ~ 1.0)
            double ratio = (currentValue - startVal) / valueRange;

            // Y 좌표 범위 내에서 비율에 해당하는 실제 Y 좌표를 계산
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
