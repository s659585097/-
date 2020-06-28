using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace 影像處理
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnDlg = new OpenFileDialog();
            opnDlg.Filter = "所有圖片文件 | *.bmp; *.pcx; *.png; *.jpg; *.gif;" +
                "*.tif; *.ico; *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf|" +
                "位圖( *.bmp; *.jpg; *.png;...) | *.bmp; *.pcx; *.png; *.jpg; *.gif; *.tif; *.ico|" +
                "矢量圖( *.wmf; *.eps; *.emf;...) | *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf";
            opnDlg.Title = "Open file";
            opnDlg.ShowHelp = true;
            if (opnDlg.ShowDialog() == DialogResult.OK)
            {
                string pFileName = opnDlg.FileName;
                try
                {
                    Bitmap pBitmap = new Bitmap(pFileName);
                    //pBitmap = (Bitmap)Image.FromFile(pFileName);
                    pictureBox1.Image = pBitmap;
                    label1.Text = "您所選擇的圖";
                    textBox1.Text = opnDlg.FileName;
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;//這是讓程式一開始時，選項中就會有預設的，以免忘記選擇
        }

        private void Save(Image no)
        {
            //儲存圖片
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Title = "保存為";
            // OverwritePrompt: 控制在將要在改寫現在檔時是否提示用戶，只有在ValidateNames為真值時，才適用 
            saveDlg.OverwritePrompt = true;
            saveDlg.Filter =
                "BMP文件 (*.bmp) | *.bmp|" +
                "GIF文件 (*.gif) | *.gif|" +
                "JPEG文件 (*.jpg) | *.jpg|" +
                "PNG文件 (*.png) | *.png";
            saveDlg.ShowHelp = true;
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveDlg.FileName;
                string strFilExtn = fileName.Remove(0, fileName.Length - 3);
                switch (strFilExtn)
                {
                    case "bmp":
                        no.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case "jpg":
                        no.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "gif":
                        no.Save(fileName, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case "tif":
                        no.Save(fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    case "png":
                        no.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //二值化
            if (textBox1.Text != null)
            {
                Bitmap bitImage = new Bitmap(pictureBox1.Image);//二值化pictureBox1中的圖片
                Color c;
                int height = pictureBox1.Image.Height;
                int width = pictureBox1.Image.Width;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        c = bitImage.GetPixel(j, i);
                        int r = c.R;
                        int g = c.G;
                        int b = c.B;
                        if ((r + g + b) / 3 >= 127)
                        {
                            bitImage.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                        }
                        else
                        {
                            bitImage.SetPixel(j, i, Color.FromArgb(0, 0, 0));
                        }
                    }
                }
                pictureBox2.Image = bitImage;        //顯示二值化後的圖片
                label2.Text = "二值化後";       //標示為二值化
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(二值化)
            Save(pictureBox2.Image);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //灰階化
            Bitmap bitmap = new Bitmap(pictureBox1.Image);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int gray = (
                        bitmap.GetPixel(x, y).R +
                        bitmap.GetPixel(x, y).G +
                        bitmap.GetPixel(x, y).B) / 3;
                    Color color = Color.FromArgb(gray, gray, gray);
                    bitmap.SetPixel(x, y, color);
                }
            }
            pictureBox3.Image = bitmap;     //顯示灰階化後的圖
            label3.Text = "灰階化";        //標示
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(灰階化)
            Save(pictureBox3.Image);
        }



        static Bitmap Sobel(Bitmap m)
        {
            Bitmap b = new Bitmap(m);
            int width = b.Width;
            int height = b.Height;
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

            int[,] allPixR = new int[width, height];
            int[,] allPixG = new int[width, height];
            int[,] allPixB = new int[width, height];

            int limit = 128 * 128;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    allPixR[i, j] = b.GetPixel(i, j).R;
                    allPixG[i, j] = b.GetPixel(i, j).G;
                    allPixB[i, j] = b.GetPixel(i, j).B;
                }
            }

            int new_rx = 0, new_ry = 0;
            int new_gx = 0, new_gy = 0;
            int new_bx = 0, new_by = 0;
            int rc, gc, bc;
            for (int i = 1; i < b.Width - 1; i++)
            {
                for (int j = 1; j < b.Height - 1; j++)
                {

                    new_rx = 0;
                    new_ry = 0;
                    new_gx = 0;
                    new_gy = 0;
                    new_bx = 0;
                    new_by = 0;
                    rc = 0;
                    gc = 0;
                    bc = 0;

                    for (int wi = -1; wi < 2; wi++)
                    {
                        for (int hw = -1; hw < 2; hw++)
                        {
                            rc = allPixR[i + hw, j + wi];
                            new_rx += gx[wi + 1, hw + 1] * rc;
                            new_ry += gy[wi + 1, hw + 1] * rc;

                            gc = allPixG[i + hw, j + wi];
                            new_gx += gx[wi + 1, hw + 1] * gc;
                            new_gy += gy[wi + 1, hw + 1] * gc;

                            bc = allPixB[i + hw, j + wi];
                            new_bx += gx[wi + 1, hw + 1] * bc;
                            new_by += gy[wi + 1, hw + 1] * bc;
                        }
                    }
                    if (new_rx * new_rx + new_ry * new_ry > limit || new_gx * new_gx + new_gy * new_gy > limit || new_bx * new_bx + new_by * new_by > limit)
                        b.SetPixel(i, j, Color.Black);

                    //bb.SetPixel (i, j, Color.FromArgb(allPixR[i,j],allPixG[i,j],allPixB[i,j])); 
                    else
                        b.SetPixel(i, j, Color.Transparent);
                }
            }
            return b;
        }
        
        Bitmap Lap1(Bitmap image)
        {
            var image2 = new Bitmap(image);
            for (int x = 1; x < image.Width - 1; x++)
            {
                for (int y = 1; y < image.Height - 1; y++)
                {
                    Color color2, color4, color5, color6, color8;
                    color2 = image.GetPixel(x, y - 1);
                    color4 = image.GetPixel(x - 1, y);
                    color5 = image.GetPixel(x, y);
                    color6 = image.GetPixel(x + 1, y);
                    color8 = image.GetPixel(x, y + 1);
                    int r = color2.R + color4.R + color5.R * (-4) + color6.R + color8.R;
                    int g = color2.G + color4.G + color5.G * (-4) + color6.G + color8.G;
                    int b = color2.B + color4.B + color5.B * (-4) + color6.B + color8.B;
                    int avg = (r + g + b) / 3;
                    if (avg > 255) avg = 255;
                    if (avg < 0) avg = 0;
                    image2.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                }
            }
            return image2;
        }

        

        private void button4_Click(object sender, EventArgs e)
        {
            //邊緣偵測
            Bitmap b = new Bitmap(pictureBox1.Image);
            if (radioButton1.Checked == true)
                pictureBox4.Image = Sobel(b);//顯示Sobel邊緣偵測後的圖
            if (radioButton3.Checked == true)
            {
                pictureBox4.Image = Lap1(b);//顯示laplacian邊緣偵測後的圖
            }
                        
            
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(邊緣偵測)
            Save(pictureBox4.Image);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //中值濾波
            Bitmap pic = new Bitmap(pictureBox1.Image);
            int width = pic.Width;
            int height = pic.Height;
            int[,] resultPic = new int[height, width];
            int index;
            int[] filter = new int[9];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    index = 0;
                    int nowGray;
                    for (int ii = -1; ii < 2; ii++)
                    {
                        for (int jj = -1; jj < 2; jj++)
                        {
                            if (j + jj >= 0 && j + jj < width && i + ii >= 0 && i + ii < height)
                            {
                                nowGray = pic.GetPixel(j + jj, i + ii).R;
                            }
                            else { nowGray = 0; }
                            if (index == 0) { filter[index] = nowGray; index++; }
                            else
                            {
                                if (nowGray >= filter[index - 1])
                                {
                                    filter[index++] = nowGray;
                                }
                                else
                                {
                                    int current = index - 1;
                                    while (current > 0 && filter[current] > nowGray)
                                    {
                                        filter[current + 1] = filter[current];
                                        current--;
                                    }
                                    filter[current + 1] = nowGray;
                                    index++;
                                }

                            }


                        }
                    }
                    resultPic[i, j] = filter[4];
                    // int temp = filter[4];
                    //  Color color = Color.FromArgb(temp, temp, temp);
                    // pic.SetPixel(j, i, color);
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int temp = resultPic[i, j];
                    Color color = Color.FromArgb(temp, temp, temp);
                    pic.SetPixel(j, i, color);
                }
            }
            pictureBox5.Image = pic;
            label5.Text = "中值濾波後";
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(中值濾波)
            Save(pictureBox5.Image);
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            //這邊可以看我們Pa跟Pb要不要內建用個數字就好，還是要自己輸入，可以調想要的雜訊模糊度
            if (textBox2.Text != "" && textBox3.Text != "")
            {
                // Bitmap pic = (Bitmap)Bitmap.FromFile(filename, false);
                Bitmap pic = new Bitmap(pictureBox1.Image);
                double Pa = Convert.ToDouble(textBox2.Text);//接受输入的Pa
                double Pb = Convert.ToDouble(textBox3.Text);//接受输入的Pb
                double P = Pb / (1 - Pa);//程序要,为了得到一个概率Pb事件
                int width = pic.Width;
                int height = pic.Height;
                Random rand = new Random();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int gray;
                        int noise = 1;
                        double probility = rand.NextDouble();
                        if (probility < Pa)
                        {
                            noise = 255;//有Pa概率 噪声设为最大值
                        }
                        else
                        {
                            double temp = rand.NextDouble();
                            if (temp < P)//有1 - Pa的几率到达这里，再乘以 P ，刚好等于Pb
                                noise = 0;
                        }
                        if (noise != 1)
                        {
                            gray = noise;
                        }
                        else gray = pic.GetPixel(j, i).R;
                        Color color = Color.FromArgb(gray, gray, gray);
                        pic.SetPixel(j, i, color);
                    }
                }
                pictureBox6.Image = pic;

            }
            else
            {
                MessageBox.Show("請先輸入Pa和Pb^_^");
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Bitmap oriImage = new Bitmap(pictureBox1.Image);//將原始圖存入
            int threshold = 30;
            int width = oriImage.Width;
            int height = oriImage.Height;

            BitmapData oriImageData = oriImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //創建新的bitmap存放濾波後的圖
            Bitmap newImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData newImageData = newImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            IntPtr intPtrN = newImageData.Scan0;
            IntPtr intPtr = oriImageData.Scan0;
            int size = oriImageData.Stride * height;
            byte[] oriBytes = new byte[size];
            byte[] newBytes = new byte[size];
            Marshal.Copy(intPtr, oriBytes, 0, size);
            //先複製一份一模一樣的數據到濾波數組中
            Marshal.Copy(intPtr, newBytes, 0, size);
            int[] mask = new int[9];

            int k = 3;
            for (int y = 0; y < height - 2; y++)
            {
                for (int x = 0; x < width - 2; x++)
                {
                    //因为是灰階圖RGB相同，bitmapdata數组中就以每3个pixel為間隔。
                    mask[0] = oriBytes[y * oriImageData.Stride + x * k];
                    mask[1] = oriBytes[y * oriImageData.Stride + x * k + 3];
                    mask[2] = oriBytes[y * oriImageData.Stride + x * k + 6];

                    mask[3] = oriBytes[(y + 1) * oriImageData.Stride + x * k];
                    mask[4] = oriBytes[(y + 1) * oriImageData.Stride + x * k + 3];
                    mask[5] = oriBytes[(y + 1) * oriImageData.Stride + x * k + 6];

                    mask[6] = oriBytes[(y + 2) * oriImageData.Stride + x * k];
                    mask[7] = oriBytes[(y + 2) * oriImageData.Stride + x * k + 3];
                    mask[8] = oriBytes[(y + 2) * oriImageData.Stride + x * k + 6];

                    int mean = (mask[0] + mask[1] + mask[2] + mask[3] + mask[5] + mask[6] + mask[7] + mask[8]) / 8;

                    //绝對值很重要，不要忘
                    if (Math.Abs(mask[4] - mean) > threshold)
                    {
                        //newImageData.Stride 是等於 oriImageData.Stride 的
                        newBytes[(y + 1) * oriImageData.Stride + x * k + 3] = (byte)mean;
                        newBytes[(y + 1) * oriImageData.Stride + x * k + 4] = (byte)mean;
                        newBytes[(y + 1) * oriImageData.Stride + x * k + 5] = (byte)mean;
                    }
                }
            }
            Marshal.Copy(newBytes, 0, intPtrN, size);
            oriImage.UnlockBits(oriImageData);
            newImage.UnlockBits(newImageData);

            pictureBox7.Image = newImage;
            label8.Text = "低通濾波後";

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(低通濾波)
            Save(pictureBox7.Image);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(雜訊)
            Save(pictureBox6.Image);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //侵蝕(含getRoundPixel)
            Bitmap bitImage = new Bitmap(pictureBox1.Image);
            Bitmap bitImage1 = new Bitmap(pictureBox1.Image);
            Color c;
            int height = pictureBox1.Image.Height;
            int width = pictureBox1.Image.Width;
            bool[] pixels;
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    c = bitImage.GetPixel(i, j);
                    if (bitImage.GetPixel(i, j).R == 0)
                    {
                        pixels = getRoundPixel(bitImage, i, j);
                        for (int k = 0; k < pixels.Length; k++)
                        {
                            if (pixels[k] == false)
                            {
                                //set this piexl's color to black
                                bitImage1.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                                break;
                            }
                        }
                    }
                }
            }
            pictureBox8.Image = bitImage1;
        }
        public bool[] getRoundPixel(Bitmap bitmap, int x, int y)//返回(x,y)周圍像素的情況，爲黑色，則設置爲true
        {
            bool[] pixels = new bool[8];
            Color c;
            int num = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    c = bitmap.GetPixel(x + i, y + j);
                    if (i != 0 || j != 0)
                    {
                        if (255 == c.G)//因爲經過了二值化，所以只要檢查RGB中一個屬性的值
                        {
                            pixels[num] = false;//爲白色，設置爲false
                            num++;
                        }
                        else if (0 == c.G)
                        {
                            pixels[num] = true;//爲黑色，設置爲true
                            num++;
                        }
                    }
                }
            }
            return pixels;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(侵蝕)
            Save(pictureBox8.Image);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //侵蝕(含getRoundPixel)
            Bitmap bitImage = new Bitmap(pictureBox1.Image);//處理pictureBox1中的圖片
            Bitmap bitImage1 = new Bitmap(pictureBox1.Image);
            int height = pictureBox1.Image.Height;
            int width = pictureBox1.Image.Width;
            bool[] pixels;
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {

                    if (bitImage.GetPixel(i, j).R != 0)
                    {
                        pixels = getRoundPixel(bitImage, i, j);
                        for (int k = 0; k < pixels.Length; k++)
                        {
                            if (pixels[k] == true)
                            {
                                //set this piexl's color to black
                                bitImage1.SetPixel(i, j, Color.FromArgb(0, 0, 0));
                                break;
                            }
                        }
                    }
                }
            }
            pictureBox9.Image = bitImage1;        //顯示侵蝕後的圖片
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(膨脹)
            Save(pictureBox9.Image);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //負片效果
            if (null == pictureBox1)
                throw new Exception();

            int iHeight = pictureBox1.Image.Height;
            int iWidth = pictureBox1.Image.Width;

            Bitmap newBitmap = new Bitmap(iWidth, iHeight);
            Bitmap oldBitmap = pictureBox1.Image as Bitmap;

            try
            {
                Color pixel;        //表示一個像素點

                for (int x = 1; x < iWidth; x++)
                {
                    for (int y = 1; y < iHeight; y++)
                    {
                        int r, g, b;        //分別表示一個像素點紅 綠 藍的份量

                        pixel = oldBitmap.GetPixel(x, y);

                        r = 255 - pixel.R;
                        g = 255 - pixel.G;
                        b = 255 - pixel.B;

                        newBitmap.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
                    }
                }
            }
            catch (Exception ee)
            {
                throw new Exception();
            }
            pictureBox10.Image = newBitmap;      //顯示負片處理過的圖
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(負片效果)
            Save(pictureBox10.Image);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //調整亮度
            Bitmap a = new Bitmap(pictureBox1.Image);
            int v = Convert.ToInt16(textBox4.Text);
            System.Drawing.Imaging.BitmapData bmpData = a.LockBits(new Rectangle(0, 0, a.Width, a.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int bytes = a.Width * a.Height * 3;
            IntPtr ptr = bmpData.Scan0;
            int stride = bmpData.Stride;
            unsafe
            {
                byte* p = (byte*)ptr;
                int temp;
                for (int j = 0; j < a.Height; j++)
                {
                    for (int i = 0; i < a.Width * 3; i++, p++)
                    {
                        temp = (int)(p[0] + v);
                        temp = (temp > 255) ? 255 : temp < 0 ? 0 : temp;
                        p[0] = (byte)temp;
                    }
                    p += stride - a.Width * 3;
                }
            }
            a.UnlockBits(bmpData);
            pictureBox11.Image = a;      //顯示調整亮度後的圖片
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(亮度調整)
            Save(pictureBox11.Image);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //透明度調整
            Bitmap src = new Bitmap(pictureBox1.Image);
            Bitmap bImage = new Bitmap(pictureBox1.Image);
            int num = Convert.ToInt16(textBox5.Text);
            int w = src.Width;
            int h = src.Height;
            Bitmap dstBitmap = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData srcData = src.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData dstData = dstBitmap.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* pIn = (byte*)srcData.Scan0.ToPointer();
                byte* pOut = (byte*)dstData.Scan0.ToPointer();
                byte* p;
                int stride = srcData.Stride;
                int r, g, b;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        p = pIn;
                        b = pIn[0];
                        g = pIn[1];
                        r = pIn[2];
                        pOut[1] = (byte)g;
                        pOut[2] = (byte)r;
                        pOut[3] = (byte)num;
                        pOut[0] = (byte)b;
                        pIn += 4;
                        pOut += 4;
                    }
                    pIn += srcData.Stride - w * 4;
                    pOut += srcData.Stride - w * 4;
                }
                src.UnlockBits(srcData);
                dstBitmap.UnlockBits(dstData);
                pictureBox12.Image = dstBitmap;
            }
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(透明度調整)
            Save(pictureBox12.Image);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //對比度調整
            Bitmap b = new Bitmap(pictureBox1.Image);
            int degree = Convert.ToInt16(textBox6.Text);
            if (degree < -100) degree = -100;
            if (degree > 100) degree = 100;

            try
            {
                double pixel = 0;
                double contrast = (100.0 + degree) / 100.0;
                contrast *= contrast;
                int width = b.Width;
                int height = b.Height;
                BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* p = (byte*)data.Scan0;
                    int offset = data.Stride - width * 3;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // 處理指定位置像素的對比度
                            for (int i = 0; i < 3; i++)
                            {
                                pixel = ((p[i] / 255.0 - 0.5) * contrast + 0.5) * 255;
                                if (pixel < 0) pixel = 0;
                                if (pixel > 255) pixel = 255;
                                p[i] = (byte)pixel;
                            } // i
                            p += 3;
                        } // x
                        p += offset;
                    } // y
                }
                b.UnlockBits(data);
                pictureBox13.Image = b;
            }
            catch
            {
                pictureBox13.Image = null;
            }
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(對比度調整)
            Save(pictureBox13.Image);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            int a = 25;
            Bitmap img = new Bitmap(pictureBox1.Image);
            for (int h = 0; h < img.Height; h += a)
            {
                for (int w = 0; w < img.Width; w += a)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int count = 0;
                    for (int x = w; (x < w + a && x < img.Width); x++)
                    {
                        for (int y = h; (y < h + a && y < img.Height); y++)
                        {
                            Color pix = img.GetPixel(x, y);
                            avgR += pix.R;
                            avgG += pix.G;
                            avgB += pix.B;
                            count++;
                        }
                    }
                    avgR = avgR / count;
                    avgG = avgG / count;
                    avgB = avgB / count;
                    for (int x = w; (x < w + a && x < img.Width); x++)
                    {
                        for (int y = h; (y < h + a && y < img.Height); y++)
                        {
                            Color newColor = Color.FromArgb(avgR, avgG, avgB);
                            img.SetPixel(x, y, newColor);
                        }
                    }
                }
            }
            pictureBox14.Image = img;
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(馬賽克)
            Save(pictureBox14.Image);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //銳化
            int Height = this.pictureBox1.Image.Height;
            int Width = this.pictureBox1.Image.Width;
            Bitmap newBitmap = new Bitmap(Width, Height);
            Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
            Color pixel;
            //拉普拉斯模板
            int[] Laplacian = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
            for (int x = 1; x < Width - 1; x++)
                for (int y = 1; y < Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    int Index = 0;
                    for (int col = -1; col <= 1; col++)
                        for (int row = -1; row <= 1; row++)
                        {
                            pixel = oldBitmap.GetPixel(x + row, y + col); r += pixel.R * Laplacian[Index];
                            g += pixel.G * Laplacian[Index];
                            b += pixel.B * Laplacian[Index];
                            Index++;
                        }
                    //處理颜色值溢出
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    newBitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                }
            pictureBox15.Image = newBitmap;
        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(銳化)
            Save(pictureBox15.Image);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            //柔化
            int Height = this.pictureBox1.Image.Height;
            int Width = this.pictureBox1.Image.Width;
            Bitmap bitmap = new Bitmap(Width, Height);
            Bitmap MyBitmap = (Bitmap)this.pictureBox1.Image;
            Color pixel;
            //高斯模板
            int[] Gauss = { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
            for (int x = 1; x < Width - 1; x++)
                for (int y = 1; y < Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    int Index = 0;
                    for (int col = -1; col <= 1; col++)
                        for (int row = -1; row <= 1; row++)
                        {
                            pixel = MyBitmap.GetPixel(x + row, y + col);
                            r += pixel.R * Gauss[Index];
                            g += pixel.G * Gauss[Index];
                            b += pixel.B * Gauss[Index];
                            Index++;
                        }
                    r /= 16;
                    g /= 16;
                    b /= 16;
                    //处理颜色值溢出
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    bitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                }
            pictureBox16.Image = bitmap;
        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(柔化)
            Save(pictureBox16.Image);
        }

        public class LockBitmap
        {
            Bitmap source = null;
            IntPtr Iptr = IntPtr.Zero;
            BitmapData bitmapData = null;

            public byte[] Pixels { get; set; }
            public int Depth { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }

            public LockBitmap(Bitmap source)
            {
                this.source = source;
            }

            /// <summary>
            /// Lock bitmap data
            /// </summary>
            public void LockBits()
            {
                try
                {
                    // Get width and height of bitmap
                    Width = source.Width;
                    Height = source.Height;

                    // get total locked pixels count
                    int PixelCount = Width * Height;

                    // Create rectangle to lock
                    Rectangle rect = new Rectangle(0, 0, Width, Height);

                    // get source bitmap pixel format size
                    Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

                    // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                    if (Depth != 8 && Depth != 24 && Depth != 32)
                    {
                        throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                    }

                    // Lock bitmap and return bitmap data
                    bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                                 source.PixelFormat);

                    // create byte array to copy pixel values
                    int step = Depth / 8;
                    Pixels = new byte[PixelCount * step];
                    Iptr = bitmapData.Scan0;

                    // Copy data from pointer to array
                    Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Unlock bitmap data
            /// </summary>
            public void UnlockBits()
            {
                try
                {
                    // Copy data from byte array to pointer
                    Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

                    // Unlock bitmap data
                    source.UnlockBits(bitmapData);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// Get the color of the specified pixel
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public Color GetPixel(int x, int y)
            {
                Color clr = Color.Empty;

                // Get color components count
                int cCount = Depth / 8;

                // Get start index of the specified pixel
                int i = ((y * Width) + x) * cCount;

                if (i > Pixels.Length - cCount)
                    throw new IndexOutOfRangeException();

                if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
                {
                    byte b = Pixels[i];
                    byte g = Pixels[i + 1];
                    byte r = Pixels[i + 2];
                    byte a = Pixels[i + 3]; // a
                    clr = Color.FromArgb(a, r, g, b);
                }
                if (Depth == 24) // For 24 bpp get Red, Green and Blue
                {
                    byte b = Pixels[i];
                    byte g = Pixels[i + 1];
                    byte r = Pixels[i + 2];
                    clr = Color.FromArgb(r, g, b);
                }
                if (Depth == 8)
                // For 8 bpp get color value (Red, Green and Blue values are the same)
                {
                    byte c = Pixels[i];
                    clr = Color.FromArgb(c, c, c);
                }
                return clr;
            }

            /// <summary>
            /// Set the color of the specified pixel
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="color"></param>
            public void SetPixel(int x, int y, Color color)
            {
                // Get color components count
                int cCount = Depth / 8;

                // Get start index of the specified pixel
                int i = ((y * Width) + x) * cCount;

                if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
                {
                    Pixels[i] = color.B;
                    Pixels[i + 1] = color.G;
                    Pixels[i + 2] = color.R;
                    Pixels[i + 3] = color.A;
                }
                if (Depth == 24) // For 24 bpp set Red, Green and Blue
                {
                    Pixels[i] = color.B;
                    Pixels[i + 1] = color.G;
                    Pixels[i + 2] = color.R;
                }
                if (Depth == 8)
                // For 8 bpp set color value (Red, Green and Blue values are the same)
                {
                    Pixels[i] = color.B;
                }
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //浮雕
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            int height = bmp.Height;
            int width = bmp.Width;
            Bitmap newbmp = new Bitmap(width, height);

            LockBitmap lbmp = new LockBitmap(bmp);
            LockBitmap newlbmp = new LockBitmap(newbmp);
            lbmp.LockBits();
            newlbmp.LockBits();

            Color pixel1, pixel2;
            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    pixel1 = lbmp.GetPixel(x, y);
                    pixel2 = lbmp.GetPixel(x + 1, y + 1);
                    r = Math.Abs(pixel1.R - pixel2.R + 128);
                    g = Math.Abs(pixel1.G - pixel2.G + 128);
                    b = Math.Abs(pixel1.B - pixel2.B + 128);
                    if (r > 255)
                        r = 255;
                    if (r < 0)
                        r = 0;
                    if (g > 255)
                        g = 255;
                    if (g < 0)
                        g = 0;
                    if (b > 255)
                        b = 255;
                    if (b < 0)
                        b = 0;
                    newlbmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            lbmp.UnlockBits();
            newlbmp.UnlockBits();
            pictureBox17.Image = newbmp;
        }

        private void pictureBox17_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(浮雕)
            Save(pictureBox17.Image);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            //霧化
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            int height = bmp.Height;
            int width = bmp.Width;
            Bitmap newbmp = new Bitmap(width, height);

            LockBitmap lbmp = new LockBitmap(bmp);
            LockBitmap newlbmp = new LockBitmap(newbmp);
            lbmp.LockBits();
            newlbmp.LockBits();

            System.Random MyRandom = new Random();
            Color pixel;
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int k = MyRandom.Next(123456);
                    //像素塊大小
                    int dx = x + k % 19;
                    int dy = y + k % 19;
                    if (dx >= width)
                        dx = width - 1;
                    if (dy >= height)
                        dy = height - 1;
                    pixel = lbmp.GetPixel(dx, dy);
                    newlbmp.SetPixel(x, y, pixel);
                }
            }
            lbmp.UnlockBits();
            newlbmp.UnlockBits();
            pictureBox18.Image = newbmp;
        }

        private void pictureBox18_Click(object sender, EventArgs e)
        {
            //點按圖片便可將影像另存新檔(霧化)
            Save(pictureBox18.Image);
        }
    }
}



        

