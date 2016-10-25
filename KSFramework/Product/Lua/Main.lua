local UIBase = import("UI/UIBase")

local Main = {}

function Main:Start()
    print("Main.lua start begin!")
    AB_Game.AddLogicStartActions(function()
        print("Main.lua start finish!")
    end);
end

return Main
