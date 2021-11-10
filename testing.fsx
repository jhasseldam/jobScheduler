open jobScheduler
open System

printfn "##### Testing of the Job Scheduler #####"

let dummyJob = { Id = 0; Duration = 0; HasPriority = false; Submitted = DateTime.Now }
let j1 = {Id = 1; Duration = 3; HasPriority = false; Submitted = DateTime.Now}
let j2 = {Id = 2; Duration = 3; HasPriority = true; Submitted = DateTime.Now}
printfn "%A" <| (j1.Submitted > j2.Submitted)

printfn "###  Testing scheduleJob (job : Job) : unit ###"

scheduleJob j1
for int i = 0 to 5 do
    scheduleJob j2
scheduleJob j1
printfn "QUEUE %A\n" QUEUE
printfn "PRIORITY %A" QUEUE_PRIORITY

let vars = [SIZE_QUEUE; FST_INDEX_Q; LST_INDEX_Q; SIZE_PRIORITY; FST_INDEX_PRIORITY; LST_INDEX_PRIORITY]
for var in vars do
    printfn "%A" var

