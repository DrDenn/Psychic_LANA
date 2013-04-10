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
			Width          = Convert.ToInt32(line[0]);
			Height         = Convert.ToInt32(line[1]);
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
					if (i<0 || i>=Width || j<0 || j>=Height || !passability[i, j])
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
	}
}
