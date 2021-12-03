namespace CityGMLExporter
{
    partial class Form1
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.inputBrowser = new System.Windows.Forms.Button();
            this.outputBrowser = new System.Windows.Forms.Button();
            this.exportBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(145, 33);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(475, 22);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "D:\\Share data\\KICT.json";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(145, 65);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(475, 22);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = "D:\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Input Json";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 69);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "CityGML output";
            // 
            // inputBrowser
            // 
            this.inputBrowser.Location = new System.Drawing.Point(629, 33);
            this.inputBrowser.Margin = new System.Windows.Forms.Padding(4);
            this.inputBrowser.Name = "inputBrowser";
            this.inputBrowser.Size = new System.Drawing.Size(56, 25);
            this.inputBrowser.TabIndex = 4;
            this.inputBrowser.Text = "...";
            this.inputBrowser.UseVisualStyleBackColor = true;
            this.inputBrowser.Click += new System.EventHandler(this.InputBrowser_Click);
            // 
            // outputBrowser
            // 
            this.outputBrowser.Location = new System.Drawing.Point(629, 65);
            this.outputBrowser.Margin = new System.Windows.Forms.Padding(4);
            this.outputBrowser.Name = "outputBrowser";
            this.outputBrowser.Size = new System.Drawing.Size(56, 25);
            this.outputBrowser.TabIndex = 5;
            this.outputBrowser.Text = "...";
            this.outputBrowser.UseVisualStyleBackColor = true;
            this.outputBrowser.Click += new System.EventHandler(this.OutputBrowser_Click);
            // 
            // exportBtn
            // 
            this.exportBtn.Location = new System.Drawing.Point(332, 97);
            this.exportBtn.Margin = new System.Windows.Forms.Padding(4);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(100, 28);
            this.exportBtn.TabIndex = 6;
            this.exportBtn.Text = "Export";
            this.exportBtn.UseVisualStyleBackColor = true;
            this.exportBtn.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 150);
            this.Controls.Add(this.exportBtn);
            this.Controls.Add(this.outputBrowser);
            this.Controls.Add(this.inputBrowser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button inputBrowser;
        private System.Windows.Forms.Button outputBrowser;
        private System.Windows.Forms.Button exportBtn;
    }
}

