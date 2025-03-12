
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices; //;marshal
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace Sepia
{
    public partial class MainForm : Form
    {
        //calling convention, cla cpp byte(int0-255) a int nie ma znaczenia
        //deklaracja funkcji zewnetrznych bibliotek ddl
        [DllImport(@"C:\Users\Julia\Desktop\Sepia\x64\Release\DLLCPP.dll")]
         public static extern void ApplySepiaFilter(IntPtr pixelBuffer, int width, int bytesPerPixel, byte P, byte X, int startRow, int endRow, int stride);

        [DllImport(@"C:\Users\Julia\Desktop\Sepia\x64\Release\DLLASM.dll")]
         public static extern void ApplySepiaFilterAsm(IntPtr pixelBuffer, int width, int bytesPerPixel, int stride, byte P, int startRow, int endRow, byte X);

        //zmienne przechowywyjace obraz do przetwarzania
        private Bitmap bitmap;
        private Bitmap originalBitmap;

        //parametry sterujace
        private int numberOfThreads;  //liczba watkow
        private int filterChoice;     //wybrany filtr cpp/asm
        private byte sepiaParameter;  //parametr sepii P
        private byte X;               //parametr szumu X

        //elementy gui
        private ProgressBar progressBar;
        private Label labelTime;

        public MainForm()
        {
            InitializeComponent(); //inicjalizacja elementow gui

            //obsluga wyboru liczby watkow z ComboBox
            ComboBox comboThreads = Controls["comboThreads"] as ComboBox;
            if (comboThreads != null)
            {
                comboThreads.SelectedIndexChanged += ComboThreads_SelectedIndexChanged;
            }
        }

        private PictureBox pictureBoxOriginal; //obraz wgrywany
        private PictureBox pictureBoxProcessed; //obraz generowany

        private void InitializeComponent()
        {
            //okno glowne
            this.Text = "Sepia Filter GUI";
            this.ClientSize = new System.Drawing.Size(1000, 660);

            int centerX = this.ClientSize.Width / 2; //wysrodkowanie elementow

            //label do wyboru pliku bmp
            Label labelFile = new Label()
            {
                Text = "Select BMP File:",
                AutoSize = true,
                Location = new Point(centerX - 120, 20)
            };
            this.Controls.Add(labelFile);

            //button do wyboru pliku
            Button buttonSelectFile = new Button()
            {
                Text = "Browse",
                Width = 100,
                Location = new Point(centerX, 15)
            };
            buttonSelectFile.Click += ButtonSelectFile_Click; //obsluga klikniecia
            this.Controls.Add(buttonSelectFile);

            //obszar wyswietlania oryginalnego, wgranego obrazu
            pictureBoxOriginal = new PictureBox()
            {
                Width = 400,
                Height = 300,
                Location = new Point(centerX - 450, 60),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(pictureBoxOriginal);

            //obszar wyswietlania przetworzonego obrazu
            pictureBoxProcessed = new PictureBox()
            {
                Width = 400,
                Height = 300,
                Location = new Point(centerX + 50, 60),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            this.Controls.Add(pictureBoxProcessed);

            //label wyboru liczby watkow
            Label labelThreads = new Label()
            {
                Text = "Number of Threads:",
                AutoSize = true,
                Location = new Point(centerX - 150, 380)
            };
            this.Controls.Add(labelThreads);

            //comboboc do wyboru liczby watkow
            ComboBox comboThreads = new ComboBox()
            {
                Name = "comboThreads",
                Width = 100,
                Location = new Point(centerX, 375)
            };
            comboThreads.Items.AddRange(new object[] { 1, 2, 4, 8, 16, 32, 64 });
            comboThreads.SelectedIndex = 0;
            comboThreads.DropDownStyle = ComboBoxStyle.DropDownList;
            comboThreads.SelectedIndexChanged += ComboThreads_SelectedIndexChanged;
            this.Controls.Add(comboThreads);
            numberOfThreads = 1; //domyslnie ustawione na 1

            //label do wyboru filtru cpp/asm
            Label labelFilter = new Label()
            {
                Text = "Filter Type:",
                AutoSize = true,
                Location = new Point(centerX - 150, 420)
            };
            this.Controls.Add(labelFilter);

            //przycisk radiowy do wyboru cpp/asm
            RadioButton radioCpp = new RadioButton()
            {
                Text = "C++",
                AutoSize = true,
                Location = new Point(centerX - 30, 415)
            };

            RadioButton radioAsm = new RadioButton()
            {
                Text = "ASM",
                AutoSize = true,
                Location = new Point(centerX + 50, 415)
            };

            radioCpp.Checked = true; //domyslnie wybrany cpp
            radioCpp.CheckedChanged += (s, e) => { if (radioCpp.Checked) filterChoice = 1; };
            radioAsm.CheckedChanged += (s, e) => { if (radioAsm.Checked) filterChoice = 2; };
            this.Controls.Add(radioCpp);
            this.Controls.Add(radioAsm);
            filterChoice = 1;

            //label do parametru sepiii
            Label labelP = new Label()
            {
                Text = "Sepia Parameter (20-40):",
                AutoSize = true,
                Location = new Point(centerX - 150, 460)
            };
            this.Controls.Add(labelP);

            //numericupdown do parametru sepii
            NumericUpDown numericP = new NumericUpDown()
            {
                Width = 60,
                Location = new Point(centerX, 455),
                Minimum = 20,
                Maximum = 40,
                Value = 30
            };
            numericP.ValueChanged += (s, e) => { sepiaParameter = (byte)numericP.Value; };
            this.Controls.Add(numericP);
            sepiaParameter = 20; 

            //label do parametru szumu
            Label labelX = new Label()
            {
                Text = "Noise Parameter (0-50):",
                AutoSize = true,
                Location = new Point(centerX - 150, 500)
            };
            this.Controls.Add(labelX);

            //numericupdown do parametru szumu
            NumericUpDown numericX = new NumericUpDown()
            {
                Width = 60,
                Location = new Point(centerX, 495),
                Minimum = 0,
                Maximum = 50,
                Value = 25
            };
            // numericX.ValueChanged += (s, e) => { sepiaParameter = (byte)numericX.Value; };
            numericX.ValueChanged += (s, e) => { X = (byte)numericX.Value; };

            this.Controls.Add(numericX);
            X = 25;

            //progress bar do wsazania postepu przetwarzania
            progressBar = new ProgressBar()
            {
                Width = 600,
                Height = 20,
                Location = new Point(centerX - 300, 550)
            };
            this.Controls.Add(progressBar);

            //label do wyswietlania czasu przetwarzania
            labelTime = new Label()
            {
                Text = "Processing Time: 0 ms",
                AutoSize = true,
                Location = new Point(centerX - 75, 580)
            };
            this.Controls.Add(labelTime);

            //przycisk rozpoczynajacy przetwarzania obrazu
            Button buttonSubmit = new Button()
            {
                Text = "Submit",
                Width = 100,
                Location = new Point(centerX - 50, 620)
            };

            //obsluga klikniecie przycisku 'submit'
            buttonSubmit.Click += (s, e) =>
            {
                if (originalBitmap == null)
                {
                    MessageBox.Show("Please select a BMP file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bitmap = (Bitmap)originalBitmap.Clone();

                progressBar.Value = 0;
                ProcessImage();
                pictureBoxProcessed.Image = (Bitmap)bitmap.Clone();

                MessageBox.Show("Processing complete. Image saved to the desktop.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                progressBar.Value = 100;
            };
            this.Controls.Add(buttonSubmit);
        }

        //obsluga przycisku do wyboru pliku bmp
        private void ButtonSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "BMP Files (*.bmp)|*.bmp",
                Title = "Select BMP File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalBitmap = new Bitmap(openFileDialog.FileName); //zaladowanie obrazu
                bitmap = (Bitmap)originalBitmap.Clone();           //klonowanie do przetwarzania
                pictureBoxOriginal.Image = (Bitmap)originalBitmap.Clone();  //wyswietlanie obrazu oryginalnego
                MessageBox.Show($"File loaded: {openFileDialog.FileName}", "File Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //obsluga zmiany liczby watkow
        private void ComboThreads_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo != null)
            {
                numberOfThreads = (int)combo.SelectedItem; //ustawienie liczby watkow
            }
        }

        //funkcja do przetwarzania obrazu
        private void ProcessImage()
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8; //ilosc bajtow na piksel
            //odwrocona kolejnosc bitmaty
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = bitmapData.Scan0; //wskaznik do danych bitmatu
            int stride = bitmapData.Stride; //rozmiar wiersza w bajtach

            //kopiowanie danych bitmatu do bufora
            byte[] pixelBuffer = new byte[stride * height];
            Marshal.Copy(ptr, pixelBuffer, 0, pixelBuffer.Length);
            IntPtr bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(pixelBuffer, 0);
            //podzial danych na watki
            int[] startIndices, endIndices;
            DivideDataForThreads(height, numberOfThreads, out startIndices, out endIndices);
            //tworzenie watkow przetwarzajacych obraz
            Thread[] threads = new Thread[numberOfThreads];
            Stopwatch threadStopwatch = Stopwatch.StartNew(); //start pomiaru czasu przetwarzania

            for (int i = 0; i < numberOfThreads; i++)
            {
                int startRow = startIndices[i];
                int endRow = endIndices[i];

                threads[i] = new Thread(() =>
                {
                    if (filterChoice == 1)
                    { 
                        //wywolanie funkcji cpp
                        ApplySepiaFilter(bufferPtr, width, bytesPerPixel, sepiaParameter, X, startRow, endRow, stride);
                    }
                    else
                    {
                        //wywolanie funkcji asm
                        ApplySepiaFilterAsm(bufferPtr, width, bytesPerPixel, stride, sepiaParameter, startRow, endRow, X);
                    }
                });
                threads[i].Start(); //uruchomienie watku
            }

            foreach (var thread in threads)
            {
                thread.Join(); //oczekiwanie na zakonczenie watku
            }

            threadStopwatch.Stop(); //zatrzymanie pomiaru czasu
            double threadProcessingTime = threadStopwatch.Elapsed.TotalMilliseconds;

            Invoke(new Action(() =>
            {
                labelTime.Text = $"Thread Processing Time: {threadProcessingTime:F2} ms"; //wyswietlenie czsu przetwarzania
            }));

            Marshal.Copy(pixelBuffer, 0, ptr, pixelBuffer.Length); //skopiowanie danych do bitmatu
            bitmap.UnlockBits(bitmapData); //odblokowanie bitmatu
            //zapisanie przetworzonego obrazu na pulpicie
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputPath = System.IO.Path.Combine(desktopPath, $"SepiaOutput.bmp");
            bitmap.Save(outputPath);
        }

        //funkcja do podzialu danych na watki
        private void DivideDataForThreads(int totalRows, int numberOfThreads, out int[] startIndices, out int[] endIndices)
        {
            startIndices = new int[numberOfThreads]; // Tablica początkowych indeksów wierszy dla wątków
            endIndices = new int[numberOfThreads];   // Tablica końcowych indeksów wierszy dla wątków

            int baseChunkSize = totalRows / numberOfThreads; //liczba wierszy na watek,podstawowy rozmiar danych na watki
            int extraRows = totalRows % numberOfThreads; //dodatkowe wiersze do rozdzielenia
           
           
            int currentStart = 0; //indeks poczatkowy dla obecnego watku
            for (int i = 0; i < numberOfThreads; i++)
            {
                // Dla wątków z dodatkowym wierszem zwiększamy `chunkSize` o 1
                int chunkSize = baseChunkSize + (i < extraRows ? 1 : 0); //przydzielenie dodatkowych wierszy
                startIndices[i] = currentStart; //ustaw poczatek zakresu
                endIndices[i] = currentStart + chunkSize; //ustaw koniec zakresu
                currentStart = endIndices[i];//zaktualizuj poczatek dla nastepnego watku
            }
        }
    }
}
