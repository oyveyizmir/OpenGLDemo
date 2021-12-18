using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpGL;
using System.Globalization;

namespace OpenGLDemo
{
    class Model
    {
        public List<Point> Vertices { get; private set; }

        public void Import(string fileName)
        {
            Load(fileName);
            CenterModel();
        }

        void Load(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                var vertices = new List<Point>();
                string a = null;
                while ((a = reader.ReadLine()) != null)
                {
                    string[] lines = a.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length == 0)
                        continue;
                    switch (lines[0])
                    {
                        case "v":
                            if (lines.Length != 4)
                                throw new Exception("wrong row format" + a);
                            Point point = new Point(float.Parse(lines[1], CultureInfo.InvariantCulture),
                                float.Parse(lines[2], CultureInfo.InvariantCulture),
                                float.Parse(lines[3], CultureInfo.InvariantCulture));
                            vertices.Add(point);
                            break;
                        case "#":
                        case "o":
                        case "f":
                        case "vt":
                        case "vn":
                        case "usemtl":
                        case "s":
                        case "l":
                        case "mtllib":
                        case "g":
                            break;
                        default:
                            throw new Exception("unknown row type " + a);
                    }
                }
                Vertices = vertices;
            }
        }

        void CenterModel()
        {
            Point min = new Point(float.MaxValue, float.MaxValue, float.MaxValue);
            Point max = new Point(float.MinValue, float.MinValue, float.MinValue);
            Point center = new Point(0, 0, 0);

            foreach (var point in Vertices)
            {
                min.X = Math.Min(min.X, point.X);
                min.Y = Math.Min(min.Y, point.Y);
                min.Z = Math.Min(min.Z, point.Z);

                max.X = Math.Max(max.X, point.X);
                max.Y = Math.Max(max.Y, point.Y);
                max.Z = Math.Max(max.Z, point.Z);
            }

            center.X = (min.X + max.X) / 2;
            center.Y = (min.Y + max.Y) / 2;
            center.Z = (min.Z + max.Z) / 2;

            foreach (var point in Vertices)
            {
                point.X -= center.X;
                point.Y -= center.Y;
                point.Z -= center.Z;
            }
        }
    }
}
