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
// Added
using Psychic_Lana.Overhead;
using Psychic_Lana.Maps;
using Psychic_Lana.Entities;
using System.IO;
using System.Text;

namespace Psychic_Lana.Screens
{
	/// <summary>
	/// Main Game processing screen
	/// </summary>
	public class GameScreen : Screen
	{
		public MapManager mapManager;
		public Entity Player;

		public GameScreen()
			: base()
		{
			TransitionIn = TransIn.Fade;
			TransitionOut = TransOut.Instant;
			ControlTransIn = true;
			ControlTransOut = false;

			mapManager = new MapManager();
		}
		public override void Initialize()
		{
			// Begin Map shit (make this a for loop later)
			Map testMap = new Map();
			testMap.Initialize(new StreamReader(@"Maps/MapFiles/Tilesets/testTileset.tsd"), new StreamReader(@"Maps/MapFiles/MapData/testMap.dat"));
			mapManager.AddMap(testMap);
			mapManager.Transition(0);

			// Create Player
			Player = new Entity();
			Player.Initialize(this, GlobalReference.startX, GlobalReference.startY, new Rectangle(2, 18, 12, 9));
			Player.AddSpriteSheet("standing", Game.game.Content.Load<Texture2D>(@"Graphics/Entity/player/standing"), 16, 27);
			Player.AddSpriteSheet("walking", Game.game.Content.Load<Texture2D>(@"Graphics/Entity/player/walking"), 16, 27);
			Player.Mode = AIMode.DirectControl;
			testMap.Entities.Add(Player);

			base.Initialize();
		}
		public override void LoadContent()
		{
			base.LoadContent();
		}
		public override void Update(GameTime gametime)
		{
			mapManager.Update(gametime);
		}
		public override void Draw(SpriteBatch spriteBatch)
		{

			mapManager.Draw(spriteBatch);
			if (GlobalReference.debug)
			{
				StringBuilder debugLine = new StringBuilder();
				debugLine.Append("Posi:(" + Player.Position.X + ", " + Player.Position.Y + ") "); // Position 
				debugLine.Append("Tile:(" + Player.TilePosition.X + ", " + Player.TilePosition.Y + ") "); // Tile Position
				debugLine.Append("Cent:(" + Player.Center.X + ", " + Player.Center.Y + ") "); // Center
				//debugLine.Append("Exc:(" + Player.Excess.X + ", " + Player.Excess.Y + ") "); // Excess Heading
				debugLine.Append("\nMove: " + Player.MovementDirection); // Direction

				spriteBatch.DrawString(GlobalReference.default08, debugLine, GlobalReference.getScreenTopLeft() + new Vector2(5, 5), Color.Black);
				spriteBatch.DrawString(GlobalReference.default08, debugLine, GlobalReference.getScreenTopLeft() + new Vector2(4, 4), Color.White);
			}
		}
	}
}
