namespace GoogleClashofClansLauncher
{
    partial class SettingsForm
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
            this.settingsRecognitionButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.apiEndpointLabel = new System.Windows.Forms.Label();
            this.apiEndpointTextBox = new System.Windows.Forms.TextBox();
            this.apiKeyLabel = new System.Windows.Forms.Label();
            this.apiKeyTextBox = new System.Windows.Forms.TextBox();
            this.saveSettingsButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.apiSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.panel1.SuspendLayout();
            this.apiSettingsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsRecognitionButton
            // 
            this.settingsRecognitionButton.Location = new System.Drawing.Point(44, 88);
            this.settingsRecognitionButton.Name = "settingsRecognitionButton";
            this.settingsRecognitionButton.Size = new System.Drawing.Size(516, 29);
            this.settingsRecognitionButton.TabIndex = 0;
            this.settingsRecognitionButton.Text = "识别设置图像并点击 (res/2/002.png)";
            this.settingsRecognitionButton.UseVisualStyleBackColor = true;
            this.settingsRecognitionButton.Click += new System.EventHandler(this.settingsRecognitionButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(44, 120);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(45, 20);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "就绪";
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.titleLabel.Location = new System.Drawing.Point(44, 30);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(169, 31);
            this.titleLabel.TabIndex = 2;
            this.titleLabel.Text = "设置功能界面";
            // 
            // apiSettingsGroupBox
            // 
            this.apiSettingsGroupBox.Controls.Add(this.apiKeyTextBox);
            this.apiSettingsGroupBox.Controls.Add(this.apiKeyLabel);
            this.apiSettingsGroupBox.Controls.Add(this.apiEndpointTextBox);
            this.apiSettingsGroupBox.Controls.Add(this.apiEndpointLabel);
            this.apiSettingsGroupBox.Controls.Add(this.saveSettingsButton);
            this.apiSettingsGroupBox.Location = new System.Drawing.Point(44, 88);
            this.apiSettingsGroupBox.Name = "apiSettingsGroupBox";
            this.apiSettingsGroupBox.Size = new System.Drawing.Size(516, 180);
            this.apiSettingsGroupBox.TabIndex = 3;
            this.apiSettingsGroupBox.TabStop = false;
            this.apiSettingsGroupBox.Text = "AI API 设置";
            // 
            // apiEndpointLabel
            // 
            this.apiEndpointLabel.AutoSize = true;
            this.apiEndpointLabel.Location = new System.Drawing.Point(20, 40);
            this.apiEndpointLabel.Name = "apiEndpointLabel";
            this.apiEndpointLabel.Size = new System.Drawing.Size(93, 20);
            this.apiEndpointLabel.TabIndex = 0;
            this.apiEndpointLabel.Text = "API 接口地址";
            // 
            // apiEndpointTextBox
            // 
            this.apiEndpointTextBox.Location = new System.Drawing.Point(120, 37);
            this.apiEndpointTextBox.Name = "apiEndpointTextBox";
            this.apiEndpointTextBox.Size = new System.Drawing.Size(370, 27);
            this.apiEndpointTextBox.TabIndex = 1;
            // 
            // apiKeyLabel
            // 
            this.apiKeyLabel.AutoSize = true;
            this.apiKeyLabel.Location = new System.Drawing.Point(20, 80);
            this.apiKeyLabel.Name = "apiKeyLabel";
            this.apiKeyLabel.Size = new System.Drawing.Size(83, 20);
            this.apiKeyLabel.TabIndex = 2;
            this.apiKeyLabel.Text = "API 密钥";
            // 
            // apiKeyTextBox
            // 
            this.apiKeyTextBox.Location = new System.Drawing.Point(120, 77);
            this.apiKeyTextBox.Name = "apiKeyTextBox";
            this.apiKeyTextBox.PasswordChar = '*';
            this.apiKeyTextBox.Size = new System.Drawing.Size(370, 27);
            this.apiKeyTextBox.TabIndex = 3;
            // 
            // saveSettingsButton
            // 
            this.saveSettingsButton.Location = new System.Drawing.Point(405, 120);
            this.saveSettingsButton.Name = "saveSettingsButton";
            this.saveSettingsButton.Size = new System.Drawing.Size(85, 30);
            this.saveSettingsButton.TabIndex = 4;
            this.saveSettingsButton.Text = "保存设置";
            this.saveSettingsButton.UseVisualStyleBackColor = true;
            this.saveSettingsButton.Click += new System.EventHandler(this.saveSettingsButton_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 320);
            this.Controls.Add(this.apiSettingsGroupBox);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.settingsRecognitionButton);
            this.Name = "SettingsForm";
            this.Text = "设置功能";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.apiSettingsGroupBox.ResumeLayout(false);
            this.apiSettingsGroupBox.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button settingsRecognitionButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label apiEndpointLabel;
        private System.Windows.Forms.TextBox apiEndpointTextBox;
        private System.Windows.Forms.Label apiKeyLabel;
        private System.Windows.Forms.TextBox apiKeyTextBox;
        private System.Windows.Forms.Button saveSettingsButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox apiSettingsGroupBox;
    }
}