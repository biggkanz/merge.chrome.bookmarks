namespace MergeBookmark

module Merge =
    
    open Domain
        
    let notContainsBookmark lst mark =
        let dups =
            lst
            |> Seq.where (fun x -> x.url = mark.url)
            |> Seq.toList
        dups.Length = 0        
        
    let getUniqueBookmarks marks1 marks2 =
        marks2
        |> Seq.where (notContainsBookmark marks1)