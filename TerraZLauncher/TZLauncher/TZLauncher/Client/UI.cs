using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using ReLogic.Graphics;
using ReLogic;
using Microsoft.Xna.Framework.Input;
using Terraria.UI;
using Terraria.Chat;
using System.Diagnostics;
using Terraria.Graphics.Capture;
using System.Net;
using Newtonsoft.Json;
using System.Timers;

namespace TerraZ.Client
{
    public class MainUI
    {
        public void Initialize()
        {
            Gradient = new Texture2D(Main.spriteBatch.GraphicsDevice, 1, 100);
            Color[] colors = new Color[100];
            float f = 1f;
            for (int i = 0; i < 100; i++)
            {
                f -= 0.01f;
                colors[i] = new Color(255, 255, 255, f);
            }
            Gradient.SetData<Color>(colors);

            Gradient2 = new Texture2D(Main.spriteBatch.GraphicsDevice, 100, 1);
            Gradient2.SetData<Color>(colors);

            Action a = () =>
            {
                string str = "/clean";
                using (WebClient web = new WebClient())
                    str = web.DownloadString("http://s.terraz.ru:7878/status");

                RestAPI = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);
                TZLauncher.LauncherCore.WriteInfoBG(str);
            };
            Timer restTimer = new Timer(5000)
            {
                AutoReset = true,
                Enabled = true
            };
            restTimer.Elapsed += (x, y) => a.ThreadPush();
        }

