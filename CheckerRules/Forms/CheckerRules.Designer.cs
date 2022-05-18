namespace BBI.JD.Forms
{
    partial class CheckerRules
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
            this.tree_Rules = new System.Windows.Forms.TreeView();
            this.btn_CheckAll = new System.Windows.Forms.Button();
            this.btn_Unckeck = new System.Windows.Forms.Button();
            this.btn_Execute = new System.Windows.Forms.Button();
            this.chk_Links = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Config = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // tree_Rules
            // 
            this.tree_Rules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tree_Rules.CheckBoxes = true;
            this.tree_Rules.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.tree_Rules.ItemHeight = 18;
            this.tree_Rules.Location = new System.Drawing.Point(12, 38);
            this.tree_Rules.Name = "tree_Rules";
            this.tree_Rules.Size = new System.Drawing.Size(390, 316);
            this.tree_Rules.TabIndex = 2;
            this.tree_Rules.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tree_Rules_AfterCheck);
            // 
            // btn_CheckAll
            // 
            this.btn_CheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_CheckAll.Location = new System.Drawing.Point(251, 361);
            this.btn_CheckAll.Name = "btn_CheckAll";
            this.btn_CheckAll.Size = new System.Drawing.Size(71, 23);
            this.btn_CheckAll.TabIndex = 4;
            this.btn_CheckAll.Text = "Check all";
            this.btn_CheckAll.UseVisualStyleBackColor = true;
            this.btn_CheckAll.Click += new System.EventHandler(this.btn_CheckAll_Click);
            // 
            // btn_Unckeck
            // 
            this.btn_Unckeck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Unckeck.Location = new System.Drawing.Point(331, 361);
            this.btn_Unckeck.Name = "btn_Unckeck";
            this.btn_Unckeck.Size = new System.Drawing.Size(71, 23);
            this.btn_Unckeck.TabIndex = 5;
            this.btn_Unckeck.Text = "Uncheck all";
            this.btn_Unckeck.UseVisualStyleBackColor = true;
            this.btn_Unckeck.Click += new System.EventHandler(this.btn_Uncheck_Click);
            // 
            // btn_Execute
            // 
            this.btn_Execute.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btn_Execute.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.btn_Execute.Location = new System.Drawing.Point(148, 424);
            this.btn_Execute.Name = "btn_Execute";
            this.btn_Execute.Size = new System.Drawing.Size(124, 55);
            this.btn_Execute.TabIndex = 6;
            this.btn_Execute.Text = "Execute";
            this.btn_Execute.UseVisualStyleBackColor = true;
            this.btn_Execute.Click += new System.EventHandler(this.btn_Execute_Click);
            // 
            // chk_Links
            // 
            this.chk_Links.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chk_Links.AutoSize = true;
            this.chk_Links.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.chk_Links.Location = new System.Drawing.Point(12, 363);
            this.chk_Links.Name = "chk_Links";
            this.chk_Links.Size = new System.Drawing.Size(164, 21);
            this.chk_Links.TabIndex = 3;
            this.chk_Links.Text = "Execute rules on links";
            this.chk_Links.UseVisualStyleBackColor = true;
            this.chk_Links.CheckedChanged += new System.EventHandler(this.chk_Links_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(244, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "Rules available for checking";
            // 
            // btn_Config
            // 
            this.btn_Config.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Config.Location = new System.Drawing.Point(348, 12);
            this.btn_Config.Name = "btn_Config";
            this.btn_Config.Size = new System.Drawing.Size(54, 23);
            this.btn_Config.TabIndex = 1;
            this.btn_Config.Text = "Config";
            this.btn_Config.UseVisualStyleBackColor = true;
            this.btn_Config.Click += new System.EventHandler(this.btn_Config_Click);
            // 
            // CheckerRules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 491);
            this.Controls.Add(this.btn_Config);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chk_Links);
            this.Controls.Add(this.btn_Execute);
            this.Controls.Add(this.btn_Unckeck);
            this.Controls.Add(this.btn_CheckAll);
            this.Controls.Add(this.tree_Rules);
            this.Name = "CheckerRules";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Checker Rules"; // GetTiTleForm();
            this.Load += new System.EventHandler(this.CheckerRules_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tree_Rules;
        private System.Windows.Forms.Button btn_CheckAll;
        private System.Windows.Forms.Button btn_Unckeck;
        private System.Windows.Forms.Button btn_Execute;
        private System.Windows.Forms.CheckBox chk_Links;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Config;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}