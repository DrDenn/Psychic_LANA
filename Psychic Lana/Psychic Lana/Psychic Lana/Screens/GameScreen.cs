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
		public Entity Player, Player2;

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
			Player.Initialize(this, GlobalReference.startX, GlobalReference.startY, new Rectangle(2, 0, 12, 12));
			Player.AddSpriteSheet("standing", Game.game.Content.Load<Texture2D>(@"Graphics/Entity/player/standing"), 16, 27);
			Player.AddSpriteSheet("walking", Game.game.Content.Load<Texture2D>(@"Graphics/Entity/player/walking"), 16, 27);
			Player.Mode = AIMode.DirectControl;
			testMap.Entities.Add(Player);

			// Create Entity
			Player2 = new Entity();
			Player2.Initialize(this, GlobalReference.startX + 100, GlobalReference.startY + 100, new Rectangle(0, 0, 12, 12));
			Player2.AddSpriteSheet("standing", Game.game.Content.Load<Texture2D>(@"Graphics/Entity/player/testStanding"), 12, 23);
			Player2.AddSpriteSheet("walking", Game.game.Content.Load<Texture2D>(@"Graphics/Entity/player/testWalking"), 12, 23);
			Player2.AddSpriteSheet("specialAttack", Game.game.Content.Load<Texture2D>(@"Graphics/Entity/player/testSpecialAttack"), 12, 23);
			// Attack
			Player2.CreatureAttack = new GameObjects.Attack();
			Player2.CreatureAttack.Initialize(this, 0, 0, new Rectangle(0, 0, 16, 16));
			Player2.CreatureAttack.AddSpriteSheet("attack", Game.game.Content.Load<Texture2D>(@"Graphics/Entity/testAttackEffect"), 16, 16);
			
			Player2.Mode = AIMode.Wait;
			testMap.Entities.Add(Player2);

			// Set players as each other's target
			Player.Target = Player2;
			Player2.Target = Player;

			base.Initialize();
		}
		public override void LoadContent()
		{
			base.LoadContent();
		}
		public override void Update(GameTime gametime)
		{
			if (Input.Pressed(Controls.Console))
			{
				if (Player.Mode == AIMode.DirectControl)
				{
					Player2.Mode = AIMode.DirectControl;
					Player.Mode = AIMode.Wait;
				}
				else
				{
					Player.Mode = AIMode.DirectControl;
					Player2.Mode = AIMode.Wait;
				}
			}
			if (Input.Pressed(Controls.Cancel))
			{
				if (Player.Mode == AIMode.Wait)
					Player.Mode = AIMode.Path;
				else if (Player.Mode == AIMode.Path)
					Player.Mode = AIMode.Wait;

				if (Player2.Mode == AIMode.Wait)
					Player2.Mode = AIMode.Path;
				else if (Player2.Mode == AIMode.Path)
					Player2.Mode = AIMode.Wait;
			}

			mapManager.Update(gametime);
		}
		public override void Draw(SpriteBatch spriteBatch)
		{

			mapManager.Draw(spriteBatch);
			if (GlobalReference.debug)
			{
				StringBuilder debugLine = new StringBuilder();
				debugLine.Append("Play 1: " + Player.Mode + " "); // AI Mode
				debugLine.Append("Play 2: " + Player2.Mode + " "); // AI Mode
				//debugLine.Append("Posi:(" + Player.Position.X + ", " + Player.Position.Y + ") "); // Position 
				//debugLine.Append("Tile:(" + Player.TilePosition.X + ", " + Player.TilePosition.Y + ") "); // Tile Position
				//debugLine.Append("Cent:(" + Player.Center.X + ", " + Player.Center.Y + ") "); // Center
				//debugLine.Append("Exc:(" + Player.Excess.X + ", " + Player.Excess.Y + ") "); // Excess Heading
				//debugLine.Append("\nFace: " + Player.Facing + " "); // Facing 
				//debugLine.Append("Anim: " + Player.GraphicState + " "); // Graphic state
				//debugLine.Append("Move: " + Player.MovementDirection + " "); // Direction
				//debugLine.Append("TlSz: (" + Player.CollisionTiles.X + ", " + Player.CollisionTiles.Y + ") "); // Collision box tile size


				spriteBatch.DrawString(GlobalReference.default08, debugLine, GlobalReference.getScreenTopLeft() + new Vector2(5, 5), Color.Black);
				spriteBatch.DrawString(GlobalReference.default08, debugLine, GlobalReference.getScreenTopLeft() + new Vector2(4, 4), Color.White);
			}
		}
	}
}
