using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using Unicode;
using System.Drawing;

namespace SwiftMiX
{
    class ImportClass : IDisposable
    {
        FormMain f1 = null;
        Utf8Checker utf8Checker = null;

        const int XMLCODESLEN = 5;

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
        public ImportClass(FormMain f)
        {
            f1 = f;
            utf8Checker = new Utf8Checker();
        }
        //---------------------------------------------------------------------------
        public int Import(FormPlaylist f, string title)
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
            if (f == null)
                return 0;

            int Count = 0;

            //// Read tag info
            //MusicTagReader mtr = new MusicTagReader();
            //MP3SongInfo msi = mtr.GetInfo(fileName);

            ////#EXTINF:321,Example Artist - Example title
            ////Greatest Hits\Example.mp3
            //string seconds = "-1";
            //if (msi.Duration.Length == 8) // hh:mm:ss
            //{
            //  try
            //  {
            //    string str = msi.Duration.Replace(":", "");
            //    uint iHours = Convert.ToUInt32(str.Substring(0, 2));
            //    uint iMinutes = Convert.ToUInt32(str.Substring(2, 2));
            //    uint iSeconds = Convert.ToUInt32(str.Substring(4, 2));
            //    iSeconds += (iHours * 60 * 60) + (iMinutes * 60);
            //    seconds = iSeconds.ToString();
            //  }
            //  catch
            //  {
            //    seconds = "-1";
            //  }
            //}

            try
            {
                string fileName = string.Empty;

                using (OpenFileDialog od = new OpenFileDialog())
                {
                    od.Title = title;

                    od.Filter = "All Files (*.*)|*.*|" + // 1               
                                  "Windows Media (wpl)|*.wpl|" +
                                  "MPEG UTF-8 (m3u8)|*.m3u8|" +
                                  "MPEG ANSI (m3u)|*.m3u|" +
                                  "Adv Stream XML (asx)|*.asx|" +
                                  "XML Shareable (xspf)|*.xspf|" +
                                  "Win Audio XML (wax)|*.wax|" +
                                  "Windows XML (wmx)|*.wmx|" +
                                  "Windows Video (wvx)|*.wvx|" +
                                  "Winamp (pls)|*.pls|" +
                                  "Text (txt)|*.txt"; // 11

                    od.FilterIndex = 2; // .wpl

                    // File 1
                    if (od.ShowDialog() != DialogResult.OK)
                        return 0;

                    fileName = od.FileName;
                }

                var fi = new FormImport();
                if (fi.ShowDialog() == DialogResult.OK)
                    Count = ProcessSongListFile(fi.EncodingIndex, fileName, f);
            }
            catch
            {
                MessageBox.Show("Unable to Import list! (1)");
            }

