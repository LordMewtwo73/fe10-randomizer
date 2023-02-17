using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using FE10FileExtract;
using System.Diagnostics;


namespace FE10Randomizer_v0._1
{
	public partial class Randomizer_Main : Form
	{
		// location of randomizer
		string file = System.IO.Directory.GetCurrentDirectory();
		// location of DATA/files/
		string dataLocation;
		// location of DATA/sys/
		string sysLocation;
		// location of shop file
		string shopfileLocation;

		// extraction classes
		DisposFile[] ChapterData;
		FEDataFile CharacterData, ClassData, ItemData, SkillData, TerrData, MapData;

		// bools for successful randomization
		bool notvanilla;
		bool rerandomized = false;
		bool validfolder = false;
		bool classeschanged = false;

		// list of all chapters in game
		string[] chapters;

		// error flag, 0 = no error
		int errorflag = 0;

		// number of units to change
		int totalUnitNumber;

		// number of units changed to heron
		int heronNumber;

		// whether user has NTSC-U 1.0 or 1.1, or PAL
		int gameVersion;

		string chapterFile;
		StreamWriter dataWriter;

		// create random number generator
		Random seedGenerator = new Random();
		Random random;

		// keeps track of current character
		int charNum;

		int weightflag = 0;

		bool restrictIke = true;

		// class for jarod in 1-10, used to keep same in 1-11
		int jarodclass;
		List<string> enemyoldclass = new List<string>();
		List<int> enemynewclass = new List<int>();

		string randomizationSettings = "";

		// arrays that hold character data
		Character[] characters;

		// arrays that hold class data
		Job[] classes;

		// list of decompiled script files
		string[] script_exl;

		// info for outputlog
		int[] towerUnits = new int[12];
		string eventItemsOutput = "";
		string randEnemyOutput = "";
		string randPromoOutput = "";
		string bargainOutput = "";
		string forgeOutput = "";

		// bool for april fools
		bool aprilFools = false;

		public Randomizer_Main()
		{

			InitializeComponent();

			InitializeToolTips();

			comboClassOptions.SelectedIndex = 0;
			comboIke.SelectedIndex = 34;
			comboMicc.SelectedIndex = 0;
			comboElincia.SelectedIndex = 18;
			comboMicClass.SelectedIndex = 17;
			comboIkeClass.SelectedIndex = 1;
			comboElinciaClass.SelectedIndex = 25;

			textBox1.Text = "Welcome to LordMewtwo73's FE10 Randomizer! Please load in the DATA\\files folder of an extracted iso.";

			numericSeed.Value = seedGenerator.Next();
		}

		// *************************************************************************************** BUTTON PRESS FUNCTIONS
		#region

		// loads ISO files and checks version and if they're clean
		private void btnLoad_Click(object sender, EventArgs e)
		{
			if (folderBD.ShowDialog() == DialogResult.OK)
			{
				textBox1.Text = "Checking ISO.";
				Application.DoEvents();

				dataLocation = folderBD.SelectedPath;
				sysLocation = dataLocation.Remove(dataLocation.Length - 5, 5) + "sys";

				// makes sure all files exist, gets version, and checks if the game is clean
				bool filesfound = checkFiles();
				if (filesfound)
				{
					validfolder = true;
					if (gameVersion == 0)
						lblLocation.Text = "NTSC 1.1 - " + dataLocation;
					else if (gameVersion == 1)
						lblLocation.Text = "NTSC 1.0 - " + dataLocation;
					else if (gameVersion == 2)
						lblLocation.Text = "PAL - " + dataLocation;
					else
						lblLocation.Text = dataLocation;

					// enable user to randomize
					btnRandomize.Enabled = true;

					if (!rerandomized)
					{
						textBox1.Text = "Select desired randomization settings, then press the randomize button.";
						textBox1.BackColor = TextBox.DefaultBackColor;
						textBox1.ForeColor = TextBox.DefaultForeColor;
						Application.DoEvents();
					}
				}
				else
				{
					dataLocation = "";
					lblLocation.Text = "No DATA\\files folder selected";
					textBox1.Text = "One or more files are missing or invalid at the chosen path.";
					Application.DoEvents();
				}
			}

		}

		// loads up settings from previous randomization
		private void LoadSettings_Click(object sender, EventArgs e)
		{
			// open file dialog
			openFD.Title = "Load Randomization Settings";
			openFD.DefaultExt = "ini";
			openFD.InitialDirectory = file + "\\settings";
			openFD.Filter = "Settings Files | *.ini";

			if (openFD.ShowDialog() == DialogResult.OK)
			{
				System.Windows.Forms.CheckBox[] checkBoxes;
				System.Windows.Forms.ComboBox[] comboBoxes;
				System.Windows.Forms.NumericUpDown[] numericUpDowns;
				System.Windows.Forms.RadioButton[] radioButtons;

				try
				{
					Stream fileStream = openFD.OpenFile();
					System.IO.StreamReader settingreader = new System.IO.StreamReader(fileStream);
					string settingstring = settingreader.ReadLine();
					settingreader.Close();

					if (settingstring != "")
					{
						string[] eachsetting = settingstring.Split(',');
						string versionNum = eachsetting[0];

						// get lists based on version settings were saved from
						if (versionNum == "3.3.0" | versionNum == "3.2.1")
						{
							checkBoxes = new System.Windows.Forms.CheckBox[]  { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxPart2Enemies,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz,cbxBonusBEXP, cbxTormodT3, cbxLaguzCanto, cbxStatBooster,
														   cbxStatBoostMult, cbxNegGrowths, cbxStoryPromo, cbxAuthority, cbxSkillCap, cbxMicClass, cbxIkeClass,
															cbxHeronSpread, cbxSkillSetNum, cbxWeapPatch,cbxDragonCanto,cbxElinciaClass,cbxSkillVanilla,cbxSkillSetNum,
															cbxChooseElincia,cbxRecrVanillaClass,cbxBonusItems,cbxEnemBonusDrop,cbx1to1EnemyRand,cbxRandAllies,cbxPart2PCs,
															cbxPart2Allies,cbxLetheMordy, cbxUniversalSkills, cbxBossBonusDrop,cbxMistCrown,cbxDruidCrown};

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc, comboIkeClass, comboMicClass, comboElinciaClass, comboElincia };

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
																numericClassBaseDev, numericClassBaseShuf, numericStatCapMin,numStatBoostMin, numStatBoostMax,
																numSkillVanillaPlus,numSkillSet};

							radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
						}
						else if (versionNum == "3.1.0")
						{
							checkBoxes = new System.Windows.Forms.CheckBox[]  { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxPart2Enemies,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz,cbxBonusBEXP, cbxTormodT3, cbxLaguzCanto, cbxStatBooster,
														   cbxStatBoostMult, cbxNegGrowths, cbxStoryPromo, cbxAuthority, cbxSkillCap, cbxMicClass, cbxIkeClass,
															cbxHeronSpread, cbxSkillSetNum, cbxWeapPatch,cbxDragonCanto,cbxElinciaClass,cbxSkillVanilla,cbxSkillSetNum,
															cbxChooseElincia,cbxRecrVanillaClass,cbxBonusItems,cbxEnemBonusDrop,cbx1to1EnemyRand,cbxRandAllies,cbxPart2PCs,
															cbxPart2Allies,cbxLetheMordy, cbxUniversalSkills, cbxBossBonusDrop};

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc, comboIkeClass, comboMicClass, comboElinciaClass, comboElincia };

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
																numericClassBaseDev, numericClassBaseShuf, numericStatCapMin,numStatBoostMin, numStatBoostMax,
																numSkillVanillaPlus,numSkillSet};

							radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
						}
						else if (versionNum == "3.0.2" | versionNum == "3.0.1")
						{
							checkBoxes = new System.Windows.Forms.CheckBox[]  { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxPart2Enemies,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz,cbxBonusBEXP, cbxTormodT3, cbxLaguzCanto, cbxStatBooster,
														   cbxStatBoostMult, cbxNegGrowths, cbxStoryPromo, cbxAuthority, cbxSkillCap, cbxMicClass, cbxIkeClass,
															cbxHeronSpread, cbxSkillSetNum, cbxWeapPatch,cbxDragonCanto,cbxElinciaClass,cbxSkillVanilla,cbxSkillSetNum,
															cbxChooseElincia,cbxRecrVanillaClass,cbxBonusItems,cbxEnemBonusDrop,cbx1to1EnemyRand,cbxRandAllies,cbxPart2PCs,
															cbxPart2Allies,cbxLetheMordy, cbxUniversalSkills};

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc, comboIkeClass, comboMicClass, comboElinciaClass, comboElincia };

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
																numericClassBaseDev, numericClassBaseShuf, numericStatCapMin,numStatBoostMin, numStatBoostMax,
																numSkillVanillaPlus,numSkillSet};

							radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
						}
						else if (versionNum == "3.0.0")
						{
							checkBoxes = new System.Windows.Forms.CheckBox[]  { cbxAffinity, cbxBio, cbxBirdVision, cbxBKfight, cbxBKnerf, cbxBuffBosses, cbxChestKey,
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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxPart2Enemies,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz,cbxBonusBEXP, cbxTormodT3, cbxLaguzCanto, cbxStatBooster,
														   cbxStatBoostMult, cbxNegGrowths, cbxStoryPromo, cbxAuthority, cbxSkillCap, cbxMicClass, cbxIkeClass,
															cbxHeronSpread, cbxSkillSetNum, cbxWeapPatch,cbxDragonCanto,cbxElinciaClass,cbxSkillVanilla,cbxSkillSetNum,
															cbxChooseElincia,cbxRecrVanillaClass,cbxBonusItems,cbxEnemBonusDrop,cbx1to1EnemyRand,cbxRandAllies,cbxPart2PCs,
															cbxPart2Allies,cbxLetheMordy};

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc, comboIkeClass, comboMicClass, comboElinciaClass, comboElincia };

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
																numericClassBaseDev, numericClassBaseShuf, numericStatCapMin,numStatBoostMin, numStatBoostMax,
																numSkillVanillaPlus,numSkillSet};

							radioButtons = new System.Windows.Forms.RadioButton[] { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};
						}
						else if (versionNum == "2.4.2" | versionNum == "2.4.3" | versionNum == "2.4.4")
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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxPart2Enemies,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz,cbxBonusBEXP, cbxTormodT3, cbxLaguzCanto, cbxStatBooster,
														   cbxStatBoostMult, cbxNegGrowths, cbxStoryPromo, cbxAuthority, cbxSkillCap, cbxMicClass, cbxIkeClass,
														   cbxHeronSpread, cbxSkillSetNum, cbxWeapPatch, cbxDragonCanto, cbxElinciaClass};

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc, comboIkeClass, comboMicClass, comboElinciaClass };

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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxPart2Enemies,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz,cbxBonusBEXP, cbxTormodT3, cbxLaguzCanto, cbxStatBooster,
														   cbxStatBoostMult, cbxNegGrowths, cbxStoryPromo, cbxAuthority, cbxSkillCap, cbxMicClass, cbxIkeClass,
														   cbxHeronSpread, cbxSkillSetNum, cbxWeapPatch};

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc, comboIkeClass, comboMicClass };

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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxPart2Enemies,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz};

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc };

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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxPart2Enemies, cbxParagon};

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc };

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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem};

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc };

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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass};

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc };

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

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke, comboMicc };

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

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke };

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

							comboBoxes = new System.Windows.Forms.ComboBox[] { comboClassOptions, comboIke };

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

						textBox1.Text = "Settings File loaded successfully.";
						textBox1.BackColor = TextBox.DefaultBackColor;
						textBox1.ForeColor = TextBox.DefaultForeColor;
						Application.DoEvents();

					}
				}
				catch(Exception ex)
				{
					textBox1.Text = "Settings File not formatted properly, could not load.";
					textBox1.BackColor = Color.LightPink;
					Application.DoEvents();
				}
			}
		}

		// saves settings from current randomization
		private void SaveSettings_Click(object sender, EventArgs e)
		{
			// open file dialog
			saveFD.Title = "Save Randomization Settings";
			saveFD.DefaultExt = "ini";
			saveFD.InitialDirectory = file + "\\settings";
			saveFD.Filter = "Settings Files | *.ini";

			if (saveFD.ShowDialog() == DialogResult.OK)
			{
				string settingstring = SaveSettingsString();
				
				// save
				Stream fileStream = saveFD.OpenFile();
				StreamWriter settingwriter = new StreamWriter(fileStream);
				settingwriter.WriteLine(settingstring);
				settingwriter.Close();
			}
		}

		// calls all randomization functions
		private void btnRandomize_Click(object sender, EventArgs e)
		{
			// disable front panel components
			FrontPanel_Disable();

			// initialize variables for randomization
			Initialize();

			// check to see if the user has selected not-okay options for randomization
			bool validchoices = CheckValidChoices();

			// continue only if valid folder loaded and options are valid
			if (validfolder & validchoices)
			{
				// overwrite a pre-randomized ISO with vanilla files
				if (notvanilla)
					CopyVanillaFiles();

				// delete temp files created from ExtractFiles
				DeleteTemp();

				// extract dispos, shop, and FE10Data files
				ExtractFiles();
				ExaltScripts();

				// if user selects "trial mode", the ISO is not overwritten
				if (!cbxTrialMode.Checked)
				{
					// mark ISO as randomized
					ChangeISO_ID();
				}

				// modifies various character/class stats based on user selection
				DataFileModifications();

				// randomize with desired settings
				Randomize();

				// if user selects "trial mode", the ISO is not overwritten
				if (!cbxTrialMode.Checked)
				{
					// mark ISO as randomized
					//ChangeISO_ID();
					// compress files back to ISO
					CompressFiles();
					CompressScripts();
					// move some files (depending on selections)
					MoveFiles();

					notvanilla = true;
				}

				// saves setting string for outputlog
				SaveSettingsString();

				// create outputlog
				CreateOutputLog();

				// delete temp files created from ExtractFiles
				//DeleteTemp();

				textBox1.Text = "Randomization Complete! Check outputlog.htm for details";
				textBox1.BackColor = TextBox.DefaultBackColor;
				textBox1.ForeColor = TextBox.DefaultForeColor;
				Application.DoEvents();
			}

			// re-enable front panel components
			FrontPanel_Enable();
		}

		#endregion

		// *************************************************************************************** INITIALIZATION FUNCTIONS
		#region

		// sets tooltips for all front panel objects
		private void InitializeToolTips()
		{
			toolTip1.SetToolTip(cbxMistCrown, "puts a holy crown into mist's starting inventory; with Random Event Items, the scripted holy crown will be randomized");
			toolTip1.SetToolTip(cbxDruidCrown, "puts a master crown into starting inventory of any character randomized into a druid; druids still cannot promote via EXP");
			toolTip1.SetToolTip(cbxLetheMordy, "brom and nephenee gain assistance in 2-1");
			toolTip1.SetToolTip(cbxPart2PCs, "increases all stats of many part 2 characters by 2 points each");
			toolTip1.SetToolTip(cbxPart2Allies, "increases all stats of the crimean allies in part 2 by 3 points each");
			toolTip1.SetToolTip(cbxRandAllies, "randomizes yellow ally units in each chapter");
			toolTip1.SetToolTip(cbx1to1EnemyRand, "randomizes enemies/allies in a class by class fashion - ie, all bandits in one chapter will turn into the same random class");
			toolTip1.SetToolTip(cbxEnemBonusDrop, "each enemy gains a random droppable item");
			toolTip1.SetToolTip(cbxBonusItems, "each character gains a bonus item upon recruitment");
			toolTip1.SetToolTip(cbxRecrVanillaClass, "keeps vanilla classes even when characters change - ie, Micaiah turns into Ike, but she stays a lightmage");
			toolTip1.SetToolTip(cbxChooseElincia, "allows you to select the character to replace elincia");
			toolTip1.SetToolTip(cbxTrialMode, "Use this to get an outputlog for your desired settings without actually modifying your ISO");
			toolTip1.SetToolTip(cbxDragonCanto, "Goldoa do be movin' tho");
			toolTip1.SetToolTip(cbxWeapPatch, "removes venin and bronze weapons from the game and replaces them with new effective weapons and weapons that effect weapon triangle differently; see README.htm for more details");
			toolTip1.SetToolTip(cbxBonusBEXP, "grants a significantly increased amount of BEXP for completing a level after each chapter");
			toolTip1.SetToolTip(cbxTormodT3, "vika/muarim have increased stats, and tormod will start as a LV1 firearchsage if classes are not randomized; " +
											"with random classes, he, vika, and muarim will be treated as a t3 beorc unit or non-royal laguz unit");
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
			toolTip1.SetToolTip(cbxSkillSetNum, "gives each playable character random skills equal to the number specified");
			toolTip1.SetToolTip(cbxEasyPromotion, "physical classes will promote into other physical units, and same with magical classes");
			toolTip1.SetToolTip(cbxNoEnemyLaguz, "all beorc enemies will only be randomized into beorc classes, and laguz will stay laguz");
			toolTip1.SetToolTip(cbxParagon, "replaces the final skill of each character with Paragon");
			toolTip1.SetToolTip(cbxDarkMag, "allows light sage and light priestess classes to use dark magic as well as normal weapon types");
			toolTip1.SetToolTip(cbxClassPatch, "adds custom phys/mag hybrid classes made by LordMewtwo into the game. Can be used with or without random classes. See README.htm for futher details");
			toolTip1.SetToolTip(cbxKnifeCrit, "increases the critical rate of all knives by 5%");
			toolTip1.SetToolTip(cbxRandPromotion, "randomizes the promotion line for each beorc class that can normally promote; WARNING: Do not promote Ike before Ch3-5 without selecting Horse " +
												"Parkour, in case he promotes into a mounted unit and cannot climb ledges");
			toolTip1.SetToolTip(cbxMagicPatch, "magic tomes are given new stats as well as new effects; see README.htm for further details");
			toolTip1.SetToolTip(cbxPart2Enemies, "decreases all stats of the rebellion enemies in part 2 by 3 points each");
			toolTip1.SetToolTip(cbxFormshift, "adds a formshift skill scroll, equipable to all laugz, and allows formshift to be removed from laguz royals");
			toolTip1.SetToolTip(cbxWhiteGem, "changes all hidden coins to white gems, which are also modified to be worth 30k each");
			toolTip1.SetToolTip(cbxHPShuffleclass, "adds the HP stat into the shuffle pool; this may inflate stats, as HP is usually much higher than other stats");
			toolTip1.SetToolTip(cbxShuffleClassBases, "adds up total bases (except HP&LCK) + selected addition and redistributes randomly to each stat");
			toolTip1.SetToolTip(cbxRandClassBases, "randomly changes base stats of all classes within vanilla value +/- selected deviation");
			toolTip1.SetToolTip(cbxStatCapDev, "randomly changes stat caps within vanilla value +/- selected deviation");
			toolTip1.SetToolTip(cbxStatCapFlat, "increases all stat caps by selected value");
			toolTip1.SetToolTip(cbxT3Statcaps, "changes to stat caps will only be applied to T3 units");
			toolTip1.SetToolTip(cbxSkillVanilla, "gives each playable character random skills equal to the number of skills they usually have plus specified addition");
			toolTip1.SetToolTip(cbxSkillRand, "randomize skills for player characters");
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
			toolTip1.SetToolTip(cbxRandEnemy, "randomizes the classes of each enemy in each chapter, some restrictions apply. See README.htm for more details");
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

		// checks for main.dol, FE10Data.cms, and a handful of other folders, gets version number, checks if vanilla
		private bool checkFiles()
		{
			string fe10data = dataLocation + "\\FE10Data.cms";
			string maindol = sysLocation + "\\main.dol";
			string chapterfolder = dataLocation + "\\zmap\\";
			string scriptfolder = dataLocation + "\\Scripts\\";
			bool allgood = true;

			allgood &= File.Exists(fe10data);
			allgood &= File.Exists(maindol);
			allgood &= Directory.Exists(chapterfolder);
			allgood &= Directory.Exists(scriptfolder);

			if (allgood)
			{
				getVersion(maindol);
				if (errorflag == 0)
					CheckVanilla();
				else
					return (false);
			}
			return (allgood);
		}

		// gets version number of selected radiant dawn iso
		private void getVersion(string maindol)
		{
			// open main.dol
			using (var stream = new System.IO.FileStream(maindol, System.IO.FileMode.Open,
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
			}
		}

		// disables front panel from user
		private void FrontPanel_Disable()
		{
			// disable components so user can't change properties during randomization
			btnRandomize.Enabled = false;
			menuStrip1.Enabled = false;
			numericSeed.Enabled = false;
			tabControl1.Enabled = false;
		}

		// enables front panel for user
		private void FrontPanel_Enable()
		{
			// disable components so user can't change properties during randomization
			btnRandomize.Enabled = true;
			menuStrip1.Enabled = true;
			numericSeed.Enabled = true;
			tabControl1.Enabled = true;
		}

		private bool CheckValidChoices()
		{
			textBox1.Text = "Checking your choices";
			Application.DoEvents();

			// force heron randomization if herons are chosen as ike/micaiah
			if (cbxRandRecr.Checked & (comboIke.SelectedIndex == 69 | comboIke.SelectedIndex == 70 | comboIke.SelectedIndex == 71 |
									   comboMicc.SelectedIndex == 69 | comboMicc.SelectedIndex == 70 | comboMicc.SelectedIndex == 71 |
									   comboElincia.SelectedIndex == 69 | comboElincia.SelectedIndex == 70 | comboElincia.SelectedIndex == 71))
			{
				cbxHerons.Checked = true;
			}

			// see if ike's class needs to be restricted
			restrictIke = !(cbxStatCaps.Checked | cbxBKfight.Checked | cbxBKnerf.Checked);

			// check class weights
			if (!checkWeights())
				return (false);

			// lords can't be the same person
			if (cbxChooseIke.Checked & cbxChooseMic.Checked & comboIke.SelectedIndex == comboMicc.SelectedIndex)
			{
				textBox1.Text = "Micaiah and Ike cannot be the same character";
				return (false);
			}
			if (cbxChooseElincia.Checked & cbxChooseMic.Checked & comboElincia.SelectedIndex == comboMicc.SelectedIndex)
			{
				textBox1.Text = "Micaiah and Elincia cannot be the same character";
				return (false);
			}
			if (cbxChooseIke.Checked & cbxChooseElincia.Checked & comboIke.SelectedIndex == comboElincia.SelectedIndex)
			{
				textBox1.Text = "Elincia and Ike cannot be the same character";
				return (false);
			}

			//make sure ike and micaiah are not selected as bastian or volke
			if (cbxChooseMic.Checked)
			{
				string chosenname = characters[comboMicc.SelectedIndex].Name;
				if (chosenname == "bastian" | chosenname == "volke")
				{
					textBox1.Text = "Micaiah cannot be changed into Bastian or Volke due to a hit rate bug";
					return false;
				}
				else if (!cbxClassRand.Checked & !cbxClassSwap.Checked)
				{
					if (chosenname == "rafiel" | chosenname == "reyson" | chosenname == "leanne")
					{
						textBox1.Text = "Micaiah cannot be changed into heron characters without class randomization";
						return false;
					}
					if (chosenname == "laura" | chosenname == "rhys" | chosenname == "oliver" | chosenname == "valtome" | chosenname == "numida" |
						chosenname == "hetzel" | chosenname == "lekain")
					{
						textBox1.Text = "Micaiah cannot be changed into " + char.ToUpper(chosenname[0]) + chosenname.Substring(1) + " because they are a priest. Use class randomization to allow Micaiah to become this character.";
						return false;
					}
					if (!cbxRecrVanillaClass.Checked)
					{
						if (chosenname == "elincia" | chosenname == "ike" | chosenname == "pelleas" | chosenname == "sanaki" | chosenname == "lehran" |
						chosenname == "izuka" | chosenname == "mist" | chosenname == "zelgius" | chosenname == "blackknight" | chosenname == "ashera" |
						chosenname == "ena" | chosenname == "gareth" | chosenname == "nasir" | chosenname == "kurthnaga" | chosenname == "dheginsea")
						{
							textBox1.Text = "Micaiah cannot be changed into " + char.ToUpper(chosenname[0]) + chosenname.Substring(1) + " because their class does not have a Tier 1 equivalent. Use class randomization to allow Micaiah to become this character.";
							return false;
						}
					}
				}
			}
			if (cbxChooseIke.Checked)
			{
				string chosenname = characters[comboIke.SelectedIndex].Name;
				if (chosenname == "bastian" | chosenname == "volke")
				{
					textBox1.Text = "Ike cannot be changed into Bastian or Volke due to a hit rate bug";
					return false;
				}
				else if (!cbxClassRand.Checked & !cbxClassSwap.Checked)
				{
					if (chosenname == "rafiel" | chosenname == "reyson" | chosenname == "leanne")
					{
						textBox1.Text = "Ike cannot be changed into heron characters without class randomization";
						return false;
					}
					if (restrictIke)
					{
						if (chosenname == "micaiah" | chosenname == "laura" | chosenname == "sothe" | chosenname == "ilyana" | chosenname == "tormod" |
							chosenname == "heather" | chosenname == "calill" | chosenname == "soren" | chosenname == "rhys" | chosenname == "sanaki" |
							chosenname == "pelleas" | chosenname == "oliver" | chosenname == "lehran" | chosenname == "valtome" | chosenname == "numida" |
							chosenname == "izuka" | chosenname == "hetzel" | chosenname == "lekain" | chosenname == "sephiran")
						{
							textBox1.Text = "Ike cannot be changed into " + char.ToUpper(chosenname[0]) + chosenname.Substring(1) + " because they are a magical unit and he will not be able to defeat the Black Knight. Use Nerf BK or class randomization to allow Ike to become this character.";
							return false;
						}
					}
					if (!cbxHorseParkour.Checked)
					{
						if (chosenname == "fiona" | chosenname == "geoffrey" | chosenname == "kieran" | chosenname == "astrid" | chosenname == "makalov" |
							chosenname == "titania" | chosenname == "mist" | chosenname == "oscar" | chosenname == "renning")
						{
							textBox1.Text = "Ike cannot be changed into " + char.ToUpper(chosenname[0]) + chosenname.Substring(1) + " because they are a mounted unit. Use Horse Parkour or class randomization to allow Ike to become this character.";
							return false;
						}
					}
					if (!cbxRecrVanillaClass.Checked)
					{
						if (chosenname == "elincia" | chosenname == "sanaki" | chosenname == "lehran" | chosenname == "izuka" | chosenname == "blackknight" | chosenname == "ashera")
						{
							textBox1.Text = "Ike cannot be changed into " + char.ToUpper(chosenname[0]) + chosenname.Substring(1) + " because their class does not have a Tier 2 equivalent. Use class randomization to allow Ike to become this character.";
							return false;
						}
						if (chosenname == "micaiah" | chosenname == "edward" | chosenname == "leonardo" | chosenname == "nolan" | chosenname == "laura" | chosenname == "ilyana" |
							chosenname == "aran" | chosenname == "meg" | chosenname == "jill" | chosenname == "fiona")
						{
							textBox1.Text = "Ike cannot be changed into " + char.ToUpper(chosenname[0]) + chosenname.Substring(1) + " because that character cannot be changed into a T2 class. Use class randomization to allow Ike to become this character.";
							return false;
						}
					}
				}
			}
			if (cbxChooseElincia.Checked)
			{
				string chosenname = characters[comboElincia.SelectedIndex].Name;
				if (chosenname == "bastian" | chosenname == "volke")
				{
					textBox1.Text = "Elincia cannot be changed into Bastian or Volke due to a hit rate bug";
					return false;
				}
				else if (!cbxClassRand.Checked & !cbxClassSwap.Checked)
				{
					if (chosenname == "rafiel" | chosenname == "reyson" | chosenname == "leanne")
					{
						textBox1.Text = "Elincia cannot be changed into heron characters without class randomization";
						return false;
					}

					if (!cbxRecrVanillaClass.Checked)
					{
						if (chosenname != "nailah" & chosenname != "elincia" & chosenname != "naesala" & chosenname != "sanaki" & chosenname != "tibarn" &
							chosenname != "stefan" & chosenname != "oliver" & chosenname != "caineghis" & chosenname != "giffca" & chosenname != "kurthnaga" &
							chosenname != "ena" & chosenname != "renning" & chosenname != "nasir" & chosenname != "gareth" & chosenname != "lehran" &
							chosenname != "hetzel" & chosenname != "levail" & chosenname != "lekain" & chosenname != "zelgius" & chosenname != "dheginsea" & chosenname != "sephiran")
						{
							textBox1.Text = "Elincia cannot be changed into " + char.ToUpper(chosenname[0]) + chosenname.Substring(1) + " because that character cannot be changed into a T3 class. Use class randomization to allow Elincia to become this character.";
							return false;
						}
					}

				}
			}

			// if we got this far, everything is good
			return (true);
		}

		// checks the user selections for class weights on front panel
		private bool checkWeights()
		{

			// lords need a class selected
			if (cbxClassRand.Checked & ((cbxMicClass.Checked & comboMicClass.SelectedItem.ToString() == "-") |
										(cbxIkeClass.Checked & comboIkeClass.SelectedItem.ToString() == "-") |
										(cbxElinciaClass.Checked & comboElinciaClass.SelectedItem.ToString() == "-")))
			{
				textBox1.Text = "Please select valid class for Ike, Micaiah and/or Elincia";
				return (false);
			}

			// check to make sure at least one class weight overall is greater than 0
			if (cbxClassRand.Checked & (
				(radioBeast0.Checked & radioBird0.Checked & radioDragon0.Checked) &
				(radioArmor0.Checked & radioCav0.Checked & radioFly0.Checked & radioInfantry0.Checked & radioMages0.Checked)))
			{
				textBox1.Text = "At least one class type must have a non-zero weight!";
				return (false);
			}
			// check to make sure at least one not-dragon class weight overall is greater than 0
			if (cbxClassRand.Checked & (
				(radioBeast0.Checked & radioBird0.Checked) &
				(radioArmor0.Checked & radioCav0.Checked & radioFly0.Checked & radioInfantry0.Checked & radioMages0.Checked)))
			{
				textBox1.Text = "Early game beorc cannot turn into dragons. Please select at least one other class type to have a non-zero weight.";
				return (false);
			}
			// check to make sure at least one class weight is greater than 0 for both races
			if (cbxClassRand.Checked & comboClassOptions.SelectedIndex == 0 & (
				(radioBeast0.Checked & radioBird0.Checked & radioDragon0.Checked) |
				(radioArmor0.Checked & radioCav0.Checked & radioFly0.Checked & radioInfantry0.Checked & radioMages0.Checked)))
			{
				textBox1.Text = "'Basic' class randomization is impossible without both laguz and beorc classes!";
				return (false);
			}

			if (cbxRandRecr.Checked & cbxClassRand.Checked & comboClassOptions.SelectedIndex == 0 &
				(radioBeast0.Checked & radioBird0.Checked))
			{
				textBox1.Text = "Early game units cannot turn into dragons. Please select at least one other laguz class type to have a non-zero weight.";
				return (false);
			}
			return (true);
		}

		// extracts FE10Data.cms, turns datafile, all chapter files, and shop files into .csv files
		private void ExtractFiles()
		{
			textBox1.Text = "Extracting Files";
			Application.DoEvents();

			string compressed = dataLocation + "\\FE10Data.cms";
			string decompressed = dataLocation + "\\FE10Data.cms.decompressed";

			// decompress cms file
			List<byte> compressedbytes = new List<byte>();
			using (var stream = new System.IO.FileStream(compressed, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
			{
				stream.Position = 0;
				for (int i = 0; i < stream.Length; i++)
				{
					compressedbytes.Add((byte)stream.ReadByte());
				}
			}
			byte[] decompressedbytes = LZ77.Decompress(compressedbytes.ToArray(), 0);

			using (var stream = new System.IO.FileStream(decompressed, System.IO.FileMode.Create,
					System.IO.FileAccess.ReadWrite))
			{
				stream.Position = 0;
				foreach (byte data in decompressedbytes)
					stream.WriteByte(data);
			}

			// delete old temp files
			string tempfolder = file + "\\assets\\temp";
			foreach (string path in Directory.GetFiles(tempfolder))
				File.Delete(path);

			// extract to csv files
			Directory.CreateDirectory(tempfolder + "\\data");
			Directory.CreateDirectory(tempfolder + "\\chapter");
			//Directory.CreateDirectory(tempfolder + "\\anim");
			//Directory.CreateDirectory(tempfolder + "\\battle");

			//FE10ExtractCompress.ExtractFE10Anim(file + "\\assets\\gamedata" + "\\FE10Anim.cms.decompressed", tempfolder + "\\anim");
			//FE10ExtractCompress.ExtractFE10Battle(file + "\\assets\\gamedata" + "\\FE10Battle.cms.decompressed", tempfolder + "\\battle");

			//FE10ExtractCompress.CompressFE10Anim(file + "\\assets\\gamedata" + "\\FE10Anim.cms.decompressed", tempfolder + "\\anim");
			//FE10ExtractCompress.CompressFE10Battle(file + "\\assets\\gamedata" + "\\FE10Battle.cms.decompressed", tempfolder + "\\battle");

			FE10ExtractCompress.ExtractFE10Data(decompressed, tempfolder + "\\data");
			// csv to classes
			foreach (string path in Directory.GetFiles(tempfolder + "\\data"))
			{
				if (Path.GetFileName(path).Contains("PersonData"))
					CharacterData = new FEDataFile(path);
				else if (Path.GetFileName(path).Contains("JobData"))
					ClassData = new FEDataFile(path);
				else if (Path.GetFileName(path).Contains("ItemData"))
					ItemData = new FEDataFile(path);
				else if (Path.GetFileName(path).Contains("SkillData"))
					SkillData = new FEDataFile(path);
				else if (Path.GetFileName(path).Contains("TerrainData"))
					TerrData = new FEDataFile(path);
				else if (Path.GetFileName(path).Contains("ChapterData"))
					MapData = new FEDataFile(path);
			}

			// extract dispos files to csv
			ChapterData = new DisposFile[chapters.Length];
			for (int i = 0; i < chapters.Length; i++)
			{
				string chapterpath = dataLocation + "\\zmap\\bmap0" + chapters[i] + "\\dispos_h.bin";
				if (chapters[i] == "emap407c") // ena, kurth, cain, and giffca appear in a weird file
					chapterpath = dataLocation + "\\zmap\\emap0407c\\dispos_c.bin";
				else if (chapters[i] == "emap407d") // location of characters locked to tower
					chapterpath = dataLocation + "\\zmap\\emap0407d\\dispos_c.bin";
				string outpath = tempfolder + "\\chapter\\" + chapters[i] + ".csv";
				FE10ExtractCompress.ExtractDispos(chapterpath, outpath);
				// csv to class
				ChapterData[i] = new DisposFile(outpath);
			}

			// extract shop file
			string shopfile = dataLocation + "\\Shop\\shopitem_h.bin";
			string outfile = tempfolder + "\\Shop.csv";
			FE10ExtractCompress.ExtractShopfile(shopfile, outfile);
			shopfileLocation = outfile;
		}

		// uses thane98's Exalt script editor to decompile scripts into usable txt files
		private void ExaltScripts()
		{
			// copy scripts to temp folder
			string scriptloc = dataLocation + "\\Scripts\\";
			string outloc = file + "\\assets\\temp\\script\\";

			if (!Directory.Exists(outloc))
				Directory.CreateDirectory(outloc);

			string[] scripts = Directory.GetFiles(scriptloc);
			List<string> newscripts = new List<string>();

			for (int i = 0; i < scripts.Length; i++)
			{
				string tempfilename = Path.GetFileName(scripts[i]);
				if (tempfilename.StartsWith("C") & !tempfilename.Contains("C0000") & !tempfilename.Contains("C0401") & !tempfilename.Contains("CFINAL"))
				{
					newscripts.Add(outloc + tempfilename);
					File.Copy(scripts[i], outloc + tempfilename, true);
				}
			}

			script_exl = new string[newscripts.Count];

			// write batch file
			StreamWriter writer = new StreamWriter(outloc + "decompile.bat");
			for (int i = 0; i < newscripts.Count; i++)
			{
				writer.WriteLine("\"exalt-cli.exe\" -g FE10 decompile \"" + newscripts[i] + "\"");
				// save locations of decompiled files
				script_exl[i] = newscripts[i].Replace(".cmb", ".exl");
			}
			writer.Close();

			// run batch file
			Process p = new Process();
			p.StartInfo.WorkingDirectory = outloc;
			p.StartInfo.FileName = "decompile.bat";
			p.StartInfo.CreateNoWindow = false;
			p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
			p.Start();
			p.WaitForExit();

		}

		// initializes variables and reads in characterdata and classdata
		private void Initialize()
		{
			textBox1.Text = "Initializing";
			Application.DoEvents();


			// initialize character information
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\CharacterInfo.csv");
			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			characters = new Character[lines.Length - 1];
			// loop through all characters, skipping header line
			for (int i = 0; i < characters.Length; i++)
			{
				characters[i] = new Character(lines[i + 1]);

				// modifications based on selections
				if (cbxTormodT3.Checked)
				{
					if (characters[i].Name == "tormod")
					{
						characters[i].Tier = "d";
						characters[i].Level = 1;
					}
					else if (characters[i].Name == "maurim" | characters[i].Name == "vika")
					{
						characters[i].Tier = "d";
						characters[i].Level = 21;
					}
				}
				if (cbxClassPatch.Checked)
				{
					// change vanilla classes of certain characters
					if (characters[i].Name == "sothe") // sothe trickster
						characters[i].VanillaClass = 73;
					else if (characters[i].Name == "jill") // jill malig kn
						characters[i].VanillaClass = 82;
					else if (characters[i].Name == "zihark") // zihark dread fighter
						characters[i].VanillaClass = 69;
					else if (characters[i].Name == "fiona") // fiona thnder kn
						characters[i].VanillaClass = 77;
					else if (characters[i].Name == "nephenee") // nephenee lancer
						characters[i].VanillaClass = 71;
					else if (characters[i].Name == "kieran") // kieran fire pal
						characters[i].VanillaClass = 80;
					else if (characters[i].Name == "calill") // calill bartendress
						characters[i].VanillaClass = 75;
					else if (characters[i].Name == "oscar") // oscar thunder pal
						characters[i].VanillaClass = 78;
					else if (characters[i].Name == "rhys") // rhys warmonk
						characters[i].VanillaClass = 85;
					else if (characters[i].Name == "stefan") // stefan yasha
						characters[i].VanillaClass = 70;
					else if (characters[i].Name == "oliver") // oliver crusader
						characters[i].VanillaClass = 86;
				}
				if (cbxLetheMordy.Checked)
				{
					// move lethe and mordy to 2-1
					if (characters[i].Name == "lethe" | characters[i].Name == "mordecai")
						characters[i].Chapter = "202";
				}

			}


			// initialize class information
			dataReader = new System.IO.StreamReader(file + "\\assets\\ClassInfo.csv");
			lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			classes = new Job[lines.Length - 1];
			// loop through all classes, skipping header line
			for (int i = 0; i < classes.Length; i++)
			{
				classes[i] = new Job(lines[i + 1]);

				// modifications based on selections
				if (cbxClassPatch.Checked)
				{
					// rogue and whisper will use female classes
					if (classes[i].Name == "rogue")
						classes[i].JID = "JID_ROGUE_F";
					else if (classes[i].Name == "whisper")
						classes[i].JID = "JID_ESPION_F";
					// reaver/assassin can use bows
					else if (classes[i].Name == "reaver")
						classes[i].Weapon_P = "A;B;G";
					else if (classes[i].Name == "assassin")
					{
						classes[i].Weapon_P = "K;B;K";
						classes[i].Weapon_E = "K;B";
					}
					// queen can use light
					else if (classes[i].Name == "queen")
					{
						classes[i].Weapon_P = "S;H;M";
						classes[i].Weapon_E = "S;M";
					}
					// cleric/valkyrie can use wind
					else if (classes[i].Name == "cleric")
						classes[i].Weapon_P = "S;H;W";
					else if (classes[i].Name == "valkyrie")
						classes[i].Weapon_P = "S;H;W";
					// windarchsage turns into son of ashnard
					else if (classes[i].Name == "windarchsage")
					{
						classes[i].Name = "sonofashnard";
						classes[i].Weapon_P = "W;S;F";
						classes[i].Weapon_E = "W;S";
						classes[i].Classtype_P = "F";
						classes[i].Classtype_E = "F";
					}

					// change animation pointers
					if (classes[i].Name == "myrmidon" | classes[i].Name == "swordmaster" | classes[i].Name == "trueblade")
						classes[i].Animation = "0;2";
					else if (classes[i].Name == "soldier" | classes[i].Name == "halberdier" | classes[i].Name == "sentinal")
						classes[i].Animation = "11;12";
					else if (classes[i].Name == "lncknight" | classes[i].Name == "lncpaladin" | classes[i].Name == "lncslvknight")
						classes[i].Animation = "18;18";
					else if (classes[i].Name == "dragknight" | classes[i].Name == "dragmaster" | classes[i].Name == "dragonlord")
						classes[i].Animation = "28;28";
					else if (classes[i].Name == "thief" | classes[i].Name == "rogue" | classes[i].Name == "whisper")
						classes[i].Animation = "9;9";
					else if (classes[i].Name == "firemage" | classes[i].Name == "firesage" | classes[i].Name == "firearchsage")
						classes[i].Animation = "33;33";
					else if (classes[i].Name == "thdrmage" | classes[i].Name == "thdrsage" | classes[i].Name == "thdrarchsage")
						classes[i].Animation = "35;36";
					else if (classes[i].Name == "windmage" | classes[i].Name == "windsage" | classes[i].Name == "windarchsage")
						classes[i].Animation = "37;37";
					else if (classes[i].Name == "priest" | classes[i].Name == "bishop" | classes[i].Name == "saint")
						classes[i].Animation = "41;42";
				}
				else
				{
					// disable custom classes
					if (i > 68 & i < 87)
					{
						classes[i].Classtype_P = "x";
						classes[i].Classtype_E = "x";
					}
				}
				if (cbxDarkMag.Checked)
				{
					if (classes[i].Name == "lightsage" | classes[i].Name == "lightpriestess")
					{
						classes[i].Weapon_P = "M;H;D";
						classes[i].Weapon_E = "M;D";
					}
				}
				if (cbxFireMag.Checked)
				{
					if (classes[i].Name == "druid")
					{
						classes[i].Weapon_P = "D;F;D";
						classes[i].Weapon_E = "D;F";
					}
					else if (classes[i].Name == "summoner")
					{
						classes[i].Weapon_P = "D;F;H";
						classes[i].Weapon_E = "D;F";
					}
				}
			}

			// list of chapters in game
			dataReader = new StreamReader(file + "\\assets\\chapterlist.txt");
			chapters = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			// jarod class for 1-10 and 1-11
			jarodclass = -1;
			// initialize variables
			heronNumber = 0;
			forgeOutput = "";
			eventItemsOutput = "";
			bargainOutput = "";
			randEnemyOutput = "";

			// reset error flag
			errorflag = 0;
			// set number of units to change
			totalUnitNumber = 72;
			// generate randomizer with seed
			random = new Random(Convert.ToInt32(numericSeed.Value));
			// very important boolean that is used many times in the program
			classeschanged = cbxClassRand.Checked | cbxClassSwap.Checked | (cbxRandRecr.Checked & !cbxRecrVanillaClass.Checked);
		}

		// modifies certain files in ISO so dolphin will treat it as a different game
		private void ChangeISO_ID()
		{
			string headerfile = dataLocation.Remove(dataLocation.Length - 5, 5) + "disc\\header.bin";
			string bootfile = sysLocation + "\\boot.bin";
			string tmdfile = dataLocation.Remove(dataLocation.Length - 5, 5) + "tmd.bin";
			string ticketfile = dataLocation.Remove(dataLocation.Length - 5, 5) + "ticket.bin";
			string openingfile = dataLocation + "\\opening.bnr";

			string[] files2change = new string[4] { headerfile, bootfile, tmdfile, ticketfile };
			int[] places2change = new int[4] { 2, 2, 402, 482 };

			// change id
			for (int i = 0; i < 4; i++)
			{
				using (var stream = new System.IO.FileStream(files2change[i], System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
				{
					stream.Position = places2change[i];
					stream.WriteByte(0x58); // write X here to make game ID "RFXE01"
				}
			}

			// change name
			using (var stream = new System.IO.FileStream(openingfile, System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
			{
				string namestring = "Fire Emblem: Random Dawn";
				byte[] namebytes = Encoding.ASCII.GetBytes(namestring);
				stream.Position = 177;
				foreach (byte letter in namebytes)
				{
					stream.WriteByte(letter);
					stream.WriteByte(0x00);
				}
				stream.WriteByte(0x00);
			}

			// change picture
			string sourcePath = file + "\\assets\\gamedata\\banner_en.tpl";
			string targetPath = dataLocation + "\\etc\\banner_en.tpl";
			System.IO.File.Copy(sourcePath, targetPath, true);
		}

		// checks if selected ISO is vanilla, if not, attempts to copy back-up vanilla files (if they exist)
		private void CheckVanilla()
		{
			// these files will be modified with version 3.0.0+ of the randomizer
			string headerfile = dataLocation.Remove(dataLocation.Length - 5, 5) + "disc\\header.bin";
			string bootfile = sysLocation + "\\boot.bin";
			string tmdfile = dataLocation.Remove(dataLocation.Length - 5, 5) + "tmd.bin";
			string ticketfile = dataLocation.Remove(dataLocation.Length - 5, 5) + "ticket.bin";

			string[] files2check = new string[4] { headerfile, bootfile, tmdfile, ticketfile };
			int[] places2check = new int[4] { 2, 2, 402, 482 };

			notvanilla = false;
			for (int i = 0; i < 4; i++)
			{
				using (var stream = new System.IO.FileStream(files2check[i], System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
				{
					stream.Position = places2check[i];
					notvanilla = notvanilla | (stream.ReadByte() == 0x58); // should be an X here if the ISO has been randomized
				}
			}


			// this file will have abbreviated IDs for earlier versions of the randomizer
			string scriptfile = dataLocation + "\\Scripts\\C0101.cmb";
			using (var stream = new System.IO.FileStream(scriptfile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
			{
				stream.Position = 881;
				if (stream.ReadByte() == 0x41)
					notvanilla = notvanilla;
				else
					notvanilla = true;
			}

			// if not vanilla, check if vanilla files have been backed up
			if (notvanilla)
			{
				string vanillabackup = file + "\\assets\\vanillafiles";
				if (gameVersion == 0)
					vanillabackup += "\\NTSC11\\";
				else if (gameVersion == 1)
					vanillabackup += "\\NTSC10\\";
				else
					vanillabackup += "\\PAL\\";
				string[] vanillafiles;
				if (Directory.Exists(vanillabackup))
					vanillafiles = getRecursiveFiles(vanillabackup);
				else
					vanillafiles = new string[0];
				if (vanillafiles.Length < 100)
				{
					// no backup, warn user
					rerandomized = true;
					textBox1.Text = "WARNING: These files have already been randomized and the randomizer does not have copies of clean files to overwrite with. Please open a clean ISO, otherwise re-randomize at your own risk.";
					textBox1.BackColor = Color.LightPink;
					Application.DoEvents();
				}
				else
					rerandomized = false;
			}
			else
			{
				// keep a copy of vanilla files in the randomizer folder
				BackupVanillaFiles();

				rerandomized = false;
			}
		}

		// backs up files that the randomizer will edit (only does this if the selected ISO is vanilla)
		private void BackupVanillaFiles()
		{
			textBox1.Text = "Loading ISO";
			Application.DoEvents();
			//string dataLocation = "C:\\Users\\josia\\Documents\\Fire Emblem Radiant Dawn CLEAN\\DATA\\files";
			//string file = "C:\\Users\\josia\\Documents\\test";
			//int gameVersion = 0;

			// back up files to \\assets\\vanillafiles\\[gameversion]\\
			string isopath = dataLocation.Remove(dataLocation.Length - 5, 5);
			string backuppath = file + "\\assets\\vanillafiles\\";
			if (gameVersion == 0)
				backuppath += "NTSC11\\";
			else if (gameVersion == 1)
				backuppath += "NTSC10\\";
			else
				backuppath += "PAL\\";

			if (!Directory.Exists(backuppath))
			{
				Directory.CreateDirectory(backuppath);
			}

			// list of files edited by randomizer
			string[] files2copy = new string[68] { "tmd.bin", "ticket.bin","disc\\header.bin","sys\\boot.bin","sys\\main.dol", "files\\Mess\\e_common.m", "files\\etc\\banner_en.tpl",
													"files\\opening.bnr","files\\FE10Data.cms","files\\FE10Anim.cms","files\\FE10Battle.cms","files\\Face\\facedata.bin",
													"files\\Face\\wide\\facedata.bin","files\\Scripts\\","files\\Shop\\", "files\\zmap\\", "files\\ymu\\sworder2_z",
													"files\\ymu\\sworder3_z","files\\ymu\\soldier2_f","files\\ymu\\soldier3_f","files\\ymu\\thief2_s","files\\ymu\\thief3_s",
													"files\\ymu\\knight1_sp","files\\ymu\\knight2_f","files\\ymu\\knight3_f","files\\ymu\\knight2_ax_k","files\\ymu\\knight3_k",
													"files\\ymu\\erincia_p","files\\ymu\\d_knight1_j","files\\ymu\\d_knight2_j","files\\ymu\\d_knight3_j","files\\ymu\\cleric",
													"files\\ymu\\cleric2","files\\ymu\\mage3_s","files\\ymu\\sworder3_s","files\\ymu\\knight2_o","files\\ymu\\knight3_o",
													"files\\zu\\swoUz","files\\zu\\swoSz","files\\zu\\solUn","files\\zu\\solSn","files\\zu\\figSb","files\\zu\\thiUs",
													"files\\zu\\thiSs","files\\zu\\volke","files\\zu\\magfUc","files\\zu\\magfSc","files\\zu\\knilLf",
													"files\\zu\\knilUf","files\\zu\\silSf","files\\zu\\kniaUk","files\\zu\\golSk","files\\zu\\erinc","files\\zu\\dknLj",
													"files\\zu\\dknUj","files\\zu\\dknSj","files\\zu\\priUk","files\\zu\\priSk","files\\zu\\misU","files\\zu\\misS",
													"files\\zu\\magwSs","files\\zu\\swoSs","files\\zu\\figSn","files\\zu\\knilUo","files\\zu\\silSo","files\\zu\\priSo",
													"files\\window\\icon.cms", "files\\window\\icon_wide.cms"};

			List<string> copyfrom = new List<string>();
			List<string> copyto = new List<string>();

			foreach (string copyloc in files2copy)
			{
				// get list of all files to move
				bool isfile = Path.HasExtension(isopath + copyloc);
				if (isfile)
				{
					copyfrom.Add(isopath + copyloc);
					copyto.Add(backuppath + copyloc);
				}
				else
				{
					string[] thesefiles = getRecursiveFiles(isopath + copyloc);
					foreach (string thisfile in thesefiles)
					{
						copyfrom.Add(thisfile);
						copyto.Add(backuppath + thisfile.Replace(isopath, ""));
					}
				}
			}

			// loops through each file and copy
			for (int i = 0; i < copyfrom.Count; i++)
			{
				// make sure folders in backup exist
				string[] folders = copyto[i].Split('\\');
				for (int j = 1; j < folders.Length; j++)
				{
					string temppath = folders[0];
					for (int k = 1; k < j; k++)
						temppath += "\\" + folders[k];
					if (!Directory.Exists(temppath))
						Directory.CreateDirectory(temppath);
				}

				if (!File.Exists(copyto[i]))
					System.IO.File.Copy(copyfrom[i], copyto[i], false);
			}
		}

		// overwrite unclean ISO with backed-up vanilla files
		private void CopyVanillaFiles()
		{
			textBox1.Text = "Vanilla-izing.";
			Application.DoEvents();

			string isopath = dataLocation.Remove(dataLocation.Length - 5, 5);
			string backuppath = file + "\\assets\\vanillafiles\\";
			if (gameVersion == 0)
				backuppath += "NTSC11\\";
			else if (gameVersion == 1)
				backuppath += "NTSC10\\";
			else
				backuppath += "PAL\\";

			// list of files edited by randomizer
			string[] files2copy = getRecursiveFiles(backuppath);

			List<string> copyfrom = new List<string>();
			List<string> copyto = new List<string>();

			foreach (string copyloc in files2copy)
			{
				File.Copy(copyloc, isopath + copyloc.Replace(backuppath, ""), true);
			}
		}

		// moves edited files depending on user selections
		private void MoveFiles()
		{
			textBox1.Text = "Messing with your files";
			Application.DoEvents();

			string sourcePath, targetPath, sourcefile, targetfile;

			if (cbxForge.Checked)
			{
				// forge stuff
				sourcePath = file + "\\assets\\forgedata\\";
				targetPath = dataLocation + "\\xwp\\forge\\";
				foreach (string path in System.IO.Directory.GetFiles(sourcePath))
					System.IO.File.Copy(path, targetPath + Path.GetFileName(path), true);
			}
			if (cbxWinCon.Checked)
			{
				// moves edited script files to proper folder (these are explict instead of a loop through all files due to
				// some people having old depreciated script files from an earlier version of the randomizer)
				sourcePath = file + "\\assets\\scriptdata\\";
				targetPath = dataLocation + "\\Scripts\\";

				sourcefile = sourcePath + "C0401.cmb";
				targetfile = targetPath + "C0401.cmb";
				System.IO.File.Copy(sourcefile, targetfile, true);

				/*
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
				*/
			}
			if (cbxWeapPatch.Checked)
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
			if (cbxClassPatch.Checked)
			{
				// moves FE10Battle and FE10Animation files
				sourcePath = file + "\\assets\\gamedata\\";
				targetPath = dataLocation + "\\";

				sourcefile = sourcePath + "FE10Anim.cms";
				targetfile = targetPath + "FE10Anim.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
				sourcefile = sourcePath + "FE10Battle.cms";
				targetfile = targetPath + "FE10Battle.cms";
				System.IO.File.Copy(sourcefile, targetfile, true);
			}
			if (aprilFools & errorflag == 0)
			{
				// camilla time
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
		}

		#endregion

		// *************************************************************************************** TOPLEVEL FUNCTIONS
		#region

		private void Randomize()
		{
			// choose a class for each character and save in array
			RandomizeClass();

			// save changes to dispos file
			SavePlayertoDispos();

			// saves random recruitment, promotion changes, animation changes, and other class randomization things to fe10data
			SaveFE10Data_ClassRand();

			// weapon randomization
			if (cbxRandWeap.Checked)
				weaponRandomizer();

			// stat randomization
			Stats();

			// enemy changes
			SaveEnemiestoDispos();
			SaveFE10Data_Enemies();

			// other randomizations
			variousCharacterRandomizers();

			// shop stuff
			Shop();

			// modifying dispos and data files based on user selections
			DisposModifications();
			FEDataModifications();

			// text, script, and main.dol changes, but only if this is a real randomization
			if (!cbxTrialMode.Checked)
			{
				ScriptModifications();
				TextModifications();
				MaindolModifications();
				if (cbxRandRecr.Checked | cbxChooseMic.Checked | cbxChooseIke.Checked | cbxChooseElincia.Checked)
					RecrFaceSwap();
			}

		}
		private void DataFileModifications()
		{
			textBox1.Text = "Initializing Data File";
			Application.DoEvents();

			// general base stat modifications for classes and characters
			if (classeschanged | cbxRandEnemy.Checked | cbxRandAllies.Checked | cbxRandBosses.Checked)
				baseStatChanges();

			// remove fiona's first animation due to wiibafu fucking it up sometimes
			if (true)
			{
				string[] fionaanim = CharacterData.ReadStringArray("PID_FRIEDA", "Animations");
				fionaanim[0] = fionaanim[1];
				CharacterData.Write("PID_FRIEDA", "Animations", fionaanim);
			}

			// class patch
			if (cbxClassPatch.Checked)
			{
				LM73classPatch();
				LaguzMJID();
				ClassPatch_statchanges();
				if (!cbxClassRand.Checked & !cbxClassSwap.Checked & !cbxRandRecr.Checked)
					ClassPatch_changeVanillaClasses();
			}
			// weapon patch
			if (cbxWeapPatch.Checked)
				LM73WeapPatch();
			// magic patch
			if (cbxMagicPatch.Checked)
				LM73MagicPatch();
			// knife change
			if (cbxKnifeCrit.Checked)
				knifeModifier();

			// add lethe and mordy to 2-1
			if (cbxLetheMordy.Checked)
				addLetheMordyEarly();
		}

		private void RandomizeClass()
		{
			textBox1.Text = "Changing classes";
			Application.DoEvents();

			if (cbxRandRecr.Checked | cbxChooseIke.Checked | cbxChooseMic.Checked | cbxChooseElincia.Checked)
				recruitmentOrderRando();

			if (cbxClassRand.Checked)
				chooseRandClasses();

			else if (cbxClassSwap.Checked)
				chooseSwappedClasses();

			// set lord classes
			if (cbxIkeClass.Checked)
			{
				string[] strings = comboIkeClass.SelectedItem.ToString().Split(' ');
				characters[34].NewClass = Convert.ToInt32(strings[0]);
			}
			if (cbxMicClass.Checked)
			{
				string[] strings = comboMicClass.SelectedItem.ToString().Split(' ');
				characters[0].NewClass = Convert.ToInt32(strings[0]);
			}
			if (cbxElinciaClass.Checked)
			{
				string[] strings = comboElinciaClass.SelectedItem.ToString().Split(' ');
				characters[18].NewClass = Convert.ToInt32(strings[0]);
			}
		}

		// stats hub, controls what functions to run
		private void Stats()
		{
			textBox1.Text = "Calculating busted stats";
			Application.DoEvents();

			// growths
			if (cbxGrowthRand.Checked | cbxZeroGrowths.Checked)
				growthRateModifier();
			else if (cbxGrowthShuffle.Checked)
				growthShuffle();
			// class bases
			if (cbxRandClassBases.Checked | cbxShuffleClassBases.Checked)
				classBaseStats();
			// character bases
			if (cbxRandBases.Checked)
				randBaseStats();
			else if (cbxShuffleBases.Checked)
				shuffleBaseStats();
			// swap str/mag if necessary
			if (cbxStrMag.Checked)
				strMagSwap();
			// class stat caps
			if (cbxStatCaps.Checked | cbxStatCapDev.Checked | cbxStatCapFlat.Checked)
				statCapChanges();
		}

		// shop randomizers
		private void Shop()
		{
			textBox1.Text = "Editing Shop Files";
			Application.DoEvents();

			StreamReader dataReader = new StreamReader(file + "\\assets\\dropshopitems.csv");
			string[] allitems = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			dataReader = new StreamReader(shopfileLocation);
			string[] shoplines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			// randomizes items found in each chapter's armory and shop
			if (cbxRandShop.Checked | cbxBargains.Checked | cbxForge.Checked)
			{
				for (int i = 0; i < shoplines.Length; i++)
				{
					if (shoplines[i].StartsWith("WSHOP") & cbxRandShop.Checked)
					{ // armory
						i += 2; // skip header
						string[] chosenitems = chooseShopItems(shoplines, allitems, "WSHOP", i);
						for (int j = 0; j < chosenitems.Length; j++)
						{
							string[] ShopData = shoplines[i].Split(',');
							ShopData[0] = chosenitems[j];
							shoplines[i] = String.Join(",", ShopData);
							i++;
						}
						i--;
					}
					else if (shoplines[i].StartsWith("ISHOP") & (cbxRandShop.Checked | cbxBargains.Checked))
					{ // shop
						if (cbxBargains.Checked)
							bargainOutput += ";" + shoplines[i].Split('_')[2];
						i += 2; // skip header
						string[] chosenitems = chooseShopItems(shoplines, allitems, "ISHOP", i);

						for (int j = 0; j < chosenitems.Length; j++)
						{
							if (chosenitems[j] != "")
							{
								string[] ShopData = shoplines[i].Split(',');
								ShopData[0] = chosenitems[j];
								shoplines[i] = String.Join(",", ShopData);
							}
							i++;
						}
						i--;
					}
					else if (shoplines[i].StartsWith("FSHOP_ITEMS") & cbxForge.Checked)
					{ // forge
						forgeOutput += ";" + shoplines[i].Split('_')[2];
						i += 2; // skip header
						string[] chosenforge = chooseForgeItems(shoplines, allitems, i);
						for (int j = 0; j < chosenforge.Length; j++)
						{
							while (!shoplines[i].Contains("IID_"))
								i++;
							string[] ShopData = shoplines[i].Split(',');
							ShopData[2] = chosenforge[j];
							forgeOutput += "," + chosenforge[j];
							shoplines[i] = String.Join(",", ShopData);
							i++;
						}
						i--;
					}
					else if (shoplines[i].StartsWith("FSHOP_CARDs"))
					{
						break; // possible code here for forge card randomization
					}
				}

				// save back to csv
				StreamWriter writer = new StreamWriter(shopfileLocation);
				for (int i = 0; i < shoplines.Length; i++)
					writer.WriteLine(shoplines[i]);
				writer.Close();
			}
			// forge isn't randomized, but classes are changed/new classes added - guarantees weapon types in forge (adds Worm to all forges)
			if ((classeschanged | cbxClassPatch.Checked) & !cbxForge.Checked)
			{
				for (int i = 0; i < shoplines.Length; i++)
				{
					if (shoplines[i].Contains("MIK_MG") & !shoplines[i].Contains("IID_"))
					{ // magic at forge
						if (shoplines[i].Contains("MDV_FI"))
						{
							string[] ShopData = shoplines[i].Split(',');
							ShopData[2] = "IID_FIRE";
							shoplines[i] = String.Join(",", ShopData);
						}
						else if (shoplines[i].Contains("MDV_TH"))
						{
							string[] ShopData = shoplines[i].Split(',');
							ShopData[2] = "IID_THUNDER";
							shoplines[i] = String.Join(",", ShopData);
						}
						else if (shoplines[i].Contains("MDV_WD"))
						{
							string[] ShopData = shoplines[i].Split(',');
							ShopData[2] = "IID_WIND";
							shoplines[i] = String.Join(",", ShopData);
						}
						else if (shoplines[i].Contains("MDV_LIT"))
						{
							string[] ShopData = shoplines[i].Split(',');
							ShopData[2] = "IID_LIGHT";
							shoplines[i] = String.Join(",", ShopData);
						}
						else if (shoplines[i].Contains("MDV_D"))
						{
							string[] ShopData = shoplines[i].Split(',');
							ShopData[2] = "IID_WORM";
							shoplines[i] = String.Join(",", ShopData);
						}
					}
					else if (shoplines[i].StartsWith("FSHOP_CARDs"))
					{
						break;
					}
				}
				// save back to csv
				StreamWriter writer = new StreamWriter(shopfileLocation);
				for (int i = 0; i < shoplines.Length; i++)
					writer.WriteLine(shoplines[i]);
				writer.Close();
			}
		}

		#endregion

		// *************************************************************************************** CLASS & CHARACTER FUNCTIONS
		#region

		// selects recruitment order
		private void recruitmentOrderRando()
		{
			textBox1.Text = "Swapping recruitment";
			Application.DoEvents();

			// read in recruitment class data
			StreamReader dataReader;
			if (cbxClassPatch.Checked == true)
				dataReader = new System.IO.StreamReader(file + "\\assets\\classpatch\\RandoRecruitData.csv");
			else
				dataReader = new System.IO.StreamReader(file + "\\assets\\RandoRecruitData.csv");

			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < characters.Length; i++)
			{
				string[] split = lines[i + 1].Split(',');
				characters[i].RecrClasses = new int[4] { Convert.ToInt32(split[2]), Convert.ToInt32(split[3]), Convert.ToInt32(split[4]), Convert.ToInt32(split[5]) };
			}

			// save growths for enemy characters
			dataReader = new StreamReader(file + "\\assets\\RecrEnemyGrowths.csv");
			lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < characters.Length; i++)
			{
				for (int j = 1; j < lines.Length; j++)
				{
					if (characters[i].PID == lines[j].Split(',')[0])
					{
						int[] newgrowths = new int[8];
						string[] split = lines[j].Split(',');
						for (int k = 0; k < newgrowths.Length; k++)
						{
							newgrowths[k] = Convert.ToInt32(split[k + 1]);
						}
						CharacterData.Write(characters[i].PID, "Growths", newgrowths);
					}
				}
			}

			// 72 playable characters
			int totalchars = 72;
			if (cbxEnemyRecruit.Checked == true)
			{
				// include enemy characters, but don't include BK or ashera if class rando is off
				if (cbxClassRand.Checked | cbxClassSwap.Checked | cbxRecrVanillaClass.Checked)
					totalchars = 86;
				else
					totalchars = 84;
			}

			// randomize recruitment order
			for (int i = 0; i < totalchars; i++)
			{
				if (cbxRandRecr.Checked)
				{
					// randomize until j is not i
					int j = random.Next(0, totalchars);
					while (i == j)
						j = random.Next(0, totalchars);
					// swap newrecr of i and j characters
					int temp;
					if (characters[i].NewRecr == -1)
						temp = i;
					else
						temp = characters[i].NewRecr;
					if (characters[j].NewRecr == -1)
						characters[i].NewRecr = j;
					else
						characters[i].NewRecr = characters[j].NewRecr;
					characters[j].NewRecr = temp;
				}
				else
				{
					// no full recruitment randomization, only swapping miccy or ike
					characters[i].NewRecr = i;
				}
			}

			bool finished = false;
			// loop until we find a good selection
			while (!finished)
			{
				string newname;
				bool tryagain = false;
				int problemchild = -1;

				// check for any issues with current selection
				for (int i = 0; i < totalchars; i++)
				{
					newname = characters[characters[i].NewRecr].Name;
					int[] possibleclasses = characters[characters[i].NewRecr].RecrClasses;

					// bastian and volke can cause issues if they're turned into certain characters
					if (characters[i].Name == "elincia" | characters[i].Name == "tormod" | characters[i].Name == "tauroneo" | characters[i].Name == "nephenee" |
						characters[i].Name == "lucia" | characters[i].Name == "geoffrey" | characters[i].Name == "tibarn" | characters[i].Name == "jarod" |
						characters[i].Name == "ludveck" | characters[i].Name == "septimus" | characters[i].Name == "valtome" | characters[i].Name == "numida" |
						characters[i].Name == "izuka" | characters[i].Name == "hetzel" | characters[i].Name == "levail" | characters[i].Name == "lekain" |
						characters[i].Name == "zelgius" | characters[i].Name == "dheginsea" | characters[i].Name == "sephiran" | characters[i].Name == "blackknight" |
						characters[i].Name == "ashera" | characters[i].Name == "micaiah" | characters[i].Name == "ike")
					{
						if (newname == "bastian" | newname == "volke")
							tryagain = true;
					}

					// herons can't replace t3/4 characters
					if ((characters[i].Tier == "c" | characters[i].Tier == "d") & !cbxRecrVanillaClass.Checked)
					{
						if (newname == "rafiel" | newname == "reyson" | newname == "leanne")
							tryagain = true;
					}

					// heron names are limiting to PID - PID_LEARNE and PID_RAFIEL limit the characters to ten, PID_RIEUSION gives twelve
					if (characters[i].PID.Length > 10 & (newname == "rafiel" | newname == "leanne"))
						tryagain = true;
					if (characters[i].PID.Length > 12 & (newname == "reyson"))
						tryagain = true;

					// heather can't be heron character if both herons and thieves can't be randomized
					if (characters[i].Name == "heather" & !cbxRecrVanillaClass.Checked)
					{
						if (!cbxHerons.Checked & cbxThieves.Checked)
							if (newname == "rafiel" | newname == "reyson" | newname == "leanne")
								tryagain = true;
					}

					// some things only matter if we're not randomizing classes
					if (!cbxClassRand.Checked & !cbxClassSwap.Checked & !cbxRecrVanillaClass.Checked)
					{
						// herons can't replace sothe,brom,nephenee,micaiah,ike
						if (characters[i].Name == "sothe" | characters[i].Name == "brom" | characters[i].Name == "nephenee" | characters[i].Name == "micaiah" |
							characters[i].Name == "ike")
						{
							if (newname == "rafiel" | newname == "reyson" | newname == "leanne")
								tryagain = true;
						}
						// micaiah can't be priest
						if (characters[i].Name == "micaiah")
						{
							if (newname == "laura" | newname == "rhys" | newname == "oliver" | newname == "valtome" | newname == "numida" | newname == "hetzel" | newname == "lekain")
								tryagain = true;
						}
						// if BK fight isn't nerfed, ike can't be rogue, raven or magic
						if (characters[i].Name == "ike")
						{
							if (restrictIke)
							{
								if (newname == "micaiah" | newname == "laura" | newname == "sothe" | newname == "ilyana" | newname == "tormod" |
									newname == "heather" | newname == "calill" | newname == "soren" | newname == "rhys" | newname == "sanaki" |
									newname == "pelleas" | newname == "oliver" | newname == "lehran" | newname == "valtome" | newname == "numida" |
									newname == "izuka" | newname == "hetzel" | newname == "lekain" | newname == "sephiran")
									tryagain = true;
							}
						}
						// ranulf and ike can't be mounted if horses can't climb ledges
						if (characters[i].Name == "ranulf" | characters[i].Name == "ike")
						{
							if (!cbxHorseParkour.Checked)
							{
								if (newname == "fiona" | newname == "geoffrey" | newname == "kieran" | newname == "astrid" | newname == "makalov" |
									newname == "titania" | newname == "mist" | newname == "oscar" | newname == "renning")
									tryagain = true;
							}
						}
						// characters can't change into someone in a certain tier due to class
						if (characters[i].Tier == "a" & possibleclasses[0] == 999)
							tryagain = true;
						else if (characters[i].Tier == "b" & possibleclasses[1] == 999)
							tryagain = true;
						else if (characters[i].Tier == "c" & possibleclasses[2] == 999)
							tryagain = true;
						else if (characters[i].Tier == "d" & possibleclasses[3] == 999)
							tryagain = true;
					}

					//  boss classes aren't randomized, boss characters can't end up as impossible classes
					if (cbxEnemyRecruit.Checked & !cbxRandBosses.Checked)
					{
						if (characters[i].Name == "jarod" | characters[i].Name == "ludveck" | characters[i].Name == "septimus" | characters[i].Name == "valtome" |
							characters[i].Name == "numida" | characters[i].Name == "izuka" | characters[i].Name == "hetzel" | characters[i].Name == "levail" |
							characters[i].Name == "lekain" | characters[i].Name == "zelgius" | characters[i].Name == "dheginsea" | characters[i].Name == "sephiran" |
							characters[i].Name == "blackknight" | characters[i].Name == "ashera")
						{
							if (characters[i].Tier == "a" & possibleclasses[0] == 999)
								tryagain = true;
							else if (characters[i].Tier == "b" & possibleclasses[1] == 999)
								tryagain = true;
							else if (characters[i].Tier == "c" & possibleclasses[2] == 999)
								tryagain = true;
							else if (characters[i].Tier == "d" & possibleclasses[3] == 999)
								tryagain = true;
						}
					}

					if (tryagain)
					{
						problemchild = i;
						break;
					}
				}

				if (tryagain)
				// something is wrong, roll a new random recruit for the problem child
				{
					int j = random.Next(0, totalchars);
					while (problemchild == j)
						j = random.Next(0, totalchars);
					// swap newrecr of i and j characters
					int temp;
					if (characters[problemchild].NewRecr == -1)
						temp = problemchild;
					else
						temp = characters[problemchild].NewRecr;
					if (characters[j].NewRecr == -1)
						characters[problemchild].NewRecr = j;
					else
						characters[problemchild].NewRecr = characters[j].NewRecr;
					characters[j].NewRecr = temp;
				}
				// everything else works, now let's make sure miccy is who is picked
				else if (cbxChooseMic.Checked & characters[0].NewRecr != comboMicc.SelectedIndex)
				{
					int current = characters[0].NewRecr;
					int desired = comboMicc.SelectedIndex;
					for (int i = 0; i < totalchars; i++)
					{
						if (characters[i].NewRecr == desired)
						{
							characters[i].NewRecr = current;
							characters[0].NewRecr = desired;
							break;
						}
					}
					// then we have to go back through the loop to make sure that didn't screw anything up
				}
				// everything else works, now let's make sure ike is who is picked
				else if (cbxChooseIke.Checked & characters[34].NewRecr != comboIke.SelectedIndex)
				{
					int current = characters[34].NewRecr;
					int desired = comboIke.SelectedIndex;
					for (int i = 0; i < totalchars; i++)
					{
						if (characters[i].NewRecr == desired)
						{
							characters[i].NewRecr = current;
							characters[34].NewRecr = desired;
							break;
						}
					}
					// then we have to go back through the loop to make sure that didn't screw anything up
				}
				// everything else works, now let's make sure elincia is who is picked
				else if (cbxChooseElincia.Checked & characters[18].NewRecr != comboElincia.SelectedIndex)
				{
					int current = characters[18].NewRecr;
					int desired = comboElincia.SelectedIndex;
					for (int i = 0; i < totalchars; i++)
					{
						if (characters[i].NewRecr == desired)
						{
							characters[i].NewRecr = current;
							characters[18].NewRecr = desired;
							break;
						}
					}
					// then we have to go back through the loop to make sure that didn't screw anything up
				}
				else // sweet release
					finished = true;
			}

			// set up inverse recruitment values (shows where the character ended up)
			for (int i = 0; i < totalchars; i++)
			{
				for (int j = 0; j < totalchars; j++)
				{
					if (characters[j].NewRecr == i)
					{
						characters[i].RecrInverse = j;
						break;
					}
				}
			}

			// save new race and class (class will be overwritten later if random classes are on, but race is important either way)
			for (int i = 0; i < totalchars; i++)
			{
				if (!cbxRecrVanillaClass.Checked)
				{
					characters[i].NewRace = characters[characters[i].NewRecr].Race;
					characters[i].RecrRace = characters[characters[i].NewRecr].Race;
					characters[i].NewName = characters[characters[i].NewRecr].Name;

					int[] possibleclasses = characters[characters[i].NewRecr].RecrClasses;
					if (characters[i].Tier == "a")
						characters[i].NewClass = possibleclasses[0];
					else if (characters[i].Tier == "b")
						characters[i].NewClass = possibleclasses[1];
					else if (characters[i].Tier == "c")
						characters[i].NewClass = possibleclasses[2];
					else if (characters[i].Tier == "d")
						characters[i].NewClass = possibleclasses[3];
				}
				else // keep vanilla classes, there is no new class
					characters[i].NewClass = -1;
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
					bool stop = false;
					while (!stop)
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
						{ stop = false; }
						else if (cbxThieves.Checked == true & heronCharNums[i] == 24) // heather must stay thief
						{ stop = false; }
						else if (characters[heronCharNums[i]].Tier == "c" | characters[heronCharNums[i]].Tier == "d") // c and d tier characters can't be herons
						{ stop = false; }
						// heron names are limiting to PID - PID_LEARNE and PID_RAFIEL limit the characters to ten
						else if (characters[heronCharNums[i]].PID.Length > 10)
						{ stop = false; }
						else if (cbxClassRand.Checked & comboClassOptions.SelectedIndex == 0) // beorc can't be heron if race-mixing is off
						{
							if (cbxRandRecr.Checked)
							{
								if (characters[heronCharNums[i]].NewRace == "B")
									stop = false;
								else
									stop = true;
							}
							else
							{
								if (characters[heronCharNums[i]].Race == "B")
									stop = false;
								else
									stop = true;
							}
						}
						else
						{
							stop = true;
						}

						if (stop)
						{
							for (int j = 0; j < 3; j++)
							{
								if (i != j & heronCharNums[i] == heronCharNums[j]) // can't have two units be the same heron
									stop = false;
							}
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
								weights[j] = (int)Math.Pow(2, (i - 1));
							else
								weights[j] = 0;
						}
					}
				}
				// prevent early game laguz
				if (comboClassOptions.SelectedIndex == 1 & cbxNoLaguz.Checked == true & characters[charNum].Race == "B" & charNum < 18)
				{
					weights[5] = 0;
					weights[6] = 0;
					weights[7] = 0;
				}
				for (int i = 1; i < 8; i++)
					weights[i] += weights[i - 1];


				// let us begin
				if (cbxLords.Checked == true & (characters[charNum].Name == "micaiah" | characters[charNum].Name == "elincia" | characters[charNum].Name == "ike"))
				{
					// micaiah/elincia/ike are unchanged
					if (characters[charNum].Name == "micaiah")
						characters[charNum].NewClass = 54; // lightmage
					else if (characters[charNum].Name == "elincia")
						characters[charNum].NewClass = 67; // queen
					else
						characters[charNum].NewClass = 3; // hero
				}
				else if (cbxThieves.Checked == true & (characters[charNum].Name == "sothe" | characters[charNum].Name == "heather"))
				{
					// sothe/heather are unchanged
					if (characters[charNum].Name == "sothe" & cbxClassPatch.Checked == true)
						characters[charNum].NewClass = 73; // trickster
					else
						characters[charNum].NewClass = 42; // rogue
				}
				else
				{
					// choose class type from weights
					if (comboClassOptions.SelectedIndex == 0)
					{
						// no race-mixing
						if ((cbxRandRecr.Checked == false & characters[charNum].Race == "B") |
							(cbxRandRecr.Checked == true & characters[charNum].RecrRace == "B"))
							classtype = random.Next(weights[4]); // 5 beorc class types
						else if (characters[charNum].Tier == "a")
							classtype = random.Next(weights[4], weights[6]); // 2 laguz class types (tier a can't be dragon)
						else
							classtype = random.Next(weights[4], weights[7]);
					}
					else
					{
						// race-mixing
						if (characters[charNum].Tier == "a")
							classtype = random.Next(weights[6]); // tier a can't be dragon
						else
							classtype = random.Next(weights[7]);
					}
					// get classtype from weights
					for (int i = 0; i < 8; i++)
					{
						if (classtype < weights[i])
						{
							classtype = i;
							break;
						}
					}

					// restrict ike
					if (characters[charNum].Name == "ike")
					{
						// always prevent cavalry
						if (classtype == 2 & cbxHorseParkour.Checked == false)
							classtype = 0;
						// restrict magic when BK fight is impossible
						else if (restrictIke & classtype == 1)
							classtype = 0;
					}
					// restrict ranulf from cavalry
					if (characters[charNum].Name == "ranulf")
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
						for (int k = 0; k < classes.Length; k++)
						{
							if (characters[charNum].Name == "ike" & restrictIke)
							{
								// ike can't be rogue, raven, or heron (magic and horse are already covered earlier in code)
								if (classes[k].Tier_P.Contains(characters[charNum].Tier) & classes[k].Classtype_P == classtypestring
													& !classes[k].Name.Contains("rogue") & !classes[k].Name.Contains("raven") & !classes[k].Name.Contains("heron"))
									possibleclasses.Add(k);
							}
							else if (characters[charNum].Name == "micaiah" | characters[charNum].Name == "sothe" | characters[charNum].Name == "ike" |
									 characters[charNum].Name == "brom" | characters[charNum].Name == "nephenee")
							{
								// micaiah, sothe, ike, brom, nephenee cannot be herons
								if (classes[k].Tier_P.Contains(characters[charNum].Tier) & classes[k].Classtype_P == classtypestring
													& !classes[k].Name.Contains("heron"))
									possibleclasses.Add(k);
							}
							else if (characters[charNum].Name == "edward" & characters[0].NewClass == 57)
							{
								// edward can't be a priest or heron if micaiah is a priest
								if (classes[k].Tier_P.Contains(characters[charNum].Tier) & classes[k].Classtype_P == classtypestring
													& !classes[k].Name.Contains("priest") & !classes[k].Name.Contains("heron"))
									possibleclasses.Add(k);
							}
							else if (cbxHerons.Checked == false)
							{
								// give herons their normal classes
								if ((!cbxRandRecr.Checked & characters[charNum].Name == "rafiel") | (cbxRandRecr.Checked & characters[charNum].NewName == "rafiel"))
									possibleclasses.Add(93);
								else if ((!cbxRandRecr.Checked & characters[charNum].Name == "leanne") | (cbxRandRecr.Checked & characters[charNum].NewName == "leanne"))
									possibleclasses.Add(94);
								else if ((!cbxRandRecr.Checked & characters[charNum].Name == "reyson") | (cbxRandRecr.Checked & characters[charNum].NewName == "reyson"))
									possibleclasses.Add(95);
								// restrict everybody else from heron
								else
								{
									if (classes[k].Tier_P.Contains(characters[charNum].Tier) & classes[k].Classtype_P == classtypestring
														& !classes[k].Name.Contains("heron"))
										possibleclasses.Add(k);
								}
							}
							else
							{
								// no restrictions
								if (classes[k].Tier_P.Contains(characters[charNum].Tier) & classes[k].Classtype_P == classtypestring)
									possibleclasses.Add(k);
							}
						}
					}
					else
					{
						// oliver mode
						if (characters[charNum].Tier == "a")
						{
							if (!cbxHerons.Checked)
							{
								if ((!cbxRandRecr.Checked & characters[charNum].Name == "rafiel") | (cbxRandRecr.Checked & characters[charNum].NewName == "rafiel"))
									possibleclasses.Add(93);
								else if ((!cbxRandRecr.Checked & characters[charNum].Name == "leanne") | (cbxRandRecr.Checked & characters[charNum].NewName == "leanne"))
									possibleclasses.Add(94);
								else if ((!cbxRandRecr.Checked & characters[charNum].Name == "reyson") | (cbxRandRecr.Checked & characters[charNum].NewName == "reyson"))
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
						else if (characters[charNum].Tier == "b")
						{
							if (!cbxHerons.Checked)
							{
								if ((!cbxRandRecr.Checked & characters[charNum].Name == "rafiel") | (cbxRandRecr.Checked & characters[charNum].NewName == "rafiel"))
									possibleclasses.Add(93);
								else if ((!cbxRandRecr.Checked & characters[charNum].Name == "leanne") | (cbxRandRecr.Checked & characters[charNum].NewName == "leanne"))
									possibleclasses.Add(94);
								else if ((!cbxRandRecr.Checked & characters[charNum].Name == "reyson") | (cbxRandRecr.Checked & characters[charNum].NewName == "reyson"))
									possibleclasses.Add(95);
								else
								{
									if (characters[charNum].Name == "ike" & restrictIke)
										possibleclasses.Add(3); // hero
									else
									{
										possibleclasses.Add(55); // lightsage
										possibleclasses.Add(58); // bishop
									}
									if (cbxClassPatch.Checked)
										possibleclasses.Add(85); // warmonk
								}
							}
							else
							{
								if (characters[charNum].Name == "ike" & restrictIke)
									possibleclasses.Add(3); // hero
								else
								{
									possibleclasses.Add(55); // lightsage
									possibleclasses.Add(58); // bishop
								}
								if (cbxClassPatch.Checked)
									possibleclasses.Add(85); // warmonk
							}
						}
						else
						{
							possibleclasses.Add(56); // lightpriestess
							possibleclasses.Add(59); // saint

							if (cbxClassPatch.Checked)
								possibleclasses.Add(86); // crusader
						}
					}

					// select random class from possible classes
					if (possibleclasses.Count == 0)
					{
						textBox1.Text = "Errorcode 11: no possible class for " + characters[charNum].Name + ". Please report this error on discord with your settings.ini file";
						errorflag = 1;
					}
					else
					{
						if (characters[charNum].Chapter == "0" & cbxRandBosses.Checked)
						// if boss character and bosses are randomized later, don't worry about it
						{ }
						else
						{
							characters[charNum].NewClass = possibleclasses.ElementAt(random.Next(possibleclasses.Count));
							characters[charNum].JID = classes[characters[charNum].NewClass].JID;
						}
					}

					if (cbxHerons.Checked)
					{
						// if character is destined to be heron, replace chosen new class
						for (int k = 0; k < 3; k++)
						{
							if (heronCharNums[k] == charNum)
							{
								characters[charNum].NewClass = 93 + k; // 93 is rafiel heron class
								characters[charNum].JID = classes[characters[charNum].NewClass].JID;
							}
						}
					}
				}

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
				if (cbxClassPatch.Checked == true)
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

					if (cbxLords.Checked & (characters[i].Name == "micaiah" | characters[i].Name == "elincia" | characters[i].Name == "ike"))
					{
						if (characters[i].Name == "micaiah")
							characters[i].NewClass = Convert.ToInt32(values[1]);
						else if (characters[i].Name == "elincia")
							characters[i].NewClass = Convert.ToInt32(values[3]);
						else if (characters[i].Name == "ike")
							characters[i].NewClass = Convert.ToInt32(values[2]);
					}
					else if (cbxThieves.Checked & (characters[i].Name == "sothe" | characters[i].Name == "heather"))
					{
						characters[i].NewClass = Convert.ToInt32(values[2]);
					}
					else if (!cbxHerons.Checked & (characters[i].Name == "rafiel" | characters[i].Name == "leanne" | characters[i].Name == "reyson"))
					{
						if (!cbxRandRecr.Checked)
							characters[i].NewClass = Convert.ToInt32(values[2]);
						else
							characters[characters[i].RecrInverse].NewClass = Convert.ToInt32(values[2]);
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
						bool stop = false;
						while (!stop)
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
							{ stop = false; }
							else if (cbxThieves.Checked == true & heronCharNums[i] == 24) // heather must stay thief
							{ stop = false; }
							else if (characters[heronCharNums[i]].Tier == "c" | characters[heronCharNums[i]].Tier == "d") // c and d tier characters can't be herons
							{ stop = false; }
							else if (characters[heronCharNums[i]].PID.Length > 10)
							{ stop = false; }
							else if (cbxClassRand.Checked & comboClassOptions.SelectedIndex == 0) // beorc can't be heron if race-mixing is off
							{
								if (cbxRandRecr.Checked)
								{
									if (characters[heronCharNums[i]].NewRace == "B")
										stop = false;
								}
								else
								{
									if (characters[heronCharNums[i]].Race == "B")
										stop = false;
								}
							}
							else
							{
								stop = true;
								for (int j = 0; j < 3; j++)
								{
									if (i != j & heronCharNums[i] == heronCharNums[j]) // can't have two units be the same heron
										stop = false;
								}
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


				// loop through all characters
				for (int charNum = 0; charNum < 72; charNum++)
				{
					if (redo == 1)
						break;

					if (cbxLords.Checked & (characters[charNum].Name == "micaiah" | characters[charNum].Name == "elincia" | characters[charNum].Name == "ike"))
					{ }
					else if (cbxThieves.Checked & (characters[charNum].Name == "sothe" | characters[charNum].Name == "heather"))
					{ }
					else if (!cbxHerons.Checked & ((cbxRandRecr.Checked == false & (charNum >= 69)) |
							(cbxRandRecr.Checked & (characters[charNum].NewRecr == 69 | characters[charNum].NewRecr == 70 | characters[charNum].NewRecr == 71))))
					{ }
					else if (charNum == heronCharNums[0] | charNum == heronCharNums[1] | charNum == heronCharNums[2])
					{ }
					else
					{
						int randselect = random.Next(swapt1.Count);
						int times = 0;
						if (characters[charNum].Tier == "a")
						{
							while (true)
							{
								if (swapt1[randselect] == 999) // not a possible class for tier 1
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if (cbxNoLaguz.Checked == true & swaprace[randselect] == "L" & characters[charNum].Race == "B" & charNum < 18) // no early game laguz
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if (charNum == 0 & (classes[swapt1[randselect]].Name.Contains("heron") |
														classes[swapt1[randselect]].Name.Contains("priest"))) // micaiah can't be heron or priest
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
							characters[charNum].NewClass = swapt1[randselect];

						}
						else if (characters[charNum].Tier == "b")
						{
							while (true)
							{
								if (swapt2[randselect] == 999) // not possible for tier 2
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if (cbxNoLaguz.Checked & swaprace[randselect] == "L" & characters[charNum].Race == "B" & charNum < 18) // no early game laguz
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if ((characters[charNum].Name == "micaiah" | characters[charNum].Name == "sothe" | characters[charNum].Name == "ike" |
										  characters[charNum].Name == "brom" | characters[charNum].Name == "nephenee") &
										  classes[swapt2[randselect]].Name.Contains("heron")) // ike,sothe,brom,neph can't be heron
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if ((characters[charNum].Name == "ike" | characters[charNum].Name == "ranulf") &
										!cbxHorseParkour.Checked & classes[swapt2[randselect]].Classtype_P == "H") // ike,ranulf can't be mounted
								{
									randselect = random.Next(swapt1.Count);
									times += 1;
								}
								else if (charNum == 34 & restrictIke == true & (classes[swapt2[randselect]].Classtype_P == "M" |
																				classes[swapt2[randselect]].Name.Contains("rogue") |
																				classes[swapt2[randselect]].Name.Contains("raven"))) // ike can't be rogue, raven, or magic
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

							characters[charNum].NewClass = swapt2[randselect];
						}
						else if (characters[charNum].Tier == "c")
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
							characters[charNum].NewClass = swapt3[randselect];
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
							characters[charNum].NewClass = swapt4[randselect];
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

		// choose weapons for a player unit based on class and number of weapons, outputs a size-4 array of strings (no weapons are blank strings)
		private string[] ChoosePlayerT1Weapons(int classnum, int numweapons)
		{
			string[] weapons = new string[4];
			string[] weapontypes = classes[classnum].Weapon_P.Split(';');

			for (int i = 0; i < 4; i++)
			{
				if (i >= numweapons)
					weapons[i] = "";
				else
				{
					string switchstring;
					if (i >= weapontypes.Length)
						switchstring = weapontypes[0];
					else
						switchstring = weapontypes[i];

					switch (switchstring)

					{
						case "S":
							{
								if (cbxWeapPatch.Checked)
									weapons[i] = new string[2] { "IID_IRONSWORD", "IID_WINDSWORD" }[random.Next(0, 2)];
								else
									weapons[i] = new string[3] { "IID_IRONSWORD", "IID_WINDSWORD", "IID_POISONSWORD" }[random.Next(0, 3)];
								break;
							}
						case "L":
							{
								if (cbxWeapPatch.Checked)
									weapons[i] = new string[2] { "IID_IRONLANCE", "IID_HANDSPEAR" }[random.Next(0, 2)];
								else
									weapons[i] = new string[3] { "IID_IRONLANCE", "IID_HANDSPEAR", "IID_POISONLANCE" }[random.Next(0, 3)];
								break;
							}
						case "A":
							{
								if (cbxWeapPatch.Checked)
									weapons[i] = new string[2] { "IID_IRONAXE", "IID_HANDAXE" }[random.Next(0, 2)];
								else
									weapons[i] = new string[3] { "IID_IRONAXE", "IID_HANDAXE", "IID_POISONAXE" }[random.Next(0, 3)];
								break;
							}
						case "K":
							{
								weapons[i] = new string[2] { "IID_IRONDAGGER", "IID_IRONKNIFE" }[random.Next(0, 2)];
								break;
							}
						case "B":
							{
								if (cbxWeapPatch.Checked)
									weapons[i] = "IID_IRONBOW";
								else
									weapons[i] = new string[2] { "IID_IRONBOW", "IID_POISONBOW" }[random.Next(0, 2)];
								break;
							}
						case "F":
							{
								weapons[i] = "IID_FIRE";
								break;
							}
						case "T":
							{
								weapons[i] = "IID_THUNDER";
								break;
							}
						case "W":
							{
								weapons[i] = "IID_WIND";
								break;
							}
						case "M":
							{
								weapons[i] = "IID_LIGHT";
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
										weapons[i] = "IID_TORCH";
								}
								break;
							}
						case "X":
							{
								if (i == 0 | i == 2)
								{
									weapons[i] = "IID_OLIVI";
								}
								else
								{
									int weaponChance = random.Next(10);
									if (weaponChance < 8)
										weapons[i] = "IID_OLIVI";
									else if (weaponChance < 9)
										weapons[i] = "IID_REDGEM";
									else
										weapons[i] = "IID_HALFBEAST";
								}
								break;
							}
					}
				}
			}
			return (weapons);
		}
		private string[] ChoosePlayerT2Weapons(int classnum, int numweapons, string charname)
		{
			string[] weapons = new string[4];
			string[] weapontypes = classes[classnum].Weapon_P.Split(';');

			for (int i = 0; i < 4; i++)
			{
				if (i >= numweapons)
					weapons[i] = "";
				else
				{
					string switchstring;
					if (i >= weapontypes.Length)
						switchstring = weapontypes[0];
					else
						switchstring = weapontypes[i];

					switch (switchstring)
					{
						case "S":
							{
								if (i == 0 & charname == "ike")
									weapons[i] = "IID_ALONDITE"; // ettard
								else if (i == 0 & charname == "mist")
									weapons[i] = "IID_FLORETE"; // ettard
								else
								{
									if (cbxWeapPatch.Checked)
										weapons[i] = new string[6] { "IID_STEELSWORD", "IID_IRONBLADE", "IID_BRAVESWORD", "IID_POISONSWORD", "IID_WINDSWORD", "IID_BRONZESWORD" }[random.Next(0, 6)];
									else
										weapons[i] = new string[4] { "IID_STEELSWORD", "IID_IRONBLADE", "IID_BRAVESWORD", "IID_WINDSWORD" }[random.Next(0, 4)];
								}
								break;
							}
						case "L":
							{
								if (cbxWeapPatch.Checked)
									weapons[i] = new string[6] { "IID_STEELLANCE", "IID_IRONSPEAR", "IID_BRAVELANCE", "IID_POISONLANCE", "IID_HANDSPEAR", "IID_BRONZELANCE" }[random.Next(0, 6)];
								else
									weapons[i] = new string[4] { "IID_STEELLANCE", "IID_IRONSPEAR", "IID_BRAVELANCE", "IID_HANDSPEAR" }[random.Next(0, 4)];
								break;
							}
						case "A":
							{
								if (cbxWeapPatch.Checked)
									weapons[i] = new string[6] { "IID_STEELAXE", "IID_IRONPOLEAXE", "IID_BRAVEAXE", "IID_POISONAXE", "IID_HANDAXE", "IID_BRONZEAXE" }[random.Next(0, 6)];
								else
									weapons[i] = new string[4] { "IID_STEELAXE", "IID_IRONPOLEAXE", "IID_BRAVEAXE", "IID_HANDAXE" }[random.Next(0, 4)];
								break;
							}
						case "K":
							{
								if (cbxWeapPatch.Checked)
									weapons[i] = new string[6] { "IID_STEELDAGGER", "IID_STEELKNIFE", "IID_BEASTKILLER", "IID_KARD", "IID_BRONZEDAGGER", "IID_BRONZEKNIFE" }[random.Next(0, 6)];
								else
									weapons[i] = new string[4] { "IID_STEELDAGGER", "IID_STEELKNIFE", "IID_BEASTKILLER", "IID_KARD" }[random.Next(0, 4)];
								break;
							}
						case "B":
							{
								if (cbxWeapPatch.Checked)
									weapons[i] = new string[6] { "IID_STEELBOW", "IID_LOFABOW", "IID_KILLERBOW", "IID_IRONLONGBOW", "IID_BRONZEBOW", "IID_POISONBOW" }[random.Next(0, 6)];
								else
									weapons[i] = new string[4] { "IID_STEELBOW", "IID_LOFABOW", "IID_KILLERBOW", "IID_IRONLONGBOW" }[random.Next(0, 4)];
								break;
							}
						case "G":
							{
								weapons[i] = "IID_BOWGUN";
								break;
							}
						case "F":
							{
								weapons[i] = new string[3] { "IID_ELFIRE", "IID_METEOR", "IID_FIRE" }[random.Next(0, 3)];
								break;
							}
						case "T":
							{
								weapons[i] = new string[3] { "IID_ELTHUNDER", "IID_THUNDERSTORM", "IID_THUNDER" }[random.Next(0, 3)];
								break;
							}
						case "W":
							{
								weapons[i] = new string[3] { "IID_ELWIND", "IID_BLIZZARD", "IID_WIND" }[random.Next(0, 3)];
								break;
							}
						case "M":
							{
								weapons[i] = new string[3] { "IID_ELLIGHT", "IID_PURGE", "IID_LIGHT" }[random.Next(0, 3)];
								break;
							}
						case "D":
							{
								weapons[i] = new string[3] { "IID_KAREAU", "IID_FENRIR", "IID_WORM" }[random.Next(0, 3)];
								break;
							}
						case "I":
							{
								weapons[i] = "IID_MASTERCROWN";
								break;
							}
						case "H":
							{
								if (i == 0)
								{
									weapons[i] = "IID_RELIVE";
								}
								else if (i == 1)
								{
									int weaponChance = random.Next(10);
									if (weaponChance < 4) // 40% chance of mend
										weapons[i] = "IID_RELIVE";
									else if (weaponChance < 6) // 20% chance of restore
										weapons[i] = "IID_RESTORE";
									else if (weaponChance < 9) // 30% chance of physic
										weapons[i] = "IID_REBLOW";
									else                  // 10% chance of ward
										weapons[i] = "IID_MSHIELD";
								}
								else
								{
									weapons[i] = "IID_SLEEP";
								}
								break;
							}
						case "X":
							{
								if (i == 0)
								{
									weapons[i] = "IID_CHANGESTONE";
								}
								else if (i == 1)
								{
									int weaponChance = random.Next(10);
									if (weaponChance < 4) // 40% chance of olivi
										weapons[i] = "IID_OLIVI";
									else if (weaponChance < 6) // 20% chance of halfbeast
										weapons[i] = "IID_HALFBEAST";
									else if (weaponChance < 7)
										weapons[i] = "IID_COIN";
									else if (weaponChance < 8)
										weapons[i] = "IID_BLUEGEM";
									else if (weaponChance < 9)
										weapons[i] = "IID_CHANGESTONE";
									else
									{
										if (cbxFormshift.Checked)
											weapons[i] = "IID_TROOP";
										else
											weapons[i] = "IID_SATORISIGN";
									}
								}
								else
								{
									if (random.Next(10) < 5)
										weapons[i] = "IID_OLIVI";
									else
										weapons[i] = "IID_DEATHCARD";
								}
								break;
							}
					}
				}
			}
			return (weapons);
		}
		private string[] ChoosePlayerT3Weapons(int classnum, int numweapons, string charname)
		{
			string[] weapons = new string[4];
			string[] weapontypes = classes[classnum].Weapon_P.Split(';');

			for (int i = 0; i < 4; i++)
			{
				if (i >= numweapons)
					weapons[i] = "";
				else
				{
					string switchstring;
					if (i >= weapontypes.Length)
						switchstring = weapontypes[0];
					else
						switchstring = weapontypes[i];

					switch (switchstring)
					{
						case "S":
							{
								if (i == 0 & charname == "ike")
									weapons[i] = "IID_ALONDITE"; // ettard
								else if (i == 0 & charname == "mist")
									weapons[i] = "IID_FLORETE"; // florete
								else if (i == 0 & (charname == "stephan" | charname == "lehran"))
									weapons[i] = "IID_VAGUEKATTI"; // ss weapon
								else if (i == 2)
									weapons[i] = "IID_WINDSWORD";
								else
								{
									if (cbxWeapPatch.Checked)
										weapons[i] = new string[7] { "IID_KILLSWORD", "IID_SILVERSWORD", "IID_STORMSWORD", "IID_BRAVESWORD", "IID_DRAGONKILLER", "IID_STEELBLADE", "IID_POISONSWORD" }[random.Next(0, 7)];
									else
										weapons[i] = new string[6] { "IID_KILLSWORD", "IID_SILVERSWORD", "IID_STORMSWORD", "IID_BRAVESWORD", "IID_DRAGONKILLER", "IID_STEELBLADE" }[random.Next(0, 6)];
								}
								break;
							}
						case "L":
							{
								if (i == 0 & (charname == "stephan" | charname == "lehran"))
									weapons[i] = "IID_ZANEZPHTE"; // ss weapon
								else if (i == 2)
									weapons[i] = "IID_HANDSPEAR";
								else
								{
									if (cbxWeapPatch.Checked)
										weapons[i] = new string[7] { "IID_KILLERLANCE", "IID_SILVERLANCE", "IID_SHORTSPEAR", "IID_BRAVELANCE", "IID_HORSEKILLER", "IID_STEELSPEAR", "IID_POISONLANCE" }[random.Next(0, 7)];
									else
										weapons[i] = new string[6] { "IID_KILLERLANCE", "IID_SILVERLANCE", "IID_SHORTSPEAR", "IID_BRAVELANCE", "IID_HORSEKILLER", "IID_STEELSPEAR" }[random.Next(0, 6)];
								}
								break;
							}
						case "A":
							{
								if (i == 0 & (charname == "stephan" | charname == "lehran"))
									weapons[i] = "IID_URVAN"; // ss weapon
								else if (i == 2)
									weapons[i] = "IID_HANDAXE";
								else
								{
									if (cbxWeapPatch.Checked)
										weapons[i] = new string[7] { "IID_KILLERAXE", "IID_SILVERAXE", "IID_SHORTAXE", "IID_BRAVEAXE", "IID_HAMMER", "IID_STEELPOLEAXE", "IID_POISONAXE" }[random.Next(0, 7)];
									else
										weapons[i] = new string[6] { "IID_KILLERAXE", "IID_SILVERAXE", "IID_SHORTAXE", "IID_BRAVEAXE", "IID_HAMMER", "IID_STEELPOLEAXE" }[random.Next(0, 6)];
								}
								break;
							}
						case "K":
							{
								if (i == 0 & (charname == "stephan" | charname == "lehran"))
									weapons[i] = "IID_BASELARD"; // ss weapon
								else
								{
									if (cbxWeapPatch.Checked)
										weapons[i] = new string[7] { "IID_STILETTO", "IID_SILVERKNIFE", "IID_SILVERDAGGER", "IID_BEASTKILLER", "IID_STEELDAGGER", "IID_BRONZEDAGGER", "IID_BRONZEKNIFE" }[random.Next(0, 7)];
									else
										weapons[i] = new string[5] { "IID_STILETTO", "IID_SILVERKNIFE", "IID_SILVERDAGGER", "IID_BEASTKILLER", "IID_STEELDAGGER" }[random.Next(0, 5)];
								}
								break;
							}
						case "B":
							{
								if (i == 0 & (charname == "stephan" | charname == "lehran"))
									weapons[i] = "IID_VALFLECHE"; // ss weapon
								else
								{
									if (cbxWeapPatch.Checked)
										weapons[i] = new string[7] { "IID_KILLERBOW", "IID_SILVERBOW", "IID_STEELLONGBOW", "IID_CHINONBOW", "IID_BRAVEBOW", "IID_IRONARCH", "IID_POISONBOW" }[random.Next(0, 7)];
									else
										weapons[i] = new string[6] { "IID_KILLERBOW", "IID_SILVERBOW", "IID_STEELLONGBOW", "IID_CHINONBOW", "IID_BRAVEBOW", "IID_IRONARCH" }[random.Next(0, 6)];
								}
								break;
							}
						case "G":
							{
								weapons[i] = "IID_CROSSBOW";
								break;
							}
						case "F":
							{
								if (i == 0 & (charname == "stephan" | charname == "lehran"))
									weapons[i] = "IID_REXFLAME"; // ss weapon
								else if (i == 0 & charname == "sanaki")
									weapons[i] = "IID_CYMBELINE";
								else
									weapons[i] = new string[4] { "IID_GIGAFIRE", "IID_METEOR", "IID_ELFIRE", "IID_FIRE" }[random.Next(0, 4)];
								break;
							}
						case "T":
							{
								if (i == 0 & (charname == "stephan" | charname == "lehran"))
									weapons[i] = "IID_REXBOLT"; // ss weapon
								else
									weapons[i] = new string[4] { "IID_GIGATHUNDER", "IID_THUNDERSTORM", "IID_ELTHUNDER", "IID_THUNDER" }[random.Next(0, 4)];
								break;
							}
						case "W":
							{
								if (i == 0 & (charname == "stephan" | charname == "lehran"))
									weapons[i] = "IID_REXCALIBUR"; // ss weapon
								else
									weapons[i] = new string[4] { "IID_GIGAWIND", "IID_BLIZZARD", "IID_ELWIND", "IID_WIND" }[random.Next(0, 4)];
								break;
							}
						case "M":
							{
								if (i == 0 & (charname == "stephan" | charname == "lehran"))
									weapons[i] = "IID_REXAURA"; // ss weapon
								else
									weapons[i] = new string[5] { "IID_RESIRE", "IID_SHINE", "IID_PURGE", "IID_ELLIGHT", "IID_LIGHT" }[random.Next(0, 5)];
								break;
							}
						case "D":
							{
								if (i == 0 & (charname == "stephan" | charname == "lehran"))
									weapons[i] = "IID_BARVERT"; // ss weapon
								else
									weapons[i] = new string[4] { "IID_UERINE", "IID_KAREAU", "IID_FENRIR", "IID_WORM" }[random.Next(0, 4)];
								break;
							}
						case "H":
							{
								if (i == 0)
								{
									if (charname == "stephan" | charname == "lehran")
										weapons[i] = "IID_GODDESSROD";
									else
									{
										int weaponChance = random.Next(10);
										if (weaponChance < 4)
											weapons[i] = "IID_RECOVER";
										else if (weaponChance < 8)
											weapons[i] = "IID_REBLOW";
										else
											weapons[i] = "IID_SLEEP";
									}
								}
								else if (i == 1)
								{
									int weaponChance = random.Next(10);
									if (weaponChance < 4) // 40% chance of physic
										weapons[i] = "IID_REBLOW";
									else if (weaponChance < 7) // 30% chance of rescue
										weapons[i] = "IID_RESCUE";
									else if (weaponChance < 9) // 20% chance of silence
										weapons[i] = "IID_SILENCE";
									else                  // 10% chance of hammerne
										weapons[i] = "IID_HAMMERNE";
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
									if (charname == "stephan" | charname == "lehran")
										weapons[i] = "IID_ENERGYDROP";
									else
										weapons[i] = "IID_COIN";
								}
								else if (i == 1)
								{
									int weaponChance = random.Next(10);
									if (weaponChance < 1)
										weapons[i] = "IID_ENERYDROP";
									else if (weaponChance < 2)
										weapons[i] = "IID_SPIRITDUST";
									else if (weaponChance < 3)
										weapons[i] = "IID_SPEEDWING";
									else if (weaponChance < 4)
										weapons[i] = "IID_DRAGONSHIELD";
									else if (weaponChance < 5)
										weapons[i] = "IID_TALISMAN";
									else if (weaponChance < 6)
										weapons[i] = "IID_ELIXIR";
									else if (weaponChance < 7)
										weapons[i] = "IID_COIN";
									else if (weaponChance < 8)
										weapons[i] = "IID_BLUEGEM";
									else if (weaponChance < 9)
										weapons[i] = "IID_ANASTASISCARD";
									else
										weapons[i] = "IID_WHITEGEM";
								}
								else
								{
									if (random.Next(10) < 9)
										weapons[i] = "IID_ANGELROBE";
									else
										weapons[i] = "IID_BOOTS";
								}
								break;
							}
					}
				}
			}
			return (weapons);
		}

		// changes transformation gauges of new laguz units
		private void laguzModifications()
		{
			int numberofunits;
			if ((cbxClassRand.Checked & cbxHerons.Checked) | cbxRandRecr.Checked)
				numberofunits = 72;
			else
				numberofunits = 69;

			// loop through characters
			for (int charNum = 0; charNum < numberofunits; charNum++)
			{
				if (characters[charNum].NewClass != -1)
				{
					if (characters[charNum].NewRace == "L" & (characters[charNum].Tier != "c" | cbxFormshift.Checked))
					{
						int[] gaugevals = new int[4];

						if (cbxGaugeRand.Checked)
						{
							// randomly assign value from chosen min and max (inclusive) and output
							gaugevals[0] = random.Next(Convert.ToInt32(numericLaguzMin1.Value),
								(Convert.ToInt32(numericLaguzMax1.Value) + 1));
							gaugevals[1] = random.Next(Convert.ToInt32(numericLaguzMin2.Value),
								(Convert.ToInt32(numericLaguzMax2.Value) + 1));
							gaugevals[2] = random.Next(Convert.ToInt32(numericLaguzMin3.Value),
								(Convert.ToInt32(numericLaguzMax3.Value) + 1));
							gaugevals[3] = random.Next(Convert.ToInt32(numericLaguzMin4.Value),
								(Convert.ToInt32(numericLaguzMax4.Value) + 1));
						}
						else
						{
							switch (characters[charNum].NewClass)
							{
								case 87:
								case 99:
									// lion
									gaugevals = new int[4] { 5, 10, 3, 2 };
									break;
								case 88:
									// tiger
									gaugevals = new int[4] { 8, 15, 4, 3 };
									break;
								case 89:
									// cat
									gaugevals = new int[4] { 10, 15, 5, 4 };
									break;
								case 90:
								case 100:
									// wolf
									gaugevals = new int[4] { 6, 10, 4, 3 };
									break;
								case 91:
								case 101:
									// hawk
									gaugevals = new int[4] { 8, 15, 4, 3 };
									break;
								case 92:
								case 102:
									// raven
									gaugevals = new int[4] { 6, 10, 4, 3 };
									break;
								case 93:
								case 94:
								case 95:
									// heron
									gaugevals = new int[4] { 4, 10, 5, 6 };
									break;
								case 96:
									// red dragon
									gaugevals = new int[4] { 5, 6, 2, 1 };
									break;
								case 97:
									// white dragon
									gaugevals = new int[4] { 4, 5, 2, 1 };
									break;
								case 98:
								case 103:
									// black dragon
									gaugevals = new int[4] { 5, 6, 2, 1 };
									break;
								default:
									// laguz royals - if formshift scroll, give them actually gauges
									if (cbxFormshift.Checked)
										gaugevals = new int[4] { 5, 10, 3, 2 };
									else
										gaugevals = new int[4] { 0, 0, 1, 1 };
									break;
							}
						}

						gaugevals[2] = 256 - gaugevals[2];
						gaugevals[3] = 256 - gaugevals[3];
						// write to datafile
						CharacterData.Write(characters[charNum].PID, "Laguz_Gauge", gaugevals);
					}
				}
				else if (cbxGaugeRand.Checked & characters[charNum].Race == "L" & (characters[charNum].Tier != "c" | cbxFormshift.Checked))
				{
					int[] gaugevals = new int[4];

					// randomly assign value from chosen min and max (inclusive) and output
					gaugevals[0] = random.Next(Convert.ToInt32(numericLaguzMin1.Value),
						(Convert.ToInt32(numericLaguzMax1.Value) + 1));
					gaugevals[1] = random.Next(Convert.ToInt32(numericLaguzMin2.Value),
						(Convert.ToInt32(numericLaguzMax2.Value) + 1));
					gaugevals[2] = random.Next(Convert.ToInt32(numericLaguzMin3.Value),
						(Convert.ToInt32(numericLaguzMax3.Value) + 1));
					gaugevals[3] = random.Next(Convert.ToInt32(numericLaguzMin4.Value),
						(Convert.ToInt32(numericLaguzMax4.Value) + 1));

					gaugevals[2] = 256 - gaugevals[2];
					gaugevals[3] = 256 - gaugevals[3];
					// write to datafile
					CharacterData.Write(characters[charNum].PID, "Laguz_Gauge", gaugevals);
				}
			}
		}

		// swaps promotion lines around
		private void promotionSwapper()
		{
			List<int> tier1 = new List<int>();
			List<int> tier2 = new List<int>();
			List<int> tier3 = new List<int>();

			if (cbxEasyPromotion.Checked) // keeps physical units to promote into physical, magic into magic
			{
				List<int> magtier1 = new List<int>();
				List<int> magtier2 = new List<int>();
				List<int> magtier3 = new List<int>();
				// build tier lists
				for (int i = 0; i < classes.Length; i++)
				{
					if (classes[i].Classtype_P != "x")
					{
						if (classes[i].Tier_P.Contains('a') & classes[i].Race == "B" & classes[i].Classtype_P != "M")
							tier1.Add(i);
						else if (classes[i].Tier_P.Contains('b') & classes[i].Race == "B" & classes[i].Classtype_P != "M")
							tier2.Add(i);
						else if (classes[i].Tier_P.Contains('c') & classes[i].Race == "B" & classes[i].Classtype_P != "M")
							tier3.Add(i);

						if (classes[i].Tier_P.Contains('a') & classes[i].Race == "B" & classes[i].Classtype_P == "M")
							magtier1.Add(i);
						else if (classes[i].Tier_P.Contains('b') & classes[i].Race == "B" & classes[i].Classtype_P == "M")
							magtier2.Add(i);
						else if (classes[i].Tier_P.Contains('c') & classes[i].Race == "B" & classes[i].Classtype_P == "M")
							magtier3.Add(i);
					}
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
				for (int i = 0; i < classes.Length; i++)
				{
					if (classes[i].Classtype_P != "x")
					{
						if (classes[i].Tier_P.Contains('a') & classes[i].Race == "B")
							tier1.Add(i);
						else if (classes[i].Tier_P.Contains('b') & classes[i].Race == "B")
							tier2.Add(i);
						else if (classes[i].Tier_P.Contains('c') & classes[i].Race == "B")
							tier3.Add(i);
					}
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
					t1string = classes[t1].Name;
				if (t2 == 999)
					t2string = "n/a";
				else
					t2string = classes[t2].Name;
				t3string = classes[t3].Name;
				randPromoOutput += "<tr> <td>" + t1string + "</td> <td>" + t2string +
									"</td> <td>" + t3string + "</td> </tr>";
			}
			randPromoOutput += "</table>";


			// store promotion paths
			for (int i = 0; i < classes.Length; i++)
			{
				if (classes[i].Classtype_P != "x")
				{
					int foundindex;
					if (classes[i].Tier_P.Contains('a') & classes[i].Race == "B")
					{
						foundindex = tier1.FindIndex(x => x == i);
						classes[i].PromoPath = tier1.ElementAt(foundindex).ToString() + ";" +
											tier2.ElementAt(foundindex).ToString() + ";" +
											tier3.ElementAt(foundindex).ToString();
					}
					else if (classes[i].Tier_P.Contains('b') & classes[i].Race == "B")
					{
						foundindex = tier2.FindIndex(x => x == i);
						classes[i].PromoPath = tier1.ElementAt(foundindex).ToString() + ";" +
											tier2.ElementAt(foundindex).ToString() + ";" +
											tier3.ElementAt(foundindex).ToString();
					}
					else if (classes[i].Tier_P.Contains('c') & classes[i].Race == "B")
					{
						foundindex = tier3.FindIndex(x => x == i);
						classes[i].PromoPath = tier1.ElementAt(foundindex).ToString() + ";" +
											tier2.ElementAt(foundindex).ToString() + ";" +
											tier3.ElementAt(foundindex).ToString();
					}
				}
				else
					classes[i].PromoPath = "null";
			}

			// write paths to fe10data
			for (int i = 0; i < tier1.Count; i++)
			{
				if (tier1.ElementAt(i) != 999)
				{
					// read jid from promoted class
					string promojid = classes[tier2.ElementAt(i)].JID;
					ClassData.Write(classes[tier1.ElementAt(i)].JID, "Next_Class", promojid);
				}
				if (tier2.ElementAt(i) != 999)
				{
					// read jid from promoted class
					string promojid = classes[tier3.ElementAt(i)].JID;
					ClassData.Write(classes[tier2.ElementAt(i)].JID, "Next_Class", promojid);
				}
			}

			addPromoBonuses();
		}

		// adds promotion bonuses to classes normally not promoted into
		private void addPromoBonuses()
		{
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\newPromotionBonuses.csv");
			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			// skip header line
			for (int i = 1; i < lines.Length; i++)
			{
				string[] values = lines[i].Split(',');
				string name = values[1];
				// read from file
				int[] promogains = new int[8];
				for (int j = 0; j < 8; j++)
					promogains[j] = Convert.ToInt32(values[j + 2]);
				// write to data
				ClassData.Write(name, "Promo_Gains", promogains);
			}
		}

		// changes animations to match new classes
		private void animationChanger()
		{
			// changes animations of randomized units

			// input animation pointers
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\animationList.csv");
			string[] animationlines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			int animationnumber;
			if ((cbxClassRand.Checked & cbxHerons.Checked) | cbxRandRecr.Checked)
				animationnumber = 72;
			else
				animationnumber = 69;

			for (int charNum = 0; charNum < animationnumber; charNum++)
			{
				int charclass;
				if (characters[charNum].NewClass != -1)
					charclass = characters[charNum].NewClass;
				else
					charclass = characters[charNum].VanillaClass;

				if (cbxRandPromotion.Checked & charclass < 87)
				{
					string[] animationpointers1, animationpointers2, animationpointers3;
					string[] promoline = classes[charclass].PromoPath.Split(';');

					// separate animation pointers
					animationpointers3 = classes[Convert.ToInt32(promoline[2])].Animation.Split(';');
					if (promoline[1] != "999")
						animationpointers2 = classes[Convert.ToInt32(promoline[1])].Animation.Split(';');
					else
						animationpointers2 = animationpointers3;
					if (promoline[0] != "999")
						animationpointers1 = classes[Convert.ToInt32(promoline[0])].Animation.Split(';');
					else
						animationpointers1 = animationpointers2;

					// create 4-animation string
					string[] saveanim = new string[4];
					string[] origanim = CharacterData.ReadStringArray(characters[charNum].PID, "Animations");
					string[][] allanimpointers = { animationpointers1, animationpointers2, animationpointers3, animationpointers3 };
					for (int j = 0; j < allanimpointers.Length; j++)
					{
						// do not write over original animation if not class rand
						if ((!cbxClassRand.Checked & !cbxClassSwap.Checked & !cbxRandRecr.Checked) & (
									(characters[charNum].Tier == "a" & j == 0) | (characters[charNum].Tier == "b" & j == 1) | (characters[charNum].Tier == "c" & j == 2)))
						{
							saveanim[j] = origanim[j];
						}
						else
						{
							int minanimpointer = Convert.ToInt32(allanimpointers[j][0]);
							int maxanimpointer = Convert.ToInt32(allanimpointers[j][1]) + 1;

							// choose animation, select the line from original read-in lines
							string fullanimpointer = animationlines[random.Next(minanimpointer, maxanimpointer)];
							string[] split = fullanimpointer.Split(',');
							saveanim[j] = split[j + 1];
						}
					}
					// save new animation
					CharacterData.Write(characters[charNum].PID, "Animations", saveanim);
					CharacterData.Write(characters[charNum].PID, "JID", classes[charclass].JID);
				}
				else
				{
					// don't do this if classes haven't been changed
					string[] animationpointers;
					if (characters[charNum].NewClass == -1)
					{ }
					else if (cbxRandRecr.Checked & !cbxClassRand.Checked & !cbxClassSwap.Checked & // already have correct animations from rand recruit
						( characters[charNum].Name != "micaiah" | !cbxMicClass.Checked) &			// micaiah gets animation change if class randomized
						( characters[charNum].Name != "ike" | !cbxIkeClass.Checked) &				// same with ike
						( characters[charNum].Name != "elincia" | !cbxElinciaClass.Checked))		// and elincia
					{ }
					else
					{
						animationpointers = classes[characters[charNum].NewClass].Animation.Split(';');

						int minanimpointer = Convert.ToInt32(animationpointers[0]);
						int maxanimpointer = Convert.ToInt32(animationpointers[1]) + 1;

						// choose animation, select the line from original read-in lines
						string fullanimpointer = animationlines[random.Next(minanimpointer, maxanimpointer)];
						string[] split = fullanimpointer.Split(',');
						string[] saveanim = new string[4];
						for (int i = 0; i < 4; i++)
							saveanim[i] = split[i + 1];

						// save new animation
						CharacterData.Write(characters[charNum].PID, "Animations", saveanim);
						CharacterData.Write(characters[charNum].PID, "JID", classes[charclass].JID);
					}
				}
			}
		}

		// adds custom hybrid classes to the game
		private void LM73classPatch()
		{
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\LM73ClassPatch.csv");
			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			// FE10Data changes
			#region
			for (int i = 1; i < lines.Length; i++)
			{
				string[] split = lines[i].Split(',');

				string JID = split[0];
				// new mjid and mh_j
				string MJID = split[8];
				string MH_J = split[12];
				// min/max weapon rank strings
				string minrank = split[15];
				string maxrank = split[16];

				// change information in FE10Data
				if (MJID != "")
					ClassData.Write(JID, "MJID", MJID);
				if (MH_J != "")
					ClassData.Write(JID, "MH_J", MH_J);
				ClassData.Write(JID, "Base_WeaponRank", minrank);
				ClassData.Write(JID, "Max_WeaponRank", maxrank);

				// add to class types and skills
				string[] choosejid = new string[9] { "JID_SWORDMASTER_F", "JID_SWORDESCHATOS_F","JID_HALBERDIER_F","JID_HOLYLANCER_F",
													"JID_ROGUE", "JID_ESPION", "JID_DRAGONKNIGHT_F","JID_DRAGONMASTER_F","JID_RLINDWURM_F" };
				for (int j = 0; j < choosejid.Length; j++)
				{
					if (JID == choosejid[j])
					{
						string[] tempread, outputthis;
						// skills
						tempread = ClassData.ReadStringArray(JID, "Skills");
						outputthis = new string[tempread.Length + 1];
						for (int k = 0; k < tempread.Length; k++)
							outputthis[k] = tempread[k];
						outputthis[tempread.Length] = "SID_MAGE";
						ClassData.Write(JID, "Skills", outputthis);
						// class types
						tempread = ClassData.ReadStringArray(JID, "Class_Types");
						outputthis = new string[tempread.Length + 1];
						for (int k = 0; k < tempread.Length; k++)
							outputthis[k] = tempread[k];
						outputthis[tempread.Length] = "SFXC_MAGE";
						ClassData.Write(JID, "Class_Types", outputthis);
					}
				}

			}
			// change bastion's starting weapon ranks to have SS in thunder
			CharacterData.Write("PID_ULYSSES", "Base_WeaponRank", "------A*A--B");
			// write some stats for soren's new class son of ashnard
			ClassData.Write("JID_ARCHSAGE_W", "CON", 12);
			ClassData.Write("JID_ARCHSAGE_W", "Mount_Type", 4);
			ClassData.Write("JID_ARCHSAGE_W", "Mount_WT", 31);
			ClassData.Write("JID_ARCHSAGE_W", "Movement_Type", 10);
			ClassData.Write("JID_ARCHSAGE_W", "MOV", 9);
			string[] sorenskills = new string[5] { "SID_HIGHEST", "SID_CANTO", "SID_FLY", "SID_STUN", "SID_MAGE" };
			ClassData.Write("JID_ARCHSAGE_W", "Skills", sorenskills);
			string[] sorenclasstypes = new string[3] { "SFXC_HUMAN", "SFXC_DRAGON", "SFXC_MAGE" };
			ClassData.Write("JID_ARCHSAGE_W", "Class_Types", sorenclasstypes);

			// give lightning thief lethality over bane
			string[] tempskills = ClassData.ReadStringArray("JID_ROGUE", "Skills");
			for (int i = 0; i < tempskills.Length; i++)
				if (tempskills[i] == "SID_LETHALITY")
					tempskills[i] = "SID_BANE";
			ClassData.Write("JID_ROGUE", "Skills", tempskills);
			// give crusader colossus over corona
			tempskills = ClassData.ReadStringArray("JID_SAINT_F", "Skills");
			for (int i = 0; i < tempskills.Length; i++)
				if (tempskills[i] == "SID_AURORAL")
					tempskills[i] = "SID_COLOSSUS";
			ClassData.Write("JID_SAINT_F", "Skills", tempskills);
			#endregion

			// animation file changes and script changes, if not trial
			if (!cbxTrialMode.Checked)
			{
				// script changes
				for (int i = 1; i < lines.Length; i++)
				{
					string[] split = lines[i].Split(',');

					string JID = split[0];
					// new class name
					string newclassname = split[1];
					// new description
					string newdescription = split[2];
					// locations of name/description in text documents
					int namelocation, namepointerloc, descriplocation;
					if (gameVersion == 2) // PAL iso
					{
						// location in e_common where name is stored in text
						namelocation = Convert.ToInt32(split[21]);
						// pointer for text name
						namepointerloc = Convert.ToInt32(split[22]);
						// location in e_common where description is stored in text
						descriplocation = Convert.ToInt32(split[23]);
					}
					else
					{
						// location in e_common where name is stored in text
						namelocation = Convert.ToInt32(split[9]);
						// pointer for text name
						namepointerloc = Convert.ToInt32(split[10]);
						// location in e_common where description is stored in text
						descriplocation = Convert.ToInt32(split[13]);
					}

					// change text for mjids and mh_js
					using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_common.m", System.IO.FileMode.Open,
								System.IO.FileAccess.ReadWrite))
					{
						if (namelocation != 0)
						{
							stream.Position = namelocation;
							byte[] namestring = System.Text.Encoding.ASCII.GetBytes(newclassname);
							stream.Write(namestring, 0, namestring.Length);
							stream.WriteByte(0x00);
							stream.Position = namepointerloc;
							byte[] newnamepointer = int2bytes(namelocation - 32);
							stream.Write(newnamepointer, 0, 4);
						}
						if (descriplocation != 0)
						{
							string[] description = newdescription.Split(';');
							stream.Position = descriplocation;
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

				// animation file changes
				#region
				string sourcePath, targetPath, sourcefile, targetfile;
				for (int i = 1; i < lines.Length; i++)
				{
					string[] split = lines[i].Split(',');

					string JID = split[0];
					// zu animation files to move
					string movetozu = split[3];
					string movetozu2 = split[4];
					string movetofile = split[5];
					string movefromzu = split[6];
					string movefromfile = split[7];
					// ymu animation files to move
					string movetoymu = split[17];
					string movetoymu2 = split[18];
					string ymufiles2move = split[19];
					string movefromymu = split[20];

					// move animation files accordingly
					if (movefromzu != "")
					{
						for (int twice = 0; twice < 2; twice++)
						{
							sourcePath = dataLocation + "\\zu\\" + movefromzu + "\\";
							if (twice == 0)
								targetPath = dataLocation + "\\zu\\" + movetozu + "\\";
							else
							{
								if (movetozu2 != "")
									targetPath = dataLocation + "\\zu\\" + movetozu2 + "\\";
								else
									break;
							}
							string[] targetfiles = movetofile.Split(';');
							string[] sourcefiles = movefromfile.Split(';');
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
					if (movefromymu != "")
					{
						for (int twice = 0; twice < 2; twice++)
						{
							sourcePath = dataLocation + "\\ymu\\" + movefromymu + "\\";
							if (twice == 0)
								targetPath = dataLocation + "\\ymu\\" + movetoymu + "\\";
							else
							{
								if (movetoymu2 != "")
									targetPath = dataLocation + "\\ymu\\" + movetoymu2 + "\\";
								else
									break;
							}
							string[] filenames = ymufiles2move.Split(';');
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
				#endregion
			}
		}

		// fix laguz mjids and mh_js that are used for custom classes
		private void LaguzMJID()
		{
			// load in new class data
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ClassPatch_MJID.csv");
			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			for (int i = 1; i < lines.Length; i++)
			{
				string[] splitline = lines[i].Split(',');
				if (splitline[1].StartsWith("MJID"))
					ClassData.Write(splitline[0], "MJID", splitline[1]);
				else if (splitline[1].StartsWith("MH_J"))
					ClassData.Write(splitline[0], "MH_J", splitline[1]);
			}
		}

		// stat changes for custom classes
		private void ClassPatch_statchanges()
		{
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ClassPatch_NewStats.csv");
			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			for (int i = 1; i < lines.Length; i++)
			{
				string[] split = lines[i].Split(',');
				string ID = split[0];
				string statchange = split[1];
				int[] newbases = new int[8];
				for (int j = 0; j < 8; j++)
					newbases[j] = Convert.ToInt32(split[j + 2]);

				// save to data file
				if (ID.StartsWith("JID"))
					ClassData.Write(ID, statchange, newbases);
				else if (ID.StartsWith("PID"))
					CharacterData.Write(ID, statchange, newbases);
			}
		}

		// when there are no class changes, modify vanilla classes of certain characters
		private void ClassPatch_changeVanillaClasses()
		{
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ClassPatch_VanillaClassChanges.csv");
			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			for (int i = 1; i < lines.Length; i++)
			{
				string[] split = lines[i].Split(',');
				int id = Convert.ToInt32(split[1]);
				string JID = split[2];
				string[] weapons = new string[4] { split[3], split[4], split[5], "" };
				int chapindex = -1;

				if (characters[id].Chapter != "0")
				{
					for (int k = 0; k < chapters.Length; k++)
					{
						if (characters[id].Chapter == chapters[k])
						{
							chapindex = k;
							break;
						}
					}
				}
				if (chapindex != -1)
				{
					// save new class and weapons
					ChapterChar tempchar = ChapterData[chapindex].Read(characters[id].PID);
					tempchar.JID = JID;
					if (weapons[0] != "")
						tempchar.Weapons = weapons;
					ChapterData[chapindex].Write(tempchar);
				}
			}
		}

		// randomizes skills, authority stars, biorhythm, and affinity
		private void variousCharacterRandomizers()
		{
			textBox1.Text = "Misc Character Changes";
			Application.DoEvents();

			StreamReader dataReader = new StreamReader(file + "\\assets\\skillList.csv");
			string[] allskills = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			

			for (int i = 0; i < characters.Length; i++)
			{
				// stop when done with player characters
				if (characters[i].Chapter == "0")
					break;


				// randomizes skills or adds paragon
				if (cbxSkillRand.Checked | cbxParagon.Checked | cbxLaguzParagon.Checked)
				{
					string newskills;
					if (cbxSkillRand.Checked)
						// initialize with locked skills
						newskills = characters[i].LockedSkills;
					else
						// initialize with vanilla skills
						newskills = String.Join(";", CharacterData.ReadStringArray(characters[i].PID, "Skills"));

					List<string> possibleskills = new List<string>();
					string[] classtypes;
					if (characters[i].NewClass != -1)
						classtypes = ClassData.ReadStringArray(classes[characters[i].NewClass].JID, "Class_Types");
					else
						classtypes = ClassData.ReadStringArray(classes[characters[i].VanillaClass].JID, "Class_Types");
					bool islaguz = String.Join(",", classtypes).Contains("SFXC_ALIZE");
					for (int j = 0; j < allskills.Length; j++)
					{
						if (!islaguz & (allskills[j].Split(',')[1] == "SID_HALFBEAST" | allskills[j].Split(',')[1] == "SID_KING"))
						{ } // non-laguz can't have wildheart or formshift
						else if (!cbxFormshift.Checked & allskills[j].Split(',')[1] == "SID_KING")
						{ } // no formshift scroll
						else
							possibleskills.Add(allskills[j].Split(',')[1]);
					}

					int numskills = 0;
					if (cbxSkillRand.Checked & cbxSkillVanilla.Checked) // keeps vanilla number of skills, plus whatever user decides
						numskills = characters[i].SkillNum + (int)numSkillVanillaPlus.Value;
					else if (cbxSkillRand.Checked & cbxSkillSetNum.Checked) // set number for all characters
						numskills = (int)numSkillSet.Value;
					if (cbxParagon.Checked | (islaguz & cbxLaguzParagon.Checked))
						numskills += 1;

					if (numskills > 6)
						numskills = 6;

					for (int k = 0; k < numskills; k++)
					{
						if (k == 0 & (cbxParagon.Checked | (islaguz & cbxLaguzParagon.Checked)))
						{
							// give paragon as first skill, then remove from possible skills
							if (!newskills.Contains("SID_PARAGON"))
								newskills += ";" + "SID_PARAGON";
							possibleskills.Remove("SID_PARAGON");
						}
						else
						{
							int randskill = random.Next(possibleskills.Count);
							newskills += ";" + possibleskills[randskill];
							possibleskills.RemoveAt(randskill);
						}
					}

					CharacterData.Write(characters[i].PID, "Skills", newskills.Split(';'));

				}

				// authority stars
				if (cbxAuthority.Checked)
					CharacterData.Write(characters[i].PID, "Authority", random.Next(6));

				// biorhythm
				if (cbxBio.Checked)
					CharacterData.Write(characters[i].PID, "Biorhythm", random.Next(10));

				// affinity
				if (cbxAffinity.Checked)
				{
					string[] affinities = new string[8] { "telius", "thunder", "wind", "water", "heaven", "fire", "dark", "light" };
					CharacterData.Write(characters[i].PID, "Affinity", affinities[random.Next(8)]);
				}
			}
		}

		// swap portraits for random recruitment
		private void RecrFaceSwap()
		{
			// 72 playable characters
			int totalchars = 72;
			if (cbxEnemyRecruit.Checked & cbxRandRecr.Checked)
			{
				// include enemy characters, but don't include BK or ashera if class rando is off
				if (cbxClassRand.Checked | cbxClassSwap.Checked | cbxRecrVanillaClass.Checked)
					totalchars = 86;
				else
					totalchars = 84;
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
					int[,] facedata = new int[totalchars, 36];
					// open facedata file
					using (var stream = new System.IO.FileStream(facefile, System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						// read in all face data
						for (int i = 0; i < totalchars; i++)
						{
							stream.Position = characters[i].FIDLoc + 12;
							for (int j = 0; j < 36; j++)
								facedata[i, j] = stream.ReadByte();
						}

						// swap that data baby
						for (int charNum = 0; charNum < totalchars; charNum++)
						{
							if (characters[charNum].RecrInverse != -1)
							{
								stream.Position = characters[characters[charNum].NewRecr].FIDLoc + 12;
								for (int j = 0; j < 4; j++)
									stream.WriteByte(Convert.ToByte(facedata[charNum, j]));



								//stream.Position = characters[charNum].FIDLoc + 20;
								//for (int j = 8; j < 36; j++)
								//	stream.WriteByte(Convert.ToByte(facedata[characters[charNum].NewRecr, j]));
							}
						}

						if (characters[0].NewRecr != -1)
						{
							// micaiah2
							stream.Position = 6464 + 20;
							for (int i = 8; i < 36; i++)
								stream.WriteByte(Convert.ToByte(facedata[characters[0].NewRecr, i]));
							// micaiah3
							stream.Position = 6608 + 20;
							for (int i = 8; i < 36; i++)
								stream.WriteByte(Convert.ToByte(facedata[characters[0].NewRecr, i]));
						}

						if (characters[5].NewRecr != -1)
						{
							// sothe2
							stream.Position = 224 + 20;
							for (int i = 8; i < 36; i++)
								stream.WriteByte(Convert.ToByte(facedata[characters[5].NewRecr, i]));
						}

						if (characters[8].NewRecr != -1)
						{
							// meg2
							stream.Position = 6992 + 20;
							for (int i = 8; i < 36; i++)
								stream.WriteByte(Convert.ToByte(facedata[characters[8].NewRecr, i]));
						}

						if (characters[25].NewRecr != -1)
						{
							// lucia2
							stream.Position = 7664 + 20;
							for (int i = 8; i < 36; i++)
								stream.WriteByte(Convert.ToByte(facedata[characters[25].NewRecr, i]));
						}

						if (characters[34].NewRecr != -1)
						{
							// ike2
							//break;
							stream.Position = 3344 + 20;
							for (int i = 8; i < 36; i++)
								stream.WriteByte(Convert.ToByte(facedata[characters[34].NewRecr, i]));
						}
					}
				}
			}
			catch
			{
				textBox1.Text = "Errorcode 09: Cannot find portrait files! Randomization incomplete!";
				errorflag = 1;
			}
		}

		// part of crimean softlock prevention intiative - obtain mordy and lethe one chapter earlier
		private void addLetheMordyEarly()
		{
			ChapterChar oldlethe = new ChapterChar();
			ChapterChar oldmordy = new ChapterChar();
			ChapterChar newlethe, newmordy;

			// read in data for mordy and lethe from 2_3
			for (int i = 0; i < chapters.Length; i++)
			{
				if (chapters[i] == "203")
				{
					oldlethe = ChapterData[i].Read("PID_RETHE");
					oldmordy = ChapterData[i].Read("PID_MORDY");
					break;
				}
			}
			// save to 2_2
			for (int i = 0; i < chapters.Length; i++)
			{
				if (chapters[i] == "202")
				{
					// initialize with neph's data
					newlethe = ChapterData[i].Read("PID_NEPHENEE");
					newmordy = ChapterData[i].Read("PID_NEPHENEE");

					// overwrite with old class, weapons, items, and skills
					newlethe.PID = oldlethe.PID;
					newlethe.JID = oldlethe.JID;
					newlethe.Weapons = oldlethe.Weapons;
					newlethe.Items = oldlethe.Items;
					newlethe.Skills = oldlethe.Skills;
					newlethe.Level = oldlethe.Level;
					newmordy.PID = oldmordy.PID;
					newmordy.JID = oldmordy.JID;
					newmordy.Weapons = oldmordy.Weapons;
					newmordy.Items = oldmordy.Items;
					newmordy.Skills = oldmordy.Skills;
					newmordy.Level = oldmordy.Level;

					// change location
					newlethe.CutsceneLoc = new int[2] { 26, 25 };
					newlethe.Location = new int[2] { 26, 25 };
					newmordy.CutsceneLoc = new int[2] { 25, 26 };
					newmordy.Location = new int[2] { 25, 26 };

					// insert into chapter
					ChapterData[i].Insert(newlethe, "PID_NEPHENEE");
					ChapterData[i].Insert(newmordy, "PID_NEPHENEE");
					break;
				}
			}
		}

		#endregion

		// *************************************************************************************** STAT FUNCTIONS
		#region

		// makes changes to character/class base stats in (vain)
		// attempt to balance random classes
		private void baseStatChanges()
		{
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\NewBases.csv");
			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			// only need to randomize personal bases when character units are randomized
			int number;
			if (classeschanged)
				number = lines.Length;
			else
				number = 82;

			for (int i = 1; i < number; i++)
			{
				string[] split = lines[i].Split(',');
				string ID = split[0];
				string statchange = split[1];
				int[] newbases = new int[8];
				for (int j = 0; j < 8; j++)
					newbases[j] = Convert.ToInt32(split[j + 2]);

				// change rogue to female rogue if classpatch on
				if (cbxClassPatch.Checked & ID == "JID_ROGUE")
					ID = "JID_ROGUE_F";

				// save to data file
				if (ID.StartsWith("JID"))
					ClassData.Write(ID, statchange, newbases);
				else if (ID.StartsWith("PID"))
					CharacterData.Write(ID, statchange, newbases);
			}
		}

		// does the growth randomization
		private void growthRateModifier()
		{
			for (int i = 0; i < characters.Length; i++)
			{
				if (characters[i].Chapter != "0") // only for playable characters
				{
					int[] growths = CharacterData.ReadIntArray(characters[i].PID, "Growths");
					// user selected minimums
					int[] setminimum = new int[8]{Convert.ToInt32(numericHP.Value),Convert.ToInt32(numericATK.Value),Convert.ToInt32(numericMAG.Value),Convert.ToInt32(numericSKL.Value),
											Convert.ToInt32(numericSPD.Value),Convert.ToInt32(numericLCK.Value),Convert.ToInt32(numericDEF.Value),Convert.ToInt32(numericRES.Value) };
					for (int j = 0; j < growths.Length; j++)
					{
						if (cbxZeroGrowths.Checked)
							growths[j] = 0;
						else
						{
							int mingrowth = growths[j] - Convert.ToInt32(numericGrowth.Value);
							int maxgrowth = growths[j] + Convert.ToInt32(numericGrowth.Value);
							// set min and max based on user selection
							if (cbxGrowthCap.Checked & maxgrowth > numericGrowthCap.Value)
								maxgrowth = (int)numericGrowthCap.Value + 1;
							if (cbxGrowthCap.Checked & mingrowth > numericGrowthCap.Value)
								mingrowth = (int)numericGrowthCap.Value;
							if (maxgrowth < setminimum[j])
								maxgrowth = setminimum[j] + 1;
							if (mingrowth < setminimum[j])
								mingrowth = setminimum[j];
							// random growth
							int randgrowth = random.Next(mingrowth, maxgrowth);
							// check for overflow
							if (randgrowth > 255)
								randgrowth = 255;
							else if (randgrowth < 0)
								randgrowth = 0;
							// save
							growths[j] = randgrowth;
						}
					}
					// write new growths
					CharacterData.Write(characters[i].PID, "Growths", growths);
				}
			}
		}

		// does the growth shuffling
		private void growthShuffle()
		{
			for (int i = 0; i < characters.Length; i++)
			{
				if (characters[i].Chapter != "0") // only for playable characters
				{
					int[] growths = CharacterData.ReadIntArray(characters[i].PID, "Growths");
					int totalgrowth = Convert.ToInt32(numericGrowthShuffle.Value);
					for (int j = 0; j < growths.Length; j++)
						totalgrowth += growths[j];

					// use shuffle function to do things
					int[] newgrowths = shuffler(totalgrowth, 8);
					//check for growths under 15%
					if (cbxGrowthShuffleMin.Checked)
					{
						for (int k = 0; k < 8; k++)
						{
							while (newgrowths[k] < 15)
							{
								newgrowths[k] += 1;
								int dev = 15 - newgrowths[k];
								for (int j = k + 1; j < k + 8; j++)
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
								newgrowths[k] = 15;
							}
						}
					}
					int maximum = 255;
					if (cbxGrowthShuffleMax.Checked)
						maximum = 100;
					// check for growths greater than max
					for (int k = 0; k < 8; k++)
					{
						if (newgrowths[k] > maximum)
						{
							int dev = newgrowths[k] - maximum;
							for (int j = k + 1; j < k + 8; j++)
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
							newgrowths[k] = maximum;
						}
						if (newgrowths[k] > 255)
							newgrowths[k] = 255;
					}
					// write
					CharacterData.Write(characters[i].PID, "Growths", newgrowths);
				}
			}
		}

		// modifies class base stats by deviation or by shuffling
		private void classBaseStats()
		{
			// read from file
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\JIDlist.txt");
			string[] JIDs = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			for (int i = 0; i < JIDs.Length; i++)
			{
				int[] bases = ClassData.ReadIntArray(JIDs[i], "Bases");
				string transclass = ClassData.ReadString(JIDs[i], "Transform_Class");
				string[] skills = ClassData.ReadStringArray(JIDs[i], "Skills");
				if (transclass != "" & String.Join(",", skills).Contains("SID_LYCANTHROPE")) // this is a transformed laguz class - needs same bases as untransformed
				{
					ClassData.Write(JIDs[i], "Bases", ClassData.ReadIntArray(transclass, "Bases"));
				}
				else
				{
					if (cbxRandClassBases.Checked)
					{
						for (int j = 0; j < 8; j++)
						{
							if (bases[j] > 127)
								bases[j] -= 256;
							int minbase = bases[j] - Convert.ToInt32(numericClassBaseDev.Value);
							int maxbase = bases[j] + Convert.ToInt32(numericClassBaseDev.Value);
							// prevent class bases from being less than zero
							if (minbase < 0)
								minbase = 0;
							if (maxbase < 0)
								maxbase = 0;
							if (minbase > 127)
								minbase = 127;
							if (maxbase > 127)
								maxbase = 127;


							int randstat = random.Next(minbase, maxbase + 1);
							bases[j] = randstat;

						}
						ClassData.Write(JIDs[i], "Bases", bases);
					}
					else if (cbxShuffleClassBases.Checked)
					{
						int[] newstats = new int[8];
						int hpval = bases[0];
						int totalstat = Convert.ToInt32(numericClassBaseShuf.Value);
						for (int j = 0; j < bases.Length; j++)
						{
							if ((!cbxHPShuffleclass.Checked & j == 0) | j == 5) // don't add hp and luck
							{ }
							else // everything else is added to total
								totalstat += bases[j];
						}

						int[] shuffledstats;
						if (cbxHPShuffleclass.Checked)
							shuffledstats = shuffler(totalstat, 7);
						else
							shuffledstats = shuffler(totalstat, 6);

						// move stats and insert hp and lck
						if (!cbxHPShuffleclass.Checked)
						{
							newstats[7] = shuffledstats[5]; // res
							newstats[6] = shuffledstats[4]; // def
							newstats[5] = 0;                // lck
							newstats[4] = shuffledstats[3]; // spd
							newstats[3] = shuffledstats[2]; // skl
							newstats[2] = shuffledstats[1]; // mag
							newstats[1] = shuffledstats[0]; // atk
							newstats[0] = hpval;            // hp
						}
						else // luck is always zero for classes
						{
							newstats[7] = shuffledstats[6]; // res
							newstats[6] = shuffledstats[5]; // def
							newstats[5] = 0;                // lck
							for (int k = 0; k < 5; k++)
								newstats[k] = shuffledstats[k];
						}

						for (int x = 0; x < newstats.Length; x++)
							if (newstats[x] > 127)
								newstats[x] = 127;

						// write
						ClassData.Write(JIDs[i], "Bases", newstats);
					}
				}
			}
		}

		// randomizes base stats by deviation
		private void randBaseStats()
		{
			for (int i = 0; i < characters.Length; i++)
			{
				if (characters[i].Chapter != "0") // only for playable characters
				{
					if (characters[i].NewClass == -1)
						characters[i].NewClass = characters[i].VanillaClass;
					int[] bases = CharacterData.ReadIntArray(characters[i].PID, "Bases");
					int[] classbases = ClassData.ReadIntArray(classes[characters[i].NewClass].JID, "Bases");

					for (int j = 0; j < bases.Length; j++)
						if (bases[j] > 127)
							bases[j] -= 256;

					for (int j = 0; j < bases.Length; j++)
					{
						int minbase = bases[j] - Convert.ToInt32(numericBaseRand.Value);
						int maxbase = bases[j] + Convert.ToInt32(numericBaseRand.Value);
						// prevent personal + class bases from being less than zero
						if (minbase + classbases[j] < 0)
							minbase = -classbases[j];
						if (maxbase + classbases[j] < 0)
							maxbase = -classbases[j];
						// randomize
						bases[j] = random.Next(minbase, maxbase + 1);
					}

					for (int j = 0; j < bases.Length; j++)
					{
						if (bases[j] > 127)
							bases[j] = 127;
						if (bases[j] < 0)
							bases[j] += 256;
					}
					CharacterData.Write(characters[i].PID, "Bases", bases);
				}
			}
		}

		// shuffles base stats with possible deviation
		private void shuffleBaseStats()
		{
			for (int i = 0; i < characters.Length; i++)
			{
				if (characters[i].Chapter != "0") // only for playable characters
				{
					int[] bases = CharacterData.ReadIntArray(characters[i].PID, "Bases");

					for (int j = 0; j < bases.Length; j++)
						if (bases[j] > 127)
							bases[j] -= 256;

					int[] newstats = new int[8];
					int hpval = bases[0];
					int lckval = bases[5];
					int totalstat = Convert.ToInt32(numericBaseShuffle.Value);
					for (int j = 0; j < bases.Length; j++)
					{
						if (!cbxHPLCKShuffle.Checked & (j == 0) | j == 5) // don't add hp and luck
						{ }
						else // everything else is added to total
							totalstat += bases[j];
					}

					int[] shuffledstats;
					if (cbxHPLCKShuffle.Checked)
						shuffledstats = shuffler(totalstat, 8);
					else
						shuffledstats = shuffler(totalstat, 6);

					// move stats and insert hp and lck
					if (!cbxHPLCKShuffle.Checked)
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
						for (int k = 0; k < 8; k++)
							newstats[k] = shuffledstats[k];
					}

					// write
					for (int j = 0; j < newstats.Length; j++)
					{
						if (newstats[j] > 127)
							newstats[j] = 127;
						if (newstats[j] < 0)
							newstats[j] += 256;
					}
					CharacterData.Write(characters[i].PID, "Bases", newstats);
				}
			}
		}

		// swaps str and mag growth/bases for units who changed from physical
		// to magical or vice-versa
		private void strMagSwap()
		{
			for (int i = 0; i < 72; i++)
			{
				if (characters[i].NewClass != -1)
				{
					int newclassint = characters[i].NewClass;
					if (newclassint != -1)
					{
						string classtype = classes[newclassint].Classtype_P;
						string jid = classes[newclassint].JID;
						int[] bases = CharacterData.ReadIntArray(characters[i].PID, "Bases");
						int[] growths = CharacterData.ReadIntArray(characters[i].PID, "Growths");
						bool ismagic = classtype == "M" | jid == "JID_WHITEDRAGON";

						// bases are signed bytes, so anything over 127 is negative
						int strbase = bases[1];
						int magbase = bases[2];
						if (strbase > 127)
							strbase -= 256;
						if (magbase > 127)
							magbase -= 127;

						if ((ismagic & strbase > magbase) | (!ismagic & strbase < magbase))
						{
							// save original base value, not calculated negative number
							int temp = bases[1];
							bases[1] = bases[2];
							bases[2] = temp;
						}
						if ((ismagic & growths[1] > growths[2]) | (!ismagic & growths[1] < growths[2]))
						{
							int temp = growths[1];
							growths[1] = growths[2];
							growths[2] = temp;
						}
						// write
						CharacterData.Write(characters[i].PID, "Bases", bases);
						CharacterData.Write(characters[i].PID, "Growths", growths);
					}
				}
			}
		}

		// sets stat caps to desired values
		private void statCapChanges()
		{
			// read from file
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\JIDlist.txt");
			string[] JIDs = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			for (int i = 0; i < JIDs.Length; i++)
			{
				int[] statcaps = ClassData.ReadIntArray(JIDs[i], "Caps");
				string transclass = ClassData.ReadString(JIDs[i], "Transform_Class");
				string[] skills = ClassData.ReadStringArray(JIDs[i], "Skills");

				if (transclass != "" & String.Join(",", skills).Contains("SID_LYCANTHROPE")) // this is a transformed laguz class - needs double caps as untransformed
				{
					int[] transstatcaps = ClassData.ReadIntArray(transclass, "Caps");
					for (int j = 1; j < 8; j++)
					{
						if (j != 5) // luck is the same
						{
							transstatcaps[j] = transstatcaps[j] * 2;
							if (transstatcaps[j] > 115)
								transstatcaps[j] = 115;
						}
					}
					ClassData.Write(JIDs[i], "Caps", transstatcaps);
				}
				else
				{
					int tier;
					if (String.Join(",", skills).Contains("SID_HIGHEST")) // tier 3 has HIGHEST skill
						tier = 3;
					else if (String.Join(",", skills).Contains("SID_HIGHER") | String.Join(",", skills).Contains("SID_ANIMALIZE")) // tier 2 or laguz class
						tier = 2;
					else
						tier = 1;

					if (!cbxT3Statcaps.Checked | (cbxT3Statcaps.Checked & tier == 3))
					{
						int[] newcaps = new int[8];
						if (cbxStatCaps.Checked)
						{
							int cap2write;
							if (tier == 1)
								cap2write = (int)numericStatCap1.Value;
							else if (tier == 2)
								cap2write = (int)numericStatCap2.Value;
							else
								cap2write = (int)numericStatCap3.Value;
							// set hp to max
							newcaps[0] = 115;
							for (int j = 1; j < 8; j++)
								newcaps[j] = cap2write;
						} // set to user specification
						else if (cbxStatCapDev.Checked)
						{
							for (int j = 0; j < 8; j++)
							{
								int min = statcaps[j] - (int)numericStatCapDev.Value;
								int max = statcaps[j] + (int)numericStatCapDev.Value;
								if (min < numericStatCapMin.Value)
									min = (int)numericStatCapMin.Value;
								if (min > 115)
									min = 115;
								if (max < numericStatCapMin.Value)
									max = (int)numericStatCapMin.Value;
								if (max > 115)
									max = 115;
								newcaps[j] = random.Next(min, max + 1);
							}
						} // random deviation from vanilla
						else if (cbxStatCapFlat.Checked)
						{
							for (int j = 0; j < 8; j++)
							{
								int temp = statcaps[j] + (int)numericStatCapFlat.Value;
								if (temp > 115)
									temp = 115;
								if (temp < 0)
									temp = 0;
								if (temp < numericStatCapMin.Value)
									temp = (int)numericStatCapMin.Value;
								newcaps[j] = temp;
							}
						} // set addition above vanilla
						  // write
						ClassData.Write(JIDs[i], "Caps", newcaps);
					}
				}
			}
		}

		#endregion

		// *************************************************************************************** WEAPON & ITEM FUNCTIONS
		#region

		// changes stats of magic tomes
		private void LM73MagicPatch()
		{

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\LM73MagicPatch.csv");
			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			// fe10data changes
			for (int i = 1; i < lines.Length; i++)
			{
				string[] split = lines[i].Split(',');
				string IID = split[0];
				string rank = split[5];
				int MT = Convert.ToInt32(split[6]);
				int HIT = Convert.ToInt32(split[7]);
				int Crit = Convert.ToInt32(split[8]);
				int WT = Convert.ToInt32(split[9]);
				int minrange = Convert.ToInt32(split[10]);
				int maxrange = Convert.ToInt32(split[11]);
				int uses = Convert.ToInt32(split[12]);
				int cost = Convert.ToInt32(split[13]);
				string attributes = split[14];
				string effective = split[15];
				string bonus = split[16];

				ItemData.Write(IID, "Rank", rank);
				ItemData.Write(IID, "MT", MT);
				ItemData.Write(IID, "HIT", HIT);
				ItemData.Write(IID, "CRIT", Crit);
				ItemData.Write(IID, "WT", WT);
				ItemData.Write(IID, "MinRange", minrange);
				ItemData.Write(IID, "MaxRange", maxrange);
				ItemData.Write(IID, "Uses", uses);
				ItemData.Write(IID, "Cost/Use", cost);
				if (attributes != "")
					ItemData.Write(IID, "Attributes", attributes.Split(';'));
				if (effective != "")
					ItemData.Write(IID, "Effectiveness", effective.Split(';'));
				if (bonus != "")
					ItemData.Write(IID, "Bonuses", bonus.Split(';'));
			}

			// script changes, if not trial
			if (!cbxTrialMode.Checked)
			{
				for (int i = 1; i < lines.Length; i++)
				{
					string[] split = lines[i].Split(',');
					string IID = split[0];
					string new_tomename = split[1];
					// new description
					string new_tomedesc = split[3];
					int tome_nameloc, tome_descloc;
					if (gameVersion == 2) // PAL iso
					{
						// location to write name in e_common.m
						tome_nameloc = Convert.ToInt32(split[17]);
						// location of description in e_common.m
						tome_descloc = Convert.ToInt32(split[18]);
					}
					else
					{
						// location to write name in e_common.m
						tome_nameloc = Convert.ToInt32(split[2]);
						// location of description in e_common.m
						tome_descloc = Convert.ToInt32(split[4]);
					}

					// change text for name and description
					using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_common.m", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						if (tome_nameloc != 0)
						{
							stream.Position = tome_nameloc;
							byte[] namestring = System.Text.Encoding.ASCII.GetBytes(new_tomename);
							stream.Write(namestring, 0, namestring.Length);
							stream.WriteByte(0x00);
						}
						if (tome_descloc != 0)
						{
							string[] description = new_tomedesc.Split(';');
							stream.Position = tome_descloc;
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
		}

		// changes venin/bronze weapons to cooler things
		private void LM73WeapPatch()
		{
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\LM73WeaponPatch.csv");
			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			// fe10data changes
			for (int i = 1; i < lines.Length; i++)
			{
				string[] split = lines[i].Split(',');
				string IID = split[0];
				string damagetype = split[1];
				string rank = split[6];
				int MT = Convert.ToInt32(split[7]);
				int HIT = Convert.ToInt32(split[8]);
				int Crit = Convert.ToInt32(split[9]);
				int WT = Convert.ToInt32(split[10]);
				int minrange = Convert.ToInt32(split[11]);
				int maxrange = Convert.ToInt32(split[12]);
				int uses = Convert.ToInt32(split[13]);
				int cost = Convert.ToInt32(split[14]);
				string attributes = split[15];
				string effective = split[16];
				string bonus = split[17];

				if (damagetype != "")
					ItemData.Write(IID, "Damage_Type", damagetype);
				ItemData.Write(IID, "Rank", rank);
				ItemData.Write(IID, "MT", MT);
				ItemData.Write(IID, "HIT", HIT);
				ItemData.Write(IID, "CRIT", Crit);
				ItemData.Write(IID, "WT", WT);
				ItemData.Write(IID, "MinRange", minrange);
				ItemData.Write(IID, "MaxRange", maxrange);
				ItemData.Write(IID, "Uses", uses);
				ItemData.Write(IID, "Cost/Use", cost);
				ItemData.Write(IID, "Attributes", attributes.Split(';'));
				ItemData.Write(IID, "Effectiveness", effective.Split(';'));
				ItemData.Write(IID, "Bonuses", bonus.Split(';'));
			}
			// change weapon ranks to be minimum D for sword/axe/lance/bow
			ClassData.Write("JID_BLADEDUX", "Base_WeaponRank", "CD----------");
			ClassData.Write("JID_BLADEDUX_F", "Base_WeaponRank", "CD----------");
			ClassData.Write("JID_GREATDUX", "Base_WeaponRank", "D-C---------");
			ClassData.Write("JID_FALCONKNIGHT", "Base_WeaponRank", "DC----------");

			// script changes, if not trial
			if (!cbxTrialMode.Checked)
			{
				for (int i = 1; i < lines.Length; i++)
				{
					string[] split = lines[i].Split(',');
					string IID = split[0];
					string new_weapname = split[2];
					// new description
					string new_weapdesc = split[4];
					int weap_nameloc, weap_descloc;
					if (gameVersion == 2) // PAL iso
					{
						// location to write name in e_common.m
						weap_nameloc = Convert.ToInt32(split[18]);
						// location of description in e_common.m
						weap_descloc = Convert.ToInt32(split[19]);
					}
					else
					{
						// location to write name in e_common.m
						weap_nameloc = Convert.ToInt32(split[3]);
						// location of description in e_common.m
						weap_descloc = Convert.ToInt32(split[5]);
					}

					// change text for name and description
					using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_common.m", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						if (weap_nameloc != 0)
						{
							stream.Position = weap_nameloc;
							byte[] namestring = System.Text.Encoding.ASCII.GetBytes(new_weapname);
							stream.Write(namestring, 0, namestring.Length);
							stream.WriteByte(0x00);
						}
						if (weap_descloc != 0)
						{
							string[] description = new_weapdesc.Split(';');
							stream.Position = weap_descloc;
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
		}

		// adds crit to knives
		private void knifeModifier()
		{
			string[] knives = new string[13] { "IID_BRONZEKNIFE", "IID_IRONKNIFE", "IID_STEELKNIFE", "IID_SILVERKNIFE",
											  "IID_BRONZEDAGGER", "IID_IRONDAGGER", "IID_STEELDAGGER", "IID_SILVERDAGGER",
											  "IID_KARD","IID_STILETTO","IID_BEASTKILLER","IID_PESHKABZ","IID_BASELARD" };

			for (int i = 0; i < knives.Length; i++)
			{
				int crit = ItemData.ReadInt(knives[i], "CRIT");
				crit += 5;
				ItemData.Write(knives[i], "CRIT", crit);
			}
		}

		// randomizes weapon mt,acc,wt,etc
		private void weaponRandomizer()
		{
			textBox1.Text = "Screwing up weapons";
			Application.DoEvents();

			int[] weapMin = new int[5];
			int[] weapDev = new int[5];
			int[] weapMax = new int[5];

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

			// read from file
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\weaponlist.txt");
			string[] IIDs = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			for (int i = 0; i < IIDs.Length; i++)
			{
				string type = ItemData.ReadString(IIDs[i], "Weapon_Type");
				string rank = ItemData.ReadString(IIDs[i], "Rank");
				int uses = ItemData.ReadInt(IIDs[i], "Uses");
				string[] attributes = ItemData.ReadStringArray(IIDs[i], "Attributes");
				if (type == "blow" & !cbxLaguzWeap.Checked) // laguz weapon type
				{ }
				else
				{
					int vanilaStat, minStat, maxStat;
					string[] statnames = new string[5] { "MT", "HIT", "CRIT", "WT", "Uses" };
					for (int j = 0; j < 5; j++)
					{
						// read in stat
						vanilaStat = ItemData.ReadInt(IIDs[i], statnames[j]);

						if (cbxStaveUse.Checked & type == "rod" & statnames[j] == "Uses" & uses < 15)
						{ } // keep uses of rare staves
						else if (cbxSiegeUse.Checked & String.Join(",", attributes).Contains("longfar") & statnames[j] == "Uses")
						{ } // keep uses of siege tomes/ballistae
						else
						{
							// only change if deviation is not zero
							if (weapDev[j] != 0 | vanilaStat < weapMin[j] | vanilaStat > weapMax[j])
							{
								// calculate min and max possible values
								minStat = vanilaStat - weapDev[j];
								maxStat = vanilaStat + weapDev[j];

								// if S or SS rank laguz weapon, force min to be value of previous (max for WT)
								if (type == "blow" & (rank == "S" | rank == "*"))
								{
									int prev_val = ItemData.ReadInt(IIDs[i - 1], statnames[j]);
									// return to current weapon
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
								int randStat = random.Next(minStat, maxStat + 1);
								ItemData.Write(IIDs[i], statnames[j], randStat);
							}
						}
					}
				}
			}
		}

		// creates array of random items for writing to armory, shop, or bargain
		private string[] chooseShopItems(string[] shoplines, string[] allitems, string shoptype, int startint)
		{
			List<string> possibleitems = new List<string>();
			List<string> possibleitemtypes = new List<string>();
			List<string> chosenitems = new List<string>();

			// basic items for the ironshop selection
			string[] basics = new string[0];
			if (shoptype == "WSHOP")
				basics = new string[10] { "IID_IRONSWORD", "IID_IRONLANCE", "IID_IRONAXE", "IID_IRONBOW", "IID_IRONKNIFE",
											"IID_FIRE", "IID_THUNDER", "IID_WIND", "IID_LIGHT", "IID_WORM" };
			else if (shoptype == "ISHOP")
				basics = new string[3] { "IID_HERB", "IID_OLIVI", "IID_LIVE" };

			// get list of possible items
			for (int i = 0; i < allitems.Length; i++)
			{
				string[] split = allitems[i].Split(',');
				if (shoptype == "WSHOP" & (split[1] == "i" | split[1] == "h" | split[1] == "p" | split[1] == "x"))
				{ } // must be a weapon for armory and forge
				else if (shoptype == "ISHOP" & !(split[1] == "i" | split[1] == "h" | split[1] == "p" | split[1] == "x"))
				{ } // must not be a weapon for shop
				else
				{
					possibleitems.Add(split[0]);
					possibleitemtypes.Add(split[1]);
				}
			}

			// add troop scroll
			if (cbxFormshift.Checked & shoptype == "ISHOP")
			{
				possibleitems.Add("IID_TROOP");
				possibleitemtypes.Add("x");
			}

			// set up weights based on user selection
			int[] bargainweights = new int[10] { (int)numericBargSword.Value,(int)numericBargLance.Value,(int)numericBargAxe.Value,(int)numericBargBow.Value,
												(int)numericBargKnife.Value,(int)numericBargTome.Value,(int)numericBargStave.Value,(int)numericBargItem.Value,
												(int)numericBargStat.Value, (int)numericBargSkill.Value};
			string[] bargaintypes = new string[10] { "s", "l", "a", "b", "k", "t", "h", "i", "p", "x" };
			int bargainweighttotal = 0;
			for (int i = 0; i < bargainweights.Length; i++)
				bargainweighttotal += bargainweights[i];

			int currint = startint;
			bool addedweaps = false;
			// // loop and choose armory, shop, and bargain changes
			while (shoplines[currint].StartsWith("IID_") & possibleitems.Count != 0)
			{
				string[] itemdeets = shoplines[currint].Split(',');
				// armory and shop use either the basic items (if ironshop is selected) or random items
				if (cbxRandShop.Checked & (shoptype == "WSHOP" | (shoptype == "ISHOP" & itemdeets[1] == "0")))
				{
					if (cbxIronShop.Checked & (currint - startint < basics.Length))
					{
						chosenitems.Add(basics[currint - startint]);
						int index = possibleitems.IndexOf(basics[currint - startint]);
						if (index != -1)
						{
							possibleitems.RemoveAt(index);
							possibleitemtypes.RemoveAt(index);
						}
					}
					else
					{
						int randchoice = random.Next(possibleitems.Count);
						chosenitems.Add(possibleitems[randchoice]);
						possibleitems.RemoveAt(randchoice);
						possibleitemtypes.RemoveAt(randchoice);
					}
				}
				// dormammu ive come to bargain
				else if (cbxBargains.Checked & shoptype == "ISHOP" & itemdeets[1] == "1")
				{
					// add all weapons to possible item list, as weapons can show up in bargains
					if (!addedweaps)
					{
						for (int i = 0; i < allitems.Length; i++)
						{
							string[] split = allitems[i].Split(',');
							if (!(split[1] == "i" | split[1] == "h" | split[1] == "p" | split[1] == "x"))
							{
								possibleitems.Add(split[0]);
								possibleitemtypes.Add(split[1]);
							}
						}
						// only do this once
						addedweaps = true;
					}

					// choose type from weights
					string typechoice = "";
					if (bargainweighttotal == 0)
						typechoice = bargaintypes[random.Next(bargaintypes.Length)];
					else
					{
						int randtype = random.Next(bargainweighttotal);
						for (int i = 0; i < bargainweights.Length; i++)
						{
							if (randtype < bargainweights[i])
							{
								typechoice = bargaintypes[i];
								break;
							}
							else
								randtype -= bargainweights[i];
						}
					}

					// split 't' for tome into f,w,t,m,d (fire, wind, thunder, light, dark)
					string[] magictypes = new string[5] { "f", "t", "w", "m", "d" };
					if(typechoice == "t")
						typechoice = magictypes[random.Next(5)];


					if (typechoice != "")
					{
						int randchoice = random.Next(possibleitems.Count);
						while (possibleitemtypes[randchoice] != typechoice)
							randchoice = random.Next(possibleitems.Count);
						chosenitems.Add(possibleitems[randchoice]);
						bargainOutput += "," + possibleitems[randchoice];
						possibleitems.RemoveAt(randchoice);
						possibleitemtypes.RemoveAt(randchoice);
					}
					else
						chosenitems.Add("");
				}
				else
					chosenitems.Add("");

				currint++;
			}



			return (chosenitems.ToArray());
		}

		// creates array of random items for writing to forge
		private string[] chooseForgeItems(string[] shoplines, string[] allitems, int startint)
		{
			// forge changes
			List<string> possibleitems = new List<string>();
			List<string> possibleitemtypes = new List<string>();
			List<string> chosenitems = new List<string>();
			int currint = startint;

			for (int i = 0; i < allitems.Length; i++)
			{
				string[] split = allitems[i].Split(',');
				if (split[1] == "i" | split[1] == "h" | split[1] == "p" | split[1] == "g" | split[1] == "x")
				{ } // must be a weapon for forge
				if (split[0].Contains("IID_ALONDITE") | split[0].Contains("IID_FLORETE") | split[0].Contains("IID_ETTARD") | split[0].Contains("IID_CYMBELINE"))
				{ } // cannot be in forge
				else
				{
					possibleitems.Add(split[0]);
					possibleitemtypes.Add(split[1]);
				}
			}

			// set up weights based on user selection
			int[] forgeweights = new int[6] { (int)numericForgeSword.Value,(int)numericForgeLance.Value,(int)numericForgeAxe.Value,
											  (int)numericForgeBow.Value,(int)numericForgeKnife.Value,(int)numericForgeTome.Value };
			string[] forgetypes = new string[6] { "s", "l", "a", "b", "k", "t" };
			int forgeweighttotal = 0;
			for (int i = 0; i < forgeweights.Length; i++)
				forgeweighttotal += forgeweights[i];

			// loop and choose weapons
			while (shoplines[currint].StartsWith("MIK_") & possibleitems.Count != 0)
			{
				if (shoplines[currint].Contains("IID_"))
				{
					string[] itemdeets = shoplines[currint].Split(',');

					// choose type from weights
					string typechoice = "";
					if (forgeweighttotal == 0)
						typechoice = forgetypes[random.Next(forgetypes.Length)];
					else
					{
						int randtype = random.Next(forgeweighttotal);
						for (int i = 0; i < forgeweights.Length; i++)
						{
							if (randtype < forgeweights[i])
							{
								typechoice = forgetypes[i];
								break;
							}
							else
								randtype -= forgeweights[i];
						}
					}

					if (typechoice != "")
					{
						int attempts = 0;
						int randchoice = random.Next(possibleitems.Count);
						while (possibleitemtypes[randchoice] != typechoice)
						{
							randchoice = random.Next(possibleitems.Count);
							attempts += 1;
							if (attempts >= 1000)
								break;
						}
						chosenitems.Add(possibleitems[randchoice]);
						possibleitems.RemoveAt(randchoice);
						possibleitemtypes.RemoveAt(randchoice);
					}
					else
						chosenitems.Add("");
				}
				currint++;
			}

			return (chosenitems.ToArray());
		}

		#endregion

		// *************************************************************************************** ENEMY FUNCTIONS
		#region

		// determines random enemy class based on user parameters
		private ChapterChar chooseEnemyClass(ChapterChar editchar, string chaptername)
		{
			string JID = editchar.JID;
			string classtype, tier;
			string skills = String.Join(",", ClassData.ReadStringArray(JID, "Skills"));
			string types = String.Join(",", ClassData.ReadStringArray(JID, "Class_Types"));

			if (types.Contains("SFXC_ALIZE"))
				classtype = "L"; // laguz
			else if (types.Contains("SFXC_ARMOR"))
				classtype = "A"; // armor
			else if (skills.Contains("SID_FLY"))
				classtype = "F"; // flier
			else if (types.Contains("SFXC_KNIGHT"))
				classtype = "C"; // cavalry
			else if (JID == "JID_PRIEST" | JID == "JID_BISHOP" | JID == "JID_BISHOP_SP" | JID == "JID_SAINT_SP" | JID == "JID_CLERIC" | JID == "JID_VALKYRIA")
				classtype = "H"; // healer
			else if (types.Contains("SFXC_MAGE") | JID.Contains("SPIRIT"))
				classtype = "M"; // magic
			else
				classtype = "I"; // infantry

			if (skills.Contains("SID_HIGHEST"))
			{
				if (cbxTier3Enemies.Checked)
					tier = "4";
				else
					tier = "3";
			}
			else if (skills.Contains("SID_HIGHER") | (types.Contains("SFXC_ALIZE") & !chaptername.StartsWith("1"))) // laguz are considered t1 for part 1 enemies
				tier = "2";
			else
				tier = "1";

			// randomize except healers, unless chosen
			if (classtype != "H" | cbxEnemHealers.Checked)
			{
				List<int> possibleclasses = new List<int>();
				// random enemy classes
				if (cbxRandEnemy.Checked | cbxRandAllies.Checked)
				{
					// if 1:1 randomization, checks to see if this class has been changed in this chapter yet
					if (cbx1to1EnemyRand.Checked)
					{
						for (int x = 0; x < enemyoldclass.Count; x++)
						{
							if (editchar.JID == enemyoldclass[x])
							{
								possibleclasses.Add(enemynewclass[x]);
								break;
							}
						}
					}
					// if we found something from 1:1 rando, skip this part
					if (possibleclasses.Count == 0)
					{
						for (int x = 0; x < classes.Length; x++)
						{
							if (!classes[x].Tier_E.Contains(tier))
							{ } // incorrect tier
							else if (classes[x].Classtype_E == "H" & !cbxEnemHealers.Checked)
							{ } // can't turn into healer
							else if (((classes[x].Classtype_E == "L" & !(classtype == "L")) | (!(classes[x].Classtype_E == "L") & classtype == "L")) & cbxNoEnemyLaguz.Checked)
							{ } // laguz and beorc must stay the same type
							else if (classes[x].Classtype_E != classtype & cbxSimilarEnemy.Checked)
							{ } // must be similar classtype
							else if (classes[x].Name.Contains("dragon") & !chaptername.StartsWith("31") & !chaptername.StartsWith("4"))
							{ } // no dragons until end of part 3
							else if (classes[x].JID.Contains("SPIRIT") & !cbxSpirits.Checked)
							{ } // no spirits
							else if (!chaptername.StartsWith("407") & classes[x].JID.Contains("SPIRIT"))
							{ } // no spirits before tower
							else if (classes[x].Classtype_E != "F" & classtype == "F" & (chaptername == "107" | chaptername == "308" | chaptername == "403"))
							{ } // fliers in these chapters need to stay flying or script crashes
							else if (classes[x].Name.Contains("thief") & chaptername.StartsWith("10"))
							{ } // no early thieves
							else if (chaptername.StartsWith("10") & classes[x].Classtype_E == "H" & classtype != "H")
							{ } // no early healers (unless they already are)
							else
								possibleclasses.Add(x); // success
						}
					}
				}
				// t3 only
				else if (tier == "4")
				{
					// go one class up from the t2 variant of the SP class
					if (!JID.Contains("SPIRIT"))
					{
						string name = JID.Split('_')[1];            // JID _ SWORDMASTER _ SP
						for (int k = 0; k < classes.Length; k++)
							if (classes[k].JID.Contains(name))      // JID_SWORDMASTER
							{
								if (classes[k].JID == "JID_SAINT")  // saint doesnt have a promotion
									possibleclasses.Add(k);
								else
									possibleclasses.Add(k + 1);     // JID_SWORDESCHATOS
							}
					}
				}

				int newclass = -1;
				if (possibleclasses.Count > 0)
				{
					newclass = possibleclasses[random.Next(possibleclasses.Count)];
					if (cbx1to1EnemyRand.Checked)
					{
						enemyoldclass.Add(editchar.JID);
						enemynewclass.Add(newclass);
					}
					editchar.JID = classes[newclass].JID;
				}


				if (newclass != -1)
				{
					// modify transstate of new laguz
					if (classtype != "L" & classes[newclass].Classtype_E == "L")
					{
						// sets gauge to a random value
						editchar.TransState = random.Next(5, 31);
					}

					// modify ai
					string[] ai = editchar.AI;
					// non healer turned into healer
					if (classtype != "H" & classes[newclass].Classtype_E == "H" & (tier == "1" | tier == "2"))
						{
						ai[0] = "SEQ_ALLUNITROD100";
						ai[1] = "SEQ_NOMOVE";
					}
					// non laguz turned into laguz
					if (classtype != "L" & classes[newclass].Classtype_E == "L")
					{
						ai[0] = "SEQ_ATK100_BREAKMOVE_LAGUZ";
						ai[1] = "SEQ_NEARESTUNITMOVE";
					}
					// healer/laguz turned into non healer/laguz
					if ((classtype == "H" | classtype == "L") & classes[newclass].Classtype_E != "L" & classes[newclass].Classtype_E != "H")
					{
						ai[0] = "SEQ_ALLUNITATTACK100";
						ai[1] = "SEQ_NEARESTUNITMOVE";
					}
					editchar.AI = ai;
				}
			}
			return (editchar);
		}

		// determines random boss class based on user parameters
		private ChapterChar chooseBossClass(ChapterChar editchar, string chaptername)
		{
			string JID = editchar.JID;
			string classtype, tier;
			string skills = String.Join(",", ClassData.ReadStringArray(JID, "Skills"));
			string types = String.Join(",", ClassData.ReadStringArray(JID, "Class_Types"));

			if (types.Contains("SFXC_ALIZE"))
				classtype = "L"; // laguz
			else if (types.Contains("SFXC_ARMOR"))
				classtype = "A"; // armor
			else if (skills.Contains("SID_FLY"))
				classtype = "F"; // flier
			else if (types.Contains("SFXC_KNIGHT"))
				classtype = "C"; // cavalry
			else if (JID == "JID_PRIEST" | JID == "JID_BISHOP" | JID == "JID_BISHOP_SP" | JID == "JID_SAINT_SP" | JID == "JID_CLERIC" | JID == "JID_VALKYRIA")
				classtype = "H"; // healer
			else if (types.Contains("SFXC_MAGE") | JID.Contains("SPIRIT"))
				classtype = "M"; // magic
			else
				classtype = "I"; // infantry

			if (skills.Contains("SID_HIGHEST"))
			{
				if (cbxTier3Enemies.Checked | chaptername.StartsWith("406") | chaptername.StartsWith("407")) // bosses of 4_6 and tower are t3, not _SP classes
					tier = "4";
				else
					tier = "3";
			}
			else if (skills.Contains("SID_HIGHER") | (types.Contains("SFXC_ALIZE") & !chaptername.StartsWith("1"))) // laguz are considered t1 for part 1 enemies
				tier = "2";
			else
				tier = "1";

			List<int> possibleclasses = new List<int>();
			// random enemy classes
			if (cbxRandBosses.Checked)
			{
				if (editchar.PID == "PID_JELD" & jarodclass != -1)
					possibleclasses.Add(jarodclass);
				else
				{
					for (int x = 0; x < classes.Length; x++)
					{
						if (!classes[x].Tier_E.Contains(tier))
						{ } // incorrect tier
						else if (((classes[x].Classtype_E == "L" & !(classtype == "L")) | (!(classes[x].Classtype_E == "L") & classtype == "L")) & cbxNoEnemyLaguz.Checked)
						{ } // laguz and beorc must stay the same type
						else if (classes[x].Name.Contains("dragon") & !chaptername.StartsWith("31") & !chaptername.StartsWith("4"))
						{ } // no dragons until end of part 3
						else if (classes[x].JID.Contains("SPIRIT") & !cbxSpirits.Checked)
						{ } // no spirits
						else if (!chaptername.StartsWith("407") & classes[x].JID.Contains("SPIRIT"))
						{ } // no spirits before tower
						else if (classes[x].Name.Contains("thief") & chaptername.StartsWith("10"))
						{ } // no early thieves
						else
							possibleclasses.Add(x); // success
					}
				}
			}
			// rand recruit including bosses
			else if (cbxRandRecr.Checked & cbxEnemyRecruit.Checked)
			{
				for (int x = 0; x < characters.Length; x++)
				{
					if (characters[x].PID == editchar.PID)
					{
						if (characters[x].NewClass != -1)
							possibleclasses.Add(characters[x].NewClass);
						break;
					}
				}
			}
			// t3 only
			else if (tier == "4")
			{
				// go one class up from the t2 variant of the SP class
				string name = JID.Split('_')[1];            // JID _ SWORDMASTER _ SP
				for (int k = 0; k < classes.Length; k++)
					if (classes[k].JID.Contains(name))      // JID_SWORDMASTER
					{
						if (classes[k].JID == "JID_SAINT")  // saint doesnt have a promotion
							possibleclasses.Add(k);
						else
							possibleclasses.Add(k + 1);     // JID_SWORDESCHATOS
					}
			}

			int newclass = -1;
			if (possibleclasses.Count > 0)
			{
				newclass = possibleclasses[random.Next(possibleclasses.Count)];
				editchar.JID = classes[newclass].JID;
				if (editchar.PID == "PID_JELD")
					jarodclass = newclass;

			}

			// modify ai
			if (newclass != -1)
			{
				string[] ai = editchar.AI;
				// non healer turned into healer
				if (classtype != "H" & classes[newclass].Classtype_E == "H" & (tier == "1" | tier == "2"))
				{
					ai[0] = "SEQ_ALLUNITROD100";
				}
				// turned into laguz
				if (classtype != "L" & classes[newclass].Classtype_E == "L")
				{
					ai[0] = "SEQ_ATK100_BREAKMOVE_LAGUZ";
				}
				// healer/laguz turned into non healer/laguz
				if ((classtype == "H" | classtype == "L") & classes[newclass].Classtype_E != "L" & classes[newclass].Classtype_E != "H")
				{
					ai[0] = "SEQ_ALLUNITATTACK100";
				}
				editchar.AI = ai;
			}

			return (editchar);
		}

		// chooses weapons for enemy based on class, what chapter they're on, and if they're a boss
		private string[] chooseEnemyWeapons(ChapterChar editchar, string chaptername, bool isboss)
		{
			string[] weapons = editchar.Weapons;
			for (int i = 0; i < classes.Length; i++)
			{
				if (classes[i].JID == editchar.JID)
				{
					string[] weapontypes = classes[i].Weapon_E.Split(';');
					if (classes[i].Classtype_E == "H" & (!(cbxRandEnemy.Checked | cbxRandAllies.Checked) | !cbxEnemHealers.Checked) & !isboss)
					{ } // don't modify healers
					else
					{
						string partofgame;
						// set part of game
						#region
						if (chaptername == "101" | chaptername == "102" | chaptername == "103")
							partofgame = "1a";
						else if (chaptername.StartsWith("10"))
							partofgame = "1b";
						else if (chaptername.StartsWith("1"))
							partofgame = "1c";
						else if (chaptername == "201" | chaptername == "202")
							partofgame = "2a";
						else if (chaptername.StartsWith("2"))
							partofgame = "2b";
						else if (chaptername == "301" | chaptername == "302" | chaptername == "303" | chaptername == "304")
							partofgame = "3a";
						else if (chaptername.StartsWith("30"))
							partofgame = "3b";
						else if (chaptername.StartsWith("3"))
							partofgame = "3c";
						else if (chaptername == "401" | chaptername == "402" | chaptername == "403")
							partofgame = "4a";
						else if (chaptername == "404" | chaptername == "405" | chaptername == "406")
							partofgame = "4b";
						else
							partofgame = "T";
						#endregion

						for (int j = 0; j < weapons.Length; j++)
						{
							if (weapons[j] != "" | j == 0) // must have at least one weapon
							{
								bool droppable = weapons[j].Contains("/D");
								List<string> possibleweapons = new List<string>();

								StreamReader dataReader = new StreamReader(file + "\\assets\\enemyweaponlist.csv");
								string[] enemweapons = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
								dataReader.Close();
								for (int k = 1; k < enemweapons.Length; k++)
								{
									string[] split = enemweapons[k].Split(',');
									bool issiege = split[0] == "IID_METEOR" | split[0] == "IID_BLIZZARD" | split[0] == "IID_THUNDERSTORM" | split[0] == "IID_PURGE" | split[0] == "IID_FENRIR";
									if (weapontypes[j % 2] != split[1])
									{ } // different weapon type
									else if (editchar.PID == "PID_IZCA" | editchar.PID == "PID_LEKAIN" | editchar.PID == "PID_RUBALE")
									{
										// izuka, lekain, levail have SS weapon
										if (split[2] == "" & split[3] == "T")
											possibleweapons.Add(split[0]);
									}
									else if (cbxNoSiege.Checked & issiege)
									{ } // no siege
									else if (cbxOnlySiege.Checked & (weapontypes[j % 2] == "F" | weapontypes[j % 2] == "T" | weapontypes[j % 2] == "W" |
																	 weapontypes[j % 2] == "M" | weapontypes[j % 2] == "D"))
									{
										// only siege allowed
										if (issiege)
											possibleweapons.Add(split[0]);
									}
									else
									{
										// check if part of the game is correct for this weapon
										string checkpart;
										if (cbxWeapPatch.Checked & split[4] != "")
											checkpart = split[4];
										else
											checkpart = split[2];
										if (cbxEnemWeaps.Checked | isboss)
											checkpart += ";" + split[3];

										if (checkpart.Contains(partofgame))
											possibleweapons.Add(split[0]);
									}
								}

								if (possibleweapons.Count > 0)
								{
									weapons[j] = possibleweapons[random.Next(possibleweapons.Count)];
									if (droppable & !weapons[j].Contains("/D"))
										weapons[j] += "/D";
								}
								else
									weapons[j] = "IID_COIN";
							}
						}
					}
					break;
				}
			}
			return (weapons);
		}

		// randomizes droppable items for enemies
		private string[] chooseEnemyDrops(ChapterChar editchar, bool isboss)
		{
			StreamReader dataReader = new StreamReader(file + "\\assets\\dropshopitems.csv");
			string[] allitems = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			string[] items = editchar.Items;
			string enemyclass = editchar.JID;
			string enemyweaps = "";

			// get usable weapon types for enemy class
			for (int i = 0; i < classes.Length; i++)
			{
				if (classes[i].JID == enemyclass)
				{
					enemyweaps = classes[i].Weapon_E;
					break;
				}
			}

			for (int i = 0; i < items.Length; i++)
			{
				if (items[i].Contains("/D") & (cbxEnemDrops.Checked | (cbxEnemBonusDrop.Checked & !isboss) | (cbxBossBonusDrop.Checked & isboss)))
				{
					if (!items[i].Contains("TREASUREKEY"))
					{
						// random item, first column of file is IID
						items[i] = allitems[random.Next(allitems.Length)].Split(',')[0] + "/D";
					}
					break;
				}
				else if (items[i] == "" & ((cbxEnemBonusDrop.Checked & !isboss) | (cbxBossBonusDrop.Checked & isboss)))
				{
					int attempts = 0;
					while (attempts < 1000) // characters cannot be given a weapon that their class is able to use - screws up weapon ranks
					{
						int itemrand = random.Next(allitems.Length);
						string randitemtype = allitems[itemrand].Split(',')[1].ToUpper();
						if (!enemyweaps.ToUpper().Contains(randitemtype) & randitemtype != "X")
						{
							items[i] = allitems[random.Next(allitems.Length)].Split(',')[0] + "/D";
							break;
						}
						attempts += 1;
					}
					break;
				}
			}
			return (items);
		}

		// randomly chooses skills to add to all enemies
		private string[] chooseEnemySkills(ChapterChar editchar, bool isboss)
		{
			StreamReader dataReader = new StreamReader(file + "\\assets\\enemySkillList.csv");
			string[] allskills = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			List<string> possibleskills = new List<string>();
			string[] classtypes = ClassData.ReadStringArray(editchar.JID, "Class_Types");
			for (int i = 0; i < allskills.Length; i++)
			{
				//if (!String.Join(",", classtypes).Contains("SFXC_ALIZE") & (allskills[i].Split(',')[1] == "SID_HALFBEAST" | allskills[i].Split(',')[1] == "SID_KING"))
				//{ } // non-laguz can't have wildheart or formshift
				//else if (!cbxFormshift.Checked & allskills[i].Split(',')[1] == "SID_KING")
				//{ } // no formshift scroll
				//else
				possibleskills.Add(allskills[i].Split(',')[1]);
			}

			// number of skills to randomize
			int numskills;
			if (cbxEnemySkills.Checked)
				numskills = (int)numericEnemySkills.Value;
			else
				numskills = 0;
			if (cbxBossSkills.Checked & isboss)
				numskills += 1;

			string[] skills = editchar.Skills;
			for (int i = 0; i < numskills; i++)
			{
				int j = i;
				// find next blank skill
				while (skills[j] != "")
				{
					j++;
					if (j == skills.Length)
						break;
				}
				// if not finished, randomly choose skill
				if (j < skills.Length)
				{
					int randchoice = random.Next(possibleskills.Count);
					skills[j] = possibleskills[randchoice];
					// delete from list so there's no duplicates
					possibleskills.RemoveAt(randchoice);
				}
			}
			return (skills);
		}

		#endregion

		// *************************************************************************************** SAVING FUNCTIONS
		#region

		// saves modified classes, weapons, and items for player units to the dispos file
		private void SavePlayertoDispos()
		{
			for (int i = 0; i < characters.Length; i++)
			{
				int chapindex = -1;
				if (characters[i].Chapter != "0")
				{
					for (int k = 0; k < chapters.Length; k++)
					{
						if (characters[i].Chapter == chapters[k])
						{
							chapindex = k;
							break;
						}
					}
				}
				if (chapindex != -1)
				{
					// only continue if there's a new class to save
					if (characters[i].NewClass != -1)
					{
						int newclass = characters[i].NewClass;
						string PID = characters[i].PID;

						characters[i].NewRace = classes[newclass].Race;

						ChapterChar tempchar = ChapterData[chapindex].Read(PID);
						// save class name
						tempchar.JID = classes[newclass].JID;
						// save transformation status
						if (characters[i].NewRace == "B")
							tempchar.TransState = 0;
						// calculate new level, if necessary
						if (characters[i].Race != characters[i].NewRace)
						{
							// change level when switching races
							if (characters[i].NewRace == "L")
							{
								if (characters[i].Tier == "b")
									characters[i].Level += 10;
								else if (characters[i].Tier == "c" | characters[i].Tier == "d")
									characters[i].Level += 20;
								// sets trans gauge
								tempchar.TransState = 30;
							}
							else
							{
								if (characters[i].Tier == "b")
									characters[i].Level -= 10;
								else if (characters[i].Tier == "c" | characters[i].Tier == "d")
									characters[i].Level -= 20;
								// coerce to 18 due to game not allowing exp to units of 19? unsure why this occurs
								if (characters[i].Level > 18)
									characters[i].Level = 18;
							}
							if (characters[i].Level < 1)
								characters[i].Level = 1;
							if (characters[i].Level > 40)
								characters[i].Level = 40;
						}
						// save level to fe10data and dispos file
						CharacterData.Write(characters[i].PID, "Level", characters[i].Level);
						tempchar.Level = characters[i].Level;
						// weapons
						if (characters[i].Tier == "a")
							tempchar.Weapons = ChoosePlayerT1Weapons(newclass, characters[i].WeaponNum);
						else if (characters[i].Tier == "b")
							tempchar.Weapons = ChoosePlayerT2Weapons(newclass, characters[i].WeaponNum, characters[i].NewName);
						else
							tempchar.Weapons = ChoosePlayerT3Weapons(newclass, characters[i].WeaponNum, characters[i].NewName);

						// if druid, add mastercrown
						if (classes[newclass].Name == "druid" & cbxDruidCrown.Checked)
						{
							string[] tempitems = tempchar.Items;
							for (int x = 0; x < tempitems.Length; x++)
							{
								if (tempitems[x] == "" | x == tempitems.Length - 1)
								{
									tempitems[x] = "IID_MASTERCROWN";
									break;
								}
							}
							tempchar.Items = tempitems;
						}

						// fix lyre
						//if (characters[i].Name == "lyre")
						//	tempchar.Color = 3;
						// save back into dispos file
						ChapterData[chapindex].Write(tempchar);
					}

					// gives each player character an extra item
					if (cbxBonusItems.Checked)
					{
						StreamReader dataReader = new StreamReader(file + "\\assets\\dropshopitems.csv");
						string[] allitems = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
						dataReader.Close();

						string PID = characters[i].PID;
						string weaps;
						if (characters[i].NewClass != -1)
							weaps = classes[characters[i].NewClass].Weapon_P;
						else
							weaps = classes[characters[i].VanillaClass].Weapon_P;

						ChapterChar tempchar = ChapterData[chapindex].Read(PID);
						for (int x = 0; x < tempchar.Items.Length; x++)
						{
							if (tempchar.Items[x] == "")
							{
								int attempts = 0;
								while (attempts < 1000) // characters cannot be given a weapon that their class is able to use - screws up weapon ranks
								{
									int itemrand = random.Next(allitems.Length);
									if (!weaps.ToUpper().Contains(allitems[itemrand].Split(',')[1].ToUpper()))
									{
										tempchar.Items[x] = allitems[itemrand].Split(',')[0];
										break;
									}
									attempts += 1;
								}
								break;
							}
						}

						// save back into dispos file
						ChapterData[chapindex].Write(tempchar);

					}
				}
			}
		}

		// calls various save functions for various class randomization selections
		private void SaveFE10Data_ClassRand()
		{
			// swaps character data info for random recruitment
			if (cbxRandRecr.Checked | cbxChooseMic.Checked | cbxChooseIke.Checked | cbxChooseElincia.Checked)
				SwapRecruitInfo();
			// change laguz gauges for new classes
			if (classeschanged | cbxMicClass.Checked | cbxIkeClass.Checked | cbxElinciaClass.Checked | cbxGaugeRand.Checked)
				laguzModifications();
			// randomizes promotion order
			if (cbxRandPromotion.Checked)
				promotionSwapper();
			// changes character animation based on new classes / promotions
			if (classeschanged | cbxRandPromotion.Checked | cbxMicClass.Checked | cbxIkeClass.Checked | cbxElinciaClass.Checked)
				animationChanger();
			// everyone is Oliver
			if (cbxClassRand.Checked & comboClassOptions.SelectedIndex == 10)
				veryImportantFunction();
		}

		// swaps most information for characters around for rand recruitment
		private void SwapRecruitInfo()
		{
			// keep temp instance of all character's data
			List<string> MPID = new List<string>(), MNPID = new List<string>(), affinity = new List<string>();
			List<string> FID = new List<string>();
			List<string[]> animations = new List<string[]>();
			List<int> biorhythm = new List<int>(), authority = new List<int>();
			List<int[]> growths = new List<int[]>();

			for (int i = 0; i < characters.Length; i++)
			{
				MPID.Add(CharacterData.ReadString(characters[i].PID, "MPID"));
				MNPID.Add(CharacterData.ReadString(characters[i].PID, "MNPID"));
				FID.Add(CharacterData.ReadString(characters[i].PID, "FID"));
				affinity.Add(CharacterData.ReadString(characters[i].PID, "Affinity"));
				biorhythm.Add(CharacterData.ReadInt(characters[i].PID, "Biorhythm"));
				authority.Add(CharacterData.ReadInt(characters[i].PID, "Authority"));
				growths.Add(CharacterData.ReadIntArray(characters[i].PID, "Growths"));

				string[] temp = CharacterData.ReadStringArray(characters[i].PID, "Animations");
				// fill empty animations
				if (temp[2] == "")
					temp[2] = temp[1];
				if (temp[3] == "")
					temp[3] = temp[2];
				if (temp[1] == "")
					temp[1] = temp[2];
				if (temp[0] == "")
					temp[0] = temp[1];
				animations.Add(temp);
			}

			// save data from new character into each slot
			for (int i = 0; i < characters.Length; i++)
			{
				if (characters[i].NewRecr != -1)
				{
					CharacterData.Write(characters[i].PID, "MPID", MPID[characters[i].NewRecr]);
					CharacterData.Write(characters[i].PID, "MNPID", MNPID[characters[i].NewRecr]);
					CharacterData.Write(characters[i].PID, "Affinity", affinity[characters[i].NewRecr]);
					CharacterData.Write(characters[i].PID, "Biorhythm", biorhythm[characters[i].NewRecr]);
					CharacterData.Write(characters[i].PID, "Authority", authority[characters[i].NewRecr]);
					CharacterData.Write(characters[i].PID, "Growths", growths[characters[i].NewRecr]);
					if (!cbxRecrVanillaClass.Checked)
						CharacterData.Write(characters[i].PID, "Animations", animations[characters[i].NewRecr]);

					CharacterData.Write(characters[i].PID, "FID", FID[characters[i].NewRecr]);
				}
			}

			// swap bases around to fit a similar weight to the new character
			recruitBaseSwapping();
		}

		// swaps order of bases for characters
		private void recruitBaseSwapping()
		{
			string line;
			string[] values;

			int totalchars = 72;
			if (cbxEnemyRecruit.Checked)
			{
				if (cbxClassRand.Checked | cbxClassSwap.Checked | cbxRecrVanillaClass.Checked)
					totalchars = 86;
				else
					totalchars = 84;
			}

			int[,] charBaseOrder = new int[totalchars, 6];

			System.IO.StreamReader dataReader;
			dataReader = new System.IO.StreamReader(file + "\\assets\\CharBaseOrder.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all
			for (int i = 0; i < totalchars; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				// order of bases for each character
				for (int j = 2; j < values.Length; j++)
					charBaseOrder[i, j - 2] = Convert.ToInt32(values[j]);
			}
			dataReader.Close();

			for (int i = 0; i < totalchars; i++)
			{
				if (characters[i].NewRecr != -1)
				{
					int[] bases_vanilla = new int[6];
					int[] bases_swapped = new int[6];
					int[] order_vanilla = new int[6];
					int[] order_new = new int[6];
					// get orders
					for (int j = 0; j < 6; j++)
					{
						order_vanilla[j] = charBaseOrder[i, j];
						order_new[j] = charBaseOrder[characters[i].NewRecr, j];
					}

					// read in bases
					int[] tempint = CharacterData.ReadIntArray(characters[i].PID, "Bases");
					// skip hp and luck
					bases_vanilla[0] = tempint[1];
					bases_vanilla[1] = tempint[2];
					bases_vanilla[2] = tempint[3];
					bases_vanilla[3] = tempint[4];
					bases_vanilla[4] = tempint[6];
					bases_vanilla[5] = tempint[7];

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
					tempint[1] = bases_vanilla[0];
					tempint[2] = bases_vanilla[1];
					tempint[3] = bases_vanilla[2];
					tempint[4] = bases_vanilla[3];
					tempint[6] = bases_vanilla[4];
					tempint[7] = bases_vanilla[5];

					// write back to csv
					CharacterData.Write(characters[i].PID, "Bases", tempint);
				}
			}
		}

		// oliver mode - copy and paste his mpid, mnpid, and fid everywhere
		private void veryImportantFunction()
		{
			string oliverMPID = CharacterData.ReadString("PID_OLIVER", "MPID");
			string oliverMNPID = CharacterData.ReadString("PID_OLIVER", "MNPID");
			string oliverFID = CharacterData.ReadString("PID_OLIVER", "FID");

			int numberofunits;
			if (((cbxClassRand.Checked | cbxClassSwap.Checked) & cbxHerons.Checked) | cbxRandRecr.Checked)
				numberofunits = 72;
			else
				numberofunits = 69;

			// loop through characters
			for (int charNum = 0; charNum < numberofunits; charNum++)
			{
				// do the thing
				CharacterData.Write(characters[charNum].PID, "MPID", oliverMPID);
				CharacterData.Write(characters[charNum].PID, "MNPID", oliverMNPID);
				CharacterData.Write(characters[charNum].PID, "FID", oliverFID);
			}
		}

		// does various enemy/ally modifications and saves to dispos files
		private void SaveEnemiestoDispos()
		{
			textBox1.Text = "Making weird enemies";
			Application.DoEvents();

			for (int i = 0; i < chapters.Length; i++)
			{
				if (!chapters[i].Contains("emap"))
				{
					string chapterenemyoutput = "";
					// header for enemy outputlog
					randEnemyOutput += "<br>" + chapters[i];
					randEnemyOutput += htmlSpoilerButton(chapters[i]);
					randEnemyOutput += "<div id=\"" + chapters[i] + "\" style=\"display:none\">";

					randEnemyOutput += "<table><tr> <th>Name</th> <th>Class</th></tr>";

					enemyoldclass = new List<string>();
					enemynewclass = new List<int>();
					// read in all characters in the chapter file
					ChapterChar[] disposchars = ChapterData[i].ReadAll();
					for (int j = 0; j < disposchars.Length; j++)
					{
						if (disposchars[j].Color == 1 | disposchars[j].Color == 3) // red boi or yellow allies
						{
							// check if player character
							bool ispc = false;
							for (int x = 0; x < characters.Length; x++)
							{
								if (characters[x].Chapter == "0")
									break;
								if (characters[x].PID == disposchars[j].PID)
								{
									ispc = true;
									break;
								}
							}
							bool isboss = false;
							StreamReader dataReader = new StreamReader(file + "\\assets\\bosslist.csv");
							string[] bosspids = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
							dataReader.Close();
							// check if boss
							for (int k = 0; k < bosspids.Length; k++)
							{
								if (disposchars[j].PID == bosspids[k].Split(',')[0] & chapters[i] == bosspids[k].Split(',')[1])
								{
									isboss = true;
									break;
								}
							}
							if (!isboss)
							{
								bool generic = false;
								dataReader = new StreamReader(file + "\\assets\\enemyPIDlist.txt");
								string[] enemypids = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
								dataReader.Close();
								// check if generic enemy, not a playable character
								for (int k = 0; k < enemypids.Length; k++)
								{
									if (disposchars[j].PID == enemypids[k])
									{
										// don't randomize spirits if not selected
										if (!enemypids[k].Contains("SPIRIT") | cbxSpirits.Checked)
											generic = true;
										break;
									}
								}

								// generic, time to do modifications
								if (generic)
								{
									// first, randomize class and modify ai if necessary
									if ((cbxRandEnemy.Checked & disposchars[j].Color == 1) | (cbxRandAllies.Checked & disposchars[j].Color == 3) | (cbxTier3Enemies.Checked & chapters[i].StartsWith("4")))
										disposchars[j] = chooseEnemyClass(disposchars[j], chapters[i]);
									// change weapons
									if ((cbxRandEnemy.Checked & disposchars[j].Color == 1) | (cbxRandAllies.Checked & disposchars[j].Color == 3) | (cbxEnemWeaps.Checked & disposchars[j].Color == 1))
										disposchars[j].Weapons = chooseEnemyWeapons(disposchars[j], chapters[i], false);
									// dropped items
									if (cbxEnemDrops.Checked | cbxEnemBonusDrop.Checked)
										disposchars[j].Items = chooseEnemyDrops(disposchars[j], isboss);
									// skills
									if (cbxEnemySkills.Checked & disposchars[j].Color == 1)
										disposchars[j].Skills = chooseEnemySkills(disposchars[j], false);

									chapterenemyoutput += EnemyOutputLog(disposchars[j]);
								}
							}
							else
							{
								// change class and weapons
								if (cbxRandBosses.Checked | (cbxRandRecr.Checked & cbxEnemyRecruit.Checked) | (cbxTier3Enemies.Checked & chapters[i].StartsWith("4")))
									disposchars[j] = chooseBossClass(disposchars[j], chapters[i]);
								// change weapons
								if (cbxRandBosses.Checked | cbxEnemWeaps.Checked)
									disposchars[j].Weapons = chooseEnemyWeapons(disposchars[j], chapters[i], true);
								// dropped items
								if (cbxEnemDrops.Checked | cbxBossBonusDrop.Checked)
									disposchars[j].Items = chooseEnemyDrops(disposchars[j], isboss);
								// skills
								if (cbxEnemySkills.Checked | cbxBossSkills.Checked)
									disposchars[j].Skills = chooseEnemySkills(disposchars[j], true);

								chapterenemyoutput += EnemyOutputLog(disposchars[j]);
							}
						}
					}
					ChapterData[i].WriteAll(disposchars);

					// finish outputlog addition
					randEnemyOutput += chapterenemyoutput;
					randEnemyOutput += "</table></div>";
				}
			}
		}

		// add enemies to outputlog variable
		private string EnemyOutputLog(ChapterChar disposchar)
		{
			string outstring = "<tr><td>" + disposchar.PID + "</td>";
			outstring += "<td>" + disposchar.JID + "</td>";

			foreach (string weapon in disposchar.Weapons)
			{
				if (weapon != "")
					outstring += "<td>" + weapon + "</td>";
			}

			foreach (string item in disposchar.Items)
			{
				if (item != "")
					outstring += "<td>" + item + "</td>";
			}

			foreach (string skill in disposchar.Skills)
			{
				if (skill != "")
					outstring += "<td>" + skill + "</td>";
			}

			outstring += "</tr>";
			return (outstring);
		}

		// various fe10 data modifications for enemies
		private void SaveFE10Data_Enemies()
		{
			StreamReader dataReader = new StreamReader(file + "\\assets\\enemyPIDlist.txt");
			string[] enemypids = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			for (int i = 0; i < enemypids.Length; i++)
			{
				// growth additions
				if (cbxEnemyGrowth.Checked)
				{
					int[] growths = CharacterData.ReadIntArray(enemypids[i], "Growths");
					for (int j = 0; j < growths.Length; j++)
					{
						// laguz have lowered growth increases, because their stats get crazy
						if (enemypids[i].Contains("GALIA") | enemypids[i].Contains("GOLDOA") | enemypids[i].Contains("_DUD_"))
							growths[j] += ((int)numericEnemyGrowth.Value / 4);
						else
							growths[j] += (int)numericEnemyGrowth.Value;
						if (growths[j] > 255)
							growths[j] = 255;
					}
					CharacterData.Write(enemypids[i], "Growths", growths);
				}

				// deletes animiation pointers for enemy PIDs so they use default animations
				if (cbxRandEnemy.Checked | cbxRandAllies.Checked | cbxTier3Enemies.Checked)
				{
					CharacterData.Write(enemypids[i], "Animations", new string[4] { "", "", "", "" });
				}

				// laguz changes
				if (cbxRandEnemy.Checked | cbxRandAllies.Checked)
				{
					// sets laguz gauge to that of a tiger
					if (enemypids[i].Contains("GALIA") | enemypids[i].Contains("GOLDOA") | enemypids[i].Contains("_DUD_"))
					{ } // these already have trans gauge levels
					else
						CharacterData.Write(enemypids[i], "Laguz_Gauge", new int[4] { 8, 15, 252, 253 });

					// sets laguz weapon rank to S for part 4 enemies
					if (enemypids[i].Contains("APOSTLE") | enemypids[i].Contains("SPIRIT"))
					{
						char[] weaprank = CharacterData.ReadString(enemypids[i], "Weapon_Ranks").ToArray<char>();
						weaprank[5] = 'S';
						CharacterData.Write(enemypids[i], "Weapon_Ranks", String.Join("", weaprank));
					}
				}

				// weapon ranks
				if (cbxRandEnemy.Checked | cbxRandAllies.Checked | cbxEnemWeaps.Checked)
				{
					if (enemypids[i].Contains("APOSTLE") | enemypids[i].Contains("SPIRIT"))
					{
						char[] weaprank = CharacterData.ReadString(enemypids[i], "Weapon_Ranks").ToArray<char>();
						for (int x = 0; x < weaprank.Length; x++)
						{
							if (x != 5)
								weaprank[x] = '*';
						}
						CharacterData.Write(enemypids[i], "Weapon_Ranks", String.Join("", weaprank));
					}
				}


			}


			dataReader = new StreamReader(file + "\\assets\\bosslist.csv");
			string[] bosspids = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			for (int i = 0; i < bosspids.Length; i++)
			{
				string PID = bosspids[i].Split(',')[0];
				// boss stat additions
				if (cbxBuffBosses.Checked)
				{
					// split 35 bosses into five parts
					int part = i / 7;
					// split given value into five parts
					int addition = (part + 1) * (int)Math.Round((float)numericBossStats.Value / 5.0);
					if (bosspids[i].Contains("PID_CEPHERAN") | bosspids[i].Contains("PID_JELD,111"))
					{ } // don't change stats of sephiram or jarod for the second time
					else
					{
						int[] bases = CharacterData.ReadIntArray(PID, "Bases");
						for (int j = 0; j < bases.Length; j++)
						{
							bases[j] += addition;
							if (bases[j] > 255)
								bases[j] -= 256;
						}
						CharacterData.Write(PID, "Bases", bases);
					}
				}

				// deletes animiation pointers for boss PIDs so they use default animations, unless bosses are random recruitment
				if (cbxRandBosses.Checked)
				{
					CharacterData.Write(PID, "Animations", new string[4] { "", "", "", "" });
					string[] anims = CharacterData.ReadStringArray(PID, "Animations");
				}
				else if (cbxRandRecr.Checked & cbxEnemyRecruit.Checked)
				{
					// zero out only for bosses not on the random recruitment list
					for (int j = 0; j < characters.Length; j++)
					{
						if (characters[j].PID == PID & characters[j].NewRecr != -1)
							break;
						if (j == characters.Length - 1)
							CharacterData.Write(PID, "Animations", new string[4] { "", "", "", "" });
					}
				}
			}
		}

		// modifies dispos and data files based on user selections
		private void DisposModifications()
		{
			textBox1.Text = "Modifying Chapter Files";
			Application.DoEvents();

			for (int i = 0; i < chapters.Length; i++)
			{
				// if weapon patch, but enemy weapons are unchanged, need to remove bronze weapons from early game
				if (chapters[i].StartsWith("1") & cbxWeapPatch.Checked & !(cbxEnemWeaps.Checked | cbxRandEnemy.Checked))
				{
					ChapterChar[] allchars = ChapterData[i].ReadAll();
					for (int j = 0; j < allchars.Length; j++)
					{
						if (allchars[j].Color == 1) // red
						{
							string[] weapons = allchars[j].Weapons;
							for (int x = 0; x < weapons.Length; x++)
							{
								if (weapons[x].Contains("BRONZE"))
									weapons[x] = weapons[x].Replace("BRONZE", "IRON");
							}
							allchars[j].Weapons = weapons;
						}
					}
					ChapterData[i].WriteAll(allchars);
				}


				if (chapters[i] == "106")
				{
					// save jill
					if (cbxJillAI.Checked)
					{
						ChapterChar[] allchars = ChapterData[i].ReadAll();
						for (int j = 0; j < allchars.Length; j++)
						{
							if (allchars[j].Color == 1) // red
							{
								string[] ai = allchars[j].AI;
								if ((ai[0] == "SEQ_ALLUNITATTACK100" | ai[0] == "SEQ_ALLATK100_NOJILL") & allchars[j].Location[1] < 5)
									ai[0] = "SEQ_NOATTACK";//SEQ_ALLATK100_NOJILL";
								if (ai[1] == "SEQ_NEARESTUNITMOVE")
									ai[1] = "SEQ_ATTACKRANGEMOVE";//SEQ_ATTACKRANGEMOVE";
								allchars[j].AI = ai;
							}
						}
						ChapterData[i].WriteAll(allchars);
					}

					// volug does not gain halfshift if he is beorc
					if (classeschanged)
					{
						if (characters[9].NewRace == "B")
						{
							ChapterChar volug = ChapterData[i].Read(characters[9].PID);
							string[] skills = volug.Skills;
							for (int j = 0; j < skills.Length; j++)
							{
								// blank out halfbeast
								skills[j] = "";
							}
							volug.Skills = skills;
							ChapterData[i].Write(volug);
						}
					}
				}
				else if (chapters[i] == "107")
				{
					// save fiona
					if (cbxFionaAI.Checked)
					{
						ChapterChar[] allchars = ChapterData[i].ReadAll();
						for (int j = 0; j < allchars.Length; j++)
						{
							if (allchars[j].Color == 1) // red
							{
								string[] ai = allchars[j].AI;
								if (ai[0] == "SEQ_ATK100_MARAD_2" | ai[0] == "SEQ_ALLUNITATTACK100")
									ai[0] = "SEQ_ATK100_EXCEPTFOR_MARAD";
								if (ai[1] == "SEQ_NEARESTUNITMOVE")
									ai[1] = "SEQ_NEARESTUNITMOVE_EXCEPTFOR_MARAD";
								allchars[j].AI = ai;
							}
						}
						ChapterData[i].WriteAll(allchars);
					}
				}
				else if (chapters[i] == "108")
				{
					// turn tormod to t3 if classes/recruitment isn't randomized
					if (cbxTormodT3.Checked & !classeschanged)
					{
						ChapterChar tormod = ChapterData[i].Read("PID_TOPUCK");
						tormod.JID = "JID_ARCHSAGE_F";
						ChapterData[i].Write(tormod);
					}
				}
				else if (chapters[i] == "202")
				{
					// heather starts as blue unit
					if (cbxHeatherBlue.Checked)
					{
						ChapterChar heather = ChapterData[i].Read("PID_HEATHER");
						heather.Color = 0;
						ChapterData[i].WriteColor(heather);
					}
				}
				else if (chapters[i] == "301")
				{
					// ike gets nihil
					if (cbxNihil.Checked)
					{
						ChapterChar ike = ChapterData[i].Read("PID_IKE");
						string[] skills = ike.Skills;
						for (int j = 0; j < skills.Length; j++)
						{
							// add nihil
							if (skills[j] == "")
							{
								skills[j] = "SID_NIHIL";
								break;
							}
						}
						ike.Skills = skills;
						ChapterData[i].Write(ike);
					}
					// mist gets holy crown
					if (cbxMistCrown.Checked)
					{
						ChapterChar mist = ChapterData[i].Read("PID_MIST");
						string[] items = mist.Items;
						for (int j = 0; j < items.Length; j++)
						{
							// add holy crown
							if (items[j] == "" | j == items.Length - 1)
							{
								items[j] = "IID_HOLYCROWN";
								break;
							}
						}
						mist.Items = items;
						ChapterData[i].Write(mist);
					}
				}
				else if (chapters[i] == "311")
				{
					// move elincia out of range of enemy bowmen if random enemies
					if (cbxRandEnemy.Checked)
					{
						ChapterChar elincia = ChapterData[i].Read("PID_ERINCIA");
						int[] location = elincia.Location;
						location[0] = 15;
						elincia.Location = location;
						elincia.CutsceneLoc = location;
						ChapterData[i].Write(elincia);
					}
				}
				else if (chapters[i] == "emap407d")
				{
					// kurth and ena no longer required for tower
					if (cbxKurthEna.Checked)
					{
						ChapterChar kurth = ChapterData[i].Read("PID_KURTHNAGA");
						ChapterChar ena = ChapterData[i].Read("PID_ENA");
						// set first bitflags to zero
						string[] split = kurth.FullInfo.Split(',');
						split[1] = "0";
						kurth.FullInfo = String.Join(",", split);
						split = ena.FullInfo.Split(',');
						split[1] = "0";
						ena.FullInfo = String.Join(",", split);
						ChapterData[i].Write(kurth);
						ChapterData[i].Write(ena);
					}

					// choose random tower units
					if (cbxTowerUnits.Checked)
					{
						ChapterChar[] allchars = ChapterData[i].ReadAll();
						List<string> PIDs = new List<string>();
						List<string> names = new List<string>();
						List<int> numbers = new List<int>();

						// get list of possible choices
						for (int j = 0; j < characters.Length; j++)
						{
							if (characters[j].Chapter == "0")
							{ } // can't include bosses, obvi
							else if (!cbxKurthEna.Checked & (characters[j].Name == "kurthnaga" | characters[j].Name == "ena"))
							{ } // these two are already required
							else if (characters[j].Name == "ike" | characters[j].Name == "micaiah" | characters[j].Name == "sothe" | characters[j].Name == "sanaki")
							{ } // already required
							else if (characters[j].Name == "gareth" | characters[j].Name == "nasir" | characters[j].Name == "lehran")
							{ } // dont have these characters yet
							else if (classeschanged)
							{
								// dont include herons
								if (classes[characters[j].NewClass].Name.Contains("heron"))
								{ }
								else
								{
									PIDs.Add(characters[j].PID);
									names.Add(characters[j].Name);
									numbers.Add(j);
								}
							}
							else if (!(classeschanged))
							{
								// dont include herons
								if (classes[characters[j].VanillaClass].Name.Contains("heron"))
								{ }
								else
								{
									PIDs.Add(characters[j].PID);
									names.Add(characters[j].Name);
									numbers.Add(j);
								}
							}
						}

						int numsaved = 0;
						// write choices
						for (int j = 0; j < allchars.Length; j++)
						{
							string[] split = allchars[j].FullInfo.Split(',');
							if (split[1] == "0")
							{
								// set required status
								split[1] = "24";
								allchars[j].FullInfo = String.Join(",", split);
								// choose random and save
								int randchoice = random.Next(PIDs.Count);
								allchars[j].PID = PIDs[randchoice];
								towerUnits[numsaved] = numbers[randchoice];
								numsaved++;
								// remove from lists
								PIDs.RemoveAt(randchoice);
								names.RemoveAt(randchoice);
								numbers.RemoveAt(randchoice);
							}
						}

						ChapterData[i].WriteAll(allchars);
					}
				}
				else if (chapters[i] == "407b")
				{
					// black knight nerf
					if (cbxBKnerf.Checked)
					{
						ChapterChar blackknight = ChapterData[i].Read("PID_DARKKNIGHT");
						string[] skills = blackknight.Skills;
						for (int j = 0; j < skills.Length; j++)
						{
							// blank out imbue
							if (skills[j].Contains("SID_CURE"))
								skills[j] = "";
							// move skills so blank is last
							if (skills[j] == "" & j < skills.Length - 1)
							{
								if (skills[j + 1] != "")
								{
									skills[j] = skills[j + 1];
									skills[j + 1] = "";
								}
							}
						}
						blackknight.Skills = skills;
						ChapterData[i].Write(blackknight);
					}

					// let someone else fight BK
					if (cbxBKfight.Checked)
					{
						ChapterChar sothe = ChapterData[i].Read("PID_SOTHE");
						int[] location = sothe.Location;
						location[1] = 9;
						sothe.Location = location;
						sothe.CutsceneLoc = location;
						ChapterData[i].Write(sothe);
					}
				}


			}
		}


		// various modifications of FE10Data, scripts, text, and main.dol due to user selections
		private void FEDataModifications()
		{
			textBox1.Text = "Modifying FE10Data File";
			Application.DoEvents();

			// if enemy classes are randomized, add growth rates to Light Mage to make her class scale with level
			if (cbxRandEnemy.Checked | cbxRandAllies.Checked | cbxRandBosses.Checked)
			{
				int[] miccgrowths = new int[8] { 40, 15, 60, 40, 35, 60, 20, 70 };
				ClassData.Write("JID_LIGHTMAGE", "Growths", miccgrowths);
				ClassData.Write("JID_LIGHTSAGE", "Growths", miccgrowths);
				ClassData.Write("JID_SHAMAN", "Growths", miccgrowths);
			}

			// change ranged type from sword to white breath
			if (cbxFlorete.Checked)
				ItemData.Write("IID_FLORETE", "Damage_Type", "neutral");

			// ragnell, florete, amiti, cymbeline, ettard modifications
			if (cbxGMweaps.Checked)
			{
				string[] gmweaps = new string[5] { "IID_RAGNELL", "IID_AMITE", "IID_FLORETE", "IID_CYMBELINE", "IID_ALONDITE" };
				string[] ranks = new string[5] { "*", "S", "A", "*", "S" };

				for (int i = 0; i < gmweaps.Length; i++)
				{
					// remove eq and valuable attributes, so anyone can use and they can be sold (only florete, ettard, and cymbeline can be sold)
					List<string> newattributes = new List<string>();
					string[] attributes = ItemData.ReadStringArray(gmweaps[i], "Attributes");
					for (int j = 0; j < attributes.Length; j++)
					{
						if (attributes[j].StartsWith("eq"))
						{ } // don't add eq 
						else if (i >= 2 & attributes[j] == "valuable")
						{ }
						else
							newattributes.Add(attributes[j]);
					}
					ItemData.Write(gmweaps[i], "Attributes", newattributes.ToArray());

					// new weapon ranks
					ItemData.Write(gmweaps[i], "Rank", ranks[i]);
				}

				// give elincia S rank swords so she can use new amiti
				char[] elinciaranks = CharacterData.ReadString("PID_ERINCIA", "Weapon_Ranks").ToArray<char>();
				elinciaranks[0] = 'S';
				CharacterData.Write("PID_ERINCIA", "Weapon_Ranks", String.Join("", elinciaranks));
			}

			// caladbolg, tarvos, lughnasadh, thani modifications
			if (cbxDBweaps.Checked)
			{
				string[] dbweaps = new string[4] { "IID_CALADBORG", "IID_TARVOS", "IID_LUGHNASAD", "IID_THANY" };
				string[] ranks = new string[4] { "B", "B", "B", "D" };

				for (int i = 0; i < dbweaps.Length; i++)
				{
					// remove eq and valuable attributes, so anyone can use and they can be sold
					List<string> newattributes = new List<string>();
					string[] attributes = ItemData.ReadStringArray(dbweaps[i], "Attributes");
					for (int j = 0; j < attributes.Length; j++)
					{
						if (attributes[j].StartsWith("eq"))
						{ } // don't add eq 
						else if (attributes[j] == "valuable")
						{ } // remove valuable
						else
							newattributes.Add(attributes[j]);
					}
					ItemData.Write(dbweaps[i], "Attributes", newattributes.ToArray());

					// new weapon ranks
					ItemData.Write(dbweaps[i], "Rank", ranks[i]);
				}
			}

			// buff vika and maurim's stats
			if (cbxTormodT3.Checked)
			{
				int[] basestats = CharacterData.ReadIntArray("PID_VIZE", "Bases");
				for (int i = 0; i < basestats.Length; i++)
				{
					basestats[i] += 2;
					if (basestats[i] > 255)
						basestats[i] = 255;
				}
				CharacterData.Write("PID_VIZE", "Bases", basestats);

				basestats = CharacterData.ReadIntArray("PID_MWARIM", "Bases");
				for (int i = 0; i < basestats.Length; i++)
				{
					basestats[i] += 2;
					if (basestats[i] > 255)
						basestats[i] = 255;
				}
				CharacterData.Write("PID_MWARIM", "Bases", basestats);
			}

			// change stats for oliver-only mode
			if (cbxClassRand.Checked & comboClassOptions.SelectedIndex == 10)
			{
				// priest base defense to 5
				int[] basestats = ClassData.ReadIntArray("JID_PRIEST", "Bases");
				basestats[6] = 5;
				ClassData.Write("JID_PRIEST", "Bases", basestats);

				// light mage defense to 4
				basestats = ClassData.ReadIntArray("JID_LIGHTMAGE", "Bases");
				basestats[6] = 4;
				ClassData.Write("JID_LIGHTMAGE", "Bases", basestats);

				// bishop base defense to 11
				basestats = ClassData.ReadIntArray("JID_BISHOP", "Bases");
				basestats[6] = 11;
				ClassData.Write("JID_BISHOP", "Bases", basestats);

				// light sage base defense to 12
				basestats = ClassData.ReadIntArray("JID_LIGHTSAGE", "Bases");
				basestats[6] = 12;
				ClassData.Write("JID_LIGHTSAGE", "Bases", basestats);
			}

			// give druid and summoner fire ranks
			if (cbxFireMag.Checked)
			{
				ClassData.Write("JID_DRUID", "Base_WeaponRank", "------D---C-");
				ClassData.Write("JID_DRUID", "Max_WeaponRank", "------A---S-");
				ClassData.Write("JID_DRUID_SP", "Base_WeaponRank", "------B---A-");
				ClassData.Write("JID_DRUID_SP", "Max_WeaponRank", "------S---*-");
				ClassData.Write("JID_SUMMONER", "Base_WeaponRank", "------B---SS");
				ClassData.Write("JID_SUMMONER", "Max_WeaponRank", "------S---*S");
			}

			// give lightsage and priestess dark ranks
			if (cbxDarkMag.Checked)
			{
				ClassData.Write("JID_LIGHTSAGE", "Base_WeaponRank", "---------CCC");
				ClassData.Write("JID_LIGHTSAGE", "Max_WeaponRank", "---------SAA");
				ClassData.Write("JID_SHAMAN", "Base_WeaponRank", "---------ABB");
				ClassData.Write("JID_SHAMAN", "Max_WeaponRank", "---------*S*");
			}

			// give chest key 5 uses
			if (cbxChestKey.Checked)
				ItemData.Write("IID_TREASUREKEY", "Uses", 5);

			// set all classes' FOW vision to max
			if (cbxNoFOW.Checked)
			{
				// read from file
				System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\JIDlist.txt");
				string[] JIDs = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				dataReader.Close();

				for (int i = 0; i < JIDs.Length; i++)
				{
					ClassData.Write(JIDs[i], "FOW", 127);
				}
			}
			// increase bird FOW vision
			else if (cbxBirdVision.Checked)
			{
				string[] birds = new string[10] {"JID_HAWK","JID_BIRDTRIBE_H","JID_CROW","JID_BIRDTRIBE_C","JID_CROW_F",
												"JID_BIRDTRIBE_C/F","JID_HAWK_TI","JID_KINGHAWK","JID_CROW_NA","JID_KINGCROW" };

				for (int i = 0; i < birds.Length; i++)
				{
					ClassData.Write(birds[i], "FOW", 2);
				}
			}

			// sellable promotion items/satori sign
			if (cbxSellableItems.Checked)
			{
				string[] promoitems = new string[4] { "IID_MASTERCROWN", "IID_HOLYCROWN", "IID_MASTERPROOF", "IID_SATORISIGN" };

				for (int i = 0; i < promoitems.Length; i++)
				{
					// remove valuable attributes so they can be sold
					List<string> newattributes = new List<string>();
					string[] attributes = ItemData.ReadStringArray(promoitems[i], "Attributes");
					for (int j = 0; j < attributes.Length; j++)
					{
						if (attributes[j] == "valuable")
						{ } // remove valuable
						else
							newattributes.Add(attributes[j]);
					}
					ItemData.Write(promoitems[i], "Attributes", newattributes.ToArray());

					// change price from 30k to 10k for master proof, holy crown, and satori sign
					if (i > 0)
						ItemData.Write(promoitems[i], "Cost/Use", 10000);
				}
			}

			// changes laguz gem to 3k per use (total 15k)
			if (cbxLowerPrice.Checked)
				ItemData.Write("IID_CHANGEGEM", "Cost/Use", 3000);

			// gives whisper classes lethality instead of bane
			if (cbxLethality.Checked)
			{
				string[] whispers = new string[2] { "JID_ESPION", "JID_ESPION_F" };

				for (int i = 0; i < whispers.Length; i++)
				{
					string[] skills = ClassData.ReadStringArray(whispers[i], "Skills");
					for (int j = 0; j < skills.Length; j++)
					{
						// this may seem backwards, but the japanese names for these skills are swapped
						if (skills[j] == "SID_LETHALITY")
						{
							skills[j] = "SID_BANE";
							break;
						}
					}
					ClassData.Write(whispers[i], "Skills", skills);
				}
			}

			// change whitegem to be 30k when sold
			if (cbxWhiteGem.Checked)// & cbxEventItems.Checked)
				ItemData.Write("IID_WHITEGEM", "Cost/Use", 60000);

			// random stat boosters
			if (cbxStatBooster.Checked)
			{
				string[] statboosters = new string[8] { "IID_ANGELROBE", "IID_ENERGYDROP", "IID_SPIRITPOWDER", "IID_SECRETBOOK",
														"IID_SPEEDWING", "IID_GODDESSICON", "IID_DRAGONSHIELD", "IID_TALISMAN" };
				// skip if both values are zero
				if (numStatBoostMax.Value != 0 | numStatBoostMin.Value != 0)
				{
					for (int i = 0; i < statboosters.Length; i++)
					{
						int[] statchanges = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
						if (cbxStatBoostMult.Checked)
						{
							// choose number of random stats
							int[] statstats = new int[6] { 1, 1, 1, 2, 2, 3 };
							int numstats = statstats[random.Next(statstats.Length)];
							// set stat changes
							for (int j = 0; j < numstats; j++)
							{
								int tries = 0;
								int statchoice = -1;
								while (tries < 100)
								{
									// choose a random stat
									statchoice = random.Next(8);
									if (statchanges[statchoice] == 0)
										break;
									else
										statchoice = -1;
								}

								// choose a stat change
								if (statchoice != -1)
								{
									int newstatchange = 0;
									while (newstatchange == 0)
										newstatchange = random.Next((int)numStatBoostMin.Value, (int)numStatBoostMax.Value + 1);
									statchanges[statchoice] = newstatchange;
								}
							}
						}
						else
						{
							// choose a random stat
							int statchoice = random.Next(8);
							// choose a stat change
							int newstatchange = 0;
							while (newstatchange == 0)
								newstatchange = random.Next((int)numStatBoostMin.Value, (int)numStatBoostMax.Value + 1);
							statchanges[statchoice] = newstatchange;
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

						// write
						ItemData.Write(statboosters[i], "Bonuses", statchanges);
					}
				}
			}

			// give beast laguz canto skill
			if (cbxLaguzCanto.Checked)
			{
				string[] beasts = new string[8] { "JID_LION", "JID_LION_GI", "JID_LION_CA", "JID_TIGER", "JID_CAT", "JID_CAT_F", "JID_WOLF", "JID_WOLF_F" };
				string[] untrans = new string[8] { "JID_BEASTTRIBE_L", "JID_KINGLION_GI", "JID_KINGLION", "JID_BEASTTRIBE_T",
													"JID_BEASTTRIBE_C", "JID_BEASTTRIBE_C/F", "JID_BEASTTRIBE_W", "JID_QUEENWOLF" };

				// add skill
				for (int i = 0; i < beasts.Length; i++)
				{
					string[] skills = ClassData.ReadStringArray(beasts[i], "Skills");
					string[] newskills = new string[skills.Length + 1];
					newskills[0] = "SID_CANTO";
					for (int j = 0; j < skills.Length; j++)
						newskills[j + 1] = skills[j];
					ClassData.Write(beasts[i], "Skills", newskills);

					// raise skill capacity by 10 to make up for canto costing 10
					int capacity = ClassData.ReadInt(beasts[i], "Capacity");
					ClassData.Write(beasts[i], "Capacity", capacity + 10);
					// also raise for untransformed class
					capacity = ClassData.ReadInt(untrans[i], "Capacity");
					ClassData.Write(untrans[i], "Capacity", capacity + 10);
				}
			}

			// give dragon laguz canto skill
			if (cbxDragonCanto.Checked)
			{
				string[] dragons = new string[5] { "JID_REDDRAGON", "JID_REDDRAGON_F", "JID_WHITEDRAGON", "JID_BLACKDRAGON", "JID_BLACKDRAGON_KU" };
				string[] untrans = new string[5] { "JID_DRAGONTRIBE_R", "JID_DRAGONTRIBE_R/F", "JID_DRAGONTRIBE_W", "JID_DRAGONKING", "JID_DRAGONPRINCE" };

				// add skill
				for (int i = 0; i < dragons.Length; i++)
				{
					string[] skills = ClassData.ReadStringArray(dragons[i], "Skills");
					string[] newskills = new string[skills.Length + 1];
					newskills[0] = "SID_CANTO";
					for (int j = 0; j < skills.Length; j++)
						newskills[j + 1] = skills[j];
					ClassData.Write(dragons[i], "Skills", newskills);

					// raise skill capacity by 10 to make up for canto costing 10
					int capacity = ClassData.ReadInt(dragons[i], "Capacity");
					ClassData.Write(dragons[i], "Capacity", capacity + 10);
					// also raise for untransformed class
					capacity = ClassData.ReadInt(untrans[i], "Capacity");
					ClassData.Write(untrans[i], "Capacity", capacity + 10);
				}
			}

			// give cymbeline a cost
			if (cbxRandShop.Checked | cbxBargains.Checked | cbxForge.Checked)
				ItemData.Write("IID_CYMBELINE", "Cost/Use", 200);

			// some weapon changes due to class/shop/item randomization
			if (classeschanged | cbxRandShop.Checked | cbxEventItems.Checked | cbxGMweaps.Checked)
			{
				// amiti / ragnell / staves / rudol gem are not locked to unit
				string[] weapons = new string[6] { "IID_AMITE", "IID_RAGNELL", "IID_ELSLEEP", "IID_ELSILENCE", "IID_REWARP", "IID_RUDOLGEM" };
				for (int i = 0; i < weapons.Length; i++)
				{
					// remove sealsteal
					List<string> newattributes = new List<string>();
					string[] attributes = ItemData.ReadStringArray(weapons[i], "Attributes");
					for (int j = 0; j < attributes.Length; j++)
					{
						if (attributes[j].StartsWith("sealsteal"))
						{ }
						else
							newattributes.Add(attributes[j]);
					}
					ItemData.Write(weapons[i], "Attributes", newattributes.ToArray());
				}

				// ballistae have price of 1500gp
				string[] ballista = new string[3] { "IID_LONGARCH", "IID_IRONARCH", "IID_KILLERARCH" };
				for (int i = 0; i < ballista.Length; i++)
					ItemData.Write(ballista[i], "Cost/Use", 300);
			}

			// remove ike/micaiah/sothe story promotions
			if (classeschanged | cbxStoryPromo.Checked)
			{
				string[] dudes = new string[4] { "JID_BRAVE", "JID_ROGUE", "JID_LIGHTMAGE", "JID_LIGHTSAGE" };
				for (int i = 0; i < dudes.Length; i++)
				{
					// remove sealsteal
					List<string> newskills = new List<string>();
					string[] skills = ClassData.ReadStringArray(dudes[i], "Skills");
					for (int j = 0; j < skills.Length; j++)
					{
						if (skills[j].StartsWith("SID_EVENT_CC"))
						{ }
						else
							newskills.Add(skills[j]);
					}
					ClassData.Write(dudes[i], "Skills", newskills.ToArray());
				}
			}

			// swap locktouch and treasurehunt
			if (classeschanged | cbxChooseIke.Checked | cbxChooseMic.Checked | cbxChooseElincia.Checked)
			{
				string[] theives = new string[5] { "JID_THIEF", "JID_ROGUE", "JID_ROGUE_F", "JID_ESPION", "JID_ESPION_F" };
				string[] dudes = new string[2] { "PID_SOTHE", "PID_HEATHER" };

				for (int i = 0; i < theives.Length; i++)
				{
					string[] skills = ClassData.ReadStringArray(theives[i], "Skills");
					for (int j = 0; j < skills.Length; j++)
					{
						if (skills[j] == "SID_THUNT")
						{
							skills[j] = "SID_KEYFREE";
							break;
						}
					}
					ClassData.Write(theives[i], "Skills", skills);
				}
				for (int i = 0; i < dudes.Length; i++)
				{
					string[] skills = CharacterData.ReadStringArray(dudes[i], "Skills");
					for (int j = 0; j < skills.Length; j++)
					{
						if (skills[j] == "SID_KEYFREE")
						{
							skills[j] = "SID_THUNT";
							break;
						}
					}
					CharacterData.Write(dudes[i], "Skills", skills);
				}
			}

			// give ike/sothe proper skill so his portrait works
			if (classeschanged | cbxChooseIke.Checked | cbxChooseMic.Checked | cbxChooseElincia.Checked)
			{
				if (characters[5].NewClass != -1)
				{
					if (classes[characters[5].NewClass].Race == "L")
					{
						string[] sotheskills = CharacterData.ReadStringArray("PID_SOTHE", "Skills");
						string[] newskills = new string[sotheskills.Length + 1];
						newskills[0] = "SID_HIGHER";
						for (int i = 0; i < sotheskills.Length; i++)
						{
							newskills[i + 1] = sotheskills[i];
						}
						CharacterData.Write("PID_SOTHE", "Skills", newskills);
					}
				}
				if (characters[34].NewClass != -1)
				{
					if (classes[characters[34].NewClass].Race == "L")
					{
						string[] ikeskills = CharacterData.ReadStringArray("PID_IKE", "Skills");
						string[] newskills = new string[ikeskills.Length + 1];
						newskills[0] = "SID_HIGHER";
						for (int i = 0; i < ikeskills.Length; i++)
						{
							newskills[i + 1] = ikeskills[i];
						}
						CharacterData.Write("PID_IKE", "Skills", newskills);
					}
				}
			}

			// modifies weapon ranks for characters to better match their class
			if (classeschanged | cbxChooseIke.Checked | cbxChooseMic.Checked | cbxChooseElincia.Checked)
			{
				for (int x = 0; x < characters.Length; x++)
				{
					if (characters[x].Chapter != "0")
					{
						if (characters[x].NewClass != -1)
						{
							string classranks = ClassData.ReadString(classes[characters[x].NewClass].JID, "Base_WeaponRank");
							string charranks = CharacterData.ReadString(characters[x].PID, "Weapon_Ranks");
							List<int> classrankorder = new List<int>();
							List<char> charranktypes = new List<char>();

							char[] ranktypes = new char[7] { '*', 'S', 'A', 'B', 'C', 'D', 'E' };
							for (int k = 0; k < ranktypes.Length; k++)
							{
								foreach (char rank in charranks)
								{
									if (rank == ranktypes[k])
									{
										charranktypes.Add(ranktypes[k]);
									}
								}
								for (int i = 0; i < classranks.Length; i++)
								{
									if (classranks[i] == ranktypes[k])
									{
										classrankorder.Add(i);
									}
								}
							}

							if (charranktypes.Count > 0 & classrankorder.Count > 0)
							{

								string outranks = classranks;
								for (int i = 0; i < classrankorder.Count; i++)
								{
									outranks = outranks.Substring(0, classrankorder[i]) + charranktypes[0] + outranks.Substring(classrankorder[i] + 1);
									if (charranktypes.Count > 1)
										charranktypes.RemoveAt(0);
								}

								CharacterData.Write(characters[x].PID, "Weapon_Ranks", outranks);
							}

						}
					}
				}

			}

			// changes arrowknight to promote to female bow paladin, as there is no t3 male equiv
			if (classeschanged | cbxChooseIke.Checked | cbxChooseMic.Checked | cbxChooseElincia.Checked)
			{
				ClassData.Write("JID_BOWKNIGHT", "Next_Class", "JID_ARROWKNIGHT_F");
				ClassData.Write("JID_ARROWKNIGHT_F", "Prev_Class", "JID_BOWKNIGHT");
			}

			// adds druid promotion
			if (classeschanged)
			{
				ClassData.Write("JID_DRUID", "Next_Class", "JID_SUMMONER");
				ClassData.Write("JID_SUMMONER", "Prev_Class", "JID_DRUID");
			}

			// lehran,stephan SS rank in all weapons
			if (classeschanged)
			{
				CharacterData.Write("PID_SOANEVALCKE", "Weapon_Ranks", "************");
				CharacterData.Write("PID_ERLAN", "Weapon_Ranks", "************");
			}

			// remove weapon caps on all classes
			if (cbxWeapCaps.Checked)
			{
				// read from file
				System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\JIDlist.txt");
				string[] JIDs = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				dataReader.Close();

				for (int i = 0; i < JIDs.Length; i++)
				{
					ClassData.Write(JIDs[i], "Max_WeaponRank", "************");
				}
			}

			// remove restrictions for all skills
			if (cbxUniversalSkills.Checked)
			{
				// read from file
				System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\skillcaplist.txt");
				string[] skilllist = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				dataReader.Close();

				for (int i = 0; i < skilllist.Length; i++)
				{
					string[] condition = SkillData.ReadStringArray(skilllist[i], "Condition_1");
					if (condition.Length > 0)
					{
						if (condition[0] != "")
						{
							if (skilllist[i].Contains("SID_HALFBEAST"))
							{
								SkillData.Write(skilllist[i], "Condition_1", new string[4] { "1", skilllist[i], "1", "SID_KING" });
								SkillData.Write(skilllist[i], "Condition_2", new string[2] { "0", "SFXC_ALIZE" });
							}
							else if (skilllist[i].Contains("SID_KING"))
							{
								SkillData.Write(skilllist[i], "Condition_1", new string[4] { "1", skilllist[i], "1", "SID_HALFBEAST" });
								SkillData.Write(skilllist[i], "Condition_2", new string[2] { "0", "SFXC_ALIZE" });
							}
							else
							{
								SkillData.Write(skilllist[i], "Condition_1", new string[2] { "1", skilllist[i] });
								SkillData.Write(skilllist[i], "Condition_2", new string[4] { "0", "SFXC_HUMAN", "0", "SFXC_ALIZE" });
							}
						}
					}
				}
			}

			// changes blessing usability to restrict heron classes instead of characters
			if (classeschanged & !cbxUniversalSkills.Checked)
			{
				string[] conditions = SkillData.ReadStringArray("SID_BLESSING", "Condition_2");
				for (int i = 0; i < conditions.Length; i++)
				{
					if (conditions[i] == "PID_RIEUSION")
						conditions[i] = "JID_PRINCEEGRET";
					else if (conditions[i] == "PID_LEARNE")
						conditions[i] = "JID_PRINCESSEGRET";
					else if (conditions[i] == "PID_RAFIEL")
						conditions[i] = "JID_PRINCEEGRET_RA";
				}
				SkillData.Write("SID_BLESSING", "Condition_2", conditions);
			}

			// all dragons can use all pool skills
			if (cbxDragonSkills.Checked)
			{
				string[] pools = new string[3] { "SID_WHITEWAVE", "SID_REDWAVE", "SID_BLACKWAVE" };
				string[] newcond = new string[2] { "0", "SFXC_DRAGON" };

				for (int i = 0; i < pools.Length; i++)
				{
					SkillData.Write(pools[i], "Condition_2", newcond);
				}
			}
			// blackwave is tied to PID kurth and dheg, need to change to JID tied
			else if (!cbxUniversalSkills.Checked)
			{
				string[] newcond = new string[8] { "0", "JID_DRAGONKING", "0", "JID_DRAGONPRINCE", "0", "JID_BLACKDRAGON", "0", "JID_BLACKDRAGON_KU" };
				SkillData.Write("SID_BLACKWAVE", "Condition_2", newcond);
			}

			// bird and heron skills tend to be PID tied, let's change that to JID tied
			if (classeschanged & !cbxUniversalSkills.Checked)
			{
				// tibarn and naesala are hand picked for some things
				string[] birdskills = new string[2] { "SID_CALL", "SID_VORTEX" };

				for (int i = 0; i < birdskills.Length; i++)
				{
					string[] conditions = SkillData.ReadStringArray(birdskills[i], "Condition_2");
					for (int j = 0; j < conditions.Length; j++)
					{
						if (conditions[j] == "PID_TIBARN")
							conditions[j] = "JID_KINGHAWK";
						else if (conditions[j] == "PID_NAESALA")
							conditions[j] = "JID_KINGCROW";
					}
					SkillData.Write(birdskills[i], "Condition_2", conditions);
				}

				// herons can't do many things
				string[] heronskills = new string[11] { "SID_HALFBEAST", "SID_WRATH", "SID_ADEPT", "SID_COUNTER", "SID_VANTAGE",
													"SID_WARMUP", "SID_BIRDBUSTER", "SID_BEASTBUSTER", "SID_DRAGONBUSTER", "SID_PARITY", "SID_GAMBLE" };

				for (int i = 0; i < heronskills.Length; i++)
				{
					string[] conditions = SkillData.ReadStringArray(heronskills[i], "Condition_1");
					for (int j = 0; j < conditions.Length; j++)
					{
						if (conditions[j] == "PID_RIEUSION")
							conditions[j] = "JID_PRINCEEGRET";
						else if (conditions[j] == "PID_LEARNE")
							conditions[j] = "JID_PRINCESSEGRET";
						else if (conditions[j] == "PID_RAFIEL")
							conditions[j] = "JID_PRINCEEGRET_RA";
					}
					SkillData.Write(heronskills[i], "Condition_1", conditions);
				}
			}

			// turn IID_TROOP into formshift scroll
			if (cbxFormshift.Checked)
			{
				// change capacity from 0 to 25
				SkillData.Write("SID_KING", "Capacity", 25);
				// conditions
				string[] newcond = new string[4] { "1", "SID_KING", "0", "SFXC_ALIZE" };
				SkillData.Write("SID_KING", "Condition_1", newcond);
				// scroll
				SkillData.Write("SID_KING", "Unlock_Item", "IID_TROOP");

				// IID_TROOP changes
				ItemData.Write("IID_TROOP", "Cost/Use", 5000);

				// -- since formshift now has 25 capacity, let's raise each laguz royal's capacity by 25
				// no longer need to increase capacity as the skill is default on necessary characters, not classes
				string[] kings = new string[10] { "JID_QUEENWOLF", "JID_KINGLION", "JID_KINGCROW", "JID_KINGHAWK", "JID_DRAGONKING",
												"JID_WOLF_F", "JID_LION_CA", "JID_CROW_NA", "JID_HAWK_TI", "JID_BLACKDRAGON" };

				for (int i = 0; i < kings.Length; i++)
				{
					//int capacity = ClassData.ReadInt(kings[i], "Capacity");
					//ClassData.Write(kings[i], "Capacity", capacity + 25);

					// remove formshift from classes - will be given to characters who are royals
					string[] skills = ClassData.ReadStringArray(kings[i], "Skills");
					List<string> newskills = new List<string>();
					for (int j = 0; j < skills.Length; j++)
					{
						if (skills[j].StartsWith("SID_KING"))
						{ }
						else
							newskills.Add(skills[j]);
					}
					ClassData.Write(kings[i], "Skills", newskills.ToArray());
				}

				// add formshift to character if character is a royal laguz
				for (int i = 0; i < characters.Length; i++)
				{
					if (characters[i].Chapter != "0") // only playable characters
					{
						int charclass;
						if (characters[i].NewClass != -1)
							charclass = characters[i].NewClass;
						else
							charclass = characters[i].VanillaClass;

						if (classes[charclass].Race == "L" & classes[charclass].Tier_P.Contains("c")) // royal laguz
						{
							string[] skills = CharacterData.ReadStringArray(characters[i].PID, "Skills");
							List<string> newskills = new List<string>();
							bool has_king = false;
							for (int j = 0; j < skills.Length; j++)
							{
								if (skills[j] != "")
									newskills.Add(skills[j]);
								// if character already has formshift due to random skills, don't add it again
								if (skills[j].StartsWith("SID_KING"))
									has_king = true;
							}
							if (!has_king)
								newskills.Add("SID_KING");
							CharacterData.Write(characters[i].PID, "Skills", newskills.ToArray());
						}
					}
				}

				// add formshift to dhegensia
				string[] dhegskills = CharacterData.ReadStringArray("PID_DHEGINHANSEA", "Skills");
				List<string> newdhegskills = new List<string>();
				for (int j = 0; j < dhegskills.Length; j++)
					newdhegskills.Add(dhegskills[j]);

				newdhegskills.Add("SID_KING");
				CharacterData.Write("PID_DHEGINHANSEA", "Skills", newdhegskills.ToArray());

			}

			// skill capacity randomization
			if (cbxSkillCap.Checked)
			{
				// read from file
				System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\skillcaplist.txt");
				string[] skilllist = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				dataReader.Close();

				for (int i = 0; i < skilllist.Length; i++)
				{
					int capacity = SkillData.ReadInt(skilllist[i], "Capacity");
					if (capacity != 0)
					{
						// chance of changing by multiples of 5
						// if skill is canto or shove, do not allow capacity to increase to prevent a softlock
						int changechance = random.Next(100);
						if (changechance < 15) // 15% no change
						{ }
						else if (changechance < 38) // 23% -5
							capacity -= 5;
						else if (changechance < 60 & skilllist[i] != "SID_CANTO" & skilllist[i] != "SID_TACKLE") // 22% +5
							capacity += 5;
						else if (changechance < 75) // 15% -10
							capacity -= 10;
						else if (changechance < 90 & skilllist[i] != "SID_CANTO" & skilllist[i] != "SID_TACKLE") // 15% +10
							capacity += 10;
						else if (changechance < 95) // 5% -15
							capacity -= 15;
						else if (skilllist[i] != "SID_CANTO" & skilllist[i] != "SID_TACKLE") // 5% +15
							capacity += 15;
						// coerce
						if (capacity < 0)
							capacity = 0;
						// write
						SkillData.Write(skilllist[i], "Capacity", capacity);
					}
				}
			}

			// make blessing only cost 5 capacity for herons
			if (cbxSkillRand.Checked)
				SkillData.Write("SID_BLESSING", "Capacity", 5);

			// randomizes the movement of all classes, then modifies ledge/swamp tile movement
			// to be equal to minimum movement for all classes
			if (cbxRandMove.Checked)
			{
				// read from file
				System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\JIDlist.txt");
				string[] JIDs = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				dataReader.Close();

				int movemin = Convert.ToInt32(numericMoveMin.Value);
				int movemax = Convert.ToInt32(numericMoveMax.Value);

				// loop through classes and change movement
				for (int i = 0; i < JIDs.Length; i++)
				{
					ClassData.Write(JIDs[i], "MOV", random.Next(movemin, movemax + 1));
				}

				// change movement for swamp/ledge to minimum movement
				// weird terrain names
				string[] terrains = new string[7] { "92-69-8D-B7", "92-E1-82-A2-8A-52", "90 F3 90 A3 82 50", "90 F3 90 A3 82 51",
													"90 EC", "90 EC 82 51", "90 EC 82 52" };

				for (int i = 0; i < terrains.Length; i++)
				{
					int[] costs = TerrData.ReadIntArray(terrains[i], "Cost");
					for (int j = 0; j < costs.Length; j++)
					{
						if (costs[j] != 0 & costs[j] != 255 & costs[j] > movemin) // 0 is end, 255 is impassible so we shouldn't overwrite that
							costs[j] = movemin;
					}
					TerrData.Write(terrains[i], "Cost", costs);
				}
			}

			// modify ledges and swamp so horses can move up them
			if (cbxHorseParkour.Checked)
			{
				// weird terrain names
				string[] terrains = new string[7] { "92-69-8D-B7", "92-E1-82-A2-8A-52", "90-F3-90-A3-82-50", "90-F3-90-A3-82-51",
													"90-EC", "90-EC-82-51", "90-EC-82-52" };

				for (int i = 0; i < terrains.Length; i++)
				{
					int[] costs = TerrData.ReadIntArray(terrains[i], "Cost");
					if (cbxRandMove.Checked & numericMoveMin.Value < 6)
					{
						// change to minimum selected movement
						costs[8] = (int)numericMoveMin.Value;
						costs[9] = (int)numericMoveMin.Value;
					}
					else
					{
						// change to movement cost of 6
						costs[8] = 6;
						costs[9] = 6;
					}
					TerrData.Write(terrains[i], "Cost", costs);
				}
			}

			// make sky and cloud tiles function as grass
			if (classeschanged | cbxRandEnemy.Checked | cbxRandBosses.Checked)
			{
				// weird terrain names
				string[] terrains = new string[2] { "8B-F3", "89-5F" };

				for (int i = 0; i < terrains.Length; i++)
				{
					int[] costs = TerrData.ReadIntArray(terrains[i], "Cost");
					for (int j = 0; j < costs.Length; j++)
					{
						if (costs[j] != 0)
							costs[j] = 1;
					}
					TerrData.Write(terrains[i], "Cost", costs);
				}
			}

			// lower stats of part 2 enemies and buffs stats of part 2 allies
			if (cbxPart2Enemies.Checked | cbxPart2Allies.Checked)
			{
				StreamReader dataReader = new StreamReader(file + "\\assets\\enemyPIDlist.txt");
				string[] enemypids = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				dataReader.Close();

				for (int i = 0; i < enemypids.Length; i++)
				{
					if (cbxPart2Enemies.Checked & enemypids[i].Contains("REBELLION"))
					{
						int[] bases = CharacterData.ReadIntArray(enemypids[i], "Bases");
						for (int x = 0; x < bases.Length; x++)
						{
							bases[x] -= 3;
							if (bases[x] < 0)
								bases[x] += 256;
						}
						CharacterData.Write(enemypids[i], "Bases", bases);
					}
					else if (cbxPart2Allies.Checked & enemypids[i].Contains("CRIMEA"))
					{
						int[] bases = CharacterData.ReadIntArray(enemypids[i], "Bases");
						for (int x = 0; x < bases.Length; x++)
						{
							bases[x] += 3;
							if (bases[x] > 255)
								bases[x] = 255;
						}
						CharacterData.Write(enemypids[i], "Bases", bases);
					}
				}
			}

			// buffs stats of characters obtained in part 2
			if (cbxPart2PCs.Checked)
			{
				string[] part2pcs = new string[9] { "PID_CHAP", "PID_NEPHENEE", "PID_HEATHER", "PID_ERINCIA", "PID_LUCHINO", "PID_KEVIN", "PID_STELLA", "PID_MAKAROV", "PID_CALILL" };

				for (int i = 0; i < part2pcs.Length; i++)
				{
					int[] bases = CharacterData.ReadIntArray(part2pcs[i], "Bases");
					for (int x = 0; x < bases.Length; x++)
					{
						bases[x] += 2;
						if (bases[x] > 255)
							bases[x] = 255;
					}
					CharacterData.Write(part2pcs[i], "Bases", bases);
				}
			}

			// modify the "win conditions" listed to the player - only visual change,
			// script files still need to be changed elsewhere in the code
			if (cbxWinCon.Checked)
			{
				string[] mapnames = new string[6] { "C0401", "C0402", "C0403", "C0404", "C0405", "C0407a" };

				string defeatboss = MapData.ReadStringArray("C0202", "Objective_H")[0];
				string seizemap = MapData.ReadStringArray("C0111", "Objective_H")[0];

				string[] objs = new string[3] { "Objective_E", "Objective_M", "Objective_H" };

				for (int i = 0; i < mapnames.Length; i++)
				{
					for (int j = 0; j < objs.Length; j++)
					{
						string[] conditions = MapData.ReadStringArray(mapnames[i], objs[j]);
						if (mapnames[i] == "C0405")
							conditions[0] = seizemap;
						else
							conditions[0] = defeatboss;

						MapData.Write(mapnames[i], objs[j], conditions);
					}
				}
			}

			// unused
			if (aprilFools)
			{
				for (int k = 0; k < 69; k++)
				{
					/*
					byte[] mpid = { 0x00, 0x03, 0x20, 0x68 };
					byte[] mnpid = { 0x00, 0x03, 0x18, 0x1c };
					// change mpids and mnpid to EDDIE (replaced with camilla)
					stream.Position = charPID[k] + 4;
					stream.Write(mpid, 0, 4);
					stream.Write(mnpid, 0, 4);
					*/
				}
			}

		}
		private void ScriptModifications()
		{
			textBox1.Text = "Modifying Script Files";
			Application.DoEvents();

			StreamReader dataReader = new StreamReader(file + "\\assets\\dropshopitems.csv");
			string[] allitems = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			int rafielchar = -1, leannechar = -1, reysonchar = -1;
			bool nailahbeorc = false, volugbeorc = false, vikabeorc = false, mwarimbeorc = false, nealuchibeorc = false, rafielbeorc = false, kurthbeorc = false;
			
			for (int i = 0; i < characters.Length; i++)
			{
				// check race of various laguz characters that would cause changes to scripts if they are beorc
				if (characters[i].Name == "nailah")
					nailahbeorc = characters[i].NewRace == "B";
				else if (characters[i].Name == "volug")
					volugbeorc = characters[i].NewRace == "B";
				else if (characters[i].Name == "vika")
					vikabeorc = characters[i].NewRace == "B";
				else if (characters[i].Name == "maurim")
					mwarimbeorc = characters[i].NewRace == "B";
				else if (characters[i].Name == "nealuchi")
					nealuchibeorc = characters[i].NewRace == "B";
				else if (characters[i].Name == "rafiel")
					rafielbeorc = characters[i].NewRace == "B";
				else if (characters[i].Name == "kurthnaga")
					kurthbeorc = characters[i].NewRace == "B";

				// get which PIDs were changed to herons
				int newclass = characters[i].NewClass;
				if (newclass > 92 & newclass < 96) // is a heron
				{
					if (newclass == 93) // first heron is rafiel
						rafielchar = i;
					else if (newclass == 94) // second is leanne
						leannechar = i;
					else // reyson
						reysonchar = i;
				}
			}

			// loop through extracted script files
			for (int x = 0; x < script_exl.Length; x++)
			{
				string scriptname = Path.GetFileNameWithoutExtension(script_exl[x]);
				// read in all lines
				StreamReader tempread = new StreamReader(script_exl[x]);
				string[] filelines = tempread.ReadToEnd().Split('\n');//new string[1] { Environment.NewLine }, StringSplitOptions.None);
				tempread.Close();

				// set up edited lines
				List<string> outscriptlines = new List<string>();

				// loop through each line looking for certain text depending on settings
				for (int y = 0; y < filelines.Length; y++)
				{
					string templine = filelines[y];
					// if this is false, the line will be deleted at the end of the iteration
					bool saveline = true;

					// changes to prevent laguz from transforming if they are no longer laguz and also softlocks
					if (classeschanged | cbxChooseElincia.Checked | cbxChooseIke.Checked | cbxChooseMic.Checked)
					{
						// 1-8 volug
						if (scriptname.Contains("0106") & (volugbeorc) & templine.Contains("UnitTransform"))
							saveline = false;

						// 1-7 muarim, vika
						if (scriptname.Contains("0108") & (mwarimbeorc | vikabeorc) & templine.Contains("UnitTransform"))
							saveline = false;

						// 1-8 nailah, volug, muarim, vika
						if (scriptname.Contains("0109") & (nailahbeorc | volugbeorc | mwarimbeorc | vikabeorc) & templine.Contains("UnitTransform"))
							saveline = false;

						// 2-P nealuchi
						if (scriptname.Contains("0201") & (nealuchibeorc) & templine.Contains("UnitTransform"))
							saveline = false;

						// 3-6 volug doesnt get halfshift
						if (scriptname.Contains("0307") & (volugbeorc) & templine.Contains("UnitAddSkill") & templine.Contains("SID_HALFBEAST"))
							saveline = false;

						// 3-13 nailah
						if (scriptname.Contains("0314") & (nailahbeorc) & templine.Contains("UnitTransform"))
							saveline = false;

						// 3-F nailah, volug, rafiel
						if (scriptname.Contains("0315") & (nailahbeorc | volugbeorc | rafielbeorc) & templine.Contains("UnitTransform"))
						{
							if (filelines[y - 1].Contains("PID_NIKE") | filelines[y - 1].Contains("PID_OLUGH") | filelines[y-1].Contains("PID_RAFIEL"))
								saveline = false;
						}

						// 4-4 muarim, vika
						if (scriptname.Contains("0405") & (mwarimbeorc | vikabeorc) & templine.Contains("UnitTransform"))
							saveline = false;

						// 4-Fc kurth doesnt get formshift
						if (scriptname.Contains("0407c") & (kurthbeorc) & templine.Contains("UnitGetSkill") & templine.Contains("SID_KING"))
							saveline = false;
					}

					// remove 3-4 skrimir fight
					if (classeschanged | cbxChooseElincia.Checked | cbxChooseIke.Checked | cbxChooseMic.Checked)
					{
						if (scriptname.Contains("0305") & templine.Contains("NetuzoBattle"))
							saveline = false;
					}

					// remove game overs (without ironman mode)
					if (classeschanged & !cbxIronMan.Checked)
					{
						// 1-5 jill, tauroneo, zihark
						if (scriptname.Contains("0106") & templine.Contains("gf_gameover"))
						{
							if (filelines[y - 1].Contains("PID_JILL") | filelines[y - 1].Contains("PID_ZIHARK") | filelines[y - 1].Contains("PID_TAURONEO"))
								saveline = false;
						}

						// 1-6 fiona
						if (scriptname.Contains("0106") & templine.Contains("gf_gameover"))
						{
							if (filelines[y - 1].Contains("PID_FRIEDA"))
								saveline = false;
						}

						// 2-1 nephenee, brom
						if (scriptname.Contains("0202") & templine.Contains("gf_gameover"))
						{
							if (filelines[y - 1].Contains("PID_CHAP") | filelines[y - 1].Contains("PID_NEPHENEE"))
								saveline = false;
						}
					}

					// remove game overs for ironman mode
					if (cbxIronMan.Checked)
					{
						if (templine.Contains("gf_gameover"))
						{
							// if the gameover is set by a unit dying, the previous line will contain a PID and the next line a MDIE_ script
							if (filelines[y - 1].Contains("PID_") & filelines[y + 1].Contains("DIE"))
							{
								// now we remove all lines that aren't characters that still provide gameovers in ironman mode
								if (filelines[y - 1].Contains("PID_MICAIAH"))
								{ }
								else if (filelines[y - 1].Contains("PID_IKE"))
								{ }
								else if (filelines[y - 1].Contains("PID_LAURA") & scriptname.Contains("0103"))
								{ }
								else if (filelines[y - 1].Contains("PID_ERINCIA") & (scriptname.Contains("0201") | scriptname.Contains("0205") | scriptname.Contains("0311")))
								{ }
								else if (filelines[y - 1].Contains("PID_LUCHINO") & scriptname.Contains("0203"))
								{ }
								else if (filelines[y - 1].Contains("PID_GEOFFRAY") & (scriptname.Contains("0204") | scriptname.Contains("0205") | scriptname.Contains("0310")))
								{ }
								else if (filelines[y - 1].Contains("PID_LAY") & scriptname.Contains("0301") | scriptname.Contains("0305"))
								{ }
								else if (filelines[y - 1].Contains("PID_SKRIMIR") & scriptname.Contains("0301"))
								{ }
								else if (filelines[y - 1].Contains("PID_TIBARN") & (scriptname.Contains("0312") | scriptname.Contains("0403") | scriptname.Contains("0406")))
								{ }
								else
									saveline = false;

							}
						}
					}

					// remove ragnell restriction from ashera kill
					if (true)
					{
						if (scriptname.Contains("0407e") & templine.Contains("UnitGetEquipWepI"))
						{
							string[] splits = templine.Split(new string[1] { "PID_IKE" }, StringSplitOptions.None);
							templine = splits[0] + "PID_IKE\"))) {";
						}
					}

					// tormod crew gets new items in 4-4
					if (classeschanged)
					{
						if (scriptname.Contains("0405") & templine.Contains("UnitAddItem"))
						{
							string[] newweaps;
							int unitnum = -1;
							if (filelines[y - 2].Contains("PID_TOPUCK"))
								unitnum = 14;
							else if (filelines[y - 2].Contains("PID_MWARIM"))
								unitnum = 15;
							else if (filelines[y - 2].Contains("PID_VIZE"))
								unitnum = 16;

							if (unitnum != -1)
							{
								int numweapons = 2;
								if (unitnum == 14)
									numweapons = 3;

								if (characters[unitnum].NewClass != -1)
								{
									if (classes[characters[unitnum].NewClass].Tier_P.Contains("b"))
										newweaps = ChoosePlayerT2Weapons(characters[unitnum].NewClass, numweapons, characters[unitnum].Name);
									else
										newweaps = ChoosePlayerT3Weapons(characters[unitnum].NewClass, numweapons, characters[unitnum].Name);
									for (int i = 0; i < newweaps.Length; i++)
									{
										if (newweaps[i] != "")
										{
											string oldweap = templine.Split('\"')[1];
											if (oldweap != "")
												templine = templine.Replace(oldweap, newweaps[i]);
											outscriptlines.Add(templine);
											y++;
											templine = filelines[y];
										}
									}

								}
							}
						}
					}

					// various heron changes in 4-F
					if (classeschanged)
					{
						if (scriptname.Contains("0407"))
						{
							if (templine.Contains("PID_RAFIEL") & rafielchar != -1)
								templine = templine.Replace("PID_RAFIEL", characters[rafielchar].PID);
							if (templine.Contains("PID_LEARNE") & leannechar != -1)
								templine = templine.Replace("PID_LEARNE", characters[leannechar].PID);
							if (templine.Contains("PID_RIEUSION") & reysonchar != -1)
								templine = templine.Replace("PID_RIEUSION", characters[reysonchar].PID);
						}
					}

					// gives extra BEXP at the end of levels
					if (cbxBonusBEXP.Checked)
					{
						if (templine.Contains("Achieve_CLEAR_BONUS"))
						{
							string[] splits = templine.Split(new string[1] { "BONUS" }, StringSplitOptions.None);
							templine = splits[0] + "BONUS(32767, 32767);";
						}
					}

					// event item randomization
					if (cbxEventItems.Checked | cbxWhiteGem.Checked)
					{
						// hidden treasure, village visit/treasurechest, base convo
						if (templine.Contains("IID_") & (templine.Contains("GetRndTreasure") | templine.Contains("MindGetItem") | templine.Contains("UnitGetItemShowing")))
						{
							string olditem = "IID_" + templine.Split(new string[1] { "IID_" }, StringSplitOptions.None)[1].Split('\"')[0];
							string newitem = "";

							// don't randomize these
							if (olditem.Contains("RUDOLGEM") | olditem.Contains("AMITE") | olditem.Contains("SILVERCARD") |
								olditem.Contains("RAGNELL") | olditem.Contains("ETTARD"))
							{ }
							else if (olditem.Contains("HOLYCROWN") & !cbxMistCrown.Checked)
							{ }
							else if (olditem.Contains("REBLOW") & scriptname.Contains("0205"))
							{ }
							else if (cbxNoRandPromotions.Checked & (olditem.Contains("MASTERPROOF") | olditem.Contains("MASTERCROWN")))
							{ }
							else if (cbxWhiteGem.Checked & olditem.Contains("IID_COIN"))
							{
								// only white gems replace coins
								newitem = "IID_WHITEGEM";
							}
							else if (cbxEventItems.Checked)
							{
								// actually randomize item
								newitem = allitems[random.Next(allitems.Length)].Split(',')[0];
							}

							// save
							if (newitem != "")
							{
								eventItemsOutput += scriptname + "," + olditem + "," + newitem + ";";
								templine = templine.Replace(olditem, newitem);
							}
						}
					}

					// win con changes to part 4
					if (cbxWinCon.Checked)
					{
						if ((scriptname.Contains("0402") & templine.Contains("MS_0402_DIE")) |
							(scriptname.Contains("0403") & templine.Contains("MS_0403_DIE")) |
							(scriptname.Contains("0404") & templine.Contains("MS_0404_DIE")) |
							(scriptname.Contains("0407a") & templine.Contains("MS_0407a_DIE_L")) )
						{
							outscriptlines.Add(templine);
							outscriptlines.Add("    set(\"gf_complete\");");
							y++;
							templine = filelines[y];
						}

						if (scriptname.Contains("0405") & templine.Contains("callback[0x7]() {"))
						{
							outscriptlines.Add("callback[0x5](5, 2, 13, \"烽P上右\") {");
							outscriptlines.Add("set(\"gf_complete\");");
							outscriptlines.Add("}");
							y += 3;
						}
					}


					if (saveline)
						outscriptlines.Add(templine);
				}

				// save all changes back to file
				StreamWriter writer = new StreamWriter(script_exl[x]);
				for (int i = 0; i < outscriptlines.Count; i++)
					writer.WriteLine(outscriptlines[i]);
				writer.Close();
				
			}


		}
		private void TextModifications()
		{
			textBox1.Text = "Modifying Text";
			Application.DoEvents();

			using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_common.m", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
			{
				// change description of holy crown
				if (characters[37].NewRecr != -1) // character replacing mist
				{
					if (gameVersion == 2) // PAL iso
						stream.Position = 56121;
					else
						stream.Position = 52801;

					string mistname = characters[characters[37].NewRecr].Name;
					mistname = mistname[0].ToString().ToUpper() + mistname.Substring(1) + ".";
					byte[] mistnamebytes = System.Text.Encoding.ASCII.GetBytes(mistname);
					stream.Write(mistnamebytes, 0, mistname.Length);
					stream.WriteByte(0x00);
				}

				// change name and description of IID_TROOP
				if (cbxFormshift.Checked)
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

				// saves the new modifiers of random stat boosters as their descriptions
				if (cbxStatBooster.Checked)
				{
					string[] statboosters = new string[8] { "IID_ANGELROBE", "IID_ENERGYDROP", "IID_SPIRITPOWDER", "IID_SECRETBOOK",
														"IID_SPEEDWING", "IID_GODDESSICON", "IID_DRAGONSHIELD", "IID_TALISMAN" };
					int[] usetext = new int[statboosters.Length];
					int[] desctext = new int[statboosters.Length];

					string line;
					string[] values;

					System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\StatBoosters.csv");
					// skip new line
					line = dataReader.ReadLine();
					// read lines
					for (int i = 0; i < statboosters.Length; i++)
					{
						line = dataReader.ReadLine();
						values = line.Split(',');

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

					// loop through each statbooster
					for (int i = 0; i < statboosters.Length; i++)
					{
						int[] statchanges = ItemData.ReadIntArray(statboosters[i], "Bonuses");

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

				// unused
				if (aprilFools)
				{
					/*
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
					*/
				}

			}

			// heron changes - changes what names pop up when choosen the heron for the tower
			if (classeschanged)
			{
				using (var stream = new System.IO.FileStream(dataLocation + "\\Mess\\e_c0407a.m", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int i = 0; i < characters.Length; i++)
					{
						int newclass = characters[i].NewClass;
						if (newclass > 92 & newclass < 96) // is a heron
						{
							string heronname = characters[i].Name;
							if (cbxRandRecr.Checked == true)
								heronname = characters[characters[i].NewRecr].Name;
							if (heronname.Length > 6)
								heronname = heronname.Substring(0, 6);
							byte[] heronbytes = System.Text.Encoding.ASCII.GetBytes(heronname);


							if (newclass == 93) // first heron is rafiel
								stream.Position = 6768;
							else if (newclass == 94) // second is leanne
								stream.Position = 6784;
							else // reyson
								stream.Position = 6776;
							for (int j = 0; j < heronbytes.Length; j++)
								stream.WriteByte(heronbytes[j]);
							stream.WriteByte(0x00);
						}
					}
				}
			}

		}
		private void MaindolModifications()
		{
			textBox1.Text = "Modifying main.dol";
			Application.DoEvents();

			using (var stream = new System.IO.FileStream(sysLocation + "\\main.dol", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
			{
				// changes pids for herons in main.dol
				if (classeschanged)
				{
					for (int i = 0; i < characters.Length; i++)
					{
						int newclass = characters[i].NewClass;
						if (newclass > 92 & newclass < 96) // is a heron
						{
							byte[] PIDbytes = System.Text.Encoding.ASCII.GetBytes(characters[i].PID);

							if (gameVersion == 0)
							{
								if (newclass == 93) // first heron is rafiel
									stream.Position = 3559766;
								else if (newclass == 94) // second is leanne
									stream.Position = 3559755;
								else // reyson
									stream.Position = 3559742;
							}
							else if (gameVersion == 1)
							{
								if (newclass == 93) // first heron is rafiel
									stream.Position = 3559606;
								else if (newclass == 94) // second is leanne
									stream.Position = 3559595;
								else // reyson
									stream.Position = 3559582;
							}
							else if (gameVersion == 2)
							{
								if (newclass == 93) // first heron is rafiel
									stream.Position = 3564270;
								else if (newclass == 94) // second is leanne
									stream.Position = 3564259;
								else // reyson
									stream.Position = 3564246;
							}
							for (int j = 0; j < 12; j++)
							{
								if (j == 10 & (newclass == 93 | newclass == 94))
									break;
								if (j >= PIDbytes.Length)
									stream.WriteByte(0x00);
								else
									stream.WriteByte(PIDbytes[j]);
							}
						}
					}
				}

				// adds back in aspects of the game that were taken away
				// in hard mode for no understandable reason
				if (cbxEnemyRange.Checked | cbxWeapTri.Checked | cbxMapAff.Checked)
				{
					if (gameVersion == 0)
					{
						if (cbxEnemyRange.Checked)
						{
							stream.Position = 1628235;
							stream.WriteByte(3);

							stream.Position = 322075;
							stream.WriteByte(3);

							stream.Position = 408031;
							stream.WriteByte(3);
						}
						if (cbxWeapTri.Checked)
						{
							stream.Position = 157227;
							stream.WriteByte(3);
						}
						if (cbxMapAff.Checked)
						{
							stream.Position = 1004883;
							stream.WriteByte(3);

							stream.Position = 1135043;
							stream.WriteByte(3);
						}
					}
					else if (gameVersion == 1)
					{
						if (cbxEnemyRange.Checked)
						{
							stream.Position = 1628171;
							stream.WriteByte(3);

							stream.Position = 322011;
							stream.WriteByte(3);

							stream.Position = 407967;
							stream.WriteByte(3);
						}
						if (cbxWeapTri.Checked)
						{
							stream.Position = 157163;
							stream.WriteByte(3);
						}
						if (cbxMapAff.Checked)
						{
							stream.Position = 1004819;
							stream.WriteByte(3);

							stream.Position = 1134979;
							stream.WriteByte(3);
						}
					}
					else if (gameVersion == 2)
					{
						if (cbxEnemyRange.Checked)
						{
							stream.Position = 1523555;
							stream.WriteByte(3);

							stream.Position = 209219;
							stream.WriteByte(3);

							stream.Position = 295307;
							stream.WriteByte(3);
						}
						if (cbxWeapTri.Checked)
						{
							stream.Position = 44171;
							stream.WriteByte(3);
						}
						if (cbxMapAff.Checked)
						{
							stream.Position = 898031;
							stream.WriteByte(3);

							stream.Position = 1029303;
							stream.WriteByte(3);
						}
					}
				}

				// zero / negative growths patch
				if (cbxZeroGrowths.Checked | cbxNegGrowths.Checked)
				{
					int growthcalc = 0;
					int bexpcalc = 0;
					int[] positiveones = new int[3];
					if (gameVersion == 0)
					{
						growthcalc = 431204;
						bexpcalc = 432564;//431560;
						positiveones = new int[3] { 430236, 430144, 430164 };
					}
					else if (gameVersion == 1)
					{
						growthcalc = 431140;
						bexpcalc = 432500;// 431496;
						positiveones = new int[3] { 430172, 430080, 430100 };
					}
					else if (gameVersion == 2)
					{
						growthcalc = 318504;
						bexpcalc = 319864;// 318860;
						positiveones = new int[3] { 317536, 317444, 317464 };
					}

					// assembly command to load 0 or -1 into r0 (which is then added to current stat)
					byte[] addzero = new byte[4] { 0x38, 0x00, 0x00, 0x00 };
					byte[] subone = new byte[4] { 0x38, 0x00, 0xFF, 0xFF };

					// growthcalc adds one to value if no other stats increase; this needs to change to 0
					// growth calculation occurs for all eight stats with 12 byte intervals
					stream.Position = growthcalc;
					for (int i = 0; i < 8; i++)
					{
						stream.Write(addzero, 0, 4);
						stream.Position += 8;
					}

					// bexpcalc does the bexp calculation, if less than 3 stats increase; this changes to 0 in zero, -1 in negative
					// same as growth calc, this occurs for all eight stats in 12 byte intervals
					stream.Position = bexpcalc;
					for (int i = 0; i < 8; i++)
					{
						if (cbxZeroGrowths.Checked)
							stream.Write(addzero, 0, 4);
						else if (cbxNegGrowths.Checked)
							stream.Write(subone, 0, 4);
						stream.Position += 8;
					}

					// if negative growths is on, need to change the +1 to -1 in a couple different places
					if (cbxNegGrowths.Checked)
					{
						for (int i = 0; i < positiveones.Length; i++)
						{
							// change add 1 operation to subtract 1
							stream.Position = positiveones[i] + 2;
							stream.WriteByte(0xFF);
							stream.WriteByte(0xFF);
						}
					}

				}

				if (cbxZeroGrowths.Checked & false)
				{
					int growthcalc = 0;
					int bexpcalc = 0;
					if (gameVersion == 0)
					{
						growthcalc = 431204;
						bexpcalc =   432564;//431560;
					}
					else if (gameVersion == 1)
					{
						growthcalc = 431140;
						bexpcalc =   432500;// 431496;
					}
					else if (gameVersion == 2)
					{
						growthcalc = 318504;
						bexpcalc =   319864;// 318860;
					}

					if (growthcalc != 0 & bexpcalc != 0)
					{
						byte[] nop = new byte[4] { 0x38, 0x00, 0x00, 0x00 };
						// growth calculation occurs for all eight stats with 12 byte intervals - change four bytes that add one (0x88 0x01 0x00 0x00) to four bytes that add zero (0x88 0x00 0x00 0x00)
						stream.Position = growthcalc;
						for (int i = 0; i < 8; i++)
						{
							stream.Write(nop, 0, 4);
							stream.Position += 8;
						}
						// bexp calculation occurs for all eight stats with 8 byte intervals - change four bytes that add one (0x88 0x01 0x00 0x00) to four bytes that add zero (0x88 0x00 0x00 0x00)
						stream.Position = bexpcalc;
						for (int i = 0; i < 8; i++)
						{
							//stream.Position += 1;
							//stream.WriteByte(0x00);
							//stream.Position += 2;
							stream.Write(nop, 0, 4);
							stream.Position += 8;
						}
					}
				}

				// negative growths patch
				if (cbxNegGrowths.Checked & false)
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

					byte[] nop = new byte[4] { 0x38, 0x00, 0x00, 0x00 };
					// growth calculation occurs for all eight stats with 12 byte intervals - change four bytes that add one (0x88 0x01 0x00 0x00) to four bytes that add zero (0x88 0x00 0x00 0x00)
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
		}

		#endregion

		// *************************************************************************************** CLOSING FUNCTIONS
		#region

		// deletes temp files used in any previous randomization process
		private void DeleteTemp()
		{
			string tempfolder = file + "\\assets\\temp\\";
			string[] allfiles = getRecursiveFiles(tempfolder);
			foreach (string onefile in allfiles)
			{
				if (!onefile.Contains("exalt-cli.exe"))
					File.Delete(onefile);
			}
		}

		// compresses files back to ISO
		private void CompressFiles()
		{
			textBox1.Text = "Compressing FE10Data";
			Application.DoEvents();

			string tempfolder = file + "\\assets\\temp";

			string compressed = dataLocation + "\\FE10Data.cms";
			string decompressed = dataLocation + "\\FE10Data.cms.decompressed";

			// classes back to csv files
			CharacterData.Save();
			ClassData.Save();
			ItemData.Save();
			SkillData.Save();
			TerrData.Save();
			MapData.Save();

			// compress csv back to data file
			FE10ExtractCompress.CompressFE10Data(decompressed, tempfolder + "\\data");

			// compress FE10Data.cms.decompressed back to cms file
			List<byte> decompressedbytes = new List<byte>();
			using (var stream = new System.IO.FileStream(decompressed, System.IO.FileMode.Open,
					System.IO.FileAccess.ReadWrite))
			{
				// read all decompressed bytes
				stream.Position = 0;
				for (int i = 0; i < stream.Length; i++)
				{
					decompressedbytes.Add((byte)stream.ReadByte());
				}
			}
			// compress
			byte[] compressedbytes = LZ77.Compress(decompressedbytes.ToArray());
			// save all compressed bytes
			using (var stream = new System.IO.FileStream(compressed, System.IO.FileMode.Create,
					System.IO.FileAccess.ReadWrite))
			{
				stream.Position = 0;
				foreach (byte data in compressedbytes)
					stream.WriteByte(data);
			}

			// compress dispos csvs back to dispos files
			for (int i = 0; i < ChapterData.Length; i++)
			{
				// save classes back to csv
				ChapterData[i].Save();

				string chapterpath = dataLocation + "\\zmap\\bmap0" + chapters[i] + "\\dispos_h.bin";
				if (chapters[i] == "emap407c") // ena, kurth, cain, and giffca appear in a weird file
					chapterpath = dataLocation + "\\zmap\\emap0407c\\dispos_c.bin";
				else if (chapters[i] == "emap407d") // location of characters locked to tower
					chapterpath = dataLocation + "\\zmap\\emap0407d\\dispos_c.bin";
				string csvpath = tempfolder + "\\chapter\\" + chapters[i] + ".csv";
				FE10ExtractCompress.CompressDispos(chapterpath, csvpath);
			}

			// compress shop file back
			string shopfile = dataLocation + "\\Shop\\shopitem_h.bin";
			string csvfile = tempfolder + "\\Shop.csv";
			FE10ExtractCompress.CompressShopfile(shopfile, csvfile);
			shopfile = dataLocation + "\\Shop\\shopitem_m.bin";
			FE10ExtractCompress.CompressShopfile(shopfile, csvfile);
		}

		// compile scripts with Exalt
		private void CompressScripts()
		{
			string scriptloc = dataLocation + "\\Scripts\\";
			string outloc = file + "\\assets\\temp\\script\\";

			// write batch file
			StreamWriter writer = new StreamWriter(outloc + "compile.bat");
			for (int i = 0; i < script_exl.Length; i++)
			{
				writer.WriteLine("\"exalt-cli.exe\" -g FE10 compile \"" + script_exl[i] + "\"");
			}
			writer.Close();

			// run batch file
			Process p = new Process();
			p.StartInfo.WorkingDirectory = outloc;
			p.StartInfo.FileName = "compile.bat";
			p.StartInfo.CreateNoWindow = false;
			p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
			p.Start();
			p.WaitForExit();

			// copy files back
			string[] scripts = Directory.GetFiles(outloc);

			for (int i = 0; i < scripts.Length; i++)
			{
				string extension = Path.GetExtension(scripts[i]);
				string tempfilename = Path.GetFileName(scripts[i]);
				if (extension == ".cmb")
				{
					File.Copy(scripts[i], scriptloc + tempfilename, true);
				}
			}

			
		}

		// saves settings string for outputlog
		private string SaveSettingsString()
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
														   cbxStatCapFlat, cbxT3Statcaps, cbxSkillVanilla, cbxBargains, cbxRandMove, cbxEnemySkills, cbxBossSkills,
														   cbxHorseParkour, cbxNoFOW, cbxIronMan, cbxRandClassBases, cbxShuffleClassBases, cbxHPShuffleclass, cbxWhiteGem,
														   cbxFormshift, cbxDarkMag, cbxClassPatch, cbxKnifeCrit, cbxRandPromotion, cbxMagicPatch, cbxPart2Enemies,
														   cbxParagon, cbxEasyPromotion, cbxNoEnemyLaguz,cbxBonusBEXP, cbxTormodT3, cbxLaguzCanto, cbxStatBooster,
														   cbxStatBoostMult, cbxNegGrowths, cbxStoryPromo, cbxAuthority, cbxSkillCap, cbxMicClass, cbxIkeClass,
															cbxHeronSpread, cbxSkillSetNum, cbxWeapPatch,cbxDragonCanto,cbxElinciaClass,cbxSkillVanilla,cbxSkillSetNum,
															cbxChooseElincia,cbxRecrVanillaClass,cbxBonusItems,cbxEnemBonusDrop,cbx1to1EnemyRand,cbxRandAllies,cbxPart2PCs,
															cbxPart2Allies,cbxLetheMordy, cbxUniversalSkills, cbxBossBonusDrop,cbxMistCrown,cbxDruidCrown};
			System.Windows.Forms.ComboBox[] comboBoxes = { comboClassOptions, comboIke, comboMicc, comboIkeClass, comboMicClass, comboElinciaClass, comboElincia };
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
																numericClassBaseDev, numericClassBaseShuf, numericStatCapMin,numStatBoostMin, numStatBoostMax,
																numSkillVanillaPlus,numSkillSet};
			System.Windows.Forms.RadioButton[] radioButtons = { radioArmor0, radioArmor1, radioArmor2, radioArmor3, radioArmor4, radioArmor5,
																radioBeast0, radioBeast1, radioBeast2, radioBeast3, radioBeast4, radioBeast5,
																radioBird0, radioBird1, radioBird2, radioBird3, radioBird4, radioBird5,
																radioCav0, radioCav1, radioCav2, radioCav3, radioCav4, radioCav5,
																radioDragon0, radioDragon1, radioDragon2, radioDragon3, radioDragon4, radioDragon5,
																radioFly0, radioFly1, radioFly2, radioFly3, radioFly4, radioFly5,
																radioInfantry0, radioInfantry1, radioInfantry2, radioInfantry3, radioInfantry4, radioInfantry5,
																radioMages0, radioMages1, radioMages2, radioMages3, radioMages4, radioMages5};

			// convert choices into strings
			string settingstring = "3.3.0,";
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

			randomizationSettings = settingstring;

			return (settingstring);
		}

		// writes outputlog for user
		private void CreateOutputLog()
		{
			textBox1.Text = "Creating log";
			Application.DoEvents();

			System.IO.StreamWriter logwriter = new System.IO.StreamWriter(file + "\\outputlog.htm");

			System.IO.StreamReader reader = new System.IO.StreamReader(file + "\\assets\\logheader.txt");
			string outlogtext = reader.ReadToEnd() + "\n";
			reader.Close();

			// add hyperlinks
			if (cbxTowerUnits.Checked)
				outlogtext += "<a href=\"#tower\">Tower Units</a>\n";
			if (cbxRandWeap.Checked)
				outlogtext += "<a href=\"#weapons\">Weapon Stats</a>\n";
			if (cbxRandClassBases.Checked | cbxShuffleClassBases.Checked | cbxRandMove.Checked)
				outlogtext += "<a href=\"#class\">Class Bases</a>\n";
			if (cbxStatCaps.Checked | cbxStatCapDev.Checked | cbxStatCapFlat.Checked)
				outlogtext += "<a href=\"#caps\">Stat Caps</a>\n";
			if (cbxStatBooster.Checked)
				outlogtext += "<a href=\"#boost\">Stat Boosters</a>\n";
			if (cbxSkillCap.Checked)
				outlogtext += "<a href=\"#skillcap\">Skill Capacities</a>\n";
			if (cbxEventItems.Checked)
				outlogtext += "<a href=\"#event\">Event Items</a>\n";
			if (cbxBargains.Checked | cbxForge.Checked)
				outlogtext += "<a href=\"#shop\">Shop</a>\n";
			if (cbxRandEnemy.Checked | cbxEnemBonusDrop.Checked)
				outlogtext += "<a href=\"#enemy\">Enemies</a>\n";
			if (cbxRandPromotion.Checked)
				outlogtext += "<a href=\"#promo\">Promotion Lines</a>\n";

			// add hidden chosen settings and if iso was re-randomized
			if (rerandomized)
				outlogtext += "<! -- RE-RANDOMIZED ISO -- >";
			outlogtext += "<! -- " + randomizationSettings + " -- >\n";

			outlogtext += "<h2>Seed: " + numericSeed.Value + "</h2>\n";

			//outlogtext += "<br><h3>Character Info</h3>";
			//outlogtext += htmlSpoilerButton("characterinfo");
			//outlogtext += "<div id=\"characterinfo\" style=\"display:none\">";

			outlogtext += "<div class=\"tab\">";
			for (charNum = 0; charNum < characters.Length; charNum++)
			{
				outlogtext += "<button class=\"tablinks\" onclick=\"openChar(event, '" + characters[charNum].Name +
					"')\" id=\"defaultOpen\"><img src=\"assets/logpics/" + characters[charNum].Name + ".png\" alt=\"" + characters[charNum].Name +
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
			//outlogtext += "</div>";

			for (charNum = 0; charNum < characters.Length; charNum++)
			{
				outlogtext += "<div id=\"" + characters[charNum].Name + "\" class=\"tabcontent\">";
				bool isplayerchar = characters[charNum].Chapter != "0";
				string PID = characters[charNum].PID;

				if (isplayerchar)
				{
					// affinity
					string affinity = CharacterData.ReadString(PID, "Affinity");
					if (affinity == "telius")
						affinity = "earth";

					outlogtext += "<h2><img src=\"assets/logpics/" + affinity + ".png\" alt=\"" + affinity +
						".png\" style=\"width: 32px; height: 32px; \">" + char.ToUpper(characters[charNum].Name[0]) +
						characters[charNum].Name.Substring(1) + "";

					// authority
					int auth = CharacterData.ReadInt(PID, "Authority");
					for (int i = 0; i < auth; i++)
					{
						outlogtext += "<img src=\"assets/logpics/star.png\" alt=\"star.png\" style=\"width: 18px; height: 18px;\">";
					}
					outlogtext += "</h2>";

					// biorhythm
					string bio = CharacterData.ReadString(PID, "Biorhythm");

					outlogtext += "<img src=\"assets/logpics/bio" + bio.ToString() + ".png\" alt=\"" +
						bio.ToString() + ".png\" style=\"width: 256px; height: 64px;\">";
				}

				// random recruit
				if ((cbxRandRecr.Checked | (characters[charNum].NewRecr != -1 & characters[charNum].NewRecr != charNum)) & 
					(isplayerchar | (cbxEnemyRecruit.Checked & charNum < 83) | cbxEnemyRecruit.Checked & cbxClassRand.Checked))
				{
					string recrname = characters[characters[charNum].NewRecr].Name;
					outlogtext += "<h4>Character</h4><img src=\"assets/logpics/" + recrname +
						".png\" alt=\"" + recrname + ".png\" style=\"width: 64px; height: 64px; \">";
				}

				if (isplayerchar)
				{
					if (characters[charNum].NewClass != -1)
						// class
						outlogtext += "<h4>Class</h4><p>" + classes[characters[charNum].NewClass].Name + "</p>";
				}

				if (isplayerchar)
				{
					// laguz gauge
					int[] laguzgauge = CharacterData.ReadIntArray(PID, "Laguz_Gauge");
					for (int j = 0; j < laguzgauge.Length; j++)
						if (laguzgauge[j] > 127)
							laguzgauge[j] -= 256;

					outlogtext += "<h4>Transformation Gauge</h4><table><tr><th>+/turn</th>" +
					"<th>+/battle</th><th>-/turn</th><th>-/battle</th></tr><tr>";

					for (int k = 0; k < 4; k++)
						outlogtext += "<td>" + laguzgauge[k].ToString() + "</td>";

					outlogtext += "</tr></table>";

					// bases
					int[] charbases = CharacterData.ReadIntArray(PID, "Bases");
					for (int j = 0; j < charbases.Length; j++)
						if (charbases[j] > 127)
							charbases[j] -= 256;
					outlogtext += "<h4>Personal Bases</h4><table><tr><th>HP</th><th>STR</th><th>MAG</th>" +
						"<th>SKL</th><th>SPD</th><th>LCK</th><th>DEF</th><th>RES</th></tr><tr>";

					for (int k = 0; k < 8; k++)
						outlogtext += "<td>" + charbases[k].ToString() + "</td>";

					outlogtext += "</tr></table>";

					// class bases
					string JID;
					JID = characters[charNum].JID;
					//if (classeschanged)
						//JID = classes[characters[charNum].NewClass].JID;
					//else
						//JID = classes[characters[charNum].VanillaClass].JID;
					outlogtext += "<h4>Class Bases</h4><table><tr><th>HP</th><th>STR</th><th>MAG</th>" +
						"<th>SKL</th><th>SPD</th><th>LCK</th><th>DEF</th><th>RES</th></tr><tr>";

					string[] classbases = ClassData.ReadStringArray(JID, "Bases");
					for (int k = 0; k < 8; k++)
						outlogtext += "<td>" + classbases[k] + "</td>";

					outlogtext += "</tr></table>";

					// growths
					string[] growths = CharacterData.ReadStringArray(PID, "Growths");
					outlogtext += "<h4>Growths</h4><table><tr><th>HP</th><th>STR</th><th>MAG</th>" +
						"<th>SKL</th><th>SPD</th><th>LCK</th><th>DEF</th><th>RES</th></tr><tr>";

					for (int k = 0; k < 8; k++)
						outlogtext += "<td>" + growths[k] + "</td>";

					outlogtext += "</tr></table>";

					// skills output
					outlogtext += "<h4>Skills</h4>";
					StreamReader readall = new StreamReader(file + "\\assets\\skillList.csv");
					string[] allskills = readall.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					readall.Close();
					string[] charskills = CharacterData.ReadStringArray(PID, "Skills");
					for (int k = 0; k < charskills.Length; k++)
					{
						if (charskills[k] != "")
						{
							for (int x = 0; x < allskills.Length; x++)
							{
								if (charskills[k] == allskills[x].Split(',')[1])
								{
									outlogtext += "<div class=\"img_wrap\"><img src=\"assets/logpics/" + allskills[x].Split(',')[0] + ".png\" alt=\"" +
									allskills[x].Split(',')[0] + ".png\" style=\"width: 64px; height: 64px; \"><p class=\"img_description\">" +
									allskills[x].Split(',')[0] + "</p></div>";
									break;
								}
							}
						}
					}
				}


				outlogtext += "</div>";



			}

			reader = new System.IO.StreamReader(file + "\\assets\\logscript.txt");
			outlogtext += reader.ReadToEnd() + "\n";
			reader.Close();


			if (cbxTowerUnits.Checked)
			{
				outlogtext += "<br><hr><br><h2 id=\"tower\">Tower Units</h2>";

				// loop twice, once for vanilla characters, once for randrecr
				for (int i = 0; i < 2; i++)
				{
					if (i == 0 & cbxRandRecr.Checked)
						outlogtext += "<h3>Vanilla Characters</h3>";
					else if (i == 1)
					{
						if (!cbxRandRecr.Checked)
							break;
						else
							outlogtext += "<br><h3>Random Recruitment Characters</h3>";
					}

					outlogtext += htmlSpoilerButton("towerunits" + i.ToString());
					outlogtext += "<div id=\"towerunits" + i.ToString() + "\" style=\"display:none\">";

					// standard required units - ike, micaiah, sothe, sanaki, kurthnaga, ena
					int[] towerInts = new int[16] { 34, 0, 5, 54, 63, 64, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

					int x = 0;
					for (int k = 4; k < 16; k++)
					{
						if (!cbxKurthEna.Checked & k < 6)
						{ }
						else
						{
							towerInts[k] = towerUnits[x];
							x++;
						}
					}

					for (int k = 0; k < 16; k++)
					{
						string towerName;
						if (i == 0)
							towerName = characters[towerInts[k]].Name;
						else
							towerName = characters[characters[towerInts[k]].NewRecr].Name;

						outlogtext += "<img src=\"assets/logpics/" + towerName + ".png\" alt=\"" + towerName + ".png\" style=\"width:64px;height:64px;\">";
					}

					outlogtext += "</div>";
				}
			}


			if (cbxRandWeap.Checked)
			{
				// read from file
				System.IO.StreamReader dataReader2 = new System.IO.StreamReader(file + "\\assets\\weaponlist.txt");
				string[] IIDs = dataReader2.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				dataReader2.Close();

				outlogtext += "<br><hr><br><h2 id=\"weapons\">Weapon Stats</h2>";

				outlogtext += htmlSpoilerButton("weaponsbtn");
				outlogtext += "<div id=\"weaponsbtn\" style=\"display:none\">";

				outlogtext += "<table><tr> <th>Name</th> <th>MT</th> <th>ACC</th> <th>CRT</th> <th>WT</th> <th>USE</th> </tr>";

				for (int i = 0; i < IIDs.Length; i++)
				{
					outlogtext += "<tr>";
					outlogtext += "<td>" + IIDs[i] + "</td>";
					outlogtext += "<td>" + ItemData.ReadString(IIDs[i], "MT") + "</td>";
					outlogtext += "<td>" + ItemData.ReadString(IIDs[i], "HIT") + "</td>";
					outlogtext += "<td>" + ItemData.ReadString(IIDs[i], "CRIT") + "</td>";
					outlogtext += "<td>" + ItemData.ReadString(IIDs[i], "WT") + "</td>";
					outlogtext += "<td>" + ItemData.ReadString(IIDs[i], "Uses") + "</td>";
					outlogtext += "</tr>";
				}
				outlogtext += "</table>";
				outlogtext += "</div>";
			}

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\JIDlist.txt");
			string[] JIDs = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			// class bases
			if (cbxRandClassBases.Checked | cbxShuffleClassBases.Checked | cbxRandMove.Checked)
			{
				outlogtext += "<br><hr><br><h2 id=\"class\">Class Bases</h2>";

				outlogtext += htmlSpoilerButton("classbases");
				outlogtext += "<div id=\"classbases\" style=\"display:none\">";

				outlogtext += "<table><tr> <th>Name</th> <th>HP</th> <th>STR</th> <th>MAG</th> <th>SKL</th> <th>SPD</th> " +
													"<th>LCK</th> <th>DEF</th> <th>RES</th> <th>MOV</th> </tr>";

				for (int i = 0; i < JIDs.Length; i++)
				{
					string[] classbases = ClassData.ReadStringArray(JIDs[i], "Bases");

					outlogtext += "<tr>";
					for (int j = 0; j < classes.Length; j++)
					{
						if (JIDs[i] == classes[j].JID)
						{
							outlogtext += "<td>" + classes[j].Name + "</td>";
							break;
						}
						else if (j == classes.Length - 1)
							outlogtext += "<td>" + JIDs[i] + "</td>";
					}
					for (int j = 0; j < classbases.Length; j++)
						outlogtext += "<td>" + classbases[j] + "</td>";
					outlogtext += "<td>" + ClassData.ReadString(JIDs[i], "MOV") + "</td>";
					outlogtext += "</tr>";
				}
				outlogtext += "</table>";
				outlogtext += "</div>";

			}
			// class stat caps
			if (cbxStatCaps.Checked | cbxStatCapDev.Checked | cbxStatCapFlat.Checked)
			{
				outlogtext += "<br><hr><br><h2 id=\"caps\">Class Stat Caps</h2>";

				outlogtext += htmlSpoilerButton("statcaps");
				outlogtext += "<div id=\"statcaps\" style=\"display:none\">";

				outlogtext += "<table><tr> <th>Name</th> <th>HP</th> <th>STR</th> <th>MAG</th> <th>SKL</th> <th>SPD</th> " +
													"<th>LCK</th> <th>DEF</th> <th>RES</th> </tr>";

				for (int i = 0; i < JIDs.Length; i++)
				{
					string[] caps = ClassData.ReadStringArray(JIDs[i], "Caps");

					outlogtext += "<tr>";
					for (int j = 0; j < classes.Length; j++)
					{
						if (JIDs[i] == classes[j].JID)
						{
							outlogtext += "<td>" + classes[j].Name + "</td>";
							break;
						}
						else if (j == classes.Length - 1)
							outlogtext += "<td>" + JIDs[i] + "</td>";
					}
					for (int j = 0; j < caps.Length; j++)
						outlogtext += "<td>" + caps[j] + "</td>";
					outlogtext += "</tr>";
				}
				outlogtext += "</table>";
				outlogtext += "</div>";

			}

			// stat boosters
			if (cbxStatBooster.Checked)
			{
				string[] statboosters = new string[8] { "IID_ANGELROBE", "IID_ENERGYDROP", "IID_SPIRITPOWDER", "IID_SECRETBOOK",
														"IID_SPEEDWING", "IID_GODDESSICON", "IID_DRAGONSHIELD", "IID_TALISMAN" };

				outlogtext += "<br><hr><br><h2 id=\"boost\">Stat Boosters</h2>";

				outlogtext += htmlSpoilerButton("statboosterbtn");
				outlogtext += "<div id=\"statboosterbtn\" style=\"display:none\">";

				outlogtext += "<table><tr> <th>Item</th> <th>HP</th> <th>STR</th> <th>MAG</th> <th>SKL</th> <th>SPD</th> " +
													"<th>LCK</th> <th>DEF</th> <th>RES</th> </tr>";

				for (int i = 0; i < statboosters.Length; i++)
				{
					int[] bonuses = ItemData.ReadIntArray(statboosters[i], "Bonuses");

					outlogtext += "<tr>";
					outlogtext += "<td>" + statboosters[i].Remove(0, 4) + "</td>";
					for (int j = 0; j < 8; j++)
					{
						if (bonuses[j] > 127)
							bonuses[j] -= 256;
						outlogtext += "<td>" + bonuses[j].ToString() + "</td>";
					}
					outlogtext += "</tr>";
				}
				outlogtext += "</table>";
				outlogtext += "</div>";
			}

			// skill capacities
			if (cbxSkillCap.Checked)
			{
				// read from file
				System.IO.StreamReader reader2 = new System.IO.StreamReader(file + "\\assets\\skillcaplist.txt");
				string[] skilllist = reader2.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				reader2.Close();

				outlogtext += "<br><hr><br><h2 id=\"skillcap\">Skill Capacities</h2>";

				outlogtext += htmlSpoilerButton("skillcapbtn");
				outlogtext += "<div id=\"skillcapbtn\" style=\"display:none\">";

				outlogtext += "<table><tr> <th>Skill Name</th> <th>Capacity</th> </tr>";

				for (int i = 0; i < skilllist.Length; i++)
				{
					outlogtext += "<tr>";
					outlogtext += "<td>" + skilllist[i].Remove(0, 4) + "</td>";
					outlogtext += "<td>" + SkillData.ReadString(skilllist[i], "Capacity") + "</td>";
					outlogtext += "</tr>";
				}
				outlogtext += "</table>";
				outlogtext += "</div>";
			}

			// event items
			if (cbxEventItems.Checked)
			{
				string outputhis = parseOutputString(eventItemsOutput);
				outputhis += "</table>";

				outlogtext += "<br><hr><br><h2 id=\"event\">Event Items</h2>";

				outlogtext += htmlSpoilerButton("eventitems");
				outlogtext += "<div id=\"eventitems\" style=\"display:none\">";

				outlogtext += "<table><tr> <th>Chapter</th> <th>Old Item</th> <th>New Item</th> </tr>";

				outlogtext += outputhis;
				outlogtext += "</div>";
			}

			// header for shop pointer
			if (cbxBargains.Checked | cbxForge.Checked)
			{
				outlogtext += "<br><hr><br><h2 id=\"shop\">";
				if (cbxBargains.Checked)
					outlogtext += "Bargains</h2>";
				else
					outlogtext += "Forge</h2>";
			}

			// bargain output
			if(cbxBargains.Checked)
			{
				outlogtext += htmlSpoilerButton("bargains");
				outlogtext += "<div id=\"bargains\" style=\"display:none\">";

				outlogtext += "<table><tr> <th>Chapter</th> <th>Items</th></tr>";
				string outputhis = parseOutputString(bargainOutput);
				outputhis += "</table>";
				outlogtext += outputhis;
				outlogtext += "</div>";
			}

			// forge
			if (cbxForge.Checked)
			{
				if (cbxBargains.Checked)
					outlogtext += "<br><hr><br><h2>Forge</h2>";

				outlogtext += htmlSpoilerButton("forge");
				outlogtext += "<div id=\"forge\" style=\"display:none\">";

				outlogtext += "<table><tr> <th>Chapter</th> <th>Items</th></tr>";
				string outputhis = parseOutputString(forgeOutput);
				outputhis += "</table>";
				outlogtext += outputhis;
				outlogtext += "</div>";
			}



			if (cbxRandEnemy.Checked | cbxEnemBonusDrop.Checked)
			{
				outlogtext += "<br><hr><br><h2 id=\"enemy\">Enemies</h2>";

				outlogtext += htmlSpoilerButton("enemies");
				outlogtext += "<div id=\"enemies\" style=\"display:none\">";

				outlogtext += randEnemyOutput;
				outlogtext += "</div>";
			}

			if (cbxRandPromotion.Checked)
			{
				outlogtext += "<br><hr><br><h2 id=\"promo\">Promotion Lines</h2>";

				outlogtext += htmlSpoilerButton("bargains");
				outlogtext += "<div id=\"bargains\" style=\"display:none\">" + randPromoOutput;
				outlogtext += "</div>";
			}


			outlogtext += "</body></html>";

			logwriter.WriteLine(outlogtext);
			logwriter.Close();

		}

		#endregion




		// OTHER FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

		// gets array of all files in all folders in a given directory
		public static string[] getRecursiveFiles(string directorypath)
		{
			List<string> allfiles = new List<string>();
			List<string> directories = new List<string>();
			directories.Add(directorypath);

			while (directories.Count > 0)
			{
				string[] getfiles = Directory.GetFiles(directories[0]);
				for (int i = 0; i < getfiles.Length; i++)
					allfiles.Add(getfiles[i]);
				string[] getpaths = Directory.GetDirectories(directories[0]);
				for (int i = 0; i < getpaths.Length; i++)
					directories.Add(getpaths[i]);
				directories.RemoveAt(0);
			}

			return (allfiles.ToArray());
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

		// creates array with random values that add up to given total - used for shuffling
		private int[] shuffler(int total, int number)
		{
			int deviation = total / 10;
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

		private string htmlSpoilerButton(string buttonname)
		{
			string spoilerbutton = "<button title=\"Click to Show/Hide Content\" type=\"button\" onclick=\"if(document.getElementById('";
			spoilerbutton += buttonname;
			spoilerbutton += "').style.display=='none') {document.getElementById('";
			spoilerbutton += buttonname;
			spoilerbutton += "').style.display=''}else{document.getElementById('";
			spoilerbutton += buttonname; 
			spoilerbutton += "').style.display='none'}\">Show/Hide</button>";

			return (spoilerbutton);
		}

		// splits an output string that is delimited by commas and semicolons and creates rows in an html table
		private string parseOutputString(string outputstring)
		{
			string outputthis = "";
			string[] entries = outputstring.Split(';');
			for (int i = 0; i < entries.Length; i++)
			{
				if (entries[i] != "" & !entries[i].StartsWith("C00"))
				{
					string[] parts = entries[i].Split(',');
					if (parts.Length > 1)
					{
						outputthis += "<tr>";
						for (int j = 0; j < parts.Length; j++)
						{
							if (j == 0)
							{
								int game_part = Convert.ToInt32(parts[j][2].ToString());
								int game_chapter = Convert.ToInt32(parts[j].Substring(3, 2));
								string partchapter;
								if (game_chapter == 1)
									partchapter = game_part.ToString() + "-P";
								else if ((game_part == 1 & game_chapter == 11) |
										 (game_part == 2 & game_chapter == 5) |
										 (game_part == 3 & game_chapter == 15))
									partchapter = game_part.ToString() + "-E";
								else if (game_part == 4 & game_chapter == 7)
									partchapter = "4-T";
								else
									partchapter = game_part.ToString() + "-" + (game_chapter - 1).ToString();
								parts[j] = partchapter;
							}
							outputthis += "<td>" + parts[j] + "</td>";
						}
						outputthis += "</tr>";
					}
				}
			}

			return (outputthis);
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
		#region
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

		private void cbxClassRand_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxClassRand.Checked)
				cbxClassSwap.Checked = false;

			cbxRecrVanillaClass.Enabled = !(cbxClassRand.Checked | cbxClassSwap.Checked);
			if (cbxClassSwap.Checked | cbxClassRand.Checked)
				cbxRecrVanillaClass.Checked = false;

			panelClass.Enabled = cbxClassRand.Checked | cbxClassSwap.Checked;
			panelClass2.Enabled = cbxClassRand.Checked | cbxClassSwap.Checked;
			if (cbxRandRecr.Checked == false)
				cbxStoryPromo.Enabled = !cbxClassRand.Checked;

			if (cbxEnemyRecruit.Checked == true)
			{
				if (cbxClassRand.Checked == true)
				{
					comboIke.Items.Add("blackknight");
					comboIke.Items.Add("ashera");
					comboMicc.Items.Add("blackknight");
					comboMicc.Items.Add("ashera");
					comboElincia.Items.Add("blackknight");
					comboElincia.Items.Add("ashera");
				}
				else
				{
					if (comboIke.SelectedIndex >= 84)
						comboIke.SelectedIndex = 34;
					comboIke.Items.RemoveAt(84);
					comboIke.Items.RemoveAt(84);
					if (comboMicc.SelectedIndex >= 84)
						comboMicc.SelectedIndex = 0;
					comboMicc.Items.RemoveAt(84);
					comboMicc.Items.RemoveAt(84);
					if (comboElincia.SelectedIndex >= 84)
						comboElincia.SelectedIndex = 18;
					comboElincia.Items.RemoveAt(84);
					comboElincia.Items.RemoveAt(84);
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
			}
			else
			{
				cbxNoLaguz.Enabled = false;
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

			if (cbxEnemyRecruit.Checked & cbxRandRecr.Checked)
			{
				string[] enemies = { "jarod","ludveck","septimus","valtome","numida","izuka","hetzel",
									"levail","lekain","zelgius","dheginsea","sephiram"};

				for (int i = 0; i < enemies.Length; i++)
				{
					comboIke.Items.Add(enemies[i]);
					comboMicc.Items.Add(enemies[i]);
					comboElincia.Items.Add(enemies[i]);
				}
				if (cbxClassRand.Checked == true)
				{
					comboIke.Items.Add("blackknight");
					comboIke.Items.Add("ashera");
					comboMicc.Items.Add("blackknight");
					comboMicc.Items.Add("ashera");
					comboElincia.Items.Add("blackknight");
					comboElincia.Items.Add("ashera");
				}
			}
			else
			{
				if (comboIke.SelectedIndex >= 72)
					comboIke.SelectedIndex = 34;
				if (comboMicc.SelectedIndex >= 72)
					comboMicc.SelectedIndex = 0;
				if (comboElincia.SelectedIndex >= 72)
					comboElincia.SelectedIndex = 18;

				

				for (int i = 0; i < 12; i++)
				{
					if (comboIke.Items.Count > 72)
					{
						comboIke.Items.RemoveAt(72);
						comboMicc.Items.RemoveAt(72);
						comboElincia.Items.RemoveAt(72);
					}
				}
				if (cbxClassRand.Checked == true)
				{
					if (comboIke.Items.Count > 72)
					{
						comboIke.Items.RemoveAt(72);
						comboIke.Items.RemoveAt(72);
						comboMicc.Items.RemoveAt(72);
						comboMicc.Items.RemoveAt(72);
						comboElincia.Items.RemoveAt(72);
						comboElincia.Items.RemoveAt(72);
					}
				}
			}


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
			panelEnemy.Enabled = cbxRandEnemy.Checked | cbxRandAllies.Checked;
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
			if (cbxEnemyRecruit.Checked & cbxRandRecr.Checked)
			{
				string[] enemies = { "jarod","ludveck","septimus","valtome","numida","izuka","hetzel",
									"levail","lekain","zelgius","dheginsea","sephiram"};

				for (int i = 0; i < enemies.Length; i++)
				{
					comboIke.Items.Add(enemies[i]);
					comboMicc.Items.Add(enemies[i]);
					comboElincia.Items.Add(enemies[i]);
				}
				if (cbxClassRand.Checked == true)
				{
					comboIke.Items.Add("blackknight");
					comboIke.Items.Add("ashera");
					comboMicc.Items.Add("blackknight");
					comboMicc.Items.Add("ashera");
					comboElincia.Items.Add("blackknight");
					comboElincia.Items.Add("ashera");
				}
			}
			else
			{
				if (comboIke.SelectedIndex >= 72)
					comboIke.SelectedIndex = 34;
				if (comboMicc.SelectedIndex >= 72)
					comboMicc.SelectedIndex = 0;
				if (comboElincia.SelectedIndex >= 72)
					comboElincia.SelectedIndex = 18;

				for (int i = 0; i < 12; i++)
				{
					if (comboIke.Items.Count > 72)
						comboIke.Items.RemoveAt(72);
					if (comboMicc.Items.Count > 72)
						comboMicc.Items.RemoveAt(72);
					if (comboElincia.Items.Count > 72)
						comboElincia.Items.RemoveAt(72);
				}
				if (cbxClassRand.Checked == true)
				{
					if (comboIke.Items.Count > 72)
					{
						comboIke.Items.RemoveAt(72);
						comboIke.Items.RemoveAt(72);
					}
					if (comboMicc.Items.Count > 72)
					{
						comboMicc.Items.RemoveAt(72);
						comboMicc.Items.RemoveAt(72);
					}
					if (comboElincia.Items.Count > 72)
					{
						comboElincia.Items.RemoveAt(72);
						comboElincia.Items.RemoveAt(72);
					}
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
			//cbxWhiteGem.Enabled = cbxEventItems.Checked;
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
			cbxSkillVanilla.Enabled = cbxSkillRand.Checked;
			cbxSkillSetNum.Enabled = cbxSkillRand.Checked;
		}

		private void cbxSkillMax_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxSkillVanilla.Checked)
				cbxSkillSetNum.Checked = false;
			else if (!cbxSkillSetNum.Checked)
				cbxSkillSetNum.Checked = true;
		}

		private void cbxSkillUno_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxSkillSetNum.Checked)
				cbxSkillVanilla.Checked = false;
			else if (!cbxSkillVanilla.Checked)
				cbxSkillVanilla.Checked = true;
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

		private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void viewReadMeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(@file + "\\documentation - Please read!\\README.htm");
		}

		private void discordServerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://discord.gg/dKfhvFj");
		}

		private void cbxUniversalSkills_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxUniversalSkills.Checked)
				cbxDragonSkills.Checked = false;
		}

		private void cbxDragonSkills_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxDragonSkills.Checked)
				cbxUniversalSkills.Checked = false;
		}

		private void cbxEnemDrops_CheckedChanged(object sender, EventArgs e)
		{
			//if (cbxEnemDrops.Checked)
			//	cbxEnemBonusDrop.Checked = false;
		}

		private void cbxEnemBonusDrop_CheckedChanged(object sender, EventArgs e)
		{
			//if (cbxEnemBonusDrop.Checked | cbxBossBonusDrop.Checked)
				//cbxEnemDrops.Checked = false;
		}

		private void cbxRecrVanillaClass_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void cbxBio_CheckedChanged(object sender, EventArgs e)
		{

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
			comboClassOptions.SelectedIndex = 1;

			if (cbxClassSwap.Checked)
				cbxClassRand.Checked = false;

			panelClass.Enabled = cbxClassRand.Checked;

			panelClass2.Enabled = cbxClassSwap.Checked | cbxClassRand.Checked;
			if (cbxRandRecr.Checked == false)
				cbxStoryPromo.Enabled = !cbxClassSwap.Checked;

			if (cbxEnemyRecruit.Checked == true)
			{
				if (cbxClassSwap.Checked == true)
				{
					comboIke.Items.Add("blackknight");
					comboIke.Items.Add("ashera");
					comboMicc.Items.Add("blackknight");
					comboMicc.Items.Add("ashera");
					comboElincia.Items.Add("blackknight");
					comboElincia.Items.Add("ashera");
				}
				else
				{
					if (comboIke.SelectedIndex >= 84)
						comboIke.SelectedIndex = 34;
					comboIke.Items.RemoveAt(84);
					comboIke.Items.RemoveAt(84);
					if (comboMicc.SelectedIndex >= 84)
						comboMicc.SelectedIndex = 0;
					comboMicc.Items.RemoveAt(84);
					comboMicc.Items.RemoveAt(84);
					if (comboElincia.SelectedIndex >= 84)
						comboElincia.SelectedIndex = 18;
					comboElincia.Items.RemoveAt(84);
					comboElincia.Items.RemoveAt(84);
				}
			}

			ikeMicClassCombos();
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
		#endregion
	}

	

	public class Character
	{
		// normal data
		public string Name, PID, Chapter, Tier, Race, LockedSkills, PhysMag, JID;
		public int Level, SkillNum, WeaponNum, VanillaClass, FIDLoc;

		// new data (randomized class, recruitment, etc)
		public string NewRace, RecrRace, NewName;
		public int NewClass, NewRecr, RecrInverse;
		public int[] RecrClasses;

		public Character(string CharacterInfoLine)
		{
			string[] split = CharacterInfoLine.Split(',');
			Name = split[0];
			PID = split[1];
			Chapter = split[2];
			Tier = split[3];
			Level = Convert.ToInt32(split[4]);
			Race = split[5];
			LockedSkills = split[6];
			SkillNum = Convert.ToInt32(split[7]);
			WeaponNum = Convert.ToInt32(split[8]);
			FIDLoc = Convert.ToInt32(split[9]);
			PhysMag = split[10];
			VanillaClass = Convert.ToInt32(split[11]);
			JID = split[12];

			NewRecr = -1;
			NewClass = -1;
			RecrInverse = -1;
			NewRace = Race;
			NewName = Name;
		}
	}

	public class Job
	{
		public string Name, JID, Tier_P, Weapon_P, Classtype_P, Race, Animation, Tier_E, Weapon_E, Classtype_E;
		// randomized promotions
		public string PromoPath;

		public Job(string ClassInfoLine)
		{
			string[] split = ClassInfoLine.Split(',');
			Name = split[1];
			JID = split[2];
			Tier_P = split[3];
			Weapon_P = split[4];
			Classtype_P = split[5];
			Race = split[6];
			Animation = split[7];
			Tier_E = split[8];
			Weapon_E = split[9];
			Classtype_E = split[10];
		}
	}

}

