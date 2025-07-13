# A3 to A4 Image Splitter and PDF Merger

## Overview

This Windows Forms tool allows you to batch process images or PDFs in a selected folder, automatically splitting landscape (A3-like) images into two A4-sized images, resizing all images to standard A4 dimensions, and merging them into a single PDF file. The tool is designed for .NET 8 and uses PDFsharp for PDF generation.

## Features

- Select a folder containing images (`.jpg`, `.jpeg`, `.png`, `.bmp`)
- Automatically splits landscape images (width > height) into two A4 images
- Portrait images are resized to A4 without splitting
- All processed images are saved in a new `A4_Processed` subfolder
- Merges all processed images into a single A4-sized PDF
- Progress bar shows processing status
- PDF and images are named and sorted for easy reference
- Fixed-size, non-resizable window for consistent UI

## Requirements

- .NET 8 SDK
- PDFsharp (add via NuGet: `PdfSharp`)

## Usage

1. **Build and run the application** in Visual Studio 2022 or later.
2. **Click "Select Folder"** and choose the folder containing your images.
3. **Click "Process Images"** to start batch processing.
   - The tool will delete any previous `A4_Processed` subfolder in the selected folder.
   - All images will be processed and saved in the new `A4_Processed` subfolder.
   - A merged PDF named `MergedA4Images.pdf` will be created in the same subfolder.
4. **Check the status label and progress bar** for completion and the output path.

## Output

- **Processed images:** Saved in `[SelectedFolder]\A4_Processed`
- **Merged PDF:** `[SelectedFolder]\A4_Processed\MergedA4Images.pdf`

## Notes

- Only images with extensions `.jpg`, `.jpeg`, `.png`, `.bmp` are processed.
- Landscape images are split; portrait images are only resized.
- The application window is fixed size and cannot be resized.
