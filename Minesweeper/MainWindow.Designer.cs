using System;
using System.Windows.Forms;

namespace Minesweeper
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.GameItem = new System.Windows.Forms.MenuItem();
            this.newItem = new System.Windows.Forms.MenuItem();
            this.Sep1 = new System.Windows.Forms.MenuItem();
            this.BeginnerItem = new System.Windows.Forms.MenuItem();
            this.BeginnerButHarderItem = new System.Windows.Forms.MenuItem();
            this.IntermediateItem = new System.Windows.Forms.MenuItem();
            this.IntermediateButHarderItem = new System.Windows.Forms.MenuItem();
            this.ExpertItem = new System.Windows.Forms.MenuItem();
            this.ExpertButHarderItem = new System.Windows.Forms.MenuItem();
            this.CustomItem = new System.Windows.Forms.MenuItem();
            this.LoadFromItem = new System.Windows.Forms.MenuItem();
            this.Sep2 = new System.Windows.Forms.MenuItem();
            this.MarksItem = new System.Windows.Forms.MenuItem();
            this.SolvableItem = new System.Windows.Forms.MenuItem();
            this.HelpItem = new System.Windows.Forms.MenuItem();
            this.DebugItem = new System.Windows.Forms.MenuItem();
            this.FlagminesItem = new System.Windows.Forms.MenuItem();
            this.RevealItem = new System.Windows.Forms.MenuItem();
            this.WinItem = new System.Windows.Forms.MenuItem();
            this.Sep3 = new System.Windows.Forms.MenuItem();
            this.Solve = new System.Windows.Forms.MenuItem();
            this.AboutItem = new System.Windows.Forms.MenuItem();
            this.minefieldInstance = new Minesweeper.Minefield();
            this.minefieldBackDropInstance = new Minesweeper.MinefieldBackdrop();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.GameItem,
            this.HelpItem});
            // 
            // GameItem
            // 
            this.GameItem.Index = 0;
            this.GameItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.newItem,
            this.Sep1,
            this.BeginnerItem,
            this.BeginnerButHarderItem,
            this.IntermediateItem,
            this.IntermediateButHarderItem,
            this.ExpertItem,
            this.ExpertButHarderItem,
            this.CustomItem,
            this.LoadFromItem,
            this.Sep2,
            this.MarksItem,
            this.SolvableItem});
            this.GameItem.Text = "Game";
            // 
            // newItem
            // 
            this.newItem.Index = 0;
            this.newItem.Text = "New";
            this.newItem.Click += new System.EventHandler(this.New_Click);
            // 
            // Sep1
            // 
            this.Sep1.Index = 1;
            this.Sep1.Text = "-";
            // 
            // BeginnerItem
            // 
            this.BeginnerItem.Index = 2;
            this.BeginnerItem.Text = "Beginner(9x9, 10 mines)";
            this.BeginnerItem.Click += new System.EventHandler(this.Beginner_Click);
            // 
            // BeginnerButHarderItem
            // 
            this.BeginnerButHarderItem.Index = 3;
            this.BeginnerButHarderItem.Text = "Beginner, but harder(9x9, 35 mines)";
            this.BeginnerButHarderItem.Click += new System.EventHandler(this.BeginnerButHarder_Click);
            // 
            // IntermediateItem
            // 
            this.IntermediateItem.Index = 4;
            this.IntermediateItem.Text = "Intermediate(16x16, 40 mines)";
            this.IntermediateItem.Click += new System.EventHandler(this.Intermediate_Click);
            // 
            // IntermediateButHarderItem
            // 
            this.IntermediateButHarderItem.Index = 5;
            this.IntermediateButHarderItem.Text = "Intermediate, but harder(16x16, 100 mines)";
            this.IntermediateButHarderItem.Click += new System.EventHandler(this.IntermediateButHarder_Click);
            // 
            // ExpertItem
            // 
            this.ExpertItem.Index = 6;
            this.ExpertItem.Text = "Expert(30x16, 99 mines)";
            this.ExpertItem.Click += new System.EventHandler(this.Expert_Click);
            // 
            // ExpertButHarderItem
            // 
            this.ExpertButHarderItem.Index = 7;
            this.ExpertButHarderItem.Text = "Expert, but harder(30x16, 200 mines)";
            this.ExpertButHarderItem.Click += new System.EventHandler(this.ExpertButHarder_Click);
            // 
            // CustomItem
            // 
            this.CustomItem.Index = 8;
            this.CustomItem.Text = "Custom...";
            this.CustomItem.Click += new System.EventHandler(this.Custom_Click);
            // 
            // LoadFromItem
            // 
            this.LoadFromItem.Index = 9;
            this.LoadFromItem.Text = "Load from image...";
            this.LoadFromItem.Click += new System.EventHandler(this.LoadFrom_Click);
            // 
            // Sep2
            // 
            this.Sep2.Index = 10;
            this.Sep2.Text = "-";
            // 
            // MarksItem
            // 
            this.MarksItem.Index = 11;
            this.MarksItem.Text = "Marks(?)";
            this.MarksItem.Click += new System.EventHandler(this.MarksItem_Click);
            // 
            // SolvableItem
            // 
            this.SolvableItem.Index = 12;
            this.SolvableItem.Text = "Generate solvable fields only";
            this.SolvableItem.Checked = Properties.Settings.Default.alwaysSolvableEnabled;
            this.SolvableItem.Click += new System.EventHandler(this.SolvableItem_Click);
            // 
            // HelpItem
            // 
            this.HelpItem.Index = 1;
            this.HelpItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.DebugItem,
            this.FlagminesItem,
            this.RevealItem,
            this.WinItem,
            this.Solve,
            this.Sep3,
            this.AboutItem});
            this.HelpItem.Text = "Help";
            // 
            // DebugItem
            // 
            this.DebugItem.Index = 0;
            this.DebugItem.Text = "Debug";
            this.DebugItem.Click += new System.EventHandler(this.Debug_Click);
            // 
            // FlagminesItem
            // 
            this.FlagminesItem.Index = 1;
            this.FlagminesItem.Text = "Flag all mines";
            this.FlagminesItem.Click += new System.EventHandler(this.FlagMines_Click);
            // 
            // RevealItem
            // 
            this.RevealItem.Index = 2;
            this.RevealItem.Text = "Reveal";
            this.RevealItem.Click += new System.EventHandler(this.RevealItem_Click);
            // 
            // WinItem
            // 
            this.WinItem.Index = 3;
            this.WinItem.Text = "Win";
            this.WinItem.Click += new System.EventHandler(this.Win_Click);
            // 
            // SolveItem
            // 
            this.Solve.Index = 4;
            this.Solve.Text = "Solve";
            this.Solve.Click += new System.EventHandler(this.SolveAction);
            // 
            // Sep3
            // 
            this.Sep3.Index = 5;
            this.Sep3.Text = "-";
            // 
            // AboutItem
            // 
            this.AboutItem.Index = 6;
            this.AboutItem.Text = "About";
            this.AboutItem.Click += AboutItem_Click;
            // 
            // minefieldInstance
            // 
            this.minefieldInstance.DebugView = false;
            this.minefieldInstance.FieldSize = new System.Drawing.Size(16, 16);
            this.minefieldInstance.Location = new System.Drawing.Point(12, 55);
            this.minefieldInstance.Name = "minefieldInstance";
            this.minefieldInstance.Size = new System.Drawing.Size(256, 256);
            this.minefieldInstance.TabIndex = 0;
            this.minefieldInstance.Gamestart += new System.EventHandler(this.minefieldInstance_Gamestart);
            this.minefieldInstance.TileClick += new System.EventHandler(this.minefieldInstance_TileClick);
            this.minefieldInstance.TileFlag += new System.EventHandler(this.minefieldInstance_TileFlag);
            this.minefieldInstance.GameOver += new Minesweeper.Minefield.GameOverHandler(this.minefieldInstance_GameOver);
            this.minefieldInstance.MouseDown += new System.Windows.Forms.MouseEventHandler(this.minefieldInstance_MouseDown);
            this.minefieldInstance.MouseUp += new System.Windows.Forms.MouseEventHandler(this.minefieldInstance_MouseUp);
            this.minefieldInstance.StartGenerating += new System.EventHandler(this.minefieldInstance_OnStartGenerating);
            this.minefieldInstance.EndGenerating += new System.EventHandler(this.minefieldInstance_OnEndGenerating);
            // 
            // minefieldBgroundInstance
            // 
            this.minefieldBackDropInstance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minefieldBackDropInstance.face = ((byte)(0));
            this.minefieldBackDropInstance.Location = new System.Drawing.Point(0, 0);
            this.minefieldBackDropInstance.Name = "BackgroundInstance";
            this.minefieldBackDropInstance.Size = new System.Drawing.Size(276, 235);
            this.minefieldBackDropInstance.TabIndex = 3;
            this.minefieldBackDropInstance.FaceClicked += new System.EventHandler(this.New_Click);
            this.minefieldBackDropInstance.MouseDown += new System.Windows.Forms.MouseEventHandler(this.minefieldBackdrop1_MouseDown);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(276, 235);
            this.Controls.Add(this.minefieldInstance);
            this.Controls.Add(this.minefieldBackDropInstance);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.Name = "MainWindow";
            this.Text = "Minesweeper";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion

        private Minefield minefieldInstance;
        private MinefieldBackdrop minefieldBackDropInstance;
        private MenuItem GameItem;
        private MenuItem newItem;
        private MenuItem Sep1;
        private MenuItem BeginnerItem;
        private MenuItem BeginnerButHarderItem;
        private MenuItem IntermediateItem;
        private MenuItem IntermediateButHarderItem;
        private MenuItem ExpertItem;
        private MenuItem ExpertButHarderItem;
        private MenuItem CustomItem;
        private MenuItem HelpItem;
        private MenuItem DebugItem;
        private MenuItem LoadFromItem;
        private MenuItem FlagminesItem;
        private MenuItem RevealItem;
        private MenuItem WinItem;
        private MenuItem Sep2;
        private MenuItem MarksItem;
        private MenuItem SolvableItem;
        private MenuItem Sep3;
        private MenuItem AboutItem;
        private MenuItem Solve;
        public System.Windows.Forms.MainMenu mainMenu1;
    }
}

