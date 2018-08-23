using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using TDEClient.Managers;
using NativeUI;
using static CitizenFX.Core.Native.API;

namespace TDEClient
{
    public class Main : BaseScript
    {
        public static List<TdeItem> TdeList = new List<TdeItem>();
        public static bool EnableDebug;
        
        protected static UIMenu UiMenu = null;
        protected static MenuPool MenuPool = new MenuPool();
        protected static Size Screen;
        
        protected static SizeF Res = GetScreenResolutionMaintainRatio();
        private static float Height = CitizenFX.Core.UI.Screen.Resolution.Height;
        private static readonly float Ratio = Res.Width / Res.Height;
        private static readonly float Width = Height * Ratio;
        
        protected static Point CursorPosition;
        private static bool _isShowCursor = false;
        
        public Main()
        {
            Tick += OnTick;
            Tick += DrawUi;
            Tick += ProcessMainMenu;
            
            Screen = CitizenFX.Core.UI.Screen.Resolution;
            
            SetCursorSprite((int) CursorSprite.Normal);
        }
        
        public static SizeF GetScreenResolutionMaintainRatio()
        {
            return new SizeF(CitizenFX.Core.UI.Screen.Resolution.Height * ((float) CitizenFX.Core.UI.Screen.Resolution.Width / (float) CitizenFX.Core.UI.Screen.Resolution.Height), CitizenFX.Core.UI.Screen.Resolution.Height);
        }

        public static void ShowMainMenu()
        {
            HideMenu();
            var menu = new Menu();
            UiMenu = menu.Create("Menu", "~b~By Appi");
            
            ShowCursor(false);
            
            var list = new List<dynamic> {"Rectangle", "Text", "Sprite"};
            
            menu.AddMenuItemList(UiMenu, "Create", list).OnListSelected += async (uimenu, index) =>
            {
                HideMenu();

                if (index == 0)
                {
                    var tdeItem = new TdeItem
                    {
                        Id = TdeList.Count,
                        Type = index,
                        Text = "Rectangle",
                        R = 255,
                        G = 255,
                        B = 255,
                        A = 255,
                        SizeW = 100f,
                        SizeH = 10f,
                        IsSelected = true
                    };
                    TdeList.Add(tdeItem);
                    
                    foreach (var localItem in TdeList)
                        localItem.IsSelected = localItem.Id == tdeItem.Id;
                }
                else if (index == 1)
                {
                    string text = await Menu.GetUserInput("Text...", null, 128);
                    var tdeItem = new TdeItem
                    {
                        Id = TdeList.Count,
                        Type = index,
                        Text = text,
                        R = 255,
                        G = 255,
                        B = 255,
                        A = 255,
                        SizeW = 1f,
                        IsSelected = true
                    };
                    
                    TdeList.Add(tdeItem);
                    
                    foreach (var localItem in TdeList)
                        localItem.IsSelected = localItem.Id == tdeItem.Id;
                }
                else if (index == 2)
                {
                    string name = await Menu.GetUserInput("Dict", null, 128);
                    string text = await Menu.GetUserInput("Name", null, 128);
                    var tdeItem = new TdeItem
                    {
                        Id = TdeList.Count,
                        Type = index,
                        Text = text,
                        Name = name,
                        R = 255,
                        G = 255,
                        B = 255,
                        A = 255,
                        SizeW = 100f,
                        SizeH = 100f,
                        IsSelected = true
                    };
                    
                    TdeList.Add(tdeItem);
                    
                    foreach (var localItem in TdeList)
                        localItem.IsSelected = localItem.Id == tdeItem.Id;
                }
                
                ShowCursor(true);
                SendNotification("You can edit position (use LMB)");
                SendNotification("Press ~b~F1~s~ if you wanna stop edit");
            };
            
            menu.AddMenuItem(UiMenu, "TDE List").Activated += (uimenu, index) =>
            {
                HideMenu();
                ShowTdeListMenu();
            };
            
            var outline = new List<dynamic> {"Disable", "Enable"};
            var menuListItem2 = menu.AddMenuItemList(UiMenu, "Debug", outline);
            menuListItem2.Index = EnableDebug ? 1 : 0;
            menuListItem2.OnListSelected += (uimenu, index) =>
            {
                EnableDebug = index == 1;
            };
            
            menu.AddMenuItem(UiMenu, "~g~Export (C#)").Activated += async (uimenu, index) =>
            {
                HideMenu();
                if(TdeList.Count < 1)
                {
                    SendNotification("~r~There are no textdraws created.");
                    return;
                }
                var fileName = await Menu.GetUserInput("File name");
                TriggerServerEvent("TDE:Export:Csharp", fileName, TdeList);
                SendNotification($"Export ~b~C#~s~. Filename: ~b~{fileName}.txt");
            };
            
            
            menu.AddMenuItem(UiMenu, "~g~Export (Lua)").Activated += async (uimenu, index) =>
            {
                HideMenu();
                if(TdeList.Count < 1)
                {
                    SendNotification("~r~There are no textdraws created.");
                    return;
                }
                var fileName = await Menu.GetUserInput("File name");
                TriggerServerEvent("TDE:Export:Lua", fileName, TdeList);
                SendNotification($"Export ~b~Lua~s~. Filename: ~b~{fileName}.lua");
            };
            
            menu.AddMenuItem(UiMenu, "~o~Clear All").Activated += (uimenu, index) =>
            {
                HideMenu();
                ShowClearAllMenu();
            };
            
            menu.AddMenuItem(UiMenu, "~r~Close").Activated += (uimenu, item) =>
            {
                HideMenu();
            };
            
            MenuPool.Add(UiMenu);
        }

