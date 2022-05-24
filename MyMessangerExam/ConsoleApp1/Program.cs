using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
           
            using (FileStream stream = File.OpenWrite(@"C:\Users\Super\Desktop\serverTest.ico"))
            {
                Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(@"C:\Users\Super\Desktop\server.png");
                Icon.FromHandle(bitmap.GetHicon()).Save(stream);
            }
        }
    }
}
