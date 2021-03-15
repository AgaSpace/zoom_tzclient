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
                    ShowGUI = !ShowGUI;

                if (!ShowGUI || Main.gameMenu || Main.player == null)
                    return;

                Main.LocalPlayer.mouseInterface = true;

                if (OldMouse.ScrollWheelValue < NewMouse.ScrollWheelValue) Page--;
                else if (OldMouse.ScrollWheelValue > NewMouse.ScrollWheelValue) Page++;

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                Main.spriteBatch.Draw(Gradient, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);
                Main.spriteBatch.Draw(Gradient, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);

                DrawPlayers();

                DrawMenu(Color.SkyBlue, Color.Aqua, new Rectangle(375, 75, 650, 450), "Manage");
                DrawInventory();

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
            
            Main.playerInventory = false;
            Main.inventoryScale = 0.6f;
            Player p = Main.player[SelectedPlayer];
            int f = 0;
            for (int i = 0; i < 10; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.DarkSlateGray, Color.SkyBlue, 380 + f, 145))
                    SelectedItem = i;
                f += 37;
            }
            f = 0;
            for (int i = 10; i < 20; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.DarkSlateGray, Color.SkyBlue, 380 + f, 145 + 37))
                    SelectedItem = i;
                f += 37;
            }
            f = 0;
            for (int i = 20; i < 30; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.DarkSlateGray, Color.SkyBlue, 380 + f, 145 + (37 * 2)))
                    SelectedItem = i;
                f += 37;
            }
            f = 0;
            for (int i = 30; i < 40; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.DarkSlateGray, Color.SkyBlue, 380 + f, 145 + (37 * 3)))
                    SelectedItem = i;
                f += 37;
            }
            f = 0;
            for (int i = 50; i < 54; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.Yellow, Color.SkyBlue, 380 + f, 145 + (37 * 4)))
                    SelectedItem = i;
                f += 37;
            }
            f += 37;
            for (int i = 54; i < 58; i++)
            {
                if (DrawItem(p.inventory[i].netID, p.inventory[i].stack, Color.YellowGreen, Color.SkyBlue, 380 + f, 145 + (37 * 4)))
                    SelectedItem = i;
                f += 37;
            }

            if (DrawItem(p.inventory[58].netID, p.inventory[58].stack, Color.Aqua, Color.SkyBlue, 380 + f, 145 + (37 * 4)))
                SelectedItem = 58;

            float pix = 145f + 35f + (37f * 5f);
            if (SelectedItem != -1)
            {
                DrawItem(p.inventory[SelectedItem].netID, p.inventory[SelectedItem].stack, Color.DarkSlateGray, Color.SkyBlue, 380, 145 + 35 +  (37 * 5));
                TextLight(p.inventory[SelectedItem].Name, 425f, pix, Color.White * 0.15f, Color.White, 1f);
                TextLight("Slot ID: " + SelectedItem, 425f, pix + 30f, new Color(0,0,0,0), Color.White, 1f);

                if (TextLightPlayerButton("Remove Item", 425, pix + 55f, 1f))
                    ClientUtils.SendData(new PacketWriter()
                        .SetType(15)
                        .PackInt16(1)
                        .PackInt16((short)p.whoAmI)
                        .PackInt16((short)SelectedItem)
                        .GetByteData());

            }
            if (TextLightPlayerButton("Freeze", 380, pix + 78f, 1f))
            {
                ChatHelper.SendChatMessageFromClient(new ChatMessage("/gbuff \"" + p.name + "\" 156 10"));
                ChatHelper.SendChatMessageFromClient(new ChatMessage("/gbuff \"" + p.name + "\" 47 10"));
                ChatHelper.SendChatMessageFromClient(new ChatMessage("/gbuff \"" + p.name + "\" 149 10"));
            }

            if (!Main.ServerSideCharacter) return;

            if (TextLightPlayerButton("Manage Inventory", 380, pix + 78f + 24f, 1f))
            {
                ChatHelper.SendChatMessageFromClient(new ChatMessage("/invsee \"" + p.name + "\""));
                Main.playerInventory = true;
                ShowGUI = false;
            }
            if (TextLightPlayerButton("Save & Restore Inventory", 380 + (int)(5f + FontAssets.MouseText.Value.MeasureString("Manage Inventory").X), pix + 78f + 24f, 1f))
            {
                ChatHelper.SendChatMessageFromClient(new ChatMessage("/invsee -s"));
            }
            if (TextLightPlayerButton("Restore Inventory", 380 + (int)(10f + FontAssets.MouseText.Value.MeasureString("Manage Inventory").X  + FontAssets.MouseText.Value.MeasureString("Save & Restore Inventory").X), pix + 78f + 24f, 1f))
            {
                ChatHelper.SendChatMessageFromClient(new ChatMessage("/invsee"));
            }
        }

        public void DrawPlayers()
        {
            TextLightDeathFont("Select Player", 45f, 25f, Color.White * 0.45f, Color.White, 1f);

            int j = 1;

            foreach (Player p in from i in Main.player where i.active select i)
            {
                if (TextLightPlayerButton(p.name, 45f, 75f + (j * 24), 1f))
                {
                    SelectedPlayer = p.whoAmI;
                    SelectedItem = -1;
                }
                j += 1;
            }
        }
        
        public bool DrawItem(int netID, int count, Color none, Color hovered, int X, int Y)
        {
            bool result = false;

            Color c = none;
            Rectangle r = new Rectangle(X, Y, 33, 33);
            if (r.Contains(Main.mouseX, Main.mouseY))
            {
                c = hovered; 
                if (NewMouse.LeftButton == ButtonState.Pressed && OldMouse.LeftButton == ButtonState.Released)
                    result = true;
            }

            Item i = new Item();
            i.SetDefaults(netID);
            i.stack = count;

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(X, Y, 30, 30), new Color(30, 30, 30));
            Main.spriteBatch.Draw(Gradient, new Rectangle(X, Y, 30, 30), Color.Black);

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(X, Y + 30, 30, 3), Color.Black);
            Main.spriteBatch.Draw(Gradient2, new Rectangle(X, Y + 30, 30, 3), c);
            ItemSlot.Draw(Main.spriteBatch, ref i, 21, new Vector2((float)X, (float)Y), default);

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

        public bool TextLightPlayerButton(string name, float x, float y, float size)
        {
            bool result = false;
            Color color1 = Color.White;
            Color color2 = new Color(0,0,0,0);
            Rectangle r = new Rectangle((int)x, (int)y, (int)FontAssets.MouseText.Value.MeasureString(name).X, (int)FontAssets.MouseText.Value.MeasureString(name).Y);
            if (r.Contains(Main.mouseX, Main.mouseY))
            {
                color1 = Color.Yellow;
                color2 = Color.Yellow * 0.25f;

                if (NewMouse.LeftButton == ButtonState.Pressed && OldMouse.LeftButton == ButtonState.Released)
                    result = true;
            }
            TextLight(name, x, y, color2, color1, size);
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
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x + 2f, y), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x - 2f, y), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x, y + 2f), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, text, new Vector2(x, y - 2f), color, 0f, default(Vector2), size, SpriteEffects.None, 0f);

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

        private int Page;
        private int SelectedPlayer;
        private int SelectedItem = -1;

        private Texture2D Gradient;
        private Texture2D Gradient2;
        public bool ShowGUI { get; private set; }
    }
}
