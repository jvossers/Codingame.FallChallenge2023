internal class Program
{
    static void Main(string[] args)
    {
        var ctx = new GameContext(Console.ReadLine);

        while (true)
        {
            ctx.NextTurn(Console.ReadLine);

            var leftDrone = ctx.Self.Drones.MinBy(d => d.X);
            var rightDrone = ctx.Self.Drones.MaxBy(d => d.X);

            var leftUseLight = leftDrone.Y > 2500 && leftDrone.Battery >=5 ? "1" : "0";
            var rightUseLight = rightDrone.Y > 2500 && rightDrone.Battery >= 5 ? "1" : "0";

            if (ctx.CurrentState == WorkflowState.Descending)
            {
                int leftDroneTargetX = 2000;
                int leftDroneTargetY = 8750;

                int rightDroneTargetX = 8000;
                int rightDroneTargetY = 8750;

                if (leftDrone.X == leftDroneTargetX && leftDrone.Y == leftDroneTargetY &&
                    rightDrone.X == rightDroneTargetX && rightDrone.Y == rightDroneTargetY)
                {
                    ctx.CurrentState = WorkflowState.Converging;
                }
                else
                {
                    leftDrone.Command = $"MOVE {leftDroneTargetX} {leftDroneTargetY} {leftUseLight}";
                    rightDrone.Command = $"MOVE {rightDroneTargetX} {rightDroneTargetY} {rightUseLight}";
                }
            }

            if (ctx.CurrentState == WorkflowState.Converging)
            {
                int leftDroneTargetX = 4000;
                int leftDroneTargetY = 8750;

                int rightDroneTargetX = 6000;
                int rightDroneTargetY = 8750;

                if (leftDrone.X == leftDroneTargetX && leftDrone.Y == leftDroneTargetY &&
                    rightDrone.X == rightDroneTargetX && rightDrone.Y == rightDroneTargetY)
                {
                    ctx.CurrentState = WorkflowState.Surfacing;
                }
                else
                {
                    leftDrone.Command = $"MOVE {leftDroneTargetX} {leftDroneTargetY} {leftUseLight}";
                    rightDrone.Command = $"MOVE {rightDroneTargetX} {rightDroneTargetY} {rightUseLight}";
                }
            }

            if(ctx.CurrentState == WorkflowState.Surfacing)
            {
                int leftDroneTargetX = 2000;
                int leftDroneTargetY = 500;

                int rightDroneTargetX = 8000;
                int rightDroneTargetY = 500;

                if (leftDrone.X == leftDroneTargetX && leftDrone.Y == leftDroneTargetY &&
                    rightDrone.X == rightDroneTargetX && rightDrone.Y == rightDroneTargetY)
                {
                    ctx.CurrentState = WorkflowState.Descending;
                }
                else
                {
                    leftDrone.Command = $"MOVE {leftDroneTargetX} {leftDroneTargetY} {leftUseLight}";
                    rightDrone.Command = $"MOVE {rightDroneTargetX} {rightDroneTargetY} {rightUseLight}";
                }
            }

            foreach (var drone in ctx.Self.Drones)
            {
                ctx.QueueCommand(drone.Command);
            }

            ctx.EndRound();
        }
    }
}