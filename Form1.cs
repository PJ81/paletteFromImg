using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace paletteFromImg {
    public partial class Form1 : Form {

        private List<Color> colors = new List<Color>();
        private Bitmap img;
        private bool validFile;
        private Thread getImageThread;

        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {

            if(dlgOpen.ShowDialog()== DialogResult.OK) {
                colors.Clear();
                listView1.Items.Clear();

                img = new Bitmap(dlgOpen.FileName);
                pictureBox1.Image = img;
            } 
        }

        private double dist(Color c1, Color c2) {
            int r1 = c1.R, g1 = c1.G, b1 = c1.B,
                r2 = c2.R, g2 = c2.G, b2 = c2.B;

            int r = r1 - r2, g = g1 - g2, b = b1 - b2;
            r *= r;
            g *= g;
            b *= b;

            return Math.Sqrt(r + g + b);
        }

        private bool acceptColor(Color d) {
            int ds = Convert.ToInt32(textBox1.Text);
            if (ds < 1) {
                ds = 1;
                textBox1.Text = ds.ToString();
            }

            foreach (Color c in colors) {
                if (dist(c, d) < ds) return false;
            }
            return true;
        }

        private Color getForeColor(Color c) {
            float br2 = 0.3f * c.R + 0.59f * c.G + 0.11f * c.B;
            if (br2 > 127.0) return Color.Black;
            return Color.White;
        }

        private void button2_Click(object sender, EventArgs e) {
            if (img == null) return;

            colors.Clear();
            listView1.Items.Clear();

            Cursor = Cursors.WaitCursor;
            for (int y = 0; y < img.Height; y++) {
                for(int x = 0; x < img.Width;x++) {
                    Color c = img.GetPixel(x, y);
                    if(acceptColor(c)) colors.Add(c);
                }
            }

            Cursor = Cursors.Default;

            foreach(Color c in colors) {
                ListViewItem li = new ListViewItem();
                li.BackColor = c;
                li.ForeColor = getForeColor(c);
                li.Text = "\n #" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2") + " " ;
                listView1.Items.Add(li);
            }

            MessageBox.Show("Colors count: " + colors.Count.ToString());
            
        }

        private void button3_Click(object sender, EventArgs e) {
            if (listView1.Items.Count < 1) return;

            string txt = "[";
            foreach(ListViewItem li in listView1.Items) {
                txt += "\"" + li.Text.Trim() + "\", "; 
            }

            Clipboard.SetText(txt.Substring(0, txt.Length - 2) + "]");
        }

        private void Form1_DragDrop(object sender, DragEventArgs e) {
            if (validFile) {
                while (getImageThread.IsAlive) {
                    Application.DoEvents();
                    Thread.Sleep(0);
                }

                colors.Clear();
                listView1.Items.Clear();

                pictureBox1.Image = img;
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e) {
            string filename = getFilename(e);
            validFile = !filename.Equals("");

            if (validFile) {
                getImageThread = new Thread(new ThreadStart(() =>
                {
                    img = new Bitmap(filename);
                }));
                getImageThread.Start();
                e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private string getFilename(DragEventArgs e) {

            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
                Array data = e.Data.GetData("FileDrop") as Array;
                if (data != null) {
                    if ((data.Length == 1) && (data.GetValue(0) is String)) {
                        string filename = ((string[])data)[0];
                        string ext = Path.GetExtension(filename).ToLower();
                        if ((ext == ".jpg") || (ext == ".jpeg") || (ext == ".png") || (ext == ".bmp")) {
                            return filename;
                        }
                    }
                }
            }
            return "";
        }
    }
}
