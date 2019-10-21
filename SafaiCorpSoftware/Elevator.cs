// Default declarations
private MyCommandLine cmd;
private IMyTextPanel OutPanel;

//
private const string ElePistonBlockName = "ele_pistons";
private float[] EleFloorHeights = {0f,17f,34.5f,52f,69.5f,87f};
// private PistonGrid ElePistons;

public Program(){
    OutPanel = GridTerminalSystem.GetBlockWithName("Test_LCD") as IMyTextPanel;
    cmd = new MyCommandLine();

    // CurrElevator = new Elevator();
}

public void Main(string argument, UpdateType updateSource){
    try {
        int floor_input = Int32.Parse(argument);
        CurrElevator = new Elevator(ElePistonBlockName,EleFloorHeights);
        CurrElevator.GoToFloor(floor_input);
        OutPanel.WriteText("Going to floor " + argument + "\n", true);
    } catch {
        OutPanel.WriteText("Non-Int value provided...\n", true);
        
    }
}

public class Elevator{
    private List<IMyPistonBase> ElePistons;
    private float[] EleFloorHeights;
    private float PistSpeed = 0.5f;
    public Elevator(string PistonsGroupName, float[] FloorHeights)
    {
        //Instatiate: Provide Group name and floor heights
        IMyBlockGroup ElePistonsGroup = GridTerminalSystem.GetBlockGroupWithName(PistonsGroupName);
        ElePistonsGroup.GetBlocksOfType<IMyPistonBase>(ElePistons);
        EleFloorHeights = FloorHeights;
    }
    public void GoToFloor(int floor){
        float CurrentHeight = GetElevatorHeight(this.ElePistons);
        float TargetHeight = this.EleFloorHeights[floor];
        float Delta = TargetHeight - CurrentHeight;
        float pDelt = Delta/this.ElePistons.Count;
        if(Delta < 0f){
            foreach(IMyPistonBase p in this.ElePistons){
                p.SetValueFloat("MinLimit", pDelt);
                p.SetValueFloat("Velocity",this.PistSpeed * -1f);
                p.SetValueFloat("MaxLimit", 10);
            }
        }
        else if(Delta > 0f){
            foreach(IMyPistonBase p in this.ElePistons){
                p.SetValueFloat("MaxLimit", pDelt);
                p.SetValueFloat("Velocity",this.PistSpeed);
                p.SetValueFloat("MinLimit", 10);
            }
        }

    }

    private float GetElevatorHeight(List<IMyPistonBase> ElePistons){
        float h = 0f;
        foreach(IMyPistonBase p in ElePistons){
            if(p.Velocity < 0f){
                h = h + p.MinLimit;
            } else {
                h = h + p.MaxLimit;
            }
        }
        return h;
    }
}