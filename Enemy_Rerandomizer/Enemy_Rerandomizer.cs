using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Enemy_Rerandomizer
{
	public partial class Enemy_Rerandomizer : Form
	{
		// location of randomizer
		string file = System.IO.Directory.GetCurrentDirectory();
		// location of DATA/files/
		string dataLocation;
		// location of DATA/sys/
		string sysLocation;
		// extraction classes
		DisposFile[] ChapterData;
		FEDataFile CharacterData, ClassData;

		// arrays that hold class data
		Job[] classes;

		// list of all chapters in game
		string[] chapters;

		// class for jarod in 1-10, used to keep same in 1-11
		int jarodclass;
		List<string> enemyoldclass = new List<string>();
		List<int> enemynewclass = new List<int>();

		Random random;

		int errorflag = 0;
		bool validfolder;


		public Enemy_Rerandomizer()
		{
			InitializeComponent();

			textBox1.Text = "Please load DATA\\files\\ folder of the ISO to begin";
			Application.DoEvents();
		}

		// *************************************************************************************** BUTTON PRESS FUNCTIONS
		#region

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
					lblLocation.Text = dataLocation;

					// enable user to randomize
					btnRandomize.Enabled = true;

					textBox1.Text = "Select desired randomization settings, then press the randomize button.";
					Application.DoEvents();
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

		private void btnRandomize_Click(object sender, EventArgs e)
		{
			// disable front panel components
			FrontPanel_Disable();

			// initialize variables for randomization
			Initialize();

			if (validfolder)
			{
				// delete temp files created from ExtractFiles
				DeleteTemp();

				// extract dispos, shop, and FE10Data files
				ExtractFiles();

				// enemy changes
				SaveEnemiestoDispos();
				SaveFE10Data_Enemies();

				// compress files back to ISO
				CompressFiles();

				// delete temp files created from ExtractFiles
				//DeleteTemp();

				textBox1.Text = "Re-randomization Complete";
				Application.DoEvents();
			}

			// re-enable front panel components
			FrontPanel_Enable();
		}

		#endregion

		// *************************************************************************************** INITIALIZATION FUNCTIONS
		#region

		// checks for main.dol, FE10Data.cms, and a handful of other folders, gets version number, checks if vanilla
		private bool checkFiles()
		{
			string chapterfolder = dataLocation + "\\zmap\\";
			string fe10data = dataLocation + "\\FE10Data.cms";
			bool allgood = true;

			allgood &= File.Exists(fe10data);
			allgood &= Directory.Exists(chapterfolder);
			return (allgood);
		}

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

		// disables front panel from user
		private void FrontPanel_Disable()
		{
			// disable components so user can't change properties during randomization
			btnRandomize.Enabled = false;
			btnLoad.Enabled = false;
		}

		// enables front panel for user
		private void FrontPanel_Enable()
		{
			// disable components so user can't change properties during randomization
			btnRandomize.Enabled = true;
			btnLoad.Enabled = true;
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

			FE10ExtractCompress.ExtractFE10Data(decompressed, tempfolder + "\\data");
			// csv to classes
			foreach (string path in Directory.GetFiles(tempfolder + "\\data"))
			{
				if (Path.GetFileName(path).Contains("PersonData"))
					CharacterData = new FEDataFile(path);
				else if (Path.GetFileName(path).Contains("JobData"))
					ClassData = new FEDataFile(path);
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
		}

		// initializes variables and reads in characterdata and classdata
		private void Initialize()
		{
			textBox1.Text = "Initializing";
			Application.DoEvents();

			// initialize class information
			StreamReader dataReader = new StreamReader(file + "\\assets\\ClassInfo.csv");
			string[] lines = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			classes = new Job[lines.Length - 1];
			// loop through all classes, skipping header line
			for (int i = 0; i < classes.Length; i++)
			{
				classes[i] = new Job(lines[i + 1]);
			}

			// list of chapters in game
			dataReader = new StreamReader(file + "\\assets\\chapterlist.txt");
			chapters = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			// jarod class for 1-10 and 1-11
			jarodclass = -1;
			// reset error flag
			errorflag = 0;
			// generate randomizer with seed
			random = new Random();
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
					if (classtype != "H" & classes[newclass].Classtype_E == "H")
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

		// chooses weapons for enemy based on class, what chapter they're on, and if they're a boss
		private string[] chooseEnemyWeapons(ChapterChar editchar, string chaptername, bool isboss)
		{
			string[] weapons = editchar.Weapons;
			for (int i = 0; i < classes.Length; i++)
			{
				if (classes[i].JID == editchar.JID)
				{
					string[] weapontypes = classes[i].Weapon_E.Split(';');
					if (classes[i].Classtype_E == "H" & (!(cbxRandEnemy.Checked | cbxRandAllies.Checked) | !cbxEnemHealers.Checked))
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

		// does various enemy/ally modifications and saves to dispos files
		private void SaveEnemiestoDispos()
		{
			textBox1.Text = "Making weird enemies";
			Application.DoEvents();

			for (int i = 0; i < chapters.Length; i++)
			{
				enemyoldclass = new List<string>();
				enemynewclass = new List<int>();
				// read in all characters in the chapter file
				ChapterChar[] disposchars = ChapterData[i].ReadAll();
				for (int j = 0; j < disposchars.Length; j++)
				{
					if (disposchars[j].Color == 1 | disposchars[j].Color == 3) // red boi or yellow allies
					{
						bool boss = false;
						StreamReader dataReader = new StreamReader(file + "\\assets\\bosslist.csv");
						string[] bosspids = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
						dataReader.Close();
						// check if boss
						for (int k = 0; k < bosspids.Length; k++)
						{
							if (disposchars[j].PID == bosspids[k].Split(',')[0] & chapters[i] == bosspids[k].Split(',')[1])
							{
								boss = true;
								break;
							}
						}
						if (!boss)
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
							}
						}
					}
				}
				ChapterData[i].WriteAll(disposchars);
			}
		}

		// various fe10 data modifications for enemies
		private void SaveFE10Data_Enemies()
		{
			StreamReader dataReader = new StreamReader(file + "\\assets\\enemyPIDlist.txt");
			string[] enemypids = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			for (int i = 0; i < enemypids.Length; i++)
			{
				// deletes animiation pointers for enemy PIDs so they use default animations
				if (cbxRandEnemy.Checked | cbxRandAllies.Checked)
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
					if (enemypids[i].Contains("APOSTLE"))
					{
						char[] weaprank = CharacterData.ReadString(enemypids[i], "Weapon_Ranks").ToArray<char>();
						weaprank[5] = 'S';
						CharacterData.Write(enemypids[i], "Weapon_Ranks", String.Join("", weaprank));
					}
				}


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
		}

		#endregion

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


		private void cbxRandEnemy_CheckedChanged(object sender, EventArgs e)
		{
			panelEnemy.Enabled = cbxRandAllies.Checked | cbxRandEnemy.Checked;
		}

		private void cbxRandAllies_CheckedChanged(object sender, EventArgs e)
		{
			panelEnemy.Enabled = cbxRandAllies.Checked | cbxRandEnemy.Checked;
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
