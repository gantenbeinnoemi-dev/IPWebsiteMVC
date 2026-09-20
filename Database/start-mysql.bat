@echo off
rem Startet den MyBooks-MySQL-Server (Port 3307). Fenster offen lassen, solange du die Website testest.
rem Beenden: Fenster schliessen oder Strg+C.
"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqld.exe" --no-defaults --datadir="%USERPROFILE%\MyBooksMySQL\data" --port=3307 --mysqlx=OFF --bind-address=127.0.0.1 --console
