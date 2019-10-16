    private const double x = 51831.79;
    private const double y = -29551.79;
    private const double z = 12227.12;

    private double xStep;
    private double yStep;
    private double zStep;
    private int steps;
    IMyRemoteControl Remote;

    public Program()
    {

        Remote = GridTerminalSystem.GetBlockWithName("progrem") as IMyRemoteControl;
        Remote.ClearWaypoints();
        if(!Remote.CanControlShip)
        {
            Echo("cant control ship");
        }

        if(!Remote.IsAutoPilotEnabled)
        {
            Echo("autopilot off");
        }

        Remote.ControlThrusters = true;
        //IMyTextPanel OutPanel = GridTerminalSystem.GetBlockWithName("progout") as IMyTextPanel;
        Vector3D goal = new Vector3D(x, y, z);
        Vector3D current = Remote.GetPosition();

        double xdif = current.X - goal.X;
        double ydif = current.Y - goal.Y;
        double zdif = current.Z - goal.Z;

        xStep = xdif/10;
        yStep = ydif/10;
        zStep = zdif/10;

        steps = 0;

        //Runtime.UpdateFrequency = UpdateFrequency.Update100;

    }

    public void Main(string argument, UpdateType updateSource)
    {

        if(!Remote.CanControlShip)
        {
            Echo("cant control ship");
        }

        if(!Remote.IsAutoPilotEnabled)
        {
            Echo("autopilot off");
        }

        if(steps < 10)
        {
            Echo(steps + "");
            Remote.ClearWaypoints();
            Vector3D current = Remote.GetPosition();
            Vector3D newPos = new Vector3D(current.X + xStep, current.Y + yStep, current.Z + zStep);
            Remote.AddWaypoint(newPos, "nextItr");
            steps++;
            Remote.SetAutoPilotEnabled(true);
        }
        else
        {
            Echo("done");
        }
    }       