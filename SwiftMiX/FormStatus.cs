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
using System.Windows.Forms;

// ORDER:
//
//•Constant Fields
//•Fields
//•Constructors
//•Finalizers (Destructors)
//•Delegates
//•Events
//•Enums
//•Interfaces
//•Properties
//•Indexers
//•Methods
//•Structs
//•Classes

namespace SwiftMiX
{
    public partial class FormStatus : Form
    {

      private int keyPressed = -1;
      public int KeyPressed // Calling Form should poll this...
      {
        get
        {
          return keyPressed;
        }
      }

      public int ProgressValue // Calling Form should poll this...
      {
        get
        {
          return this.progressBar.Value;
        }

        set
        {
          this.progressBar.Value = value;
        }
      }

      // Constructor
      public FormStatus()
      {
        InitializeComponent();
        ProgressReset();
      }

      public void SetText(string text)
      {
        this.textBox1.Text = text;
      }

      public void ProgressReset()
      {
        this.progressBar.Value = 0;
        keyPressed = -1; // Reset
      }

      private void FormStatus_KeyDown(object sender, KeyEventArgs e)
      {
        // If KeyPreview is true, this function will handle
        // key events first and then pass them to the focused
        // child control.
        keyPressed = (int)e.KeyCode;  // Tell background worker to Cancel...

        // Set this to block key events from
        // being passed to the child-control with focus
        e.Handled = true;
      }

      private void FormStatus_VisibleChanged(object sender, EventArgs e)
      {
        ProgressReset();
      }

      private void FormStatus_FormClosing(object sender, FormClosingEventArgs e)
      {
        ProgressReset();
      }
    }
  }
