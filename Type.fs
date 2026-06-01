namespace SixSevenGen

// Type.fs

type Operator =
    | Add
    | Sub
    | Mul
    | Mod

type Token =
    | Num of int
    | Oper of Operator
    | OpeningP
    | ClosingP

type Attempt = {
    GivenNums: int list
    GivenOpers: Operator list
    UserAttempt: Token list
    UserTarget: int
}

type Gamestate = {
    AvailableTargets: int Set
    Scores: float list        // Scores from each att
    CurrentAttempt: int     // 1, 2, 3
}