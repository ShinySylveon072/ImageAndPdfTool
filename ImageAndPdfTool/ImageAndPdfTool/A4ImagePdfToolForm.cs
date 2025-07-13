using System.Drawing;
using System.Drawing.Imaging;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace ImageAndPdfTool
{
    public partial class A4ImagePdfToolForm : Form
    {
        private string? selectedFolder;

        public A4ImagePdfToolForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen; // Start centered
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                selectedFolder = dialog.SelectedPath;
                txtFolder.Text = selectedFolder;
                lblStatus.Text = "Folder selected.";
            }
        }

        private static Bitmap ResizeToA4(Image src)
        {
            // Resize image to A4 size (portrait, 300dpi)
            int a4Width = 2480;
            int a4Height = 3508;
            var bmp = new Bitmap(a4Width, a4Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.Clear(Color.White);
                g.DrawImage(src, 0, 0, a4Width, a4Height);
            }
            return bmp;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFolder))
            {
                MessageBox.Show("Please select a folder first.");
                return;
            }

            lblStatus.Text = "Processing...";
            Application.DoEvents();

            string processedFolder = Path.Combine(selectedFolder, "A4_Processed");

            // Delete old A4_Processed folder and its contents if it exists
            if (Directory.Exists(processedFolder))
            {
                try
                {
                    Directory.Delete(processedFolder, true); // true: recursive delete
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete old A4_Processed folder: {ex.Message}");
                    return;
                }
            }
            Directory.CreateDirectory(processedFolder);

            var imageFiles = Directory.GetFiles(selectedFolder, "*.*")
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var a4Images = new List<string>();

            // Set progress bar maximum value
            progressBar.Minimum = 0;
            progressBar.Maximum = imageFiles.Count;
            progressBar.Value = 0;

            int processedCount = 0;
            foreach (var file in imageFiles)
            {
                try
                {
                    using var img = Image.FromFile(file);

                    if (img.Width > img.Height)
                    {
                        // Split landscape image into two A4 images
                        int halfWidth = img.Width / 2;
                        using (var part1 = new Bitmap(halfWidth, img.Height))
                        using (var g1 = Graphics.FromImage(part1))
                        {
                            g1.DrawImage(img, new Rectangle(0, 0, halfWidth, img.Height),
                                new Rectangle(0, 0, halfWidth, img.Height), GraphicsUnit.Pixel);

                            using var a4Part1 = ResizeToA4(part1);
                            var part1Path = Path.Combine(processedFolder, $"{Path.GetFileNameWithoutExtension(file)}_(1){Path.GetExtension(file)}");
                            a4Part1.Save(part1Path, ImageFormat.Jpeg);
                            a4Images.Add(part1Path);
                        }
                        using (var part2 = new Bitmap(img.Width - halfWidth, img.Height))
                        using (var g2 = Graphics.FromImage(part2))
                        {
                            g2.DrawImage(img, new Rectangle(0, 0, img.Width - halfWidth, img.Height),
                                new Rectangle(halfWidth, 0, img.Width - halfWidth, img.Height), GraphicsUnit.Pixel);

                            using var a4Part2 = ResizeToA4(part2);
                            var part2Path = Path.Combine(processedFolder, $"{Path.GetFileNameWithoutExtension(file)}_(2){Path.GetExtension(file)}");
                            a4Part2.Save(part2Path, ImageFormat.Jpeg);
                            a4Images.Add(part2Path);
                        }
                    }
                    else
                    {
                        // Portrait image: resize to A4 and save
                        using var a4Img = ResizeToA4(img);
                        var destPath = Path.Combine(processedFolder, Path.GetFileName(file));
                        a4Img.Save(destPath, ImageFormat.Jpeg);
                        a4Images.Add(destPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing image: {file}\n{ex.Message}");
                }

                processedCount++;
                progressBar.Value = processedCount;
                Application.DoEvents(); // Refresh UI
            }

            var sortedA4Images = a4Images.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();

            if (sortedA4Images.Count == 0)
            {
                MessageBox.Show("No images processed. PDF not generated.");
                lblStatus.Text = "No images processed.";
                progressBar.Value = 0;
                return;
            }

            var pdf = new PdfDocument();
            foreach (var imgPath in sortedA4Images)
            {
                using var img = XImage.FromFile(imgPath);
                var page = pdf.AddPage();
                page.Width = XUnit.FromPoint(595);   // A4 width in points (72 dpi)
                page.Height = XUnit.FromPoint(842);  // A4 height in points (72 dpi)
                using var gfx = XGraphics.FromPdfPage(page);
                gfx.DrawImage(img, 0, 0, page.Width.Point, page.Height.Point);
            }
            var pdfPath = Path.Combine(processedFolder, "MergedA4Images.pdf");
            pdf.Save(pdfPath);

            lblStatus.Text = $"Done! PDF saved: {pdfPath}";
            progressBar.Value = progressBar.Maximum;
            MessageBox.Show("Processing complete!\nPDF saved:\n" + pdfPath);
        }
    }
}