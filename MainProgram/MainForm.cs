using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZKILLBOARDDATA;
using Newtonsoft.Json;
using System.Net.Http;


namespace MainProgram
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);

        }

        #region ALLOW DRAG WINDOW RESIZE 
        //ALLOW DRAG WINDOW RESIZE start
        const uint WM_NCHITTEST = 0x0084, WM_MOUSEMOVE = 0x0200,
               HTLEFT = 10, HTRIGHT = 11, HTBOTTOMRIGHT = 17,
               HTBOTTOM = 15, HTBOTTOMLEFT = 16, HTTOP = 12,
               HTTOPLEFT = 13, HTTOPRIGHT = 14;
        Size formSize;
        Point screenPoint;
        Point clientPoint;
        Dictionary<uint, Rectangle> boxes;
        const int RHS = 10; // RESIZE_HANDLE_SIZE
        bool handled;
        #endregion

        //SUBMIT
        public void Btn_submit_Click(object sender, EventArgs e)
        {

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "C# App");
            var task = client.GetAsync("https://zkillboard.com/api/stats/characterID/224802743/")
              .ContinueWith((taskwithresponse) =>
              {
                  var response = taskwithresponse.Result;
                  var jsonString = response.Content.ReadAsStringAsync();
                  jsonString.Wait();

                  var obj = JsonConvert.DeserializeObject<GettingStarted>(jsonString.Result);
                  OutputBox.Text = obj.Attackers.Rank.ToString();





              });
        }
        #region ALLOW DRAG WINDOW RESIZE 
        protected override void WndProc(ref Message m)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                base.WndProc(ref m);
                return;
            }

            handled = false;
            if (m.Msg == WM_NCHITTEST || m.Msg == WM_MOUSEMOVE)
            {
                formSize = this.Size;
                screenPoint = new Point(m.LParam.ToInt32());
                clientPoint = this.PointToClient(screenPoint);

                boxes = new Dictionary<uint, Rectangle>() {
                {HTBOTTOMLEFT, new Rectangle(0, formSize.Height - RHS, RHS, RHS)},
                {HTBOTTOM, new Rectangle(RHS, formSize.Height - RHS, formSize.Width - 2*RHS, RHS)},
                {HTBOTTOMRIGHT, new Rectangle(formSize.Width - RHS, formSize.Height - RHS, RHS, RHS)},
                {HTRIGHT, new Rectangle(formSize.Width - RHS, RHS, RHS, formSize.Height - 2*RHS)},
                {HTTOPRIGHT, new Rectangle(formSize.Width - RHS, 0, RHS, RHS) },
                {HTTOP, new Rectangle(RHS, 0, formSize.Width - 2*RHS, RHS) },
                {HTTOPLEFT, new Rectangle(0, 0, RHS, RHS) },
                {HTLEFT, new Rectangle(0, RHS, RHS, formSize.Height - 2*RHS) }
            };

                foreach (var hitBox in boxes)
                {
                    if (hitBox.Value.Contains(clientPoint))
                    {
                        m.Result = (IntPtr)hitBox.Key;
                        handled = true;
                        break;
                    }
                }
            }

            if (!handled)
                base.WndProc(ref m);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor,
                    this.ClientSize.Width - 8, this.ClientSize.Height - 16, 16, 16);
            }

            base.OnPaint(e);
        }
        //ALLOW DRAG WINDOW RESIZE end

        //stores mouse position before pressing
        public bool isMouseDown = false;
        public int xLast;
        public int yLast;

        //Mouse UP Event
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        //Mouse MOVE Event
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)

            {
                int newY = this.Top + (e.Y - yLast);
                int newX = this.Left + (e.X - xLast);
                this.Location = new Point(newX, newY);
            }
        }

        //Mouse DOWN Event
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            xLast = e.X;
            yLast = e.Y;
        }

        //CloseButton Click Event
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

    }
}
