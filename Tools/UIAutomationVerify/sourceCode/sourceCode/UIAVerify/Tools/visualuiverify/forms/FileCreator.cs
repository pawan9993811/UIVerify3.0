using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VisualUIAVerify.Controls;

namespace VisualUIAVerify.Forms
{
    public partial class FileCreator : Form
    {
        string fileDirPath = @"C:\CFW";
        public string xmlLocation;
        string strSearchDirectory;
        string txtFileLocation = null;
        string txtFileName = null;
        private const string notAllowedChar = @"/';}{~`!^*?<>|""";

        public FileCreator()
        {
            InitializeComponent();
            FileLocation.Text = fileDirPath;
        }

        public string NewFileLocation
        {
            get { return xmlLocation; }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FileName.Text) || string.IsNullOrEmpty(FileLocation.Text))
            {
                MessageBox.Show("File Name and Folder Location should not be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (!ValidateFileName(FileName.Text) || !TryGetFullPath(FileLocation.Text))
            {
                if(!ValidateFileName(FileName.Text))
                    MessageBox.Show("Invalid file name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if(!TryGetFullPath(FileLocation.Text))
                    MessageBox.Show("Invalid folder path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string folderName = string.Concat(FileLocation.Text.Substring(FileLocation.Text.LastIndexOf("\\"),
                FileLocation.Text.Length - FileLocation.Text.LastIndexOf("\\")).Replace(@"\", ""));

            FileLocation.Text = Path.GetFullPath(FileLocation.Text);

            if (FileLocation.Text == fileDirPath)
            {
                DialogResult dr = MessageBox.Show("Folder Name: '" + folderName + "' already exists. \nClick OK to enter File Location", "Error", MessageBoxButtons.OK);
                FileLocation.Focus();
                return;
            }
          
            try
            {
                //if (!ValidateFileName(FileName.Text))
                //{
                //    MessageBox.Show("Invalid file name .Expected file name [Examlple : fileName.xml]", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                // check File not created
                bool fileExists = File.Exists(FileLocation.Text + @"\" + FileName.Text);
                bool folderExists = Directory.Exists(FileLocation.Text);
                if (fileExists)
                {
                    MessageBox.Show("Folder &File already Exists", "Warining",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Exclamation,
                                  MessageBoxDefaultButton.Button1);
                    xmlLocation = FileLocation.Text + "\\" + FileName.Text;
                    return;
                }
                else if (folderExists)
                {
                    DialogResult dr = MessageBox.Show("Folder already exists but file not exists.\nDo you want to create xml file as folder already exists? ", "Warining",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Exclamation,
                                  MessageBoxDefaultButton.Button1);

                    if (dr == DialogResult.Yes)
                    {
                        File.Create(FileLocation.Text + "\\" + FileName.Text);
                        //Strore file location
                        xmlLocation = FileLocation.Text + "\\" + FileName.Text;
                        DialogResult d = MessageBox.Show("File creation successful. File Name: " + string.Concat(FileLocation.Text, @"\", FileName.Text), "Message", MessageBoxButtons.OK);
                        if (d == DialogResult.OK)
                        {
                            this.Close();
                        }
                    }
                    else if (dr == DialogResult.No)
                    {
                        FileLocation.Focus();
                    }
                }
                else
                {
                    Directory.CreateDirectory(FileLocation.Text);
                    //create xml file 
                    File.Create(FileLocation.Text + "\\" + FileName.Text);
                    //Strore file location
                    xmlLocation = FileLocation.Text + "\\" + FileName.Text;
                    DialogResult d = MessageBox.Show("'File Created Successfully:' " + string.Concat(FileLocation.Text, @"\", FileName.Text), "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (d == DialogResult.OK)
                    {
                        this.Close();
                    }
                }
            }
            catch
            {
                
                MessageBox.Show("Error Cannot Access Location", "Warining",
                                  MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1);
            }
            AutomationElementTreeControl aetc = new AutomationElementTreeControl();
            aetc.FileName = NewFileLocation;
        }

        public static bool ValidateFileName(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName)) { return false; }
            if (fileName.Contains(" ")) { return false; }
            string temp = notAllowedChar + ".,+_)(*&^%$#@!~";
            bool flag = true;
            string extention = Path.GetExtension(fileName);
            string fName = Path.GetFileNameWithoutExtension(fileName);
            try
            {
                flag &= extention != "";
                flag &= extention.ToUpper().Equals(".XML");
                flag &= !temp.Any(s => fName.Contains(s));
                flag &= !string.IsNullOrEmpty(fName);
                return flag;
            }
            catch { return false; }
        }

        private static bool TryGetFullPath(string path, bool exactPath = true)
        {
            if (String.IsNullOrWhiteSpace(path)) { return false; }
            if (path.Contains(" ")) { return false; }
            bool isValid = true;
            try
            {
                string fullPath = Path.GetFullPath(path);
                if (exactPath)
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
                else
                {
                    isValid = Path.IsPathRooted(path);
                }
            }
            catch (Exception)
            {
                isValid = false;
            }
            return isValid;
        }
        public void btnBrowser_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(fileDirPath))
                {
                    Directory.CreateDirectory(fileDirPath);
                }
                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                folderBrowser.SelectedPath = fileDirPath;
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    strSearchDirectory = folderBrowser.SelectedPath;
                    FileLocation.Text = strSearchDirectory;
                }
            }
            catch
            {
                MessageBox.Show("Error Cannot Access Location", "Warining",
                                   MessageBoxButtons.OK,
                                  MessageBoxIcon.Exclamation,
                                  MessageBoxDefaultButton.Button1);
            }
        }

        private void FileLocation_Enter(object sender, EventArgs e)
        {
            txtFileLocation = FileLocation.Text;
        }

        private void FileName_Enter(object sender, EventArgs e)
        {
            txtFileName = FileName.Text;
        }

        private void FileName_Leave(object sender, EventArgs e)
        {
            if (notAllowedChar.Any(s => FileName.Text.Contains(s)))
            {
                MessageBox.Show("A file name cannot contain any of the following characters " + @"/';}{~`!^*?<>|""",
                                    "Error",
                                   MessageBoxButtons.OK,
                                  MessageBoxIcon.Error,
                                  MessageBoxDefaultButton.Button3);
                FileName.Text = txtFileName;
            }
            else if (!ValidateFileName(FileName.Text))
            {
                MessageBox.Show("Invalid file name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                var vf = FileLocation.Text.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                if (vf[vf.Length - 1].Equals(Path.GetFileNameWithoutExtension(FileName.Text)))
                    return;
                FileLocation.Text = FileLocation.Text + @"\" + Path.GetFileNameWithoutExtension(FileName.Text);
                txtFileName = FileName.Text;
                txtFileLocation = FileLocation.Text;
            }
        }
    }
}
