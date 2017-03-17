using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
//using System.Net;
using System.Web;
using System.Windows.Forms;
using MediaTags;


namespace SwiftMiX
{
    class ExportClass : IDisposable
    {
        //---------------------------------------------------------------------------
        internal const int EXPORT_PATH_RELATIVE = 0;
        internal const int EXPORT_PATH_ROOTED = 1;
        internal const int EXPORT_PATH_ABSOLUTE = 2;
        internal const int EXPORT_PATH_NONE = 3;

        private FormMain f1 = null;

        private const int XMLCODESLEN = 5;

        // Note: for XML just 5: &lt; (<), &amp; (&), &gt; (>), &quot; ("), and &apos; (')
        string[] XMLCODES = new string[XMLCODESLEN] { "&quot;", "&apos;", "&gt;", "&lt;", "&amp;" }; // place &amp last to make a serch/replace faster!

        // The index of the char in this string looks up its HTML replacement
        // string in the table above.
        string XMLCHARS = "\"\'><&";
        //---------------------------------------------------------------------------
        public void Dispose()
        {
            // We have no global resources...
        }
        //---------------------------------------------------------------------------
        public ExportClass(FormMain f)
        {
            f1 = f;
        }
        //---------------------------------------------------------------------------

        #region Export HTML-Style Playlist (WMP Compatible)

        public int Export(FormPlaylist f, string title, string playlist)
        //<smil>
        //    <head><title>This Is A Test</title></head>
        //    <body>
        //        <seq>
        //            <media src="F:\Music\Steely Dan\Countdown to Ecstasy\05 Show Biz Kids.wma"/>
        //            <media src="F:\Music\Steely Dan\Countdown to Ecstasy\06 My Old School.wma"/>
        //	</seq>
        //    </body>
        //</smil>
        {
            if (f == null) return (0);

            //if (!FormMain.FREEWARE && f1.pk.ComputeDaysRemaining() <= 0)
            //{
            //    MessageBox.Show("Trial Expired: " + FormMain.WEBSITE.ToString());
            //    return (0);
            //}

            int len = f.clbCount;

            if (len == 0)
            {
                MessageBox.Show("There are no items to export!");
                return (0);
            }

            int Count = 0;

            try
            {
                SaveFileDialog sd = new SaveFileDialog();

                sd.Title = title;
                sd.InitialDirectory = f1.DefaultFolderPath;
                sd.FileName = playlist;
                sd.Filter = "All Files (*.*)|*.*|" + // 1
                  "Windows Media (wpl)|*.wpl|" +
                  "MPEG UTF-8 (m3u8)|*.m3u8|" +
                  "MPEG ANSI (m3u)|*.m3u|" +
                  "Adv Stream XML (asx)|*.asx|" +
                  "XML Shareable (xspf)|*.xspf|" +
                  "Win Audio XML (wax)|*.wax|" +
                  "Windows XML (wmx)|*.wmx|" +
                  "Winamp (pls)|*.pls|" +
                  "Text (txt)|*.txt"; // 11
                sd.FilterIndex = 2; // wpl

                // File 1
                if (sd.ShowDialog() == DialogResult.Cancel) return -1;

                String uName = sd.FileName;

                if (String.IsNullOrEmpty(uName)) return -1;

                FormExport fe = new FormExport();

                String sPlayer = (f.Title == "Player A") ? "A" : "B";
                fe.Title = "Export Player " + sPlayer + " List"; // ANSI only!
                fe.FileName = uName;
                fe.Mode = EXPORT_PATH_ABSOLUTE;

                if (fe.ShowDialog() == DialogResult.Cancel) return -1;

                String fName = fe.FileName;
                bool bFileExists = File.Exists(fName);

                if (bFileExists)
                {
                    String sMsg = "File Already Exists:\n\n\"" + fName + "\"\n\nOverwrite it?";

                    DialogResult result1 = MessageBox.Show(sMsg,
                                               "SwiftMiX",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question,
                                                MessageBoxDefaultButton.Button1);

                    if (result1 == DialogResult.No) return -1;
                }

                // Gets the count of items exported
                Count = WriteXml(f, fName, fe.Mode, fe.SaveAsUtf8, fe.UncPathFormat);
            }
            catch
            {
                MessageBox.Show("Unable to Export list!");
            }

            return Count;
        }
        //---------------------------------------------------------------------------
        public int ExportNoDialog(List<string> sl, string fileName)
        {
            //if (!FormMain.FREEWARE && f1.pk.ComputeDaysRemaining() <= 0)
            //{
            //    MessageBox.Show("Trial Expired: " + FormMain.WEBSITE.ToString());
            //    return 0;
            //}

            //FormPlaylist f, String uListFullPath, int Mode, bool bSaveAsUtf8, bool bUncPathFormat
            return WriteXml(sl, fileName, 2, true, false); // filenames only
        }
        //---------------------------------------------------------------------------
        int WriteXml(FormPlaylist f, String listFullPath, int Mode, bool bSaveAsUtf8, bool bUncPathFormat)
        {
            if (f == null) return 0;

            int Count = f.clbCount;

            if (Count == 0) return 0;

            List<string> sl = new List<string>();

            if (sl == null) return 0;

            for (int ii = 0; ii < Count; ii++)
                sl.Add(f.clbItem(ii));

            return WriteXml(sl, listFullPath, Mode, bSaveAsUtf8, bUncPathFormat);
        }

