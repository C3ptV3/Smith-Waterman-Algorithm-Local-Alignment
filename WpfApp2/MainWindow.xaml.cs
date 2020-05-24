using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace WpfApp2
{
    [System.Security.SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        private const string Kernel32_DllName = "kernel32.dll";

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32_DllName)]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            //#if DEBUG
            if (!HasConsole)
            {
                AllocConsole();
            }
            //#endif
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            //#if DEBUG
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
            //#endif
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ConsoleManager.Show();
            string refSeq = "ATGCATCCCATGAC";
            string alignSeq = "TCTATATCCGT";

            int mismatch = -2;
            int match =2;
            int gap = -3;

            int x = refSeq.Length+1;
            int y = alignSeq.Length+1;

            int[,] matrix = new int[y, x];
            for (int j = 0; j < x; j++)
            {
                matrix[0, j] = 0;
            }
            for (int j = 0; j < y; j++)
            {
                matrix[j, 0] = 0;
            }
            Console.Write("  ");
            for (int j = 0; j < x-1; j++)
            {
                Console.Write(" "+refSeq[j]);
            }
            Console.WriteLine();

            for (int j = 1; j < y; j++)
            {
                Console.Write(" " +alignSeq[j-1]);
                for (int i = 1; i < x; i++)
                {
                    matrix[j, i] = Math.Max(Math.Max( refSeq[i-1] == alignSeq[j-1] ? (matrix[j-1,i-1]+match) : (matrix[j - 1, i - 1] + mismatch),matrix[j-1,i]+gap), Math.Max(matrix[j, i-1]+gap,0));
                    Console.Write(" " + matrix[j, i]);
                }
                Console.WriteLine();
            }


        }
    }
}
