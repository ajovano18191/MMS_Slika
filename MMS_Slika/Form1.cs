using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MMS_Slika
{
    public partial class Form1 : Form
    {
        Bitmap originalBMP = new Bitmap(1, 1);
        Bitmap filteredBmp = new Bitmap(1, 1);

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            if(ofdImage.ShowDialog() == DialogResult.OK) {
                string filePath = ofdImage.FileName;
                if(filePath.EndsWith(".ako")) {
                    Cursor = Cursors.WaitCursor;
                    originalBMP = await Task.Run(() => {
                        return AKO2BMP.Load(filePath);
                    });
                    Cursor = Cursors.Default;
                }
                else {
                    originalBMP = (Bitmap)Bitmap.FromFile(filePath);
                }

                pbOriginal.Image = originalBMP;
                await FilterImage(originalBMP);
            }
        }

        private async Task FilterImage(Bitmap bmp)
        {
            await Task.Run(() => {
                lock(filteredBmp) {
                    this.Invoke(() => Cursor = Cursors.WaitCursor);
                    filteredBmp = (Bitmap)bmp.Clone();
                    if(cbBaseFilter.Checked) {
                        filteredBmp = Filters.Invert(filteredBmp);
                    }
                    if(cbAdvancedFilter.Checked) {
                        filteredBmp = Filters.Kuwahara(filteredBmp, 6);
                    }
                    if(cbDithering.Checked) {
                        filteredBmp = Filters.BurkesDithering(this, filteredBmp);
                    }
                    pbFiltered.Image = filteredBmp;
                    this.Invoke(() => Cursor = Cursors.Default);
                }
            });
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            sfdImage.FileName = ofdImage.SafeFileName.Remove(ofdImage.SafeFileName.LastIndexOf('.')) + ".ako";
            if(sfdImage.ShowDialog() == DialogResult.OK) {
                string filePath = sfdImage.FileName;
                Cursor = Cursors.WaitCursor;
                byte wordLength = 8;
                if(rbDS16.Checked) {
                    wordLength = 4;
                }
                else if(rbDS256.Checked) {
                    wordLength = 8;
                }
                await Task.Run(() => {
                    lock(filteredBmp) {
                        BMP2AKO.Save(filePath, filteredBmp, wordLength);
                    }
                });
                Cursor = Cursors.Default;
            }
        }

        private async void cbDithering_CheckedChanged(object sender, EventArgs e)
        {
            await FilterImage(originalBMP);
        }

        private async void cbBaseFilter_CheckedChanged(object sender, EventArgs e)
        {
            await FilterImage(originalBMP);
        }

        private async void cbAdvancedFilter_CheckedChanged(object sender, EventArgs e)
        {
            await FilterImage(originalBMP);
        }
    }
}
