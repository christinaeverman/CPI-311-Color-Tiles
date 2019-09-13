using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

using CPI311.GameEngine;

namespace FinalGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class FinalGame : Game
    {
        // ***************************** Scenes ***********************************
        public class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }

        Dictionary<String, Scene> scenes;
        Scene currentScene;
        // ************************************************************************

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model boxModel;
        Texture2D squareTexture;
        SpriteFont font;
        Sprite NormalSquare;
        Sprite BoxSquare;
        Sprite ColorSquare;
        Dictionary<String, GUIElement> guiElements;
        Color BackgroundColor = Color.CornflowerBlue;
        
        List<Block> blocks;
        Player player;

        Camera camera;
        Light light;
        Effect effect;

        Grid grid;

        float timer;
        float levelTimer;
        float levelChangeTimer;
        float levelTime;

        public FinalGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);
            ScreenManager.Setup(1280, 720);

            scenes = new Dictionary<String, Scene>();
            guiElements = new Dictionary<String, GUIElement>();

            timer = 0.0f;
            levelTimer = 0.0f;
            levelChangeTimer = 0.0f;

            boxModel = Content.Load<Model>("box2");
            squareTexture = Content.Load<Texture2D>("Square");

            NormalSquare = new Sprite(squareTexture);
            NormalSquare.Color = Color.White;
            ColorSquare = new Sprite(squareTexture);
            ColorSquare.Color = Color.Orange;
            BoxSquare = new Sprite(squareTexture);
            BoxSquare.Color = Color.Blue;

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * 30 + Vector3.Right * 15;
            
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

            effect = Content.Load<Effect>("SimpleShading");
            font = Content.Load<SpriteFont>("Font");

            // ************************* GUI Elements **********************************
            Button toInstructionsButton = new Button();
            toInstructionsButton.Texture = squareTexture;
            toInstructionsButton.Text = "  Play";
            toInstructionsButton.Bounds = new Rectangle(560, 300, 150, 50);
            toInstructionsButton.Action += Instructions;
            guiElements.Add("Instructions", toInstructionsButton);

            Button playButton = new Button();
            playButton.Texture = squareTexture;
            playButton.Text = "  Play";
            playButton.Bounds = new Rectangle(560, 300, 150, 50);
            playButton.Action += PlayLevel1;
            guiElements.Add("Play1", playButton);

            Button reset1Button = new Button();
            reset1Button.Texture = squareTexture;
            reset1Button.Text = "  Reset";
            reset1Button.Bounds = new Rectangle(50, 85, 150, 30);
            reset1Button.Action += ResetLevel1;
            guiElements.Add("Reset1", reset1Button);

            Button reset2Button = new Button();
            reset2Button.Texture = squareTexture;
            reset2Button.Text = "  Reset";
            reset2Button.Bounds = new Rectangle(50, 85, 150, 30);
            reset2Button.Action += ResetLevel2;
            guiElements.Add("Reset2", reset2Button);

            Button reset3Button = new Button();
            reset3Button.Texture = squareTexture;
            reset3Button.Text = "  Reset";
            reset3Button.Bounds = new Rectangle(50, 85, 150, 30);
            reset3Button.Action += ResetLevel3;
            guiElements.Add("Reset3", reset3Button);

            Button creditsButton = new Button();
            creditsButton.Texture = squareTexture;
            creditsButton.Text = "  Credits";
            creditsButton.Bounds = new Rectangle(560, 375, 150, 50);
            creditsButton.Action += Credits;
            guiElements.Add("Credits", creditsButton);

            Button menuButton = new Button();
            menuButton.Texture = squareTexture;
            menuButton.Text = "  Menu";
            menuButton.Bounds = new Rectangle(560, 300, 150, 50);
            menuButton.Action += Menu;
            guiElements.Add("Menu", menuButton);

            CheckBox screenBox = new CheckBox();
            screenBox.Text = " Full Screen";
            screenBox.Texture = squareTexture;
            screenBox.Box = squareTexture;
            screenBox.Bounds = new Rectangle(50, 50, 150, 30);
            screenBox.Action += MakeFullScreen;
            guiElements.Add("FullScreen", screenBox);
            // *************************************************************************

            // ***************************** Scenes ************************************
            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Instructions", new Scene(InstructionsUpdate, InstructionsDraw));
            scenes.Add("Level1", new Scene(Level1Update, Level1Draw));
            scenes.Add("Level2", new Scene(Level2Update, Level2Draw));
            scenes.Add("Level3", new Scene(Level3Update, Level3Draw));
            scenes.Add("Win", new Scene(WinUpdate, WinDraw));
            scenes.Add("Credits", new Scene(CreditsUpdate, CreditsDraw));
            currentScene = scenes["Menu"];
            // *************************************************************************
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();

            guiElements["FullScreen"].Update();
            currentScene.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGreen);
            spriteBatch.Begin();
            guiElements["FullScreen"].Draw(spriteBatch, font);
            spriteBatch.End();
            currentScene.Draw();
            base.Draw(gameTime);
        }

        void MainMenuUpdate()
        {
            guiElements["Instructions"].Update();
            guiElements["Credits"].Update();
        }

        void MainMenuDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "COLOR TILES", new Vector2(580, 200), Color.Black);
            guiElements["Instructions"].Draw(spriteBatch, font);
            guiElements["Credits"].Draw(spriteBatch, font);
            spriteBatch.End();
        }

        void InstructionsUpdate()
        {
            guiElements["Play1"].Update();
        }

        void InstructionsDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Instructions", new Vector2(600, 75), Color.Black);
            spriteBatch.DrawString(font, "Use WASD to move the player (black block).", 
                new Vector2(100, 125), Color.Black);
            spriteBatch.DrawString(font, "Moving the player to a different square will change the color of that square.",
                new Vector2(100, 150), Color.Black);
            spriteBatch.DrawString(font, "Moving towards a square that contains a blue block will push the block.",
                new Vector2(100, 175), Color.Black);
            spriteBatch.DrawString(font, "The blue blocks will not change the color of a square.",
                new Vector2(100, 200), Color.Black);
            spriteBatch.DrawString(font, "Goal: Match the solution picture in the upper-right corner of the level by moving the player, changing the colors of the grid squares and pushing the blocks, ",
                new Vector2(100, 225), Color.Black);
            spriteBatch.DrawString(font, "and ending with the player positioned in the lower-right corner of the grid.",
                new Vector2(100, 250), Color.Black);
            guiElements["Play1"].Draw(spriteBatch, font);
            spriteBatch.End();
        }

        void Level1Update()
        {
            // Go to main menu
            if (InputManager.IsKeyPressed(Keys.Escape))
                currentScene = scenes["Menu"];

            // Shortcut to level 2
            if (InputManager.IsKeyPressed(Keys.N))
            {
                currentScene = scenes["Level2"];
                ResetLevel2(guiElements["Reset1"]);
                levelTimer = 0;
                player.numMoves = 0;
            }
            
            levelTimer += Time.ElapsedGameTime;
            player.Update();
            
            foreach (Block block in blocks)
                block.Update();

            foreach (GridNode node in grid.Nodes)
            {
                // Player pressed grid space
                if (node.IsPressed
                    && Vector3.Distance(player.Transform.Position, node.Transform.Position) <= 6)
                {
                    if (timer == 0.0f)
                    {
                        node.Rigidbody.Velocity = Vector3.Down;
                        node.Rigidbody.Acceleration = Vector3.Down * 9.81f;
                        node.Rigidbody.AnimationSpeed = 1;
                    }

                    timer += Time.ElapsedGameTime;

                    if (node.Transform.Position.Y >= node.OriginalPosition.Y && timer > 0.1f)
                    {
                        node.IsPressed = false;
                        timer = 0.0f;
                        node.Rigidbody.Velocity = Vector3.Zero;
                        node.Rigidbody.Acceleration = Vector3.Zero;
                        node.Rigidbody.AnimationSpeed = 0;
                    }
                    else if (timer >= 0.1f)
                    {
                        node.Rigidbody.Velocity = Vector3.Up;
                        node.Rigidbody.Acceleration = Vector3.Up * 9.81f;
                    }
                    node.Update();
                }
            }

            if (grid.Equals(GetGridSolution1()) && player.currentNode.Row == grid.End.Row
                && player.currentNode.Col == grid.End.Col)
            {
                if (levelChangeTimer == 0.0f)
                    levelTime = levelTimer;

                levelChangeTimer += Time.ElapsedGameTime;

                if (levelChangeTimer >= 3.0f)
                {
                    currentScene = scenes["Level2"];
                    ResetLevel2(guiElements["Reset1"]);
                    levelChangeTimer = 0.0f;
                    levelTimer = 0.0f;
                    player.numMoves = 0;
                }
            }

            guiElements["Reset1"].Update();
        }

        void Level1Draw()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (Block block in blocks)
                block.Draw();
            grid.Draw();
            player.Draw();

            // Draw Solution
            DrawSolution1();

            spriteBatch.Begin();
            guiElements["Reset1"].Draw(spriteBatch, font);
            if (levelChangeTimer > 0.0f)
            {
                float Minutes = (int)(levelTime / 60);
                float Seconds = levelTime % 60;
                spriteBatch.DrawString(font, "Level 1 Time: " + Minutes + " minutes and " 
                    + Seconds + " seconds", new Vector2(520, 100), Color.Black);
                spriteBatch.DrawString(font, "Number of Moves: " + player.numMoves,
                    new Vector2(520, 125), Color.Black);
            }
            spriteBatch.End();
        }
        
        void Level2Update()
        {
            // Go to main menu
            if (InputManager.IsKeyPressed(Keys.Escape))
                currentScene = scenes["Menu"];

            // Shortcut to level 3
            if (InputManager.IsKeyPressed(Keys.N))
            {
                currentScene = scenes["Level3"];
                ResetLevel3(guiElements["Reset3"]);
                levelTimer = 0;
                player.numMoves = 0;
            }

            // Shortcut to level 1
            if (InputManager.IsKeyPressed(Keys.P))
            {
                currentScene = scenes["Level1"];
                ResetLevel1(guiElements["Reset2"]);
                levelTimer = 0;
                player.numMoves = 0;
            }

            levelTimer += Time.ElapsedGameTime;
            player.Update();

            foreach (Block block in blocks)
                block.Update();

            foreach (GridNode node in grid.Nodes)
            {
                // Player pressed grid space
                if (node.IsPressed
                    && Vector3.Distance(player.Transform.Position, node.Transform.Position) <= 6)
                {
                    if (timer == 0.0f)
                    {
                        node.Rigidbody.Velocity = Vector3.Down;
                        node.Rigidbody.Acceleration = Vector3.Down * 9.81f;
                        node.Rigidbody.AnimationSpeed = 1;
                    }

                    timer += Time.ElapsedGameTime;

                    if (node.Transform.Position.Y >= node.OriginalPosition.Y && timer > 0.1f)
                    {
                        node.IsPressed = false;
                        timer = 0.0f;
                        node.Rigidbody.Velocity = Vector3.Zero;
                        node.Rigidbody.Acceleration = Vector3.Zero;
                        node.Rigidbody.AnimationSpeed = 0;
                    }
                    else if (timer >= 0.1f)
                    {
                        node.Rigidbody.Velocity = Vector3.Up;
                        node.Rigidbody.Acceleration = Vector3.Up * 9.81f;
                    }
                    node.Update();
                }
            }

            if (grid.Equals(GetGridSolution2()) && player.currentNode.Row == grid.End.Row
                && player.currentNode.Col == grid.End.Col)
            {
                if (levelChangeTimer == 0.0f)
                    levelTime = levelTimer;

                levelChangeTimer += Time.ElapsedGameTime;

                if (levelChangeTimer >= 3.0f)
                {
                    currentScene = scenes["Level3"];
                    ResetLevel3(guiElements["Reset2"]);
                    levelChangeTimer = 0.0f;
                    levelTimer = 0.0f;
                    player.numMoves = 0;
                }
            }

            guiElements["Reset2"].Update();
        }
        void Level2Draw()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (Block block in blocks)
                block.Draw();
            grid.Draw();
            player.Draw();

            // Draw Solution
            DrawSolution2();

            spriteBatch.Begin();
            guiElements["Reset2"].Draw(spriteBatch, font);
            if (levelChangeTimer > 0.0f)
            {
                float Minutes = (int)(levelTime / 60);
                float Seconds = levelTime % 60;
                spriteBatch.DrawString(font, "Level 3 Time: " + Minutes + " minutes and " + Seconds + " seconds",
                    new Vector2(520, 100), Color.Black);
                spriteBatch.DrawString(font, "Number of Moves: " + player.numMoves,
                    new Vector2(520, 125), Color.Black);
            }
            spriteBatch.End();
        }

        void Level3Update()
        {
            // Go to main menu
            if (InputManager.IsKeyPressed(Keys.Escape))
                currentScene = scenes["Menu"];

            // Shortcut to level 2
            if (InputManager.IsKeyPressed(Keys.P))
            {
                currentScene = scenes["Level2"];
                ResetLevel2(guiElements["Reset3"]);
                levelTimer = 0;
                player.numMoves = 0;
            }

            // Shortcut to Win Screen
            if (InputManager.IsKeyPressed(Keys.N))
            {
                currentScene = scenes["Win"];
                levelTimer = 0;
                player.numMoves = 0;
            }

            levelTimer += Time.ElapsedGameTime;
            player.Update();

            foreach (Block block in blocks)
                block.Update();

            foreach (GridNode node in grid.Nodes)
            {
                // Player pressed grid space
                if (node.IsPressed
                    && Vector3.Distance(player.Transform.Position, node.Transform.Position) <= 6)
                {
                    if (timer == 0.0f)
                    {
                        node.Rigidbody.Velocity = Vector3.Down;
                        node.Rigidbody.Acceleration = Vector3.Down * 9.81f;
                        node.Rigidbody.AnimationSpeed = 1;
                    }

                    timer += Time.ElapsedGameTime;

                    if (node.Transform.Position.Y >= node.OriginalPosition.Y && timer > 0.1f)
                    {
                        node.IsPressed = false;
                        timer = 0.0f;
                        node.Rigidbody.Velocity = Vector3.Zero;
                        node.Rigidbody.Acceleration = Vector3.Zero;
                        node.Rigidbody.AnimationSpeed = 0;
                    }
                    else if (timer >= 0.1f)
                    {
                        node.Rigidbody.Velocity = Vector3.Up;
                        node.Rigidbody.Acceleration = Vector3.Up * 9.81f;
                    }
                    node.Update();
                }
            }

            if (grid.Equals(GetGridSolution3()) && player.currentNode.Row == grid.End.Row
                && player.currentNode.Col == grid.End.Col)
            {
                if (levelChangeTimer == 0.0f)
                    levelTime = levelTimer;

                levelChangeTimer += Time.ElapsedGameTime;

                if (levelChangeTimer >= 3.0f)
                {
                    currentScene = scenes["Win"];
                    levelChangeTimer = 0.0f;
                    levelTimer = 0.0f;
                    player.numMoves = 0;
                }
            }

            guiElements["Reset3"].Update();
        }

        void Level3Draw()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (Block block in blocks)
                block.Draw();
            grid.Draw();
            player.Draw();

            // Draw Solution
            DrawSolution3();

            spriteBatch.Begin();
            guiElements["Reset3"].Draw(spriteBatch, font);
            if (levelChangeTimer > 0.0f)
            {
                float Minutes = (int)(levelTime / 60);
                float Seconds = levelTime % 60;
                spriteBatch.DrawString(font, "Level 3 Time: " + Minutes + " minutes and "
                    + Seconds + " seconds", new Vector2(520, 100), Color.Black);
                spriteBatch.DrawString(font, "Number of Moves: " + player.numMoves,
                    new Vector2(520, 125), Color.Black);
            }
            spriteBatch.End();
        }

        void WinUpdate()
        {
            levelChangeTimer += Time.ElapsedGameTime;

            if (levelChangeTimer >= 5.0f)
            {
                levelChangeTimer = 0.0f;
                currentScene = scenes["Menu"];
            }
        }

        void WinDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Congratulations! You win!", 
                new Vector2(480, 100), Color.Black);
            spriteBatch.End();
        }

        void CreditsUpdate()
        {
            guiElements["Menu"].Update();
        }

        void CreditsDraw()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Credits", new Vector2(620, 75), Color.Black);
            spriteBatch.DrawString(font, "Game Created By: Christina Everman", 
                new Vector2(400, 125), Color.Black);
            spriteBatch.DrawString(font, "Models, Textures, & Sound from CPI 311, Yoshi Kobayashi", 
                new Vector2(400, 150), Color.Black);
            guiElements["Menu"].Draw(spriteBatch, font);
            spriteBatch.End();
        }

        void DrawSolution1()
        {
            Grid Grid = GetGridSolution1();

            spriteBatch.Begin();

            for (int i = 0; i < Grid.Rows; i++)
            {
                for (int j = 0; j < Grid.Cols; j++)
                {
                    if (!Grid.Nodes[i, j].ColorChange && !Grid.Nodes[i, j].HasBox)
                    {
                        NormalSquare.Position = new Vector2(1124 + 32 * j, 32 + 32 * i);
                        NormalSquare.Draw(spriteBatch);
                    }
                    if (Grid.Nodes[i, j].ColorChange)
                    {
                        ColorSquare.Position = new Vector2(1124 + 32 * j, 32 + 32 * i);
                        ColorSquare.Draw(spriteBatch);
                    }
                    if (Grid.Nodes[i, j].HasBox)
                    {
                        BoxSquare.Position = new Vector2(1124 + 32 * j, 32 + 32 * i);
                        BoxSquare.Draw(spriteBatch);
                    }
                }
            }

            spriteBatch.End();
        }

        void DrawSolution2()
        {
            Grid Grid = GetGridSolution2();

            spriteBatch.Begin();

            for (int i = 0; i < Grid.Rows; i++)
            {
                for (int j = 0; j < Grid.Cols; j++)
                {
                    if (!Grid.Nodes[i, j].ColorChange && !Grid.Nodes[i, j].HasBox)
                    {
                        NormalSquare.Position = new Vector2(1060 + 32 * j, 32 + 32 * i);
                        NormalSquare.Draw(spriteBatch);
                    }
                    if (Grid.Nodes[i, j].ColorChange)
                    {
                        ColorSquare.Position = new Vector2(1060 + 32 * j, 32 + 32 * i);
                        ColorSquare.Draw(spriteBatch);
                    }
                    if (Grid.Nodes[i, j].HasBox)
                    {
                        BoxSquare.Position = new Vector2(1060 + 32 * j, 32 + 32 * i);
                        BoxSquare.Draw(spriteBatch);
                    }
                }
            }

            spriteBatch.End();
        }

        void DrawSolution3()
        {
            Grid Grid = GetGridSolution3();

            spriteBatch.Begin();

            for (int i = 0; i < Grid.Rows; i++)
            {
                for (int j = 0; j < Grid.Cols; j++)
                {
                    if (!Grid.Nodes[i, j].ColorChange && !Grid.Nodes[i, j].HasBox)
                    {
                        NormalSquare.Position = new Vector2(996 + 32 * j, 32 + 32 * i);
                        NormalSquare.Draw(spriteBatch);
                    }
                    if (Grid.Nodes[i, j].ColorChange)
                    {
                        ColorSquare.Position = new Vector2(996 + 32 * j, 32 + 32 * i);
                        ColorSquare.Draw(spriteBatch);
                    }
                    if (Grid.Nodes[i, j].HasBox)
                    {
                        BoxSquare.Position = new Vector2(996 + 32 * j, 32 + 32 * i);
                        BoxSquare.Draw(spriteBatch);
                    }
                }
            }

            spriteBatch.End();
        }

        Grid GetGridSolution1()
        {
            Grid Solution = new Grid(Content, camera, GraphicsDevice, light, 4, 4);

            Solution.Nodes[0, 2].ColorChange = true;
            Solution.Nodes[0, 3].ColorChange = true;
            Solution.Nodes[0, 2].ColorChange = true;
            Solution.Nodes[1, 0].ColorChange = true;
            Solution.Nodes[1, 3].ColorChange = true;
            Solution.Nodes[2, 0].ColorChange = true;
            Solution.Nodes[2, 1].ColorChange = true;
            Solution.Nodes[3, 1].ColorChange = true;
            Solution.Nodes[3, 3].ColorChange = true;

            Solution.Nodes[3, 0].HasBox = true;

            return Solution;
        }

        Grid GetGridSolution2()
        {
            Grid Solution = new Grid(Content, camera, GraphicsDevice, light, 6, 6);

            Solution.Nodes[1, 0].ColorChange = true;
            Solution.Nodes[1, 1].ColorChange = true;
            Solution.Nodes[1, 2].ColorChange = true;
            Solution.Nodes[2, 0].ColorChange = true;
            Solution.Nodes[2, 1].ColorChange = true;
            Solution.Nodes[2, 3].ColorChange = true;
            Solution.Nodes[2, 5].ColorChange = true;
            Solution.Nodes[3, 0].ColorChange = true;
            Solution.Nodes[3, 2].ColorChange = true;
            Solution.Nodes[3, 5].ColorChange = true;
            Solution.Nodes[4, 0].ColorChange = true;
            Solution.Nodes[4, 2].ColorChange = true;
            Solution.Nodes[4, 5].ColorChange = true;
            Solution.Nodes[5, 0].ColorChange = true;
            Solution.Nodes[5, 1].ColorChange = true;
            Solution.Nodes[5, 2].ColorChange = true;
            Solution.Nodes[5, 4].ColorChange = true;
            Solution.Nodes[5, 5].ColorChange = true;

            Solution.Nodes[0, 2].HasBox = true;
            Solution.Nodes[1, 5].HasBox = true;

            return Solution;
        }

        Grid GetGridSolution3()
        {
            Grid Solution = new Grid(Content, camera, GraphicsDevice, light, 8, 8);

            Solution.Nodes[1, 0].ColorChange = true;
            Solution.Nodes[1, 2].ColorChange = true;
            Solution.Nodes[1, 3].ColorChange = true;
            Solution.Nodes[1, 4].ColorChange = true;
            Solution.Nodes[1, 5].ColorChange = true;
            Solution.Nodes[1, 6].ColorChange = true;
            Solution.Nodes[2, 4].ColorChange = true;
            Solution.Nodes[2, 5].ColorChange = true;
            Solution.Nodes[2, 6].ColorChange = true;
            Solution.Nodes[3, 0].ColorChange = true;
            Solution.Nodes[3, 1].ColorChange = true;
            Solution.Nodes[3, 2].ColorChange = true;
            Solution.Nodes[3, 4].ColorChange = true;
            Solution.Nodes[3, 6].ColorChange = true;
            Solution.Nodes[4, 0].ColorChange = true;
            Solution.Nodes[4, 2].ColorChange = true;
            Solution.Nodes[4, 4].ColorChange = true;
            Solution.Nodes[4, 5].ColorChange = true;
            Solution.Nodes[5, 0].ColorChange = true;
            Solution.Nodes[5, 1].ColorChange = true;
            Solution.Nodes[5, 2].ColorChange = true;
            Solution.Nodes[5, 3].ColorChange = true;
            Solution.Nodes[5, 4].ColorChange = true;
            Solution.Nodes[5, 5].ColorChange = true;
            Solution.Nodes[5, 6].ColorChange = true;
            Solution.Nodes[6, 3].ColorChange = true;
            Solution.Nodes[6, 4].ColorChange = true;
            Solution.Nodes[6, 5].ColorChange = true;
            Solution.Nodes[6, 7].ColorChange = true;
            Solution.Nodes[7, 3].ColorChange = true;
            Solution.Nodes[7, 4].ColorChange = true;
            Solution.Nodes[7, 5].ColorChange = true;
            Solution.Nodes[7, 6].ColorChange = true;
            Solution.Nodes[7, 7].ColorChange = true;

            Solution.Nodes[1, 7].HasBox = true;
            Solution.Nodes[6, 0].HasBox = true;
            Solution.Nodes[4, 1].HasBox = true;

            return Solution;
        }

        void ResetLevel1(GUIElement element)
        {
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition += Vector3.Up * 20 + Vector3.Backward * 16;
            camera.Transform.Rotate(Vector3.Left, MathHelper.PiOver4);

            grid = new Grid(Content, camera, GraphicsDevice, light, 4, 4);
            player = new Player(Content, camera, GraphicsDevice, light, grid);

            Block block1 = new Block(Content, camera, GraphicsDevice, light, player, grid.Nodes[1, 0]);
            block1.Transform.Position = grid.Nodes[1, 0].Transform.Position + Vector3.Up * 5;
            blocks = new List<Block>();
            blocks.Add(block1);
        }

        void ResetLevel2(GUIElement element)
        {
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition += Vector3.Up * 24 + Vector3.Backward * 20;
            camera.Transform.Rotate(Vector3.Left, MathHelper.PiOver4);

            grid = new Grid(Content, camera, GraphicsDevice, light, 6, 6);
            player = new Player(Content, camera, GraphicsDevice, light, grid);

            Block block1 = new Block(Content, camera, GraphicsDevice, light, player, grid.Nodes[2, 2]);
            block1.Transform.Position = grid.Nodes[2, 2].Transform.Position + Vector3.Up * 5;
            Block block2 = new Block(Content, camera, GraphicsDevice, light, player, grid.Nodes[3, 5]);
            block2.Transform.Position = grid.Nodes[3, 5].Transform.Position + Vector3.Up * 5;
            blocks = new List<Block>();
            blocks.Add(block1);
            blocks.Add(block2);
        }

        void ResetLevel3(GUIElement element)
        {
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition += Vector3.Up * 28 + Vector3.Backward * 24;
            camera.Transform.Rotate(Vector3.Left, MathHelper.PiOver4);

            grid = new Grid(Content, camera, GraphicsDevice, light, 8, 8);
            player = new Player(Content, camera, GraphicsDevice, light, grid);

            Block block1 = new Block(Content, camera, GraphicsDevice, light, player, grid.Nodes[3, 3]);
            block1.Transform.Position = grid.Nodes[3, 3].Transform.Position + Vector3.Up * 5;
            Block block2 = new Block(Content, camera, GraphicsDevice, light, player, grid.Nodes[3, 4]);
            block2.Transform.Position = grid.Nodes[3, 4].Transform.Position + Vector3.Up * 5;
            Block block3 = new Block(Content, camera, GraphicsDevice, light, player, grid.Nodes[4, 4]);
            block3.Transform.Position = grid.Nodes[4, 4].Transform.Position + Vector3.Up * 5;
            blocks = new List<Block>();
            blocks.Add(block1);
            blocks.Add(block2);
            blocks.Add(block3);
        }
        
        void PlayLevel1(GUIElement element)
        {
            currentScene = scenes["Level1"];
            ResetLevel1(element);
        }

        void PlayLevel2(GUIElement element)
        {
            currentScene = scenes["Level2"];
            ResetLevel2(element);
        }

        void PlayLevel3(GUIElement element)
        {
            currentScene = scenes["Level3"];
            ResetLevel3(element);
        }

        void Instructions(GUIElement element)
        {
            currentScene = scenes["Instructions"];
        }

        void Win(GUIElement element)
        {
            currentScene = scenes["Win"];
        }

        void Menu(GUIElement element)
        {
            currentScene = scenes["Menu"];
        }

        void Credits(GUIElement element)
        {
            currentScene = scenes["Credits"];
        }

        void MakeFullScreen(GUIElement element)
        {
            ScreenManager.Setup(!ScreenManager.IsFullScreen,
                ScreenManager.Width, ScreenManager.Height);
        }
    }
}