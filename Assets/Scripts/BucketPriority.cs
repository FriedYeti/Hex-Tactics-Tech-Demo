using System;
using System.Collections.Generic;

public class BucketPriority<T> {
    private List<List<T>> queue;
    private readonly bool isMonotonic;
    private int monotonicIndex;
    private int queueCount = 0;

    public BucketPriority(int approxMaxSize, bool isMonotonic = false) {
        queue = new List<List<T>>();
        this.isMonotonic = isMonotonic;
        monotonicIndex = 0;
        for (int i = 0; i < approxMaxSize; i++) {
            List<T> newList = new List<T>();
            queue.Add(newList);
        }
    }

    public void Enqueue(int priority, T newData) {
        while (queue.Count <= priority) {
            List<T> newList = new List<T>();
            queue.Add(newList);
        }
        if (isMonotonic && priority < monotonicIndex) {
            throw new ArgumentException("Cannot add a lower priority than has been removed from a monotonic priority");
        }
        queueCount++;
        queue[priority].Add(newData);
    }

    public T Dequeue() {
        if (queueCount < 1) { return default(T); }
        if (isMonotonic) {
            while (queue[monotonicIndex].Count < 1) {
                monotonicIndex++;
            }
            T temp = queue[monotonicIndex][queue[monotonicIndex].Count - 1];
            queue[monotonicIndex].RemoveAt(queue[monotonicIndex].Count - 1);
            queueCount--;
            return temp;
        }
        else {
            int priorityIndex = 0;
            while (queue[priorityIndex].Count < 1) {
                priorityIndex++;
            }
            T temp = queue[priorityIndex][queue[priorityIndex].Count - 1];
            queue[priorityIndex].RemoveAt(queue[priorityIndex].Count - 1);
            queueCount--;
            return temp;
        }
    }

    public T Peak() {
        if (queueCount < 1) { return default(T); }
        if (isMonotonic) {
            while (queue[monotonicIndex].Count < 1) {
                monotonicIndex++;
            }
            T temp = queue[monotonicIndex][queue[monotonicIndex].Count - 1];
            return temp;
        }
        else {
            int priorityIndex = 0;
            while (queue[priorityIndex].Count < 1) {
                priorityIndex++;
            }
            T temp = queue[priorityIndex][queue[priorityIndex].Count - 1];
            return temp;
        }

    }

    public bool Empty() {
        return queueCount == 0;
    }
}