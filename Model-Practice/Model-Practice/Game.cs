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

namespace Model_Practice
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldState;
        MouseState oldMouse;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }



        Model sword;
        Model dude;
        Model landscape;
        Model animationtest;
        Model swordpink;
        float aspectRatio;

        Vector3 modelPosition = Vector3.Zero;
        float modelRotation = 0.0f;
        float modelRotationSpeed = 0.0f;

        static Vector3 cameraLookAtStart = Vector3.Zero;
        static Vector3 cameraPositionStart = new Vector3(0.0f, 0.0f, 50.0f);
        static float cameraSpeed = 0.5f;
        Vector3 cameraLookAt = cameraLookAtStart;
        Vector3 cameraPosition = cameraPositionStart;

        static float mouseSpeed = 0.1f;

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            landscape = Content.Load<Model>("Models\\landscape");
            dude = Content.Load<Model>("Models\\dude");
            animationtest = Content.Load<Model>("Models\\animationtest");
            sword = Content.Load<Model>("Models\\sword");
            swordpink = Content.Load<Model>("Models\\swordpink");

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            UpdateMouse();
            UpdateInput();
            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(modelRotationSpeed);

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);

            Matrix[] transforms = new Matrix[sword.Bones.Count];
            sword.CopyAbsoluteBoneTransformsTo(transforms);

            float spacing = 100.0f;
            int count = 4;
            for (int Y = 1; Y <= count; Y++)
            {
                for (int Z = 1; Z <= count; Z++)
                {
                    for (int X = 1; X <= count; X++)
                    {
                        Vector3 v = new Vector3() { X = X * spacing, Y = Y * spacing, Z = Z * spacing };
                        foreach (ModelMesh mesh in swordpink.Meshes)
                        {
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                effect.EnableDefaultLighting();
                                effect.World = transforms[mesh.ParentBone.Index] *
                                    Matrix.CreateRotationY(modelRotation) *
                                    Matrix.CreateTranslation(modelPosition + v);
                                effect.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
                                effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                                    MathHelper.ToRadians(45.0f), aspectRatio,
                                    1.0f, 10000.0f);
                            }
                            mesh.Draw();
                        }
                    }
                }
            }           
            DrawText("Camera Position " +
                "(X:" + cameraPosition.X.ToString() + 
                ",Y:" + cameraPosition.Y.ToString() +
                ",Z:" + cameraPosition.Z.ToString() + ") -- Camera Look At " +
                "(X:" + cameraLookAt.X.ToString() + 
                ",Y:" + cameraLookAt.Y.ToString() + 
                ",Z:" + cameraLookAt.Z.ToString() + ")");

            base.Draw(gameTime);
        }
        private void DrawText(string text)
        {
            spriteBatch.Begin();
            SpriteFont font = Content.Load<SpriteFont>("Verdana");
            spriteBatch.DrawString(font,text,new Vector2(0.0f,0.0f),Color.White);
            spriteBatch.End();
        }
        private void UpdateMouse()
        {
            MouseState newMouse = Mouse.GetState();

            float XDiff = 0.0f;
            float YDiff = 0.0f;
            if (newMouse.RightButton == ButtonState.Pressed)
            {
                if (newMouse.X != oldMouse.X)
                {
                    XDiff = newMouse.X - oldMouse.X;
                    cameraLookAt.X += XDiff * mouseSpeed;
                }
                if (newMouse.Y != oldMouse.Y)
                {
                    YDiff = newMouse.Y - oldMouse.Y;
                    cameraLookAt.Y += YDiff * mouseSpeed;
                }
            }

            oldMouse = newMouse;
        }
        private void UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Q))
            {
                modelRotationSpeed += 0.01f;
            }
            if (newState.IsKeyDown(Keys.E))
            {
                modelRotationSpeed -= 0.01f;
            }

            // move forward
            if (newState.IsKeyDown(Keys.W))
            {
                //cameraLookAt.Z -= cameraSpeed;
                //cameraPosition.Z -= cameraSpeed;
                Vector3 blah = cameraPosition - cameraLookAt;
                float slope = blah.X / blah.Z;
                cameraPosition += blah;
                cameraLookAt += blah;
            }
            // move backward
            if (newState.IsKeyDown(Keys.S))
            {
                //cameraLookAt.Z += cameraSpeed;
                //cameraPosition.Z += cameraSpeed;
                Vector3 blah = cameraPosition - cameraLookAt;
                cameraPosition -= blah;
                cameraLookAt -= blah;
            }

            if (newState.IsKeyDown(Keys.A))
            {
                cameraPosition.X += cameraSpeed;
                cameraLookAt.X += cameraSpeed;
            }
            if (newState.IsKeyDown(Keys.D))
            {
                cameraPosition.X -= cameraSpeed;
                cameraLookAt.X -= cameraSpeed;
            }

            if (newState.IsKeyDown(Keys.Z))
            {
                cameraPosition.Y += cameraSpeed;
                cameraLookAt.Y += cameraSpeed;
            }
            if (newState.IsKeyDown(Keys.X))
            {
                cameraPosition.Y -= cameraSpeed;
                cameraLookAt.Y -= cameraSpeed;
            }
            if (newState.IsKeyDown(Keys.Enter))
            {
                cameraLookAt = cameraLookAtStart;
                cameraPosition = cameraPositionStart;
            }
            // Update saved state.
            oldState = newState;
        }
    }
}
