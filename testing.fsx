open jobScheduler
open System

printfn "########################################"
printfn "##### Testing of the Job Scheduler #####"
printfn "########################################"


printfn "\n## Insert 3 jobs without priority"
for i = 0 to SIZE_QUEUE - 2 do
    let j1 = {Id = i+1; Duration = 1; HasPriority = false; Submitted = DateTime.Now}
    scheduleJob j1
printfn "ELEMENTS_Q  = 3:\t %b" (ELEMENTS_Q = 3)


printfn "\n## Remove 3 jobs without priority"
printfn "# Check we start from index 0 in queue again"
for i = 0 to SIZE_QUEUE - 2 do
    runNextJob ()
printfn "ELEMENTS_Q  = 0:\t %b" (ELEMENTS_Q = 0)
printfn "FST_INDEX_Q = 0:\t %b" (FST_INDEX_Q = 0)
printfn "LST_INDEX_Q = 0:\t %b" (LST_INDEX_Q = 0)


printfn "\n## Check we don't any jobs when queues are empty"
runNextJob ()


printfn "\n## Fill queue with normal jobs. Check we get a queue of double size"
for i = 0 to SIZE_QUEUE - 1 do
    scheduleJob {Id = i+1; Duration = 1; HasPriority = false; Submitted = DateTime.Now}
printfn "ELEMENTS_Q = 4:\t\t %b" (ELEMENTS_Q = 4)
printfn "SIZE_QUEUE = 8:\t\t %b" (SIZE_QUEUE = 8)


printf "\n## Fill the new queue with normal jobs. "
printfn "Check we get a queue of 4 times the size of the original queue"
for i = 4 to SIZE_QUEUE - 1 do
    scheduleJob {Id = i+1; Duration = 1; HasPriority = false; Submitted = DateTime.Now}
printfn "ELEMENTS_Q = 8:\t\t %b" (ELEMENTS_Q = 8)
printfn "SIZE_QUEUE = 16:\t %b" (SIZE_QUEUE = 16)


Threading.Thread.Sleep 1000
printfn "\n## Schedule 3 priority jobs. Check they go in the priority queue"
for i = 0 to SIZE_PRIORITY - 2 do
    let j2 = {Id = i+100; Duration = 1; HasPriority = true;  Submitted = DateTime.Now}
    scheduleJob j2
printfn "ELEMENTS_PRIORITY = 3:\t %b" (ELEMENTS_PRIORITY = 3)
printfn "SIZE_PRIORITY = 4:\t %b" (SIZE_PRIORITY = 4)


printfn "\n## Check priority jobs are run before unprioritized jobs"
runNextJob ()
printfn "ELEMENTS_PRIORITY = 2:\t %b" (ELEMENTS_PRIORITY = 2)
printfn "ELEMENTS_Q = 8:\t\t %b" (ELEMENTS_Q = 8)
runNextJob ()
printfn "ELEMENTS_PRIORITY = 1:\t %b" (ELEMENTS_PRIORITY = 1)
printfn "ELEMENTS_Q = 8:\t\t %b" (ELEMENTS_Q = 8)


printfn "\n## Check oldest job older than 5 seconds are run first"
Threading.Thread.Sleep 3000
printfn "%A" QUEUE.[FST_INDEX_Q]
printfn "%A" QUEUE_PRIORITY.[FST_INDEX_PRIORITY]
runNextJob ()
printfn "ELEMENTS_PRIORITY = 1:\t %b" (ELEMENTS_PRIORITY = 1)
printfn "ELEMENTS_Q = 7:\t\t %b" (ELEMENTS_Q = 7)


