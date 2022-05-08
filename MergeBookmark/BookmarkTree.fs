module MergeBookmark.BookmarkTree

open System
open Util
open Domain

let private containsMark lst mark =
    let duplicates =
        lst
        |> Seq.where (fun x -> x.href = mark.href)
        |> Seq.toList
    duplicates.Length > 0        
    
let private getUniqueBookmarks marks1 marks2 =
    marks2
    |> Seq.where (fun x -> not <| containsMark marks1 x)

let Print (tree:BookmarkTree) =
    let rec loop depth (t:BookmarkTree) =
        let spacer = String(' ', depth * 4)
        match t with
        | LeafNode m ->        
            printfn "%s |- %A" spacer m.name
        | InternalNode (f,subtrees) ->
            printfn "%s | %s" spacer f.name
            subtrees |> Seq.iter (loop (depth + 1))
    loop 0 tree
    
let ContainsMark (mark:MarkInfo) (tree:BookmarkTree) =
    let fMark acc m =
        if mark.href = m.href
        then true
        else acc

    let fFolder acc f =
        acc
        
    Tree.fold fMark fFolder false tree

//todo: cleanup    
let ContainsMarks (tree2:BookmarkTree) (tree1:BookmarkTree) =
    let marks2 = tree2 |> Tree.flattenLeaf
    let marks1 = tree1 |> Tree.flattenLeaf
    
    let containsMark m marks =
        marks
        |> Seq.toList
        |> List.tryFind (fun x -> x.href = m.href)
    
    marks2
    |> Seq.map (fun x -> containsMark x marks1)
    
let DistinctMarks (tree2:BookmarkTree) (tree1:BookmarkTree) =
    let distinct m marks =
        let dup =
            marks
            |> Seq.toList
            |> List.tryFind (fun x -> x.href = m.href)
        
        match dup with
        | Some _ -> None
        | _ -> Some m
        
    let marks2 = tree2 |> Tree.flattenLeaf
    let marks1 = tree1 |> Tree.flattenLeaf
    
    marks2
    |> Seq.map (fun x -> distinct x marks1)
    |> Seq.toList
    |> List.choose id

