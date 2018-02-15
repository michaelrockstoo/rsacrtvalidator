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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        int igFormWidth = new int();  //窗口宽度
        int igFormHeight = new int(); //窗口高度
        float fgWidthScaling = new float(); //宽度缩放比例
        float fgHeightScaling = new float(); //高度缩放比例

        public Form1()
        {
            InitializeComponent();
            igFormWidth = this.ClientSize.Width;
            igFormHeight = this.ClientSize.Height;
            InitConTag(this);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            P.Text = null;
            Q.Text = null;
            DQ1.Text = null;
            DP1.Text = null;
            PQ.Text = null;
            Modulus.Text = null;
            PrivateExponent.Text = null;
        }

        //记录控件集初始的 位置、大小、字体大小信息
        private void InitConTag(Control cons)
        {
            foreach (Control con in cons.Controls) //遍历控件集
            {
                con.Tag = con.Left + "," + con.Top + "," + con.Width + "," + con.Height + "," + con.Font.Size;
                if (con.Controls.Count > 0) //处理子控件
                {
                    InitConTag(con);
                }
            }
        }

        //重新调整控件的 位置、大小、字体大小
        private void ResizeCon(float widthScaling, float heightScaling, Control cons)
        {
            float fTmp = new float();

            foreach (Control con in cons.Controls) //遍历控件集
            {
                string[] conTag = con.Tag.ToString().Split(new char[] { ',' });
                fTmp = Convert.ToSingle(conTag[0]) * widthScaling;
                con.Left = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[1]) * heightScaling;
                con.Top = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[2]) * widthScaling;
                con.Width = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[3]) * heightScaling;
                con.Height = (int)fTmp;
                fTmp = Convert.ToSingle(conTag[4]) * heightScaling;
                //con.Font = new Font("", (fTmp == 0) ? 0.1f : fTmp);
                if (con.Controls.Count > 0) //处理子控件
                {
                    ResizeCon(widthScaling, heightScaling, con);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (P.Text.Length == 0 || Q.Text.Length == 0 || Exponent.Text.Length == 0)
            {
                MessageBox.Show("P, Q, and e must not be empty!");
                return;
            }
            byte[] P1 = Program.ConvertToByteArray(P.Text);
            byte[] Q1 = Program.ConvertToByteArray(Q.Text);
            byte[] E  = Program.ConvertToByteArray(Exponent.Text);
            
            BigInteger P2 = Program.FromBigEndian(P1);
            BigInteger Q2 = Program.FromBigEndian(Q1);
            BigInteger E1 = Program.FromBigEndian(E);

            if(!Program.IsProbablyPrime(P2) || !Program.IsProbablyPrime(Q2))
            {
                MessageBox.Show("P or Q is not a Prime Number");
                DQ1.Text = null;
                DP1.Text = null;
                PQ.Text = null;
                Modulus.Text = null;
                PrivateExponent.Text = null;
                return;
            }
           
            BigInteger M1 = BigInteger.Multiply(P2, Q2);
            BigInteger PMinus1 = BigInteger.Subtract(P2, BigInteger.One); // P-1
            BigInteger QMinus1 = BigInteger.Subtract(Q2, BigInteger.One); // Q-1
            BigInteger Phi = BigInteger.Multiply(PMinus1, QMinus1);
            BigInteger D1 = Program.Modinv(E1, Phi); // Private Exponent
            BigInteger DQ2 = BigInteger.Remainder(D1, QMinus1); // dQ = (1/e) mod (q-1)
            BigInteger DP2 = BigInteger.Remainder(D1, PMinus1); // dP = (1/e) mod (p-1)
            BigInteger PQ2 = Program.Modinv(Q2, P2);

            byte[] C = Program.ConvertToByteArray(DQ2.ToString("X"));
            DQ1.Text = Program.ByteArrayToHex(C);

            C = Program.ConvertToByteArray(DP2.ToString("X"));
            DP1.Text = Program.ByteArrayToHex(C);

            C = Program.ConvertToByteArray(PQ2.ToString("X"));
            PQ.Text = Program.ByteArrayToHex(C);

            C = Program.ConvertToByteArray(M1.ToString("X"));
            Modulus.Text = Program.ByteArrayToHex(C);

            C = Program.ConvertToByteArray(D1.ToString("X"));
            PrivateExponent.Text = Program.ByteArrayToHex(C);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (igFormWidth == 0 || igFormHeight == 0) return;
            fgWidthScaling = (float)this.ClientSize.Width / (float)igFormWidth;
            fgHeightScaling = (float)this.ClientSize.Height / (float)igFormHeight;
            ResizeCon(fgWidthScaling, fgHeightScaling, this);
        }
    }
}
