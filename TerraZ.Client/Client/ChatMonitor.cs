using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace TerraZ.Client
{
	// Token: 0x0200001A RID: 26
	public class ChatMonitor
	{
		// Token: 0x0600006B RID: 107 RVA: 0x0000240E File Offset: 0x0000060E
		public ChatMonitor()
		{
			this._showCount = Settings.ChatLength;
			this._startChatLine = 0;
			this._messages = new List<ChatMessageContainer>();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002433 File Offset: 0x00000633
		public void NewText(string newText, byte R = 255, byte G = 255, byte B = 255)
		{
			this.AddNewMessage(newText, new Color((int)R, (int)G, (int)B), -1);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002446 File Offset: 0x00000646
		public void NewTextMultiline(string text, bool force = false, Color c = default(Color), int WidthLimit = -1)
		{
			this.AddNewMessage(text, c, WidthLimit);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00002D08 File Offset: 0x00000F08
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

		// Token: 0x0600006F RID: 111 RVA: 0x00002D60 File Offset: 0x00000F60
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

		// Token: 0x06000070 RID: 112 RVA: 0x00002452 File Offset: 0x00000652
		public void Clear()
		{
			this._messages.Clear();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00002F0C File Offset: 0x0000110C
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

		// Token: 0x06000072 RID: 114 RVA: 0x0000245F File Offset: 0x0000065F
		public void Offset(int linesOffset)
		{
			this._startChatLine += linesOffset;
			this.ClampMessageIndex();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00002F78 File Offset: 0x00001178
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

		// Token: 0x06000074 RID: 116 RVA: 0x00002475 File Offset: 0x00000675
		public void ResetOffset()
		{
			this._startChatLine = 0;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000247E File Offset: 0x0000067E
		public void OnResolutionChange()
		{
			this._recalculateOnNextUpdate = true;
		}

		// Token: 0x0400002F RID: 47
		private const int MaxMessages = 500;

		// Token: 0x04000030 RID: 48
		private int _showCount;

		// Token: 0x04000031 RID: 49
		private int _startChatLine;

		// Token: 0x04000032 RID: 50
		private List<ChatMessageContainer> _messages;

		// Token: 0x04000033 RID: 51
		private bool _recalculateOnNextUpdate;
	}
}
