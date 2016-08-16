GEM (Git Environment Manager) Project
=====================================

Description:
------------
Windows systray icon-based manager for SCC environments.
Currently supports Git.

Features:
---------
1. Detect and track changes in local SCC environments.
2. Display information related to the current status of the tracked environments.
3. Allow commenting on environments for better tracking of their purpose.
4. Allow the execution of workspace-environment related operations.

External Packages:
------------------
1. libgit2sharp: https://github.com/libgit2/libgit2sharp
2. WPF NotifyIcon (Hardcodet.Wpf.TaskbarNotification): http://www.codeproject.com/Articles/36468/WPF-NotifyIcon
3. Extended WPF Toolkit™ Community Edition (Extended.Wpf.Toolkit): http://wpftoolkit.codeplex.com/
4. WPFConverters (Kent.Boogaart.Converters): https://github.com/kentcb/WPFConverters

Changelog:
----------

New version
- Added the ability to define custom commands to the context menu of the environments.
- Added an option to start with Windows (in Configuration/General).
- Added OK (v) and Cancel (x) buttons to the edit of an environment's custom comment.
- When editing an environment, the systray icon is changed to indicated that.
- Added 'Open Configuration file' to the main menu of the systray icon.
- Fixed: Could not open the custom environment text-editing when clicking on the environments' dialog on an empty area.
- Fixed: When altering the custom comment on an SCC environment, took time to update the screen. Now the update is immediate.
- Fixed: Sometimes copying information to the Clipboard would fail on an unrecoverable COM Exception.

Internally:
- Added utility: A custom button with changing images upon mouse hover.
- Added A utility text box with a clear button and a red-border to indicate invalid value, used in several places.

Features to add:
----------------
2. Configuration GUI for the environments' custom menu.
3. When the application fails to load the configuration, allow to open the .config file for editing. If the user edits - try to load it again.
4. Edit box for environment details - allow multiline.
5. Color coding for the environments.
5.2. Quick view
    - A more elaborate view with colors.
    - Indicate when there's an environment being edited.
5.3. Convert messagebox dialog messages to a notifications in the systray.
6. Environment SCC status.
7. Environment SCC properties.

Current Issues:
---------------
1. When clicking an environment, the text in the TextBox is all selected, only it does not show.
2. Running an elevated command with a working directory set - the working directory is ignored. This is a conflict with the properties of the ProcessStartInfo used to spawn the process.
	- In order to run elevated, the Verb property needs to be set to "runas", but also UseShellExecute set to 'true'. When UseShellExecute is 'true', the WorkingDirectory property is taken
		as the executable's path rather than the direcrtory set for the running environment of the process.
3. Copying the 'Info' on an environment to the Clipboard, it may return an old value (very rare).

16.11.2015 Version 1.1.0
========================
- In the 'GEM identified environments' list, added a checkbox that allows to select the environments that appear on the quick-view (mouse hover on the tray icon).
- Projects target .NET v4.5.2 .
- Upon start, consolidating of the configuration to the actual environments state by the folder name and SCC-information changed to
  only the folder name. This allows keeping the comment for environments that change SCC information (Git branch) even when the
  application has crashed or not working at the moment.
- Added a "Open Latest Log" user-command to the Application-menu.
- Copy to clipboard command is retried with constant 500msec wait, because if another process is locking the clipboard - the command fails.
- Logging of all user-commands.
- Fixed: When refreshing the folders structure, the custom information would get lost and also from the configuration (irrecoverable).