        public static void ShowTdeListMenu()
        {
            HideMenu();
            var menu = new Menu();
            UiMenu = menu.Create("Menu", "~b~Tde list");

            foreach (var item in TdeList)
            {
                menu.AddMenuItem(UiMenu, $"{item.Id}. {item.Text}").Activated += (uimenu, index) =>
                {
                    HideMenu();
                    ShowItemEditMenu(item);
                };
            }
            
            menu.AddMenuItem(UiMenu, "~g~Back").Activated += (uimenu, item) =>
            {
                HideMenu();
                ShowMainMenu();
            };
            
            menu.AddMenuItem(UiMenu, "~r~Close").Activated += (uimenu, item) =>
            {
                HideMenu();
                ShowCursor(false);
            };
            
            MenuPool.Add(UiMenu);
        }

        public static void ShowItemEditMenu(TdeItem tdeItem)
        {
            HideMenu();
            var menu = new Menu();
            UiMenu = menu.Create("Menu", $"~b~Edit: {tdeItem.Text}");
            
            foreach (var localItem in TdeList)
                localItem.IsSelected = localItem.Id == tdeItem.Id;
            
            menu.AddMenuItem(UiMenu, "Edit position").Activated += (uimenu, index) =>
            {
                HideMenu();
                ShowCursor(true);
                SendNotification("You can edit position (use LMB)");
                SendNotification("Press ~b~F1~s~ if you wanna stop edit");
            };
            
            menu.AddMenuItem(UiMenu, "Set RGBA color").Activated += async (uimenu, index) =>
            {
                HideMenu();
                int r = Convert.ToInt32(await Menu.GetUserInput("R", null, 3));
                int g = Convert.ToInt32(await Menu.GetUserInput("G", null, 3));
                int b = Convert.ToInt32(await Menu.GetUserInput("B", null, 3));
                int a = Convert.ToInt32(await Menu.GetUserInput("A", null, 3));

                foreach (var localItem in TdeList)
                {
                    if (tdeItem.Id != localItem.Id) continue;
                    localItem.R = r;
                    localItem.G = g;
                    localItem.B = b;
                    localItem.A = a;
                    tdeItem.R = r;
                    tdeItem.G = g;
                    tdeItem.B = b;
                    tdeItem.A = a;
                    SendNotification($"New color: {r} {g} {b} {a}");
                    ShowItemEditMenu(localItem);
                }
            };

            if (tdeItem.Type == 0 || tdeItem.Type == 2)
            {
                menu.AddMenuItem(UiMenu, "Size").Activated += async (uimenu, index) =>
                {
                    HideMenu();
                    float w = float.Parse(await Menu.GetUserInput("Width", null, 10));
                    float h = float.Parse(await Menu.GetUserInput("Height", null, 10));
                    foreach (var localItem in TdeList)
                    {
                        if (tdeItem.Id != localItem.Id) continue;
                        localItem.SizeW = w;
                        localItem.SizeH = h;
                        tdeItem.SizeW = w;
                        tdeItem.SizeH = h;
                        SendNotification($"New size: {w}px {h}px");
                        ShowItemEditMenu(localItem);
                    }
                };
                
                if (tdeItem.Type == 2)
                {
                    var menuItem0 = menu.AddMenuItem(UiMenu, "Heading");
                    menuItem0.SetRightLabel(tdeItem.Heading.ToString(CultureInfo.InvariantCulture));
                    menuItem0.Activated += async (uimenu, index) =>
                    {
                        HideMenu();
                        float w = float.Parse(await Menu.GetUserInput("Heading", null, 10));
                        foreach (var localItem in TdeList)
                        {
                            if (tdeItem.Id != localItem.Id) continue;
                            localItem.Heading = w;
                            tdeItem.Heading = w;
                            SendNotification($"New heading: {w}");
                            ShowItemEditMenu(localItem);
                        }
                    };
                }
            }
            else if (tdeItem.Type == 1)
            {
                menu.AddMenuItem(UiMenu, "Edit label").Activated += async (uimenu, index) =>
                {
                    HideMenu();
                    string l = await Menu.GetUserInput("New label", null, 128);
                    foreach (var localItem in TdeList)
                    {
                        if (tdeItem.Id != localItem.Id) continue;
                        localItem.Text = l;
                        tdeItem.Text = l;
                        SendNotification($"New label: {l}");
                        ShowItemEditMenu(localItem);
                    }
                };

                var menuItem0 = menu.AddMenuItem(UiMenu, "Font size");
                menuItem0.SetRightLabel(tdeItem.SizeW.ToString(CultureInfo.InvariantCulture));
                menuItem0.Activated += async (uimenu, index) =>
                {
                    HideMenu();
                    float w = float.Parse(await Menu.GetUserInput("Size", null, 10));
                    foreach (var localItem in TdeList)
                    {
                        if (tdeItem.Id != localItem.Id) continue;
                        localItem.SizeW = w;
                        tdeItem.SizeW = w;
                        SendNotification($"New font size: {w}");
                        ShowItemEditMenu(localItem);
                    }
                };
                
                var font = new List<dynamic> {"0", "1", "2", "3", "4", "5", "6", "7"};
                var menuListItem0 = menu.AddMenuItemList(UiMenu, "Font", font);
                menuListItem0.Index = tdeItem.Font;
                menuListItem0.OnListSelected += (uimenu, index) =>
                {
                    foreach (var localItem in TdeList)
                    {
                        if (tdeItem.Id != localItem.Id) continue;
                        localItem.Font = index;
                        tdeItem.Font = index;
                        SendNotification($"New font: {font[index]}");
                    }
                };
                
                var aligment = new List<dynamic> {"Left", "Center", "Right"};
                var menuListItem1 = menu.AddMenuItemList(UiMenu, "Aligment", aligment);
                menuListItem1.Index = tdeItem.Aligment;
                menuListItem1.OnListSelected += (uimenu, index) =>
                {
                    foreach (var localItem in TdeList)
                    {
                        if (tdeItem.Id != localItem.Id) continue;
                        localItem.Aligment = index;
                        tdeItem.Aligment = index;
                        SendNotification($"New aligment: {aligment[index]}");
                    }
                };
                
                var outline = new List<dynamic> {"Disable", "Enable"};
                var menuListItem2 = menu.AddMenuItemList(UiMenu, "Outline", outline);
                menuListItem2.Index = tdeItem.Outline ? 1 : 0;
                menuListItem2.OnListSelected += (uimenu, index) =>
                {
                    foreach (var localItem in TdeList)
                    {
                        if (tdeItem.Id != localItem.Id) continue;
                        localItem.Outline = index == 1;
                        tdeItem.Outline = index == 1;
                        SendNotification($"New outline: {outline[index]}");
                    }
                };
                
                var shadow = new List<dynamic> {"Disable", "Enable"};
                var menuListItem3 = menu.AddMenuItemList(UiMenu, "Shadow", shadow);
                menuListItem3.Index = tdeItem.Shadow ? 1 : 0;
                menuListItem3.OnListSelected += (uimenu, index) =>
                {
                    foreach (var localItem in TdeList)
                    {
                        if (tdeItem.Id != localItem.Id) continue;
                        localItem.Shadow = index == 1;
                        tdeItem.Shadow = index == 1;
                        SendNotification($"New shadow: {shadow[index]}");
                    }
                };
            }
            
            var floatH = new List<dynamic> {"Up", "Center", "Down"};
            var menuListItem4 = menu.AddMenuItemList(UiMenu, "Vertical align", floatH);
            menuListItem4.Index = tdeItem.VerticalAligment;
            menuListItem4.OnListSelected += (uimenu, index) =>
            {
                foreach (var localItem in TdeList)
                {
                    if (tdeItem.Id != localItem.Id) continue;
                    localItem.VerticalAligment = index;
                    tdeItem.VerticalAligment = index;
                    SendNotification($"New float vertical: {floatH[index]}");
                }
            };
            
            var floatW = new List<dynamic> {"Left", "Center", "Right"};
            var menuListItem5 = menu.AddMenuItemList(UiMenu, "Horizontal align", floatW);
            menuListItem5.Index = tdeItem.HorizontalAligment;
            menuListItem5.OnListSelected += (uimenu, index) =>
            {
                foreach (var localItem in TdeList)
                {
                    if (tdeItem.Id != localItem.Id) continue;
                    localItem.HorizontalAligment = index;
                    tdeItem.HorizontalAligment = index;
                    SendNotification($"New float horizontal: {floatW[index]}");
                }
            };
            
            menu.AddMenuItem(UiMenu, "~g~Copy").Activated += (uimenu, item) =>
            {
                HideMenu();
                TdeList.Add(tdeItem);
                ShowTdeListMenu();
            };
            
            menu.AddMenuItem(UiMenu, "~o~Delete").Activated += (uimenu, item) =>
            {
                HideMenu();
                TdeList.Remove(tdeItem);
                ShowTdeListMenu();
            };
            
            menu.AddMenuItem(UiMenu, "~r~Close").Activated += (uimenu, item) =>
            {
                HideMenu();
                ShowCursor(false);
            };
            
            MenuPool.Add(UiMenu);
        }

