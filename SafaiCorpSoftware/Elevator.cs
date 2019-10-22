// Default declarations
private MyCommandLine cmd;
private IMyTextPanel OutPanel;

//
private List<IMyPistonBase> ElePistonsList;
private const string ElePistonGroupName = "ele_pistons";
private float[] EleFloorHeights = {0f,17f,34.5f,52f,69.5f,87f,104.5f};
private Elevator CurrElevator;

// private PistonGrid ElePistons;

public Program(){
    OutPanel = GridTerminalSystem.GetBlockWithName("ele_lcd") as IMyTextPanel;
    cmd = new MyCommandLine();

    //Setup:
    ElePistonsList = new List<IMyPistonBase>();
    IMyBlockGroup ElePistonsGroup = GridTerminalSystem.GetBlockGroupWithName(ElePistonGroupName);
    ElePistonsGroup.GetBlocksOfType<IMyPistonBase>(ElePistonsList);
    CurrElevator = new Elevator(ElePistonsList,EleFloorHeights, OutPanel);

}

public void Main(string argument, UpdateType updateSource){
    int floor_input = Int32.Parse(argument);
    
    if(floor_input > EleFloorHeights.Count()){
        OutPanel.WriteText("Max floor = " + EleFloorHeights.Count().ToString());
    } else {
        CurrElevator.GoToFloor(floor_input);
    }
}

public class Elevator{

    private IMyTextPanel LogPanel;
    private List<IMyPistonBase> ElePistons;
    private float[] EleFloorHeights;
    private float PistSpeed = 0.5f;
    public Elevator(List<IMyPistonBase> ElePistonsList, float[] FloorHeights, IMyTextPanel OutPanel)
    {
        //Instatiate: Provide Group name and floor heights
        ElePistons = ElePistonsList;
        EleFloorHeights = FloorHeights;
        LogPanel = OutPanel;
    }
    public void GoToFloor(int floor){
        float CurrentHeight = GetElevatorHeight(this.ElePistons);
        float TargetHeight = this.EleFloorHeights[floor];
        float Delta = TargetHeight - CurrentHeight;
        float pDelt = TargetHeight/this.ElePistons.Count;

        if(Delta < 0f){
            LogPanel.WriteText("Going down. Floor " + floor.ToString() , false);
            foreach(IMyPistonBase p in this.ElePistons){
                p.MinLimit = pDelt;
                p.Velocity = this.PistSpeed * -1f;
                p.MaxLimit = 10;
            }
        }
        else if(Delta > 0f){
            LogPanel.WriteText("Going up. Floor " + floor.ToString() , false);
            foreach(IMyPistonBase p in this.ElePistons){
                p.MaxLimit = pDelt;
                p.Velocity = this.PistSpeed;
                p.MinLimit = 0;
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