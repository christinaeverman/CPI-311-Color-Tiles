using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

using CPI311.GameEngine;

namespace Assignment04
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Assignment04 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TerrainRenderer terrain;
        List<Camera> cameras;
        Effect effect;
        AStarSearch search;
        int gridSize = 20; // 20x20 maze
        Model cube;
        Model sphere;
        Random random = new Random();
        Player player;
        Light light;
        List<Agent> agents;
        SpriteFont font;
        Texture2D texture;
        bool alienCaught;
        Bomb bomb;

        public Assignment04()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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

            cameras = new List<Camera>();
            for (int i = 0; i < 2; i++)
            {
                cameras.Add(new Camera());
                cameras[i].Transform = new Transform();
            }
            
            cameras[0].Transform.LocalPosition = Vector3.Up * 60;
            cameras[0].Transform.Rotate(Vector3.Left, MathHelper.PiOver2);

            terrain = new TerrainRenderer(
                Content.Load<Texture2D>("mazeH2"),
                Vector2.One * 100,
                Vector2.One * 200);
            terrain.NormalMap = Content.Load<Texture2D>("mazeN2");
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1, 5, 1);
            
            agents = new List<Agent>();

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.3f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            search = new AStarSearch(gridSize, gridSize);

            foreach (AStarNode node in search.Nodes)
            {
                if (terrain.GetAltitude(node.Position) > 0)
                    node.Passable = false;
            }

            light = new Light();
            light.Transform = new Transform();

            texture = Content.Load<Texture2D>("Square");
            cube = Content.Load<Model>("Box2");
            sphere = Content.Load<Model>("Sphere");

            player = new Player(terrain, Content, cameras[0], GraphicsDevice, light);
            player.Add<BoxCollider>();
            Renderer playerRenderer = new Renderer(cube, player.Rigidbody.Transform, cameras[0], 
                Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture);
            player.Add<Renderer>(playerRenderer);

            for (int i = 0; i < 3; i++)
            {
                agents.Add(new Agent(terrain, Content, cameras[0], GraphicsDevice, light));
                agents[i].Add<Rigidbody>();
                Renderer agentRenderer = new Renderer(sphere, agents[i].Rigidbody.Transform,
                    cameras[0], Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture);
                agents[i].Add<Renderer>(agentRenderer);
                while (!(agents[i].search.Start = agents[i].search.Nodes[random.Next(agents[i].search.Cols),
                    random.Next(agents[i].search.Rows)]).Passable);
            }

            bomb = new Bomb(terrain, Content, cameras[0], graphics.GraphicsDevice, light, player);
            bomb.Add<Rigidbody>();
            Renderer bombRenderer = new Renderer(cube, bomb.Rigidbody.Transform, cameras[0],
                Content, graphics.GraphicsDevice, light, 1, "SimpleShading", 20f, texture);
            bomb.Add<Renderer>(bombRenderer);
            int randomPos2 = (int)random.Next(0, 19);
            while (!(bomb.search.Start = bomb.search.Nodes[random.Next(bomb.search.Cols),
                    random.Next(bomb.search.Rows)]).Passable) ;

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.Update(gameTime);
            InputManager.Update();
            player.Update();
            bomb.Update();
            
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                agents[i].Update();

                Vector3 distance = player.Transform.LocalPosition - agents[i].Transform.LocalPosition;
                if (distance.Length() < 2)
                {
                    alienCaught = true;
                    count = i;
                    break;
                }
            }

            if (alienCaught)
            {
                player.aliensCaught++;
                agents.Remove(agents[count]);
                agents.Add(new Agent(terrain, Content, cameras[0], GraphicsDevice, light));
                agents[2].Add<Rigidbody>();
                Renderer agentRenderer = new Renderer(sphere, agents[2].Rigidbody.Transform,
                    cameras[0], Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture);
                agents[2].Add<Renderer>(agentRenderer);
                alienCaught = false;
            }

            Vector3 distance2 = player.Transform.LocalPosition - bomb.Transform.LocalPosition;
            if (distance2.Length() < 5)
            {
                player = null;
                bomb = null;
                player = new Player(terrain, Content, cameras[0], GraphicsDevice, light);
                player.Add<BoxCollider>();
                Renderer playerRenderer = new Renderer(cube, player.Rigidbody.Transform, cameras[0],
                    Content, GraphicsDevice, light, 1, "SimpleShading", 20f, texture);
                player.Add<Renderer>(playerRenderer);
                bomb = new Bomb(terrain, Content, cameras[0], graphics.GraphicsDevice, light, player);
                bomb.Add<Rigidbody>();
                Renderer bombRenderer = new Renderer(cube, bomb.Rigidbody.Transform, cameras[0],
                    Content, graphics.GraphicsDevice, light, 1, "SimpleShading", 20f, texture);
                bomb.Add<Renderer>(bombRenderer);
                while (!(bomb.search.Start = bomb.search.Nodes[random.Next(bomb.search.Cols),
                    random.Next(bomb.search.Rows)]).Passable) ;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect.Parameters["NormalMap"].SetValue(terrain.NormalMap);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["View"].SetValue(cameras[0].View);
            effect.Parameters["Projection"].SetValue(cameras[0].Projection);
            effect.Parameters["LightPosition"].SetValue(cameras[0].Transform.Position + Vector3.Up * 10);
            effect.Parameters["CameraPosition"].SetValue(cameras[0].Transform.Position);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();
            }

            player.Draw();

            foreach (Agent agent in agents)
                agent.Draw();
            
            bomb.Draw();

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Aliens Caught: " + player.aliensCaught, new Vector2(0, 25), Color.Black);
            spriteBatch.DrawString(font, "Time Spent: " + player.timeSpent, Vector2.Zero, Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
