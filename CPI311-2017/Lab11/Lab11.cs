using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

using CPI311.GameEngine;

namespace Lab11
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Lab11 : Game
    {
        // ******************************* Inner Class **********************************
        public class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }
        // ******************************************************************************

        Dictionary<String, Scene> scenes;
        Scene currentScene;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Texture2D texture;
        Color background = Color.Blue;
        List<GUIElement> guiElements;

        public Lab11()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);
            ScreenManager.Setup(1280, 720);

            scenes = new Dictionary<string, Scene>();
            guiElements = new List<GUIElement>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            texture = Content.Load<Texture2D>("Square");

            GUIGroup group = new GUIGroup();

            Button exitButton = new Button();
            exitButton.Texture = texture;
            exitButton.Text = "   Exit";
            exitButton.Bounds = new Rectangle(50, 50, 300, 20);
            exitButton.Action += ExitGame;
            group.Children.Add(exitButton);

            CheckBox optionBox = new CheckBox();
            optionBox.Text = " Full Screen";
            optionBox.Texture = texture;
            optionBox.Box = texture;
            optionBox.Bounds = new Rectangle(50, 75, 300, 20);
            optionBox.Action += MakeFullScreen;
            group.Children.Add(optionBox);

            Button changeSceneBox = new Button();
            changeSceneBox.Text = "   Change Scene";
            changeSceneBox.Texture = texture;
            //changeSceneBox.Box = texture;
            changeSceneBox.Bounds = new Rectangle(50, 100, 300, 20);
            changeSceneBox.Action += ChangeScene;
            group.Children.Add(changeSceneBox);

            guiElements.Add(group);

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            currentScene = scenes["Menu"];

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();
            currentScene.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            currentScene.Draw();
            base.Draw(gameTime);
        }

        void ExitGame(GUIElement element)
        {
            background = (background == Color.White ? Color.Blue : Color.White);
        }

        void MainMenuUpdate()
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }
        void MainMenuDraw()
        {
            spriteBatch.Begin();
            foreach (GUIElement element in guiElements)
                element.Draw(spriteBatch, font);
            spriteBatch.End();
        }
        void PlayUpdate()
        {
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
        }

        void PlayDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Play Mode! Press \"Esc\" to go back",
                Vector2.Zero, Color.Black);
            spriteBatch.End();
        }

        void MakeFullScreen(GUIElement element)
        {
            ScreenManager.Setup(!ScreenManager.IsFullScreen,
                ScreenManager.Width, ScreenManager.Height);
        }

        void ChangeScene(GUIElement element)
        {
            currentScene = scenes["Play"];
        }
    }
}