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
using Psychic_Lana.Screens;
using Psychic_Lana.Graphics;
using Psychic_Lana.Maps;

namespace Psychic_Lana.GameObjects
{
	public enum AttackType { DirectControl, Wait, Seek, Path };
	public class Attack
	{
		public SpriteSheet CurrentGraphic;
		public Rectangle Collision;
		public Vector2 Position;
		public Dictionary<String, SpriteSheet> Graphics = new Dictionary<String, SpriteSheet>();
		public String GraphicState = "attack";
		public double Frame = 0;
		public bool Done;

		public virtual void Initialize(GameScreen gameScreen, int x, int y, Rectangle collisionbox)
		{
			Position.X = x;
			Position.Y = y;
			Collision = collisionbox;
		}
		/// <summary>
		/// Add a graphic to the sprite sheet dictionary
		/// </summary>
		/// <param name="name"> Key </param>
		/// <param name="spriteSheet"> Graphic </param>
		/// <param name="elementWidth"> Width of an individual Element </param>
		/// <param name="elementHeight"> Height of an individual Element </param>
		public virtual void AddSpriteSheet(String name, Texture2D spriteSheet, int elementWidth, int elementHeight)
		{
			SpriteSheet graphic = new SpriteSheet();
			graphic.Initialize(spriteSheet, elementWidth, elementHeight);
			Graphics.Add(name, graphic);
		}

		public virtual void Update(GameTime gameTime)
		{
			if (Frame >= 16)
				Done = true;
			Frame += 0.3;
		}
		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (Graphics.TryGetValue(GraphicState, out CurrentGraphic))
			{
				CurrentGraphic.Draw(spriteBatch, SpritePosition(), (int)Frame, 0);
			}
			else
			{
				GlobalReference.DrawFilledRectangle(spriteBatch, new Rectangle((int)Position.X, (int)Position.Y, Collision.Width, Collision.Height), Color.Red, 0.50f);
			}
			if (GlobalReference.debugGraphics)
			{
				GlobalReference.DrawFilledRectangle(spriteBatch, new Rectangle((int)Position.X, (int)Position.Y, Collision.Width, Collision.Height), Color.Red, 1.0f);
			}
		}
		/// <summary>
		/// Gets the postion of where the sprite should be drawn (since position is determined by the collision box
		/// </summary>
		/// <returns> The Rectangle representing the sprite to be drawn </returns>
		Rectangle SpritePosition()
		{
			return new Rectangle((int)Position.X - Collision.X, (int)Position.Y + Collision.Y + Collision.Height - CurrentGraphic.ElementHeight, CurrentGraphic.ElementWidth, CurrentGraphic.ElementHeight);
		}
	}
}
