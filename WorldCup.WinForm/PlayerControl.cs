using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldCup.Data.Models;

namespace WorldCup.WinForm
{
    public partial class PlayerControl : UserControl
    {


        public Player PlayerData { get; set; }
        public bool IsFavourite { get; set; }


        //for starting the drag and drop process
        private bool _isDragging = false;
        private Point _dragStartPoint; //coordinates of a player you choose


        public PlayerControl(Player playerData, bool isFavourite)
        {
            InitializeComponent();
            PlayerData = playerData;
            IsFavourite = isFavourite;

            lblPlayerInfo.Text = $"{PlayerData.ShirtNumber} - {PlayerData.Name} ({PlayerData.Position})" +
            (PlayerData.Captain ? " 🧢" : "");

            // try to find player image
            string fileName = PlayerData.Name.ToLower().Replace(" ", "_") + ".jpg";
            string fullPath = Path.Combine("Assets", fileName);
            string fallbackPath = Path.Combine("Assets", "personDefault.jpg");

            if (File.Exists(fullPath))
            {
                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    playerImage.Image = new Bitmap(Image.FromStream(stream));
                }
            }
            else
            {
                using (var stream = new FileStream(fallbackPath, FileMode.Open, FileAccess.Read))
                {
                    playerImage.Image = new Bitmap(Image.FromStream(stream));
                }
            }

            this.MouseDown += PlayerControl_MouseDown;
            this.MouseMove += PlayerControl_MouseMove;
        }


        private void PlayerControl_MouseDown(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"PC MOUSE DOWN e {e}");

            _isDragging = true;
            _dragStartPoint = e.Location;
        }

        private void PlayerControl_MouseMove(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"PC MOUSE MOVE e {e}");

            if (_isDragging && e.Button == MouseButtons.Left)
            {
                _isDragging = false;
                DoDragDrop(this, DragDropEffects.Move);
            }

            System.Diagnostics.Debug.WriteLine(e.Button);
        }


        public void SetFavourite(bool favorite)
        {
            IsFavourite = favorite;
            //picStar.Visible = favorite;
        }

        private void PlayerControl_Load(object sender, EventArgs e)
        {

        }

        private void lblPlayerInfo_Click(object sender, EventArgs e)
        {

        }

        private void playerImage_Click(object sender, EventArgs e)
        {

        }
    }
}
