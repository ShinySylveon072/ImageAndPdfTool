using System.Drawing;
using System.Drawing.Imaging;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;

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

            var allFiles = Directory.GetFiles(selectedFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f =>
                    f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();

            progressBar.Minimum = 0;
            progressBar.Maximum = allFiles.Count;
            progressBar.Value = 0;

            var pdf = new PdfDocument();
            int processedCount = 0;

            foreach (var file in allFiles)
            {
                string ext = Path.GetExtension(file).ToLowerInvariant();
                try
                {
                    if (ext == ".pdf")
                    {
                        var destPdfPath = Path.Combine(processedFolder, Path.GetFileName(file));
                        File.Copy(file, destPdfPath, true);

                        using var inputDoc = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                        for (int idx = 0; idx < inputDoc.PageCount; idx++)
                        {
                            pdf.AddPage(inputDoc.Pages[idx]);
                        }
                    }
                    else
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

                                using var ximg = XImage.FromFile(part1Path);
                                var page = pdf.AddPage();
                                page.Width = XUnit.FromPoint(595);
                                page.Height = XUnit.FromPoint(842);
                                using var gfx = XGraphics.FromPdfPage(page);
                                gfx.DrawImage(ximg, 0, 0, page.Width.Point, page.Height.Point);
                            }
                            using (var part2 = new Bitmap(img.Width - halfWidth, img.Height))
                            using (var g2 = Graphics.FromImage(part2))
                            {
                                g2.DrawImage(img, new Rectangle(0, 0, img.Width - halfWidth, img.Height),
                                    new Rectangle(halfWidth, 0, img.Width - halfWidth, img.Height), GraphicsUnit.Pixel);

                                using var a4Part2 = ResizeToA4(part2);
                                var part2Path = Path.Combine(processedFolder, $"{Path.GetFileNameWithoutExtension(file)}_(2){Path.GetExtension(file)}");
                                a4Part2.Save(part2Path, ImageFormat.Jpeg);

                                using var ximg = XImage.FromFile(part2Path);
                                var page = pdf.AddPage();
                                page.Width = XUnit.FromPoint(595);
                                page.Height = XUnit.FromPoint(842);
                                using var gfx = XGraphics.FromPdfPage(page);
                                gfx.DrawImage(ximg, 0, 0, page.Width.Point, page.Height.Point);
                            }
                        }
                        else
                        {
                            // Portrait image: resize to A4 and save
                            using var a4Img = ResizeToA4(img);
                            var destPath = Path.Combine(processedFolder, Path.GetFileName(file));
                            a4Img.Save(destPath, ImageFormat.Jpeg);

                            using var ximg = XImage.FromFile(destPath);
                            var page = pdf.AddPage();
                            page.Width = XUnit.FromPoint(595);
                            page.Height = XUnit.FromPoint(842);
                            using var gfx = XGraphics.FromPdfPage(page);
                            gfx.DrawImage(ximg, 0, 0, page.Width.Point, page.Height.Point);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing file: {file}\n{ex.Message}");
                }

                processedCount++;
                progressBar.Value = processedCount;
                Application.DoEvents();
            }

            if (pdf.PageCount == 0)
            {
                MessageBox.Show("No images or PDFs processed. PDF not generated.");
                lblStatus.Text = "No images or PDFs processed.";
                progressBar.Value = 0;
                return;
            }

            var pdfPath = Path.Combine(processedFolder, "MergedA4Images.pdf");
            pdf.Save(pdfPath);

            lblStatus.Text = $"Done! PDF saved: {pdfPath}";
            progressBar.Value = progressBar.Maximum;
            MessageBox.Show("Processing complete!\nPDF saved:\n" + pdfPath);
        }
    }
}