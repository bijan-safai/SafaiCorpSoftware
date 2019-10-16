private List<IMyPistonBase> Pistons;
private IMyTimerBlock PistonDelayTimer;
private int i;

public Program()

{
    IMyBlockGroup pistonsGroup = GridTerminalSystem.GetBlockGroupWithName("pistons");
    Pistons = new List<IMyPistonBase>();
    pistonsGroup.GetBlocksOfType<IMyPistonBase>(Pistons);
    PistonDelayTimer = GridTerminalSystem.GetBlockWithName("timerDelay") as IMyTimerBlock;
    i = 0;
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
    if(Pistons[i].CurrentPosition == Pistons[i].HighestPosition)
    {
        i++;
    }
    
    if(i < Pistons.Count())
    {
        Pistons[i].MaxLimit += 1.0f;
        PistonDelayTimer.StartCountdown();
    }
    /*else
    {
        TODO send some kind of alert back to main base that drill is done
    }*/
    
}