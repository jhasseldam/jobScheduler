module jobScheduler
open System

type Seconds = int
type Job = {
    Id : int
    Duration : Seconds
    HasPriority : bool
    Submitted : DateTime
}
type Queue = Job []

// We have to use a dummy that all entrances of the array/the queue can point
// to when first allocating the queue.
let dummyJob = {
    Id          = 0;
    Duration    = 0;
    HasPriority = false;
    Submitted   = DateTime.Now
}

(* Initialize a queue as an array with every entrance pointing to the record
 * dummyJob. *)
let init_queue (size : int) : Queue = Array.create size dummyJob

(** The Queue for normal jobs without priority **)
let mutable SIZE_QUEUE  = 4
let mutable QUEUE       = init_queue SIZE_QUEUE
let mutable FST_INDEX_Q = 0
let mutable LST_INDEX_Q = 0
let mutable ELEMENTS_Q  = 0

(** The Queue for priority jobs **)
let mutable SIZE_PRIORITY      = 4
let mutable QUEUE_PRIORITY     = init_queue SIZE_PRIORITY
let mutable FST_INDEX_PRIORITY = 0
let mutable LST_INDEX_PRIORITY = 0
let mutable ELEMENTS_PRIORITY  = 0

(* Next step in the implementation would be to make each queue into a record *)
//type Queue = {
//    mutable Size : int
//    mutable Queue : Queue
//    mutable Fst_index : int
//    mutable Lst_index : int
//}

(* Immutable variables *)
let FIVE_SECONDS = 5
let SECONDS_TO_MILISECONDS = 1000

(***********************************)
(** Functions for scheduling jobs **)
(***********************************)

(* Double the size of the queue, by building a new queue consisting of the old
 * queue and an empty queue of the same size. *)
let increaseSizeQueue (queue : outref<Queue>) (size : byref<int>) : unit =
    let extraSpace = init_queue size in
    queue <- Array.append queue extraSpace
    size  <- size * 2

(* Enqueue a job at the end of a queue.
 * If the queue is filled the queue size is doubled *)
let enqueue (queue : outref<Queue>) (job : Job) (lstIndex : byref<int>)
            (size : byref<int>) (numberOfElements : outref<int>) : unit =
    queue.[lstIndex] <- job
    lstIndex <- lstIndex + 1
    numberOfElements <- numberOfElements + 1
    // If the queue is filled we double the size of the queue
    if lstIndex > size - 1 then
        increaseSizeQueue &queue &size
    else ()

(* Schedule a job by enqueing it in the priority queue or the normal queue *)
let scheduleJob (job : Job) : unit =
    if job.HasPriority then
        enqueue &QUEUE_PRIORITY job &LST_INDEX_PRIORITY &SIZE_PRIORITY
                &ELEMENTS_PRIORITY
    else
        enqueue &QUEUE job &LST_INDEX_Q &SIZE_QUEUE &ELEMENTS_Q
    printfn "Scheduled Job with id %d" job.Id


(********************************)
(** Functions for running jobs **)
(********************************)

(* Get the age of a specific job in Seconds *)
let getAge (job : Job) : Seconds = (DateTime.Now - job.Submitted).Seconds

(* Run a job. The execution time depends on the duration of the job. *)
let runJob (job : Job) : unit =
    printfn "Running job %d" job.Id
    let milisecs = job.Duration * SECONDS_TO_MILISECONDS in
    Threading.Thread.Sleep milisecs |> ignore // ignore return value from Sleep

(* Dequeue a job by removing it.
 * If there have been dequed the number of jobs equal to the size of a full
 * queue, we circle around and index from 0 again. This is done to exploit the
 * memory allocated for the queue. *)
let dequeue (queue : outref<Queue>) (fstIndex : byref<int>)
 (lstIndex : byref<int>) (size : byref<int>) (noOfElements : outref<int>): unit =
    queue.[fstIndex] <- dummyJob
    fstIndex <- fstIndex + 1
    noOfElements <- noOfElements - 1
    // Start queue from index 0 again (circular), if the number of jobs run are
    // equal to the size of the array.
    if fstIndex = lstIndex && fstIndex = size - 1 then
        fstIndex <- 0
        lstIndex <- 0
    else ()

(* Run the next job according to the following rules:
 * 1. If there is any job older than 5 seconds, it should run.
 * 2. If there are priority jobs, the oldest should run before any normal job.
 * 3. The oldest submitted job should run. *)
let runNextJob unit : unit =
    let queueIsEmpty : bool = FST_INDEX_Q = LST_INDEX_Q
    let priorityQueueIsEmpty : bool = FST_INDEX_PRIORITY = LST_INDEX_PRIORITY
    let jobPriority = QUEUE_PRIORITY.[FST_INDEX_PRIORITY]
    let job  = QUEUE.[FST_INDEX_Q]

    // Checking if queues are empty
    if queueIsEmpty && priorityQueueIsEmpty then
        printfn "No jobs to run"
    // If the normal queue is empty we run a priority job
    elif queueIsEmpty then
        runJob jobPriority
        dequeue &QUEUE_PRIORITY &FST_INDEX_PRIORITY &LST_INDEX_PRIORITY
                &SIZE_PRIORITY &ELEMENTS_PRIORITY
    // If the priority queue is empty we run a normal job
    elif priorityQueueIsEmpty then
        runJob job
        dequeue &QUEUE &FST_INDEX_Q &LST_INDEX_Q &SIZE_QUEUE &ELEMENTS_Q
    // Otherwise we have both a priority job and a normal job
    else
        let jobAge  : Seconds = getAge job
        let jobPAge : Seconds = getAge jobPriority

        // If there is any job older than 5 seconds it gets executed
        if jobAge > FIVE_SECONDS || jobPAge > FIVE_SECONDS then
            if jobPAge >= jobAge then
                runJob jobPriority
                dequeue &QUEUE_PRIORITY &FST_INDEX_PRIORITY &LST_INDEX_PRIORITY
                        &SIZE_PRIORITY &ELEMENTS_PRIORITY
            else
                runJob job
                dequeue &QUEUE &FST_INDEX_Q &LST_INDEX_Q &SIZE_QUEUE &ELEMENTS_Q
        // If we both have a normal and a priority job younger than 5 seconds we
        // always run the priority job.
        else
            runJob jobPriority
            dequeue &QUEUE_PRIORITY &FST_INDEX_PRIORITY &LST_INDEX_PRIORITY
                    &SIZE_PRIORITY &ELEMENTS_PRIORITY

