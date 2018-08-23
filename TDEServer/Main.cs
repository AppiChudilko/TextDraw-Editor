using System;
using System.Collections.Generic;
using System.IO;
using CitizenFX.Core;

namespace TDEServer
{
    public class Main : BaseScript
    {
        public static List<TdeItem> TdeList = new List<TdeItem>();
        
        public Main()
        {
            EventHandlers.Add("TDE:Export:Csharp", new Action<string, dynamic>(ExportCsharp));
            EventHandlers.Add("TDE:Export:Lua", new Action<string, dynamic>(ExportLua));
            EventHandlers.Add("TDE:Export", new Action<string, string>(Export));
            
            Debug.WriteLine("TDE By Appi. v0.1");
            Debug.WriteLine("TextDraw Editor has been loaded.");
        }

        public static void ExportCsharp(string fileName, dynamic data)
        {
            Debug.WriteLine("START EXPORT C#");
            
            TdeList.Clear();

            fileName = fileName + ".txt";

            Export(fileName, $"====================[TDE by APPI]======================");
            Export(fileName, $"                {DateTime.Now:dd/MM/yyyy} {DateTime.Now:HH:mm:ss tt}");
            Export(fileName, $"=======================================================");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"");
            
            var localData = (IList<Object>) data;
            foreach (var item in localData)
            {
                try
                {
                    var tdeInfo = new TdeItem();
                    var localItem = (IDictionary<String, Object>) item;
                
                    foreach (var property in typeof(TdeItem).GetProperties())
                        property.SetValue(tdeInfo, localItem[property.Name], null);

                    if (tdeInfo.Type == 0)
                        Export(fileName, $"DrawRectangle({tdeInfo.PosX}f, {tdeInfo.PosY}f, {tdeInfo.SizeW}f, {tdeInfo.SizeH}f, {tdeInfo.R}, {tdeInfo.G}, {tdeInfo.B}, {tdeInfo.A}, {tdeInfo.VerticalAligment}, {tdeInfo.HorizontalAligment});");
                    else if (tdeInfo.Type == 1)
                        Export(fileName, $"DrawText(\"{tdeInfo.Text}\", {tdeInfo.PosX}f, {tdeInfo.PosY}f, {tdeInfo.SizeW}f, {tdeInfo.R}, {tdeInfo.G}, {tdeInfo.B}, {tdeInfo.A}, {tdeInfo.Font}, {tdeInfo.Aligment}, {tdeInfo.Shadow.ToString().ToLower()}, {tdeInfo.Outline.ToString().ToLower()}, 0, {tdeInfo.VerticalAligment}, {tdeInfo.HorizontalAligment});");
                    else if (tdeInfo.Type == 2)
                        Export(fileName, $"DrawGameSprite(\"{tdeInfo.Name}\", \"{tdeInfo.Text}\", {tdeInfo.PosX}f, {tdeInfo.PosY}f, {tdeInfo.SizeW}f, {tdeInfo.SizeH}f, {tdeInfo.R}, {tdeInfo.G}, {tdeInfo.B}, {tdeInfo.A}, {tdeInfo.VerticalAligment}, {tdeInfo.HorizontalAligment});");
                    
                    TdeList.Add(tdeInfo);
                }
                catch (Exception e)
                {
                    Export(fileName, e.ToString());
                    throw;
                }
            }
            
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, "protected static SizeF Res = GetScreenResolutionMaintainRatio();");
            Export(fileName, "private static float Height = CitizenFX.Core.UI.Screen.Resolution.Height;");
            Export(fileName, "private static readonly float Ratio = Res.Width / Res.Height;");
            Export(fileName, "private static readonly float Width = Height * Ratio;");
            Export(fileName, "");
            Export(fileName, "public static SizeF GetScreenResolutionMaintainRatio()");
            Export(fileName, "{");
            Export(fileName, "    return new SizeF(CitizenFX.Core.UI.Screen.Resolution.Height * ((float) CitizenFX.Core.UI.Screen.Resolution.Width / (float) CitizenFX.Core.UI.Screen.Resolution.Height), CitizenFX.Core.UI.Screen.Resolution.Height);");
            Export(fileName, "}");
            Export(fileName, "");
            Export(fileName, "public static async void DrawGameSprite(string dict, string txtName, float xPos, float yPos, float width, float height, float heading, int r, int g, int b, int alpha, int vAlig = 0, int hAlig = 0)");
            Export(fileName, "{");
            Export(fileName, "    if (!IsHudPreferenceSwitchedOn() || !CitizenFX.Core.UI.Screen.Hud.IsVisible) return;");
            Export(fileName, "");
            Export(fileName, "    if (!HasStreamedTextureDictLoaded(dict))");
            Export(fileName, "        RequestStreamedTextureDict(dict, true);");
            Export(fileName, "");
            Export(fileName, "    float w = width / Width;");
            Export(fileName, "    float h = height / Height;");
            Export(fileName, "    float x = ToHorizontalAlignment(hAlig, xPos) / Width + w * 0.5f;");
            Export(fileName, "    float y = ToVerticalAlignment(vAlig, yPos) / Height + h * 0.5f;");
            Export(fileName, "");
            Export(fileName, "    CitizenFX.Core.Native.API.DrawSprite(dict, txtName, x, y, w, h, heading, r, g, b, alpha);");
            Export(fileName, "}");
            Export(fileName, "");
            Export(fileName, "");
            Export(fileName, "public static void DrawRectangle(float xPos, float yPos, float wSize, float hSize, int r, int g, int b, int alpha, int vAlig = 0, int hAlig = 0)");
            Export(fileName, "{");
            Export(fileName, "    if (!IsHudPreferenceSwitchedOn() || !CitizenFX.Core.UI.Screen.Hud.IsVisible) return;");
            Export(fileName, "");
            Export(fileName, "    float w = width / Width;");
            Export(fileName, "    float h = height / Height;");
            Export(fileName, "    float x = ToHorizontalAlignment(hAlig, xPos) / Width + w * 0.5f;");
            Export(fileName, "    float y = ToVerticalAlignment(vAlig, yPos) / Height + h * 0.5f;");
            Export(fileName, "");
            Export(fileName, "    DrawRect(x, y, w, h, r, g, b, alpha);");
            Export(fileName, "}");
            Export(fileName, "");
            Export(fileName, "public static void DrawText(string caption, float xPos, float yPos, float scale, int r, int g, int b, int alpha, int font, int justify, bool shadow, bool outline, int wordWrap, int vAlig = 0, int hAlig = 0)");
            Export(fileName, "{");
            Export(fileName, "    if (!IsHudPreferenceSwitchedOn() || !CitizenFX.Core.UI.Screen.Hud.IsVisible) return;");
            Export(fileName, "");
            Export(fileName, "    float x = ToHorizontalAlignment(hAlig, xPos) / Width;");
            Export(fileName, "    float y = ToVerticalAlignment(vAlig, yPos) / Height;");
            Export(fileName, "");
            Export(fileName, "    SetTextFont(font);");
            Export(fileName, "    SetTextScale(1f, scale);");
            Export(fileName, "    SetTextColour(r, g, b, alpha);");
            Export(fileName, "    if (shadow) SetTextDropShadow();");
            Export(fileName, "    if (outline) SetTextOutline();");
            Export(fileName, "");
            Export(fileName, "    switch (justify)");
            Export(fileName, "    {");
            Export(fileName, "        case 1:");
            Export(fileName, "            SetTextCentre(true);");
            Export(fileName, "            break;");
            Export(fileName, "        case 2:");
            Export(fileName, "            SetTextRightJustify(true);");
            Export(fileName, "            SetTextWrap(0, x);");
            Export(fileName, "            break;");
            Export(fileName, "    }");
            Export(fileName, "");
            Export(fileName, "    if (wordWrap != 0)");
            Export(fileName, "        SetTextWrap(x, (xPos + wordWrap) / Width);");
            Export(fileName, "");
            Export(fileName, "    BeginTextCommandDisplayText(\"STRING\");");
            Export(fileName, "");
            Export(fileName, "    const int maxStringLength = 99;");
            Export(fileName, "    for (int i = 0; i < caption.Length; i += maxStringLength)");
            Export(fileName, "        AddTextComponentSubstringPlayerName(caption.Substring(i, System.Math.Min(maxStringLength, caption.Length - i)));");
            Export(fileName, "    EndTextCommandDisplayText(x, y);");
            Export(fileName, "}");
            Export(fileName, "");
            Export(fileName, "public static float ToVerticalAlignment(int type, float x)");
            Export(fileName, "{");
            Export(fileName, "    if (type == 2)");
            Export(fileName, "        return Res.Height - x;");
            Export(fileName, "    if (type == 1)");
            Export(fileName, "        return Res.Height / 2 + x;");
            Export(fileName, "    return x;");
            Export(fileName, "}");
            Export(fileName, "");
            Export(fileName, "public static float ToHorizontalAlignment(int type, float y)");
            Export(fileName, "{");
            Export(fileName, "    if (type == 2)");
            Export(fileName, "        return Res.Width - y;");
            Export(fileName, "    if (type == 1)");
            Export(fileName, "        return Res.Width / 2 + y;");
            Export(fileName, "    return y;");
            Export(fileName, "}");
            Export(fileName, "");
            Export(fileName, "");
            Export(fileName, "");
            Export(fileName, $"=====================[Count {TdeList.Count}]======================");
            
