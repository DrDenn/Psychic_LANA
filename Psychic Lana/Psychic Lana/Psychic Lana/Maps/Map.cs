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
using Psychic_Lana.Entities;
using System.IO;

namespace Psychic_Lana.Maps
{
	/// <summary>
	/// Class representing a map
	/// </summary>
	public class Map
	{
		// Transition Information
		public TransIn TransitionIn = TransIn.Instant;
		public TransOut TransitionOut = TransOut.Instant;
		public bool ControlTransIn = false;
		public bool ControlTransOut = false;

		// Map information
		Tileset tileSet;
		List<int[,]> layers;
		bool[,] passability;
		public int Width;
		public int Height;

		//Entity List
		public List<Entity> Entities = new List<Entity>();

		public Map()
		{
			tileSet = new Tileset();
			layers = new List<int[,]>();
		}
		public void Initialize(StreamReader tileSetReader, StreamReader sr)
		{
			String[] line;
			//Tileset
			tileSet.Initialize(tileSetReader);

			//Map
			line = sr.ReadLine().Split(GlobalReference.Separators, StringSplitOptions.RemoveEmptyEntries);
			Width = Convert.ToInt32(line[0]);
			Height = Convert.ToInt32(line[1]);
			int layerCount = Convert.ToInt32(line[2]);
			int eventCount = Convert.ToInt32(line[3]);
			passability = new bool[Width, Height];


			for (int k = 0; k < layerCount; k++)
			{
				layers.Add(new int[Width, Height]);
				for (int j = 0; j < Height; j++)
				{
					for (int i = 0; i < Width; i++)
					{
						// Set next tile
						int next = sr.Read();
						while (next < 'a' || next > 'z') // Skip non tile data
							next = sr.Read();
						next -= 'a';
						layers.ElementAt(k)[i, j] = next;

						// Set Passability information for this tile
						PassType passType = tileSet.PassTypes[next];
						if (passType == PassType.LowerBlock || passType == PassType.UpperBlock)
							passability[i, j] = false;
						else if (passType == PassType.LowerPass)
							passability[i, j] = true;
					}
				}
			}

			// Populate events later
			// LOL   LOLOLO LOL
			// LOL   LO  LO LOL
			// LOLOL LOLOLO LOLOL
		}
		public void LoadContent()
		{

		}
		public void Update(GameTime gameTime)
		{
			tileSet.Update(gameTime);
			for (int i = 0; i < Entities.Count; i++)
			{
				Entities.ElementAt(i).Update(gameTime);
			}
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			for (int k = 0; k < layers.Count; k++)
			{
				int[,] layer = layers.ElementAt(k);
				for (int j = 0; j < Height; j++)
				{
					for (int i = 0; i < Width; i++)
					{
						tileSet.Draw(spriteBatch, layer[i, j], i, j);
					}
				}
			}
			for (int i = 0; i < Entities.Count; i++)
			{
				GlobalReference.DrawPath(spriteBatch, Entities.ElementAt(i).Path);
			}

			Entities.Sort
				(
				delegate(Entity first, Entity second)
				{
					return (int)(first.Position.Y + first.Collision.Height) - (int)(second.Position.Y + second.Collision.Height);
				}
				);
			for (int i = 0; i < Entities.Count; i++)
			{
				Entities.ElementAt(i).Draw(spriteBatch);
			}
		}
		/// <summary>
		/// Given a range, return true if there is any blocked tile
		/// </summary>
		/// <param name="start"> Vector to the top left </param>
		/// <param name="end"> Vector to the Bottom Right </param>
		/// <returns> True if a collision is detected, false otherwise </returns>
		public bool CheckCollisions(Vector2 start, Vector2 end)
		{
			int startX = (int)start.X / 16;
			int startY = (int)start.Y / 16;
			int endX = (int)end.X / 16;
			int endY = (int)end.Y / 16;
			for (int j = startY; j <= endY; j++)
				for (int i = startX; i <= endX; i++)
					if (i < 0 || i >= Width || j < 0 || j >= Height || !passability[i, j])
						return true;
			return false;
		}
		/// <summary>
		/// Given a point, return true if the tile is blocked
		/// </summary>
		/// <param name="position"> Vector representing the position </param>
		/// <returns> True if a collision is detected, false otherwise </returns>
		public bool CheckCollisions(Vector2 position)
		{
			int x = (int)position.X / 16;
			int y = (int)position.Y / 16;
			if (x < 0 || x >= Width || y < 0 || y > Height || !passability[x, y])
				return true;
			return false;
		}
		/// <summary>
		/// Given a point, number of tiles wide, and number of tiles tall, checks if a space has room. 
		/// returns 0,0 if unable to move, otherwise returns a vector towards its center.
		/// </summary>
		/// <param name="position"> x,y position to check from </param>
		/// <param name="width"> number of tiles wide </param>
		/// <param name="height"> number of tiles tall </param>
		/// <returns></returns>
		public Vector2 CheckForSpace(Vector2 position, int width, int height)
		{
			// If the starting position is blocked, invalid
			if (CheckCollisions(position))
				return new Vector2(0, 0);

			// Set variables to each end of the starting tile (indicated by where position lies)
			float north = (float)Math.Floor(position.Y / GlobalReference.TileSize) * GlobalReference.TileSize;
			float south = north + GlobalReference.TileSize;
			float west = (float)Math.Floor(position.X / GlobalReference.TileSize) * GlobalReference.TileSize;
			float east = west + GlobalReference.TileSize;

			int horizontal = 0;
			for (Vector2 check = new Vector2(position.X - GlobalReference.TileSize, position.Y); !CheckCollisions(check) && horizontal < width; check.X -= GlobalReference.TileSize)
			{
				horizontal++;
				west -= GlobalReference.TileSize;
			}
			for (Vector2 check = new Vector2(position.X + GlobalReference.TileSize, position.Y); !CheckCollisions(check) && horizontal < width; check.X += GlobalReference.TileSize)
			{
				horizontal++;
				east += GlobalReference.TileSize;
			}

			int vertical = 0;
			for (Vector2 check = new Vector2(position.X, position.Y - GlobalReference.TileSize); !CheckCollisions(check) && vertical < height; check.Y -= GlobalReference.TileSize)
			{
				vertical++;
				north -= GlobalReference.TileSize;
			}
			for (Vector2 check = new Vector2(position.X, position.Y + GlobalReference.TileSize); !CheckCollisions(check) && vertical < height; check.Y += GlobalReference.TileSize)
			{
				vertical++;
				south += GlobalReference.TileSize;
			}

			// If space has been found, calculate the distance to the center of all of them
			if (horizontal == width && vertical == height)
			{
				Vector2 center = new Vector2((float)Math.Floor((west + east) / 2), (float)Math.Floor((north + south) / 2));
				center = center - position;
				center.X /= center.X < 0 ? -center.X : center.X;
				center.Y /= center.Y < 0 ? -center.Y : center.Y;
				return center;
			}
			return new Vector2(0, 0);
		}
		public Boolean ValidPath(int sX, int sY, int tX, int tY)
		{
			if (tX < 0 || tX >= Width || tY < 0 || tY >= Height || !passability[tX, tY])
				return false;
			if (sX == tX || sY == tY)
				return true;
			if (sX - tX >= 0)
				if (!passability[sX - 1, sY])
					return false;
			if (sX - tX < 0)
				if (!passability[sX + 1, sY])
					return false;
			if (sY - tY >= 0)
				if (!passability[sX, sY - 1])
					return false;
			if (sY - tY < 0)
				if (!passability[sX, sY + 1])
					return false;
			return true;
		}

