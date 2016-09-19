using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace KinkyTools
{
    class KinkyTools
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            [MarshalAs(UnmanagedType.U2)]
            public short Year;
            [MarshalAs(UnmanagedType.U2)]
            public short Month;
            [MarshalAs(UnmanagedType.U2)]
            public short DayOfWeek;
            [MarshalAs(UnmanagedType.U2)]
            public short Day;
            [MarshalAs(UnmanagedType.U2)]
            public short Hour;
            [MarshalAs(UnmanagedType.U2)]
            public short Minute;
            [MarshalAs(UnmanagedType.U2)]
            public short Second;
            [MarshalAs(UnmanagedType.U2)]
            public short Milliseconds;

            public SYSTEMTIME(DateTime dt)
            {
                Year = (short)dt.Year;
                Month = (short)dt.Month;
                DayOfWeek = (short)dt.DayOfWeek;
                Day = (short)dt.Day;
                Hour = (short)dt.Hour;
                Minute = (short)dt.Minute;
                Second = (short)dt.Second;
                Milliseconds = (short)dt.Millisecond;
            }
        }

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetSystemTime(ref SYSTEMTIME time);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern int UnregisterHotKey(IntPtr hwnd, int id);

        class Frame : Form
        {
            public Frame()
            {
                int w = 300, h = 105;
                Icon icon = new Icon("../../content/orc_icon.ico");
                this.Icon = icon;
                this.SetBounds(0, 0, w, h);
                this.Text = "NKT - Time Changer";
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;

                uint[] keys = { 0x70, 0x71, 0x72, 0x73, 0x74, 0x75 };
                string[] keyNames = { "F1", "F2", "F3", "F4", "F5", "F6" };

                ComboBox keySelection = new ComboBox();
                keySelection.Text = "Select bound key";
                keySelection.Parent = this;
                keySelection.SetBounds(5, 5, w - 25, 25);

                for(int i = 0; i < keys.Length; i++ )
                {
                    keySelection.Items.Add("CTRL + " + keyNames[ i ]);
                }

                ApplyButton apply = new ApplyButton(() =>
                {
                    if (keySelection.SelectedIndex != -1)
                    {
                        ChangeBind(this.Handle, keys[keySelection.SelectedIndex]);
                    }
                } );
                apply.Text = "Apply";
                apply.Parent = this;
                apply.SetBounds(5, 35, w - 25, 25);

                RegisterHotKey(this.Handle, 0, 0x0002, keys[ 0 ]);
            }

            protected override void WndProc(ref Message m)
            {
                if(m.Msg == 0x0312)
                {
                    ChangeTime();
                }

                base.WndProc(ref m);
            }
        }

        class ApplyButton : Button
        {
            public delegate void action();
            public action Action;
            public ApplyButton( action _action )
            {
                this.Action = _action;
            }

            protected override void OnClick(EventArgs e)
            {
                this.Action();
                base.OnClick(e);
            }
        }

        static void ChangeBind( IntPtr handle, uint key )
        {
            UnregisterHotKey(handle, 0);
            RegisterHotKey(handle, 0, 0x0002, key);
        }

        static void ChangeTime()
        {
            DateTime curTime = DateTime.Now;
            curTime.AddHours(1);

            SYSTEMTIME newTime = new SYSTEMTIME( curTime );
            SetSystemTime(ref newTime);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            Frame frame = new Frame();
            frame.ShowDialog();
        }
    }
}