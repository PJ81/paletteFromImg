using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace paletteFromImg {
    public partial class Form1 : Form {

        private List<Color> colors = new List<Color>();
        Bitmap img;

        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {

            if(dlgOpen.ShowDialog()== DialogResult.OK) {
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
            string txt = "[";
            foreach(ListViewItem li in listView1.Items) {
                txt += "\"" + li.Text.Trim() + "\", "; 
            }

            Clipboard.SetText(txt.Substring(0, txt.Length - 2) + "]");
        }
    }
}
