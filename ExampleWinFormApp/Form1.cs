using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using de.nanofocus.NFEval;
using System.IO;
using System.Security.Cryptography;

namespace ExampleWinFormApp
{
    public partial class Form1 : Form
    {

        VariantEditorControl.VariantEditorControl vc = new VariantEditorControl.VariantEditorControl();


       
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = "Drop your file here.";
            
            panel2.Controls.Add(vc);
        }

       
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void panel2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] filePath = (string[])e.Data.GetData(DataFormats.FileDrop, false);
        }

        private void panel2_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file);
                if (ext.Equals(".npsx", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.Effect = DragDropEffects.Copy;
                    vc.LoadData(file);
                    if (toolStripStatusLabel2.Text == "")
                    {
                        toolStripStatusLabel2.Text += Path.GetFullPath(file);
                    }
                    label2.Text = "";
                    string tempDir = Path.Combine(Path.GetTempPath(), "VariantEditor");
                    Directory.CreateDirectory(tempDir);
                    return;
                }
            }
        }

        private string GetFileHash(string fInfo)
        {
            HashAlgorithm hashAlgo = HashAlgorithm.Create();
            using (var stream = new FileStream(fInfo, FileMode.Open, FileAccess.Read))
            {

                var hash = hashAlgo.ComputeHash(stream);
                hashAlgo.Dispose();
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            using (openFileDialog1)
            {
                openFileDialog1.Filter = "NPSX Files (*.npsx)|*.npsx";
                openFileDialog1.ShowReadOnly = true;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    path = openFileDialog1.FileName;
                    string fileName = Path.GetFileName(path);
                    vc.LoadData(path);
                    Console.WriteLine(GetFileHash(path));
                    if (toolStripStatusLabel2.Text == "")
                    {
                        toolStripStatusLabel2.Text += Path.GetFullPath(path);
                    }
                    label2.Text = "";
                    string tempDir = Path.Combine(Path.GetTempPath(), "VariantEditor");
                    Directory.CreateDirectory(tempDir);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