        int WriteXml(List<string> slIn, String listFullPath, int Mode, bool bSaveAsUtf8, bool bUncPathFormat)
        //
        // uListFullPath must be UTF-8 (we use MainForm->WriteStringToFileW() to save the UTF-8 TStringList()...
        // (we use the vcl's ansi string-parsing functions on uListFullPath - they work on a utf-8 string
        // but not on a WideString)
        //
        // The bSaveAsUtf8 and bUncPathFormat flags pertain to the file-paths in the play-list
        //
        // Mode
        // EXPORT_PATH_RELATIVE       0
        // EXPORT_PATH_ROOTED         1
        // EXPORT_PATH_ABSOLUTE       2
        // EXPORT_PATH_NONE           3
        {
            if (slIn == null || slIn.Count == 0)
                return 0;

            List<string> slOut = new List<string>();

            if (slOut == null) return 0;

            int Count = 0;

            try
            {
                int len = slIn.Count;

                String Ext = Path.GetExtension(listFullPath).ToLower();

                if (Ext.Length > 0 && Ext.IndexOf(".") == 0)
                    Ext = Ext.Remove(0, 1);

                String sTemp;

                f1.ProgressBarValue = 0;

                if (Ext == "wpl")
                {
                    slOut.Add("<?wpl version=\"1.0\"?>");
                    slOut.Add("<smil>");
                    slOut.Add(" <head>");
                    // From a file generated by Windows Media Player:
                    // <meta name="Generator" content="Microsoft Windows Media Player -- 12.0.9600.17415"/>
                    // <meta name="ItemCount" content="85"/>
                    slOut.Add("   <meta name=\"Generator\" content=\"SwiftMiX Player -- " + FormMain.REVISION + "\"/>");
                    slOut.Add("   <title>" + Path.GetFileName(listFullPath) + "</title>");
                    slOut.Add(" </head>");
                    slOut.Add(" <body>");
                    slOut.Add("   <seq>");

                    for (int ii = 0; ii < len; ii++)
                    {
                        try
                        {
                            // After call, sName has the file-name only, sTemp has the full song-path
                            String sName = slIn[ii];

                            if (sName.Length == 0) continue;

                            // tack on a leading "file://", ProcessFileName will strip it off then add it back...
                            if (bUncPathFormat && !f1.IsUri(sName))
                                sName = sName.Insert(0, "file://");

                            sTemp = ProcessFileName(ref sName, listFullPath, Mode, bUncPathFormat);

                            if (sTemp != String.Empty)
                            {
                                sTemp = InsertXMLSpecialCodes(sTemp); // replace "&" with "&amp;"

                                slOut.Add("     <media src=\"" + sTemp + "\"/>");
                                Count++;
                            }
                        }
                        catch { }

                        f1.ProgressBarValue = (100 * ii) / len;
                    }

                    slOut.Add("   </seq>");
                    slOut.Add(" </body>");
                    slOut.Add("</smil>");
                }
                else if (Ext == "xspf") // Save as Windows-Media-Player XML file
                {
                    String sEnc = bSaveAsUtf8 ? "\"UTF-8\"" : "\"ANSI\"";
                    slOut.Add("<?xml version=\"1.0\" encoding=" + sEnc + "?>");
                    slOut.Add("<playlist version=\"1\" xmlns=\"http://xspf.org/ns/0/\">");
                    slOut.Add(" <tracklist>");

                    for (int ii = 0; ii < len; ii++)
                    {
                        try
                        {
                            // After call, sName has the file-name only, sTemp has the full song-path
                            String sName = slIn[ii];

                            if (sName.Length == 0) continue;

                            // tack on a leading "file://", ProcessFileName will strip it off then add it back...
                            if (bUncPathFormat && !f1.IsUri(sName))
                                sName = sName.Insert(0, "file://");

                            sTemp = ProcessFileName(ref sName, listFullPath, Mode, bUncPathFormat);

                            if (sTemp != String.Empty)
                            {
                                // properly do percent-encoding
                                // NOTE: for XSPF bUncPathFormat should ALWAYS be set!
                                if (bUncPathFormat)
                                {
                                    sTemp = sTemp.Replace("\\", "/");
                                    sTemp = PercentEncode(sTemp, true);

                                    //const string table = " !#$&'()*+,;=?@[]";

                                    //var uri = new Uri(sTemp);
                                    //sTemp = uri.AbsolutePath;

                                    //string sOut = String.Empty;
                                    //int tableLen = table.Length;
                                    //int tempLen = sTemp.Length;
                                    //for (int jj = 0; jj < tempLen; jj++)
                                    //{
                                    //    int kk;
                                    //    char c = sTemp[jj];
                                    //    for (kk = 0; kk < tableLen; kk++)
                                    //    {
                                    //        if (c == table[kk])
                                    //        {
                                    //            sOut += "%" + ((byte)c).ToString("X2");
                                    //            break;
                                    //        }
                                    //    }

                                    //    if (kk >= tableLen)
                                    //        sOut += c;
                                    //}
                                }
                                else
                                    sTemp = InsertXMLSpecialCodes(sTemp); // replace "&" with "&amp;"

                                slOut.Add("   <track>");
                                slOut.Add("     <title>" + sName + "</title>");
                                slOut.Add("     <location>" + sTemp + "</location>");
                                slOut.Add("   </track>");
                                Count++;
                            }
                        }
                        catch { }

                        f1.ProgressBarValue = (100 * ii) / len;
                    }

                    slOut.Add(" </tracklist>");
                    slOut.Add("</playlist>");
                }
                else if (Ext == "asx" || Ext == "wax" || Ext == "wmx") // Save as Windows-Media-Player XML file
                {
                    slOut.Add("<ASX version = \"3.0\">");
                    String sEnc = bSaveAsUtf8 ? "\"UTF-8\"" : "\"ANSI\"";
                    slOut.Add("   <PARAM name = \"encoding\" value = " + sEnc + " />");
                    slOut.Add("   <TITLE>" + Path.GetFileName(listFullPath) + "</TITLE>");

                    for (int ii = 0; ii < len; ii++)
                    {
                        try
                        {
                            // After call, sName has the file-name only, sTemp has the full song-path
                            String sName = slIn[ii];

                            if (sName.Length == 0) continue;

                            // tack on a leading "file://", ProcessFileName will strip it off then add it back...
                            if (bUncPathFormat && !f1.IsUri(sName))
                                sName = sName.Insert(0, "file://");

                            sTemp = ProcessFileName(ref sName, listFullPath, Mode, bUncPathFormat);

                            if (sTemp != String.Empty)
                            {
                                slOut.Add("   <ENTRY>");
                                slOut.Add("     <TITLE>" + sName + "</TITLE>");
                                slOut.Add("     <REF HREF = \"" + sTemp + "\" />");
                                slOut.Add("   </ENTRY>");
                                Count++;
                            }
                        }
                        catch { }

                        f1.ProgressBarValue = (100 * ii) / len;
                    }

                    slOut.Add("</ASX>");
                }
                else if (Ext == "pls") // Save as PLSv2 (Winamp)
                {
                    slOut.Add("[playlist]");

                    for (int ii = 0; ii < len; ii++)
                    {
                        try
                        {
                            // After call, sName has the file-name only, sTemp has the full song-path
                            String sName = slIn[ii];
                            sTemp = ProcessFileName(ref sName, listFullPath, Mode, bUncPathFormat);

                            if (!String.IsNullOrEmpty(sTemp))
                            {
                                String sCount = (Count + 1).ToString() + "=";
                                slOut.Add("File" + sCount + sTemp);
                                slOut.Add("Title" + sCount + Path.GetFileNameWithoutExtension(sTemp));
                                slOut.Add("Length" + sCount + "-1"); // ignore length (usually for streaming)
                                Count++;
                            }
                        }
                        catch { }

                        f1.ProgressBarValue = (100 * ii) / len;
                    }

                    slOut.Add("NumberOfEntries=" + Count);
                    slOut.Add("Version=2");
                }
                else // handle m3u and m3u8
                {
                    // Save as plain-text file
                    for (int ii = 0; ii < len; ii++)
                    {
                        try
                        {
                            // After call, sName has the file-name only, sTemp has the full song-path
                            String sName = slIn[ii];
                            sTemp = ProcessFileName(ref sName, listFullPath, Mode, bUncPathFormat);

                            if (sTemp != String.Empty)
                            {
                                slOut.Add(sTemp);
                                Count++;
                            }
                        }
                        catch { }

                        f1.ProgressBarValue = (100 * ii) / len;
                    }
                }

                if (slOut.Count > 0)
                {
                    Encoding enc = bSaveAsUtf8 ? Encoding.UTF8 : Encoding.Default;

                    if (!WriteSongListFile(slOut, listFullPath, enc))
                        MessageBox.Show("Problem writing to file!");
                }

                f1.ProgressBarValue = -1; // Hide
            }
            catch
            {
                MessageBox.Show("Error In NoDialog()");
            }

            return Count;
        }
        //---------------------------------------------------------------------------
        String PercentEncode(String sTemp, bool bEncodeAbove127)
        {
            byte[] ba = Encoding.UTF8.GetBytes(sTemp);

            // .XSPF Playlist Percent Encoding:
            // Can use these unencoded: A-Z, a-z, 0-9
            // Must %hex encode these (including SPACE) :/?#[]@ sub-delims !$&'()*+,;=
            // include SPACE, don't include -._~'/:
            byte[] baTable = Encoding.UTF8.GetBytes(" !#$&'()*+,;=?@[]");

            String sOut = String.Empty;
            int lenBa = ba.Length;
            int lenTable = baTable.Length;
            for (int ii = 0; ii < lenBa; ii++)
            {
                byte c = ba[ii];
                int jj;
                for (jj = 0; jj < lenTable; jj++)
                {
                    if (c == baTable[jj])
                    {
                        // hex encode SPACE, etc.
                        sOut += "%" + c.ToString("X2");
                        break;
                    }
                }

                // if not found in the special chars table, handle it below...
                if (jj >= lenTable)
                {
                    // hex encode the control chars
                    if (c < ' ' || (bEncodeAbove127 && c > 127))
                        sOut += "%" + c.ToString("X2");
                    else
                        sOut += (char)c;
                }
            }
            return sOut;
        }
        //---------------------------------------------------------------------------
        public bool WriteSongListFile(List<string> sl, string fileName, Encoding encoding)
        {
            //Write to file
            if (sl.Count == 0) return false;

            StreamWriter sw = new StreamWriter(fileName, false, encoding);

            if (sw == null) return false;

            try
            {
                foreach (string item in sl)
                    sw.WriteLine(item);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                sw.Close();
            }
        }
        //---------------------------------------------------------------------------
        String ProcessFileName(ref String sName, String uListFullPath, int Mode, bool bUncPathFormat)
        // sName is the full file path from the list-box. It can be in "file:/localhost/drive/path/file.ext" format
        // or like "C:\path\song.wma", "relative-path\song.wma", "\rooted-relative-path\song.wma"
        // ".\path\song.wma", "..\path\song.wma", "./path/song.mp3".
        //
        // sName is a reference which returns the File name only. We return the path we want to write into to the playlist
        {
            try
            {
                String sTemp = sName; // Save original song-path

                // Return the title (filename) in uName
                sName = Path.GetFileNameWithoutExtension(sTemp);

                // If it's a non-file URL like HTTP://, just return it as-is...
                if (f1.IsUri(sTemp) && !f1.IsFileUri(sTemp))
                    return sTemp;

                // Convert to "normal" file-path we can work with
                String sSavePrefix = StripFileUriPrefixIfAny(ref sTemp);

                // Ok, it's a file: URI. Convert to normal Windows path if needed and apply user path options...
                sTemp = GetFileString(uListFullPath, sTemp, Mode); // add user-selected path options

                if (bUncPathFormat)
                {
                    sTemp = sTemp.Replace('\\', '/');

                    // FYI: Looks like Microsoft metafiles that have relative links HAVE to
                    // be on the local server... then you access those playlists via an ASX
                    // metafile at the client which has a <ENTRYREF HREF = "" />
                    // that points to the remote playlist...
                    if (Mode != EXPORT_PATH_RELATIVE)
                        sTemp = sTemp.Insert(0, sSavePrefix); // put back the "file:/localhost/" part...
                    else if (sTemp.IndexOf("./") != 0) // don't keep adding more!
                        sTemp = sTemp.Insert(0, "./"); // this is required for UNIX paths (ok for Windows too!)
                }

                return sTemp;
            }
            catch
            {
                MessageBox.Show("Error In ProcessFileName()");
                return "";
            }
        }
        //---------------------------------------------------------------------------
        String GetFileString(String uListFullPath, String sSongFullPath, int Mode)
        // uListFullPath is the path/filename of the list we are writing to.
        // sSongFullPath is the path/filename of the song.
        //
        // Mode:
        // EXPORT_PATH_RELATIVE       0
        // EXPORT_PATH_ROOTED         1
        // EXPORT_PATH_ABSOLUTE       2
        // EXPORT_PATH_NONE           3
        //
        // Only call this with a path of the form "file:/localhost/drive/path/file.ext"
        //
        // This function converts the input path to a "standard" file path and applies
        // the user's chosen Mode to it. The return string is a "standard" path without
        // the "file:" prefix and with "/" changed to "\"
        //
        {
            // ANSI ReplaceAll should work ok on a UTF-8 path...
            sSongFullPath = sSongFullPath.Replace('/', '\\');

            try
            {
                String sTemp = ""; // default returned string...

                if (Mode == EXPORT_PATH_RELATIVE)
                {
                    // This function returns FileNameAndPath if the song is on a different drive than the list's RootPath
                    // The relative path returned has no leading backslash. "..\..\" are inserted automatically to go up.
                    try { sTemp = GetRelativePath(Path.GetDirectoryName(uListFullPath), sSongFullPath); }
                    catch { MessageBox.Show("Error 1 In GetFileString()"); }
                }
                else if (Mode == EXPORT_PATH_NONE)
                    sTemp = Path.GetFileName(sSongFullPath);
                else if (Mode == EXPORT_PATH_ROOTED)
                {
                    String sDrive = Path.GetPathRoot(sSongFullPath);

                    if (sDrive != String.Empty)
                    {
                        int pos = uListFullPath.IndexOf(sDrive);
                        if (pos >= 0)
                            sTemp = sSongFullPath.Replace(sDrive, "\\"); // strip drive
                    }
                }
                else // EXPORT_PATH_ABSOLUTE
                    sTemp = sSongFullPath;

                return sTemp;
            }
            catch { MessageBox.Show("Error 2 In GetFileString()"); return ""; }
        }
        //---------------------------------------------------------------------------
        // returns filespec relative to folder
        string GetRelativePath(string folder, string filespec)
        {
            Uri pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
                folder += Path.DirectorySeparatorChar;
            Uri folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
        //---------------------------------------------------------------------------
        //MakeRelativePath(@"c:\dev\foo\bar", @"c:\dev\junk\readme.txt")
        //returns: "..\..\junk\readme.txt"
        //
        //MakeRelativePath(@"c:\dev\foo\bar", @"c:\dev\foo\bar\docs\readme.txt")
        //returns: "docs\readme.txt"
        //
        // make fullPath relative to workingDirectory
        //public string MakeRelativePath(string workingDirectory, string fullPath)
        //{
        //    string result = string.Empty;
        //    int offset;

        //    // this is the easy case.  The file is inside of the working directory.
        //    if (fullPath.StartsWith(workingDirectory))
        //    {
        //        return fullPath.Substring(workingDirectory.Length + 1);
        //    }

        //    // the hard case has to back out of the working directory
        //    string[] baseDirs = workingDirectory.Split(new char[] { ':', '\\', '/' });
        //    string[] fileDirs = fullPath.Split(new char[] { ':', '\\', '/' });

        //    // if we failed to split (empty strings?) or the drive letter does not match
        //    if (baseDirs.Length <= 0 || fileDirs.Length <= 0 || baseDirs[0] != fileDirs[0])
        //    {
        //        // can't create a relative path between separate harddrives/partitions.
        //        return fullPath;
        //    }

        //    // skip all leading directories that match
        //    for (offset = 1; offset < baseDirs.Length; offset++)
        //    {
        //        if (baseDirs[offset] != fileDirs[offset])
        //            break;
        //    }

        //    // back out of the working directory
        //    for (int i = 0; i < (baseDirs.Length - offset); i++)
        //    {
        //        result += "..\\";
        //    }

        //    // step into the file path
        //    for (int i = offset; i < fileDirs.Length - 1; i++)
        //    {
        //        result += fileDirs[i] + "\\";
        //    }

        //    // append the file
        //    result += fileDirs[fileDirs.Length - 1];

        //    return result;
        //}
        //---------------------------------------------------------------------------
        String InsertXMLSpecialCodes(String sIn)
        {
            String sOut = "";

            // Special XML replacements...
            try
            {
                int len = sIn.Length;
                for (int ii = 0; ii < len; ii++)
                    sOut += XmlSpecialCharEncode(sIn[ii]);
            }
            catch { }

            return sOut;
        }
        //---------------------------------------------------------------------------
        String XmlSpecialCharEncode(char c)
        {
            for (int ii = 0; ii < XMLCODESLEN; ii++)
                if (XMLCHARS[ii] == c)
                    return XMLCODES[ii];

            return c.ToString();
        }
        //---------------------------------------------------------------------------
        String StripFileUriPrefixIfAny(ref String sIn)
        // Returns sIn by reference "as-is" or with the "file:/localhost/" part stripped.
        // The "normal" return string is empty if no changes were made to sIn or
        // it contains the "file:/localhost/" part so it can be restored if needed...
        {
            try
            {
                if (sIn.Length < 8) return "";
                String sTemp = sIn.Substring(0, 6).ToLower();
                if (sTemp != "file:/") return "";
                int len = sIn.Length;
                int ii = 6; // start looking for the next / after file:/
                for (; ii < len; ii++)
                    if (sIn[ii] == '/') break;
                if (ii == len) return ""; // did not find second '/'!
                sTemp = sIn.Substring(0, ii + 1);
                sIn = sIn.Substring(ii + 1, len - (ii + 1));
                return sTemp;
            }
            catch { return ""; }
        }
        //---------------------------------------------------------------------------
        #endregion
    }
}
