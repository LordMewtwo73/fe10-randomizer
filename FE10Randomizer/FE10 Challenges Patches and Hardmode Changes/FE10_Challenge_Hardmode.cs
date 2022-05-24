using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FE10FileExtract;
using System.IO;

namespace FE10_Challenges_Patches_and_Hardmode_Changes
{
	public partial class Form1 : Form
	{
		// location of randomizer
		string file = System.IO.Directory.GetCurrentDirectory();
		// location of DATA/files/
		string dataLocation;
		// location of DATA/sys/
		string sysLocation;

		// extraction classes
		DisposFile ChapterData;
		FEDataFile CharacterData;

		// bools for successful randomization
		bool notvanilla;
		bool rerandomized = false;
		bool validfolder = false;

		// list of all chapters in game
		string[] chapters;

		// error flag, 0 = no error
		int errorflag = 0;

		// whether user has NTSC-U 1.0 or 1.1, or PAL
		int gameVersion;

		string chapterFile;
		StreamWriter dataWriter;

		// create random number generator
		Random random;

		// arrays that hold character data
		Character[] characters;

		// arrays that hold class data
		Job[] classes;

		// info for outputlog
		string[] towerUnits = new string[12];


		public Form1()
		{
			InitializeComponent();

			InitializeToolTips();

			textBox1.Text = "Please load DATA\\files\\ folder of the ISO to begin";
			Application.DoEvents();
		}

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

					textBox1.Text = "Select desired settings, then press the Make Changes button.";
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

			// continue only if valid folder loaded and options are valid
			if (validfolder)
			{
				// overwrite a pre-randomized ISO with vanilla files
				if (notvanilla)
					CopyVanillaFiles();

				// delete temp files created from ExtractFiles
				DeleteTemp();

				// extract dispos, shop, and FE10Data files
				ExtractFiles();

				// modifies various character/class stats based on user selection
				DataFileModifications();

				// randomize with desired settings
				Randomize();

				// compress files back to ISO
				CompressFiles();
				// mark ISO as randomized
				ChangeISO_ID();

				notvanilla = true;

				// create outputlog
				CreateOutputLog();

				// delete temp files created from ExtractFiles
				DeleteTemp();

				textBox1.Text = "Randomization Complete! Check outputlog.htm for details";
				Application.DoEvents();
			}

			// re-enable front panel components
			FrontPanel_Enable();
		}

		// *************************************************************************************** INITIALIZATION FUNCTIONS
		#region

		// sets tooltips for all front panel objects
		private void InitializeToolTips()
		{
			toolTip1.SetToolTip(cbxNegGrowths, "units will lose stats upon level up");
			toolTip1.SetToolTip(cbxIronMan, "characters other than Micaiah, Ike, and Part 2 main units dying will not result in a gameover; check README.htm for more details");
			toolTip1.SetToolTip(cbxTowerUnits, "the randomizer selects 10 random characters that will be required to enter the tower alongside the usual required 6");
			toolTip1.SetToolTip(cbxEnemyRange, "adds enemy ranges to hardmode");
			toolTip1.SetToolTip(cbxWeapTri, "adds weapon triangle to hardmode");
			toolTip1.SetToolTip(cbxMapAff, "adds map affinities to hardmode");
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
			btnRandomize.Enabled = true;
			btnLoad.Enabled = true;
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
			}

