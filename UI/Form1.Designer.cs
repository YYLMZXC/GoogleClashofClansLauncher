namespace GoogleClashofClansLauncher.UI
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
            inputTextBox = new TextBox();
            simulateButton = new Button();
            fixed123Button = new Button();
            titleLabel = new Label();
            mouseClickButton = new Button();
            imageRecognitionButton = new Button();
            settingsButton = new Button();
            recognize003Button = new Button();
            SuspendLayout();
            // 
            // inputTextBox
            // 
            inputTextBox.Location = new Point(44, 88);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.PlaceholderText = "请输入要模拟的键盘内容...";
            inputTextBox.Size = new Size(316, 27);
            inputTextBox.TabIndex = 0;
            // 
            // simulateButton
            // 
            simulateButton.Location = new Point(366, 86);
            simulateButton.Name = "simulateButton";
            simulateButton.Size = new Size(94, 29);
            simulateButton.TabIndex = 1;
            simulateButton.Text = "模拟输入";
            simulateButton.UseVisualStyleBackColor = true;
            simulateButton.Click += simulateButton_Click;
            // 
            // fixed123Button
            // 
            fixed123Button.Location = new Point(466, 86);
            fixed123Button.Name = "fixed123Button";
            fixed123Button.Size = new Size(94, 29);
            fixed123Button.TabIndex = 2;
            fixed123Button.Text = "固定输入123";
            fixed123Button.UseVisualStyleBackColor = true;
            fixed123Button.Click += fixed123Button_Click;
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Microsoft YaHei UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 134);
            titleLabel.Location = new Point(44, 30);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(221, 31);
            titleLabel.TabIndex = 3;
            titleLabel.Text = "键盘输入模拟器工具";
            // 
            // mouseClickButton
            // 
            mouseClickButton.Location = new Point(44, 121);
            mouseClickButton.Name = "mouseClickButton";
            mouseClickButton.Size = new Size(516, 29);
            mouseClickButton.TabIndex = 4;
            mouseClickButton.Text = "点击模拟测试 (1秒3次，持续10秒)";
            mouseClickButton.UseVisualStyleBackColor = true;
            mouseClickButton.Click += mouseClickButton_Click;
            // 
            // imageRecognitionButton
            // 
            imageRecognitionButton.Enabled = false;
            imageRecognitionButton.Location = new Point(44, 156);
            imageRecognitionButton.Name = "imageRecognitionButton";
            imageRecognitionButton.Size = new Size(516, 29);
            imageRecognitionButton.TabIndex = 5;
            imageRecognitionButton.Text = "识别图像并点击 (功能暂未启用)";
            imageRecognitionButton.UseVisualStyleBackColor = true;
            // 
            // settingsButton
            // 
            settingsButton.Location = new Point(490, 10);
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(30, 30);
            settingsButton.TabIndex = 6;
            settingsButton.Text = "设置";
            settingsButton.UseVisualStyleBackColor = true;
            settingsButton.Click += settingsButton_Click;
            // 
            // recognize003Button
            // 
            recognize003Button.Location = new Point(44, 191);
            recognize003Button.Name = "recognize003Button";
            recognize003Button.Size = new Size(516, 29);
            recognize003Button.TabIndex = 7;
            recognize003Button.Text = "识别003图像并点击3次";
            recognize003Button.UseVisualStyleBackColor = true;
            recognize003Button.Click += recognize003Button_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(604, 245);
            Controls.Add(recognize003Button);
            Controls.Add(settingsButton);
            Controls.Add(imageRecognitionButton);
            Controls.Add(mouseClickButton);
            Controls.Add(titleLabel);
            Controls.Add(fixed123Button);
            Controls.Add(simulateButton);
            Controls.Add(inputTextBox);
            Name = "Form1";
            Text = "键盘与鼠标模拟器";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button simulateButton;
        private System.Windows.Forms.Button fixed123Button;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button mouseClickButton;
        private System.Windows.Forms.Button imageRecognitionButton;
        private System.Windows.Forms.Button recognize003Button;
        private System.Windows.Forms.Button settingsButton;
    }
}
