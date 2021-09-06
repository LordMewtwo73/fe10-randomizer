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

		bool restrictIke = true;

		bool abbreviateON = false;

		string randomizationSettings = "";

		// if the ISO is randomized and the user selects "continue", don't check for rando again
		bool check4rando = true;

		// arrays that hold character data
		string[] charName = new string[86];
		string[] charChapter = new string[86];
		string[] charTier = new string[86];
		int[] charLevel = new int[86];
		string[] charRace = new string[86];
		int[] charLocation = new int[86];
		int[] charAnimation = new int[86];
		int[] charSkill = new int[86];
		string[] charSkillNum = new string[86];
		int[] charGauge = new int[86];
		int[] charGrowth = new int[86];
		int[] charBases = new int[86];
		int[] charWeapNum = new int[86];
		int[] charLevLoc = new int[86];
		int[] charPID = new int[86];
		int[] charBio = new int[86];
		int[] charFID = new int[86];
		string[] charClasstype = new string[86];
		int[] vanillaClassBases = new int[86];
		int[] charVanillaClass = new int[86];

		// arrays that hold class data
		string[] claName = new string[128];
		string[] claJID = new string[128];
		string[] claPlayTier = new string[128];
		string[] claPlayWeap = new string[128];
		string[] claPlayType = new string[128];
		string[] claRace = new string[128];
		string[] claAnim = new string[128];
		string[] claEnemTier = new string[128];
		string[] claEnemWeap = new string[128];
		string[] claEnemType = new string[128];
		int[] claDataLoc = new int[128];
		string[] claPromoPath = new string[87];

		// info for outputlog
		string[,] newSkills = new string[72,4];
		string[] towerUnits = new string[12];
		string eventItemsOutput = "";
		string randEnemyOutput = "";
		string randPromoOutput = "";

		// arrays that hold new character data of random recruitment
		string[] newRace = new string[86];
		int[] newClass = new int[86];
		int[] newRecr = new int[86];
		int[] recrInverse = new int[86];
		string[] recrRace = new string[86];
		string[] recrName = new string[86];

		// bool for april fools
		bool aprilFools = false;

		// extra bytes to add to datafile
		List<byte> newpointerbytes;

		public Form1()
		{

			InitializeComponent();

			InitializeToolTips();

			comboClassOptions.SelectedIndex = 0;
			comboLord.SelectedIndex = 34;
			comboMicc.SelectedIndex = 0;
			comboMicClass.SelectedIndex = 17;
			comboIkeClass.SelectedIndex = 1;
			comboElinciaClass.SelectedIndex = 25;

			LoadSettings();

			textBox1.Text = "Welcome to LordMewtwo73's FE10 Randomizer! Please load in the DATA\\files folder of an extracted iso.";
			
			numericSeed.Value = seedGenerator.Next();
		}


		private void button1_Click(object sender, EventArgs e)
		{
			// disable components so user can't change properties during randomization
			button1.Enabled = false;
			btnLoad.Enabled = false;
			numericSeed.Enabled = false;
			tabControl1.Enabled = false;

			if (cbxRandRecr.Checked == true | cbxClassRand.Checked == true | cbxClassSwap.Checked == true | cbxRandEnemy.Checked == true |
					cbxRandBosses.Checked == true | cbxTier3Enemies.Checked == true | cbxEventItems.Checked == true | cbxEnemDrops.Checked == true |
					cbxRandShop.Checked == true | cbxBargains.Checked == true | cbxForge.Checked == true | cbxClassPatch.Checked == true | cbxTormodT3.Checked == true)
				abbreviateON = true;
			else
				abbreviateON = false;

			// force heron randomization if herons are chosen as ike/micaiah
			if (cbxRandRecr.Checked == true & (comboLord.SelectedIndex == 69 | comboLord.SelectedIndex == 70 | comboLord.SelectedIndex == 71 |
												comboMicc.SelectedIndex == 69 | comboMicc.SelectedIndex == 70 | comboMicc.SelectedIndex == 71))
			{
				cbxHerons.Checked = true;
			}

			textBox1.Text = "Initializing";
			Application.DoEvents();
			initialize();

			// beginLog();

			textBox1.Text = "Checking class weights";
			Application.DoEvents();
			checkWeights();

			

			if (errorflag == 0)
			{
				SaveSettings();
			}

			if (errorflag == 0)
			{
				textBox1.Text = "Finding game version";
				Application.DoEvents();
				getVersion();
			}

			if (errorflag == 0)
			{
				textBox1.Text = "Organizing files";
				Application.DoEvents();
				fileOrganizer();

				if (abbreviateON)
				{
					textBox1.Text = "Abbreviating IDs";
					Application.DoEvents();
					abbreviate();
				}
			}


			if ((cbxClassRand.Checked == true | cbxRandRecr.Checked == true | cbxRandEnemy.Checked == true | cbxRandBosses.Checked == true)
				& errorflag == 0)
			{
				textBox1.Text = "\"Balancing\" units";
				Application.DoEvents();
				baseStatChanges();
			}

			if (cbxClassPatch.Checked == true & errorflag == 0)
			{
				textBox1.Text = "Creating new classes";
				Application.DoEvents();
				LM73classPatch();
			}


			if (cbxRandRecr.Checked == true & errorflag == 0)
			{
				textBox1.Text = "Swaping recruitment";
				Application.DoEvents();
				recruitmentOrderRando();
				recruitBaseSwapping();
			}



			if (errorflag == 0)
			{
				textBox1.Text = "Changing classes";
				Application.DoEvents();
				Classes();
			}


			if (errorflag == 0)
			{
				textBox1.Text = "Screwing up weapons";
				Application.DoEvents();
				Weapons();
			}


			if (errorflag == 0)
			{
				textBox1.Text = "Calculating busted stats";
				Application.DoEvents();
				Stats();
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
				textBox1.Text = "Randomization Complete! Check outputlog.htm for details";
				
				// reset bool
				check4rando = true;
			}

			//dataWriter.Close();
			button1.Enabled = true;
			btnLoad.Enabled = true;
			numericSeed.Enabled = true;
			tabControl1.Enabled = true;

		}

		// INITIALIZATION FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// sets tooltips for all front panel objects
		private void InitializeToolTips()
		{
			toolTip1.SetToolTip(cbxDragonCanto, "Goldoa do be movin' tho");
			toolTip1.SetToolTip(cbxWeapPatch, "removes venin and bronze weapons from the game and replaces them with new effective weapons and weapons that effect weapon triangle differently; see README.htm for more details");
			toolTip1.SetToolTip(cbxBonusBEXP, "grants a significantly increased amount of BEXP for completing a level after each chapter");
			toolTip1.SetToolTip(cbxTormodT3, "tormod will start as a LV1 firearchsage if classes are not randomized; with random classes, he will be " +
											"treated as a t3 beorc unit or non-royal laguz unit");
			toolTip1.SetToolTip(cbxLaguzCanto, "gives the canto skill to cats, tigers, wolves, and lions");
			toolTip1.SetToolTip(cbxStatBooster, "modifies the stat change from stat boosters such as Energy Ring, Speedwing, etc; the in-game help description will show the item's new stat changes");
			toolTip1.SetToolTip(cbxStatBoostMult, "stat boosters can change more than one stat");
			toolTip1.SetToolTip(cbxNegGrowths, "units will lose stats upon level up");
			toolTip1.SetToolTip(cbxStoryPromo, "Micaiah, Sothe, and Ike will promote normally instead of being tied to game events. Automatic with class randomization");
			toolTip1.SetToolTip(cbxAuthority, "randomizes the authority stars for each unit (though your leaders are really the only ones that matter)");
			toolTip1.SetToolTip(cbxSkillCap, "changes the skill capacity cost of all skills (including t3 speciality skills like Astra)");
			toolTip1.SetToolTip(cbxMicClass, "forces Micaiah's slot to a chosen class");
			toolTip1.SetToolTip(cbxIkeClass, "forces Ike's slot to a chosen class");
			toolTip1.SetToolTip(cbxElinciaClass, "forces Elincia's slot to a chosen class");
			toolTip1.SetToolTip(cbxHeronSpread, "the three herons will be spread out, obtained in part 1, 2, and 3 respectively");
			toolTip1.SetToolTip(cbxSkillUno, "each playable character will only have one random skill when recruited");
			toolTip1.SetToolTip(cbxEasyPromotion, "physical classes will promote into other physical units, and same with magical classes");
			toolTip1.SetToolTip(cbxNoEnemyLaguz, "all beorc enemies will only be randomized into beorc classes, and laguz will stay laguz");
			toolTip1.SetToolTip(cbxParagon, "replaces the final skill of each character with Paragon");
			toolTip1.SetToolTip(cbxDarkMag, "allows light sage and light priestess classes to use dark magic as well as normal weapon types");
			toolTip1.SetToolTip(cbxClassPatch, "adds custom phys/mag hybrid classes made by LordMewtwo into the game. Can be used with or without random classes. See README.htm for futher details");
			toolTip1.SetToolTip(cbxKnifeCrit, "increases the critical rate of all knives by 5%");
			toolTip1.SetToolTip(cbxRandPromotion, "randomizes the promotion line for each beorc class that can normally promote; WARNING: Do not promote Ike before Ch3-5 without selecting Horse " +
												"Parkour, in case he promotes into a mounted unit and cannot climb ledges");
			toolTip1.SetToolTip(cbxMagicPatch, "magic tomes are given new stats as well as new effects; see README.htm for further details");
			toolTip1.SetToolTip(cbxBabyPart2, "decreases all stats of the rebellion enemies in part 2 by 3 points each");
			toolTip1.SetToolTip(cbxFormshift, "adds a formshift skill scroll, equipable to all laugz, and allows formshift to be removed from laguz royals");
			toolTip1.SetToolTip(cbxWhiteGem, "changes all hidden coins to white gems, which are also modified to be worth 30k each");
			toolTip1.SetToolTip(cbxHPShuffleclass, "adds the HP stat into the shuffle pool; this may inflate stats, as HP is usually much higher than other stats");
			toolTip1.SetToolTip(cbxShuffleClassBases, "adds up total bases (except HP&LCK) + selected addition and redistributes randomly to each stat");
			toolTip1.SetToolTip(cbxRandClassBases, "randomly changes base stats of all classes within vanilla value +/- selected deviation");
			toolTip1.SetToolTip(cbxStatCapDev, "randomly changes stat caps within vanilla value +/- selected deviation");
			toolTip1.SetToolTip(cbxStatCapFlat, "increases all stat caps by selected value");
			toolTip1.SetToolTip(cbxT3Statcaps, "changes to stat caps will only be applied to T3 units");
			toolTip1.SetToolTip(cbxSkillMax, "gives each playable character random skills equal to the maximum number of skills that can be assigned to the unit");
			toolTip1.SetToolTip(cbxSkillRand, "gives each playable character random skills equal to the number of skills they usually have");
			toolTip1.SetToolTip(cbxForge, "randomizes weapons that can be forged in each chapter using selected weights; higher number equals higher chance, 0 is no chance");
			toolTip1.SetToolTip(cbxBargains, "randomizes bargain bin items using selected weights; higher number equals higher chance, 0 is no chance");
			toolTip1.SetToolTip(cbxRandMove, "randomizes movement of all classes; swamp and ledge movement cost will be reduced to the selected minimum value to prevent softlocks");
			toolTip1.SetToolTip(cbxEnemySkills, "gives all enemies random skills up to the number selected");
			toolTip1.SetToolTip(cbxBossSkills, "gives all bosses an extra skill more than the value selected for generic enemy skills; gives 1 skill if enemy skills are not selected");
			toolTip1.SetToolTip(cbxHorseParkour, "horses can use 6 spaces of movement to climb ledges or cross swamp; allows Ike to randomize into horse classes");
			toolTip1.SetToolTip(cbxNoFOW, "sets vision of all units to max, negating fog of war mechanic");
			toolTip1.SetToolTip(cbxIronMan, "characters other than Micaiah, Ike, and Part 2 main units dying will not result in a gameover; check README.htm for more details");
			toolTip1.SetToolTip(cbxClassSwap, "changes the classes of units while keeping the total number of each class equal to vanilla");
			toolTip1.SetToolTip(cbxNoRandPromotions, "promotion items gained through events will not be randomized");
			toolTip1.SetToolTip(cbxIronShop, "every chapter's shop will have iron weapons, herbs, olivi grass, and heal staves");
			toolTip1.SetToolTip(cbxChooseMic, "allows you to select the character to replace micaiah");
			toolTip1.SetToolTip(cbxSimilarEnemy, "random enemies will be the same type as their vanilla counterpart: infantry,mounted,armored,magic,or laguz");
			toolTip1.SetToolTip(cbxHeatherBlue, "sets Heather as a player unit right as she appears in chapter 2-1");
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
			toolTip1.SetToolTip(cbxTowerUnits, "the randomizer selects 10 random characters that will be required to enter the tower alongside the usual required 6");
			toolTip1.SetToolTip(cbxChooseIke, "allows you to select the character to replace Ike");
			toolTip1.SetToolTip(cbxEnemyRecruit, "adds various major antagonists to the recruitment pool. See README.htm for more details");
			toolTip1.SetToolTip(cbxRandRecr, "changes order in which you recieve characters. See README.htm for details regarding what stats are modified");
			toolTip1.SetToolTip(cbxGrowthRand, "randomizes each growth individually based off of original +/- deviation and limited by minimums");
			toolTip1.SetToolTip(cbxGrowthShuffle, "adds up total growths and redistributes randomly to each stat");
			toolTip1.SetToolTip(cbxGrowthCap, "sets maximum growths");
			toolTip1.SetToolTip(cbxBuffBosses, "increases stats on a linear scale, where early bosses gain less than lategame bosses. does not affect BK, dheginsea, and ashera");
			toolTip1.SetToolTip(cbxRandBosses, "chanegs class of all bosses. does not affect BK, dheginsea, and ashera");
			toolTip1.SetToolTip(cbxNoLaguz, "laguz can have busted stats for early game, so this prevents early laguz");
			toolTip1.SetToolTip(cbxEnemyRange, "adds enemy ranges to hardmode");
			toolTip1.SetToolTip(cbxWeapTri, "adds weapon triangle to hardmode");
			toolTip1.SetToolTip(cbxMapAff, "adds map affinities to hardmode");
			toolTip1.SetToolTip(cbxStrMag, "the higher of a unit's STR/MAG growth and personal bases will be put into STR if their class is physical or MAG if their class is magical");
			toolTip1.SetToolTip(cbxLords, "keeps Ike, Elincia, and Micaiah (or whoever replaces them with random recruitment) as Hero, Queen, and Light Mage, respectively");
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
			toolTip1.SetToolTip(cbxDBweaps, "Allows any unit to use special weapons: Thani => D; Caladbolg, Tarvos, Lughnasadh => B");
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
			System.Windows.Forms.CheckBox[] checkBoxes;
			System.Windows.Forms.ComboBox[] comboBoxes;
			System.Windows.Forms.NumericUpDown[] numericUpDowns;
			System.Windows.Forms.RadioButton[] radioButtons;

			
			


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
				string versionNum = eachsetting[0];

				// get lists based on version settings were saved from
				if (versionNum == "2.4.2" | versionNum == "2.4.3" | versionNum == "2.4.4")
				{
					checkBoxes = new System.Windows.Forms.CheckBox[] { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths, cbxSimilarEnemy, cbxGrowthShuffleMax,
														   cbxHeatherBlue,cbxChooseMic,cbxIronShop,cbxClassSwap,cbxNoRandPromotions,cbxStatCapDev,
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillMax, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxBabyPart2,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz,cbxBonusBEXP, cbxTormodT3, cbxLaguzCanto, cbxStatBooster,
														   cbxStatBoostMult, cbxNegGrowths, cbxStoryPromo, cbxAuthority, cbxSkillCap, cbxMicClass, cbxIkeClass,
														   cbxHeronSpread, cbxSkillUno, cbxWeapPatch, cbxDragonCanto, cbxElinciaClass};

					comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboLord, comboMicc, comboIkeClass, comboMicClass, comboElinciaClass };

					numericUpDowns = new System.Windows.Forms.NumericUpDown[]  { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin, numericStatCapDev,
																numericStatCapFlat, numericMoveMin, numericMoveMax, numericEnemySkills, numericBargSword,
																numericBargLance, numericBargAxe, numericBargBow, numericBargKnife, numericBargTome,
																numericBargStave, numericBargStat, numericBargItem, numericBargSkill, numericForgeSword,
																numericForgeLance, numericForgeAxe, numericForgeBow, numericForgeKnife, numericForgeTome,
																numericClassBaseDev, numericClassBaseShuf, numericStatCapMin,numStatBoostMin, numStatBoostMax};

					radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
				}
				else if (versionNum == "2.4.1")
				{
					checkBoxes = new System.Windows.Forms.CheckBox[] { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths, cbxSimilarEnemy, cbxGrowthShuffleMax,
														   cbxHeatherBlue,cbxChooseMic,cbxIronShop,cbxClassSwap,cbxNoRandPromotions,cbxStatCapDev,
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillMax, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxBabyPart2, 
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz,cbxBonusBEXP, cbxTormodT3, cbxLaguzCanto, cbxStatBooster, 
														   cbxStatBoostMult, cbxNegGrowths, cbxStoryPromo, cbxAuthority, cbxSkillCap, cbxMicClass, cbxIkeClass, 
														   cbxHeronSpread, cbxSkillUno, cbxWeapPatch};

					comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboLord, comboMicc, comboIkeClass, comboMicClass };

					numericUpDowns = new System.Windows.Forms.NumericUpDown[]  { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin, numericStatCapDev,
																numericStatCapFlat, numericMoveMin, numericMoveMax, numericEnemySkills, numericBargSword,
																numericBargLance, numericBargAxe, numericBargBow, numericBargKnife, numericBargTome,
																numericBargStave, numericBargStat, numericBargItem, numericBargSkill, numericForgeSword,
																numericForgeLance, numericForgeAxe, numericForgeBow, numericForgeKnife, numericForgeTome,
																numericClassBaseDev, numericClassBaseShuf, numericStatCapMin,numStatBoostMin, numStatBoostMax};

					radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
				}
				else if (versionNum == "2.3.2" | versionNum == "2.3.1")
				{
					checkBoxes = new System.Windows.Forms.CheckBox[] { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths, cbxSimilarEnemy, cbxGrowthShuffleMax,
														   cbxHeatherBlue,cbxChooseMic,cbxIronShop,cbxClassSwap,cbxNoRandPromotions,cbxStatCapDev,
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillMax, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxBabyPart2,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz};

					comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboLord, comboMicc };

					numericUpDowns = new System.Windows.Forms.NumericUpDown[]  { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin, numericStatCapDev,
																numericStatCapFlat, numericMoveMin, numericMoveMax, numericEnemySkills, numericBargSword,
																numericBargLance, numericBargAxe, numericBargBow, numericBargKnife, numericBargTome,
																numericBargStave, numericBargStat, numericBargItem, numericBargSkill, numericForgeSword,
																numericForgeLance, numericForgeAxe, numericForgeBow, numericForgeKnife, numericForgeTome,
																numericClassBaseDev, numericClassBaseShuf, numericStatCapMin};

					radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
				}
				else if (versionNum == "2.3.0")
				{
					checkBoxes = new System.Windows.Forms.CheckBox[] { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths, cbxSimilarEnemy, cbxGrowthShuffleMax,
														   cbxHeatherBlue,cbxChooseMic,cbxIronShop,cbxClassSwap,cbxNoRandPromotions,cbxStatCapDev,
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillMax, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxBabyPart2, cbxParagon};

					comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboLord, comboMicc };

					numericUpDowns = new System.Windows.Forms.NumericUpDown[]  { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin, numericStatCapDev,
																numericStatCapFlat, numericMoveMin, numericMoveMax, numericEnemySkills, numericBargSword,
																numericBargLance, numericBargAxe, numericBargBow, numericBargKnife, numericBargTome,
																numericBargStave, numericBargStat, numericBargItem, numericBargSkill, numericForgeSword,
																numericForgeLance, numericForgeAxe, numericForgeBow, numericForgeKnife, numericForgeTome,
																numericClassBaseDev, numericClassBaseShuf, numericStatCapMin};

					radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
				}
				else if (versionNum == "2.2.2")
				{
					checkBoxes = new System.Windows.Forms.CheckBox[] { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths, cbxSimilarEnemy, cbxGrowthShuffleMax,
														   cbxHeatherBlue,cbxChooseMic,cbxIronShop,cbxClassSwap,cbxNoRandPromotions,cbxStatCapDev,
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillMax, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem};

					comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboLord, comboMicc };

					numericUpDowns = new System.Windows.Forms.NumericUpDown[]  { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin, numericStatCapDev,
																numericStatCapFlat, numericMoveMin, numericMoveMax, numericEnemySkills, numericBargSword,
																numericBargLance, numericBargAxe, numericBargBow, numericBargKnife, numericBargTome,
																numericBargStave, numericBargStat, numericBargItem, numericBargSkill, numericForgeSword,
																numericForgeLance, numericForgeAxe, numericForgeBow, numericForgeKnife, numericForgeTome,
																numericClassBaseDev, numericClassBaseShuf};

					radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
				}
				else if (versionNum == "2.2.0")
				{
					checkBoxes = new System.Windows.Forms.CheckBox[] { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths, cbxSimilarEnemy, cbxGrowthShuffleMax,
														   cbxHeatherBlue,cbxChooseMic,cbxIronShop,cbxClassSwap,cbxNoRandPromotions,cbxStatCapDev,
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillMax, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass};

					comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboLord, comboMicc };

					numericUpDowns = new System.Windows.Forms.NumericUpDown[]  { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin, numericStatCapDev,
																numericStatCapFlat, numericMoveMin, numericMoveMax, numericEnemySkills, numericBargSword,
																numericBargLance, numericBargAxe, numericBargBow, numericBargKnife, numericBargTome,
																numericBargStave, numericBargStat, numericBargItem, numericBargSkill, numericForgeSword,
																numericForgeLance, numericForgeAxe, numericForgeBow, numericForgeKnife, numericForgeTome,
																numericClassBaseDev, numericClassBaseShuf};

					radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
				}
				else if (versionNum == "2.1.0")

				{
					checkBoxes = new System.Windows.Forms.CheckBox[] { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths, cbxSimilarEnemy, cbxGrowthShuffleMax,
														   cbxHeatherBlue,cbxChooseMic,cbxIronShop,cbxClassSwap,cbxNoRandPromotions};

					comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboLord, comboMicc };

					numericUpDowns = new System.Windows.Forms.NumericUpDown[]  { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin};

					radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
				}
				else if (versionNum == "2.0.2")
				{
					checkBoxes = new System.Windows.Forms.CheckBox[] { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths, cbxSimilarEnemy, cbxGrowthShuffleMax,
														   cbxHeatherBlue};

					comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboLord };

					numericUpDowns = new System.Windows.Forms.NumericUpDown[]  { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin};

					radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
				}
				else
				{
					checkBoxes = new System.Windows.Forms.CheckBox[] { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
														   cbxChooseIke, cbxClassRand, cbxDBweaps, cbxDragonSkills, cbxEnemDrops, cbxEnemHealers,
														   cbxEnemWeaps, cbxEnemyGrowth, cbxEnemyRange, cbxEnemyRecruit, cbxEventItems, cbxFionaAI,
														   cbxFireMag, cbxFlorete, cbxForge, cbxGaugeRand, cbxGMweaps, cbxGrowthCap, cbxGrowthRand,
														   cbxGrowthShuffle, cbxGrowthShuffleMin, cbxHerons, cbxHPLCKShuffle, cbxJillAI, cbxKurthEna,
														   cbxLaguzWeap, cbxLethality, cbxLords, cbxLowerPrice, cbxMapAff, cbxNihil, cbxNoLaguz,
														   cbxNoSiege, cbxOnlySiege, cbxRandBases, cbxRandBosses, cbxRandEnemy, cbxRandRecr,
														   cbxRandShop, cbxRandWeap, cbxSellableItems, cbxShuffleBases, cbxSiegeUse, cbxSkillRand,
														   cbxSpirits, cbxStatCaps, cbxStaveUse, cbxStrMag, cbxThieves, cbxTier3Enemies, cbxTowerUnits,
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths};

					comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboLord };

					numericUpDowns = new System.Windows.Forms.NumericUpDown[]  { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin};

					radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
				}


				int total = numericUpDowns.Length + checkBoxes.Length + comboBoxes.Length + radioButtons.Length;

				if (eachsetting[0].Contains("."))
				{
					for (int i = 1; i < eachsetting.Length; i++)
						eachsetting[i - 1] = eachsetting[i];
				}

				for (int i = 0; i < numericUpDowns.Length; i++)
				{
					int updownvalue = Convert.ToInt32(eachsetting[i]);
					if (updownvalue > numericUpDowns[i].Maximum)
						updownvalue = (int)numericUpDowns[i].Maximum;
					else if (updownvalue < numericUpDowns[i].Minimum)
						updownvalue = (int)numericUpDowns[i].Minimum;
					numericUpDowns[i].Value = updownvalue;
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
														   cbxWeapCaps, cbxWeapTri, cbxWinCon, cbxZeroGrowths, cbxSimilarEnemy, cbxGrowthShuffleMax,
														   cbxHeatherBlue,cbxChooseMic,cbxIronShop,cbxClassSwap,cbxNoRandPromotions,cbxStatCapDev,
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillMax, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxBabyPart2,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz,cbxBonusBEXP, cbxTormodT3, cbxLaguzCanto, cbxStatBooster, 
														   cbxStatBoostMult, cbxNegGrowths, cbxStoryPromo, cbxAuthority, cbxSkillCap, cbxMicClass, cbxIkeClass, 
														   cbxHeronSpread, cbxSkillUno, cbxWeapPatch,cbxDragonCanto,cbxElinciaClass};


			System.Windows.Forms.ComboBox[] comboBoxes = { comboClassOptions, comboLord, comboMicc, comboIkeClass, comboMicClass, comboElinciaClass };

			System.Windows.Forms.NumericUpDown[] numericUpDowns = { numericACCdev, numericACCmax, numericACCmin, numericATK, numericBaseRand,
																numericBaseShuffle, numericBossStats, numericCRTdev, numericCRTmax, numericCRTmin,
																numericDEF, numericEnemyGrowth, numericGrowth, numericGrowthCap, numericGrowthShuffle,
																numericHP, numericLaguzMax1, numericLaguzMax2, numericLaguzMax3, numericLaguzMax4,
																numericLaguzMin1, numericLaguzMin2, numericLaguzMin3, numericLaguzMin4, numericLCK,
																numericMAG, numericMTdev, numericMTmax, numericMTmin, numericRES, numericSKL,
																numericSPD, numericStatCap1, numericStatCap2, numericStatCap3, numericUSEdev,
																numericUSEmax, numericUSEmin, numericWTdev, numericWTmax, numericWTmin, numericStatCapDev,
																numericStatCapFlat, numericMoveMin, numericMoveMax, numericEnemySkills, numericBargSword,
																numericBargLance, numericBargAxe, numericBargBow, numericBargKnife, numericBargTome,
																numericBargStave, numericBargStat, numericBargItem, numericBargSkill, numericForgeSword,
																numericForgeLance, numericForgeAxe, numericForgeBow, numericForgeKnife, numericForgeTome,
																numericClassBaseDev, numericClassBaseShuf, numericStatCapMin,numStatBoostMin, numStatBoostMax};

			System.Windows.Forms.RadioButton[] radioButtons = { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};

			string settingstring = "2.4.4,";
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

			randomizationSettings = settingstring;

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
				charChapter[i] = values[2];
				// what tier they are; a=tierone ie. archer, b=tiertwo ie. sniper, c=tierthree
				charTier[i] = values[3];
				// starting level of character
				charLevel[i] = Convert.ToInt32(values[4]);
				// race of character; B=beorc, L=laguz
				charRace[i] = values[5];
				// location of character's class in chapter data
				charLocation[i] = Convert.ToInt32(values[6]);
				// location of character's animation in big Data file
				charAnimation[i] = Convert.ToInt32(values[1]) + 52;
				// location of character's skills in big Data file
				// number of skills character has
				charSkill[i] = Convert.ToInt32(values[1]) + 32;
				charSkillNum[i] = values[7];
				// location of character's laguz gauge in big Data file
				charGauge[i] = Convert.ToInt32(values[1]) + 73;
				// location of character's growth rates in big Data file
				charGrowth[i] = Convert.ToInt32(values[1]) + 87;
				// location of character's base stats
				charBases[i] = Convert.ToInt32(values[1]) + 77;
				// number of weapons in character's starting inventory
				charWeapNum[i] = Convert.ToInt32(values[8]);
				// location of characters level in chapter data
				charLevLoc[i] = Convert.ToInt32(values[9]);
				// location of characters PID
				charPID[i] = Convert.ToInt32(values[1]) + 4;
				// location of characters biorhythm
				charBio[i] = Convert.ToInt32(values[1]) + 68;
				// location of FID in facedata.bin
				charFID[i] = Convert.ToInt32(values[10]);
				// whether a unit is physical (p) or magical (m)
				charClasstype[i] = values[11];
				// location of class bases for vanilla class of character
				vanillaClassBases[i] = Convert.ToInt32(values[12]);
				// class number for vanilla class
				charVanillaClass[i] = Convert.ToInt32(values[13]);
			}
			if (cbxTormodT3.Checked == true)
			{
				charTier[14] = "d";
				charLevel[14] = 1;
			}

			dataReader.Close();

			// initialize class information
			dataReader = new System.IO.StreamReader(file + "\\assets\\ClassInfo.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all classes
			for (int i = 0; i < claName.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				// class name
				claName[i] = values[1];
				// class jid
				claJID[i] = values[2];
				// class tier for player purposes
				claPlayTier[i] = values[3];
				// class weapon type for player
				claPlayWeap[i] = values[4];
				// class type for player purposes
				claPlayType[i] = values[5];
				// class race
				claRace[i] = values[6];
				// class animations
				claAnim[i] = values[7];
				// class tier for enemy purposes
				claEnemTier[i] = values[8];
				// class weapon type for enemy
				claEnemWeap[i] = values[9];
				// class type for enemy purposes
				claEnemType[i] = values[10];
				// location of class data in big data file
				claDataLoc[i] = Convert.ToInt32(values[11]);
			}
			dataReader.Close();

			// modify classes if patch enabled
			if (cbxClassPatch.Checked == true)
			{
				// rogue and whisper will use female classes
				claJID[42] = "JID_KI5";
				claJID[43] = "JID_KI6";
				claDataLoc[42] = 47576;
				claDataLoc[43] = 47848;
				// reaver/assassin can use bows
				claPlayWeap[25] = "A;B;G";
				claPlayWeap[44] = "K;B;K";
				claEnemWeap[44] = "K;B";
				// queen can use light
				claPlayWeap[67] = "S;H;M";
				claEnemWeap[67] = "S;M";
				// cleric/valkyrie can use wind
				claPlayWeap[64] = "S;H;W";
				claPlayWeap[65] = "S;H;W";
				// windarchsage turns into son of ashnard
				claName[53] = "sonofashnard";
				claPlayWeap[53] = "W;S;F";
				claEnemWeap[53] = "W;S";
				claEnemType[53] = "F";
				claPlayType[53] = "F";
				// change animation pointers
				claAnim[0] = "0;2";
				claAnim[1] = "0;2";
				claAnim[2] = "0;2";
				claAnim[11] = "11;12";
				claAnim[12] = "11;12";
				claAnim[13] = "11;12";
				claAnim[14] = "18;18";
				claAnim[15] = "18;18";
				claAnim[16] = "18;18";
				claAnim[32] = "28;28";
				claAnim[33] = "28;28";
				claAnim[34] = "28;28";
				claAnim[41] = "9;9";
				claAnim[42] = "9;9";
				claAnim[43] = "9;9";
				claAnim[45] = "33;33";
				claAnim[46] = "33;33";
				claAnim[47] = "33;33";
				claAnim[48] = "35;36";
				claAnim[49] = "35;36";
				claAnim[50] = "35;36";
				claAnim[51] = "37;37";
				claAnim[52] = "37;37";
				claAnim[53] = "37;37";
				claAnim[57] = "41;42";
				claAnim[58] = "41;42";
				claAnim[59] = "41;42";
				// change vanilla classes of certain characters
				charVanillaClass[5] = 73; // sothe trickster
				charVanillaClass[10] = 82; // jill malig kn
				charVanillaClass[12] = 69; // zihark dread fighter
				charVanillaClass[13] = 77; // fiona thnder kn
				charVanillaClass[23] = 71; // nephenee lancer
				charVanillaClass[29] = 80; // kieran fire pal
				charVanillaClass[33] = 75; // calill bartendress
				charVanillaClass[40] = 78; // oscar thunder pal
				charVanillaClass[43] = 85; // rhys warmonk
				charVanillaClass[57] = 70; // stefan yasha
				charVanillaClass[58] = 86; // oliver crusader
			}
			else
			{
				// disable custom classes
				for (int i = 69; i < 87; i++)
				{
					claPlayTier[i] = "x";
					claEnemTier[i] = "x";
				}
			}
			if (cbxDarkMag.Checked == true)
			{
				claPlayWeap[55] = "M;H;D";
				claPlayWeap[56] = "M;H;D";
				claEnemWeap[55] = "M;D";
				claEnemWeap[56] = "M;D";
			}
			if (cbxFireMag.Checked == true)
			{
				claPlayWeap[62] = "D;F;D";
				claPlayWeap[63] = "D;F;H";
				claEnemWeap[62] = "D;F";
				claEnemWeap[63] = "D;F";
			}

			// heron number
			heronNumber = 0;

			// reset error flag
			errorflag = 0;

			// reset pointer bytes
			newpointerbytes = new List<byte>();

			// set number of units to change
			totalUnitNumber = 72;

			// see if ike's class needs to be restricted
			if (cbxStatCaps.Checked == false & cbxBKfight.Checked == false & cbxBKnerf.Checked == false)
				restrictIke = true;
			else
				restrictIke = false;

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
			if (cbxChooseIke.Checked == true & cbxChooseMic.Checked == true & comboLord.SelectedIndex == comboMicc.SelectedIndex)
			{
				textBox1.Text = "Micaiah and Ike cannot be the same character!";
				errorflag = 1;
			}

			if (cbxClassRand.Checked == true & ((cbxMicClass.Checked == true & comboMicClass.SelectedItem.ToString() == "-") | 
												(cbxIkeClass.Checked == true & comboIkeClass.SelectedItem.ToString() == "-") |
												(cbxElinciaClass.Checked == true & comboElinciaClass.SelectedItem.ToString() == "-")))
			{
				textBox1.Text = "Please select valid class for Ike, Micaiah and/or Elincia";
				errorflag = 1;
			}


			// check to make sure at least one class weight overall is greater than 0
			if (cbxClassRand.Checked == true & (
				(radioBeast0.Checked == true & radioBird0.Checked == true & radioDragon0.Checked == true) &
				(radioArmor0.Checked == true & radioCav0.Checked == true & radioFly0.Checked == true & radioInfantry0.Checked == true & radioMages0.Checked == true)))
			{
				textBox1.Text = "At least one class type must have a non-zero weight!";
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
				textBox1.Text = "'Basic' class randomization is impossible without both laguz and beorc classes!";
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
						textBox1.Text = "Errorcode 01: Game version unknown. Randomization incomplete!";
						errorflag = 1;
					}

					// check for previously randomized iso
					if (errorflag == 0)
					{
						if (check4rando)
						{
							if (gameVersion == 0)
								stream.Position = 3527752;
							else if (gameVersion == 1)
								stream.Position = 3527592;
							else if (gameVersion == 2)
								stream.Position = 3531064;

							int prevRand = stream.ReadByte();
							if (prevRand == 0)
							{
								textBox1.Text = "You are attempting to randomize a previously randomized ISO. This may cause unforseen issues; click \"Randomize\" again to continue at your own risk";
								errorflag = 1;
								check4rando = false;
							}
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Errorcode 02: Cannot find DATA/sys/main.dol. Make sure you are selecting the correct folder. Randomization incomplete!";
				errorflag = 1;
			}
		}

		// moves edited shop, weapon, data, .cms, and chapter files
		private void fileOrganizer()
		{
			string sourcePath, targetPath, sourcefile, targetfile;

			// FE10Data
			if (cbxRandBosses.Checked | (cbxRandRecr.Checked == true & cbxEnemyRecruit.Checked == true) | cbxRandEnemy.Checked == true | cbxTier3Enemies.Checked == true)
				sourcePath = file + "\\assets\\gameData\\FE10Data.cms.decompressed";
			else
				sourcePath = file + "\\assets\\FE10Data.cms.decompressed";
			targetPath = dataLocation + "\\FE10Data.cms.decompressed";
			try
			{
				System.IO.File.Copy(sourcePath, targetPath, true);
				if (cbxRandBosses.Checked | (cbxRandRecr.Checked == true & cbxEnemyRecruit.Checked == true) | cbxRandEnemy.Checked == true)
					sourcePath = file + "\\assets\\gameData\\FE10Data.cms";
				else
					sourcePath = file + "\\assets\\FE10Data.cms";

				targetPath = dataLocation + "\\FE10Data.cms";
				System.IO.File.Copy(sourcePath, targetPath, true);
			}
			catch
			{
				textBox1.Text = "Errorcode 03: Randomizer asset files not found.  Randomization incomplete!";
				errorflag = 1;
			}


			if (abbreviateON)
			{
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
					textBox1.Text = "Errorcode 04: Randomizer asset files not found.  Randomization incomplete!";
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
						textBox1.Text = "Errorcode 05: Randomizer asset files not found.  Randomization incomplete!";
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
					textBox1.Text = "Errorcode 06: Randomizer asset files not found.  Randomization incomplete!";
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

			}
		  //}


			

			if (cbxWinCon.Checked == true)
			{
				// moves edited script files to proper folder
				sourcePath = file + "\\assets\\scriptdata\\";
				targetPath = dataLocation + "\\Scripts\\";

				sourcefile = sourcePath + "C0401.cmb";
				targetfile = targetPath + "C0401.cmb";
				System.IO.File.Copy(sourcefile, targetfile, true);

				sourcefile = sourcePath + "C0402.cmb";
				targetfile = targetPath + "C0402.cmb";
				System.IO.File.Copy(sourcefile, targetfile, true);

				sourcefile = sourcePath + "C0403.cmb";
				targetfile = targetPath + "C0403.cmb";
				System.IO.File.Copy(sourcefile, targetfile, true);

				sourcefile = sourcePath + "C0404.cmb";
				targetfile = targetPath + "C0404.cmb";
				System.IO.File.Copy(sourcefile, targetfile, true);

				sourcefile = sourcePath + "C0405.cmb";
				targetfile = targetPath + "C0405.cmb";
				System.IO.File.Copy(sourcefile, targetfile, true);

				sourcefile = sourcePath + "C0407a.cmb";
				targetfile = targetPath + "C0407a.cmb";
				System.IO.File.Copy(sourcefile, targetfile, true);
			}

			if (cbxWeapPatch.Checked == true)
			{
				// modify icons
				sourcePath = file + "\\assets\\images\\icons\\";
				targetPath = dataLocation + "\\window\\";

				sourcefile = sourcePath + "icon.cms";
				targetfile = targetPath + "icon.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "icon_wide.cms";
				targetfile = targetPath + "icon_wide.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
			}

			if (aprilFools & errorflag == 0)
			{
				sourcePath = file + "\\assets\\images\\04012021\\";
				targetPath = dataLocation + "\\Face\\wide\\";
				string[] images;
				
				try 
				{
					// collect all files in image folder
					images = System.IO.Directory.GetFiles(sourcePath);

					for (int k = 0; k < images.Length; k++)
					{
						if (images[k].Contains(".cms"))
						{
							string[] directories = images[k].Split('\\');
							sourcefile = images[k];
							targetfile = targetPath + directories[directories.Length - 1];
							System.IO.File.Copy(sourcefile, targetfile, true);
						}
					}
				}
				catch 
				{
					textBox1.Text = "Error: could not find image asset files. Randomization incomplete!";
					errorflag = 1;
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
			// moves abbreviated cms files to proper folders
			string sourcePath = file + "\\assets\\gamedata\\";
			string targetPath = dataLocation + "\\";
			string sourcefile, targetfile;
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
				if (gameVersion == 2)
					sourcefile = sourcePath + "FE10Conversation_PAL.cms";
				else
					sourcefile = sourcePath + "FE10Conversation.cms";
				targetfile = targetPath + "FE10Conversation.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "FE10Effect.cms";
				targetfile = targetPath + "FE10Effect.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "FE10Growth.cms";
				targetfile = targetPath + "FE10Growth.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "sound_data.cms";
				targetfile = targetPath + "Sound\\sound_data.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "sound_data_en.cms";
				targetfile = targetPath + "Sound\\sound_data_en.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
			}
			catch
			{
				textBox1.Text = "Errorcode 07: Randomizer asset files not found.  Randomization incomplete!";
				errorflag = 1;
			}


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

			
			// loc,loc2,class(4),level,mpid(4),mnpid(4),fid(4),aff(4),bio,auth,laguz(4),growths(8),animations(16)
			int[,] allRecrData = new int[recrName.Length, 53];

			System.IO.StreamReader dataReader;
			if (cbxClassPatch.Checked == true)
				dataReader = new System.IO.StreamReader(file + "\\assets\\classpatch\\RandoRecruitData.csv");
			else
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

				// herons can't replace t3/4 characters
				else if ((cbxClassRand.Checked == false | cbxHerons.Checked == false) & (charTier[recrInverse[69]] == "c" | charTier[recrInverse[69]] == "d" | recrInverse[69] > 71))
					tryagain = recrInverse[69];

				else if ((cbxClassRand.Checked == false | cbxHerons.Checked == false) & (charTier[recrInverse[70]] == "c" | charTier[recrInverse[70]] == "d" | recrInverse[70] > 71))
					tryagain = recrInverse[70];

				else if ((cbxClassRand.Checked == false | cbxHerons.Checked == false) & (charTier[recrInverse[71]] == "c" | charTier[recrInverse[71]] == "d" | recrInverse[71] > 71))
					tryagain = recrInverse[71];

				// herons can't replace sothe,brom,nephenee,micaiah,ike
				else if ((cbxClassRand.Checked == false | cbxHerons.Checked == false) & (newRecr[5] == 69 | newRecr[5] == 70 | newRecr[5] == 71))
					tryagain = 5;
				else if ((cbxClassRand.Checked == false | cbxHerons.Checked == false) & (newRecr[22] == 69 | newRecr[22] == 70 | newRecr[22] == 71))
					tryagain = 22;
				else if ((cbxClassRand.Checked == false | cbxHerons.Checked == false) & (newRecr[23] == 69 | newRecr[23] == 70 | newRecr[23] == 71))
					tryagain = 23;
				else if ((cbxClassRand.Checked == false | cbxHerons.Checked == false) & (newRecr[0] == 69 | newRecr[0] == 70 | newRecr[0] == 71))
					tryagain = 0;
				else if ((cbxClassRand.Checked == false | cbxHerons.Checked == false) & (newRecr[34] == 69 | newRecr[34] == 70 | newRecr[34] == 71))
					tryagain = 34;

				// heather can't be heron character if both herons and thieves can't be randomized
				else if (cbxHerons.Checked == false & cbxThieves.Checked == true & (newRecr[24] == 69 | newRecr[24] == 70 | newRecr[24] == 71))
					tryagain = 24;

				else if (cbxClassRand.Checked == false) // if classes are randomized, these don't matter
				{
					// micaiah can't be a priest/heron
					if (newRecr[0] == 4 | newRecr[0] == 43 | newRecr[0] == 58 | newRecr[0] == 69 | newRecr[0] == 70 |  // laura,rhys,oliver,rafiel,leanne
						newRecr[0] == 71 | newRecr[0] == 75 | newRecr[0] == 76 | newRecr[0] == 78 | newRecr[0] == 80)  // reyson,valtome,numida,hetzel,lekain
						tryagain = 0;

					// ike can't be mounted if horses can't climb ledges
					else if (cbxHorseParkour.Checked == false & (newRecr[34] == 13 | newRecr[34] == 28 | newRecr[34] == 29 | newRecr[34] == 30 | //fiona,geoffrey,kieran,astrid
							  newRecr[34] == 31 | newRecr[34] == 35 | newRecr[34] == 37 | newRecr[34] == 40 | newRecr[34] == 65)) //makalov,titania,mist,oscar,renning
						tryagain = 34;

					// if BK fight isn't nerfed, ike can't be rogue, raven or magic
					else if (restrictIke & (newRecr[34] == 0 | newRecr[34] == 4 | newRecr[34] == 5 | newRecr[34] == 6 |     //micaiah,laura,sothe,ilyana
							newRecr[34] == 14 | newRecr[34] == 24 | newRecr[34] == 33 | newRecr[34] == 36 | newRecr[34] == 43 | //tormod,heather,calill,soren,rhys
							newRecr[34] == 54 | newRecr[34] == 56 | newRecr[34] == 58 | newRecr[34] == 68 | newRecr[34] == 75 | //sanaki,pelleas,oliver,lehran,valtome
							newRecr[34] == 76 | newRecr[34] == 77 | newRecr[34] == 78 | newRecr[34] == 80 | newRecr[34] == 83 | //numida,izuka,hetzel,lekain,sephiran
							newRecr[34] == 16 | newRecr[34] == 20 | newRecr[34] == 53)) // vika,nealuchi,naesala
						tryagain = 34;

					// ranulf can't be mounted if horses can't climb ledges
					else if (cbxHorseParkour.Checked == false & (newRecr[45] == 13 | newRecr[45] == 28 | newRecr[45] == 29 | newRecr[45] == 30 | //fiona,geoffrey,kieran,astrid
							newRecr[45] == 31 | newRecr[45] == 35 | newRecr[45] == 37 | newRecr[45] == 40 | newRecr[45] == 65)) //makalov,titania,mist,oscar,renning
						tryagain = 45;

					// certain characters can't be certain tiers due to class
					else
					{
						int possiblenewclass;
						for (int i = 0; i < totalchars; i++)
						{
							if (charTier[i] == "a")
								possiblenewclass = allRecrData[newRecr[i], 2];
							else if (charTier[i] == "b")
								possiblenewclass = allRecrData[newRecr[i], 3];
							else if (charTier[i] == "c")
								possiblenewclass = allRecrData[newRecr[i], 4];
							else
								possiblenewclass = allRecrData[newRecr[i], 5];

							if (possiblenewclass == 999) // no possible class for this character's tier
							{
								tryagain = i;
								break;
							}
						}
					}

				}
				// however, if classes are random but boss classes aren't, this is still a problem for boss characters
				else if (cbxEnemyRecruit.Checked == true & cbxRandBosses.Checked == false)
				{
					int possiblenewclass;
					for (int i = 72; i < totalchars; i++)
					{
						if (charTier[i] == "a")
							possiblenewclass = allRecrData[newRecr[i], 2];
						else if (charTier[i] == "b")
							possiblenewclass = allRecrData[newRecr[i], 3];
						else if (charTier[i] == "c")
							possiblenewclass = allRecrData[newRecr[i], 4];
						else
							possiblenewclass = allRecrData[newRecr[i], 5];

						if (possiblenewclass == 999) // no possible class for this character's tier
						{
							tryagain = i;
							break;
						}
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
				// everything else works, now let's make sure miccy is who is picked
				else if ((cbxClassRand.Checked == true & cbxChooseMic.Checked == true &
						comboMicc.SelectedIndex != 59 & comboMicc.SelectedIndex != 60) & (newRecr[0] != comboMicc.SelectedIndex))
				{
					int current = newRecr[0];
					int desired = comboMicc.SelectedIndex;
					int otherslot = recrInverse[desired];
					newRecr[otherslot] = current;
					newRecr[0] = desired;
					recrInverse[current] = otherslot;
					recrInverse[desired] = 0;

					// then we have to go back through the loop to make sure that didn't screw anything up
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
				textBox1.Text = "Errorcode 08: Error in recruitment randomization. Randomization incomplete!";
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
				for (int m = 0; m < 2; m++)
				{
					string facefile = dataLocation;
					if (m == 0)
						facefile += "\\Face\\wide\\facedata.bin";
					else
						facefile += "\\Face\\facedata.bin";
					int[,] facedata = new int[totalchars, 28];
					// open facedata file
					using (var stream = new System.IO.FileStream(facefile, System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						// read in all face data
						for (int i = 0; i < totalchars; i++)
						{
							stream.Position = charFID[i] + 20;
							for (int j = 0; j < 28; j++)
								facedata[i, j] = stream.ReadByte();
						}

						// swap that data baby
						for (int charNum = 0; charNum < totalchars; charNum++)
						{
							stream.Position = charFID[charNum] + 20;
							for (int j = 0; j < 28; j++)
								stream.WriteByte(Convert.ToByte(facedata[newRecr[charNum], j]));
						}

						// micaiah2
						stream.Position = 6464 + 20;
						for (int i = 0; i < 28; i++)
							stream.WriteByte(Convert.ToByte(facedata[newRecr[0], i]));
						// micaiah3
						stream.Position = 6608 + 20;
						for (int i = 0; i < 28; i++)
							stream.WriteByte(Convert.ToByte(facedata[newRecr[0], i]));

						// sothe2
						stream.Position = 224 + 20;
						for (int i = 0; i < 28; i++)
							stream.WriteByte(Convert.ToByte(facedata[newRecr[5], i]));

						// lucia2
						stream.Position = 7664 + 20;
						for (int i = 0; i < 28; i++)
							stream.WriteByte(Convert.ToByte(facedata[newRecr[25], i]));

						// ike2
						stream.Position = 3344 + 20;
						for (int i = 0; i < 28; i++)
							stream.WriteByte(Convert.ToByte(facedata[newRecr[34], i]));
					}
				}
			}
			catch
			{
				textBox1.Text = "Errorcode 09: Cannot find portrait files! Randomization incomplete!";
				errorflag = 1;
			}
		}

		// swaps order of bases for characters
		private void recruitBaseSwapping()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			string line;
			string[] values;

			int[] charAddress = new int[86];
			int[,] charBaseOrder = new int[charAddress.Length, 6];

			System.IO.StreamReader dataReader;
			dataReader = new System.IO.StreamReader(file + "\\assets\\CharBaseOrder.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all
			for (int i = 0; i < charAddress.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				// character name
				charAddress[i] = Convert.ToInt32(values[1]);
				// uhh... everything else
				for (int j = 2; j < values.Length; j++)
					charBaseOrder[i, j - 2] = Convert.ToInt32(values[j]);
			}
			int totalchars = 72;
			if (cbxEnemyRecruit.Checked == true)
			{
				if (cbxClassRand.Checked == true)
					totalchars = 86;
				else
					totalchars = 84;
			}
			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < totalchars; i++)
					{
						int[] bases_vanilla = new int[6];
						int[] bases_swapped = new int[6];
						int[] order_vanilla = new int[6];
						int[] order_new = new int[6];
						// get orders
						for (int j = 0; j < 6; j++)
						{
							order_vanilla[j] = charBaseOrder[i, j];
							order_new[j] = charBaseOrder[newRecr[i], j];
						}

						// read in bases
						stream.Position = charAddress[i] + 1;
						for (int j = 0; j < 6; j++)
						{
							if (j == 4)
								// skip luck
								stream.Position += 1;
							bases_vanilla[j] = stream.ReadByte();
						}

						// swap order
						for (int j = 0; j < 6; j++)
						{
							for (int k = 0; k < 6; k++)
							{
								if (order_new[j] == order_vanilla[k])
								{
									bases_swapped[j] = bases_vanilla[k];
									break;
								}
							}
						}

						// write back to iso
						stream.Position = charAddress[i] + 1;
						for (int j = 0; j < 6; j++)
						{
							if (j == 4)
								// skip luck
								stream.Position += 1;
							stream.WriteByte(Convert.ToByte(bases_swapped[j]));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Errorcode 10: Error in recruitment stat rearranging. Randomization incomplete!";
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
				if (cbxClassRand.Checked == true & cbxClassSwap.Checked == true)
					chooseSwappedClasses();
				else if (cbxClassRand.Checked == true & cbxClassSwap.Checked == false)
					chooseRandClasses();

				classChanger();

				// heron changes
				heronChanges();

				// change laguz gauges for new classes
				laguzModifications();

				if (cbxRandPromotion.Checked == true)
					promotionSwapper();
				// change animations to new randomized classes
				if(cbxClassRand.Checked == true)
					animationChanger();

				if (cbxClassRand.Checked == true)
				{
					// uhh yes very important
					if (comboClassOptions.SelectedIndex == 10)
						veryImportantFunction();
				}
			}
			else if (cbxRandPromotion.Checked == true)
			{
				promotionSwapper();
				animationChanger();
			}

		}

		// shuffles list of vanilla classes and redistributes
		private void chooseSwappedClasses()
		{
			while (true)
			{
				List<string> swapclassname = new List<string>();
				List<int> swapt1 = new List<int>();
				List<int> swapt2 = new List<int>();
				List<int> swapt3 = new List<int>();
				List<int> swapt4 = new List<int>();
				List<string> swaprace = new List<string>();

				int redo = 0;

				System.IO.StreamReader dataReader;
				if(cbxClassPatch.Checked == true)
					dataReader = new System.IO.StreamReader(file + "\\assets\\classpatch\\classSwap.csv");
				else
					dataReader = new System.IO.StreamReader(file + "\\assets\\classSwap.csv");

				string line;
				string[] values;
				// skip header line
				line = dataReader.ReadLine();
				// loop through all
				for (int i = 0; i < 72; i++)
				{
					line = dataReader.ReadLine();
					values = line.Split(',');

					if (cbxLords.Checked == true & (i == 0 | i == 18 | i == 34))
					{
						if (i == 0)
							newClass[i] = Convert.ToInt32(values[1]);
						else if (i == 18)
							newClass[i] = Convert.ToInt32(values[3]);
						else if (i == 34)
							newClass[i] = Convert.ToInt32(values[2]);
					}
					else if (cbxThieves.Checked == true & (i == 5 | i == 24))
					{
						newClass[i] = Convert.ToInt32(values[2]);
					}
					else if (cbxHerons.Checked == false & (i == 69 | i == 70 | i == 71))
					{
						if (cbxRandRecr.Checked == false)
							newClass[i] = Convert.ToInt32(values[2]);
						else
							newClass[recrInverse[i]] = Convert.ToInt32(values[2]);
					}
					else if (i > 68 & i < 72)
					{ }
					else
					{
						swapclassname.Add(values[0]);
						swapt1.Add(Convert.ToInt32(values[1]));
						swapt2.Add(Convert.ToInt32(values[2]));
						swapt3.Add(Convert.ToInt32(values[3]));
						swapt4.Add(Convert.ToInt32(values[4]));
						swaprace.Add(values[5]);
					}
				}

				int[] heronCharNums = new int[3] { -1, -1, -1 };
				// choose our herons before anything
				if (cbxHerons.Checked == true)
				{
					for (int i = 0; i < 3; i++)
					{
						while (true)
						{
							if (cbxHeronSpread.Checked == true)
							{
								if (i == 0)
								{
									heronCharNums[i] = random.Next(1, 18); // edward to vika
									if (heronCharNums[i] == 17)
										heronCharNums[i] = 69; // rafiel stays the same
								} // part 1
								else if (i == 1)
								{
									heronCharNums[i] = random.Next(19, 35); // marcia to calill
									if (heronCharNums[i] == 34)
										heronCharNums[i] = 70; // leanne stays the same
								} // part 2
								else
								{
									heronCharNums[i] = random.Next(35, 58); // titania to pelleas
									if (heronCharNums[i] == 57)
										heronCharNums[i] = 71; // reyson stays the same
								} // part 3
							}
							else
								heronCharNums[i] = random.Next(72);

							if (heronCharNums[i] == 0 | heronCharNums[i] == 5 | heronCharNums[i] == 34 |
								heronCharNums[i] == 23 | heronCharNums[i] == 22) // micaiah, sothe, brom, neph, ike can't be herons
							{ }
							else if (cbxThieves.Checked == true & heronCharNums[i] == 24) // heather must stay theif
							{ }
							else if (charTier[heronCharNums[i]] == "c" | charTier[heronCharNums[i]] == "d") // c and d tier characters can't be herons
							{ }
							else if (cbxClassRand.Checked == true & comboClassOptions.SelectedIndex == 0 & charRace[heronCharNums[i]] == "B") // beorc can't be heron if race-mixing is off
							{ }
							else
							{
								bool stop = true;
								for (int j = 0; j < 3; j++)
								{
									if (i != j & heronCharNums[i] == heronCharNums[j]) // can't have two units be the same heron
										stop = false;
								}
								if (stop)
									break;
							}
						}
					}

					if (cbxHeronSpread.Checked == true)
					{
						// swap order so rafiel-heron isn't always in part 1, etc
						int randresult = random.Next(3);
						int temp = heronCharNums[0];
						heronCharNums[0] = heronCharNums[randresult];
						heronCharNums[randresult] = temp;
						randresult = random.Next(1, 3);
						temp = heronCharNums[1];
						heronCharNums[1] = heronCharNums[randresult];
						heronCharNums[randresult] = temp;
					}


					for (int i = 0; i < 3; i++)
					{
						newClass[heronCharNums[i]] = 93 + i; // 93 is rafiel heron
					}
				}
				
				
				// loop through all characters
				for (int charNum = 0; charNum < 72; charNum++)
				{
					if (redo == 1)
						break;

					if (cbxLords.Checked == true & (charNum == 0 | charNum == 18 | charNum == 34))
					{ }
					else if (cbxThieves.Checked == true & (charNum == 5 | charNum == 24))
					{ }
					else if (cbxHerons.Checked == false & ((cbxRandRecr.Checked == false & (charNum >= 69)) |
							(cbxRandRecr.Checked == true & (newRecr[charNum] == 69 | newRecr[charNum] == 70 | newRecr[charNum] == 71))))
					{ }
					else if (charNum == heronCharNums[0] | charNum == heronCharNums[1] | charNum == heronCharNums[2])
					{ }
					else
					{
						int randselect = random.Next(swapt1.Count);
						int times = 0;
						if (charTier[charNum] == "a")
						{
							while (true)
							{
								if (swapt1[randselect] == 999) // not a possible class for tier 1
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if (cbxNoLaguz.Checked == true & swaprace[randselect] == "L" & charRace[charNum] == "B" & charNum < 18) // no early game laguz
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if (charNum == 0 & (claName[swapt1[randselect]].Contains("heron") |
														claName[swapt1[randselect]].Contains("priest"))) // micaiah can't be heron or priest
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else
									break;

								if (times > 100)
								{
									redo = 1;
									break;
								}
							}
							newClass[charNum] = swapt1[randselect];

						}
						else if (charTier[charNum] == "b")
						{
							while (true)
							{
								if (swapt2[randselect] == 999) // not possible for tier 2
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if (cbxNoLaguz.Checked == true & swaprace[randselect] == "L" & charRace[charNum] == "B" & charNum < 18) // no early game laguz
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if ((charNum == 5 | charNum == 34 | charNum == 22 | charNum == 23) & claName[swapt2[randselect]].Contains("heron")) // ike,sothe,brom,neph can't be heron
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if ((charNum == 34 | charNum == 45) & cbxHorseParkour.Checked == false & claPlayType[swapt2[randselect]] == "H") // ike,ranulf can't be mounted
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if (charNum == 34 & restrictIke == true & (claPlayType[swapt2[randselect]] == "M" | 
																				claJID[swapt2[randselect]].Contains("K") |
																				claJID[swapt2[randselect]].Contains("R0"))) // ike can't be rogue, raven, or magic
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else
									break;

								// if there are no possible selections left, redo swap
								if (times > 100)
								{
									redo = 1;
									break;
								}
							}

							newClass[charNum] = swapt2[randselect];
						}
						else if (charTier[charNum] == "c")
						{
							while (swapt3[randselect] == 999) // not possible for tier 3
							{
								randselect = random.Next(swapt1.Count);
								times += 1;
								// if there are no possible selections left, redo swap
								if (times > 100)
								{
									redo = 1;
									break;
								}
							}
							newClass[charNum] = swapt3[randselect];
						}
						else
						{
							while (swapt4[randselect] == 999) // not possible for kurth/ena/giffca/gareth/nasir
							{
								randselect = random.Next(swapt1.Count);
								times += 1;
								// if there are no possible selections left, redo swap
								if (times > 100)
								{
									redo = 1;
									break;
								}
							}
							newClass[charNum] = swapt4[randselect];
						}

						swapclassname.RemoveAt(randselect);
						swapt1.RemoveAt(randselect);
						swapt2.RemoveAt(randselect);
						swapt3.RemoveAt(randselect);
						swapt4.RemoveAt(randselect);
						swaprace.RemoveAt(randselect);
					}
				}

				// all worked, let's continue
				if (redo == 0)
					break;
			}
		}

		// selects random class based on selected settings
		private void chooseRandClasses()
		{
			bool[,] stringmatrix =  { { radioInfantry0.Checked, radioInfantry1.Checked, radioInfantry2.Checked, radioInfantry3.Checked, radioInfantry4.Checked, radioInfantry5.Checked},
							{ radioMages0.Checked, radioMages1.Checked, radioMages2.Checked, radioMages3.Checked, radioMages4.Checked, radioMages5.Checked},
							{ radioCav0.Checked, radioCav1.Checked, radioCav2.Checked, radioCav3.Checked, radioCav4.Checked, radioCav5.Checked},
							{ radioArmor0.Checked, radioArmor1.Checked, radioArmor2.Checked, radioArmor3.Checked, radioArmor4.Checked, radioArmor5.Checked},
							{ radioFly0.Checked, radioFly1.Checked, radioFly2.Checked, radioFly3.Checked, radioFly4.Checked, radioFly5.Checked},
							{ radioBeast0.Checked, radioBeast1.Checked, radioBeast2.Checked, radioBeast3.Checked, radioBeast4.Checked, radioBeast5.Checked},
							{ radioBird0.Checked, radioBird1.Checked, radioBird2.Checked, radioBird3.Checked, radioBird4.Checked, radioBird5.Checked},
							{ radioDragon0.Checked, radioDragon1.Checked, radioDragon2.Checked, radioDragon3.Checked, radioDragon4.Checked, radioDragon5.Checked} };

			int[] heronCharNums = new int[3] { -1, -1, -1 };
			// choose our herons before anything
			if (cbxHerons.Checked == true)
			{
				for (int i = 0; i < 3; i++)
				{
					while (true)
					{
						if (cbxHeronSpread.Checked == true)
						{
							if (i == 0)
							{
								heronCharNums[i] = random.Next(1, 18); // edward to vika
								if (heronCharNums[i] == 17)
									heronCharNums[i] = 69; // rafiel stays the same
							} // part 1
							else if (i == 1)
							{
								heronCharNums[i] = random.Next(19, 35); // marcia to calill
								if (heronCharNums[i] == 34)
									heronCharNums[i] = 70; // leanne stays the same
							} // part 2
							else
							{
								heronCharNums[i] = random.Next(35, 58); // titania to pelleas
								if (heronCharNums[i] == 57)
									heronCharNums[i] = 71; // reyson stays the same
							} // part 3
						}
						else
							heronCharNums[i] = random.Next(72);

						if (heronCharNums[i] == 0 | heronCharNums[i] == 5 | heronCharNums[i] == 34 |
							heronCharNums[i] == 23 | heronCharNums[i] == 22) // micaiah, sothe, brom, neph, ike can't be herons
						{ }
						else if (cbxThieves.Checked == true & heronCharNums[i] == 24) // heather must stay theif
						{ }
						else if (charTier[heronCharNums[i]] == "c" | charTier[heronCharNums[i]] == "d") // c and d tier characters can't be herons
						{ }
						else if (cbxClassRand.Checked == true & comboClassOptions.SelectedIndex == 0 & charRace[heronCharNums[i]] == "B") // beorc can't be heron if race-mixing is off
						{ }
						else
						{
							bool stop = true;
							for (int j = 0; j < 3; j++)
							{
								if (i != j & heronCharNums[i] == heronCharNums[j]) // can't have two units be the same heron
									stop = false;
							}
							if (stop)
								break;
						}
					}
				}

				if (cbxHeronSpread.Checked == true)
				{
					// swap order so rafiel-heron isn't always in part 1, etc
					int randresult = random.Next(3);
					int temp = heronCharNums[0];
					heronCharNums[0] = heronCharNums[randresult];
					heronCharNums[randresult] = temp;
					randresult = random.Next(1, 3);
					temp = heronCharNums[1];
					heronCharNums[1] = heronCharNums[randresult];
					heronCharNums[randresult] = temp;
				}
			}

			// loop through all 72 playable characters
			for (int charNum = 0; charNum < 72; charNum++)
			{
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



				// let us begin
				if (cbxLords.Checked == true & (charNum == 0 | charNum == 18 | charNum == 34))
				{
					// micaiah/elincia/ike are unchanged
					if (charNum == 0)
						newClass[charNum] = 54; // lightmage
					else if (charNum == 18)
						newClass[charNum] = 67; // queen
					else
						newClass[charNum] = 3; // hero
				}
				else if (cbxThieves.Checked == true & (charNum == 5 | charNum == 24))
				{
					// sothe/heather are unchanged
					if (charNum == 5 & cbxClassPatch.Checked == true)
						newClass[charNum] = 73; // trickster
					else
						newClass[charNum] = 42; // rogue
				}
				else
				{
					// choose class type from weights
					if (comboClassOptions.SelectedIndex == 0)
					{
						// no race-mixing
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
						// race-mixing
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
					// restrict ike
					if (charNum == 34)
					{
						// always prevent cavalry
						if (classtype == 2 & cbxHorseParkour.Checked == false)
							classtype = 0;
						// restrict magic when BK fight is impossible
						else if (restrictIke & classtype == 1)
							classtype = 0;
					}
					// restrict ranulf from cavalry
					if (charNum == 45)
					{
						if (classtype == 2 & cbxHorseParkour.Checked == false)
							classtype = 5;
					}


					// generate random class from class type
					List<int> possibleclasses = new List<int>();
					if (comboClassOptions.SelectedIndex != 10)
					{
						// get classtype in string form
						string classtypestring = "";
						switch (classtype)
						{
							case 0: // infantry
								classtypestring = "I";
								break;
							case 1: // mages
								classtypestring = "M";
								break;
							case 2: // cavalry
								classtypestring = "H";
								break;
							case 3: // armor
								classtypestring = "A";
								break;
							case 4: // flying beorc
								classtypestring = "F";
								break;
							case 5: // beasts
								classtypestring = "L";
								break;
							case 6: // birds
								classtypestring = "B";
								break;
							case 7: // dragons
								classtypestring = "D";
								break;
						}
						// add classes to list that hit all parameters
						for (int k = 0; k < claName.Length; k++)
						{
							if (charNum == 34 & restrictIke)
							{
								// ike can't be rogue, raven, or heron (magic and horse are already covered earlier in code)
								if (claPlayTier[k].Contains(charTier[charNum]) & claPlayType[k] == classtypestring
													& claJID[k].Contains("K") == false & claJID[k].Contains("R0") == false & claName[k].Contains("heron") == false)
									possibleclasses.Add(k);
							}
							else if (charNum == 0 | charNum == 5 | charNum == 34 | charNum == 23 | charNum == 22)
							{
								// micaiah, sothe, ike, brom, nephenee cannot be herons
								if (claPlayTier[k].Contains(charTier[charNum]) & claPlayType[k] == classtypestring
													& claName[k].Contains("heron") == false)
									possibleclasses.Add(k);
							}
							else if (charNum == 1 & newClass[0] == 57)
							{
								// edward can't be a priest or heron if micaiah is a priest
								if (claPlayTier[k].Contains(charTier[charNum]) & claPlayType[k] == classtypestring
													& claName[k].Contains("priest") == false & claName[k].Contains("heron") == false)
									possibleclasses.Add(k);
							}
							else if (cbxHerons.Checked == false)
							{
								// give herons their normal classes
								if ((cbxRandRecr.Checked == false & charNum == 69) | (cbxRandRecr.Checked == true & newRecr[charNum] == 69))
									possibleclasses.Add(93);
								else if ((cbxRandRecr.Checked == false & charNum == 70) | (cbxRandRecr.Checked == true & newRecr[charNum] == 70))
									possibleclasses.Add(94);
								else if ((cbxRandRecr.Checked == false & charNum == 71) | (cbxRandRecr.Checked == true & newRecr[charNum] == 71))
									possibleclasses.Add(95);
								// restrict everybody else from heron
								else
								{
									if (claPlayTier[k].Contains(charTier[charNum]) & claPlayType[k] == classtypestring
														& claName[k].Contains("heron") == false)
										possibleclasses.Add(k);
								}
							}
							else
							{
								// no restrictions
								if (claPlayTier[k].Contains(charTier[charNum]) & claPlayType[k] == classtypestring)
									possibleclasses.Add(k);
							}
						}
					}
					else
					{
						// oliver mode
						if (charTier[charNum] == "a")
						{
							if (cbxHerons.Checked == false)
							{
								if ((cbxRandRecr.Checked == false & charNum == 69) | (cbxRandRecr.Checked == true & newRecr[charNum] == 69))
									possibleclasses.Add(93);
								else if ((cbxRandRecr.Checked == false & charNum == 70) | (cbxRandRecr.Checked == true & newRecr[charNum] == 70))
									possibleclasses.Add(94);
								else if ((cbxRandRecr.Checked == false & charNum == 71) | (cbxRandRecr.Checked == true & newRecr[charNum] == 71))
									possibleclasses.Add(95);
								else
								{
									possibleclasses.Add(54); // lightmage
									if (charNum != 0)
										possibleclasses.Add(57); // priest
								}
							}
							else
							{
								possibleclasses.Add(54); // lightmage
								if (charNum != 0)
									possibleclasses.Add(57); // priest
							}
						}
						else if (charTier[charNum] == "b")
						{
							if (cbxHerons.Checked == false)
							{
								if ((cbxRandRecr.Checked == false & charNum == 69) | (cbxRandRecr.Checked == true & newRecr[charNum] == 69))
									possibleclasses.Add(93);
								else if ((cbxRandRecr.Checked == false & charNum == 70) | (cbxRandRecr.Checked == true & newRecr[charNum] == 70))
									possibleclasses.Add(94);
								else if ((cbxRandRecr.Checked == false & charNum == 71) | (cbxRandRecr.Checked == true & newRecr[charNum] == 71))
									possibleclasses.Add(95);
								else
								{
									if (charNum == 34 & restrictIke == true)
										possibleclasses.Add(3); // hero
									else
									{
										possibleclasses.Add(55); // lightsage
										possibleclasses.Add(58); // bishop
									}
									if (cbxClassPatch.Checked == true)
										possibleclasses.Add(85); // warmonk
								}
							}
							else
							{
								if (charNum == 34 & restrictIke == true)
									possibleclasses.Add(3); // hero
								else
								{
									possibleclasses.Add(55); // lightsage
									possibleclasses.Add(58); // bishop
								}
								if (cbxClassPatch.Checked == true)
									possibleclasses.Add(85); // warmonk
							}
						}
						else
						{
							possibleclasses.Add(56); // lightpriestess
							possibleclasses.Add(59); // saint
							
							if (cbxClassPatch.Checked == true)
								possibleclasses.Add(86); // crusader
						}
					}

					// select random class from possible classes
					if (possibleclasses.Count == 0)
					{
						textBox1.Text = "Errorcode 11: no possible class for " + charName[charNum] + ". Please report this error on discord with your settings.ini file";
						errorflag = 1;
					}
					else
					{
						newClass[charNum] = possibleclasses.ElementAt(random.Next(possibleclasses.Count));
					}

					// remove possible heron from selection and add next heron
					/*
					if (cbxHerons.Checked == true)
					{
						if (newClass[charNum] == 93)
						{
							claPlayTier[93] = "x";
							claPlayTier[94] = "a;b";
						}
						else if (newClass[charNum] == 94)
						{
							claPlayTier[94] = "x";
							claPlayTier[95] = "a;b";
						}
						else if (newClass[charNum] == 95)
						{
							claPlayTier[95] = "x";
						}
					} */


					if (cbxHerons.Checked == true)
					{
						// if character is destined to be heron, replace chosen new class
						for (int k = 0; k < 3; k++)
						{
							if (heronCharNums[k] == charNum)
								newClass[charNum] = 93 + k; // 93 is rafiel heron class
						}
					}
				}
			
			}

		}

		// changes class due to random classes and/or recruitment
		private void classChanger()
		{
			int unitsToChange;
			if ((cbxClassRand.Checked == true & cbxHerons.Checked == true) | cbxRandRecr.Checked == true)
				unitsToChange = 72;
			else
				unitsToChange = 69;

			// change miccy/ike's classes to be the chosen ones
			if (cbxClassRand.Checked == true & cbxIkeClass.Checked == true)
			{
				string[] strings = comboIkeClass.SelectedItem.ToString().Split(' ');
				newClass[34] = Convert.ToInt32(strings[0]);
			}
			if (cbxClassRand.Checked == true & cbxMicClass.Checked == true)
			{
				string[] strings = comboMicClass.SelectedItem.ToString().Split(' ');
				newClass[0] = Convert.ToInt32(strings[0]);
			}
			if (cbxClassRand.Checked == true & cbxElinciaClass.Checked == true)
			{
				string[] strings = comboElinciaClass.SelectedItem.ToString().Split(' ');
				newClass[18] = Convert.ToInt32(strings[0]);
			}

			// randomize class for each character
			for (charNum = 0; charNum < unitsToChange; charNum++)
			{
				// units given before endgame are stored in a special place
				if (charChapter[charNum] == "407")
					chapterFile = dataLocation + "\\zmap\\emap0407c\\dispos_c.bin";
				else
					chapterFile = dataLocation + "\\zmap\\bmap0" + charChapter[charNum] + "\\dispos_h.bin";

				
				int randClass = newClass[charNum];
				int newLevel = charLevel[charNum];

				newRace[charNum] = claRace[newClass[charNum]];
				byte[] classname = System.Text.Encoding.ASCII.GetBytes(claJID[newClass[charNum]]);

				if (charRace[charNum] != newRace[charNum])
				{
					// change level when switching races
					if (newRace[charNum] == "L")
					{
						if(charTier[charNum] == "b")
							newLevel += 10;
						else if (charTier[charNum] == "c" | charTier[charNum] == "d")
							newLevel += 20;
					}
					else
					{
						if (charTier[charNum] == "b")
							newLevel -= 10;
						else if (charTier[charNum] == "c" | charTier[charNum] == "d")
							newLevel -= 20;
						// coerce to 18 due to game not allowing exp to units of 19? unsure why this occurs
						if (newLevel > 18)
							newLevel = 18;
					}
					if (newLevel < 1)
						newLevel = 1;
					if (newLevel > 40)
						newLevel = 40;
				}


				// time to change inventory
				byte[] weaponone = new byte[8];
				byte[] weapontwo = new byte[8];
				byte[] weaponthree = new byte[8];
				string[] weaponstrings = new string[3];

				if (charTier[charNum] == "a")
					weaponstrings = TierOneWeapons(randClass);
				else if (charTier[charNum] == "b" | (charTier[charNum] == "d" & newRace[charNum] == "L"))
					weaponstrings = TierTwoWeapons(randClass);
				else
					weaponstrings = TierThreeWeapons(randClass);

				weaponone = System.Text.Encoding.ASCII.GetBytes(weaponstrings[0]);
				weapontwo = System.Text.Encoding.ASCII.GetBytes(weaponstrings[1]);
				weaponthree = System.Text.Encoding.ASCII.GetBytes(weaponstrings[2]);

				
				// write to chapter
				//try
				//{
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
				//}
				//catch
				//{
				//	textBox1.Text = "Error 02: Game files not found! Abandoning Randomization...";
				//	errorflag = 1;
				//}


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
					textBox1.Text = "Errorcode 12: Script files not found! Randomization incomplete!";
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
					textBox1.Text = "Errorcode 13: Error in level modifications. Randomization incomplete!";
					errorflag = 1;
				}

			}
		}

		// changes pids in main.dol to reflect new herons and other things
		private void heronChanges()
		{
			// pids
			string[] PIDlist = System.IO.File.ReadAllLines(file +
				"\\assets\\charPIDs.txt");
			

			string mainFile = dataLocation.Remove(dataLocation.Length - 5, 5) + "sys\\main.dol";
			try
			{
				// modify units with heron abilities
				using (var stream = new System.IO.FileStream(mainFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int j = 0; j < 72; j++)
					{
						byte[] PIDbytes = System.Text.Encoding.ASCII.GetBytes(PIDlist[j]);

						if (newClass[j] > 92 & newClass[j] < 96)
						{
							if (gameVersion == 0)
							{
								if (newClass[j] == 93) // first heron is rafiel
									stream.Position = 3559766;
								else if (newClass[j] == 94) // second is leanne
									stream.Position = 3559755;
								else // reyson
									stream.Position = 3559742;
								for (int i = 0; i < 7; i++)
									stream.WriteByte(PIDbytes[i]);
							}
							else if (gameVersion == 1)
							{
								if (newClass[j] == 93) // first heron is rafiel
									stream.Position = 3559606;
								else if (newClass[j] == 94) // second is leanne
									stream.Position = 3559595;
								else // reyson
									stream.Position = 3559582;
								for (int i = 0; i < 7; i++)
									stream.WriteByte(PIDbytes[i]);
							}
							else if (gameVersion == 2)
							{
								if (newClass[j] == 93) // first heron is rafiel
									stream.Position = 3564270;
								else if (newClass[j] == 94) // second is leanne
									stream.Position = 3564259;
								else // reyson
									stream.Position = 3564246;
								for (int i = 0; i < 7; i++)
									stream.WriteByte(PIDbytes[i]);
							}
						}
					}
				}
				// modify locked units from finale
				using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0407a.cmb", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int j = 0; j < 72; j++)
					{
						byte[] PIDbytes = System.Text.Encoding.ASCII.GetBytes(PIDlist[j]);

						if (newClass[j] > 92 & newClass[j] < 96)
						{
							if (newClass[j] == 93) // first heron is rafiel
								stream.Position = 4222;
							else if (newClass[j] == 94) // second is leanne
								stream.Position = 4252;
							else // reyson
								stream.Position = 2456;
							for (int i = 0; i < 7; i++)
								stream.WriteByte(PIDbytes[i]);
						}
					}
				}
				// change text for choosing in finale
				using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_c0407a.m", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int j = 0; j < 72; j++)
					{
						if (newClass[j] > 92 & newClass[j] < 96)
						{
							byte[] PIDbytes = System.Text.Encoding.ASCII.GetBytes(PIDlist[j]);
							string heronname = charName[j];
							if (cbxRandRecr.Checked == true)
								heronname = charName[newRecr[j]];
							if (heronname.Length > 6)
								heronname = heronname.Substring(0, 6);
							byte[] heronbytes = System.Text.Encoding.ASCII.GetBytes(heronname);


							if (newClass[j] == 93) // first heron is rafiel
								stream.Position = 6768;
							else if (newClass[j] == 94) // second is leanne
								stream.Position = 6784;
							else // reyson
								stream.Position = 6776;
							for (int i = 0; i < heronbytes.Length; i++)
								stream.WriteByte(heronbytes[i]);
							stream.WriteByte(0x00);
						}
					}
				}
				// change who gets weapons blessed
				using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0407c.cmb", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int j = 0; j < 72; j++)
					{
						if (newClass[j] > 92 & newClass[j] < 96)
						{
							byte[] PIDbytes = System.Text.Encoding.ASCII.GetBytes(PIDlist[j]);
							if (newClass[j] == 93) // first heron is rafiel
								stream.Position = 579;
							else if (newClass[j] == 94) // second is leanne
								stream.Position = 683;
							else // reyson
								stream.Position = 636;
							for (int i = 0; i < 7; i++)
								stream.WriteByte(PIDbytes[i]);
						}
					}
				}
				// change who causes the conversations in sephiran's chapter
				using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0407d.cmb", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int j = 0; j < 72; j++)
					{
						if (newClass[j] > 92 & newClass[j] < 96)
						{
							byte[] PIDbytes = System.Text.Encoding.ASCII.GetBytes(PIDlist[j]);
							if (newClass[j] == 93) // first heron is rafiel
								stream.Position = 1062;
							else if (newClass[j] == 94) // second is leanne
								stream.Position = 1108;
							else // reyson
								stream.Position = 1138;
							for (int i = 0; i < 7; i++)
								stream.WriteByte(PIDbytes[i]);
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Errorcode 14: Error in heron modifications. Randomization incomplete!";
				errorflag = 1;
			}
		}

		// swaps promotion lines around
		private void promotionSwapper()
		{
			List<int> tier1 = new List<int>();
			List<int> tier2 = new List<int>();
			List<int> tier3 = new List<int>();

			if (cbxEasyPromotion.Checked == true) // keeps physical units to promote into physical, magic into magic
			{
				List<int> magtier1 = new List<int>();
				List<int> magtier2 = new List<int>();
				List<int> magtier3 = new List<int>();
				// build tier lists
				for (int i = 0; i < claName.Length; i++)
				{
					if (claPlayTier[i].Contains('a') & claRace[i] == "B" & claPlayType[i] != "M")
						tier1.Add(i);
					else if (claPlayTier[i].Contains('b') & claRace[i] == "B" & claPlayType[i] != "M")
						tier2.Add(i);
					else if (claPlayTier[i].Contains('c') & claRace[i] == "B" & claPlayType[i] != "M")
						tier3.Add(i);

					if (claPlayTier[i].Contains('a') & claRace[i] == "B" & claPlayType[i] == "M")
						magtier1.Add(i);
					else if (claPlayTier[i].Contains('b') & claRace[i] == "B" & claPlayType[i] == "M")
						magtier2.Add(i);
					else if (claPlayTier[i].Contains('c') & claRace[i] == "B" & claPlayType[i] == "M")
						magtier3.Add(i);
				}
				// shuffle tier 2 and 3 physical
				int choice, temp;
				for (int i = 0; i < tier2.Count; i++)
				{
					choice = random.Next(tier2.Count);
					temp = tier2.ElementAt(choice);
					tier2.Insert(choice, tier2.ElementAt(i));
					tier2.RemoveAt(choice + 1);
					tier2.Insert(i, temp);
					tier2.RemoveAt(i + 1);
				}
				for (int i = 0; i < tier3.Count; i++)
				{
					choice = random.Next(tier3.Count);
					temp = tier3.ElementAt(choice);
					tier3.Insert(choice, tier3.ElementAt(i));
					tier3.RemoveAt(choice + 1);
					tier3.Insert(i, temp);
					tier3.RemoveAt(i + 1);
				}
				// shuffle tier 2 and 3 magical
				for (int i = 0; i < magtier2.Count; i++)
				{
					choice = random.Next(magtier2.Count);
					temp = magtier2.ElementAt(choice);
					magtier2.Insert(choice, magtier2.ElementAt(i));
					magtier2.RemoveAt(choice + 1);
					magtier2.Insert(i, temp);
					magtier2.RemoveAt(i + 1);
				}
				for (int i = 0; i < magtier3.Count; i++)
				{
					choice = random.Next(magtier3.Count);
					temp = magtier3.ElementAt(choice);
					magtier3.Insert(choice, magtier3.ElementAt(i));
					magtier3.RemoveAt(choice + 1);
					magtier3.Insert(i, temp);
					magtier3.RemoveAt(i + 1);
				}
				// pad tier 1 and 2 until they are as long as tier 3
				while (tier1.Count < tier3.Count)
					tier1.Add(999);
				while (tier2.Count < tier3.Count)
					tier2.Add(999);
				while (magtier1.Count < magtier3.Count)
					magtier1.Add(999);
				while (magtier2.Count < magtier3.Count)
					magtier2.Add(999);
				// add magic lists to normal lists
				List<int>[] magtiers = { magtier1, magtier2, magtier3 };
				List<int>[] phystiers = { tier1, tier2, tier3 };
				for (int i = 0; i < magtiers.Length; i++)
				{
					for (int j = 0; j < magtiers[i].Count; j++)
						phystiers[i].Add(magtiers[i].ElementAt(j));
				}
			}
			else
			{
				// build tier lists
				for (int i = 0; i < claName.Length; i++)
				{
					if (claPlayTier[i].Contains('a') & claRace[i] == "B")
						tier1.Add(i);
					else if (claPlayTier[i].Contains('b') & claRace[i] == "B")
						tier2.Add(i);
					else if (claPlayTier[i].Contains('c') & claRace[i] == "B")
						tier3.Add(i);
				}
				// shuffle tier 2 and 3
				int choice, temp;
				for (int i = 0; i < tier2.Count; i++)
				{
					choice = random.Next(tier2.Count);
					temp = tier2.ElementAt(choice);
					tier2.Insert(choice, tier2.ElementAt(i));
					tier2.RemoveAt(choice + 1);
					tier2.Insert(i, temp);
					tier2.RemoveAt(i + 1);
				}
				for (int i = 0; i < tier3.Count; i++)
				{
					choice = random.Next(tier3.Count);
					temp = tier3.ElementAt(choice);
					tier3.Insert(choice, tier3.ElementAt(i));
					tier3.RemoveAt(choice + 1);
					tier3.Insert(i, temp);
					tier3.RemoveAt(i + 1);
				}
				// pad tier 1 and 2 until they are as long as tier 3
				while (tier1.Count < tier3.Count)
					tier1.Add(999);
				while (tier2.Count < tier3.Count)
					tier2.Add(999);
			}

			// save for outputlog
			randPromoOutput = "<table> <tr><th>Tier 1</th> <th>Tier 2</th> <th>Tier 3</th></tr>";
			for (int i = 0; i < tier1.Count; i++)
			{
				int t1, t2, t3;
				string t1string, t2string, t3string;
				t1 = tier1.ElementAt(i);
				t2 = tier2.ElementAt(i);
				t3 = tier3.ElementAt(i);
				if (t1 == 999)
					t1string = "n/a";
				else
					t1string = claName[t1];
				if (t2 == 999)
					t2string = "n/a";
				else
					t2string = claName[t2];
				t3string = claName[t3];
				randPromoOutput += "<tr> <td>" + t1string + "</td> <td>" + t2string +
									"</td> <td>" + t3string + "</td> </tr>";
			}
			randPromoOutput += "</table>";


			// store promotion paths
			for (int i = 0; i < claPromoPath.Length; i++)
			{
				int foundindex;
				if (claPlayTier[i].Contains('a') & claRace[i] == "B")
				{
					foundindex = tier1.FindIndex(x => x == i);
					claPromoPath[i] = tier1.ElementAt(foundindex).ToString() + ";" +
										tier2.ElementAt(foundindex).ToString() + ";" +
										tier3.ElementAt(foundindex).ToString();
				}
				else if (claPlayTier[i].Contains('b') & claRace[i] == "B")
				{
					foundindex = tier2.FindIndex(x => x == i);
					claPromoPath[i] = tier1.ElementAt(foundindex).ToString() + ";" +
										tier2.ElementAt(foundindex).ToString() + ";" +
										tier3.ElementAt(foundindex).ToString();
				}
				else if (claPlayTier[i].Contains('c') & claRace[i] == "B")
				{
					foundindex = tier3.FindIndex(x => x == i);
					claPromoPath[i] = tier1.ElementAt(foundindex).ToString() + ";" +
										tier2.ElementAt(foundindex).ToString() + ";" +
										tier3.ElementAt(foundindex).ToString();
				}
			}

			// write paths to fe10data
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < tier1.Count; i++)
					{
						if (tier1.ElementAt(i) != 999)
						{
							// read jid from promoted class
							stream.Position = claDataLoc[tier2.ElementAt(i)];
							byte[] promojid = new byte[4];
							stream.Read(promojid, 0, 4);
							// go to promotion pointer of this class
							stream.Position = claDataLoc[tier1.ElementAt(i)] + 20;
							stream.Write(promojid, 0, 4);
						}
						if (tier2.ElementAt(i) != 999)
						{
							// read jid from promoted class
							stream.Position = claDataLoc[tier3.ElementAt(i)];
							byte[] promojid = new byte[4];
							stream.Read(promojid, 0, 4);
							// go to promotion pointer of this class
							stream.Position = claDataLoc[tier2.ElementAt(i)] + 20;
							stream.Write(promojid, 0, 4);
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error in randPromo: Cannot find \\files\\FE10Data.cms.decompressed!";
				errorflag = 1;
			}

			addPromoBonuses();
		}

		// adds promotion bonuses to classes normally not promoted into
		private void addPromoBonuses()
		{
			int[,] inputMatrix = new int[8, 9];

			string line;
			string[] values;

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\newPromotionBonuses.csv");

			// skip header line
			line = dataReader.ReadLine();

			for (int i = 0; i < 8; i++)
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
			dataReader.Close();

			try
			{
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < 8; i++)
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
				textBox1.Text = "Error in promo bonuses: Cannot find file \\FE10Data.cms.decompressed!";
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
					int animationnumber;
					if ((cbxClassRand.Checked == true & cbxHerons.Checked == true) | cbxRandRecr.Checked == true)
						animationnumber = 72;
					else
						animationnumber = 69;

					for (int charNum = 0; charNum < animationnumber; charNum++)
					{
						int charclass;
						if (cbxClassRand.Checked == false & cbxRandRecr.Checked == false)
							charclass = charVanillaClass[charNum];
						else
							charclass = newClass[charNum];

						if (cbxRandPromotion.Checked == true & charclass < 87)
						{
							string[] animationpointers1,animationpointers2,animationpointers3;
							string[] promoline = claPromoPath[charclass].Split(';');

							// separate animation pointers
							animationpointers3 = claAnim[Convert.ToInt32(promoline[2])].Split(';');
							if(promoline[1] != "999")
								animationpointers2 = claAnim[Convert.ToInt32(promoline[1])].Split(';');
							else
								animationpointers2 = animationpointers3;
							if (promoline[0] != "999")
								animationpointers1 = claAnim[Convert.ToInt32(promoline[0])].Split(';');
							else
								animationpointers1 = animationpointers2;

							// change position to location of character animation in dataFile
							stream.Position = (long)Convert.ToDouble(charAnimation[charNum]);
							string[][] allanimpointers = { animationpointers1, animationpointers2, animationpointers3, animationpointers3 };
							for (int j = 0; j < allanimpointers.Length; j++)
							{
								int minanimpointer = Convert.ToInt32(allanimpointers[j][0]);
								int maxanimpointer = Convert.ToInt32(allanimpointers[j][1]) + 1;

								// line offset from the myrmidon in animationPointers.txt
								int animationOffset = random.Next(minanimpointer, maxanimpointer);
								// only 3 bytes of animation
								string fullanimpointer = pointerList[animationOffset].Remove(0, (j*12)).Remove(11);

								// do not write over original animation if not class rand
								if ((cbxClassRand.Checked == false & cbxRandRecr.Checked == false) & (
											(charTier[charNum] == "a" & j == 0) | (charTier[charNum] == "b" & j == 1) | (charTier[charNum] == "c" & j == 2)))
									stream.Position += 4;
								else
								{
									// write the 4 bytes for animation
									stream.WriteByte(0x00);
									for (int k = 0; k < 3; k++)
									{
										string outByte = (fullanimpointer.Remove(0, (k * 4)) + " ").Remove(3); // had to pad with space at the end so Remove(3) wouldn't through an error on the last three digits
										stream.WriteByte(Convert.ToByte(outByte));
									}
								}
							}
						}
						else
						{
							// don't do this if classes haven't been changed
							string[] animationpointers;
							if (cbxClassRand.Checked == false & cbxRandRecr.Checked == false)
							{ }
							else
							{
								animationpointers = claAnim[newClass[charNum]].Split(';');

								int minanimpointer = Convert.ToInt32(animationpointers[0]);
								int maxanimpointer = Convert.ToInt32(animationpointers[1]) + 1;

								// line offset from the myrmidon in animationPointers.txt
								int animationOffset = random.Next(minanimpointer, maxanimpointer);


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
				}
			}
			catch
			{
				textBox1.Text = "Errorcode 15: Error in the animation station. Randomization incomplete!";
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
					int numberofunits;
					if ((cbxClassRand.Checked == true & cbxHerons.Checked == true) | cbxRandRecr.Checked == true)
						numberofunits = 72;
					else
						numberofunits = 69;

					// loop through characters
					for (int charNum = 0; charNum < numberofunits; charNum++)
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
									case 87:
									case 99:
										// lion
										outByte1 = 5;
										outByte2 = 10;
										outByte3 = 3;
										outByte4 = 2;
										break;
									case 88:
										// tiger
										outByte1 = 8;
										outByte2 = 15;
										outByte3 = 4;
										outByte4 = 3;
										break;
									case 89:
										// cat
										outByte1 = 10;
										outByte2 = 15;
										outByte3 = 5;
										outByte4 = 4;
										break;
									case 90:
									case 100:
										// wolf
										outByte1 = 6;
										outByte2 = 10;
										outByte3 = 4;
										outByte4 = 3;
										break;
									case 91:
									case 101:
										// hawk
										outByte1 = 8;
										outByte2 = 15;
										outByte3 = 4;
										outByte4 = 3;
										break;
									case 92:
									case 102:
										// raven
										outByte1 = 6;
										outByte2 = 10;
										outByte3 = 4;
										outByte4 = 3;
										break;
									case 93:
									case 94:
									case 95:
										// heron
										outByte1 = 4;
										outByte2 = 10;
										outByte3 = 5;
										outByte4 = 6;
										break;
									case 96:
										// red dragon
										outByte1 = 5;
										outByte2 = 6;
										outByte3 = 2;
										outByte4 = 1;
										break;
									case 97:
										// white dragon
										outByte1 = 4;
										outByte2 = 5;
										outByte3 = 2;
										outByte4 = 1;
										break;
									case 98:
									case 103:
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
				textBox1.Text = "Errorcode 16: Error in laguz gauge changes. Randomization incomplete!";
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

					int numberofunits;
					if ((cbxClassRand.Checked == true & cbxHerons.Checked == true) | cbxRandRecr.Checked == true)
						numberofunits = 72;
					else
						numberofunits = 69;

					// loop through characters
					for (int charNum = 0; charNum < numberofunits; charNum++)
					{
						// do the thing
						stream.Position = charPID[charNum] + 4;
						for (int i = 0; i < 12; i++)
							stream.WriteByte(importantInfo[i]);
					}

					// change some important stats
					stream.Position = 51910;
					stream.WriteByte(5);

					stream.Position = 48918;
					stream.WriteByte(4);

					stream.Position = 52182;
					stream.WriteByte(11);

					stream.Position = 49734;
					stream.WriteByte(12);
				}
			}
			catch
			{
				textBox1.Text = "Errorcode 17: Oliver mode caused errors. Randomization incomplete!";
				errorflag = 1;
			}
		}

		// adds custom hybrid classes to the game
		private void LM73classPatch()
		{
			int[] fe10dataloc = new int[24];
			string[] newclassname = new string[fe10dataloc.Length];
			string[] abbrname = new string[fe10dataloc.Length];
			string[] movetozu = new string[fe10dataloc.Length];
			string[] movetozu2 = new string[fe10dataloc.Length];
			string[] movetofile = new string[fe10dataloc.Length];
			string[] movefromzu = new string[fe10dataloc.Length];
			string[] movefromfile = new string[fe10dataloc.Length];
			int[] namelocation = new int[fe10dataloc.Length];
			int[] namepointerloc = new int[fe10dataloc.Length];
			int[] mjidpointer = new int[fe10dataloc.Length];
			string[] newdescription = new string[fe10dataloc.Length];
			int[] descriplocation = new int[fe10dataloc.Length];
			int[] mh_jpointer = new int[fe10dataloc.Length];
			int[] minrankloc = new int[fe10dataloc.Length];
			int[] maxrankloc = new int[fe10dataloc.Length];
			string[] minrank = new string[fe10dataloc.Length];
			string[] maxrank = new string[fe10dataloc.Length];
			string[] movetoymu = new string[fe10dataloc.Length];
			string[] movetoymu2 = new string[fe10dataloc.Length];
			string[] ymufiles2move = new string[fe10dataloc.Length];
			string[] movefromymu = new string[fe10dataloc.Length];

			string line;
			string[] values;
			// load in new class data
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\LM73ClassPatch.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all new classes
			for (int i = 0; i < fe10dataloc.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				// location in fe10data
				fe10dataloc[i] = Convert.ToInt32(values[1]);
				// new class name
				newclassname[i] = values[2];
				// new description
				newdescription[i] = values[3];
				// abbreviated class name
				abbrname[i] = values[4];
				// folder where animations are stored
				movetozu[i] = values[5];
				// folder where second characters animations are stored
				movetozu2[i] = values[6];
				// files to create
				movetofile[i] = values[7];
				// folder where new animations are stored
				movefromzu[i] = values[8];
				// files to move
				movefromfile[i] = values[9];
				if (gameVersion == 2) // PAL iso
				{
					// location in e_common where name is stored in text
					namelocation[i] = Convert.ToInt32(values[27]);
					// pointer for text name
					namepointerloc[i] = Convert.ToInt32(values[28]);
					// location in e_common where description is stored in text
					descriplocation[i] = Convert.ToInt32(values[29]);
				}
				else
				{
					// location in e_common where name is stored in text
					namelocation[i] = Convert.ToInt32(values[11]);
					// pointer for text name
					namepointerloc[i] = Convert.ToInt32(values[12]);
					// location in e_common where description is stored in text
					descriplocation[i] = Convert.ToInt32(values[15]);
				}
				// mjid pointer
				mjidpointer[i] = Convert.ToInt32(values[13]);
				// mh_j pointer
				mh_jpointer[i] = Convert.ToInt32(values[16]);
				if (cbxRandBosses.Checked | (cbxRandRecr.Checked == true & cbxEnemyRecruit.Checked == true) | cbxRandEnemy.Checked == true | cbxTier3Enemies.Checked == true)
				{
					// location of minimum weapon ranks
					minrankloc[i] = Convert.ToInt32(values[21]);
					// location of maximum weapon ranks
					maxrankloc[i] = Convert.ToInt32(values[22]);
				}
				else
				{
					// location of minimum weapon ranks
					minrankloc[i] = Convert.ToInt32(values[17]);
					// location of maximum weapon ranks
					maxrankloc[i] = Convert.ToInt32(values[18]);
				}
				// min weapon rank string
				minrank[i] = values[19];
				// max weapon rank string
				maxrank[i] = values[20];
				movetoymu[i] = values[23];
				movetoymu2[i] = values[24];
				ymufiles2move[i] = values[25];
				movefromymu[i] = values[26];
			}
			dataReader.Close();


			// move animation files accordingly
			string sourcePath, targetPath, sourcefile, targetfile;
			for (int i = 0; i < fe10dataloc.Length; i++)
			{
				if (movefromzu[i] != "")
				{
					for (int twice = 0; twice < 2; twice++)
					{
						sourcePath = dataLocation + "\\zu\\" + movefromzu[i] + "\\";
						if (twice == 0)
							targetPath = dataLocation + "\\zu\\" + movetozu[i] + "\\";
						else
						{
							if (movetozu2[i] != "")
								targetPath = dataLocation + "\\zu\\" + movetozu2[i] + "\\";
							else
								break;
						}
						string[] targetfiles = movetofile[i].Split(';');
						string[] sourcefiles = movefromfile[i].Split(';');
						for (int j = 0; j < targetfiles.Length; j++)
						{
							sourcefile = sourcePath + sourcefiles[j];
							targetfile = targetPath + targetfiles[j];
							System.IO.File.Copy(sourcefile, targetfile, true);
							string sourcetype = sourcefiles[j].Remove(2);
							string targettype = targetfiles[j].Remove(2);
							// rename files inside pak file from sourcetype to target type
							if (sourcetype != targettype)
							{
								using (var stream = new System.IO.FileStream(targetfile, System.IO.FileMode.Open,
									System.IO.FileAccess.ReadWrite))
								{
									stream.Position = 0;
									while (stream.Position < 500)
									{
										byte[] readbytes = new byte[2];
										stream.Read(readbytes, 0, 2);
										string bytestring = System.Text.Encoding.ASCII.GetString(readbytes);
										if (bytestring == sourcetype)
										{
											stream.Position -= 2;
											readbytes = System.Text.Encoding.ASCII.GetBytes(targettype);
											stream.Write(readbytes, 0, 2);
										}
										else
											stream.Position -= 1;
									}
								}
							}
						}
					}
				}
				if (movefromymu[i] != "")
				{
					for (int twice = 0; twice < 2; twice++)
					{
						sourcePath = dataLocation + "\\ymu\\" + movefromymu[i] + "\\";
						if (twice == 0)
							targetPath = dataLocation + "\\ymu\\" + movetoymu[i] + "\\";
						else
						{
							if (movetoymu2[i] != "")
								targetPath = dataLocation + "\\ymu\\" + movetoymu2[i] + "\\";
							else
								break;
						}
						string[] filenames = ymufiles2move[i].Split(';');
						for (int j = 0; j < filenames.Length; j++)
						{
							sourcefile = sourcePath + filenames[j];
							targetfile = targetPath + filenames[j];
							System.IO.File.Copy(sourcefile, targetfile, true);
						}
					}
				}
			}

			// move files for soren's class specifically
			sourcePath = dataLocation + "\\zu\\pknSt\\sw.pak";
			targetPath = dataLocation + "\\zu\\magwSs\\sw.pak";
			System.IO.File.Copy(sourcePath, targetPath, true);

			sourcePath = dataLocation + "\\zu\\pknSt\\ms.pak";
			targetPath = dataLocation + "\\zu\\magwSs\\ms.pak";
			System.IO.File.Copy(sourcePath, targetPath, true);

			sourcePath = dataLocation + "\\zu\\pknSt\\ms.pak";
			targetPath = dataLocation + "\\zu\\magwSs\\mg.pak";
			System.IO.File.Copy(sourcePath, targetPath, true);
			// replace ms with mg
			using (var stream = new System.IO.FileStream(targetPath, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
			{
				stream.Position = 0;
				while (stream.Position < 500)
				{
					byte[] readbytes = new byte[2];
					stream.Read(readbytes, 0, 2);
					string bytestring = System.Text.Encoding.ASCII.GetString(readbytes);
					if (bytestring == "ms")
					{
						stream.Position -= 2;
						readbytes = System.Text.Encoding.ASCII.GetBytes("mg");
						stream.Write(readbytes, 0, 2);
					}
					else
						stream.Position -= 1;
				}
			}

			sourcePath = dataLocation + "\\zu\\dknSh\\pack.cmp";//dknSh.tpl";
			targetPath = dataLocation + "\\zu\\magwSs\\pack.cmp";//magwSs.tpl";
			System.IO.File.Copy(sourcePath, targetPath, true);
			// move sword files for crusader class
			sourcePath = dataLocation + "\\zu\\swoSz\\sw.pak";
			targetPath = dataLocation + "\\zu\\priSk\\sw.pak";
			System.IO.File.Copy(sourcePath, targetPath, true);
			targetPath = dataLocation + "\\zu\\priSo\\sw.pak";
			System.IO.File.Copy(sourcePath, targetPath, true);

			sourcePath = dataLocation + "\\zu\\swoSz\\ms.pak";
			targetPath = dataLocation + "\\zu\\priSk\\ms.pak";
			System.IO.File.Copy(sourcePath, targetPath, true);
			targetPath = dataLocation + "\\zu\\priSo\\ms.pak";
			System.IO.File.Copy(sourcePath, targetPath, true);
			
			
			// change information in FE10Data
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
			{
				int endpoint = 0;
				for (int i = 0; i < fe10dataloc.Length; i++)
				{
					
					// go to MJID
					if (mjidpointer[i] != 0)
					{
						stream.Position = fe10dataloc[i] + 4;
						byte[] mjidbytes = int2bytes(mjidpointer[i]);
						stream.Write(mjidbytes, 0, 4);
					}
					if (mh_jpointer[i] != 0)
					{
						stream.Position = fe10dataloc[i] + 12;
						byte[] mh_jbytes = int2bytes(mh_jpointer[i]);
						stream.Write(mh_jbytes, 0, 4);
					}
					
					// write weapon rank pointers
					stream.Position = minrankloc[i];
					byte[] minrankbytes = System.Text.Encoding.ASCII.GetBytes(minrank[i]);
					stream.Write(minrankbytes, 0, 12);
					stream.WriteByte(0x00);
					stream.Position = maxrankloc[i];
					byte[] maxrankbytes = System.Text.Encoding.ASCII.GetBytes(maxrank[i]);
					stream.Write(maxrankbytes, 0, 12);
					stream.WriteByte(0x00);
					if (i == fe10dataloc.Length - 1)
						endpoint = (int)stream.Position;

					// go to weapon ranks
					stream.Position = fe10dataloc[i] + 36;
					byte[] minrankpointer, maxrankpointer;
					if (cbxFormshift.Checked == false)
					{
						minrankpointer = int2bytes(minrankloc[i] - 32);
						maxrankpointer = int2bytes(maxrankloc[i] - 32);
					}
					else // formshift adds 12 bytes of pointers, so weapon ranks at the end of the file will be 12 spaces later
					{
						minrankpointer = int2bytes(minrankloc[i] - 32 + 12);
						maxrankpointer = int2bytes(maxrankloc[i] - 32 + 12);
					}

					stream.Write(minrankpointer, 0, 4);
					stream.Write(maxrankpointer, 0, 4);

					// change bastion's starting weapon ranks to have SS in thunder
					stream.Position = 170885;
					stream.WriteByte(0x2A); // SS rank in thunder
					stream.WriteByte(0x41); // A rank in wind
					
				}

				// write file size
				stream.Position = 0;
				byte[] filesizebytes = int2bytes(endpoint);
				stream.Write(filesizebytes, 0, 4);

				// write some stats for soren's new class
				stream.Position = fe10dataloc[fe10dataloc.Length - 1] + 44;
				// CON
				stream.WriteByte(0x0D);
				stream.Position += 2;
				// mount type
				stream.WriteByte(0x04);
				// mount weight
				stream.WriteByte(0x1F);
				stream.Position += 3;
				// movement type
				stream.WriteByte(0x0A);
				// MOV
				stream.WriteByte(0x09);


				// skill changes for custom classes
				byte[] mageskill = { 0x00, 0x03, 0x56, 0xD2 };
				byte[] dragonsfx = { 0x00, 0x03, 0x51, 0x84 };
				byte[] lethality = { 0x00, 0x03, 0x52, 0xC7 };
				byte[] colossus = { 0x00, 0x03, 0x53, 0x6E };
			
				// give lightning thief lethality over bane
				stream.Position = 47788;
				stream.Write(lethality, 0, 4);
				// give crusader colossus over corona
				stream.Position = 52676;
				stream.Write(colossus, 0, 4);
				// give mage sfxto custom classes
				int[] sfxloc = { 53424, 54104, 55872, 54376, 55600, 56960, 57232, 57640 };
				byte[] magesfx = { 0x00, 0x03, 0x51, 0xBB };
				for (int i = 0; i < sfxloc.Length; i++)
				{
					stream.Position = sfxloc[i] + 84;
					stream.Write(mageskill, 0, 4);
					stream.Position = sfxloc[i] + 100;
					stream.Write(magesfx, 0, 4);
				}

				// copy dragonlord skills over to soren's new class
				byte[] dragonlordskill = new byte[12];
				stream.Position = 57568;
				stream.Read(dragonlordskill, 0, 12);
				stream.Position = 50904;
				stream.Write(dragonlordskill, 0, 12);
				stream.Write(mageskill, 0, 4);
				stream.Position = 50940;
				stream.Write(dragonsfx, 0, 4);

			}
			

			// change text for mjids and mh_js
			using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_common.m", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
			{
				for (int i = 0; i < fe10dataloc.Length; i++)
				{
					if (namelocation[i] != 0)
					{
						stream.Position = namelocation[i];
						byte[] namestring = System.Text.Encoding.ASCII.GetBytes(newclassname[i]);
						stream.Write(namestring, 0, namestring.Length);
						stream.WriteByte(0x00);
						stream.Position = namepointerloc[i];
						byte[] newnamepointer = int2bytes(namelocation[i] - 32);
						stream.Write(newnamepointer, 0, 4);
					}
					if (descriplocation[i] != 0)
					{
						string[] description = newdescription[i].Split(';');
						stream.Position = descriplocation[i];
						for (int j = 0; j < description.Length; j++)
						{
							byte[] descrstring = System.Text.Encoding.ASCII.GetBytes(description[j]);
							stream.Write(descrstring, 0, descrstring.Length);
							if(j!=description.Length-1)
								stream.WriteByte(0x0A); // new line
							else
								stream.WriteByte(0x00);
						}

					}
				}
			}

			LaguzMJID();
			ClassPatch_statchanges();
			if (cbxClassRand.Checked == false & cbxRandRecr.Checked == false)
				ClassPatch_changeVanillaClasses();
		}

		// fix laguz mjids and mh_js that are used for custom classes
		private void LaguzMJID()
		{
			int[] loc2paste = new int[41];
			int[] loc2copy = new int[loc2paste.Length];

			string line;
			string[] values;
			// load in new class data
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ClassPatch_MJID.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all new classes
			for (int i = 0; i < loc2paste.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');

				loc2paste[i] = Convert.ToInt32(values[1]);
				loc2copy[i] = Convert.ToInt32(values[2]);
			}
			dataReader.Close();

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
			{
				for (int i = 0; i < loc2paste.Length; i++)
				{
					int offset;
					if (i < 21)
						offset = 4; // mjid
					else
						offset = 12; // mh_j
					stream.Position = loc2copy[i] + offset;
					byte[] readbytes = new byte[4];
					stream.Read(readbytes, 0, 4);
					stream.Position = loc2paste[i] + offset;
					stream.Write(readbytes, 0, 4);
				}
			}
		}

		// stat changes for custom classes
		private void ClassPatch_statchanges()
		{
			int[,] inputMatrix = new int[78, 9];

			string line;
			string[] values;

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ClassPatch_NewStats.csv");

			// skip header line
			line = dataReader.ReadLine();

			for (int i = 0; i < 78; i++)
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
			dataReader.Close();

			try
			{
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < 78; i++)
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
				textBox1.Text = "Error in custom class stat changes: Cannot find file \\FE10Data.cms.decompressed!";
				errorflag = 1;
			}
		}

		// when there are no class changes, modify vanilla classes of certain characters
		private void ClassPatch_changeVanillaClasses()
		{
			int[] charID = new int[16];
			string[] newJID = new string[charID.Length];
			string[] newIID1 = new string[charID.Length];
			string[] newIID2 = new string[charID.Length];
			string[] newIID3 = new string[charID.Length];

			string line;
			string[] values;
			// load in new class data
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ClassPatch_VanillaClassChanges.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all new classes
			for (int i = 0; i < charID.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');

				charID[i] = Convert.ToInt32(values[1]);
				newJID[i] = values[2];
				newIID1[i] = values[3];
				newIID2[i] = values[4];
				newIID3[i] = values[5];
			}
			dataReader.Close();

			for (int i = 0; i < charID.Length; i++)
			{
				chapterFile = dataLocation + "\\zmap\\bmap0" + charChapter[charID[i]] + "\\dispos_h.bin";

				// open chapter file
				using (var stream = new System.IO.FileStream(chapterFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// write new jid
					stream.Position = charLocation[charID[i]];
					byte[] jidbytes = System.Text.Encoding.ASCII.GetBytes(newJID[i]);
					stream.Write(jidbytes, 0, 7);
					stream.Position += 13;
					// write new weapons
					if (newIID1[i] != "")
					{
						byte[] iidbytes = System.Text.Encoding.ASCII.GetBytes(newIID1[i]);
						stream.Write(iidbytes, 0, 8);
						stream.Position += 12;
					}
					if (newIID2[i] != "")
					{
						byte[] iidbytes = System.Text.Encoding.ASCII.GetBytes(newIID2[i]);
						stream.Write(iidbytes, 0, 8);
						stream.Position += 12;
					}
					if (newIID3[i] != "")
					{
						byte[] iidbytes = System.Text.Encoding.ASCII.GetBytes(newIID3[i]);
						stream.Write(iidbytes, 0, 8);
					}
				}
			}
		}


		// WEAPON FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// selects random weapons for new classes
		private string[] TierOneWeapons(int randClass)
		{
			string[] weapontypes = claPlayWeap[randClass].Split(';');
			string[] weapons = new string[weapontypes.Length];
			for (int i = 0; i < weapontypes.Length; i++)
			{
				switch (weapontypes[i])
				{
					case "S":
						{
							if (i == 0 | i == 2)
							{
								if (random.Next(10) < 8 | cbxWeapPatch.Checked == true) // 80% chance of iron
									weapons[i] = "IID_IRNS";
								else                     // 20% chance of venin
									weapons[i] = "IID_POIS";
							}
							else
							{
								if (random.Next(10) < 7) // 70% chance of wind
									weapons[i] = "IID_WNDS";
								else                     // 30% chance of iron
									weapons[i] = "IID_IRNS";
							}
							break;
						}
					case "L":
						{
							if (i == 0 | i == 2)
							{
								if (random.Next(10) < 8 | cbxWeapPatch.Checked == true) // 80% chance of iron
									weapons[i] = "IID_IRNL";
								else                     // 20% chance of venin
									weapons[i] = "IID_POIL";
							}
							else
							{
								if (random.Next(10) < 7) // 70% chance of javelin
									weapons[i] = "IID_JAVE";
								else                     // 30% chance of iron
									weapons[i] = "IID_IRNL";
							}
							break;
						}
					case "A":
						{
							if (i == 0 | i == 2)
							{
								if (random.Next(10) < 8 | cbxWeapPatch.Checked == true) // 80% chance of iron
									weapons[i] = "IID_IRNA";
								else                     // 20% chance of venin
									weapons[i] = "IID_POIA";
							}
							else
							{
								if (random.Next(10) < 7) // 70% chance of handaxe
									weapons[i] = "IID_HAND";
								else                     // 30% chance of iron
									weapons[i] = "IID_IRNA";
							}
							break;
						}
					case "K":
						{
							if (i == 0 | i == 2)
							{
								weapons[i] = "IID_IRND"; // dagger
							}
							else
							{
								if (random.Next(10) < 7) // 70% chance of knife
									weapons[i] = "IID_IRNK";
								else                     // 30% chance of dagger
									weapons[i] = "IID_IRND";
							}
							break;
						}
					case "B":
						{
							if (i == 0 | i == 2)
							{
								if (random.Next(10) < 8 | cbxWeapPatch.Checked == true) // 80% chance of iron
									weapons[i] = "IID_IRNB";
								else                     // 20% chance of venin
									weapons[i] = "IID_POIB";
							}
							else
							{
								weapons[i] = "IID_IRNB";
							}
							break;
						}
					case "F":
						{
							weapons[i] = "IID_FIRE";
							break;
						}
					case "T":
						{
							weapons[i] = "IID_THUN";
							break;
						}
					case "W":
						{
							weapons[i] = "IID_WIND";
							break;
						}
					case "M":
						{
							weapons[i] = "IID_LIGH";
							break;
						}
					case "D":
						{
							weapons[i] = "IID_WORM";
							break;
						}
					case "H":
						{
							if (i == 0 | i == 2)
							{
								weapons[i] = "IID_LIVE";
							}
							else
							{
								if (random.Next(10) < 8)
									weapons[i] = "IID_LIVE";
								else                     
									weapons[i] = "IID_TORC";
							}
							break;
						}
					case "X":
						{
							if (i == 0 | i == 2)
							{
								weapons[i] = "IID_OLIV";
							}
							else
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 8)
									weapons[i] = "IID_OLIV";
								else if(weaponChance < 9)
									weapons[i] = "IID_REDG";
								else
									weapons[i] = "IID_HALF";
							}
							break;
						}
				}
			}

			return weapons;
		}

		private string[] TierTwoWeapons(int randClass)
		{
			string[] weapontypes = claPlayWeap[randClass].Split(';');
			string[] weapons = new string[weapontypes.Length];
			for (int i = 0; i < weapontypes.Length; i++)
			{
				switch (weapontypes[i])
				{
					case "S":
						{
							if (i == 0)
							{
								if (charName[charNum] == "ike")
									weapons[i] = "IID_ALON"; // ettard
								else if (charName[charNum] == "mist")
									weapons[i] = "IID_FLOR"; // florete
								else
								{
									int weaponChance = random.Next(10);
									if (weaponChance < 6)       // 60% chance of steel
										weapons[i] = "IID_STES";
									else if (weaponChance < 9)  // 30% chance of blade
									{
										if (cbxWeapPatch.Checked == false)
											weapons[i] = "IID_IRBL";
										else
											weapons[i] = "IID_POIS";
									}
									else                        // 10% chance of brave
										weapons[i] = "IID_BRAS";
								}
							}
							else if (i == 1)
							{
								if (random.Next(10) < 5) // 50% chance of windsword
									weapons[i] = "IID_WNDS";
								else if (cbxWeapPatch.Checked == false) // blade
									weapons[i] = "IID_IRBL";
								else
									weapons[i] = "IID_BROS";
							}
							else
							{
								weapons[i] = "IID_STES";
							}
							break;
						}
					case "L":
						{
							if (i == 0)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 6)       // 60% chance of steel
									weapons[i] = "IID_STEL";
								else if (weaponChance < 9)  // 30% chance of spear
								{
									if (cbxWeapPatch.Checked == false)
										weapons[i] = "IID_IRSP";
									else
										weapons[i] = "IID_POIL";
								}
								else                        // 10% chance of brave
									weapons[i] = "IID_BRAL";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 5) // 50% chance of javelin
									weapons[i] = "IID_JAVE";
								else if (cbxWeapPatch.Checked == false) // spear
									weapons[i] = "IID_IRSP";
								else
									weapons[i] = "IID_BROL";
							}
							else
							{
								weapons[i] = "IID_STEL";
							}
							break;
						}
					case "A":
						{
							if (i == 0)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 6)       // 60% chance of steel
									weapons[i] = "IID_STEA";
								else if (weaponChance < 9)  // 30% chance of polax
								{
									if (cbxWeapPatch.Checked == false)
										weapons[i] = "IID_IRPA";
									else
										weapons[i] = "IID_POIA";
								}
								else                        // 10% chance of brave
									weapons[i] = "IID_BRAA";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 5) // 50% chance of handaxe
									weapons[i] = "IID_HAND";
								else if (cbxWeapPatch.Checked == false) // polax
									weapons[i] = "IID_IRPA";
								else
									weapons[i] = "IID_BROA";
							}
							else
							{
								weapons[i] = "IID_STEA";
							}
							break;
						}
					case "K":
						{
							if (i == 0)
							{
								if (random.Next(10) < 8 | cbxWeapPatch.Checked == false)
									weapons[i] = "IID_STED";
								else
									weapons[i] = "IID_BROK";
							}
							else if (i == 1)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 7) // 70% chance of knife
									weapons[i] = "IID_STEK";
								else if (weaponChance < 9 | cbxWeapPatch.Checked == false)  // beastkiller
									weapons[i] = "IID_BEAS";
								else
									weapons[i] = "IID_BROD";
							}
							else
							{
								weapons[i] = "IID_KARD";
							}
							break;
						}
					case "B":
						{
							if (i == 0)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 6)       // 60% chance of steel
									weapons[i] = "IID_STEB";
								else if (weaponChance < 9)  // 30% chance of rolf
								{
									if (cbxWeapPatch.Checked == false)
										weapons[i] = "IID_LOFA";
									else
										weapons[i] = "IID_POIB";
								}
								else                        // 10% chance of killer
									weapons[i] = "IID_KILB";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7) // 70% chance of longbow
									weapons[i] = "IID_IRLB";
								else if (cbxWeapPatch.Checked == false) // steel
									weapons[i] = "IID_LOFA";
								else
									weapons[i] = "IID_BROB";

							}
							else
							{
								weapons[i] = "IID_STEB";
							}
							break;
						}
					case "G":
						{
							weapons[i] = "IID_BOWG";
							break;
						}
					case "F":
						{
							if (i == 0)
							{
								weapons[i] = "IID_ELFI";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7) 
									weapons[i] = "IID_ELFI";
								else                      
									weapons[i] = "IID_METE";
							}
							else
							{
								weapons[i] = "IID_FIRE";
							}
							break;
						}
					case "T":
						{
							if (i == 0)
							{
								weapons[i] = "IID_ELTH";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7)
									weapons[i] = "IID_ELTH";
								else
									weapons[i] = "IID_BOLT";
							}
							else
							{
								weapons[i] = "IID_THUN";
							}
							break;
						}
					case "W":
						{
							if (i == 0)
							{
								weapons[i] = "IID_ELWI";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7)
									weapons[i] = "IID_ELWI";
								else
									weapons[i] = "IID_BLIZ";
							}
							else
							{
								weapons[i] = "IID_WIND";
							}
							break;
						}
					case "M":
						{
							if (i == 0)
							{
								weapons[i] = "IID_ELLI";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7)
									weapons[i] = "IID_ELLI";
								else
									weapons[i] = "IID_PURG";
							}
							else
							{
								weapons[i] = "IID_LIGH";
							}
							break;
						}
					case "D":
						{
							if (i == 0)
							{
								weapons[i] = "IID_KARE";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7)
									weapons[i] = "IID_KARE";
								else
									weapons[i] = "IID_FENR";
							}
							else
							{
								weapons[i] = "IID_WORM";
							}
							break;
						}
					case "I":
						{
							weapons[i] = "IID_MAST";
							break;
						}
					case "H":
						{
							if (i == 0)
							{
								weapons[i] = "IID_RELI";
							}
							else if (i == 1)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 4) // 40% chance of mend
									weapons[i] = "IID_RELI";
								else if (weaponChance < 6) // 20% chance of restore
									weapons[i] = "IID_REST";
								else if (weaponChance < 9) // 30% chance of physic
									weapons[i] = "IID_REBL";
								else                  // 10% chance of ward
									weapons[i] = "IID_MSHI";
							}
							else
							{
								weapons[i] = "IID_SLEE";
							}
							break;
						}
					case "X":
						{
							if (i == 0)
							{
								weapons[i] = "IID_CHAN";
							}
							else if (i == 1)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 4) // 40% chance of olivi
									weapons[i] = "IID_OLIV";
								else if (weaponChance < 6) // 20% chance of halfbeast
									weapons[i] = "IID_HALF";
								else if (weaponChance < 7)
									weapons[i] = "IID_COIN";
								else if (weaponChance < 8)
									weapons[i] = "IID_BLUE";
								else if (weaponChance < 9)
									weapons[i] = "IID_CHAN";
								else
								{
									if (cbxFormshift.Checked == true)
										weapons[i] = "IID_TROO";
									else
										weapons[i] = "IID_SATO";
								}
							}
							else
							{
								if (random.Next(10) < 5)
									weapons[i] = "IID_OLIV";
								else
									weapons[i] = "IID_DEAT";
							}
							break;
						}
				}
			}

			return weapons;
		}

		private string[] TierThreeWeapons(int randClass)
		{
			string[] weapontypes = claPlayWeap[randClass].Split(';');
			string classtype = claPlayType[randClass];
			string[] weapons = new string[weapontypes.Length];
			for (int i = 0; i < weapontypes.Length; i++)
			{
				switch (weapontypes[i])
				{
					case "S":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_VAGU";
								else if (charName[charNum] == "ike")
									weapons[i] = "IID_ALON"; // ettard
								else if (charName[charNum] == "mist")
									weapons[i] = "IID_FLOR"; // florete
								else
								{
									if (random.Next(10) < 5)
										weapons[i] = "IID_KILS";
									else
										weapons[i] = "IID_SILS";
								}
							}
							else if (i == 1)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 5)
									weapons[i] = "IID_STOR";
								else if (weaponChance < 8)
									weapons[i] = "IID_DRAK";
								else
									weapons[i] = "IID_BRAS";
							}
							else
							{
								if (classtype == "A")
									weapons[i] = "IID_WNDS";
								else if (cbxWeapPatch.Checked == false)
									weapons[i] = "IID_STBL";
								else
									weapons[i] = "IID_POIS";
							}
							break;
						}
					case "L":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_ZANE";
								else
								{
									if (random.Next(10) < 5)
										weapons[i] = "IID_KILL";
									else
										weapons[i] = "IID_SILL";
								}
							}
							else if (i == 1)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 5)
									weapons[i] = "IID_SHSP";
								else if (weaponChance < 8)
									weapons[i] = "IID_HORS";
								else
									weapons[i] = "IID_BRAL";
							}
							else
							{
								if (classtype == "A")
									weapons[i] = "IID_JAVE";
								else if (cbxWeapPatch.Checked == false)
									weapons[i] = "IID_STSP";
								else
									weapons[i] = "IID_POIL";
							}
							break;
						}
					case "A":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_URVA";
								else
								{
									if (random.Next(10) < 5)
										weapons[i] = "IID_KILA";
									else
										weapons[i] = "IID_SILA";
								}
							}
							else if (i == 1)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 5)
									weapons[i] = "IID_SHOR";
								else if (weaponChance < 8)
									weapons[i] = "IID_HAMR";
								else
									weapons[i] = "IID_BRAA";
							}
							else
							{
								if (classtype == "A")
									weapons[i] = "IID_HAND";
								else if (cbxWeapPatch.Checked == false)
									weapons[i] = "IID_STPA";
								else
									weapons[i] = "IID_POIA";
							}
							break;
						}
					case "K":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_BASE";
								else
								{
									int weaponChance = random.Next(10);
									if (weaponChance < 5)
										weapons[i] = "IID_STIL";
									else if (weaponChance < 8 | cbxWeapPatch.Checked == false)
										weapons[i] = "IID_SILK";
									else
										weapons[i] = "IID_BROK";
								}
							}
							else if (i == 1)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 5)
									weapons[i] = "IID_SILD";
								else if (weaponChance < 8)
									weapons[i] = "IID_BEAS";
								else if (cbxWeapPatch.Checked == false)
									weapons[i] = "IID_SILD";
								else
									weapons[i] = "IID_BROD";
							}
							else
							{
								weapons[i] = "IID_STED";
							}
							break;
						}
					case "B":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_VALF";
								else
								{
									if (random.Next(10) < 5)
										weapons[i] = "IID_KILB";
									else
										weapons[i] = "IID_SILB";
								}
							}
							else if (i == 1)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 5)
									weapons[i] = "IID_STLB";
								else if (weaponChance < 8)
									weapons[i] = "IID_CHIN";
								else
									weapons[i] = "IID_BRAB";
							}
							else
							{
								if (cbxWeapPatch.Checked == false)
									weapons[i] = "IID_IRBA";
								else
									weapons[i] = "IID_POIB";
							}
							break;
						}
					case "G":
						{
							weapons[i] = "IID_CROS";
							break;
						}
					case "F":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_REXF";
								else if (charName[charNum] == "sanaki")
									weapons[i] = "IID_CYMB";
								else
									weapons[i] = "IID_GIGF";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7)
									weapons[i] = "IID_METE";
								else
									weapons[i] = "IID_ELFI";
							}
							else
							{
								weapons[i] = "IID_ELFI";
							}
							break;
						}
					case "T":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_REXB";
								else
									weapons[i] = "IID_GIGT";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7)
									weapons[i] = "IID_BOLT";
								else
									weapons[i] = "IID_ELTH";
							}
							else
							{
								weapons[i] = "IID_ELTH";
							}
							break;
						}
					case "W":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_REXC";
								else
									weapons[i] = "IID_GIGW";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7)
									weapons[i] = "IID_BLIZ";
								else
									weapons[i] = "IID_ELWI";
							}
							else
							{
								weapons[i] = "IID_ELWI";
							}
							break;
						}
					case "M":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_REXA";
								else
									weapons[i] = "IID_RESI";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7)
									weapons[i] = "IID_SHIN";
								else
									weapons[i] = "IID_PURG";
							}
							else
							{
								weapons[i] = "IID_ELLI";
							}
							break;
						}
					case "D":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_BARV";
								else
									weapons[i] = "IID_UERI";
							}
							else if (i == 1)
							{
								if (random.Next(10) < 7)
									weapons[i] = "IID_KARE";
								else
									weapons[i] = "IID_FENR";
							}
							else
							{
								weapons[i] = "IID_KARE";
							}
							break;
						}
					case "H":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_GODR";
								else
								{
									int weaponChance = random.Next(10);
									if (weaponChance < 4)
										weapons[i] = "IID_RECO";
									else if (weaponChance < 8)
										weapons[i] = "IID_REBL";
									else
										weapons[i] = "IID_SLEE";
								}
							}
							else if (i == 1)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 4) // 40% chance of physic
									weapons[i] = "IID_REBL";
								else if (weaponChance < 7) // 30% chance of rescue
									weapons[i] = "IID_RESC";
								else if (weaponChance < 9) // 20% chance of silence
									weapons[i] = "IID_SILE";
								else                  // 10% chance of hammerne
									weapons[i] = "IID_HAMM";
							}
							else
							{
								weapons[i] = "IID_LIVE";
							}
							break;
						}
					case "X":
						{
							if (i == 0)
							{
								if (charName[charNum] == "stephan" | charName[charNum] == "lehran")
									weapons[i] = "IID_ENER";
								else
									weapons[i] = "IID_COIN";
							}
							else if (i == 1)
							{
								int weaponChance = random.Next(10);
								if (weaponChance < 1) 
									weapons[i] = "IID_ENER";
								else if(weaponChance < 2)
									weapons[i] = "IID_SPIR";
								else if(weaponChance < 3)
									weapons[i] = "IID_SPEE";
								else if(weaponChance < 4)
									weapons[i] = "IID_DRAG";
								else if(weaponChance < 5)
									weapons[i] = "IID_TALI";
								else if (weaponChance < 6)
									weapons[i] = "IID_ELIX";
								else if (weaponChance < 7)
									weapons[i] = "IID_COIN";
								else if (weaponChance < 8)
									weapons[i] = "IID_BLUE";
								else if (weaponChance < 9)
									weapons[i] = "IID_ANAS";
								else
									weapons[i] = "IID_WHIT";
							}
							else
							{
								if (random.Next(10) < 9)
									weapons[i] = "IID_ANGE";
								else
									weapons[i] = "IID_BOOT";
							}
							break;
						}
				}
			}

			return weapons;
		}

		// weapons hub
		private void Weapons()
		{
			if (cbxMagicPatch.Checked == true & errorflag == 0)
				LM73MagicPatch();

			if (cbxWeapPatch.Checked == true & errorflag == 0)
				LM73WeapPatch();

			if (cbxKnifeCrit.Checked == true & errorflag == 0)
				knifeModifier();

			if (cbxRandWeap.Checked == true & errorflag == 0)
				weaponRandomizer();
		}

		// changes stats of magic tomes
		private void LM73MagicPatch()
		{
			int[] magicloc = new int[33];
			string[] swapmagic = new string[magicloc.Length];
			int[] swampmagicloc = new int[magicloc.Length];
			string[] new_tomename = new string[magicloc.Length];
			int[] tome_nameloc = new int[magicloc.Length];
			string[] new_tomedesc = new string[magicloc.Length];
			int[] tome_descloc = new int[magicloc.Length];
			int[] rankpointer = new int[magicloc.Length];
			int[] tome_mt = new int[magicloc.Length];
			int[] tome_hit = new int[magicloc.Length];
			int[] tome_crt = new int[magicloc.Length];
			int[] tome_wt = new int[magicloc.Length];
			int[] tome_rangemin = new int[magicloc.Length];
			int[] tome_rangemax = new int[magicloc.Length];
			int[] tome_use = new int[magicloc.Length];
			int[] tome_cost = new int[magicloc.Length];
			string[] tome_effect = new string[magicloc.Length];
			int[] effectpointer = new int[magicloc.Length];
			int[] tomestatboost = new int[magicloc.Length];


			string line;
			string[] values;
			// load in new class data
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\LM73MagicPatch.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all new tomes
			for (int i = 0; i < magicloc.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				// location in fe10data
				magicloc[i] = Convert.ToInt32(values[1]);
				// name of magic swapping with this one for pointer purposes
				swapmagic[i] = values[2];
				// location of new swapped magic
				swampmagicloc[i] = Convert.ToInt32(values[3]);
				// new tome name in e_common.m
				new_tomename[i] = values[4];
				if (gameVersion == 2) // PAL iso
				{
					// location to write name in e_common.m
					tome_nameloc[i] = Convert.ToInt32(values[22]);
					// location of description in e_common.m
					tome_descloc[i] = Convert.ToInt32(values[23]);
				}
				else
				{
					// location to write name in e_common.m
					tome_nameloc[i] = Convert.ToInt32(values[5]);
					// location of description in e_common.m
					tome_descloc[i] = Convert.ToInt32(values[7]);
				}
				// new description
				new_tomedesc[i] = values[6];
				// pointer for rank
				rankpointer[i] = Convert.ToInt32(values[9]);
				// weapon stats
				tome_mt[i] = Convert.ToInt32(values[10]);
				tome_hit[i] = Convert.ToInt32(values[11]);
				tome_crt[i] = Convert.ToInt32(values[12]);
				tome_wt[i] = Convert.ToInt32(values[13]);
				tome_rangemin[i] = Convert.ToInt32(values[14]);
				tome_rangemax[i] = Convert.ToInt32(values[15]);
				tome_use[i] = Convert.ToInt32(values[16]);
				tome_cost[i] = Convert.ToInt32(values[17]);
				// description of new effect
				tome_effect[i] = values[19];
				// pointer for new effect
				effectpointer[i] = Convert.ToInt32(values[20]);
				// stat that gets boosted by tome
				tomestatboost[i] = Convert.ToInt32(values[21]);
			}
			dataReader.Close();


			// mess around with stats in fe10data.cms
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			try
			{
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < magicloc.Length; i++)
					{
						// swap where necessary
						if (swapmagic[i] != "")
						{
							byte[] tome1 = new byte[52];
							byte[] tome2 = new byte[52];
							// read
							stream.Position = magicloc[i];
							stream.Read(tome1, 0, tome1.Length);
							stream.Position = swampmagicloc[i];
							stream.Read(tome2, 0, tome2.Length);
							// write
							stream.Position = magicloc[i];
							stream.Write(tome2, 0, tome2.Length);
							stream.Position = swampmagicloc[i];
							stream.Write(tome1, 0, tome1.Length);
						}

						// write new stats
						stream.Position = magicloc[i];
						// rank
						stream.Position += 20;
						if (rankpointer[i] != 0)
						{
							byte[] rankbytes = int2bytes(rankpointer[i]);
							stream.Write(rankbytes, 0, 4);
						}
						else
							stream.Position += 4;
						// cost per use
						stream.Position += 14;
						if (tome_cost[i] != 0)
						{
							byte[] cost = int2bytes(tome_cost[i]);
							stream.Write(cost, 2, 2);
						}
						else
							stream.Position += 2;
						// mt,acc,crit,wt,uses
						stream.WriteByte((byte)tome_mt[i]);
						stream.WriteByte((byte)tome_hit[i]);
						stream.WriteByte((byte)tome_crt[i]);
						stream.WriteByte((byte)tome_wt[i]);
						stream.WriteByte((byte)tome_use[i]);
						// range
						stream.Position += 1;
						stream.WriteByte((byte)tome_rangemin[i]);
						stream.WriteByte((byte)tome_rangemax[i]);

						// effect
						stream.Position += 5;
						if (tome_effect[i] == "SPECIAL")
						{
							stream.WriteByte(0x01);
							stream.WriteByte(0x00);
							stream.Position += 1;
							byte[] effect = int2bytes(effectpointer[i]);
							stream.Write(effect, 0, 4);
						}
						else if (tome_effect[i] == "EFFECTIVE")
						{
							stream.WriteByte(0x00);
							stream.WriteByte(0x01);
							stream.Position += 1;
							byte[] effect = int2bytes(effectpointer[i]);
							stream.Write(effect, 0, 4);
						}
						else
							stream.Position += 3;

						// stat boost if applicable
						if (tomestatboost[i] != 0)
						{
							for (int j = 0; j < 8; j++)
							{
								if (j != tomestatboost[i])
									stream.WriteByte(0x00); // not the stat boosted
								else if (j == 5)
									stream.WriteByte(0x05); // luck gets +5
								else
									stream.WriteByte(0x03); // others get +3
							}
						}

					}

					/*
					// set up dark and bow class SFX for weapon effectiveness
					//int darkpointer = 221159;
					int bowpointer = 221115;
					//int[] darkclasses = { 49752, 50568, 51384, 51520, 51656 };
					int[] bowclasses = { 44584, 44720, 44856, 44992, 53696, 54512, 54648, 55192, 56008 };
					
					for (int i = 0; i < darkclasses.Length; i++)
					{
						// go to final classtype
						stream.Position = darkclasses[i] + 100;
						byte[] pointerbytes = int2bytes(darkpointer);
						stream.Write(pointerbytes, 0, 4);
					}
					
					for (int i = 0; i < bowclasses.Length; i++)
					{
						// go to final classtype
						stream.Position = bowclasses[i] + 100;
						byte[] pointerbytes = int2bytes(bowpointer);
						stream.Write(pointerbytes, 0, 4);
					}

					// swap def and res of thunder mage classes to help the fact that thunder is 1-range
					int[] thunderclasses = { 48502, 48638, 49318, 49454, 50134, 50814 };
					for (int i = 0; i < thunderclasses.Length; i++)
					{
						int def, res;
						// stat caps, bases, growths, promotion gains
						stream.Position = thunderclasses[i];
						for (int j = 0; j < 4; j++)
						{
							def = stream.ReadByte();
							res = stream.ReadByte();
							stream.Position -= 2;
							stream.WriteByte((byte)res);
							stream.WriteByte((byte)def);
							stream.Position += 6;
						}
					}
					*/

				}
			}
			catch
			{
				textBox1.Text = "Error in magicpatch: can't find files\\FE10Data.cms.decompressed!";
				errorflag = 1;
			}


			// change text for name and description
			try
			{
				using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_common.m", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < magicloc.Length; i++)
					{
						if (tome_nameloc[i] != 0)
						{
							stream.Position = tome_nameloc[i];
							byte[] namestring = System.Text.Encoding.ASCII.GetBytes(new_tomename[i]);
							stream.Write(namestring, 0, namestring.Length);
							stream.WriteByte(0x00);
						}
						if (tome_descloc[i] != 0)
						{
							string[] description = new_tomedesc[i].Split(';');
							stream.Position = tome_descloc[i];
							for (int j = 0; j < description.Length; j++)
							{
								byte[] descrstring = System.Text.Encoding.ASCII.GetBytes(description[j]);
								stream.Write(descrstring, 0, descrstring.Length);
								if (j != description.Length - 1)
									stream.WriteByte(0x0A); // new line
								else
									stream.WriteByte(0x00);
							}

						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error in magicpatch: can't find files\\Mess\\e_common.m!";
				errorflag = 1;
			}


		}

		// changes venin/bronze weapons to cooler things
		private void LM73WeapPatch()
		{
			int[] weaploc = new int[10];
			int[] newfunctionloc = new int[weaploc.Length];
			string[] new_weapname = new string[weaploc.Length];
			int[] weap_nameloc = new int[weaploc.Length];
			string[] new_weapdesc = new string[weaploc.Length];
			int[] weap_descloc = new int[weaploc.Length];
			int[] rankpointer = new int[weaploc.Length];
			int[] weap_mt = new int[weaploc.Length];
			int[] weap_hit = new int[weaploc.Length];
			int[] weap_crt = new int[weaploc.Length];
			int[] weap_wt = new int[weaploc.Length];
			int[] weap_rangemin = new int[weaploc.Length];
			int[] weap_rangemax = new int[weaploc.Length];
			int[] weap_use = new int[weaploc.Length];
			int[] weap_cost = new int[weaploc.Length];
			string[] weap_effect = new string[weaploc.Length];
			int[] effectpointer = new int[weaploc.Length];


			string line;
			string[] values;
			// load in new class data
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\LM73WeaponPatch.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all new weaps
			for (int i = 0; i < weaploc.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				// location in fe10data
				weaploc[i] = Convert.ToInt32(values[1]);
				// pointer for new function
				newfunctionloc[i] = Convert.ToInt32(values[3]);
				// new weap name in e_common.m
				new_weapname[i] = values[4];
				if (gameVersion == 2) // PAL iso
				{
					// location to write name in e_common.m
					weap_nameloc[i] = Convert.ToInt32(values[22]);
					// location of description in e_common.m
					weap_descloc[i] = Convert.ToInt32(values[23]);
				}
				else
				{
					// location to write name in e_common.m
					weap_nameloc[i] = Convert.ToInt32(values[5]);
					// location of description in e_common.m
					weap_descloc[i] = Convert.ToInt32(values[7]);
				}
				// new description
				new_weapdesc[i] = values[6];
				// pointer for rank
				rankpointer[i] = Convert.ToInt32(values[9]);
				// weapon stats
				weap_mt[i] = Convert.ToInt32(values[10]);
				weap_hit[i] = Convert.ToInt32(values[11]);
				weap_crt[i] = Convert.ToInt32(values[12]);
				weap_wt[i] = Convert.ToInt32(values[13]);
				weap_rangemin[i] = Convert.ToInt32(values[14]);
				weap_rangemax[i] = Convert.ToInt32(values[15]);
				weap_use[i] = Convert.ToInt32(values[16]);
				weap_cost[i] = Convert.ToInt32(values[17]);
				// description of new effect
				weap_effect[i] = values[19];
				// pointer for new effect
				effectpointer[i] = Convert.ToInt32(values[20]);
			}
			dataReader.Close();


			// mess around with stats in fe10data.cms
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			try
			{
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < weaploc.Length; i++)
					{
						// write new stats
						stream.Position = weaploc[i];
						// new function if necessary
						stream.Position += 16;
						if (newfunctionloc[i] != 0)
						{
							byte[] functionbytes = int2bytes(newfunctionloc[i]);
							stream.Write(functionbytes, 0, 4);
						}
						else
							stream.Position += 4;
						// rank
						if (rankpointer[i] != 0)
						{
							byte[] rankbytes = int2bytes(rankpointer[i]);
							stream.Write(rankbytes, 0, 4);
						}
						else
							stream.Position += 4;
						// cost per use
						stream.Position += 14;
						if (weap_cost[i] != 0)
						{
							byte[] cost = int2bytes(weap_cost[i]);
							stream.Write(cost, 2, 2);
						}
						else
							stream.Position += 2;
						// mt,acc,crit,wt,uses
						stream.WriteByte((byte)weap_mt[i]);
						stream.WriteByte((byte)weap_hit[i]);
						stream.WriteByte((byte)weap_crt[i]);
						stream.WriteByte((byte)weap_wt[i]);
						stream.WriteByte((byte)weap_use[i]);
						// range
						stream.Position += 1;
						stream.WriteByte((byte)weap_rangemin[i]);
						stream.WriteByte((byte)weap_rangemax[i]);

						// effect
						stream.Position += 5;
						if (weap_effect[i] == "SPECIAL")
						{
							if (i < 4 | i > 7) // sword/lance or knife
							{
								stream.WriteByte(0x01);
								stream.WriteByte(0x00);
							}
							else if (i < 6) // axe
							{
								stream.WriteByte(0x01);
								stream.WriteByte(0x01); // keeps the effectiveness vs doors
							}
							else if (i < 8) // bow
							{
								stream.WriteByte(0x02); // keeps the expandrange skill for marksman
								stream.WriteByte(0x01); // keeps the effectiveness vs flying
								stream.Position += 4; // skips over said expandrange skill
							}
							stream.Position += 1;
							byte[] effect = int2bytes(effectpointer[i]);
							stream.Write(effect, 0, 4);
						}
						else if (weap_effect[i] == "EFFECTIVE")
						{
							if (i < 4 | i > 7) // sword/lance or knife
							{
								stream.WriteByte(0x00);
								stream.WriteByte(0x01);
							}
							else if (i < 6) // axe
							{
								stream.WriteByte(0x00);
								stream.WriteByte(0x02); // keeps the effectiveness vs doors
							}
							else if (i < 8) // bow
							{
								stream.WriteByte(0x01); // keeps the expandrange skill for marksman
								stream.WriteByte(0x02); // keeps the effectiveness vs flying
								stream.Position += 4; // skips over said expandrange skill
							}
							stream.Position += 1;
							byte[] effect = int2bytes(effectpointer[i]);
							stream.Write(effect, 0, 4);
						}
						else
							stream.Position += 3;


					}

					// change weapon ranks to be minimum D for sword/axe/lance/bow
					int[] eranks = { 177742, 177755, 177680 }; // boy do i love hardcoded values
					for (int i = 0; i < eranks.Length; i++)
					{
						stream.Position = eranks[i];
						stream.WriteByte(0x44); // D
					}
				}
			}
			catch
			{
				textBox1.Text = "Error in weaponpatch: can't find files\\FE10Data.cms.decompressed!";
				errorflag = 1;
			}


			// change text for name and description
			try
			{
				using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_common.m", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < weaploc.Length; i++)
					{
						if (weap_nameloc[i] != 0)
						{
							stream.Position = weap_nameloc[i];
							byte[] namestring = System.Text.Encoding.ASCII.GetBytes(new_weapname[i]);
							stream.Write(namestring, 0, namestring.Length);
							stream.WriteByte(0x00);
						}
						if (weap_descloc[i] != 0)
						{
							string[] description = new_weapdesc[i].Split(';');
							stream.Position = weap_descloc[i];
							for (int j = 0; j < description.Length; j++)
							{
								byte[] descrstring = System.Text.Encoding.ASCII.GetBytes(description[j]);
								stream.Write(descrstring, 0, descrstring.Length);
								if (j != description.Length - 1)
									stream.WriteByte(0x0A); // new line
								else
									stream.WriteByte(0x00);
							}

						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error in weappatch: can't find files\\Mess\\e_common.m!";
				errorflag = 1;
			}


		}

		// adds crit to knives
		private void knifeModifier()
		{
			int[] weapLocation = new int[13];

			string line;
			string[] values;
			// initialize character information
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\WeaponData.csv");

			// skip to knives
			for (int i = 0; i < 75; i++)
				line = dataReader.ReadLine();
			// loop through knives
			for (int i = 0; i < weapLocation.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');

				weapLocation[i] = Convert.ToInt32(values[1]);
			}
			dataReader.Close();


			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			
			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < weapLocation.Length; i++)
					{
						stream.Position = weapLocation[i] + 42;
						int knifemt = stream.ReadByte();
						knifemt += 5;
						if (knifemt > 127)
							knifemt = 127;
						stream.Position -= 1;
						stream.WriteByte((byte)knifemt);
					}
				}
			}
			catch
			{
				textBox1.Text = "Errorcode 18: Error in knife modifier. Randomization incomplete!";
				errorflag = 1;
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
							// read in stat
							randStat = stream.ReadByte();
							// only change if deviation is not zero
							if (weapDev[j] != 0)
							{
								// calculate min and max possible values
								minStat = randStat - weapDev[j];
								maxStat = randStat + weapDev[j];

								// if S or SS rank laguz weapon, force min to be value of previous (max for WT)
								if (i >= 143 & (i - 143) % 3 != 0)
								{
									int current_loc = (int)stream.Position;
									// go to previous weapon and read
									stream.Position = weapLocation[i - 1] + 40 + j;
									int prev_val = stream.ReadByte();
									// return to current weapon
									stream.Position = current_loc;
									if (j == 3) // WT
									{
										if (maxStat > prev_val)
											maxStat = prev_val;
									}
									else
									{
										if (minStat < prev_val)
											minStat = prev_val;
									}
								}

								// limit min and max to specified values
								if (minStat < weapMin[j])
									minStat = weapMin[j];
								else if (minStat > weapMax[j])
									minStat = weapMax[j];
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
			}
			catch
			{
				textBox1.Text = "Errorcode 19: Error in weapon randomization. Randomization incomplete!";
				errorflag = 1;
			}
		}



		// STAT AND GROWTH FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// stats hub, controls what functions to run
		private void Stats()
		{
			if (cbxGrowthRand.Checked == true)
				growthRateModifier();
			else if (cbxGrowthShuffle.Checked == true)
				growthShuffle();
			else if (cbxZeroGrowths.Checked == true)
				zeroGrowthsPatch();
			if (cbxNegGrowths.Checked == true)
				negGrowthsPatch();

			if ((cbxRandClassBases.Checked == true | cbxShuffleClassBases.Checked == true) & errorflag == 0)
				classBaseStats();

			if (cbxRandBases.Checked == true & errorflag == 0)
				randBaseStats();
			else if (cbxShuffleBases.Checked == true & errorflag == 0)
				shuffleBaseStats();


			// swap str/mag if necessary
			if (cbxStrMag.Checked == true & (cbxRandRecr.Checked == true | cbxClassRand.Checked == true) & errorflag == 0)
				strMagSwap();

			if ((cbxStatCaps.Checked == true | cbxStatCapDev.Checked == true | cbxStatCapFlat.Checked == true) & errorflag == 0 )
				statCapChanges();
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
				textBox1.Text = "Errorcode 20: Error in growthrate modification. Randomization incomplete!";
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
						newgrowths = shuffler(totalgrowth, 8);
						/*
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
						*/
						//check for growths under 15%
						if (cbxGrowthShuffleMin.Checked == true)
						{
							for (int i = 0; i < 8; i++)
							{
								while(newgrowths[i] < 15)
								{
									newgrowths[i] += 1;
									int dev = 15 - newgrowths[i];
									for (int j = i + 1; j < i + 8; j++)
									{
										int iteration = j;
										if (iteration > 7)
											iteration -= 8;
										// take deviation off of a larger growth
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
						int maximum = 255;
						if (cbxGrowthShuffleMax.Checked == true)
							maximum = 100;
						// check for growths greater than 255%
						for (int i = 0; i < 8; i++)
						{
							if (newgrowths[i] > maximum)
							{
								int dev = newgrowths[i] - maximum;
								for (int j = i + 1; j < i + 8; j++)
								{
									int iteration = j;
									if (iteration > 7)
										iteration -= 8;
									// give deviation to smaller growth
									if (newgrowths[iteration] <= maximum - dev)
									{
										newgrowths[iteration] += dev;
										break;
									}
								}
								newgrowths[i] = maximum;
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
				textBox1.Text = "Errorcode 21: Error in growthrate shuffling. Randomization incomplete!";
				errorflag = 1;
			}
		}

		// new zero growths patch!
		private void zeroGrowthsPatch()

		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			// set all growths to zero
			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int charNum = 0; charNum < totalUnitNumber; charNum++)
					{
						// go to first growth rate
						stream.Position = charGrowth[charNum];
						
						for (int k = 0; k < 8; k++)
						{
							stream.WriteByte(0x00);
						}
					}
				}

				// modify main.dol
				string mainFile = dataLocation.Remove(dataLocation.Length - 5, 5) + "sys\\main.dol";
				using (var stream = new System.IO.FileStream(mainFile, System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
				{
					int growthcalc = 0;
					int bexpcalc = 0;
					if (gameVersion == 0)
					{
						growthcalc = 431204;
						bexpcalc = 431560;
					}
					else if (gameVersion == 1)
					{
						growthcalc = 431140;
						bexpcalc = 431496;
					}
					else if (gameVersion == 2)
					{
						growthcalc = 318504;
						bexpcalc = 318860;
					}
					
					if (growthcalc != 0 & bexpcalc != 0)
					{
						byte[] nop = new byte[4] { 0x60, 0x00, 0x00, 0x00 };
						// growth calculation occurs for all eight stats with 12 byte intervals - replace first four bytes with nop (0x60 00 00 00)
						stream.Position = growthcalc;
						for (int i = 0; i < 8; i++)
						{
							stream.Write(nop, 0, 4);
							stream.Position += 8;
						}
						// bexp calculation occurs for all eight stats with 8 byte intervals - replace first four bytes with nop
						stream.Position = bexpcalc;
						for (int i = 0; i < 8; i++)
						{
							stream.Write(nop, 0, 4);
							stream.Position += 4;
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error in Zero Growths patch! Randomization failed!";
				errorflag = 1;
			}
		}

		// negative growths
		private void negGrowthsPatch()
		{
			// modify main.dol
			string mainFile = dataLocation.Remove(dataLocation.Length - 5, 5) + "sys\\main.dol";
			using (var stream = new System.IO.FileStream(mainFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
			{
				int[] locations = new int[4];
				if (gameVersion == 0)
				{
					locations[0] = 430236;
					locations[1] = 431204;
					locations[2] = 430144;
					locations[3] = 430164;
				}
				else if (gameVersion == 1)
				{
					locations[0] = 430172;
					locations[1] = 431140;
					locations[2] = 430080;
					locations[3] = 430100;
				}
				else if (gameVersion == 2)
				{
					locations[0] = 317536;
					locations[1] = 318504;
					locations[2] = 317444;
					locations[3] = 317464;
				}
				// change add 1 operation to subtract 1
				stream.Position = locations[0] + 2;
				stream.WriteByte(0xFF);
				stream.WriteByte(0xFF);

				byte[] nop = new byte[4] { 0x60, 0x00, 0x00, 0x00 };
				// growth calculation occurs for all eight stats with 12 byte intervals - replace first four bytes with nop (0x60 00 00 00)
				stream.Position = locations[1];
				for (int i = 0; i < 8; i++)
				{
					stream.Write(nop, 0, 4);
					stream.Position += 8;
				}

				// change load 1 operation to load -1
				stream.Position = locations[2] + 2;
				stream.WriteByte(0xFF);
				stream.WriteByte(0xFF);

				// change add 1 operation to subtract 1
				stream.Position = locations[3] + 2;
				stream.WriteByte(0xFF);
				stream.WriteByte(0xFF);
			}
		}

		// modifies class base stats by deviation or by shuffling
		private void classBaseStats()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			int[] basestatLoc = new int[157];
			string[] laguzclasses = new string[basestatLoc.Length];

			string line;
			string[] values;

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\StatCaps.csv");

			// skip header line/
			line = dataReader.ReadLine();
			// loop through all classes
			for (int i = 0; i < basestatLoc.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				basestatLoc[i] = Convert.ToInt32(values[1]) + 8;
				laguzclasses[i] = values[4];
			}
			dataReader.Close();

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < basestatLoc.Length; i++)
					{
						if (laguzclasses[i] != "L")
						{
							if (cbxRandClassBases.Checked == true)
							{
								int basestat;
								int randstat;
								// go to first stat
								stream.Position = basestatLoc[i];
								for (int k = 0; k < 8; k++)
								{

									// read base
									basestat = stream.ReadByte();
									// change from signed to decimal
									if (basestat > 127)
										basestat -= 256;
									int minbase = basestat - Convert.ToInt32(numericClassBaseDev.Value);
									int maxbase = basestat + Convert.ToInt32(numericClassBaseDev.Value);
									// prevent class bases from being less than zero
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
							else if (cbxShuffleClassBases.Checked == true)
							{
								int hpval = 0;
								int totalstat = Convert.ToInt32(numericClassBaseShuf.Value);
								int basestat;
								int[] newstats = new int[8];
								// go to first stat
								stream.Position = basestatLoc[i];
								for (int k = 0; k < 8; k++)
								{
									// read base
									basestat = stream.ReadByte();

									if (k == 0)
										hpval = basestat;

									// change from signed to decimal
									if (basestat > 127)
										basestat -= 256;

									if ((cbxHPShuffleclass.Checked == false & k == 0) | k == 5) // don't add hp and luck
									{ }
									else // everything else is added to total
										totalstat += basestat;
								}
								int[] shuffledstats;
								if (cbxHPShuffleclass.Checked == true)
								{
									shuffledstats = shuffler(totalstat, 7);
								}
								else
								{
									shuffledstats = shuffler(totalstat, 6);
								}
								/*
								// randomize partitions
								int numbstats;
								if (cbxHPShuffleclass.Checked == true)
									numbstats = 7;
								else
									numbstats = 6;
								if (totalstat < 0)
									totalstat = 0;
								for (int k = 0; k < numbstats; k++)
								{
									if (k == numbstats - 1)
										newstats[k] = totalstat;
									else
										newstats[k] = random.Next(totalstat);
								}
								// sort by descending order
								Array.Sort(newstats);
								Array.Reverse(newstats);
								for (int k = 0; k < numbstats; k++)
								{
									if (k == numbstats - 1)
										newstats[k] = newstats[k];
									else
										newstats[k] = newstats[k] - newstats[k + 1];
								}
								*/

								// move stats and insert hp and lck
								if (cbxHPShuffleclass.Checked == false)
								{
									newstats[7] = shuffledstats[5]; // res
									newstats[6] = shuffledstats[4]; // def
									newstats[5] = 0;				// lck
									newstats[4] = shuffledstats[3]; // spd
									newstats[3] = shuffledstats[2]; // skl
									newstats[2] = shuffledstats[1]; // mag
									newstats[1] = shuffledstats[0]; // atk
									newstats[0] = hpval;			// hp
								}
								else // luck is always zero for classes
								{
									newstats[7] = shuffledstats[6]; // res
									newstats[6] = shuffledstats[5]; // def
									newstats[5] = 0;                // lck
									for (int k = 0; k < 5; k++)
										newstats[k] = shuffledstats[k];
								}

								// write new base to game
								stream.Position = basestatLoc[i];
								for (int k = 0; k < 8; k++)
								{
									stream.WriteByte(Convert.ToByte(newstats[k]));

								}
							}
						}
						else
						{
							// transformed laguz classes need to have same base stats as untransformed version
							stream.Position = basestatLoc[i - 1];
							byte[] laguzstats = new byte[8];
							stream.Read(laguzstats, 0, 8);
							stream.Position = basestatLoc[i];
							stream.Write(laguzstats, 0, 8);
						}
					}

				}
			}
			catch
			{
				textBox1.Text = "Error in Class Bases: Cannot find file \\FE10Data.cms.decompressed!";
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
						int[] classbases = new int[8];
						// read in class bases for character
						if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
							stream.Position = claDataLoc[newClass[charNum]] + 112;
						else
							stream.Position = claDataLoc[charVanillaClass[charNum]] + 112;

						for (int i = 0; i < 8; i++)
							classbases[i] = stream.ReadByte();

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
							// prevent personal + class bases from being less than zero
							if (minbase + classbases[k] < 0)
								minbase = -classbases[k];
							if (maxbase + classbases[k] < 0)
								maxbase = -classbases[k];

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
				textBox1.Text = "Errorcode 22: Error in base stat modification. Randomization incomplete!";
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
						int[] shuffledstats;
						if (cbxHPLCKShuffle.Checked == true)
						{
							shuffledstats = shuffler(totalstat, 8);
						}
						else
						{
							shuffledstats = shuffler(totalstat, 6);
						}

						/*
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
						}*/

						// move stats and insert hp and lck
						if (cbxHPLCKShuffle.Checked == false)
						{
							newstats[7] = shuffledstats[5]; // res
							newstats[6] = shuffledstats[4]; // def
							newstats[5] = lckval;           // lck
							newstats[4] = shuffledstats[3]; // spd
							newstats[3] = shuffledstats[2]; // skl
							newstats[2] = shuffledstats[1]; // mag
							newstats[1] = shuffledstats[0]; // atk
							newstats[0] = hpval;            // hp
						}
						else
						{
							for (int i = 0; i < 8; i++)
								newstats[i] = shuffledstats[i];
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
				textBox1.Text = "Errorcode 23: Error in base stat shuffling. Randomization incomplete!";
				errorflag = 1;
			}
		}

		// makes changes to character/class base stats in (vain)
		// attempt to balance random classes
		private void baseStatChanges()
		{
			int[,] inputMatrix = new int[108, 9];

			string line;
			string[] values;

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\NewBases.csv");

			// skip header line
			line = dataReader.ReadLine();

			for (int i = 0; i < 108; i++)
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
			dataReader.Close();

			// only need to randomize personal bases when character units are randomized
			int number;
			if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
				number = 108;
			else
				number = 81;

			// change rogue to female rogue if classpatch on
			if (cbxClassPatch.Checked == true)
				inputMatrix[79, 0] = 47688;

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
				textBox1.Text = "Errorcode 24: Error in base stat modifications. Randomization incomplete!";
				errorflag = 1;
			}

		}

		// swaps str and mag growth/bases for units who changed from physical
		// to magical or vice-versa
		private void strMagSwap()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			int[] magClasses = { 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 
								 66, 68, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 93, 94, 95, 97 };
			
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

						// go to atk base
						stream.Position = charBases[charNum] + 1;
						atkgrowth = stream.ReadByte();
						maggrowth = stream.ReadByte();
						if (atkgrowth > 127)
							atkgrowth -= 256;
						if (maggrowth > 127)
							maggrowth -= 256;
						if ((classType == "m" & atkgrowth > maggrowth) |
							(classType == "p" & atkgrowth < maggrowth))
						{
							stream.Position -= 2;
							if (atkgrowth < 0)
								atkgrowth += 256;
							if (maggrowth < 0)
								maggrowth += 256;
							stream.WriteByte(Convert.ToByte(maggrowth));
							stream.WriteByte(Convert.ToByte(atkgrowth));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Errorcode 25: Error in str/mag swap. Randomization incomplete!";
				errorflag = 1;
			}
		}

		// sets stat caps to desired values
		private void statCapChanges()
		{
			string line;
			string[] values;
			int statCap1 = Convert.ToInt32(numericStatCap1.Value);
			int statCap2 = Convert.ToInt32(numericStatCap2.Value);
			int statCap3 = Convert.ToInt32(numericStatCap3.Value);
			int statDev = Convert.ToInt32(numericStatCapDev.Value);
			int statFlat = Convert.ToInt32(numericStatCapFlat.Value);
			int[] capLocation = new int[157];
			string[] statTier = new string[capLocation.Length];
			string[] physmag = new string[capLocation.Length];
			string[] laguzclasses = new string[capLocation.Length];

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\StatCaps.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all 157 classes
			for (int i = 0; i < capLocation.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				capLocation[i] = Convert.ToInt32(values[1]);
				physmag[i] = values[2];
				statTier[i] = values[3];
				laguzclasses[i] = values[4];
			}

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{

					for (int i = 0; i < capLocation.Length; i++)
					{
						if (cbxT3Statcaps.Checked == false | (cbxT3Statcaps.Checked == true & statTier[i] == "3"))
						{
							stream.Position = capLocation[i];
							if (cbxStatCaps.Checked == true)
							{
								// write max for HP
								stream.WriteByte(0x7F);
								// max cap for all other 7 stats per class
								for (int j = 0; j < 7; j++)
								{
									if (statTier[i] == "1")
										stream.WriteByte(Convert.ToByte(statCap1));
									else if (statTier[i] == "2")
										stream.WriteByte(Convert.ToByte(statCap2));
									else
										stream.WriteByte(Convert.ToByte(statCap3));
								}
							}
							else if (cbxStatCapDev.Checked == true)
							{
								// each cap is read, then randomized within deviation
								for (int j = 0; j < 8; j++)
								{
									int newstatcap = stream.ReadByte();
									int min = newstatcap - statDev;
									int max = newstatcap + statDev;
									if (min < numericStatCapMin.Value)
										min = (int)numericStatCapMin.Value;
									if (min > 115)
										min = 115;
									if (max < numericStatCapMin.Value)
										max = (int)numericStatCapMin.Value;
									if (max > 115)
										max = 115;
									newstatcap = random.Next(min, max + 1);
									stream.Position -= 1;
									stream.WriteByte(Convert.ToByte(newstatcap));
								}
							}
							else if (cbxStatCapFlat.Checked == true)
							{
								// each cap is read, then flat addition
								for (int j = 0; j < 8; j++)
								{
									int newstatcap = stream.ReadByte() + statFlat;
									if (newstatcap > 115)
										newstatcap = 115;
									if (newstatcap < numericStatCapMin.Value)
										newstatcap = (int)numericStatCapMin.Value;
									stream.Position -= 1;
									stream.WriteByte(Convert.ToByte(newstatcap));
								}
							}
						}
						// make sure stat caps of transformed laguz are at least equal to stat caps of untransformed laguz
						if (laguzclasses[i] == "L")
						{
							stream.Position = capLocation[i - 1];
							byte[] laguzstatcaps = new byte[8];
							stream.Read(laguzstatcaps, 0, 8);
							stream.Position = capLocation[i];
							for (int j = 0; j < 8; j++)
							{
								if (stream.ReadByte() < laguzstatcaps[j])
								{
									stream.Position -= 1;
									stream.WriteByte(laguzstatcaps[j]);
								}
							}
						}
					}

				}
			}
			catch
			{
				textBox1.Text = "Errorcode 26: Error in stat cap modification. Randomization incomplete!";
				errorflag = 1;
			}
		}

		// creates array with random values that add up to given total - used for shuffling
		private int[] shuffler(int total, int number)
		{
			int deviation = total/10;
			if (deviation < 2)
				deviation = 2;
			int percentage = 50;
			int[] statblock = new int[number];
			for (int i = 0; i < statblock.Length; i++)
				statblock[i] = 0;

			while (total > 0)
			{
				int addition;
				if (total < deviation)
					addition = random.Next(0, total + 1);
				else
					addition = random.Next(0, deviation + 1);
				if (random.Next(100) < percentage)
				{
					statblock[random.Next(number)] += addition;
					total -= addition;
				}
			}
			return (statblock);
		}



		// ENEMY FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// enemy hub, decides what functions to run
		private void Enemies()
		{
			if ((cbxRandEnemy.Checked == true | cbxTier3Enemies.Checked == true | cbxEnemWeaps.Checked == true) & errorflag == 0)
				genericEnemyRandomizer();

			if (cbxEnemDrops.Checked == true & errorflag == 0)
				enemyDrops();

			if (cbxBabyPart2.Checked == true & errorflag == 0)
				lowerPart2Stats();

			if (cbxEnemyGrowth.Checked == true & errorflag == 0)
				enemyGrowthModifier();

			if ((cbxEnemySkills.Checked == true | cbxBossSkills.Checked == true) & errorflag == 0)
				enemySkillRandom();

			if (cbxRandBosses.Checked == true | (cbxRandRecr.Checked == true & cbxEnemyRecruit.Checked == true))
				bossRando();

			if (cbxBuffBosses.Checked == true)
				bossBuff();

			if (cbxRandBosses.Checked | (cbxRandRecr.Checked == true & cbxEnemyRecruit.Checked == true) | cbxRandEnemy.Checked == true | cbxTier3Enemies.Checked == true)
			{
				enemyAnimations();
				bossAnimations();
			}

			if (cbxRandEnemy.Checked == true & cbxSimilarEnemy.Checked == false)
				laguzEnemyAI();
		}

		// randomizes enemy classes
		private void genericEnemyRandomizer()
		{
			string[] enemFile = new string[370];
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
			string[] enemClassType = new string[enemFile.Length];
			string[] outputchapter = new string[enemFile.Length];
			string[] enemWeapType = new string[enemFile.Length];

	
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
				enemClassType[i] = values[17];
				outputchapter[i] = values[18];
				enemWeapType[i] = values[19];
			}
			dataReader.Close();

			string newweapon;
			byte[] classbyte = new byte[7];
			byte[] weapbyte = new byte[8];

			int numberOfEnemies = enemFile.Length;
			if (cbxSpirits.Checked == false)
			{
				numberOfEnemies -= 6;
				// negate spirits
				claEnemTier[125] = "x";
				claEnemTier[126] = "x";
				claEnemTier[127] = "x";
			}

			if (cbxRandEnemy.Checked == true | cbxTier3Enemies.Checked == true)
			{
				randEnemyOutput = "<table><tr> <th>Chapter</th> <th>Original Class</th> <th>New Class</th> </tr>";

				string[] newclassname = System.IO.File.ReadAllLines(file + "\\assets\\enemyclassnames.txt");

				// let's randomize
				for (int enemNum = 0; enemNum < numberOfEnemies; enemNum++)
				{
					randEnemyOutput += "<tr> <td>" + outputchapter[enemNum] + "</td> <td>" + enemClassName[enemNum] + "</td> <td>";

					// healers have to be included or the current unit can't be a healer
					if (cbxEnemHealers.Checked != true | enemClassType[enemNum] != "H")
					{
						using (var stream = new System.IO.FileStream(dataLocation + enemFile[enemNum], System.IO.FileMode.Open,
								System.IO.FileAccess.ReadWrite))
						{
							List<int> possibleclasses = new List<int>();

							// random enemies %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
							if (cbxRandEnemy.Checked == true)
							{
								// look for t3 classes (marked with 4) instead of SP (marked with 3)
								if (cbxTier3Enemies.Checked == true & enemClass[enemNum] == "3")
									enemClass[enemNum] = "4";

								for (int k = 0; k < claName.Length; k++)
								{
									if (cbxSimilarEnemy.Checked == true | (enemClassType[enemNum] == "L" & cbxNoEnemyLaguz.Checked == true))
									{
										if ((enemChap[enemNum] != "3c" & enemChap[enemNum].Contains("4") == false
											& enemChap[enemNum].Contains("T") == false) & claName[k].Contains("dragon"))
										// no dragons before 3c
										{ }
										else if (enemChap[enemNum].Contains("T") == false & claName[k].Contains("spirit"))
										// no spirits before tower
										{ }
										else if (claEnemTier[k].Contains(enemClass[enemNum]) & ((claEnemType[k] == enemClassType[enemNum])
												| (cbxEnemHealers.Checked == false & claEnemType[k] == "H" & enemClassType[enemNum] == "M")))
											possibleclasses.Add(k);
									}
									else
									{
										if ((enemChap[enemNum] != "3c" & enemChap[enemNum].Contains("4") == false
											& enemChap[enemNum].Contains("T") == false) & claName[k].Contains("dragon"))
										// no dragons before 3c
										{ }
										else if (enemChap[enemNum].Contains("T") == false & claName[k].Contains("spirit"))
										// no spirits before tower
										{ }
										else if (enemFile[enemNum].Contains("bmap0107") & enemClassType[enemNum] == "F" & claEnemType[k] != "F")
										// these pegasus knights need to be changed into other flying units
										{ }
										else if ((cbxEnemHealers.Checked == true | enemChap[enemNum] == "1a") & claName[k].Contains("priest") |
											(enemChap[enemNum].Contains("1") & claName[k].Contains("thief")))
										// no priests or thieves on early chapters
										{ }
										else if (cbxNoEnemyLaguz.Checked == true & enemClassType[enemNum] != "L" & claEnemType[k] == "L")
										// beorc enemies can't be laguz with this checkbox
										{ }
										else if (claEnemTier[k].Contains(enemClass[enemNum]))
											possibleclasses.Add(k);
									}
								}
							}
							// no random enemies, only part 4 enemies changing to T3
							else if (cbxRandEnemy.Checked == false & cbxTier3Enemies.Checked == true)
							{
								// change class as long as it is a part 4, T2 unit
								if (enemClass[enemNum] == "3" & enemClassName[enemNum].Substring(enemClassName[enemNum].Length - 2, 2) == "SP")
								{
									string[] t2name = new string[18];
									int[] t3equiv = new int[t2name.Length];
									dataReader = new System.IO.StreamReader(file + "\\assets\\EnemyT3List.csv");

									for (int i = 0; i < t2name.Length; i++)
									{
										line = dataReader.ReadLine();
										values = line.Split(',');
										t2name[i] = values[0];
										t3equiv[i] = Convert.ToInt32(values[1]);
									}
									dataReader.Close();

									// loop through all classes to find one that matches
									for (int i = 0; i < t2name.Length; i++)
									{
										if (t2name[i] == enemClassName[enemNum])
										{
											possibleclasses.Add(t3equiv[i]);
											break;
										}
									}
								}
							}

							if (possibleclasses.Count != 0)
							{
								// choose random class from list of possible
								int newenemclass = possibleclasses.ElementAt(random.Next(possibleclasses.Count));

								// outputlog
								randEnemyOutput += claName[newenemclass] + "</td> </tr>";

								// go to class location
								stream.Position = enemLoc[enemNum];
								// convert to bytes and write
								classbyte = System.Text.Encoding.ASCII.GetBytes(claJID[newenemclass]);
								for (int i = 0; i < 7; i++)
									stream.WriteByte(classbyte[i]);

								// weapons %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

								if (cbxRandEnemy.Checked == true) // don't need to change weapons if it's just t3 enemies
								{
									// go to first weapon location
									stream.Position = Convert.ToInt32(enemWeap1Loc[enemNum]);
									// set up weapon type
									string weaptype;
									if (random.Next(10) < 8 | cbxWeapCaps.Checked == false)
										weaptype = claEnemWeap[newenemclass].Remove(1);
									else
										weaptype = claEnemWeap[newenemclass].Remove(0, 2);

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
										stream.Position = Convert.ToInt32(enemWeap2Loc[enemNum]);

										if (random.Next(10) < 8)
											weaptype = claEnemWeap[newenemclass].Remove(1);
										else
											weaptype = claEnemWeap[newenemclass].Remove(0, 2);

										// pick random weapon
										newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

										// convert to bytes and write
										weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
										for (int i = 0; i < 8; i++)
											stream.WriteByte(weapbyte[i]);
									}
									if (enemWeap3Loc[enemNum] != "")
									{
										stream.Position = Convert.ToInt32(enemWeap3Loc[enemNum]);

										if (random.Next(10) < 8)
											weaptype = claEnemWeap[newenemclass].Remove(1);
										else
											weaptype = claEnemWeap[newenemclass].Remove(0, 2);

										// pick random weapon
										newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

										// convert to bytes and write
										weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
										for (int i = 0; i < 8; i++)
											stream.WriteByte(weapbyte[i]);
									}
									if (enemWeap4Loc[enemNum] != "")
									{
										stream.Position = Convert.ToInt32(enemWeap4Loc[enemNum]);

										if (random.Next(10) < 8)
											weaptype = claEnemWeap[newenemclass].Remove(1);
										else
											weaptype = claEnemWeap[newenemclass].Remove(0, 2);

										// pick random weapon
										newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

										// convert to bytes and write
										weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
										for (int i = 0; i < 8; i++)
											stream.WriteByte(weapbyte[i]);
									}
									if (enemWeap5Loc[enemNum] != "")
									{
										stream.Position = Convert.ToInt32(enemWeap5Loc[enemNum]);

										if (random.Next(10) < 8)
											weaptype = claEnemWeap[newenemclass].Remove(1);
										else
											weaptype = claEnemWeap[newenemclass].Remove(0, 2);

										// pick random weapon
										newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

										// convert to bytes and write
										weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
										for (int i = 0; i < 8; i++)
											stream.WriteByte(weapbyte[i]);
									}
									if (enemWeap6Loc[enemNum] != "")
									{
										stream.Position = Convert.ToInt32(enemWeap6Loc[enemNum]);

										if (random.Next(10) < 8)
											weaptype = claEnemWeap[newenemclass].Remove(1);
										else
											weaptype = claEnemWeap[newenemclass].Remove(0, 2);

										// pick random weapon
										newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], false);

										// convert to bytes and write
										weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
										for (int i = 0; i < 8; i++)
											stream.WriteByte(weapbyte[i]);
									}
								}
							}
							else
							{
								randEnemyOutput += enemClassName[enemNum] + "</td> </tr>";
							}
						}

					}
					else
					{
						randEnemyOutput += enemClassName[enemNum] + "</td> </tr>";
					}
				}
				randEnemyOutput += "</table>";


				// change all enemies to feral ones
				enemyLaguz();

				// give part 4 enemies S rank in laguz
				enemyLaguzRank();
			}
			else if (cbxEnemWeaps.Checked == true)
			{
				// increase difficult of enemy weapons without random classes
				for (int enemNum = 0; enemNum < numberOfEnemies; enemNum++)
				{
					using (var stream = new System.IO.FileStream(dataLocation + enemFile[enemNum], System.IO.FileMode.Open,
								System.IO.FileAccess.ReadWrite))
					{
						// go to first weapon location
						stream.Position = Convert.ToInt32(enemWeap1Loc[enemNum]);

						// pick random weapon
						newweapon = getEnemyWeapon(enemWeapType[enemNum], enemChap[enemNum], false);

						// convert to bytes and write
						weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
						for (int i = 0; i < 8; i++)
							stream.WriteByte(weapbyte[i]);

						if (enemNum > 379)
							stream.WriteByte(0x00);

						// repeat for weapons 2-6
						if (enemWeap2Loc[enemNum] != "")
						{
							stream.Position = Convert.ToInt32(enemWeap2Loc[enemNum]);
							newweapon = getEnemyWeapon(enemWeapType[enemNum], enemChap[enemNum], false);

							// convert to bytes and write
							weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapbyte[i]);
						}
						if (enemWeap3Loc[enemNum] != "")
						{
							stream.Position = Convert.ToInt32(enemWeap3Loc[enemNum]);
							newweapon = getEnemyWeapon(enemWeapType[enemNum], enemChap[enemNum], false);

							// convert to bytes and write
							weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapbyte[i]);
						}
						if (enemWeap4Loc[enemNum] != "")
						{
							stream.Position = Convert.ToInt32(enemWeap4Loc[enemNum]);
							newweapon = getEnemyWeapon(enemWeapType[enemNum], enemChap[enemNum], false);

							// convert to bytes and write
							weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapbyte[i]);
						}
						if (enemWeap5Loc[enemNum] != "")
						{
							stream.Position = Convert.ToInt32(enemWeap5Loc[enemNum]);
							newweapon = getEnemyWeapon(enemWeapType[enemNum], enemChap[enemNum], false);

							// convert to bytes and write
							weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapbyte[i]);
						}
						if (enemWeap6Loc[enemNum] != "")
						{
							stream.Position = Convert.ToInt32(enemWeap6Loc[enemNum]);
							newweapon = getEnemyWeapon(enemWeapType[enemNum], enemChap[enemNum], false);

							// convert to bytes and write
							weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
							for (int i = 0; i < 8; i++)
								stream.WriteByte(weapbyte[i]);
						}
					}
				}
			}


		}

		// gets weapons for new enemy classes
		private string getEnemyWeapon(string weaptype, string chapter, bool boss)
		{
			// weapons
			System.IO.StreamReader dataReader;
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

			if (cbxNoSiege.Checked == true)
			{
				// remove siege options
				fires[4] = fires[3];
				fires[7] = fires[6];
				thunders[4] = thunders[3];
				thunders[7] = thunders[6];
				winds[4] = winds[3];
				winds[7] = winds[6];
				lights[4] = lights[3];
				lights[9] = lights[8];
				darks[4] = darks[3];
			}
			if (cbxWeapPatch.Checked == true)
			{
				// force bronze/venin weapons a little later now that they're different
				swords[1] = swords[2];
				swords[2] = swords[4];
				swords[3] = "IID_BROS";
				swords[4] = swords[5];
				swords[5] = swords[6];
				swords[6] = "IID_POIS";

				lances[1] = lances[2];
				lances[2] = lances[4];
				lances[3] = "IID_BROL";
				lances[4] = lances[5];
				lances[5] = lances[6];
				lances[6] = "IID_POIL";

				axes[0] = axes[1];
				axes[1] = axes[3];
				axes[2] = axes[4];
				axes[3] = "IID_BROA";
				axes[4] = axes[5];
				axes[5] = axes[6];
				axes[6] = "IID_POIA";

				bows[0] = bows[1];
				bows[1] = bows[3];
				bows[2] = "IID_BROB";
				bows[3] = bows[4];
				bows[4] = bows[5];
				bows[5] = "IID_POIB";

				daggers[0] = daggers[2];
				daggers[1] = daggers[3];
				daggers[2] = daggers[4];
				daggers[3] = "IID_BROD";
				daggers[4] = daggers[5];
				daggers[5] = daggers[6];
				daggers[6] = "IID_BROK";
			}

			int min = 0;
			int max = 1;

			if (weaptype == "S") // swords
			{
				if (chapter == "1a")
				{
					min = 0;
					max = 4;
					if (cbxWeapPatch.Checked == true)
						max = 2;
				}
				else if (chapter == "1b")
				{
					min = 3;
					max = 6;
					if (cbxWeapPatch.Checked == true)
					{
						min = 1;
						max = 4;
					}
				}
				else if (chapter == "1c")
				{
					min = 4;
					max = 8;
					if (cbxWeapPatch.Checked == true)
						min = 2;
				}
				else if (chapter == "2a")
				{
					min = 3;
					max = 8;
					if (cbxWeapPatch.Checked == true)
						min = 1;
				}
				else if (chapter == "2b")
				{
					min = 5;
					max = 11;
					if (cbxWeapPatch.Checked == true)
						min = 4;
				}
				else if (chapter == "3a")
				{
					min = 6;
					max = 11;
					if (cbxWeapPatch.Checked == true)
						min = 5;
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
				else if (chapter == "SS")
				{
					min = 15;
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
					max = 4;
					if (cbxWeapPatch.Checked == true)
						max = 2;
				}
				else if (chapter == "1b")
				{
					min = 3;
					max = 7;
					if (cbxWeapPatch.Checked == true)
					{
						min = 1;
						max = 5;
					}
				}
				else if (chapter == "1c")
				{
					min = 5;
					max = 9;
					if (cbxWeapPatch.Checked == true)
						min = 3;
				}
				else if (chapter == "2a")
				{
					min = 3;
					max = 9;
					if (cbxWeapPatch.Checked == true)
						min = 1;
				}
				else if (chapter == "2b")
				{
					min = 5;
					max = 11;
					if (cbxWeapPatch.Checked == true)
						min = 3;
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
				else if (chapter == "SS")
				{
					min = 15;
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
					max = 3;
					if (cbxWeapPatch.Checked == true)
						max = 1;
				}
				else if (chapter == "1b")
				{
					min = 2;
					max = 6;
					if (cbxWeapPatch.Checked == true)
					{
						min = 0;
						max = 5;
					}
				}
				else if (chapter == "1c")
				{
					min = 4;
					max = 8;
					if (cbxWeapPatch.Checked == true)
						min = 2;
				}
				else if (chapter == "2a")
				{
					min = 2;
					max = 8;
					if (cbxWeapPatch.Checked == true)
						min = 0;
				}
				else if (chapter == "2b")
				{
					min = 4;
					max = 10;
					if (cbxWeapPatch.Checked == true)
						min = 2;
				}
				else if (chapter == "3a")
				{
					min = 5;
					max = 10;
					if (cbxWeapPatch.Checked == true)
						min = 4;
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
				else if (chapter == "SS")
				{
					min = 14;
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
					max = 3;
					if (cbxWeapPatch.Checked == true)
						max = 1;
				}
				else if (chapter == "1b")
				{
					min = 1;
					max = 4;
					if (cbxWeapPatch.Checked == true)
					{
						min = 0;
						max = 2;
					}
				}
				else if (chapter == "1c")
				{
					min = 3;
					max = 7;
					if (cbxWeapPatch.Checked == true)
						min = 1;
				}
				else if (chapter == "2a")
				{
					min = 2;
					max = 6;
					if (cbxWeapPatch.Checked == true)
						min = 0;
				}
				else if (chapter == "2b")
				{
					min = 4;
					max = 8;
					if (cbxWeapPatch.Checked == true)
						min = 2;
				}
				else if (chapter == "3a")
				{
					min = 5;
					max = 8;
					if (cbxWeapPatch.Checked == true)
						min = 4;
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
				else if (chapter == "SS")
				{
					min = 12;
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
				else if (chapter == "SS")
				{
					min = 4;
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
					if (cbxWeapPatch.Checked == true)
						max = 1;
				}
				else if (chapter == "1b")
				{
					min = 3;
					max = 6;
					if (cbxWeapPatch.Checked == true)
					{
						min = 1;
						max = 5;
					}
				}
				else if (chapter == "1c")
				{
					min = 4;
					max = 9;
					if (cbxWeapPatch.Checked == true)
						min = 2;
				}
				else if (chapter == "2a")
				{
					min = 3;
					max = 7;
					if (cbxWeapPatch.Checked == true)
						min = 1;
				}
				else if (chapter == "2b")
				{
					min = 4;
					max = 10;
					if (cbxWeapPatch.Checked == true)
						min = 2;
				}
				else if (chapter == "3a")
				{
					min = 5;
					max = 10;
					if (cbxWeapPatch.Checked == true)
						min = 4;
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
				else if (chapter == "SS")
				{
					min = 12;
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
					max = 3;
				}
				else if (chapter == "1c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 3;
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
				else if (chapter == "SS")
				{
					min = 10;
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
					max = 3;
				}
				else if (chapter == "1c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 3;
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
				else if (chapter == "SS")
				{
					min = 10;
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
					max = 3;
				}
				else if (chapter == "1c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 3;
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
				else if (chapter == "SS")
				{
					min = 10;
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
					max = 3;
				}
				else if (chapter == "1c")
				{
					min = 2;
					max = 5;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 3;
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
				else if (chapter == "SS")
				{
					min = 12;
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
				else if (chapter == "SS")
				{
					min = 7;
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
					max = 2;
				}
				else if (chapter == "1c")
				{
					min = 1;
					max = 4;
				}
				else if (chapter == "2a")
				{
					min = 0;
					max = 2;
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
				else if (chapter == "SS")
				{
					min = 8;
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
			int[] location = new int[chapfile.Length];

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
				int itemPointerOffset;
				if (cbxFormshift.Checked == true)
				{
					itemPointerOffset = random.Next(221);
					while (itemPointerOffset > 68 & itemPointerOffset < 73)
						itemPointerOffset = random.Next(221);
					itemPointerOffset *= 20;
				}
				else
				{
					itemPointerOffset = random.Next(220);
					while (itemPointerOffset > 68 & itemPointerOffset < 73)
						itemPointerOffset = random.Next(220);
					itemPointerOffset *= 20;
				}

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

		// lowers stats of all enemies in part 2
		private void lowerPart2Stats()
		{
			// number of total enemy types in csv file
			string[] pidname = new string[323];
			int[] piddataloc = new int[323];
			string line;
			string[] values;
			System.IO.StreamReader reader = new System.IO.StreamReader(file + "\\assets\\PIDdataLoc.csv");
			for (int i = 0; i < piddataloc.Length; i++)
			{
				line = reader.ReadLine();
				values = line.Split(',');
				pidname[i] = values[0];
				piddataloc[i] = Convert.ToInt32(values[1]);
			}
			reader.Close();

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < piddataloc.Length; i++)
					{
						if (pidname[i].Contains("REBELLION"))
						{
							stream.Position = piddataloc[i] + 77;
							for (int j = 0; j < 8; j++)
							{
								int statbyte = stream.ReadByte();
								statbyte -= 3;
								if (statbyte < 0)
									statbyte += 256;
								stream.Position -= 1;
								stream.WriteByte((byte)statbyte);
							}
						}
					}

				}
			}
			catch
			{
				textBox1.Text = "Error in part 2 stat lowering: Cannot find file \\FE10Data.cms.decompressed!";
				errorflag = 1;
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

			int[] location = new int[130];
			int[] pointer = new int[130];
			int[] pointer2 = new int[pointer.Length];
			string line;
			string[] values;
			System.IO.StreamReader reader = new System.IO.StreamReader(file + "\\assets\\enemyAnimations.csv");
			for (int i = 0; i < pointer.Length; i++)
			{
				line = reader.ReadLine();
				values = line.Split(',');
				location[i] = Convert.ToInt32(values[0]);
				//pointer[i] = Convert.ToInt32(values[2]);
				//pointer2[i] = Convert.ToInt32(values[3]);
			}
			reader.Close();

			using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
			{
				for (int i = 0; i < location.Length; i++)
				{
					// zero out animation
					stream.Position = location[i];
					for (int j = 0; j < 16; j++)
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
						stream.WriteByte(0x9C);
						stream.WriteByte(0xB0);
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
								randGrowth = enemyGrowth + (Convert.ToInt32(numericEnemyGrowth.Value) / 3);

							
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

		// gives enemies/bosses a number of random skills
		private void enemySkillRandom()
		{
			// load skill list
			// input skill pointers
			string[] pointerList = System.IO.File.ReadAllLines(file +
				"\\assets\\enemySkillList.txt");
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

			// start of bosses in csv file
			int boss_start = 199;
			// number of total enemy types in csv file
			int[] skillloc = new int[234];
			string line;
			string[] values;
			System.IO.StreamReader reader = new System.IO.StreamReader(file + "\\assets\\EnemySkillLoc.csv");
			for (int i = 0; i < skillloc.Length; i++)
			{
				line = reader.ReadLine();
				values = line.Split(',');
				skillloc[i] = Convert.ToInt32(values[1]);
			}
			reader.Close();

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int charNum = 0; charNum < skillloc.Length; charNum++)
					{
						stream.Position = skillloc[charNum];
						// skip "sid_boss" skill
						if (charNum >= boss_start)
							stream.Position += 4;
						// keep track of number of skills already added
						int i = 0;
						int maxnum = Convert.ToInt32(numericEnemySkills.Value);
						if (cbxEnemySkills.Checked == false)
							maxnum = 0;
						if (cbxBossSkills.Checked == true & maxnum < 3 & charNum >= boss_start)
							maxnum += 1;
						while (i < maxnum)
						{
							randSkill = random.Next(skillName.Length);
							// write output
							stream.WriteByte(0);
							stream.WriteByte(Convert.ToByte(firstByte[randSkill]));
							stream.WriteByte(Convert.ToByte(secondByte[randSkill]));
							stream.WriteByte(Convert.ToByte(thirdByte[randSkill]));

							// keep track of number of skills added
							i += 1;
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Error in Enemy Skills: Cannot find file \\FE10Data.cms.decompressed!";
				errorflag = 1;
			}
		}

		// randomizes classes/weapons for bosses
		private void bossRando()

		{
			string[] enemFile = new string[34];
			string[] enemChap = new string[enemFile.Length];
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


			string newclass = "";
			string newweapon;
			int randbossclass = 0;
			byte[] classbyte = new byte[7];
			byte[] weapbyte = new byte[8];

			int numberOfEnemies = enemFile.Length;
			int startpoint = 0;
			if (cbxRandBosses.Checked == false)
				startpoint = 23;

			// negate spirits
			claEnemTier[125] = "x";
			claEnemTier[126] = "x";
			claEnemTier[127] = "x";

			// let's randomize
			for (int enemNum = startpoint; enemNum < numberOfEnemies; enemNum++)
			{
				
				using (var stream = new System.IO.FileStream(dataLocation + enemFile[enemNum], System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// pick random class
					if (cbxRandBosses.Checked == true)
					{
						List<int> possibleclasses = new List<int>();

						// look for t3 classes (marked with 4) instead of SP (marked with 3)
						if (cbxTier3Enemies.Checked == true & enemClass[enemNum] == "3")
							enemClass[enemNum] = "4";

						if (enemChap[enemNum] != "1c") // jarod keeps class from previous chapter
						{
							for (int k = 0; k < claName.Length; k++)
							{
								if (claEnemTier[k].Contains("b") == false & claEnemType[k] == "L")
								// no non-royal laguz as bosses
								{ }
								else if (claEnemTier[k].Contains(enemClass[enemNum]) | (claEnemTier[k].Contains("b") & enemClass[enemNum] == "4")
											& claName[k].Contains("priest") == false)
									possibleclasses.Add(k);
							}
							randbossclass = possibleclasses.ElementAt(random.Next(possibleclasses.Count));
							newclass = claJID[randbossclass];
						}						
					}
					else // only random recruitment
					{
						randbossclass = newClass[recrNum[enemNum]];
						newclass = claJID[randbossclass];
					}

					// go to class location
					stream.Position = enemLoc[enemNum];

					// convert to bytes and write
					classbyte = System.Text.Encoding.ASCII.GetBytes(newclass);
					for (int i = 0; i < 7; i++)
						stream.WriteByte(classbyte[i]);



					// go to first weapon location
					stream.Position = Convert.ToInt32(enemWeap1Loc[enemNum]);
					// set up weapon type
					string weaptype;
					if (random.Next(10) < 8)
						weaptype = claEnemWeap[randbossclass].Remove(1);
					else
						weaptype = claEnemWeap[randbossclass].Remove(0, 2);

					// pick random weapon
					// izuka, lekain, and levail get guaranteed SS ranks
					if (enemName[enemNum] == "Izuka" | enemName[enemNum] == "Lekain" | enemName[enemNum] == "Levail")
						newweapon = getEnemyWeapon(weaptype, "SS", true);
					else
						newweapon = getEnemyWeapon(weaptype, enemChap[enemNum], true);

					// convert to bytes and write
					weapbyte = System.Text.Encoding.ASCII.GetBytes(newweapon);
					for (int i = 0; i < 8; i++)
						stream.WriteByte(weapbyte[i]);

					// repeat for weapon 2
					if (enemWeap2Loc[enemNum] != "")
					{
						stream.Position = Convert.ToInt32(enemWeap2Loc[enemNum]);

						if (random.Next(10) < 8)
							weaptype = claEnemWeap[randbossclass].Remove(1);
						else
							weaptype = claEnemWeap[randbossclass].Remove(0, 2);

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

			int[] location = new int[34];
			int[] pointer = new int[34];
			int[] pointer2 = new int[pointer.Length];
			string line;
			string[] values;
			System.IO.StreamReader reader = new System.IO.StreamReader(file + "\\assets\\bossAnimations.csv");
			for (int i = 0; i < pointer.Length; i++)
			{
				line = reader.ReadLine();
				values = line.Split(',');
				location[i] = Convert.ToInt32(values[0]);
				//pointer[i] = Convert.ToInt32(values[2]);
				//pointer2[i] = Convert.ToInt32(values[3]);
			}
			reader.Close();

			int startpoint = 0;//14;
			//if (cbxRandBosses.Checked == true)
			//	startpoint = 0;

			using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
			{
				for (int i = startpoint; i < location.Length; i++)
				{
					// zero out animation
					stream.Position = location[i];
					for (int j = 0; j < 16; j++)
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

		// changes laguz AI in chapter 3-6 as well as chapter 3-13
		private void laguzEnemyAI()
		{
			using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0307\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
			{
				string AIchanges = "SEQ_ALLUNITATTACK100";
				byte[] AIbytes = System.Text.Encoding.ASCII.GetBytes(AIchanges);

				int[] AIlocations = { 7600, 7668, 7692};
				for (int i = 0; i < AIlocations.Length; i++)
				{
					stream.Position = AIlocations[i];
					for (int j = 0; j < AIbytes.Length; j++)
						stream.WriteByte(AIbytes[j]);

					stream.WriteByte(0x00);
				}
			}
			using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0314\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
			{
				string AIchanges = "SEQ_ALLUNITATTACK100";
				byte[] AIbytes = System.Text.Encoding.ASCII.GetBytes(AIchanges);

				int AIlocations = 14358;
				stream.Position = AIlocations;
				for (int j = 0; j < AIbytes.Length; j++)
					stream.WriteByte(AIbytes[j]);

				stream.WriteByte(0x00);

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

			if (cbxAuthority.Checked == true & errorflag == 0)
				authorityStarRando();
			
			if ((cbxRandShop.Checked == true | cbxBargains.Checked == true) & errorflag == 0)
				shopRandomizer();
			
			if (cbxForge.Checked == true & errorflag == 0)
				forgeRandom();

			if (cbxBio.Checked == true & errorflag == 0)
				bioRandomizer();

			if (cbxEventItems.Checked == true & errorflag == 0)
				eventItemRandomizer();

			if (cbxRandMove.Checked == true & errorflag == 0)
				moveRandom();
		}

		// randomizes skills for all playable characters
		private void skillRandomizer()
		{
			// load skill list
			// input skill pointers
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
						int numskills = 0;
						for (int i = 0; i < 4; i++)
						{
							// set position to skill data
							stream.Position = charSkill[charNum] + (4 * i);
							if ((cbxSkillUno.Checked == false & (charSkillNum[charNum][i] == 'A' | // not one per character, "A"lways change
															(charSkillNum[charNum][i] == 'S' & cbxSkillMax.Checked == true))) | // not one per character, "S"ometimes change (only with max skills)
								cbxSkillUno.Checked == true & charSkillNum[charNum][i] != 'N' & numskills == 0) // only one per character, only change if first non-"N"ever skill
							{
								if (cbxRandRecr.Checked == true)
								{
									if ((cbxClassRand.Checked == true & newRace[charNum] == "B") |
										(cbxClassRand.Checked == false & recrRace[charNum] == "B"))
										randSkill = random.Next(skillName.Length - 2); // beorc can't get wildheart/formshift, last skill on list
									else
									{
										if (cbxFormshift.Checked == true & charTier[charNum] != "c")
											randSkill = random.Next(skillName.Length);
										else
											randSkill = random.Next(skillName.Length - 1); // can't get formshift
									}
								}
								else
								{
									if ((cbxClassRand.Checked == true & newRace[charNum] == "B") |
										(cbxClassRand.Checked == false & charRace[charNum] == "B"))
										randSkill = random.Next(skillName.Length - 2); // beorc can't get wildheart/formshift, last skill on list
									else
									{
										if (cbxFormshift.Checked == true & charTier[charNum] != "c")
											randSkill = random.Next(skillName.Length);
										else
											randSkill = random.Next(skillName.Length - 1); // can't get formshift
									}
								}

								numskills += 1;

								// write output
								stream.WriteByte(0);
								stream.WriteByte(Convert.ToByte(firstByte[randSkill]));
								stream.WriteByte(Convert.ToByte(secondByte[randSkill]));
								stream.WriteByte(Convert.ToByte(thirdByte[randSkill]));

								newSkills[charNum, i] = skillName[randSkill];
							}
							else if (cbxSkillUno.Checked == true & charSkillNum[charNum][i] == 'A')
							{
								// replace normal skills past the first one with treasure hunt
								byte[] thuntskill = { 0x00, 0x03, 0x58, 0x50 };
								stream.Write(thuntskill, 0, 4);
							}
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

		// authority star randomization. Not much to say here
		private void authorityStarRando()
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
						stream.Position = charBio[charNum] + 4;

						// authority 0-5
						stream.WriteByte(Convert.ToByte(random.Next(6)));
					}
				}
			}
			catch
			{
				textBox1.Text = "Error in Authority Star changes. Randomization failed.";
				errorflag = 1;
			}
		}

		// randomizes items found in each chapter's shop
		private void shopRandomizer()
		{
			string line;
			string[] values;
			int[] highBytes;
			if (cbxFormshift.Checked == true)
				highBytes = new int[221];
			else
				highBytes = new int[220];
			int[] lowBytes = new int[highBytes.Length];
			string[] itemtypes = new string[highBytes.Length];
			string[] itemnames = new string[highBytes.Length];
			int armoryNum = 116;

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ShopStuff.csv");

			// loop through items in shop file
			for (int i = 0; i < highBytes.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				lowBytes[i] = Convert.ToInt32(values[1]);
				highBytes[i] = Convert.ToInt32(values[0]);
				itemtypes[i] = values[4];
				itemnames[i] = values[3];
			}

			// create weighted array
			List<int> lowList = new List<int>();
			List<int> highList = new List<int>();

			for (int i = 10; i < itemtypes.Length; i++) // first ten are iron/basic weapons
			{
				// prevent bronze weapons as well
				if (itemtypes[i] == "s" & itemnames[i].Contains("BRONZE") == false)
				{
					for (int j = 0; j < numericBargSword.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (itemtypes[i] == "l" & itemnames[i].Contains("BRONZE") == false)
				{
					for (int j = 0; j < numericBargLance.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (itemtypes[i] == "a" & itemnames[i].Contains("BRONZE") == false)
				{
					for (int j = 0; j < numericBargAxe.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (itemtypes[i] == "k" & itemnames[i].Contains("BRONZE") == false)
				{
					for (int j = 0; j < numericBargKnife.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (itemtypes[i] == "b" & itemnames[i].Contains("BRONZE") == false)
				{
					for (int j = 0; j < numericBargBow.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (itemtypes[i] == "t")
				{
					for (int j = 0; j < numericBargTome.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (itemtypes[i] == "h")
				{
					for (int j = 0; j < numericBargStave.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (itemtypes[i] == "i")
				{
					for (int j = 0; j < numericBargItem.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (itemtypes[i] == "p")
				{
					for (int j = 0; j < numericBargStat.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (itemtypes[i] == "x")
				{
					for (int j = 0; j < numericBargSkill.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
			}
			int[] barghighbytes, barglowbytes;
			if (lowList.Count != 0)
			{
				barghighbytes = highList.ToArray();
				barglowbytes = lowList.ToArray();
			}
			else
			{
				barghighbytes = highBytes;
				barglowbytes = lowBytes;
			}

			int itemSelection;
			try
			{
				using (var stream = new System.IO.FileStream(dataLocation + "\\Shop\\shopitem_h.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					if (cbxRandShop.Checked == true) // only randomize armory if shop rando is selected
					{
						// go to beginning of armory pointers
						stream.Position = 230;
						int ironweap = 0;
						while (stream.Position < 9104) // 9130 is start of shop pointers
						{
							if (cbxIronShop.Checked == true)
							{
								if (ironweap > 9)
									itemSelection = random.Next(10, armoryNum);
								else // forces iron weapons for first 10 slots
									itemSelection = ironweap;
							}
							else
								itemSelection = random.Next(armoryNum);

							// write new weapon
							stream.WriteByte(Convert.ToByte(highBytes[itemSelection]));
							stream.WriteByte(Convert.ToByte(lowBytes[itemSelection]));
							// go to next weapon pointer
							stream.Position += 6;
							if (stream.ReadByte() != 0)
							{
								ironweap += 1;
								stream.Position -= 1;
							}
							else // next chapter's armory
							{
								ironweap = 0;
								stream.Position += 3;
							}
						}
					}
					// go to beginning of shop pointers
					stream.Position = 9130;
					int healitems = 0;
					// write stuff until you get to the end of the shop pointers
					while (stream.Position < 12260) // 12260 is the end of the shop pointers
					{
						// check to see if it is a bargain bin item
						stream.Position += 2;
						if (stream.ReadByte() == 1) // bargain
						{
							stream.Position -= 3;
							itemSelection = random.Next(barghighbytes.Length);

							// if bargain random is selected, write
							if (cbxBargains.Checked == true)
							{
								stream.WriteByte(Convert.ToByte(barghighbytes[itemSelection]));
								stream.WriteByte(Convert.ToByte(barglowbytes[itemSelection]));
							}
							else
								stream.Position += 2;
						}
						else
						{
							stream.Position -= 3;
							if (cbxIronShop.Checked == true)
							{
								if (healitems > 3)
									itemSelection = random.Next(armoryNum + 7, highBytes.Length);
								else
									itemSelection = healitems + armoryNum;
							}
							else
								itemSelection = random.Next(armoryNum, highBytes.Length);
							// if shop random is selected, write
							if (cbxRandShop.Checked == true)
							{
								stream.WriteByte(Convert.ToByte(highBytes[itemSelection]));
								stream.WriteByte(Convert.ToByte(lowBytes[itemSelection]));
							}
							else
								stream.Position += 2;
						}
						
						// go to next item pointer
						stream.Position += 6;
						if (stream.ReadByte() != 0)
						{
							healitems += 1;
							stream.Position -= 1;
						}
						else // next chapter's shop
						{
							healitems = 0;
							stream.Position += 3;
							// go to next non-zero byte
							while (stream.ReadByte() == 0) { }
							stream.Position = stream.Position - 1;
						}
					}
				}

				using (var stream = new System.IO.FileStream(dataLocation + "\\Shop\\shopitem_m.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					if (cbxRandShop.Checked == true) // only randomize armory if shop rando is selected
					{
						// go to beginning of armory pointers
						stream.Position = 230;
						int ironweap = 0;
						while (stream.Position < 9104) // 9130 is start of shop pointers
						{
							if (cbxIronShop.Checked == true)
							{
								if (ironweap > 9)
									itemSelection = random.Next(10, armoryNum);
								else // forces iron weapons for first 10 slots
									itemSelection = ironweap;
							}
							else
								itemSelection = random.Next(armoryNum);

							// write new weapon
							stream.WriteByte(Convert.ToByte(highBytes[itemSelection]));
							stream.WriteByte(Convert.ToByte(lowBytes[itemSelection]));
							// go to next weapon pointer
							stream.Position += 6;
							if (stream.ReadByte() != 0)
							{
								ironweap += 1;
								stream.Position -= 1;
							}
							else // next chapter's armory
							{
								ironweap = 0;
								stream.Position += 3;
							}
						}
					}
					// go to beginning of shop pointers
					stream.Position = 9130;
					int healitems = 0;
					// write stuff until you get to the end of the shop pointers
					while (stream.Position < 12260) // 12260 is the end of the shop pointers
					{
						// check to see if it is a bargain bin item
						stream.Position += 2;
						if (stream.ReadByte() == 1) // bargain
						{
							stream.Position -= 3;
							itemSelection = random.Next(barghighbytes.Length);

							// if bargain random is selected, write
							if (cbxBargains.Checked == true)
							{
								stream.WriteByte(Convert.ToByte(barghighbytes[itemSelection]));
								stream.WriteByte(Convert.ToByte(barglowbytes[itemSelection]));
							}
							else
								stream.Position += 2;
						}
						else
						{
							stream.Position -= 3;
							if (cbxIronShop.Checked == true)
							{
								if (healitems > 3)
									itemSelection = random.Next(armoryNum + 7, highBytes.Length);
								else
									itemSelection = healitems + armoryNum;
							}
							else
								itemSelection = random.Next(armoryNum, highBytes.Length);
							// if shop random is selected, write
							if (cbxRandShop.Checked == true)
							{
								stream.WriteByte(Convert.ToByte(highBytes[itemSelection]));
								stream.WriteByte(Convert.ToByte(lowBytes[itemSelection]));
							}
							else
								stream.Position += 2;
						}

						// go to next item pointer
						stream.Position += 6;
						if (stream.ReadByte() != 0)
						{
							healitems += 1;
							stream.Position -= 1;
						}
						else // next chapter's shop
						{
							healitems = 0;
							stream.Position += 3;
							// go to next non-zero byte
							while (stream.ReadByte() == 0) { }
							stream.Position = stream.Position - 1;
						}
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
			string[] weaptype = new string[highBytes.Length];

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ForgeStuff.csv");

			// loop through items in forge file
			for (int i = 0; i < highBytes.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				lowBytes[i] = Convert.ToInt32(values[1]);
				highBytes[i] = Convert.ToInt32(values[0]);
				weaptype[i] = values[3];
			}

			// create weighted array
			List<int> lowList = new List<int>();
			List<int> highList = new List<int>();

			for (int i = 0; i < weaptype.Length; i++)
			{
				if (weaptype[i] == "s")
				{
					for (int j = 0; j < numericForgeSword.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (weaptype[i] == "l")
				{
					for (int j = 0; j < numericForgeLance.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (weaptype[i] == "a")
				{
					for (int j = 0; j < numericForgeAxe.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (weaptype[i] == "k")
				{
					for (int j = 0; j < numericForgeKnife.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (weaptype[i] == "b")
				{
					for (int j = 0; j < numericForgeBow.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
				else if (weaptype[i] == "t")
				{
					for (int j = 0; j < numericForgeTome.Value; j++)
					{
						lowList.Add(lowBytes[i]);
						highList.Add(highBytes[i]);
					}
				}
			}
			int[] fullhighbytes, fulllowbytes;
			if (lowList.Count != 0)
			{
				fullhighbytes = highList.ToArray();
				fulllowbytes = lowList.ToArray();
			}
			else
			{
				fullhighbytes = highBytes;
				fulllowbytes = lowBytes;
			}

			int itemSelection;
			try
			{
				using (var stream = new System.IO.FileStream(dataLocation + "\\Shop\\shopitem_h.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// write stuff until you get to the end of the forge pointers
					stream.Position = 13014;
					while (stream.Position < 43246) // 43246 is the end of the forge pointers
					{
						int currentbyte = stream.ReadByte();
						if (currentbyte == 0) // pointer is empty, skip
							stream.Position += 1;
						else // random weapon for pointer
						{
							stream.Position -= 1; 
							itemSelection = random.Next(fullhighbytes.Length);
							stream.WriteByte(Convert.ToByte(fullhighbytes[itemSelection]));
							stream.WriteByte(Convert.ToByte(fulllowbytes[itemSelection]));
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
							itemSelection = random.Next(fullhighbytes.Length);
							stream.WriteByte(Convert.ToByte(fullhighbytes[itemSelection]));
							stream.WriteByte(Convert.ToByte(fulllowbytes[itemSelection]));
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
			string[] itemChapter = new string[133];
			int[] itemLocation = new int[itemChapter.Length];
			string[] itemName = new string[itemChapter.Length];
			string[] chapter4output = new string[itemChapter.Length];

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
				// chapter name for outputlog
				chapter4output[i] = values[3];
			}
			dataReader.Close();

			string[] newitemname = System.IO.File.ReadAllLines(file + "\\assets\\randomItemNames.txt");

			eventItemsOutput = "<table><tr> <th>Chapter</th> <th>Original Item</th> <th>New Item</th> </tr>";

			for (int itemNum = 0; itemNum < itemChapter.Length; itemNum++)
			{
				eventItemsOutput += "<tr> <td>" + chapter4output[itemNum] + "</td> <td>" + itemName[itemNum] + "</td> <td>";
				if (cbxNoRandPromotions.Checked == true & itemName[itemNum].Contains("MASTER"))
				{
					eventItemsOutput += itemName[itemNum] + "</td></tr>";
				}
				else
				{
					byte[] itembytes = new byte[8];

					// choose random item, each is 20 bytes
					int itemPointerOffset;
					if (cbxWhiteGem.Checked == true & itemName[itemNum].Contains("COIN"))
					{
						itemPointerOffset = 215;
					}
					else
					{
						if (cbxFormshift.Checked == true)
							itemPointerOffset = random.Next(221);
						else
							itemPointerOffset = random.Next(220);
					}

					eventItemsOutput += newitemname[itemPointerOffset] + "</td></tr>";

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

			eventItemsOutput += "</table>";
		}

		// randomizes the movement of all classes, then modifies ledge/swamp tile movement
		// to be equal to minimum movement for all classes
		private void moveRandom()
		{
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			int[] moveloc = new int[157];

			string line;
			string[] values;
			
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\classMovement.csv");

			// loop through all classes
			for (int i = 0; i < moveloc.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				moveloc[i] = Convert.ToInt32(values[1]);
			}
			dataReader.Close();

			int movemin = Convert.ToInt32(numericMoveMin.Value);
			int movemax = Convert.ToInt32(numericMoveMax.Value);

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					// loop through classes and change movement
					for (int i = 0; i < moveloc.Length; i++)
					{
						stream.Position = moveloc[i];
						int randmove = random.Next(movemin, movemax + 1);

						stream.WriteByte(Convert.ToByte(randmove));
					}

					// change movement for swamp/ledge to minimum movement
					int[] movecosts = { 92300, 92344, 92872, 92916 };
					for (int i = 0; i < movecosts.Length; i++)
					{
						stream.Position = movecosts[i];
						// loop through all 23 movement types
						for (int j = 0; j < 23; j++)
						{
							int inbyte = stream.ReadByte();
							// if tile is normally passible (not 255), change cost to minimum movement
							if (inbyte != 255 & inbyte > movemin)
							{
								stream.Position -= 1;
								stream.WriteByte(Convert.ToByte(movemin));
							}
						}
					}
					
				}
			}
			catch
			{
				textBox1.Text = "Error in move randomization: Cannot find file \\FE10Data.cms.decompressed.";
				errorflag = 1;
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

			// tormod is tier 3

			if (cbxTormodT3.Checked == true & cbxClassRand.Checked == false & cbxRandRecr.Checked == false)
			{
				using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0108\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
				{
					stream.Position = 9522;
					stream.WriteByte(0x33);
				}
			}

			// modify chapters so game over does not occur when certain characters die
			if ((cbxClassRand.Checked == true | cbxRandRecr.Checked == true | cbxRandEnemy.Checked == true) & errorflag == 0 & cbxIronMan.Checked == false)
			{
				// jill, zihark, tauroneo in 1_6
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0106.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 10572;
						stream.WriteByte(0x00);
						stream.WriteByte(0x00);

						stream.Position = 10620;
						stream.WriteByte(0x00);
						stream.WriteByte(0x00);

						stream.Position = 10668;
						stream.WriteByte(0x00);
						stream.WriteByte(0x00);
					}
				}
				catch
				{
					textBox1.Text = "Files not found! Abandoning randomization...";
					errorflag = 1;
				}
				// fiona in 1_7
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0107.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 13960;
						stream.WriteByte(0x00);
						stream.WriteByte(0x00);
					}
				}
				catch
				{
					textBox1.Text = "Files not found! Abandoning randomization...";
					errorflag = 1;
				}
				// brom, nephenee in 2_2
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0202.cmb", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 8396;
						stream.WriteByte(0x00);
						stream.WriteByte(0x00);

						stream.Position = 8444;
						stream.WriteByte(0x00);
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
						stream.WriteByte(0x0F);
						stream.Position += 1;
						stream.WriteByte(0x0F);
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

						stream.Position = 6448;
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
				List<int> numbers = new List<int>();

				System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\TowerUnits.csv");
				string line;
				string[] values;
				
				for (int i = 0; i < 65; i++)
				{
					line = dataReader.ReadLine();
					values = line.Split(',');

					// don't add kurth/ena
					if (cbxKurthEna.Checked == false & (Convert.ToInt32(values[2]) == 63 | Convert.ToInt32(values[2]) == 64))
					{ }
					// don't add herons
					else if (cbxRandRecr.Checked == true | cbxClassRand.Checked == true)
					{
						if (newClass[Convert.ToInt32(values[2])] == 93 | newClass[Convert.ToInt32(values[2])] == 94 |
							newClass[Convert.ToInt32(values[2])] == 95)
						{ }
						else
						{
							PIDs.Add(values[1]);
							names.Add(values[0]);
							numbers.Add(Convert.ToInt32(values[2]));
						}
					}
					else if (cbxRandRecr.Checked == false & cbxClassRand.Checked == false & (Convert.ToInt32(values[2]) == 69 | 
						Convert.ToInt32(values[2]) == 70 | Convert.ToInt32(values[2]) == 71))
					{ }
					else
					{
						PIDs.Add(values[1]);
						names.Add(values[0]);
						numbers.Add(Convert.ToInt32(values[2]));
					}
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
						if (abbreviateON)
							output = "NIHI";
						byte[] outbytes = System.Text.Encoding.ASCII.GetBytes(output);
						for (int i = 0; i < outbytes.Length; i++)
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
						stream.Position = 8820;
						string output = "NIHIL";
						if (abbreviateON)
							output = "NIHI";
						byte[] outbytes = System.Text.Encoding.ASCII.GetBytes(output);
						for (int i = 0; i < outbytes.Length; i++)
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

			// heather starts as blue unit
			if (cbxHeatherBlue.Checked == true)
			{
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\zmap\\bmap0202\\dispos_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 1733;
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

			// remove gameovers caused by characters other than micaiah/ike dying
			if (cbxIronMan.Checked == true & errorflag == 0)
			{
				string dataFile = dataLocation + "\\Scripts\\";
				string[] scriptfile = new string[62];
				int[] gameoverLoc = new int[scriptfile.Length];

				string line;
				string[] values;

				System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ironmanData.csv");

				// loop through all classes
				for (int i = 0; i < gameoverLoc.Length; i++)
				{
					line = dataReader.ReadLine();
					values = line.Split(',');
					scriptfile[i] = values[0];
					gameoverLoc[i] = Convert.ToInt32(values[1]);
				}
				dataReader.Close();

				for (int i = 0; i < gameoverLoc.Length; i++)
				{
					try
					{
						using (var stream = new System.IO.FileStream(dataFile + scriptfile[i], System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
						{
							stream.Position = gameoverLoc[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
						}
					}
					catch
					{
						textBox1.Text = "Error in IronMan mode: Cannot find script files";
						errorflag = 1;
					}
				}
			}

			fe10dataChanges();

			textChanges();

			if (cbxBonusBEXP.Checked == true & errorflag == 0)
				bonusBonusEXP();
			
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
					if (cbxRandRecr.Checked == true | cbxHerons.Checked == true)
					{
						// changes wildheart usability to restrict heron classes instead of characters

						// overwrite rafiel with princeegret_ra
						stream.Position = 90109;
						stream.WriteByte(0x02);
						stream.WriteByte(0xDA);
						stream.WriteByte(0xB1);
						// overwrite reyson with princeegret
						stream.Position = 90117;
						stream.WriteByte(0x02);
						stream.WriteByte(0xDA);
						stream.WriteByte(0xC1);
						// overwrite leanne with princessegret
						stream.Position = 90125;
						stream.WriteByte(0x02);
						stream.WriteByte(0xDA);
						stream.WriteByte(0xD4);
					}
					if (cbxFlorete.Checked == true)
					{
						// go to florete
						stream.Position = 66067;
						// change ranged type from sword to white breath
						stream.WriteByte(14);
					}
					if (cbxGMweaps.Checked == true)
					{
						// remove locks - ragnell, florete, amiti, cymbeline, ettard
						int[] locks = { 66240, 66108, 66412, 70988, 66176 };
						for (int i = 0; i < locks.Length; i++)
						{
							stream.Position = locks[i];
							stream.WriteByte(0);
							stream.WriteByte(0);
							stream.WriteByte(0);
							stream.WriteByte(0);
						}

						// allow weapons to be sold - florete,ettard,cymbeline
						int[] valuableflag = { 66104, 66328, 70984 };
						for (int i = 0; i < valuableflag.Length; i++)
						{
							stream.Position = valuableflag[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
						}

						// give elincia S rank swords so she can use new amiti
						stream.Position = 177226;
						stream.WriteByte(0x53);

						// replace weapon level
						stream.Position = 66200; // ragnell gets SS
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0x98);
						stream.WriteByte(0x84);

						stream.Position = 66136; // ettard gets S
						stream.WriteByte(0x00);
						stream.WriteByte(0x03);
						stream.WriteByte(0x51);
						stream.WriteByte(0x07);

						stream.Position = 66068; // florete gets A
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0x9E);
						stream.WriteByte(0x8B);

						stream.Position = 66372; // amiti gets S
						stream.WriteByte(0x00);
						stream.WriteByte(0x03);
						stream.WriteByte(0x51);
						stream.WriteByte(0x07);

						stream.Position = 70948; // cymbeline gets SS
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0x98);
						stream.WriteByte(0x84);
					}
					if (cbxDBweaps.Checked == true)
					{
						// remove locks - caladbolg,tarvos,lughnasadh,thani
						int[] locks = { 65784, 68280, 69204, 72288 };
						for (int i = 0; i < 4; i++)
						{
							stream.Position = locks[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
						}

						// allow weapons to be sold - caladbolg,tarvos,lughnasadh,thani
						int[] valuableflag = { 65780, 68276, 69200, 72284, };
						for (int i = 0; i < valuableflag.Length; i++)
						{
							stream.Position = valuableflag[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);
						}

						// replace weapon level
						stream.Position = 72248; // thani gets D
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0xB6);
						stream.WriteByte(0x03);

						stream.Position = 65744; // caladbolg gets B
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0xB4);
						stream.WriteByte(0x28);

						stream.Position = 68240; // tarvos gets B
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0xB4);
						stream.WriteByte(0x28);

						stream.Position = 69160; // lughnasdh gets B
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0xB4);
						stream.WriteByte(0x28);

					}
					if (cbxDragonSkills.Checked == true)
					{
						int[] skills = { 90221, 90229, 90237, 90245, 90277, 90285, 90317, 90325 };
						for (int i = 0; i < skills.Length; i++)
						{
							stream.Position = skills[i];
							stream.WriteByte(0x03);
							stream.WriteByte(0x51);
							stream.WriteByte(0x84);
						}
					}
					if (cbxFireMag.Checked == true)
					{
						// go to druid minimum ranks
						stream.Position = 170559;
						// write D in fire
						stream.WriteByte(0x44);
						// go to druid maximium ranks
						stream.Position = 170572;
						// write A in fire
						stream.WriteByte(0x41);

						// go to summoner minimum ranks
						stream.Position = 170585;
						// write B in fire
						stream.WriteByte(0x42);
						// go to summoner maximium ranks
						stream.Position = 170481;
						// write S in fire
						stream.WriteByte(0x53);
					}
					if (cbxDarkMag.Checked == true)
					{
						// go to light sage minimum ranks
						stream.Position = 170693;
						// write C in dark
						stream.WriteByte(0x43);
						// go to light sage maximum ranks
						stream.Position = 170745;
						// write A in dark
						stream.WriteByte(0x41);

						// go to light priestess minimum ranks
						stream.Position = 170628;
						// write B in dark
						stream.WriteByte(0x42);
						// go to light priestess maximum ranks
						stream.Position = 170446;
						// write S in dark
						stream.WriteByte(0x53);
					}
					if (cbxChestKey.Checked == true)
					{
						stream.Position = 78952;
						stream.WriteByte(0x05);
					}
					if (cbxBirdVision.Checked == true & cbxNoFOW.Checked == false)
					{
						// location of visions for hawk, hawkking, raven, female raven, ravenking, and all untransformed classes
						int[] visions = { 60007, 60143, 60279, 60415, 60551, 60687, 60823, 60959, 61095, 61231 };
						for (int i = 0; i < visions.Length; i++)
						{
							stream.Position = visions[i];
							stream.WriteByte(0x02);
						}
						
					}
					if (cbxNoFOW.Checked == true)
					{
						int[] fowloc = new int[157];

						string line;
						string[] values;

						System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\classMovement.csv");

						// loop through all classes
						for (int i = 0; i < fowloc.Length; i++)
						{
							line = dataReader.ReadLine();
							values = line.Split(',');
							fowloc[i] = Convert.ToInt32(values[1]) + 2;
						}
						dataReader.Close();

						for (int i = 0; i < fowloc.Length; i++)
						{
							stream.Position = fowloc[i];
							stream.WriteByte(0x7F);
						}
					}
					if (cbxSellableItems.Checked == true)
					{
						// remove 'rarity' modifier that prevents selling
						int[] items = { 78720, 78780, 78840, 78904 };
						for (int i = 0; i < items.Length; i++)
						{
							stream.Position = items[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x03);
							stream.WriteByte(0x60);
							stream.WriteByte(0x6D);
						}

						// change price from 30k to 10k for master proof, holy crown, and satori sign
						int[] cost = { 78762, 78822, 78886 };
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
						stream.Position = 83454;
						stream.WriteByte(0x0B);
						stream.WriteByte(0xB8);
					}
					if (cbxWinCon.Checked == true)
					{
						// shows player how to win chapter -> change to defeat boss
						int[] wincons = { 162102, 162134, 162166, //4_1 
										  162282, 162314, 162346, //4_2
										  162822, 162854, 162886, //4_3
										  163362, 163394, 163426, //4_4
										  164982, 165014, 165046};//4_7a
						for (int i = 0; i < wincons.Length; i++)
						{
							stream.Position = wincons[i];
							stream.WriteByte(0x33);
							stream.WriteByte(0xEC);
						}
						// change to seize
						int[] otherwin = { 163902, 163934, 163966 }; //4_5
						for (int j = 0; j < otherwin.Length; j++)
						{
							stream.Position = otherwin[j];
							stream.WriteByte(0x34);
							stream.WriteByte(0x08);
						}
					}
					if (cbxLethality.Checked == true)
					{
						int[] banes = { 47789, 47925};
						for (int i = 0; i < banes.Length; i++)
						{
							stream.Position = banes[i];
							stream.WriteByte(0x03);
							stream.WriteByte(0x52);
							stream.WriteByte(0xC7);
						}
					}
					if (cbxHorseParkour.Checked == true)
					{
						int[] ledgecosts = { 92308, 92352, 92968 };
						for (int i = 0; i < ledgecosts.Length; i++)
						{
							stream.Position = ledgecosts[i];
							if (cbxRandMove.Checked == true & numericMoveMin.Value < 6)
							{
								// change to minimum selected movement
								stream.WriteByte(Convert.ToByte(numericMoveMin.Value));
								stream.WriteByte(Convert.ToByte(numericMoveMin.Value));
							}
							else
							{
								// change to movement cost of 6
								stream.WriteByte(0x06);
								stream.WriteByte(0x06);
							}
						}
					}
					if (cbxWhiteGem.Checked & cbxEventItems.Checked)
					{
						// change value of white gem to 30k when sold
						stream.Position = 79674;
						stream.WriteByte(0xEA);
						stream.WriteByte(0x60);
					}
					if (cbxFormshift.Checked == true)
					{
						// need to write three new pointers for troop scroll to give formshift
							// 1. IID_TROOP in item slot of skill data
						stream.Position = 86764;
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0xD2);
						stream.WriteByte(0x7B);
						// change capacity from 0 to 25
						stream.Position += 2;
						stream.WriteByte(0x19);
						// write number of classtypes/specific pid or jids
						stream.Position += 1;
						stream.WriteByte(0x02);
						stream.WriteByte(0x03);
							// 2. classtypes for skill assignment
						stream.Position += 2;
						stream.WriteByte(0x00);
						stream.WriteByte(0x01);
						stream.WriteByte(0x5F);
						stream.WriteByte(0xC8);
							// 3. specific pid/jid for skill assignment
						stream.WriteByte(0x00);
						stream.WriteByte(0x01);
						stream.WriteByte(0x5F);
						stream.WriteByte(0xD8);

						// next, these pointers need to be added to the large pointer list when the data file gets compressed
						newpointerbytes.Add(0x00);
						newpointerbytes.Add(0x01);
						newpointerbytes.Add(0x52);
						newpointerbytes.Add(0xCC);
						newpointerbytes.Add(0x00);
						newpointerbytes.Add(0x01);
						newpointerbytes.Add(0x52);
						newpointerbytes.Add(0xD8);
						newpointerbytes.Add(0x00);
						newpointerbytes.Add(0x01);
						newpointerbytes.Add(0x52);
						newpointerbytes.Add(0xDC);

						// have to add that to the top of the file
						stream.Position = 3;
						int rollover = 12;
						// add 3*4 to total file size
						while (rollover != 0)
						{
							int filesize = stream.ReadByte();
							filesize += rollover;
							rollover = 0;
							if (filesize > 255)
							{
								filesize -= 256;
								rollover = 1;
							}
							stream.Position -= 1;
							stream.WriteByte(Convert.ToByte(filesize));
							stream.Position -= 2;
						}
						stream.Position = 11;
						rollover = 3;
						// add 3 to number of pointers
						while (rollover != 0)
						{
							int filesize = stream.ReadByte();
							filesize += rollover;
							rollover = 0;
							if (filesize > 255)
							{
								filesize -= 256;
								rollover = 1;
							}
							stream.Position -= 1;
							stream.WriteByte(Convert.ToByte(filesize));
							stream.Position -= 2;
						}

						// abbreviate IID_TROOP to IID_TROO
						stream.Position = 184995;
						stream.WriteByte(0x00);

						// make troop scroll worth 5k instead of 1k
						stream.Position = 83210;
						stream.WriteByte(0x13);
						stream.WriteByte(0x88);

						// since formshift now has 25 capacity, let's raise each laguz royal's capacity by 25
						int[] capacities = { 58374, 58510, 59734, 59870, 60278, 60414, 61094, 61230, 62998, 63134};
						for (int i = 0; i < capacities.Length; i++)
						{
							stream.Position = capacities[i];
							int capbyte = stream.ReadByte();
							capbyte += 25;
							stream.Position -= 1;
							stream.WriteByte(Convert.ToByte(capbyte));
						}
					}
					if (cbxParagon.Checked == true)
					{
						for (int i = 0; i < charSkill.Length; i++)
						{
							int paragonpointer = 218938;
							// go to last skill
							stream.Position = charSkill[i] + 12;
							byte[] paragonbytes = int2bytes(paragonpointer);
							stream.Write(paragonbytes, 0, 4);
						}
					}
					if (cbxLaguzParagon.Checked == true)
					{
						for (int i = 0; i < charSkill.Length; i++)
						{
							if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
							{
								if (newRace[i] == "L")
								{
									int paragonpointer = 218938;
									// go to last skill
									stream.Position = charSkill[i] + 12;
									byte[] paragonbytes = int2bytes(paragonpointer);
									stream.Write(paragonbytes, 0, 4);
								}
							}
							else
							{
								if (charRace[i] == "L")
								{
									int paragonpointer = 218938;
									// go to last skill
									stream.Position = charSkill[i] + 12;
									byte[] paragonbytes = int2bytes(paragonpointer);
									stream.Write(paragonbytes, 0, 4);
								}
							}
						}
					}
					if (cbxStatBooster.Checked == true)
					{
						int[] statboost = new int[8];
						int[] usetext = new int[statboost.Length];
						int[] desctext = new int[statboost.Length];

						string line;
						string[] values;

						System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\StatBoosters.csv");

						// skip new line
						line = dataReader.ReadLine();
						// loop through all classes
						for (int i = 0; i < statboost.Length; i++)
						{
							line = dataReader.ReadLine();
							values = line.Split(',');
							statboost[i] = Convert.ToInt32(values[1]);
							if (gameVersion != 2)
							{
								usetext[i] = Convert.ToInt32(values[2]);
								desctext[i] = Convert.ToInt32(values[4]);
							}
							else
							{
								usetext[i] = Convert.ToInt32(values[3]);
								desctext[i] = Convert.ToInt32(values[5]);
							}
						}
						dataReader.Close();

						// skip if both values are zero
						if (numStatBoostMax.Value != 0 | numStatBoostMin.Value != 0)
						{

							for (int i = 0; i < statboost.Length; i++)
							{
								int[] statchanges = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
								if (cbxStatBoostMult.Checked == false)
								{
									// choose a random stat
									int statchoice = random.Next(8);
									// choose a stat change
									statchanges[statchoice] = random.Next((int)numStatBoostMin.Value, (int)numStatBoostMax.Value + 1);
								}
								else
								{
									// choose number of random stats
									int numstats = random.Next(1, 4);
									// set stat changes
									for (int j = 0; j < numstats; j++)
									{
										statchanges[random.Next(8)] += random.Next((int)numStatBoostMin.Value, (int)numStatBoostMax.Value + 1);
									}
								}
								// hp has higher increase/decrease
								if (statchanges[0] > 0)
									statchanges[0] *= 3;
								else if (statchanges[0] < 0)
									statchanges[0] *= 2;
								// coerce to byte
								for (int j = 0; j < 8; j++)
									if (statchanges[j] < 0)
										statchanges[j] += 256;
								// go to HP location
								stream.Position = statboost[i];
								// write
								for (int j = 0; j < 8; j++)
									stream.WriteByte((byte)statchanges[j]);
							}
							
						}

					}
					if (cbxLaguzCanto.Checked == true)
					{
						//JID_LION,JID_LION_GI,JID_LION_CA,JID_TIGER,JID_CAT,JID_CAT_F,JID_WOLF,JID_WOLF_F
						int[] beastlaguz = { 57996, 58268, 58540, 58812, 59084, 59356, 59628, 59900 };
						byte[] cantopointer = { 0x00, 0x03, 0x53, 0x4B};
						for (int i = 0; i < beastlaguz.Length; i++)
						{
							stream.Position = beastlaguz[i];
							stream.Write(cantopointer, 0, 4);
							// raise skill cap by 10
							stream.Position -= 34;
							int skillcap = stream.ReadByte();
							stream.Position -= 1;
							stream.WriteByte((byte)(skillcap + 10));
							// raise skill cap of untransformed, too
							stream.Position -= 137;
							skillcap = stream.ReadByte();
							stream.Position -= 1;
							stream.WriteByte((byte)(skillcap + 10));
						}
					}
					if (cbxDragonCanto.Checked == true)
					{
						//JID_JID_REDDRAGON,JID_REDDRAGON_F,JID_WHITEDRAGON,JID_BLACKDRAGON,JID_BLACKDRAGON_KU
						int[] draglaguz = { 62348, 62620, 62892, 63164, 63436 };
						byte[] cantopointer = { 0x00, 0x03, 0x53, 0x4B };
						for (int i = 0; i < draglaguz.Length; i++)
						{
							stream.Position = draglaguz[i];
							stream.Write(cantopointer, 0, 4);
							// raise skill cap by 10
							stream.Position -= 34;
							int skillcap = stream.ReadByte();
							stream.Position -= 1;
							stream.WriteByte((byte)(skillcap + 10));
							// raise skill cap of untransformed, too
							stream.Position -= 137;
							skillcap = stream.ReadByte();
							stream.Position -= 1;
							stream.WriteByte((byte)(skillcap + 10));
						}
					}
					if (cbxSkillCap.Checked == true)
					{
						int[] skilllocs = new int[71];
						string[] skillname = new string[skilllocs.Length];

						string line;
						string[] values;

						System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\SkillLoc.csv");

						// loop through all classes
						for (int i = 0; i < skilllocs.Length; i++)
						{
							line = dataReader.ReadLine();
							values = line.Split(',');
							skillname[i] = values[0];
							skilllocs[i] = Convert.ToInt32(values[1]);
						}
						dataReader.Close();

						for (int j = 0; j < skilllocs.Length; j++)
						{
							stream.Position = skilllocs[j] + 30;
							int capacity = stream.ReadByte();
							if (capacity != 0)
							{
								// chance of changing by multiples of 5
								// if skill is canto or shove, do not allow capacity to increase to prevent a softlock
								int changechance = random.Next(100);
								if (changechance < 15) // 15% no change
								{ }
								else if (changechance < 38) // 23% -5
									capacity -= 5;
								else if (changechance < 60 & skillname[j] != "CANTO" & skillname[j] != "SHOVE") // 22% +5
									capacity += 5;
								else if (changechance < 75) // 15% -10
									capacity -= 10;
								else if (changechance < 90 & skillname[j] != "CANTO" & skillname[j] != "SHOVE") // 15% +10
									capacity += 10;
								else if (changechance < 95) // 5% -15
									capacity -= 15;
								else if (skillname[j] != "CANTO" & skillname[j] != "SHOVE") // 5% +15
									capacity += 15;
								// coerce
								if (capacity < 0)
									capacity = 0;
								// write
								stream.Position -= 1;
								stream.WriteByte((byte)capacity);
							}
						}
					}
					if (cbxRandShop.Checked == true | cbxBargains.Checked == true)
					{
						// give cymbeline a cost
						stream.Position = 70967;
						stream.WriteByte(0xC8);
					}

					if (cbxSkillRand.Checked == true)
					{
						// make blessing only cost 5 capacity for herons
						stream.Position = 88090;
						stream.WriteByte(0x05);
					}
					if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true | cbxRandShop.Checked == true | cbxEventItems.Checked == true | cbxGMweaps.Checked == true)
					{
						// amiti / ragnell / staves / rudol gem are not locked to unit
						int[] weapons = { 66248, 66420, 77312, 77436, 77568, 83592 };
						for (int i = 0; i < weapons.Length; i++)
						{
							stream.Position = weapons[i];
							for (int j = 0; j < 4; j++)
								stream.WriteByte(0x00);
						}

						// ballistae have price of 1500gp
						int[] ballista = { 69774, 69706 };
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
						int[] tiles = { 92608, 92652 };
						for (int i = 0; i < tiles.Length; i++)
						{
							stream.Position = tiles[i];
							for (int j = 0; j < 23; j++)
								stream.WriteByte(0x01);
						}
					}

					if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true | cbxStoryPromo.Checked == true)
					{
						// ike / micaiah / sothe story promotions
						int[] promotions = { 42200, 47504, 48860, 49680 };
						for (int i = 0; i < promotions.Length; i++)
						{
							stream.Position = promotions[i];
							for (int j = 0; j < 4; j++)
								stream.WriteByte(0x00);
						}
					}

					if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
					{
						
						// locktouch on thief,rogue,whisper
						int[] classes = { 47372, 47516, 47784};
						for (int i = 0; i < classes.Length; i++)
						{
							stream.Position = classes[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x03);
							stream.WriteByte(0x56);
							stream.WriteByte(0x59);
						}
						// treasurehunt on sothe, heather
						int[] units = { 2000, 5264};
						for (int i = 0; i < units.Length; i++)
						{
							stream.Position = units[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x03);
							stream.WriteByte(0x58);
							stream.WriteByte(0x50);
						}
						// lehran,stephan SS rank in all weapons
						int[] bigbois = { 3340, 844};
						for (int i = 0; i < bigbois.Length; i++)
						{
							stream.Position = bigbois[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x02);
							stream.WriteByte(0x98);
							stream.WriteByte(0x86);
						}
						// changes laguz skills from pid based to jid based
						int[] birdskills = { 89956, 89964, 90060, 90068 };
						int[] heronskills = { 90148, 90156, 90164 };
						for (int i = 0; i < birdskills.Length; i++)
						{
							stream.Position = birdskills[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x02);
							stream.WriteByte(0xD9);
							if (i % 2 == 0)
								stream.WriteByte(0x9E);
							else
								stream.WriteByte(0x91);

						}
						for (int i = 0; i < heronskills.Length; i++)
						{
							stream.Position = heronskills[i];
							stream.WriteByte(0x00);
							stream.WriteByte(0x02);
							stream.WriteByte(0xDA);
							if (i == 0)
								stream.WriteByte(0xB1);
							else if (i == 1)
								stream.WriteByte(0xC1);
							else
								stream.WriteByte(0xD4);
						}
						if (cbxDragonSkills.Checked == false)
						{
							// changes night tide from pid based to jid based
							stream.Position = 90317;
							stream.WriteByte(0x02);
							stream.WriteByte(0xD6);
							stream.WriteByte(0xA9);
							stream.Position = 90325;
							stream.WriteByte(0x02);
							stream.WriteByte(0xD7);
							stream.WriteByte(0x14);
						}
					}


					if (aprilFools)
					{
						for (int k = 0; k < 69; k++)
						{
							byte[] mpid = { 0x00, 0x03, 0x20, 0x68 };
							byte[] mnpid = { 0x00, 0x03, 0x18, 0x1c };
							// change mpids and mnpid to EDDIE (replaced with camilla)
							stream.Position = charPID[k] + 4;
							stream.Write(mpid, 0, 4);
							stream.Write(mnpid, 0, 4);
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

		// changes text when necessary
		private void textChanges()
		{
			if (cbxFormshift.Checked == true)
			{
				using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_common.m", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					string scrollname = "Royal Scroll";
					string description = "Allows you to acquire Formshift at base";

					if (gameVersion == 2) // PAL iso
						stream.Position = 9892;
					else
						stream.Position = 9856;
					byte[] stringbytes = System.Text.Encoding.ASCII.GetBytes(scrollname);
					stream.Write(stringbytes, 0, scrollname.Length);
					stream.WriteByte(0x00);

					if (gameVersion == 2) // PAL iso
						stream.Position = 61760;
					else
						stream.Position = 58440;
					byte[] descripbyte = System.Text.Encoding.ASCII.GetBytes(description);
					stream.Write(descripbyte, 0, description.Length);

				}
			}

			if (cbxStatBooster.Checked == true)
			{
				int[] statboost = new int[8];
				int[] usetext = new int[statboost.Length];
				int[] desctext = new int[statboost.Length];

				string line;
				string[] values;

				System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\StatBoosters.csv");

				// skip new line
				line = dataReader.ReadLine();
				// loop through all classes
				for (int i = 0; i < statboost.Length; i++)
				{
					line = dataReader.ReadLine();
					values = line.Split(',');
					statboost[i] = Convert.ToInt32(values[1]);
					if (gameVersion != 2)
					{
						usetext[i] = Convert.ToInt32(values[2]);
						desctext[i] = Convert.ToInt32(values[4]);
					}
					else
					{
						usetext[i] = Convert.ToInt32(values[3]);
						desctext[i] = Convert.ToInt32(values[5]);
					}
				}
				dataReader.Close();

				string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

				for (int i = 0; i < statboost.Length; i++)
				{
					int[] statchanges = new int[8];
					// open data file
					using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						// read in stat changes
						stream.Position = statboost[i];
						for (int j = 0; j < statchanges.Length; j++)
							statchanges[j] = stream.ReadByte();
					}
					using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_common.m", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						// modify use text
						string outstring = "Stats changed.";
						byte[] outbytes = System.Text.Encoding.ASCII.GetBytes(outstring);
						stream.Position = usetext[i];
						stream.Write(outbytes, 0, outbytes.Length);
						while (stream.ReadByte() != 0x00)
						{
							stream.Position -= 1;
							stream.WriteByte(0x00);
						}
						// modify description text based off of stat changes
						outstring = "";
						string[] statnames = new string[10] { "HP", "STR", "MAG", "SKL", "SPD", "LCK", "DEF", "RES", "MOV", "CON" };
						for (int j = 0; j < statchanges.Length; j++)
						{
							if (statchanges[j] != 0)
							{
								if (outstring != "")
									outstring += "; ";
								outstring += statnames[j];
								if (statchanges[j] <= 127)
								{
									outstring += " +";
									outstring += statchanges[j].ToString();
								}
								// negative
								if (statchanges[j] > 127)
								{
									statchanges[j] -= 256;
									outstring += " ";
									outstring += statchanges[j].ToString();
								}
							}
						}
						byte[] outbytes2 = System.Text.Encoding.ASCII.GetBytes(outstring);
						stream.Position = desctext[i];
						stream.Write(outbytes2, 0, outbytes2.Length);
						while (stream.ReadByte() != 0x00)
						{
							stream.Position -= 1;
							stream.WriteByte(0x00);
						}
					}
				}
			}

			if (aprilFools)
			{
				using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_common.m", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					string camilla = "Camilla";
					string desc_camilla = "She's your sister, pervert.";
					if (gameVersion == 2) // PAL iso
						stream.Position = 3508;
					else
						stream.Position = 3472;
					byte[] stringbytes = System.Text.Encoding.ASCII.GetBytes(camilla);
					stream.Write(stringbytes, 0, camilla.Length);
					stream.WriteByte(0x00);

					string bonk = "Bonk";
					if (gameVersion == 2) // PAL iso
						stream.Position = 8728;
					else
						stream.Position = 8684;
					byte[] descripbyte = System.Text.Encoding.ASCII.GetBytes(bonk);
					while (stream.ReadByte() != 0x4A)
					{
						stream.Position -= 1;
						stream.Write(descripbyte, 0, bonk.Length);
						stream.WriteByte(0x00);
						stream.Position += 3;
					}

					string police = "Horny Police";
					if (gameVersion == 2) // PAL iso
						stream.Position = 7020;
					else
						stream.Position = 6984;
					byte[] classbytes = System.Text.Encoding.ASCII.GetBytes(police);
					stream.Write(classbytes, 0, police.Length);
					stream.WriteByte(0x00);

				}
			}
		}

		// gives extra BEXP at the end of levels
		private void bonusBonusEXP()
		{
			string line;
			string[] values;
			int[] bexploc = new int[41];
			string[] scriptfiles = new string[bexploc.Length];

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\bexpdata.csv");
			// skip header line
			line = dataReader.ReadLine();

			for (int i = 0; i < bexploc.Length; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				scriptfiles[i] = values[0];
				bexploc[i] = Convert.ToInt32(values[2]);
			}
			dataReader.Close();

			for (int i = 0; i < bexploc.Length; i++)
			{
				string dataFile = dataLocation + "\\Scripts\\" + scriptfiles[i];
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					stream.Position = bexploc[i];
					// normal/easy mode
					stream.Position -= 3;
					if (stream.ReadByte() == 0x19)
					{
						// each value is only one byte long
						stream.Position -= 2;
						stream.WriteByte(0x7F);
						stream.Position += 1;
						stream.WriteByte(0x7F);
					}
					else
					{
						// each value is 2 bytes long
						stream.Position -= 4;
						stream.WriteByte(0x7F);
						stream.WriteByte(0xFF);
						stream.Position += 1;
						stream.WriteByte(0x7F);
						stream.WriteByte(0xFF);
					}
					
					stream.Position = bexploc[i];
					// hard mode
					stream.Position += 4;
					if (stream.ReadByte() == 0x19)
					{
						// each value is only one byte long
						stream.WriteByte(0x7F);
						stream.Position += 1;
						stream.WriteByte(0x7F);
					}
					else
					{
						// each value is 2 bytes long
						stream.WriteByte(0x7F);
						stream.WriteByte(0xFF);
						stream.Position += 1;
						stream.WriteByte(0x7F);
						stream.WriteByte(0xFF);
					}
				}
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
						stream.WriteByte(0x00);
						stream.WriteByte(0x02);
						stream.WriteByte(0x98);
						stream.WriteByte(0x86);
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
			
			List<byte> fullfilebytes = new List<byte>();

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			// open data file
			using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
			{
				stream.Position = 0;
				for (int i = 0; i < stream.Length; i++)
				{
					if (i == 264012 & newpointerbytes != null & newpointerbytes.Count != 0)
					{
						for (int j = 0; j < newpointerbytes.Count; j++)
							fullfilebytes.Add(newpointerbytes.ElementAt(j));
					}
					fullfilebytes.Add((byte)stream.ReadByte());	
				}
			}
			byte[] entirefreakingfile = fullfilebytes.ToArray();
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

			// add hyperlinks
			if (cbxTowerUnits.Checked == true)
				outlogtext += "<a href=\"#tower\">Tower Units</a>";
			if (cbxRandWeap.Checked == true)
				outlogtext += "<a href=\"#weapons\">Weapon Stats</a>";
			if (cbxRandClassBases.Checked == true | cbxShuffleClassBases.Checked == true | cbxRandMove.Checked == true)
				outlogtext += "<a href=\"#class\">Class Bases</a>";
			if (cbxStatCaps.Checked == true | cbxStatCapDev.Checked == true | cbxStatCapFlat.Checked == true)
				outlogtext += "<a href=\"#caps\">Stat Caps</a>";
			if (cbxStatBooster.Checked == true)
				outlogtext += "<a href=\"#boost\">Stat Boosters</a>";
			if (cbxSkillCap.Checked == true)
				outlogtext += "<a href=\"#skillcap\">Skill Capacities</a>";
			if (cbxEventItems.Checked == true)
				outlogtext += "<a href=\"#event\">Event Items</a>";
			if (cbxRandEnemy.Checked == true)
				outlogtext += "<a href=\"#enemy\">Enemies</a>";
			if (cbxRandPromotion.Checked == true)
				outlogtext += "<a href=\"#promo\">Promotion Lines</a>";

			// add hidden chosen settings and if iso was re-randomized
			if (check4rando == false)
				outlogtext += "<! -- RE-RANDOMIZED ISO -- >";
			outlogtext += "<! -- " + randomizationSettings + " -- >";

			outlogtext += "<h2>Seed: " + numericSeed.Value + "</h2><div class=\"tab\">";
			for (charNum = 0; charNum < charName.Length; charNum++)
			{
				outlogtext += "<button class=\"tablinks\" onclick=\"openChar(event, '" + charName[charNum] +
					"')\" id=\"defaultOpen\"><img src=\"assets/logpics/" + charName[charNum] + ".png\" alt=\"" + charName[charNum] +
					".png\" style=\"width:64px; height:64px;\"></button><br>";

				// move herons to correct order
				if (charNum == 17)
					charNum = 68;
				else if (charNum == 69)
					charNum = 17;
				else if (charNum == 21)
					charNum = 69;
				else if (charNum == 70)
					charNum = 21;
				else if (charNum == 47)
					charNum = 70;
				else if (charNum == 71)
					charNum = 47;
				else if (charNum == 68)
					charNum = 71;
			}
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
							case 98:
								AFFstring = "earth";
								break;
							case 105:
								AFFstring = "thunder";
								break;
							case 182:
								AFFstring = "wind";
								break;
							case 176:
								AFFstring = "water";
								break;
							case 75:
								AFFstring = "heaven";
								break;
							case 48:
								AFFstring = "fire";
								break;
							case 231:
								AFFstring = "dark";
								break;
							default:
								AFFstring = "light";
								break;
						}

						outlogtext += "<h2><img src=\"assets/logpics/" + AFFstring + ".png\" alt=\"" + AFFstring +
							".png\" style=\"width: 32px; height: 32px; \">" + char.ToUpper(charName[charNum][0]) +
							charName[charNum].Substring(1) + "";

						// go to authority
						stream.Position = charBio[charNum] + 4;
						int auth = stream.ReadByte();
						for (int i = 0; i < auth; i++)
						{
							outlogtext += "<img src=\"assets/logpics/star.png\" alt=\"star.png\" style=\"width: 18px; height: 18px;\">";
						}
						outlogtext += "</h2>";

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
						outlogtext += "<h4>Class</h4><p>" + claName[newClass[charNum]] + "</p>";
					}

					if (charNum < 72)
					{
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
						// class bases
						if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
							stream.Position = claDataLoc[newClass[charNum]] + 112;
						else
							stream.Position = claDataLoc[charVanillaClass[charNum]] + 112;
						outlogtext += "<h4>Class Bases</h4><table><tr><th>HP</th><th>STR</th><th>MAG</th>" +
							"<th>SKL</th><th>SPD</th><th>LCK</th><th>DEF</th><th>RES</th></tr><tr>";
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

						// skills output
						if (cbxSkillRand.Checked == true)
						{
							outlogtext += "<h4>Skills</h4>";
							for (int k = 0; k < 4; k++)
							{
								if (cbxParagon.Checked == true & k == 3)
								{
									outlogtext += "<div class=\"img_wrap\"><img src=\"assets/logpics/paragon.png\" alt=\"" +
										"paragon.png\" style=\"width: 64px; height: 64px; \"><p class=\"img_description\">" +
										"paragon</p></div>";
								}
								else if (newSkills[charNum, k] != "" & newSkills[charNum, k] != null)
								{
									outlogtext += "<div class=\"img_wrap\"><img src=\"assets/logpics/" + newSkills[charNum, k].ToLower() + ".png\" alt=\"" +
										newSkills[charNum, k].ToLower() + ".png\" style=\"width: 64px; height: 64px; \"><p class=\"img_description\">" +
										newSkills[charNum, k].ToLower() + "</p></div>";
								}
							}
						}
						else if (cbxParagon.Checked == true)
						{
							outlogtext += "<h4>Skills</h4>";
							outlogtext += "<div class=\"img_wrap\"><img src=\"assets/logpics/paragon.png\" alt=\"" +
										"paragon.png\" style=\"width: 64px; height: 64px; \"><p class=\"img_description\">" +
										"paragon</p></div>";
						}
					}


					outlogtext += "</div>";

					

				}

				reader = new System.IO.StreamReader(file + "\\assets\\logscript.txt");
				outlogtext += reader.ReadToEnd();
				reader.Close();


				if (cbxTowerUnits.Checked == true)
				{
					outlogtext += "<br><hr><br><h2 id=\"tower\">Tower Units</h2>";

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
						outlogtext += "<img src=\"assets/logpics/" + towerUnits[10] + ".png\" alt=\"" + towerUnits[10] + ".png\" style=\"width:64px;height:64px;\">" +
						"<img src=\"assets/logpics/" + towerUnits[11] + ".png\" alt=\"" + towerUnits[11] + ".png\" style=\"width:64px;height:64px;\">";
					}

					for (int k = 0; k < 10; k++)
					{
						outlogtext += "<img src=\"assets/logpics/" + towerUnits[k] + ".png\" alt=\"" + towerUnits[k] + ".png\" style=\"width:64px;height:64px;\">";
					}
				}


				if (cbxRandWeap.Checked == true)
				{
					outlogtext += "<br><hr><br><h2 id=\"weapons\">Weapon Stats</h2>";
					outlogtext += "<table><tr> <th>Name</th> <th>MT</th> <th>ACC</th> <th>CRT</th> <th>WT</th> <th>USE</th> </tr>";
					
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

						outlogtext += "<tr> <td>" + values[0] + "</td>";

						stream.Position = Convert.ToInt32(values[1]) + 40;
						for (int j = 0; j < 5; j++)
						{
							outlogtext += "<td>";
							int weapstat = stream.ReadByte();
							if (weapstat > 127)
								weapstat -= 256;
							outlogtext += weapstat.ToString() + "</td>";
						}
						outlogtext += "</tr>";
					}
					outlogtext += "</table>";
					dataReader.Close();
				}


				if (cbxRandClassBases.Checked == true | cbxShuffleClassBases.Checked == true | cbxRandMove.Checked == true)
				{
					outlogtext += "<br><hr><br><h2 id=\"class\">Class Bases</h2>";
					outlogtext += "<table><tr> <th>Name</th> <th>HP</th> <th>STR</th> <th>MAG</th> <th>SKL</th> <th>SPD</th> " +
														"<th>LCK</th> <th>DEF</th> <th>RES</th> <th>MOV</th> </tr>";

					string line;
					string[] values;
					// initialize character information
					System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\StatCaps.csv");

					// skip header line/
					line = dataReader.ReadLine();
					// loop through all 157 classes
					for (int i = 0; i < 157; i++)
					{
						line = dataReader.ReadLine();
						values = line.Split(',');

						outlogtext += "<tr> <td>" + values[0] + "</td>";
						// 8 base stats
						int classstat;
						stream.Position = Convert.ToInt32(values[1]) + 8;
						for (int j = 0; j < 8; j++)
						{
							outlogtext += "<td>";
							classstat = stream.ReadByte();
							if (classstat > 127)
								classstat -= 256;
							outlogtext += classstat.ToString() + "</td>";
						}
						// movement
						stream.Position -= 67;
						classstat = stream.ReadByte();
						if (classstat > 127)
							classstat -= 256;
						outlogtext += "<td>" + classstat.ToString() + "</td>";
						outlogtext += "</tr>";
					}
					outlogtext += "</table>";
					dataReader.Close();

				}


				if (cbxStatCaps.Checked == true | cbxStatCapDev.Checked == true | cbxStatCapFlat.Checked == true)
				{
					outlogtext += "<br><hr><br><h2 id=\"caps\">Class Stat Caps</h2>";
					outlogtext += "<table><tr> <th>Name</th> <th>HP</th> <th>STR</th> <th>MAG</th> <th>SKL</th> <th>SPD</th> " +
														"<th>LCK</th> <th>DEF</th> <th>RES</th> </tr>";

					string line;
					string[] values;
					// initialize character information
					System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\StatCaps.csv");

					// skip header line/
					line = dataReader.ReadLine();
					// loop through all 157 classes
					for (int i = 0; i < 157; i++)
					{
						line = dataReader.ReadLine();
						values = line.Split(',');

						outlogtext += "<tr> <td>" + values[0] + "</td>";
						// 8 stats
						int classstat;
						stream.Position = Convert.ToInt32(values[1]);
						for (int j = 0; j < 8; j++)
						{
							outlogtext += "<td>";
							classstat = stream.ReadByte();
							if (classstat > 127)
								classstat -= 256;
							outlogtext += classstat.ToString() + "</td>";
						}
						outlogtext += "</tr>";
					}
					outlogtext += "</table>";
					dataReader.Close();

				}

				if (cbxStatBooster.Checked == true)
				{
					outlogtext += "<br><hr><br><h2 id=\"boost\">Stat Boosters</h2>";
					outlogtext += "<table><tr> <th>Item</th> <th>HP</th> <th>STR</th> <th>MAG</th> <th>SKL</th> <th>SPD</th> " +
														"<th>LCK</th> <th>DEF</th> <th>RES</th> </tr>";

					string line;
					string[] values;

					System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\StatBoosters.csv");

					// skip new line
					line = dataReader.ReadLine();

					for (int i = 0; i < 8; i++)
					{
						line = dataReader.ReadLine();
						values = line.Split(',');
						outlogtext += "<tr> <th>" + values[0] + "</th>";
						stream.Position = Convert.ToInt32(values[1]);
						for (int j = 0; j < 8; j++)
						{
							int readin = stream.ReadByte();
							if (readin > 127)
								readin -= 256;
							outlogtext += "<th>" + readin.ToString() + "</th>";
						}
						outlogtext += "</tr>";
					}
					dataReader.Close();
					outlogtext += "</table>";
				}

				if (cbxSkillCap.Checked == true)
				{
					outlogtext += "<br><hr><br><h2 id=\"skillcap\">Skill Capacities</h2>";
					outlogtext += "<table><tr> <th>Skill Name</th> <th>Capacity</th> </tr>";

					string line;
					string[] values;

					System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\SkillLoc.csv");

					// loop through all classes
					for (int i = 0; i < 71; i++)
					{
						outlogtext += "<tr>";
						line = dataReader.ReadLine();
						values = line.Split(',');

						outlogtext += "<th>" + values[0] + "</th>";
						stream.Position = Convert.ToInt32(values[1]) + 30;
						int skillcapval = stream.ReadByte();
						outlogtext += "<th>" + skillcapval.ToString() + "</th>";

						outlogtext += "</tr>";
					}
					dataReader.Close();

					outlogtext += "</table>";
				}

			}
						

			if (cbxEventItems.Checked == true)
			{
				outlogtext += "<br><hr><br><h2 id=\"event\">Event Items</h2>" + eventItemsOutput;
			}

			if (cbxRandEnemy.Checked == true)
			{
				outlogtext += "<br><hr><br><h2 id=\"enemy\">Enemies</h2>" + randEnemyOutput;
			}

			if (cbxRandPromotion.Checked == true)
			{
				outlogtext += "<br><hr><br><h2 id=\"promo\">Promotion Lines</h2>" + randPromoOutput;
			}


			outlogtext += "</body></html>";

			logwriter.WriteLine(outlogtext);
			logwriter.Close();

		}


		private void ikeMicClassCombos()
		{
			if (cbxClassPatch.Checked == true)
			{
				comboMicClass.Items[23] = "77 - thdrknight";
				comboMicClass.Items[24] = "82 - maligknight";
				comboIkeClass.Items[23] = "69 - dread fighter";
				comboIkeClass.Items[24] = "71 - lancer";
				comboIkeClass.Items[25] = "73 - trickster";
				comboIkeClass.Items[26] = "75 - bartendress";
				comboIkeClass.Items[29] = "83 - maligmaster";
				comboIkeClass.Items[30] = "85 - warmonk";
				comboElinciaClass.Items[27] = "70 - yasha";
				comboElinciaClass.Items[28] = "72 - holy lancer";
				comboElinciaClass.Items[29] = "74 - lightning thief";
				comboElinciaClass.Items[30] = "76 - enchantress";
				comboElinciaClass.Items[31] = "79 - platinumknight";
				comboElinciaClass.Items[32] = "81 - bronzeknight";
				comboElinciaClass.Items[33] = "84 - maliglord";
				comboElinciaClass.Items[34] = "86 - crusader";

				if (cbxHorseParkour.Checked == true)
				{
					comboIkeClass.Items[27] = "78 - thdrpaladin";
					comboIkeClass.Items[28] = "80 - firepaladin";
				}
				else
				{
					comboIkeClass.Items[27] = "-";
					comboIkeClass.Items[28] = "-";
				}
			}
			else
			{
				comboMicClass.Items[23] = "-";
				comboMicClass.Items[24] = "-";
				comboIkeClass.Items[23] = "-";
				comboIkeClass.Items[24] = "-";
				comboIkeClass.Items[25] = "-";
				comboIkeClass.Items[26] = "-";
				comboIkeClass.Items[27] = "-";
				comboIkeClass.Items[28] = "-";
				comboIkeClass.Items[29] = "-";
				comboIkeClass.Items[30] = "-";
				comboElinciaClass.Items[27] = "-";
				comboElinciaClass.Items[28] = "-";
				comboElinciaClass.Items[29] = "-";
				comboElinciaClass.Items[30] = "-";
				comboElinciaClass.Items[31] = "-";
				comboElinciaClass.Items[32] = "-";
				comboElinciaClass.Items[33] = "-";
				comboElinciaClass.Items[34] = "-";
			}
			if (cbxHorseParkour.Checked == true)
			{
				comboIkeClass.Items[2] = "6 - swdpaladin";
				comboIkeClass.Items[5] = "15 - lncpaladin";
				comboIkeClass.Items[9] = "27 - axepaladin";
				comboIkeClass.Items[13] = "39 - bowpaladin";
				comboIkeClass.Items[22] = "64 - cleric";
			}
			else
			{
				comboIkeClass.Items[2] = "-";
				comboIkeClass.Items[5] = "-";
				comboIkeClass.Items[9] = "-";
				comboIkeClass.Items[13] = "-";
				comboIkeClass.Items[22] = "-";
			}
			if (cbxStatCaps.Checked == true | cbxBKfight.Checked == true | cbxBKnerf.Checked == true)
			{
				comboIkeClass.Items[14] = "42 - rogue";
				comboIkeClass.Items[15] = "46 - firesage";
				comboIkeClass.Items[16] = "49 - thdrsage";
				comboIkeClass.Items[17] = "52 - windsage";
				comboIkeClass.Items[18] = "55 - lightsage";
				comboIkeClass.Items[19] = "58 - bishop";
				comboIkeClass.Items[20] = "60 - darksage";
				comboIkeClass.Items[21] = "62 - druid";
				comboIkeClass.Items[36] = "92 - raven";
			}
			else
			{
				comboIkeClass.Items[14] = "-";
				comboIkeClass.Items[15] = "-";
				comboIkeClass.Items[16] = "-";
				comboIkeClass.Items[17] = "-";
				comboIkeClass.Items[18] = "-";
				comboIkeClass.Items[19] = "-";
				comboIkeClass.Items[20] = "-";
				comboIkeClass.Items[21] = "-";
				comboIkeClass.Items[36] = "-";
			}
			
		}
		
		static int bytes2int(byte[] fourbytes)
		{
			int total = 0;
			for (int i = 0; i < fourbytes.Length; i++)
			{
				total += fourbytes[fourbytes.Length - 1 - i] * (int)(Math.Pow(256, i));
			}
			return total;
		}

		static byte[] int2bytes(int number)
		{
			byte[] converted = { 0x00, 0x00, 0x00, 0x00 };
			int iter = 0;
			for (int i = 3; i > 0; i--)
			{
				int overflow = (int)(Math.Pow(256, i));
				while (number >= overflow)
				{
					number -= overflow;
					converted[iter] += 1;
				}
				iter += 1;
			}
			converted[3] = (byte)number;
			return converted;
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

				// reset rando bool
				check4rando = true;
			}

		}

		private void cbxClassRand_CheckedChanged(object sender, EventArgs e)
		{
			panelClass.Enabled = cbxClassRand.Checked;
			if(cbxRandRecr.Checked == false)
				cbxStoryPromo.Enabled = !cbxClassRand.Checked;
			cbxChooseIke.Enabled = cbxClassRand.Checked;
			cbxChooseMic.Enabled = cbxClassRand.Checked;

			if (cbxEnemyRecruit.Checked == true)
			{
				if (cbxClassRand.Checked == true)
				{
					comboLord.Items.Add("blackknight");
					comboLord.Items.Add("ashera");
					comboMicc.Items.Add("blackknight");
					comboMicc.Items.Add("ashera");
				}
				else
				{
					if (comboLord.SelectedIndex >= 84)
						comboLord.SelectedIndex = 34;
					comboLord.Items.RemoveAt(84);
					comboLord.Items.RemoveAt(84);
					if (comboMicc.SelectedIndex >= 84)
						comboMicc.SelectedIndex = 0;
					comboMicc.Items.RemoveAt(84);
					comboMicc.Items.RemoveAt(84);
				}
			}

			ikeMicClassCombos();
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
				cbxNegGrowths.Checked = false;
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

			// can only prevent early laguz/swap classes in full random
			if (comboClassOptions.SelectedIndex == 1)
			{
				cbxNoLaguz.Enabled = true;
				cbxClassSwap.Enabled = true;
			}
			else
			{
				cbxNoLaguz.Enabled = false;
				cbxClassSwap.Enabled = false;
			}

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
			if (cbxClassRand.Checked == false)
				cbxStoryPromo.Enabled = !cbxRandRecr.Checked;
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
			cbxGrowthShuffleMax.Enabled = cbxGrowthShuffle.Checked;
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
				string[] enemies = { "jarod","ludveck","septimus","valtome","numida","izuka","hetzel",
									"levail","lekain","zelgius","dheginsea","sephiram"};

				for (int i = 0; i < enemies.Length; i++)
				{
					comboLord.Items.Add(enemies[i]);
					comboMicc.Items.Add(enemies[i]);
				}
				if (cbxClassRand.Checked == true)
				{
					comboLord.Items.Add("blackknight");
					comboLord.Items.Add("ashera");
					comboMicc.Items.Add("blackknight");
					comboMicc.Items.Add("ashera");
				}
			}
			else
			{
				if (comboLord.SelectedIndex >= 72)
					comboLord.SelectedIndex = 34;
				if (comboMicc.SelectedIndex >= 72)
					comboMicc.SelectedIndex = 0;

				for (int i = 0; i < 12; i++)
				{
					comboLord.Items.RemoveAt(72);
					comboMicc.Items.RemoveAt(72);
				}
				if (cbxClassRand.Checked == true)
				{
					comboLord.Items.RemoveAt(72);
					comboLord.Items.RemoveAt(72);
					comboMicc.Items.RemoveAt(72);
					comboMicc.Items.RemoveAt(72);
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

		private void cbxEventItems_CheckedChanged(object sender, EventArgs e)
		{
			cbxNoRandPromotions.Enabled = cbxEventItems.Checked;
			cbxWhiteGem.Enabled = cbxEventItems.Checked;
		}

		private void cbxRandShop_CheckedChanged(object sender, EventArgs e)
		{
			cbxIronShop.Enabled = cbxRandShop.Checked;
		}

		private void cbxStatCaps_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxStatCaps.Checked == true)
			{
				cbxStatCapDev.Checked = false;
				cbxStatCapFlat.Checked = false;
			}
			ikeMicClassCombos();
		}

		private void cbxStatCapDev_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxStatCapDev.Checked == true)
			{
				cbxStatCaps.Checked = false;
				cbxStatCapFlat.Checked = false;
			}
		}

		private void cbxStatCapFlat_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxStatCapFlat.Checked == true)
			{
				cbxStatCapDev.Checked = false;
				cbxStatCaps.Checked = false;
			}
		}

		private void numericMoveMin_ValueChanged(object sender, EventArgs e)
		{
			numericMoveMax.Minimum = numericMoveMin.Value;
		}

		private void numericMoveMax_ValueChanged(object sender, EventArgs e)
		{
			numericMoveMin.Maximum = numericMoveMax.Value;
		}

		private void cbxNoFOW_CheckedChanged(object sender, EventArgs e)
		{
			cbxBirdVision.Enabled = !cbxNoFOW.Checked;
		}

		private void cbxBargains_CheckedChanged(object sender, EventArgs e)
		{
			panelbargain.Enabled = cbxBargains.Checked;
		}

		private void cbxForge_CheckedChanged(object sender, EventArgs e)
		{
			panelforge.Enabled = cbxForge.Checked;
		}

		private void cbxRandClassBases_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxRandClassBases.Checked == true)
				cbxShuffleClassBases.Checked = false;
		}

		private void cbxShuffleClassBases_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxShuffleClassBases.Checked == true)
				cbxRandClassBases.Checked = false;
			cbxHPShuffleclass.Enabled = cbxShuffleClassBases.Checked;
		}

		private void cbxRandPromotion_CheckedChanged(object sender, EventArgs e)
		{
			cbxEasyPromotion.Enabled = cbxRandPromotion.Checked;
		}

		private void numStatBoostMax_ValueChanged(object sender, EventArgs e)
		{
			numStatBoostMin.Maximum = numStatBoostMax.Value;
		}

		private void numStatBoostMin_ValueChanged(object sender, EventArgs e)
		{
			numStatBoostMax.Minimum = numStatBoostMin.Value;
		}

		private void cbxNegGrowths_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxNegGrowths.Checked == true)
				cbxZeroGrowths.Checked = false;
		}

		private void cbxSkillRand_CheckedChanged(object sender, EventArgs e)
		{
			cbxSkillMax.Enabled = cbxSkillRand.Checked;
			cbxSkillUno.Enabled = cbxSkillRand.Checked;
		}

		private void cbxSkillMax_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxSkillMax.Checked == true)
				cbxSkillUno.Checked = false;
		}

		private void cbxSkillUno_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxSkillUno.Checked == true)
				cbxSkillMax.Checked = false;
		}

		private void cbxClassPatch_CheckedChanged(object sender, EventArgs e)
		{
			ikeMicClassCombos();
		}

		private void cbxHorseParkour_CheckedChanged(object sender, EventArgs e)
		{
			ikeMicClassCombos();
		}

		private void cbxBKnerf_CheckedChanged(object sender, EventArgs e)
		{
			ikeMicClassCombos();
		}

		private void cbxBKfight_CheckedChanged(object sender, EventArgs e)
		{
			ikeMicClassCombos();
		}

		private void cbxLords_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxLords.Checked == true)
			{
				cbxMicClass.Checked = false;
				cbxIkeClass.Checked = false;
				cbxElinciaClass.Checked = false;
			}
		}

		private void cbxMicClass_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxMicClass.Checked == true)
			{
				cbxLords.Checked = false;
			}
		}

		private void cbxIkeClass_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxIkeClass.Checked == true | cbxElinciaClass.Checked == true)
			{
				cbxLords.Checked = false;
			}
		}

		private void cbxClassSwap_CheckedChanged(object sender, EventArgs e)
		{
			
		}

		private void cbxParagon_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxParagon.Checked == true)
				cbxLaguzParagon.Checked = false;
		}

		private void cbxLaguzParagon_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxLaguzParagon.Checked == true)
				cbxParagon.Checked = false;
		}
	}
}

