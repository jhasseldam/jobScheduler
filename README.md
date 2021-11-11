# jobScheduler
Made by Camilla Krogh Dalsgaard

## Compile
$ fsharpc jobScheduler.fs testing.fsx

## Execute in Mono runtime
$ mono testing.exe


## The Datastructure
The scheduled jobs are put in two different queues. One queue for jobs with
priority and one for jobs without priority.

Each of the queues are implemented as an array of jobs. Both are global
accessible, since both `runNextJob` and `scheduleJob` does not take the data
structure as parameter.

A job is a record with the required fields.


Both queues have size 4 when starting. Both the index for the first and last
element is 0:

first
last
```--------------------
|   |    |    |    |
--------------------```


If the queue is filled, meaning there are enqued 4 elements, we double the size
of the queue:

first                    last    first                   last
------------------------         --------------------------------------------
|Job | Job | Job | Job |     ==> |Job | Job | Job | Job |    |    |    |    |
------------------------          --------------------------------------------


If we first fill 3 jobs into the queue, and then run these 3 jobs, we move both
indexes back to 0 to utilize the memory allocated:

                                                     first     first
first             last                               last      last
----------------------- runNextJob () x3 -----------------      -----------------
|Job | Job | Job |    |    ==>           |   |   |   |   |  =>  |   |   |   |   |
-----------------------                  -----------------      -----------------


This choice of datastructure is made because it takes constant time O(1) to
enqueue and dequeue in an array.
If we have to double the size of the queue it takes O(size) time.

## Comments
It's clearly not pretty that the queues initially is filled with a dummy job,
and there probably would be a better way of solving this part.

Furthermore I think it is errorprone to pass arguments by reference like I have
done here. But the datastructures need to be global accessible, and I didn't
want to write the same piece of code for both queues, so I decided to pass by
reference.


## Future Work
- Implement a queue as a record:

type Queue = {
    mutable Size : int
    mutable Queue : Queue
    mutable Fst_index : int
    mutable Lst_index : int
}


- Consider having the priority queue and the other queue in a tuple:

    (PRIORITY_QUEUE, QUEUE)

- Implement halving the size of the queue when some threshold is reached

- Test further. Make a better framework with helper functions for testing