            Debug.WriteLine($"FINISH EXPORT TEXT DRAWS ({TdeList.Count})");
        }

        public static void ExportLua(string fileName, dynamic data)
        {
            Debug.WriteLine("START EXPORT LUA");
            
            TdeList.Clear();

            fileName = fileName + ".lua";

            Export(fileName, $"--====================[TDE by APPI]======================");
            Export(fileName, $"--                {DateTime.Now:dd/MM/yyyy} {DateTime.Now:HH:mm:ss tt}");
            Export(fileName, $"--                Thanks Disquse for help");
            Export(fileName, $"--=======================================================");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"local Width, Height = GetActiveScreenResolution()");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"Citizen.CreateThread(function()");
            Export(fileName, $"    while true do");
            Export(fileName, $"        Width, Height = GetActiveScreenResolution()");
            Export(fileName, $"        Wait(100)");
            Export(fileName, $"    end");
            Export(fileName, $"end)");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"local function ToHorizontalAlignment(atype, x)");
            Export(fileName, $"    if atype == 2 then");
            Export(fileName, $"        return Width - x");
            Export(fileName, $"    elseif atype == 1 then");
            Export(fileName, $"        return Width / 2 + x");
            Export(fileName, $"    end");
            Export(fileName, $"    return x");
            Export(fileName, $"end");
            Export(fileName, $"");
            Export(fileName, $"local function ToVerticalAlignment(atype, y)");
            Export(fileName, $"    if atype == 2 then");
            Export(fileName, $"        return Height - y");
            Export(fileName, $"    elseif atype == 1 then");
            Export(fileName, $"        return Height / 2 + y");
            Export(fileName, $"    end");
            Export(fileName, $"    return y");
            Export(fileName, $"end");
            Export(fileName, $"");
            Export(fileName, $"function DrawText(caption, xPos, yPos, scale, r, g, b, alpha, font, justify, shadow, outline, wordWrap, vAlig, hAlig)");
            Export(fileName, $"    vAlig = vAlig or 0");
            Export(fileName, $"    hAlig = hAlig or 0");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"    if not IsHudPreferenceSwitchedOn() or IsHudHidden() then");
            Export(fileName, $"        return");
            Export(fileName, $"    end");
            Export(fileName, $"");
            Export(fileName, $"    local x = ToHorizontalAlignment(hAlig, xPos) / Width");
            Export(fileName, $"    local y = ToVerticalAlignment(vAlig, yPos) / Height");
            Export(fileName, $"");
            Export(fileName, $"    SetTextFont(font);");
            Export(fileName, $"    SetTextScale(1.0, scale);");
            Export(fileName, $"    SetTextColour(r, g, b, alpha);");
            Export(fileName, $"");
            Export(fileName, $"    if shadow then SetTextDropShadow() end");
            Export(fileName, $"    if outline then SetTextOutline() end");
            Export(fileName, $"");
            Export(fileName, $"    if justify == 1 then");
            Export(fileName, $"        SetTextCentre(true)");
            Export(fileName, $"    elseif justify == 2 then");
            Export(fileName, $"        SetTextRightJustify(true)");
            Export(fileName, $"        SetTextWrap(0, x)");
            Export(fileName, $"    end");
            Export(fileName, $"");
            Export(fileName, $"    if wordWrap ~= 0 then");
            Export(fileName, $"        SetTextWrap(x, (xPos + wordWrap) / Width)");
            Export(fileName, $"    end");
            Export(fileName, $"");
            Export(fileName, $"    BeginTextCommandDisplayText(\"STRING\")");
            Export(fileName, $"    local maxStringLength = 99");
            Export(fileName, $"    for i = 0, #caption, maxStringLength do");
            Export(fileName, $"        AddTextComponentSubstringPlayerName(string.sub(caption, i, #caption))");
            Export(fileName, $"    end");
            Export(fileName, $"    EndTextCommandDisplayText(x, y)");
            Export(fileName, $"end");
            Export(fileName, $"");
            Export(fileName, $"function DrawGameSprite(dict, txtName, xPos, yPos, width, height, heading, r, g, b, alpha, vAlig, hAlig)");
            Export(fileName, $"    vAlig = vAlig or 0");
            Export(fileName, $"    hAlig = hAlig or 0");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"    if not IsHudPreferenceSwitchedOn() or IsHudHidden() then");
            Export(fileName, $"        return");
            Export(fileName, $"    end");
            Export(fileName, $"");
            Export(fileName, $"    local x = ToHorizontalAlignment(hAlig, xPos) / Width");
            Export(fileName, $"    local y = ToVerticalAlignment(vAlig, yPos) / Height");
            Export(fileName, $"");
            Export(fileName, $"    if not HasStreamedTextureDictLoaded(dict) then");
            Export(fileName, $"        RequestStreamedTextureDict(dict, true)");
            Export(fileName, $"    end");
            Export(fileName, $"");
            Export(fileName, $"    local w = wSize / Width");
            Export(fileName, $"    local h = hSize / Height");
            Export(fileName, $"    local x = ToHorizontalAlignment(hAlig, xPos) / Width + w * 0.5");
            Export(fileName, $"    local y = ToVerticalAlignment(vAlig, yPos) / Height + h * 0.5");
            Export(fileName, $"");
            Export(fileName, $"    DrawSprite(dict, txtName, x, y, w, h, heading, r, g, b, alpha)");
            Export(fileName, $"end");
            Export(fileName, $"");
            Export(fileName, $"function DrawRectangle(xPos, yPos, wSize, hSize, r, g, b, alpha, vAlig, hAlig)");
            Export(fileName, $"    vAlig = vAlig or 0");
            Export(fileName, $"    hAlig = hAlig or 0");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"    if not IsHudPreferenceSwitchedOn() or IsHudHidden() then");
            Export(fileName, $"        return");
            Export(fileName, $"    end");
            Export(fileName, $"");
            Export(fileName, $"    local w = wSize / Width");
            Export(fileName, $"    local h = hSize / Height");
            Export(fileName, $"    local x = ToHorizontalAlignment(hAlig, xPos) / Width + w * 0.5");
            Export(fileName, $"    local y = ToVerticalAlignment(vAlig, yPos) / Height + h * 0.5");
            Export(fileName, $"");
            Export(fileName, $"    DrawRect(x, y, w, h, r, g, b, alpha)");
            Export(fileName, $"end");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"Citizen.CreateThread(function()");
            Export(fileName, $"    while true do");
            
            var localData = (IList<Object>) data;
            foreach (var item in localData)
            {
                try
                {
                    var tdeInfo = new TdeItem();
                    var localItem = (IDictionary<String, Object>) item;
                
                    foreach (var property in typeof(TdeItem).GetProperties())
                        property.SetValue(tdeInfo, localItem[property.Name], null);

                    if (tdeInfo.Type == 0)
                        Export(fileName, $"        DrawRectangle({tdeInfo.PosX}, {tdeInfo.PosY}, {tdeInfo.SizeW}, {tdeInfo.SizeH}, {tdeInfo.R}, {tdeInfo.G}, {tdeInfo.B}, {tdeInfo.A}, {tdeInfo.VerticalAligment}, {tdeInfo.HorizontalAligment})");
                    else if (tdeInfo.Type == 1)
                        Export(fileName, $"        DrawText(\"{tdeInfo.Text}\", {tdeInfo.PosX}, {tdeInfo.PosY}, {tdeInfo.SizeW}, {tdeInfo.R}, {tdeInfo.G}, {tdeInfo.B}, {tdeInfo.A}, {tdeInfo.Font}, {tdeInfo.Aligment}, {tdeInfo.Shadow.ToString().ToLower()}, {tdeInfo.Outline.ToString().ToLower()}, 0, {tdeInfo.VerticalAligment}, {tdeInfo.HorizontalAligment})");
                    else if (tdeInfo.Type == 2)
                        Export(fileName, $"        DrawGameSprite(\"{tdeInfo.Name}\", \"{tdeInfo.Text}\", {tdeInfo.PosX}, {tdeInfo.PosY}, {tdeInfo.SizeW}, {tdeInfo.SizeH}, {tdeInfo.R}, {tdeInfo.G}, {tdeInfo.B}, {tdeInfo.A}, {tdeInfo.VerticalAligment}, {tdeInfo.HorizontalAligment})");
                    
                    TdeList.Add(tdeInfo);
                }
                catch (Exception e)
                {
                    Export(fileName, e.ToString());
                    throw;
                }
            }
            
            Export(fileName, $"        Wait(0)");
            Export(fileName, $"    end");
            Export(fileName, $"end)");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"");
            Export(fileName, $"--=====================[Count {TdeList.Count}]======================");
            
            Debug.WriteLine($"FINISH EXPORT TEXT DRAWS ({TdeList.Count})");
        }
        
        public static void Export(string filename, string log)
        {
            try
            {
                File.AppendAllText($"{filename}", $" {log}\n");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e}");
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