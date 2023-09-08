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
using System.Diagnostics;

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

		// list of decompiled script files
		string[] script_exl;

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
				ExaltScripts();

				// modifies various character/class stats based on user selection
				DataFileModifications();

				// randomize with desired settings
				Randomize();

				// compress files back to ISO
				CompressFiles();
				CompressScripts();
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