            return Count;
        }
        //---------------------------------------------------------------------------
        public int ProcessSongListFile(int encodingIndex, string fileName, FormPlaylist fA, FormPlaylist fB = null)
        {
            if (!System.IO.File.Exists(fileName))
                return 0;

            int Count = 0;

            try
            {
                // If FormImport returned -1 as encodingIndex, We use utf8Checker to determine the encoding, otherwise
                // its been manually chosen... StreamReader converts from enc to Windows Unicode (UTF-16LE)

                Encoding enc;

                switch (encodingIndex)
                {
                    case -1: // Auto
                        enc = utf8Checker.Check(fileName) ? Encoding.UTF8 : Encoding.Default;
                        break;
                    case 0:
                        enc = Encoding.UTF8;
                        break;
                    case 1:
                        enc = Encoding.ASCII;
                        break;
                    case 2:
                        enc = Encoding.Unicode;
                        break;
                    case 3:
                        enc = Encoding.BigEndianUnicode;
                        break;
                    case 4:
                        enc = Encoding.UTF32;
                        break;
                    case 5:
                        enc = Encoding.UTF7;
                        break;
                    case 6:
                    default:
                        enc = Encoding.Default;
                        break;
                }

                var r = new StreamReader(fileName, enc);

                string sTemp = null;

                List<string> sl = new List<string>();

                // Read file into List<string>
                while (!r.EndOfStream && (sTemp = r.ReadLine()) != null)
                    sl.Add(sTemp);

                r.Close();

                string Ext = Path.GetExtension(fileName).ToLower();

                List<string> slSongsToAdd;

                if (Ext == ".pls")
                    slSongsToAdd = ReadIniFile(sl, fileName, fA, fB);
                else
                {
                    // Returns Count == 0 if not an XML file-extension
                    slSongsToAdd = XmlParser(Ext, sl, fileName, fA, fB);

                    // Nothing read? Try reading it as a plain-text file...
                    if (slSongsToAdd.Count == 0)
                        slSongsToAdd = ReadPlainText(sl, fileName, fA, fB);
                }

                if (slSongsToAdd.Count > 0)
                {
                    // here we have a new list of song-file-paths to add to one (or both) main play-lists...
                    // but we do NOT want to add songs that are duplicates... so we have to check for that!
                    foreach (var song in slSongsToAdd)
                    {
                        // Interleave one list between two players if fB is defined...
                        if (fB == null || (Count & 1) == 0)
                        {
                            if (!SongIsInList(fA, song))
                                if (AddFile(fA, song, fileName))
                                    Count++;
                        }
                        else
                        {
                            if (!SongIsInList(fB, song))
                                if (AddFile(fB, song, fileName))
                                Count++;
                        }
                    }
                }

                f1.ProgressBarValue = -1; // Hide progressbar

                // Set color
                if (fA != null)
                {
                    if (fA.clbCheckedCount == 0)
                        fA.clbColor = Color.Red;
                    else if (fA.clbCheckedCount == 1)
                        fA.clbColor = Color.Yellow;
                    else
                        fA.clbColor = Color.FromArgb(184, 207, 245);
                }

                if (fB != null)
                {
                    if (fB.clbCheckedCount == 0)
                        fB.clbColor = Color.Red;
                    else if (fB.clbCheckedCount == 1)
                        fB.clbColor = Color.Yellow;
                    else
                        fB.clbColor = Color.FromArgb(184, 207, 245);
                }
            }
            catch
            {
                MessageBox.Show("Unable to Import list (2)!");
            }

            return Count;
        }
        //---------------------------------------------------------------------------
        // Winamp playlist-files

