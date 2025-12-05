using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace LoginC
{
    public partial class Dashboard : Form
    {
        public int ID;
        public string name;
        public Dashboard(int userId, string username)
        {
            InitializeComponent();
			pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // O 

			pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			pictureBox2.SizeMode = PictureBoxSizeMode.Zoom; // O 

			pictureBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			pictureBox3.SizeMode = PictureBoxSizeMode.Zoom; // O 

            label3.Text = username;
            ID = userId; name = username;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
			Products F3 = new Products(ID, name);
			F3.Show();
			this.Hide();
			// Mostrar el siguiente formulario
		}

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
			label2.Text = "PRODUCTOS";
		}

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
			label2.Text = "";
		}

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
			label2.Text = "USUARIOS";
		}

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
			label2.Text = "";
		}

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
			label2.Text = "VENTAS";
		}

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
			label2.Text = "";
		}

        private void Dashboard_Load(object sender, EventArgs e)
        {
           
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Sales F3 = new Sales();
            F3.Show();
            this.Hide();
            // Mostrar el siguiente formulario
        }
    }
}
