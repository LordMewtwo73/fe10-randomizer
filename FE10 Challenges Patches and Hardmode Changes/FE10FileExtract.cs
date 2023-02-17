using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FE10_Challenges_Patches_and_Hardmode_Changes
{
	public class FEDataFile
	{
		string FilePath;
		string[] ReadLines;

		/// <summary>
		/// reads all lines from csvpath (saved as FilePath) and saves them in ReadLines
		/// </summary>
		/// <param name="csvpath"></param>
		public FEDataFile(string csvpath)
		{
			FilePath = csvpath;
			StreamReader reader = new StreamReader(FilePath);
			ReadLines = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
			reader.Close();
		}

		/// <summary>
		/// saves ReadLines back into FilePath
		/// </summary>
		public void Save()
		{
			StreamWriter writer = new StreamWriter(FilePath);
			for (int i = 0; i < ReadLines.Length; i++)
				writer.WriteLine(ReadLines[i]);
			writer.Close();
		}


		public string[] ReadStringArray(string keyname, string specifier)
		{
			// find keyname
			int location = -1;
			for (int i = 0; i < ReadLines.Length; i++)
			{
				string[] splitstring = ReadLines[i].Split(',');
				if (splitstring[0] == keyname)
				{
					location = i;
					break;
				}
			}

			// return blank if not found
			if (location == -1)
				return (new string[1] { "" });

			// iterate from keyname to find specifier
			for (int i = location; i < ReadLines.Length; i++)
			{
				string[] splitstring = ReadLines[i].Split(',');
				if (splitstring[0] == specifier)
				{
					// return blank if nothing
					if (splitstring.Length == 1)
						return (new string[1] { "" });
					// return all except first column
					string[] outstring = new string[splitstring.Length - 1];
					for (int j = 1; j < splitstring.Length; j++)
					{
						outstring[j - 1] = splitstring[j];
					}
					return (outstring);

				}
			}

			// if we got this far without returning, that means we didn't find anything
			return (new string[1] { "" });

		}
		public string ReadString(string keyname, string specifier)
		{
			{
				// find keyname
				int location = -1;
				for (int i = 0; i < ReadLines.Length; i++)
				{
					string[] splitstring = ReadLines[i].Split(',');
					if (splitstring[0] == keyname)
					{
						location = i;
						break;
					}
				}

				// return blank if not found
				if (location == -1)
					return ("");

				// iterate from keyname to find specifier
				for (int i = location; i < ReadLines.Length; i++)
				{
					string[] splitstring = ReadLines[i].Split(',');
					if (splitstring[0] == specifier)
					{
						// return blank if nothing
						if (splitstring.Length == 1)
							return ("");
						// return all except first column
						return (splitstring[1]);

					}
				}

				// if we got this far without returning, that means we didn't find anything
				return ("");

			}
		}
		public int[] ReadIntArray(string keyname, string specifier)
		{
			// find keyname
			int location = -1;
			for (int i = 0; i < ReadLines.Length; i++)
			{
				string[] splitstring = ReadLines[i].Split(',');
				if (splitstring[0] == keyname)
				{
					location = i;
					break;
				}
			}

			// return blank if not found
			if (location == -1)
				return (new int[1] { -1 });

			// iterate from keyname to find specifier
			for (int i = location; i < ReadLines.Length; i++)
			{
				string[] splitstring = ReadLines[i].Split(',');
				if (splitstring[0] == specifier)
				{
					// return blank if nothing
					if (splitstring.Length == 1)
						return (new int[1] { -1 });
					// return all except first column
					int[] outint = new int[splitstring.Length - 1];
					for (int j = 1; j < splitstring.Length; j++)
					{
						outint[j - 1] = Convert.ToInt32(splitstring[j]);
					}
					return (outint);

				}
			}

			// if we got this far without returning, that means we didn't find anything
			return (new int[1] { -1 });

		}
		public int ReadInt(string keyname, string specifier)
		{
			{
				// find keyname
				int location = -1;
				for (int i = 0; i < ReadLines.Length; i++)
				{
					string[] splitstring = ReadLines[i].Split(',');
					if (splitstring[0] == keyname)
					{
						location = i;
						break;
					}
				}

				// return blank if not found
				if (location == -1)
					return (-1);

				// iterate from keyname to find specifier
				for (int i = location; i < ReadLines.Length; i++)
				{
					string[] splitstring = ReadLines[i].Split(',');
					if (splitstring[0] == specifier)
					{
						// return blank if nothing
						if (splitstring.Length == 1)
							return (-1);
						// return all except first column
						return (Convert.ToInt32(splitstring[1]));

					}
				}

				// if we got this far without returning, that means we didn't find anything
				return (-1);

			}
		}

		public void Write(string keyname, string specifier, string[] savethis)
		{
			// find keyname
			int location = -1;
			for (int i = 0; i < ReadLines.Length; i++)
			{
				string[] splitstring = ReadLines[i].Split(',');
				if (splitstring[0] == keyname)
				{
					location = i;
					break;
				}
			}

			// return blank if not found
			if (location != -1)
			{
				// iterate from keyname to find specifier
				for (int i = location; i < ReadLines.Length; i++)
				{
					string[] splitstring = ReadLines[i].Split(',');
					if (splitstring[0] == specifier)
					{
						// save data
						ReadLines[i] = specifier + "," + String.Join(",", savethis);
						break;
					}
				}
			}

		}
		public void Write(string keyname, string specifier, string savethis)
		{
			// find keyname
			int location = -1;
			for (int i = 0; i < ReadLines.Length; i++)
			{
				string[] splitstring = ReadLines[i].Split(',');
				if (splitstring[0] == keyname)
				{
					location = i;
					break;
				}
			}

			// return blank if not found
			if (location != -1)
			{
				// iterate from keyname to find specifier
				for (int i = location; i < ReadLines.Length; i++)
				{
					string[] splitstring = ReadLines[i].Split(',');
					if (splitstring[0] == specifier)
					{
						// save data
						string savestring = specifier + "," + savethis;
						ReadLines[i] = savestring;
						break;
					}
				}
			}
		}
		public void Write(string keyname, string specifier, int savethis)
		{
			// find keyname
			int location = -1;
			for (int i = 0; i < ReadLines.Length; i++)
			{
				string[] splitstring = ReadLines[i].Split(',');
				if (splitstring[0] == keyname)
				{
					location = i;
					break;
				}
			}

			// return blank if not found
			if (location != -1)
			{
				// iterate from keyname to find specifier
				for (int i = location; i < ReadLines.Length; i++)
				{
					string[] splitstring = ReadLines[i].Split(',');
					if (splitstring[0] == specifier)
					{
						// save data
						string savestring = specifier + "," + savethis.ToString();
						ReadLines[i] = savestring;
						break;
					}
				}
			}
		}
		public void Write(string keyname, string specifier, int[] savethis)
		{
			// find keyname
			int location = -1;
			for (int i = 0; i < ReadLines.Length; i++)
			{
				string[] splitstring = ReadLines[i].Split(',');
				if (splitstring[0] == keyname)
				{
					location = i;
					break;
				}
			}

			// return blank if not found
			if (location != -1)
			{
				// iterate from keyname to find specifier
				for (int i = location; i < ReadLines.Length; i++)
				{
					string[] splitstring = ReadLines[i].Split(',');
					if (splitstring[0] == specifier)
					{
						// save data
						string savestring = specifier;
						for (int j = 0; j < savethis.Length; j++)
							savestring += "," + savethis[j].ToString();
						ReadLines[i] = savestring;
						break;
					}
				}
			}
		}

	}

	public class ChapterChar
	{
		public string FullInfo;
		public int DisposID;
		public string PID, JID;
		public int[] CutsceneLoc, Location;
		public int Level, Color, TransState;
		public string[] Weapons, Items, Skills, AI;
		public string MoveType;

		/// <summary>
		/// splits line of character in a dispos file into all class variables
		/// </summary>
		/// <param name="charline"></param>
		public ChapterChar(string charline)
		{
			FullInfo = charline;
			string[] split = FullInfo.Split(',');
			DisposID = Convert.ToInt32(split[0]);
			PID = split[3];
			JID = split[4];
			CutsceneLoc = new int[2] { Convert.ToInt32(split[5]), Convert.ToInt32(split[6]) };
			Location = new int[2] { Convert.ToInt32(split[7]), Convert.ToInt32(split[8]) };
			Level = Convert.ToInt32(split[9]);
			Color = Convert.ToInt32(split[10]);
			TransState = Convert.ToInt32(split[11]);
			Weapons = new string[4] { split[17], split[18], split[19], split[20] };
			Items = new string[3] { split[21], split[22], split[23] };
			Skills = new string[5] { split[24], split[25], split[26], split[27], split[28] };
			AI = new string[4] { split[29], split[30], split[31], split[33] };
			MoveType = split[32];
		}
		public ChapterChar()
		{
			FullInfo = "";
			DisposID = -1;
			PID = "";
		}

		/// <summary>
		/// saves local class variables into FullInfo
		/// </summary>
		public void Refresh()
		{
			string[] split = FullInfo.Split(',');
			split[0] = DisposID.ToString();
			split[3] = PID;
			split[4] = JID;
			for (int i = 5; i < 7; i++)
				split[i] = CutsceneLoc[i - 5].ToString();
			for (int i = 7; i < 9; i++)
				split[i] = Location[i - 7].ToString();
			split[9] = Level.ToString();
			split[10] = Color.ToString();
			split[11] = TransState.ToString();
			for (int i = 17; i < 21; i++)
				split[i] = Weapons[i - 17];
			for (int i = 21; i < 24; i++)
				split[i] = Items[i - 21];
			for (int i = 24; i < 29; i++)
				split[i] = Skills[i - 24];
			split[29] = AI[0];
			split[30] = AI[1];
			split[31] = AI[2];
			split[32] = MoveType;
			split[33] = AI[3];

			FullInfo = String.Join(",", split);
		}
	}

	public class DisposFile
	{
		public string FilePath;
		public string[] ReadLines;
		public int maxID;

		/// <summary>
		/// reads all lines from csvpath (saved as FilePath) and saves them in ReadLines
		/// </summary>
		/// <param name="csvpath"></param>
		public DisposFile(string csvpath)
		{
			FilePath = csvpath;
			StreamReader reader = new StreamReader(FilePath);
			ReadLines = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
			reader.Close();
			maxID = 0;
			for (int i = 0; i < ReadLines.Length; i++)
			{
				try
				{
					int disposID = Convert.ToInt32(ReadLines[i].Split(',')[0]);
					if (disposID > maxID)
						maxID = disposID;
				}
				catch
				{
					if (i != 0)
						i++;
				}
			}
		}

		/// <summary>
		/// saves ReadLines back into FilePath
		/// </summary>
		public void Save()
		{
			StreamWriter writer = new StreamWriter(FilePath);
			for (int i = 0; i < ReadLines.Length; i++)
				writer.WriteLine(ReadLines[i]);
			writer.Close();
		}

		/// <summary>
		/// reads character info for selected PID
		/// </summary>
		/// <param name="charPID"></param>
		/// <returns></returns>
		public ChapterChar Read(string charPID)
		{
			for (int i = 0; i < ReadLines.Length; i++)
			{
				if (ReadLines[i].Contains(charPID))
				{
					ChapterChar outchar = new ChapterChar(ReadLines[i]);
					return (outchar);
				}
			}
			// did not find, return blank
			ChapterChar blank = new ChapterChar();
			return (blank);
		}

		public ChapterChar[] ReadAll()
		{
			List<ChapterChar> allchars = new List<ChapterChar>();
			for (int i = 0; i < ReadLines.Length; i++)
			{
				if (ReadLines[i].Split(',').Length > 5 & !ReadLines[i].StartsWith("disposID"))
					allchars.Add(new ChapterChar(ReadLines[i]));
			}
			return (allchars.ToArray());
		}

		/// <summary>
		/// writes character changes to dispos file
		/// </summary>
		/// <param name="char2write"></param>
		public void Write(ChapterChar char2write)
		{
			for (int i = 0; i < ReadLines.Length; i++)
			{
				if (ReadLines[i].Contains(char2write.PID))
				{
					char2write.Refresh();
					ReadLines[i] = char2write.FullInfo;
					break;
				}
			}
		}

		public void WriteAll(ChapterChar[] chars2write)
		{
			for (int j = 0; j < chars2write.Length; j++)
			{
				for (int i = 0; i < ReadLines.Length; i++)
				{
					if (ReadLines[i].StartsWith(chars2write[j].DisposID.ToString()))
					{
						chars2write[j].Refresh();
						ReadLines[i] = chars2write[j].FullInfo;
						break;
					}
				}
			}
		}

		/// <summary>
		/// inserts a given character after the character with selected PID
		/// </summary>
		/// <param name="char2insert"></param>
		/// <param name="afterPID"></param>
		public void Insert(ChapterChar char2insert, string afterPID)
		{
			int mostrecentheader = 0;
			string[] templines = new string[ReadLines.Length + 1];
			for (int i = 0; i < ReadLines.Length; i++)
			{
				if (ReadLines[i].Split(',').Length == 5)
					mostrecentheader = i;
				// save each line
				templines[i] = ReadLines[i];
				// insert after this line
				if (ReadLines[i].Contains(afterPID))
				{
					// set new ID
					char2insert.DisposID = maxID + 1;
					char2insert.Refresh();
					// insert
					templines[i + 1] = char2insert.FullInfo;
					// save the rest
					for (int j = i + 1; j < ReadLines.Length; j++)
						templines[j + 1] = ReadLines[j];
					// add one to character count
					string[] splitstring = templines[mostrecentheader].Split(',');
					splitstring[1] = (Convert.ToInt32(splitstring[1]) + 1).ToString();
					templines[mostrecentheader] = String.Join(",", splitstring);
					// save as ReadLines and exit
					ReadLines = templines;
					maxID += 1;
					break;
				}
			}
		}
	}


	public class FE10ExtractCompress
	{
		//public static void NotMain()
		//{
			//string thispath = Directory.GetCurrentDirectory();
			//string[] allfiles = getRecursiveFiles(thispath);
			//test_readskills(thispath + "\\pidlist.txt", thispath + "\\skillsbypid.csv", thispath + "\\FE10Data\\001%PersonData.csv");

			//BackupVanillaFiles();

			//ExtractDispos(thispath + "\\dispos_c.bin", thispath + "\\dispos\\outputdispos.csv");
			//CompressDispos(thispath + "\\dispos\\dispostest_c.bin", thispath + "\\dispos\\outputdispos.csv");

			//ExtractShopfile(thispath + "\\shopitem_h.bin", thispath + "\\outputshop.csv");
			//CompressShopfile(thispath + "\\shop_test.bin", thispath + "\\outputshop.csv");

			//ExtractFE10Data(thispath + "\\FE10Data_test.cms.decompressed", thispath + "\\FE10Data");
			//ExtractFE10Data(thispath + "\\FE10Data.cms.decompressed", thispath + "\\FE10Data");
			//CompressFE10Data(thispath + "\\FE10Data_test.cms.decompressed", thispath + "\\FE10Data");

			//ExtractFE10Anim(thispath + "\\FE10Anim.cms.decompressed", thispath + "\\FE10Anim");
			//CompressFE10Anim(thispath + "\\FE10Anim.cms.decompressed_out", thispath + "\\FE10Anim");

			//ExtractFE10Battle(thispath + "\\FE10Battle.cms.decompressed", thispath + "\\FE10Battle");
			//CompressFE10Battle(thispath + "\\FE10Battle.cms.decompressed_out", thispath + "\\FE10Battle");

			//ExtractFE10Battle(thispath + "\\FE10Battle.cms.decompressed_modded", thispath + "\\FE10Battle_modded");
			//ExtractFE10Anim(thispath + "\\FE10Anim.cms.decompressed_modded", thispath + "\\FE10Anim_modded");

			//ExtractScript(thispath + "\\C0101.cmb", thispath + "\\outputscript.csv");
			//CompressScript(thispath + "\\C0101test.cmb", thispath + "\\outputscript.csv");
			//ExtractScript(thispath + "\\C0101test.cmb", thispath + "\\outputscript.csv");

			//test();
			//check(thispath + "\\FE10Data.cms.decompressed", thispath + "\\FE10Data_test.cms.decompressed");
		//}


		public static void ExtractDispos(string dispospath, string csvpath)
		{
			byte[] readbytes = new byte[4];
			int filesize, num_pointers1, num_pointers2;
			int header, dataregion, pointer1, pointer2, endregion;
			int size_header, size_dataregion, size_pointer1, size_pointer2, size_endregion;
			int pointerstart;
			List<string> labels = new List<string>();
			List<int> offsets = new List<int>();

			List<string> outstring = new List<string>();

			// header always is at the top, and it is always 32 bytes long
			header = 0;
			size_header = 32;
			dataregion = 32;

			using (var stream = new FileStream(dispospath, FileMode.Open, FileAccess.ReadWrite))
			{
				// read header
				#region
				stream.Position = header;
				// full file size
				stream.Read(readbytes, 0, 4);
				filesize = bytes2int(readbytes);
				// data region size
				stream.Read(readbytes, 0, 4);
				size_dataregion = bytes2int(readbytes);
				// number of pointers in pointer section 1
				stream.Read(readbytes, 0, 4);
				num_pointers1 = bytes2int(readbytes);
				// number of pointers in pointer section 2
				stream.Read(readbytes, 0, 4);
				num_pointers2 = bytes2int(readbytes);

				// calculate other locations and sizes
				pointer1 = dataregion + size_dataregion;
				size_pointer1 = num_pointers1 * 4;
				pointer2 = pointer1 + size_pointer1;
				size_pointer2 = num_pointers2 * 8;
				endregion = pointer2 + size_pointer2;
				size_endregion = filesize - endregion;
				#endregion

				// read labels from pointer2
				#region
				stream.Position = pointer2;
				for (int i = 0; i < num_pointers2; i++)
				{
					stream.Read(readbytes, 0, 4);
					int dataoffset = bytes2int(readbytes);
					offsets.Add(dataoffset + 32);
					stream.Read(readbytes, 0, 4);
					int endoffset = bytes2int(readbytes);
					labels.Add(ReadPointer(stream, endregion + endoffset));
				}
				// order secondary pointers
				List<string> unsort_labels = new List<string>();
				List<int> unsort_offsets = new List<int>();
				for (int i = 0; i < labels.Count; i++)
				{
					unsort_labels.Add(labels[i]);
					unsort_offsets.Add(offsets[i]);
				}
				offsets.Sort();
				for (int i = 0; i < labels.Count; i++)
					labels[offsets.FindIndex(a => a == unsort_offsets[i])] = unsort_labels[i];
				#endregion

				// start reading dataregion
				#region
				for (int k = 0; k < offsets.Count; k++)
				{
					bool skip = false;
					string tempstring = labels[k];
					stream.Position = offsets[k];
					if (k < offsets.Count - 1)
					{
						if (offsets[k + 1] - offsets[k] == 4)
						{
							stream.Read(readbytes, 0, 4);
							string datetime = ReadPointer(stream, bytes2int(readbytes) + 32);
							outstring.Add(tempstring + "," + datetime);
							skip = true;
						}
					}

					if (!skip)
					{
						// first four bytes regarding the group of units
						int numberunits = stream.ReadByte();
						tempstring += "," + numberunits.ToString();
						for (int i = 0; i < 3; i++)
							tempstring += "," + stream.ReadByte().ToString();
						outstring.Add(tempstring);

						//unit data
						outstring.Add("disposID,various unknown bitflags(required unit/required space/etc),moreflags,PID,JID,cutscene xcoord,cutscene ycord,xcoord,ycoord," +
										"level,color(0=blue 1=red 2=green 3=yellow),0=untrans/1=trans,unknown byte,unknown4byte,unknown4byte,unknown4byte,possiblyaffinity," +
										"weapon1,weapon2,weapon3,weapon4,item1,item2,item3,skill1,skill2,skill3,skill4,skill5,highhp_goal,highhp_strat,lowhp_goal,movetype,lowhp_strat");
						for (int unitnum = 0; unitnum < numberunits; unitnum++)
						{
							tempstring = "";
							// two bytes disposID
							int disposID = stream.ReadByte();
							disposID = (disposID * 256) + stream.ReadByte();
							tempstring += disposID.ToString();
							// unknown bitflags
							for (int i = 0; i < 2; i++)
								tempstring += "," + stream.ReadByte();
							// PID and JID
							for (int i = 0; i < 2; i++)
							{
								stream.Read(readbytes, 0, 4);
								tempstring += "," + ReadPointer(stream, bytes2int(readbytes) + 32);
							}
							// coordinates, level, color, transstate, unknown byte
							for (int i = 0; i < 8; i++)
								tempstring += "," + stream.ReadByte();
							// unknown sets of 4 bytes (four total)
							// weapons, items, and skills (12 total)
							// hp goals and strategies (five total)
							// adds to 21
							for (int i = 0; i < 21; i++)
							{
								stream.Read(readbytes, 0, 4);
								int tempint = bytes2int(readbytes);
								if (tempint >= 0 & tempint < filesize)
									tempstring += "," + ReadPointer(stream, tempint + 32);
								else
									tempstring += ",";
							}

							outstring.Add(tempstring);
						}

						//outstring.Add(" ");
					}
				}

				#endregion

			}

			StreamWriter writer = new StreamWriter(csvpath);
			for (int i = 0; i < outstring.Count; i++)
				writer.WriteLine(outstring[i]);
			writer.Close();
		}

		public static void CompressDispos(string dispospath, string csvpath)
		{
			byte[] readbytes = new byte[4];
			int filesize, dataregionsize, num_pointers1, num_pointers2;
			int lines = 0;
			List<string> labels = new List<string>();
			List<int> dataoffsets = new List<int>();
			List<string> pointernames = new List<string>();
			List<int> pointerloc = new List<int>();

			// read in all data
			StreamReader reader = new StreamReader(csvpath);
			string[] readlines = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
			reader.Close();

			using (var stream = new FileStream(dispospath, FileMode.Create, FileAccess.ReadWrite))
			{
				stream.Position = 0;
				// create blank header - will edit later
				for (int i = 0; i < 32; i++)
					stream.WriteByte(0x00);

				// use data to write the sections of the datablock
				for (int i = 0; i < readlines.Length; i++)
				{
					string[] cells = readlines[i].Split(',');
					if (cells.Length == 2)
					{
						// first line, datetime entry
						labels.Add(cells[0]);
						dataoffsets.Add((int)stream.Position);
						pointernames.Add(cells[1]);
						pointerloc.Add((int)stream.Position);
						// create blank pointer - will edit later
						for (int x = 0; x < 4; x++)
							stream.WriteByte(0x00);
					}
					else if (cells.Length == 5)
					{
						// header of data section
						labels.Add(cells[0]);
						dataoffsets.Add((int)stream.Position);
						lines = Convert.ToInt32(cells[1]);
						for (int x = 1; x < 5; x++)
							stream.WriteByte((byte)Convert.ToInt32(cells[x]));
						// skip header text
						i++;
					}
					else if (cells.Length > 5)
					{
						// 2-byte disposID
						readbytes = int2bytes(Convert.ToInt32(cells[0]));
						for (int x = 2; x < 4; x++)
							stream.WriteByte(readbytes[x]);
						// single byte flags
						for (int x = 1; x < 3; x++)
							stream.WriteByte((byte)Convert.ToInt32(cells[x]));
						// PID and JID pointers
						for (int x = 3; x < 5; x++)
						{
							pointernames.Add(cells[x]);
							pointerloc.Add((int)stream.Position);
							// create blank pointer - will edit later
							for (int y = 0; y < 4; y++)
								stream.WriteByte(0x00);
						}
						for (int x = 5; x < 13; x++)
							stream.WriteByte((byte)Convert.ToInt32(cells[x]));
						for (int x = 13; x < cells.Length; x++)
						{
							if (cells[x] != "")
							{
								pointernames.Add(cells[x]);
								pointerloc.Add((int)stream.Position);
							}
							// create blank pointer - will edit later
							for (int y = 0; y < 4; y++)
								stream.WriteByte(0x00);
						}

					}

				}

				// write pointernames to file
				List<string> filterednames = new List<string>();
				List<int> pointerdestination = new List<int>();
				for (int i = 0; i < pointernames.Count; i++)
					filterednames.Add(pointernames[i]);
				filterednames.Sort();
				pointerdestination.Add((int)stream.Position);
				stream.Write(Encoding.ASCII.GetBytes(filterednames[0]), 0, Encoding.ASCII.GetBytes(filterednames[0]).Length);
				stream.WriteByte(0x00);
				for (int i = 1; i < filterednames.Count; i++)
				{
					if (filterednames[i - 1] == filterednames[i])
					{
						filterednames.RemoveAt(i);
						i--;
					}
					else
					{
						pointerdestination.Add((int)stream.Position);
						stream.Write(Encoding.ASCII.GetBytes(filterednames[i]), 0, Encoding.ASCII.GetBytes(filterednames[i]).Length);
						stream.WriteByte(0x00);
					}
				}
				// pad
				while (stream.Position % 4 != 0)
					stream.WriteByte(0x00);

				// write pointer regions to file
				dataregionsize = (int)stream.Position - 32;
				for (int i = 0; i < pointerloc.Count; i++)
					stream.Write(int2bytes(pointerloc[i] - 32),0,4);

				// alphabetize secondary pointers
				List<string> unsort_labels = new List<string>();
				List<int> unsort_offsets = new List<int>();
				for (int i = 0; i < labels.Count; i++)
				{
					unsort_labels.Add(labels[i]);
					unsort_offsets.Add(dataoffsets[i]);
				}
				labels.Sort();
				for (int i = 0; i < labels.Count; i++)
					dataoffsets[labels.FindIndex(a => a == unsort_labels[i])] = unsort_offsets[i];

				List<byte> endregion = new List<byte>();
				for (int i = 0; i < labels.Count; i++)
				{
					stream.Write(int2bytes(dataoffsets[i] - 32),0,4);
					stream.Write(int2bytes(endregion.Count),0,4);
					byte[] asciibytes = Encoding.ASCII.GetBytes(labels[i]);
					for (int x = 0; x < asciibytes.Length; x++)
						endregion.Add(asciibytes[x]);
					endregion.Add(0x00);
				}

				// write end region
				stream.Write(endregion.ToArray(),0, endregion.ToArray().Length);
				filesize = (int)stream.Position;

				// go back and fill pointers
				for (int i = 0; i < pointerloc.Count; i++)
				{
					stream.Position = pointerloc[i];
					for (int j = 0; j < filterednames.Count; j++)
					{
						if (pointernames[i] == filterednames[j])
						{
							stream.Write(int2bytes(pointerdestination[j] - 32),0,4);
						}
					}
				}

				// back to beginning and write header
				stream.Position = 0;
				stream.Write(int2bytes(filesize),0,4);
				stream.Write(int2bytes(dataregionsize),0,4);
				stream.Write(int2bytes(pointerloc.Count),0,4);
				stream.Write(int2bytes(labels.Count),0,4);

			}

		}

		public static void ExtractShopfile(string shoppath, string csvpath)
		{
			byte[] readbytes = new byte[4];
			int filesize, num_pointers1, num_pointers2;
			int header, dataregion, pointer1, pointer2, endregion;
			int size_header, size_dataregion, size_pointer1, size_pointer2, size_endregion;
			int pointerstart;
			List<string> labels = new List<string>();
			List<int> offsets = new List<int>();

			List<string> outstring = new List<string>();

			// header always is at the top, and it is always 32 bytes long
			header = 0;
			size_header = 32;
			dataregion = 32;

			using (var stream = new FileStream(shoppath, FileMode.Open, FileAccess.ReadWrite))
			{
				// read header
				#region
				stream.Position = header;
				// full file size
				stream.Read(readbytes, 0, 4);
				filesize = bytes2int(readbytes);
				// data region size
				stream.Read(readbytes, 0, 4);
				size_dataregion = bytes2int(readbytes);
				// number of pointers in pointer section 1
				stream.Read(readbytes, 0, 4);
				num_pointers1 = bytes2int(readbytes);
				// number of pointers in pointer section 2
				stream.Read(readbytes, 0, 4);
				num_pointers2 = bytes2int(readbytes);

				// calculate other locations and sizes
				pointer1 = dataregion + size_dataregion;
				size_pointer1 = num_pointers1 * 4;
				pointer2 = pointer1 + size_pointer1;
				size_pointer2 = num_pointers2 * 8;
				endregion = pointer2 + size_pointer2;
				size_endregion = filesize - endregion;
				#endregion

				// read labels from pointer2
				#region
				stream.Position = pointer2;
				for (int i = 0; i < num_pointers2; i++)
				{
					stream.Read(readbytes, 0, 4);
					int dataoffset = bytes2int(readbytes);
					offsets.Add(dataoffset + 32);
					stream.Read(readbytes, 0, 4);
					int endoffset = bytes2int(readbytes);
					labels.Add(ReadPointer(stream, endregion + endoffset));
				}

				// order secondary pointers
				List<string> unsort_labels = new List<string>();
				List<int> unsort_offsets = new List<int>();
				for (int i = 0; i < labels.Count; i++)
				{
					unsort_labels.Add(labels[i]);
					unsort_offsets.Add(offsets[i]);
				}
				offsets.Sort();
				for (int i = 0; i < labels.Count; i++)
					labels[offsets.FindIndex(a => a == unsort_offsets[i])] = unsort_labels[i];
				#endregion

				// start reading dataregion
				#region
				for (int k = 0; k < offsets.Count; k++)
				{
					string tempstring = labels[k];
					stream.Position = offsets[k];

					if (labels[k].Contains("SHOP_PERSON"))
					{
						outstring.Add(tempstring);
						outstring.Add("Armory,Shop,Forge,??");
						tempstring = stream.ReadByte().ToString();
						for (int x = 1; x < 4; x++)
							tempstring += "," + stream.ReadByte().ToString();
						outstring.Add(tempstring);
					}
					else if (labels[k].Contains("WSHOP_ITEMS") | labels[k].Contains("ISHOP_ITEMS"))
					{
						outstring.Add(tempstring);
						outstring.Add("IID,bargain(1=yes 0=no),bargainID,unknown");
						while (true)
						{
							stream.Read(readbytes, 0, 4);
							int itempointer = bytes2int(readbytes);
							if (itempointer == 0)
								break;
							else
							{
								tempstring = ReadPointer(stream, itempointer + 32);
								for (int x = 0; x < 4; x++)
									tempstring += "," + stream.ReadByte().ToString();
								outstring.Add(tempstring);
							}

						}
					}
					else if (labels[k].Contains("FSHOP_ITEMS"))
					{
						outstring.Add(tempstring);
						outstring.Add("mik,mkv,IID");
						for (int i = 0; i < 60; i++)
						{
							tempstring = "";
							for (int j = 0; j < 3; j++)
							{
								stream.Read(readbytes, 0, 4);
								int itempointer = bytes2int(readbytes);
								if (itempointer == 0)
									tempstring += ",";
								else
									tempstring += ReadPointer(stream, itempointer + 32) + ",";
							}
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("FSHOP_CARD"))
					{
						stream.Position += 3;
						int cards = stream.ReadByte();
						outstring.Add(tempstring);
						outstring.Add("image,MT,ACC,CRT,COIN,%chance,?,?,?");
						for (int i = 0; i < cards; i++)
						{
							tempstring = "";
							stream.Read(readbytes, 0, 4);
							int messpoint = bytes2int(readbytes);
							if (messpoint == 0)
							{ }
							else
								tempstring += ReadPointer(stream, messpoint + 32);
							for (int j = 0; j < 8; j++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}

					}
					else
					{
						stream.Read(readbytes, 0, 4);
						string openingpointer = ReadPointer(stream, bytes2int(readbytes) + 32);
						outstring.Add(tempstring + "," + openingpointer);
					}

				}

				#endregion

			}

			outstring.Add("");

			StreamWriter writer = new StreamWriter(csvpath);
			for (int i = 0; i < outstring.Count; i++)
				writer.WriteLine(outstring[i]);
			writer.Close();
		}

		public static void CompressShopfile(string shoppath, string csvpath)
		{
			byte[] readbytes = new byte[4];
			int filesize, dataregionsize, num_pointers1, num_pointers2;
			int lines = 0;
			List<string> labels = new List<string>();
			List<int> dataoffsets = new List<int>();
			List<string> pointernames = new List<string>();
			List<int> pointerloc = new List<int>();

			// read in all data
			StreamReader reader = new StreamReader(csvpath);
			string[] readlines = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
			reader.Close();

			using (var stream = new FileStream(shoppath, FileMode.Create, FileAccess.ReadWrite))
			{
				stream.Position = 0;
				// create blank header - will edit later
				for (int i = 0; i < 40; i++)
					stream.WriteByte(0x00);

				// use data to write the sections of the datablock
				string sectionname = "";
				for (int i = 0; i < readlines.Length; i++)
				{
					if (readlines[i] != "")
					{
						string[] cells = readlines[i].Split(',');
						// header of a data section
						sectionname = cells[0];
						labels.Add(cells[0]);
						dataoffsets.Add((int)stream.Position);
						// skip header text
						i += 2;
						cells = readlines[i].Split(',');

						// write depending on type of section
						if (sectionname.Contains("SHOP_PERSON"))
						{
							for (int x = 0; x < 4; x++)
								stream.WriteByte((byte)Convert.ToInt32(cells[x]));
						}
						else if (sectionname.Contains("WSHOP_ITEMS") | sectionname.Contains("ISHOP_ITEMS"))
						{
							while (readlines[i].Split(',').Length > 1)
							{
								cells = readlines[i].Split(',');
								pointernames.Add(cells[0]);
								pointerloc.Add((int)stream.Position);
								// create blank pointer - will edit later
								for (int y = 0; y < 4; y++)
									stream.WriteByte(0x00);
								for (int x = 1; x < 5; x++)
									stream.WriteByte((byte)Convert.ToInt32(cells[x]));
								i++;
							}
							i--;
							// buffer between chapters
							for (int y = 0; y < 4; y++)
								stream.WriteByte(0x00);
						}
						else if (sectionname.Contains("FSHOP_ITEMS"))
						{
							while (readlines[i].Split(',').Length > 1)
							{
								cells = readlines[i].Split(',');
								for (int j = 0; j < 3; j++)
								{
									if (cells[j] != "")
									{
										pointernames.Add(cells[j]);
										pointerloc.Add((int)stream.Position);
									}
									// create blank pointer - will edit later
									for (int y = 0; y < 4; y++)
										stream.WriteByte(0x00);
								}
								i++;
							}
							i--;
						}
						else if (sectionname.Contains("FSHOP_CARD"))
						{
							int cards = 0;
							for (int y = 0; y < 4; y++)
								stream.WriteByte(0x00);
							while (readlines[i].Split(',').Length > 1)
							{
								cards += 1;
								cells = readlines[i].Split(',');
								pointernames.Add(cells[0]);
								pointerloc.Add((int)stream.Position);
								// create blank pointer - will edit later
								for (int y = 0; y < 4; y++)
									stream.WriteByte(0x00);
								for (int x = 1; x < 9; x++)
									stream.WriteByte((byte)Convert.ToInt32(cells[x]));
								i++;
							}
							long currpos = stream.Position;
							stream.Position -= (12 * cards) + 1;
							stream.WriteByte((byte)cards);
							stream.Position = currpos;
							i--;
						}
					}
				}

				// write pointernames to file
				List<string> filterednames = new List<string>();
				List<int> pointerdestination = new List<int>();
				for (int i = 0; i < pointernames.Count; i++)
					filterednames.Add(pointernames[i]);
				filterednames.Sort();
				pointerdestination.Add((int)stream.Position);
				stream.Write(Encoding.ASCII.GetBytes(filterednames[0]),0, Encoding.ASCII.GetBytes(filterednames[0]).Length);
				stream.WriteByte(0x00);
				for (int i = 1; i < filterednames.Count; i++)
				{
					if (filterednames[i - 1] == filterednames[i])
					{
						filterednames.RemoveAt(i);
						i--;
					}
					else
					{
						pointerdestination.Add((int)stream.Position);
						stream.Write(Encoding.ASCII.GetBytes(filterednames[i]),0, Encoding.ASCII.GetBytes(filterednames[i]).Length);
						stream.WriteByte(0x00);
					}
				}
				// pad
				while (stream.Position % 4 != 0)
					stream.WriteByte(0x00);

				// write pointer regions to file
				dataregionsize = (int)stream.Position - 32;
				for (int i = 0; i < pointerloc.Count; i++)
					stream.Write(int2bytes(pointerloc[i] - 32),0,4);

				// alphabetize secondary pointers
				List<string> unsort_labels = new List<string>();
				List<int> unsort_offsets = new List<int>();
				for (int i = 0; i < labels.Count; i++)
				{
					unsort_labels.Add(labels[i]);
					unsort_offsets.Add(dataoffsets[i]);
				}
				labels.Sort();
				for (int i = 0; i < labels.Count; i++)
					dataoffsets[labels.FindIndex(a => a == unsort_labels[i])] = unsort_offsets[i];

				List<byte> endregion = new List<byte>();
				for (int i = 0; i < labels.Count; i++)
				{
					stream.Write(int2bytes(dataoffsets[i] - 32),0,4);
					stream.Write(int2bytes(endregion.Count),0,4);
					byte[] asciibytes = Encoding.ASCII.GetBytes(labels[i]);
					for (int x = 0; x < asciibytes.Length; x++)
						endregion.Add(asciibytes[x]);
					endregion.Add(0x00);
				}

				// write end region
				stream.Write(endregion.ToArray(),0, endregion.ToArray().Length);
				filesize = (int)stream.Position;

				// go back and fill pointers
				for (int i = 0; i < pointerloc.Count; i++)
				{
					stream.Position = pointerloc[i];
					for (int j = 0; j < filterednames.Count; j++)
					{
						if (pointernames[i] == filterednames[j])
						{
							stream.Write(int2bytes(pointerdestination[j] - 32),0,4);
						}
					}
				}

				// back to beginning and write header
				stream.Position = 0;
				stream.Write(int2bytes(filesize),0,4);
				stream.Write(int2bytes(dataregionsize),0,4);
				stream.Write(int2bytes(pointerloc.Count),0,4);
				stream.Write(int2bytes(labels.Count),0,4);

			}
		}

		public static void ExtractFE10Data(string datapath, string extractfolder)
		{
			byte[] readbytes = new byte[4];
			int filesize, num_pointers1, num_pointers2;
			int header, dataregion, pointer1, pointer2, endregion;
			int size_header, size_dataregion, size_pointer1, size_pointer2, size_endregion;
			int pointerstart;
			List<string> labels = new List<string>();
			List<int> offsets = new List<int>();

			List<string> outstring = new List<string>();

			// header always is at the top, and it is always 32 bytes long
			header = 0;
			size_header = 32;
			dataregion = 32;

			if (!Directory.Exists(extractfolder))
				Directory.CreateDirectory(extractfolder);
			string[] files = Directory.GetFiles(extractfolder);
			for (int i = 0; i < files.Length; i++)
				File.Delete(files[i]);

			using (var stream = new FileStream(datapath, FileMode.Open, FileAccess.ReadWrite))
			{
				// read header
				#region
				stream.Position = header;
				// full file size
				stream.Read(readbytes, 0, 4);
				filesize = bytes2int(readbytes);
				// data region size
				stream.Read(readbytes, 0, 4);
				size_dataregion = bytes2int(readbytes);
				// number of pointers in pointer section 1
				stream.Read(readbytes, 0, 4);
				num_pointers1 = bytes2int(readbytes);
				// number of pointers in pointer section 2
				stream.Read(readbytes, 0, 4);
				num_pointers2 = bytes2int(readbytes);

				// calculate other locations and sizes
				pointer1 = dataregion + size_dataregion;
				size_pointer1 = num_pointers1 * 4;
				pointer2 = pointer1 + size_pointer1;
				size_pointer2 = num_pointers2 * 8;
				endregion = pointer2 + size_pointer2;
				size_endregion = filesize - endregion;
				#endregion

				// read labels from pointer2
				#region
				stream.Position = pointer2;
				for (int i = 0; i < num_pointers2; i++)
				{
					stream.Read(readbytes, 0, 4);
					int dataoffset = bytes2int(readbytes);
					offsets.Add(dataoffset + 32);
					stream.Read(readbytes, 0, 4);
					int endoffset = bytes2int(readbytes);
					labels.Add(ReadPointer(stream, endregion + endoffset));
				}

				// order secondary pointers
				List<string> unsort_labels = new List<string>();
				List<int> unsort_offsets = new List<int>();
				for (int i = 0; i < labels.Count; i++)
				{
					unsort_labels.Add(labels[i]);
					unsort_offsets.Add(offsets[i]);
				}
				offsets.Sort();
				for (int i = 0; i < labels.Count; i++)
					labels[offsets.FindIndex(a => a == unsort_offsets[i])] = unsort_labels[i];
				#endregion

				// start reading dataregion
				#region
				for (int k = 0; k < offsets.Count; k++)
				{
					bool savedata = true;
					outstring.Clear();
					int pointer;
					string tempstring = labels[k];
					stream.Position = offsets[k];

					if (labels[k].Contains("DatabaseHead"))
					{
						stream.Position += 4;
						for (int i = 0; i < 2; i++)
						{
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							outstring.Add(name);
						}
					}
					else if (labels[k].Contains("PersonData"))
					{
						stream.Read(readbytes, 0, 4);
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							int skillnum = stream.ReadByte();
							stream.Position += 3;
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							stream.Position -= 7;
							// name as header
							outstring.Add(name);

							string[] captions = new string[9] { "Flags", "Level", "Gender", "MPID", "MNPID", "FID", "JID", "Affinity", "Weapon_Ranks" };
							for (int i = 0; i < 3; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());
							stream.Position += 4;
							for (int i = 3; i < 9; i++)
							{
								stream.Read(readbytes, 0, 4);
								outstring.Add(captions[i] + "," + ReadPointer(stream, bytes2int(readbytes) + 32));
							}

							// Skills
							tempstring = "Skills";
							for (int i = 0; i < skillnum; i++)
							{
								stream.Read(readbytes, 0, 4);
								tempstring += "," + ReadPointer(stream, bytes2int(readbytes) + 32);
							}
							outstring.Add(tempstring);

							tempstring = "Buffer";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);

							// Animations
							tempstring = "Animations";
							for (int i = 0; i < 4; i++)
							{
								stream.Read(readbytes, 0, 4);
								tempstring += "," + ReadPointer(stream, bytes2int(readbytes) + 32);
							}
							outstring.Add(tempstring);
							// other bytes
							outstring.Add("Biorhythm," + stream.ReadByte().ToString());
							tempstring = "Unknown";
							for (int i = 0; i < 3; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
							outstring.Add("Authority," + stream.ReadByte().ToString());
							// laguz gauge
							tempstring = "Laguz_Gauge";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
							// Bases and Growths
							tempstring = "Bases";
							for (int i = 0; i < 8; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
							outstring.Add("CON," + stream.ReadByte().ToString());
							outstring.Add("MOV," + stream.ReadByte().ToString());
							tempstring = "Growths";
							for (int i = 0; i < 8; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);

							stream.Position += 1;
						}
					}
					else if (labels[k].Contains("JobData"))
					{
						stream.Read(readbytes, 0, 4);
						int numjob = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numjob; x++)
						{
							//Console.WriteLine(x.ToString());
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							// name as header
							outstring.Add(name + "," + name);

							string[] captions = new string[20] { "MJID", "Unknown_String", "MH_J", "Prev_Class", "Next_Class",
																"Transform_Class", "Locked_Item", "Animation", "Base_WeaponRank",
																"Max_WeaponRank", "CON", "Armor_Type", "Armor_WT", "Mount_Type",
																"Mount_WT", "Unknown", "Movement_Type", "MOV", "Capacity", "FOW"};
							for (int i = 0; i < 10; i++)
							{
								stream.Read(readbytes, 0, 4);
								pointer = bytes2int(readbytes);
								if (pointer != 0)
									outstring.Add(captions[i] + "," + ReadPointer(stream, pointer + 32));
								else
									outstring.Add(captions[i] + ",");
							}
							for (int i = 10; i < 15; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());
							int skillnum = stream.ReadByte();
							int classtypenum = stream.ReadByte();
							for (int i = 15; i < 20; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());
							stream.Position += 4;

							// Skills
							tempstring = "Skills";
							for (int i = 0; i < skillnum; i++)
							{
								stream.Read(readbytes, 0, 4);
								tempstring += "," + ReadPointer(stream, bytes2int(readbytes) + 32);
							}
							outstring.Add(tempstring);

							tempstring = "Satori_Sign,";
							stream.Read(readbytes, 0, 4);
							pointer = bytes2int(readbytes);
							if (pointer != 0)
								tempstring += ReadPointer(stream, pointer + 32);
							outstring.Add(tempstring);

							// Class Types
							tempstring = "Class_Types";
							for (int i = 0; i < classtypenum; i++)
							{
								stream.Read(readbytes, 0, 4);
								tempstring += "," + ReadPointer(stream, bytes2int(readbytes) + 32);
							}
							outstring.Add(tempstring);

							// Caps, Bases, and Growths
							tempstring = "Caps";
							for (int i = 0; i < 8; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
							tempstring = "Bases";
							for (int i = 0; i < 8; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
							tempstring = "Growths";
							for (int i = 0; i < 8; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
							tempstring = "Promo_Gains";
							for (int i = 0; i < 8; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);

						}
					}
					else if (labels[k].Contains("ItemData"))
					{
						stream.Read(readbytes, 0, 4);
						int numitems = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numitems; x++)
						{
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							// name as header
							outstring.Add(name + "," + name);

							string[] captions = new string[19] { "MIID", "MH_I", "Weapon_Type", "Damage_Type", "Rank",
																"EID_1", "EID_2", "EID_3", "Unknown","Icon",
																"MT","HIT","CRIT","WT","Uses","EXP","MinRange","MaxRange",
																"Unknown2"};
							for (int i = 0; i < 8; i++)
							{
								stream.Read(readbytes, 0, 4);
								pointer = bytes2int(readbytes);
								if (pointer != 0)
									outstring.Add(captions[i] + "," + ReadPointer(stream, pointer + 32));
								else
									outstring.Add(captions[i] + ",");
							}
							for (int i = 8; i < 10; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());
							tempstring = "Cost/Use";
							readbytes[0] = 0x00;
							readbytes[1] = 0x00;
							stream.Read(readbytes, 2, 2);
							tempstring += "," + bytes2int(readbytes).ToString();
							outstring.Add(tempstring);
							for (int i = 10; i < 19; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());
							stream.Position += 3;
							outstring.Add("Unknown3," + stream.ReadByte().ToString());

							int attributenum = stream.ReadByte();
							int effectnum = stream.ReadByte();
							int statflag = stream.ReadByte();

							// Attributes
							tempstring = "Attributes";
							for (int i = 0; i < attributenum; i++)
							{
								stream.Read(readbytes, 0, 4);
								tempstring += "," + ReadPointer(stream, bytes2int(readbytes) + 32);
							}
							outstring.Add(tempstring);

							// Effectiveness
							tempstring = "Effectiveness";
							for (int i = 0; i < effectnum; i++)
							{
								stream.Read(readbytes, 0, 4);
								tempstring += "," + ReadPointer(stream, bytes2int(readbytes) + 32);
							}
							outstring.Add(tempstring);

							tempstring = "Bonuses";
							if (statflag != 0)
							{
								for (int i = 0; i < 12; i++)
									tempstring += "," + stream.ReadByte().ToString();
							}
							outstring.Add(tempstring);

						}
					}
					else if (labels[k].Contains("SkillData"))
					{
						stream.Read(readbytes, 0, 4);
						int numskills = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numskills; x++)
						{
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							// name as header
							outstring.Add(name + "," + name);

							string[] captions = new string[10] { "MSID", "MH_S", "Description2", "EID_1", "EID_2", "Unlock_Item",
																"ID", "Capacity_Type", "Capacity","Icon"};
							for (int i = 0; i < 6; i++)
							{
								stream.Read(readbytes, 0, 4);
								pointer = bytes2int(readbytes);
								if (pointer != 0)
									outstring.Add(captions[i] + "," + ReadPointer(stream, pointer + 32));
								else
									outstring.Add(captions[i] + ",");
							}
							for (int i = 6; i < 10; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());

							int conditionnum = stream.ReadByte();
							int condition2num = stream.ReadByte();

							outstring.Add("Unknown1," + stream.ReadByte().ToString());
							outstring.Add("Unknown2," + stream.ReadByte().ToString());

							// Condition_1
							tempstring = "Condition_1";
							if (conditionnum > 0)
							{
								stream.Read(readbytes, 0, 4);
								long prevpos = stream.Position;
								stream.Position = bytes2int(readbytes) + 32;
								for (int i = 0; i < conditionnum; i++)
								{
									tempstring += "," + stream.ReadByte();
									stream.Position += 3;
									stream.Read(readbytes, 0, 4);
									tempstring += "," + ReadPointer(stream, bytes2int(readbytes) + 32);
								}
								stream.Position = prevpos;
							}
							else
							{
								//tempstring += ",";
								stream.Position += 4;
							}
							outstring.Add(tempstring);

							// Condition_2
							tempstring = "Condition_2";
							if (condition2num > 0)
							{
								stream.Read(readbytes, 0, 4);
								long prevpos = stream.Position;
								stream.Position = bytes2int(readbytes) + 32;
								for (int i = 0; i < condition2num; i++)
								{
									tempstring += "," + stream.ReadByte();
									stream.Position += 3;
									stream.Read(readbytes, 0, 4);
									tempstring += "," + ReadPointer(stream, bytes2int(readbytes) + 32);
								}
								stream.Position = prevpos;
							}
							else
							{
								//tempstring += ",";
								stream.Position += 4;
							}
							outstring.Add(tempstring);



						}
					}
					else if (labels[k].Contains("SID_"))
					{
						savedata = false;
					}
					else if (labels[k].Contains("RelianceParam"))
					{
						string[] captions = new string[16] { "N", "C", "B", "A", "Chapter_Bonus", "Unknown_1","Adjacent_Bonus","Carry_Bonus","Heal_Bonus",
															"Shove_Bonus","Unknown_2","add00","add01","add02","add03","add04" };
						for (int x = 0; x < 16; x++)
							outstring.Add(captions[x] + "," + stream.ReadByte().ToString());
					}
					else if (labels[k].Contains("DivineData"))
					{
						stream.Read(readbytes, 0, 4);
						int numaff = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numaff; x++)
						{
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							// name as header
							outstring.Add(name + "," + name);

							string[] captions = new string[4] { "MT", "DEF", "HIT", "AVO" };
							for (int i = 0; i < 4; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());

							stream.Position += 4;

						}
					}
					else if (labels[k].Contains("GameData"))
					{
						tempstring = "";
						for (int x = 0; x < 136; x++)
							tempstring += stream.ReadByte().ToString() + ",";
						outstring.Add(tempstring);
					}
					else if (labels[k].Contains("TerrainData"))
					{
						stream.Read(readbytes, 0, 4);
						//int numskills = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < 199; x++)
						{
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							// name as header
							outstring.Add(name + "," + name);

							stream.Read(readbytes, 0, 4);
							pointer = bytes2int(readbytes);
							if (pointer != 0)
								outstring.Add("Name" + "," + ReadPointer(stream, pointer + 32));
							else
								outstring.Add("Name");

							stream.Read(readbytes, 0, 4);
							long prevpos = stream.Position;
							stream.Position = bytes2int(readbytes) + 32;
							string[] captions = new string[11] { "Ground_AVO", "Ground_DEF", "Ground_RES", "Air_AVO", "Air_DEF", "Air_RES", "Heal_Percent", "Unknown1",
																"EID","SFX_1","SFX_2" };
							for (int i = 0; i < 8; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());
							for (int i = 8; i < 11; i++)
							{
								stream.Read(readbytes, 0, 4);
								pointer = bytes2int(readbytes);
								if (pointer != 0)
									outstring.Add(captions[i] + "," + ReadPointer(stream, pointer + 32));
								else
									outstring.Add(captions[i] + ",");
							}

							tempstring = "Cost";
							for (int i = 0; i < 24; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);

							stream.Position = prevpos;
							outstring.Add("ID," + stream.ReadByte().ToString());
							stream.Position += 3;

						}
					}
					else if (labels[k].Contains("BattleTerrIndex"))
					{
						stream.Read(readbytes, 0, 4);
						readbytes[1] = 0x00;
						int numterr = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numterr; x++)
						{
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							// name as header
							outstring.Add("Name," + name);
							tempstring = "Terrain_Data";
							for (int i = 0; i < 100; i++)
							{
								stream.Read(readbytes, 0, 4);
								tempstring += "," + bytes2int(readbytes).ToString();
							}

							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("BattleTerrName"))
					{
						while (stream.Position < offsets[k + 1])
						{
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							outstring.Add(name);
						}
					}
					else if (labels[k].Contains("RelianceData"))
					{
						stream.Read(readbytes, 0, 4);
						int numsupp = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numsupp; x++)
						{
							//Console.WriteLine(x.ToString());
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							// name as header
							outstring.Add(name);

							stream.Read(readbytes, 0, 4);
							int numsupp2 = bytes2int(readbytes);
							for (int y = 0; y < numsupp2; y++)
							{
								tempstring = ",";
								stream.Read(readbytes, 0, 4);
								tempstring += ReadPointer(stream, bytes2int(readbytes) + 32);
								readbytes = new byte[4] { 0, 0, 0, 0 };
								stream.Read(readbytes, 2, 2);
								tempstring += "," + bytes2int(readbytes).ToString();
								outstring.Add(tempstring);

								stream.Position += 2;
							}
						}
					}
					else if (labels[k].Contains("ChapterData"))
					{
						stream.Read(readbytes, 0, 4);
						int numchap = bytes2int(readbytes);

						for (int x = 0; x < numchap; x++)
						{
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							// name as header
							outstring.Add("Mapname," + name);

							string[] captions = new string[20] { "MCT", "zmap", "ScriptFile", "MessFile",
																"Unknown_1", "Unknown_2", "Unknown_3", "Unknown_4",
																"Objective_E", "Objective_M", "Objective_H",
																"BEXP", "Convoy", "Mounted_Mod",
																"Affinity","RID_1","Terrain","Weather","RID_2","Commander"};
							for (int i = 0; i < 8; i++)
							{
								stream.Read(readbytes, 0, 4);
								pointer = bytes2int(readbytes);
								if (pointer != 0)
									outstring.Add(captions[i] + "," + ReadPointer(stream, pointer + 32));
								else
									outstring.Add(captions[i] + ",");
							}
							for (int i = 8; i < 11; i++)
							{
								tempstring = captions[i];
								for (int b = 0; b < 8; b++)
								{
									stream.Read(readbytes, 0, 4);
									pointer = bytes2int(readbytes);
									if (pointer != 0)
										tempstring += "," + ReadPointer(stream, pointer + 32);
									else
										tempstring += ",";
								}
								outstring.Add(tempstring);
							}
							#region
							outstring.Add("MapID," + stream.ReadByte().ToString());
							tempstring = "Unknown_5";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
							for (int i = 11; i < 14; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());
							tempstring = "Unknown_6";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
							#endregion
							for (int i = 14; i < 20; i++)
							{
								stream.Read(readbytes, 0, 4);
								pointer = bytes2int(readbytes);
								if (pointer != 0)
									outstring.Add(captions[i] + "," + ReadPointer(stream, pointer + 32));
								else
									outstring.Add(captions[i] + ",");
							}
							tempstring = "Unknown_7";
							for (int i = 0; i < 12; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
							//stream.Position += 4;
						}
					}
					else if (labels[k].Contains("GroupData"))
					{
						stream.Read(readbytes, 0, 4);
						int numgroup = bytes2int(readbytes);

						for (int x = 0; x < numgroup; x++)
						{
							stream.Read(readbytes, 0, 4);
							string IDname = bytes2int(readbytes).ToString();
							outstring.Add("IDname," + IDname);

							string[] captions = new string[2] { "Group_ID", "Message" };
							for (int i = 0; i < 2; i++)
							{
								stream.Read(readbytes, 0, 4);
								pointer = bytes2int(readbytes);
								if (pointer != 0)
									outstring.Add(captions[i] + "," + ReadPointer(stream, pointer + 32));
								else
									outstring.Add(captions[i] + ",");
							}
						}
					}
					else if (labels[k].Contains("KiznaData"))
					{
						stream.Read(readbytes, 0, 4);
						int numgroup = bytes2int(readbytes);

						for (int x = 0; x < numgroup; x++)
						{
							stream.Read(readbytes, 0, 4);
							tempstring = ReadPointer(stream, bytes2int(readbytes) + 32);
							stream.Read(readbytes, 0, 4);
							outstring.Add(tempstring + "," + ReadPointer(stream, bytes2int(readbytes) + 32));

							string[] captions = new string[2] { "Unknown", "CRT_DODGE" };
							for (int i = 0; i < 2; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());

							stream.Position += 2;
						}
					}
					else if (labels[k].Contains("DivineParam"))
					{
						stream.Read(readbytes, 0, 4);
						int numaffin = bytes2int(readbytes);

						for (int x = 0; x < numaffin; x++)
						{
							stream.Read(readbytes, 0, 4);
							tempstring = ReadPointer(stream, bytes2int(readbytes) + 32);
							stream.Read(readbytes, 0, 4);
							outstring.Add(tempstring + "," + ReadPointer(stream, bytes2int(readbytes) + 32));

							tempstring = "Unknown";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);

						}
					}
					else if (labels[k].Contains("SukumiData"))
					{
						stream.Read(readbytes, 0, 4);
						int numaffin = bytes2int(readbytes);

						for (int x = 0; x < numaffin; x++)
						{
							stream.Read(readbytes, 0, 4);
							tempstring = ReadPointer(stream, bytes2int(readbytes) + 32);
							stream.Read(readbytes, 0, 4);
							outstring.Add(tempstring + "," + ReadPointer(stream, bytes2int(readbytes) + 32));

							string[] captions = new string[2] { "MT_Bonus", "HIT_Bonus" };
							for (int i = 0; i < 2; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());

							stream.Position += 2;
						}
					}
					else if (labels[k].Contains("BioData"))
					{
						outstring.Add("Amplitude,Unknown,Frequency,Y-Trans");
						for (int x = 0; x < 10; x++)
						{
							tempstring = "";
							for (int y = 0; y < 4; y++)
							{
								stream.Read(readbytes, 0, 4);
								tempstring += bytes2float(readbytes).ToString() + ",";
							}
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("BioParam"))
					{
						outstring.Add("Threshold,HIT_Bonus,AVO_Bonus,CRT_Bonus,Skill_Bonus,HiddenItem_Bonus");

						for (int x = 0; x < 5; x++)
						{
							tempstring = "";
							stream.Read(readbytes, 0, 4);
							tempstring += bytes2float(readbytes).ToString();
							for (int y = 0; y < 5; y++)
								tempstring += "," + stream.ReadByte().ToString();
							stream.Position += 3;
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("NOBTL_TABLE"))
					{
						stream.Read(readbytes, 0, 4);
						int numbattle = bytes2int(readbytes);

						for (int x = 0; x < numbattle; x++)
						{
							stream.Read(readbytes, 0, 4);
							string name = ReadPointer(stream, bytes2int(readbytes) + 32);
							// name as header
							outstring.Add(name + "," + name);

							tempstring = "";
							while (true)
							{
								stream.Read(readbytes, 0, 4);
								int readint = bytes2int(readbytes);
								if (readint == 0)
									break;
								else
								{
									tempstring += "," + ReadPointer(stream, readint + 32);
									stream.Position += 4;
								}
							}
							outstring.Add(tempstring);
						}
					}
					else
					{
						savedata = false;
					}

					if (savedata)
					{
						string number = k.ToString();
						while (number.Length < 3)
							number = "0" + number;
						StreamWriter writer = new StreamWriter(extractfolder + "\\" + number + "%" + labels[k] + /*offsets[k].ToString() +*/ ".csv");
						for (int i = 0; i < outstring.Count; i++)
							writer.WriteLine(outstring[i]);
						writer.Close();
					}
				}

				#endregion

			}


		}

		public static void CompressFE10Data(string datapath, string extractfolder)
		{
			byte[] readbytes = new byte[4];
			int filesize, dataregionsize, num_pointers1, num_pointers2;
			int lines = 0;
			List<string> labels = new List<string>();
			List<int> dataoffsets = new List<int>();
			List<string> pointernames = new List<string>();
			List<int> pointerloc = new List<int>();

			string[] filepaths = Directory.GetFiles(extractfolder);
			string[] files = new string[filepaths.Length];

			for (int i = 0; i < filepaths.Length; i++)
				files[i] = Path.GetFileName(filepaths[i]).Split('%')[1].Split('.')[0];

			using (var stream = new FileStream(datapath, FileMode.Create, FileAccess.ReadWrite))
			{
				stream.Position = 0;
				// create blank header - will edit later
				for (int i = 0; i < 32; i++)
					stream.WriteByte(0x00);
				for (int i = 0; i < files.Length; i++)
				{
					if (!files[i].Contains("TerrainData"))
					{
						labels.Add(files[i]);
						dataoffsets.Add((int)stream.Position);
					}
					StreamReader reader = new StreamReader(filepaths[i]);
					string[] readlines = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
					reader.Close();

					if (files[i].Contains("DatabaseHead"))
					{
						for (int x = 0; x < 4; x++)
							stream.WriteByte(0x00);
						for (int j = 0; j < readlines.Length; j++)
						{
							if (readlines[j] != "")
							{
								pointernames.Add(readlines[j]);
								pointerloc.Add((int)stream.Position);
								// create blank pointer - will edit later
								for (int x = 0; x < 4; x++)
									stream.WriteByte(0x00);
							}
						}
					}
					else if (files[i].Contains("PersonData"))
					{
						// number of characters
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("PID_"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("PID_"))
									break;
							}
							// get name and skill count
							string name = inData[0][0];
							inData.RemoveAt(0);
							int numskills = 0;
							for (int x = 0; x < inData.Count; x++)
							{
								if (inData[x][0] == "Skills")
								{
									numskills = inData[x].Length - 1;
									break;
								}
							}
							// write first four bytes
							stream.WriteByte((byte)numskills);
							for (int x = 0; x < 3; x++)
							{
								stream.WriteByte(Convert.ToByte(inData[0][1]));
								inData.RemoveAt(0);
							}
							// write name pointer
							pointernames.Add(name);
							pointerloc.Add((int)stream.Position);
							// create blank pointer - will edit later
							for (int x = 0; x < 4; x++)
								stream.WriteByte(0x00);
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}

								}
							}

							stream.WriteByte(0x00);
						}
					}
					else if (files[i].Contains("JobData"))
					{
						// number of classes
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("JID_"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("JID_"))
									break;
							}
							// get skill count and classtype count
							int numskills = 0;
							int numclasstype = 0;
							for (int x = 0; x < inData.Count; x++)
							{
								if (inData[x][0] == "Skills")
								{
									numskills = inData[x].Length - 1;
								}
								if (inData[x][0] == "Class_Types")
								{
									numclasstype = inData[x].Length - 1;
									break;
								}
							}
							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}

								}
								// write skill count and classtype count
								if (inData[x][0] == "Mount_WT")
								{
									stream.WriteByte((byte)numskills);
									stream.WriteByte((byte)numclasstype);
								}
								// pad after FOW
								if (inData[x][0] == "FOW")
								{
									stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);
								}
							}

						}
					}
					else if (files[i].Contains("ItemData"))
					{
						// number of items
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("IID_"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("IID_"))
									break;
							}
							// get attribute, effectiveness count, and bonuses
							int numAttributes = 0;
							int numEffectiveness = 0;
							bool bonuses = false;
							for (int x = 0; x < inData.Count; x++)
							{
								if (inData[x][0] == "Attributes")
								{
									numAttributes = inData[x].Length - 1;
								}
								if (inData[x][0] == "Effectiveness")
								{
									numEffectiveness = inData[x].Length - 1;
								}
								if (inData[x][0] == "Bonuses")
								{
									bonuses = inData[x].Length > 1;
									break;
								}
							}
							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										if (inData[x][0] == "Cost/Use")
										{
											readbytes = int2bytes(Convert.ToInt32(inData[x][y]));
											stream.Write(readbytes, 2, 2);
										}
										else
											stream.WriteByte(Convert.ToByte(inData[x][y]));
									}

								}
								// pad
								if (inData[x][0] == "Unknown2")
								{
									stream.Write(new byte[] { 0x00, 0x00, 0x00 },0,3);
								}
								// numbers
								if (inData[x][0] == "Unknown3")
								{
									stream.WriteByte((byte)numAttributes);
									stream.WriteByte((byte)numEffectiveness);
									if (bonuses)
										stream.WriteByte(0x01);
									else
										stream.WriteByte(0x00);
								}
							}

						}
					}
					else if (files[i].Contains("SkillData"))
					{
						List<long> skills_pointerloc = new List<long>();
						List<byte[]> skillconditions = new List<byte[]>();
						List<string> skillcon_pointernames = new List<string>();
						List<long> skillcon_pointerloc = new List<long>();

						// number of skills
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("SID_"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("SID_"))
									break;
							}

							// get name and add to pointer2 labels
							string name = inData[0][0];
							labels.Add(name);
							dataoffsets.Add((int)stream.Position);

							// get condition counts
							int numCondition1 = 0;
							int numCondition2 = 0;
							for (int x = 0; x < inData.Count; x++)
							{
								if (inData[x][0] == "Condition_1")
								{
									numCondition1 = (inData[x].Length - 1) / 2;
								}
								if (inData[x][0] == "Condition_2")
								{
									numCondition2 = (inData[x].Length - 1) / 2;
									break;
								}
							}
							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								if (!inData[x][0].Contains("Condition"))
								{
									for (int y = 1; y < inData[x].Length; y++)
									{
										bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

										if (ispointer)
										{
											if (inData[x][y] != "")
											{
												pointernames.Add(inData[x][y]);
												pointerloc.Add((int)stream.Position);
											}
											// create blank pointer - will edit later
											for (int z = 0; z < 4; z++)
												stream.WriteByte(0x00);
										}
										else
										{
											if (inData[x][0] == "Cost/Use")
											{
												readbytes = int2bytes(Convert.ToInt32(inData[x][y]));
												stream.Write(readbytes, 2, 2);
											}
											else
												stream.WriteByte(Convert.ToByte(inData[x][y]));
										}
									}
								}
								else
								{
									if (inData[x].Length > 1)
									{
										skills_pointerloc.Add(stream.Position);
										pointerloc.Add((int)stream.Position);
										pointernames.Add("SKIP");
										List<byte> templist = new List<byte>();
										for (int y = 1; y < inData[x].Length; y++)
										{
											bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

											if (ispointer)
											{
												skillcon_pointernames.Add(inData[x][y]);
												int totalbytes = 0;
												for (int z = 0; z < skillconditions.Count; z++)
													totalbytes += skillconditions[z].Length;
												totalbytes += templist.Count;
												skillcon_pointerloc.Add(totalbytes);
												// create blank pointer - will edit later
												for (int z = 0; z < 4; z++)
													templist.Add(0x00);
											}
											else
											{
												templist.Add(Convert.ToByte(inData[x][y]));
												for (int z = 0; z < 3; z++)
													templist.Add(0x00);
											}
										}
										skillconditions.Add(templist.ToArray());
									}
									stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);
								}
								// numbers
								if (inData[x][0] == "Icon")
								{
									stream.WriteByte((byte)numCondition1);
									stream.WriteByte((byte)numCondition2);
								}
							}

						}

						for (int j = 0; j < skillcon_pointerloc.Count; j++)
						{
							long currpos = stream.Position;
							skillcon_pointerloc[j] += currpos;
							pointerloc.Add((int)skillcon_pointerloc[j]);
							pointernames.Add(skillcon_pointernames[j]);
						}
						for (int j = 0; j < skillconditions.Count; j++)
						{
							long prevpos = stream.Position;
							readbytes = int2bytes((int)stream.Position - 32);
							stream.Position = skills_pointerloc[j];
							stream.Write(readbytes,0,4);
							stream.Position = prevpos;
							stream.Write(skillconditions[j],0, skillconditions[j].Length);
						}

					}
					else if (files[i].Contains("RelianceParam"))
					{
						for (int j = 0; j < readlines.Length; j++)
						{
							Console.WriteLine(stream.Position.ToString());
							if (readlines[j] != "")
								stream.WriteByte(Convert.ToByte(readlines[j].Split(',')[1]));
						}
					}
					else if (files[i].Contains("DivineData"))
					{
						// number of entries
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("MT"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (line < readlines.Length - 1)
								{
									if (readlines[line + 1].StartsWith("MT"))
										break;
								}
							}
							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										if (inData[x][0] == "Cost/Use")
										{
											readbytes = int2bytes(Convert.ToInt32(inData[x][y]));
											stream.Write(readbytes, 2, 2);
										}
										else
											stream.WriteByte(Convert.ToByte(inData[x][y]));
									}

								}
							}
							stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);

						}
					}
					else if (files[i].Contains("GameData"))
					{
						string[] outputvals = readlines[0].Split(',');
						for (int k = 0; k < 136; k++)
						{
							stream.WriteByte(Convert.ToByte(outputvals[k]));
						}
					}
					else if (files[i].Contains("TerrainData"))
					{
						List<long> terraindataloc = new List<long>();
						List<string> terraindata_info = new List<string>();
						List<string> terrainpointerstring = new List<string>();

						// loop through each
						int line = 0;
						for (int j = 0; j < 199; j++)
						{
							Console.WriteLine(j.ToString() + " / " + 100.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								if (readlines[line] != "")
									inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (line < readlines.Length - 1)
								{
									if (readlines[line + 1].StartsWith("Name"))
										break;
								}
							}
							// save terrain information for later
							terrainpointerstring.Add(inData[0][1] + "," + inData[1][1] + "," + inData[14][1]);

							// see if this data matches previous data
							string terraindata = "";
							bool duplicate = false;
							for (int x = 2; x < inData.Count - 1; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									terraindata += inData[x][y];
								}
							}
							for (int x = 0; x < terraindata_info.Count; x++)
							{
								if (terraindata == terraindata_info[x])
								{
									terraindataloc.Add(terraindataloc[x]);
									duplicate = true;
									break;
								}
							}
							terraindata_info.Add(terraindata);
							if (!duplicate)
							{
								terraindataloc.Add(stream.Position);
								for (int x = 2; x < inData.Count - 1; x++)
								{
									for (int y = 1; y < inData[x].Length; y++)
									{
										bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

										if (ispointer)
										{
											if (inData[x][y] != "")
											{
												pointernames.Add(inData[x][y]);
												pointerloc.Add((int)stream.Position);
											}
											// create blank pointer - will edit later
											for (int z = 0; z < 4; z++)
												stream.WriteByte(0x00);
										}
										else
										{
											stream.WriteByte(Convert.ToByte(inData[x][y]));
										}

									}

								}
							}

						}

						for (int j = 0; j < 264; j++)
							stream.WriteByte(0x00);

						// now write pointer2
						labels.Add(files[i]);
						dataoffsets.Add((int)stream.Position);

						stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);

						for (int j = 0; j < terrainpointerstring.Count; j++)
						{
							string[] splitstring = terrainpointerstring[j].Split(',');
							for (int x = 0; x < 2; x++)
							{
								if (splitstring[x] != "")
								{
									pointernames.Add(splitstring[x]);
									pointerloc.Add((int)stream.Position);
								}
								// create blank pointer - will edit later
								stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);
							}
							pointernames.Add("SKIP");
							pointerloc.Add((int)stream.Position);
							stream.Write(int2bytes((int)terraindataloc[j] - 32),0,4);

							stream.WriteByte(Convert.ToByte(splitstring[2]));
							stream.Write(new byte[] { 0x00, 0x00, 0x00 },0,3);
						}
						stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);
					}
					else if (files[i].Contains("BattleTerrIndex"))
					{
						// number of entries
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("Name"))
								num++;
						readbytes = int2bytes(num);
						readbytes[1] = 0xC7;
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("Name"))
									break;
							}

							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									string teststring = inData[x][y];
									if (teststring[0] == '-')
										teststring = teststring.Remove(0, 1);

									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(teststring, @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										// saved as 4-byte value
										readbytes = int2bytes(Convert.ToInt32(inData[x][y]));
										stream.Write(readbytes,0,4);
									}

								}
							}

						}
					}
					else if (files[i].Contains("BattleTerrName"))
					{
						for (int j = 0; j < readlines.Length; j++)
						{
							if (readlines[j] != "")
							{
								pointernames.Add(readlines[j]);
								pointerloc.Add((int)stream.Position);
								// create blank pointer - will edit later
								for (int x = 0; x < 4; x++)
									stream.WriteByte(0x00);
							}
						}
					}
					else if (files[i].Contains("RelianceData"))
					{
						// number of entries
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("PID_"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								if (readlines[line] != "")
									inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("PID_"))
									break;
							}
							// name
							string name = inData[0][0];
							inData.RemoveAt(0);
							if (name != "")
							{
								pointernames.Add(name);
								pointerloc.Add((int)stream.Position);
							}
							// create blank pointer - will edit later
							stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);
							// number of supports
							readbytes = int2bytes(inData.Count);
							stream.Write(readbytes,0,4);

							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									string teststring = inData[x][y];
									if (teststring[0] == '-')
										teststring = teststring.Remove(0, 1);

									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(teststring, @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);
									}
									else
									{
										// saved as 2-byte value
										readbytes = int2bytes(Convert.ToInt32(inData[x][y]));
										stream.Write(readbytes, 2, 2);
										stream.WriteByte(0x00);
										stream.WriteByte(0x00);
									}

								}
							}

						}
					}
					else if (files[i].Contains("ChapterData"))
					{
						// number of entries
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("Mapname"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("Mapname"))
									break;
							}
							// name
							string name = inData[0][1];
							inData.RemoveAt(0);
							if (name != "")
							{
								pointernames.Add(name);
								pointerloc.Add((int)stream.Position);
							}
							// create blank pointer - will edit later
							stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);

							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}

								}
							}

						}
					}
					else if (files[i].Contains("GroupData"))
					{
						// number of items
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("IDname"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("IDname"))
									break;
							}

							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										// saved as 4-byte value
										readbytes = int2bytes(Convert.ToInt32(inData[x][y]));
										stream.Write(readbytes,0,4);
									}

								}
							}

						}
					}
					else if (files[i].Contains("KiznaData"))

					{
						// number of entries
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("Unknown"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (line < readlines.Length - 1)
								{
									if (readlines[line + 1].StartsWith("Unknown"))
										break;
								}
							}
							string name = inData[0][0];
							if (name != "")
							{
								pointernames.Add(name);
								pointerloc.Add((int)stream.Position);
							}
							// create blank pointer - will edit later
							stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);

							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}
								}
							}
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);

						}

						stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);

					}
					else if (files[i].Contains("DivineParam"))
					{
						// number of entries
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("Unknown"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (line < readlines.Length - 1)
								{
									if (readlines[line + 1].StartsWith("Unknown"))
										break;
								}
							}
							string name = inData[0][0];
							if (name != "")
							{
								pointernames.Add(name);
								pointerloc.Add((int)stream.Position);
							}
							// create blank pointer - will edit later
							stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);

							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}
								}
							}

						}
					}
					else if (files[i].Contains("SukumiData"))
					{
						// number of entries
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("MT_"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (line < readlines.Length - 1)
								{
									if (readlines[line + 1].StartsWith("MT_"))
										break;
								}
							}
							string name = inData[0][0];
							if (name != "")
							{
								pointernames.Add(name);
								pointerloc.Add((int)stream.Position);
							}
							// create blank pointer - will edit later
							stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);

							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}
								}
							}
							stream.WriteByte(0x00);
							stream.WriteByte(0x00);

						}
					}
					else if (files[i].Contains("BioData"))
					{
						for (int x = 1; x < readlines.Length; x++)
						{
							string[] inData = readlines[x].Split(',');
							for (int y = 0; y < inData.Length; y++)
							{
								if (inData[y] != "")
								{
									readbytes = float2bytes((float)Convert.ToDouble(inData[y]));
									stream.Write(readbytes,0,4);
								}
							}
						}
					}
					else if (files[i].Contains("BioParam"))
					{
						for (int x = 1; x < readlines.Length; x++)
						{
							if (readlines[x] != "")
							{
								string[] inData = readlines[x].Split(',');
								if (inData[0] != "")
								{
									readbytes = float2bytes((float)Convert.ToDouble(inData[0]));
									stream.Write(readbytes,0,4);
								}
								for (int y = 1; y < inData.Length; y++)
								{
									stream.WriteByte(Convert.ToByte(inData[y]));
								}
								stream.Write(new byte[] { 0x00, 0x00, 0x00 },0,3);
							}
						}
					}
					else if (files[i].Contains("NOBTL_TABLE"))
					{
						// number of entries
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("PID_"))
								num++;
						readbytes = int2bytes(num);
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("PID_"))
									break;
							}

							// get name and add to pointer2 labels
							string name = inData[0][0].Replace("PID", "NOBTL");
							labels.Add(name);
							dataoffsets.Add((int)stream.Position);

							if (inData[0][1] != "")
							{
								pointernames.Add(inData[0][1]);
								pointerloc.Add((int)stream.Position);
							}
							// create blank pointer - will edit later
							for (int z = 0; z < 4; z++)
								stream.WriteByte(0x00);
							inData.RemoveAt(0);

							// write all rows
							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}
									stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);

								}
							}
							stream.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 },0,4);

						}
					}
					else
					{
					}
					Console.WriteLine(files[i]);
				}

				// write pointernames to file
				List<string> filterednames = new List<string>();
				List<int> pointerdestination = new List<int>();
				for (int i = 0; i < pointernames.Count; i++)
					filterednames.Add(pointernames[i]);
				filterednames.Sort();
				// remove duplicates
				for (int i = 1; i < filterednames.Count; i++)
				{
					if (filterednames[i - 1] == filterednames[i])
					{
						filterednames.RemoveAt(i);
						i--;
					}
				}
				// turn strings into byte arrays
				List<byte[]> pointerbytes = new List<byte[]>();
				for (int i = 0; i < filterednames.Count; i++)
				{
					string[] splitname = filterednames[i].Split('-');
					bool listofbytes = splitname.Length > 1;
					for (int x = 0; x < splitname.Length; x++)
					{
						if (splitname[x] == "" | splitname[x].Contains("*"))
						{
							listofbytes = false;
							break;
						}
					}
					if (listofbytes)
						pointerbytes.Add(bytestring2byte(splitname));
					else
						pointerbytes.Add(Encoding.ASCII.GetBytes(filterednames[i]));
				}
				// sort by bytes
				List<byte[]> unsort_pointerbytes = new List<byte[]>();
				List<string> unsort_filterednames = new List<string>();
				for (int i = 0; i < pointerbytes.Count; i++)
				{
					unsort_pointerbytes.Add(pointerbytes[i]);
					unsort_filterednames.Add(filterednames[i]);
				}
				pointerbytes = sortByteArrayList(pointerbytes);
				for (int i = 0; i < pointerbytes.Count; i++)
					filterednames[pointerbytes.FindIndex(a => a == unsort_pointerbytes[i])] = unsort_filterednames[i];

				// write pointers
				for (int i = 0; i < filterednames.Count; i++)
				{
					pointerdestination.Add((int)stream.Position);
					if (filterednames[i] != "SKIP")
					{
						stream.Write(pointerbytes[i], 0, pointerbytes[i].Length);
						stream.WriteByte(0x00);
					}
				}
				// pad
				while (stream.Position % 4 != 0)
					stream.WriteByte(0x00);

				// write pointer regions to file
				dataregionsize = (int)stream.Position - 32;
				for (int i = 0; i < pointerloc.Count; i++)
					stream.Write(int2bytes(pointerloc[i] - 32),0,4);

				// alphabetize secondary pointers
				List<string> unsort_labels = new List<string>();
				List<int> unsort_offsets = new List<int>();
				for (int i = 0; i < labels.Count; i++)
				{
					unsort_labels.Add(labels[i]);
					unsort_offsets.Add(dataoffsets[i]);
				}
				labels.Sort();
				for (int i = 0; i < labels.Count; i++)
					dataoffsets[labels.FindIndex(a => a == unsort_labels[i])] = unsort_offsets[i];

				List<byte> endregion = new List<byte>();
				for (int i = 0; i < labels.Count; i++)
				{
					stream.Write(int2bytes(dataoffsets[i] - 32),0,4);
					stream.Write(int2bytes(endregion.Count),0,4);
					byte[] asciibytes = Encoding.ASCII.GetBytes(labels[i]);
					for (int x = 0; x < asciibytes.Length; x++)
						endregion.Add(asciibytes[x]);
					endregion.Add(0x00);
				}

				// write end region
				stream.Write(endregion.ToArray(), 0, endregion.ToArray().Length);
				filesize = (int)stream.Position;

				// go back and fill pointers
				for (int i = 0; i < pointerloc.Count; i++)
				{
					stream.Position = pointerloc[i];
					for (int j = 0; j < filterednames.Count; j++)
					{
						if (pointernames[i] == filterednames[j] & filterednames[j] != "SKIP")
						{
							stream.Write(int2bytes(pointerdestination[j] - 32), 0, 4);
						}
					}
				}

				// back to beginning and write header
				stream.Position = 0;
				stream.Write(int2bytes(filesize),0,4);
				stream.Write(int2bytes(dataregionsize),0,4);
				stream.Write(int2bytes(pointerloc.Count),0,4);
				stream.Write(int2bytes(labels.Count),0,4);



			}

		}

		#region
		/*
		public static void ExtractFE10Anim(string datapath, string extractfolder)
		{
			byte[] readbytes = new byte[4];
			int filesize, num_pointers1, num_pointers2;
			int header, dataregion, pointer1, pointer2, endregion;
			int size_header, size_dataregion, size_pointer1, size_pointer2, size_endregion;
			int pointerstart;
			List<string> labels = new List<string>();
			List<int> offsets = new List<int>();

			List<string> outstring = new List<string>();

			// header always is at the top, and it is always 32 bytes long
			header = 0;
			size_header = 32;
			dataregion = 32;

			string[] files = Directory.GetFiles(extractfolder);
			for (int i = 0; i < files.Length; i++)
				File.Delete(files[i]);

			using (var stream = new FileStream(datapath, FileMode.Open, FileAccess.ReadWrite))
			{
				// read header
				#region
				stream.Position = header;
				// full file size
				stream.Read(readbytes, 0, 4);
				filesize = bytes2int(readbytes);
				// data region size
				stream.Read(readbytes, 0, 4);
				size_dataregion = bytes2int(readbytes);
				// number of pointers in pointer section 1
				stream.Read(readbytes, 0, 4);
				num_pointers1 = bytes2int(readbytes);
				// number of pointers in pointer section 2
				stream.Read(readbytes, 0, 4);
				num_pointers2 = bytes2int(readbytes);

				// calculate other locations and sizes
				pointer1 = dataregion + size_dataregion;
				size_pointer1 = num_pointers1 * 4;
				pointer2 = pointer1 + size_pointer1;
				size_pointer2 = num_pointers2 * 8;
				endregion = pointer2 + size_pointer2;
				size_endregion = filesize - endregion;
				#endregion

				// read labels from pointer2
				#region
				stream.Position = pointer2;
				for (int i = 0; i < num_pointers2; i++)
				{
					stream.Read(readbytes, 0, 4);
					int dataoffset = bytes2int(readbytes);
					offsets.Add(dataoffset + 32);
					stream.Read(readbytes, 0, 4);
					int endoffset = bytes2int(readbytes);
					labels.Add(ReadPointer(stream, endregion + endoffset));
				}

				// order secondary pointers
				List<string> unsort_labels = new List<string>();
				List<int> unsort_offsets = new List<int>();
				for (int i = 0; i < labels.Count; i++)
				{
					unsort_labels.Add(labels[i]);
					unsort_offsets.Add(offsets[i]);
				}
				offsets.Sort();
				for (int i = 0; i < labels.Count; i++)
					labels[offsets.FindIndex(a => a == unsort_offsets[i])] = unsort_labels[i];
				#endregion

				// start reading dataregion
				#region
				for (int k = 0; k < offsets.Count; k++)
				{
					bool savedata = true;
					outstring.Clear();
					int pointer;
					string tempstring = labels[k];
					stream.Position = offsets[k];
					Console.WriteLine(stream.Position);

					if (labels[k].Contains("AnimHeader"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							stream.Read(readbytes, 0, 4);
							string namestring = ReadPointer(stream, bytes2int(readbytes) + 32);
							outstring.Add(namestring + "," + namestring);

							stream.Read(readbytes, 0, 4);
							outstring.Add("unknown," + ReadPointer(stream, bytes2int(readbytes) + 32));

							string[] captions = new string[4] { "unknown1", "ID", "unknown2", "unknown3" };
							for (int i = 0; i < 4; i++)
								outstring.Add(captions[i] + "," + stream.ReadByte().ToString());

							stream.Read(readbytes, 0, 4);
							outstring.Add("ymufolder," + ReadPointer(stream, bytes2int(readbytes) + 32));

							stream.Read(readbytes, 0, 4);
							long currpos = stream.Position;
							stream.Position = bytes2int(readbytes) + 32;
							stream.Read(readbytes, 0, 4);
							outstring.Add("dataName," + ReadPointer(stream, bytes2int(readbytes) + 32));
							stream.Position = currpos;
						}
					}
					else if (labels[k].Contains("AnimData"))
					{
						while (stream.Position < offsets[k + 1])
						{
							stream.Read(readbytes, 0, 4);
							string namestring = ReadPointer(stream, bytes2int(readbytes) + 32);
							outstring.Add("NAME," + namestring);

							stream.Read(readbytes, 0, 4);
							outstring.Add("sfx_move," + ReadPointer(stream, bytes2int(readbytes) + 32));
							stream.Read(readbytes, 0, 4);
							outstring.Add("sfx_floormove," + ReadPointer(stream, bytes2int(readbytes) + 32));
							stream.Read(readbytes, 0, 4);
							int temp = bytes2int(readbytes);
							if (temp == 0)
								outstring.Add("sfx_trans,");
							else
								outstring.Add("sfx_something," + ReadPointer(stream, temp + 32));

							tempstring = "unknownbytes";
							for (int i = 0; i < 8; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);

							outstring.Add("KEY,wait,move,twait,walk,atk,atk2,mag,mag2,crit,atku,atk2u,magu,mag2u,critu,atkd,atk2d,magd,mag2d,critd,arc,arc2,arc3,arc4,arc5," +
										  "rod,tack,avoid,poise,guard,damage,trans,dead,up,down,flip,flip2,evt1,evt2,evt3,evt4,evt5,evt6,evt7,evt8,evt9,evt10,evt11,evt12");

							string[] heldweap = new string[9] { "none", "sword", "lance", "javelin", "axe", "handaxe", "bow", "staff", "bowgun" };
							for (int j = 0; j < 9; j++)
							{
								tempstring = heldweap[j];
								for (int i = 0; i < 48; i++)
									tempstring += "," + stream.ReadByte().ToString();
								outstring.Add(tempstring);
							}
						}
					}
					else if (labels[k].Contains("AnimMotion"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							stream.Read(readbytes, 0, 4);
							string namestring = ReadPointer(stream, bytes2int(readbytes) + 32);
							outstring.Add("NAME," + namestring);

							stream.Read(readbytes, 0, 4);
							outstring.Add("unknown," + ReadPointer(stream, bytes2int(readbytes) + 32));

							tempstring = "unknownbytes";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("AnimWeapon"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							stream.Read(readbytes, 0, 4);
							string namestring = ReadPointer(stream, bytes2int(readbytes) + 32);
							outstring.Add("NAME," + namestring);

							stream.Read(readbytes, 0, 4);
							outstring.Add("unknown," + ReadPointer(stream, bytes2int(readbytes) + 32));
						}
					}
					else
					{
						savedata = false;
					}

					if (savedata)
					{
						string number = k.ToString();
						while (number.Length < 3)
							number = "0" + number;
						StreamWriter writer = new StreamWriter(extractfolder + "\\" + number + "%" + labels[k] +  ".csv");
						for (int i = 0; i < outstring.Count; i++)
							writer.WriteLine(outstring[i]);
						writer.Close();
					}
				}

				#endregion

			}


		}

		public static void CompressFE10Anim(string datapath, string extractfolder)
		{
			byte[] readbytes = new byte[4];
			int filesize, dataregionsize, num_pointers1, num_pointers2;
			int lines = 0;
			List<string> labels = new List<string>();
			List<int> dataoffsets = new List<int>();
			List<string> pointernames = new List<string>();
			List<int> pointerloc = new List<int>();

			List<string> datanames = new List<string>();
			List<int> datanamesloc = new List<int>();

			string[] filepaths = Directory.GetFiles(extractfolder);
			string[] files = new string[filepaths.Length];

			for (int i = 0; i < filepaths.Length; i++)
				files[i] = Path.GetFileName(filepaths[i]).Split('%')[1].Split('.')[0];

			using (var stream = new FileStream(datapath, FileMode.Create, FileAccess.ReadWrite))
			{
				stream.Position = 0;
				// create blank header - will edit later
				for (int i = 0; i < 32; i++)
					stream.WriteByte(0x00);
				for (int i = 0; i < files.Length; i++)
				{
					labels.Add(files[i]);
					dataoffsets.Add((int)stream.Position);
					StreamReader reader = new StreamReader(filepaths[i]);
					string[] readlines = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
					reader.Close();

					if (files[i].Contains("AnimHeader"))
					{
						// number of animations
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("AID_"))
								num++;
						readbytes = int2bytes(num);
						// only uses first two bytes to store number
						readbytes[0] = readbytes[2];
						readbytes[1] = readbytes[3];
						readbytes[2] = 0x00;
						readbytes[3] = 0x00;
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("AID_"))
									break;
							}


							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											if (inData[x][0] == "dataName")
											{
												datanames.Add(inData[x][y]);
												datanamesloc.Add((int)stream.Position);
												pointernames.Add("SKIP");
												pointerloc.Add((int)stream.Position);
											}
											else
											{
												pointernames.Add(inData[x][y]);
												pointerloc.Add((int)stream.Position);
											}
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}

								}
							}

						}

					}
					else if (files[i].Contains("AnimData"))
					{
						// number of animations
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("NAME"))
								num++;

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("NAME"))
									break;
							}


							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											if (inData[x][0] == "KEY")
											{
											}
											else
											{
												if (inData[x][0] == "NAME")
												{
													// go back into animheader and point to this location
													int currloc = (int)stream.Position;
													for (int z = 0; z < datanames.Count; z++)
													{
														if (inData[x][y] == datanames[z])
														{
															stream.Position = datanamesloc[z];
															readbytes = int2bytes(currloc - 32);
															stream.Write(readbytes,0,4);
														}
													}
													stream.Position = currloc;
												}
												pointernames.Add(inData[x][y]);
												pointerloc.Add((int)stream.Position);
												// create blank pointer - will edit later
												for (int z = 0; z < 4; z++)
													stream.WriteByte(0x00);
											}
										}
										else
										{
											// create blank pointer - will edit later
											for (int z = 0; z < 4; z++)
												stream.WriteByte(0x00);
										}
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}

								}
							}

						}

					}
					else if (files[i].Contains("AnimMotion"))
					{
						// number of animations
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("NAME"))
								num++;
						readbytes = int2bytes(num);
						// only uses first two bytes to store number
						readbytes[0] = readbytes[2];
						readbytes[1] = readbytes[3];
						readbytes[2] = 0x00;
						readbytes[3] = 0x00;
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("NAME"))
									break;
							}


							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}

								}
							}

						}
					}
					else if (files[i].Contains("AnimWeapon"))
					{
						// number of animations
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith("NAME"))
								num++;
						readbytes = int2bytes(num);
						// only uses first two bytes to store number
						readbytes[0] = readbytes[2];
						readbytes[1] = readbytes[3];
						readbytes[2] = 0x00;
						readbytes[3] = 0x00;
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith("NAME"))
									break;
							}


							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}

								}
							}

						}

					}
					else
					{
					}
					Console.WriteLine(files[i]);
				}

				// write pointernames to file
				List<string> filterednames = new List<string>();
				List<int> pointerdestination = new List<int>();
				for (int i = 0; i < pointernames.Count; i++)
					filterednames.Add(pointernames[i]);
				filterednames.Sort();
				// remove duplicates
				for (int i = 1; i < filterednames.Count; i++)
				{
					if (filterednames[i - 1] == filterednames[i])
					{
						filterednames.RemoveAt(i);
						i--;
					}
				}
				// turn strings into byte arrays
				List<byte[]> pointerbytes = new List<byte[]>();
				for (int i = 0; i < filterednames.Count; i++)
				{
					string[] splitname = filterednames[i].Split('-');
					bool listofbytes = splitname.Length > 1;
					for (int x = 0; x < splitname.Length; x++)
					{
						if (splitname[x] == "" | splitname[x].Contains("*"))
						{
							listofbytes = false;
							break;
						}
					}
					if (listofbytes)
						pointerbytes.Add(bytestring2byte(splitname));
					else
						pointerbytes.Add(Encoding.ASCII.GetBytes(filterednames[i]));
				}
				// sort by bytes
				List<byte[]> unsort_pointerbytes = new List<byte[]>();
				List<string> unsort_filterednames = new List<string>();
				for (int i = 0; i < pointerbytes.Count; i++)
				{
					unsort_pointerbytes.Add(pointerbytes[i]);
					unsort_filterednames.Add(filterednames[i]);
				}
				pointerbytes = sortByteArrayList(pointerbytes);
				for (int i = 0; i < pointerbytes.Count; i++)
					filterednames[pointerbytes.FindIndex(a => a == unsort_pointerbytes[i])] = unsort_filterednames[i];

				// write pointers
				for (int i = 0; i < filterednames.Count; i++)
				{
					pointerdestination.Add((int)stream.Position);
					if (filterednames[i] != "SKIP")
					{
						stream.Write(pointerbytes[i],0, pointerbytes[i].Length);
						stream.WriteByte(0x00);
					}
				}
				// pad
				while (stream.Position % 4 != 0)
					stream.WriteByte(0x00);

				// write pointer regions to file
				dataregionsize = (int)stream.Position - 32;
				for (int i = 0; i < pointerloc.Count; i++)
					stream.Write(int2bytes(pointerloc[i] - 32),0,4);

				// alphabetize secondary pointers
				List<string> unsort_labels = new List<string>();
				List<int> unsort_offsets = new List<int>();
				for (int i = 0; i < labels.Count; i++)
				{
					unsort_labels.Add(labels[i]);
					unsort_offsets.Add(dataoffsets[i]);
				}
				labels.Sort();
				for (int i = 0; i < labels.Count; i++)
					dataoffsets[labels.FindIndex(a => a == unsort_labels[i])] = unsort_offsets[i];

				List<byte> endregion = new List<byte>();
				for (int i = 0; i < labels.Count; i++)
				{
					stream.Write(int2bytes(dataoffsets[i] - 32),0,4);
					stream.Write(int2bytes(endregion.Count),0,4);
					byte[] asciibytes = Encoding.ASCII.GetBytes(labels[i]);
					for (int x = 0; x < asciibytes.Length; x++)
						endregion.Add(asciibytes[x]);
					endregion.Add(0x00);
				}

				// write end region
				stream.Write(endregion.ToArray(),0, endregion.ToArray().Length);
				filesize = (int)stream.Position;

				// go back and fill pointers
				for (int i = 0; i < pointerloc.Count; i++)
				{
					stream.Position = pointerloc[i];
					for (int j = 0; j < filterednames.Count; j++)
					{
						if (pointernames[i] == filterednames[j] & filterednames[j] != "SKIP")
						{
							stream.Write(int2bytes(pointerdestination[j] - 32));
						}
					}
				}

				// back to beginning and write header
				stream.Position = 0;
				stream.Write(int2bytes(filesize),0,4);
				stream.Write(int2bytes(dataregionsize),0,4);
				stream.Write(int2bytes(pointerloc.Count),0,4);
				stream.Write(int2bytes(labels.Count),0,4);



			}

		}


		public static void ExtractFE10Battle(string datapath, string extractfolder)
		{
			byte[] readbytes = new byte[4];
			int filesize, num_pointers1, num_pointers2;
			int header, dataregion, pointer1, pointer2, endregion;
			int size_header, size_dataregion, size_pointer1, size_pointer2, size_endregion;
			int pointerstart;
			List<string> labels = new List<string>();
			List<int> offsets = new List<int>();

			List<string> outstring = new List<string>();

			// header always is at the top, and it is always 32 bytes long
			header = 0;
			size_header = 32;
			dataregion = 32;

			string[] files = Directory.GetFiles(extractfolder);
			for (int i = 0; i < files.Length; i++)
				File.Delete(files[i]);

			using (var stream = new FileStream(datapath, FileMode.Open, FileAccess.ReadWrite))
			{
				// read header
				#region
				stream.Position = header;
				// full file size
				stream.Read(readbytes, 0, 4);
				filesize = bytes2int(readbytes);
				// data region size
				stream.Read(readbytes, 0, 4);
				size_dataregion = bytes2int(readbytes);
				// number of pointers in pointer section 1
				stream.Read(readbytes, 0, 4);
				num_pointers1 = bytes2int(readbytes);
				// number of pointers in pointer section 2
				stream.Read(readbytes, 0, 4);
				num_pointers2 = bytes2int(readbytes);

				// calculate other locations and sizes
				pointer1 = dataregion + size_dataregion;
				size_pointer1 = num_pointers1 * 4;
				pointer2 = pointer1 + size_pointer1;
				size_pointer2 = num_pointers2 * 8;
				endregion = pointer2 + size_pointer2;
				size_endregion = filesize - endregion;
				#endregion

				// read labels from pointer2
				#region
				stream.Position = pointer2;
				for (int i = 0; i < num_pointers2; i++)
				{
					stream.Read(readbytes, 0, 4);
					int dataoffset = bytes2int(readbytes);
					offsets.Add(dataoffset + 32);
					stream.Read(readbytes, 0, 4);
					int endoffset = bytes2int(readbytes);
					labels.Add(ReadPointer(stream, endregion + endoffset));
				}

				// order secondary pointers
				List<string> unsort_labels = new List<string>();
				List<int> unsort_offsets = new List<int>();
				for (int i = 0; i < labels.Count; i++)
				{
					unsort_labels.Add(labels[i]);
					unsort_offsets.Add(offsets[i]);
				}
				offsets.Sort();
				for (int i = 0; i < labels.Count; i++)
					labels[offsets.FindIndex(a => a == unsort_offsets[i])] = unsort_labels[i];
				#endregion

				// start reading dataregion
				#region
				for (int k = 0; k < offsets.Count; k++)
				{
					bool savedata = true;
					outstring.Clear();
					int pointer;
					string tempstring = labels[k];
					stream.Position = offsets[k];
					Console.WriteLine(labels[k] + "," + stream.Position);

					if (labels[k].Contains("AnimData"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[5] { "AID", "PID", "JID", "groundanim", "flyinganim" };
							for (int y = 0; y < 5; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}
						}
					}
					else if (labels[k].Contains("FootEffect"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[6] { "Tile", "EID1", "EID2", "EID3", "EID4", "EID5" };
							for (int y = 0; y < 6; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}
						}
					}
					else if (labels[k].Contains("SFXList"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string writestring = "NAME,";
							stream.Read(readbytes, 0, 4);
							int tempnum = bytes2int(readbytes);
							if (tempnum != 0)
							{
								string namestring = ReadPointer(stream, tempnum + 32);
								writestring += namestring;
							}
							outstring.Add(writestring);

							tempstring = "unknownbytes";
							for (int i = 0; i < 16; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("SFXData"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[5] { "Tile", "SFX1", "SFX2", "SFX3", "SFX4" };
							for (int y = 0; y < 5; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}
						}
					}
					else if (labels[k].Contains("BSetup"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string writestring = "NAME,";
							stream.Read(readbytes, 0, 4);
							int tempnum = bytes2int(readbytes);
							if (tempnum != 0)
							{
								string namestring = ReadPointer(stream, tempnum + 32);
								writestring += namestring;
							}
							outstring.Add(writestring);

							tempstring = "unknownbytes";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("BWeaponType"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[2] { "NAME", "Weapontype" };
							for (int y = 0; y < 2; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}
						}
					}
					else if (labels[k].Contains("BWeapon"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[8] { "NAME", "Weapon", "Weapon2", "EID_ATK", "EID_CRIT", "EID_MAG", "EID_MAG_HEAD", "Unknown" };
							for (int y = 0; y < 8; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}
							tempstring = "unknownbytes";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("MapModel"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[2] { "NAME", "SND" };
							for (int y = 0; y < 2; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}
							tempstring = "unknownbytes";
							for (int i = 0; i < 16; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("MapData"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string writestring = "NAME,";
							stream.Read(readbytes, 0, 4);
							int tempnum = bytes2int(readbytes);
							if (tempnum != 0)
							{
								string namestring = ReadPointer(stream, tempnum + 32);
								writestring += namestring;
							}
							outstring.Add(writestring);
							tempstring = "unknownbytes";
							for (int i = 0; i < 12; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("BSky"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[2] { "NAME", "unknown" };
							for (int y = 0; y < 2; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}
						}
					}
					else if (labels[k].Contains("BSkill"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[5] { "NAME", "SID", "EID1", "EID2", "EID3" };
							for (int y = 0; y < 5; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}
						}
					}
					else if (labels[k].Contains("BMotion"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[2] { "NAME", "ymuMotion" };
							for (int y = 0; y < 2; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}

							tempstring = "unknownbytes";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("BFlags") | labels[k].Contains("BParam"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string writestring = "NAME,";
							stream.Read(readbytes, 0, 4);
							int tempnum = bytes2int(readbytes);
							if (tempnum != 0)
							{
								string namestring = ReadPointer(stream, tempnum + 32);
								writestring += namestring;
							}
							outstring.Add(writestring);
						}
					}
					else if (labels[k].Contains("BCamFileIndex") | labels[k].Contains("BCamFileRate"))
					{
						tempstring = "unknownbytes";
						for (int i = 0; i < 100; i++)
							tempstring += "," + stream.ReadByte().ToString();
						outstring.Add(tempstring);
					}
					else if (labels[k].Contains("BCamFile"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[2] { "NAME", "unknown" };
							for (int y = 0; y < 2; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}

							tempstring = "unknownbytes";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("BCamGroupIndex"))
					{
						for (int x = 0; x < 32; x++)
						{
							tempstring = "unknownbytes";
							for (int i = 0; i < 585; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("BCamGroup"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[2] { "NAME", "unknown" };
							for (int y = 0; y < 2; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}

							tempstring = "unknownbytes";
							for (int i = 0; i < 12; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("CamEffect"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string writestring = "NAME,";
							stream.Read(readbytes, 0, 4);
							int tempnum = bytes2int(readbytes);
							if (tempnum != 0)
							{
								string namestring = ReadPointer(stream, tempnum + 32);
								writestring += namestring;
							}
							outstring.Add(writestring);

							tempstring = "unknownbytes";
							for (int i = 0; i < 12; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("CamList"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[2] { "NAME", "unknown" };
							for (int y = 0; y < 2; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}

							tempstring = "unknownbytes";
							for (int i = 0; i < 64; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("CamType"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string writestring = "NAME,";
							stream.Read(readbytes, 0, 4);
							int tempnum = bytes2int(readbytes);
							if (tempnum != 0)
							{
								string namestring = ReadPointer(stream, tempnum + 32);
								writestring += namestring;
							}
							outstring.Add(writestring);

							tempstring = "unknownbytes";
							for (int i = 0; i < 4; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("BUnit"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[4] { "NAME", "Class", "EID1", "EID2" };
							for (int y = 0; y < 4; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}


							tempstring = "unknownbytes";
							for (int i = 0; i < 48; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);

							headers = new string[16] { "None", "Sword", "Lance", "Axe", "Dagger", "Javelin", "Handaxe", "Knife", "Windsword", "Magic", "Bow", "Bowgun", "Laguz", "Breath", "Rod", "unk" };
							for (int y = 0; y < 16; y++)
							{
								tempstring = headers[y];
								for (int i = 0; i < 16; i++)
									tempstring += "," + stream.ReadByte().ToString();
								outstring.Add(tempstring);
							}

							tempstring = "unknownbytes";
							for (int i = 0; i < 20; i++)
								tempstring += "," + stream.ReadByte().ToString();
							outstring.Add(tempstring);
						}
					}
					else if (labels[k].Contains("BTex"))
					{
						readbytes[0] = 0x00;
						readbytes[1] = 0x00;
						stream.Read(readbytes, 2, 2);
						stream.Position += 2;
						int numchar = bytes2int(readbytes);
						//outstring.Add(tempstring);
						for (int x = 0; x < numchar; x++)
						{
							string[] headers = new string[2] { "NAME", "Class" };
							for (int y = 0; y < 2; y++)
							{
								string writestring = headers[y] + ",";
								stream.Read(readbytes, 0, 4);
								int tempnum = bytes2int(readbytes);
								if (tempnum != 0)
								{
									string namestring = ReadPointer(stream, tempnum + 32);
									writestring += namestring;
								}
								outstring.Add(writestring);
							}
						}
					}
					else
					{
						savedata = false;
					}

					if (savedata)
					{
						string number = k.ToString();
						while (number.Length < 3)
							number = "0" + number;
						StreamWriter writer = new StreamWriter(extractfolder + "\\" + number + "%" + labels[k] +  ".csv");
						for (int i = 0; i < outstring.Count; i++)
							writer.WriteLine(outstring[i]);
						writer.Close();
					}
				}

				#endregion

			}


		}
		public static void CompressFE10Battle(string datapath, string extractfolder)
		{
			byte[] readbytes = new byte[4];
			int filesize, dataregionsize, num_pointers1, num_pointers2;
			int lines = 0;
			List<string> labels = new List<string>();
			List<int> dataoffsets = new List<int>();
			List<string> pointernames = new List<string>();
			List<int> pointerloc = new List<int>();

			string[] filepaths = Directory.GetFiles(extractfolder);
			string[] files = new string[filepaths.Length];

			for (int i = 0; i < filepaths.Length; i++)
				files[i] = Path.GetFileName(filepaths[i]).Split('%')[1].Split('.')[0];

			using (var stream = new FileStream(datapath, FileMode.Create, FileAccess.ReadWrite))
			{
				stream.Position = 0;
				// create blank header - will edit later
				for (int i = 0; i < 32; i++)
					stream.WriteByte(0x00);
				for (int i = 0; i < files.Length; i++)
				{
					labels.Add(files[i]);
					dataoffsets.Add((int)stream.Position);
					StreamReader reader = new StreamReader(filepaths[i]);
					string[] readlines = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
					reader.Close();

					if (files[i].Contains("CamFileIndex") | files[i].Contains("CamFileRate") | files[i].Contains("CamGroupIndex"))
					{
						for (int j = 0; j < readlines.Length; j++)
						{
							string[] splitvals = readlines[j].Split(',');
							for (int k = 1; k < splitvals.Length; k++)
							{
								stream.WriteByte(Convert.ToByte(splitvals[k]));
							}
						}
					}
					else if (files[i].Contains("FE10B"))
					{
						string headname;
						if (files[i].Contains("AnimData"))
							headname = "AID";
						else if (files[i].Contains("FootEffect") | files[i].Contains("SFXData"))
							headname = "Tile";
						else
							headname = "NAME";
						// number of animations
						int num = 0;
						for (int j = 0; j < readlines.Length; j++)
							if (readlines[j].StartsWith(headname))
								num++;
						readbytes = int2bytes(num);
						// only uses first two bytes to store number
						readbytes[0] = readbytes[2];
						readbytes[1] = readbytes[3];
						readbytes[2] = 0x00;
						readbytes[3] = 0x00;
						stream.Write(readbytes,0,4);

						// loop through each
						int line = 0;
						for (int j = 0; j < num; j++)
						{
							Console.WriteLine(j.ToString() + " / " + num.ToString());
							List<string[]> inData = new List<string[]>();
							while (line < readlines.Length)
							{
								inData.Add(readlines[line].Split(','));
								line++;
								if (line == readlines.Length)
									break;
								if (readlines[line].StartsWith(headname))
									break;
							}


							for (int x = 0; x < inData.Count; x++)
							{
								for (int y = 1; y < inData[x].Length; y++)
								{
									bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(inData[x][y], @"^\d+$");

									if (ispointer)
									{
										if (inData[x][y] != "")
										{
											pointernames.Add(inData[x][y]);
											pointerloc.Add((int)stream.Position);
										}
										// create blank pointer - will edit later
										for (int z = 0; z < 4; z++)
											stream.WriteByte(0x00);
									}
									else
									{
										stream.WriteByte(Convert.ToByte(inData[x][y]));
									}

								}
							}

						}

					}
					else
					{
					}
					Console.WriteLine(files[i]);
				}

				// write pointernames to file
				List<string> filterednames = new List<string>();
				List<int> pointerdestination = new List<int>();
				for (int i = 0; i < pointernames.Count; i++)
					filterednames.Add(pointernames[i]);
				filterednames.Sort();
				// remove duplicates
				for (int i = 1; i < filterednames.Count; i++)
				{
					if (filterednames[i - 1] == filterednames[i])
					{
						filterednames.RemoveAt(i);
						i--;
					}
				}
				// turn strings into byte arrays
				List<byte[]> pointerbytes = new List<byte[]>();
				for (int i = 0; i < filterednames.Count; i++)
				{
					string[] splitname = filterednames[i].Split('-');
					bool listofbytes = splitname.Length > 1;
					for (int x = 0; x < splitname.Length; x++)
					{
						if (splitname[x] == "" | splitname[x].Contains("*"))
						{
							listofbytes = false;
							break;
						}
					}
					if (listofbytes)
						pointerbytes.Add(bytestring2byte(splitname));
					else
						pointerbytes.Add(Encoding.ASCII.GetBytes(filterednames[i]));
				}
				// sort by bytes
				List<byte[]> unsort_pointerbytes = new List<byte[]>();
				List<string> unsort_filterednames = new List<string>();
				for (int i = 0; i < pointerbytes.Count; i++)
				{
					unsort_pointerbytes.Add(pointerbytes[i]);
					unsort_filterednames.Add(filterednames[i]);
				}
				pointerbytes = sortByteArrayList(pointerbytes);
				for (int i = 0; i < pointerbytes.Count; i++)
					filterednames[pointerbytes.FindIndex(a => a == unsort_pointerbytes[i])] = unsort_filterednames[i];

				// write pointers
				for (int i = 0; i < filterednames.Count; i++)
				{
					pointerdestination.Add((int)stream.Position);
					if (filterednames[i] != "SKIP")
					{
						stream.Write(pointerbytes[i]);
						stream.WriteByte(0x00);
					}
				}
				// pad
				while (stream.Position % 4 != 0)
					stream.WriteByte(0x00);

				// write pointer regions to file
				dataregionsize = (int)stream.Position - 32;
				for (int i = 0; i < pointerloc.Count; i++)
					stream.Write(int2bytes(pointerloc[i] - 32));

				// alphabetize secondary pointers
				List<string> unsort_labels = new List<string>();
				List<int> unsort_offsets = new List<int>();
				for (int i = 0; i < labels.Count; i++)
				{
					unsort_labels.Add(labels[i]);
					unsort_offsets.Add(dataoffsets[i]);
				}
				labels.Sort();
				for (int i = 0; i < labels.Count; i++)
					dataoffsets[labels.FindIndex(a => a == unsort_labels[i])] = unsort_offsets[i];

				List<byte> endregion = new List<byte>();
				for (int i = 0; i < labels.Count; i++)
				{
					stream.Write(int2bytes(dataoffsets[i] - 32),0,4);
					stream.Write(int2bytes(endregion.Count),0,4);
					byte[] asciibytes = Encoding.ASCII.GetBytes(labels[i]);
					for (int x = 0; x < asciibytes.Length; x++)
						endregion.Add(asciibytes[x]);
					endregion.Add(0x00);
				}

				// write end region
				stream.Write(endregion.ToArray());
				filesize = (int)stream.Position;

				// go back and fill pointers
				for (int i = 0; i < pointerloc.Count; i++)
				{
					stream.Position = pointerloc[i];
					for (int j = 0; j < filterednames.Count; j++)
					{
						if (pointernames[i] == filterednames[j] & filterednames[j] != "SKIP")
						{
							stream.Write(int2bytes(pointerdestination[j] - 32));
						}
					}
				}

				// back to beginning and write header
				stream.Position = 0;
				stream.Write(int2bytes(filesize),0,4);
				stream.Write(int2bytes(dataregionsize),0,4);
				stream.Write(int2bytes(pointerloc.Count),0,4);
				stream.Write(int2bytes(labels.Count),0,4);



			}

		}


		public static void ExtractScript(string scriptpath, string csvpath)
		{
			byte[] readbytes = new byte[4];
			List<string> outstring = new List<string>();
			List<int> commandlocs = new List<int>();

			using (var stream = new FileStream(scriptpath, FileMode.Open, FileAccess.ReadWrite))
			{
				// name
				string name = ReadPointer(stream, 4);
				outstring.Add(name);
				// location of the start of command pointers
				stream.Position = 40;
				stream.Read(readbytes, 0, 4);
				stream.Position = ReversePointer(readbytes);

				stream.Read(readbytes, 0, 4);
				while (!(readbytes[0] == 0 & readbytes[1] == 0 & readbytes[2] == 0 & readbytes[3] == 0))
				{
					commandlocs.Add(ReversePointer(readbytes));
					stream.Read(readbytes, 0, 4);
				}

				for (int i = 0; i < commandlocs.Count; i++)
				{
					stream.Position = commandlocs[i];

					int nameloc = -1;
					int dataloc = -1;
					stream.Read(readbytes, 0, 4);
					if (!(readbytes[0] == 0 & readbytes[1] == 0 & readbytes[2] == 0 & readbytes[3] == 0))
						nameloc = ReversePointer(readbytes);
					stream.Read(readbytes, 0, 4);
					if (!(readbytes[0] == 0 & readbytes[1] == 0 & readbytes[2] == 0 & readbytes[3] == 0))
						dataloc = ReversePointer(readbytes);

					if (nameloc == -1)
						nameloc = dataloc;

					string tempstring = "";
					while (stream.Position < nameloc)
						tempstring += "," + stream.ReadByte().ToString();
					outstring.Add("headerbytes" + tempstring);

					tempstring = "";
					while (stream.Position < dataloc)
						tempstring += "," + stream.ReadByte().ToString();
					outstring.Add("namebytes" + tempstring);

					stream.Position = dataloc;
					tempstring = "";
					while (true)
					{
						if (i < commandlocs.Count - 1)
						{
							if (stream.Position >= commandlocs[i + 1])
							{
								outstring.Add(tempstring);
								break;
							}
						}
						else
						{
							if (stream.Position >= stream.Length)
							{
								outstring.Add(tempstring);
								break;
							}
						}


						int pointloc, pointloc2;
						string pointname;
						byte currbyte = (byte)stream.ReadByte();
						tempstring += "," + currbyte.ToString();
						switch (currbyte)
						{
							case 0x1C:
								// 1 byte pointer
								pointloc = stream.ReadByte();
								pointname = ReadPointer(stream, pointloc + 43);
								if (pointname == "")
								{
									pointname = ReadPointer(stream, pointloc + 44);
									tempstring += "," + pointname;
								}
								else
								{
									stream.Position -= 1;
								}
								break;
							case 0x1D:
								// 2 byte pointer
								readbytes[0] = 0x00;
								readbytes[1] = 0x00;
								stream.Read(readbytes, 2, 2);
								pointloc = bytes2int(readbytes);
								pointname = ReadPointer(stream, pointloc + 43);
								if (pointname == "")
								{
									pointname = ReadPointer(stream, pointloc + 44);
									tempstring += "," + pointname;
								}
								else
								{
									stream.Position -= 2;
								}
								break;
							case 0x38:
								// function pointer (2 bytes)
								pointloc = stream.ReadByte();
								pointloc2 = stream.ReadByte();
								pointloc = bytes2int(new byte[4] { 0x00, 0x00, (byte)pointloc, (byte)pointloc2 });
								pointname = ReadPointer(stream, pointloc + 44);
								tempstring += "," + pointname;
								break;
							case 0x19:
								// single byte int
								pointloc = stream.ReadByte();
								tempstring += "," + pointloc;
								break;
							case 0x1A:
								// single byte int
								readbytes[0] = 0x00;
								readbytes[1] = 0x00;
								stream.Read(readbytes, 2, 2);
								pointloc = bytes2int(readbytes);
								tempstring += "," + pointloc;
								break;
							case 0x20:
							case 0x47:
								// end of line
								outstring.Add(tempstring);
								tempstring = "";
								break;
						}
					}

					outstring.Add("");

				}

			}

			StreamWriter writer = new StreamWriter(csvpath);
			for (int i = 0; i < outstring.Count; i++)
				writer.WriteLine(outstring[i]);
			writer.Close();
		}

		public static void CompressScript(string scriptpath, string csvpath)
		{
			byte[] readbytes = new byte[4];
			int numcommands = 0;
			int[] commandloc;
			List<int> commandindex = new List<int>();
			List<string> pointernames = new List<string>();
			List<int> pointerlength = new List<int>();
			List<string> sortedpointers = new List<string>();
			List<int> pointerloc = new List<int>();
			int endofpointers;

			// read in all data
			StreamReader reader = new StreamReader(csvpath);
			string[] readlines = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
			reader.Close();

			// save list of pointers
			for (int i = 1; i < readlines.Length; i++)
			{
				if (readlines[i].StartsWith("headerbytes"))
				{
					commandindex.Add(i);
					numcommands += 1;
				}
				string[] splitline = readlines[i].Split(',');
				if (splitline.Length < 2)
				{ }
				else
				{
					for (int j = 1; j < splitline.Length; j++)
					{
						bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(splitline[j], @"^\d+$");
						if (ispointer)
						{
							bool writethis = true;
							for (int x = 0; x < pointernames.Count; x++)
							{
								if (splitline[j] == pointernames[x])
								{
									writethis = false;
									break;
								}
							}
							if (writethis)
							{
								if (splitline[j - 1] == "28")
									pointerlength.Add(1);
								else
									pointerlength.Add(2);
								pointernames.Add(splitline[j]);
							}
						}
					}
				}
			}

			using (var stream = new FileStream(scriptpath, FileMode.Create, FileAccess.ReadWrite))
			{
				string cmb = "cmb";
				stream.Write(Encoding.ASCII.GetBytes(cmb),0, Encoding.ASCII.GetBytes(cmb).Length);
				stream.WriteByte(0x00);
				stream.Write(Encoding.ASCII.GetBytes(readlines[0].Split(',')[0]),0, Encoding.ASCII.GetBytes(readlines[0].Split(',')[0]).Length);
				while (stream.Position < 24)
					stream.WriteByte(0x00);
				stream.Write(new byte[4] { 0x24, 0x10, 0x06, 0x20 },0,4);
				while (stream.Position < 36)
					stream.WriteByte(0x00);
				stream.WriteByte(0x2C);
				while (stream.Position < 44)
					stream.WriteByte(0x00);

				// sort pointers
				for (int i = 0; i < pointernames.Count; i++)
				{
					if (pointerlength[i] == 1)
						sortedpointers.Add(pointernames[i]);
				}
				for (int i = 0; i < pointernames.Count; i++)
				{
					if (pointerlength[i] == 2)
						sortedpointers.Add(pointernames[i]);
				}
				for (int i = 0; i < pointernames.Count; i++)
				{
					pointernames[i] = sortedpointers[i];
				}

				List<byte[]> pointerbytes = new List<byte[]>();
				for (int i = 0; i < pointernames.Count; i++)
				{
					string[] splitname = pointernames[i].Split('-');
					bool listofbytes = splitname.Length > 1;
					for (int x = 0; x < splitname.Length; x++)
					{
						if (splitname[x] == "" | splitname[x].Contains("*"))
						{
							listofbytes = false;
							break;
						}
					}
					if (listofbytes)
						pointerbytes.Add(bytestring2byte(splitname));
					else
					{
						string tempy = pointernames[i];
						tempy = tempy.Replace('|', ',');
						pointerbytes.Add(Encoding.ASCII.GetBytes(tempy));
					}
				}

				// write pointers
				for (int i = 0; i < pointerbytes.Count; i++)
				{
					pointerloc.Add((int)stream.Position - 44);
					stream.Write(pointerbytes[i]);
					stream.WriteByte(0x00);
				}

				// pad
				while (stream.Position % 4 != 0)
					stream.WriteByte(0x00);

				endofpointers = (int)stream.Position;

				// pad, will be pointers later
				for (int i = 0; i < numcommands + 1; i++)
					stream.Write(new byte[4] { 0x00, 0x00, 0x00, 0x00 });

				// save commands
				commandloc = new int[numcommands];
				for (int i = 0; i < numcommands; i++)
				{
					string header = readlines[commandindex[i]];
					string name = readlines[commandindex[i] + 1];
					commandloc[i] = (int)stream.Position;
					// name pointer
					if (name.Split(',').Length < 2)
						stream.Write(new byte[4] { 0x00, 0x00, 0x00, 0x00 });
					else
						stream.Write(int2ReverseBytes((int)stream.Position + 8 + header.Split(',').Length - 1));

					// datapointer
					if (name.Split(',').Length < 2)
						stream.Write(int2ReverseBytes((int)stream.Position + 4 + header.Split(',').Length - 1));
					else
						stream.Write(int2ReverseBytes((int)stream.Position + 4 + name.Split(',').Length - 1 + header.Split(',').Length - 1));

					// header
					string[] splitstring = header.Split(',');
					for (int j = 1; j < splitstring.Length; j++)
						stream.WriteByte(Convert.ToByte(splitstring[j]));
					// name
					if (name.Split(',').Length >= 2)
					{
						splitstring = name.Split(',');
						for (int j = 1; j < splitstring.Length; j++)
							stream.WriteByte(Convert.ToByte(splitstring[j]));
					}

					int x = 1;
					while (readlines[commandindex[i] + 1 + x] != "")
					{
						string[] splitline = readlines[commandindex[i] + 1 + x].Split(',');
						for (int j = 1; j < splitline.Length; j++)
						{
							bool ispointer = !System.Text.RegularExpressions.Regex.IsMatch(splitline[j], @"^\d+$");
							if (!ispointer)
							{
								switch (splitline[j])
								{
									case "28":
									case "XXXXXXXXXXXXXXXXXXXXX":
										// one byte pointer
										stream.WriteByte(Convert.ToByte(splitline[j]));
										//stream.WriteByte(0x1D);
										j += 1;
										for (int k = 0; k < pointernames.Count; k++)
										{
											if (pointernames[k] == splitline[j])
											{
												readbytes = int2bytes(pointerloc[k]);
												stream.Write(readbytes, 3, 1);
												break;
											}
											if (k == pointernames.Count - 1)
												j--;
										}
										break;
									//case "28":
									case "29":
										// two byte pointer
										int location = -1;

										j += 1;
										for (int k = 0; k < pointernames.Count; k++)
										{
											if (pointernames[k] == splitline[j])
											{
												location = pointerloc[k];
												break;
											}
										}
										if (location == -1)
										{
											stream.WriteByte(Convert.ToByte(splitline[j]));
											j--;
										}
										else if (true)//location > 255)
										{
											stream.WriteByte(0x1D);
											readbytes = int2bytes(location);
											stream.Write(readbytes, 2, 2);
										}
										else
										{
											stream.WriteByte(0x1C);
											stream.WriteByte((byte)location);
										}
										break;
									case "56":
										// two byte function pointer
										stream.WriteByte(Convert.ToByte(splitline[j]));
										j += 1;
										for (int k = 0; k < pointernames.Count; k++)
										{
											if (pointernames[k] == splitline[j])
											{
												readbytes = int2bytes(pointerloc[k]);
												stream.Write(readbytes, 2, 2);
												break;
											}
										}
										break;
									case "25":
										// one byte integer
										stream.WriteByte(Convert.ToByte(splitline[j]));
										j += 1;
										stream.WriteByte(Convert.ToByte(splitline[j]));
										break;
									case "26":
										// two byte integer
										stream.WriteByte(Convert.ToByte(splitline[j]));
										j += 1;
										readbytes = int2bytes(Convert.ToInt32(splitline[j]));
										stream.Write(readbytes, 2, 2);
										break;
									default:
										stream.WriteByte(Convert.ToByte(splitline[j]));
										break;

								}
							}

						}

						x++;
					}
					// pad
					//while (stream.Position % 4 != 0)
					//stream.WriteByte(0x00);

				}

				// save pointers back
				stream.Position = endofpointers;
				for (int i = 0; i < numcommands; i++)
					stream.Write(int2ReverseBytes(commandloc[i]));

				stream.Position = 40;
				stream.Write(int2ReverseBytes(endofpointers));

			}
		}

		*/
		#endregion


		static byte[] int2ReverseBytes(int inint)
		{
			byte[] newbytes = int2bytes(inint);
			byte[] revbytes = new byte[newbytes.Length];
			for (int i = 0; i < newbytes.Length; i++)
				revbytes[i] = newbytes[newbytes.Length - 1 - i];
			return (revbytes);
		}

		// reverses bytes, then calculates int
		static int ReversePointer(byte[] inbytes)
		{
			byte[] revbytes = new byte[inbytes.Length];
			for (int i = 0; i < inbytes.Length; i++)
				revbytes[i] = inbytes[inbytes.Length - 1 - i];
			return (bytes2int(revbytes));
		}

		// turns a byte array into an int (takes any size byte array)
		static int bytes2int(byte[] inbytes)
		{
			int outint = 0;
			Array.Reverse(inbytes);
			for (int i = 0; i < inbytes.Length; i++)
			{
				outint += inbytes[i] * (int)Math.Pow(256, i);
			}
			return outint;
		}

		// turns an int to a 4-byte value (always coerces to 4 size array)
		static byte[] int2bytes(int inint)
		{
			byte[] outbytes = new byte[4] { 0, 0, 0, 0 };
			long longint = inint;
			if (longint < 0)
				longint += 4294967296;
			for (int i = 0; i < outbytes.Length; i++)
			{
				while (longint >= (long)Math.Pow(256, outbytes.Length - 1 - i))
				{
					longint -= (long)Math.Pow(256, outbytes.Length - 1 - i);
					outbytes[i] += 1;
				}
			}
			return outbytes;
		}

		// turns a 4-byte array into a floating point value
		static float bytes2float(byte[] inbytes)
		{
			byte[] newArray = new[] { inbytes[3], inbytes[2], inbytes[1], inbytes[0] };
			return BitConverter.ToSingle(newArray, 0);
		}

		// turns a float value into a 4-byte array
		static byte[] float2bytes(float infloat)
		{
			byte[] outbyte = new byte[4];
			Buffer.BlockCopy(new float[] { infloat }, 0, outbyte, 0, 4);
			Array.Reverse(outbyte);
			return (outbyte);
		}

		// goes to location in stream and reads ASCII until a break (0x00)
		static string ReadPointer(FileStream stream, int location)
		{
			int x;
			List<byte> templist = new List<byte>();
			long origpos = stream.Position;
			stream.Position = location;
			while ((x = stream.ReadByte()) != 0x00 & x > 0)
			{
				templist.Add((byte)x);
			}
			stream.Position = origpos;
			bool ascii = true;
			for (int i = 0; i < templist.Count; i++)
			{
				if (templist[i] < 32 | templist[i] > 126)
				{
					ascii = false;
					break;
				}
			}
			if (templist.Count > 0)
			{
				string readstring;
				if (ascii)
				{
					for (int i = 0; i < templist.Count; i++)
						if (templist[i] == 0x2C)
							templist[i] = 0x7C;
					readstring = Encoding.ASCII.GetString(templist.ToArray());
					if (System.Text.RegularExpressions.Regex.IsMatch(readstring, @"^\d+$"))
						readstring = BitConverter.ToString(templist.ToArray());
				}
				else
					readstring = BitConverter.ToString(templist.ToArray());
				return (readstring);
			}
			else
				return ("");
		}

		static byte[] bytestring2byte(string[] bytestring)
		{
			byte[] output = new byte[bytestring.Length];
			char[] hexletters = { 'A', 'B', 'C', 'D', 'E', 'F' };
			for (int i = 0; i < bytestring.Length; i++)
			{
				int temp = 0;
				for (int j = 0; j < 2; j++)
				{
					if (Convert.ToInt32(bytestring[i][j]) > 64)
						temp += (int)((Convert.ToInt32(bytestring[i][j]) - 65 + 10) * Math.Pow(16, 1 - j));
					else
						temp += (int)((Convert.ToInt32(bytestring[i][j]) - 48) * Math.Pow(16, 1 - j));
				}
				output[i] = (byte)temp;
			}
			return (output);
		}

		static List<byte[]> sortByteArrayList(List<byte[]> inlist)
		{
			List<byte[]> mylist = new List<byte[]>();
			for (int i = 0; i < inlist.Count; i++)
				mylist.Add(inlist[i]);
			for (int i = 1; i < mylist.Count; i++)
			{
				for (int j = i - 1; j >= 0; j--)
				{
					bool swap = false;
					int length = 0;
					if (mylist[j].Length < mylist[j + 1].Length)
						length = mylist[j].Length;
					else
						length = mylist[j + 1].Length;

					for (int k = 0; k < length; k++)
					{
						if (mylist[j][k] > mylist[j + 1][k])
						{
							swap = true;
							break;
						}
						else if (mylist[j][k] < mylist[j + 1][k])
						{
							swap = false;
							break;
						}
						if (k == length - 1)
						{
							if (mylist[j].Length > mylist[j + 1].Length)
								swap = true;
						}
					}

					if (swap)
					{
						byte[] temp = mylist[j];
						mylist[j] = mylist[j + 1];
						mylist[j + 1] = temp;
					}
				}
			}
			return mylist;
		}
	}
}
