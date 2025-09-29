using GoogleClashofClansLauncher.Input;
using System.Threading;

namespace GoogleClashofClansLauncher
{
    public partial class Form1 : Form
    {
        private KeyboardSimulator keyboardSimulator;

        public Form1()
        {
            InitializeComponent();
            keyboardSimulator = new KeyboardSimulator();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void simulateButton_Click(object sender, EventArgs e)
        {
            // 获取用户输入的文本
            string textToSimulate = inputTextBox.Text;
            
            if (!string.IsNullOrEmpty(textToSimulate))
            {
                // 延迟1秒后执行模拟输入，让用户有时间切换到目标窗口
                Thread.Sleep(1000);
                keyboardSimulator.TypeText(textToSimulate);
            }
        }

        private void fixed123Button_Click(object sender, EventArgs e)
        {
            // 延迟1秒后执行固定的"123"输入
            Thread.Sleep(1000);
            keyboardSimulator.TypeText("123");
        }
    }
}
