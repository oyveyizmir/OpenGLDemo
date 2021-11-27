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
    class Importer
    {
        public List<Point> s()
        {

            using (StreamReader reader = new StreamReader(@"D:\projects\OpenGLDemo\12140_Skull_v3_L2.obj"))
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
                return vertices;
            }
        }    
    }
}
