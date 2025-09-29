namespace GoogleClashofClansLauncher
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.simulateButton = new System.Windows.Forms.Button();
            this.fixed123Button = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(44, 88);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(316, 27);
            this.inputTextBox.TabIndex = 0;
            this.inputTextBox.PlaceholderText = "请输入要模拟的键盘内容...";
            // 
            // simulateButton
            // 
            this.simulateButton.Location = new System.Drawing.Point(366, 86);
            this.simulateButton.Name = "simulateButton";
            this.simulateButton.Size = new System.Drawing.Size(94, 29);
            this.simulateButton.TabIndex = 1;
            this.simulateButton.Text = "模拟输入";
            this.simulateButton.UseVisualStyleBackColor = true;
            this.simulateButton.Click += new System.EventHandler(this.simulateButton_Click);
            // 
            // fixed123Button
            // 
            this.fixed123Button.Location = new System.Drawing.Point(466, 86);
            this.fixed123Button.Name = "fixed123Button";
            this.fixed123Button.Size = new System.Drawing.Size(94, 29);
            this.fixed123Button.TabIndex = 2;
            this.fixed123Button.Text = "固定输入123";
            this.fixed123Button.UseVisualStyleBackColor = true;
            this.fixed123Button.Click += new System.EventHandler(this.fixed123Button_Click);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.titleLabel.Location = new System.Drawing.Point(44, 30);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(264, 31);
            this.titleLabel.TabIndex = 3;
            this.titleLabel.Text = "键盘输入模拟器工具";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 151);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.fixed123Button);
            this.Controls.Add(this.simulateButton);
            this.Controls.Add(this.inputTextBox);
            this.Name = "Form1";
            this.Text = "键盘模拟器";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button simulateButton;
        private System.Windows.Forms.Button fixed123Button;
        private System.Windows.Forms.Label titleLabel;
    }
}
