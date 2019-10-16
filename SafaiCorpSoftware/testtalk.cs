private IMyTextPanel OutPanel;
private IMyRadioAntenna Ant;

public Program()
{
    OutPanel = GridTerminalSystem.GetBlockWithName("progout") as IMyTextPanel;
    Ant = GridTerminalSystem.GetBlockWithName("ant") as IMyRadioAntenna;
    IGC.RegisterBroadcastListener("testbroadcast");
    List<IMyBroadcastListener> listners = new List<IMyBroadcastListener>();
    IGC.GetBroadcastListeners(listners);
    listners[0].SetMessageCallback("yeet");
    IGC.UnicastListener.SetMessageCallback("uni");

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

    if(argument == "yeet")
    {
        List<IMyBroadcastListener> listners = new List<IMyBroadcastListener>();
        IGC.GetBroadcastListeners(listners);
        if(listners[0].HasPendingMessage)
        {
            MyIGCMessage message = listners[0].AcceptMessage();
            OutPanel.WriteText(message.Data.ToString(), false);
        }
    }

    if(argument == "uni")
    {
        if(IGC.UnicastListener.HasPendingMessage)
        {
            MyIGCMessage message = IGC.UnicastListener.AcceptMessage();
            OutPanel.WriteText(message.Tag + "\n", false);
            OutPanel.WriteText(message.Data.ToString(), true);
        }
    }

    if(argument == "send")
    {
        long address = 110010918201879565;
        IGC.SendUnicastMessage(address, $"Pass/Uni/{IGC.Me}/testuincast", "this is testing unicast passthrough");
        //IGC.SendBroadcastMessage("Aquire_SafNetTest_UnicastId", "HELLO WORLD", TransmissionDistance.TransmissionDistanceMax);
        OutPanel.WriteText("Sending...", false);
    }
}