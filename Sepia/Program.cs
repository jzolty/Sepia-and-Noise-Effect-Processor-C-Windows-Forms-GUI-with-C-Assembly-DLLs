//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Runtime.InteropServices;
//using System.Threading;
//using System.Diagnostics;
//using System.IO;

//namespace JaProj
//{
//    class Program
//    {
//        // Import funkcji z DLL dla efektu sepia
//        [DllImport(@"C:\Users\Julia\Desktop\Sepia\x64\Release\DLLCPP.dll")]
//        public static extern void ApplySepiaFilter(IntPtr pixelBuffer, int width, int bytesPerPixel, byte P, byte X, int startRow, int endRow, int stride);

//        // Import funkcji z DLL dla efektu czarno-białego
//        [DllImport(@"C:\Users\Julia\Desktop\Sepia\x64\Release\DLLASM.dll")]
//        public static extern void ApplySepiaFilterAsm(IntPtr pixelBuffer, int width, int bytesPerPixel, int stride, byte P, int startRow, int endRow, byte X);

//        static void Main(string[] args)
//        {

//            string filePath = @"C:\Users\Julia\Desktop\Sepia\Samples\2400x1600.bmp";
//            Bitmap bitmap = new Bitmap(filePath);

//            int numberOfThreads;
//            do
//            {
//                Console.WriteLine("Podaj liczbę wątków (od 1 do 64):");
//                if (!int.TryParse(Console.ReadLine(), out numberOfThreads) || numberOfThreads < 1 || numberOfThreads > 64)
//                {
//                    Console.WriteLine("Niepoprawna liczba wątków. Podaj liczbę z zakresu 1-64.");
//                }
//            } while (numberOfThreads < 1 || numberOfThreads > 64);


//            int filterChoice;
//            do
//            {
//                Console.WriteLine("Wybierz efekt: 1 - CPP, 2 - ASM(red):");
//                if (!int.TryParse(Console.ReadLine(), out filterChoice) || (filterChoice != 1 && filterChoice != 2))
//                {
//                    Console.WriteLine("Niepoprawny wybór. Wybierz 1 lub 2.");
//                }
//            } while (filterChoice != 1 && filterChoice != 2);

//            byte P = 0;
//            //if (filterChoice == 1)
//            //{
//            do
//            {
//                Console.WriteLine("Podaj współczynnik P (zakres 20-40):");
//                if (!byte.TryParse(Console.ReadLine(), out P) || P < 20 || P > 40)
//                {
//                    Console.WriteLine("Niepoprawny współczynnik P. Podaj liczbę z zakresu 20-40.");
//                }
//            } while (P < 20 || P > 40);

//            // }

//            byte X = 0;
//            //if (filterChoice == 1)
//            //{
//            do
//            {
//                Console.WriteLine("Podaj wartość X dla efektu postarzenia (zakres 0-50):");
//                if (!byte.TryParse(Console.ReadLine(), out X) || X < 0 || X > 50)
//                {
//                    Console.WriteLine("Niepoprawny parametr X. Podaj liczbę z zakresu 0-50.");
//                }
//            } while (X < 0 || X > 50);
//            //}

//            int height = bitmap.Height;
//            int width = bitmap.Width;

//            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
//            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
//            int stride = bitmapData.Stride;

//            IntPtr ptr = bitmapData.Scan0;
//            byte[] pixelBuffer = new byte[stride * height];
//            Marshal.Copy(ptr, pixelBuffer, 0, pixelBuffer.Length);
//            IntPtr bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(pixelBuffer, 0);

//            int[] startIndices, endIndices;
//            DivideDataForThreads(height, numberOfThreads, out startIndices, out endIndices);

//            Thread[] threads = new Thread[numberOfThreads];
//            Stopwatch stopwatch = new Stopwatch();
//            stopwatch.Start();

//            for (int i = 0; i < numberOfThreads; i++)
//            {
//                int startRow = startIndices[i];
//                int endRow = endIndices[i];

//                if (startRow < 0 || endRow > height || startRow >= endRow)
//                {
//                    throw new ArgumentOutOfRangeException("Niepoprawny zakres wierszy.");
//                }

//                if (filterChoice == 1)
//                {
//                    threads[i] = new Thread(() => ProcessSepiaSection(bufferPtr, width, bytesPerPixel, stride, P, X, startRow, endRow));


//                }
//                else
//                {
//                    threads[i] = new Thread(() => ProcessApplySepiaFilterAsm(bufferPtr, width, bytesPerPixel, stride, P, startRow, endRow, X));
//                }
//                threads[i].Start();
//            }

//            foreach (Thread thread in threads)
//            {
//                thread.Join();
//            }

//            stopwatch.Stop();
//            Console.WriteLine($"Czas przetwarzania przy {numberOfThreads} wątkach: {stopwatch.Elapsed.TotalMilliseconds} ms");

//            Marshal.Copy(pixelBuffer, 0, ptr, pixelBuffer.Length);
//            bitmap.UnlockBits(bitmapData);

//            string outputFilePath;
//            if (filterChoice == 1)
//            {
//                outputFilePath = @"C:\Users\Julia\Desktop\Sepia\SepiaCpp.bmp";
//            }
//            else
//            {
//                outputFilePath = @"C:\Users\Julia\Desktop\Sepia\SepiaAsm.bmp";
//            }
//            bitmap.Save(outputFilePath);
//            Console.WriteLine("Przetwarzanie zakończone. Zapisano wynik jako: " + outputFilePath);
//        }

//        static void ProcessSepiaSection(IntPtr bufferPtr, int width, int bytesPerPixel, int stride, byte P, byte X, int startRow, int endRow)
//        {
//            ApplySepiaFilter(bufferPtr, width, bytesPerPixel, P, X, startRow, endRow, stride);
//        }



//        static void ProcessApplySepiaFilterAsm(IntPtr bufferPtr, int width, int bytesPerPixel, int stride, byte P, int startRow, int endRow, byte X)
//        {
//            ApplySepiaFilterAsm(bufferPtr, width, bytesPerPixel, stride, P, startRow, endRow, X);
//        }

//        static void DivideDataForThreads(int totalRows, int numberOfThreads, out int[] startIndices, out int[] endIndices)
//        {
//            startIndices = new int[numberOfThreads];
//            endIndices = new int[numberOfThreads];

//            int baseChunkSize = totalRows / numberOfThreads;
//            int extraRows = totalRows % numberOfThreads;

//            int currentStart = 0;
//            for (int i = 0; i < numberOfThreads; i++)
//            {
//                int chunkSize = baseChunkSize;
//                if (i < extraRows)
//                {
//                    chunkSize += 1;
//                }

//                startIndices[i] = currentStart;
//                endIndices[i] = currentStart + chunkSize;
//                currentStart = endIndices[i];
//            }
//        }
//    }
//}

using Sepia;
using System;
using System.Windows.Forms;

namespace JaProj
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Włączenie stylu wizualnego i uruchomienie formularza
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
