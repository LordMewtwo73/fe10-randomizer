namespace FE10Randomizer_v0._1
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
			this.button1 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.cbxSkillRand = new System.Windows.Forms.CheckBox();
			this.cbxClassRand = new System.Windows.Forms.CheckBox();
			this.cbxGrowthRand = new System.Windows.Forms.CheckBox();
			this.cbxGaugeRand = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.numericLaguzMin = new System.Windows.Forms.NumericUpDown();
			this.numericLaguzMax = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnLoad = new System.Windows.Forms.Button();
			this.folderBD = new System.Windows.Forms.FolderBrowserDialog();
			this.lblLocation = new System.Windows.Forms.Label();
			this.numericGrowth = new System.Windows.Forms.NumericUpDown();
			this.numericHP = new System.Windows.Forms.NumericUpDown();
			this.numericSPD = new System.Windows.Forms.NumericUpDown();
			this.numericATK = new System.Windows.Forms.NumericUpDown();
			this.numericLCK = new System.Windows.Forms.NumericUpDown();
			this.numericDEF = new System.Windows.Forms.NumericUpDown();
			this.numericMAG = new System.Windows.Forms.NumericUpDown();
			this.numericSKL = new System.Windows.Forms.NumericUpDown();
			this.numericRES = new System.Windows.Forms.NumericUpDown();
			this.panelGrowths = new System.Windows.Forms.Panel();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.panelClass = new System.Windows.Forms.Panel();
			this.label15 = new System.Windows.Forms.Label();
			this.cbxHerons = new System.Windows.Forms.CheckBox();
			this.comboClassOptions = new System.Windows.Forms.ComboBox();
			this.cbxZeroGrowths = new System.Windows.Forms.CheckBox();
			this.cbxRandShop = new System.Windows.Forms.CheckBox();
			this.cbxStatCaps = new System.Windows.Forms.CheckBox();
			this.label14 = new System.Windows.Forms.Label();
			this.numericSeed = new System.Windows.Forms.NumericUpDown();
			this.numericStatCap = new System.Windows.Forms.NumericUpDown();
			this.cbxAffinity = new System.Windows.Forms.CheckBox();
			this.cbxBio = new System.Windows.Forms.CheckBox();
			this.cbxEventItems = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.numericLaguzMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLaguzMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericGrowth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericHP)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericSPD)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericATK)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLCK)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericDEF)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericMAG)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericSKL)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRES)).BeginInit();
			this.panelGrowths.SuspendLayout();
			this.panelClass.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericSeed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericStatCap)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Enabled = false;
			this.button1.Location = new System.Drawing.Point(82, 419);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(624, 51);
			this.button1.TabIndex = 0;
			this.button1.Text = "Randomize";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBox1
			// 
			this.textBox1.Enabled = false;
			this.textBox1.Location = new System.Drawing.Point(33, 491);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(724, 22);
			this.textBox1.TabIndex = 1;
			// 
			// cbxSkillRand
			// 
			this.cbxSkillRand.AutoSize = true;
			this.cbxSkillRand.Location = new System.Drawing.Point(27, 362);
			this.cbxSkillRand.Name = "cbxSkillRand";
			this.cbxSkillRand.Size = new System.Drawing.Size(137, 21);
			this.cbxSkillRand.TabIndex = 2;
			this.cbxSkillRand.Text = "Randomize Skills";
			this.cbxSkillRand.UseVisualStyleBackColor = true;
			// 
			// cbxClassRand
			// 
			this.cbxClassRand.AutoSize = true;
			this.cbxClassRand.Location = new System.Drawing.Point(26, 128);
			this.cbxClassRand.Name = "cbxClassRand";
			this.cbxClassRand.Size = new System.Drawing.Size(154, 21);
			this.cbxClassRand.TabIndex = 3;
			this.cbxClassRand.Text = "Randomize Classes";
			this.cbxClassRand.UseVisualStyleBackColor = true;
			this.cbxClassRand.CheckedChanged += new System.EventHandler(this.cbxClassRand_CheckedChanged);
			// 
			// cbxGrowthRand
			// 
			this.cbxGrowthRand.AutoSize = true;
			this.cbxGrowthRand.Location = new System.Drawing.Point(508, 124);
			this.cbxGrowthRand.Name = "cbxGrowthRand";
			this.cbxGrowthRand.Size = new System.Drawing.Size(157, 21);
			this.cbxGrowthRand.TabIndex = 3;
			this.cbxGrowthRand.Text = "Randomize Growths";
			this.cbxGrowthRand.UseVisualStyleBackColor = true;
			this.cbxGrowthRand.CheckedChanged += new System.EventHandler(this.cbxGrowthRand_CheckedChanged);
			// 
			// cbxGaugeRand
			// 
			this.cbxGaugeRand.AutoSize = true;
			this.cbxGaugeRand.Location = new System.Drawing.Point(13, 93);
			this.cbxGaugeRand.Name = "cbxGaugeRand";
			this.cbxGaugeRand.Size = new System.Drawing.Size(180, 21);
			this.cbxGaugeRand.TabIndex = 3;
			this.cbxGaugeRand.Text = "Random Laguz Gauges";
			this.cbxGaugeRand.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(52, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(735, 29);
			this.label1.TabIndex = 4;
			this.label1.Text = "PLEASE READ README.TXT TO PROPERLY USE RANDOMIZER";
			// 
			// numericLaguzMin
			// 
			this.numericLaguzMin.Location = new System.Drawing.Point(48, 121);
			this.numericLaguzMin.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.numericLaguzMin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericLaguzMin.Name = "numericLaguzMin";
			this.numericLaguzMin.Size = new System.Drawing.Size(56, 22);
			this.numericLaguzMin.TabIndex = 5;
			this.numericLaguzMin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericLaguzMin.ValueChanged += new System.EventHandler(this.numericLaguzMin_ValueChanged);
			// 
			// numericLaguzMax
			// 
			this.numericLaguzMax.Location = new System.Drawing.Point(149, 120);
			this.numericLaguzMax.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.numericLaguzMax.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericLaguzMax.Name = "numericLaguzMax";
			this.numericLaguzMax.Size = new System.Drawing.Size(51, 22);
			this.numericLaguzMax.TabIndex = 5;
			this.numericLaguzMax.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.numericLaguzMax.ValueChanged += new System.EventHandler(this.numericLaguzMax_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 124);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 17);
			this.label2.TabIndex = 6;
			this.label2.Text = "min:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(110, 123);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 17);
			this.label3.TabIndex = 7;
			this.label3.Text = "max:";
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(49, 68);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(118, 48);
			this.btnLoad.TabIndex = 8;
			this.btnLoad.Text = "Load Folder";
			this.btnLoad.UseVisualStyleBackColor = true;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// lblLocation
			// 
			this.lblLocation.AutoSize = true;
			this.lblLocation.Location = new System.Drawing.Point(14, 44);
			this.lblLocation.Name = "lblLocation";
			this.lblLocation.Size = new System.Drawing.Size(193, 17);
			this.lblLocation.TabIndex = 9;
			this.lblLocation.Text = "No DATA/files folder selected";
			// 
			// numericGrowth
			// 
			this.numericGrowth.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericGrowth.Location = new System.Drawing.Point(240, 6);
			this.numericGrowth.Name = "numericGrowth";
			this.numericGrowth.Size = new System.Drawing.Size(60, 22);
			this.numericGrowth.TabIndex = 10;
			this.numericGrowth.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			// 
			// numericHP
			// 
			this.numericHP.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericHP.Location = new System.Drawing.Point(60, 48);
			this.numericHP.Name = "numericHP";
			this.numericHP.Size = new System.Drawing.Size(70, 22);
			this.numericHP.TabIndex = 11;
			this.numericHP.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// numericSPD
			// 
			this.numericSPD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericSPD.Location = new System.Drawing.Point(196, 48);
			this.numericSPD.Name = "numericSPD";
			this.numericSPD.Size = new System.Drawing.Size(69, 22);
			this.numericSPD.TabIndex = 12;
			this.numericSPD.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// numericATK
			// 
			this.numericATK.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericATK.Location = new System.Drawing.Point(60, 87);
			this.numericATK.Name = "numericATK";
			this.numericATK.Size = new System.Drawing.Size(70, 22);
			this.numericATK.TabIndex = 13;
			this.numericATK.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// numericLCK
			// 
			this.numericLCK.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericLCK.Location = new System.Drawing.Point(196, 87);
			this.numericLCK.Name = "numericLCK";
			this.numericLCK.Size = new System.Drawing.Size(69, 22);
			this.numericLCK.TabIndex = 14;
			this.numericLCK.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// numericDEF
			// 
			this.numericDEF.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericDEF.Location = new System.Drawing.Point(195, 124);
			this.numericDEF.Name = "numericDEF";
			this.numericDEF.Size = new System.Drawing.Size(70, 22);
			this.numericDEF.TabIndex = 15;
			this.numericDEF.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// numericMAG
			// 
			this.numericMAG.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericMAG.Location = new System.Drawing.Point(60, 125);
			this.numericMAG.Name = "numericMAG";
			this.numericMAG.Size = new System.Drawing.Size(70, 22);
			this.numericMAG.TabIndex = 16;
			this.numericMAG.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// numericSKL
			// 
			this.numericSKL.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericSKL.Location = new System.Drawing.Point(60, 161);
			this.numericSKL.Name = "numericSKL";
			this.numericSKL.Size = new System.Drawing.Size(70, 22);
			this.numericSKL.TabIndex = 17;
			this.numericSKL.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// numericRES
			// 
			this.numericRES.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericRES.Location = new System.Drawing.Point(195, 161);
			this.numericRES.Name = "numericRES";
			this.numericRES.Size = new System.Drawing.Size(70, 22);
			this.numericRES.TabIndex = 18;
			this.numericRES.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// panelGrowths
			// 
			this.panelGrowths.Controls.Add(this.label12);
			this.panelGrowths.Controls.Add(this.numericGrowth);
			this.panelGrowths.Controls.Add(this.label13);
			this.panelGrowths.Controls.Add(this.label11);
			this.panelGrowths.Controls.Add(this.label10);
			this.panelGrowths.Controls.Add(this.label9);
			this.panelGrowths.Controls.Add(this.label8);
			this.panelGrowths.Controls.Add(this.label7);
			this.panelGrowths.Controls.Add(this.label6);
			this.panelGrowths.Controls.Add(this.label5);
			this.panelGrowths.Controls.Add(this.label4);
			this.panelGrowths.Controls.Add(this.numericRES);
			this.panelGrowths.Controls.Add(this.numericSKL);
			this.panelGrowths.Controls.Add(this.numericMAG);
			this.panelGrowths.Controls.Add(this.numericDEF);
			this.panelGrowths.Controls.Add(this.numericLCK);
			this.panelGrowths.Controls.Add(this.numericATK);
			this.panelGrowths.Controls.Add(this.numericSPD);
			this.panelGrowths.Controls.Add(this.numericHP);
			this.panelGrowths.Enabled = false;
			this.panelGrowths.Location = new System.Drawing.Point(507, 147);
			this.panelGrowths.Name = "panelGrowths";
			this.panelGrowths.Size = new System.Drawing.Size(311, 190);
			this.panelGrowths.TabIndex = 19;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(13, 90);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(39, 17);
			this.label12.TabIndex = 27;
			this.label12.Text = "ATK:";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(4, 5);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(230, 17);
			this.label13.TabIndex = 20;
			this.label13.Text = "Maximum Deviation in Growths (%):";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(12, 127);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(43, 17);
			this.label11.TabIndex = 26;
			this.label11.Text = "MAG:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(15, 164);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(38, 17);
			this.label10.TabIndex = 25;
			this.label10.Text = "SKL:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(148, 164);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(40, 17);
			this.label9.TabIndex = 24;
			this.label9.Text = "RES:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(148, 128);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(39, 17);
			this.label8.TabIndex = 23;
			this.label8.Text = "DEF:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(148, 51);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(40, 17);
			this.label7.TabIndex = 22;
			this.label7.Text = "SPD:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(148, 90);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(38, 17);
			this.label6.TabIndex = 21;
			this.label6.Text = "LCK:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(18, 51);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(31, 17);
			this.label5.TabIndex = 20;
			this.label5.Text = "HP:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(4, 25);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(145, 17);
			this.label4.TabIndex = 19;
			this.label4.Text = "Minimum Growths (%)";
			// 
			// panelClass
			// 
			this.panelClass.Controls.Add(this.label15);
			this.panelClass.Controls.Add(this.cbxHerons);
			this.panelClass.Controls.Add(this.comboClassOptions);
			this.panelClass.Controls.Add(this.label3);
			this.panelClass.Controls.Add(this.label2);
			this.panelClass.Controls.Add(this.numericLaguzMax);
			this.panelClass.Controls.Add(this.numericLaguzMin);
			this.panelClass.Controls.Add(this.cbxGaugeRand);
			this.panelClass.Enabled = false;
			this.panelClass.Location = new System.Drawing.Point(13, 153);
			this.panelClass.Name = "panelClass";
			this.panelClass.Size = new System.Drawing.Size(481, 163);
			this.panelClass.TabIndex = 21;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(16, 9);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(193, 17);
			this.label15.TabIndex = 13;
			this.label15.Text = "Class Randomization Options";
			// 
			// cbxHerons
			// 
			this.cbxHerons.AutoSize = true;
			this.cbxHerons.Location = new System.Drawing.Point(14, 67);
			this.cbxHerons.Name = "cbxHerons";
			this.cbxHerons.Size = new System.Drawing.Size(461, 21);
			this.cbxHerons.TabIndex = 12;
			this.cbxHerons.Text = "Include Herons? (NOTE: Herons can only sing for one unit at a time)";
			this.cbxHerons.UseVisualStyleBackColor = true;
			// 
			// comboClassOptions
			// 
			this.comboClassOptions.FormattingEnabled = true;
			this.comboClassOptions.Items.AddRange(new object[] {
            "Basic (Beorc -> Beorc; Laguz -> Laguz)",
            "Full (Beorc can randomize into Laguz and vice-versa)",
            "Beorc-Only Mode",
            "Laguz-Only Mode",
            "Classic Medieval (Myrmidon, Soldier, Fighter, Archer, Thief, Hero, Assassin)",
            "Magic is Everything! (Mages, Priest, Druid, Chancellor, Empress)",
            "Horse Lovers Anonymous (Knights, Cleric)",
            "Tanks a Lot (Armor Knights)",
            "Fly Guys (Pegasus, Wyvern, Hawk, Raven, Heron*, Queen)",
            "Lions & Tigers & Dragons, Oh My! (Lion, Tiger, Cat, Wolf, Dragons)",
            "True Beauty (???)"});
			this.comboClassOptions.Location = new System.Drawing.Point(13, 27);
			this.comboClassOptions.Name = "comboClassOptions";
			this.comboClassOptions.Size = new System.Drawing.Size(454, 24);
			this.comboClassOptions.TabIndex = 10;
			// 
			// cbxZeroGrowths
			// 
			this.cbxZeroGrowths.AutoSize = true;
			this.cbxZeroGrowths.Location = new System.Drawing.Point(508, 342);
			this.cbxZeroGrowths.Name = "cbxZeroGrowths";
			this.cbxZeroGrowths.Size = new System.Drawing.Size(106, 21);
			this.cbxZeroGrowths.TabIndex = 22;
			this.cbxZeroGrowths.Text = "0% Growths";
			this.cbxZeroGrowths.UseVisualStyleBackColor = true;
			this.cbxZeroGrowths.CheckedChanged += new System.EventHandler(this.cbxZeroGrowths_CheckedChanged);
			// 
			// cbxRandShop
			// 
			this.cbxRandShop.AutoSize = true;
			this.cbxRandShop.Location = new System.Drawing.Point(27, 335);
			this.cbxRandShop.Name = "cbxRandShop";
			this.cbxRandShop.Size = new System.Drawing.Size(145, 21);
			this.cbxRandShop.TabIndex = 23;
			this.cbxRandShop.Text = "Randomize Shops";
			this.cbxRandShop.UseVisualStyleBackColor = true;
			// 
			// cbxStatCaps
			// 
			this.cbxStatCaps.AutoSize = true;
			this.cbxStatCaps.Location = new System.Drawing.Point(508, 373);
			this.cbxStatCaps.Name = "cbxStatCaps";
			this.cbxStatCaps.Size = new System.Drawing.Size(235, 21);
			this.cbxStatCaps.TabIndex = 24;
			this.cbxStatCaps.Text = "Set Stat Caps (HP will be at 127)";
			this.cbxStatCaps.UseVisualStyleBackColor = true;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(197, 73);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(258, 17);
			this.label14.TabIndex = 26;
			this.label14.Text = "Randomization Seed (0 - 2147483647):";
			// 
			// numericSeed
			// 
			this.numericSeed.Location = new System.Drawing.Point(293, 93);
			this.numericSeed.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this.numericSeed.Name = "numericSeed";
			this.numericSeed.Size = new System.Drawing.Size(153, 22);
			this.numericSeed.TabIndex = 27;
			this.numericSeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// numericStatCap
			// 
			this.numericStatCap.Location = new System.Drawing.Point(747, 372);
			this.numericStatCap.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
			this.numericStatCap.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.numericStatCap.Name = "numericStatCap";
			this.numericStatCap.Size = new System.Drawing.Size(56, 22);
			this.numericStatCap.TabIndex = 28;
			this.numericStatCap.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// cbxAffinity
			// 
			this.cbxAffinity.AutoSize = true;
			this.cbxAffinity.Location = new System.Drawing.Point(293, 335);
			this.cbxAffinity.Name = "cbxAffinity";
			this.cbxAffinity.Size = new System.Drawing.Size(147, 21);
			this.cbxAffinity.TabIndex = 29;
			this.cbxAffinity.Text = "Randomize Affinity";
			this.cbxAffinity.UseVisualStyleBackColor = true;
			// 
			// cbxBio
			// 
			this.cbxBio.AutoSize = true;
			this.cbxBio.Location = new System.Drawing.Point(293, 362);
			this.cbxBio.Name = "cbxBio";
			this.cbxBio.Size = new System.Drawing.Size(168, 21);
			this.cbxBio.TabIndex = 30;
			this.cbxBio.Text = "Randomize Biorhythm";
			this.cbxBio.UseVisualStyleBackColor = true;
			// 
			// cbxEventItems
			// 
			this.cbxEventItems.AutoSize = true;
			this.cbxEventItems.Location = new System.Drawing.Point(27, 389);
			this.cbxEventItems.Name = "cbxEventItems";
			this.cbxEventItems.Size = new System.Drawing.Size(376, 21);
			this.cbxEventItems.TabIndex = 31;
			this.cbxEventItems.Text = "Randomize Event Items (base convos, chests, villages)";
			this.cbxEventItems.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(830, 545);
			this.Controls.Add(this.cbxEventItems);
			this.Controls.Add(this.cbxBio);
			this.Controls.Add(this.cbxAffinity);
			this.Controls.Add(this.numericStatCap);
			this.Controls.Add(this.numericSeed);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.cbxStatCaps);
			this.Controls.Add(this.cbxRandShop);
			this.Controls.Add(this.cbxZeroGrowths);
			this.Controls.Add(this.panelClass);
			this.Controls.Add(this.panelGrowths);
			this.Controls.Add(this.lblLocation);
			this.Controls.Add(this.btnLoad);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cbxGrowthRand);
			this.Controls.Add(this.cbxClassRand);
			this.Controls.Add(this.cbxSkillRand);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "LordMewtwo\'s FE 10 Randomizer v1.3.0 - 05/26/2020";
			((System.ComponentModel.ISupportInitialize)(this.numericLaguzMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLaguzMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericGrowth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericHP)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericSPD)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericATK)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLCK)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericDEF)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericMAG)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericSKL)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericRES)).EndInit();
			this.panelGrowths.ResumeLayout(false);
			this.panelGrowths.PerformLayout();
			this.panelClass.ResumeLayout(false);
			this.panelClass.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericSeed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericStatCap)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.CheckBox cbxSkillRand;
		private System.Windows.Forms.CheckBox cbxClassRand;
		private System.Windows.Forms.CheckBox cbxGrowthRand;
		private System.Windows.Forms.CheckBox cbxGaugeRand;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericLaguzMin;
		private System.Windows.Forms.NumericUpDown numericLaguzMax;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.FolderBrowserDialog folderBD;
		private System.Windows.Forms.Label lblLocation;
		private System.Windows.Forms.NumericUpDown numericGrowth;
		private System.Windows.Forms.NumericUpDown numericHP;
		private System.Windows.Forms.NumericUpDown numericSPD;
		private System.Windows.Forms.NumericUpDown numericATK;
		private System.Windows.Forms.NumericUpDown numericLCK;
		private System.Windows.Forms.NumericUpDown numericDEF;
		private System.Windows.Forms.NumericUpDown numericMAG;
		private System.Windows.Forms.NumericUpDown numericSKL;
		private System.Windows.Forms.NumericUpDown numericRES;
		private System.Windows.Forms.Panel panelGrowths;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Panel panelClass;
		private System.Windows.Forms.CheckBox cbxZeroGrowths;
		private System.Windows.Forms.CheckBox cbxRandShop;
		private System.Windows.Forms.CheckBox cbxStatCaps;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown numericSeed;
		private System.Windows.Forms.NumericUpDown numericStatCap;
		private System.Windows.Forms.ComboBox comboClassOptions;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.CheckBox cbxHerons;
		private System.Windows.Forms.CheckBox cbxAffinity;
		private System.Windows.Forms.CheckBox cbxBio;
		private System.Windows.Forms.CheckBox cbxEventItems;
	}
}

