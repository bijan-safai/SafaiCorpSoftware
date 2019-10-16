private IMyTextPanel OutPanel;
private IMyCameraBlock Camera;

public Program()
{
    OutPanel = GridTerminalSystem.GetBlockWithName("progout") as IMyTextPanel;
    Camera = GridTerminalSystem.GetBlockWithName("cam") as IMyCameraBlock;
    Camera.EnableRaycast = true;
    
}



public void Save()

{

    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means. 
    // 
    // This method is optional and can be removed if not
    // needed.

}



public void Main(string argument, UpdateType updateSource)

{
    Echo("max range = " + Camera.AvailableScanRange);
    Echo("Time until scan = " + Camera.TimeUntilScan(10f));
    Echo("Can scan = " + Camera.CanScan(10f));
    Echo(Camera.RaycastConeLimit + "");
    Echo(Camera.RaycastDistanceLimit + "");
    // try to raycast
    MyDetectedEntityInfo detected = Camera.Raycast(15f, 0f, 0f);

    Echo(detected.Type.ToString());
    // update output
    if(!detected.IsEmpty())
    {
        OutPanel.FontColor = Color.Green;
        OutPanel.WriteText("Detected");
    } 
    else
    {
        OutPanel.FontColor = Color.Red;
        OutPanel.WriteText("Not Detected");
    }

}