        public static void ShowClearAllMenu()
        {
            HideMenu();
            var menu = new Menu();
            UiMenu = menu.Create("Menu", "~b~Clear all?");
            
            menu.AddMenuItem(UiMenu, "~g~Yes").Activated += (uimenu, index) =>
            {
                HideMenu();
                TdeList.Clear();
                ShowMainMenu();
            };
            
            menu.AddMenuItem(UiMenu, "~r~No").Activated += (uimenu, item) =>
            {
                HideMenu();
                ShowMainMenu();
            };
            
            MenuPool.Add(UiMenu);
        }

        public static void ShowCursor(bool isShow)
        {
            _isShowCursor = isShow;
        }

        public static bool IsShowCursor()
        {
            return _isShowCursor;
        }

        public static Point GetCursorPoint()
        {
            return CursorPosition;
        }
        
        public static void SendNotification(string message, bool blink = true, bool saveToBrief = true)
        {
            SetNotificationTextEntry("THREESTRINGS");
            foreach (string msg in StringToArray(message))
                if (msg != null)
                    AddTextComponentSubstringPlayerName(msg);
            DrawNotification(blink, saveToBrief);
        }
        
        public static string[] StringToArray(string inputString)
        {
            string[] outputString = new string[3];

            var lastSpaceIndex = 0;
            var newStartIndex = 0;
            outputString[0] = inputString;

            if (inputString.Length <= 99) return outputString;
            
            for (int i = 0; i < inputString.Length; i++)
            {
                if (inputString.Substring(i, 1) == " ")
                {
                    lastSpaceIndex = i;
                }

                if (inputString.Length > 99 && i >= 98)
                {
                    if (i == 98)
                    {
                        outputString[0] = inputString.Substring(0, lastSpaceIndex);
                        newStartIndex = lastSpaceIndex + 1;
                    }
                    if (i > 98 && i < 198)
                    {
                        if (i == 197)
                        {
                            outputString[1] = inputString.Substring(newStartIndex, (lastSpaceIndex - (outputString[0].Length - 1)) - (inputString.Length - 1 > 197 ? 1 : -1));
                            newStartIndex = lastSpaceIndex + 1;
                        }
                        else if (i == inputString.Length - 1 && inputString.Length < 198)
                        {
                            outputString[1] = inputString.Substring(newStartIndex, ((inputString.Length - 1) - outputString[0].Length));
                            newStartIndex = lastSpaceIndex + 1;
                        }
                    }
                        
                    if (i <= 197) continue;
                        
                    if (i == inputString.Length - 1 || i == 296)
                    {
                        outputString[2] = inputString.Substring(newStartIndex, ((inputString.Length - 1) - outputString[0].Length) - outputString[1].Length);
                    }
                }
            }

            return outputString;
        }
        
