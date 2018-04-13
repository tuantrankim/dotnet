namespace WcfClient
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnGetEmployee = new System.Windows.Forms.Button();
            this.btnSaveEmployee = new System.Windows.Forms.Button();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtGender = new System.Windows.Forms.TextBox();
            this.txtDateOfBirth = new System.Windows.Forms.TextBox();
            this.btnTransaction = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSaveEmployeeAsync = new System.Windows.Forms.Button();
            this.btnInternalService = new System.Windows.Forms.Button();
            this.btnClientBehavior = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Gender";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Date Of Birth";
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblMessage.Location = new System.Drawing.Point(11, 345);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(112, 25);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "lblMessage";
            // 
            // btnGetEmployee
            // 
            this.btnGetEmployee.Location = new System.Drawing.Point(232, 154);
            this.btnGetEmployee.Name = "btnGetEmployee";
            this.btnGetEmployee.Size = new System.Drawing.Size(112, 23);
            this.btnGetEmployee.TabIndex = 1;
            this.btnGetEmployee.Text = "Get Employee";
            this.btnGetEmployee.UseVisualStyleBackColor = true;
            this.btnGetEmployee.Click += new System.EventHandler(this.btnGetEmployee_Click);
            // 
            // btnSaveEmployee
            // 
            this.btnSaveEmployee.Location = new System.Drawing.Point(18, 154);
            this.btnSaveEmployee.Name = "btnSaveEmployee";
            this.btnSaveEmployee.Size = new System.Drawing.Size(194, 23);
            this.btnSaveEmployee.TabIndex = 2;
            this.btnSaveEmployee.Text = "Save Employee";
            this.btnSaveEmployee.UseVisualStyleBackColor = true;
            this.btnSaveEmployee.Click += new System.EventHandler(this.btnSaveEmployee_Click);
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(107, 27);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(100, 20);
            this.txtID.TabIndex = 3;
            this.txtID.Text = "1";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(107, 53);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(100, 20);
            this.txtName.TabIndex = 4;
            this.txtName.Text = "Test";
            // 
            // txtGender
            // 
            this.txtGender.Location = new System.Drawing.Point(107, 79);
            this.txtGender.Name = "txtGender";
            this.txtGender.Size = new System.Drawing.Size(100, 20);
            this.txtGender.TabIndex = 5;
            this.txtGender.Text = "Male";
            // 
            // txtDateOfBirth
            // 
            this.txtDateOfBirth.Location = new System.Drawing.Point(107, 105);
            this.txtDateOfBirth.Name = "txtDateOfBirth";
            this.txtDateOfBirth.Size = new System.Drawing.Size(100, 20);
            this.txtDateOfBirth.TabIndex = 6;
            this.txtDateOfBirth.Text = "1/1/2015";
            // 
            // btnTransaction
            // 
            this.btnTransaction.Location = new System.Drawing.Point(234, 183);
            this.btnTransaction.Name = "btnTransaction";
            this.btnTransaction.Size = new System.Drawing.Size(110, 23);
            this.btnTransaction.TabIndex = 7;
            this.btnTransaction.Text = "Transaction";
            this.btnTransaction.UseVisualStyleBackColor = true;
            this.btnTransaction.Click += new System.EventHandler(this.btnTransaction_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(18, 183);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(194, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Save Employee client Async";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnSaveEmployee2_Click);
            // 
            // btnSaveEmployeeAsync
            // 
            this.btnSaveEmployeeAsync.Location = new System.Drawing.Point(18, 212);
            this.btnSaveEmployeeAsync.Name = "btnSaveEmployeeAsync";
            this.btnSaveEmployeeAsync.Size = new System.Drawing.Size(194, 23);
            this.btnSaveEmployeeAsync.TabIndex = 2;
            this.btnSaveEmployeeAsync.Text = "Save Employee Server Async";
            this.btnSaveEmployeeAsync.UseVisualStyleBackColor = true;
            this.btnSaveEmployeeAsync.Click += new System.EventHandler(this.btnSaveEmployeeAsync_Click);
            // 
            // btnInternalService
            // 
            this.btnInternalService.Location = new System.Drawing.Point(18, 242);
            this.btnInternalService.Name = "btnInternalService";
            this.btnInternalService.Size = new System.Drawing.Size(194, 23);
            this.btnInternalService.TabIndex = 8;
            this.btnInternalService.Text = "InternalServiceOnServer";
            this.btnInternalService.UseVisualStyleBackColor = true;
            this.btnInternalService.Click += new System.EventHandler(this.btnInternalService_Click);
            // 
            // btnClientBehavior
            // 
            this.btnClientBehavior.Location = new System.Drawing.Point(232, 212);
            this.btnClientBehavior.Name = "btnClientBehavior";
            this.btnClientBehavior.Size = new System.Drawing.Size(180, 23);
            this.btnClientBehavior.TabIndex = 7;
            this.btnClientBehavior.Text = "Client and Server Behavior";
            this.btnClientBehavior.UseVisualStyleBackColor = true;
            this.btnClientBehavior.Click += new System.EventHandler(this.btnClientBehavior_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 379);
            this.Controls.Add(this.btnInternalService);
            this.Controls.Add(this.btnClientBehavior);
            this.Controls.Add(this.btnTransaction);
            this.Controls.Add(this.txtDateOfBirth);
            this.Controls.Add(this.txtGender);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.btnSaveEmployeeAsync);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSaveEmployee);
            this.Controls.Add(this.btnGetEmployee);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnGetEmployee;
        private System.Windows.Forms.Button btnSaveEmployee;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtGender;
        private System.Windows.Forms.TextBox txtDateOfBirth;
        private System.Windows.Forms.Button btnTransaction;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSaveEmployeeAsync;
        private System.Windows.Forms.Button btnInternalService;
        private System.Windows.Forms.Button btnClientBehavior;
    }
}

