private IMyRadioAntenna Ant;
List<IMyBroadcastListener> Listners;

public Program()

{

    Ant = GridTerminalSystem.GetBlockWithName("ant") as IMyRadioAntenna;
    IGC.RegisterBroadcastListener("logs");
    Listners = new List<IMyBroadcastListener>();
    IGC.GetBroadcastListeners(Listners);
    Listners[0].SetMessageCallback("log");
    
    Ant.AttachedProgrammableBlock = Me.EntityId;
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
    if(argument == "log")
    {
        MyIGCMessage log = Listners[0].AcceptMessage();
        Me.CustomData += log.Data.ToString();
    }
}