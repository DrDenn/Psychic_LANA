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

namespace Psychic_Lana.Entities
{
	/// <summary>
	/// Enumeration representing various AI control Methods 
	/// </summary>
	public enum AIMode { DirectControl, Wait};
	/// <summary>
	/// Enumeration for Direction
	/// </summary>
	public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest, Nowhere };
	/// <summary>
	/// Class representing an in-game Entity. This holds all entity data and handles updates for AI and player control
	/// </summary>
	public class Entity
	{
		// Temporary Data
		public Texture2D sprite;

		// Positional Information
		/// <summary>
		/// Entity Collision Box
		/// </summary>
		public Rectangle Collision;
		/// <summary>
		/// Entity Position Vector (Top left of Collision Box
		/// </summary>
		public Vector2 Position;
		/// <summary>
		/// Vector pointing to the center of the hitbox (upper left corner)
		/// </summary>
		public Vector2 Center;
		/// <summary>
		/// Vector to the tile of Position (Position / 16)
		/// </summary>
		public Vector2 TilePosition;
		/// <summary>
		/// Vector of Movement
		/// </summary>
		public Vector2 Heading;
		/// <summary>
		/// Vector of Excess Position (beyond integer value
		/// </summary>
		public Vector2 Excess;
		/// <summary>
		/// Direction of movement
		/// </summary>
		public Direction MovementDirection;
		/// <summary>
		/// Movement Speed
		/// </summary>
		public float Speed = 5.5f; 


		/// <summary>
		/// Entity AI Mode
		/// </summary>
		public AIMode Mode = AIMode.Wait;
		GameScreen gameScreen;


		public virtual void Initialize(GameScreen gameScreen, Texture2D sprite, int x, int y, Rectangle Collision)
		{
			this.sprite = sprite;
			Position.X = x;
			Position.Y = y;
			GlobalReference.setTilePosition(Position, TilePosition);
			this.gameScreen = gameScreen;
			this.Collision = Collision;
		}

		public virtual void Update(GameTime gameTime)
		{
			UpdateAI();
			UpdateCollisions();
			UpdatePosition();
		}

		/// <summary>
		/// Updates the Heading based on the AI Mode
		/// </summary>
		void UpdateAI()
		{
			switch (Mode)
			{
				case AIMode.DirectControl:
					UpdateDirectControl();
					break;
				case AIMode.Wait:
					break;
				default:
					break;
			}
			// Calculate Direction
			if (Heading.Y < 0 && Heading.X == 0)
				MovementDirection = Direction.North;
			else if (Heading.Y < 0 && Heading.X > 0)
				MovementDirection = Direction.NorthEast;
			else if (Heading.Y == 0 && Heading.X > 0)
				MovementDirection = Direction.East;
			else if (Heading.Y > 0 && Heading.X > 0)
				MovementDirection = Direction.SouthEast;
			else if (Heading.Y > 0 && Heading.X == 0)
				MovementDirection = Direction.South;
			else if (Heading.Y > 0 && Heading.X < 0)
				MovementDirection = Direction.SouthWest;
			else if (Heading.Y == 0 && Heading.X < 0)
				MovementDirection = Direction.West;
			else if (Heading.Y < 0 && Heading.X < 0)
				MovementDirection = Direction.NorthWest;
			else
				MovementDirection = Direction.Nowhere;

			if (MovementDirection != Direction.Nowhere)
			{
				// Add Excess to Heading
				Heading = Heading + Excess;

				// Generate new Excess
				Excess.X = Heading.X - (float)Math.Floor(Heading.X);
				Excess.Y = Heading.Y - (float)Math.Floor(Heading.Y);

				// Truncate Heading
				Heading.X = (float)Math.Floor(Heading.X);
				Heading.Y = (float)Math.Floor(Heading.Y);
			}
			else
			{
				// Zero Heading and Excess
				Heading *= 0;
				Excess *= 0;
			}
			
		}
		/// <summary>
		/// Check for player input and update the Heading
		/// </summary>
		void UpdateDirectControl()
		{
			// Process Directional Inputs
			if (Input.Down(Controls.Up))
				Heading.Y -= 1;
			if (Input.Down(Controls.Right))
				Heading.X += 1;
			if (Input.Down(Controls.Down))
				Heading.Y += 1;
			if (Input.Down(Controls.Left))
				Heading.X -= 1;
			// If x and y are BOTH non-zero, normalize Heading
			if (Heading.X != 0 && Heading.Y != 0)
				Heading.Normalize();
			Heading = Heading * Speed;
		}
		/// <summary>
		/// Updates the heading based on the collisions
		/// </summary>
		void UpdateCollisions()
		{
			Vector2 topLeft = Position; // + Heading; // Also the position Vector
			Vector2 topRight = new Vector2(topLeft.X + (Collision.Width - 1), topLeft.Y);
			Vector2 bottomLeft = new Vector2(topLeft.X, topLeft.Y + (Collision.Height -1));
			Vector2 bottomRight = new Vector2(topRight.X, bottomLeft.Y);

			Vector2 xHeading = new Vector2(Heading.X, 0);
			Vector2 yHeading = new Vector2(0, Heading.Y);

			switch (MovementDirection)
			{
				case Direction.North:
					if (gameScreen.mapManager.Current.CheckCollisions(topLeft + yHeading, topRight + yHeading))
					{
						Heading.Y = (float)Math.Floor(Position.Y / GlobalReference.TileSize) * GlobalReference.TileSize - Position.Y;
					}
					break;
				case Direction.NorthEast:
					if (gameScreen.mapManager.Current.CheckCollisions(topLeft + yHeading, topRight + yHeading))
					{
						Heading.Y = (float)Math.Floor(Position.Y / 16) * 16 - Position.Y;
					}
					if (gameScreen.mapManager.Current.CheckCollisions(topRight + xHeading, bottomRight + xHeading))
					{
						Heading.X = (float)Math.Floor((Position.X + Collision.Width - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.X + Collision.Width);
					}
					if (xHeading.X == Heading.X && yHeading.Y == Heading.Y && gameScreen.mapManager.Current.CheckCollisions(topRight + Heading))
					{
						Heading.X = (float)Math.Floor((Position.X + Collision.Width - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.X + Collision.Width);
					}
					break;
				case Direction.East:
					if (gameScreen.mapManager.Current.CheckCollisions(topRight + xHeading, bottomRight + xHeading))
					{
						Heading.X = (float)Math.Floor((Position.X + Collision.Width - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.X + Collision.Width);
					}
					break;
				case Direction.SouthEast:
					if (gameScreen.mapManager.Current.CheckCollisions(topRight + xHeading, bottomRight + xHeading))
					{
						Heading.X = (float)Math.Floor((Position.X + Collision.Width - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.X + Collision.Width);
					}
					if (gameScreen.mapManager.Current.CheckCollisions(bottomLeft + yHeading, bottomRight + yHeading))
					{
						Heading.Y = (float)Math.Floor((Position.Y + Collision.Height - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.Y + Collision.Height);
					}
					if (xHeading.X == Heading.X && yHeading.Y == Heading.Y && gameScreen.mapManager.Current.CheckCollisions(bottomRight + Heading))
					{
						Heading.Y = (float)Math.Floor((Position.Y + Collision.Height - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.Y + Collision.Height);
					}
					break;
				case Direction.South:
					if (gameScreen.mapManager.Current.CheckCollisions(bottomLeft + yHeading, bottomRight + yHeading))
					{
						Heading.Y = (float)Math.Floor((Position.Y + Collision.Height - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.Y + Collision.Height);
					}
					break;
				case Direction.SouthWest:
					if (gameScreen.mapManager.Current.CheckCollisions(bottomLeft + yHeading, bottomRight + yHeading))
					{
						Heading.Y = (float)Math.Floor((Position.Y + Collision.Height - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.Y + Collision.Height);
					}
					if (gameScreen.mapManager.Current.CheckCollisions(topLeft + xHeading, bottomLeft + xHeading))
					{
						Heading.X = (float)Math.Floor(Position.X / GlobalReference.TileSize) * GlobalReference.TileSize - Position.X;
					}
					if (xHeading.X == Heading.X && yHeading.Y == Heading.Y && gameScreen.mapManager.Current.CheckCollisions(bottomLeft + Heading))
					{
						Heading.X = (float)Math.Floor(Position.X / GlobalReference.TileSize) * GlobalReference.TileSize - Position.X;
					}
					break;
				case Direction.West:
					if (gameScreen.mapManager.Current.CheckCollisions(topLeft + xHeading, bottomLeft + xHeading))
					{
						Heading.X = (float)Math.Floor(Position.X / GlobalReference.TileSize) * GlobalReference.TileSize - Position.X;
					}
					break;
				case Direction.NorthWest:
					if (gameScreen.mapManager.Current.CheckCollisions(topLeft + xHeading, bottomLeft + xHeading))
					{
						Heading.X = (float)Math.Floor(Position.X / GlobalReference.TileSize) * GlobalReference.TileSize - Position.X;
					}
					if (gameScreen.mapManager.Current.CheckCollisions(topLeft + yHeading, topRight + yHeading))
					{
						Heading.Y = (float)Math.Floor(Position.Y / GlobalReference.TileSize) * GlobalReference.TileSize - Position.Y;
					}
					if (xHeading.X == Heading.X && yHeading.Y == Heading.Y && gameScreen.mapManager.Current.CheckCollisions(topLeft + Heading))
					{
						Heading.X = (float)Math.Floor(Position.X / GlobalReference.TileSize) * GlobalReference.TileSize - Position.X;
					}
					break;
				default:

					break;
			}
		}
		/// <summary>
		/// Updates Position as well as general upkeep
		/// </summary>
		void UpdatePosition()
		{
			// Update Position
			Position = Position + Heading;

			// Update Tile Position
			TilePosition.X = (float)Math.Floor(Position.X / 16);
			TilePosition.Y = (float)Math.Floor(Position.Y / 16);

			// Update Center
			Center.X = Position.X + Collision.Width / 2;
			Center.Y = Position.Y + Collision.Height / 2;

			// Zero Heading
			Heading *= 0;

			// Update camera if player
			if (Mode == AIMode.DirectControl)
				Game.game.Camera = Matrix.CreateTranslation(
					-MathHelper.Clamp(Center.X - GlobalReference.ScreenWidth / 2, 0, gameScreen.mapManager.Current.Width * GlobalReference.TileSize - GlobalReference.ScreenWidth),
					-MathHelper.Clamp(Center.Y - GlobalReference.ScreenHeight / 2, 0, gameScreen.mapManager.Current.Height * GlobalReference.TileSize - GlobalReference.ScreenHeight), 0);

		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{

			spriteBatch.Draw(sprite, SpritePosition(), Color.White);
			if (GlobalReference.debugGraphics)
			{
				GlobalReference.DrawFilledRectangle(spriteBatch, new Rectangle((int)Position.X, (int)Position.Y, Collision.Width, Collision.Height), Color.Red, 0.50f);
			}
		}
		/// <summary>
		/// Gets the postion of where the sprite should be drawn (since position is determined by the collision box
		/// </summary>
		/// <returns> The Rectangle representing the sprite to be drawn </returns>
		Rectangle SpritePosition()
		{
			return new Rectangle((int)Position.X - Collision.X, (int)Position.Y - Collision.Y, sprite.Width, sprite.Height);
		}
	}
}
