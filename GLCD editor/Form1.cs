using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GLCD_editor
{
    public partial class Form1 : Form
    {
        private ArrayList font_data;
        private int font_width;
        private int font_height;
        private int byte_count;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox4.Text = Convert.ToString(int.Parse(textBox4.Text)+1);
            update_char();
        }

        private void update_char()
        {
            font_width = int.Parse(textBox2.Text);
            font_height = int.Parse(textBox3.Text);

            if (font_width > 8)
            {
                byte_count = 2;
            }
            else
            {
                byte_count = 1;
            }
            dataGridView1.Columns.Clear();
            for (int i = 1; i <= font_width; i++)
            {
                dataGridView1.Columns.Add("cl" + i.ToString(), "cl1" + i.ToString());
            }
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Width = 15;
            }
            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add(font_height);
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Height = 15;
            }

            textBox1.Clear();

            int ch = int.Parse(textBox4.Text);

            int fidx = (ch - 1) * font_height * byte_count;
            for (int i = fidx; i < fidx + font_height * byte_count; i++)
            {
                textBox1.Text += "0x" + Convert.ToString((byte)font_data[i], 16).PadLeft(2, '0').ToUpper() + ",";
            }
            String byte_str;
            int ri = 0;
            textBox1.Text += "\r\n";
            for (int i = fidx; i < fidx + font_height * byte_count; i += byte_count)
            {
                if (byte_count == 2)
                {
                    byte_str = Convert.ToString((byte)font_data[i + 1], 2).PadLeft(8, '0') + Convert.ToString((byte)font_data[i], 2).PadLeft(8, '0');
                }
                else
                {
                    byte_str = Convert.ToString((byte)font_data[i], 2).PadLeft(8, '0');
                }
                byte_str = byte_str.Substring(byte_str.Length - font_width);
                textBox1.Text += byte_str + "\r\n";
                for (int j = 0; j < byte_str.Length; j++)
                {
                    if (byte_str.Substring(j, 1) == "1")
                    {
                        dataGridView1.Rows[ri].Cells[j].Style.BackColor = Color.Black;
                    }
                    else
                    {
                        dataGridView1.Rows[ri].Cells[j].Style.BackColor = Color.White;
                    }
                }
                ri++;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                ((DataGridView)sender).SelectedCells[0].Selected = false;
            }
            catch { }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Black)
            {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            }
            else
            {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Black;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.InitialDirectory = Environment.CurrentDirectory;
            String buf;
            String load_array="";
            int cmt_idx;
            textBox1.Clear();
            if (od.ShowDialog(this) == DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(od.FileName))
                {
                    while (!sr.EndOfStream)
                    {
                        buf = sr.ReadLine();
                        if (buf.IndexOf('#') == -1)
                        {
                            cmt_idx = buf.IndexOf("//");
                            if (cmt_idx != -1)
                            { //есть комментарий
                                buf = buf.Substring(0, cmt_idx);
                            }
                            load_array += buf + "\r\n";
                        }
                        else
                        {
                            MatchCollection match = Regex.Matches(buf, @"FONT_([0-9]+)X([0-9]+)");
                            foreach (Match m in match)
                            {
                                if (m.Groups.Count == 3)
                                {
                                    textBox2.Text = m.Groups[1].Value;
                                    textBox3.Text = m.Groups[2].Value;
                                }
                            }
                        }
                    }
                }
                load_array = load_array.Substring(load_array.IndexOf('{') + 1, load_array.LastIndexOf('}') - load_array.IndexOf('{') - 1);
                load_array = load_array.Replace(" ", "").Replace("\r","").Replace("\n","");
                char[] charSeparators = new char[] { ',' };
                font_data = new ArrayList();
                string[] values = load_array.Split(charSeparators);
                if (values.Count() > 0)
                {
                    foreach(String str in values)
                    {
                        font_data.Add(Convert.ToByte(str, 16));
                    }
                }
                //MessageBox.Show(font_data.Count.ToString());
                textBox1.Text = load_array;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox4.Text = Convert.ToString(int.Parse(textBox4.Text) - 1);
            update_char();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int ch = int.Parse(textBox4.Text);

            int fidx = (ch - 1) * font_height * byte_count;
            for (int i = fidx; i < fidx + font_height * byte_count; i++)
            {
                textBox1.Text += "0x" + Convert.ToString((byte)font_data[i], 16).PadLeft(2, '0').ToUpper() + ",";
            }
            String byte_str;
            byte nd;
            textBox1.Text += "\r\n";
            for (int i = 0; i < font_height* byte_count; i+= byte_count)
            {
                byte_str = "";
                for (int j = 0; j < font_width; j++)
                {
                    if (dataGridView1.Rows[i/byte_count].Cells[j].Style.BackColor == Color.Black)
                    {
                        byte_str += "1";
                    }
                    else
                    {
                        byte_str += "0";
                    }
                }

                
                textBox1.Text += byte_str + "\r\n";

                if (byte_count == 2)
                {
                    nd = Convert.ToByte(byte_str.Substring(font_width-8), 2);
                    textBox1.Text += font_data[fidx + i].ToString() + "-"+ nd.ToString();
                    font_data[fidx + i] = nd;
                    nd = Convert.ToByte(byte_str.Substring(0, font_width - 8), 2);
                    textBox1.Text += "," + font_data[fidx + i + 1].ToString() + "-" + nd.ToString() + "\r\n";
                    font_data[fidx + i + 1] = nd;
                }
                else
                {
                    nd = Convert.ToByte(byte_str, 2);
                    font_data[fidx+i] = nd;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            int fi = 0x20;
            String buf = "{\r\n";

            for (int i = 0; i < font_data.Count; i+= font_height * byte_count)
            {
                for (int j = 0; j < font_height*byte_count; j++)
                {
                    buf += "0x"+Convert.ToString((byte)font_data[i+j], 16).ToUpper().PadLeft(2, '0') ;
                    if (i + j + 1 < font_data.Count)
                    {
                        buf += ",";
                    }
                }
                buf += " //  " + "0x" + Convert.ToString(fi,16).ToUpper()  +"\r\n";
                fi++;
            }

            buf += "}";
            textBox1.Text = buf;
        }
    }
}
