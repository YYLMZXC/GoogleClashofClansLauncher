using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GoogleClashofClansLauncher.Input
{
    /// <summary>
    /// 最小化鼠标模拟器：仅提供移动、左/右/中键单击、简单点击测试。
    /// </summary>
    public sealed class MouseSimulator : IDisposable
    {
        #region Win32
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint flags, uint dx, uint dy, uint data, IntPtr extra);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        private const uint MOVE = 0x0001;
        private const uint LEFTDOWN = 0x0002;
        private const uint LEFTUP = 0x0004;
        private const uint RIGHTDOWN = 0x0008;
        private const uint RIGHTUP = 0x0010;
        private const uint MIDDLEDOWN = 0x0020;
        private const uint MIDDLEUP = 0x0040;
        #endregion

        /// <summary>移动鼠标到屏幕坐标</summary>
        public void Move(int x, int y) => SetCursorPos(x, y);

        /// <summary>左键单击一次</summary>
        public void LeftClick()
        {
            mouse_event(LEFTDOWN, 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(20);
            mouse_event(LEFTUP, 0, 0, 0, IntPtr.Zero);
        }

        /// <summary>右键单击一次</summary>
        public void RightClick()
        {
            mouse_event(RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(20);
            mouse_event(RIGHTUP, 0, 0, 0, IntPtr.Zero);
        }

        /// <summary>中键单击一次</summary>
        public void MiddleClick()
        {
            mouse_event(MIDDLEDOWN, 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(20);
            mouse_event(MIDDLEUP, 0, 0, 0, IntPtr.Zero);
        }

        /// <summary>
        /// 简单点击测试：在屏幕中心连续左键点击 30 次（3 次/秒，共 10 秒）。
        /// 可通过 cancellationToken 提前退出。
        /// </summary>
        public void ClickTest(CancellationToken token = default)
        {
            var (cX, cY) = (SystemInformation.PrimaryMonitorSize.Width / 2,
                            SystemInformation.PrimaryMonitorSize.Height / 2);
            Move(cX, cY);

            for (int i = 0; i < 30 && !token.IsCancellationRequested; i++)
            {
                LeftClick();
                Thread.Sleep(330); // ≈ 3 次/秒
            }
        }

        public void Dispose() { /* 暂无非托管资源 */ }
    }
}