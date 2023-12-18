public class Player
{
    public int Score { get; set; }
    public List<Creature> Scans { get; set; }
    public List<Drone> Drones { get; set; }

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
    }

    public int Id { get; set; }
    public int Emergency { get; set; }
    public int Battery { get; set; }
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

public class Route
{
    public Route(double distance, Creature target)
    {
        Target = target;
        Distance = distance;
    }

    public double Score { get; set; }
    public double Distance { get; }
    public Creature Target { get; }
}

public class GameContext
{
    public List<Creature> Creatures { get; }
    public Player Self { get; }
    public Player Opponent { get; }
    public List<string> Commands { get; }
    public int RoundCounter { get; private set; }

    

    public GameContext(Func<string> readLine)
    {
        Commands = new List<string>();
        Self = new Player();
        Opponent = new Player();
        Creatures = new List<Creature>();

        var creatureCount = int.Parse(readLine());

        for (int i = 0; i < creatureCount; i++)
        {
            Creatures.Add(new Creature(readLine));
        }
    }

    public List<Route> GetRoutes(Drone drone)
    {
        var routes = new List<Route>();

        foreach (var creature in Creatures)
        {
            var route = new Route(drone.DistanceTo(creature), creature);
            route.Score = CalculateScore(route);

            // TODO: score should be based on how many points it would give me as wel as distance 

            routes.Add(route);
        }

        return routes;
    }

    public double CalculateScore(Route route)
    {
        if (Self.Scans.Any(s => s.Id == route.Target.Id))
        {
            // already scanned
            return 0;
        }

        return Position.TopLeft.DistanceTo(Position.BottomRight) - route.Distance;
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

        #region ignore for now
        int radarBlipCount = int.Parse(readLine());
        for (int i = 0; i < radarBlipCount; i++)
        {
            var inputs = readLine().Split(' ');
            int droneId = int.Parse(inputs[0]);
            int creatureId = int.Parse(inputs[1]);
            string radar = inputs[2];
        }
        #endregion
    }



    public void QueueCommand(string command)
    {
        Commands.Add(command);
    }

    public void EndRound()
    {
        if (!Commands.Any())
        {
            Commands.Add("WAIT 1");
        }
        Console.WriteLine(String.Join(";", Commands));
        Commands.Clear();
        RoundCounter++;
    }
}