			// extract dispos files to csv
			string chapterpath = dataLocation + "\\zmap\\emap0407d\\dispos_c.bin";
			string outpath = tempfolder + "\\chapter\\emap407d.csv";
			FE10ExtractCompress.ExtractDispos(chapterpath, outpath);
			// csv to class
			ChapterData = new DisposFile(outpath);
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
			}

			// list of chapters in game
			dataReader = new StreamReader(file + "\\assets\\chapterlist.txt");
			chapters = dataReader.ReadToEnd().Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			dataReader.Close();

			// reset error flag
			errorflag = 0;
			// generate randomizer with seed
			random = new Random();
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
				string namestring = "Fire Emblem: Radiant Dawn Randomized";
				byte[] namebytes = Encoding.ASCII.GetBytes(namestring);
				stream.Position = 117;
				foreach (byte letter in namebytes)
				{
					stream.WriteByte(letter);
					stream.WriteByte(0x00);
				}
			}
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
				string[] vanillafiles = Directory.GetFiles(vanillabackup);
				if (vanillafiles.Length == 0)
				{
					// no backup, warn user
					rerandomized = true;
					textBox1.Text = "WARNING: These files have already been randomized and the randomizer does not have copies of clean files. Re-randomize at your own risk.";
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

			// list of files edited by randomizer
			string[] files2copy = new string[64] { "tmd.bin", "ticket.bin","disc\\header.bin","sys\\boot.bin","sys\\main.dol",
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
													"files\\zu\\magwSs","files\\zu\\swoSs","files\\zu\\figSn","files\\zu\\knilUo","files\\zu\\silSo","files\\zu\\priSo" };



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

		#endregion
		// *************************************************************************************** TOPLEVEL FUNCTIONS
		#region

		private void DataFileModifications()
		{
			textBox1.Text = "Initializing Data File";
			Application.DoEvents();

			// remove fiona's first animation due to wiibafu fucking it up sometimes
			if (true)
			{
				string[] fionaanim = CharacterData.ReadStringArray("PID_FRIEDA", "Animations");
				fionaanim[0] = fionaanim[1];
				CharacterData.Write("PID_FRIEDA", "Animations", fionaanim);
			}
		}

		#endregion

		// *************************************************************************************** STAT FUNCTIONS
		#region

		// does the growth randomization
		private void growthRateModifier()
		{
			for (int i = 0; i < characters.Length; i++)
			{
				if (characters[i].Chapter != "0") // only for playable characters
				{
					int[] growths = CharacterData.ReadIntArray(characters[i].PID, "Growths");
					for (int j = 0; j < growths.Length; j++)
					{
						if (cbxZeroGrowths.Checked)
							growths[j] = 0;
					}
					// write new growths
					CharacterData.Write(characters[i].PID, "Growths", growths);
				}
			}
		}

		
		#endregion

		// *************************************************************************************** SAVING FUNCTIONS
		#region


		// modifies dispos and data files based on user selections
		private void DisposModifications()
		{
			textBox1.Text = "Modifying Chapter Files";
			Application.DoEvents();

			// choose random tower units
			if (cbxTowerUnits.Checked)
			{
				ChapterChar[] allchars = ChapterData.ReadAll();
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
					else
					{
						// dont include herons
						if (classes[characters[j].VanillaClass].Name.Contains("heron"))
						{ }
						else
						{
							PIDs.Add(characters[j].PID);
							names.Add(characters[j].Name);
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
						towerUnits[numsaved] = names[randchoice];
						numsaved++;
						// remove from lists
						PIDs.RemoveAt(randchoice);
						names.RemoveAt(randchoice);
					}
				}

				ChapterData.WriteAll(allchars);
			}
		}

		private void ScriptModifications()
		{
			textBox1.Text = "Modifying Script Files";
			Application.DoEvents();

			// remove gameovers caused by characters other than micaiah/ike dying
			if (cbxIronMan.Checked)
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

		}

		private void MaindolModifications()
		{
			textBox1.Text = "Modifying main.dol";
			Application.DoEvents();

			using (var stream = new System.IO.FileStream(sysLocation + "\\main.dol", System.IO.FileMode.Open,
						System.IO.FileAccess.ReadWrite))
			{
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

				// zero growths patch
				if (cbxZeroGrowths.Checked)
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

				// negative growths patch
				if (cbxNegGrowths.Checked)
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
				File.Delete(onefile);
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

			// save classes back to csv
			ChapterData.Save();

			string chapterpath = dataLocation + "\\zmap\\emap0407d\\dispos_c.bin";
			string csvpath = tempfolder + "\\chapter\\emap407d.csv";
			FE10ExtractCompress.CompressDispos(chapterpath, csvpath);
		}

		// writes outputlog for user
		private void CreateOutputLog()
		{
			textBox1.Text = "Creating log";
			Application.DoEvents();

			System.IO.StreamWriter logwriter = new System.IO.StreamWriter(file + "\\TOWER_outputlog.htm");

			System.IO.StreamReader reader = new System.IO.StreamReader(file + "\\assets\\logheader.txt");
			string outlogtext = reader.ReadToEnd();
			reader.Close();


			if (cbxTowerUnits.Checked)
			{
				outlogtext += "<br><hr><br><h2 id=\"tower\">Tower Units</h2>";

				outlogtext += "<img src=\"assets/logpics/ike.png\" alt=\"ike.png\" style=\"width:64px;height:64px;\">" +
					"<img src=\"assets/logpics/micaiah.png\" alt=\"micaiah.png\" style=\"width:64px;height:64px;\">" +
					"<img src=\"assets/logpics/sothe.png\" alt=\"sothe.png\" style=\"width:64px;height:64px;\">" +
					"<img src=\"assets/logpics/sanaki.png\" alt=\"sanaki.png\" style=\"width:64px;height:64px;\">";

				if (!cbxKurthEna.Checked)
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


			outlogtext += "</body></html>";

			logwriter.WriteLine(outlogtext);
			logwriter.Close();

		}

		#endregion






		private void Randomize()
		{
			growthRateModifier();
			DisposModifications();
			ScriptModifications();
			MaindolModifications();
		}



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

	}


	public class Character
	{
		// normal data
		public string Name, PID, Chapter, Tier, Race, LockedSkills, PhysMag;
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
