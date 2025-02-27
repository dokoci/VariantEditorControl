﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace ExampleWinFormApp
{
    public partial class Form1 : Form
    {

        VariantEditorControl.VariantEditorControl vc = new VariantEditorControl.VariantEditorControl();
        private string fileName { get; set; }
        private string sourcePath { get; set; }
        private string tempPath = Path.Combine(Path.GetTempPath(), "VariantEditor");
        private string sourceFile { get; set; }
        private string tempFile { get; set; }

        private bool isFileThere;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            Directory.CreateDirectory(tempPath);
            isFileThere = false;
            panel1.Controls.Add(vc);
            FormClosing += Form1_FormClosing;
            OpenExtension();
        }

        private bool IsFileInUse(string file)
        {
            FileStream s = null;
            try
            {
                s = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                MessageBox.Show("File is in use!");
                return true;
            }
            using (s)
            {
                return false;
            }
        }
        private void OpenExtension()
        {
            label2.Text = "Drop your file here.";
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {

                string ext = Path.GetExtension(arg);
                fileName = Path.GetFileName(arg);
                sourcePath = Path.GetDirectoryName(arg);
                sourceFile = Path.Combine(sourcePath, fileName);
                tempFile = Path.Combine(tempPath, fileName);
                DeleteTempFile(tempFile);
                //File.Copy(sourceFile, tempFile, true);
                if (ext.Equals(".npsx", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (toolStripStatusLabel2.Text == "")
                    {
                        toolStripStatusLabel2.Text += Path.GetFullPath(arg);
                    }
                    vc.LoadData(sourceFile);
                    isFileThere = true;
                    label2.Text = "";
                    return;
                }
            }
        }


        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (!isFileThere && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void DeleteTempFile(string tempFile)
        {
            if (File.Exists(tempFile))
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch (IOException e)
                {

                    MessageBox.Show(e.Message);
                    return;
                }
            }
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var file in files)
            {
                string ext = Path.GetExtension(file);
                fileName = Path.GetFileName(file);
                sourcePath = Path.GetDirectoryName(file);
                sourceFile = Path.Combine(sourcePath, fileName);
                tempFile = Path.Combine(tempPath, fileName);
                DeleteTempFile(tempFile);
                File.Copy(sourceFile, tempFile, true);
                if (ext.Equals(".npsx", StringComparison.CurrentCultureIgnoreCase))
                {
                    //e.Effect = DragDropEffects.Copy;
                    //vc.LoadData(file);
                    if (toolStripStatusLabel2.Text == "")
                    {
                        toolStripStatusLabel2.Text += Path.GetFullPath(file);
                    }
                    isFileThere = true;
                    button1.Enabled = true;
                    label2.Text = "";

                    vc.LoadData(sourceFile);

                    //vc.path = tempFile;
                    //Console.WriteLine(GetFileHash(file));
                    textBox1.Text = GetFileHash(file);
                    textBox2.Text = GetFileHash(tempFile);

                    return;
                }
                else
                {
                    MessageBox.Show("Wrong file !", "VariantEditor");
                }
            }
        }

        private string GetFileHash(string fInfo)
        {
            HashAlgorithm hashAlgo = HashAlgorithm.Create();
            using (FileStream stream = new FileStream(fInfo, FileMode.Open, FileAccess.Read))
            {
                var hash = hashAlgo.ComputeHash(stream);
                hashAlgo.Dispose();
                string final = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
                return final;
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
                    // Console.WriteLine(GetFileHash(path));
                    if (toolStripStatusLabel2.Text == "")
                    {
                        toolStripStatusLabel2.Text += Path.GetFullPath(path);
                    }
                    label2.Text = "";
                    vc.LoadData(path);
                    isFileThere = true;
                    button1.Enabled = true;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isFileThere == true)
            {

                if (UnsavedChanges() == true)
                {

                    DialogResult dialogResult = MessageBox.Show("Do you want to save your changed data ?", "VariantEditor - Unsaved changes.", MessageBoxButtons.OKCancel);
                    if (dialogResult == DialogResult.OK)
                    {
                        Save();
                        Application.Exit();
                    }
                    else
                    {
                        ((FormClosingEventArgs)e).Cancel = true;
                    }
                }
            }
            Application.Exit();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isFileThere == true)
            {
                vc.SaveData(tempFile);
                //if (vc.hexStringOriginal == vc.hexStringEditet)
                //{
                //    Console.WriteLine("sdfvsdfadfvs");
                //}
                //textBox1.Text = GetFileHash(sourceFile);
                //textBox2.Text = GetFileHash(tempFile);
                if (UnsavedChanges() == true)
                {
                    DialogResult dialogResult = MessageBox.Show("Do you want to save your changed data ?", "VariantEditor - Unsaved changes.", MessageBoxButtons.YesNoCancel);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Save();
                    }
                    else if (dialogResult == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        //DeleteTempFile(tempFile);
                    }
                }
            }
            //DeleteTempFile(tempFile);
        }

        private bool UnsavedChanges()
        {
            string originalHash = GetFileHash(sourceFile);
            string tempHash = GetFileHash(tempFile);
            if (originalHash != tempHash)
            {
                return true;
            }
            return false;
        }
        private void Save()
        {
            //vc.SaveData(sourceFile);
            //string originalHash = GetFileHash(sourceFile);
            //string tempHash = GetFileHash(tempFile);
            //if (originalHash == tempHash)
            //{
            //    Console.WriteLine("Original - The same HASH");
            //}

            //if (UnsavedChanges() == true)
            //{
            vc.SaveData(sourceFile);
            //}

            //textBox1.Text = originalHash;
            //textBox2.Text = tempHash;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (isFileThere)
            {

                Save();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (vc is IDisposable)
            {
                ((IDisposable)vc).Dispose();
                if (vc.IsDisposed == true)
                {
                    fileName = null;
                    sourcePath = null;
                    sourceFile = null;
                    tempFile = null;

                    isFileThere = false;
                    panel1.Controls.Add(vc);
                    button1.Enabled = false;
                    //FormClosing += Form1_FormClosing;
                    //OpenExtension();
                }
            }

        }


    }
}
