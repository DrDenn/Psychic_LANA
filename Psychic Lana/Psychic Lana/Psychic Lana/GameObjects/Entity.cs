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

namespace Psychic_Lana.Entities
{
	/// <summary>
	/// Enumeration representing various AI control Methods 
	/// </summary>
	public enum AIMode { DirectControl, Wait, Seek, Path };
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
		/// Number of tiles Collision box takes up in x and y direction
		/// </summary>
		public Vector2 CollisionTiles;
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
		/// Direction the entity is facing (four directions)
		/// </summary>
		public Direction Facing = Direction.South;
		/// <summary>
		/// Movement Speed
		/// </summary>
		public float Speed = 2.2f;


		/// <summary>
		/// Entity AI Mode
		/// </summary>
		public AIMode Mode = AIMode.Wait;
		public Entity Target;
		public List<Vector2> Path = null;


		//GameScreen gameScreen;
		/// <summary>
		/// Reference to the current map
		/// </summary>
		Map map;


		public virtual void Initialize(GameScreen gameScreen, int x, int y, Rectangle collisionbox)
		{
			Position.X = x;
			Position.Y = y;
			GlobalReference.setTilePosition(Position, TilePosition);
			map = gameScreen.mapManager.Current;
			Collision = collisionbox;
			CollisionTiles.X = (float)Math.Floor((double)((Collision.Width - 1) / GlobalReference.TileSize));
			CollisionTiles.Y = (float)Math.Floor((double)((Collision.Height - 1) / GlobalReference.TileSize));
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

		// Handles AI update, collision check, and position update
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
					Path = null;
					break;
				case AIMode.Seek:
					if(Target != null)
						UpdateSeek(Target.Center);
					break;
				case AIMode.Path:
					UpdatePath();
					break;
				default:
					break;
			}

