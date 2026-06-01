namespace SixSevenGen

module SetGenerator =
    let rng = System.Random()
    let operSet = [Add; Sub; Mul; Mod]

    let randomNumberGenerator (): int list =
        let small1 = rng.Next(1, 10)
        let small2 = rng.Next(1, 10)
        let med1 = rng.Next(10, 100)
        let med2 = rng.Next(10, 100)
        let large = rng.Next(100, 1000)
        [small1; small2; med1; med2; large]

    let sortOperators (ops: Operator list): Operator list =
        let order = function
            | Add -> 0
            | Sub -> 1
            | Mul -> 2
            | Mod -> 3
        ops |> List.sortBy order
    
    let randomOperatorGenerator (): Operator list =
        let op1 = operSet[rng.Next(0, 4)]
        let op2 = operSet[rng.Next(0, 4)]
        [Add; Mul; op1; op2] |> sortOperators
    
    let swapOperator (oglist: Operator list) (idx: int): Operator list =
        let rec replace (avoid: Operator) =
            let o = operSet[rng.Next(0, 4)]
            if o = avoid then replace avoid
            else o
        let newop = replace oglist.[idx]
        oglist |> List.mapi (fun i x -> if i = idx then newop else x)