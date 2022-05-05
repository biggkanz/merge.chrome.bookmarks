module MergeBookmark.Domain

type Tree<'LeafData,'INodeData> =
    | LeafNode of 'LeafData
    | InternalNode of 'INodeData * Tree<'LeafData,'INodeData> seq
    
type Folder = {name:string; date:string; modified:string}
type Mark = {name:string; href:string; icon:string}

type BookmarkTree = Tree<Mark,Folder>    

// intermediate data for converting bookmark file
type FolderEntry = int * int * Folder
type MarkEntry = int * int * Mark

/// intermediate data with primaryId and parentId
type Entry =
    | FolderEntry of FolderEntry
    | MarkEntry of MarkEntry

/// raw data parsed from a bookmark file line
type BookmarkLine =
    | Folder of Folder
    | Mark of Mark
    | ListOpenTag 
    | ListCloseTag    
 
/// Tree collection functions   
module Tree =
    
    open MergeBookmark.Utility

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