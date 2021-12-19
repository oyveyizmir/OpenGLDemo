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
        public delegate char Keys(object sender, KeyEventArgs e);

        Model model;

        
        const double zNear = 1;
        const double zFar = 1000;
        const double sizeCorrectionFactor = 2;

        const double stepAlpha = 1 * Math.PI / 180;
        const double stepBeta = 1 * Math.PI / 180;
        const double stepR = 1;
        const double stepFov = 1 * Math.PI / 180;
        const double defaultFov = 30 * Math.PI / 180;
        const double minFov = 1 * Math.PI / 180;
        const double maxFov = 80 * Math.PI / 180;

        const double maxBeta = 89.5 * Math.PI / 180;

        double fov = defaultFov;
        double cameraR;
        double cameraAlpha;
        double cameraBeta;

        bool axesVisible = true;

        double[] maxExtension;

        enum RotationAxis
        {
            Z = 0,
            X = 1,
            Y = 2
        }

        RotationAxis rotationAxis;

        public MainWindow()
        {
            InitializeComponent();

            model = new Model();
            model.Import(@"D:\projects\OpenGLDemo\12140_Skull_v3_L2.obj");

            maxExtension = new double[3]
                {
                    Math.Max(model.Extension.Z, model.Extension.Y),
                    Math.Max(model.Extension.X, model.Extension.Z),
                    Math.Max(model.Extension.Y, model.Extension.X)
                };

            InitRotation(RotationAxis.Z);
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;
            
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.ClearColor(0.3f, 0.3f, 0.3f, 0.3f);
        }
        
        
        public void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;

            gl.MatrixMode(MatrixMode.Projection);

            PositionCamera(gl);

            gl.MatrixMode(MatrixMode.Modelview);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            if (axesVisible)            
                DrawLineAxes(gl);

            DrawModel(gl);
        }

        void PositionCamera(OpenGL gl)
        {
            float[] coord = new float[3];

            coord[(int)rotationAxis] = (float)(cameraR * Math.Cos(cameraAlpha) * Math.Cos(cameraBeta));
            coord[((int)rotationAxis + 1) % 3] = (float)(cameraR * Math.Sin(cameraAlpha) * Math.Cos(cameraBeta));
            coord[((int)rotationAxis + 2) % 3] = (float)(cameraR * Math.Sin(cameraBeta));

            gl.LoadIdentity();
            double nearSize = zNear * Math.Tan(fov);
            gl.Frustum(-nearSize, nearSize, -nearSize, nearSize, zNear, zFar);
            gl.LookAt(coord[0], coord[1], coord[2], 0, 0, 0,
                rotationAxis == RotationAxis.X ? 1 : 0,
                rotationAxis == RotationAxis.Y ? 1 : 0,
                rotationAxis == RotationAxis.Z ? 1 : 0);
        }

        void DrawModel(OpenGL gl)
        {
            gl.Begin(BeginMode.Points);

            gl.Color(0, 1F, 1F);
            foreach (var point in model.Vertices)
                gl.Vertex(point.X, point.Y, point.Z);

            gl.End();
        }

        void DrawLineAxes(OpenGL gl)
        {
            gl.Begin(BeginMode.Lines);
            gl.LineWidth(5f);

            float length = 1.5f * Math.Max(model.Extension.X, Math.Max(model.Extension.Y, model.Extension.Z));

            gl.Color(1f, 0, 0);
            gl.Vertex(0, 0, 0);
            gl.Vertex(length, 0, 0);

            gl.Color(0, 1f, 0);
            gl.Vertex(0, 0, 0);
            gl.Vertex(0, length, 0);

            gl.Color(0, 0, 1f);
            gl.Vertex(0, 0, 0);
            gl.Vertex(0, 0, length);

            gl.End();
        }

        // Draws axes as dotted lines
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

        void InitRotation(RotationAxis axis)
        {
            rotationAxis = axis;
            fov = defaultFov;
            cameraR = maxExtension[(int)axis] * sizeCorrectionFactor / Math.Tan(fov);
            cameraAlpha = 0;
            cameraBeta = 0;
        }

        public void Key_Capture(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Up:
                    if (cameraBeta + stepBeta <= maxBeta)
                        cameraBeta += stepBeta;
                    break;
                case Key.Down:
                    if (cameraBeta - stepBeta >= -maxBeta)
                        cameraBeta -= stepBeta;
                    break;
                case Key.Left:
                    cameraAlpha -= stepAlpha;
                    break;
                case Key.Right:
                    cameraAlpha += stepAlpha;
                    break;
                case Key.Add:
                    cameraR -= stepR;
                    break;
                case Key.Subtract:
                    cameraR += stepR;
                    break;
                case Key.X:
                    InitRotation(RotationAxis.X);
                    break;
                case Key.Y:
                    InitRotation(RotationAxis.Y);
                    break;
                case Key.Z:
                    InitRotation(RotationAxis.Z);
                    break;
                case Key.A:
                    axesVisible = !axesVisible;
                    break;
                case Key.PageUp:
                    if (fov - stepFov >= minFov)
                        fov -= stepFov;
                    break;
                case Key.PageDown:
                    if (fov + stepFov <= maxFov)
                        fov += stepFov;
                    break;
            }
        }

        public void Mouse_Capture(object sender, MouseEventArgs e)
        {
        }

        private void Mouse_Capture(object sender, ManipulationDeltaEventArgs e)
        {
        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {
            var gl = args.OpenGL;
            double winWidth = Width;
            double winHeight = Height;
        }


        private void OpenGLControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
