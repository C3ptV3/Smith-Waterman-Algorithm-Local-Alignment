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
        public void DoStuff(Grid DynamicGrid)
        {
            ColumnDefinition gridCol1 = new ColumnDefinition();
            gridCol1.Width = new System.Windows.GridLength(60);

            DynamicGrid.ColumnDefinitions.Add(gridCol1);
        }
        public void DoStuff1(Grid DynamicGrid)
        {
            RowDefinition gridRow1 = new RowDefinition();
            gridRow1.Height = new System.Windows.GridLength(60);
            DynamicGrid.RowDefinitions.Add(gridRow1);
        }
        public TextBlock DoStuff2(int i, string a)
        {
            TextBlock txtBlock1 = new TextBlock();

            txtBlock1.Text = a;

            txtBlock1.FontSize = 15;

            txtBlock1.VerticalAlignment = VerticalAlignment.Top;

            Grid.SetRow(txtBlock1, 0);

            Grid.SetColumn(txtBlock1, i);

            return txtBlock1;
        }
        public TextBlock DoStuff3(int i, string a)
        {
            TextBlock txtBlock1 = new TextBlock();

            txtBlock1.Text = a;

            txtBlock1.FontSize = 15;

            txtBlock1.VerticalAlignment = VerticalAlignment.Top;

            Grid.SetRow(txtBlock1, i);

            Grid.SetColumn(txtBlock1, 0);

            return txtBlock1;
        }
        public MainWindow()
        {
            Grid DynamicGrid = new Grid();
            DynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
            DynamicGrid.VerticalAlignment = VerticalAlignment.Top;
            DynamicGrid.ShowGridLines = true;

            ConsoleManager.Show();
            string[] lines = File.ReadAllLines("C:\\Users\\ehabi\\source\\repos\\WpfApp2\\WpfApp2\\seqT.txt");
            string[] lines2 = File.ReadAllLines("C:\\Users\\ehabi\\source\\repos\\WpfApp2\\WpfApp2\\seqS.txt");

            string refSeq = lines[1];
            string alignSeq = lines2[1];

            Console.WriteLine("Mismatch :");
            int mismatch = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Match :");
            int match = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Gap :");
            int gap = Int32.Parse(Console.ReadLine()); 

            int x = refSeq.Length+1;
            int y = alignSeq.Length+1;

            DynamicGrid.Width = 70 * x;
            DynamicGrid.Height = 80 * y;

            for (int i = 0; i < x + 2; i++)
            {
                DoStuff(DynamicGrid);
            }
            for (int i = 0; i < y + 2; i++)
            {
                DoStuff1(DynamicGrid);
            }

            for (int i = 0; i < refSeq.Length; i++)
            {
                DynamicGrid.Children.Add(DoStuff2(i + 2, refSeq[i].ToString()));

            }
            for (int i = 0; i < alignSeq.Length; i++)
            {
                DynamicGrid.Children.Add(DoStuff3(i + 2, alignSeq[i].ToString()));

            }

            int[,] matrix = new int[y, x];

            int[,] traceback = new int[y, x];
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    traceback[i, j] = 0;
                }

            }


            for (int j = 0; j < x; j++)
            {
                matrix[0, j] = 0;
            }
            for (int j = 0; j < y; j++)
            {
                matrix[j, 0] = 0;
            }
            int max=0;

            int[,] maxindex = new int[1, 2];

            for (int j = 1; j < y; j++)
            {
                
                for (int i = 1; i < x; i++)
                {
                    matrix[j, i] = Math.Max(Math.Max( refSeq[i-1] == alignSeq[j-1] ? (matrix[j-1,i-1]+match) : (matrix[j - 1, i - 1] + mismatch),matrix[j-1,i]+gap), Math.Max(matrix[j, i-1]+gap,0));
                    if(matrix[j, i] > max) 
                    { 
                        max = matrix[j, i];
                        maxindex[0, 0] = j;
                        maxindex[0, 1] = i;
                    }
                }
               
            }

            int lastx = maxindex[0, 0];
            int lasty = maxindex[0, 1];

            do
            {

                traceback[lastx, lasty] = 1;

                lastx -= 1;
                lasty -= 1;
            } while (matrix[lastx-1, lasty-1]-match > 0);

            traceback[lastx, lasty] = 1;
            int count = 0;
            int flag = 0;
            for (int i = 0; i < y; i++)
                for (int j = 0; j < x; j++)
                {
                    {
                        TextBlock txtBlock1 = new TextBlock();
                        txtBlock1.Text = string.Empty;
                        if (traceback[i, j] == 1) { 
                            txtBlock1.Background = Brushes.Red;
                            txtBlock1.Text += "↖";
                            count += 1;
                        }
                        
                        
                    
                        txtBlock1.Text += matrix[i, j].ToString();
                        txtBlock1.FontSize = 15;

                        txtBlock1.VerticalAlignment = VerticalAlignment.Center;

                        Grid.SetRow(txtBlock1, i + 1);

                        Grid.SetColumn(txtBlock1, j + 1);
                        DynamicGrid.Children.Add(txtBlock1);
                    }
                }

            for(int i = 0; i < count; i++)
            {
                if (refSeq[maxindex[0, 1] - count + i]!=alignSeq[maxindex[0, 0] - count + i])
                {
                    continue;
                }
                Console.Write(refSeq[maxindex[0, 1]-count+i]);
            }
            Console.WriteLine();
            for (int i = 0; i < count; i++)
            {
                if (refSeq[maxindex[0, 1] - count + i] != alignSeq[maxindex[0, 0] - count + i])
                {
                    continue;
                }
                Console.Write(refSeq[maxindex[0, 1] - count + i]);
            }
            Console.WriteLine();
            Console.WriteLine("Score is :"+max);

            Application.Current.MainWindow.Content = DynamicGrid;
        }
    }
}
