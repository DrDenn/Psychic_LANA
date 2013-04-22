using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
// added
using Psychic_Lana.Overhead;

namespace Psychic_Lana.Graphics
{
	public class SpriteSheet
	{
		public Texture2D Sheet;
		public int ElementWidth;
		public int ElementHeight;
		public int HorizontalCount;
		public int VerticalCount;

		public void Initialize(Texture2D spriteSheet, int elementWidth, int elementHeight)
		{
			Sheet = spriteSheet;
			ElementWidth = elementWidth;
			ElementHeight = elementHeight;
			HorizontalCount = Sheet.Width / ElementWidth;
			VerticalCount = Sheet.Height / ElementHeight;
		}
		public void Draw(SpriteBatch spriteBatch, Rectangle position, int xIndex, int yIndex)
		{
			spriteBatch.Draw(Sheet, position,
				new Rectangle((xIndex % HorizontalCount) * ElementWidth, (yIndex % VerticalCount) * ElementHeight, ElementWidth, ElementHeight),
				Color.White);
		}
	}
}
