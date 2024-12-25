@echo off
echo:signtool sign /f "C:\Users\DewzaCSharp\Documents\! c# projects\DeSeBot Selfbot\build\DewzaCSharp.pfx" /p 123 /tr http://timestamp.digicert.com /td sha512 /fd sha512 "C:\Users\DewzaCSharp\Documents\! c# projects\DeSeBot Selfbot\build\DeSeBotSelfbot.exe"
echo:
echo:
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.20348.0\x64\startcmdhere.bat"
pause