        public static async void DrawSprite(string dict, string txtName, float xPos, float yPos, float width, float height, float heading, int r, int g, int b, int alpha, int vAlig = 0, int hAlig = 0)
        {
            if (!IsHudPreferenceSwitchedOn() || !CitizenFX.Core.UI.Screen.Hud.IsVisible) return;
        
            if (!HasStreamedTextureDictLoaded(dict))
                RequestStreamedTextureDict(dict, true);
            
            float w = width / Width;
            float h = height / Height;
            float x = ToHorizontalAlignment(hAlig, xPos) / Width + w * 0.5f;
            float y = ToVerticalAlignment(vAlig, yPos) / Height + h * 0.5f;
            
            CitizenFX.Core.Native.API.DrawSprite(dict, txtName, x, y, w, h, heading, r, g, b, alpha);
        }
        
        public static void DrawRectangle(float xPos, float yPos, float wSize, float hSize, int r, int g, int b, int alpha, int vAlig = 0, int hAlig = 0)
        {
            if (!IsHudPreferenceSwitchedOn() || !CitizenFX.Core.UI.Screen.Hud.IsVisible) return;
            
            float w = wSize / Width;
            float h = hSize / Height;
            float x = ToHorizontalAlignment(hAlig, xPos) / Width + w * 0.5f;
            float y = ToVerticalAlignment(vAlig, yPos) / Height + h * 0.5f;
        
            DrawRect(x, y, w, h, r, g, b, alpha);
        }
        
