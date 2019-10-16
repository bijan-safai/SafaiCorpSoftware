/*
    This script can be used as a base to create communication satallites.
    Implements SafNet v1.0 as defined in safnet.txt
    Programmable block name will be the stallite name.
 */

private string AquireTag;
private string RespondTag;
private const string BroadAquireTag = "Aquire_All_CommSat_UnicastId";
private const string BroadRespondTag = "Provide_All_CommSat_UnicastId";
private const string SafNetVersion = "v1.0";

private RoutingSatUnicastMessageHandler Handler;

public Program()
{
    Echo($"Setting up Comm sat with name {Me.DisplayNameText}");
    AquireTag = $"Aquire_{Me.DisplayNameText}_UnicastId";
    RespondTag = $"Provide_{Me.DisplayNameText}_UnicastId";
    // Blocks on the network can ask for the routers address
    IMyBroadcastListener aquireListener = IGC.RegisterBroadcastListener(AquireTag);
    aquireListener.SetMessageCallback(AquireTag);

    IMyBroadcastListener broadAquireListener = IGC.RegisterBroadcastListener(BroadAquireTag);
    broadAquireListener.SetMessageCallback(BroadAquireTag);

    // Unicast messages will parsed and handled
    IGC.UnicastListener.SetMessageCallback("unicastMessage");

    Handler = new RoutingSatUnicastMessageHandler(IGC, Me.DisplayNameText, SafNetVersion);

    SetupAntenna();

    Me.GetSurface(0).WriteText($"Comm sat {Me.DisplayNameText}", false);
    Echo($"Comm sat {Me.DisplayNameText} online");
}


public void Main(string argument, UpdateType updateSource)

{
    if(argument == "unicastMessage")
    {
        while(IGC.UnicastListener.HasPendingMessage)
        {
            Handler.HandleSafNetMessage(IGC.UnicastListener.AcceptMessage());
        }
    }
    else if(argument == AquireTag)
    {
        IGC.SendBroadcastMessage(RespondTag, IGC.Me, TransmissionDistance.TransmissionDistanceMax);
    }
    else if(argument == BroadAquireTag)
    {
        IGC.SendBroadcastMessage(BroadRespondTag, $"{Me.DisplayNameText}/{IGC.Me}");
    } 
}


public void SetupAntenna()
{
    IMyRadioAntenna antenna = GridTerminalSystem.GetBlockWithName($"{Me.DisplayNameText}_radio") as IMyRadioAntenna;
    IMyLaserAntenna laser = GridTerminalSystem.GetBlockWithName($"{Me.DisplayNameText}_laser") as IMyLaserAntenna;

    antenna.AttachedProgrammableBlock = Me.EntityId;
    laser.AttachedProgrammableBlock = Me.EntityId;
}

private class RoutingSatUnicastMessageHandler
{
    private IMyIntergridCommunicationSystem IGC;
    private string SatName;
    private string SafNetVer;

    public RoutingSatUnicastMessageHandler(IMyIntergridCommunicationSystem iGC, string satName, string safNetVer)
    {
        IGC = iGC;
        SatName = satName;
        SafNetVer = safNetVer;
    }

    public void HandleSafNetMessage(MyIGCMessage rawMessage)
    {
        try
        {
            string[] tokenizedMessage = rawMessage.Tag.Split('/');
            if (tokenizedMessage.Count() == 0)
            {
                throw new System.Exception("Empty tag passed");
            }

            switch (tokenizedMessage[0])
            {
                case "Pass" : PassThrough(tokenizedMessage, rawMessage); break;
                case "Script" : throw new System.Exception("Sat has no scripts defined"); //ExecuteScript(tokenizedMessage, rawMessage); break; <--- uncomment when scripts are implemented
                case "Ping" : IGC.SendUnicastMessage(rawMessage.Source, $"Ping/{SatName}", SafNetVer); break;
                default: throw new System.Exception($"Invalid toplevel tag {tokenizedMessage[0]}");
            }
        }
        catch (System.Exception e)
        {
            ErrorResponse(rawMessage, e);
        }
    }

    private void PassThrough(string[] tokenizedMessage, MyIGCMessage rawMessage)
    {
        if(tokenizedMessage.Count() < 3)
        {
            throw new System.Exception($"Malformed pass through tag {rawMessage.Tag}");
        }
        switch (tokenizedMessage[1])
        {
            case "Broad" : BroadcastPassThrough(tokenizedMessage, rawMessage); break;
            case "Uni" : UnicastPassThrough(tokenizedMessage, rawMessage); break;
            default: throw new System.Exception($"Invalid pass through method {tokenizedMessage[1]}");
        }
    }

    private void BroadcastPassThrough(string[] tokenizedMessage, MyIGCMessage rawMessage)
    {
        // if there is ever a reason to use a smaller trans dist then we need to update SafNet
        IGC.SendBroadcastMessage(tokenizedMessage[2], rawMessage.Data.ToString(), TransmissionDistance.TransmissionDistanceMax);
    }

    private void UnicastPassThrough(string[] tokenizedMessage, MyIGCMessage rawMessage)
    {
        if(tokenizedMessage.Count() < 4)
        {
            throw new System.Exception($"Malformed pass through tag {rawMessage.Tag}");
        }

        IGC.SendUnicastMessage(Convert.ToInt64(tokenizedMessage[2]), tokenizedMessage[3], rawMessage.Data.ToString());
    }

    private void ErrorResponse(MyIGCMessage orgMessage, System.Exception e)
    {
        RespondToCaller(orgMessage, $"{SatName}/Error", e.Message);
    }

    private void RespondToCaller(MyIGCMessage orgMessage, string tag, string data)
    {
        IGC.SendUnicastMessage(orgMessage.Source, tag, data);
    }
}