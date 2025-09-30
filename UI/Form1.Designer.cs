namespace GoogleClashofClansLauncher.UI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            inputTextBox = new TextBox();
            simulateButton = new Button();
            fixed123Button = new Button();
            mouseClickButton = new Button();
            settingsButton = new Button();
            SuspendLayout();
            // 
            // inputTextBox
            // 
            inputTextBox.Location = new Point(12, 12);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.PlaceholderText = "请输入要模拟的键盘内容...";
            inputTextBox.Size = new Size(300, 27);
            inputTextBox.TabIndex = 4;
            // 
            // simulateButton
            // 
            simulateButton.Location = new Point(318, 12);
            simulateButton.Name = "simulateButton";
            simulateButton.Size = new Size(94, 29);
            simulateButton.TabIndex = 3;
            simulateButton.Text = "模拟输入";
            simulateButton.UseVisualStyleBackColor = true;
            simulateButton.Click += SimulateButton_Click;
            // 
            // fixed123Button
            // 
            fixed123Button.Location = new Point(418, 12);
            fixed123Button.Name = "fixed123Button";
            fixed123Button.Size = new Size(94, 29);
            fixed123Button.TabIndex = 2;
            fixed123Button.Text = "固定输入123";
            fixed123Button.UseVisualStyleBackColor = true;
            fixed123Button.Click += Fixed123Button_Click;
            // 
            // mouseClickButton
            // 
            mouseClickButton.Location = new Point(12, 45);
            mouseClickButton.Name = "mouseClickButton";
            mouseClickButton.Size = new Size(500, 29);
            mouseClickButton.TabIndex = 1;
            mouseClickButton.Text = "点击测试（3 次/秒 × 10 秒）";
            mouseClickButton.UseVisualStyleBackColor = true;
            mouseClickButton.Click += MouseClickButton_Click;
            // 
            // settingsButton
            // 
            settingsButton.Location = new Point(482, 45);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(30, 30);
            settingsButton.TabIndex = 0;
            settingsButton.UseVisualStyleBackColor = true;
            settingsButton.Click += SettingsButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 86);
            Controls.Add(settingsButton);
            Controls.Add(mouseClickButton);
            Controls.Add(fixed123Button);
            Controls.Add(simulateButton);
            Controls.Add(inputTextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "ClashofClans键鼠模拟器";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox inputTextBox;
        private Button simulateButton;
        private Button fixed123Button;
        private Button mouseClickButton;
        private Button settingsButton;
    }
}