namespace Carcassonne2
{
    partial class ConnectToServer
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
            this.components = new System.ComponentModel.Container();
            this.IPAdressInput = new System.Windows.Forms.TextBox();
            this.IPAdressLabel = new System.Windows.Forms.Label();
            this.PortLabel = new System.Windows.Forms.Label();
            this.PortInput = new System.Windows.Forms.NumericUpDown();
            this.NameLabel = new System.Windows.Forms.Label();
            this.NameInput = new System.Windows.Forms.TextBox();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.LaunchButton = new System.Windows.Forms.Button();
            this.ErrorMessageToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PortInput)).BeginInit();
            this.ButtonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // IPAdressInput
            // 
            this.IPAdressInput.Location = new System.Drawing.Point(73, 12);
            this.IPAdressInput.Name = "IPAdressInput";
            this.IPAdressInput.Size = new System.Drawing.Size(196, 23);
            this.IPAdressInput.TabIndex = 0;
            // 
            // IPAdressLabel
            // 
            this.IPAdressLabel.AutoSize = true;
            this.IPAdressLabel.Location = new System.Drawing.Point(12, 15);
            this.IPAdressLabel.Name = "IPAdressLabel";
            this.IPAdressLabel.Size = new System.Drawing.Size(55, 15);
            this.IPAdressLabel.TabIndex = 1;
            this.IPAdressLabel.Text = "IP Adress";
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(12, 43);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(29, 15);
            this.PortLabel.TabIndex = 2;
            this.PortLabel.Text = "Port";
            // 
            // PortInput
            // 
            this.PortInput.Location = new System.Drawing.Point(47, 41);
            this.PortInput.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.PortInput.Name = "PortInput";
            this.PortInput.Size = new System.Drawing.Size(222, 23);
            this.PortInput.TabIndex = 3;
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(12, 73);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(39, 15);
            this.NameLabel.TabIndex = 4;
            this.NameLabel.Text = "Name";
            // 
            // NameInput
            // 
            this.NameInput.Location = new System.Drawing.Point(57, 70);
            this.NameInput.Name = "NameInput";
            this.NameInput.Size = new System.Drawing.Size(212, 23);
            this.NameInput.TabIndex = 5;
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ButtonPanel.Controls.Add(this.IPAdressLabel);
            this.ButtonPanel.Controls.Add(this.IPAdressInput);
            this.ButtonPanel.Controls.Add(this.PortLabel);
            this.ButtonPanel.Controls.Add(this.NameInput);
            this.ButtonPanel.Controls.Add(this.PortInput);
            this.ButtonPanel.Controls.Add(this.NameLabel);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(277, 105);
            this.ButtonPanel.TabIndex = 6;
            // 
            // LaunchButton
            // 
            this.LaunchButton.Location = new System.Drawing.Point(190, 117);
            this.LaunchButton.Name = "LaunchButton";
            this.LaunchButton.Size = new System.Drawing.Size(75, 23);
            this.LaunchButton.TabIndex = 0;
            this.LaunchButton.Text = "Launch";
            this.LaunchButton.UseVisualStyleBackColor = true;
            this.LaunchButton.Click += new System.EventHandler(this.LaunchButton_Click);
            // 
            // ErrorMessageToolTip
            // 
            this.ErrorMessageToolTip.BackColor = System.Drawing.Color.Red;
            this.ErrorMessageToolTip.OwnerDraw = true;
            this.ErrorMessageToolTip.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.ErrorMessageToolTip_Draw);
            // 
            // ConnectToServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 152);
            this.Controls.Add(this.ButtonPanel);
            this.Controls.Add(this.LaunchButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ConnectToServer";
            this.Text = "Find Server to Connect to...";
            ((System.ComponentModel.ISupportInitialize)(this.PortInput)).EndInit();
            this.ButtonPanel.ResumeLayout(false);
            this.ButtonPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TextBox IPAdressInput;
        private Label IPAdressLabel;
        private Label PortLabel;
        private NumericUpDown PortInput;
        private Label NameLabel;
        private TextBox NameInput;
        private Panel ButtonPanel;
        private Button LaunchButton;
        private ToolTip ErrorMessageToolTip;
    }
}