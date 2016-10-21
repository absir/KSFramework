set PROJECT=%1%
echo "Link Project %1%"

rmdir ..\Projects\%PROJECT%\Assets\System
rm ..\Projects\%PROJECT%\Assets\System

mklink /J ..\Projects\%PROJECT%\Assets\System	Templete\Assets\System