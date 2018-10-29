using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
//using MathNet.Numerics.LinearAlgebra;
//using MathNet.Numerics.LinearAlgebra.Double;

namespace VirtualCamera
{

    

    public partial class Form1 : Form
    {
        public const float DEG2RAD = 0.0174533f;

        private Font fnt = new Font("Arial", 10);
        private Box[] scene = new Box[2];
        private Matrix4x4 cameraToWorld = new Matrix4x4(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, -1.0f, 0.0f,
            180.0f, 190.0f, -10.0f, 1.0f);

        private Matrix4x4 worldToCamera = new Matrix4x4();
        private Vector3 clipPlanePosition = new Vector3();
        

        public Form1()
        {
            InitializeComponent();
            CreatScene();

            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);

            clipPlanePosition.Z += 10;

        }

        void CreatScene()
        {
            scene[0] = new Box();
            scene[0].leftBottom = new Vector3(1.0f, 1.0f, 1.0f);
            scene[0].rightTop = new Vector3(400.0f, 400.0f, 100.0f);

            scene[1] = new Box();
            scene[1].leftBottom = new Vector3(-10.0f, -20.0f, 10.0f);
            scene[1].rightTop = new Vector3(-40, 900.0f, 20.0f);
        }

        

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        void Draw()
        {
            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(Color.White);
            foreach (Box box in scene)
            {
                List<Vector2> projectedBox = new List<Vector2>();
                foreach (Vector3 vertex in GetBoxVertices(box))
                {
                    Vector3 verteCameraCords = MultiplyVector3byMatrix4x4(vertex, worldToCamera);
                    Vector2 vertexProjection = new Vector2();
                    vertexProjection.X = verteCameraCords.X / -verteCameraCords.Z * clipPlanePosition.Z;
                    vertexProjection.X += pictureBox1.Size.Width / 2;
                    vertexProjection.Y = verteCameraCords.Y / -verteCameraCords.Z * clipPlanePosition.Z;
                    vertexProjection.Y += pictureBox1.Size.Height / 2;
                    projectedBox.Add(vertexProjection);
                }

                Console.WriteLine("projectedBox:");
                foreach (Vector2 vec in projectedBox)
                {
                    Console.WriteLine(vec.ToString());
                }

                


                g.DrawLine(System.Drawing.Pens.Red, projectedBox[0].X, projectedBox[0].Y, projectedBox[1].X, projectedBox[1].Y);//a
                g.DrawLine(System.Drawing.Pens.Red, projectedBox[0].X, projectedBox[0].Y, projectedBox[3].X, projectedBox[3].Y);//b
                g.DrawLine(System.Drawing.Pens.Red, projectedBox[0].X, projectedBox[0].Y, projectedBox[7].X, projectedBox[7].Y);//c
                g.DrawLine(System.Drawing.Pens.Red, projectedBox[2].X, projectedBox[2].Y, projectedBox[1].X, projectedBox[1].Y);//d
                g.DrawLine(System.Drawing.Pens.Red, projectedBox[2].X, projectedBox[2].Y, projectedBox[3].X, projectedBox[3].Y);//e
                g.DrawLine(System.Drawing.Pens.Red, projectedBox[3].X, projectedBox[3].Y, projectedBox[4].X, projectedBox[4].Y);//f

                g.DrawLine(System.Drawing.Pens.Red, projectedBox[5].X, projectedBox[5].Y, projectedBox[6].X, projectedBox[6].Y);//g
                g.DrawLine(System.Drawing.Pens.Red, projectedBox[5].X, projectedBox[5].Y, projectedBox[2].X, projectedBox[2].Y);//h
                g.DrawLine(System.Drawing.Pens.Red, projectedBox[5].X, projectedBox[5].Y, projectedBox[4].X, projectedBox[4].Y);//i
                g.DrawLine(System.Drawing.Pens.Red, projectedBox[4].X, projectedBox[4].Y, projectedBox[7].X, projectedBox[7].Y);//j
                g.DrawLine(System.Drawing.Pens.Red, projectedBox[6].X, projectedBox[6].Y, projectedBox[7].X, projectedBox[7].Y);//k
                g.DrawLine(System.Drawing.Pens.Red, projectedBox[1].X, projectedBox[1].Y, projectedBox[6].X, projectedBox[6].Y);//l
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Draw();
        }


        private Vector3 MultiplyVector3byMatrix4x4(Vector3 vec, Matrix4x4 matrix)
        {
            return new Vector3(
                vec.X * matrix.M11 + vec.Y * matrix.M21 + vec.Z * matrix.M31 + matrix.M41,
                vec.X * matrix.M12 + vec.Y * matrix.M22 + vec.Z * matrix.M32 + matrix.M42,
                vec.X * matrix.M13 + vec.Y * matrix.M23 + vec.Z * matrix.M33 + +matrix.M43);
        }

        private Matrix4x4 QuaternionToMatrix4x4(Quaternion quat)
        {
            float xx = quat.X * quat.X;
            float xy = quat.X * quat.Y;
            float xz = quat.X * quat.Z;
            float xw = quat.X * quat.W;

            float yy = quat.Y * quat.Y;
            float yz = quat.Y * quat.Z;
            float yw = quat.Y * quat.W;

            float zz = quat.Z * quat.Z;
            float zw = quat.Z * quat.W;

            return new Matrix4x4(
                1 - 2 * (yy + zz), 2 * (xy - zw), 2 * (xz + yw), 0 ,
                2 * (xy + zw), 1 - 2 * (xx + zz), 2 * (yz - xw), 0 ,
                2 * (xz - yw), 2 * (yz + xw), 1 - 2 * (xx + yy), 0 ,
                0, 0, 0, 1 );
        }

        private List<Vector3> GetBoxVertices(Box box)
        {
            List<Vector3> vertices = new List<Vector3>();
            vertices.Add(box.leftBottom);//0
            vertices.Add(new Vector3(box.leftBottom.X, box.rightTop.Y, box.leftBottom.Z));//1
            vertices.Add(new Vector3(box.rightTop.X, box.rightTop.Y, box.leftBottom.Z));//2
            vertices.Add(new Vector3(box.rightTop.X, box.leftBottom.Y, box.leftBottom.Z));//3
            vertices.Add(new Vector3(box.rightTop.X, box.leftBottom.Y, box.rightTop.Z));//4
            vertices.Add(box.rightTop);//5
            vertices.Add(new Vector3(box.leftBottom.X, box.rightTop.Y, box.rightTop.Z));//6
            vertices.Add(new Vector3(box.leftBottom.X, box.leftBottom.Y, box.rightTop.Z));//7


            Console.WriteLine("GetBoxVertices");
            foreach (Vector3 vec in vertices)
            {
                Console.WriteLine(vec.ToString());
            }
            
            
            return vertices;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(10, 0, 0));
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(0, 0, 10));
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(0, 10, 0));
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(-10, 0, 0));
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(0, -10, 0));
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(0, 0, -10));
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            /*Matrix4x4 temp = new Matrix4x4(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, (float)Math.Cos(1.0f), -(float)Math.Sin(1.0f), 0.0f,
            0.0f, (float)Math.Sin(1.0f), (float)Math.Cos(1.0f), 0.0f,
            0.0f, 0.0f, 0.0f, 1.0f);*/
            Matrix4x4 temp = QuaternionToMatrix4x4(Quaternion.CreateFromAxisAngle(Vector3.UnitX, 0.17f));
            cameraToWorld *= temp;
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Matrix4x4 temp = QuaternionToMatrix4x4(Quaternion.CreateFromAxisAngle(-Vector3.UnitX, 0.17f));
            cameraToWorld *= temp;
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Matrix4x4 temp = QuaternionToMatrix4x4(Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.17f));
            cameraToWorld *= temp;
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Matrix4x4 temp = QuaternionToMatrix4x4(Quaternion.CreateFromAxisAngle(-Vector3.UnitY, 0.17f));
            cameraToWorld *= temp;
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Matrix4x4 temp = QuaternionToMatrix4x4(Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.17f));
            cameraToWorld *= temp;
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Matrix4x4 temp = QuaternionToMatrix4x4(Quaternion.CreateFromAxisAngle(-Vector3.UnitZ, 0.17f));
            cameraToWorld *= temp;
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            clipPlanePosition.Z = trackBar1.Value;
            Draw();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
