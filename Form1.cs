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

		// error flag
		int errorflag = 0;

		// number of units to change
		int totalUnitNumber;

		// number of units changed to heron
		int heronNumber;

		string chapterFile;
		System.IO.StreamWriter dataWriter;

		// create random number generator
		Random seedGenerator = new Random();
		Random random;

		// keeps track of current character
		int charNum;

		// keeps track of changes made to characters
		string[] charChanges = new string[72];

		// arrays that hold character data; explained in more detail around line 65
		string[] charName = new string[72]; // nice
		string[] charChapter = new string[72];
		string[] charTier = new string[72];
		int[] charLevel = new int[72];
		string[] charRace = new string[72];
		int[] charLocation = new int[72];
		int[] charAnimation = new int[72];
		int[] charSkill = new int[72];
		int[] charSkillNum = new int[72];
		int[] charGauge = new int[72];
		int[] charGrowth = new int[72];
		int[] charBases = new int[72];
		int[] charWeapNum = new int[72];
		int[] charLevLoc = new int[72];
		int[] charPID = new int[72];
		int[] charBio = new int[72];

		// arrays that hold new character data
		string[] newRace = new string[72];
		int[] newClass = new int[72];
		int[] newRecr = new int[69];
		int[] recrInverse = new int[69];

		public Form1()
		{
			string line;
			string[] values;
			// initialize character information
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\CharacterInfo.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all 72 characters, black knight excluded
			for (int i = 0; i < 72; i++)
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
			}
			InitializeComponent();

			//toolTip1.SetToolTip(cbxGrowthRand, "test1");
			//toolTip1.SetToolTip(cbxClassRand, "test2");

			comboClassOptions.SelectedIndex = 0;
			numericSeed.Value = seedGenerator.Next();
		}


		private void button1_Click(object sender, EventArgs e)
		{
			textBox1.Text = "Randomization in Process"; // Insert loading br here

			// heron number
			heronNumber = 0;

			// reset error flag
			errorflag = 0;

			// disable components so user can't change properties during randomization
			Form1.ActiveForm.Enabled = false;

			// set number of units to change
			if (cbxClassRand.Checked == true & cbxHerons.Checked == true)
				totalUnitNumber = 72;
			else
				totalUnitNumber = 69;

			// generate randomizer with seed
			random = new Random(Convert.ToInt32(numericSeed.Value));

			// initialize output log
			string logheader = "seed:," + numericSeed.Value.ToString();
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
				if (cbxHerons.Checked == true)
					logheader += ",heronRand";
				if (cbxGaugeRand.Checked == true)
					logheader += ",transGauge Min=" + numericLaguzMin.Value.ToString() + " Max=" + numericLaguzMax.Value.ToString();
			}

			if (cbxGrowthRand.Checked == true)
			{
				logheader += ",growthRand Dev=" + numericGrowth.Value.ToString();
				logheader += ",hpMin=" + numericHP.Value.ToString();
				logheader += ",atkMin=" + numericATK.Value.ToString();
				logheader += ",magMin=" + numericMAG.Value.ToString();
				logheader += ",sklMin=" + numericSKL.Value.ToString();
				logheader += ",spdMin=" + numericSPD.Value.ToString();
				logheader += ",lckMin=" + numericLCK.Value.ToString();
				logheader += ",defMin=" + numericDEF.Value.ToString();
				logheader += ",resMin=" + numericRES.Value.ToString();
			}

			if (cbxRandBases.Checked == true)
				logheader += ",randBaseStats-MaxDev=" + numericBaseRand.Value.ToString();
			if (cbxEnemyGrowth.Checked == true)
				logheader += ",enemyGrowth-MaxIncrease=" + numericEnemyGrowth.Value.ToString();
			if (cbxRandShop.Checked == true)
				logheader += ",randShop";
			if (cbxAffinity.Checked == true)
				logheader += ",randAffinity";
			if (cbxBio.Checked == true)
				logheader += ",randBio";
			if (cbxSkillRand.Checked == true)
				logheader += ",randSkill";
			if (cbxEventItems.Checked == true)
				logheader += ",randEventItems";
			if (cbxRandRecr.Checked == true)
				logheader += ",randRecruitment";
			if (cbxZeroGrowths.Checked == true)
				logheader += ",zeroGrowth";
			if (cbxStatCaps.Checked == true)
			{
				logheader += ",statCaps:";
				logheader += " T1=" + numericStatCap1.Value.ToString();
				logheader += " T2=" + numericStatCap2.Value.ToString();
				logheader += " T3=" + numericStatCap3.Value.ToString();
			}

			try
			{
				dataWriter = new System.IO.StreamWriter(file + "\\outputlog.csv");
				dataWriter.WriteLine(logheader);

				dataWriter.WriteLine("Name,NewName,Race,Class,LGaugeTurn,LGaugeBattle,LGaugeTurn,LGaugeBattle," +
					"Skills1,Skills2,Skills3,Skills4,Growths:HP,STR,MAG,SKL,SPD,LCK,DEF,RES");
			}
			catch
			{
				textBox1.Text = "outputlog.csv currently open in another program! Abandoning Randomization...";
				errorflag = 1;
			}

			// record names of characters
			for (int i = 0; i < totalUnitNumber; i++)
				charChanges[i] = charName[i];

			// move some edited files
			fileOrganizer();

			// select order of random recruitment
			if (cbxRandRecr.Checked == true & errorflag == 0)
			{
				recruitmentOrderRando();
				// record new names of characters
				for (int i = 0; i < totalUnitNumber; i++)
					charChanges[i] += "," + charName[i];
			}
			else
			{
				// record no changes for recruitment
				for (int i = 0; i < totalUnitNumber; i++)
					charChanges[i] += ",-";
			}


			// modify character classes/weapons
			if ((cbxClassRand.Checked == true | cbxRandRecr.Checked == true) & errorflag == 0)
			{
				
				// randomize class for each character
				for (charNum = 0; charNum < totalUnitNumber; charNum++)
				{
					classChanger();
				}

				// change laguz gauges for new classes
				laguzModifications();
				// changes that prevent bugs due to race-changing
				generalChanges();

				if (cbxClassRand.Checked == true)
				{
					// change animations to new randomized classes
					animationChanger();
					// base stat modifications
					baseStatChanges();
					// uhh yes very important
					if (comboClassOptions.SelectedIndex == 10)
						veryImportantFunction();
				}
			}
			else
			{
				// record no changes for race,class,and laguz gauge
				for (int i = 0; i < totalUnitNumber; i++)
					charChanges[i] += ",-,-,-,-,-,-";
			}

			if (cbxSkillRand.Checked == true & errorflag == 0)
			{
				skillRandomizer();
			}
			else
			{
				// record no changes for skills
				for (int i = 0; i < totalUnitNumber; i++)
					charChanges[i] += ",-,-,-,-";
			}

			if (((cbxGrowthRand.Checked == true & Convert.ToInt32(numericGrowth.Value) != 0) | cbxZeroGrowths.Checked == true) & errorflag == 0)
			{
				growthRateModifier();
			}
			else
			{
				// record no changes for growths
				for (int i = 0; i < totalUnitNumber; i++)
					charChanges[i] += ",-,-,-,-,-,-,-,-";
			}

			if (cbxRandBases.Checked == true & errorflag == 0)
			{
				randBaseStats();
			}

			if (cbxAffinity.Checked == true & errorflag == 0)
			{
				affinityRandomizer();
			}

			if (cbxEnemyGrowth.Checked == true & errorflag == 0)
			{
				enemyGrowthModifier();
			}

			if (cbxRandShop.Checked == true & errorflag == 0)
			{
				shopRandomizer();
			}

			if (cbxStatCaps.Checked == true & errorflag == 0)
			{
				removeStatCaps();
			}

			if (cbxBio.Checked == true & errorflag == 0)
			{
				bioRandomizer();
			}

			if (cbxEventItems.Checked == true & errorflag == 0)
			{
				eventItemRandomizer();
			}

			if (errorflag == 0)
			{
				outputLog();
				textBox1.Text = "Randomization Complete! Check outputlog.csv for details";
			}

			dataWriter.Close();

			Form1.ActiveForm.Enabled = true;

		}

		private void fileOrganizer()
		{
			if (cbxClassRand.Checked == true | cbxRandRecr.Checked == true)
			{
				// moves edited dispos_h files to proper folders
				string sourcePath = file + "\\assets\\chapterdata\\";
				string targetPath = dataLocation + "\\zmap\\";
				string sourcefile, targetfile;

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
					sourcefile = sourcePath + "dispos_h4_7c.bin";
					targetfile = targetPath + "bmap0407c\\dispos_h.bin";
					System.IO.File.Copy(sourcefile, targetfile, true);
					sourcefile = sourcePath + "dispos_h4_7e.bin";
					targetfile = targetPath + "bmap0407e\\dispos_h.bin";
					System.IO.File.Copy(sourcefile, targetfile, true);

					sourcefile = file + "\\assets\\scriptdata\\C0405.cmb";
					targetfile = dataLocation + "\\Scripts\\C0405.cmb";
					System.IO.File.Copy(sourcefile, targetfile, true);
				}
			}

			if (cbxEventItems.Checked == true)
			{
				// moves edited script files to proper folders
				string sourcePath = file + "\\assets\\scriptdata\\";
				string targetPath = dataLocation + "\\Scripts\\";
				string sourcefile, targetfile;

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

			if (cbxRandShop.Checked == true)
			{
				// moves edited shop files to proper folders
				string sourcePath = file + "\\assets\\shopdata\\";
				string targetPath = dataLocation + "\\Shop\\";
				string sourcefile, targetfile;

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
			}
		}

		private void recruitmentOrderRando()
		{
			string line;
			string[] values;

			string[] recrName = new string[69];
			int[] recrLoc = new int[69];
			int[] recrLoc2 = new int[69];
			int[,] recrClass = new int[69, 4];
			int[] recrLevel = new int[69];
			int[,] recrPID = new int[69, 4];
			int[,] recrBases = new int[69, 8];

			System.IO.StreamReader dataReader;
			// initialize character information
			//if (cbxClassRand.Checked == true)
				dataReader = new System.IO.StreamReader(file + "\\assets\\RecruitmentInfo-BaseModified.csv");
			//else
			//	dataReader = new System.IO.StreamReader(file + "\\assets\\RecruitmentInfo-Clean.csv");

			// skip header line
			line = dataReader.ReadLine();
			// loop through all 69
			for (int i = 0; i < 69; i++)
			{
				line = dataReader.ReadLine();
				values = line.Split(',');
				// character name
				recrName[i] = values[0];
				// location of beginning of character data
				recrLoc[i] = Convert.ToInt32(values[1]);
				// location of end of character data
				recrLoc2[i] = Convert.ToInt32(values[2]);
				// classes for character
				recrClass[i,0] = Convert.ToInt32(values[4]);
				// tier b class for character
				recrClass[i,1] = Convert.ToInt32(values[5]);
				// tier c class for character
				recrClass[i,2] = Convert.ToInt32(values[6]);
				// tier d class for character
				recrClass[i,3] = Convert.ToInt32(values[7]);
				// initial level of character
				recrLevel[i] = Convert.ToInt32(values[8]);
				// PID pointer
				recrPID[i,0] = Convert.ToInt32(values[9]);
				recrPID[i,1] = Convert.ToInt32(values[10]);
				recrPID[i,2] = Convert.ToInt32(values[11]);
				recrPID[i,3] = Convert.ToInt32(values[12]);
				// new bases
				recrBases[i,0] = Convert.ToInt32(values[13]);
				recrBases[i,1] = Convert.ToInt32(values[14]);
				recrBases[i,2] = Convert.ToInt32(values[15]);
				recrBases[i,3] = Convert.ToInt32(values[16]);
				recrBases[i,4] = Convert.ToInt32(values[17]);
				recrBases[i,5] = Convert.ToInt32(values[18]);
				recrBases[i,6] = Convert.ToInt32(values[19]);
				recrBases[i,7] = Convert.ToInt32(values[20]);
			}

			newRecr = Enumerable.Range(0, 69).ToArray();

			// randomize recruitment order
			for (int i = 0; i < 69; i++)
			{
				int j = random.Next(i, 69);
				recrInverse[newRecr[i]] = j;
				recrInverse[newRecr[j]] = i;
				int temp = newRecr[i];
				newRecr[i] = newRecr[j];
				newRecr[j] = temp;
			}

			// go into data file and move things around
			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";

			try
			{
				// open data file
				using (var stream = new System.IO.FileStream(dataFile, System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
				{
					for (int charNum = 0; charNum < 69; charNum++)
					{
						// level
						stream.Position = recrLoc[newRecr[charNum]] - 2;		
						stream.WriteByte(Convert.ToByte(recrLevel[charNum]));
						// pid
						stream.Position += 1;
						for (int j = 0; j < 4; j++)
						{
							stream.WriteByte(Convert.ToByte(recrPID[charNum,j]));
						}
						// bases
						stream.Position = recrLoc2[newRecr[charNum]] - 23;
						for (int j = 0; j < 8; j++)
						{
							stream.WriteByte(Convert.ToByte(recrBases[charNum, j]));
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
			for (int charNum = 0; charNum < 69; charNum++)
			{
				if (charTier[charNum] == "a")
					newClass[charNum] = recrClass[newRecr[charNum],0];
				else if (charTier[charNum] == "b")
					newClass[charNum] = recrClass[newRecr[charNum], 1];
				else if (charTier[charNum] == "c")
					newClass[charNum] = recrClass[newRecr[charNum], 2];
				else 
					newClass[charNum] = recrClass[newRecr[charNum], 3];

				charName[charNum] = recrName[newRecr[charNum]];
				charPID[charNum] = recrLoc[newRecr[charNum]];
			}

		}

		private void classChanger()
		{
			byte[] classname = new byte[20];
			byte[] weaponone = new byte[20];
			byte[] weapontwo = new byte[20];
			byte[] weaponthree = new byte[20];

			// units given before endgame are stored in a special place
			if (charChapter[charNum] == "407")
				chapterFile = dataLocation + "\\zmap\\emap0407c\\dispos_c.bin";
			else
				chapterFile = dataLocation + "\\zmap\\bmap0" + charChapter[charNum] + "\\dispos_h.bin";

			int randClass = 0;
			int classPointerOffset, classListOffset;
			int newLevel = charLevel[charNum];

			// if only rand recruitment, use those classes
			if (cbxClassRand.Checked == false & charNum < 69)
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
					heronNumber += 1;
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

				newClass[charNum] = classPointerOffset + 30;

				if (randClass == 29)
					heronNumber += 1;
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

				if (randClass == 33)
					heronNumber += 1;
			}

			classListOffset += classPointerOffset * 20;

			try
			{
				// open classlist.bin
				using (var stream = new System.IO.FileStream(file + "\\assets\\classlist.bin", System.IO.FileMode.Open,
						System.IO.FileAccess.Read))
				{
					stream.Position = classListOffset;
					for (int i = 0; i < 20; i++)
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
						for (int i = 0; i < 20; i++)
							weaponone[i] = Convert.ToByte(stream.ReadByte());
					}

					if (charWeapNum[charNum] > 1)
					{
						stream.Position = weapontwoOffset;
						for (int i = 0; i < 20; i++)
							weapontwo[i] = Convert.ToByte(stream.ReadByte());
					}

					if (charWeapNum[charNum] > 2)
					{
						stream.Position = weaponthreeOffset;
						for (int i = 0; i < 20; i++)
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
					for (int i = 0; i < 20; i++)
						stream.WriteByte(classname[i]);

					// write inventory
					if (charWeapNum[charNum] > 0 & // only change inventory if there's an inventory to change
						(charName[charNum] != "nasir" | newRace[charNum] == "B")) // only change nasir's inventory if he's beorc
					{
						// write first weapon
						for (int i = 0; i < 20; i++)
							stream.WriteByte(weaponone[i]);

						if (charWeapNum[charNum] > 1) // only change inventory if there's an inventory to change
						{
							// write second weapon
							for (int i = 0; i < 20; i++)
								stream.WriteByte(weapontwo[i]);

							if (charWeapNum[charNum] == 3) //third weapon if necessary
							{
								// write third weapon
								for (int i = 0; i < 20; i++)
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
						stream.Position = 11260;
						// write class
						for (int i = 0; i < 20; i++)
							stream.WriteByte(weaponone[i]);

						for (int i = 0; i < 20; i++)
							stream.WriteByte(weapontwo[i]);
					}
				}
				else if (charName[charNum] == "vika")
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0405.cmb", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
					{
						// change position to location of character in chapterFile
						stream.Position = 11300;
						// write class
						for (int i = 0; i < 20; i++)
							stream.WriteByte(weaponone[i]);
					}
				}
				else if (charName[charNum] == "maurim")
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0405.cmb", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
					{
						// change position to location of character in chapterFile
						stream.Position = 11320;
						// write class
						for (int i = 0; i < 20; i++)
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
			// save new race and class for outputlog
			charChanges[charNum] += "," + newRace[charNum] + "," + classList[newClass[charNum]];
		}

		private int chooseRandClass()
		{
			int randClass;
			// generate a random class
			if (comboClassOptions.SelectedIndex == 0) // no race-mixing
			{
				if (charTier[charNum] == "a")
				{
					if (charRace[charNum] == "B")
					{
						// no race-mixing gives 19 beorc classes
						randClass = random.Next(19);
						while (charName[charNum] == "edward" & newClass[0] == 18 & randClass == 18) //if micaiah is priest,do not allow edward to be priest
							randClass = random.Next(19);
					}
					else
					{
						// no race-mixing gives 7 non-royal, non-dragon laguz classes
						if (cbxHerons.Checked == true & heronNumber < 3)
							randClass = random.Next(19, 26);
						else // no heron class
							randClass = random.Next(19, 25);
					}
				}
				else if (charTier[charNum] == "b")
				{
					if (charRace[charNum] == "B")
					{
						if (charName[charNum] == "ike")
						{
							// ike cannot be mounted (ch3-5), rogue (cant kill bk), magic (cant kill ashera)
							int[] classes = { 0, 1, 2, 5, 6, 8, 9, 10, 12, 13 };
							randClass = classes[random.Next(10)];
						}
						else
						{
							// no race-mixing gives 23 beorc classes
							randClass = random.Next(23);
						}
					}
					else
					{
						// no race-mixing gives 10 non-royal laguz classes
						randClass = random.Next(23, 33);
						while ((cbxHerons.Checked == false | heronNumber == 3) & randClass == 29) // keep randomizing until no heron
							randClass = random.Next(23, 33);
					}
				}
				else if (charTier[charNum] == "c")
				{
					if (charRace[charNum] == "B")
					{
						// no race-mixing gives 27 beorc classes
						randClass = random.Next(27);
					}
					else
					{
						// no race-mixing gives 5 royal laguz classes
						randClass = random.Next(27, 32);
					}
				}
				else // kurth, ena, giffca, gareth, nasir
				{
					// no race-mixing gives 9 non-royal, non-heron laguz classes
					randClass = random.Next(27, 36);
				}
			}
			else if (comboClassOptions.SelectedIndex == 1) // full race-mixing
			{
				if (charTier[charNum] == "a")
				{
					// 26 total tier 1 classes
					if (cbxHerons.Checked == true & heronNumber < 3)
					{
						randClass = random.Next(26);
						while (charName[charNum] == "edward" & (newClass[0] == 18 | newClass[0] == 25) &
							(randClass == 18 | randClass == 25)) //if micaiah is priest/heron,do not allow edward to be priest/heron
							randClass = random.Next(25);
					}
					else
					{
						// no heron class
						randClass = random.Next(25);
						while (charName[charNum] == "edward" & newClass[0] == 18 &
							randClass == 18) //if micaiah is priest,do not allow edward to be priest
							randClass = random.Next(25);
					}
				}
				else if (charTier[charNum] == "b")
				{
					if (charName[charNum] == "ike")
					{
						int[] classes = { 0, 1, 2, 5, 6, 8, 9, 10, 12, 13, 23, 24, 25, 26, 27, 28, 30, 31, 32 };
						randClass = classes[random.Next(19)];
					}
					else
					{
						// 33 total tier 2 classes
						if (cbxHerons.Checked == true & heronNumber < 3)
						{
							randClass = random.Next(33);
							while (charName[charNum] == "nephenee" & newClass[22] == 59 & randClass == 29)
								//if brom is heron,do not allow nephenee to be heron
								randClass = random.Next(33);

							// ranulf can't be mounted (ch3-5)
							while ((charName[charNum] == "ranulf") &
								(randClass == 3 | randClass == 7 | randClass == 11 | randClass == 14 | randClass == 22))
								randClass = random.Next(33);
						}
						else
						{
							randClass = random.Next(33);
							while (randClass == 29)
								randClass = random.Next(33);

							while ((charName[charNum] == "ranulf") &
								(randClass == 3 | randClass == 7 | randClass == 11 | randClass == 14 | randClass == 22 | randClass == 29))
								randClass = random.Next(33);
						}
					}
				}
				else if (charTier[charNum] == "c")
				{
					randClass = random.Next(32);
				}
				else // kurth, ena, giffca, gareth, nasir
				{
					randClass = random.Next(36);
				}
			}
			else if (comboClassOptions.SelectedIndex == 2) // only beorc
			{
				if (charTier[charNum] == "a")
				{
					randClass = random.Next(19);
					while (charName[charNum] == "edward" & newClass[0] == 18 & randClass == 18) //if micaiah is priest,do not allow edward to be priest
						randClass = random.Next(19);
				}
				else if (charTier[charNum] == "b")
				{
					if (charName[charNum] == "ike")
					{
						// ike cannot be mounted (ch3-5), rogue (cant kill bk), magic (cant kill ashera)
						int[] classes = { 0, 1, 2, 5, 6, 8, 9, 10, 12, 13 };
						randClass = classes[random.Next(10)];
					}
					else
					{
						randClass = random.Next(23);
						// ranulf cannot be mounted unit because of 3_5
						while ((charName[charNum] == "ranulf") &
							(randClass == 3 | randClass == 7 | randClass == 11 | randClass == 14 | randClass == 22))
							randClass = random.Next(23);
					}
				}
				else if (charTier[charNum] == "c")
				{
					randClass = random.Next(27);
				}
				else // kurth, ena, giffca, gareth, nasir
				{
					randClass = random.Next(27);
				}
			}
			else if (comboClassOptions.SelectedIndex == 3) // only laguz
			{
				if (charTier[charNum] == "a")
				{
					if (cbxHerons.Checked == true & heronNumber < 3)
					{
						randClass = random.Next(19, 26);
						while (charName[charNum] == "edward" & newClass[0] == 25 & randClass == 25) //if micaiah is heron,do not allow edward to be heron
							randClass = random.Next(19, 25);
					}
					else // no heron class
						randClass = random.Next(19, 25);
				}
				else if (charTier[charNum] == "b")
				{
					// no race-mixing gives 10 non-royal laguz classes
					randClass = random.Next(23, 33);
					while (charName[charNum] == "nephenee" & newClass[22] == 59 & randClass == 29)
						//if brom is heron,do not allow nephenee to be heron
						randClass = random.Next(23, 33);
					while ((cbxHerons.Checked == false | heronNumber == 3 | charName[charNum] == "ike") &
						randClass == 29) // keep randomizing until no heron
						randClass = random.Next(23, 33);
				}
				else if (charTier[charNum] == "c")
				{
					randClass = random.Next(27, 32);
				}
				else // kurth, ena, giffca, gareth, nasir
				{
					randClass = random.Next(27, 36);
				}
			}
			else if (comboClassOptions.SelectedIndex == 4) // classic medieval
			{
				if (charTier[charNum] == "a")
				{
					int[] classes = { 0, 3, 4, 8, 12 };
					randClass = classes[random.Next(5)];
				}
				else if (charTier[charNum] == "b")
				{
					int[] classes = { 0, 1, 5, 9, 13, 4 };
					if (charName[charNum] == "ike")
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
			else if (comboClassOptions.SelectedIndex == 5) // magic is everything
			{
				if (charTier[charNum] == "a")
				{
					int[] classes = { 14, 15, 16, 17, 18 };
					randClass = classes[random.Next(5)];

					while (charName[charNum] == "edward" & newClass[0] == 18 & randClass == 18) //if micaiah is priest,do not allow edward to be priest
						randClass = classes[random.Next(4)];
				}
				else if (charTier[charNum] == "b")
				{
					int[] classes = { 15, 16, 17, 18, 19, 20, 21 };
					if (charName[charNum] == "ike")
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
			else if (comboClassOptions.SelectedIndex == 6) // horse lovers anonymous
			{
				if (charTier[charNum] == "a")
				{
					int[] classes = { 2, 6, 10, 13 };
					randClass = classes[random.Next(4)];
				}
				else if (charTier[charNum] == "b")
				{
					int[] classes = { 3, 7, 11, 14, 22 };
					if (charName[charNum] == "ike")
						randClass = 0; // ike can't be horsed - is hero
					else if (charName[charNum] == "ranulf")
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
			else if (comboClassOptions.SelectedIndex == 7) // tanks a lot
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
			else if (comboClassOptions.SelectedIndex == 8) // fly guys
			{
				if (charTier[charNum] == "a")
				{
					int[] classes = { 7, 11, 23, 24, 25 };
					if (charName[charNum] == "edward" & newClass[0] == 18) //if micaiah is heron,do not allow edward to be heron
						randClass = classes[random.Next(4)];
					else if (cbxHerons.Checked == true & heronNumber < 3)
						randClass = classes[random.Next(5)];
					else
						randClass = classes[random.Next(4)];
				}
				else if (charTier[charNum] == "b")
				{
					int[] classes = { 8, 12, 27, 28, 29 };
					if (charName[charNum] == "ike")
						randClass = classes[random.Next(4)]; // ike can't be heron
					else if (charName[charNum] == "nephenee" & newClass[22] == 59)
						//if brom is heron,do not allow nephenee to be heron
						randClass = classes[random.Next(4)];
					else if (cbxHerons.Checked == true & heronNumber < 3)
						randClass = classes[random.Next(5)];
					else
						randClass = classes[random.Next(4)];
				}
				else if (charTier[charNum] == "c")
				{
					int[] classes = { 7, 11, 23, 29, 30 };
					randClass = classes[random.Next(5)];
				}
				else // kurth, ena, giffca, gareth, nasir
				{
					int[] classes = { 7, 11, 23, 31, 32 };
					randClass = classes[random.Next(5)];
				}
			}
			else if (comboClassOptions.SelectedIndex == 9) // lions and tigers and dragons
			{
				if (charTier[charNum] == "a")
				{
					int[] classes = { 19, 20, 21, 22 };
					randClass = classes[random.Next(4)];
				}
				else if (charTier[charNum] == "b")
				{
					int[] classes = { 23, 24, 25, 26, 30, 31, 32 };
					randClass = classes[random.Next(7)];
				}
				else if (charTier[charNum] == "c")
				{
					int[] classes = { 27, 28, 31 };
					randClass = classes[random.Next(3)];
				}
				else // kurth, ena, giffca, gareth, nasir
				{
					int[] classes = { 27, 28, 29, 30, 33, 34, 35 };
					randClass = classes[random.Next(7)];
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
					if (charName[charNum] == "ike")
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

			return randClass;
		}

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
					return 35; // druid gets worm
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
				if (charName[charNum] == "sanaki")
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

		private void animationChanger() // animation station
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
								animationOffset = random.Next(25,27);
								break;
							case 11:
							case 42:
							case 77:
							case 111:
								// wyvern
								animationOffset = random.Next(27,29);
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
								animationOffset = random.Next(33,35);
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
								animationOffset = random.Next(36,38);
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
								// heron
								animationOffset = 70;
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
						if (cbxRandRecr.Checked == true & charNum < 69) // go to new randomized character location instead
							stream.Position = (long)Convert.ToDouble(charAnimation[newRecr[charNum]]);
						else
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
						if (cbxRandRecr.Checked == true & charNum < 69) // go to new randomized character location instead
							stream.Position = (long)Convert.ToDouble(charGauge[newRecr[charNum]]);
						else
							stream.Position = (long)Convert.ToDouble(charGauge[charNum]);

						if (newRace[charNum] == "L" & charTier[charNum] != "c")
						{
							int outByte1, outByte2, outByte3, outByte4;

							if (cbxGaugeRand.Checked == true)
							{
								// randomly assign value from chosen min and max (inclusive) and output
								outByte1 = random.Next(Convert.ToInt32(numericLaguzMin.Value),
									(Convert.ToInt32(numericLaguzMax.Value) + 1));
								outByte2 = random.Next(Convert.ToInt32(numericLaguzMin.Value),
									(Convert.ToInt32(numericLaguzMax.Value) + 1));
								outByte3 = random.Next(Convert.ToInt32(numericLaguzMin.Value),
									(Convert.ToInt32(numericLaguzMax.Value) + 1));
								outByte4 = random.Next(Convert.ToInt32(numericLaguzMin.Value),
									(Convert.ToInt32(numericLaguzMax.Value) + 1));
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

							charChanges[charNum] += "," + outByte1.ToString() + "," + outByte2.ToString() +
								",-" + outByte3.ToString() + ",-" + outByte4.ToString();

							stream.WriteByte(Convert.ToByte(outByte1)); // (+) per turn
							stream.WriteByte(Convert.ToByte(outByte2)); // (+) per battle
							stream.WriteByte(Convert.ToByte(256 - outByte3)); // (-) per turn
							stream.WriteByte(Convert.ToByte(256 - outByte4)); // (-) per battle

						}
						else
							charChanges[charNum] += ",-,-,-,-";
					}
				}
			}
			catch
			{
				textBox1.Text = "Error 04: Cannot find file \\FE10Data.cms.decompressed! Abandoning randomization...";
				errorflag = 1;
			}
		}

		private void baseStatChanges()
		{
			int[,] inputMatrix = new int[82, 9];

			string line;
			string[] values;

			string dataFile = dataLocation + "\\FE10Data.cms.decompressed";
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\NewBases.csv");

			// skip header line
			line = dataReader.ReadLine();

			for (int i = 0; i < 82; i++)
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

			// randomized recruitment already changes personal bases, so only need to change class bases
			int number;
			if (cbxRandRecr.Checked == true)
				number = 46;
			else
				number = 82;

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

		private void enemyGrowthModifier()
		{
			int[] enemyGrowthLoc = new int[263];

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

							randGrowth = enemyGrowth + random.Next(Convert.ToInt32(numericEnemyGrowth.Value) + 1);

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
							// read growth rate
							basestat = stream.ReadByte();
							// change from signed to decimal
							if (basestat > 127)
								basestat -= 256;
							int minbase = basestat - Convert.ToInt32(numericBaseRand.Value);
							int maxbase = basestat + Convert.ToInt32(numericBaseRand.Value);

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

								// randomize from growth-deviation to growth+deviation
								// unless growth-deviation is less than minimum, then randomize from minimum to growth+deviation
								// unless growth+deviation is less than minimum, then just set the minimum
								if (k == 0)
								{
									if (maxgrowth < Convert.ToInt32(numericHP.Value))
										randgrowth = Convert.ToInt32(numericHP.Value);
									else if (mingrowth < Convert.ToInt32(numericHP.Value))
										randgrowth = random.Next(Convert.ToInt32(numericHP.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 1)
								{
									if (maxgrowth < Convert.ToInt32(numericATK.Value))
										randgrowth = Convert.ToInt32(numericATK.Value);
									else if (mingrowth < Convert.ToInt32(numericATK.Value))
										randgrowth = random.Next(Convert.ToInt32(numericATK.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 2)
								{
									if (maxgrowth < Convert.ToInt32(numericMAG.Value))
										randgrowth = Convert.ToInt32(numericMAG.Value);
									else if (mingrowth < Convert.ToInt32(numericMAG.Value))
										randgrowth = random.Next(Convert.ToInt32(numericMAG.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 3)
								{
									if (maxgrowth < Convert.ToInt32(numericSKL.Value))
										randgrowth = Convert.ToInt32(numericSKL.Value);
									else if (mingrowth < Convert.ToInt32(numericSKL.Value))
										randgrowth = random.Next(Convert.ToInt32(numericSKL.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 4)
								{
									if (maxgrowth < Convert.ToInt32(numericSPD.Value))
										randgrowth = Convert.ToInt32(numericSPD.Value);
									else if (mingrowth < Convert.ToInt32(numericSPD.Value))
										randgrowth = random.Next(Convert.ToInt32(numericSPD.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 5)
								{
									if (maxgrowth < Convert.ToInt32(numericLCK.Value))
										randgrowth = Convert.ToInt32(numericLCK.Value);
									else if (mingrowth < Convert.ToInt32(numericLCK.Value))
										randgrowth = random.Next(Convert.ToInt32(numericLCK.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else if (k == 6)
								{
									if (maxgrowth < Convert.ToInt32(numericDEF.Value))
										randgrowth = Convert.ToInt32(numericDEF.Value);
									else if (mingrowth < Convert.ToInt32(numericDEF.Value))
										randgrowth = random.Next(Convert.ToInt32(numericDEF.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}
								else
								{
									if (maxgrowth < Convert.ToInt32(numericRES.Value))
										randgrowth = Convert.ToInt32(numericRES.Value);
									else if (mingrowth < Convert.ToInt32(numericRES.Value))
										randgrowth = random.Next(Convert.ToInt32(numericRES.Value), maxgrowth);
									else
										randgrowth = random.Next(mingrowth, maxgrowth);
								}

							}
							else // Zero% growths run
							{
								if (k == 5) // luck has 100% growth because RD forces at least one stat increase every level
									randgrowth = 100;
								else
									randgrowth = 0;
							}
							// write new growth to game
							stream.Position = stream.Position - 1;
							stream.WriteByte(Convert.ToByte(randgrowth));
							// write to output log
							if (cbxRandRecr.Checked == true & charNum < 69)
								charChanges[recrInverse[charNum]] += "," + randgrowth.ToString();
							else
								charChanges[charNum] += "," + randgrowth.ToString();
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
							if ((cbxClassRand.Checked == true & newRace[charNum] == "B")
									| (cbxClassRand.Checked == false & charRace[charNum] == "B"))
								randSkill = random.Next(skillName.Length - 1); // beorc can't get wildheart, last skill on list
							else
								randSkill = random.Next(skillName.Length);
							
							// write output
							stream.WriteByte(0);
							stream.WriteByte(Convert.ToByte(firstByte[randSkill]));
							stream.WriteByte(Convert.ToByte(secondByte[randSkill]));
							stream.WriteByte(Convert.ToByte(thirdByte[randSkill]));
							// write to output log
							if (cbxRandRecr.Checked == true & charNum < 69)
								charChanges[recrInverse[charNum]] += "," + skillName[randSkill];
							else
								charChanges[charNum] += "," + skillName[randSkill];

						}
						// write blanks for rest of skills to output log
						for (int i = charSkillNum[charNum]; i < 4; i++)
						{
							if (cbxRandRecr.Checked == true & charNum < 69)
								charChanges[recrInverse[charNum]] += ",-";
							else
								charChanges[charNum] += ",-";
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

		private void shopRandomizer()
		{
			string line;
			string[] values;
			int[] highBytes = new int[192];
			int[] lowBytes = new int[192];

			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\ShopStuff.csv");

			// loop through 192 items in shop file
			for (int i = 0; i < 192; i++)
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
						itemSelection = random.Next(192);
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
						itemSelection = random.Next(192);
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
							if(i < 25)
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

		private void eventItemRandomizer()
		{
			string[] itemChapter = new string[91];
			int[] itemLocation = new int[91];
			string[] itemName = new string[91];

			string line;
			string[] values;
			// initialize character information
			System.IO.StreamReader dataReader = new System.IO.StreamReader(file + "\\assets\\EventItemInfo.csv");

			// skip header line/
			line = dataReader.ReadLine();
			// loop through all 91 items from base convos, villages, and chests
			for (int i = 0; i < 91; i++)
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

			for (int itemNum = 0; itemNum < 91; itemNum++)
			{
				byte[] itembytes = new byte[20];

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
						for (int i = 0; i < 20; i++)
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
					using (var stream = new System.IO.FileStream(dataLocation + "\\Scripts\\C0" + itemChapter[itemNum] + ".cmb", 
						System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
					{
						stream.Position = itemLocation[itemNum];
						for (int i = 0; i < 20; i++)
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

		private void generalChanges()
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

			// 3-6: volug does not gain halfshift if he is beorc
			if(errorflag == 0)
			{
				try
				{
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
			if (errorflag == 0)
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

			// modify 1_6 so that game over does not occur if jill, zihark, tauroneo die
			if (cbxClassRand.Checked == true & errorflag == 0)
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
			if (cbxClassRand.Checked == true & errorflag == 0)
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

			// change bronze bow to fire in shops.. who uses bronze bow anyway?
			// then change fire to worm
			if (cbxRandShop.Checked == false & errorflag == 0)
			{
				try
				{
					using (var stream = new System.IO.FileStream(dataLocation + "\\Shop\\shopitem_h.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 43513;
						stream.WriteByte(70); //F
						stream.WriteByte(73); //I
						stream.WriteByte(82); //R
						stream.WriteByte(69); //E
						stream.WriteByte(0x00); //null

						stream.Position = 43819;
						stream.WriteByte(87); //W
						stream.WriteByte(79); //O
						stream.WriteByte(82); //R
						stream.WriteByte(77); //M
						stream.WriteByte(0x00); //null
					}
					using (var stream = new System.IO.FileStream(dataLocation + "\\Shop\\shopitem_m.bin", System.IO.FileMode.Open,
							System.IO.FileAccess.ReadWrite))
					{
						stream.Position = 43513;
						stream.WriteByte(70); //F
						stream.WriteByte(73); //I
						stream.WriteByte(82); //R
						stream.WriteByte(69); //E
						stream.WriteByte(0x00); //null

						stream.Position = 43819;
						stream.WriteByte(87); //W
						stream.WriteByte(79); //O
						stream.WriteByte(82); //R
						stream.WriteByte(77); //M
						stream.WriteByte(0x00); //null
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
		}

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

		private void outputLog()
		{
			for (charNum = 0; charNum < totalUnitNumber; charNum++)
			{
				dataWriter.WriteLine(charChanges[charNum]);
			}
		}

		private void numericLaguzMin_ValueChanged(object sender, EventArgs e)
		{
			numericLaguzMax.Minimum = numericLaguzMin.Value;
		}

		private void numericLaguzMax_ValueChanged(object sender, EventArgs e)
		{
			numericLaguzMin.Maximum = numericLaguzMax.Value;
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{

			if (folderBD.ShowDialog() == DialogResult.OK)
			{
				dataLocation = folderBD.SelectedPath;
				lblLocation.Text = dataLocation;

				button1.Enabled = true;
			}

		}

		private void cbxClassRand_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxClassRand.Checked == false)
				panelClass.Enabled = false;
			else
				panelClass.Enabled = true;
		}

		private void cbxGrowthRand_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxGrowthRand.Checked == false)
				panelGrowths.Enabled = false;
			else
			{
				panelGrowths.Enabled = true;
				cbxZeroGrowths.Checked = false;
			}
		}

		private void cbxZeroGrowths_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxZeroGrowths.Checked == true)
				cbxGrowthRand.Checked = false;
		}
	}
}

