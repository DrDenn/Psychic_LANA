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

namespace Psychic_Lana.Entities
{
	/// <summary>
	/// Enumeration representing various AI control Methods 
	/// </summary>
	public enum AIMode { DirectControl, Wait };
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
		public SpriteSheet CurrentGraphic;
		public Dictionary<String, SpriteSheet> Graphics = new Dictionary<String, SpriteSheet>();
		public String GraphicState = "standing";
		public double Frame = 0;

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
		public Direction Facing = Direction.South;
		/// <summary>
		/// Movement Speed
		/// </summary>
		public float Speed = 3.3f;


		/// <summary>
		/// Entity AI Mode
		/// </summary>
		public AIMode Mode = AIMode.Wait;
		GameScreen gameScreen;


		public virtual void Initialize(GameScreen gameScreen, int x, int y, Rectangle Collision)
		{
			Position.X = x;
			Position.Y = y;
			GlobalReference.setTilePosition(Position, TilePosition);
			this.gameScreen = gameScreen;
			this.Collision = Collision;
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

		#region Update
		public virtual void Update(GameTime gameTime)
		{
			UpdateAI();
			UpdateCollisions();
			UpdatePosition(gameTime);
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
			// Calculate Movement Direction and Facing
			if (Heading.Y < 0 && Heading.X == 0)
			{
				Facing = MovementDirection = Direction.North;
			}
			else if (Heading.Y < 0 && Heading.X > 0)
			{
				MovementDirection = Direction.NorthEast;
				if (Facing != Direction.North)
					Facing = Direction.East;
			}
			else if (Heading.Y == 0 && Heading.X > 0)
			{
				Facing = MovementDirection = Direction.East;
			}
			else if (Heading.Y > 0 && Heading.X > 0)
			{
				MovementDirection = Direction.SouthEast;
				if (Facing != Direction.East)
					Facing = Direction.South;
			}
			else if (Heading.Y > 0 && Heading.X == 0)
			{
				Facing = MovementDirection = Direction.South;
			}
			else if (Heading.Y > 0 && Heading.X < 0)
			{
				MovementDirection = Direction.SouthWest;
				if (Facing != Direction.South)
					Facing = Direction.West;
			}
			else if (Heading.Y == 0 && Heading.X < 0)
			{
				Facing = MovementDirection = Direction.West;
			}
			else if (Heading.Y < 0 && Heading.X < 0)
			{
				MovementDirection = Direction.NorthWest;
				if (Facing != Direction.West)
					Facing = Direction.North;
			}
			else
			{
				MovementDirection = Direction.Nowhere;
			}
			if (MovementDirection != Direction.Nowhere)
			{
				// Update Graphic
				if (GraphicState != "walking")
				{
					GraphicState = "walking";
					Frame = 0;
				}

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
				// Update Graphic
				if(GraphicState != "standing")
				{
					GraphicState = "standing";
					Frame = 0;
				}

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
		/// Updates the heading based on the collisions. God damn, this code looks ugly.
		/// </summary>
		void UpdateCollisions()
		{
			Vector2 topLeft = Position; // + Heading; // Also the position Vector
			Vector2 topRight = new Vector2(topLeft.X + (Collision.Width - 1), topLeft.Y);
			Vector2 bottomLeft = new Vector2(topLeft.X, topLeft.Y + (Collision.Height - 1));
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
		void UpdatePosition(GameTime gameTime)
		{
			// Update Position
			Position = Position + Heading;

			// Update Tile Position
			TilePosition.X = (float)Math.Floor(Position.X / 16);
			TilePosition.Y = (float)Math.Floor(Position.Y / 16);

			// Update Center
			Center.X = Position.X + Collision.Width / 2;
			Center.Y = Position.Y + Collision.Height / 2;

			if (GraphicState == "walking" && MovementDirection != Direction.Nowhere)
				Frame += Heading.Length() / 16;
			else
				Frame += 0.03;

			// Zero Heading
			Heading *= 0;

			// Update camera if player
			if (Mode == AIMode.DirectControl)
				Game.game.Camera = Matrix.CreateTranslation(
					-MathHelper.Clamp(Center.X - GlobalReference.ScreenWidth / 2, 0, gameScreen.mapManager.Current.Width * GlobalReference.TileSize - GlobalReference.ScreenWidth),
					-MathHelper.Clamp(Center.Y - GlobalReference.ScreenHeight / 2, 0, gameScreen.mapManager.Current.Height * GlobalReference.TileSize - GlobalReference.ScreenHeight), 0);

		}
		#endregion

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (Graphics.TryGetValue(GraphicState, out CurrentGraphic))
			{
				CurrentGraphic.Draw(spriteBatch, SpritePosition(), (int)Frame, DirectionFrame());
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

		private int DirectionFrame()
		{
			switch (Facing)
			{
				case Direction.North:
					return 0;
				case Direction.East:
					return 1;
				case Direction.South:
					return 2;
				case Direction.West:
					return 3;
				default:
					return 2;
			}
		}
		/// <summary>
		/// Gets the postion of where the sprite should be drawn (since position is determined by the collision box
		/// </summary>
		/// <returns> The Rectangle representing the sprite to be drawn </returns>
		Rectangle SpritePosition()
		{
			return new Rectangle((int)Position.X - Collision.X, (int)Position.Y - Collision.Y, CurrentGraphic.ElementWidth, CurrentGraphic.ElementHeight);
		}
	}
}
