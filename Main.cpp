// EDIT UNIT8.h TO CHANGE TRIAL-KEY!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//---------------------------------------------------------------------------
#include <vcl.h>
#include "Main.h"
#pragma hdrstop

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "WMPLib_OCX"
#pragma link "WMPLib_OCX"
#pragma link "WMPLib_OCX"
#pragma link "WMPLib_OCX"
#pragma resource "*.dfm"

TMainForm* MainForm;
#if !FREEWARE_EDITION
KeyClass* PK;
#endif

//---------------------------------------------------------------------------
__fastcall TMainForm::TMainForm(TComponent* Owner) : TForm(Owner)
{
  ListA = NULL;
  ListB = NULL;

  FFilesAddedCount = 0;
  FMaxCacheFiles = MAX_CACHE_FILES;

#if !FREEWARE_EDITION
  // Init License-Key Properties
  PK = new KeyClass();

  bool bRet;
  TLicenseKey* lk = new TLicenseKey();
  bRet = lk->ValidateLicenseKey(false);
  delete lk;

  if (!bRet)
  {
    ShowMessage("Critical error in class: TLicenseKey!");
    Release();
    Application->Terminate();
    return;
  }
#endif

#if DEBUG_ON
  CInit();
#endif

  ProgressForm = NULL; // Program tests this in RecurseFileAdd

  Application->HintColor = TColor(0xF5CFB8);
  Application->HintPause = 500;
  Application->HintHidePause = 4000;
  Application->ShowHint = true;

  FfadeAt = 10; // start fade 10 seconds before end

  GPlaylistForm = NULL;

  // Try to register the SwiftMix messages
  RWM_SwiftMixTime = RegisterWindowMessage(L"WM_SwiftMixTime");
  RWM_SwiftMixPlay = RegisterWindowMessage(L"WM_SwiftMixPlay");
  RWM_SwiftMixState = RegisterWindowMessage(L"WM_SwiftMixState");
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::FormCreate(TObject* Sender)
{
  try
  {
    String Info = String(WindowsMediaPlayer1->versionInfo);

    int tmp = Info.Pos(".");
    String Version = Info.SubString(1,tmp-1);
    tmp = Version.ToInt();
    if (tmp < 10)
    {
      ShowMessage("Sorry, you must get the newest version of\n"
        "Microsoft's Windows Media-Player!\n"
        "Your version is: " + Info);
      Release();
      Application->Terminate();
      return;
    }
  }
  catch(...)
  {
    ShowMessage("You must get the new version of\n"
        "Microsoft's Windows Media-Player!");
    Release();
    Application->Terminate();
    return;
  }

  // do this before setting up the file-cache below...
  // to set bFileCacheEnabled
  InitRegistryVars();

  if (bFileCacheEnabled) // if it's enabled in registry...
    bFileCacheEnabled = InitFileCaching();

  Caption = "SwiftMiX " + String(VERSION);
  Color = TColor(0xF5CFB8);

  // Make sure MainForm won't auto-size...
  AutoSize = false;

  // ActiveX Control needs to have its size and
  // position properties explicitly set as of
  // Version 1.38 (9/7/2010)
  WindowsMediaPlayer1->Align = alNone;
  WindowsMediaPlayer1->uiMode = "full"; // "mini", "full", "none"
  WindowsMediaPlayer1->Left = 0;
  WindowsMediaPlayer1->Top = 0;
  WindowsMediaPlayer1->Height = 45;
  WindowsMediaPlayer1->Height = 45;
// Use its default size!
//  WindowsMediaPlayer1->Width = 257;

  WindowsMediaPlayer2->Align = alNone;
  WindowsMediaPlayer2->uiMode = "full"; // "mini", "full", "none"
  WindowsMediaPlayer2->Left = WindowsMediaPlayer1->Left;
  WindowsMediaPlayer2->Top = WindowsMediaPlayer1->Height + 2;
  WindowsMediaPlayer2->Height = WindowsMediaPlayer1->Height;
  WindowsMediaPlayer2->Width = WindowsMediaPlayer1->Width;

  Panel1->Top = WindowsMediaPlayer2->Top + WindowsMediaPlayer2->Height + 2;
  Panel1->Width = WindowsMediaPlayer2->Width;

  StatusBar1->Top = Panel1->Top + Panel1->Height + 2;
  StatusBar1->Width = WindowsMediaPlayer2->Width;

//  UpDown1->Left = 0;
//  UpDown1->Height = 21;
//  UpDown1->Width = 17;
//
//  FadeRate->Left = UpDown1->Left+UpDown1->Width;
//  FadeRate->Height = UpDown1->Height;
//  FadeRate->Width = UpDown1->Width;
//
//  UpDown2->Left = FadeRate->Left+FadeRate->Width;
//  UpDown2->Height = UpDown1->Height;
//  UpDown2->Width = UpDown1->Width;
//
//  FadePoint->Left = UpDown2->Left+UpDown2->Width;;
//  FadePoint->Height = UpDown1->Height;
//  FadePoint->Width = UpDown1->Width+4;
//
//  TrackBar1->Left = FadePoint->Left+FadePoint->Width+2;
//  TrackBar1->Height = 33;
//  TrackBar1->Top = WindowsMediaPlayer2->Top+WindowsMediaPlayer2->Height+2;
//  TrackBar1->Width = WindowsMediaPlayer1->Width-TrackBar1->Left;
//
//  UpDown1->Top = TrackBar1->Top+((TrackBar1->Height-UpDown1->Height)/2);
//  FadeRate->Top = UpDown1->Top;
//  UpDown2->Top = UpDown1->Top;
//  FadePoint->Top = UpDown1->Top;

  // Now dynamically Autosize the MainForm...
  AutoSize = true;

  // Just in-case the width is too small for the status-bar's
  // time--displays...
//ShowMessage(String(WindowsMediaPlayer1->Width));
  if (ClientWidth < 240)
  {
    AutoSize = false;
    ClientWidth = 240;
  }

  // Adjust the status-bar's panels
  int Size = StatusBar1->Width/5;
  for (int ii = 0 ; ii < StatusBar1->Panels->Count ; ii++)
    StatusBar1->Panels->Items[ii]->Width = Size;

  // Create Window Docking Manager
//  FDock = new CWindowDock();
//  if (FDock == NULL)
//    ShowMessage("Unable to create docking manager!");

  //enable drag/drop files
  ::DragAcceptFiles(this->Handle, true);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::FormShow(TObject *Sender)
{
  // Create the song-list forms
  if (ListA == NULL)
    Application->CreateForm(__classid(TPlaylistForm), &ListA);
  if (ListB == NULL)
    Application->CreateForm(__classid(TPlaylistForm), &ListB);

  if (ListA == NULL || ListB == NULL)
  {
    ShowMessage("Problem creating player list-objects!");
    Application->Terminate();
  }

  WindowsMediaPlayer1->Tag = PLAYER_1;
  ListA->Wmp = WindowsMediaPlayer1;
  ListA->OtherWmp = WindowsMediaPlayer2;
  ListA->OtherForm = ListB;
  ListA->PlayerA = true; // this list is for the A player

  WindowsMediaPlayer2->Tag = PLAYER_2;
  ListB->Wmp = WindowsMediaPlayer2;
  ListB->OtherWmp = WindowsMediaPlayer1;
  ListB->OtherForm = ListA;
  ListB->PlayerA = false; // this list is not for the A player

  //-----------------
  // Add the windows to the docking system
  //-----------------
  //Set the parent window
//  if (FDock != NULL)
//  {
//    FDock->SetParent(this->Handle);
//
//    //Add the child windows
//    FDock->AddChild(ListA->Handle); // automatically dock
//    FDock->AddChild(ListB->Handle); // automatically dock
//  }

  SetCheckmarkA(FvolA);
  SetCheckmarkB(FvolB);

  MenuFaderTypeNormal->Checked = bTypeCenterFade ? false : true; // inverse
  MenuFaderModeAuto->Checked = bModeManualFade ? false : true; // inverse
  MenuSendTiming->Checked = bSendTiming ? true : false;
  MenuRepeatModeA->Checked = bRepeatModeA ? true : false;
  MenuRepeatModeB->Checked = bRepeatModeB ? true : false;
  MenuShuffleModeA->Checked = bShuffleModeA ? true : false;
  MenuShuffleModeB->Checked = bShuffleModeB ? true : false;
  MenuCacheFiles->Checked = bFileCacheEnabled ? true : false;

  try
  {
    // SetVolumes() sets currentVolA/currentVolB and both media player volumes
    // based on the values of volA/volB and the position of TrackBar1
    if (!SetVolumes() || !SetCurrentPlayer())
    {
      ShowMessage("Re-Install Windows Media Player 10 or above!");
      Release();
      Application->Terminate();
    }

    WindowsMediaPlayer1->settings->autoStart = false;
    WindowsMediaPlayer2->settings->autoStart = false;
  }
  catch(...) { ShowMessage("There was a problem setting the Volumes!"); }

  if (!bModeManualFade)
    StatusBar1->Panels->Items[2]->Text = "Auto";
  else
    StatusBar1->Panels->Items[2]->Text = "Manual";
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::InitRegistryVars(void)
{
  TRegHelper* pReg = NULL;

  try
  {
    pReg = new TRegHelper(false);

    if (pReg != NULL)
    {
      // Initial directory is the desktop
      // The strings below are in UTF-8 format
      SaveDirA = pReg->ReadSetting(SM_REGKEY_DIR_A);
      SaveDirB = pReg->ReadSetting(SM_REGKEY_DIR_B);
      ImportExt = pReg->ReadSetting(SM_REGKEY_IMPORTEXT);
      ExportExt = pReg->ReadSetting(SM_REGKEY_EXPORTEXT);

      if (SaveDirA.IsEmpty())
        SaveDirA = FsDeskDir;

      if (SaveDirB.IsEmpty())
        SaveDirB = FsDeskDir;

      if (ImportExt.IsEmpty())
        ImportExt = IMPORT_EXT;

      if (ExportExt.IsEmpty())
        ExportExt = EXPORT_EXT;

      pReg->ReadSetting(SM_REGKEY_FADERMODE, bTypeCenterFade, false);
      pReg->ReadSetting(SM_REGKEY_FADERTYPE, bModeManualFade, false);
      pReg->ReadSetting(SM_REGKEY_SENDTIMING, bSendTiming, false);
      pReg->ReadSetting(SM_REGKEY_REPEAT_A, bRepeatModeA, false);
      pReg->ReadSetting(SM_REGKEY_REPEAT_B, bRepeatModeB, false);
      pReg->ReadSetting(SM_REGKEY_SHUFFLE_A, bShuffleModeA, false);
      pReg->ReadSetting(SM_REGKEY_SHUFFLE_B, bShuffleModeB, false);
      pReg->ReadSetting(SM_REGKEY_CACHE_ENABLED, bFileCacheEnabled, true);

      pReg->ReadSetting(SM_REGKEY_CACHE_MAX_FILES, FMaxCacheFiles, MAX_CACHE_FILES);
      if (FMaxCacheFiles < MAX_CACHE_FILES)
        FMaxCacheFiles = MAX_CACHE_FILES; // need to make this the minimum!!!!

      pReg->ReadSetting(SM_REGKEY_VOL_A, FvolA, 50);
      pReg->ReadSetting(SM_REGKEY_VOL_B, FvolB, 50);

      // in case a low volume got stuck in the registry...
      if (FvolA < 10)
        FvolA = 50;
      if (FvolB < 10)
        FvolB = 50;
    }
    else
    {
      ShowMessage("Unable to read settings from the registry!");

      SaveDirA = FsDeskDir;
      SaveDirB = FsDeskDir;
      ImportExt = IMPORT_EXT;
      ExportExt = EXPORT_EXT;

      bTypeCenterFade = false;
      bModeManualFade = false;
      bSendTiming = false;
      bRepeatModeA = false;
      bRepeatModeB = false;
      bShuffleModeA = false;
      bShuffleModeB = false;
      bFileCacheEnabled = true;

      FMaxCacheFiles = MAX_CACHE_FILES;

      FvolA = 50;
      FvolB = 50;
    }
  }
  __finally
  {
    try { if (pReg != NULL) delete pReg; } catch(...) {}
  }
}
//---------------------------------------------------------------------------
// returns true if enabled
bool __fastcall TMainForm::InitFileCaching(void)
{
  FsDeskDir = GetSpecialFolder(CSIDL_DESKTOPDIRECTORY);

  // if we can find or make a Temp directory, enable custom file-cacheing
  FsCacheDir = GetSpecialFolder(CSIDL_LOCAL_APPDATA);
  if (DirectoryExists(FsCacheDir))
  {
    //FsTempDir += "\\Temp";
    //if (!DirectoryExists(FsTempDir))
    //  try { CreateDirectory(FsTempDir.c_str(), NULL); } catch(...) {}
    FsCacheDir += FILE_CACHE_PATH1; // "\\Discrete-Time Systems"
    if (!DirectoryExists(FsCacheDir))
      try { CreateDirectory(FsCacheDir.c_str(), NULL); } catch(...) {}
    FsCacheDir += FILE_CACHE_PATH2; //"\\MusicMixer"
    if (!DirectoryExists(FsCacheDir))
      try { CreateDirectory(FsCacheDir.c_str(), NULL); } catch(...) {}
    else
    {
      // directory exists so we didn't clean up... delete it and its files
      if (DeleteDirAndFiles(FsCacheDir))
        // recreate empty directory
        if (!DirectoryExists(FsCacheDir))
          try { CreateDirectory(FsCacheDir.c_str(), NULL); } catch(...) {}
    }

    return DirectoryExists(FsCacheDir) ? true : false;
  }
  else
    return false;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::FormDestroy(TObject *Sender)
{
  // Don't need to release ListA and ListB - their FormDestroy() gets called!
  // (I think the VCL calls FormDestroy for all forms created with CreateForm...)

// probably we don't need this either...
//  if (ProgressForm != NULL) ProgressForm->Release();

//  if (FDock != NULL) delete FDock;

  // delete file-cache directory and files
  if (DirectoryExists(FsCacheDir))
    DeleteDirAndFiles(FsCacheDir);

#if DEBUG_ON
  MainForm->CWrite("\r\nFormDestroy() in FormMain()\r\n");
  FreeConsole();
#endif

#if !FREEWARE_EDITION
  if (PK != NULL) delete PK;
#endif

//  Application->Terminate();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::FormClose(TObject* Sender, TCloseAction &Action)
{
  if (WindowsMediaPlayer1)
  {
    WindowsMediaPlayer1->settings->mute = true;
    WindowsMediaPlayer1->controls->stop();
  }
  if (WindowsMediaPlayer2)
  {
    WindowsMediaPlayer2->settings->mute = true;
    WindowsMediaPlayer2->controls->stop();
  }

  // Stop Timers
  AutoFadeTimer->Enabled = false;

  if (SFDlgForm != NULL)
  {
    SFDlgForm->Close();
    SFDlgForm->Release();
  }
  if (OFMSDlgForm != NULL)
  {
    OFMSDlgForm->Close();
    OFMSDlgForm->Release();
  }
  if (DirDlgForm != NULL)
  {
    DirDlgForm->Close();
    DirDlgForm->Release();
  }
  if (ListA != NULL)
  {
    ListA->Close();
    ListA->Release();
  }
  if (ListB != NULL)
  {
    ListB->Close();
    ListB->Release();
  }

  TRegHelper* pReg = NULL;

  try
  {
    pReg = new TRegHelper(true);

    if (pReg != NULL)
    {
      pReg->WriteSetting(SM_REGKEY_DIR_A, SaveDirA);
      pReg->WriteSetting(SM_REGKEY_DIR_B, SaveDirB);
      pReg->WriteSetting(SM_REGKEY_IMPORTEXT, ImportExt);
      pReg->WriteSetting(SM_REGKEY_EXPORTEXT, ExportExt);
      pReg->WriteSetting(SM_REGKEY_FADERMODE, bTypeCenterFade);
      pReg->WriteSetting(SM_REGKEY_FADERTYPE, bModeManualFade);
      pReg->WriteSetting(SM_REGKEY_SENDTIMING, bSendTiming);
      pReg->WriteSetting(SM_REGKEY_REPEAT_A, bRepeatModeA);
      pReg->WriteSetting(SM_REGKEY_REPEAT_B, bRepeatModeB);
      pReg->WriteSetting(SM_REGKEY_SHUFFLE_A, bShuffleModeA);
      pReg->WriteSetting(SM_REGKEY_SHUFFLE_B, bShuffleModeB);
      pReg->WriteSetting(SM_REGKEY_CACHE_ENABLED, bFileCacheEnabled);
      pReg->WriteSetting(SM_REGKEY_CACHE_MAX_FILES, FMaxCacheFiles);
      pReg->WriteSetting(SM_REGKEY_VOL_A, FvolA);
      pReg->WriteSetting(SM_REGKEY_VOL_B, FvolB);
    }
  }
  __finally
  {
    try { if (pReg != NULL) delete pReg; } catch(...) {}
  }

#if DEBUG_ON
  MainForm->CWrite("\r\nFormClose() in FormMain()\r\n");
#endif
}
//---------------------------------------------------------------------------
// deletes the temp "song queue" directory and files in it
bool __fastcall TMainForm::DeleteDirAndFiles(String sDir)
{
  // could use "runas" instead of "open"
  return ShellCommand(L"open", L"cmd.exe", L"/c cd ..\\ \& rd /s /q \"" + sDir + "\"");
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::ShellCommand(String sVerb, String sFile,
                                  String sCmd, bool bWaitForCompletion)
{
  int bRet = true;

  try
  {
    // example (from YahCoLoRiZe old version):
    //S = String("/c echo \"Removing YahCoLoRiZe Please Wait...\"") +
    //      String(" \& ping 1.1.1.1 -n 1 -w 2000 > nul \& ") +
    //        String("cd ..\\ \& rd /s /q \"") + S + String("\" \& ") +
    //        String("cd \"") + AppDataDir + String("\" \& ") +
    //        String("rd /s /q \"") + OUR_NAME_S + String("\"");

    if (IsWinVistaOrHigher())
    {
      // Vista, Win7,8,10
      SHELLEXECUTEINFO shExecInfo = {0};

      shExecInfo.cbSize = sizeof(SHELLEXECUTEINFO);

      // wait for it to complete (NULL if no wait)
      shExecInfo.fMask = bWaitForCompletion ? SEE_MASK_NOCLOSEPROCESS : NULL;

      shExecInfo.hwnd = NULL;
//        shExecInfo.lpVerb = L"runas"; // NOTE: this works but prompts for admin password
      shExecInfo.lpVerb = sVerb.c_str(); // "open"
      shExecInfo.lpFile = sFile.c_str(); // "cmd.exe"
      shExecInfo.lpParameters = sCmd.c_str(); // "/c cd ..\\ \& rd /s /q \"" + sDir + "\""
      shExecInfo.lpDirectory = NULL;
      shExecInfo.nShow = SW_HIDE;
      shExecInfo.hInstApp = NULL;

      ShellExecuteEx(&shExecInfo);

      // only use the two lines below for SEE_MASK_NOCLOSEPROCESS
      if (bWaitForCompletion)
      {
        //if (!WaitForSingleObject(shExecInfo.hProcess, INFINITE))
        if (WaitForSingleObject(shExecInfo.hProcess, 2000) != 0) // wait for 2 seconds
          bRet = false;

        CloseHandle(shExecInfo.hProcess);
      }
    }
    else // WinXP
    {
      STARTUPINFO si = {0};
      PROCESS_INFORMATION pi = {0};

      // Documentation requires NULL for app-name if cmd.exe is 16-bit
      // (which it must be because this is the only way it works!)
      sFile += " "; // "cmd.exe "
      sCmd = sCmd.Insert(sFile, 1);
      CreateProcess(NULL, sCmd.c_str(), NULL, NULL, FALSE,
                    CREATE_NO_WINDOW, NULL, NULL, &si, &pi);

      CloseHandle(pi.hThread);
      CloseHandle(pi.hProcess);

      if (bWaitForCompletion)
        Sleep(1000); // wait to complete
    }
  }
  catch(...)
  {
    bRet = false;
  }

  return bRet;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::StatusBar1MouseDown(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y)
{
  int j, WidthIndex;
  int iNumPanels = StatusBar1->Panels->Count;

  // for each panel in the bar...

  WidthIndex = 0;

  for (int j = 0 ; j < iNumPanels; j++)
  {
    WidthIndex += StatusBar1->Panels->Items[j]->Width;

    if (X <= WidthIndex)
    {
      if (j == 2)
        MenuFaderModeAutoClick(NULL);
      break;
    }
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::File1Click(TObject* Sender)
{
  if (FileDialog(ListA, SaveDirA, ADD_A_TITLE))
    ListA->QueueFirst();
//  if (FileDialog(ListA, ADD_A_FILES, SaveDirA)) ListA->QueueFirst();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::File2Click(TObject* Sender)
{
  if (FileDialog(ListB, SaveDirB, ADD_B_TITLE))
    ListB->QueueFirst();
//  if (FileDialog(ListB, ADD_B_FILES, SaveDirB)) ListB->QueueFirst();
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::FileDialog(TPlaylistForm* f, String &d, String t)
// d is the default directory (in UTF-8)
// t is the title (in UTF-8)
{
  if (f == NULL || f->Wmp == NULL)
    return false;

#if !FREEWARE_EDITION
  if (PK->ComputeDaysRemaining() <= 0)
  {
    ShowMessage("Trial Expired, visit: \n" + String(WEBSITE));
    return false;
  }
#endif

  bool bRet = false; // presume failure...

  try
  {
    try
    {
      TOFMSDlgForm* fd = f->CreateFileDialog();

      if (fd != NULL)
      {
        // Filters
        fd->Filters = "All Files (*.*)|*.*|"
                    "Windows Media (*.wma)|*.wma|"
                    "MP3 (*.mp3)|*.mp3|"
                    "WAV (*.wav)|*.wav";

        FFilesAddedCount = 0;

        fd->Execute(0, d, t); // no def utf-8 extension?

        if(fd->Result == IDOK)
        {
          // Press and hold Shift to bypass the file-extention filtering
          // (Checked in AddFileToListBox())
          GBypassFilters = (GetKeyState(VK_SHIFT) & 0x8000);

#if DEBUG_ON
          // Display for diagnostics
          if (fd->FileNameObjects != NULL && fd->FileNameObjects->Count > 0)
          {
            MainForm->CWrite( "\r\nPrinting selected-files list!!!!!!!\r\n");

            String s1 = "";

            for (int ii = 0; ii < fd->FileNameObjects->Count; ii++)
            {
              TWideItem* pWI = (TWideItem*)fd->FileNameObjects->Items[ii];

              if (pWI != NULL)
              {
                String s = String(pWI->s);
                String sDir = pWI->IsDirectory ? "true" : "false";
                s1 += s + " (IsDirectory: " +  sDir + ")\n";
              }
            }

            if (!s1.IsEmpty())
              MainForm->CWrite( "\r\n" + s1 + "\r\n");

            bRet = true;
          }
          else
            MainForm->CWrite( "\r\nsl TStringList is NULL or Empty!\r\n");
#endif

          if (fd->FileNameObjects != NULL && fd->FileNameObjects->Count > 0)
          {
            if (f == ListA)
              SaveDirA = fd->CurrentFolder;
            else
              SaveDirB = fd->CurrentFolder;

            int Count = fd->FileNameObjects->Count;

            TProgressForm::Init(Count);

            for (int ii = 0; ii < Count ; ii++)
            {
              Application->ProcessMessages();
              if (Application->Terminated || (int)GetAsyncKeyState(VK_ESCAPE) < 0)
                break;

              TWideItem* pWI = (TWideItem*)fd->FileNameObjects->Items[ii];

              if (pWI != NULL)
              {
                if (pWI->IsDirectory)
                {
                  if (SetCurrentDirectory(pWI->s.w_str()))
                  {
                    AddAllSongsToListBox(f);
                    TProgressForm::Move(ii);
                  }
                }
                else
                {
                  if (AddFileToListBox(f, pWI->s))
                    TProgressForm::Move(ii);
                }
              }
            }

            TProgressForm::UnInit();
          }

          // Show the listbox
          if (f->Count)
            ShowPlaylist(f);

          bRet = true;
        }
        // User canceled... only a problem if they added files via our custom mechanism...
        else if (FFilesAddedCount > 0)
        {
#if DEBUG_ON
          MainForm->CWrite("\r\nGFilesAdded is > 0...\r\n");
#endif
          bRet = true;
        }
        else
        {
#if DEBUG_ON
          MainForm->CWrite("\r\nUser canceled!!!!!!\r\n");
#endif
        }
      }
    }
    catch(...) { ShowMessage("FileDialog() threw an exception"); }
  }
  __finally
  {
    f->DestroyFileDialog();
  }

  if (bRet)
    SetVolumes();

  return bRet;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::WMDropFile(TWMDropFiles &Msg)
{
  if (Msg.Drop == 0) return;

  try
  {
    if (ListA == NULL || ListB == NULL ||
      WindowsMediaPlayer1 == NULL || WindowsMediaPlayer2 == NULL) return;

    // Which player are we over?
    int left = WindowsMediaPlayer1->Left;
    int right = left + WindowsMediaPlayer1->Width;

    int topA = WindowsMediaPlayer1->Top;
    int bottomA = topA + WindowsMediaPlayer1->Height;

    int topB = WindowsMediaPlayer2->Top;
    int bottomB = topB + WindowsMediaPlayer2->Height;

    //get drop location
    TPoint p;
    ::DragQueryPoint((HDROP)Msg.Drop, &p);

    // Convert drop-coordinates to client
    ::ScreenToClient(NULL, &p);

    if (p.x > left && p.x < right)
    {
      if (p.y > topA && p.y < bottomA)
        LoadListWithDroppedFiles(Msg, ListA);
      else if (p.y > topB && p.y < bottomB)
        LoadListWithDroppedFiles(Msg, ListB);
    }
  }
  // NOTE: Unlike C#, __finally does NOT get called if we execute a return!!!
  __finally
  {
    try { ::DragFinish((HDROP)Msg.Drop); } catch(...) {}
    Msg.Result = 0;
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::LoadListWithDroppedFiles(TWMDropFiles &Msg, TPlaylistForm* f)
{
  if (f == NULL)
    return;

  TCursor Save_Cursor;
  wchar_t* pBuf = NULL;
  TStringList* sl = NULL;
  ImportForm = NULL;

  FFilesAddedCount = 0;

  try
  {
    //get dropped files count
    int DroppedCount = ::DragQueryFileW((HDROP)Msg.Drop, -1, NULL, 0);

    if (DroppedCount == 0)
      return;

    Save_Cursor = Screen->Cursor;
    Screen->Cursor = crHourGlass;    // Show hourglass cursor

    TProgressForm::Init(DroppedCount);

    if (ImportForm == NULL)
      Application->CreateForm(__classid(TImportForm), &ImportForm);

    pBuf = new wchar_t[MAX_PATH];
    sl = new TStringList();
//    sl->Sorted = true; // don't want to mess up a user-chosen order...

    for (int ii = 0; ii < DroppedCount; ii++)
    {
      ::DragQueryFileW((HDROP)Msg.Drop, ii, pBuf, MAX_PATH);

      if (*pBuf != L'\0')
        sl->Add(pBuf);

      TProgressForm::Move(ii);
    }

    // Repair a quirky list-system... (not needed after the first item has been added to a listbox!)
    if (sl->Count > 1 && f->Count == 0)
      sl->Exchange(0, sl->Count-1);

    // Press and hold Shift to bypass the file-extention filtering
    this->GBypassFilters = (GetKeyState(VK_SHIFT) & 0x8000);

    TProgressForm::Init(sl->Count);

    String SaveDir = GetCurrentDir(); // Save

    for (int ii = 0; ii < sl->Count; ii++)
    {
      Application->ProcessMessages();

      if (Application->Terminated || (int)GetAsyncKeyState(VK_ESCAPE) < 0)
        break;

      String sFile = sl->Strings[ii]; // these strings are in UTF-8!

      String Ext = ExtractFileExt(sFile).LowerCase();

      if (Ext.IsEmpty() && DirectoryExists(sFile))
      {
        SetCurrentDir(sFile);
        AddAllSongsToListBox(f); // recurse add folder and sub-folder's songs to list
      }
      else if (this->GBypassFilters || Ext == ".wpl" || Ext == ".m3u8" || Ext == ".m3u" ||
                            Ext == ".asx" || Ext == ".xspf" || Ext == ".wax" ||
                            Ext == ".wmx" || Ext == ".wvx" ||  Ext == ".pls" || Ext == ".txt")
        FFilesAddedCount += ImportForm->NoDialog(f, sFile,
                ImportForm->GetMode(Ext, IMPORT_MODE_AUTO)); // Load the playlist
      else if (AddFileToListBox(f, sFile))
        FFilesAddedCount++;

      TProgressForm::Move(ii);
    }

    SetCurrentDir(SaveDir); // Restore

    if (FFilesAddedCount > 0)
      ShowPlaylist(f);
    else
      ShowMessage("Song(s) may not have been added because either they did not\n"
             "appear to be sound files or the file was not found.\n\n"
             "Perhaps your music drive USB cable is unplugged?\n\n"
             "(To bypass the filter-list, press and hold SHIFT until you release\n"
             "the mouse button and drag/drop the file(s) again.)");
  }
  __finally
  {
    try { TProgressForm::UnInit(); } catch(...) {}

    try { if (ImportForm != NULL) ImportForm->Release(); } catch(...) {}
    try { if (pBuf != NULL) delete [] pBuf; } catch(...) {}
    try { if (sl != NULL) delete sl; } catch(...) {}

    // Restore the previous cursor.
    Screen->Cursor = Save_Cursor;
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::AddAllSongsToListBox(TPlaylistForm* f)
// Strings are in UTF-8 format!
{
  TStringList* sl = NULL;

  try
  {
    sl = new TStringList();

    RecurseFileAdd(sl);

    int Count = sl->Count;

    if (Count > 1)
    {
      TProgressForm::Init(Count);

      for (int ii = 0 ; ii < Count ; ii++)
      {
        Application->ProcessMessages();

        if (Application->Terminated || (int)GetAsyncKeyState(VK_ESCAPE) < 0)
          break;

        if (AddFileToListBox(f, sl->Strings[ii]))
          TProgressForm::Move(ii);
      }

      TProgressForm::UnInit();

      if (FFilesAddedCount > 0)
        ShowPlaylist(f);
    }
  }
  __finally
  {
    try { if (sl != NULL) delete sl; } catch(...) {}
  }
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::AddFileToListBox(TPlaylistForm* f, String sFile)
{
  if (f == NULL || sFile.Length() == 0)
    return false;

  if (!IsAudioFile(sFile))
    return false;

  try
  {
    f->AddListItem(sFile, IsUri(sFile));

    // move the first file to the cache
    if (f->Count == 0)
      CopyFileToCache(f, -1, false);

    FFilesAddedCount++;

    return true;
  }
  catch(...) { return false; }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::RecurseFileAdd(TStringList* slFiles)
// Uses the String versions of the Win32 API FindFirstFile and FindNextFile directly
// and converts the resulting paths to UTF-8 for storage in an ordinary TStringList
//
// Use SetCurrentDirectory() to set our root directory or TOpenDialog sets it also...
{
  Application->ProcessMessages();

  if (slFiles == NULL) return;

  TStringList* slSubDirs = new TStringList();
  if (slSubDirs == NULL) return;

  TWin32FindDataW sr;
  HANDLE hFind = NULL;
  TFindexInfoLevels l = FindExInfoStandard; // FindExInfoBasic was defined later!
  TFindexSearchOps s = FindExSearchLimitToDirectories;

  try
  {
    hFind = FindFirstFileEx(L"*", l, &sr, s, NULL, (DWORD)FIND_FIRST_EX_LARGE_FETCH);

    // Get list of subdirectories into a stringlist
    if (hFind != INVALID_HANDLE_VALUE)
    {
      do
      {
        if ((sr.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0)
        {
          int len = wcslen(sr.cFileName);

          if (len == 1 && sr.cFileName[0] == L'.')
            continue;
          if (len == 2 && sr.cFileName[0] == L'.' && sr.cFileName[1] == L'.')
            continue;

          slSubDirs->Add(sr.cFileName);
        }
      } while (FindNextFile(hFind, &sr) == TRUE);
    }
  }
  __finally
  {
    try { if (hFind != NULL) FindClose(hFind); } catch(...) {}
  }

  AddFilesToStringList(slFiles);

  TProgressForm::Init(slSubDirs->Count);

  // Get songs in all subdirectories
  for (int ii = 0; ii < slSubDirs->Count; ii++)
  {
    Application->ProcessMessages();
    if (Application->Terminated || (int)GetAsyncKeyState(VK_ESCAPE) < 0)
      break;

    if (SetCurrentDirectory(slSubDirs->Strings[ii].w_str()))
    {
      RecurseFileAdd(slFiles);
      SetCurrentDirectory(L"..");
    }

    // Move Progress bar if it exists
    TProgressForm::Move(ii);
  }

  delete slSubDirs;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::AddFilesToStringList(TStringList* slFiles)
// slFiles is in UTF-8!
{
  TWin32FindDataW sr;
  HANDLE hFind = NULL;
  TFindexInfoLevels l = FindExInfoStandard; // FindExInfoBasic was defined later!
  TFindexSearchOps s = FindExSearchNameMatch;

  try
  {
    // Get the current directory
    String wdir = GetCurrentDir();

    hFind = FindFirstFileExW(L"*", l, &sr, s, NULL, (DWORD)FIND_FIRST_EX_LARGE_FETCH);

    // Get list of files into a stringlist
    if (hFind != INVALID_HANDLE_VALUE)
    {
      String ws;

      // Don't add these file-types...
      int mask = FILE_ATTRIBUTE_DIRECTORY|FILE_ATTRIBUTE_SYSTEM|FILE_ATTRIBUTE_HIDDEN;

      do
      {
        if ((sr.dwFileAttributes & mask) == 0)
        {
          ws = wdir + L"\\" + String(sr.cFileName);
          slFiles->Add(ws);
        }
      } while (FindNextFileW(hFind, &sr) == TRUE);
    }
  }
  __finally
  {
    try { if (hFind != NULL) ::FindClose(hFind); } catch(...) {}
  }
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::IsAudioFile(String sFile)
// should work ok on UTF-8 strings
{
  if (IsUri(sFile)) return true; // pass through http:// links

  if (GBypassFilters) return true;

  String sExt = ExtractFileExt(sFile).LowerCase();

  if (sExt.IsEmpty()) return false;

  return sExt == ".mp3" || sExt == ".wma" || sExt == ".asf" || sExt == ".wav" ||
          sExt == ".mpa" || sExt == ".mpe" || sExt == ".m3u" || sExt == ".avi" || sExt == ".aac" ||
          sExt == ".adt" || sExt == ".adts" || sExt == ".mp2" || sExt == ".cda" ||
          sExt == ".au" || sExt == ".snd" || sExt == ".aif" || sExt == ".aiff" || sExt == ".aifc" ||
          sExt == ".mid" || sExt == ".midi" || sExt == ".rmi" || sExt == ".m4a";
}
//---------------------------------------------------------------------------
// Could have "file:/laptop/D:/path/file.wma" so the key to telling a URL from
// a drive letter is that url preambles are more than one char!
//
// sIn should be trimmed but does not need to be lower-case...
bool __fastcall TMainForm::IsUri(String sIn)
{
  return sIn.Pos(":/") > 2; // > 2 means you must have more than 1 char before the : (like "file:/")
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::IsFileUri(String sIn)
{
  return sIn.LowerCase().Pos("file:/") == 1;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::VA_10Click(TObject* Sender)
{
  // Vol 1 10%
  SetVolumeAndCheckmarkA(10);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::VA_25Click(TObject* Sender)
{
  // Vol 1 25%
  SetVolumeAndCheckmarkA(25);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::VA_50Click(TObject* Sender)
{
  // Vol 1 50%
  SetVolumeAndCheckmarkA(50);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::VA_75Click(TObject* Sender)
{
  // Vol 1 75%
  SetVolumeAndCheckmarkA(75);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::VA_100Click(TObject* Sender)
{
  // Vol 1 100%
  SetVolumeAndCheckmarkA(100);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::VB_10Click(TObject* Sender)
{
  // Vol 2 10%
  SetVolumeAndCheckmarkB(10);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::VB_25Click(TObject* Sender)
{
  // Vol 2 25%
  SetVolumeAndCheckmarkB(25);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::VB_50Click(TObject* Sender)
{
  // Vol 2 50%
  SetVolumeAndCheckmarkB(50);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::VB_75Click(TObject* Sender)
{
  // Vol 2 75%
  SetVolumeAndCheckmarkB(75);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::VB_100Click(TObject* Sender)
{
  // Vol 2 100%
  SetVolumeAndCheckmarkB(100);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::Player1Next1Click(TObject* Sender)
{
  ListA->NextIndex = ListA->TargetIndex;
  ListA->NextPlayer();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::Player2Next1Click(TObject* Sender)
{
  ListB->NextIndex = ListB->TargetIndex;
  ListB->NextPlayer();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuForceFadeClick(TObject* Sender)
{
  ForceFade();
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::ForceFade(void)
// Manually initiate an "auto-fade"
// Also triggers next Queued song
{
  if (!WindowsMediaPlayer1 || !WindowsMediaPlayer2 || bModeManualFade)
    return false;

  try
  {
    // player 1 on now?
    if (ListB->Tag >= 0 && WindowsMediaPlayer1->playState == WMPPlayState::wmppsPlaying)
    {
      // Start Player 2
      if (WindowsMediaPlayer2->playState != WMPPlayState::wmppsPlaying)
      {
        WindowsMediaPlayer2->settings->mute = true;
        WindowsMediaPlayer2->controls->play();
        WindowsMediaPlayer2->settings->mute = false;
      }

      bFadeRight = true; // Set fade-direction
      AutoFadeTimer->Enabled = true; // Start a fade
      return true;
    }

    // player 2 on now?
    if (ListA->Tag >= 0 && WindowsMediaPlayer2->playState == WMPPlayState::wmppsPlaying)
    {
      // Start Player 1
      if (WindowsMediaPlayer1->playState != WMPPlayState::wmppsPlaying)
      {
        WindowsMediaPlayer1->settings->mute = true;
        WindowsMediaPlayer1->controls->play();
        WindowsMediaPlayer1->settings->mute = false;
      }

      bFadeRight = false; // Set fade-direction
      AutoFadeTimer->Enabled = true; // Start a fade
      return true;
    }
  }
  catch(...)
  {
    ShowMessage("ForceFade() threw an exception");
  }

  return false;
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::SetVolumeAndCheckmarkA(int v)
{
  bool bRet1 = SetVolumeA(v);
  bool bRet2 = SetCheckmarkA(v);
  return bRet1 || bRet2;
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::SetVolumeAndCheckmarkB(int v)
{
  bool bRet1 = SetVolumeB(v);
  bool bRet2 = SetCheckmarkB(v);
  return bRet1 || bRet2;
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::SetCheckmarkA(int v)
{
  try
  {
    VA_10->Checked = false;
    VA_25->Checked = false;
    VA_50->Checked = false;
    VA_75->Checked = false;
    VA_100->Checked = false;

    switch(v)
    {
      case 10:
        VA_10->Checked = true;
      break;

      case 25:
        VA_25->Checked = true;
      break;

      case 50:
        VA_50->Checked = true;
      break;

      case 75:
        VA_75->Checked = true;
      break;

      case 100:
        VA_100->Checked = true;
      break;

      default:
        VA_50->Checked = true;
      break;
    };

    return true;
  }
  catch(...) { return false; }
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::SetCheckmarkB(int v)
{
  try
  {
    VB_10->Checked = false;
    VB_25->Checked = false;
    VB_50->Checked = false;
    VB_75->Checked = false;
    VB_100->Checked = false;

    switch(v)
    {
      case 10:
        VB_10->Checked = true;
      break;

      case 25:
        VB_25->Checked = true;
      break;

      case 50:
        VB_50->Checked = true;
      break;

      case 75:
        VB_75->Checked = true;
      break;

      case 100:
        VB_100->Checked = true;
      break;

      default:
        VB_50->Checked = true;
      break;
    };

    return true;
  }
  catch(...) { return false; }
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::SetVolumes()
{
  bool bRetA = SetVolumeA();
  bool bRetB = SetVolumeB();
  return bRetA || bRetB;
}
//---------------------------------------------------------------------------
// overloaded...


bool __fastcall TMainForm::SetVolumeA(int v)
{
  FvolA = v;
  return SetVolumeA();
}

bool __fastcall TMainForm::SetVolumeA(void)
{
  if (!WindowsMediaPlayer1)
    return false;

  // TrackBar1 Min position is 0, Max position is 100 (0-100%)
  // Units for volA/volB global vars is in percent (0-100)
  try
  {
    if (bTypeCenterFade)
    {
      if (FaderTrackBar->Position < 50)
        FcurrentVolA = FvolA;
      else
        FcurrentVolA = FvolA*((100-FaderTrackBar->Position)*2)/100;
    }
    else
      FcurrentVolA = (FvolA*(100-FaderTrackBar->Position))/100;

//#if DEBUG_ON
//    MainForm->CWrite("\r\nSetVolumeA(): currentVolA = " + String(FcurrentVolA) + "\r\n");
//#endif

    WindowsMediaPlayer1->settings->volume = FcurrentVolA;

    return true;
  }
  catch(...)
  {
#if DEBUG_ON
    MainForm->CWrite("\r\nSetVolumeA() exception!\r\n");
#endif
    return false;
  }
}
//---------------------------------------------------------------------------
// overloaded...

bool __fastcall TMainForm::SetVolumeB(int v)
{
  FvolB = v;
  return SetVolumeB();
}

bool __fastcall TMainForm::SetVolumeB(void)
{
  if (!WindowsMediaPlayer2)
    return false;

  // TrackBar1 Min position is 0, Max position is 100 (0-100%)
  // Units for volA/volB global vars is in percent (0-100)
  try
  {
    if (bTypeCenterFade)
    {
      if (FaderTrackBar->Position >= 50)
        FcurrentVolB = FvolB;
      else
        FcurrentVolB = FvolB*(FaderTrackBar->Position*2)/100;
    }
    else
      FcurrentVolB = (FvolB*FaderTrackBar->Position)/100;

//#if DEBUG_ON
//    MainForm->CWrite("\r\nSetVolumeB(): volB = " + String(FvolB) + "\r\n");
//    MainForm->CWrite("\r\nSetVolumeB(): currentVolB = " + String(FcurrentVolB) + "\r\n");
//#endif

    WindowsMediaPlayer2->settings->volume = FcurrentVolB;

    return true;
  }
  catch(...)
  {
#if DEBUG_ON
    MainForm->CWrite("\r\nSetVolumeB() exception!\r\n");
#endif
    return false;
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::Exit1Click(TObject* Sender)
{
  Release();
  Application->Terminate();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuAboutClick(TObject* Sender)
{
  Application->CreateForm(__classid(TAboutForm), &AboutForm);
  AboutForm->ShowModal();
  AboutForm->Release();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::ClearPlaylistStop1Click(TObject* Sender)
{
  if (ListA != NULL) ListA->ClearAndStop();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::ClearPlaylistStop2Click(TObject* Sender)
{
  if (ListB != NULL) ListB->ClearAndStop();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuFaderModeAutoClick(TObject* Sender)
{
  if (!bModeManualFade)
  {
    bModeManualFade = true;
    MenuFaderModeAuto->Checked = false;
    MenuFaderModeAuto->Caption = "Fader Mode: Manual";
    MenuForceFade->Enabled = false;
    StatusBar1->Panels->Items[2]->Text = "Manual";
  }
  else
  {
    bModeManualFade = false;
    MenuFaderModeAuto->Checked = true;
    MenuFaderModeAuto->Caption = "Fader Mode: Auto";
    MenuForceFade->Enabled = true;
    StatusBar1->Panels->Items[2]->Text = "Auto";
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuFaderTypeNormalClick(TObject* Sender)
{
  if (!WindowsMediaPlayer1 || !WindowsMediaPlayer2)
    return;

  if (!bTypeCenterFade)
  {
    // Fader Mode - Center-Fade
    bTypeCenterFade = true;
    MenuFaderTypeNormal->Checked = false;
    MenuFaderTypeNormal->Caption = "Fader Type: Center";
  }
  else
  {
    // Fader Mode - Normal
    bTypeCenterFade = false;
    MenuFaderTypeNormal->Checked = true;
    MenuFaderTypeNormal->Caption = "Fader Type: Normal";
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::ShowPlaylist(TPlaylistForm* f)
{
  if (f == NULL)
  {
    ShowMessage("null pointer in ShowPlaylist()");
    return;
  }

  try
  {
    if (f->Count && f->Tag == -1)
      f->QueueFirst();

    f->SetTitle();

    if (f->WindowState == wsMinimized)
      f->WindowState = wsNormal;

    // already showing? skip...
    if (f->Visible)
      return;

    f->Color = TColor(0xF5CFB8);

    f->Height = Height/2 + 3;
    f->Width = 3*Width/2;

    int borderWidth;
    int borderHeight;

    if (IsWinVistaOrHigher())
    {
      borderWidth = GetSystemMetrics(SM_CXDLGFRAME);
      borderHeight = GetSystemMetrics(SM_CYDLGFRAME);
    }
    else // xp
    {
      borderWidth = 0;
      borderHeight = 0;
    }

    if (f == ListA)
    {
      // Player 1
      f->Left = Left - borderWidth;
      f->Top = Top - f->Height - borderHeight;
    }
    else
    {
      // Player 2
      f->Left = (Left + Width) - f->Width + borderWidth;
      f->Top = Top + Height + borderHeight;
    }

    if (f->Left < 1)
      f->Left = 1;

    int diff = (f->Left+f->Width)-Screen->Width;
    if (diff > 0)
      f->Left -= diff;

    if (f->Top < 1)
      f->Top = 1;

    diff = (f->Top+f->Height)-Screen->Height;
    if (diff > 0)
      f->Top -= diff;

    f->Show();
    MainForm->SetFocus();

//    GDock->WindowMoved(f->Handle);
  }
  catch(...) { ShowMessage("ShowPlaylist() threw an exception"); }
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::IsWinVistaOrHigher(void)
{
  OSVERSIONINFO vi;
  vi.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
  GetVersionEx(&vi);

  return vi.dwPlatformId == VER_PLATFORM_WIN32_NT && vi.dwMajorVersion >= 6;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::ViewPlaylist1Click(TObject* Sender)
{
  ShowPlaylist(ListA);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::ViewPlaylist2Click(TObject* Sender)
{
  ShowPlaylist(ListB);
}
//---------------------------------------------------------------------------
// Here, we either increment or decrement the fader trackbar position by the
// trackbar frequency. The base timer unit is 50ms. The up-down Fade Rate
// can be 0 to +9 or 0 to -9. When it's positive, the timer-tick is 50ms and the
// tick-frequency of the trackbar is 0-9. There are 100 steps in the
// trackbar range. So the "normal" fade is 50ms * 100 steps = 5 seconds.
// The fastest fade is (50ms*100)/9 = .55 seconds. For negative fade-rates
// the tick-frequency is 1 and the trackbar interval is 50ms to (50*9) = 450ms.
// So the slowest fade-time is 100 steps at 50ms per step * 9 or 450 * 100 or
// 45 seconds.
void __fastcall TMainForm::AutoFadeTimerEvent(TObject* Sender)
{
  if (!WindowsMediaPlayer1 || !WindowsMediaPlayer2) return;

  try
  {
    if (bFadeRight)
    {
      if (FaderTrackBar->Position < FaderTrackBar->Max)
      {
        if (FaderTrackBar->Position <= FaderTrackBar->Max-FaderTrackBar->Frequency)
          FaderTrackBar->Position += FaderTrackBar->Frequency;
        else
          FaderTrackBar->Position = FaderTrackBar->Max;
      }
      else
      {
        AutoFadeTimer->Enabled = false;

        // this is a way to periodically return focus to this
        // main form...
        RestoreFocus();

        WindowsMediaPlayer1->settings->mute = true;
        WindowsMediaPlayer1->controls->stop();
        WindowsMediaPlayer1->settings->mute = false;

        // Queue next song
        WindowsMediaPlayer1->URL = ListA->GetNextCheckCache();

        if (ListA->TargetIndex < 0)
          if (FileDialog(ListA, SaveDirA, ADD_A_TITLE))
            ListA->QueueFirst();
      }
    }
    else
    {
      if (FaderTrackBar->Position > FaderTrackBar->Min)
      {
        if (FaderTrackBar->Position >= FaderTrackBar->Frequency)
          FaderTrackBar->Position -= FaderTrackBar->Frequency;
        else
          FaderTrackBar->Position = FaderTrackBar->Min;
      }
      else
      {
        AutoFadeTimer->Enabled = false;

        // this is a way to periodically return focus to this
        // main form...
        RestoreFocus();

        WindowsMediaPlayer2->settings->mute = true;
        WindowsMediaPlayer2->controls->stop();
        WindowsMediaPlayer2->settings->mute = false;

        // Queue next song
        WindowsMediaPlayer2->URL = ListB->GetNextCheckCache();

        if (ListB->TargetIndex < 0)
          if (FileDialog(ListB, SaveDirB, ADD_B_TITLE))
            ListB->QueueFirst();
      }
    }
  }
  catch(...)
  {
#if DEBUG_ON
    ShowMessage("AutoFadeTimerEvent() threw an exception...");
#endif
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::FaderTrackBarChange(TObject* Sender)
{
  // Fader Moved
  SetVolumes();
  SetCurrentPlayer(); // Set CurrentPlayer variable (used for color-coding)
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::SetCurrentPlayer(void)
{
  if (!WindowsMediaPlayer1 || !WindowsMediaPlayer2)
    return false;

  try
  {
    if (FaderTrackBar->Position != FaderTrackBar->Max &&
              WindowsMediaPlayer1->playState == WMPPlayState::wmppsPlaying)
      CurrentPlayer |= 1;
    else
      CurrentPlayer &= ~1;

    if (FaderTrackBar->Position != FaderTrackBar->Min &&
          WindowsMediaPlayer2->playState == WMPPlayState::wmppsPlaying)
      CurrentPlayer |= 2;
    else
      CurrentPlayer &= ~2;

    // Set Bevels for status bar
    static OldPlayer = -1;

    if (CurrentPlayer != OldPlayer)
    {
      if (CurrentPlayer & 1)
      {
        StatusBar1->Panels->Items[0]->Bevel = (TStatusPanelBevel)1;
        StatusBar1->Panels->Items[1]->Bevel = (TStatusPanelBevel)1;
      }
      else
      {
        StatusBar1->Panels->Items[0]->Bevel = (TStatusPanelBevel)0;
        StatusBar1->Panels->Items[1]->Bevel = (TStatusPanelBevel)0;
      }

      if (CurrentPlayer & 2)
      {
        StatusBar1->Panels->Items[3]->Bevel = (TStatusPanelBevel)1;
        StatusBar1->Panels->Items[4]->Bevel = (TStatusPanelBevel)1;
      }
      else
      {
        StatusBar1->Panels->Items[3]->Bevel = (TStatusPanelBevel)0;
        StatusBar1->Panels->Items[4]->Bevel = (TStatusPanelBevel)0;
      }

      OldPlayer = CurrentPlayer;
    }
  }
  catch(...) { return false; }

  return true;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::Play1Click(TObject* Sender)
{
  if (!WindowsMediaPlayer1)
    return;

  WindowsMediaPlayer1->settings->mute = true;
  WindowsMediaPlayer1->controls->play();
  WindowsMediaPlayer1->settings->mute = false;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::Stop1Click(TObject* Sender)
{
  if (!WindowsMediaPlayer1)
    return;

  WindowsMediaPlayer1->settings->mute = true;
  WindowsMediaPlayer1->controls->stop();
  WindowsMediaPlayer1->settings->mute = false;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::Pause1Click(TObject* Sender)
{
  if (!WindowsMediaPlayer1)
    return;

  if (WindowsMediaPlayer1->playState == WMPPlayState::wmppsPaused) // paused?
    WindowsMediaPlayer1->controls->play();
  else
    WindowsMediaPlayer1->controls->pause();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::Play2Click(TObject* Sender)
{
  if (!WindowsMediaPlayer2)
    return;

  WindowsMediaPlayer1->settings->mute = true;
  WindowsMediaPlayer2->controls->play();
  WindowsMediaPlayer1->settings->mute = false;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::Stop2Click(TObject* Sender)
{
  if (!WindowsMediaPlayer2)
    return;

  WindowsMediaPlayer2->settings->mute = true;
  WindowsMediaPlayer2->controls->stop();
  WindowsMediaPlayer2->settings->mute = false;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::Pause2Click(TObject* Sender)
{
  if (!WindowsMediaPlayer2)
    return;

  if (WindowsMediaPlayer2->playState == WMPPlayState::wmppsPaused) // paused?
    WindowsMediaPlayer2->controls->play();
  else
    WindowsMediaPlayer2->controls->pause();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::RestoreFocus(void)
{
  if (ListA->Visible && ListA->Active && ListA->WindowState == wsMaximized) MainForm->SetFocus();
  else if (ListB->Visible && ListB->Active && ListB->WindowState == wsMaximized) MainForm->SetFocus();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::ImportPlaylist1Click(TObject* Sender)
{
  if (ListA == NULL || ListA->IsImportDlg)
    return;

  TImportForm* id = ListA->CreateImportDialog();

  int Count = id->Dialog(ListA, FsDeskDir, "Import PlayerA Playlist");

  if (Count > 0)
  {
    SetVolumes();
    Application->ProcessMessages();

    // Show the listbox
    ShowPlaylist(ListA);
  }
  else if (Count == 0)
    ShowMessage("Unable to load playlist (is it empty?)\nIs your music drive plugged in?");

  ListA->DestroyImportDialog();
}

void __fastcall TMainForm::ImportPlaylist2Click(TObject* Sender)
{
  if (ListB == NULL)
    return;
  Application->CreateForm(__classid(TImportForm), &ImportForm);

  int Count = ImportForm->Dialog(ListB, FsDeskDir, "Import PlayerB Playlist");

  if (Count > 0)
  {
    SetVolumes();
    Application->ProcessMessages();

    // Show the listbox
    ShowPlaylist(ListB);
  }
  else if (Count == 0)
    ShowMessage("Unable to load playlist (is it empty?)\nIs your music drive plugged in?");

  ImportForm->Release();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::ExportPlaylist1Click(TObject* Sender)
{
  Application->CreateForm(__classid(TExportForm), &ExportForm);

  int Count = ExportForm->Dialog(ListA, FsDeskDir, "Export PlayerA Playlist");

  if (Count == 0)
    ShowMessage("Unable to export list or list empty...");

  ExportForm->Release();
}

void __fastcall TMainForm::ExportPlaylist2Click(TObject* Sender)
{
  Application->CreateForm(__classid(TExportForm), &ExportForm);

  int Count = ExportForm->Dialog(ListB, FsDeskDir, "Export PlayerB Playlist");

  if (Count == 0)
    ShowMessage("Unable to export list or list empty...");

  ExportForm->Release();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuHelpClick(TObject* Sender)
{
  // launch default web-browser
  //ShellExecute(Handle, "open", "iexplore.exe", HELPSITE, NULL, SW_SHOW);
  ShellExecute(NULL, L"open", HELPSITE, NULL, NULL, SW_SHOWNORMAL);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuRepeatModeAClick(TObject* Sender)
{
  if (bRepeatModeA)
  {
    MenuRepeatModeA->Checked = false;
    bRepeatModeA = false;
  }
  else
  {
    MenuRepeatModeA->Checked = true;
    bRepeatModeA = true;
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuRepeatModeBClick(TObject* Sender)
{
  if (bRepeatModeB)
  {
    MenuRepeatModeB->Checked = false;
    bRepeatModeB = false;
  }
  else
  {
    MenuRepeatModeB->Checked = true;
    bRepeatModeB = true;
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuShuffleModeAClick(TObject* Sender)
{
  if (bShuffleModeA)
  {
    MenuShuffleModeA->Checked = false;
    bShuffleModeA = false;
  }
  else
  {
    MenuShuffleModeA->Checked = true;
    bShuffleModeA = true;
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuShuffleModeBClick(TObject* Sender)
{
  if (bShuffleModeB)
  {
    MenuShuffleModeB->Checked = false;
    bShuffleModeB = false;
  }
  else
  {
    MenuShuffleModeB->Checked = true;
    bShuffleModeB = true;
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuSendTimingClick(TObject* Sender)
{
  if (bSendTiming)
  {
    bSendTiming = false;
    MenuSendTiming->Checked = false;
  }
  else
  {
    bSendTiming = true;
    MenuSendTiming->Checked = true;
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuCacheFilesClick(TObject *Sender)
{
  if (bFileCacheEnabled)
  {
    // delete file-cache directory and files
    if (DirectoryExists(FsCacheDir))
      DeleteDirAndFiles(FsCacheDir);

    bFileCacheEnabled = false;
    MenuCacheFiles->Checked = false;
  }
  else
  {
    bFileCacheEnabled = InitFileCaching();
    MenuCacheFiles->Checked = bFileCacheEnabled;
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuHighPriorityClick(TObject* Sender)
{
  if (!WindowsMediaPlayer1 || !WindowsMediaPlayer2)
    return;

  // HIGH_PRIORITY_CLASS
  // NORMAL_PRIORITY_CLASS
  // REALTIME_PRIORITY_CLASS
  try
  {
    DWORD dwProcessId1;
    DWORD dwProcessId2;

    GetWindowThreadProcessId(WindowsMediaPlayer1->Handle,&dwProcessId1);
    GetWindowThreadProcessId(WindowsMediaPlayer2->Handle,&dwProcessId2);

    // Get handle to Process
    HANDLE hProcess1 = OpenProcess(PROCESS_QUERY_INFORMATION| // access flag
                            PROCESS_SET_INFORMATION,
                            false, // handle inheritance flag
                            dwProcessId1); // process identifier
    // Get handle to Process
    HANDLE hProcess2 = OpenProcess(PROCESS_QUERY_INFORMATION| // access flag
                            PROCESS_SET_INFORMATION,
                            false, // handle inheritance flag
                            dwProcessId2); // process identifier

    if (!hProcess1)
      return;

    if (!hProcess2)
    {
      CloseHandle(hProcess1);
      return;
    }

    int Priority1 = (int)GetPriorityClass(hProcess1);
    int Priority2 = (int)GetPriorityClass(hProcess2);

    if (Priority1 == HIGH_PRIORITY_CLASS || Priority2 == HIGH_PRIORITY_CLASS)
    {
      SetPriorityClass(hProcess1, NORMAL_PRIORITY_CLASS);
      SetPriorityClass(hProcess2, NORMAL_PRIORITY_CLASS);
    }
    else
    {
      SetPriorityClass(hProcess1, HIGH_PRIORITY_CLASS);
      SetPriorityClass(hProcess2, HIGH_PRIORITY_CLASS);
    }

    Priority1 = (int)GetPriorityClass(hProcess1);
    Priority2 = (int)GetPriorityClass(hProcess2);

    // Menu check-mark will reflect the true priority
    if (Priority1 == HIGH_PRIORITY_CLASS && Priority2 == HIGH_PRIORITY_CLASS)
      MenuHighPriority->Checked = true;
    else
      MenuHighPriority->Checked = false;

    CloseHandle(hProcess1);
    CloseHandle(hProcess2);
  }
  catch(...)
  {
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuExportSongFilesandListsClick(TObject* Sender)
{
  if (DirDlgForm != NULL)
    return;

   // Copy all song-list files to directory user selects
  if ((ListA != NULL && ListA->Count != 0) ||
                    (ListB != NULL && ListB->Count != 0))
  {
    MenuAutoFitToDVDCDClick(NULL);

    // Return if user needs to modify the lists due to the
    // cumulative size of the music files
    if (ComputeDiskSpace(DISKSPACE_MESSAGEBOX_YESNO) <= 0)
      return;

    Application->CreateForm(__classid(TDirDlgForm), &DirDlgForm);

    if (DirDlgForm == NULL)
      return;

    DirDlgForm->AutoScroll = false; // turn off autoscroll

    // Setting CSIDL_MYMUSIC works but the user can't go up from there!
    String wUserDir = DirDlgForm->Execute(CSIDL_DESKTOPDIRECTORY);

    DirDlgForm->Close();
    DirDlgForm->Release();

    if (wUserDir.IsEmpty())
      return;

    try
    {
      wUserDir += "\\SwiftMiX";

      if (DirectoryExists(wUserDir))
      {
        String wMsg = "Old directory already exists:\n\n" +
          wUserDir + "\n\nPlease delete it first...";
        ShowMessage(wMsg);
        return;
      }
      else
        CreateDirectory(wUserDir.w_str(), NULL); // Create base directory

      // Save the playlists
      Application->CreateForm(__classid(TExportForm), &ExportForm);

      if (ExportForm == NULL)
        return;

      // EXPORT_EXT is wpl so we need to set the bSaveAsUtf8 flag... since all the files will be
      // copied into the same directory with the associated lists, we just want the file-name in the list,
      // not the path
      // We write in utf8 without BOM
      String wFile = wUserDir + "\\SwiftMiXA." + EXPORT_EXT;
      ExportForm->NoDialog(ListA, wFile, EXPORT_PATH_NONE, EXPORT_MODE_UTF8, false, false);
      wFile = wUserDir + "\\SwiftMiXB." + EXPORT_EXT;
      ExportForm->NoDialog(ListB, wFile,  EXPORT_PATH_NONE, EXPORT_MODE_UTF8, false, false);

      ExportForm->Close();
      ExportForm->Release();
      ExportForm = NULL;

      CopyMusicFiles(ListA, wUserDir);
      CopyMusicFiles(ListB, wUserDir);
      TProgressForm::UnInit();
    }
    catch (...) { }
  }
  else
    ShowMessage("Both lists are empty!");
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::CopyMusicFiles(TPlaylistForm* f, String uUserDir)
{
  try
  {
    int count = f->Count;

    TProgressForm::Init(count, 5);

    String uSourcePath, uFileName, uDestPath;

    // Copy ListA files
    for (int ii = 0; ii < count; ii++)
    {
      Application->ProcessMessages();
      if (Application->Terminated || (int)GetAsyncKeyState(VK_ESCAPE) < 0)
      {
        TProgressForm::UnInit();
        return;
      }

      uSourcePath = GetURL(f->CheckBox, ii);

      // Note that we use the old UTF-8 string "ListA->CheckBox->Items[ii]"
      // to run the Ansi ExtractFileName() on and path should be UTF-8 too...
      // THEN we convert to wide!
      uFileName = ExtractFileName(uSourcePath);

      if (uFileName.Length() > 0)
      {
        uDestPath = uUserDir + String("\\") + uFileName;

        if (CopyFile(uSourcePath.w_str(), uDestPath.w_str(), FALSE) == 0)
          if (PromptAbort(uSourcePath))
            return;
      }

      TProgressForm::Move(ii);
    }
  }
  catch(...) {}
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::PromptAbort(String s)
{
  String sMsg = String("Unable to copy file:\n\n") + s +
                                                String("\n\nAbort?");

  if (MessageBox(Handle, sMsg.w_str(), L"File Copy Error", MB_ICONQUESTION +
                                          MB_YESNO + MB_DEFBUTTON1) == IDYES)
  {
    TProgressForm::UnInit();
    return true;
  }

  return false;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuViewDiscSpaceRequiredClick(TObject* Sender)
{
  ComputeDiskSpace(DISKSPACE_MESSAGEBOX_OK);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::MenuAutoFitToDVDCDClick(TObject* Sender)
{
  if (AutoSizeForm != NULL) return;
  Application->CreateForm(__classid(TAutoSizeForm), &AutoSizeForm);
  if (AutoSizeForm == NULL) return;

  if (AutoSizeForm->ShowModal() == mrOk)
  {
    __int64 val = ComputeDiskSpace(DISKSPACE_MESSAGEBOX_NONE);
    __int64 temp;

    if (val > AutoSizeForm->DiskSize)
    {
      temp = val-AutoSizeForm->DiskSize;

      // Call function to randomly remove at least "diff" bytes
      // alternating between lists
      __int64 newval = RandomRemove(temp);

      if (newval == temp)
        ShowMessage("User abort before operation was compleated!");
      else
      {
        String SizeStr = FormatFloat("#,##0", (double)newval);

        ShowMessage("Successfully removed songs in order to fit\n"
          "the target media...\n\n"
          "Now under media size-limit by: " + SizeStr + " Bytes");
      }
    }
//    else
//    {
//      temp = AutoSizeForm->DiskSize-val;
//      String SizeStr = FormatFloat("#,##0", (double)temp);
//      ShowMessage("Under media size-limit by: " + SizeStr + " Bytes");
//    }
}

  // Release the resources
  AutoSizeForm->Close();
  AutoSizeForm->Release();
  AutoSizeForm = NULL;
}
//---------------------------------------------------------------------------
__int64 __fastcall TMainForm::ComputeDiskSpace(int Mode)
// Add up # bytes needed
//
// Returns -1 if user wants to quit...
// otherwise returns the total # bytes required for
// both lists.
//
// Mode:
// DISKSPACE_MESSAGEBOX_NONE  0 // no box...
// DISKSPACE_MESSAGEBOX_YESNO 1
// DISKSPACE_MESSAGEBOX_OK    2
{
  String temp;

  unsigned __int64 total_size_A = 0;

  int cA = ListA->Count;
  int cB = ListB->Count;
//  int total = cA + cB;

  TProgressForm::Init(cA+cB);

  // Total size for list A
  for (int ii = 0; ii < cA; ii++)
  {
    Application->ProcessMessages();
    if (Application->Terminated || (int)GetAsyncKeyState(VK_ESCAPE) < 0)
      return -1;

    temp = GetURL(ListA->CheckBox, ii);

    if (FileExists(temp))
    {
      // Open file to get size
      HANDLE h = CreateFile(temp.w_str(),
                         GENERIC_READ,
                         0,
                         NULL,
                         OPEN_EXISTING,
                         FILE_ATTRIBUTE_NORMAL,
                         NULL);

      if (h != INVALID_HANDLE_VALUE)
      {
        total_size_A += GetFileSize(h, NULL);
        CloseHandle(h);
      }
    }

    TProgressForm::Move(ii);
  }

  unsigned __int64 total_size_B = 0;

  // Total size for list B
  for (int ii = 0; ii < cB; ii++)
  {
    Application->ProcessMessages();
    if (Application->Terminated || (int)GetAsyncKeyState(VK_ESCAPE) < 0)
      return -1;

    temp = GetURL(ListB->CheckBox, ii);

    if (FileExists(temp))
    {
      // Open file to get size
      HANDLE h = CreateFile(temp.w_str(),
                         GENERIC_READ,
                         0,
                         NULL,
                         OPEN_EXISTING,
                         FILE_ATTRIBUTE_NORMAL,
                         NULL);

      if (h != INVALID_HANDLE_VALUE)
      {
        total_size_B += GetFileSize(h, NULL);
        CloseHandle(h);
      }
    }

    TProgressForm::Move(ii);
  }

  TProgressForm::UnInit();

  unsigned __int64 total_size = total_size_A + total_size_B;

  // Quit now if not displaying a lot of info
  if (Mode == 0)
    return((__int64)total_size);

  String AKbMbGb, BKbMbGb, TKbMbGb;
  String ASizeStr, BSizeStr, TSizeStr;

  double KB, MB, GB;

  KB = 0.;
  MB = 0.;
  GB = 0.;

  if (total_size_A > 0.)
  {
    KB = total_size_A/1024.;
    if (KB >= 1024.) MB = KB/1024.;
    if (MB >= 1024.) GB = MB/1024.;
  }


  if (GB > 0)
  {
    ASizeStr = FormatFloat("#,##0.00", GB);
    AKbMbGb = " GiB";
  }
  else if (MB > 0)
  {
    ASizeStr = FormatFloat("#,##0.00", MB);
    AKbMbGb = " MiB";
  }
  else
  {
    ASizeStr = FormatFloat("#,##0.00", KB);
    AKbMbGb = " KiB";
  }

  KB = 0.;
  MB = 0.;
  GB = 0.;

  if (total_size_B > 0.)
  {
    KB = total_size_B/1024.;
    if (KB >= 1024.) MB = KB/1024.;
    if (MB >= 1024.) GB = MB/1024.;
  }

  if (GB > 0.)
  {
    BSizeStr = FormatFloat("#,##0.00", GB);
    BKbMbGb = " GiB";
  }
  else if (MB > 0.)
  {
    BSizeStr = FormatFloat("#,##0.00", MB);
    BKbMbGb = " MiB";
  }
  else
  {
    BSizeStr = FormatFloat("#,##0.00", KB);
    BKbMbGb = " KiB";
  }

  KB = 0.;
  MB = 0.;
  GB = 0.;

  if (total_size > 0.)
  {
    KB = total_size/1024.;

    if (KB >= 1024.)
      MB = KB/1024.;

    if (MB >= 1024.)
      GB = MB/1024.;
  }

  if (GB > 0.)
  {
    TSizeStr = FormatFloat("#,##0.00", GB);
    TKbMbGb = " GiB";
  }
  else if (MB > 0.)
  {
    TSizeStr = FormatFloat("#,##0.00", MB);
    TKbMbGb = " MiB";
  }
  else
  {
    TSizeStr = FormatFloat("#,##0.00", KB);
    TKbMbGb = " KiB";
  }

  String ABytesStr = FormatFloat("#,#0", total_size_A);
  String BBytesStr = FormatFloat("#,#0", total_size_B);
  String TBytesStr = FormatFloat("#,#0", total_size);

  String s =
            "List A Size: " + ASizeStr + AKbMbGb +
              " (" + ABytesStr + " bytes)(" + cA + " Songs)\n"
            "List B Size: " + BSizeStr + BKbMbGb +
              " (" + BBytesStr + " bytes)(" + cB + " Songs)\n\n"
            "Total Size: " + TSizeStr + TKbMbGb +
              " (" + TBytesStr + " bytes)\n\n"
            "Note: A typical DVD holds 4.38 GiB\n"
            "(A double-layer DVD holds 7.95 GiB)\n"
            "A typical CD-ROM holds 703.1 MiB";


  if (Mode == 1)
  {
    s += "\n\nOK to continue with export?";

    int button = MessageBox(Handle, s.w_str(), L"Export", MB_ICONQUESTION + MB_YESNO + MB_DEFBUTTON2);

    if (button == IDNO) return (-1);
  }
  else if (Mode == 2)
    MessageBox(Handle, s.w_str(), L"Disc-Space", MB_ICONINFORMATION + MB_OK);

  return((__int64)total_size);
}
//---------------------------------------------------------------------------
unsigned __int64 __fastcall TMainForm::RandomRemove(unsigned __int64 TargetBytes)
{
  unsigned __int64 acc = 0;

  TPlaylistForm* fp;

  String temp;

  while(ListA->Count > 0 || ListB->Count > 0)
  {
    // Delete from the most-populated list
    fp = (ListA->Count > 0 && ListA->Count > ListB->Count) ? ListA : ListB;

    if (fp->Count > 0)
    {
      int rand_idx = random(fp->Count);

      temp = GetURL(fp->CheckBox, rand_idx);

      if (FileExists(temp))
      {
        // Open file to get size
        HANDLE h = CreateFile(temp.w_str(),
                           GENERIC_READ,
                           0,
                           NULL,
                           OPEN_EXISTING,
                           FILE_ATTRIBUTE_NORMAL,
                           NULL);

        if (h != INVALID_HANDLE_VALUE)
        {
          int file_size = GetFileSize(h, NULL);

          int button = IDYES;

          if (bAutoSizePrompt)
          {
            String S_Player = fp == ListA ? L"A" : L"B";
            String s = String("Remove: \"") + temp + String("\" (") + String(file_size) +
                  String(" bytes) from Player ") + S_Player + String("'s list?");

            button = MessageBox(Handle, s.w_str(), L"Remove Song?", MB_ICONQUESTION + MB_YESNOCANCEL + MB_DEFBUTTON2);
          }

          if (button == IDYES)
          {
            acc += file_size;
            // Remove the item
            fp->DeleteListItem(rand_idx);
          }

          CloseHandle(h);

          if (button == IDCANCEL) return(TargetBytes);
        }
      }

      if (acc >= TargetBytes)
        return(acc-TargetBytes);
    }
  }

  // Error
  return 0;
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::FPUpDownChangingEx(TObject *Sender, bool &AllowChange,
          int NewValue, TUpDownDirection Direction)
{
  FfadeAt = NewValue;
}
//---------------------------------------------------------------------------
// NOTE: If the UpDown fade-rate is 0, we force it to be 1 to avoid
// setting the timer interval to 0. 0 is allowed but turns off the
// timer-event's hook. So -9 becomes -10 and +9 becomes +10, effectively.
void __fastcall TMainForm::FRUpDownChangingEx(TObject *Sender, bool &AllowChange,
          int NewValue, TUpDownDirection Direction)
{
  if (NewValue < 0)
  {
    if (NewValue > FRUpDown->Min)
    {
      int PrevPos = ((TUpDown*)Sender)->Position;

      if (NewValue < PrevPos)
      {
        if (FPUpDown->Position < FPUpDown->Max)
          FPUpDown->Position += 3;
      }
      else
      {
        if (FPUpDown->Position > FPUpDown->Min)
          FPUpDown->Position -= 3;
      }
    }

    AutoFadeTimer->Interval = -NewValue*40;
    FaderTrackBar->Frequency = 1;
  }
  else
  {
    AutoFadeTimer->Interval = 50;
    FaderTrackBar->Frequency = NewValue;
  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::WindowsMediaPlayerError(TObject *Sender)
{
  TWindowsMediaPlayer* wmp = dynamic_cast<TWindowsMediaPlayer*>(Sender);
  if (wmp == NULL) return;
  //TPlaylistForm* pl = wmp->Tag == PLAYER_1 ? ListA : ListB;
  //if (pl == NULL) return;

//  String sPlayer = (wmp->Tag == PLAYER_1) ? "A" : "B";
//  int errCount = wmp->Error->get_errorCount();
//
//  if (errCount)
//  {
//    int err;
//    wmp->Error->get_item(0, &err);
//    ShowMessage("Error in player " + sPlayer + ":" + String(err));
//  }
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::WindowsMediaPlayer1OpenStateChange(TObject* Sender, long NewState)
{
  if (ListA != NULL)
    ListA->OpenStateChange((WMPOpenState)NewState);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::WindowsMediaPlayer1PlayStateChange(TObject* Sender, long NewState)
{
  if (ListA != NULL)
    ListA->PlayStateChange((WMPPlayState)NewState);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::WindowsMediaPlayer1PositionChange(TObject* Sender, double oldPosition, double newPosition)
{
  if (ListA != NULL)
    ListA->PositionChange(oldPosition, newPosition);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::WindowsMediaPlayer1MediaChange(TObject* Sender, LPDISPATCH Item)
{
  // Populate MediaInfo struct
  if (ListA != NULL)
    ListA->GetSongInfo();
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::WindowsMediaPlayer2OpenStateChange(TObject* Sender, long NewState)
{
  if (ListB != NULL)
    ListB->OpenStateChange((WMPOpenState)NewState);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::WindowsMediaPlayer2PlayStateChange(TObject* Sender, long NewState)
{
  if (ListB != NULL)
    ListB->PlayStateChange((WMPPlayState)NewState);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::WindowsMediaPlayer2PositionChange(TObject* Sender, double oldPosition, double newPosition)
{
  if (ListB != NULL)
    ListB->PositionChange(oldPosition, newPosition);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::WindowsMediaPlayer2MediaChange(TObject* Sender, LPDISPATCH Item)
{
  // Populate MediaInfo struct
  if (ListB != NULL)
    ListB->GetSongInfo();
}
//---------------------------------------------------------------------------
bool __fastcall TMainForm::WriteStringToFile(String wPath, String sInfo)
// Writes sInfo (ANSI or UTF-8) to a UTF-16 path
{
  bool bRet = false;
  FILE* f = NULL;

  try
  {
    // open/create file for writing in text-mode
    f = _wfopen(wPath.w_str(), L"wb");

    unsigned length = sInfo.Length();

    if (f != NULL)
      if (fwrite(sInfo.w_str(), sizeof(char), length, f) == length)
        bRet = true;
  }
  __finally
  {
    try { if (f != NULL) fclose(f); } catch(...) {}
  }

  return bRet;
}
//---------------------------------------------------------------------------
String __fastcall TMainForm::GetSpecialFolder(int csidl)
{
  HMODULE h = NULL;
  String sOut;
  WideChar* buf = NULL;

  try
  {
    h = LoadLibraryW(L"Shell32.dll");

    if (h != NULL)
    {
      tGetFolderPath pGetFolderPath;
      pGetFolderPath = (tGetFolderPath)GetProcAddress(h, "SHGetFolderPathW");

      if (pGetFolderPath != NULL)
      {
        buf = new WideChar[MAX_PATH];
        buf[0] = L'\0';

        if ((*pGetFolderPath)(Application->Handle, csidl, NULL, SHGFP_TYPE_CURRENT, (LPTSTR)buf) == S_OK)
          sOut = String(buf);
      }
    }

  }
  __finally
  {
    if (h != NULL) FreeLibrary(h);
    if (buf != NULL) delete [] buf;
  }

  return sOut;
}
//---------------------------------------------------------------------------
//URLDownloadToFile function
//
//    07/12/2016
//    2 minutes to read
//
//Downloads bits from the Internet and saves them to a file.
//Syntax
//C++
//
//HRESULT URLDownloadToFile(
//             LPUNKNOWN            pCaller,
//             LPCTSTR              szURL,
//             LPCTSTR              szFileName,
//  _Reserved_ DWORD                dwReserved,
//             LPBINDSTATUSCALLBACK lpfnCB
//);
//
//Parameters
//
//    pCaller
//    A pointer to the controlling IUnknown interface of the calling ActiveX component, if the caller is an ActiveX component. If the calling application is not an ActiveX component, this value can be set to NULL. Otherwise, the caller is a COM object that is contained in another component, such as an ActiveX control in the context of an HTML page. This parameter represents the outermost IUnknown of the calling component. The function attempts the download in the context of the ActiveX client framework, and allows the caller container to receive callbacks on the progress of the download.
//
//    szURL
//    A pointer to a string value that contains the URL to download. Cannot be set to NULL. If the URL is invalid, INET_E_DOWNLOAD_FAILURE is returned.
//
//    szFileName
//    A pointer to a string value containing the name or full path of the file to create for the download. If szFileName includes a path, the target directory must already exist.
//
//    dwReserved
//    Reserved. Must be set to 0.
//
//    lpfnCB
//    A pointer to the IBindStatusCallback interface of the caller. By using IBindStatusCallback::OnProgress, a caller can receive download status. URLDownloadToFile calls the IBindStatusCallback::OnProgress and IBindStatusCallback::OnDataAvailable methods as data is received. The download operation can be canceled by returning E_ABORT from any callback. This parameter can be set to NULL if status is not required.
//
//Return value
//
//This function can return one of these values.
//Return code 	Description
//S_OK
//
//The download started successfully.
//E_OUTOFMEMORY
//
//The buffer length is invalid, or there is insufficient memory to complete the operation.
//INET_E_DOWNLOAD_FAILURE
//
//The specified resource or callback interface was invalid.
//Remarks
//
//URLDownloadToFile binds to a host that supports IBindHost to perform the download. To do this, it first queries the controlling IUnknown passed as pCaller for IServiceProvider, then calls IServiceProvider::QueryService with SID_SBindHost. If pCaller does not support IServiceProvider, IOleObject or IObjectWithSite is used to query the object's host container. If no IBindHost interface is supported, or pCaller is NULL, URLDownloadToFile creates its own bind context to intercept download notifications.
//
//URLDownloadToFile returns S_OK even if the file cannot be created and the download is canceled. If the szFileName parameter contains a file path, ensure that the destination directory exists before calling URLDownloadToFile. For best control over the download and its progress, an IBindStatusCallback interface is recommended.
//
//Windows Internet Explorer 8. URLDownloadToFile does not support IBindStatusCallbackEx and cannot be used to download files over 4 gigabytes (GB) in size. Refer instead to IBindStatusCallbackEx::GetBindInfoEx for a code example.
//Requirements
//
//Minimum supported client
//
//
//Windows XP
//
//Minimum supported server
//
//
//Windows 2000 Server
//
//Product
//
//
//Internet Explorer 3.0
//
//Header
//	Urlmon.h
//
//Library
//	Urlmon.lib
//
//DLL
//	Urlmon.dll
//
//Unicode and ANSI names
//URLDownloadToFileW (Unicode) and URLDownloadToFileA (ANSI)  // fetch TargetIndex into the temp-file queue

//function DownloadFile( Source, Dest: string ) : Boolean;
//type
//  TURLDownloadToFile = function( p1: IUnknown; p2: PAnsiChar; p3: PAnsiChar;
//p4: DWORD; p5: pointer ) : HResult; stdcall;
//file://function URLDownloadToFile(p1: IUnknown; p2: PAnsiChar; p3:
//PAnsiChar; p4: DWORD; p5: pointer): HResult; stdcall;external 'urlmon.dll'
//name 'URLDownloadToFileA';
//var
//  URLDownloadToFile: tURLDownloadToFile;
//  h: thandle;
//begin
//  Result := False;
//  h := LoadLibrary( 'urlmon.dll' ) ;
//  if h = 0 then
//    exit;
//  try
//    URLDownloadToFile := GetProcAddress( h, 'URLDownloadToFileA' ) ;
//    if @URLDownloadToFile = nil then
//      exit;
//    try
//      Result := UrlDownloadToFile( nil, PChar( source ) , PChar( Dest ) , 0,
//nil ) = 0;
//    except
//      Result := False;
//    end;
//  finally
//    FreeLibrary( h ) ;
//  end;
//end;
//
//example:
//if DownloadFile('http://www......','c:\file.dat') then showmessage('download
//OK');
//
//function Get-Web($url, [switch]$self, $credential, $toFile, [switch]$bytes)
//{
//    #.Synopsis
//    #    Downloads a file from the web
//    #.Description
//    #    Uses System.Net.Webclient (not the browser) to download data
//    #    from the web.
//    #.Parameter self
//    #    Uses the default credentials when downloading that page (for downloading intranet pages)
//    #.Parameter credential
//    #    The credentials to use to download the web data
//    #.Parameter url
//    #    The page to download (e.g. www.msn.com)
//    #.Parameter toFile
//    #    The file to save the web data to
//    #.Parameter bytes
//    #    Download the data as bytes
//    #.Example
//    #    # Downloads www.live.com and outputs it as a string
//    #    Get-Web http://www.live.com/
//    #.Example
//    #    # Downloads www.live.com and saves it to a file
//    #    Get-Web http://wwww.msn.com/ -toFile www.msn.com.html
//    $webclient = New-Object Net.Webclient
//    if ($credential) {
//        $webClient.Credential = $credential
//    }
//    if ($self) {
//        $webClient.UseDefaultCredentials = $true
//    }
//    if ($toFile) {
//        if (-not "$toFile".Contains(":")) {
//            $toFile = Join-Path $pwd $toFile
//        }
//        $webClient.DownloadFile($url, $toFile)
//    } else {
//        if ($bytes) {
//            $webClient.DownloadData($url)
//        } else {
//            $webClient.DownloadString($url)
//        }
//    }
//}
//---------------------------------------------------------------------------
// set idx -1 to reference the last item in the list
// returns false if list empty or error
bool __fastcall TMainForm::CopyFileToCache(TPlaylistForm* f, int idx, bool bWaitForCompletion)
{
  bool bRet = false;

  if (!f || f->CheckBox->Count == 0)
    return false;

  if (idx < 0)
    idx = f->CheckBox->Count-1;

  try
  {
    String sDestFile;

    // prefetch the next file into our temporary cache directory
    if (bFileCacheEnabled && idx >= 0 && idx < f->CheckBox->Count)
    {
      // add a new file to the cache

      String sFile = GetURL(f->CheckBox, idx);
      TPlayerURL* p = (TPlayerURL*)f->CheckBox->Items->Objects[idx];
      if (!p) return false;

      // here, we avoid a headache for a large music file that's already in the
      // process of being transferred but might not have completed.
      // So just return "success".
      //
      // We might have a problem if for any reason, the xfer fails - say a web
      // xfer via URL - then we will think we have a cache file but NOT have it.
      //
      // We might need to monitor a media-load failure (see OpenStateChange()
      // in SMList.cpp) and go back to the original url, and erase the cache
      // entry.
      if (p->cacheNumber > 0)
        return true;

      // check sFile's first characters for ftp:\\, http:\\ or https:\\
      // see if this is a URL that WebGetData can handle...
      String lsFile = sFile.LowerCase();
      int iPos1 = lsFile.Pos("http://");
      int iPos2 = lsFile.Pos("https://");
      int iPos3 = lsFile.Pos("ftp://");

      if (iPos1 == 1 || iPos2 == 1 || iPos3 == 1)
      {
        int ii;
        int len = sFile.Length();
        for (ii = len; ii > 0; ii--)
          if (sFile[ii] == '/' || sFile[ii] == '\\' || sFile[ii] == ':')
            break;

        sDestFile = CacheDir + "\\" + sFile.SubString(ii+1, len-ii);

        if (!FileExists(sDestFile))
        {
          int retCode = WebGetData(sFile, sDestFile);
          if (retCode < 0)
          {
            switch(retCode)
            {
              case -2:
                ShowMessage("Can't open new cache file: \"" + String(sDestFile) + "\"");
              break;

              case -3:
                ShowMessage("Can't load library wininet.dll");
              break;

              case -4:
                ShowMessage("Can't find one or more functions in wininet.dll");
              break;

              case -5:
                ShowMessage("Connection Failed or Syntax error");
              break;

              case -6:
                ShowMessage("Unable to open internet URL: \"" + String(sFile) + "\"");
              break;

              case -7:
                ShowMessage("Exception thrown opening internet URL");
              break;

              default:
              break;
            }

            return false;
          }
        }

        p->bDownloaded = true;
        bRet = true;
      }
      else if (FileExists(sFile))
      {
        sDestFile = CacheDir + "\\" + ExtractFileName(sFile);
        if (!FileExists(sDestFile))
        {
          if (bWaitForCompletion)
          {
            if (CopyFile(sFile.c_str(), sDestFile.c_str(), FALSE) == 0)
              return false;
          }
          else
          {
            // copy new file to cache
            String sCopy = L"/c copy \"" + sFile + "\" \"" + sDestFile + "\"";
            ShellCommand(L"open", L"cmd.exe", sCopy.c_str(), false);
          }
        }

        p->bDownloaded = false;
        bRet = true;
      }

      if(bRet)
      {
        p->cachePath = sDestFile;
        p->cacheNumber = f->CacheCount+1; // all cache #s must be non-zero!

        // delete the oldest file in the cache...
        DeleteCacheFile(f);

        f->CacheCount++;

        // if count is 0 (unlikely) - we've exceeded the # files in a 32 bit int
        // of continuous playback... if so, just turn off caching and the files
        // (up to MAX_CACHE_FILES) will be deleted on exit along with the directory
        // containing them.
        if (f->CacheCount == 0)
        {
           bFileCacheEnabled = false;
           return false;
        }
      }
    }
  }
  catch(...) {}

  return bRet;
}
//---------------------------------------------------------------------------
// returns true if we had to delete a file from the cache due to
// being at the file limit of MAX_CACHE_FILES
// cacheNumber defaults to 0 (delete oldest cache file)
// Set cacheNumber > 0 to delete a specific cached file
//
// NOTE: f->CacheCount for each player-list just keeps increasing... it's never
// decremented - the reasoning is that we can then always find the oldest cache
// file by searching for the lowest TPlayerURL* p->cacheNumber.
bool __fastcall TMainForm::DeleteCacheFile(TPlaylistForm* f, unsigned cacheNumber)
{
  bool bRet = false;

  try
  {
    // CacheCount will just keep going up as we add new files - which lets us
    // always know which is the oldest (and needs deleting)

// TODO: problem I'm having (I think) is - that we have f->CacheCount+1 > FMaxCacheFiles
// due to drag-dropping files between lists - and we delete files before the max is
// reached - perpetually - deleting files we need.
    if (FMaxCacheFiles > 0 && f->CacheCount+1 > FMaxCacheFiles)
    {
      // scan the list and delete oldest file
      unsigned minCacheCount = (unsigned)-1; // max value
      int listIdx = -1;
      for (int ii = 0; ii < f->CheckBox->Count; ii++)
      {
        TPlayerURL* p = (TPlayerURL*)f->CheckBox->Items->Objects[ii];

        // NOTE: a cacheNumber of 0 means the file has not been
        // cached and the returned URL will be the original file - which
        // we don't want to delete!!!!!!
        if (!p) continue;

        if (cacheNumber == 0)
        {
          // search for oldest cache file
          if (p->cacheNumber > 0 && p->cacheNumber < minCacheCount)
          {
            minCacheCount = p->cacheNumber;
            listIdx = ii;
          }
        }
        else // find the list index that matches a specific cacheNumber
        {
          if (p->cacheNumber == cacheNumber)
          {
            listIdx = ii;
            break;
          }
        }
      }

      if (listIdx >= 0)
      {
        TPlayerURL* p = (TPlayerURL*)f->CheckBox->Items->Objects[listIdx];

        // be extra careful not to delete the original music file!
        String sOrigCachePath = f->CheckBox->Items->Strings[listIdx];

        if (p && p->cacheNumber > 0 && FileExists(p->cachePath) && p->cachePath != sOrigCachePath)
        {
          //ShowMessage(sDelFile);
          String sCopy;
          //sCopy = L"/c takeown /f \"" + p->cachePath + "\"";
          //ShellCommand(L"open", L"cmd.exe", sCopy.c_str(), true);
          if (p->bDownloaded)
          {
            //Attrib Command Options
            //Item Explanation
            //attrib Execute the attrib command alone to see the attributes set on the files within the directory that you execute the command from.
            //+a Sets the archive file attribute to the file or directory.
            //-a Clears the archive attribute.
            //+h Sets the hidden file attribute to the file or directory.
            //-h Clears the hidden attribute.
            //+i Sets the 'not content indexed' file attribute to the file or directory.
            //-i Clears the 'not content indexed' file attribute.
            //+r Sets the read-only file attribute to the file or directory.
            //-r Clears the read-only attribute.
            //+s Sets the system file attribute to the file or directory.
            //-s Clears the system attribute.
            //+v Sets the integrity file attribute to the file or directory.
            //-v Clears the integrity attribute.
            //+x Sets the no scrub file attribute to the file or directory.
            //-x Clears the no scrub attribute.
            // drive:, path, filename This is the file (filename, optionally with drive and path), directory (path, optionally with drive), or drive that you want to view or change the attributes of. Wildcard use is allowed.
            // /s Use this switch to execute whatever file attribute display or changes you're making on the subfolders within whatever drive and/or path you've specified, or those within the folder you're executing from if you don't specify a drive or path.
            // /d This attrib option includes directories, not only files, to whatever you're executing. You can only use /d with /s.
            // /l The /l option applies whatever you're doing with the attrib command to the Symbolic Link itself instead of the target of the Symbolic Link. The /l switch only works when you're also using the /s switch.
            // /? Use the help switch with the attrib command to show details about the above options right in the Command Prompt window. Executing attrib /? is the same as using the help command to execute help attrib.
            //sCopy = L"/c attrib -s -r -h -i -x -u +a \"" + p->cachePath + "\""; // +/-RASHIXU "file" /S/D/L
            //ShellCommand(L"open", L"cmd.exe", sCopy.c_str(), true);
          }

          sCopy = L"/c del /Q /F \"" + p->cachePath + "\"";
          ShellCommand(L"open", L"cmd.exe", sCopy.c_str(), false);
          //if (!ShellCommand(L"open", L"cmd.exe", sCopy.c_str(), true))
          //  ShowMessage("failed to delete: \"" + String(sCopy) + "\"");

          // very important to restore original URL and dequeue this!!!!
          p->cachePath = sOrigCachePath;
          p->cacheNumber = 0;

          bRet = true; // return success
        }
      }
    }
  }
  catch(...) {}

  return bRet;
}
//---------------------------------------------------------------------------
String __fastcall TMainForm::GetURL(TCheckListBox* l, int idx)
{
  String sCachePath;

  try
  {
    TPlayerURL* p = (TPlayerURL*)l->Items->Objects[idx];

    if (p && bFileCacheEnabled)
      sCachePath = p->cachePath;
    else
      sCachePath = l->Items->Strings[idx];
  }
  catch(...) {};

  return sCachePath;
}
//---------------------------------------------------------------------------
// TODO - add copy of songs from web to playlist TPlayerURL URL object and
// temp-songs cache
//
int __fastcall TMainForm::WebGetData(String sUrl, String sDestFile)
{
  HINTERNET WEB_CONNECT = NULL;
  HINTERNET WEB_ADDRESS = NULL;
  int BytesWritten = 0;

  int iFileHandle = 0;
  HINSTANCE hDll = 0;
  tInternetOpen pInternetOpen; // InternetOpenW 0x007A8F0
  tInternetOpenUrl pInternetOpenUrl; // InternetOpenUrlW 0x0013CA40
  tInternetCloseHandle pInternetCloseHandle; // InternetCloseHandle 0x0005ECB0
  tInternetReadFile pInternetReadFile; // InternetReadFile 0x0007AE80
  try
  {
      hDll = LoadLibrary(L"wininet.dll"); // "wininet.dll"

      if (hDll == NULL)
        return -3;

      // Get custom dll's entry point
      pInternetOpen = (tInternetOpen)LoadProcAddr(hDll, L"InternetOpenW"); // need a leading underscore????
      pInternetOpenUrl = (tInternetOpenUrl)LoadProcAddr(hDll, L"InternetOpenUrlW"); // need a leading underscore????
      pInternetCloseHandle = (tInternetCloseHandle)LoadProcAddr(hDll, L"InternetCloseHandle"); // need a leading underscore????
      pInternetReadFile = (tInternetReadFile)LoadProcAddr(hDll, L"InternetReadFile"); // need a leading underscore????

      if (!pInternetOpen || !pInternetOpenUrl || !pInternetCloseHandle || !pInternetReadFile)
      {
        //DWORD err = GetLastError();
        //ShowMessage(String((int)pInternetOpenUrl) + ":" + String((int)err));
        return -4;
      }
  }
  catch(...)
  {
    return -4;
  }

//ShowMessage("here");
//return -2;
  try
  {
    try
    {
      iFileHandle = FileCreate(sDestFile);

      if (!iFileHandle)
        return -2;

      //  LPCWSTR lpszAgent,
      //  DWORD   dwAccessType,
      //  LPCWSTR lpszProxy,
      //  LPCWSTR lpszProxyBypass,
      //  DWORD   dwFlags
      WEB_CONNECT = pInternetOpen((LPCWSTR)L"Default_User_Agent",
       (DWORD)INTERNET_OPEN_TYPE_PRECONFIG, (LPCWSTR)NULL,
        (LPCWSTR)NULL, (DWORD)0);
      if (!WEB_CONNECT)
        return -5;

      //INTERNETAPI_(HINTERNET) InternetOpenUrlW(
      //    _In_ HINTERNET hInternet,
      //    _In_ LPCWSTR lpszUrl,
      //    _In_reads_opt_(dwHeadersLength) LPCWSTR lpszHeaders,
      //    _In_ DWORD dwHeadersLength,
      //    _In_ DWORD dwFlags,
      //    _In_opt_ DWORD_PTR dwContext
      //    );
      WEB_ADDRESS = pInternetOpenUrl(WEB_CONNECT,
       (LPCWSTR)sUrl.c_str(), (LPCWSTR)NULL, (DWORD)0,
        (DWORD)INTERNET_FLAG_KEEP_CONNECTION, (DWORD_PTR)0);
      if (!WEB_ADDRESS)
      {
         pInternetCloseHandle(WEB_CONNECT);
         return -6;
      }

      DWORD DataSize = URL_FILEMOVE_BUFSIZE;
      Char *Buf = new Char[DataSize];
      DWORD BytesRead = 0;

      do
      {
          Application->ProcessMessages(); // keep UI from hanging

          if (pInternetReadFile(WEB_ADDRESS, Buf, DataSize, &BytesRead))
          {
              if (BytesRead == 0)
                  break;

              //ShowMessage("Read " + String(BytesRead) + " from URL: \"" + String(sUrl) + "\"");

              if (BytesRead)
              {
                FileWrite(iFileHandle, Buf, BytesRead);
                BytesWritten += BytesRead;
              }
          }
          else
          {
              if (GetLastError() != ERROR_INSUFFICIENT_BUFFER)
              {
                  ShowMessage("Unable to read internet URL: \"" + String(sUrl) + "\"");
                  break;
              }

              delete[] Buf;
              DataSize += URL_FILEMOVE_BUFSIZE;
              Buf = new Char[DataSize];
          }
      }
      while (true);

      return BytesWritten;
    }
    catch(...)
    {
      return -7;
    }
  }
  __finally
  {
    if (WEB_ADDRESS)
      pInternetCloseHandle(WEB_ADDRESS);

    if (WEB_CONNECT)
      pInternetCloseHandle(WEB_CONNECT);

    if (iFileHandle)
      FileClose(iFileHandle);

    if (hDll)
      FreeLibrary(hDll);
  }

  return 0;
}
//---------------------------------------------------------------------------
FARPROC __fastcall TMainForm::LoadProcAddr(HINSTANCE hDll, String entry)
{
  if (entry.Length() == 0)
    return NULL;

  FARPROC addr;

  // Apparently GetProcAddress is AnsiString, not wchar_t!!!!!!!!!!
  AnsiString s = (AnsiString)entry;

  try
  {
    addr = (FARPROC)GetProcAddress(hDll, (LPCSTR)s.c_str());
  }
  catch(...)
  {
//    ShowMessage("exception thrown in GetProcAddress()!");
    return NULL;
  }

  return addr;
}
//---------------------------------------------------------------------------
//#include <dir.h>
//
//void __fastcall TForm1::Button1Click(TObject *Sender)
//{
//  char szFileName[MAXFILE+4];
//  int iFileHandle;
//  int iLength;
//  if (SaveDialog1->Execute())
//  {
//    if (FileExists(SaveDialog1->FileName))
//    {
//      fnsplit(SaveDialog1->FileName.c_str(), 0, 0, szFileName, 0);
//      strcat(szFileName, ".BAK");
//      RenameFile(SaveDialog1->FileName, szFileName);
//    }
//    iFileHandle = FileCreate(SaveDialog1->FileName);
//    // Write out the number of rows and columns in the grid.
//    FileWrite(iFileHandle, (char*)&(StringGrid1->ColCount), sizeof(StringGrid1->ColCount));
//    FileWrite(iFileHandle, (char*)&(StringGrid1->RowCount), sizeof(StringGrid1->RowCount));
//    for (int x=0;x<StringGrid1->ColCount;x++)
//    {
//      for (int y=0;y<StringGrid1->RowCount;y++)
//      {
//        // Write out the length of each string, followed by the string itself.
//        iLength = StringGrid1->Cells[x][y].Length();
//        FileWrite(iFileHandle, (char*)&iLength, sizeof(iLength));
//        FileWrite(iFileHandle, StringGrid1->Cells[x][y].c_str(), StringGrid1->Cells[x][y].Length());
//      }
//    }
//    FileClose(iFileHandle);
//  }
//}
//---------------------------------------------------------------------------
// UTF8 Encode/Decode
//---------------------------------------------------------------------------
//AnsiString __fastcall TMainForm::AnsiToUtf8(AnsiString sIn)
//// Code to convert ANSI to UTF-8 (Works!)
//{
//  if (sIn.IsEmpty()) return "";
//
//  // Use the MultiByteToWideChar function to determine the size of the UTF-16 representation of the string. You use
//  // this size to allocate a new buffer that can hold the UTF-16 version of the string.
//  DWORD dwNum = MultiByteToWideChar(CP_ACP, 0, sIn.c_str(), -1, NULL, 0);
//  wchar_t *pwText = new wchar_t[dwNum];
//
//  // The MultiByteToWideChar function takes the ASCII string and converts it into UTF-16, storing it in pwText.
//  MultiByteToWideChar(CP_ACP, 0, sIn.c_str(), -1, pwText, dwNum);
//
//  // The WideCharToMultiByte function tells you the size of the returned string so you can create a buffer for the UTF-8 representation.
//  dwNum = WideCharToMultiByte(CP_UTF8, 0, pwText, -1, NULL, 0, NULL, NULL);
//  char *psText = new char[dwNum];
//
//  // Convert the UTF-16 string into UTF-8, storing the result into psText.
//  WideCharToMultiByte(CP_UTF8, 0, pwText, -1, psText, dwNum, NULL, NULL);
//
//  delete [] pwText;
//
//  // Convert to VCL String.
//  if (dwNum > 1) sIn = String(psText, dwNum-1);
//  else sIn = "";
//
//  delete [] psText;
//
//  return sIn;
//}
//---------------------------------------------------------------------------
//AnsiString __fastcall TMainForm::Utf8ToAnsi(AnsiString sIn)
//// Code to convert UTF-8 to ASCII
//{
//  // Use the MultiByteToWideChar function to determine the size of the UTF-16 representation of the string. You use
//  // this size to allocate a new buffer that can hold the UTF-16 version of the string.
//  DWORD dwNum = MultiByteToWideChar(CP_UTF8, 0, sIn.c_str(), -1, NULL, 0);
//  wchar_t *pwText = new wchar_t[dwNum];
//
//  // The MultiByteToWideChar function takes the UTF-8 string and converts it into UTF-16, storing it in pwText.
//  MultiByteToWideChar(CP_UTF8, 0, sIn.c_str(), -1, pwText, dwNum);
//
//  // The WideCharToMultiByte function tells you the size of the returned string so you can create a buffer for the ANSI representation.
//  dwNum = WideCharToMultiByte(CP_ACP, 0, pwText, -1, NULL, 0, NULL, NULL);
//  char *psText = new char[dwNum];
//
//  // Convert the UTF-16 string into ANSI, storing the result into psText.
//  WideCharToMultiByte(CP_ACP, 0, pwText, -1, psText, dwNum, NULL, NULL);
//
//  delete [] pwText;
//
//  // Convert to VCL String.
//  if (dwNum > 1) sIn = String(psText, dwNum-1);
//  else sIn = "";
//
//  delete [] psText;
//
//  return sIn;
//}
//---------------------------------------------------------------------------
//WideString __fastcall TMainForm::Utf8ToWide(AnsiString sIn)
//// Code to convert UTF-8 to UTF-16 (wchar_t)
//{
//  WideString sWide = L"";
//
//  if (sIn.Length() > 0)
//  {
//    wchar_t* pwText = NULL;
//
//    try
//    {
//      // Use the MultiByteToWideChar function to determine the size of the UTF-16 representation of the string. You use
//      // this size to allocate a new buffer that can hold the UTF-16 version of the string.
//      DWORD dwNum = MultiByteToWideChar(CP_UTF8, 0, sIn.c_str(), -1, NULL, 0);
//      wchar_t *pwText = new wchar_t[dwNum];
//
//      if (pwText != NULL)
//      {
//        // The MultiByteToWideChar function takes the UTF-8 string and converts it into UTF-16, storing it in pwText.
//        MultiByteToWideChar(CP_UTF8, 0, sIn.c_str(), -1, pwText, dwNum);
//
//        // Convert to VCL WideString.
//        if (dwNum > 1)
//          sWide = WideString(pwText, dwNum-1);
//      }
//    }
//    // NOTE: Unlike C#, __finally does NOT get called if we execute a return!!!
//    __finally
//    {
//      try { if (pwText != NULL) delete [] pwText; } catch(...) {}
//    }
//  }
//
//  return sWide;
//}
//---------------------------------------------------------------------------
AnsiString __fastcall TMainForm::WideToUtf8(WideString sIn)
// Code to convert UTF-16 (WideString or WideChar wchar_t) to UTF-8
{
  if (sIn.IsEmpty()) return "";

  int nLenUtf16 = sIn.Length();

  int nLenUtf8 = WideCharToMultiByte(CP_UTF8, // UTF-8 Code Page
    0, // No special handling of unmapped chars
    sIn.c_bstr(), // wide-character string to be converted
    nLenUtf16,
    NULL, 0, // No output buffer since we are calculating length
    NULL, NULL); // Unrepresented char replacement - Use Default

  // nNameLen does NOT appear to include the NULL character!
  char* buf = new char[nLenUtf8];

  WideCharToMultiByte(CP_UTF8, // UTF-8 Code Page
    0, // No special handling of unmapped chars
    sIn.c_bstr(), // wide-character string to be converted
    nLenUtf16,
    buf,
    nLenUtf8,
    NULL, NULL); // Unrepresented char replacement - Use Default

  AnsiString sOut = String(buf, nLenUtf8); // there is no null written...
  delete [] buf;

  return sOut;
}
//---------------------------------------------------------------------------
//void __fastcall TMainForm::WMMove(TWMMove &Msg)
//{
//  try
//  {
//    if (MainForm->GDock != NULL)
//        MainForm->GDock->WindowMoved(this->Handle);
//    // call the base class handler
//    TForm::Dispatch(&Msg);
//  }
//  catch(...) { }
//}
//---------------------------------------------------------------------------
#if DEBUG_ON
// DEBUG CONSOLE!!!!!!!
static HANDLE m_Screen = NULL;

void __fastcall TMainForm::CInit(void)
{
  // allocate a console for this app
  // AllocConsole initializes standard input, standard output,
  // and standard error handles for the new console. The standard input
  // handle is a handle to the console's input buffer, and the standard
  // output and standard error handles are handles to the console's
  // screen buffer. To retrieve these handles, use the GetStdHandle function.
  AllocConsole();
  m_Screen = GetStdHandle (STD_OUTPUT_HANDLE);
}
//---------------------------------------------------------------------------
void __fastcall TMainForm::CWrite(String S)
{
  WriteConsole(m_Screen, String(S).w_str(),S.Length(),0,0);
}
#endif
//---------------------------------------------------------------------------

