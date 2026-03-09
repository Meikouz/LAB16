using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LAB16
{
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