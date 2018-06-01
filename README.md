Welcome to KoFrMa Backuper project.

HOW TO SET UP:

1. Download pack of software from KoFrMa Backup Solution (DatabaseCreator, RestAPI, Daemon, LocalDaemonConfig and AdminAppAngular).
2. Create database by running the SQL query from KoFrMa DatabaseCreator on your MySQL server.
3. Start KoFrMa RestAPI on your server where it will be permanently running and accessible by all devices 24/7.
4. Open RestAPI webpage either by accessing localhost directly from your server or by entering server address on different computer.
5. Enter the address of your MySQL server and credentials that allow unlimited access to KoFrMa database.
6. Host KoFrMa AdminApp on your server (can be different machine than the one hosting RestAPI or MySQL).
7. Login to the KoFrMa AdminApp with default credentials that were given to you with the license when you ordered it.
8. Install the KoFrMa Daemon on the computer(s) that you want to backup by running KoFrMa Daemon Installer and following instruction on-screen.
9. Run KoFrMa LocalDaemonConfig on the computer where the daemon has been installed and enter URL of the RestAPI server and password.
10. If you want to use .rar and .7z propriatory formats as destination for your backups, simply press the search button if you have them installed in most used locations or enter them manually.
11. Decide if you want to log actions that daemon does on local computer, either by writing to text file in given location or writing to Windows Event Log.
12. Restart your computer
13. You should now see the daemon listed in AdminApp and start adding tasks to it! If you want to install the daemon on multiple computers, repeat actions 8-12.

Thank you for using KoFrMa Backup Solution.
