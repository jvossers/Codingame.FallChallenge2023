public class Player
{
    public int Score { get; set; }
    public List<Creature> Scans { get; set; }
    public List<Drone> Drones { get; set; }
    public Drone PrimaryDrone => Drones[0];
    public Drone SecondaryDrone => Drones[1];

    public Player()
    {
        Score = 0;
        Scans = new List<Creature>();
        Drones = new List<Drone>();
    }
}

public class Position : IPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public double DistanceTo(IPosition subject)
    {
        int deltaX = subject.X - this.X;
        int deltaY = subject.Y - this.Y;

        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public static Position TopLeft => new Position() { X = 0, Y = 0 };
    public static Position BottomRight => new Position() { X = 9999, Y = 9999 };
}

public interface IPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public double DistanceTo(IPosition subject);
}

public class Drone : Position
{
    public Drone(Func<string> readLine)
    {
        var droneLine = readLine().Split(' ');
        Id = int.Parse(droneLine[0]);
        X = int.Parse(droneLine[1]);
        Y = int.Parse(droneLine[2]);
        Emergency = int.Parse(droneLine[3]);
        Battery = int.Parse(droneLine[4]);
        Blips = new List<Blip>();
    }

    public int Id { get; set; }
    public int Emergency { get; set; }
    public int Battery { get; set; }
    public string Command { get; set; }
    public List<Blip> Blips { get; set; }
}

public enum BlipDirection
{
    TopLeft,
    TopRight,
    BottomRight,
    BottomLeft
}


public class Blip
{
    public Creature Creature { get; set; }
    public BlipDirection Direction { get; set; }
}

public class Creature : Position
{
    public Creature(Func<string> readLine)
    {
        var creatureLine = readLine().Split(' ');
        Id = int.Parse(creatureLine[0]);
        Colour = int.Parse(creatureLine[1]);
        Type = int.Parse(creatureLine[2]);
    }

    public int Id { get; set; }
    public int Colour { get; set; }
    public int Type { get; set; }
    public int Vx { get; set; }
    public int Vy{ get; set; }
}

public class Waypoint : Position
{
    public bool Visited { get; set; }
    public Waypoint()
    {
        Visited = false;
    }
}

public enum WorkflowState
{
    Descending,
    Converging,
    Surfacing
}

public class GameContext
{
    public List<Creature> Creatures { get; }
    public Player Self { get; }
    public Player Opponent { get; }
    public List<string> Commands { get; }
    public int RoundCounter { get; private set; }

    public WorkflowState CurrentState { get; set; }

    public GameContext(Func<string> readLine)
    {
        Commands = new List<string>();
        Self = new Player();
        Opponent = new Player();
        Creatures = new List<Creature>();
        CurrentState = WorkflowState.Descending;
       
        var creatureCount = int.Parse(readLine());

        for (int i = 0; i < creatureCount; i++)
        {
            Creatures.Add(new Creature(readLine));
        }
    }

    public void NextTurn(Func<string> readLine)
    {
        Self.Scans.Clear();
        Self.Drones.Clear();
        Self.Score = int.Parse(readLine());

        Opponent.Scans.Clear();
        Opponent.Drones.Clear();
        Opponent.Score = int.Parse(readLine());

        int myScanCount = int.Parse(readLine());
        for (int i = 0; i < myScanCount; i++)
        {
            int creatureId = int.Parse(readLine());
            var creature = Creatures.Single(c => c.Id == creatureId);
            Self.Scans.Add(creature);
        }

        int opponentScanCount = int.Parse(readLine());
        for (int i = 0; i < opponentScanCount; i++)
        {
            int creatureId = int.Parse(readLine());
            var creature = Creatures.Single(c => c.Id == creatureId);
            Opponent.Scans.Add(creature);
        }

        int myDroneCount = int.Parse(readLine());
        for (int i = 0; i < myDroneCount; i++)
        {
            var drone = new Drone(readLine);
            Self.Drones.Add(drone);
        }

        int opponentDroneCount = int.Parse(readLine());
        for (int i = 0; i < opponentDroneCount; i++)
        {
            var drone = new Drone(readLine);
            Opponent.Drones.Add(drone);
        }

        #region ignore for now
        int droneScanCount = int.Parse(readLine());
        for (int i = 0; i < droneScanCount; i++)
        {
            var inputs = readLine().Split(' ');
            int droneId = int.Parse(inputs[0]);
            int creatureId = int.Parse(inputs[1]);
        }
        #endregion

        int visibleCreatureCount = int.Parse(readLine());
        for (int i = 0; i < visibleCreatureCount; i++)
        {
            var inputs = readLine().Split(' ');
            int creatureId = int.Parse(inputs[0]);
            int creatureX = int.Parse(inputs[1]);
            int creatureY = int.Parse(inputs[2]);
            int creatureVx = int.Parse(inputs[3]);
            int creatureVy = int.Parse(inputs[4]);

            var creature = Creatures.Single(c => c.Id == creatureId);
            creature.X = creatureX;
            creature.Y = creatureY;
            creature.Vx = creatureVx;
            creature.Vy = creatureVy;
        }
        
        int radarBlipCount = int.Parse(readLine());
        for (int i = 0; i < radarBlipCount; i++)
        {
            var inputs = readLine().Split(' ');
            int droneId = int.Parse(inputs[0]);
            int creatureId = int.Parse(inputs[1]);
            string radar = inputs[2];

            var creature = Creatures.Single(c => c.Id == creatureId);
            var blipDirection = radar switch
            {
                "BL" => BlipDirection.BottomLeft,
                "TL" => BlipDirection.TopLeft,
                "BR" => BlipDirection.BottomRight,
                "TR" => BlipDirection.TopRight,
                _ => throw new ArgumentOutOfRangeException()
            };

            var blip = new Blip() { Creature = creature, Direction = blipDirection };

            Self.Drones.Single(d => d.Id == droneId).Blips.Add(blip);
        }
    }

    public void QueueCommand(string command)
    {
        Commands.Add(command);
    }

    public void EndRound()
    {
        if (!Commands.Any())
        {
            Commands.Add("WAIT 0");
            Commands.Add("WAIT 0");
        }

        foreach (var command in Commands)
        {
            Console.WriteLine(command);
        }

        Commands.Clear();
        RoundCounter++;
    }
}