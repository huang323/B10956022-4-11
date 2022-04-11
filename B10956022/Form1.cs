using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualBasic.PowerPacks;

namespace B10956022
{
    public partial class Form1 : Form
    {
        UdpClient U;
        Thread Th;
        ShapeContainer C;
        ShapeContainer D;
        Point stp;
        string p;
        
        public Form1()
        {
            InitializeComponent();
        }
       

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Th.Abort();
                U.Close();
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Th = new Thread(Listen);
            Th.Start();
            button1.Enabled = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            C = new ShapeContainer();
            this.Controls.Add(C);
            D = new ShapeContainer();
            this.Controls.Add(D);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            stp = e.Location;
            p = stp.X.ToString() + "," + stp.Y.ToString();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                LineShape L = new LineShape();
                L.StartPoint = stp;
                L.EndPoint = e.Location;
                if (radioButton1.Checked) { L.BorderColor = Color.Red; }
                if (radioButton2.Checked) { L.BorderColor = Color.Green; }
                if (radioButton3.Checked) {L.BorderColor = Color.Blue; }
                if (radioButton4.Checked) { L.BorderColor = Color.Black; }
                L.Parent = C;
                stp = e.Location;
                p += "/" + stp.X.ToString() + "," + stp.Y.ToString();

            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int Port = int.Parse(textBox2.Text);
            UdpClient S = new UdpClient(textBox1.Text, Port);
            if (radioButton1.Checked) { p = "1_" + p; }
            if (radioButton2.Checked) { p = "2_" + p; }
            if (radioButton3.Checked) { p = "3_" + p; }
            if (radioButton4.Checked) { p = "4_" + p; }
            byte[] B = Encoding.Default.GetBytes(p);
            S.Send(B, B.Length);
            S.Close();
        }
        private void Listen()
        {
            int Port = int.Parse(textBox3.Text);
            U = new UdpClient(Port);
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
            while(true)
            {
                byte[] B = U.Receive(ref EP);
                string A = Encoding.Default.GetString(B);
                string[] z = A.Split('_');
                string[] Q = z[1].Split('/');
                Point[] R = new Point[Q.Length];
                for(int i=0;i<Q.Length;i++)
                {
                    string[] K = Q[i].Split(',');
                    R[i].X = int.Parse(K[0]);
                    R[i].Y = int.Parse(K[1]);
                }
                for(int i=0;i<Q.Length-1;i++)
                {
                    LineShape L = new LineShape();
                    L.StartPoint = R[i];
                    L.EndPoint = R [i + 1];
                    L.Parent = D;

                    switch(z[0])
                    {
                        case "1":
                            L.BorderColor = Color.Red;
                            break;
                        case "2":
                            L.BorderColor = Color.Green;
                            break;
                        case "3":
                            L.BorderColor = Color.Blue;
                            break;
                        case "4":
                            L.BorderColor = Color.Black;
                            break;
                    }
                }
            }
        }
    }
}
