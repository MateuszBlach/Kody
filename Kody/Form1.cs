using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Kody
{
    public partial class Generator : Form
    {
        int first;
        public Generator()
        {
            InitializeComponent();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {

        }
        
        private void btnBarcode_Click2(object sender, EventArgs e)
        {
            //Zen.Barcode.CodeEan13BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.CodeEan13WithChecksum; // ustawienie trybu generowanie kodow EAN-13
            if(txtBarcode.Text.Length == 12) {
                
                //pictureBox1.Padding = new Padding(50,50,50,50);
                //pictureBox1.Image = barcode.Draw(txtBarcode.Text, 100,trackBar1.Value); // stworzenie obrazu w oknie na podstawie cyfr wprowadzonych w polu tekstowym
   
                
                String input = txtBarcode.Text;
                int checkSum = 0;
                int factor = 1;
                for (int i = 0; i < input.Length; i++) {

                    if (i % 2 == 0) factor = 1;
                    else factor = 3;
                    checkSum += int.Parse(input[i].ToString())*factor;
                }
                checkSum = checkSum % 10;
                checkSum = 10 - checkSum;
                checkSum = checkSum % 10;

                String barcodeString = "";
                for(int i = 0; i < 12; i++) {
                    barcodeString += input[i];
                    if(trackBar1.Value > 2)barcodeString += " ";
                }
                barcodeString += checkSum;
                var resultImage = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height + 20);

                using (var graphics = Graphics.FromImage(resultImage))
                using (var font = new Font("Consolas", 6+trackBar1.Value*2))
                using (var brush = new SolidBrush(Color.Black))
                using (var format = new StringFormat() {
                    Alignment = StringAlignment.Center, 
                    LineAlignment = StringAlignment.Far
                }) {
                    graphics.Clear(Color.White);
                    graphics.DrawImage(pictureBox1.Image, 0, 0);
                    graphics.DrawString(barcodeString, font, brush, resultImage.Width / 2, resultImage.Height, format);
                }

                pictureBox1.Image = resultImage;

            } else {
                MessageBox.Show("Wprowadz dokladnie 12 cyfr");
            }


        }
        

        private void btnBarcode_Click(object sender, EventArgs e)
        {
            if (txtBarcode.Text.Length == 12)
            {
                int barWidth = 3;
                int barHeight = 100;
                int labelHeight = 20;
                pictureBox1.Padding = new Padding(50, 50, 50, 50);

                // Generate EAN-13 barcode string
                string input = txtBarcode.Text;
                int checkSum = GenerateEAN13Checksum(input);
                string barcodeString = input + checkSum;
   

                // Create barcode image
                Bitmap barcodeImage = CreateEAN13BarcodeImage(barcodeString, barWidth, barHeight,0);

                // Create result image with barcode and label
                int resultWidth = barcodeImage.Width;
                int resultHeight = barcodeImage.Height + labelHeight;
                Bitmap resultImage = new Bitmap(resultWidth, resultHeight);


                using (Graphics graphics = Graphics.FromImage(resultImage))
                using (Font font = new Font("Consolas", 8 + trackBar1.Value * 2))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                using (StringFormat format = new StringFormat()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Far
                })
                {
                    graphics.Clear(Color.White);
                    graphics.DrawImage(barcodeImage, 0, 0);
                    graphics.DrawString(barcodeString, font, brush, resultWidth / 2, resultHeight, format);
                }

                // Display result image in PictureBox
                pictureBox1.Image = resultImage;
            }
            else
            {
                MessageBox.Show("Wprowadz dokladnie 12 cyfr");
            }
        }
        private int GenerateEAN13Checksum(string data)
        {
            // Calculate EAN-13 checksum
            int evenSum = 0;
            int oddSum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                int digit = int.Parse(data[i].ToString());
                if (i == 0)
                {
                    first = digit;
                }
                if (i % 2 == 0)
                {
                    evenSum += digit;
                }
                else
                {
                    oddSum += digit * 3;
                }
            }
            int totalSum = evenSum + oddSum;
            int checksum = (10 - totalSum % 10) % 10;
            return checksum;
        }

        private Bitmap CreateEAN13BarcodeImage(string data, int barWidth, int barHeight, int offsetX)
        {
            // Calculate total width of the barcode image
            int totalWidth = (data.Length * 7 + 8) * barWidth;  // Each digit has 7 bars, and there are 8 additional bars

            // Create EAN-13 barcode image
            Bitmap barcodeImage = new Bitmap(totalWidth, barHeight);

            using (Graphics graphics = Graphics.FromImage(barcodeImage))
            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                graphics.Clear(Color.White);



                int x = offsetX;


                graphics.FillRectangle(brush, x, 0, barWidth, (barHeight / 2) + 10);
                x += barWidth;
                x += barWidth;
                graphics.FillRectangle(brush, x, 0, barWidth, (barHeight / 2) + 10);
                x += barWidth;
                for (int i = 1; i < data.Length; i++)
                {
                    int digit = int.Parse(data[i].ToString());
                    string binaryCode = GetEAN13BinaryCode(digit,i);
                    Console.WriteLine(binaryCode);
                    for (int j = 0; j < binaryCode.Length; j++)
                    {
                        if (binaryCode[j] == '1')
                        {
                            graphics.FillRectangle(brush, x, 0, barWidth, barHeight / 2);
                        }
                        x += barWidth;
                    }

                    // Add additional separator bar after every 7 bars
                    if (i == 6)
                    {
                        x += barWidth;
                        graphics.FillRectangle(brush, x, 0, barWidth, (barHeight / 2) + 10);
                        x += barWidth;
                        x += barWidth;
                        graphics.FillRectangle(brush, x, 0, barWidth, (barHeight / 2) + 10);
                        x += barWidth;
                        x += barWidth;
                    }
                }
                graphics.FillRectangle(brush, x, 0, barWidth, (barHeight / 2)+10);
                x += barWidth;
                x += barWidth;
                graphics.FillRectangle(brush, x, 0, barWidth, (barHeight / 2)+10);
                x += barWidth;
            }

            return barcodeImage;
        }




        private string GetEAN13BinaryCode(int digit,int position)
        {
            string[] codesA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
            string[] codesB = { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
            string[] codesC = { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };
            int[][] matrixD = {
                new[] {1, 1, 1, 1, 1, 1},
                new[] {1, 1, 0, 1, 0, 0},
                new[] {1, 1, 0, 0, 1, 0},
                new[] {1, 1, 0, 0, 0, 1},
                new[] {1, 0, 1, 1, 0, 0},
                new[] {1, 0, 0, 1, 1, 0},
                new[] {1, 0, 0, 0, 1, 1},
                new[] {1, 0, 1, 0, 1, 0},
                new[] {1, 0, 1, 0, 0, 1},
                new[] {1, 0, 0, 1, 0, 1}
            };

            int[] codes;


            if (position <= 6)
            {
                if (matrixD[first][position-1] == 1)
                {
                    codes = codesA[digit].Select(c => c == '0' ? 0 : 1).ToArray();
                }
                else
                {
                    codes = codesB[digit].Select(c => c == '0' ? 0 : 1).ToArray();
                }
            }
            else
            {
                codes = codesC[digit].Select(c => c == '0' ? 0 : 1).ToArray();
            }

            return string.Join("", codes);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void myPrintDocument2_PrintPage(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)

        {

            Bitmap myBitmap1 = new Bitmap(pictureBox1.Width, pictureBox1.Height); // stworzenie bitmapy poprzez pobranie szerokosci i wysokosci okna

            pictureBox1.DrawToBitmap(myBitmap1, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height)); // renderowanie zawartosci okna do mapy bitowej

            e.Graphics.DrawImage(myBitmap1, 0, 0); // rysowanie 

            myBitmap1.Dispose(); // zwolnienie zasobow uzywanych przez mape bitowa

        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            // stworzenie obiektu typu PrintDocument 
            System.Drawing.Printing.PrintDocument myPrintDocument1 = new System.Drawing.Printing.PrintDocument();
            // stworzenie obiektu typu PrintDialog (okno dialogowe)
            PrintDialog myPrinDialog1 = new PrintDialog();
            //stworzenie akcji drukowania stron
            myPrintDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(myPrintDocument2_PrintPage);
            myPrinDialog1.Document = myPrintDocument1;


            // jesli zatwierdzimy okno dialogowe wykonamy wydruk
            if (myPrinDialog1.ShowDialog() == DialogResult.OK)

            {

                myPrintDocument1.Print();

            }
        }


        private void label2_Click(object sender, EventArgs e) {

        }

        private void label2_Click_1(object sender, EventArgs e) {

        }
    }
}
