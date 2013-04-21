/* Psychic Lana
 * 
 * 
 * Authors:
 * Brandon Scott
 * TJ Horwath
 */
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
using Psychic_Lana.Screens;
using Psychic_Lana.Overhead;

namespace Psychic_Lana
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		// Singleton Objects
		/// <summary> Reference to the instance of the game </summary>
		public static Game game;
		/// <summary> Reference to the instance of the screenManager </summary>
		public static ScreenManager screenManager;

        //Flag if in fullscreen
        private int fullscreen_flag;

		// Game Instance Objects
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		// Window Dimensions
		/// <summary> The Game Window's width </summary>
		//public static int WindowWidth = 320;
		/// <summary> The Game Window's height </summary>
		//public static int WindowHeight = 240;

		/// <summary>
		/// Representents the transaltion of the viewport
		/// </summary>
		public Matrix Camera = Matrix.CreateTranslation(0, 0, 0);

		// Variables
		/// <summary> Debug flag </summary>
		public bool Debug = true; 

		public Game()
		{
			graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GlobalReference.ScreenWidth;
            graphics.PreferredBackBufferHeight = GlobalReference.ScreenHeight;
			Content.RootDirectory = "Content";

			game = this;
			screenManager = new ScreenManager();
		}

		protected override void Initialize()
		{
			this.Window.Title = "Psychic Lana";
			
			this.Window.AllowUserResizing = false;

            //Start in windowed mode
            fullscreen_flag = 0;

			// Give Screen Manager the screens and Transistion to the first screen
			screenManager.AddScreen("Introduction", new IntroScreen());
			screenManager.AddScreen("Title Screen", new TitleScreen());
			screenManager.AddScreen("Game Screen", new GameScreen());
			if(GlobalReference.debug)
				screenManager.Transition("Game Screen");
			else
				screenManager.Transition("Introduction");
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			GlobalReference.LoadContent(Content);
		}

		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (Input.Pressed(Controls.Abort)) // Debug Exit
				this.Exit();
            if (Input.Pressed(Controls.Fullscreen))
            {
				graphics.ToggleFullScreen();

				//Double screen width and height if going into fullscreen
				if (fullscreen_flag == 0)
				{
					fullscreen_flag = 1;
					GlobalReference.ScreenHeight = GlobalReference.ScreenHeight * 2;
					GlobalReference.ScreenWidth = GlobalReference.ScreenWidth * 2;
				}

				//Half screen width and height if going into windowed mode
				else if (fullscreen_flag == 1)
				{
					fullscreen_flag = 0;
					GlobalReference.ScreenHeight = GlobalReference.ScreenHeight / 2;
					GlobalReference.ScreenWidth = GlobalReference.ScreenWidth / 2;
				}
            }
				
			Input.Update(gameTime);            // Update input
			screenManager.Update(gameTime);    // Update Screen Manager (transitions and current screen)
			base.Update(gameTime);             // Base update (probably does something?)
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue); // Draw base which should probably be replaced
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Camera);
			screenManager.Draw(spriteBatch);            // Draw Screen Manager (transitions and current screen)
			spriteBatch.End();
			base.Draw(gameTime);                        // Base Draw (probably does something?)
		}
	}
}
