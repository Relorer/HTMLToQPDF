using Microsoft.Win32;

namespace HTMLToQPDF.Example.DialogWindows
{
    internal static class FileDialogHelper
    {
        public static string GetSaveFilePath(string file)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "PDF file|*.pdf";
            saveFileDialog.Title = $"Save an PDF File";
            saveFileDialog.FileName = file;
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                file = saveFileDialog.FileName;
            }

            return file;
        }
    }
}