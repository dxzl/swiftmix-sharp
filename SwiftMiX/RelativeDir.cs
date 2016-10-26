using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
// Author D. Bolton see http://cplus.about.com (c) 2010
// Modified for use in SwiftMiX by Scott Swift 2013

// Public Methods: 
//
// Construct: RelativeDirectory rd = new RelativeDirectory();
// RelativeDirectory() <= Uses the current environment's directory...
// RelativeDirectory(Dir) <= Sets Dir to the specified path...
//
// Down(string match) <= Returns true if any subdirectories in the
// current directory start with "match" 
//
// Up(), Up(int NumLevels) <= Sets the present directory up one or
// more levels and returns true unless we are at the top level.
//
// Public Properties:
// "Dir" is a read only string and returns the new directory's name.
// "Path" is read/write string and generally sets the starting directory.
namespace MyRd
{
    class RelativeDirectory
    {
        #region Vars
        private DirectoryInfo dirInfo;
        #endregion

        #region Properties
        public string Dir
        {
            get
            {
                return dirInfo.Name;
            }
        }

        public string Path
        {
            get { return dirInfo.FullName; }
            set
            {                
                try
                {
                    DirectoryInfo NewDir= new DirectoryInfo(value);
                    dirInfo = NewDir;
                }
                catch 
                { 
                    // silent
                }
            }
        }
        #endregion

        #region Constructors
        public RelativeDirectory()
        {
            dirInfo = new DirectoryInfo(Environment.CurrentDirectory);
        }
        //---------------------------------------------------------------------------

        public RelativeDirectory(string AbsoluteDir)
        {
            dirInfo = new DirectoryInfo(AbsoluteDir);
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Methods
        public Boolean Up(int NumLevels)
        // Sets the present directory up one level and returns true
        // Returns false if we are at the top...
        {
            DirectoryInfo TempDir;
            for (int i = 0; i < NumLevels; i++)
            {
                TempDir = dirInfo.Parent;
                if (TempDir != null)
                    dirInfo = TempDir;
                else
                    return false;
            }
            return true;          
        }
        //---------------------------------------------------------------------------

        public Boolean Up()
        {
            return Up(1);
        }
        //---------------------------------------------------------------------------

        public Boolean Down(string match)
        // Returns true if any subdirectories in the current directory
        // start with "match" 
        {
          try
          {
            // Get the names of subdirectories in the specified directory
            // that begin with "match"
            DirectoryInfo[] dirs = dirInfo.GetDirectories(match + '*');
            if (dirs == null)
              return false;
            dirInfo = dirs[0];
            return true;
          }
          catch
          {
            return false;
          }
        }
        #endregion
    }

    //class Program
    //{
    //    /* This requires this sort of structure to test properly
    //    app folder
    //      |_ bin
    //     *|_ obj
    //     *|   |_ debug  
    //     *|_ exe
         
    //     */
    //    static void Main(string[] args)
    //    {
    //        RelativeDirectory rd = new RelativeDirectory();
    //        Console.WriteLine("Located at {0} {1} ", rd.Path, rd.Dir);  // at debug
    //        rd.Up(2);            
    //        Console.WriteLine("Located at {0} {1} ", rd.Path, rd.Dir);  // at app folder
    //        rd.Down("obj");
    //        Console.WriteLine("Located at {0} {1} ", rd.Path, rd.Dir);  // at obj
    //        Console.ReadKey();

    //    }        
    //}
}

