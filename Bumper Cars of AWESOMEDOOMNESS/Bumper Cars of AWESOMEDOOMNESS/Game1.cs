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

using SpriteLibrary;
using BumperCars;

namespace Bumper_Cars_of_AWESOMEDOOMNESS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D CarTexture1;
        Texture2D CarTexture2;
        Texture2D OilTexture;
        Texture2D CoinTexture;

        SoundEffect BackgroundMusic;
        SoundEffect CoinSound;

        SoundEffectInstance MusicInstance;

        SpriteFont Font;

        Car PlayerCar1;
        Car PlayerCar2;

        Sprite DogeCoin;
        Sprite OilSlick;

        LinkedList<Sprite> OilSlickList;

        const double STEER_FACTOR = 5;
        const double ACCEL_FACTOR = 0.02;

        bool MultiPlayer;
        bool AIPlayer;
        bool HockeyPlayer;


        double SlickTimer = 300;
        int MaxSlicks = 16;
        int lastSlick = 0;

        int Winner;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        enum GameScreen
        {
            TITLE = 0,
            PLAYING = 1,
            GAMEOVER = 2
        }

        GameScreen CurrentScreen;
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
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

            // TODO: use this.Content to load your game content here
            CarTexture1 = this.Content.Load<Texture2D>("Images\\bumper_car1");
            CarTexture2 = this.Content.Load<Texture2D>("Images\\bumper_car2");
            OilTexture = this.Content.Load<Texture2D>("Images\\oil_slick");
            CoinTexture = this.Content.Load<Texture2D>("Images\\spinning_coin");

            CoinSound = this.Content.Load<SoundEffect>("Sound\\bumpercar_coin");
            BackgroundMusic = this.Content.Load<SoundEffect>("Sound\\Theme");

            // Main game font
            Font = this.Content.Load<SpriteFont>("GameFont");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            if (CurrentScreen == GameScreen.TITLE)
            {
                UpdateTitle(gameTime);
            }
            if (CurrentScreen == GameScreen.PLAYING)
            {
                UpdatePlaying(gameTime);
            }
            if (CurrentScreen == GameScreen.GAMEOVER)
            {
                UpdateGameOver(gameTime);
            }
            // TODO: Add your update logic here

            base.Update(gameTime);

        }

        void UpdateTitle(GameTime gameTime)
        {

            KeyboardState CurrentKeyState = Keyboard.GetState();

            GamePadState Player1PadState = GamePad.GetState(PlayerIndex.One);

            GamePadState Player2PadState = GamePad.GetState(PlayerIndex.Two);

            if ((CurrentKeyState.IsKeyDown(Keys.NumPad1)) || (Player1PadState.IsButtonDown(Buttons.X)) || (Player2PadState.IsButtonDown(Buttons.X)))
            {

                AIPlayer = true;
                StartGame(MultiPlayer, AIPlayer, HockeyPlayer);

            }
            if ((CurrentKeyState.IsKeyDown(Keys.NumPad2)) || (Player1PadState.IsButtonDown(Buttons.Y)) || (Player2PadState.IsButtonDown(Buttons.Y)))
            {

                MultiPlayer = true;
                StartGame(MultiPlayer, AIPlayer, HockeyPlayer);

            }
            if ((CurrentKeyState.IsKeyDown(Keys.NumPad9)) || (Player1PadState.IsButtonDown(Buttons.DPadDown)) || (Player2PadState.IsButtonDown(Buttons.DPadDown)))
            {

                HockeyPlayer = true;
                StartGame(MultiPlayer, AIPlayer, HockeyPlayer);

            }

        }
        void UpdatePlaying(GameTime gameTime)
        {

            KeyboardState CurrentKeyState = Keyboard.GetState();

            GamePadState Player1PadState = GamePad.GetState(PlayerIndex.One);

            GamePadState Player2PadState = GamePad.GetState(PlayerIndex.Two);

            if (Player1PadState.IsConnected)
            {

                if ((Player1PadState.IsButtonDown(Buttons.A)) || (Player1PadState.IsButtonDown(Buttons.RightTrigger)))
                {
                    PlayerCar1.Accelerate(ACCEL_FACTOR);
                }
                if ((Player1PadState.IsButtonDown(Buttons.B)) || (Player1PadState.IsButtonDown(Buttons.LeftTrigger)))
                {
                    PlayerCar1.Accelerate(-ACCEL_FACTOR);
                }
                if ((Player1PadState.IsButtonUp(Buttons.A)) && (Player1PadState.IsButtonUp(Buttons.RightTrigger)))
                {
                    PlayerCar1.Accelerate(-ACCEL_FACTOR / 2);
                }
                if (Player1PadState.ThumbSticks.Left.X < 0.5)
                {
                    PlayerCar1.Steer(STEER_FACTOR);
                }
                if (Player1PadState.ThumbSticks.Left.X > - 0.5)
                {
                    PlayerCar1.Steer(-STEER_FACTOR);
                }

            }
            else
            {

                if ((CurrentKeyState.IsKeyDown(Keys.W)) || (CurrentKeyState.IsKeyDown(Keys.Space)))
                {
                    PlayerCar1.Accelerate(ACCEL_FACTOR);
                }
                if ((CurrentKeyState.IsKeyDown(Keys.S)) || (CurrentKeyState.IsKeyDown(Keys.LeftShift)))
                {
                    PlayerCar1.Accelerate(-ACCEL_FACTOR);
                }
                if ((CurrentKeyState.IsKeyUp(Keys.W)) && (CurrentKeyState.IsKeyUp(Keys.Space)))
                {
                    PlayerCar1.Accelerate(-ACCEL_FACTOR / 2);
                }
                if (CurrentKeyState.IsKeyDown(Keys.A))
                {
                    PlayerCar1.Steer(STEER_FACTOR);
                }
                if (CurrentKeyState.IsKeyDown(Keys.D))
                {
                    PlayerCar1.Steer(-STEER_FACTOR);
                }

            }

            if (MultiPlayer)
            {
                if (Player2PadState.IsConnected)
                {

                    if ((Player2PadState.IsButtonDown(Buttons.A)) || (Player2PadState.IsButtonDown(Buttons.RightTrigger)))
                    {
                        PlayerCar2.Accelerate(ACCEL_FACTOR);
                    }
                    if ((Player2PadState.IsButtonDown(Buttons.B)) || (Player2PadState.IsButtonDown(Buttons.LeftTrigger)))
                    {
                        PlayerCar2.Accelerate(-ACCEL_FACTOR);
                    }
                    if ((Player2PadState.IsButtonUp(Buttons.A)) && (Player2PadState.IsButtonUp(Buttons.RightTrigger)))
                    {
                        PlayerCar2.Accelerate(-ACCEL_FACTOR / 2);
                    }
                    if (Player2PadState.ThumbSticks.Left.X < 0.5)
                    {
                        PlayerCar2.Steer(STEER_FACTOR);
                    }
                    if (Player2PadState.ThumbSticks.Left.X > -0.5)
                    {
                        PlayerCar2.Steer(-STEER_FACTOR);
                    }

                }
                else
                {

                    if ((CurrentKeyState.IsKeyDown(Keys.Up)) || (CurrentKeyState.IsKeyDown(Keys.NumPad0)))
                    {
                        PlayerCar1.Accelerate(ACCEL_FACTOR);
                    }
                    if ((CurrentKeyState.IsKeyDown(Keys.Down)) || (CurrentKeyState.IsKeyDown(Keys.RightControl)))
                    {
                        PlayerCar1.Accelerate(-ACCEL_FACTOR);
                    }
                    if ((CurrentKeyState.IsKeyUp(Keys.Up)) && (CurrentKeyState.IsKeyUp(Keys.NumPad0)))
                    {
                        PlayerCar1.Accelerate(-ACCEL_FACTOR / 2);
                    }
                    if (CurrentKeyState.IsKeyDown(Keys.Left))
                    {
                        PlayerCar1.Steer(STEER_FACTOR);
                    }
                    if (CurrentKeyState.IsKeyDown(Keys.Right))
                    {
                        PlayerCar1.Steer(-STEER_FACTOR);
                    }

                }
            }
            if (AIPlayer)
            {
                float CoinCenterX = DogeCoin.UpperLeft.X + DogeCoin.GetWidth();
                float CoinCenterY = DogeCoin.UpperLeft.Y + DogeCoin.GetHeight();
                float CarCenterX = PlayerCar2.UpperLeft.X + PlayerCar2.GetWidth();
                float CarCenterY = PlayerCar2.UpperLeft.Y + PlayerCar2.GetHeight();

                Vector2 Direction = new Vector2(CoinCenterX - CarCenterX, CoinCenterY - CarCenterY);

                Double Angle = Sprite.CalculateDirectionAngle(Direction);

                if (Angle <= PlayerCar2.GetDirectionAngle())// && (Angle >= PlayerCar2.GetDirectionAngle()))
                {
                    PlayerCar2.Steer(-STEER_FACTOR);
                }
                if (Angle >= PlayerCar2.GetDirectionAngle())// && (Angle <= PlayerCar2.GetDirectionAngle()))
                {
                    PlayerCar2.Steer(STEER_FACTOR);
                }

                PlayerCar2.Accelerate(ACCEL_FACTOR);
            }

            if(HockeyPlayer)
            {
                PlayerCar2.Accelerate(-ACCEL_FACTOR / 2);
            }

            if(PlayerCar1.Score == 10)
            {
                MusicInstance.Stop();

                Winner = 1;
                CurrentScreen = GameScreen.GAMEOVER;
            }
            if (PlayerCar2.Score == 10)
            {
                MusicInstance.Stop();

                Winner = 2;
                CurrentScreen = GameScreen.GAMEOVER;
            }

            if (lastSlick > SlickTimer)
            {
                NewOilSlick();
                lastSlick = gameTime.ElapsedGameTime.Milliseconds;
                SlickTimer = 300;
            }
            else
            {
                SlickTimer -= 1.66;
            }

            PlayerCar1.RotationAngle = PlayerCar1.GetDirectionAngle();
            PlayerCar2.RotationAngle = PlayerCar2.GetDirectionAngle();

            DogeCoin.Animate(gameTime);

        }
        void UpdateGameOver(GameTime gameTime)
        {

            KeyboardState CurrentKeyState = Keyboard.GetState();

            GamePadState Player1PadState = GamePad.GetState(PlayerIndex.One);

            GamePadState Player2PadState = GamePad.GetState(PlayerIndex.Two);

            if ((CurrentKeyState.IsKeyDown(Keys.Escape)) || (Player1PadState.IsButtonDown(Buttons.Start)) || (Player2PadState.IsButtonDown(Buttons.Start)))
            {
                CurrentScreen = GameScreen.TITLE;

                MultiPlayer = false;
                AIPlayer = false;
                HockeyPlayer = false;

            }
            if ((CurrentKeyState.IsKeyDown(Keys.NumPad1)) || (Player1PadState.IsButtonDown(Buttons.X)) || (Player2PadState.IsButtonDown(Buttons.X)))
            {

                MultiPlayer = false;
                AIPlayer = false;
                HockeyPlayer = false;

                AIPlayer = true;

                StartGame(MultiPlayer, AIPlayer, HockeyPlayer);

            }
            if ((CurrentKeyState.IsKeyDown(Keys.NumPad2)) || (Player1PadState.IsButtonDown(Buttons.Y)) || (Player2PadState.IsButtonDown(Buttons.Y)))
            {

                MultiPlayer = false;
                AIPlayer = false;
                HockeyPlayer = false;

                MultiPlayer = true;

                StartGame(MultiPlayer, AIPlayer, HockeyPlayer);

            }
            if ((CurrentKeyState.IsKeyDown(Keys.NumPad9)) || (Player1PadState.IsButtonDown(Buttons.DPadDown)) || (Player2PadState.IsButtonDown(Buttons.DPadDown)))
            {

                MultiPlayer = false;
                AIPlayer = false;
                HockeyPlayer = false;

                HockeyPlayer = true;

                StartGame(MultiPlayer, AIPlayer, HockeyPlayer);

            }

        }

        void StartGame(bool MultiPlayer, bool AIPlayer, bool HockeyPlayer)
        {
            
            MusicInstance = BackgroundMusic.CreateInstance();
            MusicInstance.Volume = 0.5f;
            MusicInstance.IsLooped = true;

            PlayerCar1 = new Car();
            PlayerCar2 = new Car();

            DogeCoin = new Sprite();

            OilSlickList = new LinkedList<Sprite>();

            PlayerCar1.SetTexture(CarTexture1);
            PlayerCar1.Scale = new Vector2(1.5f, 1.5f);
            PlayerCar1.RotationAngle = 360;
            PlayerCar1.UpperLeft = new Vector2(400, 200);
            PlayerCar1.MaxSpeed = 3;
            PlayerCar1.Origin = PlayerCar1.GetCenter();
            PlayerCar1.Score = 0;

            PlayerCar2.SetTexture(CarTexture2);
            PlayerCar2.Scale = new Vector2(1.5f, 1.5f);
            PlayerCar2.RotationAngle = 180;
            PlayerCar2.UpperLeft = new Vector2(300, 200);
            PlayerCar2.MaxSpeed = 3;
            PlayerCar2.Origin = PlayerCar2.GetCenter();
            PlayerCar2.Score = 0;

            DogeCoin.SetTexture(CoinTexture, 5);
            DogeCoin.AnimationInterval = 200;

            PlaceDogeCoin();

            MusicInstance.Play();

            for (int i = 0; i < MaxSlicks / 2; i++)
            {
                NewOilSlick();
            }

            CurrentScreen = GameScreen.PLAYING;

        }

        void PlaceDogeCoin()
        {
            Random RandomNumGen = new Random(DateTime.Now.Millisecond);

            DogeCoin.UpperLeft = new Vector2(RandomNumGen.Next(0, GraphicsDevice.Viewport.Width - DogeCoin.GetWidth()), RandomNumGen.Next(0, GraphicsDevice.Viewport.Height - DogeCoin.GetHeight()));
        }

        void NewOilSlick()
        {
            OilSlick = new Sprite();

            Random RandomNumGen = new Random(DateTime.Now.Millisecond);

            OilSlick.SetTexture(OilTexture);
            OilSlick.UpperLeft = new Vector2(RandomNumGen.Next(0, GraphicsDevice.Viewport.Width - OilSlick.GetWidth()), RandomNumGen.Next(0, GraphicsDevice.Viewport.Height - OilSlick.GetHeight()));

            OilSlickList.AddLast(OilSlick);

            if (OilSlickList.Count() > MaxSlicks)
            {
                OilSlickList.RemoveFirst();
            }

        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (CurrentScreen == GameScreen.TITLE)
            {

                spriteBatch.DrawString(Font, "Press 1 or X to play with 1 player!", new Vector2(20, 20), Color.Black);
                spriteBatch.DrawString(Font, "Press 2 or Y to play with 2 players!", new Vector2(20, 40), Color.Black);

            }
            if (CurrentScreen == GameScreen.PLAYING)
            {

                PlayerCar1.Draw(spriteBatch);
                PlayerCar2.Draw(spriteBatch);

                PlayerCar1.MoveAndWrap(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                PlayerCar2.MoveAndWrap(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

                PlayerCar1.CheckBump(PlayerCar2, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                PlayerCar2.CheckBump(PlayerCar1, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

                DogeCoin.Draw(spriteBatch);

                foreach (Sprite Slick in OilSlickList)
                {
                    Slick.Draw(spriteBatch);

                    if (Slick.IsCollided(PlayerCar1))
                    {
                        PlayerCar1.SetSpeedAndDirection(0.25, PlayerCar1.GetDirectionAngle());
                    }
                    if (Slick.IsCollided(PlayerCar2))
                    {
                        PlayerCar2.SetSpeedAndDirection(0.25, PlayerCar2.GetDirectionAngle());
                    }
                }

                if (!HockeyPlayer)
                {
                    if (DogeCoin.IsCollided(PlayerCar1))
                    {

                        CoinSound.Play();

                        PlayerCar1.Score++;

                        PlaceDogeCoin();

                    }
                }

                if (DogeCoin.IsCollided(PlayerCar2))
                {

                    CoinSound.Play();

                    PlayerCar2.Score++;

                    PlaceDogeCoin();

                }

                if (HockeyPlayer)
                {

                    spriteBatch.DrawString(Font, PlayerCar2.Score.ToString(), new Vector2(385, 20), Color.Black);

                }
                else
                {
                    spriteBatch.DrawString(Font, PlayerCar1.Score.ToString(), new Vector2(20, 20), Color.Black);
                    spriteBatch.DrawString(Font, PlayerCar2.Score.ToString(), new Vector2(770, 20), Color.Black);
                }

            }
            if (CurrentScreen == GameScreen.GAMEOVER)
            {

                spriteBatch.DrawString(Font, "GAME OVER", new Vector2(20, 20), Color.Black);

                if (HockeyPlayer)
                {
                    spriteBatch.DrawString(Font, "You win!", new Vector2(20, 40), Color.Black);
                }
                if (AIPlayer)
                {
                    if (Winner == 1)
                    {
                        spriteBatch.DrawString(Font, "You win!", new Vector2(20, 40), Color.Black);
                    }
                    else
                    {
                        spriteBatch.DrawString(Font, "You lose!", new Vector2(20, 40), Color.Black);
                    }
                }
                if (MultiPlayer)
                {
                    spriteBatch.DrawString(Font, "Player "+Winner+" Wins!", new Vector2(20, 40), Color.Black);
                }

                spriteBatch.DrawString(Font, "Press ESCAPE or START to return to the menu!", new Vector2(20, 60), Color.Black);
                spriteBatch.DrawString(Font, "Or just use 1 or X to play again with 1 player!", new Vector2(20, 80), Color.Black);
                spriteBatch.DrawString(Font, "Or use 2 or Y for 2 players!", new Vector2(20, 100), Color.Black);

            }


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
