using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;

  
namespace CSC3163DCircusCharlie
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SpriteFont gameFont;
        SpriteFont devFont;
        KeyboardState kState;
        Viewport view2D;

        Model player;
        Model ground;
        Model barrel;

        Matrix proj;
        Matrix view;
        Matrix worldPlayer;
        Matrix worldBarrel;


        float rotX;
        float rotY;
        float rotZ;

        Vector3 cameraPOS;
        Vector3 playerPos;
        Vector3 barrelPos;
        Vector2 playerPos2D;
        Vector2 barrelPos2D;
        Vector2 scoreLocation;
        Vector2 hitMessageLocation;
        Vector2 countdownLocation;
        Vector2 gameOverLocation;
        Vector2 devLocation;

        bool gameOver;
        bool startGame;
        bool heightIncreasing;
        bool heightDec;
        
        float playerPosInitX;
        float playerRadius;
        float barrelDistanceFront;
        float barrelDistanceBack;

        float delta;
        float jumpHeight;
        float decHeight;
        float heightChangeRatio;
        float hoverTime;

        float countdown;
        int score;
        int prevScore;
        Random rand;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            rotX = 0.0f;
            rotY = 90.0f;
            rotZ = 0.0f;
            jumpHeight = 0.3f;
            decHeight = 0.3f;
            hoverTime = 1.1f;
            heightChangeRatio = 1;
            countdown = 5;
            startGame = false;
            score = 0;
            gameOver = true;
            heightIncreasing = false;
            devLocation = new Vector2(900, 900);
            gameOverLocation = new Vector2(900, 900);
            hitMessageLocation = new Vector2(900, 900);
            scoreLocation = new Vector2(900, 900);
            countdownLocation = new Vector2(900, 900);
            prevScore = 5;


            delta = 0.2f;

            playerRadius = 50;
            heightChangeRatio = 1.0f;

            playerPosInitX = -20;


            cameraPOS = new Vector3(0, 4, 13);
            playerPos = new Vector3(-2, 0, 0);
            barrelPos = new Vector3(10, 0, 0);

            view2D = new Viewport(0, 0,
                                _graphics.PreferredBackBufferWidth,
                                _graphics.PreferredBackBufferHeight);

            rand = new Random();

            proj = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(60),
                _graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight,
                0.001f,
                1000f
                );

            view = Matrix.CreateLookAt(
                cameraPOS,
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0)
                );

            worldPlayer = Matrix.CreateScale(0.005f) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rotZ)) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(rotY)) *
                    Matrix.CreateRotationX(MathHelper.ToRadians(rotX)) *
                    Matrix.CreateTranslation(playerPos);

            worldPlayer = Matrix.CreateScale(0.005f) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rotZ)) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(rotY)) *
                    Matrix.CreateRotationX(MathHelper.ToRadians(rotX)) *
                    Matrix.CreateTranslation(barrelPos);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            player = Content.Load<Model>("Animal_Rigged_Zebu_01");
            ground = Content.Load<Model>("Uneven_Ground_Dirt_01");
            barrel = Content.Load<Model>("Barrel_Sealed_01");
            gameFont = Content.Load<SpriteFont>("galleryFont");
            devFont = Content.Load<SpriteFont>("galleryFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            kState = Keyboard.GetState();

            if (gameOver)
            {
                barrelPos = new Vector3(20, 0, 0);
                hitMessageLocation = new Vector2(10, 400);
                scoreLocation = new Vector2(900, 900);
                countdownLocation = new Vector2(900, 900);
                playerPos = new Vector3(-2, 0, 0);

                if (kState.IsKeyDown(Keys.Enter) == true)
                {
                    startGame = true;
                }
                if (startGame)
                {
                    devLocation = new Vector2(900, 900);
                    gameOverLocation = new Vector2(900, 900);
                    hitMessageLocation = new Vector2(900, 900);
                    scoreLocation = new Vector2(900, 900);
                    if (countdown > 0.1)
                    {
                        countdownLocation = new Vector2(10, 10);
                        countdown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        score = 0;
                        delta = 0.2f;
                    }
                    else
                    {
                        gameOver = false;
                    }
                }
            }
            else
            {
                // Left button control
                devLocation = new Vector2(900, 900);
                gameOverLocation = new Vector2(900, 900);
                hitMessageLocation = new Vector2(900, 900);
                scoreLocation = new Vector2(10, 10);
                countdownLocation = new Vector2(900, 900);
                if (kState.IsKeyDown(Keys.Up) == true)
                {
                    if (playerPos.Y <= 0.1)
                    {
                        heightIncreasing = true;
                        heightDec = false;
                    }
                }

                if (heightIncreasing)
                {
                    if (hoverTime > 0)
                    {
                        if (playerPos.Y < 3.5)
                        {
                            playerPos.Y += jumpHeight * heightChangeRatio;
                        }
                        else
                        {
                            heightIncreasing = false;
                            heightDec = true;
                        }
                    }
                    else
                    {
                        heightIncreasing = false;
                        heightDec = true;
                    }
                }
                else
                {
                    if (playerPos.Y > 0)
                    {
                        if (hoverTime >= 0)
                        {
                            hoverTime -= 0.1f;
                        }
                        else
                        {
                            playerPos.Y -= decHeight * heightChangeRatio;
                            if (heightChangeRatio < 1)
                            {
                                heightChangeRatio += 0.05f;
                            }
                        }
                    }
                    else
                    {
                        heightDec = false;
                        heightChangeRatio = 1;
                        hoverTime = 1.1f;
                    }
                }

                // Update player's position
                if (!gameOver)
                {
                    barrelPos.X -= delta;

                    rotX = 0;

                    // Reset player position if run out of the screen
                    if (barrelPos.X < -10)
                    {
                        barrelPos.X = 10;
                        score++;
                        if (score % prevScore == 0)
                        {
                            delta += 0.025f;
                        }
                    }

                    // if the player is hit, reset the position, the update the playerHit to False

                }
                else
                {

                }



                playerPos2D = new Vector2(view2D.Project(playerPos, proj, view, worldPlayer).X,
                                         view2D.Project(playerPos, proj, view, worldPlayer).Y);

                barrelPos2D = new Vector2(view2D.Project(barrelPos, proj, view, worldBarrel).X,
                                         view2D.Project(barrelPos, proj, view, worldBarrel).Y);
                // Reset player position if run out of the screen


                barrelDistanceFront = Vector2.Distance(new Vector2(playerPos2D.X + 30, playerPos2D.Y), barrelPos2D);
                if (barrelDistanceFront < playerRadius)
                {
                    // hit the enemy
                    gameOver = true;
                    startGame = false;
                    countdown = 5;
                    hitMessageLocation = new Vector2(10, 400);
                    scoreLocation = new Vector2(900, 900);
                    countdownLocation = new Vector2(900, 900);
                    devLocation = new Vector2(_graphics.PreferredBackBufferWidth -175, _graphics.PreferredBackBufferHeight - 30);
                    gameOverLocation = new Vector2(10, 10);
                    delta = 0.2f;
                }
                barrelDistanceBack = Vector2.Distance(new Vector2(playerPos2D.X - 80, playerPos2D.Y), barrelPos2D);
                if (barrelDistanceBack < playerRadius)
                {
                    // hit the enemy
                    gameOver = true;
                    startGame = false;
                    countdown = 5;
                    gameOverLocation = new Vector2(10, 10);
                    hitMessageLocation = new Vector2(10, 400);
                    scoreLocation = new Vector2(900, 900);
                    delta = 0.2f;
                    countdownLocation = new Vector2(900, 900);
                    devLocation = new Vector2(_graphics.PreferredBackBufferWidth - 175, _graphics.PreferredBackBufferHeight - 30);
                }
            }



            view = Matrix.CreateLookAt(
                    cameraPOS,
                    new Vector3(0, 0, 0),
                    new Vector3(0, 1, 0)
                    );

            worldPlayer = Matrix.CreateScale(0.005f) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rotZ)) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(rotY)) *
                    Matrix.CreateRotationX(MathHelper.ToRadians(rotX)) *
                    Matrix.CreateTranslation(playerPos);

            worldBarrel = Matrix.CreateScale(0.005f) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rotZ)) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(rotY)) *
                    Matrix.CreateRotationX(MathHelper.ToRadians(rotX)) *
                    Matrix.CreateTranslation(barrelPos);



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            ground.Draw(Matrix.CreateScale(10, 0.01f, 10),
                        view, proj);
            //player.Draw(world, view, proj);
            DrawModel(player, worldPlayer, view, proj);
            DrawModel(barrel, worldBarrel, view, proj);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(gameFont, "Your Score is " + score.ToString() + "! \nClick Enter To Start Game or ESC to Exit!", hitMessageLocation, Color.Black);
            _spriteBatch.DrawString(gameFont, score.ToString() , scoreLocation, Color.Black);
            _spriteBatch.DrawString(gameFont, countdown.ToString(), countdownLocation, Color.Black);
            _spriteBatch.DrawString(gameFont, "Game Over!", gameOverLocation, Color.Black);
            _spriteBatch.DrawString(devFont, "Developed by Pheba Colfer", devLocation, Color.Black, default, default,0.4f,default,default);
            _spriteBatch.End();


            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;

                    // default lighting
                    effect.EnableDefaultLighting();

                    // turn on the lighting subsystem
                    effect.LightingEnabled = true;

                    effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
                    //effect.EmissiveColor = new Vector3(0.0f, 0, 1);
                }

                mesh.Draw();
            }
        }

    }
}
