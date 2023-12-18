internal class Program
{
    static void Main(string[] args)
    {
        var ctx = new GameContext(Console.ReadLine);

        while (true)
        {
            ctx.NextTurn(Console.ReadLine);
            
            var route = ctx.GetRoutes(ctx.Self.Drones.Single()).MaxBy(r => r.Score);
            
            ctx.QueueCommand($"MOVE {route.Target.X} {route.Target.Y} 0");

            ctx.EndRound();
        }
    }
}