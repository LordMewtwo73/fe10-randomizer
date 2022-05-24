
namespace Enemy_Rerandomizer
{
	partial class Enemy_Rerandomizer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Enemy_Rerandomizer));
			this.cbxEnemWeaps = new System.Windows.Forms.CheckBox();
			this.cbxTier3Enemies = new System.Windows.Forms.CheckBox();
			this.lblLocation = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnRandomize = new System.Windows.Forms.Button();
			this.btnLoad = new System.Windows.Forms.Button();
			this.panelEnemy = new System.Windows.Forms.Panel();
			this.cbx1to1EnemyRand = new System.Windows.Forms.CheckBox();
			this.cbxNoEnemyLaguz = new System.Windows.Forms.CheckBox();
			this.cbxSpirits = new System.Windows.Forms.CheckBox();
			this.cbxEnemHealers = new System.Windows.Forms.CheckBox();
			this.cbxOnlySiege = new System.Windows.Forms.CheckBox();
			this.cbxNoSiege = new System.Windows.Forms.CheckBox();
			this.cbxSimilarEnemy = new System.Windows.Forms.CheckBox();
			this.cbxRandAllies = new System.Windows.Forms.CheckBox();
			this.cbxRandEnemy = new System.Windows.Forms.CheckBox();
			this.folderBD = new System.Windows.Forms.FolderBrowserDialog();
			this.panelEnemy.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbxEnemWeaps
			// 
			this.cbxEnemWeaps.AutoSize = true;
			this.cbxEnemWeaps.Location = new System.Drawing.Point(98, 255);
			this.cbxEnemWeaps.Margin = new System.Windows.Forms.Padding(2);
			this.cbxEnemWeaps.Name = "cbxEnemWeaps";
			this.cbxEnemWeaps.Size = new System.Drawing.Size(206, 17);
			this.cbxEnemWeaps.TabIndex = 61;
			this.cbxEnemWeaps.Text = "Enemies can have Stronger Weapons";
			this.cbxEnemWeaps.UseVisualStyleBackColor = true;
			// 
			// cbxTier3Enemies
			// 
			this.cbxTier3Enemies.AutoSize = true;
			this.cbxTier3Enemies.Location = new System.Drawing.Point(98, 235);
			this.cbxTier3Enemies.Margin = new System.Windows.Forms.Padding(2);
			this.cbxTier3Enemies.Name = "cbxTier3Enemies";
			this.cbxTier3Enemies.Size = new System.Drawing.Size(179, 17);
			this.cbxTier3Enemies.TabIndex = 62;
			this.cbxTier3Enemies.Text = "Part 4 Enemies have T3 Classes";
			this.cbxTier3Enemies.UseVisualStyleBackColor = true;
			// 
			// lblLocation
			// 
			this.lblLocation.AutoSize = true;
			this.lblLocation.Location = new System.Drawing.Point(9, 7);
			this.lblLocation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblLocation.Name = "lblLocation";
			this.lblLocation.Size = new System.Drawing.Size(148, 13);
			this.lblLocation.TabIndex = 60;
			this.lblLocation.Text = "No DATA\\files folder selected";
			// 
			// textBox1
			// 
			this.textBox1.Enabled = false;
			this.textBox1.Location = new System.Drawing.Point(12, 319);
			this.textBox1.Margin = new System.Windows.Forms.Padding(2);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(377, 38);
			this.textBox1.TabIndex = 59;
			// 
			// btnRandomize
			// 
			this.btnRandomize.Enabled = false;
			this.btnRandomize.Location = new System.Drawing.Point(62, 274);
			this.btnRandomize.Name = "btnRandomize";
			this.btnRandomize.Size = new System.Drawing.Size(280, 40);
			this.btnRandomize.TabIndex = 58;
			this.btnRandomize.Text = "Randomize Enemies";
			this.btnRandomize.UseVisualStyleBackColor = true;
			this.btnRandomize.Click += new System.EventHandler(this.btnRandomize_Click);
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(62, 23);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(280, 41);
			this.btnLoad.TabIndex = 57;
			this.btnLoad.Text = "Load DATA\\files\\";
			this.btnLoad.UseVisualStyleBackColor = true;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// panelEnemy
			// 
			this.panelEnemy.Controls.Add(this.cbx1to1EnemyRand);
			this.panelEnemy.Controls.Add(this.cbxNoEnemyLaguz);
			this.panelEnemy.Controls.Add(this.cbxSpirits);
			this.panelEnemy.Controls.Add(this.cbxEnemHealers);
			this.panelEnemy.Controls.Add(this.cbxOnlySiege);
			this.panelEnemy.Controls.Add(this.cbxNoSiege);
			this.panelEnemy.Controls.Add(this.cbxSimilarEnemy);
			this.panelEnemy.Enabled = false;
			this.panelEnemy.Location = new System.Drawing.Point(62, 95);
			this.panelEnemy.Margin = new System.Windows.Forms.Padding(2);
			this.panelEnemy.Name = "panelEnemy";
			this.panelEnemy.Size = new System.Drawing.Size(280, 136);
			this.panelEnemy.TabIndex = 56;
			// 
			// cbx1to1EnemyRand
			// 
			this.cbx1to1EnemyRand.AutoSize = true;
			this.cbx1to1EnemyRand.Location = new System.Drawing.Point(13, 8);
			this.cbx1to1EnemyRand.Margin = new System.Windows.Forms.Padding(2);
			this.cbx1to1EnemyRand.Name = "cbx1to1EnemyRand";
			this.cbx1to1EnemyRand.Size = new System.Drawing.Size(162, 17);
			this.cbx1to1EnemyRand.TabIndex = 45;
			this.cbx1to1EnemyRand.Text = "1:1 Class Changing (old way)";
			this.cbx1to1EnemyRand.UseVisualStyleBackColor = true;
			// 
			// cbxNoEnemyLaguz
			// 
			this.cbxNoEnemyLaguz.AutoSize = true;
			this.cbxNoEnemyLaguz.Location = new System.Drawing.Point(13, 110);
			this.cbxNoEnemyLaguz.Margin = new System.Windows.Forms.Padding(2);
			this.cbxNoEnemyLaguz.Name = "cbxNoEnemyLaguz";
			this.cbxNoEnemyLaguz.Size = new System.Drawing.Size(202, 17);
			this.cbxNoEnemyLaguz.TabIndex = 45;
			this.cbxNoEnemyLaguz.Text = "Beorc can\'t be Laguz and Vice-Versa";
			this.cbxNoEnemyLaguz.UseVisualStyleBackColor = true;
			// 
			// cbxSpirits
			// 
			this.cbxSpirits.AutoSize = true;
			this.cbxSpirits.Location = new System.Drawing.Point(13, 25);
			this.cbxSpirits.Margin = new System.Windows.Forms.Padding(2);
			this.cbxSpirits.Name = "cbxSpirits";
			this.cbxSpirits.Size = new System.Drawing.Size(92, 17);
			this.cbxSpirits.TabIndex = 0;
			this.cbxSpirits.Text = "Include Spirits";
			this.cbxSpirits.UseVisualStyleBackColor = true;
			// 
			// cbxEnemHealers
			// 
			this.cbxEnemHealers.AutoSize = true;
			this.cbxEnemHealers.Location = new System.Drawing.Point(13, 93);
			this.cbxEnemHealers.Margin = new System.Windows.Forms.Padding(2);
			this.cbxEnemHealers.Name = "cbxEnemHealers";
			this.cbxEnemHealers.Size = new System.Drawing.Size(146, 17);
			this.cbxEnemHealers.TabIndex = 42;
			this.cbxEnemHealers.Text = "Don\'t Randomize Healers";
			this.cbxEnemHealers.UseVisualStyleBackColor = true;
			// 
			// cbxOnlySiege
			// 
			this.cbxOnlySiege.AutoSize = true;
			this.cbxOnlySiege.Location = new System.Drawing.Point(13, 76);
			this.cbxOnlySiege.Margin = new System.Windows.Forms.Padding(2);
			this.cbxOnlySiege.Name = "cbxOnlySiege";
			this.cbxOnlySiege.Size = new System.Drawing.Size(112, 17);
			this.cbxOnlySiege.TabIndex = 42;
			this.cbxOnlySiege.Text = "Only Siege Tomes";
			this.cbxOnlySiege.UseVisualStyleBackColor = true;
			// 
			// cbxNoSiege
			// 
			this.cbxNoSiege.AutoSize = true;
			this.cbxNoSiege.Location = new System.Drawing.Point(13, 59);
			this.cbxNoSiege.Margin = new System.Windows.Forms.Padding(2);
			this.cbxNoSiege.Name = "cbxNoSiege";
			this.cbxNoSiege.Size = new System.Drawing.Size(105, 17);
			this.cbxNoSiege.TabIndex = 42;
			this.cbxNoSiege.Text = "No Siege Tomes";
			this.cbxNoSiege.UseVisualStyleBackColor = true;
			// 
			// cbxSimilarEnemy
			// 
			this.cbxSimilarEnemy.AutoSize = true;
			this.cbxSimilarEnemy.Location = new System.Drawing.Point(13, 42);
			this.cbxSimilarEnemy.Margin = new System.Windows.Forms.Padding(2);
			this.cbxSimilarEnemy.Name = "cbxSimilarEnemy";
			this.cbxSimilarEnemy.Size = new System.Drawing.Size(104, 17);
			this.cbxSimilarEnemy.TabIndex = 42;
			this.cbxSimilarEnemy.Text = "Similar Classtype";
			this.cbxSimilarEnemy.UseVisualStyleBackColor = true;
			// 
			// cbxRandAllies
			// 
			this.cbxRandAllies.AutoSize = true;
			this.cbxRandAllies.Location = new System.Drawing.Point(212, 74);
			this.cbxRandAllies.Margin = new System.Windows.Forms.Padding(2);
			this.cbxRandAllies.Name = "cbxRandAllies";
			this.cbxRandAllies.Size = new System.Drawing.Size(177, 17);
			this.cbxRandAllies.TabIndex = 54;
			this.cbxRandAllies.Text = "Randomize Generic Ally Classes";
			this.cbxRandAllies.UseVisualStyleBackColor = true;
			this.cbxRandAllies.CheckStateChanged += new System.EventHandler(this.cbxRandAllies_CheckedChanged);
			// 
			// cbxRandEnemy
			// 
			this.cbxRandEnemy.AutoSize = true;
			this.cbxRandEnemy.Location = new System.Drawing.Point(12, 74);
			this.cbxRandEnemy.Margin = new System.Windows.Forms.Padding(2);
			this.cbxRandEnemy.Name = "cbxRandEnemy";
			this.cbxRandEnemy.Size = new System.Drawing.Size(193, 17);
			this.cbxRandEnemy.TabIndex = 55;
			this.cbxRandEnemy.Text = "Randomize Generic Enemy Classes";
			this.cbxRandEnemy.UseVisualStyleBackColor = true;
			this.cbxRandEnemy.CheckedChanged += new System.EventHandler(this.cbxRandEnemy_CheckedChanged);
			// 
			// folderBD
			// 
			this.folderBD.Description = "Select DATA\\files folder";
			// 
			// Enemy_Rerandomizer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(395, 364);
			this.Controls.Add(this.cbxEnemWeaps);
			this.Controls.Add(this.cbxTier3Enemies);
			this.Controls.Add(this.lblLocation);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.btnRandomize);
			this.Controls.Add(this.btnLoad);
			this.Controls.Add(this.panelEnemy);
			this.Controls.Add(this.cbxRandAllies);
			this.Controls.Add(this.cbxRandEnemy);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Enemy_Rerandomizer";
			this.Text = "FE10 Enemy Re-Randomizer";
			this.panelEnemy.ResumeLayout(false);
			this.panelEnemy.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbxEnemWeaps;
		private System.Windows.Forms.CheckBox cbxTier3Enemies;
		private System.Windows.Forms.Label lblLocation;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btnRandomize;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.Panel panelEnemy;
		private System.Windows.Forms.CheckBox cbx1to1EnemyRand;
		private System.Windows.Forms.CheckBox cbxNoEnemyLaguz;
		private System.Windows.Forms.CheckBox cbxSpirits;
		private System.Windows.Forms.CheckBox cbxEnemHealers;
		private System.Windows.Forms.CheckBox cbxOnlySiege;
		private System.Windows.Forms.CheckBox cbxNoSiege;
		private System.Windows.Forms.CheckBox cbxSimilarEnemy;
		private System.Windows.Forms.CheckBox cbxRandAllies;
		private System.Windows.Forms.CheckBox cbxRandEnemy;
		private System.Windows.Forms.FolderBrowserDialog folderBD;
	}
}

