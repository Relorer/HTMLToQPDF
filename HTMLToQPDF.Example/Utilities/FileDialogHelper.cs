using Microsoft.Win32;

namespace HTMLToQPDF.Example.Utilities
{
    internal static class FileDialogHelper
    {
        public static string GetSaveFilePath(string file)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF file|*.pdf",
                Title = "Save an PDF File",
                FileName = file
            };

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                file = saveFileDialog.FileName;
            }

            return file;
        }
    }
}