			// If x and y are BOTH non-zero, normalize Heading
			if (Heading.X != 0 && Heading.Y != 0)
				Heading.Normalize();
			Heading = Heading * Speed;

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
			
		}

		/// <summary>
		/// Updates Heading based on the target
		/// </summary>
		/// <param name="target"></param>
		void UpdateSeek(Vector2 target)
		{
			double distX = Center.X - target.X;
			double distY = Center.Y - target.Y;
			if (distY > Collision.Height / 2)
				Heading.Y -= 1;
			if (distX < -Collision.Width / 2)
				Heading.X += 1;
			if (distY < -Collision.Height / 2)
				Heading.Y += 1;
			if (distX > Collision.Width / 2)
				Heading.X -= 1;
		}
		void FollowPath(Vector2 next)
		{
			double distX = Position.X - next.X;
			double distY = Position.Y - next.Y;
			if (distY > 0)
				Heading.Y -= 1;
			if (distX < Collision.Height / 2)
				Heading.X += 1;
			if (distY < Collision.Height / 2)
				Heading.Y += 1;
			if (distX > 0)
				Heading.X -= 1;
		}

		void UpdatePath()
		{
			if (Path == null || Path.Count() == 0 || Path.ElementAt(Path.Count() - 1) != GlobalReference.GetTilePosition(Target.Center))
				Path = map.TilePath(Position, Target.Center);
			if (Path != null && Path.Count() > 0)
			{
				if (GlobalReference.GetTilePosition(Position) == Path.ElementAt(0))
					Path.RemoveAt(0);
				if (Path.Count() > 0)
					FollowPath(new Vector2(Path.ElementAt(0).X * GlobalReference.TileSize, Path.ElementAt(0).Y * GlobalReference.TileSize));
			}
			if (Path == null)
				UpdateSeek(Target.Center);
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
					if (map.CheckCollisions(topLeft + yHeading, topRight + yHeading))
					{
						Heading.Y = (float)Math.Floor(Position.Y / GlobalReference.TileSize) * GlobalReference.TileSize - Position.Y;
						// Cornering
						Vector2 spaceCheck = map.CheckForSpace(Center - new Vector2(0, Collision.Height / 2 + GlobalReference.TileSize), (int)CollisionTiles.X, 0);
						Heading.X = spaceCheck.Length() != 0? spaceCheck.X: Heading.X;
					}
					break;
				case Direction.NorthEast:
					if (map.CheckCollisions(topLeft + yHeading, topRight + yHeading))
					{
						Heading.Y = (float)Math.Floor(Position.Y / 16) * 16 - Position.Y;
					}
					if (map.CheckCollisions(topRight + xHeading, bottomRight + xHeading))
					{
						Heading.X = (float)Math.Floor((Position.X + Collision.Width - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.X + Collision.Width);
					}
					if (xHeading.X == Heading.X && yHeading.Y == Heading.Y && map.CheckCollisions(topRight + Heading))
					{
						Heading.X = (float)Math.Floor((Position.X + Collision.Width - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.X + Collision.Width);
					}
					break;
				case Direction.East:
					if (map.CheckCollisions(topRight + xHeading, bottomRight + xHeading))
					{
						Heading.X = (float)Math.Floor((Position.X + Collision.Width - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.X + Collision.Width);
						// Cornering
						Vector2 spaceCheck = map.CheckForSpace(Center + new Vector2(Collision.Width / 2 + GlobalReference.TileSize - 1, 0), 0, (int)CollisionTiles.Y);
						Heading.Y = spaceCheck.Length() != 0 ? spaceCheck.Y : Heading.Y;
					}
					break;
				case Direction.SouthEast:
					if (map.CheckCollisions(topRight + xHeading, bottomRight + xHeading))
					{
						Heading.X = (float)Math.Floor((Position.X + Collision.Width - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.X + Collision.Width);
					}
					if (map.CheckCollisions(bottomLeft + yHeading, bottomRight + yHeading))
					{
						Heading.Y = (float)Math.Floor((Position.Y + Collision.Height - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.Y + Collision.Height);
					}
					if (xHeading.X == Heading.X && yHeading.Y == Heading.Y && map.CheckCollisions(bottomRight + Heading))
					{
						Heading.Y = (float)Math.Floor((Position.Y + Collision.Height - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.Y + Collision.Height);
					}
					break;
				case Direction.South:
					if (map.CheckCollisions(bottomLeft + yHeading, bottomRight + yHeading))
					{
						Heading.Y = (float)Math.Floor((Position.Y + Collision.Height - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.Y + Collision.Height);
						// Cornering
						Vector2 spaceCheck = map.CheckForSpace(Center + new Vector2(0, Collision.Height / 2 + GlobalReference.TileSize - 1), (int)CollisionTiles.X, 0);
						Heading.X = spaceCheck.Length() != 0 ? spaceCheck.X : Heading.X;
					}
					break;
				case Direction.SouthWest:
					if (map.CheckCollisions(bottomLeft + yHeading, bottomRight + yHeading))
					{
						Heading.Y = (float)Math.Floor((Position.Y + Collision.Height - 1) / GlobalReference.TileSize + 1) * GlobalReference.TileSize - (Position.Y + Collision.Height);
					}
					if (map.CheckCollisions(topLeft + xHeading, bottomLeft + xHeading))
					{
						Heading.X = (float)Math.Floor(Position.X / GlobalReference.TileSize) * GlobalReference.TileSize - Position.X;
					}
					if (xHeading.X == Heading.X && yHeading.Y == Heading.Y && map.CheckCollisions(bottomLeft + Heading))
					{
						Heading.X = (float)Math.Floor(Position.X / GlobalReference.TileSize) * GlobalReference.TileSize - Position.X;
					}
					break;
				case Direction.West:
					if (map.CheckCollisions(topLeft + xHeading, bottomLeft + xHeading))
					{
						Heading.X = (float)Math.Floor(Position.X / GlobalReference.TileSize) * GlobalReference.TileSize - Position.X;
						// Cornering
						Vector2 spaceCheck = map.CheckForSpace(Center - new Vector2(Collision.Width / 2 + GlobalReference.TileSize, 0), 0, (int)CollisionTiles.Y);
						Heading.Y = spaceCheck.Length() != 0 ? spaceCheck.Y : Heading.Y;
					}
					break;
				case Direction.NorthWest:
					if (map.CheckCollisions(topLeft + xHeading, bottomLeft + xHeading))
					{
						Heading.X = (float)Math.Floor(Position.X / GlobalReference.TileSize) * GlobalReference.TileSize - Position.X;
					}
					if (map.CheckCollisions(topLeft + yHeading, topRight + yHeading))
					{
						Heading.Y = (float)Math.Floor(Position.Y / GlobalReference.TileSize) * GlobalReference.TileSize - Position.Y;
					}
					if (xHeading.X == Heading.X && yHeading.Y == Heading.Y && map.CheckCollisions(topLeft + Heading))
					{
						Heading.X = (float)Math.Floor(Position.X / GlobalReference.TileSize) * GlobalReference.TileSize - Position.X;
					}
					break;
				default:
					break;
			}
			for (int i = 0; i < map.Entities.Count(); i++)
			{
				if (map.Entities.ElementAt(i) != this)
				{
					Rectangle rect1 = new Rectangle((int)map.Entities.ElementAt(i).Position.X, (int)map.Entities.ElementAt(i).Position.Y, (int)map.Entities.ElementAt(i).Collision.Width, (int)map.Entities.ElementAt(i).Collision.Height);
					Vector2 projected = Position + Heading;
					Rectangle rect2 = new Rectangle((int)projected.X, (int)projected.Y, Collision.Width, Collision.Height);
					if (rect1.Intersects(rect2))
						Heading *= 0;
				}
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
					-MathHelper.Clamp(Center.X - GlobalReference.ScreenWidth / 2, 0, map.Width * GlobalReference.TileSize - GlobalReference.ScreenWidth),
					-MathHelper.Clamp(Center.Y - GlobalReference.ScreenHeight / 2, 0, map.Height * GlobalReference.TileSize - GlobalReference.ScreenHeight), 0);

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
			return new Rectangle((int)Position.X - Collision.X, (int)Position.Y + Collision.Y + Collision.Height - CurrentGraphic.ElementHeight, CurrentGraphic.ElementWidth, CurrentGraphic.ElementHeight);
		}
	}
}
