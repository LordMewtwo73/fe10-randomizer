
namespace FE10_Challenges_Patches_and_Hardmode_Changes
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.lblLocation = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnRandomize = new System.Windows.Forms.Button();
			this.btnLoad = new System.Windows.Forms.Button();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.cbxMapAff = new System.Windows.Forms.CheckBox();
			this.cbxWeapTri = new System.Windows.Forms.CheckBox();
			this.cbxEnemyRange = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.cbxNegGrowths = new System.Windows.Forms.CheckBox();
			this.cbxIronMan = new System.Windows.Forms.CheckBox();
			this.cbxTowerUnits = new System.Windows.Forms.CheckBox();
			this.cbxZeroGrowths = new System.Windows.Forms.CheckBox();
			this.folderBD = new System.Windows.Forms.FolderBrowserDialog();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.cbxKurthEna = new System.Windows.Forms.CheckBox();
			this.groupBox6.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblLocation
			// 
			this.lblLocation.AutoSize = true;
			this.lblLocation.Location = new System.Drawing.Point(11, 9);
			this.lblLocation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblLocation.Name = "lblLocation";
			this.lblLocation.Size = new System.Drawing.Size(148, 13);
			this.lblLocation.TabIndex = 64;
			this.lblLocation.Text = "No DATA\\files folder selected";
			// 
			// textBox1
			// 
			this.textBox1.Enabled = false;
			this.textBox1.Location = new System.Drawing.Point(14, 341);
			this.textBox1.Margin = new System.Windows.Forms.Padding(2);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(310, 38);
			this.textBox1.TabIndex = 63;
			// 
			// btnRandomize
			// 
			this.btnRandomize.Enabled = false;
			this.btnRandomize.Location = new System.Drawing.Point(45, 296);
			this.btnRandomize.Name = "btnRandomize";
			this.btnRandomize.Size = new System.Drawing.Size(242, 40);
			this.btnRandomize.TabIndex = 62;
			this.btnRandomize.Text = "Make Changes";
			this.btnRandomize.UseVisualStyleBackColor = true;
			this.btnRandomize.Click += new System.EventHandler(this.btnRandomize_Click);
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(45, 25);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(242, 41);
			this.btnLoad.TabIndex = 61;
			this.btnLoad.Text = "Load DATA\\files\\";
			this.btnLoad.UseVisualStyleBackColor = true;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.cbxMapAff);
			this.groupBox6.Controls.Add(this.cbxWeapTri);
			this.groupBox6.Controls.Add(this.cbxEnemyRange);
			this.groupBox6.Location = new System.Drawing.Point(45, 206);
			this.groupBox6.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox6.Size = new System.Drawing.Size(242, 78);
			this.groupBox6.TabIndex = 69;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Make Hardmode Great Again";
			// 
			// cbxMapAff
			// 
			this.cbxMapAff.AutoSize = true;
			this.cbxMapAff.Location = new System.Drawing.Point(16, 55);
			this.cbxMapAff.Margin = new System.Windows.Forms.Padding(2);
			this.cbxMapAff.Name = "cbxMapAff";
			this.cbxMapAff.Size = new System.Drawing.Size(81, 17);
			this.cbxMapAff.TabIndex = 0;
			this.cbxMapAff.Text = "Map Affinity";
			this.cbxMapAff.UseVisualStyleBackColor = true;
			// 
			// cbxWeapTri
			// 
			this.cbxWeapTri.AutoSize = true;
			this.cbxWeapTri.Location = new System.Drawing.Point(16, 36);
			this.cbxWeapTri.Margin = new System.Windows.Forms.Padding(2);
			this.cbxWeapTri.Name = "cbxWeapTri";
			this.cbxWeapTri.Size = new System.Drawing.Size(108, 17);
			this.cbxWeapTri.TabIndex = 0;
			this.cbxWeapTri.Text = "Weapon Triangle";
			this.cbxWeapTri.UseVisualStyleBackColor = true;
			// 
			// cbxEnemyRange
			// 
			this.cbxEnemyRange.AutoSize = true;
			this.cbxEnemyRange.Location = new System.Drawing.Point(16, 16);
			this.cbxEnemyRange.Margin = new System.Windows.Forms.Padding(2);
			this.cbxEnemyRange.Name = "cbxEnemyRange";
			this.cbxEnemyRange.Size = new System.Drawing.Size(98, 17);
			this.cbxEnemyRange.TabIndex = 0;
			this.cbxEnemyRange.Text = "Enemy Ranges";
			this.cbxEnemyRange.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.cbxKurthEna);
			this.groupBox4.Controls.Add(this.cbxNegGrowths);
			this.groupBox4.Controls.Add(this.cbxIronMan);
			this.groupBox4.Controls.Add(this.cbxTowerUnits);
			this.groupBox4.Controls.Add(this.cbxZeroGrowths);
			this.groupBox4.Location = new System.Drawing.Point(45, 76);
			this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox4.Size = new System.Drawing.Size(242, 121);
			this.groupBox4.TabIndex = 70;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Challenge Runs and Patches";
			// 
			// cbxNegGrowths
			// 
			this.cbxNegGrowths.AutoSize = true;
			this.cbxNegGrowths.Location = new System.Drawing.Point(16, 33);
			this.cbxNegGrowths.Margin = new System.Windows.Forms.Padding(2);
			this.cbxNegGrowths.Name = "cbxNegGrowths";
			this.cbxNegGrowths.Size = new System.Drawing.Size(111, 17);
			this.cbxNegGrowths.TabIndex = 60;
			this.cbxNegGrowths.Text = "Negative Growths";
			this.cbxNegGrowths.UseVisualStyleBackColor = true;
			// 
			// cbxIronMan
			// 
			this.cbxIronMan.AutoSize = true;
			this.cbxIronMan.Location = new System.Drawing.Point(16, 52);
			this.cbxIronMan.Margin = new System.Windows.Forms.Padding(2);
			this.cbxIronMan.Name = "cbxIronMan";
			this.cbxIronMan.Size = new System.Drawing.Size(94, 17);
			this.cbxIronMan.TabIndex = 61;
			this.cbxIronMan.Text = "Ironman Mode";
			this.cbxIronMan.UseVisualStyleBackColor = true;
			// 
			// cbxTowerUnits
			// 
			this.cbxTowerUnits.AutoSize = true;
			this.cbxTowerUnits.Location = new System.Drawing.Point(16, 71);
			this.cbxTowerUnits.Margin = new System.Windows.Forms.Padding(2);
			this.cbxTowerUnits.Name = "cbxTowerUnits";
			this.cbxTowerUnits.Size = new System.Drawing.Size(201, 17);
			this.cbxTowerUnits.TabIndex = 37;
			this.cbxTowerUnits.Text = "Fire Emblem: Choose my Tower Units";
			this.cbxTowerUnits.UseVisualStyleBackColor = true;
			// 
			// cbxZeroGrowths
			// 
			this.cbxZeroGrowths.AutoSize = true;
			this.cbxZeroGrowths.Location = new System.Drawing.Point(16, 15);
			this.cbxZeroGrowths.Margin = new System.Windows.Forms.Padding(2);
			this.cbxZeroGrowths.Name = "cbxZeroGrowths";
			this.cbxZeroGrowths.Size = new System.Drawing.Size(82, 17);
			this.cbxZeroGrowths.TabIndex = 22;
			this.cbxZeroGrowths.Text = "0% Growths";
			this.cbxZeroGrowths.UseVisualStyleBackColor = true;
			// 
			// folderBD
			// 
			this.folderBD.Description = "Select DATA\\files folder";
			// 
			// cbxKurthEna
			// 
			this.cbxKurthEna.AutoSize = true;
			this.cbxKurthEna.Location = new System.Drawing.Point(36, 90);
			this.cbxKurthEna.Margin = new System.Windows.Forms.Padding(2);
			this.cbxKurthEna.Name = "cbxKurthEna";
			this.cbxKurthEna.Size = new System.Drawing.Size(196, 17);
			this.cbxKurthEna.TabIndex = 62;
			this.cbxKurthEna.Text = "Kurth + Ena Not Required for Tower";
			this.cbxKurthEna.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(329, 390);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.lblLocation);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.btnRandomize);
			this.Controls.Add(this.btnLoad);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form1";
			this.Text = "FE10 Challenge Patches and Hardmode Changes";
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblLocation;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btnRandomize;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.CheckBox cbxMapAff;
		private System.Windows.Forms.CheckBox cbxWeapTri;
		private System.Windows.Forms.CheckBox cbxEnemyRange;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox cbxNegGrowths;
		private System.Windows.Forms.CheckBox cbxIronMan;
		private System.Windows.Forms.CheckBox cbxTowerUnits;
		private System.Windows.Forms.CheckBox cbxZeroGrowths;
		private System.Windows.Forms.FolderBrowserDialog folderBD;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.CheckBox cbxKurthEna;
	}
}

