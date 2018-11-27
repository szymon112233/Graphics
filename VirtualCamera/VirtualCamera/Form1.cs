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

        private Box[] scene = new Box[3];
        private Matrix4x4 cameraToWorld = new Matrix4x4(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, -1.0f, 0.0f,
            180.0f, 190.0f, -100.0f, 1.0f);

        private Matrix4x4 worldToCamera = new Matrix4x4();
        private Vector3 clipPlanePosition = new Vector3();

        Vector2 lastMousePos = new Vector2();
        bool rotateCameraMouse = false;

        public Form1()
        {
            InitializeComponent();
            CreatScene();

            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);

            clipPlanePosition.Z += 100;

            //button1.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            button1.PreviewKeyDown += new PreviewKeyDownEventHandler(OnButtonKeyDown);
            pictureBox1.MouseMove += new MouseEventHandler(OnMouseMove);
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                rotateCameraMouse = false;
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                rotateCameraMouse = true;
        }

        void CreatScene()
        {
            scene[0] = new Box();
            scene[0].leftBottom = new Vector3(1.0f, 1.0f, 1.0f);
            scene[0].rightTop = new Vector3(400.0f, 400.0f, 100.0f);

            scene[1] = new Box();
            scene[1].leftBottom = new Vector3(-10.0f, -20.0f, 10.0f);
            scene[1].rightTop = new Vector3(-40, 900.0f, 20.0f);

            scene[2] = new Box();
            scene[2].leftBottom = new Vector3(420.0f, -50.0f, 1.0f);
            scene[2].rightTop = new Vector3(520.0f, 100.0f, 45.0f);
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
                List<bool> drawVertex = new List<bool>();
                foreach (Vector3 vertex in GetBoxVertices(box))
                {
                    Vector3 verteCameraCords = MultiplyVector3byMatrix4x4(vertex, worldToCamera);
                    if (-verteCameraCords.Z < clipPlanePosition.Z)
                        drawVertex.Add(false);
                    else
                        drawVertex.Add(true);
                    Vector2 vertexProjection = new Vector2();
                    vertexProjection.X = verteCameraCords.X / -verteCameraCords.Z * clipPlanePosition.Z;
                    if (float.IsInfinity(vertexProjection.X) || float.IsNaN(vertexProjection.X))
                        vertexProjection.X = 0.0f;
                    vertexProjection.X += pictureBox1.Size.Width / 2;
                    vertexProjection.Y = verteCameraCords.Y / -verteCameraCords.Z * clipPlanePosition.Z;
                    if (float.IsInfinity(vertexProjection.Y) || float.IsNaN(vertexProjection.Y))
                        vertexProjection.Y = 0.0f;
                    vertexProjection.Y += pictureBox1.Size.Height / 2;
                    projectedBox.Add(vertexProjection);
                }

                /*Console.WriteLine("projectedBox:");
                foreach (Vector2 vec in projectedBox)
                {
                    Console.WriteLine(vec.ToString());
                }*/

                

                if (drawVertex[0] && drawVertex[1])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[0].X, projectedBox[0].Y, projectedBox[1].X, projectedBox[1].Y);//a
                if (drawVertex[0] && drawVertex[3])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[0].X, projectedBox[0].Y, projectedBox[3].X, projectedBox[3].Y);//b
                if (drawVertex[0] && drawVertex[7])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[0].X, projectedBox[0].Y, projectedBox[7].X, projectedBox[7].Y);//c
                if (drawVertex[2] && drawVertex[1])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[2].X, projectedBox[2].Y, projectedBox[1].X, projectedBox[1].Y);//d
                if (drawVertex[2] && drawVertex[3])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[2].X, projectedBox[2].Y, projectedBox[3].X, projectedBox[3].Y);//e
                if (drawVertex[3] && drawVertex[4])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[3].X, projectedBox[3].Y, projectedBox[4].X, projectedBox[4].Y);//f

                if (drawVertex[5] && drawVertex[6])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[5].X, projectedBox[5].Y, projectedBox[6].X, projectedBox[6].Y);//g
                if (drawVertex[5] && drawVertex[2])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[5].X, projectedBox[5].Y, projectedBox[2].X, projectedBox[2].Y);//h
                if (drawVertex[5] && drawVertex[4])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[5].X, projectedBox[5].Y, projectedBox[4].X, projectedBox[4].Y);//i
                if (drawVertex[4] && drawVertex[7])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[4].X, projectedBox[4].Y, projectedBox[7].X, projectedBox[7].Y);//j
                if (drawVertex[6] && drawVertex[7])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[6].X, projectedBox[6].Y, projectedBox[7].X, projectedBox[7].Y);//k
                if (drawVertex[1] && drawVertex[6])
                    g.DrawLine(System.Drawing.Pens.Red, projectedBox[1].X, projectedBox[1].Y, projectedBox[6].X, projectedBox[6].Y);//l
            }

            label5.Text = string.Format("Camera Postion: ({0} | {1} | {2})", cameraToWorld.M41, cameraToWorld.M42, cameraToWorld.M43);
            label6.Text = string.Format("Camera Rotation:\n({0} | {1} | {2}\n{3} | {4} | {5}\n{6} | {7} | {8} )",
                cameraToWorld.M11, cameraToWorld.M12, cameraToWorld.M13,
                cameraToWorld.M21, cameraToWorld.M22, cameraToWorld.M23,
                cameraToWorld.M31, cameraToWorld.M32, cameraToWorld.M33);
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

        private Matrix4x4 SetRotate(Vector3 axis, float rot, Vector3 A)
        {
            Matrix4x4 output = new Matrix4x4();

            float c = (float)Math.Cos(rot);
            float s = (float)Math.Sin(rot);
            float t = 1.0f - c;
            output.M11 = c + axis.X * axis.X * t;
            output.M22 = c + axis.Y * axis.Y * t;
            output.M33 = c + axis.Z * axis.Z * t;


            float tmp1 = axis.X * axis.Y * t;
            float tmp2 = axis.Z * s;
            output.M21 = tmp1 + tmp2;
            output.M12 = tmp1 - tmp2;


            tmp1 = axis.X * axis.Z * t;
            tmp2 = axis.Y * s;
            output.M31 = tmp1 - tmp2;
            output.M13 = tmp1 + tmp2;

            tmp1 = axis.Y * axis.Z * t;
            tmp2 = axis.X * s;
            output.M32 = tmp1 + tmp2;
            output.M23 = tmp1 - tmp2;


            float a1, a2, a3;
            if (A == null)
            {
                a1 = a2 = a3 = 0;
            }
            else
            {
                a1 = A.X; a2 = A.Y; a3 = A.Z;
            }

            output.M14 = a1 - a1 * output.M11 - a2 * output.M12 - a3 * output.M13;
            output.M23 = a2 - a1 * output.M21 - a2 * output.M22 - a3 * output.M23;
            output.M34 = a3 - a1 * output.M31 - a2 * output.M32 - a3 * output.M33;
            output.M41 = output.M42 = output.M43 = 0.0f;
            output.M44 = 1.0f;

            return output;
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


            //Console.WriteLine("GetBoxVertices");
            /*foreach (Vector3 vec in vertices)
            {
                Console.WriteLine(vec.ToString());
            }*/
            
            
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

        /// Z
        private void button13_Click(object sender, EventArgs e)
        {
            Rotate(Vector3.UnitZ, 10);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Rotate(-Vector3.UnitZ, 10);
        }

        /// X
        private void button8_Click(object sender, EventArgs e)
        {
            Rotate(Vector3.UnitX, 10);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Rotate(-Vector3.UnitX, 10);
        }

        /// Y
        private void button10_Click(object sender, EventArgs e)
        {
            Rotate(Vector3.UnitY, 10);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Rotate(-Vector3.UnitY, 10);
        }


        private void Rotate(Vector3 axis, float angle)
        {
            Matrix4x4 temp = QuaternionToMatrix4x4(Quaternion.CreateFromAxisAngle(axis, angle * DEG2RAD));
            cameraToWorld = temp * cameraToWorld;
            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            cameraToWorld = new Matrix4x4(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, -1.0f, 0.0f,
            180.0f, 190.0f, -100.0f, 1.0f);
            clipPlanePosition = new Vector3();
            clipPlanePosition.Z += 100;

            bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
            Draw();
        }


        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.W)
            {
                cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(0, 0, 10));
                bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
                Draw();
            }
            else if (e.KeyCode == Keys.S)
            {
                cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(0, 0, -10));
                bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
                Draw();
            }

        }

        private void OnButtonKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyCode == Keys.W)
            {
                cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(cameraToWorld.M31, cameraToWorld.M32, cameraToWorld.M33) * -5);
                bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
                Draw();
            }
            else if (e.KeyCode == Keys.S)
            {
                cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(cameraToWorld.M31, cameraToWorld.M32, cameraToWorld.M33) * 5);
                bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
                Draw();
            }

            if (e.KeyCode == Keys.A)
            {
                cameraToWorld *= Matrix4x4.CreateTranslation( new Vector3(cameraToWorld.M11, cameraToWorld.M12, cameraToWorld.M13) * -5);
                bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
                Draw();
            }
            else if (e.KeyCode == Keys.D)
            {
                cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(cameraToWorld.M11, cameraToWorld.M12, cameraToWorld.M13) * 5);
                bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
                Draw();
            }
            else if (e.KeyCode == Keys.Z)
            {
                cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(cameraToWorld.M21, cameraToWorld.M22, cameraToWorld.M23) * -5);
                bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
                Draw();
            }
            else if (e.KeyCode == Keys.X)
            {
                cameraToWorld *= Matrix4x4.CreateTranslation(new Vector3(cameraToWorld.M21, cameraToWorld.M22, cameraToWorld.M23) * 5);
                bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
                Draw();
            }

            if (e.KeyCode == Keys.Q)
            {
                Rotate(-Vector3.UnitZ, 2.0f);
            }
            else if (e.KeyCode == Keys.E)
            {
                Rotate(Vector3.UnitZ, 2.0f);
            }
            else if (e.KeyCode == Keys.NumPad4)
            {
                Rotate(Vector3.UnitY, 10.0f);
            }
            else if (e.KeyCode == Keys.NumPad6)
            {
                Rotate(-Vector3.UnitY, 10.0f);
            }
            else if (e.KeyCode == Keys.NumPad8)
            {
                Rotate(Vector3.UnitX, 2.0f);
            }
            else if (e.KeyCode == Keys.NumPad2)
            {
                Rotate(-Vector3.UnitX, 2.0f);
            }

        }

        bool first = false;


        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (rotateCameraMouse)
            {
                if (!first)
                {
                    first = true;
                    lastMousePos = new Vector2(e.X, e.Y);
                    return;
                }

                int deltaX = (int)lastMousePos.X - e.X;
                int deltaY = (int)lastMousePos.Y - e.Y;


                lastMousePos.X = e.X;
                lastMousePos.Y = e.Y;


                Console.WriteLine(deltaX);
                Matrix4x4 temp = QuaternionToMatrix4x4(Quaternion.CreateFromAxisAngle(-Vector3.UnitY, deltaX * 0.02f * DEG2RAD));
                cameraToWorld = temp * cameraToWorld;

                temp = QuaternionToMatrix4x4(Quaternion.CreateFromAxisAngle(Vector3.UnitX, deltaY * 0.02f * DEG2RAD));
                cameraToWorld = temp * cameraToWorld;

                bool gib = Matrix4x4.Invert(cameraToWorld, out worldToCamera);
                Draw();
            }
            else
                first = false;
        }



        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            clipPlanePosition.Z = trackBar1.Value;
            Draw();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
