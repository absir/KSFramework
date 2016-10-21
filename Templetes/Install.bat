rmdir Templete\Assets\System\Absir
rmdir Templete\Assets\System\Editor
rmdir Templete\Assets\System\Plugins

rm Templete\Assets\System\Absir
rm Templete\Assets\System\Editor
rm Templete\Assets\System\Plugins

mklink /J Templete\Assets\System\Absir ..\KSFramework\Assets\Absir
mklink /J Templete\Assets\System\Editor ..\KSFramework\Assets\Editor
mklink /J Templete\Assets\System\Plugins ..\KSFramework\Assets\Plugins

:: rmdir Templete\System
:: rm Templete\System

:: mklink /J Templete\System	Templete\Assets\System