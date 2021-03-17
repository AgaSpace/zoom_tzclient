using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace TerraZ.Client
{
	public class ChatMonitor : IChatMonitor
	{
		public ChatMonitor()
		{
			this._showCount = 25;
			this._startChatLine = 0;
			this._messages = new List<ChatMessageContainer>();
		}
		public void NewText(string newText, byte R = 255, byte G = 255, byte B = 255)
		{
			this.AddNewMessage(newText, new Color((int)R, (int)G, (int)B), -1);
		}
		public void NewTextMultiline(string text, bool force = false, Color c = default(Color), int WidthLimit = -1)
		{
			this.AddNewMessage(text, c, WidthLimit);
		}
		public void AddNewMessage(string text, Color color, int widthLimitInPixels = -1)
		{
			ChatMessageContainer chatMessageContainer = new ChatMessageContainer();
			chatMessageContainer.SetContents(text, color, widthLimitInPixels);
			this._messages.Insert(0, chatMessageContainer);
			while (this._messages.Count > 500)
			{
				this._messages.RemoveAt(this._messages.Count - 1);
			}
		}
		public void DrawChat(bool drawingPlayerChat)
		{
			int num = this._startChatLine;
			int num2 = 0;
			int num3 = 0;
			while (num > 0 && num2 < this._messages.Count)
			{
				int num4 = Math.Min(num, this._messages[num2].LineCount);
				num -= num4;
				num3 += num4;
				if (num3 == this._messages[num2].LineCount)
				{
					num3 = 0;
					num2++;
				}
			}
			int num5 = 0;
			int? num6 = null;
			int snippetIndex = -1;
			int? num7 = null;
			int num8 = -1;
			while (num5 < this._showCount && num2 < this._messages.Count)
			{
				ChatMessageContainer chatMessageContainer = this._messages[num2];
				if (!chatMessageContainer.Prepared || !(drawingPlayerChat | chatMessageContainer.CanBeShownWhenChatIsClosed))
				{
					break;
				}
				TextSnippet[] snippetWithInversedIndex = chatMessageContainer.GetSnippetWithInversedIndex(num3);
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, snippetWithInversedIndex, new Vector2(88f, (float)(Main.screenHeight - 30 - 28 - num5 * 21)), 0f, Vector2.Zero, Vector2.One, out num8, -1f, 2f);
				if (num8 >= 0)
				{
					num7 = new int?(num8);
					num6 = new int?(num2);
					snippetIndex = num3;
				}
				num5++;
				num3++;
				if (num3 >= chatMessageContainer.LineCount)
				{
					num3 = 0;
					num2++;
				}
			}
			if (num6 != null && num7 != null)
			{
				TextSnippet[] snippetWithInversedIndex2 = this._messages[num6.Value].GetSnippetWithInversedIndex(snippetIndex);
				snippetWithInversedIndex2[num7.Value].OnHover();
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					snippetWithInversedIndex2[num7.Value].OnClick();
				}
			}
		}
		public void Clear()
		{
			this._messages.Clear();
			ClientUtils.canAgainSendPackage = true;
		}
		public void Update()
		{
			if (this._recalculateOnNextUpdate)
			{
				this._recalculateOnNextUpdate = false;
				for (int i = 0; i < this._messages.Count; i++)
				{
					this._messages[i].MarkToNeedRefresh();
				}
			}
			for (int j = 0; j < this._messages.Count; j++)
			{
				this._messages[j].Update();
			}
		}
		public void Offset(int linesOffset)
		{
			this._startChatLine += linesOffset;
			this.ClampMessageIndex();
		}
		private void ClampMessageIndex()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = this._startChatLine + this._showCount;
			while (num < num4 && num2 < this._messages.Count)
			{
				int num5 = Math.Min(num4 - num, this._messages[num2].LineCount);
				num += num5;
				if (num < num4)
				{
					num2++;
					num3 = 0;
				}
				else
				{
					num3 = num5;
				}
			}
			int num6 = this._showCount;
			while (num6 > 0 && num > 0)
			{
				num3--;
				num6--;
				num--;
				if (num3 < 0)
				{
					num2--;
					if (num2 == -1)
					{
						break;
					}
					num3 = this._messages[num2].LineCount - 1;
				}
			}
			this._startChatLine = num;
		}
		public void ResetOffset()
		{
			this._startChatLine = 0;
		}
		public void OnResolutionChange()
		{
			this._recalculateOnNextUpdate = true;
		}

		private int _showCount;
		private int _startChatLine;
		private List<ChatMessageContainer> _messages;
		private bool _recalculateOnNextUpdate;
	}
}
