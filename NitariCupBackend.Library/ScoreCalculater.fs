namespace NitariCupBackend.Library

open System

module ScoreCalculater =

    let private LowerStart (start: DateTime, limit: DateTime, donedate: DateTime): float =
        (donedate - start).TotalMinutes
            |> fun x -> - x * x /10000.0
            |> exp
            |> fun x -> 100.0 * (1.0 /2.0 * x + 1.0 / 2.0)

    let private OverStartLowerLimit (start: DateTime, limit: DateTime, donedate: DateTime): float =
        (donedate - limit).TotalMinutes
            |> fun x -> 100.0 * (- x / 4.0 + 1.0)

    let private OverLimit (start: DateTime, limit: DateTime, donedate: DateTime): float =
        (donedate - limit).TotalMinutes
            |> fun x -> - x * x / 100.0
            |> exp
            |> fun x -> 100.0 * (3.0 / 4.0 * x)

    let ScoreCalc (start: DateTime, limit: DateTime, donedate: DateTime): float =
        match donedate with
        | x when x < start -> LowerStart(start, limit, donedate)
        | x when x < limit -> OverStartLowerLimit(start, limit, donedate)
        | _ -> OverLimit(start, limit, donedate)
