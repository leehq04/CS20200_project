namespace SixSevenGen

module Parser =    
    let parser (input: string): Result<int * Token list, string> =

        let updateNumber cur newdigit = ((int newdigit) - (int '0')) + (cur * 10)

        let classifyChar = function
            | c when c >= '0' && c <= '9' -> 1
            | '+' | '-' | '*' | '%' -> 2
            | '(' -> 3
            | ')' -> 4
            | ' ' -> 5
            | ',' -> 6
            | _ -> 7
        
        let classifyOperator = function
            | '+' -> Add
            | '-' -> Sub
            | '*' -> Mul
            | '%' -> Mod
            | _ -> failwith "invalid operator"
        
        let rec getTarget res idx : Result<int * int, string> =
            if idx >= input.Length then Error "invalid expression"
            else
                let c = input.[idx]
                match classifyChar c with
                | 1 -> getTarget (updateNumber res c) (idx + 1)
                | 6 -> Ok (res, idx)
                | _ -> Error "invalid expression"

        let rec loopy (acc: Token list) idx curState curNumber (parenOpen: bool): Result<Token list, string> =
            if idx >= input.Length then
                if parenOpen then Error "invalid expression"
                else
                    let finalAcc =
                        if curState = 1 then (Num curNumber) :: acc else acc 
                    Ok (List.rev finalAcc)
            else
                let c = input.[idx]
                match (curState, classifyChar c) with
                    | (0, 1) ->
                        loopy acc (idx + 1) 1 (updateNumber curNumber c) false
                    | (0, 3) ->
                        loopy (OpeningP :: acc) (idx + 1) 3 0 true
                    | (1, 1) ->
                        loopy acc (idx + 1) 1 (updateNumber curNumber c) parenOpen
                    | (1, 2) ->
                        loopy ((Oper (classifyOperator c)) :: ((Num curNumber) :: acc)) (idx + 1) 2 0 parenOpen
                    | (1, 4) ->
                        if not parenOpen then Error "invalid expression"
                        else loopy (ClosingP :: ((Num curNumber) :: acc)) (idx + 1) 4 0 false
                    | (2, 1) ->
                        loopy acc (idx + 1) 1 (updateNumber 0 c) parenOpen
                    | (2, 3) ->
                        loopy (OpeningP :: acc) (idx + 1) 3 0 true
                    | (3, 1) ->
                        loopy acc (idx + 1) 1 (updateNumber 0 c) true
                    | (4, 2) ->
                        loopy ((Oper (classifyOperator c)) :: acc) (idx + 1) 2 0 false
                    | (_, 5) ->
                        loopy acc (idx + 1) curState curNumber parenOpen
                    | _ -> Error "invalid expression"
        
        match getTarget 0 0 with
        | Ok (tar, idx) ->
            match loopy [] (idx + 1) 0 0 false with
            | Ok res -> Ok (tar, res)
            | Error e -> Error e
        | Error e -> Error e