        public void Draw()
        {
            try
            {
                OldMouse = NewMouse;
                NewMouse = Mouse.GetState();

                OldKeyboard = NewKeyboard;
                NewKeyboard = Keyboard.GetState();

                if (OldKeyboard.IsKeyUp(Keys.F2) && NewKeyboard.IsKeyDown(Keys.F2))
                {
                    ShowGUI = !ShowGUI;

                    if (CaptureManager.Instance.Active)
                        CaptureManager.Instance.Active = false;

                    if (Main.playerInventory)
                        Main.playerInventory = false;
                }

                if (!ShowGUI)
                    return;

                if (Main.playerInventory)
                    ShowGUI = false;

                if (CaptureManager.Instance.Active)
                    CaptureManager.Instance.Active = false;

                if (Main.gameMenu || Main.player == null)
                {
                    ShowGUI = false;
                    SelectedPlayer = -1;
                    SelectedItem = -1;
                    SelectedDefendersForgeItem = -1;
                    SelectedPiggyBankItem = -1;
                    SelectedSafeItem = -1;
                    SelectedVoidBagItem = -1;
                }

                Main.LocalPlayer.mouseInterface = true;

                if (OldMouse.ScrollWheelValue < NewMouse.ScrollWheelValue) Page++;
                else if (OldMouse.ScrollWheelValue > NewMouse.ScrollWheelValue) Page--;

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                Main.spriteBatch.Draw(Gradient, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);
                Main.spriteBatch.Draw(Gradient, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);

                string t = DateTime.Now.AddHours(EasterEgg_Hours).ToString("HH\\:mm\\:ss");
                Vector2 vec = FontAssets.MouseText.Value.MeasureString(t);
                Rectangle r = new Rectangle(375, 5, (int)vec.X, (int)vec.Y);
                if (r.Contains(Main.mouseX, Main.mouseY) && NewMouse.LeftButton == ButtonState.Pressed && OldMouse.LeftButton == ButtonState.Released)
                    ClickCounter++;

                if (ClickCounter >= 15)
                {
                    EasterEgg_Hours += 1;
                    ClickCounter = 0;
                }
                TextLightDeathFont(t, 375f, 5f, Color.White * 0.25f, Color.White, 1f);

                TextDefault($"TerraZ: {RestAPI["playercount"]}/{RestAPI["maxplayers"]}", r.X + 250, 20, Color.White, 1f);

                DrawPlayers();
                if (SelectedPlayer != -1)
                {
                    DrawMenu(Color.LightSkyBlue, Color.DeepSkyBlue, new Rectangle(375, 75, 850, 450), " ");

                    PagesPadding = 0;
                    if (ButtonV2("Главная страница", 380, 85, Color.White, Color.LightSkyBlue, Color.DeepSkyBlue, "OPACITIES\\MAIN_BUTTON") && View != ViewID.Inventory)
                    {
                        View = ViewID.Inventory;
                        SelectedItem = -1;
                    }
                    if (ButtonV2("Копилка", 385 + PagesPadding, 85, Color.White, Color.LightSkyBlue, Color.DeepSkyBlue, "OPACITIES\\PIGGY::BANK_BUTTON") && View != ViewID.PiggyBank)
                    {
                        View = ViewID.PiggyBank;
                        SelectedPiggyBankItem = -1;
                    }
                    if (ButtonV2("Сейф", 390 + PagesPadding, 85, Color.White, Color.LightSkyBlue, Color.DeepSkyBlue, "OPACITIES\\SAFE::BANK_BUTTON") && View != ViewID.Safe)
                    {
                        View = ViewID.Safe;
                        SelectedSafeItem = -1;
                    }
                    if (ButtonV2("Сумка бездны", 395 + PagesPadding, 85, Color.White, Color.LightSkyBlue, Color.DeepSkyBlue, "OPACITIES\\VOID::BAG::BANK_BUTTON") && View != ViewID.VoidBag)
                    {
                        View = ViewID.VoidBag;
                        SelectedVoidBagItem = -1;
                    }
                    if (ButtonV2("Печь защитника", 400 + PagesPadding, 85, Color.White, Color.LightSkyBlue, Color.DeepSkyBlue, "OPACITIES\\VOID::BAG::BANK_BUTTON") && View != ViewID.DefendersForge)
                    {
                        View = ViewID.DefendersForge;
                        SelectedDefendersForgeItem = -1;
                    }

                    switch (View)
                    {
                        case ViewID.Inventory:
                            DrawInventory();
                            break;
                        case ViewID.PiggyBank:
                            DrawPiggyBank();
                            break;
                        case ViewID.Safe:
                            DrawSafe();
                            break;
                    }
                }

                Main.DrawCursor(Vector2.Zero, Main.SmartCursorEnabled);

                Main.spriteBatch.End();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void DrawInventory()
        {
            if (SelectedPlayer < 0 || SelectedPlayer > 255 || Main.player[SelectedPlayer] == null)
                return;

            Main.inventoryScale = 0.6f;
            Player p = Main.player[SelectedPlayer];
            int f = 0;
            for (int i = 0; i < 10; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145, "OPACITIES\\ID_" + i))
                    SelectedItem = (SelectedItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 10; i < 20; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145 + 37, "OPACITIES\\ID_" + i))
                    SelectedItem = (SelectedItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 20; i < 30; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145 + (37 * 2), "OPACITIES\\ID_" + i))
                    SelectedItem = (SelectedItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 30; i < 40; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145 + (37 * 3), "OPACITIES\\ID_" + i))
                    SelectedItem = (SelectedItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 40; i < 50; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145 + (37 * 4), "OPACITIES\\ID_" + i))
                    SelectedItem = (SelectedItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 50; i < 54; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.YellowGreen, Color.Yellow, 380 + f, 145 + (37 * 5), "OPACITIES\\ID_" + i))
                    SelectedItem = (SelectedItem == i ? -1 : i);
                f += 37;
            }
            f += 37;
            for (int i = 54; i < 58; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.YellowGreen, Color.Yellow, 380 + f, 145 + (37 * 5), "OPACITIES\\ID_" + i))
                    SelectedItem = (SelectedItem == i ? -1 : i);
                f += 37;
            }

            if (DrawItem(p.inventory[58].netID, p.inventory[58].stack, Color.Aqua, Color.SkyBlue, 380 + f, 145 + (37 * 5), "OPACITIES\\ID_LAST"))
                SelectedItem = (SelectedItem == 58 ? -1 : 58);

            float pix = 145f + 35f + (37f * 5f);
            if (SelectedItem != -1)
            {
                DrawItem(p.inventory[SelectedItem].netID, p.inventory[SelectedItem].stack, Color.DarkSlateGray, Color.SkyBlue, 380, 145 + 35 + (37 * 5), "OPACITIES\\SELECTED_ITEM");
                TextLight(p.inventory[SelectedItem].Name, 425f, pix + 5f, Color.White * 0.15f, Color.White, 1f);
                TextLight("Слот: " + SelectedItem, 425f, pix + 30f, new Color(0, 0, 0, 0), Color.White, 1f);

                if (p.inventory[SelectedItem].stack != 0)
                    if (TextLightPlayerButton("Удалить предмет", 425, pix + 55f, 1f, "OPACITIES\\BUTTON_REMOVE::ITEM"))
                        new ServerData.InventoryRequest((byte)p.whoAmI, (short)SelectedItem).Send();

            }

            if (TextLightPlayerButton("Заморозить", 380, pix + 78f, 1f, "OPACITIES\\BUTTON_DISABLE::PLAYER"))
            {
                ChatHelper.SendChatMessageFromClient(new ChatMessage($"/gbuff {p.whoAmI} 156 10"));
                ChatHelper.SendChatMessageFromClient(new ChatMessage($"/gbuff {p.whoAmI} 47 10"));
                ChatHelper.SendChatMessageFromClient(new ChatMessage($"/gbuff {p.whoAmI} 149 10"));
            }

            if (!Main.ServerSideCharacter) return;

            if (TextLightPlayerButton("Скопировать инвентарь", 380, pix + 78f + 24f, 1f, "OPACITIES\\BUTTON_MANAGE::INVENTORY"))
            {
                ChatHelper.SendChatMessageFromClient(new ChatMessage("/invsee \"" + p.name + "\""));
                ShowGUI = false;
                Main.playerInventory = true;
            }
            if (TextLightPlayerButton("Сохранить и вернуть свой инвентарь", 380 + (int)(5f + FontAssets.MouseText.Value.MeasureString("Скопировать инвентарь").X), pix + 78f + 24f, 1f, "OPACITIES\\BUTTON_MANAGE::INVENTORY2"))
            {
                ChatHelper.SendChatMessageFromClient(new ChatMessage("/invsee -s"));
            }
            if (TextLightPlayerButton("Вернуть свой инвентарь", 380 + (int)(10f + FontAssets.MouseText.Value.MeasureString("Скопировать инвентарь").X + FontAssets.MouseText.Value.MeasureString("Сохранить и вернуть свой инвентарь").X), pix + 78f + 24f, 1f, "OPACITIES\\BUTTON_MANAGE::INVENTORY3"))
            {
                ChatHelper.SendChatMessageFromClient(new ChatMessage("/invsee"));
            }
        }
        public void DrawPiggyBank()
        {
            if (SelectedPlayer < 0 || SelectedPlayer > 255 || Main.player[SelectedPlayer] == null)
                return;

            Main.inventoryScale = 0.6f;
            Player p = Main.player[SelectedPlayer];
            int f = 0;
            for (int i = 0; i < 10; i++)
            {
                if (DrawItem(p.bank.item[i].netID, p.bank.item[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145, "OPACITIES\\PIGGY::BANK_ID_" + i))
                    SelectedPiggyBankItem = (SelectedPiggyBankItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 10; i < 20; i++)
            {
                if (DrawItem(p.bank.item[i].netID, p.bank.item[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145 + 37, "OPACITIES\\PIGGY::BANK_ID_" + i))
                    SelectedPiggyBankItem = (SelectedPiggyBankItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 20; i < 30; i++)
            {
                if (DrawItem(p.bank.item[i].netID, p.bank.item[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145 + (37 * 2), "OPACITIES\\PIGGY::BANK_ID_" + i))
                    SelectedPiggyBankItem = (SelectedPiggyBankItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 30; i < 40; i++)
            {
                if (DrawItem(p.bank.item[i].netID, p.bank.item[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145 + (37 * 3), "OPACITIES\\PIGGY::BANK_ID_" + i))
                    SelectedPiggyBankItem = (SelectedPiggyBankItem == i ? -1 : i);
                f += 37;
            }

            float pix = 145f + 35f + (37f * 5f);
            if (SelectedPiggyBankItem != -1)
            {
                DrawItem(p.bank.item[SelectedPiggyBankItem].netID, p.bank.item[SelectedPiggyBankItem].stack, Color.SkyBlue, Color.SkyBlue, 380, 145 + 35 + (37 * 5), "OPACITIES\\SELECTED_ITEM");
                TextLight(p.bank.item[SelectedPiggyBankItem].Name, 425f, pix, Color.White * 0.15f, Color.White, 1f);
                TextLight("Slot ID: " + SelectedPiggyBankItem, 425f, pix + 30f, new Color(0, 0, 0, 0), Color.White, 1f);

                if (p.bank.item[SelectedPiggyBankItem].stack != 0)
                    if (TextLightPlayerButton("Remove Item", 425, pix + 55f, 1f, "OPACITIES\\BUTTON_REMOVE::ITEM"))
                        new ServerData.InventoryRequest((byte)p.whoAmI, (short)(99 + SelectedPiggyBankItem)).Send();

            }
        }
        public void DrawSafe()
        {
            if (SelectedPlayer < 0 || SelectedPlayer > 255 || Main.player[SelectedPlayer] == null)
                return;

            Main.inventoryScale = 0.6f;
            Player p = Main.player[SelectedPlayer];
            int f = 0;
            for (int i = 0; i < 10; i++)
            {
                if (DrawItem(p.bank2.item[i].netID, p.bank2.item[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145, "OPACITIES\\PIGGY::BANK_ID_" + i))
                    SelectedSafeItem = (SelectedSafeItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 10; i < 20; i++)
            {
                if (DrawItem(p.bank2.item[i].netID, p.bank2.item[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145 + 37, "OPACITIES\\PIGGY::BANK_ID_" + i))
                    SelectedSafeItem = (SelectedSafeItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 20; i < 30; i++)
            {
                if (DrawItem(p.bank2.item[i].netID, p.bank2.item[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145 + (37 * 2), "OPACITIES\\PIGGY::BANK_ID_" + i))
                    SelectedSafeItem = (SelectedSafeItem == i ? -1 : i);
                f += 37;
            }
            f = 0;
            for (int i = 30; i < 40; i++)
            {
                if (DrawItem(p.bank2.item[i].netID, p.bank2.item[i].stack, Color.SkyBlue, Color.Blue, 380 + f, 145 + (37 * 3), "OPACITIES\\PIGGY::BANK_ID_" + i))
                    SelectedSafeItem = (SelectedSafeItem == i ? -1 : i);
                f += 37;
            }

            float pix = 145f + 35f + (37f * 5f);
            if (SelectedPiggyBankItem != -1)
            {
                DrawItem(p.bank2.item[SelectedSafeItem].netID, p.bank2.item[SelectedSafeItem].stack, Color.DarkSlateGray, Color.DarkSlateGray, 380, 145 + 35 + (37 * 5), "OPACITIES\\SELECTED_ITEM");
                TextLight(p.bank2.item[SelectedSafeItem].Name, 425f, pix, Color.White * 0.15f, Color.White, 1f);
                TextLight("Slot ID: " + SelectedSafeItem, 425f, pix + 30f, new Color(0, 0, 0, 0), Color.White, 1f);

                if (p.bank2.item[SelectedSafeItem].stack != 0)
                    if (TextLightPlayerButton("Remove Item", 425, pix + 55f, 1f, "OPACITIES\\BUTTON_REMOVE::ITEM"))
                        new ServerData.InventoryRequest((byte)p.whoAmI, (short)(99 + SelectedSafeItem)).Send();

            }
        }

        public void DrawPlayers()
        {
            Main.spriteBatch.Draw(Gradient, new Rectangle(0, 0, 16 * 20 + 45, Main.screenHeight + 140), Color.Black);

            // TextLightDeathFont("Select Player", 45f, 25f, Color.White * 0.45f, Color.White, 1f);

            int j = Page + 1;

            foreach (Player p in from i in Main.player where i.active select i)
            {
                if (SelectedPlayer == p.whoAmI)
                {
                    TextLight(p.name, 45f, 75f + (j * 24), (Color.Yellow * 0.25f), Color.Yellow, 1f);
                }

                if (!Opacityes.ContainsKey("OPACITIES\\PLAYER_SELECTION::" + p.name + "::" + p.whoAmI))
                    Opacityes.Add("OPACITIES\\PLAYER_SELECTION::" + p.name + "::" + p.whoAmI, Opacity.Generate());

                if (TextLightPlayerButton(p.name, 45f, 75f + (j * 24), 1f, "OPACITIES\\PLAYER_SELECTION::" + p.name + "::" + p.whoAmI))
                {
                    if (SelectedPlayer == p.whoAmI)
                    {
                        SelectedPlayer = -1;
                    }
                    else
                    {
                        SelectedPlayer = p.whoAmI;
                        SelectedItem = -1;
                    }
                }
                j += 1;
            }
        }

        public bool DrawItem(int netID, int count, Color none, Color hovered, int X, int Y, string opacityID)
        {
            if (!Opacityes.ContainsKey(opacityID))
            {
                Opacityes.Add(opacityID, Opacity.Generate());
            }

            bool result = false;

            Color c = none;
            Rectangle r = new Rectangle(X, Y, 33, 33);
            if (r.Contains(Main.mouseX, Main.mouseY))
            {
                Opacityes[opacityID]++;

                if (NewMouse.LeftButton == ButtonState.Pressed && OldMouse.LeftButton == ButtonState.Released)
                    result = true;
            }
            else Opacityes[opacityID]--;

            Item i = new Item();
            i.SetDefaults(netID);
            i.stack = count;

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(X, Y, 30, 30), new Color(30, 30, 30));
            Main.spriteBatch.Draw(Gradient, new Rectangle(X, Y, 30, 30), Color.Black);

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(X, Y + 30, 30, 3), c * (c.A - Opacityes[opacityID].PublicOpacity));
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(X, Y + 30, 30, 3), Color.Yellow * Opacityes[opacityID].PublicOpacity);

            ItemSlot.Draw(Main.spriteBatch, ref i, 21, new Vector2((float)X, (float)Y), default);

            return result;
        }

        public bool ButtonV2(string Text, int X, int Y, Color color, Color gradient1, Color gradient2, string opacityID)
        {
            if (!Opacityes.ContainsKey(opacityID))
            {
                Opacityes.Add(opacityID, Opacity.Generate());
            }

            bool result = false;

            Vector2 vec = FontAssets.MouseText.Value.MeasureString(" " + Text + " ");
            PagesPadding += (int)vec.X;
            Rectangle r = new Rectangle(X, Y, (int)vec.X, 33);
            if (r.Contains(Main.mouseX, Main.mouseY))
            {
                Opacityes[opacityID]++;

                if (NewMouse.LeftButton == ButtonState.Pressed && OldMouse.LeftButton == ButtonState.Released)
                    result = true;
            }
            else Opacityes[opacityID]--;

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(X, Y + 30, (int)vec.X, 3), gradient2 * (gradient2.A - Opacityes[opacityID].PublicOpacity));
            Main.spriteBatch.Draw(Gradient2, new Rectangle(X, Y + 30, (int)vec.X, 3), gradient1 * (gradient1.A - Opacityes[opacityID].PublicOpacity));

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(X, Y + 30, (int)vec.X, 3), gradient1 * Opacityes[opacityID].PublicOpacity);
            Main.spriteBatch.Draw(Gradient2, new Rectangle(X, Y + 30, (int)vec.X, 3), gradient2 * Opacityes[opacityID].PublicOpacity);

            TextDefault(" " + Text + " ", (int)X, (int)Y + 3, color * (color.A - Opacityes[opacityID].PublicOpacity), 1f);
            TextDefault(" " + Text + " ", (int)X, (int)Y + 3, Color.YellowGreen * Opacityes[opacityID].PublicOpacity, 1f);

            return result;
        }

        public void DrawMenu(Color gradient1, Color gradient2, Rectangle rect, string text)
        {
            Main.spriteBatch.Draw(Gradient, rect, Color.Black);
            Main.spriteBatch.Draw(Gradient, rect, Color.Black);

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 5), gradient1);
            Main.spriteBatch.Draw(Gradient2, new Rectangle(rect.X, rect.Y, rect.Width, 5), gradient2);

            TextLightDeathFont(text, (float)rect.X + 5f, (float)rect.Y + 15f, Color.White * 0.45f, Color.White, 0.5f);
        }

        public bool TextLightPlayerButton(string name, float x, float y, float size, string opacityID)
        {
            if (!Opacityes.ContainsKey(opacityID))
            {
                Opacityes.Add(opacityID, Opacity.Generate());
            }

            bool result = false;
            Color color1 = Color.White;
            Color color2 = new Color(0, 0, 0, 0);
            Rectangle r = new Rectangle((int)x, (int)y, (int)FontAssets.MouseText.Value.MeasureString(name).X, (int)FontAssets.MouseText.Value.MeasureString(name).Y);
            if (r.Contains(Main.mouseX, Main.mouseY))
            {
                Opacityes[opacityID]++;

                color2 = Color.Yellow * Opacityes[opacityID].PublicOpacity;

                if (NewMouse.LeftButton == ButtonState.Pressed && OldMouse.LeftButton == ButtonState.Released)
                    result = true;
            }
            else Opacityes[opacityID]--;

            TextLight(name, x, y, new Color(0, 0, 0, 0), color1 * (int)(color1.A - color2.A), size);
            TextLight(name, x, y, Color.Yellow * (float)(Opacityes[opacityID].PublicOpacity - 0.45f), color2, size);
            return result;
        }

        public static void TextLight(string text, float x, float y, Color color, Color maincolor, float size)
        {
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(x + 2f, y), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(x - 2f, y), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(x, y + 2f), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(x, y - 2f), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);

            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(x + 1f, y), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(x - 1f, y), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(x, y + 1f), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(x, y - 1f), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);

            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(x, y), maincolor, 0f, default(Vector2), size, SpriteEffects.None, 0f);
        }
        public static void TextLightDeathFont(string text, float x, float y, Color color, Color maincolor, float size)
        {
            /*
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x + 2f, y), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x - 2f, y), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x, y + 2f), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x, y - 2f), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            */

            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x + 1f, y), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x - 1f, y), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x, y + 1f), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x, y - 1f), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);

            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x, y), maincolor, 0f, default(Vector2), size, SpriteEffects.None, 0f);
        }
        public static void TextDefault(string text, float x, float y, Color maincolor, float size)
        {
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(x, y), maincolor, 0f, default(Vector2), size, SpriteEffects.None, 0f);
        }


        private MouseState OldMouse;
        private MouseState NewMouse;

        private KeyboardState OldKeyboard;
        private KeyboardState NewKeyboard;

        private ViewID View;
        private int PagesPadding;

        private Color MouseColor1;
        private Color MouseColor2;

        private Dictionary<string, object> RestAPI = new Dictionary<string, object>()
        {
            {
                "status",
                200
            },
            {
                "port",
                7777
            },
            {
                "name",
                "TerraZ.ru Server"
            },
            {
                "world",
                "TerraZ World"
            },
            {
                "serverversion",
                "1.4.1.2"
            },
            {
                "playercount",
                -1
            },
            {
                "maxplayers",
                15000
            },
            {
                "serverpassword",
                false
            }
        };

        public bool WindowAnimation;
        public float WindowAnimationSpeed;
        public float WindowOpacity;

        private int Page;
        private int SelectedPlayer = -1;
        private int SelectedDefendersForgeItem = -1;
        private int SelectedVoidBagItem = -1;
        private int SelectedSafeItem = -1;
        private int SelectedItem = -1;
        private int SelectedPiggyBankItem = -1;

        private Dictionary<string, Opacity> Opacityes = new Dictionary<string, Opacity>();

        private Texture2D Gradient;
        private Texture2D Gradient2;
        private int ClickCounter;

        private double EasterEgg_Hours;

        public bool ShowGUI { get; private set; }
        enum ViewID
        {
            Inventory, PiggyBank, Safe, DefendersForge, VoidBag
        }

        class Opacity
        {
            public static Opacity Generate() => new Opacity() 
            { 
                PrivateOpacity = 0f 
            };

            public static Opacity operator ++(Opacity o)
            {
                if (o.PrivateOpacity < 1f)
                    o.PrivateOpacity += 0.18f;
                return o;
            }
            public static Opacity operator --(Opacity o)
            {
                if (o.PrivateOpacity > 0f)
                    o.PrivateOpacity -= 0.18f;

                return o;
            }
            private Opacity() {}

            public float PublicOpacity => this.PrivateOpacity;

            private float PrivateOpacity;
        }

    }
}
