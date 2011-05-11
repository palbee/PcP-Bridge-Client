namespace PcPv2
{
    partial class InputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputForm));
            this.workflow_combo = new System.Windows.Forms.ComboBox();
            this.title = new System.Windows.Forms.TextBox();
            this.description = new System.Windows.Forms.TextBox();
            this.workflow_label = new System.Windows.Forms.Label();
            this.title_label = new System.Windows.Forms.Label();
            this.description_label = new System.Windows.Forms.Label();
            this.about_button = new System.Windows.Forms.Button();
            this.upload_button = new System.Windows.Forms.Button();
            this.progressbar = new System.Windows.Forms.ProgressBar();
            this.status_display = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // workflow_combo
            // 
            this.workflow_combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.workflow_combo.FormattingEnabled = true;
            this.workflow_combo.Location = new System.Drawing.Point(78, 12);
            this.workflow_combo.Name = "workflow_combo";
            this.workflow_combo.Size = new System.Drawing.Size(333, 21);
            this.workflow_combo.TabIndex = 0;
            this.workflow_combo.SelectionChangeCommitted += new System.EventHandler(this.uploadEnable);
            // 
            // title
            // 
            this.title.Location = new System.Drawing.Point(78, 39);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(333, 20);
            this.title.TabIndex = 1;
            this.title.TextChanged += new System.EventHandler(this.uploadEnable);
            // 
            // description
            // 
            this.description.Location = new System.Drawing.Point(78, 65);
            this.description.Multiline = true;
            this.description.Name = "description";
            this.description.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.description.Size = new System.Drawing.Size(333, 69);
            this.description.TabIndex = 2;
            // 
            // workflow_label
            // 
            this.workflow_label.AutoSize = true;
            this.workflow_label.Location = new System.Drawing.Point(20, 15);
            this.workflow_label.Name = "workflow_label";
            this.workflow_label.Size = new System.Drawing.Size(52, 13);
            this.workflow_label.TabIndex = 3;
            this.workflow_label.Text = "Workflow";
            // 
            // title_label
            // 
            this.title_label.AutoSize = true;
            this.title_label.Location = new System.Drawing.Point(45, 42);
            this.title_label.Name = "title_label";
            this.title_label.Size = new System.Drawing.Size(27, 13);
            this.title_label.TabIndex = 4;
            this.title_label.Text = "Title";
            // 
            // description_label
            // 
            this.description_label.AutoSize = true;
            this.description_label.Location = new System.Drawing.Point(12, 68);
            this.description_label.Name = "description_label";
            this.description_label.Size = new System.Drawing.Size(60, 13);
            this.description_label.TabIndex = 5;
            this.description_label.Text = "Description";
            // 
            // about_button
            // 
            this.about_button.Location = new System.Drawing.Point(254, 140);
            this.about_button.Name = "about_button";
            this.about_button.Size = new System.Drawing.Size(76, 23);
            this.about_button.TabIndex = 7;
            this.about_button.Text = "About";
            this.about_button.UseVisualStyleBackColor = true;
            this.about_button.Click += new System.EventHandler(this.about_Click);
            // 
            // upload_button
            // 
            this.upload_button.Enabled = false;
            this.upload_button.Location = new System.Drawing.Point(336, 140);
            this.upload_button.Name = "upload_button";
            this.upload_button.Size = new System.Drawing.Size(75, 23);
            this.upload_button.TabIndex = 6;
            this.upload_button.Text = "Upload";
            this.upload_button.UseVisualStyleBackColor = true;
            this.upload_button.Click += new System.EventHandler(this.upload_click);
            // 
            // progressbar
            // 
            this.progressbar.Location = new System.Drawing.Point(12, 169);
            this.progressbar.Name = "progressbar";
            this.progressbar.Size = new System.Drawing.Size(399, 22);
            this.progressbar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressbar.TabIndex = 8;
            // 
            // status_display
            // 
            this.status_display.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.status_display.Location = new System.Drawing.Point(19, 138);
            this.status_display.Name = "status_display";
            this.status_display.Size = new System.Drawing.Size(229, 25);
            this.status_display.TabIndex = 9;
            this.status_display.Text = "Status Display";
            this.status_display.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 203);
            this.Controls.Add(this.status_display);
            this.Controls.Add(this.progressbar);
            this.Controls.Add(this.upload_button);
            this.Controls.Add(this.about_button);
            this.Controls.Add(this.description_label);
            this.Controls.Add(this.title_label);
            this.Controls.Add(this.workflow_label);
            this.Controls.Add(this.description);
            this.Controls.Add(this.title);
            this.Controls.Add(this.workflow_combo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputForm";
            this.Text = "PCP Submit V2.0";
            this.Load += new System.EventHandler(this.InputForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InputForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox workflow_combo;
        private System.Windows.Forms.TextBox title;
        private System.Windows.Forms.TextBox description;
        private System.Windows.Forms.Label workflow_label;
        private System.Windows.Forms.Label title_label;
        private System.Windows.Forms.Label description_label;
        private System.Windows.Forms.Button about_button;
        private System.Windows.Forms.Button upload_button;
        private System.Windows.Forms.ProgressBar progressbar;
        private System.Windows.Forms.Label status_display;
    }
}