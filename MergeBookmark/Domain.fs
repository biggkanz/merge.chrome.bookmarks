module MergeBookmark.Domain

type Tree<'LeafData,'INodeData> =
    | LeafNode of 'LeafData
    | InternalNode of 'INodeData * Tree<'LeafData,'INodeData> seq
    
type FolderInfo = {name:string; date:string; modified:string}
type MarkInfo = {name:string; href:string; icon:string}

type BookmarkTree = Tree<MarkInfo,FolderInfo>    

// intermediate data for converting bookmark file
type FolderEntry = int * int * FolderInfo
type MarkEntry = int * int * MarkInfo

/// intermediate data with primaryId and parentId
type Entry =
    | FolderEntry of FolderEntry
    | MarkEntry of MarkEntry

/// raw data parsed from a bookmark file line
type BookmarkLine =
    | Folder of FolderInfo
    | Mark of MarkInfo
    | ListOpen 
    | ListClose
    
type DocumentLine =
    | FolderTag of string
    | MarkTag of string
    | ListOpenTag of string
    | ListCloseTag of string
 
/// Tree collection functions   
module Tree =
    
    open MergeBookmark.Utility    
    open System

    let rec cata fLeaf fNode (tree:Tree<'LeafData,'INodeData>) :'r =
        let recurse = cata fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            fLeaf leafInfo
        | InternalNode (nodeInfo,subtrees) ->
            fNode nodeInfo (subtrees |> Seq.map recurse)

    let rec fold fLeaf fNode acc (tree:Tree<'LeafData,'INodeData>) :'r =
        let recurse = fold fLeaf fNode
        Log.green (sprintf $"fold: {acc}")
        match tree with
        | LeafNode leafInfo ->
            fLeaf acc leafInfo
        | InternalNode (nodeInfo,subtrees) ->
            // determine the local accumulator at this level
            let localAccum = fNode acc nodeInfo
            // thread the local accumulator through all the subitems using Seq.fold
            let finalAccum = subtrees |> Seq.fold recurse localAccum
            // ... and return it
            finalAccum
            
    let rec map fLeaf fNode (tree:Tree<'LeafData,'INodeData>) =
        let recurse = map fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            let newLeafInfo = fLeaf leafInfo
            LeafNode newLeafInfo
        | InternalNode (nodeInfo,subtrees) ->
            let newNodeInfo = fNode nodeInfo
            let newSubtrees = subtrees |> Seq.map recurse
            InternalNode (newNodeInfo, newSubtrees)
            
    let rec iter fLeaf fNode (tree:Tree<'LeafData,'INodeData>) =
        let recurse = iter fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            fLeaf leafInfo
        | InternalNode (nodeInfo,subtrees) ->
            subtrees |> Seq.iter recurse
            fNode nodeInfo
               
    let prettyPrint (tree:BookmarkTree) =
        let rec loop depth (tree:BookmarkTree) =
            let spacer = String(' ', depth * 4)
            match tree with
            | LeafNode m ->        
                printfn "%s |- %A" spacer m.name
            | InternalNode (f,subtrees) ->
                printfn "%s | %s" spacer f.name
                subtrees |> Seq.iter (loop (depth + 1)) 
        loop 0 tree