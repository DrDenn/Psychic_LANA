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

namespace Psychic_Lana.Overhead
{
	/// <summary>
	/// Static class containing elements and functions to be used globally
	/// </summary>
	public static class GlobalReference
	{
		// Debug
		/// <summary>
		/// Displays debug information if true
		/// </summary>
		public static bool debug = true;
		/// <summary>
		/// Displays collision box if true
		/// </summary>
		public static bool debugGraphics = false;

		// Constants
		/// <summary>
		/// Used for any calculation based on the tile size
		/// </summary>
		public static int TileSize = 16;
		/// <summary>
		/// Used for scanning through text, splits input by " "
		/// </summary>
		public static String[] Separators = { " " };
		/// <summary>
		/// Player starting postion
		/// </summary>
		public static int startX = 64, startY = 64;
		/// <summary>
		/// Default screen size
		/// </summary>
		public static int ScreenWidth = 320, ScreenHeight = 240;

		// Graphics
		/// <summary> 
		/// Single white pixel, can be recolored and streched 
		/// </summary>
		public static Texture2D whitePixel;

		// Fonts
		/// <summary> 
		/// Default font, size 08 
		/// </summary>
		public static SpriteFont default08;
		/// <summary> 
		/// Default font, size 10 
		/// </summary>
		public static SpriteFont default10;
		/// <summary> 
		/// Default font, size 14
		/// </summary>
		public static SpriteFont default14;

		public static void LoadContent(ContentManager content)
		{
			whitePixel = content.Load<Texture2D>(@"Graphics/Hud/whitePixel");
			default08 = content.Load<SpriteFont>(@"Fonts/default08");
			default10 = content.Load<SpriteFont>(@"Fonts/default10");
			default14 = content.Load<SpriteFont>(@"Fonts/default14");
		}

		#region DrawFilledScreen
		/// <summary>
		/// Draws over the current screen with black
		/// </summary>
		/// <param name="spriteBatch">Spritebatch being drawn to</param>
		public static void DrawFilledScreen(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(whitePixel, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.Black);
		}
		/// <summary>
		/// Draws over the current screen with a color
		/// </summary>
		/// <param name="spriteBatch">Spritebatch being drawn to</param>
		/// <param name="color">Color being drawn</param>
		public static void DrawFilledScreen(SpriteBatch spriteBatch, Color color)
		{
			spriteBatch.Draw(whitePixel, new Rectangle(0, 0, ScreenWidth, ScreenHeight), color);
		}
		/// <summary>
		/// Draws over the current screen with a color by a specified alpha value
		/// </summary>
		/// <param name="spriteBatch">Spritebatch being drawn to</param>
		/// <param name="color">Color being drawn</param>
		/// <param name="alpha">Alpha Value (0.0 .. 1.0)</param>
		public static void DrawFilledScreen(SpriteBatch spriteBatch, Color color, float alpha)
		{
			spriteBatch.Draw(whitePixel, new Rectangle(0, 0, ScreenWidth, ScreenHeight), color * alpha);
		}
		#endregion

		/// <summary>
		/// Draw a filled rectangle at the given area
		/// </summary>
		/// <param name="spriteBatch">Spritebatch being drawn to</param>
		/// <param name="rectangle"> Rectangle representing where it should be drawn</param>
		/// <param name="color">Color being drawn</param>
		/// <param name="alpha">Alpha Value (0.0 .. 1.0)</param>
		public static void DrawFilledRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, float alpha)
		{
			spriteBatch.Draw(whitePixel, rectangle, color * alpha);
		}

		/// <summary>
		/// Gets the tile position of a given position
		/// </summary>
		/// <param name="position"> input vector </param>
		/// <returns> output vector </returns>
		public static Vector2 GetTilePosition(Vector2 position) { return new Vector2(((int)position.X) / TileSize, ((int)position.Y) / TileSize); }
		/// <summary>
		/// Sets a vector to a tile position from another vector
		/// </summary>
		/// <param name="position"> input vector </param>
		/// <param name="output"> output vector </param>
		public static void setTilePosition(Vector2 position, Vector2 output)
		{
			output.X = ((int)position.X) / TileSize;
			output.Y = ((int)position.Y) / TileSize;
		}
		/// <summary>
		/// Get's a vector that points to (0,0) on the screen (regardless of map position)
		/// </summary>
		/// <returns> Position Vector </returns>
		public static Vector2 getScreenTopLeft()
		{
			return new Vector2(-Game.game.Camera.Translation.X, -Game.game.Camera.Translation.Y);
		}

		public static double VectorDistance(Vector2 here, Vector2 there)
		{
			return Math.Sqrt(((here.X - there.X) * (here.X - there.X)) + ((here.Y - there.Y) * (here.Y - there.Y)));
		}
	}
}
