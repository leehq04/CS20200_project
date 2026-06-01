namespace SixSevenGen

module Evaluator =
    let evaluate (att: Attempt) =
        let tokens = att.UserAttempt

        let precedence (o: Operator) =
            match o with
            | Add -> 1
            | Sub -> 1
            | Mul -> 0
            | Mod -> 0
        
        let binop (o: Operator) l r: Result<int, string> =
            match o with
            | Add -> Ok (l + r)
            | Sub -> Ok (l - r)
            | Mul -> Ok (l * r)
            | Mod -> 
                if r = 0 then Error "Division by zero"
                else Ok (l % r)

        let rec evalWOParen (nlist: int list) (olist: Operator list): Result<int, string> =
            if olist.Length = 0 then Ok (nlist.[0])
            else
                /// index of operator that should be applied first
                let fpi =
                    olist
                    |> List.mapi (fun i o -> (i, o))
                    |> List.minBy (fun (_, o) -> precedence o)
                    |> fst
                match binop olist.[fpi] nlist.[fpi] nlist.[fpi + 1] with
                | Ok newnum ->
                    let nleft = nlist |> List.take fpi
                    let nright = nlist |> List.skip (fpi + 2)
                    let oleft = olist |> List.take fpi
                    let oright = olist |> List.skip (fpi + 1)

                    evalWOParen (nleft @ (newnum :: nright)) (oleft @ oright)
                | Error e -> Error e
        
        let noParen (tokens: Token list) =
            let count = tokens |> List.filter (fun x -> x = OpeningP) |> List.length
            count = 0
        
        if noParen tokens
        then
            let npNums = tokens |> List.choose (function Num x -> Some x | _ -> None)
            let npOpers = tokens |> List.choose (function Oper o -> Some o | _ -> None)
            evalWOParen npNums npOpers
        
        else
            let openIdx = List.findIndex ((=) OpeningP) tokens
            let closeIdx = List.findIndex ((=) ClosingP) tokens
        
            let parenExpr =
                tokens
                |> List.skip (openIdx + 1)
                |> List.take (closeIdx - openIdx - 1)
            let parenNums =
                parenExpr
                |> List.choose (function Num x -> Some x | _ -> None)
            let parenOpers =
                parenExpr
                |> List.choose (function Oper o -> Some o | _ -> None)
            
            match evalWOParen parenNums parenOpers with
            | Error e -> Error e
            | Ok parenVal ->
                let tleft =
                    tokens |> List.take openIdx
                let tright =
                    tokens |> List.skip (closeIdx + 1)
                let newtokens =
                    tleft @ ((Num parenVal) :: tright)
        
                let newNums =
                    newtokens
                    |> List.choose (function Num x -> Some x | _ -> None)
                let newOpers =
                    newtokens
                    |> List.choose (function Oper o -> Some o | _ -> None)
        
                evalWOParen newNums newOpers