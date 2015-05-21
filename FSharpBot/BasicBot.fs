namespace FSharpBasicBot

module BasicBotModule =

    open System
    open System.IO
    open Newtonsoft.Json
    open ChallengeHarnessInterfaces
    open SpaceInvaders
    open System.Configuration

    type BasicBot(outputPath : string) =
        
        let OutputPath = outputPath

        let StateFile = ConfigurationManager.AppSettings.Item("StateFile")
        let MapFile = ConfigurationManager.AppSettings.Item("MapFile")
        let OutputFile = ConfigurationManager.AppSettings.Item("OutputFile")

        let Log message = 
            printfn message

        let DeserializeState jsonText = 
            let settings = JsonSerializerSettings()
            settings.Converters.Add(EntityConverter())
            settings.NullValueHandling <- NullValueHandling.Ignore
            let deserializedMatch = JsonConvert.DeserializeObject<Core.Match>(jsonText, settings)
            deserializedMatch 


        let LoadState = 
            try
                let path = Path.Combine(OutputPath,StateFile)
                let lines = File.ReadLines path
                            |> Seq.toArray
                            |> String.concat ""

                let thematch = DeserializeState(lines)
                Some(thematch)
            with
                | :? IOException as ex -> 
                    Log "Unable to read state file: %s " (StateFile)
                    Log "Stacktrace: %s " ex.StackTrace
                    None


        let LogPlayerState (player : Core.Player) =
            Log "\tPlayer %d Kills: %d" player.PlayerNumber player.Kills
            Log "\tPlayer %d Lives: %d" player.PlayerNumber player.Lives
            Log "\tPlayer %d Missiles: %d/%d" player.PlayerNumber player.Missiles.Count player.MissileLimit

        let LogMatchState (pMatch : Core.Match) =
            Log "Game state: "
            Log "\tRound Number: %d" pMatch.RoundNumber
            pMatch.Players.ForEach(fun x -> LogPlayerState x)


        let LoadMap =
            try
                let path = Path.Combine(OutputPath,MapFile)
                let lines = File.ReadLines path
                            |> Seq.toArray
                            |> String.concat System.Environment.NewLine 
                lines
                
            with
                | :? System.IO.IOException as ex -> 
                    Log "Unable to read map file: %s " MapFile
                    Log "Stacktrace: %s " ex.StackTrace
                    "Failed to load map!"


        let GetRandomShipCommand = 
            let random = System.Random()
            let possibleShipCommands  : Command.ShipCommand seq = unbox (Enum.GetValues(typeof<Command.ShipCommand>)) 
            let command = random.Next(0, Seq.length possibleShipCommands)
            Seq.nth command possibleShipCommands


        let SaveShipCommand (shipCommand : Command.ShipCommand) =
            try

                let path = Path.Combine(OutputPath,OutputFile)
                using(new StreamWriter(path))(fun writer -> writer.WriteLine(shipCommand.ToString()))

                Log "Command: %s" (shipCommand.ToString())
            with
                | :? IOException as ex -> 
                    Log "Unable to write command file: %s " (OutputFile)
                    Log "Stacktrace: %s " ex.StackTrace

        member this.Execute() =
            let gamestate = LoadState
                            |> Option.map (LogMatchState)   
            
            let map = LoadMap
            Log "Map:%s%s" Environment.NewLine map

            
            SaveShipCommand GetRandomShipCommand 
        

