using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit.GUI
{
    class WindowsRender
    {
        //private bool loaded;
        //private double step;

        Matrix4 matrixProjection, matrixModelView;
        float cameraRotation = 0f;
        float time = 0f;

        private GameWindow Game { get; set; }
        //private Camera Camera { get; set; }

        private Board Shelf { get; set; }

        public WindowsRender(GameWindow game)
        {
            this.Game = game;
            //step = 0.5;
            time = 0.0f;

            //Camera = new Camera();
            Shelf = new Board();

            Game.Load += Game_Load;
            Game.Resize += Game_Resize;
            Game.UpdateFrame += Game_UpdateFrame;
            Game.RenderFrame += Game_RenderFrame;
            Game.KeyPress += Game_KeyPress;

            game.Run(1 / 60.0);
        }

        private void Game_Load(object sender, EventArgs e)
        {
            Game.VSync = VSyncMode.On;

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);

            //loaded = true;
        }

        private void Game_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, Game.Width, Game.Height);

            matrixProjection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Game.Width / (float)Game.Height, 1f, 100f);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref matrixProjection);
        }

        private void Game_UpdateFrame(object sender, FrameEventArgs e)
        {
            if (Game.Keyboard[Key.Escape])
            {
                Game.Exit();
            }
        }

        private void Game_RenderFrame(object sender, FrameEventArgs e)
        {
            time += (float)e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.CornflowerBlue);

            #region Camera
            cameraRotation = (cameraRotation < 360f) ? (cameraRotation - 1f * (float)e.Time) : 0f;
            Matrix4.CreateRotationY(cameraRotation, out matrixModelView);
            matrixModelView *= Matrix4.LookAt(0f, 1.5f, -5f, 0f, 0f, 0f, 0f, 2f, 0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref matrixModelView);
            #endregion

            foreach (Volume c in Shelf.Cubes)
            {
                //GL.UniformMatrix4(uniform_mview, false, ref c.ModelViewProjectionMatrix);
                //GL.DrawElements(PrimitiveType.Triangles, c.indiceCount, DrawElementsType.UnsignedInt, indiceat * sizeof(uint));
                //indiceat += c.indiceCount;
            }

            /*if (!(Y > -0.2f && Y < 2f))
                step = -step;

            Y += (float) (step * e.Time);
            PosY();*/

            GL.Flush();
            Game.SwapBuffers();
        }

        private void Game_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Key.W:
                    //Camera.Move(0f, 0.1f, 0f);
                    break;
            }
        }

        /*private void PosY()
        {
            cube[1] = Y;
            cube[4] = Y;
            cube[13] = Y;
            cube[16] = Y;
        }*/
    }
}
