--------------------------------------------------------------------------------
--- Flex Time Monitor														 ---
--------------------------------------------------------------------------------

1. About:
--------------------------------------------------------------------------------

This program keeps track of your work time. Once installed it will log your PCs'
start and shutdown. With this data it can easily calculate your attendance. The 
results are shown as table. All entries may be modified and you may also add notes.

There is no voodoo going on. All data is stored in a simple xml file which can also
be viewed and edited with your favorite text editor. Also it does only keep track
of the start and end and will not bulge your system with megabytes of reboot logs. 

Complete control: you can either start it automatically (just copy a application 
link to your autostart folder) or manually every tim you like. So you can easily 
log  your efforts on some project.

2. Install:
--------------------------------------------------------------------------------
Make a backup of your history and deinstall (important) any previous versions of
Flex Time Monitor before proceeding.

1. Unzip files
2. Run setup.exe
3. Copy "doc/Autostart FlexTimeMonitor" to autostart
4. done

3. Usage:
--------------------------------------------------------------------------------
You can use the automatic start method to log your personal working hours. You
may also start and close it manually and use it to log certain periods of time 
(e.g. for a certain project).

Restarting your computer is no problem at all. Flex Time Monitor will automatically
log the correct start and end. The start is always the first start of your computer
on that day. The end is always the least shutdown on that same day.

Selecting multiple entries will show an overview of the overall time worked and
the time you should have worked. This is shown in the status bar.

You can add notes to your log file (e.g. working at home). It is possible to
edit the table fields for start and end too.

It's important to minimize the program if you want it to be minimized! A lot of
programms in the history used the close button to minimize. This doesn't make 
sense and so most applications today use the buttons accordingly. So does Flex
Time Monitor.

5. Edit settings:
-------------------------------------------------------------------------------- 
The application offers a config dialogue for certain option such as break time
and work period. 

4. Limitations
-------------------------------------------------------------------------------- 
If your work period starts on one day and ends on another (e.g. start at 20:00, 
end at 04:00) Flex Time Monitor is able to log and handle this correctly as long
as you do not restart your computer on the second day.

There is no way to enter an extended break period for a certain day. Feel free 
to add a note though.

5. Work period log
-------------------------------------------------------------------------------- 
The program saves your work start and end to an xml file. The is located in your
home directory at this location:

C:\Users\myUsername\Documents\FlexTimeMonitor\flextimeLog.xml

6. Developer
-------------------------------------------------------------------------------- 
As a developer you may want to know where to find the user configuration files:

During development look here:
C:\Users\myUsername\AppData\Local\Microsoft\FlexTimeMonitor.vshost.ex_StrongName_y12c2ghk044xvlj3hpitw4wpwaoetz0y\1.0.0.0\user.config

After installation look here:
C:\Users\myUsername\AppData\Local\Apps\2.0\NZ8NMEK8.JGZ\3N4BZ6QO.T8K\Fle..tor_9730ee460232148c_0001.0000_24b95b33d5fa1599