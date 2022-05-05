namespace MergeBookmark

module Merge =
    
    open Domain
        
    let containsMark lst mark =
        let duplicates =
            lst
            |> Seq.where (fun x -> x.href = mark.href)
            |> Seq.toList
        duplicates.Length > 0        
        
    let getUniqueBookmarks marks1 marks2 =
        marks2
        |> Seq.where (fun x -> not <| containsMark marks1 x)