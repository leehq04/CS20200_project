namespace SixSevenGen

module Main =
    open SetGenerator
    open Parser
    open Evaluator
    open InputValidator


    let calcScore tar dist =
        //let maxScore = (((tar |> string |> String.length) - 1) * 100) |> float
        100.0 * (float tar) / (float tar + float dist)
    
    let numListToString (nums: int list) =
        nums
        |> List.map string
        |> String.concat ", "
        |> sprintf "[%s]"

    let operToString (o: Operator) =
        match o with
        | Add -> "+"
        | Sub -> "-"
        | Mul -> "*"
        | Mod -> "%"

    let operListToString (ops: Operator list) =
        ops 
        |> List.map (function Add -> "+" | Sub -> "-" | Mul -> "*" | Mod -> "%")
        |> String.concat ", "
        |> sprintf "[%s]"

    let timedSwapDecision (): Option<string> =
        let mutable timedOut2 = false
        let timer2 = new System.Timers.Timer(15000.0)
        timer2.AutoReset <- false
        timer2.Elapsed.Add(fun _ ->
            timedOut2 <- true
            printf "\n\nSwap timed out, no swap performed\nPress enter to proceed...")
        timer2.Start()
        printf "\nDo you want to swap any of the operators?\nPress [y] for [YES], any other key for [NO]\nMake your decision within 15 seconds...  "
        let input = System.Console.ReadLine()
        timer2.Stop()
        timer2.Dispose()
        if timedOut2 then None
        else Some (input.Trim())

    let timedInput (ops: Operator list): Option<string> * Operator list=
        let mutable timedOut = false
        let startTime = System.DateTime.Now
        let timer = new System.Timers.Timer(120000.0)
        timer.AutoReset <- false
        timer.Elapsed.Add(fun _ ->
            timedOut <- true
            printfn "\nTime's up! Press enter to proceed to next attempt")
        timer.Start()

        let swapStage (ops: Operator list): Operator list =
            match timedSwapDecision () with
            | None -> 
                ops
            | Some "y" ->
                printfn "\nEnter the index (0 - %d) of the operator to swap:" (ops.Length - 1)
                for i in 0 .. (ops.Length - 1) do
                    printfn "   %d: %s" i (ops.[i] |> operToString)
                match System.Int32.TryParse(System.Console.ReadLine().Trim()) with
                | (true, idx) when idx >= 0 && idx < ops.Length ->
                    let newOps = swapOperator ops idx
                    printfn "New operators: %s" (operListToString newOps)
                    newOps
                | _ ->
                    printfn "Invalid index, no swap performed"
                    ops
            | Some _ ->
                printfn "No swap performed"
                ops
        
        let finOps = swapStage ops

        if timedOut then 
            timer.Stop()
            timer.Dispose()
            (None, finOps)
        else
            let remaining = 120.0 - (System.DateTime.Now - startTime).TotalSeconds
            printfn "\nPlease enter your answer within %.1f seconds..." remaining
            printfn "Input example: 67,97-33+5+(276%%1)"
            printf "\nYOUR ANSWER = "
            let input = System.Console.ReadLine()
            timer.Stop()
            timer.Dispose()
            if timedOut then (None, finOps)
            else ((Some input), finOps)

    [<EntryPoint>]
    let main argv =
        let initState: Gamestate = {
            AvailableTargets = Set.ofList [67; 676; 6767]
            Scores = []
            CurrentAttempt = 1
        }

        let rec gameLoop (state: Gamestate) =
            if state.CurrentAttempt > 3 then
                // game over
                printfn "\nGame over! Final score: %.1f / 300" (List.sum state.Scores)
            else
                printfn "\n=== Attempt %d ===" state.CurrentAttempt
                let nums = randomNumberGenerator ()
                let ops = randomOperatorGenerator ()
                printfn "Numbers: %s" (numListToString nums)
                printfn "Operators: %s" (operListToString ops)
                printfn "Optional: [(, )]"
                printfn "Available Targets: %s" (state.AvailableTargets |> Set.toList |> numListToString)

                match timedInput ops with
                | (None, _) ->
                    // score 0, continue
                    printfn "\nFailed to answer within 120 seconds\nScore for this attempt: 0.0"
                    gameLoop { state with 
                                CurrentAttempt = state.CurrentAttempt + 1
                                Scores = state.Scores @ [0.0] }
                | (Some input, finOps) ->
                    match parser input with
                    | Error e ->
                        printfn "\nInvalid input: %s" e
                        printfn "Score for this attempt: 0.0"
                        gameLoop { state with
                                    CurrentAttempt = state.CurrentAttempt + 1
                                    Scores = state.Scores @ [0.0] }
                    | Ok (tar, tokens) ->
                        let attObj = {
                            GivenNums = nums
                            GivenOpers = finOps
                            UserAttempt = tokens
                            UserTarget = tar
                        }
                        if validate attObj state then
                            match evaluate attObj with
                            | Error e ->
                                printfn "\nInvalid input: %s" e
                                printfn "Score for this attempt: 0.0"
                                gameLoop { state with
                                            CurrentAttempt = state.CurrentAttempt + 1
                                            Scores = state.Scores @ [0.0] }
                            | Ok userVal ->
                                let userDist = abs (userVal - tar)
                                let userScore = calcScore tar userDist
                                printfn "\nYour expression evaluated to: %d" userVal
                                printfn "Score for this attempt: %.1f" userScore
                                let newAT = state.AvailableTargets |> (Set.remove tar)
                                gameLoop { state with
                                            AvailableTargets = newAT
                                            CurrentAttempt = state.CurrentAttempt + 1
                                            Scores = state.Scores @ [userScore] }
                        else
                            printfn "\nInvalid input: numbers or operators don't match\nScore for this attempt: 0.0"
                            gameLoop { state with
                                        CurrentAttempt = state.CurrentAttempt + 1
                                        Scores = state.Scores @ [0.0] }

        gameLoop initState
        0