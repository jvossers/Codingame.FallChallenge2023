internal class Program
{
    static void Main(string[] args)
    {
        var ctx = new GameContext(Console.ReadLine);

        while (true)
        {
            ctx.NextTurn(Console.ReadLine);

            var useLight = "0";

            // mark waypoint as visited
            ctx.Waypoints
                .Where(w => !w.Visited && w.X == ctx.Self.Drones.Single().X && w.Y == ctx.Self.Drones.Single().Y)
                .ToList()
                .ForEach(w =>
                {
                    w.Visited = true;
                    useLight = "1";
                });

            if (ctx.Waypoints.TrueForAll(w => w.Visited))
            {
                ctx.Waypoints.ToList().ForEach(w => w.Visited = false);
            }

            var wayPoint = ctx.Waypoints.FirstOrDefault(w => !w.Visited);

            if (wayPoint != null)
            {
                ctx.QueueCommand($"MOVE {wayPoint.X} {wayPoint.Y} {useLight}");
            }

            ctx.EndRound();
        }
    }
}