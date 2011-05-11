using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace PcPv2
{
    // Callbacks to allow updating GUI during transfer or upon completion
    public delegate void ProgressDelegate(double pctComplete);
    public delegate void DoneDelegate();
    public delegate void StatusDelegate(string message);
    public delegate void ConfigureWorkFlowsDelegate(XmlDocument workflow_document);
    public delegate void ResponseDoneDelegate();
    public delegate void ExceptionAlertDelegate(Exception e, String message);
    public partial class InputForm : Form
    {
        private string filename;
        private string username;
        private string password;
        private bool authenticated = false;
        private string bridge_host = "bridge.cmich.edu";
        private bool shouldForceClose = false;
        private Bridge pcpBridge;
        UploadState state = null;
        public InputForm(string filename)
        {
            // build config path and test to see if the config exists
            string iniPath = System.IO.Path.Combine(Application.StartupPath, "PcPSubmit.ini");
            if (System.IO.File.Exists(iniPath))
            {
                // read in the config file
                // The only thing that can be in the config file is the host for the bridge.
                System.IO.StreamReader config = new System.IO.StreamReader(iniPath);
                bridge_host = config.ReadLine();
                config.Close();
            }

            
            this.filename = filename;
            this.pcpBridge = new Bridge(null, null, bridge_host);
            InitializeComponent();
        }

        private void about_Click(object sender, EventArgs e)
        {
            AboutBox dialog = new AboutBox(bridge_host);
            dialog.ShowDialog(this);
        }

        private void upload_click(object sender, EventArgs e)
        {
            if (filename.Equals(""))
            {
                OpenFileDialog fileChooser = new OpenFileDialog();
                fileChooser.Multiselect = false;
                if (fileChooser.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("User has cancelled the upload process, terminating program.");
                    forceClose();
                    return;
                }
                filename = fileChooser.FileName;
            }
            // Validate input, launch update 
            status_display.Text = "Uploading";
            string uuid = ((Workflow)workflow_combo.Items[workflow_combo.SelectedIndex]).uuid;
            string title = this.title.Text.Trim();
            string description = this.description.Text.Trim();

            Console.WriteLine(uuid);
            state = new UploadState();
            state.uploadDoneCB = this.DoneWithUpload;
            state.progCB = this.Progress;
            state.statusCB = this.ReportStatus;
            state.responseDoneCB = this.DoneWithUploadResponse;
            state.failureCB = this.ExceptionNotify;
            Console.WriteLine(this.filename);
            pcpBridge.sendFile(uuid, this.filename, title, description, state);
        }

        private void upload_watch_timer_Tick(object sender, EventArgs e)
        {
        }

        private void InputForm_Load(object sender, EventArgs e)
        {
            this.Show();
            this.Select();

            Thread t = new Thread(new ThreadStart(authenticate));
            t.Start();
            //authenticate();


        }
        private void authenticate()
        {
            try
            {
                XmlDocument workflow_document = new XmlDocument();
                while (!authenticated)
                {
                    ReportStatus("Getting Credentials");
                    Login prompt = new Login(System.Environment.UserName);
                    prompt.ShowDialog();

                    if (prompt.cancelled)
                    {
                        MessageBox.Show("User has cancelled the upload process, terminating program.");
                        forceClose();
                        return;
                    }

                    ReportStatus("Attempting Login...");

                    this.username = prompt.username;
                    this.password = prompt.password;
                    pcpBridge.Username = this.username;
                    pcpBridge.Password = this.password;
                    String response;

                    response = pcpBridge.retrieveWorkflows();

                    workflow_document.LoadXml(response);
                    XmlElement error = (XmlElement)workflow_document.SelectSingleNode("//div[@name='error']");
                    if (error == null)
                    {
                        ReportStatus("Authentication Succeeded");
                        authenticated = true;
                    }
                    else
                    {
                        ReportStatus("Authentication Error");
                        string message = "The error \"{0}\" was returned when you attempted to login.\n\rPress Ok to try logging in again, Cancel to quit.";

                        DialogResult choice = MessageBox.Show(String.Format(message, error.FirstChild.Value), "Login Problem", MessageBoxButtons.OKCancel);
                        if (choice == System.Windows.Forms.DialogResult.Cancel)
                        {
                            forceClose();
                            return;
                        }


                    }
                }
                loadWorkflow(workflow_document);
            }
            catch (WebException e)
            {
                ExceptionNotify(e, "Authentication Error");
            }
        }

        // invoked after a successful authentication
        private void loadWorkflow(XmlDocument workflow_document)
        {
            if (this.InvokeRequired)
            {
                ConfigureWorkFlowsDelegate del = new ConfigureWorkFlowsDelegate(loadWorkflow);
                this.Invoke(del, new object[] { workflow_document });
            }
            else
            {
                ReportStatus("Processing Workflows");
                XmlNode workflow = workflow_document.SelectSingleNode("//select[@name='workflow']");
                workflow_combo.Items.Clear();
                //workflow_combo.Items.Add(new Workflow("Please Choose Desired Workflow", null, "What workflow do you want ot submit your file to?"));
                List<Workflow> temp_workflows = new List<Workflow>();
                foreach (XmlElement item in workflow.ChildNodes)
                {
                    string uuid = item.GetAttribute("value");
                    string description = item.GetAttribute("title");
                    string name = item.FirstChild.Value;
                    temp_workflows.Add(new Workflow(name, uuid, description));
                    //workflow_combo.Items.Add(new Workflow(name, uuid, description));
                }

                // check that workflows exist
                if (temp_workflows.Count == 0)
                {
                    MessageBox.Show("There are no workflows to add this file to, please make sure you have access to workflows and try again.");
                    forceClose();
                    return;
                }
                else
                {
                    temp_workflows.Sort();

                    workflow_combo.Items.Add(new Workflow("Please Choose Desired Workflow", null, "What workflow do you want ot submit your file to?"));
                    workflow_combo.Items.AddRange(temp_workflows.ToArray());
                    workflow_combo.SelectedIndex = 0;
                    ReportStatus("Ready");
                    workflow_combo.Enabled = true;
                }
            }
        }
        // close the application without prompting the user
        private void forceClose()
        {
            if (this.InvokeRequired)
            {
                DoneDelegate d = new DoneDelegate(forceClose);
                this.Invoke(d);
            }
            else
            {
                shouldForceClose = true;
                Close();
                Application.Exit();
            }
        }

        private void InputForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!shouldForceClose)
            {
                DialogResult confirm = MessageBox.Show("This will cancel any pending file uploads, are you sure you wish to exit?", "PcP Submit", MessageBoxButtons.YesNo);
                if (confirm != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        private void uploadEnable(object sender, EventArgs e)
        {
            upload_button.Enabled = !String.IsNullOrEmpty(title.Text);
            upload_button.Enabled &= workflow_combo.SelectedIndex != 0;
        }

        public void ReportStatus(string message)
        {
            if (this.InvokeRequired)
            {
                StatusDelegate del = new StatusDelegate(ReportStatus);
                this.Invoke(del, new object[] { message });
            }
            else
            {
                this.status_display.Text = message;
            }
        }
        public void Progress(double pctComplete)
        {
            if (this.InvokeRequired)
            {
                ProgressDelegate del = new ProgressDelegate(Progress);
                this.Invoke(del, new object[] { pctComplete });
            }
            else
            {
                progressbar.Value = (int)pctComplete;
            }
        }

        public void ExceptionNotify(Exception e, string message)
        {

            if (this.InvokeRequired)
            {
                ExceptionAlertDelegate del = new ExceptionAlertDelegate(ExceptionNotify);
                this.Invoke(del, new object[] { e, message });
            }
            else
            {
                DialogResult choice = MessageBox.Show(e.Message, message, MessageBoxButtons.RetryCancel);
                if (choice == DialogResult.Cancel)
                {
                    forceClose();
                    return;
                }
                else {
                    return;
                }
            }
        }
        public void DoneWithUpload()
        {
            if (this.InvokeRequired)
            {
                DoneDelegate del = new DoneDelegate(DoneWithUpload);
                this.Invoke(del, new object[] { });
            }
            else
            {
                progressbar.Value = 0;
                status_display.Text = "Upload Complete, Bridge Submitting File";
                progressbar.Style = ProgressBarStyle.Marquee;
            }

        }

        public void DoneWithUploadResponse()
        {
            if (this.InvokeRequired)
            {
                ResponseDoneDelegate del = new ResponseDoneDelegate(DoneWithUploadResponse);
                this.Invoke(del);
            }
            else
            {
                progressbar.Value = 0;
                status_display.Text = "Ready";
                progressbar.Style = ProgressBarStyle.Continuous;
                // This can be false now that the upload is completed.
                this.shouldForceClose = false;

                //string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //StringBuilder sb = new StringBuilder();
                //sb.Append(this.state.response);
                //using (StreamWriter outfile = new StreamWriter(mydocpath + @"\result.txt"))
                //{
                //    outfile.Write(sb.ToString());
                //}

                XmlDocument response_document = new XmlDocument();
                response_document.LoadXml(this.state.response);
                XmlElement status = (XmlElement)response_document.SelectSingleNode("//div[@name='status']");
                if (status == null)
                {
                    string message = "There was a server error submitting your file.\n\rPress Ok to try again, Cancel to quit.";
                    DialogResult choice = MessageBox.Show(message, "Upload Problem", MessageBoxButtons.OKCancel);
                    if (choice == System.Windows.Forms.DialogResult.Cancel)
                    {
                        forceClose();
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                if (status.GetAttribute("title").Equals("0"))
                {
                    MessageBox.Show("Upload succeeded");
                    forceClose();
                    return;
                }
                else
                {
                    string message = "The error \"{0}\" was returned when you attempted to submit your podcast.\n\rPress Ok to try again, Cancel to quit.";

                    DialogResult choice = MessageBox.Show(String.Format(message, status.FirstChild.Value), "Upload Problem", MessageBoxButtons.OKCancel);
                    if (choice == System.Windows.Forms.DialogResult.Cancel)
                    {
                        forceClose();
                        return;
                    }
                    else
                    {

                        return;
                    }

                }

            }

        }
    }
}
