--------------------------------------------------------------------------------
--- Time Manager Dlx														 ---
--------------------------------------------------------------------------------

1. About:
--------------------------------------------------------------------------------

This program keeps track of your work time. Once installed
on your work PC it will log your working-day start and end.
You can use it to keep track of your working hours per week
and month or some other period of time.

It will display usefull information like when your work day
will be over. 

You can also use this programm to do your own accounting.


2. Install:
--------------------------------------------------------------------------------
1. Unzip files
2. Copy files to some place (e.g. C:\Programme\TimeManagerDlx)
3. Create new link to TimeManagerDlx.exe
4. Copy this link to Autostart folder
5. done

Now TMDlx will be started every time you start your computer.

3. Usage:
-------------------------------------------------------------------------------- 
You can (as described above) start this program automatically at computer startup
oder start it manually. It will start minimized. That means you will only see an 
icon in your systray. Double clicking this icon will show the window with your
work perio log.

It's important to minimize the program after you have had a look at your log. If
you press the close button the program will terminate. Thus be unable to log your
work end properly.

Restarting your computer is no problem at all. TimeManagerDlx will automatically
log the correct start and end. The start is always the first start of TimeManagerDlx
per day. The work end is always determined at the last shutdown of TimeManagerDlx.
As long as your work day starts on the same day it ends there is no problem.

If you have work times which pass 0:00 this will lead to wrong logging. In these cases
this program is to no use for you. Since the work period may differ greatly between
different persons there is no way telling if the period is a work period or a spare 
time period.

Selecting multiple entries will show an overview of the overall time worked and
the time you should have worked. This is shown in the status bar.

4. Work period log
-------------------------------------------------------------------------------- 
The program saves your work start and end to an xml file. The is located in your
home directory at this location:

C:\Users\myUsername\Documents\TimeManagerDlx\timeLog.xml

5. Edit settings:
-------------------------------------------------------------------------------- 
The programm comes with an xml file where you can set your work period and break
time. Though it might be to hard to locate this file though for an unexperienced
user. This file is located somewhere in your home folder.

Yeah - something like this:
C:\Users\myUsername\AppData\Local\Apps\2.0\NZ8NMEK8.JGZ\3N4BZ6QO.T8K\pixe..tion_9730ee460232148c_0001.0000_24b95b33d5fa1599