        List<string> ReadIniFile(List<string> sl, string filePath, FormPlaylist fA, FormPlaylist fB)
        // The .pls file has already been read and converted from UTF-8 to UTF-16 if necessary by StreamReader
        // and is in sl... so all we need to do is parse out the URLs and add them to our player-list(s)
        //
        // Each Url starts with FileXXX=name or fileX="My Name" (that's what we handle here at least...)
        {
            List<string> slOut = new List<string>();

            try
            {
                bool bInPlaylistSection = false;

                for (int ii = 0; ii < sl.Count; ii++)
                {
                    String sTemp = sl[ii];

                    if (sTemp == null) continue;

                    sTemp = sTemp.Trim();

                    int len = sTemp.Length;

                    if (len < 7) continue; //  Minimum entry example: "file0=x"

                    if (bInPlaylistSection)
                    {
                        if (sTemp.Substring(0, 4).ToLower() != "file") continue;

                        bool bHaveDigit = false;

                        for (int n = 4; n < len; n++)
                        {
                            if (bHaveDigit)
                            {
                                if (sTemp[n] == '=' && n + 1 < len)
                                {
                                    String sUrl = sTemp.Substring(n + 1, len - (n + 1));
                                    sUrl = sUrl.Trim();
                                    int urlLen = sUrl.Length;
                                    if (urlLen > 2 && sUrl.StartsWith("\"") && sUrl.EndsWith("\""))
                                    {
                                        sUrl = sUrl.Substring(1, urlLen - 2); // trim off the quotes
                                        // sUrl = sUrl.Trim(); leave quoted text as-is
                                        urlLen = sUrl.Length;
                                    }

                                    if (urlLen == 0) continue;

                                    try
                                    {
                                        if (ReplaceRelativePath(ref sUrl, filePath)) // returns sUrl as an absolute path...
                                            slOut.Add(sUrl);
                                    }
                                    catch { }
                                }
                                else
                                    continue;
                            }

                            if (sTemp[n] >= '0' && sTemp[n] <= '9')
                                bHaveDigit = true;
                            else
                                continue;
                        }
                    }
                    else if (sTemp.ToLower() == "[playlist]")
                        bInPlaylistSection = true;
                }
            }
            catch { }

            return slOut;
        }
        //---------------------------------------------------------------------------
        List<string> ReadPlainText(List<string> sl, string filePath, FormPlaylist fA, FormPlaylist fB)
        {
            List<string> slOut = new List<string>();


            // Read as plain-text, one file or URL per line
            for (int ii = 0; ii < sl.Count; ii++)
            {
                try
                {
                    String sFile = sl[ii];

                    if (sFile.Length > 0)
                        sFile = sFile.Trim();

                    if (sFile.Length == 0 || sFile[0] == '#') continue; // Filter out .m3u info tags...

                    if (ReplaceRelativePath(ref sFile, filePath)) // returns sFile as an absolute path...
                        slOut.Add(sFile);
                }
                catch { }
            }

            return slOut;
        }
        //---------------------------------------------------------------------------
        bool SongIsInList(FormPlaylist f, string songPath)
        {
            foreach (string existingSong in f.clbItems)
                if (existingSong == songPath) return true;
            return false;
        }
        //---------------------------------------------------------------------------
        bool AddFile(FormPlaylist f, string songPath, string filePath)
        {
            if (songPath == null) return false;

            songPath = songPath.Trim();

            if (songPath.Length == 0) return false;

            // Go try to add file in several path combinations
            // until one works...

            if (f.clbAdd(songPath)) return true; // file "as-is"

            string temp = Path.GetDirectoryName(filePath) + Path.DirectorySeparatorChar + Path.GetFileName(songPath);

            if (f.clbAdd(temp)) return true; // path of playlist + file-name stripped of original path 

            temp = "." + Path.DirectorySeparatorChar + Path.GetFileName(songPath); // .\filename

            if (f.clbAdd(temp)) return true; // playlist file-name by itself

            temp = Path.GetPathRoot(songPath);

            if (temp.Length > 0 && temp.Length < songPath.Length)
            {
                songPath = "." + Path.DirectorySeparatorChar + songPath.Substring(temp.Length);

                if (f.clbAdd(songPath)) return true; // file "as-is" except with drive replaced by .\
            }

            return false;
        }
        //---------------------------------------------------------------------------
        List<string> XmlParser(String sExt, List<string> sl, String filePath, FormPlaylist fA, FormPlaylist fB)
        // sType has "href" "location" or "source", sIn has the raw file data...
        // sl is the listbox we output to, Returns Count of files added, 0 if error or no files
        //
        // WPL
        //<smil>
        //    <head><title>This Is A Test</title></head>
        //    <body>
        //        <seq>
        //            <media src="F:\Music\Steely Dan\Countdown &amp; Ecstasy\05 Show Biz Kids.wma"/>
        //            <media src="F:\Music\Steely Dan\Countdown &amp; Ecstasy\06 My Old School.wma"/>
        //        </seq>
        //    </body>
        //</smil>
        //
        // XSPF
        //<?xml version="1.0" encoding="UTF-8"?>
        //<playlist version="1" xmlns="http://xspf.org/ns/0/">
        //  <trackList>
        //    <track>
        //      <title>Windows Path</title>
        //      <location>file:///C:/music/foo.mp3</location>
        //    </track>
        //    <track>
        //      <title>Linux Path</title>
        //      <location>file:///media/music/foo.mp3</location>
        //    </track>
        //    <track>
        //      <title>Relative Path</title>
        //      <location>music/foo.mp3</location>
        //    </track>
        //    <track>
        //      <title>External Example</title>
        //      <location>http://www.example.com/music/bar.ogg</location>
        //    </track>
        //  </trackList>
        //</playlist>
        {
            String sType;

            if (String.IsNullOrEmpty(sExt))
                sType = "src";
            else
            {
                sExt = sExt.ToLower();

                if (sExt[0] == '.')
                    sExt = sExt.Remove(0, 1);

                if (sExt == "wpl")
                    sType = "src";
                else if (sExt == "xspf")
                    sType = "location";
                else if (sExt == "asx" || sExt == "wax" || sExt == "wmx" || sExt == "wvx")
                    sType = "href";
                else
                    sType = "src";
            }

            // Decode percent codes for any xml file
            bool bDecodePercentCodes = (sExt == "xspf" || sExt == "asx" || sExt == "wax" || sExt == "wmx" || sExt == "wvx") ? true : false;
            bool bDecodeXmlCodes = (sType == "href" || sExt == "wpl") ? true : false;

            List<string> slOut = new List<string>();

            try
            {
                String s = String.Join(String.Empty, sl); // concatinate all the strings and strip line-terminators

                int OriginalLength = s.Length;

                String sTag = "";
                String sUrl = "";

                bool bTagParse = false;
                bool bUrlParse = false;

                bool bProgressBarOn = OriginalLength > 1000;
                f1.ProgressBarValue = 0;

                for (int ii = 0; ii < OriginalLength; ii++)
                {
                    if (bTagParse)
                    {
                        if (s[ii] == '>')
                        {
                            // we have some sort of complete tag...

                            sTag = sTag.Trim();
                            int len = sTag.Length;

                            // tag is <xxx href> or <xxx location> or <xxx src>
                            if (len > 0 && sTag.IndexOf(sType, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                // come here for both an opening or a closing tag!
                                bool bTagHasUrl = (!bUrlParse && sTag[len - 1] == '/');
                                bool bClosingTag = sTag[0] == '/';

                                // if we have a url inside a single tag... <location="My Path"/>
                                // or if this is a closing tag following the url... <location>My Path</location>
                                if (bTagHasUrl || (bUrlParse && bClosingTag))
                                {
                                    if (bTagHasUrl)
                                        sUrl = sTag;

                                    // can be "<media source = "this &amp; file" />"
                                    // or "<ref href = "this file" />"
                                    // or "<location = 'this file'/>"
                                    // or "<location>this file</location>"
                                    // or "<source> "this file" </source>"
                                    // or "<ref HREF> 'this file' </href>"
                                    if (ParseFileLine(ref sUrl, bTagHasUrl)) // returns sUrl by-reference, cleaned-up...
                                    {
                                        sUrl = StripCrLfAndTrim(sUrl);

                                        bool bIsFileUri = f1.IsFileUri(sUrl);

                                        if (bIsFileUri && bDecodePercentCodes)
                                            sUrl = new Uri(sUrl).LocalPath; // use this!

                                        if (bDecodeXmlCodes)
                                            sUrl = ReplaceXmlCodes(sUrl);

                                        ReplaceRelativePath(ref sUrl, filePath); // returns sUrl as an absolute path...

                                        slOut.Add(sUrl);
                                    }

                                    bUrlParse = false;
                                }
                                else if (!bUrlParse && !bClosingTag)
                                {
                                    sUrl = "";
                                    bUrlParse = true; // start of url string next char
                                }
                            }

                            bTagParse = false;
                        }
                        else
                            sTag += s[ii];
                    }
                    else if (s[ii] == '<')
                    {
                        sTag = "";
                        bTagParse = true;
                    }
                    else if (bUrlParse)
                        sUrl += s[ii]; // accumulate Url text that's between two tags

                    // Reflect our progress through a large xml file
                    if (bProgressBarOn)
                        f1.ProgressBarValue = (100 * ii) / OriginalLength;
                }
            }
            catch { }

            f1.ProgressBarValue = -1; // Hide

            return slOut;
        }
        //---------------------------------------------------------------------------
        bool ParseFileLine(ref String sRef, bool bTagHasUrl)
        // if sTagHasUrl is set we expect an equals sign... (<href = "name.ext" />)
        // sRef is both the input and output string. Returns true if no errors.
        {
            if (String.IsNullOrEmpty(sRef)) return false;

            String sFile = "";

            try
            {
                int pos;
                String sQuote = "";

                if (bTagHasUrl)
                {
                    pos = sRef.IndexOf("=");
                    if (pos >= 0)
                        sRef = sRef.Remove(0, pos + 1); // delete up through "="
                }

                pos = sRef.IndexOf("\"");
                if (pos >= 0)
                {
                    sRef = sRef.Remove(0, pos + 1); // delete up through "
                    sQuote = "\"";
                }
                else
                {
                    pos = sRef.IndexOf("\'");
                    if (pos >= 0)
                    {
                        sRef = sRef.Remove(0, pos + 1); // delete up through '
                        sQuote = "\'";
                    }
                }

                if (!String.IsNullOrEmpty(sQuote))
                    pos = sRef.IndexOf(sQuote); // is there a trailing quote?
                else
                    pos = -1;

                if (pos >= 0)
                    sFile = sRef.Substring(0, pos); // file path without quotes...
                else
                {
                    sFile = sRef;
                    // erase leftover / from the "/>" ending
                    if (bTagHasUrl)
                        if (sFile[sFile.Length - 1] == '/')
                            sFile = sFile.Substring(0, sFile.Length - 1);
                }

                sRef = sFile;

                return true;
            }
            catch { return false; }
        }
        //---------------------------------------------------------------------------
        String StripCrLfAndTrim(String sIn)
        // Trim trailing spaces and remove cr/lf chars
        {
            String sOut = "";

            for (int ii = 0; ii < sIn.Length; ii++)
                if (sIn[ii] != '\r' && sIn[ii] != '\n')
                    sOut += sIn[ii];

            return sOut.Trim();
        }
        //---------------------------------------------------------------------------
        String ReplaceXmlCodes(String sIn)
        // Replace &amp; etc...
        {
            for (int ii = 0; ii < XMLCODESLEN; ii++)
                sIn = sIn.Replace(XMLCODES[ii], XMLCHARS[ii].ToString());
            return sIn;
        }
        //---------------------------------------------------------------------------
        //String ReplacePercentCodes(String sIn)
        //// Replace %20 etc...
        //{
        //    StringBuilder sNew = new StringBuilder(sIn);
        //    int len = sNew.Length;
        //    if (len == 0) return "";

        //    if (len >= 3)
        //    {
        //        for (int ii = 0; ii < len - 3; ii++)
        //        {
        //            if (sNew[ii] == '%' && mishex(sNew[ii + 1]) && mishex(sNew[ii + 2])) // have a %hh hex code?
        //            {
        //                int val;

        //                try { val = Convert.ToInt32(sNew[ii + 1].ToString() + sNew[ii + 2].ToString(), 16); }
        //                catch { val = -1; }

        //                if (val >= 0)
        //                {
        //                    // Replace %XX with an ANSI char (might be part of a UTF-8 sequence...)
        //                    sNew[ii] = (char)val;
        //                    sNew = sNew.Remove(ii + 1, 2);
        //                    len -= 2;
        //                }
        //            }
        //        }
        //    }
        //    return sNew.ToString();
        //}
        //---------------------------------------------------------------------------
        bool ReplaceRelativePath(ref String sUrl, String sPath)
        // Takes sUrl, from an imported play-list - which could be prefixed with "file://"
        // and have forward or back-slashes... and could be a relative path or rooted-relative
        // path - and converts it to a file-path with no "file://" prefix and delimited with
        // windows-style backslashes and using sPath (the path of the song-list file)
        // to return a full file-path with name and extension.
        //
        // NOTE: Uri. has many tools in C#...
        {
            try
            {
                bool bIsFileUri = f1.IsFileUri(sUrl);

                // If it's a non-file URL like HTTP://, just return it as-is...
                if (f1.IsUri(sUrl) && !bIsFileUri)
                    return true;

                // Save old path
                String sTemp = Directory.GetCurrentDirectory();

                // Set current directory to that of our play-list file
                Directory.SetCurrentDirectory(Path.GetDirectoryName(sPath));

                if (bIsFileUri)
                {
                    sUrl = sUrl.Remove(0, 5);
                    sUrl = sUrl.Replace('\\', '/'); // make slashes consistent

                    // delete leading //
                    if (sUrl.IndexOf("///.") == 0)
                        sUrl = sUrl.Remove(0, 3);
                    else if (sUrl.IndexOf("//.") == 0)
                        sUrl = sUrl.Remove(0, 2);
                    else if (sUrl.IndexOf("/.") == 0)
                        sUrl = sUrl.Remove(0, 1);
                    else if (sUrl.IndexOf("///") == 0)
                        sUrl = sUrl.Remove(0, 3);
                    else if (sUrl.IndexOf("//") == 0)
                        sUrl = sUrl.Remove(0, 2);
                }
                else
                    sUrl = sUrl.Replace('/', '\\'); // make slashes consistent

                sUrl = Path.GetFullPath(sUrl);

                Directory.SetCurrentDirectory(sTemp); // restore old path

                return true;
            }
            catch
            {
                return false;
            }
        }
        //----------------------------------------------------------------------------
        //bool mishex(char c)
        //{
        //    if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')) return true;
        //    return false;
        //}
    }
}
