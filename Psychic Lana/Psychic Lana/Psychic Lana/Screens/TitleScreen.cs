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
using Drawing;
using Psychic_Lana.Overhead;

namespace Psychic_Lana.Screens
{
	/// <summary>
	/// Title Screen that leads to the main game
	/// </summary>
	class TitleScreen : Screen
	{
		string title = "Psychic LANA";
		public TitleScreen()
			: base()
		{
			TransitionIn = TransIn.Fade;
			TransitionOut = TransOut.Fade;
			ControlTransIn = true;
			ControlTransOut = false;
		}
		public override void Initialize()
		{
			base.Initialize();
		}
		public override void LoadContent()
		{
			base.LoadContent();
		}
		public override void Update(GameTime gametime)
		{
			if (Input.Pressed(Controls.AnyKey))
				Game.screenManager.Transition("Game Screen");
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			GlobalReference.DrawFilledScreen(spriteBatch, Color.DarkRed);
			Vector2 measurement = GlobalReference.default14.MeasureString(title);
			Vector2 textPosition = new Vector2(GlobalReference.ScreenWidth / 2 - measurement.X / 2, GlobalReference.ScreenHeight / 4 - measurement.Y / 2);
			spriteBatch.DrawString(GlobalReference.default14, title, textPosition, Color.White);

			measurement = GlobalReference.default10.MeasureString("Press Any Key");
			textPosition = new Vector2(GlobalReference.ScreenWidth / 2 - measurement.X / 2, GlobalReference.ScreenHeight / 2 - measurement.Y / 2);
			spriteBatch.DrawString(GlobalReference.default10, "Press Any Key", textPosition, Color.White);
		}
	}
}
