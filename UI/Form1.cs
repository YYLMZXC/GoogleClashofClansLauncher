
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoogleClashofClansLauncher.Core.UI;
using GoogleClashofClansLauncher.Input;

namespace GoogleClashofClansLauncher.UI;

public partial class Form1 : Form
{
    private readonly KeyboardSimulator _kb = new();
    private readonly MouseSimulator _mouse = new();
    private readonly WindowManager _wm = new();
    private CancellationTokenSource? _cts;

    private const string ProcessName = "crosvm";
    private const string Keyword = "部落冲突";

    public Form1() => InitializeComponent();

    private IntPtr GetGameWindowHandle()
    {
        foreach (var p in Process.GetProcessesByName(ProcessName))
            if (p.MainWindowTitle.Contains(Keyword, StringComparison.OrdinalIgnoreCase))
                return p.MainWindowHandle;
        IntPtr found = IntPtr.Zero;
        EnumWindows((h, _) =>
        {
            var sb = new StringBuilder(256);
            GetWindowText(h, sb, sb.Capacity);
            if (sb.ToString().Contains(Keyword, StringComparison.OrdinalIgnoreCase))
            {
                found = h;
                return false;
            }
            return true;
        }, IntPtr.Zero);
        return found;
    }

    private void simulateButton_Click(object sender, EventArgs e)
    {
        var h = GetGameWindowHandle();
        if (h == IntPtr.Zero) { Debug.WriteLine("未找到游戏窗口"); return; }
        _wm.RestoreWindowOnly(h);
        Thread.Sleep(200);
        _kb.TypeText(inputTextBox.Text);
    }

    private void fixed123Button_Click(object sender, EventArgs e)
    {
        var h = GetGameWindowHandle();
        if (h == IntPtr.Zero) { Debug.WriteLine("未找到游戏窗口"); return; }
        _wm.RestoreWindowOnly(h);
        Thread.Sleep(200);
        _kb.TypeText("123");
    }

    private async void mouseClickButton_Click(object sender, EventArgs e)
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            mouseClickButton.Text = "点击测试（3 次/秒 × 10 秒）";
            return;
        }
        var h = GetGameWindowHandle();
        if (h == IntPtr.Zero) { Debug.WriteLine("未找到游戏窗口"); return; }
        _wm.RestoreWindowOnly(h);
        _cts = new CancellationTokenSource();
        mouseClickButton.Text = "停止点击";
        await Task.Run(() =>
        {
            var (cX, cY) = (Screen.PrimaryScreen!.Bounds.Width / 2,
                            Screen.PrimaryScreen.Bounds.Height / 2);
            _mouse.Move(cX, cY);
            for (int i = 0; i < 30 && !_cts.Token.IsCancellationRequested; i++)
            {
                _mouse.LeftClick();
                Thread.Sleep(330);
            }
        }, _cts.Token);
        mouseClickButton.Text = "点击测试（3 次/秒 × 10 秒）";
        _cts.Dispose(); _cts = null;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        var proj = Directory.GetParent(Application.StartupPath)?
                            .Parent?.Parent?.Parent?.FullName;
        if (proj == null) return;
        var icon = Path.Combine(proj, "res", "2/002~1.png");
        if (File.Exists(icon))
        {
            settingsButton.Image = new Bitmap(Image.FromFile(icon), new Size(32, 32));
            settingsButton.Text = string.Empty;
        }
    }

    private void settingsButton_Click(object sender, EventArgs e) => new SettingsForm().ShowDialog();

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")] private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
}