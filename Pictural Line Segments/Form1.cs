using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pictural_Line_Segments
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Pen pen = new Pen(new SolidBrush(Color.Black), 1);
        Pen hover_pen = new Pen(new SolidBrush(Color.Red), 2);
        Pen registered_pen = new Pen(new SolidBrush(Color.Blue), 2);

        Graphics g;

        Point cursor_rel = new Point();

        bool left_click = false;
        bool right_click = false;

        List<LineSegment> LineSegments = new List<LineSegment>();

        public Point CurrentFirstPoint = new Point(-1,-1);

        Point LastHoveredPoint = new Point(-1, -1);

        int delt = 5;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;

            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (int h = 0; h < pictureBox1.Height; h += 10)
            {
                for (int w = 0; w < pictureBox1.Width; w += 10)
                {
                    if (cursor_rel.X > w - delt && cursor_rel.X < w + delt && cursor_rel.Y > h - delt && cursor_rel.Y < h + delt)
                    {
                        LastHoveredPoint = new Point(w, h);
                        if (left_click)
                        {
                            if (CurrentFirstPoint.X == -1 && CurrentFirstPoint.Y == -1)
                            {
                                CurrentFirstPoint = new Point(w, h);
                            }
                            else
                            {
                                LineSegments.Add(new LineSegment(CurrentFirstPoint, new Point(w, h)));
                                CurrentFirstPoint = new Point(-1,-1);

                                UpdateListBox();
                            }
                            left_click = false;
                        }
                        else if (right_click)
                        {
                            List<LineSegment> where1 = LineSegments.Where(value => (value.FirstPoint.X == LastHoveredPoint.X && value.FirstPoint.Y == LastHoveredPoint.Y)).ToList();
                            List<LineSegment> where2 = LineSegments.Where(value => (value.SecondPoint.X == LastHoveredPoint.X && value.SecondPoint.Y == LastHoveredPoint.Y)).ToList();

                            try
                            {
                                    // clicked on outer point of complete line segment
                                    if (where1.Count() > 0)
                                    {
                                        CurrentFirstPoint = where1[0].SecondPoint;
                                        LineSegments.Remove(where1[0]);
                                    }
                                    else if (where2.Count() > 0)
                                    {
                                        CurrentFirstPoint = where2[0].FirstPoint;
                                        LineSegments.Remove(where2[0]);
                                    }
                                // right clicked the anchor of the currently selected line
                                else if (CurrentFirstPoint.X != -1 && CurrentFirstPoint.Y != -1)
                                {
                                    CurrentFirstPoint = new Point(-1, -1);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                            UpdateListBox();

                            right_click = false;
                        }
                        /*g.DrawLine(hover_pen, new Point(w, h - 3), new Point(w, h + 3));
                        g.DrawLine(hover_pen, new Point(w - 3, h), new Point(w + 3, h));*/

                        if (CurrentFirstPoint.X == -1 && CurrentFirstPoint.Y == -1)
                        {
                            g.DrawRectangle(hover_pen, new Rectangle(new Point(LastHoveredPoint.X - 2, LastHoveredPoint.Y - 2), new Size(4,4)));
                        }
                    }

                    /*else if (checkBox1.Checked)
                    {
                        g.DrawLine(pen, new Point(w, h - 3), new Point(w, h + 3));
                        g.DrawLine(pen, new Point(w - 3, h), new Point(w + 3, h));
                    }*/

                }
            }

            if (CurrentFirstPoint.X != -1 && CurrentFirstPoint.Y != -1)
            {
                g.DrawRectangle(registered_pen, new Rectangle(new Point(CurrentFirstPoint.X - 2, CurrentFirstPoint.Y - 2), new Size(4, 4)));
                g.DrawLine(new Pen(new SolidBrush(Color.Green), 2), CurrentFirstPoint, LastHoveredPoint);
                g.DrawRectangle(hover_pen, new Rectangle(new Point(LastHoveredPoint.X - 2, LastHoveredPoint.Y - 2), new Size(4, 4)));
                //g.DrawLine(new Pen(new SolidBrush(Color.Green), 2), CurrentFirstPoint, cursor_rel);
            }

            if (checkBox3.Checked)
            {

                foreach (LineSegment seg in LineSegments)
                {
                    g.DrawRectangle(registered_pen, new Rectangle(new Point(seg.FirstPoint.X - 2, seg.FirstPoint.Y - 2), new Size(4,4)));
                    g.DrawRectangle(registered_pen, new Rectangle(new Point(seg.SecondPoint.X - 2, seg.SecondPoint.Y - 2), new Size(4,4)));
                    g.DrawLine(new Pen(new SolidBrush(Color.Blue), 2), seg.FirstPoint, seg.SecondPoint);
                }
            }
        }

        private void UpdateListBox()
        {
            listBox1.Items.Clear();
            foreach (LineSegment seg in LineSegments)
            {
                string equation = "";

                // point 1
                double x1 = seg.FirstPoint.X / 10;
                double y1 = 63 - (seg.FirstPoint.Y / 10);

                // point 2
                double x2 = seg.SecondPoint.X / 10;
                double y2 = 63 - seg.SecondPoint.Y / 10;

                double numerator = y1 - y2;
                double denominator = x1 - x2;

                // vertical line
                if (denominator == 0)
                {
                    equation = "x = " + seg.SecondPoint.X / 10;

                    double smallest = 0;
                    double largest = 0;

                    if (y1 < y2)
                    {
                        smallest = y1;
                        largest = y2;
                    }
                    else
                    {
                        smallest = y2;
                        largest = y1;
                    }

                    equation += "\n" + smallest + " <= y <=" + largest;
                }
                // horizontal or angled line
                else
                {
                    bool anghor = false;

                    equation = "y = ";

                    // angled line
                    if ((numerator / denominator) != 0)
                    {
                        anghor = true;
                        equation += Math.Round((double)(numerator / denominator), 3) + "x";
                    }
                    // horizontal line
                    else
                    {
                        anghor = false;
                    }

                    if ((63 - (seg.SecondPoint.Y / 10)) != 0)
                    {
                        if (equation != "y = ")
                            equation += " + ";

                        equation += 63 - (seg.SecondPoint.Y / 10);
                    }

                    // angled
                    if (anghor)
                    {
                        //equation += " (angled)";
                    }
                    // horizontal
                    else
                    {
                        //equation += " (horizontal)";
                    }

                    double smallest = 0;
                    double largest = 0;

                    if (x1 < x2)
                    {
                        smallest = x1;
                        largest = x2;
                    }
                    else
                    {
                        smallest = x2;
                        largest = x1;
                    }

                    equation += "\n" + smallest + " <= x <=" + largest;
                }


                listBox1.Items.Add(equation);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            cursor_rel = e.Location;
            pictureBox1.Refresh();   
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                left_click = true;
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                right_click = true;

            pictureBox1.Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                if (checkBox1.Checked)
                    pictureBox1.BackgroundImage = Properties.Resources.iss_with_grid;
                else
                    pictureBox1.BackgroundImage = Properties.Resources._693259main_jsc2012e219094_big1;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox2.Checked)
            {
                pictureBox1.BackgroundImage = null;
            }
            else
            {
                if (checkBox1.Checked)
                    pictureBox1.BackgroundImage = Properties.Resources.iss_with_grid;
                else
                    pictureBox1.BackgroundImage = Properties.Resources._693259main_jsc2012e219094_big1;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();
                e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);
            }
            catch
            {

            }
        }
    }

    public class LineSegment
    {
        public Point FirstPoint;
        public Point SecondPoint;
        public LineSegment(Point FirstPoint, Point SecondPoint)
        {
            this.FirstPoint = FirstPoint;
            this.SecondPoint = SecondPoint;
        }
    }
}
