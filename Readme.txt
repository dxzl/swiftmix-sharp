SwiftMiX C# is authored by Scott Swift. v1.73 released under GPL v3 13-March-2017.

Built with Microsoft Visual Studio, Community 2017.

Unzip the files in the x86Dlls and Resourses folders.

Referenced DLLs:

From Windows XP Windows Media Player 11:
  AxInterop.WMPLib, Interop.WMPLib.dll

From Scott Swift's mediatag-sharp GitHub repository:
  MediaTags.dll, TaglibSharp.dll, WinMediaLib.dll

From SharpZipLib on GitHub:
  ICSharpCode.SharpZipLib.dll
  
Modified TorboDockableForm (provides the magnetic-windows effect)
  TorboSS.DockableForm.dll
  
From Utf8Checker project:
  Unicode.dll

You may also need to add a reference to IWshRuntimeLibrary:
This can be found under COM Objects "Scripthost Object Model"

Important! If you are building the installer, you must set the target to "Release" and "x86"!

To build with installer, first right-click the "SwiftMiX" project and choose "Rebuild", then right-click the "SwiftMiX Installer" project and choose "Rebuild". The SwiftMiX.msi Windows installer will be in "SwiftMiX Installer\Release".

Questions? Contact author: dxzl@live.com
