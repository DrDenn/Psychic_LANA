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
	/// Screen used for the pre-title intro
	/// </summary>
	class IntroScreen : Screen
	{

		double startIntro = -1.0;
		double timeIntro = -1.0;
		int introProgress = 0;
		String[] introText = { "AI for Game Programming presents",
							   "A Frederick Reginald the Third Game",
							   "By Brandon Scott and TJ Horwath" };

		public IntroScreen()
			: base()
		{
			TransitionIn = TransIn.Instant;
			TransitionOut = TransOut.Instant;
			ControlTransIn = false;
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
			timeIntro = gametime.TotalGameTime.Seconds + (double)gametime.TotalGameTime.Milliseconds / 1000;
			if (startIntro < 0) startIntro = timeIntro;
			if (timeIntro - startIntro >= 3)
			{
				startIntro += 3;
				introProgress++;
			}

			if (Input.Pressed(Controls.AnyKey)|| introProgress >= 3)
			{
				if (introProgress >= 2)
					Game.screenManager.Transition("Title Screen");
				else
				{
					startIntro = timeIntro;
					introProgress++;
				}
			}
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			GlobalReference.DrawFilledScreen(spriteBatch, Color.Black);
			if (introProgress < 3)
			{
				Vector2 measurement = GlobalReference.default10.MeasureString(introText[introProgress]);
				Vector2 textPosition = new Vector2(GlobalReference.ScreenWidth / 2 - measurement.X / 2, GlobalReference.ScreenHeight / 2 - measurement.Y / 2);
				spriteBatch.DrawString(GlobalReference.default10, introText[introProgress], textPosition, Color.White);

				int timeSlice = (int)((timeIntro - startIntro) % 3.0d);
				float fade = (float)(timeIntro - startIntro) % 1.0f;
				switch (timeSlice)
				{
					case 0:
						GlobalReference.DrawFilledScreen(spriteBatch, Color.Black, 1.0f - fade);
						break;
					case 1:
						break;
					case 2:
						GlobalReference.DrawFilledScreen(spriteBatch, Color.Black, fade);
						break;
				}
			}
		}
	}
}
