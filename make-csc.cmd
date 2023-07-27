@echo off
setlocal

:: find csc.exe, more details see https://zhuanlan.zhihu.com/p/41821353
for /f "delims=" %%g in ('dir /s/b %windir%\Microsoft.NET\csc.exe') do (set CSC=%%g)
echo @%CSC% %%* > csc.cmd
