# 67 Generator
A command-line arithmetic puzzle game built with **F# / .NET 9**.
Construct arithmetic expressions to get as close as possible to a target number.

---

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)  
  Verify with: `dotnet --version` (should show `9.x.x`)

### Run
```bash
dotnet run
```

### Build
```bash
dotnet build
```

---

## How to Play

The goal is to construct an arithmetic expression as close as possible to a target number.

1. Run `dotnet run` from the `SixSevenGen` directory.

2. You have **three attempts**. At each attempt:

    First,
    - You are given **5 numbers** and **4 operators** from `{+, -, *, %}`.
    - You are shown which target numbers from `{67, 676, 6767}` are still available
    - You may swap one of the operators (15 seconds).

    Then, 
    - A 120-second timer starts. Enter your target and expression,
```
//input example
6767,96%(13+7)*420+6
```
- Follow this exact format (TARGET,EXPRESSION). Space characters are allowed (ignored)
- Valid input: the used target is removed from future attempts, and your score is shown.
- Invalid input or timeout: 0 score. On 0 score, set of available targets remian unchanged.

3. Your final score is the sum of scores across all three attempts.

### Example Session
```
=== Attempt 2 ===
Numbers: [8, 8, 13, 20, 553]
Operators: [+, -, *, %]
Optionak: [(, )]
Available Targets: [676, 6767]

Do you want to swap any of the operators?
Press [y] for [YES], any other key for [NO]
Make your decision within 15 seconds...  y
Enter the index (0 - 3) of the operator to swap:
   0: +
   1: -
   2: *
   3: %
1
New operators: [+, *, *, %]

Please enter your answer within 105.6 seconds...

Input example: 67,97-33+5+(276%1)
YOUR ANSWER = 6767,553*13+20*8%8 

Your expression evaluated to: 7189
Score for this attempt: 94.129921
```

## Modifications from the original requirements document

- In 2-2 and 2-3, the operator pool is changed to {+, -, *, %} (replace ^ with %). This is because % has interesting behavior, and makes reaching smaller targets easier. On the other hand, ^ is rather tricky to handle and is not as useful as the other operators.

- In 2-3, the user must make a decision on swapping an operator within 15 seconds. If the user doesnt provide input within 15 seconds, no swap is performed. 

- In 3, the timer is changed to run from the beginning of each attempt (i.e. before the swapping stage). This is because the user can exploit the operator-swapping stage to spend more time thinking about an answer.

- In 3, the timer is changed to last for 120 seconds. This is because after playing a few times I felt that 60 seconds is too short.

- In 5, the scoring system is modified to eliminate the use of "optimal distance".
Reasons for this decision include the follwing:

    A. Reduces internal complexity of the project

    B. Improves gaming experience - I realized during the process of developing this project that, with a scoring system based on optimal distance, the target selection stage of the game becomes meaningless (i.e. since the score does not reflect how "objectively close" to the target the player has gotten to, the player is given no motivation to carefully choose a "reachable / optimal" target)

The new formula is given by [ 100 * T / (T + D) ], where T is the target number and D is the user's distance.