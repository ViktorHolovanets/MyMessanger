using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace LibraryMessage
{
    public class MyFunction
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int CloseWindow(IntPtr hWnd);


        public static BitmapImage ConvertBytesToImage(byte[] source)
        {
            MemoryStream byteStream = new MemoryStream(source);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = byteStream;
            image.EndInit();
            return image;
        }
        public static byte[] ConvertToBytes(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            ms.Position = 0;
            return ms.ToArray();
        }
        public static void SetFocusWindow(string WindowTitle)
        {
            IntPtr bl = FindWindowByCaption(IntPtr.Zero, WindowTitle);
            if (bl == IntPtr.Zero) return;
            SetWindowPos(bl, IntPtr.Zero, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0040);
            SetForegroundWindow(bl);
            SetFocus(bl);
            SetActiveWindow(bl);
        }
        //
        public static void KillWindow(string WindowTitle)
        {
            IntPtr bl = FindWindowByCaption(IntPtr.Zero, WindowTitle);
            if (bl == IntPtr.Zero) return;
            DestroyWindow(bl);
        }
        public static void MyCloseWindow(string WindowTitle)
        {
            IntPtr bl = FindWindowByCaption(IntPtr.Zero, WindowTitle);
            if (bl == IntPtr.Zero) return;
            CloseWindow(bl);
        }
        public static string StringConnection(string Path)
        {
            var path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), $"..\\..\\{Path}");
            FileInfo fileInfo = new FileInfo(path);           
            return $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={fileInfo.FullName};Integrated Security=True";
        }

        public static Bitmap GetScreenBitmap()
        {           
            Bitmap BM = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics GF = Graphics.FromImage(BM);
            GF.CopyFromScreen(0, 0, 0, 0, BM.Size);
            return BM;
        }
    }
}
