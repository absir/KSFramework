local UIBase = import("UI/UIBase")

if not Cookie then local Cookie = Slua.GetClass('KSFramework.Cookie') end
if not Log then Log = Slua.GetClass('KEngine.Log') end
if not AB_Animator then AB_Animator = Slua.GetClass('Absir.AB_Animator') end

local UIDemo = {}
extends(UIDemo, UIBase)

-- create a ui instance
function UIDemo.New(controller)
    local newUI = new(UIDemo)
    newUI.Controller = controller
    return newUI
end

function UIDemo:OnInit(controller)
    Log.Info('UIDemo OnInit, do controls binding')
end

function UIDemo:OnOpen()
    Log.Info('UIDemo OnOpen, do your logic')
    Log.Info(self.Controller.name)

    local Image = self.Controller:FindGameObject('Image');

    Log.Info(Image.name)
    local animator = Image:GetComponent('Animator');

    local callback = function(index)
        Log.Info('lua callback');
    end

--    AB_Animator.Info('123');
--    AB_Animator.Info2('222', '333');
--    AB_Animator.PayAnimatorName(animator, 'DemoAni', 0, nil);
    AB_Animator.PayAnimatorName(animator, 'DemoAni', 0, callback);
end

function UIDemo:Awake()
    Log.Info('UIDemo Awake, do your logic')
end

function UIDemo:Start()
    Log.Info('UIDemo Start, do your logic')
end

return UIDemo
