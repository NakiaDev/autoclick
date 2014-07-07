﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace click
{
    public partial class Form1 : Form
    {
        //---------------------------------------------------
        #region Mouse and Keys 
        //mouse click:
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        //---------------------------------------------------
        //global keypress detection:
        public const int WM_HOTKEY_MSG_ID = 0x0312;
        private GlobalKey assign_glob;
        private GlobalKey start_glob;
        #endregion
        //---------------------------------------------------

        public Form1()
        {
            InitializeComponent();            
            Alap();
        }

        TextBox x, y, repeat, delay;
        RichTextBox rt;
        private void Alap()
        {
            this.KeyPreview = true; //MUST HAVE for the buttons
            //---------------------------------------------------
            #region Layout
            this.Text = "Auto-clicker by Nakia";
            //---------------------------------------------------
            this.Location = new Point(Screen.GetWorkingArea(this).Width - 550, 150);
            this.Size = new Size(450, 500);
            //---------------------------------------------------
            Button assign = new Button();
            Controls.Add(assign);
            assign.Text = "Assign (F6)";
            assign.Location = new Point(140, 10);
            assign.Click += assign_Click;
            assign.TabIndex = 1;
            //---------------------------------------------------
            Button add = new Button();
            Controls.Add(add);
            add.Text = "Add";
            add.Location = new Point(140, 100);
            add.Click += add_Click;
            add.TabIndex = 0;
            //---------------------------------------------------
            Button start = new Button();
            Controls.Add(start);
            start.Text = "Start (F7)";
            start.Location = new Point(275, 100);
            start.TabIndex = 2;
            start.Click += start_Click;
            //---------------------------------------------------
            x = new TextBox();
            Controls.Add(x);
            x.Location = new Point(30, 12);
            //---------------------------------------------------
            y = new TextBox();
            Controls.Add(y);
            y.Location = new Point(30, 42);
            //---------------------------------------------------
            Label xx = new Label();
            Controls.Add(xx);
            xx.Text = "X:";
            xx.Location = new Point(10, 15);
            //---------------------------------------------------
            Label yy = new Label();
            Controls.Add(yy);
            yy.Text = "Y:";
            yy.Location = new Point(10, 45);
            //---------------------------------------------------
            Label del = new Label();
            Controls.Add(del);
            del.Text = "delay:";
            del.Location = new Point(10, 75);
            del.Size = new Size(36, 22);
            //---------------------------------------------------
            delay = new TextBox();
            Controls.Add(delay);
            delay.Location = new Point(55, 72);
            delay.Size = new Size(75, 22);
            delay.Text = "1000";
            //---------------------------------------------------
            Label rep = new Label();
            Controls.Add(rep);
            rep.Text = "Repeat:";
            rep.Location = new Point(10, 105);
            rep.Size = new Size(45, 22);
            //---------------------------------------------------
            repeat = new TextBox();
            Controls.Add(repeat);
            repeat.Location = new Point(55, 102);
            repeat.Size = new Size(75, 22);
            repeat.Text = "-1";
            //---------------------------------------------------
            rt = new RichTextBox();
            Controls.Add(rt);
            rt.Location = new Point(50, 160);
            rt.Size = new Size(325, 200);
            #endregion
            //---------------------------------------------------
            assign_glob = new GlobalKey(Keys.F6, this);
            start_glob = new GlobalKey(Keys.F7, this);
            assign_glob.Register();
            start_glob.Register();
            //---------------------------------------------------
            time.Interval = 1500;
            time.Tick += time_Tick;
        }

        #region Global key functions
        private Keys Getkey(IntPtr LParam)
        {
            return (Keys)((LParam.ToInt32()) >> 16);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY_MSG_ID)
                switch (Getkey(m.LParam))
                {
                    case Keys.F6:
                        Assign();
                        break;
                    case Keys.F7:
                        Start();
                        break;
                }
            base.WndProc(ref m);
        }
        #endregion

        #region Button clicks
        void start_Click(object sender, EventArgs e)
        {
            Start();
        }

        void add_Click(object sender, EventArgs e)
        {
            Add();
        }

        void assign_Click(object sender, EventArgs e)
        {
            Assign();
        }
        #endregion

        private void Assign()
        {
            x.Text = Cursor.Position.X.ToString();
            y.Text = Cursor.Position.Y.ToString();
        }

        Timer time = new Timer();
        bool run = false;
        int db;
        private void Start()
        {
            db = Convert.ToInt32(repeat.Text);
            Cursor.Position = new Point(Convert.ToInt32(x.Text), Convert.ToInt32(y.Text));
            //---------------------------------------------------
            if (!run)
                run = true;
            else
                run = false;
            //---------------------------------------------------            
            time.Start();          
        }

        void time_Tick(object sender, EventArgs e)
        {
            if (!run || db == 0)
            {
                if (db == 0 && run)
                    run = false;
                time.Stop();
            }
            else
            {
                //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                if (db > 0)
                    db--;
            }
        }

        List<Click> click = new List<Click>();
        private void Add()
        {
            //TODO: ADD table & delay
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            click.Add(new Click(X, Y, 0, 0));
            rt.Text = rt.Text + click[click.Count - 1].Click_Out() + '\n';
        }
    }
}
