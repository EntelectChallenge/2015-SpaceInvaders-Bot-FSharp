
open System
open FSharpBasicBot.BasicBotModule


let PrintUsage() = 
    printfn "F# BasicBot usage: FSharpBot.exe <outputFilename>"
    printfn ""
    printfn "\toutputPath\tThe output folder where the match runner will output map and state files and look for the move file."

let RunBot argv =
    match argv with 
    | [|first|] -> 
        if IO.Directory.Exists(first) then 
            let bot = BasicBot(first)
            bot.Execute()
        else
            PrintUsage()
            printfn ""
            printfn "Error: Output folder \%s\ does not exist." first
    | _ -> PrintUsage()



[<EntryPoint>]
let main argv = 

    let stopwatch = Diagnostics.Stopwatch.StartNew();
    RunBot(argv)
    stopwatch.Stop()
    printfn "[BOT]\tBot finished in %d ms." stopwatch.ElapsedMilliseconds
    0 // return an integer exit code


