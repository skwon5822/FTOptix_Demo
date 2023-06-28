#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.UI;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using System.Diagnostics;
#endregion

public class PrintPdfFile : BaseNetLogic
{
    string foxitPath;
    string pdfPath;
    private LongRunningTask PdfPrintLongRunning;

    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void PrintPdf(string pdfFile) {
        // Lettura percorso Foxit Reader
        foxitPath = LogicObject.GetVariable("FoxitReaderPath").Value;
        // Lettura percorso file PDF
        pdfPath = pdfFile;
        // Esecuzione task asincrono
        PdfPrintLongRunning = new LongRunningTask(ExecuteFoxitPrint, LogicObject);
        PdfPrintLongRunning.Start();
    }

    private void ExecuteFoxitPrint() {
        // Controllo che i percorsi non siano vuoti
        // Controllo che il percorso contenga l'eseguibile
        if (!(foxitPath.Contains(".exe"))) {
            Log.Error("Pdf print", "Foxit Reader exe path error");
            return;
        }
        // Controllo file pdf
        if (!(pdfPath.Contains(".pdf"))) {
            // Controllo estensione PDF
            Log.Error("Pdf print", "File is not a PDF");
            return;
        } else if (!(pdfPath.Contains(":"))) {
            // Controllo percorso assoluto
            Log.Error("Pdf print", "Path is not absolute");
            return;
        }
        // Preparazione eseuzione processo in background
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = foxitPath;
        process.StartInfo.Arguments = "/t " + pdfPath;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        // Avvio processo di stampa
        process.Start();
        // Ottenimento del output dal comando
        string processOutput = "";
        while (!process.HasExited) {
            processOutput += process.StandardOutput.ReadToEnd();
        }
        // Log del output messaggio
        if (processOutput.Length > 1) {
            Log.Info("Pdf print", processOutput);
        } else {
            Log.Info("Pdf print", "Stampa completata");
        }
        PdfPrintLongRunning.Cancel();
    }
}
