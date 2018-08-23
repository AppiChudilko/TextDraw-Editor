using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using NativeUI;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using Sprite = NativeUI.Sprite;

namespace TDEClient.Managers
{
    public class Menu : BaseScript
    {
        public static bool IsShowInput = false;
        
        public UIMenu Create(string title, string subtitle, bool isResetBackKey = false, bool isDisableAllControls = false)
        {
            var menu = new UIMenu(title, subtitle, new Point((int) UIMenu.GetScreenResolutionMaintainRatio().Width - 450, 110)) //TODO Доработать
            {
                ScaleWithSafezone = false,
                MouseControlsEnabled = false,
                MouseEdgeEnabled = false,
                ControlDisablingEnabled = isDisableAllControls
            };
            
            menu.RefreshIndex();
            menu.Visible = true;
            
            if(isResetBackKey)
                menu.ResetKey(UIMenu.MenuControls.Back);
            
            return menu;
        }

        public void SetMenuBannerSprite(UIMenu menu, string spritedict, string spritename)
        {
            menu.SetBannerType(new Sprite(spritedict, spritename, new Point(), new Size()));
        }

        public UIMenuItem AddMenuItem(UIMenu menu, string title = "Меню", string subTitle = "", string rightLabel = "", UIMenuItem.BadgeStyle badge = UIMenuItem.BadgeStyle.None, bool isBadgeLeft = true)
        {
            var menuItem = new UIMenuItem(title, subTitle);
            
            if (rightLabel != "")
                menuItem.SetRightLabel(rightLabel);
            
            if (isBadgeLeft)
                menuItem.SetLeftBadge(badge);
            else
                menuItem.SetRightBadge(badge);
            
            menu.AddItem(menuItem);
            return menuItem;
        }

        public UIMenuListItem AddMenuItemList(UIMenu menu, string title, List<dynamic> list, string desc = "", UIMenuItem.BadgeStyle badge = UIMenuItem.BadgeStyle.None, bool isBadgeLeft = true)
        {
            var menuItem = new UIMenuListItem(title, list, 0, desc);
            if (isBadgeLeft)
                menuItem.SetLeftBadge(badge);
            else
                menuItem.SetRightBadge(badge);
            menu.AddItem(menuItem);
            return menuItem;
        }

        public UIMenuCheckboxItem AddCheckBoxItem(UIMenu menu, string title, bool isChecked = false, string desc = "", UIMenuItem.BadgeStyle badge = UIMenuItem.BadgeStyle.None, bool isBadgeLeft = true)
        {
            var menuItem = new UIMenuCheckboxItem(title, isChecked, desc);
            
            if (isBadgeLeft)
                menuItem.SetLeftBadge(badge);
            else
                menuItem.SetRightBadge(badge);
            
            menu.AddItem(menuItem);
            return menuItem;
        }

        public UIMenuColoredItem AddColoredItem(UIMenu menu, string title, string desc, Color color1, Color color2, string rightLabel = "", UIMenuItem.BadgeStyle badge = UIMenuItem.BadgeStyle.None, bool isBadgeLeft = true)
        {
            var menuItem = new UIMenuColoredItem(title, desc, color1, color2);
            
            if (rightLabel != "")
                menuItem.SetRightLabel(rightLabel);
            
            if (isBadgeLeft)
                menuItem.SetLeftBadge(badge);
            else
                menuItem.SetRightBadge(badge);
            
            menu.AddItem(menuItem);
            return menuItem;
        }
        
        public static async Task<string> GetUserInput(string windowTitle = null, string defaultText = null, int maxInputLength = 20)
        {
            IsShowInput = true;
            AddTextEntry("FMMC_KEY_TIP1", $"{windowTitle ?? "Enter"} ({maxInputLength})");

            DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP1", "", defaultText ?? "", "", "", "", maxInputLength);
            await Delay(0);
            while (true)
            {
                if (UpdateOnscreenKeyboard() == 1 || UpdateOnscreenKeyboard() == 2 || UpdateOnscreenKeyboard() == 3)
                    break;
                await Delay(0);
            }
            
            string result = GetOnscreenKeyboardResult();

            IsShowInput = false;
            if (!string.IsNullOrEmpty(result) && UpdateOnscreenKeyboard() == 1)
                return result;
            
            return "NULL";
        }
    }
}