        public static void DrawText(string caption, float xPos, float yPos, float scale, int r, int g, int b, int alpha, int font, int justify, bool shadow, bool outline, int wordWrap, int vAlig = 0, int hAlig = 0)
        {
            if (!IsHudPreferenceSwitchedOn() || !CitizenFX.Core.UI.Screen.Hud.IsVisible) return;
        
            float x = ToHorizontalAlignment(hAlig, xPos) / Width;
            float y = ToVerticalAlignment(vAlig, yPos) / Height;
            
            SetTextFont(font);
            SetTextScale(1f, scale);
            SetTextColour(r, g, b, alpha);
            
            if (shadow) SetTextDropShadow();
            if (outline) SetTextOutline();
            switch (justify)
            {
                case 1:
                    SetTextCentre(true);
                    break;
                case 2:
                    SetTextRightJustify(true);
                    SetTextWrap(0, x);
                    break;
            }
        
            if (wordWrap != 0)
                SetTextWrap(x, (xPos + wordWrap) / Width);
        
            BeginTextCommandDisplayText("STRING");
        
            const int maxStringLength = 99;
            for (int i = 0; i < caption.Length; i += maxStringLength)
                AddTextComponentSubstringPlayerName(caption.Substring(i, System.Math.Min(maxStringLength, caption.Length - i)));
            
            EndTextCommandDisplayText(x, y);
        }
        
        public static float ToVerticalAlignment(int type, float x)
        {
            if (type == 2)
                return Res.Height - x;
            if (type == 1)
                return Res.Height / 2 + x;
            return x;
        }
        
        public static float ToHorizontalAlignment(int type, float y)
        {
            if (type == 2)
                return Res.Width - y;
            if (type == 1)
                return Res.Width / 2 + y;
            return y;
        }
        
