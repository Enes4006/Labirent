using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace labirent
{
    public partial class Form1: Form
    {
        int[,] labirent;
        int boyut = 20;
        Random rnd = new Random();
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load; // Form açıldığında Form1_Load metodu çalışır
        }
        // Form_Load olayında:
        private void Form1_Load(object sender, EventArgs e)
        {
            // Button 1 - Labirent Oluştur
            Button btnOlustur = new Button
            {
                Text = "Labirent Oluştur",
                Location = new Point(20, 20),
                Size = new Size(120, 40)
            };
            btnOlustur.Click += BtnOlustur_Click;
            Controls.Add(btnOlustur);

            // Button 2 - BFS ile Çöz
            Button btnCoz = new Button
            {
                Text = "Labirenti Çöz (BFS)",
                Location = new Point(160, 20),
                Size = new Size(140, 40)
            };
            btnCoz.Click += BtnCoz_Click;
            Controls.Add(btnCoz);

            // PictureBox - Labirent Gösterimi
            PictureBox pbLabirent = new PictureBox
            {
                Name = "pbLabirent",
                Location = new Point(20, 80),
                Size = new Size(500, 500),
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(pbLabirent);



            //          DFS          //
            Button btnDFS = new Button
            {
                Text = "Labirenti Çöz (DFS)",
                Location = new Point(320, 20),
                Size = new Size(140, 40)
            };
            btnDFS.Click += BtnDFS_Click;
            Controls.Add(btnDFS);

        }
        void LabirentOlustur()
        {
            labirent = new int[boyut, boyut];
            for (int i = 0; i < boyut; i++)
            {
                for (int j = 0; j < boyut; j++)
                {
                    labirent[i, j] = (rnd.NextDouble() < 0.7) ? 0 : 1; // %70 yol; (0 = yol, 1 = duvar)
                }
            }
            labirent[0, 0] = 0; // Giriş  
            labirent[boyut - 1, boyut - 1] = 0; // Çıkış
        }
        List<Point> BFS(int[,] maze, Point start, Point end)
        {
            Queue<Point> queue = new Queue<Point>(); // BFS için kuyruk (önce giren önce çıkar)
            Dictionary<Point, Point> parent = new Dictionary<Point, Point>(); // Her noktanın hangi noktadan geldiğini tutar (yolu geri izlemek için)
            bool[,] visited = new bool[boyut, boyut]; // Hangi hücreler ziyaret edildi?

            // 4 yön: sağ, aşağı, sol, yukarı
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { 1, 0, -1, 0 };

            // Başlangıç noktası kuyruğa ekleniyor
            queue.Enqueue(start); 
            visited[start.X, start.Y] = true;


            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();

                if (current == end)
                {
                    // Yol bulundu, geri izleme
                    List<Point> path = new List<Point>();
                    while (current != start)
                    {
                        path.Add(current);
                        current = parent[current];
                    }
                    path.Reverse();
                    return path;
                }

                for (int i = 0; i < 4; i++)
                {
                    int nx = current.X + dx[i];
                    int ny = current.Y + dy[i];

                    if (nx >= 0 && ny >= 0 && nx < boyut && ny < boyut && maze[nx, ny] == 0 && !visited[nx, ny])
                    {
                        Point next = new Point(nx, ny);
                        visited[nx, ny] = true;
                        parent[next] = current;
                        queue.Enqueue(next);
                    }
                }
            }

            return null; // Yol yok
        }
        private void BtnOlustur_Click(object sender, EventArgs e)
        {
            try
            {
                LabirentOlustur();
                var bfsYolu = BFS(labirent, new Point(0, 0), new Point(boyut - 1, boyut - 1));
                var dfsYolu = DFS(labirent, new Point(0, 0), new Point(boyut - 1, boyut - 1));
                CizLabirent(bfsYolu,dfsYolu);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        // Çözüm varsa yolu çiz, yoksa uyarı ver
        private void BtnCoz_Click(object sender, EventArgs e)
        {
            var yol = BFS(labirent, new Point(0, 0), new Point(boyut - 1, boyut - 1));
            //if (yol != null)
            //    CizLabirent(yol);

            // yolun kaç adımda bulunduğunu ifade eder. BFS de daha kısadır
            if (yol != null)
            {
                CizLabirent(yol, null);
                MessageBox.Show("BFS ile bulunan yol uzunluğu: " + yol.Count + " adım.");
            }
            else
                MessageBox.Show("Çözüm yolu bulunamadı!");

        }





        //            DFS                 //
        List<Point> DFS(int[,] maze, Point start, Point end)
        {
            Stack<Point> stack = new Stack<Point>();
            Dictionary<Point, Point> parent = new Dictionary<Point, Point>();
            bool[,] visited = new bool[boyut, boyut];
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { 1, 0, -1, 0 };

            stack.Push(start);
            visited[start.X, start.Y] = true;

            while (stack.Count > 0)
            {
                Point current = stack.Pop();

                if (current == end)
                {
                    // çözüm bulunduysa geri takip et
                    List<Point> path = new List<Point>();
                    while (current != start)
                    {
                        path.Add(current);
                        current = parent[current];
                    }
                    path.Reverse();
                    return path;
                }

                for (int i = 0; i < 4; i++)
                {
                    int nx = current.X + dx[i];
                    int ny = current.Y + dy[i];

                    if (nx >= 0 && ny >= 0 && nx < boyut && ny < boyut && maze[nx, ny] == 0 && !visited[nx, ny])
                    {
                        Point next = new Point(nx, ny);
                        visited[nx, ny] = true;
                        parent[next] = current;
                        stack.Push(next);
                    }
                }
            }

            return null; // yol yoksa
        }
        void CizLabirent(List<Point> bfsYolu = null, List<Point> dfsYolu = null)
        {
            var pb = (PictureBox)this.Controls["pbLabirent"];
            Bitmap bmp = new Bitmap(pb.Width, pb.Height);
            Graphics g = Graphics.FromImage(bmp);
            int kutu = pb.Width / boyut;

            for (int i = 0; i < boyut; i++)
            {
                for (int j = 0; j < boyut; j++)
                {
                    Brush renk = (labirent[i, j] == 1) ? Brushes.Black : Brushes.White;
                    // Giriş noktası (0,0) → mavi
                    if (i == 0 && j == 0)
                        renk = Brushes.Blue;

                    // Çıkış noktası (boyut-1, boyut-1) → turuncu
                    if (i == boyut - 1 && j == boyut - 1)
                        renk = Brushes.GreenYellow;
                    g.FillRectangle(renk, j * kutu, i * kutu, kutu, kutu);
                    g.DrawRectangle(Pens.Gray, j * kutu, i * kutu, kutu, kutu);
                }
            }

            if (dfsYolu != null)
            {
                foreach (var p in dfsYolu)
                {
                    g.FillRectangle(Brushes.Red, p.Y * kutu, p.X * kutu, kutu, kutu);
                }
            }

            if (bfsYolu != null)
            {
                foreach (var p in bfsYolu)
                {
                    g.FillRectangle(Brushes.Green, p.Y * kutu, p.X * kutu, kutu, kutu);
                }
            }
            pb.Image = bmp;
        }
        private void BtnDFS_Click(object sender, EventArgs e)
        {
            var dfsYolu = DFS(labirent, new Point(0, 0), new Point(boyut - 1, boyut - 1));
            //if (dfsYolu != null)
            //    CizLabirent(null, dfsYolu);

            // yolun kaç adımda bulunduğunu ifade eder. DFS de daha uzundur
            if (dfsYolu != null)
            {
                CizLabirent(null, dfsYolu);
                MessageBox.Show("DFS ile bulunan yol uzunluğu: " + dfsYolu.Count + " adım.");
            }
            else
                MessageBox.Show("DFS: Çözüm yolu bulunamadı!");
        }

    }
}
