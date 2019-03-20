SC \\%2 stop %4

REM --Wait For 10 seconds
"%SourcesDirectory%\Front Desk Management Tool\sleep" 10

REM --Check to see if service has stopped 
sc \\%2 query %4 | FIND "STATE" | FIND "STOPPED"
if %errorlevel% neq 0 exit %errorlevel%

REM -- Copies to Host Server
xcopy "%BinariesDirectory%\*" "\\%2%3" /S /F /R /Y /I
if %errorlevel% neq 0 exit %errorlevel%

SC \\%2 start %4

REM --Wait For 10 seconds
"%SourcesDirectory%\Front Desk Management Tool\sleep" 10

REM --Check to see if service is running 
sc \\%2 query %4 | FIND "STATE" | FIND "RUNNING"
if %errorlevel% neq 0 exit %errorlevel%
