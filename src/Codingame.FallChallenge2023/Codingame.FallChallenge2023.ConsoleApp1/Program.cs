internal class Program
{
    static void Main(string[] args)
    {
        var ctx = new GameContext(Console.ReadLine);

        while (true)
        {
            ctx.NextTurn(Console.ReadLine);

            var useLight = "0";

            //ctx.QueueCommand($"MOVE {wayPoint.X} {wayPoint.Y} {useLight}");

            ctx.EndRound();
        }
    }
}