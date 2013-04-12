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
        float aspectRatio;

        Vector3 modelPosition = Vector3.Zero;
        float modelRotation = 0.0f;
        float modelRotationSpeed = 0.0f;

        Vector3 cameraLookAt = Vector3.Zero;
        float cameraZ = 1.0f;
        float cameraY = 1.0f;
        float cameraX = 1.0f;

        Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 50.0f);

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            landscape = Content.Load<Model>("Models\\landscape");
            dude = Content.Load<Model>("Models\\dude");
            animationtest = Content.Load<Model>("Models\\animationtest");
            sword = Content.Load<Model>("Models\\sword");

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix[] transforms = new Matrix[sword.Bones.Count];
            sword.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in landscape.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(modelRotation) *
                        Matrix.CreateTranslation(modelPosition);
                    effect.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
                    //effect.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);

                }
                mesh.Draw();
            }

            base.Draw(gameTime);
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
                    cameraLookAt.X += XDiff;
                }
                if (newMouse.Y != oldMouse.Y)
                {
                    YDiff = newMouse.Y - oldMouse.Y;
                    cameraLookAt.Y += YDiff;
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
                cameraLookAt.Z -= cameraZ;
                cameraPosition.Z -= cameraZ;
            }
            // move backward
            if (newState.IsKeyDown(Keys.S))
            {
                cameraLookAt.Z += cameraZ;
                cameraPosition.Z += cameraZ;
            }

            if (newState.IsKeyDown(Keys.A))
            {
                cameraPosition.X += cameraX;
                cameraLookAt.X += cameraX;
            }
            if (newState.IsKeyDown(Keys.D))
            {
                cameraPosition.X -= cameraX;
                cameraLookAt.X -= cameraX;
            }

            //    // If not down last update, key has just been pressed.
            //    if (!oldState.IsKeyDown(Keys.A))
            //    {
            //        modelRotationSpeed = modelRotationSpeed + 0.25f;
            //    }
            //}
            //else if (oldState.IsKeyDown(Keys.A))
            //{
            //    // Key was down last update, but not down now, so
            //    // it has just been released.
            //}


            // Update saved state.
            oldState = newState;
        }
    }
}
