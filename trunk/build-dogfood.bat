@echo off
cls

tools\NAnt\NAnt.exe -buildfile:default.build %1

if %errorlevel% EQU 0 (
	for /r deploy %%f in (*.zip) do (
		echo %%f

		tools\Info-ZIP\unzip.exe -o "%%f" * -x License*.txt -d tools\NAnt
		echo.
	)
)

pause
build-dogfood.bat