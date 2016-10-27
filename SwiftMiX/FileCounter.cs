//---------------------------------------------------------------------------
// SwiftMiX - Automatically fade songs between two playlists using
// dual Windows Media Player Active-X controls.
//
// Author: Scott Swift
//
// Released to GitHub under GPL v3 October, 2016
//
//---------------------------------------------------------------------------
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SwiftMiX
{
  public class DirectoryFileCounter
  {
    #region properties

    private int numDirectories = 0;
    public int NumDirectories
    {
      get { return this.numDirectories; }
    }

    private bool bCountComplete = false;
    public bool CountComplete
    {
      get { return this.bCountComplete; }
    }

    private int numFiles = 0;
    public int NumFiles
    {
      get { return this.numFiles; }
    }

    public void Reset()
    {
      numFiles = 0;
      numDirectories = 0;
      bCountComplete = false;
    }
    //---------------------------------------------------------------------------
    #endregion

    #region Background Treenode File Counter

    public Task SetTreeNodeFileCounts(TreeNode rootNode)
    {
      if (rootNode.Nodes.Count > 0)
      {
        return Task.Factory.StartNew((state) =>
        {
          try
          {
            // Call the procedure using the TreeView.
            TreeNodeCollection nodes = ((TreeNode)state).Nodes;
            foreach (TreeNode n in nodes)
              CountRecursive(n);

            bCountComplete = true;
          }
          catch
          {
            MessageBox.Show("Error: SetTreeNodeFileCounts()");
          }
        }, rootNode);
      }
      else
        return null;
    }
    //---------------------------------------------------------------------------

    private void CountRecursive(TreeNode treeNode)
    {
      TreeTagInfo tti = (TreeTagInfo)treeNode.Tag;

      this.Reset();
      this.AddCountFilesAndFolders(tti.Name);

      tti.FileCount = this.numFiles;
      treeNode.Tag = tti; // Write the tag back...

      // Count files under each node recursively.
      foreach (TreeNode tn in treeNode.Nodes)
        CountRecursive(tn);
    }
    //---------------------------------------------------------------------------
    #endregion

    #region Background File Counter

    // Pass this method the parent directory path
    public Task CountFiles(string path)
    {
      this.Reset(); // Clear counters...

      // create a task to do this in the background for responsive ui
      // state is the path
      return Task.Factory.StartNew((state) =>
      {
        try
        {
          // Get the first layer of sub directories
          this.AddCountFilesAndFolders(state.ToString());
          bCountComplete = true;
        }
        catch // Add Handlers for exceptions
        {
          this.Reset();
        }
      }, path);
    }
    //---------------------------------------------------------------------------

    // This method is called recursively
    private void AddCountFilesAndFolders(string path)
    {
      try
      {
        // Only doing the top directory to prevent an exception from stopping the entire recursion
        var dirs = from dir in
                     Directory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly)
                   select dir;

        // calling class is tracking the count of directories
        this.numDirectories += dirs.Count();

        // get the child directories
        // this uses an extension method to the IEnumerable<V> interface,
        // which will run a function on an object. In this case 'd' is the 
        // collection of directories
        dirs.ActionOnEnumerable(d => AddCountFilesAndFolders(d));
      }
      catch // Add Handlers for exceptions
      {
        this.Reset();
      }

      try
      {
        var files = from file in Directory.EnumerateFiles(path) select file;

        // count the files in the directory
        this.numFiles += files.Count();
      }
      catch// Add Handlers for exceptions
      {
        this.Reset();
      }
    }
    //---------------------------------------------------------------------------
    #endregion
  }

  #region Extension Class

  // Extension class
  public static class Extensions
  {
    // this runs the supplied method on each object in the supplied enumerable
    public static void ActionOnEnumerable<V>(this IEnumerable<V> nodes, Action<V> doit)
    {
      foreach (var node in nodes)
      {
        doit(node);
      }
    }
  }
  #endregion
}