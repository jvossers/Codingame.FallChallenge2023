internal class Program
{
    static void Main(string[] args)
    {
        var ctx = new GameContext(Console.ReadLine);

        while (true)
        {
            ctx.NextTurn(Console.ReadLine);

            //ctx.QueueCommand("");

            ctx.EndRound();
        }
    }
}