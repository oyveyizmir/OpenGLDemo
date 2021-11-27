using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.WPF;
using SharpGL.Serialization;
using SharpGL.Controls;
using SharpGL.OpenGLAttributes;
using SharpGL.RenderContextProviders;
using SharpGL.Enumerations;
using System.IO;
using System.Diagnostics;

namespace OpenGLDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double X = 0, Z = 0;//-60;
        public float AngleX, AngleY, AngleZ;

        public delegate char Keys(object sender, KeyEventArgs e);

        List<Point> points;

        bool preserveRotation = true;

        public MainWindow()
        {
            InitializeComponent();
            Importer imp = new Importer();
            points = imp.s();
        }


        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;
            
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.ClearColor(0.3f, 0.3f, 0.3f, 0.3f);

            gl.MatrixMode(MatrixMode.Projection);
            
            gl.LoadIdentity();

            gl.Frustum(-20, 20, -20, 20, 20, 100);
            gl.LookAt(60, 0, 0, 0, 0, 0, 0, 1, 0.5);

            //gl.Ortho(-10,10, -10,10, -10,10);
        }
        
        
        public void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.MatrixMode(MatrixMode.Modelview);
            if (!preserveRotation)
                gl.PushMatrix();

            gl.Translate(0, 0, Z);
            gl.Rotate(AngleX, AngleY, AngleZ);

            SharpGL.SceneGraph.Matrix m = gl.GetModelViewMatrix();

            if (preserveRotation)
            {
                AngleX = 0;
                AngleY = 0;
                AngleZ = 0;
                Z = 0;
            }
            //gl.Translate(0, 0, -9);

            DrawLineAxes(gl);

            gl.Begin(BeginMode.Points);
            
            gl.Color(0, 1F, 1F);
            foreach (var point in points)
                gl.Vertex(point.X, point.Y, point.Z);
            
            gl.End();

            if (!preserveRotation)
                gl.PopMatrix();
        }

        void DrawLineAxes(OpenGL gl)
        {
            gl.Begin(BeginMode.Lines);
            gl.LineWidth(5f);

            gl.Color(1f, 0, 0);
            gl.Vertex(0, 0, 0);
            gl.Vertex(35, 0, 0);

            gl.Color(0, 1f, 0);
            gl.Vertex(0, 0, 0);
            gl.Vertex(0, 35, 0);

            gl.Color(0, 0, 1f);
            gl.Vertex(0, 0, 0);
            gl.Vertex(0, 0, 35);

            gl.End();
        }

        void DrawDotAxes(OpenGL gl)
        {
            gl.Color(1f, 1f, 1f);
            gl.Vertex(0, 0, 0);

            for (double i = 0.5; i < 30; i += 0.5)
            {
                gl.Color(1f, 0, 0);
                gl.Vertex(i, 0, 0);
                gl.Color(0, 1f, 0);
                gl.Vertex(0, i, 0);
                gl.Color(0, 0, 1f);
                gl.Vertex(0, 0, i);
            }
        }

        public void Key_Capture(object sender, KeyEventArgs e)
        {

            float x = 1f, y = 1f, z = 1f;
            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    AngleX -= x;
                    break;
                case Key.S:
                case Key.Down:
                    AngleX += x;
                    break;
                case Key.A:
                case Key.Left:
                    AngleY -= y;
                    break;
                case Key.D:
                case Key.Right:
                    AngleY += y;
                    break;
                case Key.PageUp:
                    AngleZ -= z;
                    break;
                case Key.PageDown:
                    AngleZ += z;
                    break;
                case Key.Add:
                    Z += 1;
                    break;
                case Key.Subtract:
                    Z -= 1;
                    break;
            }
        }

        public void Mouse_Capture(object sender, MouseEventArgs e)
        {
            /*if (this.IsActive)
            {
                Cursor = Cursors.None;
                double mouseX = 0;
                double mouseY = 0;
                double newMouseX;
                double newMouseY;
                var pos = e.GetPosition(this);
                newMouseX = pos.X;
                newMouseY = pos.Y;
                mouseX -= newMouseX;
                mouseY -= newMouseY;
                AngleX = (float)Math.Atan(mouseX / mouseY);
                AngleZ = (float)Math.Acos(mouseY / Math.Sqrt(Math.Pow(mouseX, 2) + Math.Pow(mouseY, 2)));
            }
            else
                Cursor = Cursors.Arrow;*/
        }

        private void Mouse_Capture(object sender, ManipulationDeltaEventArgs e)
        {

        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;
            double winWidth = Width;
            double winHeight = Height;
            //gl.Ortho(coef, coef, coef, coef, 0, 100);
            
        }


        private void OpenGLControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
