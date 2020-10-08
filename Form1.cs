using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FE10Randomizer_v0._1
{
	public partial class Form1 : Form
	{
		// location of randomizer
		string file = System.IO.Directory.GetCurrentDirectory();

		// location of DATA/files/
		string dataLocation;

		// error flag, 0 = no error
		int errorflag = 0;

		// number of units to change
		int totalUnitNumber;

		// number of units changed to heron
		int heronNumber;

		// whether user has NTSC-U 1.0 or 1.1, or PAL
		int gameVersion;

		string chapterFile;
		System.IO.StreamWriter dataWriter;

		// create random number generator
		Random seedGenerator = new Random();
		Random random;

		// keeps track of current character
		int charNum;

		int weightflag = 0;

		// arrays that hold character data
		string[] charName = new string[86];
		string[] charChapter = new string[86];
		string[] charTier = new string[86];
		int[] charLevel = new int[86];
		string[] charRace = new string[86];
		int[] charLocation = new int[86];
		int[] charAnimation = new int[86];
		int[] charSkill = new int[86];
		int[] charSkillNum = new int[86];
		int[] charGauge = new int[86];
		int[] charGrowth = new int[86];
		int[] charBases = new int[86];
		int[] charWeapNum = new int[86];
		int[] charLevLoc = new int[86];
		int[] charPID = new int[86];
		int[] charBio = new int[86];
		int[] charFID = new int[86];
		string[] charClasstype = new string[86];

		// info for outputlog
		string[,] newSkills = new string[72,4];
		string[] towerUnits = new string[12];

		// arrays that hold new character data of random recruitment
		string[] newRace = new string[86];
		int[] newClass = new int[86];
		int[] newRecr = new int[86];
		int[] recrInverse = new int[86];
		string[] recrRace = new string[86];

		public Form1()
		{

			InitializeComponent();

			InitializeToolTips();

			LoadSettings();

			textBox1.Text = "Welcome to LordMewtwo73's FE10 Randomizer! Please load in the DATA\\files folder of an extracted iso.";
			
			comboClassOptions.SelectedIndex = 0;
			comboLord.SelectedIndex = 34;
			numericSeed.Value = seedGenerator.Next();
		}


		private void button1_Click(object sender, EventArgs e)
		{
			// disable components so user can't change properties during randomization
			button1.Enabled = false;
			btnLoad.Enabled = false;
			numericSeed.Enabled = false;
			groupBox1.Enabled = false;
			groupBox2.Enabled = false;
			groupBox3.Enabled = false;
			groupBox4.Enabled = false;
			groupBox5.Enabled = false;
			groupBox6.Enabled = false;
			groupBox7.Enabled = false;
			groupBox8.Enabled = false;
			groupBox9.Enabled = false;

			textBox1.Text = "Initializing";
			Application.DoEvents();
			initialize();

			// beginLog();

			textBox1.Text = "Checking class weights";
			Application.DoEvents();
			checkWeights();

			textBox1.Text = "Finding game version";
			Application.DoEvents();
			getVersion();

			if (errorflag == 0)
			{
				textBox1.Text = "Organizing files";
				Application.DoEvents();
				fileOrganizer();

				textBox1.Text = "Abbreviating IDs";
				Application.DoEvents();
				abbreviate();
			}


			if (cbxRandRecr.Checked == true & errorflag == 0)
			{
				textBox1.Text = "Swaping recruitment";
				Application.DoEvents();
				recruitmentOrderRando();
			}



			if (errorflag == 0)
			{
				textBox1.Text = "Changing classes";
				Application.DoEvents();
				Classes();
			}


			if (cbxRandWeap.Checked == true & errorflag == 0)
			{
				textBox1.Text = "Screwing up weapons";
				Application.DoEvents();
				weaponRandomizer();
			}


			if (errorflag == 0)
			{
				textBox1.Text = "Calculating busted stats";
				Application.DoEvents();
				Stats();
			}



			if ((cbxClassRand.Checked == true | cbxRandRecr.Checked == true | cbxRandEnemy.Checked == true | cbxRandBosses.Checked == true)
				& errorflag == 0)
			{
				textBox1.Text = "\"Balacing\" units";
				Application.DoEvents();
				baseStatChanges();
			}


			if (errorflag == 0)
			{
				textBox1.Text = "Making weird enemies";
				Application.DoEvents();
				Enemies();
			}


			if (errorflag == 0)
			{
				textBox1.Text = "Misc.";
				Application.DoEvents();
				miscRandom();
			}


			if (errorflag == 0)
			{
				textBox1.Text = "Doing... the rest";
				Application.DoEvents();
				generalChanges();
			}


			if (errorflag == 0)
			{
				textBox1.Text = "Compressing FE10Data";
				Application.DoEvents();
				closingRemarks();
				textBox1.Text = "Creating log";
				Application.DoEvents();
				outputLog();
				SaveSettings();
				textBox1.Text = "Randomization Complete! Check outputlog.htm for details";
			}

			//dataWriter.Close();
			button1.Enabled = true;
			btnLoad.Enabled = true;
			numericSeed.Enabled = true;
			groupBox1.Enabled = true;
			groupBox2.Enabled = true;
			groupBox3.Enabled = true;
			groupBox4.Enabled = true;
			groupBox5.Enabled = true;
			groupBox6.Enabled = true;
			groupBox7.Enabled = true;
			groupBox8.Enabled = true;
			groupBox9.Enabled = true;

		}

		// INITIALIZATION FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// sets tooltips for all front panel objects
		private void InitializeToolTips()
		{
			toolTip1.SetToolTip(cbxEnemHealers, "priest and bishop enemies/allies will not be randomized");
			toolTip1.SetToolTip(cbxOnlySiege, "starting in part 3, enemies randomized into magic classes will only have long-range magic (ie meteor,etc)");
			toolTip1.SetToolTip(cbxNoSiege, "enemies randomized into magic classes will not have long-range magic (ie meteor,etc)");
			toolTip1.SetToolTip(cbxWinCon, "changes 4-P, 4-1, 4-2, 4-3, and 4-Ea to \"Defeat Boss\" and 4-4 to \"Seize\"");
			toolTip1.SetToolTip(cbxFionaAI, "changes AI to prevent Fiona from dying turn 1");
			toolTip1.SetToolTip(cbxJillAI, "changes AI to prevent Jill & Co from dying turn 1");
			toolTip1.SetToolTip(cbxBKfight, "moves a starting location next to Ike in 4-Eb to help with fighting BK");
			toolTip1.SetToolTip(cbxLowerPrice, "changes to 3k per use");
			toolTip1.SetToolTip(cbxBirdVision, "changes bird laguz vision from 0 to 2");
			toolTip1.SetToolTip(cbxNihil, "guarantees a nihil skill even when Ike is not Vanguard");
			toolTip1.SetToolTip(cbxBKnerf, "makes him easier to kill");
			toolTip1.SetToolTip(cbxTowerUnits, "selects 10 characters required to enter the tower alongside the usual required 6");
			toolTip1.SetToolTip(cbxChooseIke, "allows you to select the character to replace Ike");
			toolTip1.SetToolTip(cbxEnemyRecruit, "adds various major antagonists to the recruitment pool. See README.htm for more details");
			toolTip1.SetToolTip(cbxRandRecr, "changes order in which you recieve characters. See README.htm for details regarding what stats are modified");
			toolTip1.SetToolTip(cbxGrowthRand, "randomizes each growth individually based off of original +/- deviation and limited by minimums");
			toolTip1.SetToolTip(cbxGrowthShuffle, "adds up total growths and redistributes randomly to each stat");
			toolTip1.SetToolTip(cbxGrowthCap, "sets maximum growths");
			toolTip1.SetToolTip(cbxRandBosses, "increases stats on a linear scale, where early bosses gain less than lategame bosses. does not affect BK, dheginsea, and ashera");
			toolTip1.SetToolTip(cbxBuffBosses, "does not affect BK, dheginsea, and ashera");
			toolTip1.SetToolTip(cbxNoLaguz, "laguz can have busted stats for early game, so this prevents early laguz");
			toolTip1.SetToolTip(cbxEnemyRange, "adds enemy ranges to hardmode");
			toolTip1.SetToolTip(cbxWeapTri, "adds weapon triangle to hardmode");
			toolTip1.SetToolTip(cbxMapAff, "adds map affinities to hardmode");
			toolTip1.SetToolTip(cbxStrMag, "the higher of a unit's STR/MAG growth will be put into STR if their class is physical or MAG if their class is magical (before any growth manipulation)");
			toolTip1.SetToolTip(cbxLords, "keeps Ike and Micaiah (or whoever replaces them with random recruitment) as Hero and Light Mage");
			toolTip1.SetToolTip(cbxThieves, "keeps Sothe and Heather (or whoever replaces them with random recruitment) as Rogues");
			toolTip1.SetToolTip(cbxHerons, "maximum 3 per playthrough, each functioning as either rafiel,leanne,or reyson (one of each)");
			toolTip1.SetToolTip(cbxRandBases, "randomizes each stat individually based off of deviation");
			toolTip1.SetToolTip(cbxHPLCKShuffle, "HP&LCK are usually much higher than other stats, so adding them to the total may cause units to be overpowered");
			toolTip1.SetToolTip(cbxShuffleBases, "adds up total bases (except HP&LCK) and redistributes randomly to each stat");
			toolTip1.SetToolTip(cbxSiegeUse, "keeps siege tomes, ballistae at normal uses (usually 5)");
			toolTip1.SetToolTip(cbxStaveUse, "keeps Sleep, Silence, Hammerne, Ashera Staff, etc at normal uses (usually 3)");
			toolTip1.SetToolTip(numericBaseRand, "WARNING: high base variations may result in an unwinnable game");
			toolTip1.SetToolTip(cbxRandEnemy, "1-for-1 replaces enemy and ally classes in each chapter, some restrictions apply. See README.htm for more details");
			toolTip1.SetToolTip(cbxEnemWeaps, "random enemies have a chance of having stronger weapons that usual. See PossibleEnemyWeapons.xlsx for more details");
			toolTip1.SetToolTip(cbxEnemyGrowth, "does not affect bosses");
			toolTip1.SetToolTip(cbxSpirits, "randomizes spirits in the final two chapters of the game, and allows possible spirits in all floors of the tower");
			toolTip1.SetToolTip(cbxTier3Enemies, "randomizes enemies in part 4 into player-only T3 classes with mastery skills");
			toolTip1.SetToolTip(cbxEnemDrops, "randomizes non-equipped dropable items. Also changes some items only available by stealing to be dropped");
			toolTip1.SetToolTip(numericEnemyGrowth, "WARNING: high enemy growths may result in an unwinnable game");
			toolTip1.SetToolTip(cbxGaugeRand, "randomizes how much a laguz's gauge will fill/deplete each turn/battle. WARNING: may render some laguz unusable");
			toolTip1.SetToolTip(cbxLaguzWeap, "WARNING: may render some laguz unusable (even royals!)");
			toolTip1.SetToolTip(cbxGMweaps, "Allows any unit to use special weapons: Ragnell,Cymbeline => SS; Amiti,Ettard => S; Florete => A");
			toolTip1.SetToolTip(cbxDBweaps, "Allows any unit to use special weapons: Thani => C; Caladbolg, Tarvos, Lughnasadh => D");
			toolTip1.SetToolTip(cbxEventItems, "base convos, chests, villages, events, and hidden items");
			toolTip1.SetToolTip(lblArmored, "sword/lance/axe general");
			toolTip1.SetToolTip(lblBeasts, "lion, tiger, cat, wolf");
			toolTip1.SetToolTip(lblBirds, "hawk, raven, heron (if heron checkbox is checked)");
			toolTip1.SetToolTip(lblCavalry, "sword/lance/axe/bow knight, cleric");
			toolTip1.SetToolTip(lblDragons, "red, white, and black dragons");
			toolTip1.SetToolTip(lblFlying, "pegasus, wyvern, queen");
			toolTip1.SetToolTip(lblInfantry, "myrmidon, soldier, fighter, archer, thief, hero, assassin");
			toolTip1.SetToolTip(lblMages, "wind/fire/thunder/light mage, priest, dark sage, druid, empress, chancellor");
		}

		// loads up settings from previous randomization
		private void LoadSettings()
		{
			System.Windows.Forms.CheckBox[] checkBoxes = { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths};

			System.Windows.Forms.ComboBox[] comboBoxes = { comboClassOptions, comboLord};

			System.Windows.Forms.NumericUpDown[] numericUpDowns = { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand, 
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin, 
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle, 
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4, 
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK, 
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL, 
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev, 
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin};

			System.Windows.Forms.RadioButton[] radioButtons = { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5, 
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5, 
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5, 
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5, 
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5, 
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5, 
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5, 
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};

			int fileexists;
			string settingstring = "";
			try
			{
				System.IO.StreamReader settingreader = new System.IO.StreamReader(file + "\\settings.ini");
				settingstring = settingreader.ReadLine();
				settingreader.Close();
				fileexists = 1;
			}
			catch
			{
				fileexists = 0;
			}


			if (fileexists == 1 & settingstring != "")
			{
				string[] eachsetting = settingstring.Split(',');
				int total = numericUpDowns.Length + checkBoxes.Length + comboBoxes.Length + radioButtons.Length;

				for (int i = 0; i < numericUpDowns.Length; i++)
				{
					numericUpDowns[i].Value = Convert.ToInt32(eachsetting[i]);
				}
				for (int i = 0; i < checkBoxes.Length; i++)
				{
					if (eachsetting[i + numericUpDowns.Length] == "True")
						checkBoxes[i].Checked = true;
					else
						checkBoxes[i].Checked = false;
				}
				for (int i = 0; i < comboBoxes.Length; i++)
				{
					comboBoxes[i].SelectedIndex = Convert.ToInt32(eachsetting[i + numericUpDowns.Length + checkBoxes.Length]);
				}
				for (int i = 0; i < radioButtons.Length; i++)
				{
					if (eachsetting[i + numericUpDowns.Length + checkBoxes.Length + comboBoxes.Length] == "True")
						radioButtons[i].Checked = true;
					else
						radioButtons[i].Checked = false;
				}
			}

			
		}

		// saves settings from current randomization
		private void SaveSettings()
		{
			System.Windows.Forms.CheckBox[] checkBoxes = { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths};

			System.Windows.Forms.ComboBox[] comboBoxes = { comboClassOptions, comboLord };

			System.Windows.Forms.NumericUpDown[] numericUpDowns = { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin};

			System.Windows.Forms.RadioButton[] radioButtons = { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};

			string settingstring = "";
			for (int i = 0; i < numericUpDowns.Length; i++)
			{
				settingstring += numericUpDowns[i].Value.ToString() + ",";
			}
			for (int i = 0; i < checkBoxes.Length; i++)
			{
				settingstring += checkBoxes[i].Checked.ToString() + ",";
			}
			for (int i = 0; i < comboBoxes.Length; i++)
			{
				settingstring += comboBoxes[i].SelectedIndex.ToString() + ",";
			}
			for (int i = 0; i < radioButtons.Length; i++)
			{
				settingstring += radioButtons[i].Checked.ToString() + ",";
			}
			settingstring += numericSeed.Value.ToString();

			System.IO.StreamWriter settingwriter = new System.IO.StreamWriter(file + "\\settings.ini");
			settingwriter.WriteLine(settingstring);
			settingwriter.Close();

		}

		// initializes variables and reads in characterdata
		private void initialize()
		{
			string line;
			string[] values;
			// initialize character information
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\CharacterInfo.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all characters
			for (int i = 0; i < charName.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				// character name
				charName[i] = values[0];
				// chapter first seen - where class changing will take place
				charChapter[i] = values[1];
				// what tier they are; a=tierone ie. archer, b=tiertwo ie. sniper, c=tierthree
				charTier[i] = values[2];
				// starting level of character
				charLevel[i] = Convert.ToInt32(values[3]);
				// race of character; B=beorc, L=laguz
				charRace[i] = values[4];
				// location of character's class in chapter data
				charLocation[i] = Convert.ToInt32(values[5]);
				// location of character's animation in big Data file
				charAnimation[i] = Convert.ToInt32(values[6]);
				// location of character's skills in big Data file
				charSkill[i] = Convert.ToInt32(values[7]);
				// number of skills character has
				charSkillNum[i] = Convert.ToInt32(values[8]);
				// location of character's laguz gauge in big Data file
				charGauge[i] = Convert.ToInt32(values[9]);
				// location of character's growth rates in big Data file
				charGrowth[i] = Convert.ToInt32(values[10]);
				// location of character's base stats
				charBases[i] = Convert.ToInt32(values[11]);
				// number of weapons in character's starting inventory
				charWeapNum[i] = Convert.ToInt32(values[12]);
				// location of characters level in chapter data
				charLevLoc[i] = Convert.ToInt32(values[13]);
				// location of characters PID
				charPID[i] = Convert.ToInt32(values[14]);
				// location of characters biorhythm
				charBio[i] = Convert.ToInt32(values[15]);
				// location of FID in facedata.bin
				charFID[i] = Convert.ToInt32(values[16]);
				// whether a unit is physical (p) or magical (m)
				charClasstype[i] = values[17];
			}

			// heron number
			heronNumber = 0;

			// reset error flag
			errorflag = 0;


			// set number of units to change
			if ((cbxClassRand.Checked == true & cbxHerons.Checked == true) | cbxRandRecr.Checked == true)
				totalUnitNumber = 72;
			else
				totalUnitNumber = 69;

			// generate randomizer with seed
			random = new Random(Convert.ToInt32(numericSeed.Value));
		}

		// initializes the log files with what checkboxes the user selected
		private void beginLog()
		{
			// initialize output log
			string logheader = "seed:," + numericSeed.Value.ToString();
			string logheader2 = "";
			string logheader3 = "";
			string logheader4 = "";
			if (cbxClassRand.Checked == true)
			{
				switch (comboClassOptions.SelectedIndex)
				{
					case 0:
						logheader += ",classRand = Basic";
						break;
					case 1:
						logheader += ",classRand = Full";
						break;
					case 2:
						logheader += ",classRand = Beorc";
						break;
					case 3:
						logheader += ",classRand = Laguz";
						break;
					case 4:
						logheader += ",classRand = Classic";
						break;
					case 5:
						logheader += ",classRand = Magic";
						break;
					case 6:
						logheader += ",classRand = Horse";
						break;
					case 7:
						logheader += ",classRand = Armor";
						break;
					case 8:
						logheader += ",classRand = Flying";
						break;
					case 9:
						logheader += ",classRand = Beast";
						break;
					case 10:
						logheader += ",classRand = Beauty";
						break;
					default:
						break;
				}
				if (comboClassOptions.SelectedIndex == 0 | comboClassOptions.SelectedIndex == 1)
				{
					logheader += ",infantry: ";
					if (radioInfantry0.Checked == true)
						logheader += "0";
					else if (radioInfantry1.Checked == true)
						logheader += "1";
					else if (radioInfantry2.Checked == true)
						logheader += "2";
					else if (radioInfantry3.Checked == true)
						logheader += "3";
					else if (radioInfantry4.Checked == true)
						logheader += "4";
					else
						logheader += "5";
					logheader += ",mages: ";
					if (radioMages0.Checked == true)
						logheader += "0";
					else if (radioMages1.Checked == true)
						logheader += "1";
					else if (radioMages2.Checked == true)
						logheader += "2";
					else if (radioMages3.Checked == true)
						logheader += "3";
					else if (radioMages4.Checked == true)
						logheader += "4";
					else
						logheader += "5";
					logheader += ",cavalry: ";
					if (radioCav0.Checked == true)
						logheader += "0";
					else if (radioCav1.Checked == true)
						logheader += "1";
					else if (radioCav2.Checked == true)
						logheader += "2";
					else if (radioCav3.Checked == true)
						logheader += "3";
					else if (radioCav4.Checked == true)
						logheader += "4";
					else
						logheader += "5";
					logheader += ",armor: ";
					if (radioArmor0.Checked == true)
						logheader += "0";
					else if (radioArmor1.Checked == true)
						logheader += "1";
					else if (radioArmor2.Checked == true)
						logheader += "2";
					else if (radioArmor3.Checked == true)
						logheader += "3";
					else if (radioArmor4.Checked == true)
						logheader += "4";
					else
						logheader += "5";
					logheader += ",flying: ";
					if (radioFly0.Checked == true)
						logheader += "0";
					else if (radioFly1.Checked == true)
						logheader += "1";
					else if (radioFly2.Checked == true)
						logheader += "2";
					else if (radioFly3.Checked == true)
						logheader += "3";
					else if (radioFly4.Checked == true)
						logheader += "4";
					else
						logheader += "5";
					logheader += ",beasts: ";
					if (radioBeast0.Checked == true)
						logheader += "0";
					else if (radioBeast1.Checked == true)
						logheader += "1";
					else if (radioBeast2.Checked == true)
						logheader += "2";
					else if (radioBeast3.Checked == true)
						logheader += "3";
					else if (radioBeast4.Checked == true)
						logheader += "4";
					else
						logheader += "5";
					logheader += ",birds: ";
					if (radioBird0.Checked == true)
						logheader += "0";
					else if (radioBird1.Checked == true)
						logheader += "1";
					else if (radioBird2.Checked == true)
						logheader += "2";
					else if (radioBird3.Checked == true)
						logheader += "3";
					else if (radioBird4.Checked == true)
						logheader += "4";
					else
						logheader += "5";
					logheader += ",dragons: ";
					if (radioDragon0.Checked == true)
						logheader += "0";
					else if (radioDragon1.Checked == true)
						logheader += "1";
					else if (radioDragon2.Checked == true)
						logheader += "2";
					else if (radioDragon3.Checked == true)
						logheader += "3";
					else if (radioDragon4.Checked == true)
						logheader += "4";
					else
						logheader += "5";
				}

				if (cbxHerons.Checked == true)
					logheader += ",heronRand";
				if (cbxLords.Checked == true)
					logheader += ",keepLords";
				if (cbxThieves.Checked == true)
					logheader += ",keepThieves";
				if (cbxStrMag.Checked == true)
					logheader += ",prioritize str/mag";
				if (cbxGaugeRand.Checked == true)
					logheader += ",transGauge Min=" + numericLaguzMin1.Value.ToString() + " Max=" + numericLaguzMax1.Value.ToString();
			}

			if (cbxGrowthRand.Checked == true)
			{
				logheader2 += ",growthRand Dev=" + numericGrowth.Value.ToString();
				logheader2 += ",hpMin=" + numericHP.Value.ToString();
				logheader2 += ",atkMin=" + numericATK.Value.ToString();
				logheader2 += ",magMin=" + numericMAG.Value.ToString();
				logheader2 += ",sklMin=" + numericSKL.Value.ToString();
				logheader2 += ",spdMin=" + numericSPD.Value.ToString();
				logheader2 += ",lckMin=" + numericLCK.Value.ToString();
				logheader2 += ",defMin=" + numericDEF.Value.ToString();
				logheader2 += ",resMin=" + numericRES.Value.ToString();
			}

			if (cbxRandWeap.Checked == true)
			{
				logheader3 += ",randweap";
				logheader3 += ",MT min=" + numericMTmin.Value.ToString() + ",dev=" + numericMTdev.Value.ToString() + ",max=" + numericMTmax.Value.ToString();
				logheader3 += ",ACC min=" + numericACCmin.Value.ToString() + ",dev=" + numericACCdev.Value.ToString() + ",max=" + numericACCmax.Value.ToString();
				logheader3 += ",CRT min=" + numericCRTmin.Value.ToString() + ",dev=" + numericCRTdev.Value.ToString() + ",max=" + numericCRTmax.Value.ToString();
				logheader3 += ",WT min=" + numericWTmin.Value.ToString() + ",dev=" + numericWTdev.Value.ToString() + ",max=" + numericWTmax.Value.ToString();
				logheader3 += ",USE min=" + numericUSEmin.Value.ToString() + ",dev=" + numericUSEdev.Value.ToString() + ",max=" + numericUSEmax.Value.ToString();
				if (cbxLaguzWeap.Checked == true)
					logheader3 += ",randLaguzWeap";
				if (cbxStaveUse.Checked == true)
					logheader3 += ",keepStaveUse";
			}

			if (cbxRandBases.Checked == true)
				logheader4 += ",randBaseStats-MaxDev=" + numericBaseRand.Value.ToString();
			if (cbxShuffleBases.Checked == true)
			{
				logheader4 += ",shuffleBaseStats-Addition=" + numericBaseShuffle.Value.ToString();
				if (cbxHPLCKShuffle.Checked == true)
					logheader4 += ",HP/LCKshuffle";
			}
			if (cbxEnemyGrowth.Checked == true)
				logheader4 += ",enemyGrowth-MaxIncrease=" + numericEnemyGrowth.Value.ToString();
			if (cbxRandShop.Checked == true)
				logheader4 += ",randShop";
			if (cbxAffinity.Checked == true)
				logheader4 += ",randAffinity";
			if (cbxBio.Checked == true)
				logheader4 += ",randBio";
			if (cbxSkillRand.Checked == true)
				logheader4 += ",randSkill";
			if (cbxEventItems.Checked == true)
				logheader4 += ",randEventItems";
			if (cbxRandRecr.Checked == true)
				logheader4 += ",randRecruitment";
			if (cbxZeroGrowths.Checked == true)
				logheader4 += ",zeroGrowth";
			if (cbxFlorete.Checked == true)
				logheader4 += ",magicFlorete";
			if (cbxGMweaps.Checked == true)
				logheader4 += ",removedPRFs";
			if (cbxWeapCaps.Checked == true)
				logheader4 += ",noWeapCaps";
			if (cbxEnemyRange.Checked == true)
				logheader4 += ",HM enemy ranges";
			if (cbxWeapTri.Checked == true)
				logheader4 += ",HM weap triangle";
			if (cbxMapAff.Checked == true)
				logheader4 += ",HM map affinity";
			if (cbxStatCaps.Checked == true)
			{
				logheader4 += ",statCaps:";
				logheader4 += " T1=" + numericStatCap1.Value.ToString();
				logheader4 += " T2=" + numericStatCap2.Value.ToString();
				logheader4 += " T3=" + numericStatCap3.Value.ToString();
			}

			try
			{
				dataWriter = new System.IO.StreamWriter(file + "\\outputlog.csv");
				dataWriter.WriteLine(logheader);
				if (logheader2 != "")
					dataWriter.WriteLine(logheader2);
				if (logheader3 != "")
					dataWriter.WriteLine(logheader3);
				if (logheader4 != "")
					dataWriter.WriteLine(logheader4);

				dataWriter.WriteLine("Name,NewName,Race,Class,LGaugeTurn,LGaugeBattle,LGaugeTurn,LGaugeBattle," +
					"Skills1,Skills2,Skills3,Skills4,Growths:HP,STR,MAG,SKL,SPD,LCK,DEF,RES,Bases:HP,STR,MAG,SKL,SPD,LCK,DEF,RES");
			}
			catch
			{
				textBox1.Text = "outputlog.csv currently open in another program! Abandoning Randomization...";
				errorflag = 1;
			}

		}

		// check to make sure class weights are all valid
		private void checkWeights()
		{
			// check to make sure at least one class weight overall is greater than 0
			if (cbxClassRand.Checked == true & (
				(radioBeast0.Checked == true & radioBird0.Checked == true & radioDragon0.Checked == true) &
				(radioArmor0.Checked == true & radioCav0.Checked == true & radioFly0.Checked == true & radioInfantry0.Checked == true & radioMages0.Checked == true)))
			{
				textBox1.Text = "At least one class type must have a non-zero weight! Abandoning Randomization...";
				errorflag = 1;
			}
			// check to make sure at least one not-dragon class weight overall is greater than 0
			if (cbxClassRand.Checked == true & (
				(radioBeast0.Checked == true & radioBird0.Checked == true & radioDragon0.Checked == false) &
				(radioArmor0.Checked == true & radioCav0.Checked == true & radioFly0.Checked == true & radioInfantry0.Checked == true & radioMages0.Checked == true)))
			{
				textBox1.Text = "Early game beorc cannot turn into dragons. Please select at least one other class type to have a non-zero weight.";
				errorflag = 1;
			}
			// check to make sure at least one class weight is greater than 0 for both races
			if (cbxClassRand.Checked == true & comboClassOptions.SelectedIndex == 0 & (
				(radioBeast0.Checked == true & radioBird0.Checked == true & radioDragon0.Checked == true) |
				(radioArmor0.Checked == true & radioCav0.Checked == true & radioFly0.Checked == true & radioInfantry0.Checked == true & radioMages0.Checked == true)))
			{
				textBox1.Text = "'Basic' class randomization is impossible without both laguz and beorc classes! Abandoning Randomization...";
				errorflag = 1;
			}

			if (cbxRandRecr.Checked == true & cbxClassRand.Checked == true & comboClassOptions.SelectedIndex == 0 &
				(radioBeast0.Checked == true & radioBird0.Checked == true))
			{
				textBox1.Text = "Early game units cannot turn into dragons. Please select at least one other laguz class type to have a non-zero weight.";
				errorflag = 1;
			}
		}

		// gets version number of selected radiant dawn iso
		private void getVersion()
		{
			string mainFile = dataLocation.Remove(dataLocation.Length - 5, 5) + "sys\\main.dol";
			try
			{
				// open main.dol
				using (var stream = new System.IO.FileStream(mainFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					stream.Position = 31;
					int versionbyte = Convert.ToInt32(stream.ReadByte());
					if (versionbyte == 224)
						gameVersion = 0;
					else if (versionbyte == 64)
						gameVersion = 1;
					else if (versionbyte == 160)
						gameVersion = 2;
					else
					{
						textBox1.Text = "Game version unknown. Abandoning randomization...";
						errorflag = 1;
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 25: Cannot find DATA/sys/main.dol. Abandoning randomization...";
				errorflag = 1;
			}
		}

		// moves edited shop, weapon, data, .cms, and chapter files
		private void fileOrganizer()
		{
			string sourcePath, targetPath, sourcefile, targetfile;

			// FE10Data
			if (cbxRandBosses.Checked | (cbxRandRecr.Checked == true & cbxEnemyRecruit.Checked == true) | cbxRandEnemy.Checked == true)
				sourcePath = file + "\\assets\\gameData\\FE10Data.cms.decompressed";
			else
				sourcePath = file + "\\assets\\FE10Data.cms.decompressed";
			targetPath = dataLocation + "\\FE10Data.cms.decompressed";
			try
			{
				System.IO.File.Copy(sourcePath, targetPath, true);
				sourcePath = file + "\\assets\\FE10Data.cms";
				targetPath = dataLocation + "\\FE10Data.cms";
				System.IO.File.Copy(sourcePath, targetPath, true);
			}
			catch
			{
				textBox1.Text = "Error 01: Game files not found! Abandoning Randomization...";
				errorflag = 1;
			}

			// shop stuff
			sourcePath = file + "\\assets\\shopdata\\";
			targetPath = dataLocation + "\\Shop\\";
			try
			{
				sourcefile = sourcePath + "shopitem_h.bin";
				targetfile = targetPath + "shopitem_h.bin";
				System.IO.File.Copy(sourcefile, targetfile, true);

				sourcefile = sourcePath + "shopitem_m.bin";
				targetfile = targetPath + "shopitem_m.bin";
				System.IO.File.Copy(sourcefile, targetfile, true);
			}
			catch
			{
				textBox1.Text = "Error 01: Game files not found! Abandoning Randomization...";
				errorflag = 1;
			}

			// forge stuff
			sourcePath = file + "\\assets\\forgedata\\";
			targetPath = dataLocation + "\\xwp\\forge\\";
			foreach (string path in System.IO.Directory.GetFiles(sourcePath))
			{
				System.IO.File.Copy(path, targetPath + path.Substring(path.Length - 10, 10), true);
			}

			// random character,class,and abbreviated stuff
			//if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true | cbxRandEnemy.Checked == true | cbxRandBosses.Checked == true)
			//{
				// moves edited dispos_h files to proper folders
				sourcePath = file + "\\assets\\chapterdata\\";
				targetPath = dataLocation + "\\zmap\\";

				// part 1
				for (int i = 1; i < 10; i++)
				{
					sourcefile = sourcePath + "dispos_h1_" + i.ToString() + ".bin";
					targetfile = targetPath + "bmap010" + i.ToString() + "\\dispos_h.bin";
					try
					{
						System.IO.File.Copy(sourcefile, targetfile, true);
					}
					catch
					{
						textBox1.Text = "Error 01: Game files not found! Abandoning Randomization...";
						errorflag = 1;
					}
				}
				sourcefile = sourcePath + "dispos_h1_11.bin";
				targetfile = targetPath + "bmap0111\\dispos_h.bin";
				try
				{
					System.IO.File.Copy(sourcefile, targetfile, true);
				}
				catch
				{
					textBox1.Text = "Error 01: Game files not found! Abandoning Randomization...";
					errorflag = 1;
				}
			if (errorflag == 0)
			{
				// part 2
				for (int i = 1; i < 6; i++)
				{
					sourcefile = sourcePath + "dispos_h2_" + i.ToString() + ".bin";
					targetfile = targetPath + "bmap020" + i.ToString() + "\\dispos_h.bin";
					System.IO.File.Copy(sourcefile, targetfile, true);
				}

				// part 3
				for (int i = 1; i < 10; i++)
				{
					sourcefile = sourcePath + "dispos_h3_" + i.ToString() + ".bin";
					targetfile = targetPath + "bmap030" + i.ToString() + "\\dispos_h.bin";
					try
					{
						System.IO.File.Copy(sourcefile, targetfile, true);
					}
					catch { }
				}
				for (int i = 0; i < 6; i++)
				{
					sourcefile = sourcePath + "dispos_h3_1" + i.ToString() + ".bin";
					targetfile = targetPath + "bmap031" + i.ToString() + "\\dispos_h.bin";
					try
					{
						System.IO.File.Copy(sourcefile, targetfile, true);
					}
					catch { }
				}

				// part 4
				for (int i = 1; i < 7; i++)
				{
					sourcefile = sourcePath + "dispos_h4_" + i.ToString() + ".bin";
					targetfile = targetPath + "bmap040" + i.ToString() + "\\dispos_h.bin";
					try
					{
						System.IO.File.Copy(sourcefile, targetfile, true);
					}
					catch { }
				}

				// finale
				sourcefile = sourcePath + "dispos_h4_7.bin";
				targetfile = targetPath + "emap0407c\\dispos_c.bin";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "dispos_h4_7a.bin";
				targetfile = targetPath + "bmap0407a\\dispos_h.bin";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "dispos_h4_7b.bin";
				targetfile = targetPath + "bmap0407b\\dispos_h.bin";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "dispos_h4_7c.bin";
				targetfile = targetPath + "bmap0407c\\dispos_h.bin";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "dispos_h4_7d.bin";
				targetfile = targetPath + "bmap0407d\\dispos_h.bin";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "dispos_h4_7e.bin";
				targetfile = targetPath + "bmap0407e\\dispos_h.bin";
				System.IO.File.Copy(sourcefile, targetfile, true);


			}
		  //}


			// moves abbreviated cms files to proper folders
			sourcePath = file + "\\assets\\gamedata\\";
			targetPath = dataLocation + "\\";
			try
			{
				sourcefile = sourcePath + "cp_data.cms";
				targetfile = targetPath + "Cp\\cp_data.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "FE10Anim.cms";
				targetfile = targetPath + "FE10Anim.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "FE10Battle.cms";
				targetfile = targetPath + "FE10Battle.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "FE10Conversation.cms";
				targetfile = targetPath + "FE10Conversation.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "FE10Effect.cms";
				targetfile = targetPath + "FE10Effect.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "FE10Growth.cms";
				targetfile = targetPath + "FE10Growth.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
			}
			catch
			{
				textBox1.Text = "Error 01: Game files not found! Abandoning Randomization...";
				errorflag = 1;
			}

			if (cbxWinCon.Checked == true)
			{
				// moves edited script files to proper folder
				sourcePath = file + "\\assets\\scriptdata\\";
				targetPath = dataLocation + "\\Scripts\\";

				foreach (string path in System.IO.Directory.GetFiles(sourcePath))
				{
					string[] scriptfile = path.Split('\\');
					System.IO.File.Copy(path, targetPath + scriptfile[scriptfile.Length-1], true);
				}
			}

			/*
			if (cbxEventItems.Checked == true)
			{
				// moves edited script files to proper folders
				 sourcePath = file + "\\assets\\scriptdata\\";
				 targetPath = dataLocation + "\\Scripts\\";

				// part 1
				for (int i = 2; i < 11; i++)
				{
					if (i < 10)
					{
						sourcefile = sourcePath + "C010" + i.ToString() + ".cmb";
						targetfile = targetPath + "C010" + i.ToString() + ".cmb";
					}
					else
					{
						sourcefile = sourcePath + "C01" + i.ToString() + ".cmb";
						targetfile = targetPath + "C01" + i.ToString() + ".cmb";
					}
					try
					{
						System.IO.File.Copy(sourcefile, targetfile, true);
					}
					catch
					{
						textBox1.Text = "Error 01: Game files not found! Abandoning Randomization...";
						errorflag = 1;
					}
				}
				if (errorflag == 0)
				{
					// part 2
					for (int i = 2; i < 6; i++)
					{
						sourcefile = sourcePath + "C020" + i.ToString() + ".cmb";
						targetfile = targetPath + "C020" + i.ToString() + ".cmb";
						System.IO.File.Copy(sourcefile, targetfile, true);
					}

					// part 3
					for (int i = 1; i < 16; i++)
					{
						if (i < 10)
						{
							sourcefile = sourcePath + "C030" + i.ToString() + ".cmb";
							targetfile = targetPath + "C030" + i.ToString() + ".cmb";
						}
						else
						{
							sourcefile = sourcePath + "C03" + i.ToString() + ".cmb";
							targetfile = targetPath + "C03" + i.ToString() + ".cmb";
						}
						try
						{
							System.IO.File.Copy(sourcefile, targetfile, true);
						}
						catch { }
					}

					// part 4
					for (int i = 1; i < 7; i++)
					{
						sourcefile = sourcePath + "C040" + i.ToString() + ".cmb";
						targetfile = targetPath + "C040" + i.ToString() + ".cmb";
						try
						{
							System.IO.File.Copy(sourcefile, targetfile, true);
						}
						catch { }
					}

					// finale
					sourcefile = sourcePath + "C0407a.cmb";
					targetfile = targetPath + "C0407a.cmb";
					System.IO.File.Copy(sourcefile, targetfile, true);
					sourcefile = sourcePath + "C0407c.cmb";
					targetfile = targetPath + "C0407c.cmb";
					System.IO.File.Copy(sourcefile, targetfile, true);
				}
			}
			*/
			// moves edited shop files to proper folders

		}

		// abbreviates jid, pid, and iids to be consistent lengths
		private void abbreviate()
		{
			string folder = dataLocation.Remove(dataLocation.Length - 5, 5);

			string line;
			string[] values;

			// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% PID %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			string[] filename = new string[3197];
			int[] location = new int[3197];
			string[] newname = new string[3197];
			string[] oldname = new string[3197];


			// initialize character information
			string infofile;
			infofile = file + "\\assets\\ID_data\\PIDs" + gameVersion.ToString() + ".csv";

			System.IO.StreamReader dataReader = new System.IO.StreamReader(infofile);


			for (int i = 0; i < filename.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				filename[i] = values[0];
				location[i] = Convert.ToInt32(values[1]);
				newname[i] = values[2];
				oldname[i] = values[3];
			}

			for (int i = 0; i < filename.Length; i++)
			{
				using (var stream = new System.IO.FileStream(folder + filename[i], System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					while (true)
					{
						stream.Position = location[i] + 4;
						if (newname[i] == "-")
						{
							stream.Position += 3;
						}
						else
						{
							byte[] inputname = System.Text.Encoding.ASCII.GetBytes(newname[i]);
							for (int j = 0; j < 3; j++)
								stream.WriteByte(inputname[j]);
						}
						stream.WriteByte(0);

						if (i != filename.Length - 1)
						{
							if (filename[i + 1] == filename[i])
								i++;
							else
								break;
						}
						else
							break;
					}
				}
			}

			// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% JID %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			string[] filename2 = new string[3093];
			int[] location2 = new int[3093];
			string[] newname2 = new string[3093];
			string[] oldname2 = new string[3093];


			// initialize class information
			infofile = file + "\\assets\\ID_data\\JIDs" + gameVersion.ToString() + ".csv";

			System.IO.StreamReader dataReader2 = new System.IO.StreamReader(infofile);


			for (int i = 0; i < filename2.Length; i++)
			{
				line = dataReader2.ReadLine();
				values = line.Split(',');
				filename2[i] = values[0];
				location2[i] = Convert.ToInt32(values[1]);
				newname2[i] = values[2];
				oldname2[i] = values[3];
			}

			for (int i = 0; i < filename2.Length; i++)
			{
				using (var stream = new System.IO.FileStream(folder + filename2[i], System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					while (true)
					{
						stream.Position = location2[i] + 4;
						if (newname2[i] == "-")
						{
							stream.Position += 3;
						}
						else
						{
							byte[] inputname = System.Text.Encoding.ASCII.GetBytes(newname2[i]);
							for (int j = 0; j < 3; j++)
								stream.WriteByte(inputname[j]);
						}
						stream.WriteByte(0);

						if (i != filename2.Length - 1)
						{
							if (filename2[i + 1] == filename2[i])
								i++;
							else
								break;
						}
						else
							break;
					}
				}
			}

			// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% IID %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			string[] filename3 = new string[4431];
			int[] location3 = new int[4431];
			string[] newname3 = new string[4431];
			string[] oldname3 = new string[4431];


			// initialize item information
			infofile = file + "\\assets\\ID_data\\IIDs" + gameVersion.ToString() + ".csv";

			System.IO.StreamReader dataReader3 = new System.IO.StreamReader(infofile);


			for (int i = 0; i < filename3.Length; i++)
			{
				line = dataReader3.ReadLine();
				values = line.Split(',');
				filename3[i] = values[0];
				location3[i] = Convert.ToInt32(values[1]);
				newname3[i] = values[2];
				oldname3[i] = values[3];
			}

			for (int i = 0; i < filename3.Length; i++)
			{
				using (var stream = new System.IO.FileStream(folder + filename3[i], System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					while (true)
					{
						stream.Position = location3[i] + 4;
						if (newname3[i] == "-")
						{
							stream.Position += 4;
						}
						else
						{
							byte[] inputname = System.Text.Encoding.ASCII.GetBytes(newname3[i]);
							for (int j = 0; j < 4; j++)
								stream.WriteByte(inputname[j]);
						}
						if (oldname3[i].Contains("_UFX"))
						{
							byte[] ufx = System.Text.Encoding.ASCII.GetBytes("_UFX");
							for (int j = 0; j < 4; j++)
								stream.WriteByte(ufx[j]);
						}
						if (oldname3[i].Contains("/L"))
						{
							byte[] slashL = System.Text.Encoding.ASCII.GetBytes("/L");
							for (int j = 0; j < 2; j++)
								stream.WriteByte(slashL[j]);
						}
						if (oldname3[i].Contains("/D"))
						{
							byte[] slashD = System.Text.Encoding.ASCII.GetBytes("/D");
							for (int j = 0; j < 2; j++)
								stream.WriteByte(slashD[j]);
						}
						stream.WriteByte(0);

						if (i != filename3.Length - 1)
						{
							if (filename3[i + 1] == filename3[i])
								i++;
							else
								break;
						}
						else
							break;
					}
				}
			}

			dataReader.Close();
			dataReader2.Close();
			dataReader3.Close();

		}



		// RECRUITMENT FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// randomizes recruitment
		private void recruitmentOrderRando()
		{
			string line;
			string[] values;

			string[] recrName = new string[86];
			// loc,loc2,class(4),level,mpid(4),mnpid(4),fid(4),aff(4),bio,auth,laguz(4),growths(8),animations(16)
			int[,] allRecrData = new int[recrName.Length, 53];

			System.IO.StreamReader dataReader;
			dataReader = new System.IO.StreamReader(file + "\\assets\\RandoRecruitData.csv");
			// initialize character information
			//if (cbxClassRand.Checked == true)
			//dataReader = new System.IO.StreamReader(file + "\\assets\\RecruitmentInfo-BaseModified.csv");
			//else
			//	dataReader = new System.IO.StreamReader(file + "\\assets\\RecruitmentInfo-Clean.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all
			for (int i = 0; i < recrName.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				// character name
				recrName[i] = values[0];
				// uhh... everything else
				for (int j = 2; j < values.Length; j++)
					allRecrData[i, j - 2] = Convert.ToInt32(values[j]);
			}
			int totalchars = 72;
			if (cbxEnemyRecruit.Checked == true)
			{
				if (cbxClassRand.Checked == true)
					totalchars = 86;
				else
					totalchars = 84;
			}


			newRecr = Enumerable.Range(0, totalchars).ToArray();

			// randomize recruitment order
			for (int i = 0; i < totalchars; i++)
			{
				int j = random.Next(i, totalchars);
				recrInverse[newRecr[i]] = j;
				recrInverse[newRecr[j]] = i;
				int temp = newRecr[i];
				newRecr[i] = newRecr[j];
				newRecr[j] = temp;
			}

			int getout = 0;
			int tryagain;
			// this is gonna be a long loop kids, hang on
			while (getout == 0)
			{
				tryagain = -1;

				if (newRecr[14] == 59 | newRecr[14] == 60) // bastian and volke are fucky and can't replace tormod
					tryagain = 14;

				else if (newRecr[18] == 59 | newRecr[18] == 60) // can't replace elincia either
					tryagain = 18;

				else if (newRecr[11] == 59 | newRecr[11] == 60) // or tauroneo
					tryagain = 11;

				else if (newRecr[23] == 59 | newRecr[23] == 60) // or nephenee
					tryagain = 23;

				else if (newRecr[25] == 59 | newRecr[25] == 60) // or lucia
					tryagain = 25;

				else if (newRecr[28] == 59 | newRecr[28] == 60) // orrrrr geoffrey
					tryagain = 28;

				else if (newRecr[55] == 59 | newRecr[55] == 60) // tibarn, too
					tryagain = 55;

				else if (newRecr[0] == 59 | newRecr[0] == 60) // prob not micaiah either, just in case
					tryagain = 0;

				else if (newRecr[34] == 59 | newRecr[34] == 60) // fuck, might as well prevent ike too
					tryagain = 34;
				else if (recrInverse[59] > 71) // prevent bastian from enemy bosses
					tryagain = recrInverse[59];
				else if (recrInverse[60] > 71) // prevent volke from enemy bosses
					tryagain = recrInverse[60];

				else if (cbxClassRand.Checked == false) // if classes are randomized, these don't matter
				{
					// micaiah can't be a priest/heron
					if (newRecr[0] == 4 | newRecr[0] == 43 | newRecr[0] == 58 | newRecr[0] == 69 | newRecr[0] == 70 |  // laura,rhys,oliver,rafiel,leanne
						newRecr[0] == 71 | newRecr[0] == 75 | newRecr[0] == 76 | newRecr[0] == 78 | newRecr[0] == 80)  // reyson,valtome,numida,hetzel,lekain
						tryagain = 0;

					// brom/nephenee can't be herons
					else if (newRecr[22] == 69 | newRecr[22] == 69 | newRecr[22] == 69)
						tryagain = 22;
					else if (newRecr[23] == 69 | newRecr[23] == 69 | newRecr[23] == 69)
						tryagain = 23;

					// ike can't be a heron, rogue, magic, or mounted... there's definitely a better way to do this but im lazy and this was easy
					else if ((newRecr[34] == 0 | newRecr[34] == 4 | newRecr[34] == 5 | newRecr[34] == 6 | newRecr[34] == 13 |    //micaiah,laura,sothe,ilyana,fiona
							newRecr[34] == 14 | newRecr[34] == 24 | newRecr[34] == 28 | newRecr[34] == 29 | newRecr[34] == 30 | //tormod,heather,geoffrey,kieran,astrid
							newRecr[34] == 31 | newRecr[34] == 33 | newRecr[34] == 35 | newRecr[34] == 36 | newRecr[34] == 37 | //makalov,calill,titania,soren,mist
							newRecr[34] == 40 | newRecr[34] == 43 | newRecr[34] == 54 | newRecr[34] == 56 | newRecr[34] == 58 | //oscar,rhys,sanaki,pelleas,oliver
							newRecr[34] == 65 | newRecr[34] == 68 | newRecr[34] == 69 | newRecr[34] == 70 | newRecr[34] == 71 | //renning,lehran,rafiel,leanne,reyson
							newRecr[34] == 75 | newRecr[34] == 76 | newRecr[34] == 77 | newRecr[34] == 78 | newRecr[34] == 80 | newRecr[34] == 83)) //valtome,numida,izuka,hetzel,lekain,sephiran
						tryagain = 34;

					// ranulf can't be mounted
					else if ((newRecr[45] == 13 | newRecr[45] == 28 | newRecr[45] == 29 | newRecr[45] == 30 | //fiona,geoffrey,kieran,astrid
							newRecr[45] == 31 | newRecr[45] == 35 | newRecr[45] == 37 | newRecr[45] == 40 | newRecr[45] == 65)) //makalov,titania,mist,oscar,renning
						tryagain = 45;

					// mist cannot replace a t1 character due to her class
					else if (charTier[recrInverse[37]] == "a")
						tryagain = recrInverse[37];

					// pelleas cannot replace a t1 character due to his class
					else if (charTier[recrInverse[56]] == "a")
						tryagain = recrInverse[56];

					// ike cannot replace a t1 character due to his class
					else if (charTier[recrInverse[34]] == "a")
						tryagain = recrInverse[34];

					// volke cannot replace a t1/2 character due to his class
					else if (charTier[recrInverse[60]] == "a" | charTier[recrInverse[60]] == "b")
						tryagain = recrInverse[60];

					// lehran cannot replace a t1/2 character due to his class
					else if (charTier[recrInverse[68]] == "a" | charTier[recrInverse[68]] == "b")
						tryagain = recrInverse[68];

					// elincia cannot replace a t1/2 character due to her class
					else if (charTier[recrInverse[18]] == "a" | charTier[recrInverse[18]] == "b")
						tryagain = recrInverse[18];

					// sanaki cannot replace a t1/2 character due to her class
					else if (charTier[recrInverse[54]] == "a" | charTier[recrInverse[54]] == "b")
						tryagain = recrInverse[54];

					// herons can't replace t3/4 characters
					else if (charTier[recrInverse[69]] == "c" | charTier[recrInverse[69]] == "d")
						tryagain = recrInverse[69];

					else if (charTier[recrInverse[70]] == "c" | charTier[recrInverse[70]] == "d")
						tryagain = recrInverse[70];

					else if (charTier[recrInverse[71]] == "c" | charTier[recrInverse[71]] == "d")
						tryagain = recrInverse[71];
					else if (cbxEnemyRecruit.Checked == true)
					{
						// izuka cannot replace a t1 character due to his class
						if (charTier[recrInverse[77]] == "a")
							tryagain = recrInverse[7];
						// sephiran cannot replace a t1/2 character due to her class
						else if (charTier[recrInverse[83]] == "a" | charTier[recrInverse[83]] == "b")
							tryagain = recrInverse[83];
					}
				}



				if (tryagain >= 0)
				// something is wrong, roll a new random recruit for this slot
				{
					int j = random.Next(totalchars);
					recrInverse[newRecr[tryagain]] = j;
					recrInverse[newRecr[j]] = tryagain;
					int temp = newRecr[tryagain];
					newRecr[tryagain] = newRecr[j];
					newRecr[j] = temp;
				}
				// everything else works, now let's make sure ike is who is picked
				else if ((cbxClassRand.Checked == true & cbxChooseIke.Checked == true &
						comboLord.SelectedIndex != 59 & comboLord.SelectedIndex != 60) & (newRecr[34] != comboLord.SelectedIndex))
				{
					int current = newRecr[34];
					int desired = comboLord.SelectedIndex;
					int otherslot = recrInverse[desired];
					newRecr[otherslot] = current;
					newRecr[34] = desired;
					recrInverse[current] = otherslot;
					recrInverse[desired] = 34;

					// then we have to go back through the loop to make sure that didn't screw anything up
				}
				else // sweet release
					getout = 1;
			}

			// go into data file and move things around
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{

					// loc,loc2,class(4),level,mpid(4),mnpid(4),fid(4),aff(4),bio,auth,laguz(4),growths(8)

					for (int charNum = 0; charNum < totalchars; charNum++)
					{
						// mpid
						stream.Position = allRecrData[charNum, 0];
						stream.Position += 4;
						for (int j = 0; j < 4; j++)
						{
							stream.WriteByte(Convert.ToByte(allRecrData[newRecr[charNum], 7 + j]));
						}
						// mnpid
						for (int j = 0; j < 4; j++)
						{
							stream.WriteByte(Convert.ToByte(allRecrData[newRecr[charNum], 11 + j]));
						}
						// fid
						for (int j = 0; j < 4; j++)
						{
							stream.Position += 1; //stream.WriteByte(Convert.ToByte(allRecrData[newRecr[charNum], 15 + j]));
						}
						// affinity
						stream.Position += 4;
						for (int j = 0; j < 4; j++)
						{
							stream.WriteByte(Convert.ToByte(allRecrData[newRecr[charNum], 19 + j]));
						}
						// animations
						if (cbxClassRand.Checked != true)
						{
							stream.Position = allRecrData[charNum, 1] - 48;
							for (int j = 0; j < 16; j++)
							{
								stream.WriteByte(Convert.ToByte(allRecrData[newRecr[charNum], 37 + j]));
							}
						}
						// biorhythm
						stream.Position = allRecrData[charNum, 1] - 32;
						stream.WriteByte(Convert.ToByte(allRecrData[newRecr[charNum], 23]));
						// authority
						stream.Position += 3;
						stream.WriteByte(Convert.ToByte(allRecrData[newRecr[charNum], 24]));
						// laguz gauge
						for (int j = 0; j < 4; j++)
						{
							if (charNum < 72)
								stream.WriteByte(Convert.ToByte(allRecrData[newRecr[charNum], 25 + j]));
							else
								stream.WriteByte(0x01);
						}
						// growths
						if (charNum < 72) // growths only need to change for playable characters
						{
							stream.Position += 10;
							for (int j = 0; j < 8; j++)
							{
								stream.WriteByte(Convert.ToByte(allRecrData[newRecr[charNum], 29 + j]));
							}
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 18: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}

			// save new character and class
			for (int charNum = 0; charNum < totalchars; charNum++)
			{
				if (charTier[charNum] == "a")
					newClass[charNum] = allRecrData[newRecr[charNum], 2];
				else if (charTier[charNum] == "b")
					newClass[charNum] = allRecrData[newRecr[charNum], 3];
				else if (charTier[charNum] == "c")
					newClass[charNum] = allRecrData[newRecr[charNum], 4];
				else
					newClass[charNum] = allRecrData[newRecr[charNum], 5];

				//charName[charNum] = recrName[newRecr[charNum]];
				//charPID[charNum] = recrLoc[newRecr[charNum]];
				recrRace[charNum] = charRace[newRecr[charNum]];

			}


			// face-swappers anonymous
			try
			{
				int[,] facedata = new int[totalchars, 20];
				// open facedata file
				using (var stream = new System.IO.FileStream(dataLocation + "\\Face\\wide\\facedata.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// read in all face data
					for (int i = 0; i < totalchars; i++)
					{
						stream.Position = charFID[i] + 20;
						for (int j = 0; j < 20; j++)
							facedata[i, j] = stream.ReadByte();
					}

					// swap that data baby
					for (int charNum = 0; charNum < totalchars; charNum++)
					{
						stream.Position = charFID[charNum] + 20;
						for (int j = 0; j < 20; j++)
							stream.WriteByte(Convert.ToByte(facedata[newRecr[charNum], j]));
					}

					// micaiah2
					stream.Position = 6464 + 20;
					for (int i = 0; i < 20; i++)
						stream.WriteByte(Convert.ToByte(facedata[newRecr[0], i]));
					// micaiah3
					stream.Position = 6608 + 20;
					for (int i = 0; i < 20; i++)
						stream.WriteByte(Convert.ToByte(facedata[newRecr[0], i]));

					// sothe2
					stream.Position = 224 + 20;
					for (int i = 0; i < 20; i++)
						stream.WriteByte(Convert.ToByte(facedata[newRecr[5], i]));

					// lucia2
					stream.Position = 7664 + 20;
					for (int i = 0; i < 20; i++)
						stream.WriteByte(Convert.ToByte(facedata[newRecr[25], i]));

					// ike2
					stream.Position = 3344 + 20;
					for (int i = 0; i < 20; i++)
						stream.WriteByte(Convert.ToByte(facedata[newRecr[34], i]));
				}
			}
			catch
			{
				textBox1.Text = "Error 18: Cannot find portrait files! Abandoning randomization...";
				errorflag = 1;
			}
		}



		// CLASS FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// class hub, controls what functions are run
		private void Classes()
		{
			// modify character classes/weapons
			if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
			{
				classChanger();

				// change laguz gauges for new classes
				laguzModifications();

				// change animations to new randomized classes
				animationChanger();

				if (cbxClassRand.Checked == true)
				{
					// uhh yes very important
					if (comboClassOptions.SelectedIndex == 10)
						veryImportantFunction();
				}
			}
		}

		// changes class due to random classes and/or recruitment
		private void classChanger()
		{
			// randomize class for each character
			for (charNum = 0; charNum < totalUnitNumber; charNum++)
			{
				byte[] classname = new byte[7];
				byte[] weaponone = new byte[8];
				byte[] weapontwo = new byte[8];
				byte[] weaponthree = new byte[8];

				// units given before endgame are stored in a special place
				if (charChapter[charNum] == "407")
					chapterFile = dataLocation + "\\zmap\\emap0407c\\dispos_c.bin";
				else
					chapterFile = dataLocation + "\\zmap\\bmap0" + charChapter[charNum] + "\\dispos_h.bin";

				int randClass = 0;
				int classPointerOffset, classListOffset;
				int newLevel = charLevel[charNum];

				// if only rand recruitment, use those classes
				if (cbxClassRand.Checked == false)
					randClass = newClass[charNum];
				// otherwise randomize
				else
					randClass = chooseRandClass();

				// store new race & new class & add heron number &
				// set class pointer offset in classlist.bin
				classPointerOffset = randClass;
				if (charTier[charNum] == "a")
				{
					classListOffset = 0;
					if (randClass < 19)
						newRace[charNum] = "B";
					else
						newRace[charNum] = "L";

					newClass[charNum] = classPointerOffset;

					if (randClass == 25)
					{
						heronChanges(charNum);
						if (cbxClassRand.Checked == true)
						{
							if (heronNumber == 1)
							{
								classListOffset = 640;
								classPointerOffset = 33;
								newClass[charNum] = 63;
							}
							else if (heronNumber == 2)
							{
								classListOffset = 640;
								classPointerOffset = 34;
								newClass[charNum] = 64;
							}
						}
						else
						{
							if (newRecr[charNum] == 70)
							{
								classListOffset = 640;
								classPointerOffset = 33;
								newClass[charNum] = 63;
							}
							else if (newRecr[charNum] == 71)
							{
								classListOffset = 640;
								classPointerOffset = 34;
								newClass[charNum] = 64;
							}
						}
						heronNumber += 1;
					}
				}

				else if (charTier[charNum] == "b")
				{
					classListOffset = 640;
					if (randClass < 23)
					{
						newRace[charNum] = "B";
						// if laguz was randomized into beorc, level decreases by 10
						if (charRace[charNum] == "L")
							newLevel = newLevel - 10;
						if (newLevel < 1)
							newLevel = 1;
						// coerce to 18 due to game not allowing exp to units of 19? unsure why this occurs
						if (newLevel > 18)
							newLevel = 18;
					}
					else
					{
						newRace[charNum] = "L";
						// if beorc was randomized into laguz, level increases by 10
						if (charRace[charNum] == "B")
							newLevel = newLevel + 10;
					}

					if (randClass == 29)
					{
						heronChanges(charNum);
						if (cbxClassRand.Checked == true)
						{
							if (heronNumber == 1)
								classPointerOffset = 33;
							else if (heronNumber == 2)
								classPointerOffset = 34;
						}
						else
						{
							if (newRecr[charNum] == 70)
								classPointerOffset = 33;
							else if (newRecr[charNum] == 71)
								classPointerOffset = 34;
						}
						heronNumber += 1;
					}

					newClass[charNum] = classPointerOffset + 30;
				}

				else if (charTier[charNum] == "c")
				{
					classListOffset = 1376;
					if (randClass < 27)
					{
						newRace[charNum] = "B";
						// if laguz was randomized into beorc, level decreases by 20
						if (charRace[charNum] == "L")
							newLevel = newLevel - 20;
						if (newLevel < 1)
							newLevel = 1;
					}
					else
					{
						newRace[charNum] = "L";
						// if beorc was randomized into laguz, level increases by 20
						if (charRace[charNum] == "B")
							newLevel = newLevel + 20;
					}

					newClass[charNum] = classPointerOffset + 66;
				}

				else
				{
					classListOffset = 2080;
					if (randClass < 27)
					{
						newRace[charNum] = "B";
						// if laguz was randomized into beorc, level decreases by 20
						if (charRace[charNum] == "L")
							newLevel = newLevel - 20;
						if (newLevel < 1)
							newLevel = 1;
					}
					else
					{
						newRace[charNum] = "L";
					}

					newClass[charNum] = classPointerOffset + 100;
				}

				classListOffset += classPointerOffset * 20;

				try
				{
					// open classlist.bin
					using (var stream = new System.IO.FileStream(file + "\\assets\\classlist.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.Read))
					{
						stream.Position = classListOffset;
						for (int i = 0; i < 7; i++)
							classname[i] = Convert.ToByte(stream.ReadByte());
					}
				}
				catch
				{
					textBox1.Text = "Error 01: Asset files not found! Abandoning Randomization...";
					errorflag = 1;
				}

				// time to change inventory
				int weapononeOffset = 0;
				int weapontwoOffset = 0;
				int weaponthreeOffset = 0;

				if (charWeapNum[charNum] > 0 & // only change inventory if there's an inventory to change
							(charName[charNum] != "nasir" | newRace[charNum] == "B")) // only change nasir's inventory if he's beorc
				{
					// get offset of first weapon
					if (charTier[charNum] == "a")
					{
						weapononeOffset = TierOneWeaponOne(randClass);
						// each weapon takes up 20 slots
						weapononeOffset *= 20;
					}
					else if (charTier[charNum] == "b")
					{
						weapononeOffset = TierTwoWeaponOne(randClass);
						// each weapon takes up 20 slots
						weapononeOffset *= 20;
						//tier two weapons start here
						weapononeOffset += 528;
					}
					else // tier c and d
					{
						weapononeOffset = TierThreeWeaponOne(randClass);
						// each weapon takes up 20 slots
						weapononeOffset *= 20;
						//tier three weapons start here
						weapononeOffset += 1408;
					}


					if (charWeapNum[charNum] > 1) // only change inventory if there's an inventory to change
					{
						// get offset of first weapon
						if (charTier[charNum] == "a")
						{
							weapontwoOffset = TierOneWeaponTwo(randClass);
							// each weapon takes up 20 slots
							weapontwoOffset *= 20;
						}
						else if (charTier[charNum] == "b")
						{
							weapontwoOffset = TierTwoWeaponTwo(randClass);
							// each weapon takes up 20 slots
							weapontwoOffset *= 20;
							//tier two weapons start here
							weapontwoOffset += 528;
						}
						else // tier c and d
						{
							weapontwoOffset = TierThreeWeaponTwo(randClass);
							// each weapon takes up 20 slots
							weapontwoOffset *= 20;
							//tier three weapons start here
							weapontwoOffset += 1408;
						}


						if (charWeapNum[charNum] == 3) //third weapon if necessary
						{
							// get offset of third weapon
							if (charTier[charNum] == "b")
							{
								weaponthreeOffset = TierTwoWeaponThree(randClass);
								// each weapon takes up 20 slots
								weaponthreeOffset *= 20;
								//tier two weapons start here
								weaponthreeOffset += 528;
							}
							else // tier c and d
							{
								weaponthreeOffset = TierThreeWeaponThree(randClass);
								// each weapon takes up 20 slots
								weaponthreeOffset *= 20;
								//tier three weapons start here
								weaponthreeOffset += 1408;
							}
						}
					}
				}

				try
				{
					// open weaponlist.bin
					using (var stream = new System.IO.FileStream(file + "\\assets\\weaponlist.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.Read))
					{
						if (charWeapNum[charNum] > 0 & // only change inventory if there's an inventory to change
							(charName[charNum] != "nasir" | newRace[charNum] == "B")) // only change nasir's inventory if he's beorc
						{
							stream.Position = weapononeOffset;
							for (int i = 0; i < 8; i++)
								weaponone[i] = Convert.ToByte(stream.ReadByte());
						}

						if (charWeapNum[charNum] > 1)
						{
							stream.Position = weapontwoOffset;
							for (int i = 0; i < 8; i++)
								weapontwo[i] = Convert.ToByte(stream.ReadByte());
						}

						if (charWeapNum[charNum] > 2)
						{
							stream.Position = weaponthreeOffset;
							for (int i = 0; i < 8; i++)
								weaponthree[i] = Convert.ToByte(stream.ReadByte());
						}
					}
				}
				catch
				{
					textBox1.Text = "Error 01: Asset files not found! Abandoning Randomization...";
					errorflag = 1;
				}


				try
				{
					// open chapter file
					using (var stream = new System.IO.FileStream(chapterFile, System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						// change position to location of character in chapterFile
						stream.Position = (long)Convert.ToDouble(charLocation[charNum]);
						// write class
						for (int i = 0; i < 7; i++)
							stream.WriteByte(classname[i]);
						stream.Position += 13;
						// write inventory
						if (charWeapNum[charNum] > 0 & // only change inventory if there's an inventory to change
							(charName[charNum] != "nasir" | newRace[charNum] == "B")) // only change nasir's inventory if he's beorc
						{
							// write first weapon
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weaponone[i]);
							stream.Position += 12;
							if (charWeapNum[charNum] > 1) // only change inventory if there's an inventory to change
							{
								// write second weapon
								for (int i = 0; i < 8; i++)
									stream.WriteByte(weapontwo[i]);
								stream.Position += 12;
								if (charWeapNum[charNum] == 3) //third weapon if necessary
								{
									// write third weapon
									for (int i = 0; i < 8; i++)
										stream.WriteByte(weaponthree[i]);
								}
							}
						}

						// write level
						stream.Position = (long)Convert.ToDouble(charLevLoc[charNum]);
						stream.WriteByte(Convert.ToByte(newLevel));

						// fill laguz gauge if non-royal laguz, zero out otherwise
						stream.Position += 1;
						if (newRace[charNum] == "L" & charTier[charNum] != "c")
							stream.WriteByte(30);
						else
							stream.WriteByte(0);
					}
				}
				catch
				{
					textBox1.Text = "Error 02: Game files not found! Abandoning Randomization...";
					errorflag = 1;
				}


				try
				{
					// change tormod, vika, maurim inventories in part 4
					if (charName[charNum] == "tormod")
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0405.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							// change position to location of character in chapterFile
							stream.Position = 3164;
							// write weapon
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weaponone[i]);
							stream.Position = 3139;
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapontwo[i]);
						}
					}
					else if (charName[charNum] == "vika")
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0405.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							// change position to location of character in chapterFile
							stream.Position = 3220;
							// write weapon
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weaponone[i]);
						}
					}
					else if (charName[charNum] == "maurim")
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0405.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							// change position to location of character in chapterFile
							stream.Position = 3189;
							// write weapon
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weaponone[i]);
						}
					}
				}
				catch
				{
					textBox1.Text = "Error 16: Script files not found! Abandoning Randomization...";
					errorflag = 1;
				}


				// change level in data file
				try
				{
					// open data file
					using (var stream = new System.IO.FileStream(dataLocation + "\\FE10Data.cms.decompressed", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = charPID[charNum] - 2;
						stream.WriteByte(Convert.ToByte(newLevel));
					}
				}
				catch
				{
					textBox1.Text = "Error 17: Data file not found! Abandoning Randomization...";
					errorflag = 1;
				}

				// input class list
				string[] classList = System.IO.File.ReadAllLines(file +
					"\\assets\\classnames.txt");
			}
		}

		// selects random class based on selected settings
		private int chooseRandClass()
		{
			int randClass;
			bool[,] stringmatrix =  { { radioInfantry0.Checked, radioInfantry1.Checked, radioInfantry2.Checked, radioInfantry3.Checked, radioInfantry4.Checked, radioInfantry5.Checked},
							{ radioMages0.Checked, radioMages1.Checked, radioMages2.Checked, radioMages3.Checked, radioMages4.Checked, radioMages5.Checked},
							{ radioCav0.Checked, radioCav1.Checked, radioCav2.Checked, radioCav3.Checked, radioCav4.Checked, radioCav5.Checked},
							{ radioArmor0.Checked, radioArmor1.Checked, radioArmor2.Checked, radioArmor3.Checked, radioArmor4.Checked, radioArmor5.Checked},
							{ radioFly0.Checked, radioFly1.Checked, radioFly2.Checked, radioFly3.Checked, radioFly4.Checked, radioFly5.Checked},
							{ radioBeast0.Checked, radioBeast1.Checked, radioBeast2.Checked, radioBeast3.Checked, radioBeast4.Checked, radioBeast5.Checked},
							{ radioBird0.Checked, radioBird1.Checked, radioBird2.Checked, radioBird3.Checked, radioBird4.Checked, radioBird5.Checked},
							{ radioDragon0.Checked, radioDragon1.Checked, radioDragon2.Checked, radioDragon3.Checked, radioDragon4.Checked, radioDragon5.Checked} };
			int[,] weightmatrix = new int[8, 6];
			int[] weights = { 0, 0, 0, 0, 0, 0, 0, 0 };
			int classtype;

			// create weight matrix
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (stringmatrix[j, i] == true)
					{
						if (i != 0)
							weights[j] = (int)Math.Pow(3, (i - 1));
						else
							weights[j] = 0;
					}
				}
			}
			// prevent early game laguz
			if (comboClassOptions.SelectedIndex == 1 & cbxNoLaguz.Checked == true & charRace[charNum] == "B" & charNum < 18)
			{
				weights[5] = 0;
				weights[6] = 0;
				weights[7] = 0;
			}
			for (int i = 1; i < 8; i++)
				weights[i] += weights[i - 1];


			if (cbxLords.Checked == true & (charNum == 0 | charNum == 34)) // micaiah/ike are unchanged
			{
				if (charNum == 0)
					randClass = 17;
				else
					randClass = 0;
			}
			else if (cbxThieves.Checked == true & (charNum == 5 | charNum == 24)) // sothe/heather are unchanged
			{
				randClass = 4;
			}
			else
			{
				// generate a random class type
				if (comboClassOptions.SelectedIndex == 0) // no race-mixing
				{
					if (charNum < 69)
					{
						if ((cbxRandRecr.Checked == false & charRace[charNum] == "B") |
							(cbxRandRecr.Checked == true & recrRace[charNum] == "B"))
							classtype = random.Next(weights[4]); // 5 beorc class types
						else if (charTier[charNum] == "a")
							classtype = random.Next(weights[4], weights[6]); // 2 laguz class types (tier a can't be dragon)
						else
							classtype = random.Next(weights[4], weights[7]);
					}
					else
					{
						classtype = random.Next(weights[4], weights[7]);
					}
				}
				else // race-mixing
				{
					if (charTier[charNum] == "a")
						classtype = random.Next(weights[6]); // tier a can't be dragon
					else
						classtype = random.Next(weights[7]);
				}
				for (int i = 0; i < 8; i++)
				{
					if (classtype < weights[i])
					{
						classtype = i;
						break;
					}
				}

				// generate a random class
				if (comboClassOptions.SelectedIndex != 10)
				{
					if (classtype == 0) // infantry
					{
						if (charTier[charNum] == "a")
						{
							int[] classes = { 0, 3, 4, 8, 12 };
							randClass = classes[random.Next(5)];
						}
						else if (charTier[charNum] == "b")
						{
							int[] classes = { 0, 1, 5, 9, 13, 4 };
							if (charNum == 34)
								randClass = classes[random.Next(5)]; // ike can't be rogue
							else
								randClass = classes[random.Next(6)];
						}
						else if (charTier[charNum] == "c")
						{
							int[] classes = { 0, 1, 4, 8, 12, 14, 15 };
							randClass = classes[random.Next(7)];
						}
						else // kurth, ena, giffca, gareth, nasir
						{
							int[] classes = { 0, 1, 4, 8, 12, 14, 15 };
							randClass = classes[random.Next(7)];
						}
					}
					else if (classtype == 1) // mage
					{
						if (charTier[charNum] == "a")
						{
							int[] classes = { 14, 15, 16, 17, 18 };
							randClass = classes[random.Next(5)];

							while (charNum == 1 & newClass[0] == 18 & randClass == 18) //if micaiah is priest,do not allow edward to be priest
								randClass = classes[random.Next(4)];
						}
						else if (charTier[charNum] == "b")
						{
							int[] classes = { 15, 16, 17, 18, 19, 20, 21 };
							if (charNum == 34)
								randClass = 0; // ike can't be magic - is hero
							else
								randClass = classes[random.Next(7)];
						}
						else if (charTier[charNum] == "c")
						{
							int[] classes = { 16, 17, 18, 19, 20, 21, 22, 25, 26 };
							randClass = classes[random.Next(9)];
						}
						else // kurth, ena, giffca, gareth, nasir
						{
							int[] classes = { 16, 17, 18, 19, 20, 21, 22, 25, 26 };
							randClass = classes[random.Next(9)];
						}
					}
					else if (classtype == 2) // cavalry
					{
						if (charTier[charNum] == "a")
						{
							int[] classes = { 2, 6, 10, 13 };
							randClass = classes[random.Next(4)];
						}
						else if (charTier[charNum] == "b")
						{
							int[] classes = { 3, 7, 11, 14, 22 };
							if (charNum == 34)
								randClass = 0; // ike can't be horsed - is hero
							else if (charNum == 45)
								randClass = 25; // ranulf can't be horsed - is cat
							else
								randClass = classes[random.Next(5)];
						}
						else if (charTier[charNum] == "c")
						{
							int[] classes = { 3, 6, 10, 13, 24 };
							randClass = classes[random.Next(5)];
						}
						else // kurth, ena, giffca, gareth, nasir
						{
							int[] classes = { 3, 6, 10, 13, 24 };
							randClass = classes[random.Next(5)];
						}
					}
					else if (classtype == 3) // armor
					{
						if (charTier[charNum] == "a")
						{
							int[] classes = { 1, 5, 9 };
							randClass = classes[random.Next(3)];
						}
						else if (charTier[charNum] == "b")
						{
							int[] classes = { 2, 6, 10 };
							randClass = classes[random.Next(3)];
						}
						else if (charTier[charNum] == "c")
						{
							int[] classes = { 2, 5, 9 };
							randClass = classes[random.Next(3)];
						}
						else // kurth, ena, giffca, gareth, nasir
						{
							int[] classes = { 2, 5, 9 };
							randClass = classes[random.Next(3)];
						}
					}
					else if (classtype == 4) // fly (beorc)
					{
						if (charTier[charNum] == "a")
						{
							int[] classes = { 7, 11 };
							randClass = classes[random.Next(2)];
						}
						else if (charTier[charNum] == "b")
						{
							int[] classes = { 8, 12 };
							randClass = classes[random.Next(2)];
						}
						else if (charTier[charNum] == "c")
						{
							int[] classes = { 7, 11, 23 };
							randClass = classes[random.Next(3)];
						}
						else // kurth, ena, giffca, gareth, nasir
						{
							int[] classes = { 7, 11, 23 };
							randClass = classes[random.Next(3)];
						}
					}
					else if (classtype == 5) // beasts
					{
						if (charTier[charNum] == "a")
						{
							int[] classes = { 19, 20, 21, 22 };
							randClass = classes[random.Next(4)];
						}
						else if (charTier[charNum] == "b")
						{
							int[] classes = { 23, 24, 25, 26 };
							randClass = classes[random.Next(4)];
						}
						else if (charTier[charNum] == "c")
						{
							int[] classes = { 27, 28 };
							randClass = classes[random.Next(2)];
						}
						else // kurth, ena, giffca, gareth, nasir
						{
							int[] classes = { 27, 28, 29, 30 };
							randClass = classes[random.Next(4)];
						}
					}
					else if (classtype == 6) // birds
					{
						if (charTier[charNum] == "a")
						{
							int[] classes = { 23, 24, 25 };
							if (charNum == 1 & newClass[0] == 18) //if micaiah is heron,do not allow edward to be heron
								randClass = classes[random.Next(2)];
							else if (cbxHerons.Checked == true & heronNumber < 3)
								randClass = classes[random.Next(3)];
							else
								randClass = classes[random.Next(2)];
						}
						else if (charTier[charNum] == "b")
						{
							int[] classes = { 27, 28, 29 };
							if (charNum == 34)
								randClass = classes[random.Next(2)]; // ike can't be heron
							else if (charNum == 23 & newClass[22] == 59)
								//if brom is heron,do not allow nephenee to be heron
								randClass = classes[random.Next(2)];
							else if (cbxHerons.Checked == true & heronNumber < 3)
								randClass = classes[random.Next(3)];
							else
								randClass = classes[random.Next(2)];
						}
						else if (charTier[charNum] == "c")
						{
							int[] classes = { 29, 30 };
							randClass = classes[random.Next(2)];
						}
						else // kurth, ena, giffca, gareth, nasir
						{
							int[] classes = { 31, 32 };
							randClass = classes[random.Next(2)];
						}
					}
					else // dragons
					{
						if (charTier[charNum] == "b")
						{
							int[] classes = { 30, 31, 32 };
							randClass = classes[random.Next(3)];
						}
						else if (charTier[charNum] == "c")
						{
							randClass = 31;
						}
						else // kurth, ena, giffca, gareth, nasir
						{
							int[] classes = { 33, 34, 35 };
							randClass = classes[random.Next(3)];
						}
					}
				}
				else // lol
				{
					if (charTier[charNum] == "a")
					{
						if (charName[charNum] == "micaiah")
							randClass = 17;
						else
							randClass = random.Next(17, 19);
					}
					else if (charTier[charNum] == "b")
					{
						if (charNum == 34)
							randClass = 0; // ike is hero
						else
						{
							int[] classes = { 18, 21 };
							randClass = classes[random.Next(2)];
						}
					}
					else if (charTier[charNum] == "c")
					{
						randClass = random.Next(21, 23);
					}
					else // kurth, ena, giffca, gareth, nasir
					{
						randClass = random.Next(21, 23);
					}
				}
			}

			return randClass;
		}

		// changes pids in main.dol to reflect new herons
		private void heronChanges(int charNum)
		{
			// pids
			string[] PIDlist = System.IO.File.ReadAllLines(file +
				"\\assets\\charPIDs.txt");
			byte[] PIDbytes = System.Text.Encoding.ASCII.GetBytes(PIDlist[charNum]);

			string mainFile = dataLocation.Remove(dataLocation.Length - 5, 5) + "sys\\main.dol";
			try
			{
				// open main.dol
				using (var stream = new System.IO.FileStream(mainFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					if (cbxClassRand.Checked == true)
					{
						if (gameVersion == 0)
						{
							if (heronNumber == 0) // first heron is rafiel
								stream.Position = 3559766;
							else if (heronNumber == 1) // second is leanne
								stream.Position = 3559755;
							else // reyson
								stream.Position = 3559742;
							for (int i = 0; i < 7; i++)
								stream.WriteByte(PIDbytes[i]);
						}
						else if (gameVersion == 1)
						{
							if (heronNumber == 0) // first heron is rafiel
								stream.Position = 3559606;
							else if (heronNumber == 1) // second is leanne
								stream.Position = 3559595;
							else // reyson
								stream.Position = 3559582;
							for (int i = 0; i < 7; i++)
								stream.WriteByte(PIDbytes[i]);
						}
						else if (gameVersion == 2)
						{
							if (heronNumber == 0) // first heron is rafiel
								stream.Position = 3564270;
							else if (heronNumber == 1) // second is leanne
								stream.Position = 3564259;
							else // reyson
								stream.Position = 3564246;
							for (int i = 0; i < 7; i++)
								stream.WriteByte(PIDbytes[i]);
						}
					}
					else // only random recruitment
					{
						if (gameVersion == 0)
						{
							if (newRecr[charNum] == 69) // first heron is rafiel
								stream.Position = 3559766;
							else if (newRecr[charNum] == 70) // second is leanne
								stream.Position = 3559755;
							else // reyson
								stream.Position = 3559742;
							for (int i = 0; i < 7; i++)
								stream.WriteByte(PIDbytes[i]);
						}
						else if (gameVersion == 1)
						{
							if (newRecr[charNum] == 69) // first heron is rafiel
								stream.Position = 3559606;
							else if (newRecr[charNum] == 70) // second is leanne
								stream.Position = 3559595;
							else // reyson
								stream.Position = 3559582;
							for (int i = 0; i < 7; i++)
								stream.WriteByte(PIDbytes[i]);
						}
						else if (gameVersion == 2)
						{
							if (newRecr[charNum] == 69) // first heron is rafiel
								stream.Position = 3564270;
							else if (newRecr[charNum] == 70) // second is leanne
								stream.Position = 3564259;
							else // reyson
								stream.Position = 3564246;
							for (int i = 0; i < 7; i++)
								stream.WriteByte(PIDbytes[i]);
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 25: Cannot find DATA/sys/main.dol. Abandoning randomization...";
				errorflag = 1;
			}
		}

		// changes animations to match new classes
		private void animationChanger()
		{
			// changes animations of randomized units

			// input animation pointers
			string[] pointerList = System.IO.File.ReadAllLines(file +
				"\\assets\\animationPointers.txt");

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{


						// line offset from the myrmidon in animationPointers.txt
						int animationOffset = 0;
						switch (newClass[charNum])
						{
							case 0:
							case 31:
							case 67:
							case 101:
								// myrm line
								animationOffset = random.Next(5);
								break;
							case 1:
							case 32:
							case 68:
							case 102:
								// swd armr
								animationOffset = random.Next(5, 7);
								break;
							case 2:
							case 33:
							case 69:
							case 103:
								// swd knight
								animationOffset = random.Next(7, 9);
								break;
							case 3:
							case 34:
							case 80:
							case 114:
								// thief
								animationOffset = random.Next(9, 11);
								break;
							case 4:
							case 35:
							case 70:
							case 104:
								// soldier
								animationOffset = random.Next(11, 14);
								break;
							case 5:
							case 36:
							case 71:
							case 105:
								// lnc armr
								animationOffset = random.Next(14, 16);
								break;
							case 6:
							case 37:
							case 72:
							case 106:
								// lnc knight
								animationOffset = random.Next(16, 19);
								break;
							case 7:
							case 38:
							case 73:
							case 107:
								// peg knight
								animationOffset = random.Next(19, 22);
								break;
							case 8:
							case 39:
							case 74:
							case 108:
								// fighter
								animationOffset = random.Next(22, 24);
								break;
							case 9:
							case 40:
							case 75:
							case 109:
								// axe armr
								animationOffset = 24;
								break;
							case 10:
							case 41:
							case 76:
							case 110:
								// axe knight
								animationOffset = random.Next(25, 27);
								break;
							case 11:
							case 42:
							case 77:
							case 111:
								// wyvern
								animationOffset = random.Next(27, 29);
								break;
							case 12:
							case 43:
							case 78:
							case 112:
								// archer
								animationOffset = random.Next(29, 32);
								break;
							case 13:
							case 44:
							case 79:
							case 113:
								// bow knight
								animationOffset = 32;
								break;
							case 14:
							case 45:
							case 82:
							case 116:
								// fire mage
								animationOffset = random.Next(33, 35);
								break;
							case 15:
							case 46:
							case 83:
							case 117:
								// thunder mage
								animationOffset = 35;
								break;
							case 16:
							case 47:
							case 84:
							case 118:
								// wind mage
								animationOffset = random.Next(36, 38);
								break;
							case 17:
							case 48:
							case 87:
							case 121:
								// light mage
								if (comboClassOptions.SelectedIndex == 10) // beauty
									animationOffset = 41;
								else
									animationOffset = 38;
								break;
							case 18:
							case 51:
							case 88:
							case 122:
								// priest
								if (comboClassOptions.SelectedIndex == 10) // beauty
									animationOffset = 41;
								else
									animationOffset = random.Next(39, 43);
								break;
							case 30:
							case 66:
							case 100:
								// hero
								animationOffset = 43;
								break;
							case 49:
							case 85:
							case 119:
								// dark sage
								animationOffset = 44;
								break;
							case 50:
							case 91:
							case 125:
								// druid
								animationOffset = 45;
								break;
							case 52:
							case 90:
							case 124:
								// cleric
								animationOffset = 46;
								break;
							case 81:
							case 115:
								// assassin
								animationOffset = 47;
								break;
							case 86:
							case 120:
								// chancellor
								animationOffset = 48;
								break;
							case 89:
							case 123:
								// queen
								animationOffset = 49;
								break;
							case 92:
							case 126:
								// empress
								animationOffset = 50;
								break;
							case 19:
							case 53:
							case 127:
								// lion
								animationOffset = random.Next(51, 53);
								break;
							case 20:
							case 54:
							case 128:
								// tiger
								animationOffset = random.Next(53, 56);
								break;
							case 21:
							case 55:
							case 129:
								// cat
								animationOffset = random.Next(56, 59);
								break;
							case 22:
							case 56:
							case 130:
								// wolf
								animationOffset = 59;
								break;
							case 23:
							case 57:
							case 131:
								// hawk
								animationOffset = 60;
								break;
							case 24:
							case 58:
							case 132:
								// raven
								animationOffset = 61;
								break;
							case 25:
							case 59:
								// heron (rafiel)
								animationOffset = 70;
								break;
							case 63:
								// heron (leanne)
								animationOffset = 71;
								break;
							case 64:
								// heron (reyson)
								animationOffset = 72;
								break;
							case 26:
							case 60:
							case 133:
								// red dragon
								animationOffset = 62;
								break;
							case 27:
							case 61:
							case 134:
								// white dragon
								animationOffset = 63;
								break;
							case 28:
							case 62:
							case 135:
								// black dragon
								animationOffset = 64;
								break;
							case 93:
								// kinglion
								animationOffset = 65;
								break;
							case 94:
								// queenwolf
								animationOffset = 66;
								break;
							case 95:
								// hawkking
								animationOffset = 67;
								break;
							case 96:
								// ravenking
								animationOffset = 68;
								break;
							case 97:
								// dragking
								animationOffset = 69;
								break;
							default:
								animationOffset = 0;
								break;
						}


						// change position to location of character animation in dataFile
						stream.Position = (long)Convert.ToDouble(charAnimation[charNum]);
						string outByte;

						// write the 12 bytes for animations
						for (int k = 0; k < 12; k++)
						{
							if (k == 0 | k == 3 | k == 6 | k == 9)
								stream.WriteByte(Convert.ToByte(0));
							outByte = pointerList[animationOffset].Remove(0, (k * 4));
							outByte = outByte.Remove(3);
							stream.WriteByte(Convert.ToByte(outByte));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 03: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// changes transformation gauges of new laguz units
		private void laguzModifications()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// loop through characters
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						stream.Position = (long)Convert.ToDouble(charGauge[charNum]);

						if (newRace[charNum] == "L" & charTier[charNum] != "c")
						{
							int outByte1, outByte2, outByte3, outByte4;

							if (cbxGaugeRand.Checked == true)
							{
								// randomly assign value from chosen min and max (inclusive) and output
								outByte1 = random.Next(Convert.ToInt32(numericLaguzMin1.Value),
									(Convert.ToInt32(numericLaguzMax1.Value) + 1));
								outByte2 = random.Next(Convert.ToInt32(numericLaguzMin2.Value),
									(Convert.ToInt32(numericLaguzMax2.Value) + 1));
								outByte3 = random.Next(Convert.ToInt32(numericLaguzMin3.Value),
									(Convert.ToInt32(numericLaguzMax3.Value) + 1));
								outByte4 = random.Next(Convert.ToInt32(numericLaguzMin4.Value),
									(Convert.ToInt32(numericLaguzMax4.Value) + 1));
							}
							else
							{
								switch (newClass[charNum])
								{
									case 19:
									case 53:
									case 127:
										// lion
										outByte1 = 5;
										outByte2 = 10;
										outByte3 = 3;
										outByte4 = 2;
										break;
									case 20:
									case 54:
									case 128:
										// tiger
										outByte1 = 8;
										outByte2 = 15;
										outByte3 = 4;
										outByte4 = 3;
										break;
									case 21:
									case 55:
									case 129:
										// cat
										outByte1 = 10;
										outByte2 = 15;
										outByte3 = 5;
										outByte4 = 4;
										break;
									case 22:
									case 56:
									case 130:
										// wolf
										outByte1 = 6;
										outByte2 = 10;
										outByte3 = 4;
										outByte4 = 3;
										break;
									case 23:
									case 57:
									case 131:
										// hawk
										outByte1 = 8;
										outByte2 = 15;
										outByte3 = 4;
										outByte4 = 3;
										break;
									case 24:
									case 58:
									case 132:
										// raven
										outByte1 = 6;
										outByte2 = 10;
										outByte3 = 4;
										outByte4 = 3;
										break;
									case 25:
									case 59:
									case 63:
									case 64:
										// heron
										outByte1 = 4;
										outByte2 = 10;
										outByte3 = 5;
										outByte4 = 6;
										break;
									case 26:
									case 60:
									case 133:
										// red dragon
										outByte1 = 5;
										outByte2 = 6;
										outByte3 = 2;
										outByte4 = 1;
										break;
									case 27:
									case 61:
									case 134:
										// white dragon
										outByte1 = 4;
										outByte2 = 5;
										outByte3 = 2;
										outByte4 = 1;
										break;
									case 28:
									case 62:
									case 135:
										// black dragon
										outByte1 = 5;
										outByte2 = 6;
										outByte3 = 2;
										outByte4 = 1;
										break;
									default:
										outByte1 = 0;
										outByte2 = 0;
										outByte3 = 256;
										outByte4 = 256;
										break;

								}

							}


							stream.WriteByte(Convert.ToByte(outByte1)); // (+) per turn
							stream.WriteByte(Convert.ToByte(outByte2)); // (+) per battle
							stream.WriteByte(Convert.ToByte(256 - outByte3)); // (-) per turn
							stream.WriteByte(Convert.ToByte(256 - outByte4)); // (-) per battle

						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 04: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// oliver mode
		private void veryImportantFunction()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					byte[] importantInfo = new byte[12];
					stream.Position = charPID[58] + 4;

					for (int i = 0; i < 12; i++)
						importantInfo[i] = Convert.ToByte(stream.ReadByte());

					// loop through characters
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						// do the thing
						stream.Position = charPID[charNum] + 4;
						for (int i = 0; i < 12; i++)
							stream.WriteByte(importantInfo[i]);
					}

					// change some important stats
					stream.Position = 45734;
					stream.WriteByte(5);

					stream.Position = 43146;
					stream.WriteByte(4);

					stream.Position = 45962;
					stream.WriteByte(11);

					stream.Position = 43846;
					stream.WriteByte(12);
				}
			}
			catch
			{
				textBox1.Text = "Error 13: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}



		// WEAPON FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// selects random weapons for new classes
		private int TierOneWeaponOne(int randClass)
		{
			if (randClass < 3) // sword class
			{
				if (random.Next(10) < 5) // 50% chance of iron
					return 0;
				else                     // 50% chance of venin
					return 1;
			}
			else if (randClass == 3) // dagger class
			{
				return 14; // 100% dagger
			}
			else if (randClass < 8) // lance class
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 5) // 50% chance of iron
					return 3;
				else if (weaponChance < 9)  // 40% chance of venin
					return 4;
				else                        // 10% chance of horseslayer
					return 6;
			}
			else if (randClass < 12) // axe class
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 5) // 50% chance of iron
					return 7;
				else if (weaponChance < 9)  // 40% chance of venin
					return 8;
				else                        // 10% chance of hammer
					return 10;

			}
			else if (randClass < 14) // bow class
			{
				if (random.Next(10) < 5) // 50% chance of iron
					return 11;
				else                     // 50% chance of venin
					return 12;

			}
			else if (randClass < 18) // magic class
			{
				// set pointer to correct magic
				return 17 + (randClass - 14);
			}
			else if (randClass == 18) // healer class
			{
				return 21; // 100% heal
			}
			else // laguz class
			{
				return 24; // 100% olivi grass
			}
		}
		private int TierOneWeaponTwo(int randClass)
		{
			if (randClass < 3) // sword class
			{
				if (random.Next(10) < 4) // 40% chance of windsword
					return 2;
				else
					return 0; //iron
			}
			else if (randClass == 3) // dagger class
			{
				if (random.Next(10) < 8) // 80% chance of knife
					return 15;
				else                  // 20% chance of beastkiller
					return 16;
			}
			else if (randClass < 8) // lance class
			{
				if (random.Next(10) < 3) // 30% chance of javelin
					return 5;
				else
					return 5; //iron
			}
			else if (randClass < 12) // axe class
			{
				if (random.Next(10) < 3) // 30% chance of handaxe
					return 9;
				else
					return 7; // iron
			}
			else if (randClass < 14) // bow class
			{
				return 11; // iron
			}
			else if (randClass < 18) // magic class
			{
				// set pointer to correct magic
				return 17 + (randClass - 14);
			}
			else if (randClass == 18) // healer class
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 2)
				{
					if (weaponChance < 1) // 10% chance of torch
						return 22;
					else                  // 10% chance of unlock
						return 23;
				}
				else
					return 21; //heal

			}
			else // laguz class
			{
				return 24; // 100% olivi grass
			}
		}
		private int TierTwoWeaponOne(int randClass)
		{
			if (randClass < 4) // sword class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (charName[charNum] == "ike")
					return 38; // ettard
				else if (charName[charNum] == "mist")
					return 39; // florete
				else
				{
					int weaponChance = random.Next(10);
					if (weaponChance < 6)       // 60% chance of steel
						return 0;
					else if (weaponChance < 9)  // 30% chance of blade
						return 1;
					else                        // 10% chance of brave
						return 2;
				}
			}
			else if (randClass == 4) // dagger class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 19; // 100% dagger
			}
			else if (randClass < 9) // lance class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 6) // 60% chance of steel
					return 4;
				else if (weaponChance < 9)  // 30% chance of greatlance
					return 5;
				else                        // 10% chance of brave
					return 6;
			}
			else if (randClass < 13) // axe class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 6) // 60% chance of steel
					return 9;
				else if (weaponChance < 9)  // 30% chance of poleaxe
					return 10;
				else                        // 10% chance of brave
					return 11;
			}
			else if (randClass < 15) // bow class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 6) // 60% chance of steel
					return 14;
				else if (weaponChance < 9)  // 30% chance of rolfs
					return 15;
				else                        // 10% chance of killer
					return 16;
			}
			else if (randClass < 19) // magic class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				// set pointer to correct magic
				return 22 + (2 * (randClass - 15));
			}
			else if (randClass < 21) // dark magic class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (random.Next(10) < 6) // 60% chance of worm
					return 35;
				else                     // 40% chance of carreau
					return 36;
			}
			else if (randClass < 23) // healer class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (randClass == 21)
					return 30; // bishop gets light
				else
				{
					if (charName[charNum] == "ike")
						return 38; // ettard
					else if (charName[charNum] == "mist")
						return 39; // florete
					else
						return 3;  // cleric gets windedge
				}
			}
			else // laguz class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 41; // laguz stone
			}
		}
		private int TierTwoWeaponTwo(int randClass)
		{
			if (randClass < 4) // sword class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (random.Next(10) < 5) // 50% chance of windsword
					return 3;
				else                      // blade
					return 1;
			}
			else if (randClass == 4) // dagger class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (random.Next(10) < 7) // 70% chance of knife
					return 20;
				else                  // 30% chance of beastkiller
					return 21;
			}
			else if (randClass < 9) // lance class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (random.Next(10) < 7)  // 70% chance of javelin
					return 7;
				else  // 30% chance of horseslayer
					return 8;
			}
			else if (randClass < 13) // axe class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (random.Next(10) < 7)  // 70% chance of handaxe
					return 12;
				else // 30% chance of hammer
					return 13;
			}
			else if (randClass < 15) // bow class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 4)  // 40% chance of longbow
					return 17;
				else if (randClass == 13) // bowgun
					return 18;
				else                      // rolfs
					return 15;
			}
			else if (randClass < 19) // magic class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				// set pointer to correct magic
				return 23 + (2 * (randClass - 15));
			}
			else if (randClass < 21) // dark magic class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (randClass == 19)
					return 27;  // dark sage gets thunder
				else
					return 42; // druid gets master proof
			}
			else if (randClass < 23) // healer class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 4) // 40% chance of mend
					return 31;
				else if (weaponChance < 6) // 20% chance of restore
					return 32;
				else if (weaponChance < 9) // 30% chance of physic
					return 33;
				else                  // 10% chance of ward
					return 34;
			}
			else // laguz class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 40; // olivi grass
			}
		}
		private int TierTwoWeaponThree(int randClass)
		{
			if (randClass < 4) // sword class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 2;
			}
			else if (randClass == 4) // dagger class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 21;
			}
			else if (randClass < 9) // lance class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 8;
			}
			else if (randClass < 13) // axe class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 13;
			}
			else if (randClass < 15) // bow class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 17;
			}
			else if (randClass < 19) // magic class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 23 + (2 * (randClass - 15));
			}
			else if (randClass < 21) // dark magic class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 36;
			}
			else if (randClass < 23) // healer class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 33;
			}
			else // laguz class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 40; // olivi grass
			}
		}
		private int TierThreeWeaponOne(int randClass)
		{
			if (randClass < 4) // sword class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (charName[charNum] == "stefan" | charName[charNum] == "lehran") //stefan or lehran get SS weapon
					return 63;
				else
				{
					if (random.Next(10) < 6) // 60% chance of silverswd
						return 0;
					else                     // 40% chance of silverbld
						return 2;
				}
			}
			else if (randClass < 8) // lance class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (charName[charNum] == "stefan" | charName[charNum] == "lehran") //stefan or lehran get SS weapon
					return 64;
				else
				{
					if (random.Next(10) < 6) // 60% chance of silverlnc
						return 9;
					else                     // 40% chance of silvergtlnc
						return 11;
				}

			}
			else if (randClass < 12) // axe class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (charName[charNum] == "stefan" | charName[charNum] == "lehran") //stefan or lehran get SS weapon
					return 65;
				else
				{
					if (random.Next(10) < 6) // 60% chance of silveraxe
						return 17;
					else                     // 40% chance of silverplx
						return 19;
				}
			}
			else if (randClass < 14) // bow class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (charName[charNum] == "stefan" | charName[charNum] == "lehran") //stefan or lehran get SS weapon
					return 66;
				else
				{
					if (random.Next(10) < 6) // 60% chance of silverbow
						return 25;
					else                     // 40% chance of silencer
						return 30;
				}
			}
			else if (randClass < 16) // knife class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (charName[charNum] == "stefan" | charName[charNum] == "lehran") //stefan or lehran get SS weapon
					return 67;
				else
					return 32; // silver knife
			}
			else if (randClass < 20) // magic class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (charName[charNum] == "stefan" | charName[charNum] == "lehran") //stefan or lehran get SS weapon
					return 68 + (randClass - 16);
				else
				{
					// sanaki gets cymbeline
					if (randClass != 19 & charName[charNum] == "sanaki")
						return 73; // cymbeline
					else
					{
						// set pointer to correct magic
						return 36 + (3 * (randClass - 16));
					}
				}
			}
			else if (randClass < 26) // healer class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (charName[charNum] == "stefan" | charName[charNum] == "lehran") //stefan or lehran get SS weapon
				{
					if (randClass == 20) // chancellor gets asherastaff
						return 62;
					else if (randClass < 23) // priestess&saint get rexaura
						return 72;
					else if (randClass < 25) // queen&valk get vaguekatti
						return 63;
					else                     // summoner gets balberith
						return 71;
				}
				else
					return 54; // physic
			}
			else if (randClass == 26) // empress class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (charName[charNum] == "stefan" | charName[charNum] == "lehran") //stefan or lehran get SS weapon
					return 68;
				else
				{
					// sanaki gets cymbeline
					if (charName[charNum] == "sanaki")
						return 73; // cymbeline
					else
					{
						if (random.Next(10) < 7)
							return 48; // bolganone
						else
							return 36; // arcfire
					}
				}
			}
			else // laguz class
			{
				if (charName[charNum] == "stefan") //give stefan original vague katti if he's laguz
					return 63;
				else
					return 74; // concoction; don't need laguz stone because formshift
			}
		}
		private int TierThreeWeaponTwo(int randClass)
		{
			if (randClass == 0) // vanguard
			{
				if (random.Next(10) < 4)
					return 8; // 40% chance of tempest
				else
					return 24; // shortaxe
			}
			else if (randClass == 1) // trueblade
			{
				return 4; // killing edge
			}
			else if (randClass == 2) // sword marshal
			{
				if (random.Next(10) < 7)
					return 10; // 70% chance of iron greatlance
				else
					return 12; // bravelance
			}
			else if (randClass == 3) // sword goldknight
			{
				if (random.Next(10) < 5)
					return 4; // 50% chance of killingedge
				else
					return 20; // braveaxe
			}
			if (randClass == 4) // sentinal
			{
				if (random.Next(10) < 8)
					return 16; // 80% chance of shortspear
				else
					return 15; // javelin
			}
			else if (randClass == 5) // lance marshal
			{
				if (random.Next(10) < 7)
					return 18; // 70% chance of iron poleaxe
				else
					return 20; // braveaxe
			}
			else if (randClass == 6) // lance silverknight
			{
				if (random.Next(10) < 5)
					return 28; // 50% chance of killerbow
				else
					return 26; // ironlongbow
			}
			else if (randClass == 7) // seraphknight
			{
				if (random.Next(10) < 6)
					return 16; // 60% chance of shortspear
				else
					return 3; // bravesword
			}
			else if (randClass == 8) // reaver
			{
				if (random.Next(10) < 8)
					return 24; // 80% chance of shortaxe
				else
					return 23; // handaxe
			}
			else if (randClass == 9) // axe marshal
			{
				if (random.Next(10) < 7)
					return 1; // 70% chance of iron blade
				else
					return 3; // bravesword
			}
			else if (randClass == 10) // axe goldknight
			{
				if (random.Next(10) < 5)
					return 21; // 50% chance of killeraxe
				else
					return 3; // braveswd
			}
			else if (randClass == 11) // dragonlord
			{
				if (random.Next(10) < 6)
					return 24; // 60% chance of shortaxe
				else
					return 13; // killerlance
			}
			else if (randClass == 12) // marksman
				return 28; // killer bow
			else if (randClass == 13) // bow silverknight
			{
				if (random.Next(10) < 5)
					return 13; // 50% chance of killerlance
				else
					return 27; // steellongbow						
			}
			else if (randClass < 16) // knife class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (random.Next(10) < 6)
					return 34; // 60% chance of stiletto
				else
					return 33; // silver dagger
			}
			else if (randClass < 20) // magic class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (random.Next(10) < 7)
					return 37 + (3 * (randClass - 16)); // 70% chance of rangemagic
				else
					return 38 + (3 * (randClass - 16)); // el(magic)
			}
			else if (randClass == 20) // chancellor
				return 47; // carreu
			else if (randClass == 21) // priestess
				return 50; // shine
			else if (randClass == 22) // saint
				return 49; // ellight
			else if (randClass == 23 | randClass == 24) // queen&valkyrie
				return 3; // bravesword
			else if (randClass == 25) // summoner
			{
				// sanaki gets cymbeline
				if (charName[charNum] == "sanaki" & cbxFireMag.Checked == true)
					return 73; // cymbeline
				else
					return 45; // verrine
			}
			else if (randClass == 26) // empress class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 1)
					return 37; // meteor
				else if (weaponChance < 4)
					return 40; // bolting
				else if (weaponChance < 7)
					return 43; // blizzard
				else
					return 52; // purge
			}
			else // laguz class
				return 74; // concoction; don't need laguz stone because formshift
		}
		private int TierThreeWeaponThree(int randClass)
		{
			if (randClass == 0) // vanguard
			{
				if (random.Next(10) < 5)
					return 5; // wyrmslayer
				else
					return 21; // killeraxe
			}
			else if (randClass == 1) // trueblade
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 2)
					return 5; // wyrmslayer
				else if (weaponChance < 4)
					return 3; // bravesword
				else
					return 7; // stormsword
			}
			else if (randClass == 2) // sword marshal
			{
				return 23; // handaxe
			}
			else if (randClass == 3) // sword goldknight
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 4)
					return 7; // stormsword
				else if (weaponChance < 8)
					return 23; // handaxe
				else
					return 22; // hammer
			}
			else if (randClass == 4) // sentinal
			{
				if (random.Next(10) < 5)
					return 13; // killerlance
				else
					return 12; // bravelance
			}
			else if (randClass == 5) // lance marshal
			{
				return 6; // windedge
			}
			else if (randClass == 6) // lance silverknight
			{
				if (random.Next(10) < 5)
					return 14; // horseslayer
				else
					return 12; // bravelance
			}
			else if (randClass == 7) // seraphknight
			{
				return 6; // windedge
			}
			else if (randClass == 8) // reaver
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 4)
					return 21; // killeraxe
				else if (weaponChance < 7)
					return 20; // braveaxe
				else
					return 22; // hammer
			}
			else if (randClass == 9) // axe marshal
			{
				return 15; // javelin
			}
			else if (randClass == 10) // axe goldknight
			{
				if (random.Next(10) < 5)
					return 24; // shortaxe
				else
					return 22; // hammer
			}
			else if (randClass == 11) // dragonlord
			{
				if (random.Next(10) < 5)
					return 14; // horseslayer
				else
					return 22; // hammer
			}
			else if (randClass == 12) // marksman
			{
				if (random.Next(10) < 5)
					return 29; // bravebow
				else
					return 31; // taksh
			}
			else if (randClass == 13) // bow silverknight
			{
				if (random.Next(10) < 5)
					return 14; // horseslayer
				else
					return 29; // bravebow					
			}
			else if (randClass < 16) // knife class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 35; // beastkiller
			}
			else if (randClass < 20) // magic class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				return 53; // heal
			}
			else if (randClass == 20) // chancellor
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 5)
					return 50; // shine
				else if (weaponChance < 8)
					return 51; // valaura
				else if (weaponChance < 9)
					return 60; // fortify
				else
					return 62; // asherastaff
			}
			else if (randClass == 21) // priestess
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 3)
					return 52; // purge
				else if (weaponChance < 5)
					return 55; // recover
				else if (weaponChance < 9)
					return 56; // restore
				else
					return 58; // sleep
			}
			else if (randClass < 25) // saint, queen, valk
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 4)
					return 55; // recover
				else if (weaponChance < 7)
					return 56; // restore
				else if (weaponChance < 8)
					return 57; // ward
				else if (weaponChance < 9)
					return 58; // sleep
				else
				{
					if (randClass == 22)
						return 61; // hammerne
					else
						return 59; // rescue
				}
			}
			else if (randClass == 25) // summoner
			{
				int weaponChance = random.Next(10);
				if (weaponChance < 2)
					return 46; // fenrir
				else if (weaponChance < 3)
					return 58; // sleep
				else if (weaponChance < 5)
					return 60; // fortify
				else
					return 74; // concoction
			}
			else if (randClass == 26) // empress class %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
			{
				if (random.Next(10) < 5)
					return 38; // elthunder
				else
					return 41; // elwind
			}
			else // laguz class
			{
				return 74; // concoction; don't need laguz stone because formshift
			}
		}

		// randomizes weapon mt,acc,wt,etc
		private void weaponRandomizer()
		{
			int[] weapLocation = new int[185];
			int[] weapMin = new int[5];
			int[] weapDev = new int[5];
			int[] weapMax = new int[5];

			string line;
			string[] values;
			// initialize character information
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\WeaponData.csv");

			// skip header line/
			line = dataReader.ReadLine();
			// loop through all 185 weapons
			for (int i = 0; i < 185; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');

				weapLocation[i] = Convert.ToInt32(values[1]);
			}
			dataReader.Close();

			weapMin[0] = Convert.ToInt32(numericMTmin.Value);
			weapMin[1] = Convert.ToInt32(numericACCmin.Value);
			weapMin[2] = Convert.ToInt32(numericCRTmin.Value);
			weapMin[3] = Convert.ToInt32(numericWTmin.Value);
			weapMin[4] = Convert.ToInt32(numericUSEmin.Value);

			weapDev[0] = Convert.ToInt32(numericMTdev.Value);
			weapDev[1] = Convert.ToInt32(numericACCdev.Value);
			weapDev[2] = Convert.ToInt32(numericCRTdev.Value);
			weapDev[3] = Convert.ToInt32(numericWTdev.Value);
			weapDev[4] = Convert.ToInt32(numericUSEdev.Value);

			weapMax[0] = Convert.ToInt32(numericMTmax.Value);
			weapMax[1] = Convert.ToInt32(numericACCmax.Value);
			weapMax[2] = Convert.ToInt32(numericCRTmax.Value);
			weapMax[3] = Convert.ToInt32(numericWTmax.Value);
			weapMax[4] = Convert.ToInt32(numericUSEmax.Value);

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			int loopnumb;
			if (cbxLaguzWeap.Checked == true)
				loopnumb = 185;
			else
				loopnumb = 143;

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < loopnumb; i++)
					{
						int randStat, minStat, maxStat;
						stream.Position = weapLocation[i] + 40;
						for (int j = 0; j < 5; j++)
						{
							// read in stat and calculate min and max possible values
							randStat = stream.ReadByte();
							minStat = randStat - weapDev[j];
							if (minStat < weapMin[j])
								minStat = weapMin[j];
							else if (minStat > weapMax[j])
								minStat = weapMax[j];
							maxStat = randStat + weapDev[j];
							if (maxStat > weapMax[j])
								maxStat = weapMax[j];
							else if (maxStat < weapMin[j])
								maxStat = weapMin[j];
							// random value
							randStat = random.Next(minStat, maxStat + 1);
							stream.Position -= 1;
							if (i > 130 & j == 4 & cbxStaveUse.Checked == true) // keep uses of rare staves
								stream.Position += 1;
							else if (i > 121 & i <= 130 & j == 4 & cbxSiegeUse.Checked == true) // keep uses of siege tomes/ballistae
								stream.Position += 1;
							else
								stream.WriteByte(Convert.ToByte(randStat));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 21: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}



		// STAT AND GROWTH FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// stats hub, controls what functions to run
		private void Stats()
		{
			if (cbxGrowthRand.Checked == true | cbxZeroGrowths.Checked == true)
				growthRateModifier();
			else if (cbxGrowthShuffle.Checked == true)
				growthShuffle();


			if (cbxRandBases.Checked == true & errorflag == 0)
				randBaseStats();
			else if (cbxShuffleBases.Checked == true & errorflag == 0)
				shuffleBaseStats();


			// swap str/mag if necessary
			if (cbxStrMag.Checked == true & errorflag == 0)
				strMagSwap();

			if (cbxStatCaps.Checked == true & errorflag == 0)
				removeStatCaps();
		}

		// does the growth randomization
		private void growthRateModifier()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					int growthrate;
					int randgrowth;
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						// go to first growth rate
						stream.Position = charGrowth[charNum];
						for (int k = 0; k < 8; k++)
						{
							if (cbxZeroGrowths.Checked == false)
							{
								// read growth rate
								growthrate = stream.ReadByte();
								int mingrowth = growthrate - Convert.ToInt32(numericGrowth.Value);
								int maxgrowth = growthrate + Convert.ToInt32(numericGrowth.Value);
								if (cbxGrowthCap.Checked == true & maxgrowth > numericGrowthCap.Value)
									maxgrowth = (int)numericGrowthCap.Value + 1;
								if (cbxGrowthCap.Checked == true & mingrowth > numericGrowthCap.Value)
									mingrowth = (int)numericGrowthCap.Value;

								// randomize from growth-deviation to growth+deviation
								// unless growth-deviation is less than minimum, then randomize from minimum to growth+deviation
								// unless growth+deviation is less than minimum, then just set the minimum
								if (k == 0)
								{
									if (maxgrowth <= Convert.ToInt32(numericHP.Value))
										randgrowth = Convert.ToInt32(numericHP.Value);
									else if (mingrowth < Convert.ToInt32(numericHP.Value))
										randgrowth = random.Next(Convert.ToInt32(numericHP.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 1)
								{
									if (maxgrowth <= Convert.ToInt32(numericATK.Value))
										randgrowth = Convert.ToInt32(numericATK.Value);
									else if (mingrowth < Convert.ToInt32(numericATK.Value))
										randgrowth = random.Next(Convert.ToInt32(numericATK.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 2)
								{
									if (maxgrowth <= Convert.ToInt32(numericMAG.Value))
										randgrowth = Convert.ToInt32(numericMAG.Value);
									else if (mingrowth < Convert.ToInt32(numericMAG.Value))
										randgrowth = random.Next(Convert.ToInt32(numericMAG.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 3)
								{
									if (maxgrowth <= Convert.ToInt32(numericSKL.Value))
										randgrowth = Convert.ToInt32(numericSKL.Value);
									else if (mingrowth < Convert.ToInt32(numericSKL.Value))
										randgrowth = random.Next(Convert.ToInt32(numericSKL.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 4)
								{
									if (maxgrowth <= Convert.ToInt32(numericSPD.Value))
										randgrowth = Convert.ToInt32(numericSPD.Value);
									else if (mingrowth < Convert.ToInt32(numericSPD.Value))
										randgrowth = random.Next(Convert.ToInt32(numericSPD.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 5)
								{
									if (maxgrowth <= Convert.ToInt32(numericLCK.Value))
										randgrowth = Convert.ToInt32(numericLCK.Value);
									else if (mingrowth < Convert.ToInt32(numericLCK.Value))
										randgrowth = random.Next(Convert.ToInt32(numericLCK.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 6)
								{
									if (maxgrowth <= Convert.ToInt32(numericDEF.Value))
										randgrowth = Convert.ToInt32(numericDEF.Value);
									else if (mingrowth < Convert.ToInt32(numericDEF.Value))
										randgrowth = random.Next(Convert.ToInt32(numericDEF.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else
								{
									if (maxgrowth <= Convert.ToInt32(numericRES.Value))
										randgrowth = Convert.ToInt32(numericRES.Value);
									else if (mingrowth < Convert.ToInt32(numericRES.Value))
										randgrowth = random.Next(Convert.ToInt32(numericRES.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}

								stream.Position = stream.Position - 1;
							}
							else // Zero% growths run
							{
								int[] magClasses = { 14, 15, 16, 17, 18, 27, 45, 46, 47, 48, 49, 50, 51, 52, 61, 82, 83, 84, 85, 86, 87, 88, 90, 91, 92, 116, 117, 118, 119, 120, 121, 122, 124, 125, 126, 134 };
								string classType;
								if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
								{
									classType = "p";
									for (int i = 0; i < magClasses.Length; i++)
									{
										if (newClass[charNum] == magClasses[i])
										{
											classType = "m";
											break;
										}
									}
								}
								else
									classType = charClasstype[charNum];

								if ((classType == "p" & k == 2) // physical class with mag growth
								   | (classType == "m" & k == 1)) // magic class with str growth
									randgrowth = 100;
								else
									randgrowth = 0;
							}

							// check for overflow
							if (randgrowth > 255)
								randgrowth = 255;
							else if (randgrowth < 0)
								randgrowth = 0;

							// write new growth to game
							stream.WriteByte(Convert.ToByte(randgrowth));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 05: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// does the growth shuffling
		private void growthShuffle()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						int totalgrowth = Convert.ToInt32(numericGrowthShuffle.Value);
						int basegrowth;
						int[] newgrowths = new int[8];
						// go to first growth
						stream.Position = charGrowth[charNum];
						for (int k = 0; k < 8; k++)
						{
							// read and add to total
							basegrowth = stream.ReadByte();
							totalgrowth += basegrowth;
						}
						// randomize partitions
						for (int i = 0; i < 8; i++)
						{
							if (i == 7)
								newgrowths[i] = totalgrowth;
							else
								newgrowths[i] = random.Next(totalgrowth);
						}
						// sort by descending order
						Array.Sort(newgrowths);
						Array.Reverse(newgrowths);
						// subtract to get growths
						for (int i = 0; i < 8; i++)
						{
							if (i == 7)
								newgrowths[i] = newgrowths[i];
							else
								newgrowths[i] = newgrowths[i] - newgrowths[i + 1];
						}
						//check for growths under 15%
						if (cbxGrowthShuffleMin.Checked == true)
						{
							for (int i = 0; i < 8; i++)
							{
								if (newgrowths[i] < 15)
								{
									int dev = 15 - newgrowths[i];
									for (int j = i + 1; j < i + 8; j++)
									{
										int iteration = j;
										if (iteration > 7)
											iteration -= 8;
										// take 15% off of a larger growth
										if (newgrowths[iteration] >= 15 + dev)
										{
											newgrowths[iteration] -= dev;
											break;
										}
									}
									newgrowths[i] = 15;
								}
							}
						}

						// write new growths to game
						stream.Position = charGrowth[charNum];
						for (int k = 0; k < 8; k++)
						{
							stream.WriteByte(Convert.ToByte(newgrowths[k]));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 24: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// randomizes base stats by deviation
		private void randBaseStats()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					int basestat;
					int randstat;
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						// go to first stat
						stream.Position = charBases[charNum];
						for (int k = 0; k < 8; k++)
						{
							// read base
							basestat = stream.ReadByte();
							// change from signed to decimal
							if (basestat > 127)
								basestat -= 256;
							int minbase = basestat - Convert.ToInt32(numericBaseRand.Value);
							int maxbase = basestat + Convert.ToInt32(numericBaseRand.Value);
							// prevent personal base stats from being negative for now
							if (minbase < 0)
								minbase = 0;
							if (maxbase < 0)
								maxbase = 0;

							randstat = random.Next(minbase, maxbase + 1);

							// change from decimal to signed
							if (randstat < 0)
								randstat += 256;

							// write new base to game
							stream.Position = stream.Position - 1;
							stream.WriteByte(Convert.ToByte(randstat));

						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 19: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// shuffles base stats with possible deviation
		private void shuffleBaseStats()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						int hpval = 0;
						int lckval = 0;
						int totalstat = Convert.ToInt32(numericBaseShuffle.Value);
						int basestat;
						int[] newstats = new int[8];
						// go to first stat
						stream.Position = charBases[charNum];
						for (int k = 0; k < 8; k++)
						{
							// read base
							basestat = stream.ReadByte();

							if (k == 0)
								hpval = basestat;
							else if (k == 5)
								lckval = basestat;

							// change from signed to decimal
							if (basestat > 127)
								basestat -= 256;

							if (cbxHPLCKShuffle.Checked == false & (k == 0 | k == 5)) // don't add hp and luck
							{ }
							else // everything else is added to total
								totalstat += basestat;
						}
						// randomize partitions
						int numbstats;
						if (cbxHPLCKShuffle.Checked == true)
							numbstats = 8;
						else
							numbstats = 6;
						if (totalstat < 0)
							totalstat = 0;
						for (int i = 0; i < numbstats; i++)
						{
							if (i == numbstats - 1)
								newstats[i] = totalstat;
							else
								newstats[i] = random.Next(totalstat);
						}
						// sort by descending order
						Array.Sort(newstats);
						Array.Reverse(newstats);
						for (int i = 0; i < numbstats; i++)
						{
							if (i == numbstats - 1)
								newstats[i] = newstats[i];
							else
								newstats[i] = newstats[i] - newstats[i + 1];
						}

						// move stats and insert hp and lck
						if (cbxHPLCKShuffle.Checked == false)
						{
							newstats[7] = newstats[5]; // res
							newstats[6] = newstats[4]; // def
							newstats[5] = lckval;      // lck
							newstats[4] = newstats[3]; // spd
							newstats[3] = newstats[2]; // skl
							newstats[2] = newstats[1]; // mag
							newstats[1] = newstats[0]; // atk
							newstats[0] = hpval;       // hp
						}

						// write new base to game
						stream.Position = charBases[charNum];
						for (int k = 0; k < 8; k++)
						{
							stream.WriteByte(Convert.ToByte(newstats[k]));

						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 22: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// makes changes to character/class base stats in (vain)
		// attempt to balance random classes
		private void baseStatChanges()
		{
			int[,] inputMatrix = new int[104, 9];

			string line;
			string[] values;

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\NewBases.csv");

			// skip header line
			line = dataReader.ReadLine();

			for (int i = 0; i < 104; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				inputMatrix[i, 0] = Convert.ToInt32(values[1]);
				inputMatrix[i, 1] = Convert.ToInt32(values[2]);
				inputMatrix[i, 2] = Convert.ToInt32(values[3]);
				inputMatrix[i, 3] = Convert.ToInt32(values[4]);
				inputMatrix[i, 4] = Convert.ToInt32(values[5]);
				inputMatrix[i, 5] = Convert.ToInt32(values[6]);
				inputMatrix[i, 6] = Convert.ToInt32(values[7]);
				inputMatrix[i, 7] = Convert.ToInt32(values[8]);
				inputMatrix[i, 8] = Convert.ToInt32(values[9]);
			}

			// only need to randomize personal bases when character units are randomized
			int number;
			if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
				number = 104;
			else
				number = 77;

			try
			{
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < number; i++)
					{
						stream.Position = inputMatrix[i, 0];
						for (int j = 1; j < 9; j++)
						{
							stream.WriteByte(Convert.ToByte(inputMatrix[i, j]));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 15: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}

		}

		// swaps str and mag growth for units who changed from physical
		// to magical or vice-versa
		private void strMagSwap()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			int[] magClasses = { 14, 15, 16, 17, 18, 27, 45, 46, 47, 48, 49, 50, 51, 52, 61, 82, 83, 84, 85, 86, 87, 88, 90, 91, 92, 116, 117, 118, 119, 120, 121, 122, 124, 125, 126, 134 };
			
			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					int atkgrowth, maggrowth;
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						string classType;
						if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
						{
							classType = "p";
							for (int i = 0; i < magClasses.Length; i++)
							{
								if (newClass[charNum] == magClasses[i])
								{
									classType = "m";
									break;
								}
							}
						}
						else
							classType = charClasstype[charNum];

						// go to atk growth rate
						stream.Position = charGrowth[charNum] + 1;
						atkgrowth = stream.ReadByte();
						maggrowth = stream.ReadByte();
						if ((classType == "m" & atkgrowth > maggrowth) |
							(classType == "p" & atkgrowth < maggrowth))
						{
							stream.Position -= 2;
							stream.WriteByte(Convert.ToByte(maggrowth));
							stream.WriteByte(Convert.ToByte(atkgrowth));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 27: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// sets stat caps to desired values
		private void removeStatCaps()
		{
			string line;
			string[] values;
			int statCap1 = Convert.ToInt32(numericStatCap1.Value);
			int statCap2 = Convert.ToInt32(numericStatCap2.Value);
			int statCap3 = Convert.ToInt32(numericStatCap3.Value);
			int[] capLocation = new int[157];

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\StatCaps.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all 157 classes
			for (int i = 0; i < 157; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				capLocation[i] = Convert.ToInt32(values[1]);
			}

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < 157; i++)
					{
						stream.Position = capLocation[i];
						// write max for HP
						stream.WriteByte(0x7F);
						// max cap for all other 7 stats per class
						for (int j = 0; j < 7; j++)
						{
							if (i < 25)
								stream.WriteByte(Convert.ToByte(statCap1));
							else if (i < 80)
								stream.WriteByte(Convert.ToByte(statCap2));
							else
								stream.WriteByte(Convert.ToByte(statCap3));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 08: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}



		// ENEMY FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// enemy hub, decides what functions to run
		private void Enemies()
		{
			if (cbxRandEnemy.Checked == true & errorflag == 0)
				genericEnemyRandomizer();

			if (cbxEnemDrops.Checked == true & errorflag == 0)
				enemyDrops();

			if (cbxEnemyGrowth.Checked == true & errorflag == 0)
				enemyGrowthModifier();

			if (cbxRandBosses.Checked == true | (cbxRandRecr.Checked == true & cbxEnemyRecruit.Checked == true))
				bossRando();

			if (cbxBuffBosses.Checked == true)
				bossBuff();

			if (cbxRandBosses.Checked | (cbxRandRecr.Checked == true & cbxEnemyRecruit.Checked == true) | cbxRandEnemy.Checked == true)
			{
				enemyAnimations();
				bossAnimations();
			}
		}

		// randomizes enemy classes
		private void genericEnemyRandomizer()
		{
			string[] enemFile = new string[386];
			string[] enemChap = new string[enemFile.Length];
			int[] enemLoc = new int[enemFile.Length];
			string[] enemClass = new string[enemFile.Length];
			string[] enemWeap1Loc = new string[enemFile.Length];
			string[] enemWeap1Name = new string[enemFile.Length];
			string[] enemWeap2Loc = new string[enemFile.Length];
			string[] enemWeap2Name = new string[enemFile.Length];
			string[] enemWeap3Loc = new string[enemFile.Length];
			string[] enemWeap3Name = new string[enemFile.Length];
			string[] enemWeap4Loc = new string[enemFile.Length];
			string[] enemWeap4Name = new string[enemFile.Length];
			string[] enemWeap5Loc = new string[enemFile.Length];
			string[] enemWeap5Name = new string[enemFile.Length];
			string[] enemWeap6Loc = new string[enemFile.Length];
			string[] enemWeap6Name = new string[enemFile.Length];
			string[] enemClassName = new string[enemFile.Length];

			string line;
			string[] values;
			// initialize unit information
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\EnemyUnits.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all units
			for (int i = 0; i < enemFile.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				enemFile[i] = values[0];
				enemChap[i] = values[1];
				enemLoc[i] = Convert.ToInt32(values[2]);
				enemClass[i] = values[3];
				enemWeap1Loc[i] = values[4];
				enemWeap1Name[i] = values[5];
				enemWeap2Loc[i] = values[6];
				enemWeap2Name[i] = values[7];
				enemWeap3Loc[i] = values[8];
				enemWeap3Name[i] = values[9];
				enemWeap4Loc[i] = values[10];
				enemWeap4Name[i] = values[11];
				enemWeap5Loc[i] = values[12];
				enemWeap5Name[i] = values[13];
				enemWeap6Loc[i] = values[14];
				enemWeap6Name[i] = values[15];
				enemClassName[i] = values[16];
			}
			dataReader.Close();

			// classes
			dataReader = new System.IO.StreamReader(file + "\\assets\\EnemClasses.csv");

			line = dataReader.ReadLine();
			string[] T1 = line.Split(',');
			line = dataReader.ReadLine();
			string[] T2 = line.Split(',');
			line = dataReader.ReadLine();
			string[] T3 = line.Split(',');
			line = dataReader.ReadLine();
			string[] T4 = line.Split(',');
			line = dataReader.ReadLine();
			string[] bossclass = line.Split(',');
			dataReader.Close();


			string newclass;
			string newweapon;
			byte[] classbyte = new byte[7];
			byte[] weapbyte = new byte[8];

			int numberOfEnemies = enemFile.Length;
			if (cbxSpirits.Checked == false)
				numberOfEnemies -= 6;

			// let's randomize
			for (int enemNum = 0; enemNum < numberOfEnemies; enemNum++)
			{
				if (cbxEnemHealers.Checked != true | (enemClassName[enemNum] != "JID_PRIEST"
													& enemClassName[enemNum] != "JID_BISHOP" & enemClassName[enemNum] != "JID_BISHOP_SP"))
				{
					using (var stream = new System.IO.FileStream(dataLocation + enemFile[enemNum], System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						// go to class location
						stream.Position = enemLoc[enemNum];
						// pick random class
						if (enemClass[enemNum] == "1")
						{
							newclass = T1[random.Next(20)];
							while (cbxEnemHealers.Checked == true & newclass == "JID_HI1")
								newclass = T1[random.Next(20)];
						}
						else if (enemClass[enemNum] == "2")
						{
							if (enemChap[enemNum] == "3b" | enemChap[enemNum] == "3c")
								newclass = T2[random.Next(32)];
							else
								newclass = T2[random.Next(29)];
						}
						else if (enemClass[enemNum] == "3")
						{
							if (cbxTier3Enemies.Checked == true)
							{
								if (cbxSpirits.Checked == true & enemChap[enemNum] == "T")
									newclass = T4[random.Next(39)];
								else
									newclass = T4[random.Next(36)];
							}
							else
							{
								if (cbxSpirits.Checked == true & enemChap[enemNum] == "T")
									newclass = T3[random.Next(31)];
								else
									newclass = T3[random.Next(28)];
							}
						}
						else
							newclass = T1[0];
						// convert to bytes and write
						classbyte = System.Text.Encoding.ASCII.GetBytes(newclass);
						for (int i = 0; i < 7; i++)
							stream.WriteByte(classbyte[i]);

						// go to first weapon location
						stream.Position = Convert.ToInt32(enemWeap1Loc[enemNum]);
						// set up weapon type
						string weaptype = newclass.Substring(4, 1);
						string armortype = newclass.Substring(5, 1);
						string tiertype = newclass.Substring(6, 1);
						// 20% chances of having crossbows
						if ((weaptype == "A" | weaptype == "B") & armortype == "I" & (tiertype != "0" & tiertype != "1"))
							if (random.Next(10) < 2)
								weaptype = "G";
						// 50% light(or sword)/heal
						if (weaptype == "H" & enemClass[enemNum] != "1")
							if (random.Next(10) < 5 | cbxEnemHealers.Checked == true)
								if (tiertype == "5" | tiertype == "6")
									weaptype = "S";
								else
									weaptype = "M";
						// pick random weapon
						newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

						// convert to bytes and write
						weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
						for (int i = 0; i < 8; i++)
							stream.WriteByte(weapbyte[i]);

						if (enemNum > 379)
							stream.WriteByte(0x00);

						// repeat for weapons 2-6
						if (enemWeap2Loc[enemNum] != "")
						{
							// go to first weapon location
							stream.Position = Convert.ToInt32(enemWeap2Loc[enemNum]);
							// set up weapon type
							weaptype = newclass.Substring(4, 1);
							armortype = newclass.Substring(5, 1);
							tiertype = newclass.Substring(6, 1);
							// 20% chances of having crossbows
							if ((weaptype == "A" | weaptype == "B") & armortype == "I" & (tiertype != "0" & tiertype != "1"))
								if (random.Next(10) < 2)
									weaptype = "G";
							// 50% light(or sword)/heal
							if (weaptype == "H" & enemClass[enemNum] != "1")
								if (random.Next(10) < 5 | cbxEnemHealers.Checked == true)
									if (tiertype == "5" | tiertype == "6")
										weaptype = "S";
									else
										weaptype = "M";
							// pick random weapon
							newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

							// convert to bytes and write
							weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapbyte[i]);
						}
						if (enemWeap3Loc[enemNum] != "")
						{
							// go to first weapon location
							stream.Position = Convert.ToInt32(enemWeap3Loc[enemNum]);
							// set up weapon type
							weaptype = newclass.Substring(4, 1);
							armortype = newclass.Substring(5, 1);
							tiertype = newclass.Substring(6, 1);
							// 20% chances of having crossbows
							if ((weaptype == "A" | weaptype == "B") & armortype == "I" & (tiertype != "0" & tiertype != "1"))
								if (random.Next(10) < 2)
									weaptype = "G";
							// 50% light(or sword)/heal
							if (weaptype == "H" & enemClass[enemNum] != "1")
								if (random.Next(10) < 5 | cbxEnemHealers.Checked == true)
									if (tiertype == "5" | tiertype == "6")
										weaptype = "S";
									else
										weaptype = "M";
							// pick random weapon
							newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

							// convert to bytes and write
							weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapbyte[i]);
						}
						if (enemWeap4Loc[enemNum] != "")
						{
							// go to first weapon location
							stream.Position = Convert.ToInt32(enemWeap4Loc[enemNum]);
							// set up weapon type
							weaptype = newclass.Substring(4, 1);
							armortype = newclass.Substring(5, 1);
							tiertype = newclass.Substring(6, 1);
							// 20% chances of having crossbows
							if ((weaptype == "A" | weaptype == "B") & armortype == "I" & (tiertype != "0" & tiertype != "1"))
								if (random.Next(10) < 2)
									weaptype = "G";
							// 50% light(or sword)/heal
							if (weaptype == "H" & enemClass[enemNum] != "1")
								if (random.Next(10) < 5 | cbxEnemHealers.Checked == true)
									if (tiertype == "5" | tiertype == "6")
										weaptype = "S";
									else
										weaptype = "M";
							// pick random weapon
							newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

							// convert to bytes and write
							weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapbyte[i]);
						}
						if (enemWeap5Loc[enemNum] != "")
						{
							// go to first weapon location
							stream.Position = Convert.ToInt32(enemWeap5Loc[enemNum]);
							// set up weapon type
							weaptype = newclass.Substring(4, 1);
							armortype = newclass.Substring(5, 1);
							tiertype = newclass.Substring(6, 1);
							// 20% chances of having crossbows
							if ((weaptype == "A" | weaptype == "B") & armortype == "I" & (tiertype != "0" & tiertype != "1"))
								if (random.Next(10) < 2)
									weaptype = "G";
							// 50% light(or sword)/heal
							if (weaptype == "H" & enemClass[enemNum] != "1")
								if (random.Next(10) < 5 | cbxEnemHealers.Checked == true)
									if (tiertype == "5" | tiertype == "6")
										weaptype = "S";
									else
										weaptype = "M";
							// pick random weapon
							newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

							// convert to bytes and write
							weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapbyte[i]);
						}
						if (enemWeap6Loc[enemNum] != "")
						{
							// go to first weapon location
							stream.Position = Convert.ToInt32(enemWeap6Loc[enemNum]);
							// set up weapon type
							weaptype = newclass.Substring(4, 1);
							armortype = newclass.Substring(5, 1);
							tiertype = newclass.Substring(6, 1);
							// 20% chances of having crossbows
							if ((weaptype == "A" | weaptype == "B") & armortype == "I" & (tiertype != "0" & tiertype != "1"))
								if (random.Next(10) < 2)
									weaptype = "G";
							// 50% light(or sword)/heal
							if (weaptype == "H" & enemClass[enemNum] != "1")
								if (random.Next(10) < 5 | cbxEnemHealers.Checked == true)
									if (tiertype == "5" | tiertype == "6")
										weaptype = "S";
									else
										weaptype = "M";
							// pick random weapon
							newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

							// convert to bytes and write
							weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapbyte[i]);
						}
					}
				}
			}

			// change all enemies to feral ones
			enemyLaguz();

			// give part 4 enemies S rank in laguz
			enemyLaguzRank();
		}

		// gets weapons for new enemy classes
		private string getEnemyWeapon(string weaptype, string chapter, bool boss)
		{
			// weapons
			System.IO.StreamReader dataReader;
			if (boss == false & cbxNoSiege.Checked == true)
				dataReader = new System.IO.StreamReader(file + "\\assets\\EnemWeapons_nosiege.csv");
			else
				dataReader = new System.IO.StreamReader(file + "\\assets\\EnemWeapons.csv");
			string line;

			line = dataReader.ReadLine();
			string[] swords = line.Split(',');
			line = dataReader.ReadLine();
			string[] lances = line.Split(',');
			line = dataReader.ReadLine();
			string[] axes = line.Split(',');
			line = dataReader.ReadLine();
			string[] bows = line.Split(',');
			line = dataReader.ReadLine();
			string[] crossbows = line.Split(',');
			line = dataReader.ReadLine();
			string[] daggers = line.Split(',');
			line = dataReader.ReadLine();
			string[] fires = line.Split(',');
			line = dataReader.ReadLine();
			string[] thunders = line.Split(',');
			line = dataReader.ReadLine();
			string[] winds = line.Split(',');
			line = dataReader.ReadLine();
			string[] lights = line.Split(',');
			line = dataReader.ReadLine();
			string[] darks = line.Split(',');
			line = dataReader.ReadLine();
			string[] staves = line.Split(',');
			line = dataReader.ReadLine();
			string[] laguz = line.Split(',');
			dataReader.Close();

			int min = 0;
			int max = 1;

			if (weaptype == "S") // swords
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 5;
				}
				else if (chapter == "1b")
				{
					min = 3;
					max = 8;
				}
				else if (chapter == "1c")
				{
					min = 4;
					max = 10;
				}
				else if (chapter == "2a")
				{
					min = 3;
					max = 8;
				}
				else if (chapter == "2b")
				{
					min = 5;
					max = 11;
				}
				else if (chapter == "3a")
				{
					min = 6;
					max = 11;
				}
				else if (chapter == "3b")
				{
					min = 7;
					max = 11;
				}
				else if (chapter == "3c")
				{
					min = 7;
					max = 12;
				}
				else if (chapter == "4a")
				{
					min = 9;
					max = 13;
				}
				else if (chapter == "4b")
				{
					min = 9;
					max = 14;
				}
				else if (chapter == "T")
				{
					min = 11;
					max = 15;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				return swords[random.Next(min, max)];
			}
			else if (weaptype == "L") // lances
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 5;
				}
				else if (chapter == "1b")
				{
					min = 3;
					max = 9;
				}
				else if (chapter == "1c")
				{
					min = 5;
					max = 10;
				}
				else if (chapter == "2a")
				{
					min = 3;
					max = 9;
				}
				else if (chapter == "2b")
				{
					min = 5;
					max = 11;
				}
				else if (chapter == "3a")
				{
					min = 7;
					max = 11;
				}
				else if (chapter == "3b")
				{
					min = 8;
					max = 11;
				}
				else if (chapter == "3c")
				{
					min = 8;
					max = 12;
				}
				else if (chapter == "4a")
				{
					min = 9;
					max = 13;
				}
				else if (chapter == "4b")
				{
					min = 10;
					max = 14;
				}
				else if (chapter == "T")
				{
					min = 11;
					max = 15;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				return lances[random.Next(min, max)];
			}
			else if (weaptype == "A") // axes
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "1b")
				{
					min = 2;
					max = 8;
				}
				else if (chapter == "1c")
				{
					min = 4;
					max = 9;
				}
				else if (chapter == "2a")
				{
					min = 2;
					max = 8;
				}
				else if (chapter == "2b")
				{
					min = 4;
					max = 10;
				}
				else if (chapter == "3a")
				{
					min = 5;
					max = 10;
				}
				else if (chapter == "3b")
				{
					min = 6;
					max = 10;
				}
				else if (chapter == "3c")
				{
					min = 7;
					max = 11;
				}
				else if (chapter == "4a")
				{
					min = 7;
					max = 12;
				}
				else if (chapter == "4b")
				{
					min = 8;
					max = 13;
				}
				else if (chapter == "T")
				{
					min = 10;
					max = 14;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				return axes[random.Next(min, max)];
			}
			else if (weaptype == "B") // bows
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "1b")
				{
					min = 2;
					max = 6;
				}
				else if (chapter == "1c")
				{
					min = 4;
					max = 7;
				}
				else if (chapter == "2a")
				{
					min = 2;
					max = 6;
				}
				else if (chapter == "2b")
				{
					min = 4;
					max = 8;
				}
				else if (chapter == "3a")
				{
					min = 5;
					max = 8;
				}
				else if (chapter == "3b")
				{
					min = 6;
					max = 9;
				}
				else if (chapter == "3c")
				{
					min = 7;
					max = 9;
				}
				else if (chapter == "4a")
				{
					min = 7;
					max = 10;
				}
				else if (chapter == "4b")
				{
					min = 7;
					max = 11;
				}
				else if (chapter == "T")
				{
					min = 9;
					max = 12;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				return bows[random.Next(min, max)];
			}
			else if (weaptype == "G") // bowguns
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 1;
				}
				else if (chapter == "1b")
				{
					min = 0;
					max = 1;
				}
				else if (chapter == "1c")
				{
					min = 0;
					max = 1;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 1;
				}
				else if (chapter == "2b")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "3a")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "3b")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "3c")
				{
					min = 1;
					max = 3;
				}
				else if (chapter == "4a")
				{
					min = 1;
					max = 3;
				}
				else if (chapter == "4b")
				{
					min = 1;
					max = 3;
				}
				else if (chapter == "T")
				{
					min = 1;
					max = 4;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				return crossbows[random.Next(min, max)];
			}
			else if (weaptype == "K") // knives
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 3;
				}
				else if (chapter == "1b")
				{
					min = 3;
					max = 7;
				}
				else if (chapter == "1c")
				{
					min = 4;
					max = 9;
				}
				else if (chapter == "2a")
				{
					min = 3;
					max = 7;
				}
				else if (chapter == "2b")
				{
					min = 4;
					max = 10;
				}
				else if (chapter == "3a")
				{
					min = 5;
					max = 10;
				}
				else if (chapter == "3b")
				{
					min = 6;
					max = 10;
				}
				else if (chapter == "3c")
				{
					min = 6;
					max = 10;
				}
				else if (chapter == "4a")
				{
					min = 7;
					max = 11;
				}
				else if (chapter == "4b")
				{
					min = 8;
					max = 11;
				}
				else if (chapter == "T")
				{
					min = 9;
					max = 12;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				return daggers[random.Next(min, max)];
			}
			else if (weaptype == "F") // fire
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "1b")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "1c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "2b")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3a")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3b")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "4a")
				{
					min = 5;
					max = 8;
				}
				else if (chapter == "4b")
				{
					min = 5;
					max = 8;
				}
				else if (chapter == "T")
				{
					min = 7;
					max = 10;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				if (cbxOnlySiege.Checked == true & boss == false & chapter.Substring(0, 1) != "1" & chapter.Substring(0, 1) != "2")
					return fires[4];
				else
					return fires[random.Next(min, max)];
			}
			else if (weaptype == "T") // thunder
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "1b")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "1c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "2b")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3a")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3b")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "4a")
				{
					min = 5;
					max = 8;
				}
				else if (chapter == "4b")
				{
					min = 5;
					max = 8;
				}
				else if (chapter == "T")
				{
					min = 7;
					max = 10;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				if (cbxOnlySiege.Checked == true & boss == false & chapter.Substring(0, 1) != "1" & chapter.Substring(0, 1) != "2")
					return thunders[4];
				else
					return thunders[random.Next(min, max)];
			}
			else if (weaptype == "W") // wind
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "1b")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "1c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "2b")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3a")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3b")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "4a")
				{
					min = 5;
					max = 8;
				}
				else if (chapter == "4b")
				{
					min = 5;
					max = 8;
				}
				else if (chapter == "T")
				{
					min = 7;
					max = 10;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				if (cbxOnlySiege.Checked == true & boss == false & chapter.Substring(0, 1) != "1" & chapter.Substring(0, 1) != "2")
					return winds[4];
				else
					return winds[random.Next(min, max)];
			}
			else if (weaptype == "M") // light
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "1b")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "1c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "2b")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3a")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "3b")
				{
					min = 2;
					max = 7;
				}
				else if (chapter == "3c")
				{
					min = 2;
					max = 7;
				}
				else if (chapter == "4a")
				{
					min = 7;
					max = 10;
				}
				else if (chapter == "4b")
				{
					min = 7;
					max = 10;
				}
				else if (chapter == "T")
				{
					min = 7;
					max = 12;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				if (cbxOnlySiege.Checked == true & boss == false & chapter.Substring(0, 1) != "1" & chapter.Substring(0, 1) != "2")
					return lights[4];
				else
					return lights[random.Next(min, max)];
			}
			else if (weaptype == "D") // dark
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "1b")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "1c")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 2;
				}
				else if (chapter == "2b")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "3a")
				{
					min = 0;
					max = 4;
				}
				else if (chapter == "3b")
				{
					min = 0;
					max = 5;
				}
				else if (chapter == "3c")
				{
					min = 0;
					max = 5;
				}
				else if (chapter == "4a")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "4b")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "T")
				{
					min = 4;
					max = 7;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				if (cbxOnlySiege.Checked == true & boss == false & chapter.Substring(0, 1) != "1" & chapter.Substring(0, 1) != "2")
					return darks[4];
				else
					return darks[random.Next(min, max)];
			}
			else if (weaptype == "H") // heal
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 1;
				}
				else if (chapter == "1b")
				{
					min = 0;
					max = 3;
				}
				else if (chapter == "1c")
				{
					min = 1;
					max = 4;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 3;
				}
				else if (chapter == "2b")
				{
					min = 1;
					max = 5;
				}
				else if (chapter == "3a")
				{
					min = 1;
					max = 5;
				}
				else if (chapter == "3b")
				{
					min = 1;
					max = 5;
				}
				else if (chapter == "3c")
				{
					min = 2;
					max = 6;
				}
				else if (chapter == "4a")
				{
					min = 3;
					max = 7;
				}
				else if (chapter == "4b")
				{
					min = 3;
					max = 7;
				}
				else if (chapter == "T")
				{
					min = 3;
					max = 8;
				}
				if (cbxEnemWeaps.Checked == true | boss == true)
					max += 1;
				return staves[random.Next(min, max)];
			}
			else // laguz
			{
				return laguz[0];
			}
		}

		// randomizes dropable items from enemies
		private void enemyDrops()
		{
			string[] chapfile = new string[48];
			int[] location = new int[file.Length];

			string line;
			string[] values;
			// initialize unit information
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\EnemyDrops.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all units
			for (int i = 0; i < chapfile.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				chapfile[i] = values[0];
				location[i] = Convert.ToInt32(values[1]);
			}
			dataReader.Close();

			for (int itemNum = 0; itemNum < chapfile.Length; itemNum++)
			{
				byte[] itembytes = new byte[8];

				// choose random item, each is 20 bytes
				int itemPointerOffset = random.Next(212);
				itemPointerOffset *= 20;

				try
				{
					// open itemlist.bin
					using (var stream = new System.IO.FileStream(file + "\\assets\\itemlist.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.Read))
					{
						// read random item
						stream.Position = itemPointerOffset;
						for (int i = 0; i < 8; i++)
							itembytes[i] = Convert.ToByte(stream.ReadByte());
					}
				}
				catch
				{
					textBox1.Text = "Error 30: Asset files not found! Abandoning Randomization...";
					errorflag = 1;
				}

				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + chapfile[itemNum],
						System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
					{
						// write new item
						stream.Position = location[itemNum];
						for (int i = 0; i < 8; i++)
							stream.WriteByte(itembytes[i]);
						// write droppable
						byte[] slashD = System.Text.Encoding.ASCII.GetBytes("/D");
						foreach (byte value in slashD)
							stream.WriteByte(value);
						stream.WriteByte(0x00);
					}
				}
				catch
				{
					textBox1.Text = "Error 30: Chapter files not found! Abandoning randomization...";
					errorflag = 1;
				}
			}
		}

		// changes all enemy pids to have +1 in transformation
		// gauge, effectively making them feral ones
		private void enemyLaguz()
		{
			int[] enemyLaguzLoc = new int[183];

			string line;
			string[] values;

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\EnemyLaguzGauge.csv");

			// skip header line
			line = dataReader.ReadLine();

			for (int i = 0; i < enemyLaguzLoc.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				enemyLaguzLoc[i] = Convert.ToInt32(values[1]);
			}

			dataReader.Close();

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{

					for (int i = 0; i < enemyLaguzLoc.Length; i++)
					{
						// go to first growth
						stream.Position = enemyLaguzLoc[i];
						for (int k = 0; k < 4; k++)
							stream.WriteByte(0x01);
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 28: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// deletes animiation pointers for enemy PIDs so they use default animations
		private void enemyAnimations()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			int[] location = new int[88];
			int[] pointer = new int[88];
			int[] pointer2 = new int[pointer.Length];
			string line;
			string[] values;
			System.IO.StreamReader reader = new System.IO.StreamReader(file + "\\assets\\enemyAnimations.csv");
			for (int i = 0; i < pointer.Length; i++)
			{
				line = reader.ReadLine();
				values = line.Split(',');
				location[i] = Convert.ToInt32(values[0]);
				pointer[i] = Convert.ToInt32(values[2]);
				pointer2[i] = Convert.ToInt32(values[3]);
			}
			reader.Close();

			using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
			{
				for (int i = 0; i < pointer.Length; i++)
				{
					// zero out animation
					stream.Position = location[i] + 32;
					for (int j = 0; j < 4; j++)
						stream.WriteByte(0x00);

					//// overwrite pointer
					//byte[] infobyte = new byte[4];
					//byte[] newbyte = new byte[2];
					//stream.Position = 216452;
					//while (true)
					//{
					//	stream.Read(infobyte, 0, 4);

					//	if (infobyte[1] != 0)
					//		break;

					//	if (infobyte[2] == pointer[i] & infobyte[3] == pointer2[i])
					//	{
					//		stream.Position -= 4;
					//		stream.WriteByte(0x86);
					//		stream.WriteByte(0x00);
					//		stream.WriteByte(0x53);
					//		stream.WriteByte(0x09);
					//		//stream.Position -= 6;
					//		//stream.Read(newbyte, 0, 2);
					//		//stream.Position += 2;
					//		//stream.WriteByte(newbyte[0]);
					//		//stream.WriteByte(newbyte[1]);
					//		break;
					//	}
					//}
				}
			}
		}

		// changes part 4 weapon ranks to S in laguz
		private void enemyLaguzRank()
		{
			int[] weapRank = new int[18];

			string line;
			string[] values;

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\EnemyWeaponRank.csv");


			for (int i = 0; i < weapRank.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				weapRank[i] = Convert.ToInt32(values[2]);
			}

			dataReader.Close();

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{

					for (int i = 0; i < weapRank.Length; i++)
					{
						// go to first growth
						stream.Position = weapRank[i];
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0x7E);
						stream.WriteByte(0x24);
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 31: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// increases enemy growths by selected value
		private void enemyGrowthModifier()
		{
			int[] enemyGrowthLoc = new int[157];

			string line;
			string[] values;

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\EnemyGrowthData.csv");

			// skip header line
			line = dataReader.ReadLine();

			for (int i = 0; i < enemyGrowthLoc.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				enemyGrowthLoc[i] = Convert.ToInt32(values[1]);
			}

			dataReader.Close();

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					int enemyGrowth;
					int randGrowth;

					for (int i = 0; i < enemyGrowthLoc.Length; i++)
					{
						// go to first growth
						stream.Position = enemyGrowthLoc[i];
						for (int k = 0; k < 8; k++)
						{
							// read growth rate
							enemyGrowth = stream.ReadByte();

							// beorc enemies
							if (i < 115)
								randGrowth = enemyGrowth + Convert.ToInt32(numericEnemyGrowth.Value);
							// laguz enemies
							else
								randGrowth = enemyGrowth + (Convert.ToInt32(numericEnemyGrowth.Value) / 2);

							
							if (randGrowth > 255)
								randGrowth = 255;

							// write new base to game
							stream.Position = stream.Position - 1;
							stream.WriteByte(Convert.ToByte(randGrowth));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 20: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// randomizes classes/weapons for bosses
		private void bossRando()

		{
			string[] enemFile = new string[34];
			string[] enemChap = new string[34];
			int[] enemLoc = new int[enemFile.Length];
			string[] enemClass = new string[enemFile.Length];
			string[] enemName = new string[enemFile.Length];
			string[] enemWeap1Loc = new string[enemFile.Length];
			string[] enemWeap1Name = new string[enemFile.Length];
			string[] enemWeap2Loc = new string[enemFile.Length];
			string[] enemWeap2Name = new string[enemFile.Length];
			int[] recrNum = new int[enemFile.Length];

			string line;
			string[] values;
			// initialize unit information
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\BossList.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all units
			for (int i = 0; i < enemFile.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				enemFile[i] = values[0];
				enemChap[i] = values[1];
				enemLoc[i] = Convert.ToInt32(values[2]);
				enemClass[i] = values[3];
				enemName[i] = values[4];
				enemWeap1Loc[i] = values[5];
				enemWeap1Name[i] = values[6];
				enemWeap2Loc[i] = values[7];
				enemWeap2Name[i] = values[8];
				recrNum[i] = Convert.ToInt32(values[10]);
			}
			dataReader.Close();

			// classes
			dataReader = new System.IO.StreamReader(file + "\\assets\\EnemClasses.csv");

			line = dataReader.ReadLine();
			string[] T1 = line.Split(',');
			line = dataReader.ReadLine();
			string[] T2 = line.Split(',');
			line = dataReader.ReadLine();
			string[] T3 = line.Split(',');
			line = dataReader.ReadLine();
			string[] T4 = line.Split(',');
			line = dataReader.ReadLine();
			string[] bossclass = line.Split(',');
			dataReader.Close();


			string newclass = "";
			string newweapon;
			byte[] classbyte = new byte[7];
			byte[] weapbyte = new byte[8];

			int numberOfEnemies = enemFile.Length;
			int startpoint = 0;
			if (cbxRandBosses.Checked == false)
				startpoint = 25;

			// let's randomize
			for (int enemNum = startpoint; enemNum < numberOfEnemies; enemNum++)
			{
				using (var stream = new System.IO.FileStream(dataLocation + enemFile[enemNum], System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// go to class location
					stream.Position = enemLoc[enemNum];
					// pick random class
					if (cbxRandBosses.Checked == true)
					{
						if (enemChap[enemNum] != "1c")
						{
							if (enemClass[enemNum] == "1")
								newclass = T1[random.Next(20)];
							else if (enemClass[enemNum] == "2")
								newclass = T2[random.Next(23)];
							else if (enemClass[enemNum] == "3")
							{
								if (cbxTier3Enemies.Checked == true)
									newclass = T4[random.Next(27)];
								else
									newclass = T3[random.Next(19)];
							}
							else if (enemClass[enemNum] == "4")
								newclass = bossclass[random.Next(31)];
							else
								newclass = T1[0];
							// convert to bytes and write
							classbyte = System.Text.Encoding.ASCII.GetBytes(newclass);
							for (int i = 0; i < 7; i++)
								stream.WriteByte(classbyte[i]);
						}
					}
					else // only random recruitment
					{
						int classListOffset;

						if (enemClass[enemNum] == "1")
							classListOffset = 0;
						else if (enemClass[enemNum] == "2" | enemClass[enemNum] == "3")
							classListOffset = 640;
						else if (enemClass[enemNum] == "4")
							classListOffset = 1376;
						else
							classListOffset = 0;

						classListOffset += newClass[recrNum[enemNum]] * 20;

						// open classlist.bin
						using (var stream2 = new System.IO.FileStream(file + "\\assets\\classlist.bin", System.IO.FileMode.Open,
								System.IO.FileAccess.Read))
						{
							stream2.Position = classListOffset;
							for (int i = 0; i < 7; i++)
								classbyte[i] = Convert.ToByte(stream2.ReadByte());
							for (int i = 0; i < 7; i++)
								stream.WriteByte(classbyte[i]);
							newclass = System.Text.Encoding.ASCII.GetString(classbyte);
						}

					}

					// go to first weapon location
					stream.Position = Convert.ToInt32(enemWeap1Loc[enemNum]);
					// set up weapon type
					string weaptype = newclass.Substring(4, 1);
					string armortype = newclass.Substring(5, 1);
					string tiertype = newclass.Substring(6, 1);
					// 20% chances of having crossbows
					if ((weaptype == "A" | weaptype == "B") & armortype == "I" & (tiertype != "0" & tiertype != "1"))
						if (random.Next(10) < 2)
							weaptype = "G";
					// 50% light(or sword)/heal -> 100% for bosses
					if (weaptype == "H" & enemClass[enemNum] != "1")
						if (tiertype == "5" | tiertype == "6")
							weaptype = "S";
						else
							weaptype = "M";
					// pick random weapon
					newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], true);

					// convert to bytes and write
					weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
					for (int i = 0; i < 8; i++)
						stream.WriteByte(weapbyte[i]);

					// repeat for weapon 2
					if (enemWeap2Loc[enemNum] != "")
					{
						// go to first weapon location
						stream.Position = Convert.ToInt32(enemWeap2Loc[enemNum]);
						// set up weapon type
						weaptype = newclass.Substring(4, 1);
						armortype = newclass.Substring(5, 1);
						tiertype = newclass.Substring(6, 1);
						// 20% chances of having crossbows
						if ((weaptype == "A" | weaptype == "B") & armortype == "I" & (tiertype != "0" & tiertype != "1"))
							if (random.Next(10) < 2)
								weaptype = "G";
						// 50% light(or sword)/heal -> 100% for bosses
						if (weaptype == "H" & enemClass[enemNum] != "1")
							if (tiertype == "5" | tiertype == "6")
								weaptype = "S";
							else
								weaptype = "M";
						// pick random weapon
						newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], true);

						// convert to bytes and write
						weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
						for (int i = 0; i < 8; i++)
							stream.WriteByte(weapbyte[i]);
					}
				}
			}

		}

		// deletes animiation pointers for boss PIDs so they use default animations
		private void bossAnimations()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			int[] location = new int[21];
			int[] pointer = new int[21];
			int[] pointer2 = new int[pointer.Length];
			string line;
			string[] values;
			System.IO.StreamReader reader = new System.IO.StreamReader(file + "\\assets\\bossAnimations.csv");
			for (int i = 0; i < pointer.Length; i++)
			{
				line = reader.ReadLine();
				values = line.Split(',');
				location[i] = Convert.ToInt32(values[0]);
				pointer[i] = Convert.ToInt32(values[2]);
				pointer2[i] = Convert.ToInt32(values[3]);
			}
			reader.Close();

			int startpoint = 0;//14;
			//if (cbxRandBosses.Checked == true)
			//	startpoint = 0;

			using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
			{
				for (int i = startpoint; i < pointer.Length; i++)
				{
					// zero out animation
					stream.Position = location[i] + 32;
					for (int j = 0; j < 4; j++)
						stream.WriteByte(0x00);

					//// overwrite pointer
					//byte[] infobyte = new byte[4];
					//byte[] newbyte = new byte[2];
					//stream.Position = 216452;
					//while (true)
					//{
					//	stream.Read(infobyte, 0, 4);

					//	if (infobyte[1] != 0)
					//		break;

					//	if (infobyte[2] == pointer[i] & infobyte[3] == pointer2[i])
					//	{
					//		stream.Position -= 4;
					//		stream.WriteByte(0x86);
					//		stream.WriteByte(0x00);
					//		stream.WriteByte(0x53);
					//		stream.WriteByte(0x09);
					//		//stream.Position -= 6;
					//		//stream.Read(newbyte, 0, 2);
					//		//stream.Position += 2;
					//		//stream.WriteByte(newbyte[0]);
					//		//stream.WriteByte(newbyte[1]);
					//		break;
					//	}
					//}
				}
			}
		}

		// increases base stats for bosses
		private void bossBuff()
		{
			int[] bossBases = new int[39];

			string line;
			string[] values;

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\BossBaseLoc.csv");


			for (int i = 0; i < bossBases.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				bossBases[i] = Convert.ToInt32(values[0]);
			}

			dataReader.Close();

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					int enemyBase;
					double doubdelt;
					int delta=0;
					int newBase;

					for (int i = 0; i < bossBases.Length; i++)
					{
						// go to first base
						stream.Position = bossBases[i];
						for (int k = 0; k < 8; k++)
						{
							// read growth rate
							enemyBase = stream.ReadByte();

							doubdelt = (double)(numericBossStats.Value) * ((double)i / 35);
							for (int j = 1; j <= 35; j++)
							{
								if (doubdelt % j == doubdelt)
								{
									delta = j;
									break;
								}
							}
							if (delta == 0)
								delta = 1;

							// no BK, dheg, or seph
							if (i < 35)
							{
								newBase = enemyBase + delta;

								if (newBase > 255)
									newBase = 255;

								// write new base to game
								stream.Position = stream.Position - 1;
								stream.WriteByte(Convert.ToByte(newBase));
							}


						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 20: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}



		// MISCELLANEOUS RANDOMIZATIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// controls what functions run
		private void miscRandom()
		{
			if (cbxSkillRand.Checked == true & errorflag == 0)
				skillRandomizer();

			if (cbxAffinity.Checked == true & errorflag == 0)
				affinityRandomizer();
			
			if (cbxRandShop.Checked == true & errorflag == 0)
				shopRandomizer();
			
			if (cbxForge.Checked == true & errorflag == 0)
				forgeRandom();

			if (cbxBio.Checked == true & errorflag == 0)
				bioRandomizer();

			if (cbxEventItems.Checked == true & errorflag == 0)
				eventItemRandomizer();
		}

		// randomizes skills for all playable characters
		private void skillRandomizer()
		{
			// load skill list
			// input animation pointers
			string[] pointerList = System.IO.File.ReadAllLines(file +
				"\\assets\\skillList.txt");
			int[] firstByte = new int[pointerList.Length];
			int[] secondByte = new int[pointerList.Length];
			int[] thirdByte = new int[pointerList.Length];
			string[] skillName = new string[pointerList.Length];
			int randSkill = 0;

			for (int i = 0; i < pointerList.Length; i++)
			{
				firstByte[i] = Convert.ToInt32(pointerList[i].Remove(3));
				pointerList[i] = pointerList[i].Remove(0, 4);
				secondByte[i] = Convert.ToInt32(pointerList[i].Remove(3));
				pointerList[i] = pointerList[i].Remove(0, 4);
				thirdByte[i] = Convert.ToInt32(pointerList[i].Remove(3));
				skillName[i] = pointerList[i] = pointerList[i].Remove(0, 4);
			}

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						// set position to skill data
						stream.Position = charSkill[charNum];
						for (int i = 0; i < charSkillNum[charNum]; i++)
						{
							// get random skill
							if (charNum < 69)
							{
								if ((cbxRandRecr.Checked == false) & ((cbxClassRand.Checked == true & newRace[charNum] == "B")
									| (cbxClassRand.Checked == false & charRace[charNum] == "B")))
									randSkill = random.Next(skillName.Length - 1); // beorc can't get wildheart, last skill on list
								else if ((cbxRandRecr.Checked == true) & ((cbxClassRand.Checked == true & newRace[recrInverse[charNum]] == "B")
										| (cbxClassRand.Checked == false & charRace[recrInverse[charNum]] == "B")))
									randSkill = random.Next(skillName.Length - 1); // beorc can't get wildheart, last skill on list
								else
									randSkill = random.Next(skillName.Length);
							}
							else
							{
								if ((cbxClassRand.Checked == true & newRace[charNum] == "B")
											| (cbxClassRand.Checked == false & charRace[charNum] == "B"))
									randSkill = random.Next(skillName.Length - 1); // beorc can't get wildheart, last skill on list
								else
									randSkill = random.Next(skillName.Length);
							}
							
							// write output
							stream.WriteByte(0);
							stream.WriteByte(Convert.ToByte(firstByte[randSkill]));
							stream.WriteByte(Convert.ToByte(secondByte[randSkill]));
							stream.WriteByte(Convert.ToByte(thirdByte[randSkill]));

							newSkills[charNum, i] = skillName[randSkill];

						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 06: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// randomizes items found in each chapter's shop
		private void shopRandomizer()
		{
			string line;
			string[] values;
			int[] highBytes = new int[225];
			int[] lowBytes = new int[highBytes.Length];

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ShopStuff.csv");

			// loop through items in shop file
			for (int i = 0; i < highBytes.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				lowBytes[i] = Convert.ToInt32(values[1]);
				highBytes[i] = Convert.ToInt32(values[0]);
			}

			int itemSelection;
			try
			{
				using (var stream = new System.IO.FileStream(dataLocation + "\\Shop\\shopitem_h.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// write stuff until you get to the end of the shop pointers
					stream.Position = 230;
					while (stream.Position < 12260) // 12260 is the end of the shop pointers
					{
						itemSelection = random.Next(highBytes.Length);
						stream.WriteByte(Convert.ToByte(highBytes[itemSelection]));
						stream.WriteByte(Convert.ToByte(lowBytes[itemSelection]));
						stream.Position = stream.Position + 6;
						// go to next non-zero byte
						while (stream.ReadByte() == 0) { }
						stream.Position = stream.Position - 1;
					}
				}

				using (var stream = new System.IO.FileStream(dataLocation + "\\Shop\\shopitem_m.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// write stuff until you get to the end of the shop pointers
					stream.Position = 230;
					while (stream.Position < 12260) // 12260 is the end of the shop pointers
					{
						itemSelection = random.Next(highBytes.Length);
						stream.WriteByte(Convert.ToByte(highBytes[itemSelection]));
						stream.WriteByte(Convert.ToByte(lowBytes[itemSelection]));
						stream.Position = stream.Position + 6;
						// go to next non-zero byte
						while (stream.ReadByte() == 0) { }
						stream.Position = stream.Position - 1;
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 07: Shop files not found! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// changes possible weapons to forge in each chapter
		private void forgeRandom()
		{
			string line;
			string[] values;
			int[] highBytes = new int[107];
			int[] lowBytes = new int[107];

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ForgeStuff.csv");

			// loop through items in forge file
			for (int i = 0; i < highBytes.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				lowBytes[i] = Convert.ToInt32(values[1]);
				highBytes[i] = Convert.ToInt32(values[0]);
			}

			int itemSelection;
			try
			{
				using (var stream = new System.IO.FileStream(dataLocation + "\\Shop\\shopitem_h.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// write stuff until you get to the end of the shop pointers
					stream.Position = 13014;
					while (stream.Position < 43246) // 43246 is the end of the forge pointers
					{
						int currentbyte = stream.ReadByte();
						if (currentbyte == 0) // pointer is empty, skip
							stream.Position += 1;
						else // random weapon for pointer
						{
							stream.Position -= 1; 
							itemSelection = random.Next(highBytes.Length);
							stream.WriteByte(Convert.ToByte(highBytes[itemSelection]));
							stream.WriteByte(Convert.ToByte(lowBytes[itemSelection]));
						}
						// go to next pointer
						stream.Position += 10;
					}
				}

				using (var stream = new System.IO.FileStream(dataLocation + "\\Shop\\shopitem_m.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// write stuff until you get to the end of the shop pointers
					stream.Position = 13014;
					while (stream.Position < 43246) // 43246 is the end of the forge pointers
					{
						int currentbyte = stream.ReadByte();
						if (currentbyte == 0) // pointer is empty, skip
							stream.Position += 1;
						else // random weapon for pointer
						{
							stream.Position -= 1;
							itemSelection = random.Next(highBytes.Length);
							stream.WriteByte(Convert.ToByte(highBytes[itemSelection]));
							stream.WriteByte(Convert.ToByte(lowBytes[itemSelection]));
						}
						// go to next pointer
						stream.Position += 10;
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 29: Shop files not found! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// randomizes playable characters' affinities
		private void affinityRandomizer()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			// input affinity pointers
			string[] pointerList = System.IO.File.ReadAllLines(file +
				"\\assets\\affinity.txt");

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// loop through characters
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						
						stream.Position = charPID[charNum] + 20;

						int affinitychoice = random.Next(8);
						string outByte;

						// write the 4 bytes for affinity
						for (int k = 0; k < 4; k++)
						{
							outByte = pointerList[affinitychoice].Remove(0, (k * 4));
							outByte = outByte.Remove(3);
							stream.WriteByte(Convert.ToByte(outByte));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 12: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// randomizes biorhythm type of each playable character
		private void bioRandomizer()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// loop through characters
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						stream.Position = charBio[charNum];

						// biorhythm types 0 - 9
						stream.WriteByte(Convert.ToByte(random.Next(10)));
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 11: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// changes items from chests, villages, base convos, and treasure spots
		private void eventItemRandomizer()
		{
			string[] itemChapter = new string[132];
			int[] itemLocation = new int[itemChapter.Length];
			string[] itemName = new string[itemChapter.Length];

			string line;
			string[] values;
			// initialize character information
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\EventItemInfo.csv");

			// skip header line/
			line = dataReader.ReadLine();
			// loop through all items from base convos, villages, and chests
			for (int i = 0; i < itemChapter.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				// chapter where the item is obtained
				itemChapter[i] = values[0];
				// location in script file
				itemLocation[i] = Convert.ToInt32(values[1]);
				// item name
				itemName[i] = values[2];
			}
			dataReader.Close();

			for (int itemNum = 0; itemNum < itemChapter.Length; itemNum++)
			{
				byte[] itembytes = new byte[8];

				// choose random item, each is 20 bytes
				int itemPointerOffset = random.Next(212);
				itemPointerOffset *= 20;

				try
				{
					// open itemlist.bin
					using (var stream = new System.IO.FileStream(file + "\\assets\\itemlist.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.Read))
					{
						stream.Position = itemPointerOffset;
						for (int i = 0; i < 8; i++)
							itembytes[i] = Convert.ToByte(stream.ReadByte());
					}
				}
				catch
				{
					textBox1.Text = "Error 14: Asset files not found! Abandoning Randomization...";
					errorflag = 1;
				}

				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + itemChapter[itemNum],
						System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
					{
						stream.Position = itemLocation[itemNum];
						for (int i = 0; i < 8; i++)
							stream.WriteByte(itembytes[i]);
					}
				}
				catch
				{
					textBox1.Text = "Error 14: Script files not found! Abandoning randomization...";
					errorflag = 1;
				}
			}
		}



		// OTHER FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// various changes, either optional or not
		private void generalChanges()
		{
			// changes to prevent laguz from transforming if they are no longer laguz and also softlocks
			if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
			{
				// modify chapter 1_9 so that Nailah does not start shifted (bugs out if she is non-laguz)
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0109.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 2643;
						for (int k = 0; k < 13; k++)
							stream.WriteByte(0x00);

						stream.Position = 2783;
						for (int k = 0; k < 20; k++)
							stream.WriteByte(0x00);

						stream.Position = 5124;
						for (int k = 0; k < 25; k++)
							stream.WriteByte(0x00);
					}
				}
				catch
				{
					textBox1.Text = "Error 09: Script files not found! Abandoning randomization...";
					errorflag = 1;
				}

				// volug 1_6
				if (errorflag == 0)
				{
					try
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0106.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							stream.Position = 1900;
							for (int k = 0; k < 20; k++)
								stream.WriteByte(0x00);
						}
					}
					catch
					{
						textBox1.Text = "Files not found! Abandoning randomization...";
						errorflag = 1;
					}
				}

				// if muariam or vika is beorc, modify chapter 1_8 and 4_5 so they do not shift (same bug as Nailah above)
				if (errorflag == 0)
				{
					try
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0108.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							stream.Position = 3564;
							for (int k = 0; k < 13; k++)
								stream.WriteByte(0x00);
						}

						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0405.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							stream.Position = 1677;
							for (int k = 0; k < 25; k++)
								stream.WriteByte(0x00);
						}
					}
					catch
					{
						textBox1.Text = "Files not found! Abandoning randomization...";
						errorflag = 1;
					}
				}

				// if nealuchi is beorc, do the same as above
				if (errorflag == 0)
				{
					try
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0201.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							stream.Position = 2356;
							for (int k = 0; k < 13; k++)
								stream.WriteByte(0x00);

							stream.Position = 2967;
							for (int k = 0; k < 20; k++)
								stream.WriteByte(0x00);
						}
					}
					catch
					{
						textBox1.Text = "Files not found! Abandoning randomization...";
						errorflag = 1;
					}
				}

				// volug does not gain halfshift if he is beorc
				if (errorflag == 0 & newRace[9] == "B")
				{
					try
					{
						// 1-5: volug doesn't start as halfshifted laguz for part 1
						using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0106\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							int[] skillspaces = { 1150, 1154, 1042, 1046 };
							for (int i = 0; i < skillspaces.Length; i++)
							{
								stream.Position = skillspaces[i];
								stream.WriteByte(0x00);
								stream.WriteByte(0x00);
							}

						}

						// 3-6: volug no longer gains halfshift skill
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0307.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							stream.Position = 3104;
							for (int k = 0; k < 13; k++)
								stream.WriteByte(0x00);
						}
					}
					catch
					{
						textBox1.Text = "Files not found! Abandoning randomization...";
						errorflag = 1;
					}
				}

				// 4_7c: kurth does not gain formshift if he is beorc
				if (errorflag == 0 & newRace[63] == "B")
				{
					try
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0407c.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							stream.Position = 4882;
							for (int k = 0; k < 8; k++)
								stream.WriteByte(0x00);
						}
					}
					catch
					{
						textBox1.Text = "Files not found! Abandoning randomization...";
						errorflag = 1;
					}
				}

				// modify chapter 3_14 so that Nailah does not shift
				if (errorflag == 0)
				{
					try
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0314.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							stream.Position = 4449;
							for (int k = 0; k < 13; k++)
								stream.WriteByte(0x00);
						}
					}
					catch
					{
						textBox1.Text = "Files not found! Abandoning randomization...";
						errorflag = 1;
					}
				}

				// modify chapter 3_15 so that Volug and Nailah do not start shifted
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0315.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 2767;
						for (int k = 0; k < 20; k++)
							stream.WriteByte(0x00);
					}
				}
				catch
				{
					textBox1.Text = "Error 10: Script files not found! Abandoning randomization...";
					errorflag = 1;
				}

				// remove skrimir/zelgius fight in 3_5 to prevent crashes
				if (errorflag == 0)
				{
					try
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0305.cmb", System.IO.FileMode.Open,
								System.IO.FileAccess.ReadWrite))
						{
							stream.Position = 2680;
							for (int k = 0; k < 12; k++)
								stream.WriteByte(0x00);
						}
					}
					catch
					{
						textBox1.Text = "Error 10: Script files not found! Abandoning randomization...";
						errorflag = 1;
					}
				}
			}

			// modify 1_6 so that game over does not occur if jill, zihark, tauroneo die
			if ((cbxClassRand.Checked == true | cbxRandRecr.Checked == true | cbxRandEnemy.Checked == true) & errorflag == 0)
				{
					try
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0106.cmb", System.IO.FileMode.Open,
								System.IO.FileAccess.ReadWrite))
						{
							stream.Position = 2148;
							for (int k = 0; k < 12; k++)
								stream.WriteByte(0x00);

							stream.Position = 2308;
							for (int k = 0; k < 10; k++)
								stream.WriteByte(0x00);

							stream.Position = 2398;
							for (int k = 0; k < 8; k++)
								stream.WriteByte(0x00);
						}
					}
					catch
					{
						textBox1.Text = "Files not found! Abandoning randomization...";
						errorflag = 1;
					}
				}

			// modify 1_7 so that game over does not occur if fiona dies
			if ((cbxClassRand.Checked == true | cbxRandRecr.Checked == true | cbxRandEnemy.Checked == true) & errorflag == 0)
				{
					try
					{
						using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0107.cmb", System.IO.FileMode.Open,
								System.IO.FileAccess.ReadWrite))
						{
							stream.Position = 13964;
							for (int k = 0; k < 3; k++)
								stream.WriteByte(0x00);
						}
					}
					catch
					{
						textBox1.Text = "Files not found! Abandoning randomization...";
						errorflag = 1;
					}
				}

			// move elincia out of range of enemy bowmen if random enemies
			if (cbxRandEnemy.Checked == true & errorflag == 0)
			{
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0311\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 1512;
						stream.WriteByte(0x11);
						stream.Position += 1;
						stream.WriteByte(0x11);
					}
				}
				catch
				{
					textBox1.Text = "Files not found! Abandoning randomization...";
					errorflag = 1;
				}
			}

			// save jill
			if (cbxJillAI.Checked == true)
			{
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0106\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 6512;
						string output = "0106_POINTMOVE2";
						byte[] outbytes = System.Text.Encoding.ASCII.GetBytes(output);
						for (int i = 0; i < outbytes.Length; i++)
							stream.WriteByte(outbytes[i]);
						stream.WriteByte(0x00);

						stream.Position = 1383;
						stream.WriteByte(0x38);
						stream.Position = 2007;
						stream.WriteByte(0x38);
						stream.Position = 2111;
						stream.WriteByte(0x38);
						stream.Position = 2215;
						stream.WriteByte(0x38);
						stream.Position = 3359;
						stream.WriteByte(0x38);
					}
				}
				catch
				{
					textBox1.Text = "CH1-5 not found! Abandoning randomization...";
					errorflag = 1;
				}
			}

			// save fiona
			if (cbxFionaAI.Checked == true)
			{
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0107\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 5759;
						stream.WriteByte(0x38);
						stream.Position = 6591;
						stream.WriteByte(0x38);
						stream.Position = 6695;
						stream.WriteByte(0x38);
					}
				}
				catch
				{
					textBox1.Text = "CH1-6 not found! Abandoning randomization...";
					errorflag = 1;
				}
			}

			// kurth and ena no longer required for tower
			if (cbxKurthEna.Checked == true)
			{
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\emap0407d\\dispos_c.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 1186;
						stream.WriteByte(0x00);
						stream.Position = 1290;
						stream.WriteByte(0x00);
					}
				}
				catch
				{
					textBox1.Text = "Files not found! Abandoning randomization...";
					errorflag = 1;
				}
			}

			// choose random tower units
			if (cbxTowerUnits.Checked == true)
			{
				int[] nameloc = { 1974, 2008, 2016, 2039, 2048, 2068, 2080, 2091, 2113, 2124, 1966, 1994 };
				int[] locks = { 458, 562, 666, 770, 874, 978, 1082, 1394, 1498, 1602, 1186, 1290 };
				List<string> PIDs = new List<string>();
				List<string> names = new List<string>();

				System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\TowerUnits.csv");
				string line;
				string[] values;
				int iter = 60;
				if (cbxKurthEna.Checked == true)
					iter = 62; // last two are kurth/ena
				
				for (int i = 0; i < iter; i++)
				{
					line = dataReader.ReadLine();
					values = line.Split(',');
					PIDs.Add(values[1]);
					names.Add(values[0]);
				}

				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\emap0407d\\dispos_c.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						int numUnits;
						if (cbxKurthEna.Checked == true)
							numUnits = 12;
						else
							numUnits = 10;
						for (int i = 0; i < numUnits; i++)
						{
							stream.Position = locks[i];
							stream.WriteByte(0x18);

							stream.Position = nameloc[i] + 4;
							int randchoice = random.Next(PIDs.Count);
							byte[] outbytes = System.Text.Encoding.ASCII.GetBytes(PIDs[randchoice]);
							for (int j = 0; j < 3; j++)
								stream.WriteByte(outbytes[j]);
							stream.WriteByte(0x00);

							towerUnits[i] = names[randchoice];

							// remove selected unit from pool
							PIDs.RemoveAt(randchoice);
							names.RemoveAt(randchoice);
						}
					}
				}
				catch
				{
					textBox1.Text = "Files not found! Abandoning randomization...";
					errorflag = 1;
				}
			}

			// black knight nerf
			if (cbxBKnerf.Checked == true)
			{
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0407b\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 10924;
						string output = "NIHIL";
						byte[] outbytes = System.Text.Encoding.ASCII.GetBytes(output);
						for (int i = 0; i < 5; i++)
							stream.WriteByte(outbytes[i]);
						stream.WriteByte(0x00);
					}
				}
				catch
				{
					textBox1.Text = "Files not found! Abandoning randomization...";
					errorflag = 1;
				}
			}

			// let someone else fight BK
			if (cbxBKfight.Checked == true)
			{
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0407b\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 573;
						stream.WriteByte(0x09);
						stream.Position += 1;
						stream.WriteByte(0x09);
					}
				}
				catch
				{
					textBox1.Text = "CH4-6b not found! Abandoning randomization...";
					errorflag = 1;

				}
			}

			// ike gets nihil
			if (cbxNihil.Checked == true)
			{
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0301\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 8816;
						string output = "NIHI";
						byte[] outbytes = System.Text.Encoding.ASCII.GetBytes(output);
						for (int i = 0; i < 4; i++)
							stream.WriteByte(outbytes[i]);
						stream.WriteByte(0x00);
					}
				}
				catch
				{
					textBox1.Text = "Files not found! Abandoning randomization...";
					errorflag = 1;
				}
			}

			// make ashera only die when killed by Ike, regardless of weapon
			if (errorflag == 0)
			{
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0407e.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 2892;
						stream.WriteByte(80); //P
						stream.WriteByte(73); //I
						stream.WriteByte(68); //D
						stream.WriteByte(95); //_
						stream.WriteByte(73); //I
						stream.WriteByte(75); //K
						stream.WriteByte(69); //E
						for (int k = 0; k < 36; k++)
							stream.WriteByte(0x00); //null
					}
				}
				catch
				{
					textBox1.Text = "Files not found! Abandoning randomization...";
					errorflag = 1;
				}
			}

			fe10dataChanges();
			
			if (cbxWeapCaps.Checked == true & errorflag == 0)
				removeWeapCaps();
			
			if ((cbxEnemyRange.Checked == true | cbxWeapTri.Checked == true | cbxMapAff.Checked == true) & errorflag == 0)
				makeHardModeGreatAgain();
		}

		// makes various changes depending on user selected checkboxes
		private void fe10dataChanges()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{

					if (cbxFlorete.Checked == true)
					{
						// go to florete
						stream.Position = 58247;
						// change ranged type from sword to white breath
						stream.WriteByte(130);
					}
					if (cbxGMweaps.Checked == true)
					{
						// remove locks - ragnell, florete, amiti, cymbeline, ettard
						int[] locks = { 58420, 58288, 58592, 63168, 58356 };
						for (int i = 0; i < 5; i++)
						{
							stream.Position = locks[i];
							stream.WriteByte(0);
							stream.WriteByte(0);
							stream.WriteByte(0);
							stream.WriteByte(0);
						}

						// replace weapon level
						stream.Position = 58380; // ragnell gets SS
						stream.WriteByte(0);
						stream.WriteByte(2);
						stream.WriteByte(121);
						stream.WriteByte(248);

						stream.Position = 58316; // ettard gets S
						stream.WriteByte(0);
						stream.WriteByte(3);
						stream.WriteByte(50);
						stream.WriteByte(123);

						stream.Position = 58248; // florete gets A
						stream.WriteByte(0);
						stream.WriteByte(2);
						stream.WriteByte(127);
						stream.WriteByte(255);

						stream.Position = 58552; // amiti gets S
						stream.WriteByte(0);
						stream.WriteByte(3);
						stream.WriteByte(50);
						stream.WriteByte(123);

						stream.Position = 63128; // cymbeline gets SS
						stream.WriteByte(0);
						stream.WriteByte(2);
						stream.WriteByte(121);
						stream.WriteByte(248);
					}
					if (cbxDBweaps.Checked == true)
					{
						// remove locks - caladbolg,tarvos,lughnasadh,thani
						int[] locks = { 57964, 60460, 61384, 64468 };
						for (int i = 0; i < 4; i++)
						{
							stream.Position = locks[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
						}

						// replace weapon level
						stream.Position = 64428; // thani gets D
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0x97);
						stream.WriteByte(0x77);

						stream.Position = 57924; // caladbolg gets C
						stream.WriteByte(0x00);
						stream.WriteByte(0x03);
						stream.WriteByte(0x67);
						stream.WriteByte(0xDA);

						stream.Position = 60420; // tarvos gets C
						stream.WriteByte(0x00);
						stream.WriteByte(0x03);
						stream.WriteByte(0x67);
						stream.WriteByte(0xDA);

						stream.Position = 61340; // lughnasdh gets C
						stream.WriteByte(0x00);
						stream.WriteByte(0x03);
						stream.WriteByte(0x67);
						stream.WriteByte(0xDA);

					}
					if (cbxDragonSkills.Checked == true)
					{
						int[] skills = { 82401, 82409, 82417, 82425, 82457, 82465, 82497, 82505 };
						for (int i = 0; i < skills.Length; i++)
						{
							stream.Position = skills[i];
							stream.WriteByte(0x03);
							stream.WriteByte(0x32);
							stream.WriteByte(0xF8);
						}
					}
					if (cbxFireMag.Checked == true)
					{
						// go to druid minimum ranks
						stream.Position = 162739;
						// write D in fire
						stream.WriteByte(0x44);
						// go to druid maximium ranks
						stream.Position = 162752;
						// write A in fire
						stream.WriteByte(0x41);

						// go to summoner minimum ranks
						stream.Position = 162765;
						// write B in fire
						stream.WriteByte(0x42);
						// go to summoner maximium ranks
						stream.Position = 162661;
						// write S in fire
						stream.WriteByte(0x53);
					}
					if (cbxChestKey.Checked == true)
					{
						stream.Position = 71132;
						stream.WriteByte(0x05);
					}
					if (cbxBirdVision.Checked == true)
					{
						// location of visions for hawk, hawkking, raven, female raven, ravenking, and all untransformed classes
						int[] visions = { 52563, 52683, 52811, 52939, 53075, 53195, 53323, 53443, 53571, 53699 };
						for (int i = 0; i < visions.Length; i++)
						{
							stream.Position = visions[i];
							stream.WriteByte(0x02);
						}
						
					}
					if (cbxSellableItems.Checked == true)
					{
						// remove 'rarity' modifier that prevents selling
						int[] items = { 70900, 70960, 71020, 71084 };
						for (int i = 0; i < items.Length; i++)
						{
							stream.Position = items[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x03);
							stream.WriteByte(0x41);
							stream.WriteByte(0xE1);
						}

						// change price from 30k to 10k for master proof, holy crown, and satori sign
						int[] cost = { 70942, 71002, 71066};
						for (int i = 0; i < cost.Length; i++)
						{
							stream.Position = cost[i];
							stream.WriteByte(0x27);
							stream.WriteByte(0x10);
						}
					}
					if (cbxLowerPrice.Checked == true)
					{
						// changes laguz gem to 3k per use (total 15k)
						stream.Position = 75634;
						stream.WriteByte(0x0B);
						stream.WriteByte(0xB8);
					}
					if (cbxWinCon.Checked == true)
					{
						// shows player how to win chapter -> change to defeat boss
						int[] wincons = { 154283, 154315, 154347, //4_1 
										  154463, 154495, 154527, //4_2
										  155003, 155035, 155067, //4_3
										  155543, 155575, 155607, //4_4
										  157163, 157195, 157227};//4_7a
						for (int i = 0; i < wincons.Length; i++)
						{
							stream.Position = wincons[i];
							stream.WriteByte(0x60);
						}
						// change to seize
						int[] otherwin = { 156083, 156115, 156147 }; //4_5
						for (int j = 0; j < otherwin.Length; j++)
						{
							stream.Position = otherwin[j];
							stream.WriteByte(0x7C);
						}
					}
					if (cbxLethality.Checked == true)
					{
						int[] banes = { 42213, 42333};
						for (int i = 0; i < banes.Length; i++)
						{
							stream.Position = banes[i];
							stream.WriteByte(0x03);
							stream.WriteByte(0x34);
							stream.WriteByte(0x3B);
						}
					}

					if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true | cbxRandShop.Checked == true | cbxEventItems.Checked == true | cbxGMweaps.Checked == true)
					{
						// amiti / ragnell / staves / rudol gem are not locked to unit
						int[] weapons = { 58428, 58600, 69492, 69616, 69748, 75772 };
						for (int i = 0; i < weapons.Length; i++)
						{
							stream.Position = weapons[i];
							for (int j = 0; j < 4; j++)
								stream.WriteByte(0x00);
						}

						// ballistae have price of 1500gp
						int[] ballista = { 61954, 61886 };
						for (int i = 0; i < ballista.Length; i++)
						{
							stream.Position = ballista[i];
							stream.WriteByte(0x01);
							stream.WriteByte(0x2C);
						}
					}

					if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true | cbxRandEnemy.Checked == true | cbxRandBosses.Checked == true)
					{
						// sky and cloud tiles function as grass
						int[] tiles = { 84788, 84832 };
						for (int i = 0; i < tiles.Length; i++)
						{
							stream.Position = tiles[i];
							for (int j = 0; j < 23; j++)
								stream.WriteByte(0x01);
						}
					}

					if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
					{
						// ike / micaiah / sothe story promotions
						int[] promotions = { 37552 , 41964, 43108, 43808};
						for (int i = 0; i < promotions.Length; i++)
						{
							stream.Position = promotions[i];
							for (int j = 0; j < 4; j++)
								stream.WriteByte(0x00);
						}
						// locktouch on thief,rogue,whisper
						int[] classes = { 41856, 41976, 42208};
						for (int i = 0; i < classes.Length; i++)
						{
							stream.Position = classes[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x03);
							stream.WriteByte(0x37);
							stream.WriteByte(0xCD);
						}
						// treasurehunt on sothe, heather
						int[] units = { 1776, 4684};
						for (int i = 0; i < units.Length; i++)
						{
							stream.Position = units[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x03);
							stream.WriteByte(0x39);
							stream.WriteByte(0xC4);
						}
						// lehran,stephan SS rank in all weapons
						int[] bigbois = { 2964, 772};
						for (int i = 0; i < bigbois.Length; i++)
						{
							stream.Position = bigbois[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x02);
							stream.WriteByte(0x79);
							stream.WriteByte(0xFA);
						}
						// changes laguz skills from pid based to jid based
						int[] birdskills = { 82136, 82144, 82240, 82248};
						int[] heronskills = { 82328, 82336, 82344};
						for (int i = 0; i < birdskills.Length; i++)
						{
							stream.Position = birdskills[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x02);
							stream.WriteByte(0xBB);
							if (i % 2 == 0)
								stream.WriteByte(0x12);
							else
								stream.WriteByte(0x05);

						}
						for (int i = 0; i < heronskills.Length; i++)
						{
							stream.Position = heronskills[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x02);
							stream.WriteByte(0xBC);
							if (i == 0)
								stream.WriteByte(0x25);
							else if (i == 1)
								stream.WriteByte(0x35);
							else
								stream.WriteByte(0x48);
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 23: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// removes weapon level caps on all classes
		private void removeWeapCaps()
		{
			string line;
			string[] values;
			int[] capLocation = new int[157];

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\WeapCaps.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all 157 classes
			for (int i = 0; i < 157; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				capLocation[i] = Convert.ToInt32(values[1]);
			}

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < 157; i++)
					{
						stream.Position = capLocation[i];
						
						// oops all SS rank
						stream.WriteByte(0x02);
						stream.WriteByte(0x79);
						stream.WriteByte(0xFA);
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 26: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		// adds back in aspects of the game that were taken away
		// in hard mode for no understandable reason
		private void makeHardModeGreatAgain()
		{
			string mainFile = dataLocation.Remove(dataLocation.Length - 5, 5) + "sys\\main.dol";
			try 
			{
				// open main.dol
				using (var stream = new System.IO.FileStream(mainFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					if (gameVersion == 0)
					{
						if (cbxEnemyRange.Checked == true)
						{
							stream.Position = 1628235;
							stream.WriteByte(3);

							stream.Position = 322075;
							stream.WriteByte(3);

							stream.Position = 408031;
							stream.WriteByte(3);
						}
						if (cbxWeapTri.Checked == true)
						{
							stream.Position = 157227;
							stream.WriteByte(3);
						}
						if (cbxMapAff.Checked == true)
						{
							stream.Position = 1004883;
							stream.WriteByte(3);

							stream.Position = 1135043;
							stream.WriteByte(3);
						}
					}
					else if (gameVersion == 1)
					{
						if (cbxEnemyRange.Checked == true)
						{
							stream.Position = 1628171;
							stream.WriteByte(3);

							stream.Position = 322011;
							stream.WriteByte(3);

							stream.Position = 407967;
							stream.WriteByte(3);
						}
						if (cbxWeapTri.Checked == true)
						{
							stream.Position = 157163;
							stream.WriteByte(3);
						}
						if (cbxMapAff.Checked == true)
						{
							stream.Position = 1004819;
							stream.WriteByte(3);

							stream.Position = 1134979;
							stream.WriteByte(3);
						}
					}
					else if (gameVersion == 2)
					{
						if (cbxEnemyRange.Checked == true)
						{
							stream.Position = 1523555;
							stream.WriteByte(3);

							stream.Position = 209219;
							stream.WriteByte(3);

							stream.Position = 295307;
							stream.WriteByte(3);
						}
						if (cbxWeapTri.Checked == true)
						{
							stream.Position = 44171;
							stream.WriteByte(3);
						}
						if (cbxMapAff.Checked == true)
						{
							stream.Position = 898031;
							stream.WriteByte(3);

							stream.Position = 1029303;
							stream.WriteByte(3);
						}
					}
				}
			}
			catch 
			{
				textBox1.Text = "Error 25: Cannot find DATA/sys/main.dol. Abandoning randomization...";
				errorflag = 1;
			}
		}

		// compresses fe10data
		private void closingRemarks()
		{
			if (errorflag == 0)
				textBox1.Text = "Compressing FE10Data...";
			byte[] entirefreakingfile;
			if (cbxRandBosses.Checked | (cbxRandRecr.Checked == true & cbxEnemyRecruit.Checked == true) | cbxRandEnemy.Checked == true)
				entirefreakingfile = new byte[284682];
			else
				entirefreakingfile = new byte[285118];

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			// open data file
			using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
			{
				stream.Position = 0;
				
				stream.Read(entirefreakingfile, 0, entirefreakingfile.Length);
			}
			byte[] newfreakingfile = LZ77.Compress(entirefreakingfile);

			using (var stream = new System.IO.FileStream(dataLocation + "\\FE10Data.cms", System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
			{
				stream.Position = 0;
				foreach (byte data in newfreakingfile)
					stream.WriteByte(data);
			}
		}

		// writes outputlog for user
		private void outputLog()
		{
			System.IO.StreamWriter logwriter = new System.IO.StreamWriter(file + "\\outputlog.htm");

			System.IO.StreamReader reader = new System.IO.StreamReader(file + "\\assets\\logheader.txt");
			string outlogtext = reader.ReadToEnd();
			reader.Close();

			outlogtext += "<h2>Seed: " + numericSeed.Value + "</h2><div class=\"tab\">";
			for (charNum = 0; charNum < charName.Length; charNum++)
				outlogtext += "<button class=\"tablinks\" onclick=\"openChar(event, '" + charName[charNum] +
					"')\" id=\"defaultOpen\"><img src=\"assets/logpics/" + charName[charNum] + ".png\" alt=\"" + charName[charNum] +
					".png\" style=\"width:64px; height:64px;\"></button><br>";
			outlogtext += "</div>";

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			// open data file
			using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
			{
				for (charNum = 0; charNum < charName.Length; charNum++)
				{
					outlogtext += "<div id=\"" + charName[charNum] + "\" class=\"tabcontent\">";

					if (charNum < 72)
					{
						// go to affinity
						stream.Position = charPID[charNum] + 23;
						int affinity = stream.ReadByte();
						string AFFstring;
						switch (affinity)
						{
							case 214:
								AFFstring = "earth";
								break;
							case 221:
								AFFstring = "thunder";
								break;
							case 42:
								AFFstring = "wind";
								break;
							case 36:
								AFFstring = "water";
								break;
							case 191:
								AFFstring = "heaven";
								break;
							case 164:
								AFFstring = "fire";
								break;
							case 91:
								AFFstring = "dark";
								break;
							default:
								AFFstring = "light";
								break;
						}

						outlogtext += "<h2><img src=\"assets/logpics/" + AFFstring + ".png\" alt=\"" + AFFstring +
							".png\" style=\"width: 32px; height: 32px; \">" + char.ToUpper(charName[charNum][0]) +
							charName[charNum].Substring(1) + "</h2>";

						// go to biorhythm
						stream.Position = charBio[charNum];
						int bio = stream.ReadByte();
						string biostring = bio.ToString();

						outlogtext += "<img src=\"assets/logpics/bio" + bio.ToString() + ".png\" alt=\"" +
							bio.ToString() + ".png\" style=\"width: 256px; height: 64px;\">";
					}

					// random recruit
					if (cbxRandRecr.Checked == true & (charNum < 72 | (cbxEnemyRecruit.Checked == true & charNum < 83) | 
															cbxEnemyRecruit.Checked == true & cbxClassRand.Checked == true))
					{
						outlogtext += "<h4>Character</h4><img src=\"assets/logpics/" + charName[newRecr[charNum]] +
							".png\" alt=\"" + charName[newRecr[charNum]] + ".png\" style=\"width: 64px; height: 64px; \">";
					}

					if (charNum < 72 & (cbxClassRand.Checked == true | cbxRandRecr.Checked == true))
					{
						// class
						// input class list
						string[] classList = System.IO.File.ReadAllLines(file +
							"\\assets\\classnames.txt");
						outlogtext += "<h4>Class</h4><p>" + classList[newClass[charNum]] + "</p>";


						// go to laguz gauge
						outlogtext += "<h4>Transformation Gauge</h4><table><tr><th>+/turn</th>" +
							"<th>+/battle</th><th>-/turn</th><th>-/battle</th></tr><tr>";
						stream.Position = charBio[charNum] + 5;
						for (int k = 0; k < 4; k++)
						{
							int laguzstuff = stream.ReadByte();
							if (laguzstuff > 127)
								laguzstuff -= 256;
							outlogtext += "<td>" + laguzstuff.ToString() + "</td>";
						}
						outlogtext += "</tr></table>";

						// go to bases
						outlogtext += "<h4>Personal Bases</h4><table><tr><th>HP</th><th>STR</th><th>MAG</th>" +
							"<th>SKL</th><th>SPD</th><th>LCK</th><th>DEF</th><th>RES</th></tr><tr>";
						stream.Position = charBases[charNum];
						for (int k = 0; k < 8; k++)
						{
							int basestuff = stream.ReadByte();
							if (basestuff > 127)
								basestuff -= 256;
							outlogtext += "<td>" + basestuff.ToString() + "</td>";
						}
						outlogtext += "</tr></table>";

						// go to growths
						outlogtext += "<h4>Growths</h4><table><tr><th>HP</th><th>STR</th><th>MAG</th>" +
							"<th>SKL</th><th>SPD</th><th>LCK</th><th>DEF</th><th>RES</th></tr><tr>";
						stream.Position = charGrowth[charNum];
						for (int k = 0; k < 8; k++)
						{
							int growthstuff = stream.ReadByte();
							outlogtext += "<td>" + growthstuff.ToString() + "</td>";
						}
						outlogtext += "</tr></table>";

						if (cbxSkillRand.Checked == true)
						{
							outlogtext += "<h4>Skills</h4>";
							for (int k = 0; k < 4; k++)
							{
								if (newSkills[charNum, k] != "" & newSkills[charNum,k] != null)
								{
									outlogtext += "<div class=\"img_wrap\"><img src=\"assets/logpics/" + newSkills[charNum, k].ToLower() + ".png\" alt=\"" +
										newSkills[charNum, k].ToLower() + ".png\" style=\"width: 64px; height: 64px; \"><p class=\"img_description\">" +
										newSkills[charNum, k].ToLower() + "</p></div>";
								}
							}
						}
					}

					outlogtext += "</div>";

				}
			}

			reader = new System.IO.StreamReader(file + "\\assets\\logscript.txt");
			outlogtext += reader.ReadToEnd();
			reader.Close();



			if (cbxTowerUnits.Checked == true)
			{
				outlogtext += "<br><br><h2 id=\"tower\">Tower Units</h2>";

				outlogtext += "<img src=\"assets/logpics/ike.png\" alt=\"ike.png\" style=\"width:64px;height:64px;\">" +
					"<img src=\"assets/logpics/micaiah.png\" alt=\"micaiah.png\" style=\"width:64px;height:64px;\">" +
					"<img src=\"assets/logpics/sothe.png\" alt=\"sothe.png\" style=\"width:64px;height:64px;\">" +
					"<img src=\"assets/logpics/sanaki.png\" alt=\"sanaki.png\" style=\"width:64px;height:64px;\">";

				if (cbxKurthEna.Checked != true)
				{
					outlogtext += "<img src=\"assets/logpics/kurthnaga.png\" alt=\"kurthnaga.png\" style=\"width:64px;height:64px;\">" +
					"<img src=\"assets/logpics/ena.png\" alt=\"ena.png\" style=\"width:64px;height:64px;\">";
				}
				else
				{
					outlogtext += "<img src=\"assets/logpics/" + towerUnits[10] + ".png\" alt=\"" + towerUnits[10] +  ".png\" style=\"width:64px;height:64px;\">" +
					"<img src=\"assets/logpics/" + towerUnits[11] +  ".png\" alt=\"" + towerUnits[11] + ".png\" style=\"width:64px;height:64px;\">";
				}

				for (int k = 0; k < 10; k++)
				{
					outlogtext += "<img src=\"assets/logpics/" + towerUnits[k] + ".png\" alt=\"" + towerUnits[k] + ".png\" style=\"width:64px;height:64px;\">";
				}



			}

			outlogtext += "</body></html>";

			logwriter.WriteLine(outlogtext);
			logwriter.Close();


		}




		// FRONT PANEL CONTROL FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		private void numericLaguzMin_ValueChanged(object sender, EventArgs e)
		{
			numericLaguzMax1.Minimum = numericLaguzMin1.Value;
		}
		private void numericLaguzMax_ValueChanged(object sender, EventArgs e)
		{
			numericLaguzMin1.Maximum = numericLaguzMax1.Value;
		}

		private void numericLaguzMin2_ValueChanged(object sender, EventArgs e)
		{
			numericLaguzMax2.Minimum = numericLaguzMin2.Value;
		}
		private void numericLaguzMax2_ValueChanged(object sender, EventArgs e)
		{
			numericLaguzMin2.Maximum = numericLaguzMax2.Value;
		}

		private void numericLaguzMin3_ValueChanged(object sender, EventArgs e)
		{
			numericLaguzMax3.Minimum = numericLaguzMin3.Value;
		}
		private void numericLaguzMax3_ValueChanged(object sender, EventArgs e)
		{
			numericLaguzMin3.Maximum = numericLaguzMax3.Value;
		}

		private void numericLaguzMin4_ValueChanged(object sender, EventArgs e)
		{
			numericLaguzMax4.Minimum = numericLaguzMin4.Value;
		}
		private void numericLaguzMax4_ValueChanged(object sender, EventArgs e)
		{
			numericLaguzMin4.Maximum = numericLaguzMax4.Value;
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{

			if (folderBD.ShowDialog() == DialogResult.OK)
			{
				dataLocation = folderBD.SelectedPath;
				lblLocation.Text = dataLocation;

				button1.Enabled = true;

				textBox1.Text = "Select desired randomization settings, then press the randomize button.";
				Application.DoEvents();
			}

		}

		private void cbxClassRand_CheckedChanged(object sender, EventArgs e)
		{
			panelClass.Enabled = cbxClassRand.Checked;
			cbxChooseIke.Enabled = cbxClassRand.Checked;

			if (cbxEnemyRecruit.Checked == true)
			{
				if (cbxClassRand.Checked == true)
				{
					comboLord.Items.Add("blackknight");
					comboLord.Items.Add("ashera");
				}
				else
				{
					if (comboLord.SelectedIndex >= 84)
						comboLord.SelectedIndex = 34;
					comboLord.Items.RemoveAt(84);
					comboLord.Items.RemoveAt(84);
				}
			}
		}

		private void cbxGrowthRand_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxGrowthRand.Checked == false)
				panelGrowths.Enabled = false;
			else
			{
				panelGrowths.Enabled = true;
				cbxZeroGrowths.Checked = false;
				cbxGrowthShuffle.Checked = false;
			}
		}

		private void cbxZeroGrowths_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxZeroGrowths.Checked == true)
			{
				cbxGrowthRand.Checked = false;
				cbxGrowthShuffle.Checked = false;
			}
		}

		private void cbxRandWeap_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxRandWeap.Checked == false)
				panelWeap.Enabled = false;
			else
				panelWeap.Enabled = true;
		}

		private void weaponStatsChanged(object sender, EventArgs e)
		{
			numericMTmin.Maximum = numericMTmax.Value;
			numericMTmax.Minimum = numericMTmin.Value;
			numericACCmin.Maximum = numericACCmax.Value;
			numericACCmax.Minimum = numericACCmin.Value;
			numericWTmin.Maximum = numericWTmax.Value;
			numericWTmax.Minimum = numericWTmin.Value;
			numericUSEmin.Maximum = numericUSEmax.Value;
			numericUSEmax.Minimum = numericUSEmin.Value;
			numericCRTmin.Maximum = numericCRTmax.Value;
			numericCRTmax.Minimum = numericCRTmin.Value;
		}

		private void cbxRandBases_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxRandBases.Checked == true)
				cbxShuffleBases.Checked = false;
		}

		private void cbxShuffleBases_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxShuffleBases.Checked == true)
			{
				cbxRandBases.Checked = false;
				cbxHPLCKShuffle.Enabled = true;
			}
			else
				cbxHPLCKShuffle.Enabled = false;
		}

		private void comboClassOptions_SelectedIndexChanged(object sender, EventArgs e)
		{
			// can't random recruitment with option 10
			if (comboClassOptions.SelectedIndex == 10)
			{
				cbxRandRecr.Checked = false;
				cbxRandRecr.Enabled = false;
			}
			else
				cbxRandRecr.Enabled = true;

			// can only prevent early laguz in full random
			if (comboClassOptions.SelectedIndex == 1)
				cbxNoLaguz.Enabled = true;
			else
				cbxNoLaguz.Enabled = false;

			weightflag = 1;
			// change weights
			if (comboClassOptions.SelectedIndex == 2)
			{
				radioArmor3.Checked = true;
				radioBeast0.Checked = true;
				radioBird0.Checked = true;
				radioCav3.Checked = true;
				radioDragon0.Checked = true;
				radioFly3.Checked = true;
				radioInfantry3.Checked = true;
				radioMages3.Checked = true;
			}
			else if (comboClassOptions.SelectedIndex == 3)
			{
				radioArmor0.Checked = true;
				radioBeast3.Checked = true;
				radioBird3.Checked = true;
				radioCav0.Checked = true;
				radioDragon3.Checked = true;
				radioFly0.Checked = true;
				radioInfantry0.Checked = true;
				radioMages0.Checked = true;
			}
			else if (comboClassOptions.SelectedIndex == 4)
			{
				radioArmor0.Checked = true;
				radioBeast0.Checked = true;
				radioBird0.Checked = true;
				radioCav0.Checked = true;
				radioDragon0.Checked = true;
				radioFly0.Checked = true;
				radioInfantry5.Checked = true;
				radioMages0.Checked = true;
			}
			else if (comboClassOptions.SelectedIndex == 5)
			{
				radioArmor0.Checked = true;
				radioBeast0.Checked = true;
				radioBird0.Checked = true;
				radioCav0.Checked = true;
				radioDragon0.Checked = true;
				radioFly0.Checked = true;
				radioInfantry0.Checked = true;
				radioMages5.Checked = true;
			}
			else if (comboClassOptions.SelectedIndex == 6)
			{
				radioArmor0.Checked = true;
				radioBeast0.Checked = true;
				radioBird0.Checked = true;
				radioCav5.Checked = true;
				radioDragon0.Checked = true;
				radioFly0.Checked = true;
				radioInfantry0.Checked = true;
				radioMages0.Checked = true;
			}
			else if (comboClassOptions.SelectedIndex == 7)
			{
				radioArmor5.Checked = true;
				radioBeast0.Checked = true;
				radioBird0.Checked = true;
				radioCav0.Checked = true;
				radioDragon0.Checked = true;
				radioFly0.Checked = true;
				radioInfantry0.Checked = true;
				radioMages0.Checked = true;
			}
			else if (comboClassOptions.SelectedIndex == 8)
			{
				radioArmor0.Checked = true;
				radioBeast0.Checked = true;
				radioBird5.Checked = true;
				radioCav0.Checked = true;
				radioDragon0.Checked = true;
				radioFly5.Checked = true;
				radioInfantry0.Checked = true;
				radioMages0.Checked = true;
			}
			else if (comboClassOptions.SelectedIndex == 9)
			{
				radioArmor0.Checked = true;
				radioBeast5.Checked = true;
				radioBird0.Checked = true;
				radioCav0.Checked = true;
				radioDragon5.Checked = true;
				radioFly0.Checked = true;
				radioInfantry0.Checked = true;
				radioMages0.Checked = true;
			}
			else if (comboClassOptions.SelectedIndex == 10)
			{
				radioArmor0.Checked = true;
				radioBeast0.Checked = true;
				radioBird0.Checked = true;
				radioCav0.Checked = true;
				radioDragon0.Checked = true;
				radioFly0.Checked = true;
				radioInfantry0.Checked = true;
				radioMages5.Checked = true;
			}
			weightflag = 0;
		}

		private void radioWeights_Changed(object sender, EventArgs e)
		{
			if (weightflag == 0 & comboClassOptions.SelectedIndex != 0 & comboClassOptions.SelectedIndex != 1)
				comboClassOptions.SelectedIndex = 1;
		}

		private void cbxRandRecr_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxRandRecr.Checked == true)
				panelRecr.Enabled = true;
			else
				panelRecr.Enabled = false;
		}

		private void numericGrowthCap_ValueChanged(object sender, EventArgs e)
		{
			// change maximum of growth min numerics
			if (cbxGrowthCap.Checked == true)
			{
				numericHP.Maximum = numericGrowthCap.Value;
				numericATK.Maximum = numericGrowthCap.Value;
				numericMAG.Maximum = numericGrowthCap.Value;
				numericSKL.Maximum = numericGrowthCap.Value;
				numericSPD.Maximum = numericGrowthCap.Value;
				numericLCK.Maximum = numericGrowthCap.Value;
				numericDEF.Maximum = numericGrowthCap.Value;
				numericRES.Maximum = numericGrowthCap.Value;
			}
		}

		private void cbxGrowthCap_CheckedChanged(object sender, EventArgs e)
		{
			// change maximum of growth min numerics
			if (cbxGrowthCap.Checked == true)
			{
				numericHP.Maximum = numericGrowthCap.Value;
				numericATK.Maximum = numericGrowthCap.Value;
				numericMAG.Maximum = numericGrowthCap.Value;
				numericSKL.Maximum = numericGrowthCap.Value;
				numericSPD.Maximum = numericGrowthCap.Value;
				numericLCK.Maximum = numericGrowthCap.Value;
				numericDEF.Maximum = numericGrowthCap.Value;
				numericRES.Maximum = numericGrowthCap.Value;
			}
			else
			{
				numericHP.Maximum = 255;
				numericATK.Maximum = 255;
				numericMAG.Maximum = 255;
				numericSKL.Maximum = 255;
				numericSPD.Maximum = 255;
				numericLCK.Maximum = 255;
				numericDEF.Maximum = 255;
				numericRES.Maximum = 255;
			}
		}

		private void cbxRandEnemy_CheckedChanged(object sender, EventArgs e)
		{
			panelEnemy.Enabled = cbxRandEnemy.Checked;
		}

		private void cbxGrowthShuffle_CheckedChanged(object sender, EventArgs e)
		{
			cbxGrowthShuffleMin.Enabled = cbxGrowthShuffle.Checked;
			if (cbxGrowthShuffle.Checked == true)
			{
				cbxGrowthRand.Checked = false;
				cbxZeroGrowths.Checked = false;
			}
		}

		private void cbxEnemyRecruit_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxEnemyRecruit.Checked == true)
			{
				comboLord.Items.Add("jarod");
				comboLord.Items.Add("ludveck");
				comboLord.Items.Add("septimus");
				comboLord.Items.Add("valtome");
				comboLord.Items.Add("numida");
				comboLord.Items.Add("izuka");
				comboLord.Items.Add("hetzel");
				comboLord.Items.Add("levail");
				comboLord.Items.Add("lekain");
				comboLord.Items.Add("zelgius");
				comboLord.Items.Add("dheginsea");
				comboLord.Items.Add("sephiram");
				if (cbxClassRand.Checked == true)
				{
					comboLord.Items.Add("blackknight");
					comboLord.Items.Add("ashera");
				}
			}
			else
			{
				if (comboLord.SelectedIndex >= 72)
					comboLord.SelectedIndex = 34;
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				comboLord.Items.RemoveAt(72);
				if (cbxClassRand.Checked == true)
				{
					comboLord.Items.RemoveAt(72);
					comboLord.Items.RemoveAt(72);
				}
			}
		}

		private void cbxGaugeRand_CheckedChanged(object sender, EventArgs e)
		{
			panelLaguz.Enabled = cbxGaugeRand.Checked;
		}

		private void cbxOnlySiege_CheckedChanged(object sender, EventArgs e)
		{
			if(cbxOnlySiege.Checked == true)
				cbxNoSiege.Checked = false;
		}

		private void cbxNoSiege_CheckedChanged(object sender, EventArgs e)
		{
			if(cbxNoSiege.Checked == true)
			cbxOnlySiege.Checked = false;
		}
	}
}