		public List<Vector2> TilePath(Vector2 start, Vector2 end)
		{
			// Do not calculate path if target postion is unreachable
			if (CheckCollisions(end))
				return null;

			start = GlobalReference.GetTilePosition(start);
			end = GlobalReference.GetTilePosition(end);
			// Initialize Structures
			double[,] gScore = new double[Width, Height];
			double[,] fScore = new double[Width, Height];
			bool[,] visited = new bool[Width, Height];
			List<Vector2> openset = new List<Vector2>();
			Vector2[,] cameFrom = new Vector2[Width, Height];
			for (int j = 0; j < Height; j++)
				for (int i = 0; i < Width; i++)
					cameFrom[i, j] = new Vector2(-1,-1);

			// Add starting node to the list, and calculate the score
			openset.Add(start);
			gScore[(int)start.X, (int)start.Y] = 0;
			fScore[(int)start.X, (int)start.Y] = gScore[(int)start.X, (int)start.Y] + GlobalReference.VectorDistance(start, end);

			// A* loop (Sort openset each time)
			while (openset.Count != 0)
			{
				openset.Sort(delegate(Vector2 first, Vector2 second)
				{
					return fScore[(int)first.X, (int)first.Y].CompareTo(fScore[(int)second.X, (int)second.Y]);
				});
				Vector2 current = openset.ElementAt(0);
				if (current == end)
				{
					return ReconstructTilePath(cameFrom, current);
				}

				openset.RemoveAt(0);
				visited[(int)current.X, (int)current.Y] = true;

				// Try surrounding neihgbors
				for (int j = (int)current.Y - 1; j <= (int)current.Y + 1; j++)
				{
					for (int i = (int)current.X - 1; i <= (int)current.X + 1; i++)
					{
						// Skip current position and invalid paths
						if ((i == (int)current.X && j == (int)current.Y) || !ValidPath((int)current.X, (int)current.Y, i, j))
							continue;
						Vector2 neighbor = new Vector2(i, j);
						double tentativeGScore = gScore[(int)current.X, (int)current.Y] + GlobalReference.VectorDistance(current, neighbor);
						// Skip if this is not a better path
						if (visited[(int)neighbor.X, (int)neighbor.Y])
							if (tentativeGScore >= gScore[(int)neighbor.X, (int)neighbor.Y])
								continue;

						// Check if this exists in openset
						bool contains = false;
						for (int k = 0; k < openset.Count(); k++)
						{
							if ((int)openset.ElementAt(k).X == i && (int)openset.ElementAt(k).Y == j)
							{
								contains = true;
								break;
							}
						}
						// If this is a better path or not yet in openset, evaluate
						if (!contains || tentativeGScore < gScore[(int)neighbor.X, (int)neighbor.Y])
						{
							cameFrom[(int)neighbor.X, (int)neighbor.Y] = current;
							gScore[(int)neighbor.X, (int)neighbor.Y] = tentativeGScore;
							fScore[(int)neighbor.X, (int)neighbor.Y] = tentativeGScore + GlobalReference.VectorDistance(neighbor, end);
							if (!contains)
								openset.Add(neighbor);
						}
					}
				}
			}

			return null;
		}
		List<Vector2> ReconstructTilePath(Vector2[,] cameFrom, Vector2 currentNode)
		{
			// If it's not the end, add the node
			if (!cameFrom[(int)currentNode.X, (int)currentNode.Y].Equals(new Vector2(-1,-1)))
			{
				List<Vector2> p = ReconstructTilePath(cameFrom, cameFrom[(int)currentNode.X, (int)currentNode.Y]);
				p.Add(currentNode);
				return p;
			}
			// Don't add start node (improves movement)
			List<Vector2> r = new List<Vector2>();
			return r;
		}
	}
}
