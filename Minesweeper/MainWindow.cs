using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Minesweeper
{
    public partial class MainWindow : Form
    {
        #region non-static attributes
        private DateTime GameStartedAt;
        private int delta = 0;
        private bool newgame = false;
        private string file = "";
        private BackgroundWorker fieldGeneratorWorker = new BackgroundWorker();
        #endregion

        #region getters and setters

        private int TimeChange
        {
            get { return delta; }
            set { delta = value; try { this.Invoke((MethodInvoker)delegate { minefieldBackDropInstance.Timer = value; }); } catch { } }
        }

        #endregion

        #region constructors and overrides
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region events

        protected override void OnResize(EventArgs e)
        {
            if ((this.Width - 36) / minefieldInstance.FieldSize.Width >= 8 || (this.Height - 121) / minefieldInstance.FieldSize.Height >= 8)
            {
                if ((this.Width - 36) / minefieldInstance.FieldSize.Width >= (this.Height - 121) / minefieldInstance.FieldSize.Height)
                    minefieldInstance.cSize = (this.Width - 36) / minefieldInstance.FieldSize.Width;
                else minefieldInstance.cSize = (this.Height - 121) / minefieldInstance.FieldSize.Height;
            }
            this.Width = minefieldInstance.FieldSize.Width * minefieldInstance.cSize + 36;
            this.Height = minefieldInstance.FieldSize.Height * minefieldInstance.cSize + 121;
            minefieldInstance.Width = minefieldInstance.FieldSize.Width * minefieldInstance.cSize;
            minefieldInstance.Height = minefieldInstance.FieldSize.Height * minefieldInstance.cSize;
            this.Refresh();
        }

        private void Custom_Click(object sender, EventArgs e)
        {
            CreateField GetInput = new CreateField(minefieldInstance.FieldSize.Width, minefieldInstance.FieldSize.Height, minefieldInstance.MineCount);
            if (GetInput.ShowDialog() == DialogResult.OK)
            {
                minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_NOT_OVER;
                BeginnerItem.Checked = false;
                BeginnerButHarderItem.Checked = false;
                IntermediateItem.Checked = false;
                IntermediateButHarderItem.Checked = false;
                ExpertItem.Checked = false;
                ExpertButHarderItem.Checked = false;
                CustomItem.Checked = true;
                LoadFromItem.Checked = false;
                generateNewField(GetInput.mines, new Size(GetInput.width, GetInput.height));
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            minefieldInstance.Parent = this;
            minefieldBackDropInstance.Parent = this;
            minefieldInstance.questionMarkEnabled = Properties.Settings.Default.questionMarkEnabled;
            MarksItem.Checked = minefieldInstance.questionMarkEnabled;
            Intermediate_Click(null, null);
            System.Threading.ThreadPool.QueueUserWorkItem(mouseLeftUpChecker);
        }

        private void New_Click(object sender, EventArgs e)
        {
            minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_NOT_OVER;
            if (!LoadFromItem.Checked)
            {
                generateNewField(minefieldInstance.MineCount, minefieldInstance.FieldSize);
            }
            else if (System.IO.File.Exists(file))
            {
                minefieldBackDropInstance.Refresh();
                minefieldInstance.GenerateFieldfromInput(new Bitmap(file));
                this.Width = minefieldInstance.FieldSize.Width * minefieldInstance.cSize + 36;
                this.Height = minefieldInstance.FieldSize.Height * minefieldInstance.cSize + 121;
                minefieldInstance.Refresh();
                minefieldBackDropInstance.Refresh();
            }
        }

        private void Beginner_Click(object sender, EventArgs e)
        {
            minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_NOT_OVER;
            BeginnerItem.Checked = true;
            BeginnerButHarderItem.Checked = false;
            IntermediateItem.Checked = false;
            IntermediateButHarderItem.Checked = false;
            ExpertItem.Checked = false;
            ExpertButHarderItem.Checked = false;
            CustomItem.Checked = false;
            LoadFromItem.Checked = false;
            generateNewField(10, new Size(9, 9));
        }
        
        private void BeginnerButHarder_Click(object sender, EventArgs e)
        {
            minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_NOT_OVER;
            BeginnerItem.Checked = false;
            BeginnerButHarderItem.Checked = true;
            IntermediateItem.Checked = false;
            IntermediateButHarderItem.Checked = false;
            ExpertItem.Checked = false;
            ExpertButHarderItem.Checked = false;
            CustomItem.Checked = false;
            LoadFromItem.Checked = false;
            generateNewField(35, new Size(9, 9));
        }

        private void Intermediate_Click(object sender, EventArgs e)
        {
            minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_NOT_OVER;
            BeginnerItem.Checked = false;
            BeginnerButHarderItem.Checked = false;
            IntermediateItem.Checked = true;
            IntermediateButHarderItem.Checked = false;
            ExpertItem.Checked = false;
            ExpertButHarderItem.Checked = false;
            CustomItem.Checked = false;
            LoadFromItem.Checked = false;
            generateNewField(40, new Size(16, 16));
        }
        
        private void IntermediateButHarder_Click(object sender, EventArgs e)
        {
            minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_NOT_OVER;
            BeginnerItem.Checked = false;
            BeginnerButHarderItem.Checked = false;
            IntermediateItem.Checked = false;
            IntermediateButHarderItem.Checked = true;
            ExpertItem.Checked = false;
            ExpertButHarderItem.Checked = false;
            CustomItem.Checked = false;
            LoadFromItem.Checked = false;
            generateNewField(100, new Size(16, 16));
        }

        private void Expert_Click(object sender, EventArgs e)
        {
            minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_NOT_OVER;
            BeginnerItem.Checked = false;
            BeginnerButHarderItem.Checked = false;
            IntermediateItem.Checked = false;
            IntermediateButHarderItem.Checked = false;
            ExpertItem.Checked = true;
            ExpertButHarderItem.Checked = false;
            CustomItem.Checked = false;
            LoadFromItem.Checked = false;
            generateNewField(99, new Size(30, 16));
        }
       
        private void ExpertButHarder_Click(object sender, EventArgs e)
        {
            minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_NOT_OVER;
            BeginnerItem.Checked = false;
            BeginnerButHarderItem.Checked = false;
            IntermediateItem.Checked = false;
            IntermediateButHarderItem.Checked = false;
            ExpertItem.Checked = false;
            ExpertButHarderItem.Checked = true;
            CustomItem.Checked = false;
            LoadFromItem.Checked = false;
            generateNewField(200, new Size(30, 16));
        }

        private void Debug_Click(object sender, EventArgs e)
        {
            DebugItem.Checked = !DebugItem.Checked;
            minefieldInstance.DebugView = !minefieldInstance.DebugView;
        }

        private void minefieldInstance_Gamestart(object sender, EventArgs e)
        {
            minefieldBackDropInstance.MinesHidden = minefieldInstance.MinesHidden;
            TimeChange = 0;
            newgame = true;
        }
        
        private void minefieldInstance_TileClick(object sender, EventArgs e)
        {
            if (newgame)
            {
                newgame = false;
                GameStartedAt = DateTime.Now - TimeSpan.FromSeconds(1);
                System.Threading.ThreadPool.QueueUserWorkItem(CountSeconds);
            }
        }
        
        private void minefieldInstance_TileFlag(object sender, EventArgs e)
        {
            minefieldBackDropInstance.MinesHidden = minefieldInstance.MinesHidden;
        }

        private void minefieldInstance_MouseDown(object sender, MouseEventArgs e)
        {
            if (!minefieldInstance.IsGameOver && e.Button == MouseButtons.Left)
                minefieldBackDropInstance.face = MinefieldBackdrop.FACE_MOUSE_DOWN;
        }

        private void minefieldInstance_OnStartGenerating(object sender, EventArgs e)
        {
            minefieldBackDropInstance.face = MinefieldBackdrop.FACE_WAITING;
            minefieldBackDropInstance.BlockedInteraction = minefieldInstance.BlockedInteraction = true;
            foreach (MenuItem mi in mainMenu1.MenuItems)
            {
                mi.Enabled = false;
            }
        }

        private void minefieldInstance_OnEndGenerating(object sender, EventArgs e)
        {
            minefieldBackDropInstance.face = MinefieldBackdrop.FACE_NORMAL;
            minefieldBackDropInstance.BlockedInteraction = minefieldInstance.BlockedInteraction = false;
            foreach (MenuItem mi in mainMenu1.MenuItems)
            {
                mi.Enabled = true;
            }
        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            minefieldInstance.mouseEnterDownEnabled = true;
        }

        private void minefieldBackdrop1_MouseDown(object sender, MouseEventArgs e)
        {
            minefieldInstance.mouseEnterDownEnabled = true;
        }

        private void minefieldInstance_GameOver(object sender, Minefield.GameOverArgs e)
        {
            if (e.won)
            {
                minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_OVER_WON;
            }
            else
            {
                minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_OVER_LOST;
            }
        }

        private void minefieldInstance_MouseUp(object sender, MouseEventArgs e)
        {
            if (minefieldBackDropInstance.face == MinefieldBackdrop.FACE_MOUSE_DOWN)
            {
                minefieldBackDropInstance.face = MinefieldBackdrop.FACE_NORMAL;
            }
        }

        private void LoadFrom_Click(object sender, EventArgs e)
        {
            OpenFileDialog k = new OpenFileDialog();
            k.FileName = @"C:\";
            if (k.ShowDialog() == DialogResult.OK && System.IO.File.Exists(k.FileName))
            {
                file = k.FileName;
                try
                {
                    minefieldInstance.GenerateFieldfromInput(new Bitmap(k.FileName));
                    BeginnerItem.Checked = false;
                    BeginnerButHarderItem.Checked = false;
                    IntermediateItem.Checked = false;
                    IntermediateButHarderItem.Checked = false;
                    ExpertItem.Checked = false;
                    ExpertButHarderItem.Checked = false;
                    CustomItem.Checked = false;
                    LoadFromItem.Checked = true;
                    minefieldBackDropInstance.gameState = MinefieldBackdrop.GAME_NOT_OVER;
                    this.Width = minefieldInstance.FieldSize.Width * minefieldInstance.cSize + 36;
                    this.Height = minefieldInstance.FieldSize.Height * minefieldInstance.cSize + 121;
                    minefieldBackDropInstance.Width = this.Width;
                    minefieldBackDropInstance.Height = this.Height;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                minefieldInstance.Refresh();
                minefieldBackDropInstance.Refresh();
            }
        }

        private void FlagMines_Click(object sender, EventArgs e)
        {
            minefieldInstance.FlagAllMines();
            minefieldBackDropInstance.MinesHidden = minefieldInstance.MinesHidden;
        }

        private void RevealItem_Click(object sender, EventArgs e)
        {
            minefieldInstance.RevealCluesAndBlanks();
        }

        private void Win_Click(object sender, EventArgs e)
        {
            minefieldInstance.RevealBoard(true);
        }

        private void MarksItem_Click(object sender, EventArgs e)
        {
            MarksItem.Checked = !MarksItem.Checked;
            minefieldInstance.questionMarkEnabled = MarksItem.Checked;
            Properties.Settings.Default.questionMarkEnabled = MarksItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void SolvableItem_Click(object sender, EventArgs e)
        {
            SolvableItem.Checked = !SolvableItem.Checked;
            Properties.Settings.Default.alwaysSolvableEnabled = SolvableItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Minesweeper clone with some tweaks, made by yours truly." + Environment.NewLine + Environment.NewLine + "Try left-clicking on a clue. It should reveal its unflagged neighbors, if you have enough flags around it", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SolveAction(object sender, EventArgs e)
        {
            minefieldInstance.RunVisualSolver();
        }
        #endregion

        #region separate threads

        //Constantly checks wether the left mouse has been released
        //if so, set the face in the Background back to normal

        private void mouseLeftUpChecker(object threadContext)
        {
            while (true)
            {
                if (!Control.MouseButtons.HasFlag(MouseButtons.Left))
                {
                    minefieldInstance.mouseEnterDownEnabled = false;
                    if (minefieldBackDropInstance.face == MinefieldBackdrop.FACE_MOUSE_DOWN)
                    {
                        minefieldBackDropInstance.MainThreadInvoke(() => {
                            minefieldBackDropInstance.face = MinefieldBackdrop.FACE_NORMAL;
                        });
                    }
                }
                System.Threading.Thread.Sleep(50);
            }
        }

        //Counts up the game timer displayed in minefieldBackdrop

        private void CountSeconds(object threadContext)
        {
            while (!minefieldInstance.IsGameOver && !newgame)
            {
                if (TimeChange < 0) return;
                else if (TimeChange > 999) { TimeChange = 999; return; }
                TimeChange = (int)(DateTime.Now - GameStartedAt).TotalSeconds;
                System.Threading.Thread.Sleep(500);
            }
        }
        #endregion

        #region private functions

        //method used to generate the game

        public void generateNewField(int bombCount, Size newSize)
        {
            minefieldBackDropInstance.face = MinefieldBackdrop.FACE_WAITING;
            minefieldBackDropInstance.BlockedInteraction = minefieldInstance.BlockedInteraction = true;
            minefieldInstance.FieldSize = newSize;
            minefieldInstance.GenerateField(bombCount);
            this.Width = minefieldInstance.FieldSize.Width * minefieldInstance.cSize + 36;
            this.Height = minefieldInstance.FieldSize.Height * minefieldInstance.cSize + 121;
            minefieldBackDropInstance.Width = this.Width;
            minefieldBackDropInstance.Height = this.Height;
            minefieldBackDropInstance.face = MinefieldBackdrop.FACE_NORMAL;
            minefieldBackDropInstance.BlockedInteraction = minefieldInstance.BlockedInteraction = false;
        }
        #endregion
    }
}
