rmdir Templete\Assets\System\Absir
rmdir Templete\Assets\System\Editor
rmdir Templete\Assets\System\Plugins

mklink Templete\Assets\System\Absir ..\KSFramework\Assets
mklink Templete\Assets\System\Editor ..\KSFramework\Editor
mklink Templete\Assets\System\Plugins ..\KSFramework\Plugins

mklink Templete\System	Templete\Assets\System