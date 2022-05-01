namespace MergeBookmark

module Merge =
   
    let GetDuplicates (marks1:seq<string>) (marks2:seq<string>) =
        marks1
        |> Seq.filter (fun mark ->
            marks2
            |> Seq.contains mark)

    let GetUnique (marks1:seq<string>) (marks2:seq<string>) =
        marks1
        |> Seq.filter (fun mark ->
            marks2
            |> Seq.contains mark
            |> not)