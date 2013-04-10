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
using System.IO;

namespace Psychic_Lana.Maps
{
	/// <summary>
	/// Passability type (lower or upper, passable or blocking)
	/// </summary>
	public enum PassType { LowerPass, LowerBlock, UpperPass, UpperBlock }
	/// <summary>
	/// Represents a TileSet, a set of tiles
	/// </summary>
	public class Tileset
	{
		public PassType[] PassTypes;
		public Texture2D TilesetSheet;

		public void Initialize(StreamReader sr)
		{
			TilesetSheet = Game.game.Content.Load<Texture2D>(@"Graphics/Map/testTileset/tileset");
			int tileCount = 64 * TilesetSheet.Height / TilesetSheet.Width;
			PassTypes = new PassType[tileCount];
			String line = sr.ReadLine();
			for (int i = 0; i < tileCount; i++)
			{
				PassTypes[i] = PassByNumber(Convert.ToInt32(line.ElementAt(i) - '0'));
			}
		}

		public void Update(GameTime gameTime)
		{
		}

		/// <summary>
		/// Draw Tile at a specific location
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch</param>
		/// <param name="tileID">The Tileset ID being drawn</param>
		/// <param name="x">X tile position</param>
		/// <param name="y">Y tile position</param>
		public void Draw(SpriteBatch spriteBatch, int tileID, int x, int y)
		{
			Rectangle destination = new Rectangle(x * GlobalReference.TileSize, y * GlobalReference.TileSize, GlobalReference.TileSize, GlobalReference.TileSize);
			spriteBatch.Draw(TilesetSheet, destination, Tile(tileID), Color.White);
		}
		/// <summary>
		/// Draw ONLY LOWER Tile at a specific location
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch</param>
		/// <param name="tileID">The Tileset ID being drawn</param>
		/// <param name="x">X tile position</param>
		/// <param name="y">Y tile position</param>
		public void DrawLower(SpriteBatch spriteBatch, int tileID, int x, int y)
		{
			Rectangle destination = new Rectangle(x * GlobalReference.TileSize, y * GlobalReference.TileSize, GlobalReference.TileSize, GlobalReference.TileSize);
			if(PassTypes[tileID] == PassType.LowerPass || PassTypes[tileID] == PassType.LowerBlock)
				spriteBatch.Draw(TilesetSheet, destination, Tile(tileID), Color.White);
		}
		/// <summary>
		/// Draw ONLY UPPER Tile at a specific location
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch</param>
		/// <param name="tileID">The Tileset ID being drawn</param>
		/// <param name="x">X tile position</param>
		/// <param name="y">Y tile position</param>
		public void DrawUpper(SpriteBatch spriteBatch, int tileID, int x, int y)
		{
			Rectangle destination = new Rectangle(x * GlobalReference.TileSize, y * GlobalReference.TileSize, GlobalReference.TileSize, GlobalReference.TileSize);
			if (PassTypes[tileID] == PassType.UpperPass || PassTypes[tileID] == PassType.UpperBlock)
				spriteBatch.Draw(TilesetSheet, destination, Tile(tileID), Color.White);
		}
		/// <summary>
		/// Gets a rectangle pointing to the specific tile by the index
		/// </summary>
		/// <param name="i"> Tile index</param>
		/// <returns> Tile rectangle </returns>
		public Rectangle Tile(int i)
		{
			return new Rectangle((i % 8) * GlobalReference.TileSize, (i / 8) * GlobalReference.TileSize, GlobalReference.TileSize, GlobalReference.TileSize);
		}

		private PassType PassByNumber(int input)
		{
			switch (input)
			{
				case 0:
					return PassType.LowerPass;
				case 1:
					return PassType.LowerBlock;
				case 2:
					return PassType.UpperPass;
				case 3:
					return PassType.UpperBlock;
				default:
					return PassType.UpperPass;
			}
		}
	}
}
