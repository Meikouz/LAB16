using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LAB16
{
    public partial class Form1 : Form
    {
        private Game game;

        private readonly Label[,] cells = new Label[7, 7];

        private Label lblHeroHP;
        private Label lblEnemyHP;
        private Label lblDamage;
        private Label lblCoins;
        private Label lblPosition;

        private ListBox lstInventory;
        private ListBox lstLog;

        private Button btnUp;
        private Button btnDown;
        private Button btnLeft;
        private Button btnRight;
        private Button btnAttack;
        private Button btnPick;
        private Button btnRestart;

        public Form1()
        {
            InitializeComponent();
            InitializeGameUi();

            game = new Game(UpdateView, AddLog);
            game.ResetGame();
        }

        private void InitializeGameUi()
        {
            Text = "Пригодницька гра — патерн Медіатор";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(980, 620);
            MinimumSize = new Size(980, 620);

            Controls.Clear();

            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.RowCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            Controls.Add(mainLayout);

            TableLayoutPanel mapGrid = new TableLayoutPanel();
            mapGrid.Dock = DockStyle.Fill;
            mapGrid.ColumnCount = 7;
            mapGrid.RowCount = 7;
            mapGrid.Margin = new Padding(10);
            mapGrid.BackColor = Color.Black;

            for (int i = 0; i < 7; i++)
            {
                mapGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 7F));
                mapGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 7F));
            }

            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    Label cell = new Label();
                    cell.Dock = DockStyle.Fill;
                    cell.Margin = new Padding(1);
                    cell.TextAlign = ContentAlignment.MiddleCenter;
                    cell.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
                    cell.BackColor = Color.WhiteSmoke;
                    cell.Text = ".";
                    cells[x, y] = cell;
                    mapGrid.Controls.Add(cell, x, y);
                }
            }

            mainLayout.Controls.Add(mapGrid, 0, 0);

            TableLayoutPanel rightLayout = new TableLayoutPanel();
            rightLayout.Dock = DockStyle.Fill;
            rightLayout.ColumnCount = 1;
            rightLayout.RowCount = 5;
            rightLayout.Padding = new Padding(10);
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 145));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
            mainLayout.Controls.Add(rightLayout, 1, 0);

            rightLayout.Controls.Add(CreateStatsBox(), 0, 0);
            rightLayout.Controls.Add(CreateMoveBox(), 0, 1);
            rightLayout.Controls.Add(CreateActionBox(), 0, 2);
            rightLayout.Controls.Add(CreateInventoryBox(), 0, 3);
            rightLayout.Controls.Add(CreateLogBox(), 0, 4);
        }

        private GroupBox CreateStatsBox()
        {
            GroupBox box = new GroupBox();
            box.Text = "Стан гри";
            box.Dock = DockStyle.Fill;

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = 1;
            panel.RowCount = 5;

            lblHeroHP = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            lblEnemyHP = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            lblDamage = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            lblCoins = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            lblPosition = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };

            panel.Controls.Add(lblHeroHP, 0, 0);
            panel.Controls.Add(lblEnemyHP, 0, 1);
            panel.Controls.Add(lblDamage, 0, 2);
            panel.Controls.Add(lblCoins, 0, 3);
            panel.Controls.Add(lblPosition, 0, 4);

            box.Controls.Add(panel);
            return box;
        }

        private GroupBox CreateMoveBox()
        {
            GroupBox box = new GroupBox();
            box.Text = "Рух";
            box.Dock = DockStyle.Fill;

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = 3;
            panel.RowCount = 2;

            for (int i = 0; i < 3; i++)
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            btnUp = new Button() { Text = "↑", Dock = DockStyle.Fill };
            btnLeft = new Button() { Text = "←", Dock = DockStyle.Fill };
            btnDown = new Button() { Text = "↓", Dock = DockStyle.Fill };
            btnRight = new Button() { Text = "→", Dock = DockStyle.Fill };

            btnUp.Click += (s, e) => game.Hero.Move(0, -1);
            btnLeft.Click += (s, e) => game.Hero.Move(-1, 0);
            btnDown.Click += (s, e) => game.Hero.Move(0, 1);
            btnRight.Click += (s, e) => game.Hero.Move(1, 0);

            panel.Controls.Add(btnUp, 1, 0);
            panel.Controls.Add(btnLeft, 0, 1);
            panel.Controls.Add(btnDown, 1, 1);
            panel.Controls.Add(btnRight, 2, 1);

            box.Controls.Add(panel);
            return box;
        }

        private GroupBox CreateActionBox()
        {
            GroupBox box = new GroupBox();
            box.Text = "Дії";
            box.Dock = DockStyle.Fill;

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.ColumnCount = 3;
            panel.RowCount = 1;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            btnAttack = new Button() { Text = "Атакувати", Dock = DockStyle.Fill };
            btnPick = new Button() { Text = "Підібрати", Dock = DockStyle.Fill };
            btnRestart = new Button() { Text = "Рестарт", Dock = DockStyle.Fill };

            btnAttack.Click += (s, e) => game.Hero.Attack();
            btnPick.Click += (s, e) => game.Hero.PickItem();
            btnRestart.Click += (s, e) =>
            {
                lstLog.Items.Clear();
                game.ResetGame();
            };

            panel.Controls.Add(btnAttack, 0, 0);
            panel.Controls.Add(btnPick, 1, 0);
            panel.Controls.Add(btnRestart, 2, 0);

            box.Controls.Add(panel);
            return box;
        }

        private GroupBox CreateInventoryBox()
        {
            GroupBox box = new GroupBox();
            box.Text = "Інвентар";
            box.Dock = DockStyle.Fill;

            lstInventory = new ListBox();
            lstInventory.Dock = DockStyle.Fill;

            box.Controls.Add(lstInventory);
            return box;
        }

        private GroupBox CreateLogBox()
        {
            GroupBox box = new GroupBox();
            box.Text = "Журнал подій";
            box.Dock = DockStyle.Fill;

            lstLog = new ListBox();
            lstLog.Dock = DockStyle.Fill;

            box.Controls.Add(lstLog);
            return box;
        }

        private void AddLog(string text)
        {
            lstLog.Items.Add(text);

            if (lstLog.Items.Count > 0)
                lstLog.TopIndex = lstLog.Items.Count - 1;
        }

        private void UpdateView()
        {
            if (game == null)
                return;

            lblHeroHP.Text = "HP героя: " + game.Hero.HP;
            lblEnemyHP.Text = "HP ворога: " + (game.Enemy.IsAlive ? game.Enemy.HP.ToString() : "0");
            lblDamage.Text = "Шкода героя: " + game.Hero.Damage;
            lblCoins.Text = "Монети: " + game.Inventory.Coins;
            lblPosition.Text = "Позиція героя: [" + game.Hero.Position.X + "; " + game.Hero.Position.Y + "]";

            lstInventory.Items.Clear();
            foreach (string itemName in game.Inventory.CollectedItems)
                lstInventory.Items.Add(itemName);

            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {
                    Label cell = cells[x, y];
                    cell.Text = ".";
                    cell.BackColor = Color.WhiteSmoke;
                }
            }

            foreach (Item item in game.Map.Items)
            {
                Label cell = cells[item.Position.X, item.Position.Y];
                cell.Text = GetItemSymbol(item.Type);

                switch (item.Type)
                {
                    case ItemType.Potion:
                        cell.BackColor = Color.LightPink;
                        break;
                    case ItemType.Crystal:
                        cell.BackColor = Color.LightCyan;
                        break;
                    case ItemType.Coin:
                        cell.BackColor = Color.Khaki;
                        break;
                }
            }

            if (game.Enemy.IsAlive)
            {
                Label enemyCell = cells[game.Enemy.Position.X, game.Enemy.Position.Y];
                enemyCell.Text = "E";
                enemyCell.BackColor = Color.LightCoral;
            }

            Label heroCell = cells[game.Hero.Position.X, game.Hero.Position.Y];
            heroCell.Text = "H";
            heroCell.BackColor = Color.LightGreen;

            bool canPlay = !game.IsGameOver;
            btnUp.Enabled = canPlay;
            btnDown.Enabled = canPlay;
            btnLeft.Enabled = canPlay;
            btnRight.Enabled = canPlay;
            btnAttack.Enabled = canPlay;
            btnPick.Enabled = canPlay;
        }

        private string GetItemSymbol(ItemType type)
        {
            switch (type)
            {
                case ItemType.Potion: return "P";
                case ItemType.Crystal: return "C";
                case ItemType.Coin: return "$";
                default: return "?";
            }
        }
    }

    public enum GameEventType
    {
        HeroMove,
        HeroAttack,
        EnemyAttack,
        PickItem
    }

    public interface IGameMediator
    {
        void Notify(GameColleague sender, GameEventType eventType, object data = null);
    }

    public abstract class GameColleague
    {
        protected IGameMediator mediator;

        public void SetMediator(IGameMediator mediator)
        {
            this.mediator = mediator;
        }
    }

    public class Hero : GameColleague
    {
        public Point Position { get; private set; }
        public int HP { get; private set; }
        public int Damage { get; private set; }

        public Hero(Point startPosition)
        {
            Reset(startPosition);
        }

        public void Move(int dx, int dy)
        {
            mediator.Notify(this, GameEventType.HeroMove, new Point(dx, dy));
        }

        public void Attack()
        {
            mediator.Notify(this, GameEventType.HeroAttack);
        }

        public void PickItem()
        {
            mediator.Notify(this, GameEventType.PickItem);
        }

        public void SetPosition(Point position)
        {
            Position = position;
        }

        public void ReceiveDamage(int value)
        {
            HP = Math.Max(0, HP - value);
        }

        public void Heal(int value)
        {
            HP = Math.Min(100, HP + value);
        }

        public void IncreaseDamage(int value)
        {
            Damage += value;
        }

        public void Reset(Point startPosition)
        {
            Position = startPosition;
            HP = 100;
            Damage = 20;
        }
    }

    public class Enemy : GameColleague
    {
        public Point Position { get; private set; }
        public int HP { get; private set; }
        public int Damage { get; private set; }

        public bool IsAlive
        {
            get { return HP > 0; }
        }

        public Enemy(Point startPosition)
        {
            Reset(startPosition);
        }

        public void AttackHero()
        {
            if (IsAlive)
                mediator.Notify(this, GameEventType.EnemyAttack);
        }

        public void ReceiveDamage(int value)
        {
            HP = Math.Max(0, HP - value);
        }

        public void Reset(Point startPosition)
        {
            Position = startPosition;
            HP = 100;
            Damage = 15;
        }
    }

    public enum ItemType
    {
        Potion,
        Crystal,
        Coin
    }

    public class Item
    {
        public Point Position { get; set; }
        public ItemType Type { get; set; }

        public string Name
        {
            get
            {
                switch (Type)
                {
                    case ItemType.Potion: return "Potion";
                    case ItemType.Crystal: return "Crystal";
                    case ItemType.Coin: return "Coin";
                    default: return "Unknown";
                }
            }
        }

        public Item(Point position, ItemType type)
        {
            Position = position;
            Type = type;
        }
    }

    public class GameMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private List<Item> items = new List<Item>();

        public IReadOnlyList<Item> Items
        {
            get { return items.AsReadOnly(); }
        }

        public GameMap(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public bool IsInside(Point point)
        {
            return point.X >= 0 && point.X < Width && point.Y >= 0 && point.Y < Height;
        }

        public Item GetItemAt(Point point)
        {
            return items.FirstOrDefault(i => i.Position == point);
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }

        public void ResetItems(List<Item> newItems)
        {
            items = newItems;
        }
    }

    public class Inventory
    {
        private readonly List<string> collectedItems = new List<string>();

        public IReadOnlyList<string> CollectedItems
        {
            get { return collectedItems.AsReadOnly(); }
        }

        public int Coins { get; private set; }

        public void AddItem(Item item)
        {
            collectedItems.Add(item.Name);

            if (item.Type == ItemType.Coin)
                Coins += 10;
        }

        public void Clear()
        {
            collectedItems.Clear();
            Coins = 0;
        }
    }

    public class Game : IGameMediator
    {
        private readonly Action refreshUi;
        private readonly Action<string> logMessage;
        private readonly Random random = new Random();

        private readonly Point heroStart = new Point(3, 3);

        public Hero Hero { get; private set; }
        public Enemy Enemy { get; private set; }
        public GameMap Map { get; private set; }
        public Inventory Inventory { get; private set; }

        public bool IsGameOver { get; private set; }

        public Game(Action refreshUi, Action<string> logMessage)
        {
            this.refreshUi = refreshUi;
            this.logMessage = logMessage;

            Hero = new Hero(heroStart);
            Enemy = new Enemy(new Point(5, 2));
            Map = new GameMap(7, 7);
            Inventory = new Inventory();

            Hero.SetMediator(this);
            Enemy.SetMediator(this);
        }

        public void ResetGame()
        {
            Hero.Reset(heroStart);
            Inventory.Clear();
            IsGameOver = false;

            HashSet<Point> used = new HashSet<Point>();
            used.Add(heroStart);

            Point enemyPos = GetFreePoint(used, 3);
            used.Add(enemyPos);
            Enemy.Reset(enemyPos);

            List<Item> newItems = new List<Item>();
            newItems.Add(new Item(GetFreePoint(used, 2), ItemType.Potion));
            used.Add(newItems[newItems.Count - 1].Position);

            newItems.Add(new Item(GetFreePoint(used, 2), ItemType.Crystal));
            used.Add(newItems[newItems.Count - 1].Position);

            newItems.Add(new Item(GetFreePoint(used, 2), ItemType.Coin));
            used.Add(newItems[newItems.Count - 1].Position);

            newItems.Add(new Item(GetFreePoint(used, 2), ItemType.Coin));
            used.Add(newItems[newItems.Count - 1].Position);

            Map.ResetItems(newItems);

            logMessage("Нова гра розпочата.");
            logMessage("Герой знаходиться в центрі мапи.");
            refreshUi();
        }

        private Point GetFreePoint(HashSet<Point> used, int minDistanceFromHero)
        {
            while (true)
            {
                Point p = new Point(random.Next(0, Map.Width), random.Next(0, Map.Height));

                if (used.Contains(p))
                    continue;

                if (ManhattanDistance(p, heroStart) < minDistanceFromHero)
                    continue;

                return p;
            }
        }

        public void Notify(GameColleague sender, GameEventType eventType, object data = null)
        {
            if (IsGameOver)
                return;

            switch (eventType)
            {
                case GameEventType.HeroMove:
                    HandleHeroMove(data);
                    break;

                case GameEventType.HeroAttack:
                    HandleHeroAttack();
                    break;

                case GameEventType.EnemyAttack:
                    HandleEnemyAttack();
                    break;

                case GameEventType.PickItem:
                    HandlePickItem();
                    break;
            }

            CheckGameOver();
            refreshUi();
        }

        private void HandleHeroMove(object data)
        {
            Point delta = (Point)data;
            Point newPosition = new Point(Hero.Position.X + delta.X, Hero.Position.Y + delta.Y);

            if (!Map.IsInside(newPosition))
            {
                logMessage("Герой не може вийти за межі мапи.");
                return;
            }

            if (Enemy.IsAlive && newPosition == Enemy.Position)
            {
                logMessage("На цій клітинці стоїть ворог. Спочатку атакуй його.");
                return;
            }

            Hero.SetPosition(newPosition);
            logMessage("Герой перемістився на клітинку [" + newPosition.X + "; " + newPosition.Y + "].");

            Item item = Map.GetItemAt(Hero.Position);
            if (item != null)
            {
                logMessage("На клітинці знайдено предмет: " + item.Name + ". Натисни 'Підібрати'.");
            }

            if (Enemy.IsAlive && IsAdjacent(Hero.Position, Enemy.Position))
            {
                logMessage("Ворог поруч і готується атакувати!");
                Enemy.AttackHero();
            }
        }

        private void HandleHeroAttack()
        {
            if (!Enemy.IsAlive)
            {
                logMessage("Ворог вже переможений.");
                return;
            }

            if (!IsAdjacent(Hero.Position, Enemy.Position))
            {
                logMessage("Ворог занадто далеко для атаки.");
                return;
            }

            Enemy.ReceiveDamage(Hero.Damage);
            logMessage("Герой атакував ворога на " + Hero.Damage + " HP.");

            if (!Enemy.IsAlive)
            {
                logMessage("Ворога переможено!");
                return;
            }

            logMessage("Ворог вижив і атакує у відповідь!");
            Enemy.AttackHero();
        }

        private void HandleEnemyAttack()
        {
            if (!Enemy.IsAlive)
                return;

            if (!IsAdjacent(Hero.Position, Enemy.Position))
                return;

            Hero.ReceiveDamage(Enemy.Damage);
            logMessage("Ворог атакував героя на " + Enemy.Damage + " HP.");
        }

        private void HandlePickItem()
        {
            Item item = Map.GetItemAt(Hero.Position);

            if (item == null)
            {
                logMessage("На цій клітинці немає предмета.");
                return;
            }

            Inventory.AddItem(item);

            switch (item.Type)
            {
                case ItemType.Potion:
                    Hero.Heal(25);
                    logMessage("Герой підібрав Potion і відновив 25 HP.");
                    break;

                case ItemType.Crystal:
                    Hero.IncreaseDamage(5);
                    logMessage("Герой підібрав Crystal. Шкода збільшилась на 5.");
                    break;

                case ItemType.Coin:
                    logMessage("Герой підібрав Coin. +10 монет.");
                    break;
            }

            Map.RemoveItem(item);
        }

        private void CheckGameOver()
        {
            if (Hero.HP <= 0)
            {
                IsGameOver = true;
                logMessage("Герой загинув. Гру завершено.");
            }
            else if (!Enemy.IsAlive)
            {
                IsGameOver = true;
                logMessage("Перемога! Основного ворога знищено.");
            }
        }

        private bool IsAdjacent(Point a, Point b)
        {
            return ManhattanDistance(a, b) == 1;
        }

        private int ManhattanDistance(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}