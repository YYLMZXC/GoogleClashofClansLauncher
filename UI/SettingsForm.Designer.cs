namespace GoogleClashofClansLauncher.UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.apiSettingsTabControl = new System.Windows.Forms.TabControl();
            this.apiSettingsTabPage = new System.Windows.Forms.TabPage();
            this.apiSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.deleteApiButton = new System.Windows.Forms.Button();
            this.editApiButton = new System.Windows.Forms.Button();
            this.apiTypeLabel = new System.Windows.Forms.Label();
            this.saveSettingsButton = new System.Windows.Forms.Button();
            this.customApiNameTextBox = new System.Windows.Forms.TextBox();
            this.customApiNameLabel = new System.Windows.Forms.Label();
            this.apiKeyTextBox = new System.Windows.Forms.TextBox();
            this.apiKeyLabel = new System.Windows.Forms.Label();
            this.apiEndpointTextBox = new System.Windows.Forms.TextBox();
            this.apiEndpointLabel = new System.Windows.Forms.Label();
            this.apiComboBox = new System.Windows.Forms.ComboBox();
            this.apiComboBoxLabel = new System.Windows.Forms.Label();
            this.addCustomApiButton = new System.Windows.Forms.Button();
            this.appSettingsTabPage = new System.Windows.Forms.TabPage();
            this.appSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.resetSettingsButton = new System.Windows.Forms.Button();
            this.autoStartCheckBox = new System.Windows.Forms.CheckBox();
            this.resetSettingsWarningLabel = new System.Windows.Forms.Label();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.statusStrip.SuspendLayout();
            this.apiSettingsTabControl.SuspendLayout();
            this.apiSettingsTabPage.SuspendLayout();
            this.apiSettingsGroupBox.SuspendLayout();
            this.appSettingsTabPage.SuspendLayout();
            this.appSettingsGroupBox.SuspendLayout();
            this.footerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 459);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(700, 26);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(40, 20);
            this.statusLabel.Text = "就绪";
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.titleLabel.Location = new System.Drawing.Point(25, 20);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(174, 31);
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = "应用程序设置";
            // 
            // apiSettingsTabControl
            // 
            this.apiSettingsTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.apiSettingsTabControl.Controls.Add(this.apiSettingsTabPage);
            this.apiSettingsTabControl.Controls.Add(this.appSettingsTabPage);
            this.apiSettingsTabControl.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.apiSettingsTabControl.ItemSize = new System.Drawing.Size(100, 26);
            this.apiSettingsTabControl.Location = new System.Drawing.Point(30, 60);
            this.apiSettingsTabControl.Name = "apiSettingsTabControl";
            this.apiSettingsTabControl.SelectedIndex = 0;
            this.apiSettingsTabControl.Size = new System.Drawing.Size(640, 380);
            this.apiSettingsTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.apiSettingsTabControl.TabIndex = 2;
            // 
            // apiSettingsTabPage
            // 
            this.apiSettingsTabPage.Controls.Add(this.apiSettingsGroupBox);
            this.apiSettingsTabPage.Location = new System.Drawing.Point(4, 30);
            this.apiSettingsTabPage.Name = "apiSettingsTabPage";
            this.apiSettingsTabPage.Padding = new System.Windows.Forms.Padding(10);
            this.apiSettingsTabPage.Size = new System.Drawing.Size(632, 346);
            this.apiSettingsTabPage.TabIndex = 0;
            this.apiSettingsTabPage.Text = "AI API 设置";
            this.apiSettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // apiSettingsGroupBox
            // 
            this.apiSettingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.apiSettingsGroupBox.Controls.Add(this.deleteApiButton);
            this.apiSettingsGroupBox.Controls.Add(this.editApiButton);
            this.apiSettingsGroupBox.Controls.Add(this.apiTypeLabel);
            this.apiSettingsGroupBox.Controls.Add(this.saveSettingsButton);
            this.apiSettingsGroupBox.Controls.Add(this.customApiNameTextBox);
            this.apiSettingsGroupBox.Controls.Add(this.customApiNameLabel);
            this.apiSettingsGroupBox.Controls.Add(this.apiKeyTextBox);
            this.apiSettingsGroupBox.Controls.Add(this.apiKeyLabel);
            this.apiSettingsGroupBox.Controls.Add(this.apiEndpointTextBox);
            this.apiSettingsGroupBox.Controls.Add(this.apiEndpointLabel);
            this.apiSettingsGroupBox.Controls.Add(this.apiComboBox);
            this.apiSettingsGroupBox.Controls.Add(this.apiComboBoxLabel);
            this.apiSettingsGroupBox.Controls.Add(this.addCustomApiButton);
            this.apiSettingsGroupBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.apiSettingsGroupBox.Location = new System.Drawing.Point(10, 10);
            this.apiSettingsGroupBox.Name = "apiSettingsGroupBox";
            this.apiSettingsGroupBox.Padding = new System.Windows.Forms.Padding(15);
            this.apiSettingsGroupBox.Size = new System.Drawing.Size(610, 320);
            this.apiSettingsGroupBox.TabIndex = 0;
            this.apiSettingsGroupBox.TabStop = false;
            this.apiSettingsGroupBox.Text = "AI API 配置";
            // 
            // saveSettingsButton
            // 
            this.saveSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveSettingsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.saveSettingsButton.FlatAppearance.BorderSize = 0;
            this.saveSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveSettingsButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.saveSettingsButton.ForeColor = System.Drawing.Color.White;
            this.saveSettingsButton.Location = new System.Drawing.Point(480, 260);
            this.saveSettingsButton.Name = "saveSettingsButton";
            this.saveSettingsButton.Size = new System.Drawing.Size(100, 35);
            this.saveSettingsButton.TabIndex = 9;
            this.saveSettingsButton.Text = "保存设置";
            this.saveSettingsButton.UseVisualStyleBackColor = false;
            this.saveSettingsButton.Click += new System.EventHandler(this.SaveSettingsButton_Click);
            // 
            // apiTypeLabel
            // 
            this.apiTypeLabel.AutoSize = true;
            this.apiTypeLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.apiTypeLabel.ForeColor = System.Drawing.Color.Green;
            this.apiTypeLabel.Location = new System.Drawing.Point(480, 23);
            this.apiTypeLabel.Name = "apiTypeLabel";
            this.apiTypeLabel.Size = new System.Drawing.Size(93, 20);
            this.apiTypeLabel.TabIndex = 11;
            this.apiTypeLabel.Text = "官方API";
            // 
            // editApiButton
            // 
            this.editApiButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editApiButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(215)))), ((int)(((byte)(0)))));
            this.editApiButton.FlatAppearance.BorderSize = 0;
            this.editApiButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editApiButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.editApiButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editApiButton.Location = new System.Drawing.Point(480, 170);
            this.editApiButton.Name = "editApiButton";
            this.editApiButton.Size = new System.Drawing.Size(100, 28);
            this.editApiButton.TabIndex = 7;
            this.editApiButton.Text = "编辑API";
            this.editApiButton.UseVisualStyleBackColor = false;
            this.editApiButton.Click += new System.EventHandler(this.EditApiButton_Click);
            // 
            // deleteApiButton
            // 
            this.deleteApiButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteApiButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(105)))), ((int)(((byte)(180)))));
            this.deleteApiButton.FlatAppearance.BorderSize = 0;
            this.deleteApiButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteApiButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.deleteApiButton.ForeColor = System.Drawing.Color.White;
            this.deleteApiButton.Location = new System.Drawing.Point(480, 200);
            this.deleteApiButton.Name = "deleteApiButton";
            this.deleteApiButton.Size = new System.Drawing.Size(100, 28);
            this.deleteApiButton.TabIndex = 6;
            this.deleteApiButton.Text = "删除API";
            this.deleteApiButton.UseVisualStyleBackColor = false;
            this.deleteApiButton.Click += new System.EventHandler(this.DeleteApiButton_Click);
            // 
            // customApiNameTextBox
            // 
            this.customApiNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customApiNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.customApiNameTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.customApiNameTextBox.Location = new System.Drawing.Point(120, 220);
            this.customApiNameTextBox.Name = "customApiNameTextBox";
            this.customApiNameTextBox.Size = new System.Drawing.Size(350, 27);
            this.customApiNameTextBox.TabIndex = 8;
            // 
            // customApiNameLabel
            // 
            this.customApiNameLabel.AutoSize = true;
            this.customApiNameLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.customApiNameLabel.Location = new System.Drawing.Point(15, 223);
            this.customApiNameLabel.Name = "customApiNameLabel";
            this.customApiNameLabel.Size = new System.Drawing.Size(93, 20);
            this.customApiNameLabel.TabIndex = 7;
            this.customApiNameLabel.Text = "自定义名称";
            // 
            // apiKeyTextBox
            // 
            this.apiKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.apiKeyTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.apiKeyTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.apiKeyTextBox.Location = new System.Drawing.Point(120, 120);
            this.apiKeyTextBox.Name = "apiKeyTextBox";
            this.apiKeyTextBox.PasswordChar = '*';
            this.apiKeyTextBox.Size = new System.Drawing.Size(460, 27);
            this.apiKeyTextBox.TabIndex = 4;
            // 
            // apiKeyLabel
            // 
            this.apiKeyLabel.AutoSize = true;
            this.apiKeyLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.apiKeyLabel.Location = new System.Drawing.Point(15, 123);
            this.apiKeyLabel.Name = "apiKeyLabel";
            this.apiKeyLabel.Size = new System.Drawing.Size(83, 20);
            this.apiKeyLabel.TabIndex = 3;
            this.apiKeyLabel.Text = "API 密钥";
            // 
            // apiEndpointTextBox
            // 
            this.apiEndpointTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.apiEndpointTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.apiEndpointTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.apiEndpointTextBox.Location = new System.Drawing.Point(120, 70);
            this.apiEndpointTextBox.Name = "apiEndpointTextBox";
            this.apiEndpointTextBox.Size = new System.Drawing.Size(460, 27);
            this.apiEndpointTextBox.TabIndex = 2;
            // 
            // apiEndpointLabel
            // 
            this.apiEndpointLabel.AutoSize = true;
            this.apiEndpointLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.apiEndpointLabel.Location = new System.Drawing.Point(15, 73);
            this.apiEndpointLabel.Name = "apiEndpointLabel";
            this.apiEndpointLabel.Size = new System.Drawing.Size(93, 20);
            this.apiEndpointLabel.TabIndex = 1;
            this.apiEndpointLabel.Text = "API 接口地址";
            // 
            // apiComboBox
            // 
            this.apiComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.apiComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.apiComboBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.apiComboBox.FormattingEnabled = true;
            this.apiComboBox.Location = new System.Drawing.Point(120, 20);
            this.apiComboBox.Name = "apiComboBox";
            this.apiComboBox.Size = new System.Drawing.Size(350, 28);
            this.apiComboBox.TabIndex = 0;
            this.apiComboBox.SelectedIndexChanged += new System.EventHandler(this.ApiComboBox_SelectedIndexChanged);
            // 
            // apiComboBoxLabel
            // 
            this.apiComboBoxLabel.AutoSize = true;
            this.apiComboBoxLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.apiComboBoxLabel.Location = new System.Drawing.Point(15, 23);
            this.apiComboBoxLabel.Name = "apiComboBoxLabel";
            this.apiComboBoxLabel.Size = new System.Drawing.Size(83, 20);
            this.apiComboBoxLabel.TabIndex = 0;
            this.apiComboBoxLabel.Text = "选择API";
            // 
            // addCustomApiButton
            // 
            this.addCustomApiButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addCustomApiButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(238)))), ((int)(((byte)(144)))));
            this.addCustomApiButton.FlatAppearance.BorderSize = 0;
            this.addCustomApiButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addCustomApiButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.addCustomApiButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.addCustomApiButton.Location = new System.Drawing.Point(480, 20);
            this.addCustomApiButton.Name = "addCustomApiButton";
            this.addCustomApiButton.Size = new System.Drawing.Size(100, 28);
            this.addCustomApiButton.TabIndex = 1;
            this.addCustomApiButton.Text = "添加自定义";
            this.addCustomApiButton.UseVisualStyleBackColor = false;
            this.addCustomApiButton.Click += new System.EventHandler(this.AddCustomApiButton_Click);
            // 
            // appSettingsTabPage
            // 
            this.appSettingsTabPage.Controls.Add(this.appSettingsGroupBox);
            this.appSettingsTabPage.Location = new System.Drawing.Point(4, 30);
            this.appSettingsTabPage.Name = "appSettingsTabPage";
            this.appSettingsTabPage.Padding = new System.Windows.Forms.Padding(10);
            this.appSettingsTabPage.Size = new System.Drawing.Size(632, 346);
            this.appSettingsTabPage.TabIndex = 1;
            this.appSettingsTabPage.Text = "应用设置";
            this.appSettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // appSettingsGroupBox
            // 
            this.appSettingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appSettingsGroupBox.Controls.Add(this.resetSettingsButton);
            this.appSettingsGroupBox.Controls.Add(this.autoStartCheckBox);
            this.appSettingsGroupBox.Controls.Add(this.resetSettingsWarningLabel);
            this.appSettingsGroupBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.appSettingsGroupBox.Location = new System.Drawing.Point(10, 10);
            this.appSettingsGroupBox.Name = "appSettingsGroupBox";
            this.appSettingsGroupBox.Padding = new System.Windows.Forms.Padding(15);
            this.appSettingsGroupBox.Size = new System.Drawing.Size(610, 320);
            this.appSettingsGroupBox.TabIndex = 0;
            this.appSettingsGroupBox.TabStop = false;
            this.appSettingsGroupBox.Text = "应用程序选项";
            // 
            // resetSettingsButton
            // 
            this.resetSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.resetSettingsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(160)))), ((int)(((byte)(122)))));
            this.resetSettingsButton.FlatAppearance.BorderSize = 0;
            this.resetSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetSettingsButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.resetSettingsButton.ForeColor = System.Drawing.Color.White;
            this.resetSettingsButton.Location = new System.Drawing.Point(20, 260);
            this.resetSettingsButton.Name = "resetSettingsButton";
            this.resetSettingsButton.Size = new System.Drawing.Size(120, 35);
            this.resetSettingsButton.TabIndex = 2;
            this.resetSettingsButton.Text = "重置所有设置";
            this.resetSettingsButton.UseVisualStyleBackColor = false;
            // 
            // autoStartCheckBox
            // 
            this.autoStartCheckBox.AutoSize = true;
            this.autoStartCheckBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.autoStartCheckBox.Location = new System.Drawing.Point(20, 30);
            this.autoStartCheckBox.Name = "autoStartCheckBox";
            this.autoStartCheckBox.Size = new System.Drawing.Size(176, 24);
            this.autoStartCheckBox.TabIndex = 0;
            this.autoStartCheckBox.Text = "开机自动启动应用程序";
            this.autoStartCheckBox.UseVisualStyleBackColor = true;
            // 
            // resetSettingsWarningLabel
            // 
            this.resetSettingsWarningLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resetSettingsWarningLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.resetSettingsWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.resetSettingsWarningLabel.Location = new System.Drawing.Point(150, 260);
            this.resetSettingsWarningLabel.Name = "resetSettingsWarningLabel";
            this.resetSettingsWarningLabel.Size = new System.Drawing.Size(440, 35);
            this.resetSettingsWarningLabel.TabIndex = 3;
            this.resetSettingsWarningLabel.Text = "警告：此操作将清除所有保存的设置，包括API配置和自定义设置。此操作无法撤销！";
            this.resetSettingsWarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // footerPanel
            // 
            this.footerPanel.Controls.Add(this.apiSettingsTabControl);
            this.footerPanel.Controls.Add(this.titleLabel);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerPanel.Location = new System.Drawing.Point(0, 0);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(700, 459);
            this.footerPanel.TabIndex = 3;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 485);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "应用程序设置 - 部落冲突启动器";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.apiSettingsTabControl.ResumeLayout(false);
            this.apiSettingsTabPage.ResumeLayout(false);
            this.apiSettingsGroupBox.ResumeLayout(false);
            this.apiSettingsGroupBox.PerformLayout();
            this.appSettingsTabPage.ResumeLayout(false);
            this.appSettingsGroupBox.ResumeLayout(false);
            this.appSettingsGroupBox.PerformLayout();
            this.footerPanel.ResumeLayout(false);
            this.footerPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label apiEndpointLabel;
        private System.Windows.Forms.TextBox apiEndpointTextBox;
        private System.Windows.Forms.Label apiKeyLabel;
        private System.Windows.Forms.TextBox apiKeyTextBox;
        private System.Windows.Forms.Button saveSettingsButton;
        private System.Windows.Forms.GroupBox apiSettingsGroupBox;
        private System.Windows.Forms.ComboBox apiComboBox;
        private System.Windows.Forms.Label apiComboBoxLabel;
        private System.Windows.Forms.Button addCustomApiButton;
        private System.Windows.Forms.TextBox customApiNameTextBox;
        private System.Windows.Forms.Label customApiNameLabel;
        private System.Windows.Forms.Label apiTypeLabel;
        private System.Windows.Forms.Button editApiButton;
        private System.Windows.Forms.Button deleteApiButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TabControl apiSettingsTabControl;
        private System.Windows.Forms.TabPage apiSettingsTabPage;
        private System.Windows.Forms.TabPage appSettingsTabPage;
        private System.Windows.Forms.GroupBox appSettingsGroupBox;
        private System.Windows.Forms.Button resetSettingsButton;
        private System.Windows.Forms.CheckBox autoStartCheckBox;
        private System.Windows.Forms.Label resetSettingsWarningLabel;
        private System.Windows.Forms.Panel footerPanel;
    }
}