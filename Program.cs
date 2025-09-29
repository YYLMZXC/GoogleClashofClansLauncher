using GoogleClashofClansLauncher.Core;
using GoogleClashofClansLauncher.Input;

namespace GoogleClashofClansLauncher;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Google Clash of Clans Launcher - .NET 8.0");
        Console.WriteLine("----------------------------------------");

        // ���ý���·���ʹ��ڱ���
        const string processPath = @"C:\Program Files\Google\Play Games\current\emulator\crosvm.exe"; 
        const string windowTitle = "�����ͻ(Clash of Clans)"; // ���ڱ���

        // ��ʼ��������
        var processManager = new ProcessManager();
        var windowManager = new WindowManager();
        var mouse = new MouseSimulator();
        var keyboard = new KeyboardSimulator();

        // ��������
        var cocProcess = processManager.StartProcess(processPath);
        if (cocProcess == null)
        {
            Console.WriteLine("������Ϸʧ�ܣ�");
            return;
        }

        try
        {
            // �ȴ����ڼ���
            Console.WriteLine("�ȴ���Ϸ��������...");
            System.Threading.Thread.Sleep(5000);

            // ���Ҳ������
            var windowHandle = windowManager.FindWindowByTitle(windowTitle);
            if (windowHandle == IntPtr.Zero)
            {
                Console.WriteLine("δ�ҵ���Ϸ���ڣ�");
                return;
            }

            windowManager.ActivateWindow(windowHandle);
            System.Threading.Thread.Sleep(1000);

            // ģ������ʾ��
            Console.WriteLine("��ʼģ�����...");

            // �ƶ���굽(500, 300)�����
            mouse.Move(500, 300);
            System.Threading.Thread.Sleep(1000);
            mouse.LeftClick();

            // �����ı������س�
            System.Threading.Thread.Sleep(1000);
            keyboard.TypeText("Hello Clash!");
            System.Threading.Thread.Sleep(500);
            keyboard.PressKey(VirtualKeyCode.RETURN);

            Console.WriteLine("������ɣ�");
        }
        finally
        {
            // �ȴ��û�ȷ�Ϻ�رս���
            Console.WriteLine("��������ر���Ϸ...");
            Console.ReadKey();
            processManager.CloseProcess(cocProcess);
        }
    }
}