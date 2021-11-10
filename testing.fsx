open jobScheduler
open System

printfn "##### Testing of the Job Scheduler #####"

let dummyJob = { Id = 0; Duration = 0; HasPriority = false; Submitted = DateTime.Now }
let j1 = {Id = 1; Duration = 3; HasPriority = true; Submitted = DateTime.Now}
let j2 = {Id = 2; Duration = 3; HasPriority = true; Submitted = DateTime.Now}
printfn "%A" <| (j1.Submitted > j2.Submitted)

printfn "###  Testing scheduleJob (job : Job) : unit ###"

scheduleJob j1
scheduleJob j2
scheduleJob j2
scheduleJob j2
printfn "%A" QUEUE
