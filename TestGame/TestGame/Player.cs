using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace A_Taisen_Shiyou
{
    class Player
    {
        #region Init
        const float movementSpeed = 1.5f;
        const double atkSpeed = 0.15;
        private String name;
        public Vector2 location;
        private float direction = 0f;
        private int tWidth;
        private float layerDepth = 0f;
        private Color color;
        private Player other;
        public List<Rectangle> hitArea = new List<Rectangle>();

        Texture2D hitBox;

        Texture2D Stand;
        Texture2D WalkForward;
        Texture2D WalkBack;
        Texture2D Crouch;
        Texture2D Jump;
        Texture2D JumpBack;
        Texture2D AtkZ;
        Texture2D AtkX;
        Texture2D AtkC;
        Texture2D ArialZ;
        Texture2D ArialX;
        Texture2D ArialC;

        private Game1 g;
        ContentManager cm;

        enum State
        {
            WalkForward,
            WalkBack,
            Crouch,
            Jump,
            JumpBack,
            AtkZ,
            AtkX,
            AtkC,
            ArialZ,
            ArialX,
            ArialC,
            Stand
        }

        Keys jumpKey;
        Keys crouchKey;
        Keys backKey;
        Keys forwardKey;
        Keys atk1Key;
        Keys atk2Key;
        Keys atk3Key;

        private bool jumpKeyPressed = false;
        private bool atk1KeyPressed = false;
        private bool atk2KeyPressed = false;
        private bool atk3KeyPressed = false;

        private double standCount = 0;
        private double walkForwardCount = 0;
        private double walkBackCount = 0;
        private double crouchCount = 0;
        private double jumpCount = 0;
        private double atkZCount = 0;
        private double atkXCount = 0;
        private double atkCCount = 0;
        private double arialZCount = 0;
        private double arialXCount = 0;
        private double arialCCount = 0;

        private State drawState;

        private bool attacking = false;
        private bool airborne = false;
        private bool flipHorizontal;
        private bool drawHitArea = false;
        private bool spacePressed = false;
        #endregion

        public Player(Game1 g, ContentManager cm, String name, Vector2 startLocation, Keys forwardKey, Color color, bool flip, int tWidth)
        {
            this.g = g;
            this.cm = cm;
            this.location = startLocation;
            this.name = name;
            this.flipHorizontal = flip;
            this.color = color;
            this.tWidth = tWidth;

            hitBox = this.cm.Load<Texture2D>("Hitbox");

            Stand = this.cm.Load<Texture2D>(this.name + "/Stand");
            WalkForward = this.cm.Load<Texture2D>(this.name + "/WalkForward");
            WalkBack = this.cm.Load<Texture2D>(this.name + "/WalkBack");
            Crouch = this.cm.Load<Texture2D>(this.name + "/Crouch");
            Jump = this.cm.Load<Texture2D>(this.name + "/Jump");
            JumpBack = this.cm.Load<Texture2D>(this.name + "/JumpBack");
            AtkZ = this.cm.Load<Texture2D>(this.name + "/AtkZ");
            AtkX = this.cm.Load<Texture2D>(this.name + "/AtkX");
            AtkC = this.cm.Load<Texture2D>(this.name + "/AtkC");
            ArialZ = this.cm.Load<Texture2D>(this.name + "/ArialZ");
            ArialX = this.cm.Load<Texture2D>(this.name + "/ArialX");
            ArialC = this.cm.Load<Texture2D>(this.name + "/ArialC");

            if (forwardKey.Equals(Keys.Right))
            {
                this.jumpKey = Keys.Up;
                this.crouchKey = Keys.Down;
                this.backKey = Keys.Left;
                this.forwardKey = Keys.Right;
                this.atk1Key = Keys.Z;
                this.atk2Key = Keys.X;
                this.atk3Key = Keys.C;
            }
            else if (forwardKey.Equals(Keys.D))
            {
                this.jumpKey = Keys.W;
                this.crouchKey = Keys.S;
                this.backKey = Keys.A;
                this.forwardKey = Keys.D;
                this.atk1Key = Keys.J;
                this.atk2Key = Keys.K;
                this.atk3Key = Keys.L;
            }
        }

        public void setEnemy(Player player)
        {
            this.other = player;
        }

        internal void Draw(SpriteBatch sb)
        {
            if (drawState.Equals(State.WalkForward))
            {
                if (walkForwardCount > (WalkForward.Width / tWidth))
                {
                    walkForwardCount = 0;
                }
                Rectangle sourceRect = new Rectangle((tWidth * (int)Math.Floor(walkForwardCount)), 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(WalkForward, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(WalkForward);
                setHitArea(colors, WalkForward, walkForwardCount, sourceRect, sb);
            }
            else if (drawState.Equals(State.WalkBack))
            {
                if (walkBackCount > (WalkBack.Width / tWidth))
                {
                    walkBackCount = 0;
                }
                Rectangle sourceRect = new Rectangle((tWidth * (int)Math.Floor(walkBackCount)), 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(WalkBack, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(WalkBack);
                setHitArea(colors, WalkBack, walkBackCount, sourceRect, sb);    
            }
            else if (drawState.Equals(State.Crouch))
            {
                if (crouchCount > (Crouch.Width / tWidth))
                {
                    crouchCount = 0;
                }
                Rectangle sourceRect = new Rectangle((tWidth * (int)Math.Floor(crouchCount)), 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(Crouch, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(Crouch);
                setHitArea(colors, Crouch, crouchCount, sourceRect, sb);    
            }
            else if (drawState.Equals(State.Jump))
            {
                Rectangle sourceRect = new Rectangle(0, 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(Jump, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(Jump);
                setHitArea(colors, Jump, 0, sourceRect, sb);

                if (location.Y >= 180)
                {
                    jumpCount = 0;
                    airborne = false;

                    Rectangle sourceRectEnd = new Rectangle(tWidth, 0, tWidth, 115);
                    sb.Draw(Stand, location, sourceRectEnd, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                    //Draw the hitArea
                    Color[,] colors2 = TextureTo2DArray(Stand);
                    setHitArea(colors2, Stand, standCount, sourceRectEnd, sb);
                }
            }
            else if (drawState.Equals(State.JumpBack))
            {
                Rectangle sourceRect = new Rectangle(0, 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(JumpBack, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(JumpBack);
                setHitArea(colors, JumpBack, 0, sourceRect, sb);

                if (location.Y >= 180)
                {
                    jumpCount = 0;
                    airborne = false;

                    Rectangle sourceRectEnd = new Rectangle(tWidth, 0, tWidth, 115);
                    sb.Draw(Stand, location, sourceRectEnd, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                    //Draw the hitArea
                    Color[,] colors2 = TextureTo2DArray(Stand);
                    setHitArea(colors2, Stand, standCount, sourceRectEnd, sb);
                }
            }
            else if (drawState.Equals(State.AtkZ))
            {
                Rectangle sourceRect = new Rectangle((tWidth * (int)Math.Floor(atkZCount)), 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(AtkZ, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(AtkZ);
                setHitArea(colors, AtkZ, atkZCount, sourceRect, sb);

                if (atkZCount > (AtkZ.Width / tWidth) - 0.1)
                {
                    atkZCount = 0;
                    attacking = false;
                }
            }
            else if (drawState.Equals(State.AtkX))
            {
                Rectangle sourceRect = new Rectangle((tWidth * (int)Math.Floor(atkXCount)), 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(AtkX, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(AtkX);
                setHitArea(colors, AtkX, atkXCount, sourceRect, sb);

                if (atkXCount > (AtkX.Width / tWidth) - 0.1)
                {
                    atkXCount = 0;
                    attacking = false;
                }
            }
            else if (drawState.Equals(State.AtkC))
            {
                Rectangle sourceRect = new Rectangle((tWidth * (int)Math.Floor(atkCCount)), 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(AtkC, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(AtkC);
                setHitArea(colors, AtkC, atkCCount, sourceRect, sb);

                if (atkCCount > (AtkC.Width / tWidth) - 0.1)
                {
                    atkCCount = 0;
                    attacking = false;
                }
            }
            else if (drawState.Equals(State.ArialZ))
            {
                Rectangle sourceRect = new Rectangle((tWidth * (int)Math.Floor(arialZCount)), 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(ArialZ, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                if (arialZCount > (ArialZ.Width / tWidth) - 0.1)
                {
                    arialZCount = 0;
                    drawState = State.Jump;
                }
                if (location.Y >= 180)
                {
                    jumpCount = 0;
                    arialZCount = 0;
                    airborne = false;

                    Rectangle sourceRectEnd = new Rectangle(tWidth, 0, tWidth, 115);
                    sb.Draw(Stand, location, sourceRectEnd, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                    //Draw the hitArea
                    Color[,] colors2 = TextureTo2DArray(Stand);
                    setHitArea(colors2, Stand, standCount, sourceRectEnd, sb);
                }

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(ArialZ);
                setHitArea(colors, ArialZ, arialZCount, sourceRect, sb);
            }
            else if (drawState.Equals(State.ArialX))
            {
                Rectangle sourceRect = new Rectangle((tWidth * (int)Math.Floor(arialXCount)), 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(ArialX, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                if (arialXCount > (ArialX.Width / tWidth) - 0.1)
                {
                    arialXCount = 0;
                    drawState = State.Jump;
                }
                if (location.Y >= 180)
                {
                    jumpCount = 0;
                    arialXCount = 0;
                    airborne = false;

                    Rectangle sourceRectEnd = new Rectangle(tWidth, 0, tWidth, 115);
                    sb.Draw(Stand, location, sourceRectEnd, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                    //Draw the hitArea
                    Color[,] colors2 = TextureTo2DArray(Stand);
                    setHitArea(colors2, Stand, standCount, sourceRectEnd, sb);
                }

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(ArialX);
                setHitArea(colors, ArialX, arialXCount, sourceRect, sb);
            }
            else if (drawState.Equals(State.ArialC))
            {
                Rectangle sourceRect = new Rectangle((tWidth * (int)Math.Floor(arialCCount)), 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(ArialC, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                if (arialCCount > (ArialC.Width / tWidth) - 0.1)
                {
                    arialCCount = 0;
                    drawState = State.Jump;
                }
                if (location.Y >= 180)
                {
                    jumpCount = 0;
                    arialCCount = 0;
                    airborne = false;

                    Rectangle sourceRectEnd = new Rectangle(tWidth, 0, tWidth, 115);
                    sb.Draw(Stand, location, sourceRectEnd, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                    //Draw the hitArea
                    Color[,] colors2 = TextureTo2DArray(Stand);
                    setHitArea(colors2, Stand, standCount, sourceRectEnd, sb);
                }

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(ArialC);
                setHitArea(colors, ArialC, arialCCount, sourceRect, sb);
            }
            else if (drawState.Equals(State.Stand))
            {
                if (standCount > (Stand.Width / tWidth))
                {
                    standCount = 0;
                }
                Rectangle sourceRect = new Rectangle((tWidth * (int)Math.Floor(standCount)), 0, tWidth, 115);
                //Uncomment for view on Texture size and alignment (if rectangle moves, fix sprite)
                //Rectangle hitRect = new Rectangle((int)location.X - (sourceRect.Width / 2), (int)location.Y - (sourceRect.Height / 2), tWidth, 115);
                //sb.Draw(hitBox, hitRect, Color.Red);
                sb.Draw(Stand, location, sourceRect, color, direction, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 1, checkSpriteEffects(), layerDepth);

                //Draw the hitArea
                Color[,] colors = TextureTo2DArray(Stand);
                setHitArea(colors, Stand, standCount, sourceRect, sb);                
            }
        }

        public void Update(KeyboardState keyboardState)
        {
            checkHorizontalFlip();

            if (attacking == false && airborne == false)
            {
                if (keyboardState.IsKeyDown(jumpKey) && jumpKeyPressed == false)
                {
                    jumpCount += 1;
                    location.Y -= 1;
                    airborne = true;
                    jumpKeyPressed = true;
                    drawState = State.Jump;
                }
                else if (keyboardState.IsKeyDown(atk3Key) && atk3KeyPressed == false)
                {
                    atkCCount += atkSpeed;
                    attacking = true;
                    atk3KeyPressed = true;
                    drawState = State.AtkC;
                }
                else if (keyboardState.IsKeyDown(atk2Key) && atk2KeyPressed == false)
                {
                    atkXCount += atkSpeed;
                    attacking = true;
                    atk2KeyPressed = true;
                    drawState = State.AtkX;
                }
                else if (keyboardState.IsKeyDown(atk1Key) && atk1KeyPressed == false)
                {
                    atkZCount += atkSpeed;
                    attacking = true;
                    atk1KeyPressed = true;
                    drawState = State.AtkZ;
                }
                else if (keyboardState.IsKeyDown(forwardKey))
                {
                    walkForwardCount += 0.1;
                    calcHorizontalMovement(true);
                    drawState = State.WalkForward;
                }
                else if (keyboardState.IsKeyDown(backKey))
                {
                    walkBackCount += 0.1;
                    calcHorizontalMovement(false);
                    drawState = State.WalkBack;
                }
                else if (keyboardState.IsKeyDown(crouchKey))
                {
                    crouchCount += 0.02;
                    drawState = State.Crouch;
                }
                else
                {
                    standCount += 0.1;
                    drawState = State.Stand;
                }
            }
            else if (airborne == true)
            {
                if (drawState.Equals(State.Jump))
                {
                    jumpCount += 0.1;

                    calcJump();

                    if (keyboardState.IsKeyDown(atk3Key) && atk3KeyPressed == false)
                    {
                        atk3KeyPressed = true;
                        drawState = State.ArialC;
                    }
                    else if (keyboardState.IsKeyDown(atk2Key) && atk2KeyPressed == false)
                    {
                        atk2KeyPressed = true;
                        drawState = State.ArialX;
                    }
                    else if (keyboardState.IsKeyDown(atk1Key) && atk1KeyPressed == false)
                    {
                        atk1KeyPressed = true;
                        drawState = State.ArialZ;
                    }
                    else if (keyboardState.IsKeyDown(forwardKey))
                    {
                        calcHorizontalMovement(true);
                    }
                    else if (keyboardState.IsKeyDown(backKey))
                    {
                        drawState = State.JumpBack;
                    }
                }
                else if (drawState.Equals(State.JumpBack))
                {
                    jumpCount += 0.1;

                    calcJump();

                    if (keyboardState.IsKeyDown(atk3Key) && atk3KeyPressed == false)
                    {
                        atk3KeyPressed = true;
                        drawState = State.ArialC;
                    }
                    else if (keyboardState.IsKeyDown(atk2Key) && atk2KeyPressed == false)
                    {
                        atk2KeyPressed = true;
                        drawState = State.ArialX;
                    }
                    else if (keyboardState.IsKeyDown(atk1Key) && atk1KeyPressed == false)
                    {
                        atk1KeyPressed = true;
                        drawState = State.ArialZ;
                    }
                    else if (keyboardState.IsKeyDown(backKey))
                    {
                        calcHorizontalMovement(false);
                    }
                    else
                    {
                        drawState = State.Jump;
                    }
                }
                else if (drawState.Equals(State.ArialZ))
                {
                    arialZCount += atkSpeed;
                    jumpCount += 0.1;

                    calcJump();

                    if (keyboardState.IsKeyDown(forwardKey))
                    {
                        calcHorizontalMovement(true);
                    }
                    else if (keyboardState.IsKeyDown(backKey))
                    {
                        calcHorizontalMovement(false);
                    }
                }
                else if (drawState.Equals(State.ArialX))
                {
                    arialXCount += atkSpeed;
                    jumpCount += 0.1;

                    calcJump();

                    if (keyboardState.IsKeyDown(forwardKey))
                    {
                        calcHorizontalMovement(true);
                    }
                    else if (keyboardState.IsKeyDown(backKey))
                    {
                        calcHorizontalMovement(false);
                    }
                }
                else if (drawState.Equals(State.ArialC))
                {
                    arialCCount += atkSpeed;
                    jumpCount += 0.1;

                    calcJump();

                    if (keyboardState.IsKeyDown(forwardKey))
                    {
                        calcHorizontalMovement(true);
                    }
                    else if (keyboardState.IsKeyDown(backKey))
                    {
                        calcHorizontalMovement(false);
                    }
                }
            }
            else if (attacking == true)
            {
                if (drawState.Equals(State.AtkZ))
                {
                    atkZCount += atkSpeed;
                }
                else if (drawState.Equals(State.AtkX))
                {
                    atkXCount += atkSpeed;
                }
                else if (drawState.Equals(State.AtkC))
                {
                    atkCCount += atkSpeed;
                }
            }

            if (keyboardState.IsKeyUp(jumpKey) && jumpKeyPressed == true)
            {
                jumpKeyPressed = false;
            }
            if (keyboardState.IsKeyUp(atk1Key) && atk1KeyPressed == true)
            {
                atk1KeyPressed = false;
            }
            if (keyboardState.IsKeyUp(atk2Key) && atk2KeyPressed == true)
            {
                atk2KeyPressed = false;
            }
            if (keyboardState.IsKeyUp(atk3Key) && atk3KeyPressed == true)
            {
                atk3KeyPressed = false;
            }

            if (keyboardState.IsKeyDown(Keys.Space) && drawHitArea == false && spacePressed == false)
            {
                drawHitArea = true;
                spacePressed = true;
            }
            else if (keyboardState.IsKeyDown(Keys.Space) && drawHitArea == true && spacePressed == false)
            {
                drawHitArea = false;
                spacePressed = true;
            }
            if (keyboardState.IsKeyUp(Keys.Space) && spacePressed == true)
            {
                spacePressed = false;
            }

            //if (attacking == true)
            //{
            //    g.DetectCollision(this.hitArea, other.hitArea);
            //}
        }

        private void calcJump()
        {
            //y = 0.15x^2 - 1.5x + 5.5
            int value = (int)(Math.Pow((0.15 * jumpCount), 2) - (1.5 * jumpCount) + 5.5);

            if (location.Y - value > 180)
            {
                location.Y = 180;
            }
            else
            {
                location.Y -= value;
            }
        }

        private void calcHorizontalMovement(bool forward)
        {
            if (flipHorizontal == false && forward == true)
            {
                if (location.X < 525)
                {
                    location.X += movementSpeed;
                }
            }
            else if (flipHorizontal == false && forward == false)
            {
                if (location.X > 45)
                {
                    location.X -= movementSpeed;
                }
            }
            else if (flipHorizontal == true && forward == true)
            {
                if (location.X > 45)
                {
                    location.X -= movementSpeed;
                }
            }
            else if (flipHorizontal == true && forward == false)
            {
                if (location.X < 525)
                {
                    location.X += movementSpeed;
                }
            }
        }

        private void checkHorizontalFlip()
        {
            if (location.X < other.location.X)
            {
                flipHorizontal = false;
                if (forwardKey == Keys.Right || forwardKey == Keys.Left)
                {
                    forwardKey = Keys.Right;
                    backKey = Keys.Left;
                }
                else if (forwardKey == Keys.D || forwardKey == Keys.A)
                {
                    forwardKey = Keys.D;
                    backKey = Keys.A;
                }
            }
            else
            {
                flipHorizontal = true;
                if (forwardKey == Keys.Right || forwardKey == Keys.Left)
                {
                    forwardKey = Keys.Left;
                    backKey = Keys.Right;
                }
                else if (forwardKey == Keys.D || forwardKey == Keys.A)
                {
                    forwardKey = Keys.A;
                    backKey = Keys.D;
                }
            }
        }

        private SpriteEffects checkSpriteEffects()
        {
            if (flipHorizontal == true)
            {
                return SpriteEffects.FlipHorizontally;
            }
            else
            {
                return SpriteEffects.None;
            }
        }

        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }

        private void setHitArea(Color[,] colors, Texture2D texture, double counter, Rectangle sourceRect, SpriteBatch sb)
        {
            hitArea.Clear();

            //for (int y = 0; y < 115; y++)
            //{
            //    for (int x = (tWidth * (int)Math.Floor(counter)); x < tWidth + (tWidth * (int)Math.Floor(counter)); x++)
            //    {
            //        if (colors[x, y].Equals(Color.Transparent) == false)
            //        {
            //            Rectangle r;
            //            if (flipHorizontal == false)
            //            {
            //                r = new Rectangle(((int)location.X - (sourceRect.Width / 2)) + x - (tWidth * (int)Math.Floor(counter)), ((int)location.Y - (sourceRect.Height / 2)) + y, 1, 1);
            //            }
            //            else
            //            {
            //                r = new Rectangle(((int)location.X + (sourceRect.Width / 2)) - x + (tWidth * (int)Math.Floor(counter)) - 1, ((int)location.Y - (sourceRect.Height / 2)) + y, 1, 1);
            //            }

            //            if (drawHitArea == true)
            //            {
            //                sb.Draw(hitBox, r, new Color(0, 0, 255, 225));
            //            }
            //            hitArea.Add(r);
            //        }
            //    }
            //}


        }
    }
}