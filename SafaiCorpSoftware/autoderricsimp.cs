   private IMyRemoteControl Remote;
   private MyCommandLine cmd;
   private Vector3D Target;
   //private Vector3D LastIcePos;
   public Program()
    {

        Remote = GridTerminalSystem.GetBlockWithName("progrem") as IMyRemoteControl;
        Remote.ClearWaypoints();
        cmd = new MyCommandLine();

        if(!Remote.CanControlShip)
        {
            Echo("cant control ship");
        }

        if(!Remote.IsAutoPilotEnabled)
        {
            Echo("autopilot off");
        }
        //Runtime.UpdateFrequency = UpdateFrequency.Update100;

    }

    public void Main(string argument, UpdateType updateSource)
    {       
        bool targ = cmd.Switch("SetTarget");
        Echo("" + targ);
        if(true)
        {
            Echo("hello");
            Remote.ClearWaypoints();
            Target = new Vector3D(52591.46, -28107.35, 12204.89);//ParseCoordString(cmd.Argument(1), cmd.Argument(2), cmd.Argument(3));
            Remote.AddWaypoint(Target, "target");
            Remote.SetAutoPilotEnabled(true);
        }

        
        if(cmd.Switch("CheckAtTarg"))
        {
            if(Target == Remote.GetPosition())
            {
                Remote.SetAutoPilotEnabled(false);

            }
        }

        if(cmd.Switch("EnableAuto"))
        {

        }
    }       

    public Vector3D ParseCoordString(string x, string y, string z)
    {
        return new Vector3D(Double.Parse(x), Double.Parse(y), Double.Parse(z));

    }