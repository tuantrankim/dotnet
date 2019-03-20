SC \\%1 stop %4

REM --Dirty Wait For 20 seconds
"%SourcesDirectory%\Front Desk Management Tool\Development\Dev1\HelloHost\HostUpdateTestEnviron.bat\sleep" 20

REM --Check to see if service has stopped 
sc \\%1 query %4 | FIND "STATE" | FIND "STOPPED"
if %errorlevel% neq 0 exit %errorlevel%

REM -- Copies to Host Server
xcopy "%SourcesDirectory%\Front Desk Management Tool\Development\Dev1\HelloHost\%BinariesDirectory%\*" "\\%1%3" /S /F /R /Y /I
if %errorlevel% neq 0 exit %errorlevel%

SC \\%1 start %4

REM --Dirty Wait For 20 seconds
"%SourcesDirectory%\Front Desk Management Tool\Development\Dev1\HelloHost\HostUpdateTestEnviron.bat\sleep" 20

REM --Check to see if service is running 
sc \\%1 query %4 | FIND "STATE" | FIND "RUNNING"
if %errorlevel% neq 0 exit %errorlevel%



SC \\%2 stop %4

REM --Dirty Wait For 20 seconds
"%SourcesDirectory%\Front Desk Management Tool\Development\Dev1\HelloHost\HostUpdateTestEnviron.bat\sleep" 20

REM --Check to see if service is running 
sc \\%2 query %4 | FIND "STATE" | FIND "STOPPED"
if %errorlevel% neq 0 exit %errorlevel%

REM -- Copies to Host Server
xcopy "%SourcesDirectory%\Front Desk Management Tool\Development\Dev1\HelloHost\%BinariesDirectory%\*" "\\%2%3" /S /F /R /Y /I
if %errorlevel% neq 0 exit %errorlevel%

SC \\%2 start %4

REM --Dirty Wait For 20 seconds
"%SourcesDirectory%\Front Desk Management Tool\Development\Dev1\HelloHost\HostUpdateTestEnviron.bat\sleep" 20

REM --Check to see if service is running 
sc \\%2 query %4 | FIND "STATE" | FIND "RUNNING"
if %errorlevel% neq 0 exit %errorlevel%

