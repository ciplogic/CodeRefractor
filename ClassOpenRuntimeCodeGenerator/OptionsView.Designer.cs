namespace ClassOpenRuntimeCodeGenerator
{
    partial class OptionsView
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
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxPure = new System.Windows.Forms.CheckBox();
            this.cbxCppMethod = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(321, 140);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "&Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cbxPure);
            this.groupBox1.Controls.Add(this.cbxCppMethod);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(383, 122);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // cbxPure
            // 
            this.cbxPure.AutoSize = true;
            this.cbxPure.Location = new System.Drawing.Point(6, 42);
            this.cbxPure.Name = "cbxPure";
            this.cbxPure.Size = new System.Drawing.Size(48, 17);
            this.cbxPure.TabIndex = 2;
            this.cbxPure.Text = "&Pure";
            this.cbxPure.UseVisualStyleBackColor = true;
            // 
            // cbxCppMethod
            // 
            this.cbxCppMethod.AutoSize = true;
            this.cbxCppMethod.Checked = true;
            this.cbxCppMethod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxCppMethod.Location = new System.Drawing.Point(6, 19);
            this.cbxCppMethod.Name = "cbxCppMethod";
            this.cbxCppMethod.Size = new System.Drawing.Size(86, 17);
            this.cbxCppMethod.TabIndex = 1;
            this.cbxCppMethod.Text = "&C++ function";
            this.cbxCppMethod.UseVisualStyleBackColor = true;
            // 
            // OptionsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 175);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Name = "OptionsView";
            this.Text = "OptionsView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OptionsView_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbxPure;
        private System.Windows.Forms.CheckBox cbxCppMethod;
    }
}