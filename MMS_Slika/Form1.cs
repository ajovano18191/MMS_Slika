using System.Windows.Forms;

namespace MMS_Slika
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (ofdImage.ShowDialog() == DialogResult.OK) {
                string filePath = ofdImage.FileName;
                pbOriginal.Image = Image.FromFile(filePath);
                
            }
        }
    }
}
