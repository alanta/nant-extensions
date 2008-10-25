@echo off
set TEAMCITY_PROJECT_NAME=foo
cls

tools\NAnt\NAnt.exe -buildfile:sample-tests.build %*
pause
build-sample-tests.bat %*