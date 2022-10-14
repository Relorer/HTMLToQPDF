using DevExpress.Mvvm;
using DevExpress.Mvvm.CodeGenerators;
using HTMLToQPDF.Example.DialogWindows;
using HTMLToQPDF.Example.Properties;
using HTMLToQPDF.Example.Utilities;
using System;
using System.IO;

namespace HTMLToQPDF.Example.ViewModels
{
    [GenerateViewModel]
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string HTML { get; set; }
        public string SavePath { get; set; }
        public bool CustomStyles { get; set; }

        public MainWindowViewModel()
        {
            HTML = Resources.testHtml;
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            SavePath = Path.Combine(desktop, "example.pdf");
        }

        [GenerateCommand]
        private void SelectSavePath()
        {
            SavePath = FileDialogHelper.GetSaveFilePath(SavePath);
        }

        [GenerateCommand]
        private void CreatePDF()
        {
            PDFCreator.Create(HTML, SavePath, CustomStyles);
        }

        private bool CanCreatePDF() => !string.IsNullOrEmpty(HTML);
    }
}