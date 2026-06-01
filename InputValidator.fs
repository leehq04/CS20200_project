namespace SixSevenGen

module InputValidator =
    let lessThanOneParen (tokens: Token list) =
        let count = tokens |> List.filter (fun x -> x = OpeningP) |> List.length
        count <= 1
    
    let numSetValidator (tokens: Token list) (givenNums: int list) =
        let numList = tokens |> List.choose (function Num x -> Some x | _ -> None)
        List.sort numList = List.sort givenNums
    
    let operRank o =
        match o with
        | Add -> 0
        | Sub -> 1
        | Mul -> 2
        | Mod -> 3
    
    let operSetValidator (tokens: Token list) (givenOpers: Operator list) =
        let operList = tokens |> List.choose (function Oper o -> Some o | _ -> None)
        (operList |> List.sortBy operRank) = (givenOpers |> List.sortBy operRank)
    
    let targetValidator tar (gs: Gamestate) =
        gs.AvailableTargets |> Set.contains tar
    
    let validate (att: Attempt) (gs: Gamestate) =
        let givenNums = att.GivenNums
        let givenOpers = att.GivenOpers
        let tokens = att.UserAttempt
        let target = att.UserTarget
        (lessThanOneParen tokens)
        && (numSetValidator tokens givenNums)
        && (operSetValidator tokens givenOpers)
        && (targetValidator target gs)