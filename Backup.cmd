cd

IF "%~1"=="" GOTO BuildAll
IF "%~1"=="VersionBump" GOTO VersionBump

:VersionBump
call MsBuildSolution.cmd "VersionBump"
goto End

:BuildAll
call MsBuildSolution.cmd

:End
git add --all

git submodule foreach "git add --all"
set GIT_STATUS=%ERRORLEVEL% 
if not %GIT_STATUS%==0 goto eof 

git submodule foreach "git commit -m'Auto Update SubModules'"


git commit -m"Auto Version Update"
set GIT_STATUS=%ERRORLEVEL% 
if not %GIT_STATUS%==0 goto eof 

git push --recurse-submodules=check
set GIT_STATUS=%ERRORLEVEL% 
if not %GIT_STATUS%==0 goto eof

:PullRest
call PullReques.cmd

:eof
exit /b 0

:fail 
pause 
exit /b 1