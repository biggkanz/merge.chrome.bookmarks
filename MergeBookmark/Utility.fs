namespace MergeBookmark.Utility

module Log =
    
    open System
    open System.Threading
    
    /// Print a colored log message
    let message =
        let lockObj = obj()
        fun (color : ConsoleColor) (message : string ) ->
            lock lockObj (fun () ->
                Console.ForegroundColor <- color
                printfn $"%s{message} (thread ID: %i{Thread.CurrentThread.ManagedThreadId})"
                Console.ResetColor())
    
    /// Print a red log message.
    let red = message ConsoleColor.Red
    let green = message ConsoleColor.Green
    let yellow = message ConsoleColor.Yellow
    let cyan  = message ConsoleColor.Cyan

module Time =
    
    open System

    /// convert UnixTimeSeconds to System.DateTime object
    let ToDateTime (timestamp:int64) =
        let offset = DateTimeOffset.FromUnixTimeSeconds timestamp
        offset.DateTime
        
    /// convert System.DateTime object to UnixTimeSeconds
    let ToUnixTimeSeconds (datetime:DateTime) =
        let epoch = DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)
        datetime.Subtract(epoch).TotalSeconds
        |> int64
        
module IO =
    
    open System.IO

    let ReadAllLines file =
        Log.green (sprintf $"ReadAllLines: {file}")
        try
            File.ReadAllLines file
        with
        | ex -> failwith $"error in MergeBookmark.IO.ReadAllLines: {file}"
        
    let WriteAllLines file lines =
        Log.green (sprintf $"WriteAllLines: {file}")        
        File.WriteAllLines (file, lines)
        
[<AutoOpen>]
module StringBuffer =
        open System.Text

        type StringBuffer = StringBuilder -> unit

        type StringBufferBuilder () =
            member inline _.Yield (txt: string) = fun (b: StringBuilder) -> Printf.bprintf b $"%s{txt}"
            member inline _.Yield (c: char) = fun (b: StringBuilder) -> Printf.bprintf b $"%c{c}"
            member inline _.Yield (strings: #seq<string>) =
                fun (b: StringBuilder) -> for s in strings do Printf.bprintf b $"%s{s}\n"
            member inline _.YieldFrom (f: StringBuffer) = f
            member _.Combine (f, g) = fun (b: StringBuilder) -> f b; g b
            member _.Delay f = fun (b: StringBuilder) -> (f()) b
            member _.Zero () = ignore
            
            member _.For (xs: 'a seq, f: 'a -> StringBuffer) =
                fun (b: StringBuilder) ->
                    use e = xs.GetEnumerator ()
                    while e.MoveNext() do
                        (f e.Current) b
            
            member _.While (p: unit -> bool, f: StringBuffer) =
                fun (b: StringBuilder) -> while p () do f b
                
            member _.Run (f: StringBuffer) =
                let b = StringBuilder()
                do f b
                b.ToString()

        let stringBuffer = new StringBufferBuilder ()
        
        type StringBufferBuilder with
        member inline _.Yield (b: byte) = fun (sb: StringBuilder) -> Printf.bprintf sb $"%02x{b} "