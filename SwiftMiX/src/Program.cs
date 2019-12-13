using System;
using System.Security.Permissions;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
//using System.Runtime.ExceptionServices;

namespace SwiftMiX
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    static void Main()
    {
      // Add handler for UI thread exceptions
      Application.ThreadException += new ThreadExceptionEventHandler(UIThreadException);

      // Force all WinForms errors to go through handler
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

      // This handler is for catching non-UI thread exceptions
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

      //AppDomain.CurrentDomain.FirstChanceException += FirstChanceHandler;

      try
      {
        Application.EnableVisualStyles(); // comment out to enable classic style
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAreaEnabled;
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormMain());
      }
      catch(Exception ex)
      {
        throw new Exception("SwiftMiX error...", ex);
      }
    }

    //static void FirstChanceHandler(object source, FirstChanceExceptionEventArgs e)
    //{
    //  Console.WriteLine("FirstChanceException event raised in {0}: {1}",
    //      AppDomain.CurrentDomain.FriendlyName, e.Exception.Message);
    //}

    private static void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
    {
      try
      {
        Exception ex = (Exception)e.ExceptionObject;
        MessageBox.Show("Unhadled domain exception:\n\n" + ex.Message);
      }
      catch (Exception exc)
      {
        try
        {
          MessageBox.Show("Fatal exception happend inside UnhadledExceptionHandler:\n\n"
              + exc.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
        finally
        {
          Environment.Exit(1);
        }
      }

      // It should terminate our main thread so Application.Exit() is unnecessary here
    }

    private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
    {
      try
      {
        MessageBox.Show("Unhandled exception caught.\nApplication is going to close now.");
      }
      catch
      {
        try
        {
          DialogResult dr = MessageBox.Show("Fatal exception happend inside UIThreadException handler ",
              "Fatal Windows Forms Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);

          if (dr == DialogResult.Retry)
          {
            Application.Restart();
            return;
          }

          Environment.Exit(1);
        }
        finally
        {
          Environment.Exit(1);
        }
      }

      // Here we can decide if we want to end our application or do something else
      Environment.Exit(1);
    }
  }
}