        public static void HideMenu()
        {
            MenuPool = new MenuPool();
            UiMenu = null;
        }
        
        private static async Task ProcessMainMenu()
        {
            if (UiMenu != null)
            {
                if (UiMenu.Visible)
                {
                    if (Game.PlayerPed.IsDead)
                        HideMenu();
                    
                    Game.DisableControlThisFrame(0, (Control) 157);
                    Game.DisableControlThisFrame(0, (Control) 158);
                }
            }
            else
            {
                await Delay(10);
            }
        }
        
        private static async Task OnTick()
        {
            MenuPool.ProcessMenus();
            
            if ((Game.IsControlJustPressed(0, (Control) 288) || Game.IsDisabledControlJustPressed(0, (Control) 288)))
            {
                ShowMainMenu();
            }
            if ((Game.IsControlJustPressed(0, (Control) 289) || Game.IsDisabledControlJustPressed(0, (Control) 289)))
            {
                ShowCursor(false);
            }
            if ((Game.IsControlJustPressed(0, (Control) 170) || Game.IsDisabledControlJustPressed(0, (Control) 170)))
            {
                ShowCursor(true);
            }

            if (_isShowCursor)
            {
                ShowCursorThisFrame();
                Game.DisableAllControlsThisFrame(0);
                var x = GetDisabledControlNormal(0, (int) Control.CursorX) * Screen.Width;
                var y = GetDisabledControlNormal(0, (int) Control.CursorY) * Screen.Height;
                CursorPosition = new Point((int) x, (int) y);
            }
        }
        
        private static async Task DrawUi()
        {
            foreach (var item in TdeList)
            {
                if (item.IsSelected && IsShowCursor() && (Game.IsControlPressed(0, (Control) 135) || Game.IsDisabledControlJustPressed(0, (Control) 135)))
                {
                    var pos = GetCursorPoint();
                    item.PosX = pos.X;
                    item.PosY = pos.Y;

                    if (item.HorizontalAligment == 2)
                    {
                        item.PosX = Res.Width - pos.X;
                    }
                    else if (item.HorizontalAligment == 1)
                    {
                        var posX = Res.Width / 2;
                        item.PosX = (posX - pos.X) * -1;
                    }

                    if (item.VerticalAligment == 2)
                    {
                        item.PosY = Res.Height - pos.Y;
                    }
                    else if (item.VerticalAligment == 1)
                    {
                        var posY = Res.Height / 2;
                        item.PosY = (posY - pos.Y) * -1;
                    }

                    if (EnableDebug)
                        DrawText($"X: {item.PosX}. Y: {item.PosY}. MouseX: {pos.X}. MouseY: {pos.Y}.", 10, 10, 0.3f, 255, 255, 255, 255, 0, 0, false, true, 0);
                }
                
                if (item.Type == 0)
                    DrawRectangle(item.PosX, item.PosY, item.SizeW, item.SizeH, item.R, item.G, item.B, item.A, item.VerticalAligment, item.HorizontalAligment);
                else if (item.Type == 1)
                    DrawText(item.Text, item.PosX, item.PosY, item.SizeW, item.R, item.G, item.B, item.A, item.Font, item.Aligment, item.Shadow, item.Outline, 0, item.VerticalAligment, item.HorizontalAligment);
                else if (item.Type == 2)
                    DrawSprite(item.Name, item.Text, item.PosX, item.PosY, item.SizeW, item.SizeH, item.R, item.G, item.B, item.A, item.VerticalAligment, item.HorizontalAligment);
            }
        }
    }
}

public class TdeItem
{
    public int Id { get;set; }
    public float PosX { get;set; }
    public float PosY { get;set; }
    public float Heading { get;set; }
    public int Aligment { get;set; }
    public int HorizontalAligment { get;set; }
    public int VerticalAligment { get;set; }
    public float SizeW { get;set; }
    public float SizeH { get;set; }
    public int Type { get;set; }
    public string Text { get;set; }
    public string Name { get;set; }
    public int Font { get;set; }
    public bool Shadow { get;set; }
    public bool Outline { get;set; }
    public int R { get;set; }
    public int G { get;set; }
    public int B { get;set; }
    public int A { get;set; }
    public bool IsSelected { get;set; }
}