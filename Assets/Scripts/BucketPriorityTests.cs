using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BucketPriorityTests : MonoBehaviour {
    private bool testsPassed = true;
    private bool orderedTestPass = false;
    private bool unorderedTestPass = false;
    private bool orderedSetTestPass = false;
    private bool unorderedSetTestPass = false;

    void Start () {
        orderedTestPass = OrderedTest();
        Debug.Log("Ordered Test Passed: " + orderedTestPass);
        unorderedTestPass = UnorderedTest();
        Debug.Log("Unordered Test Passed: " + unorderedTestPass);
        orderedSetTestPass = OrderedSetTest();
        Debug.Log("Ordered Set Test Passed: " + orderedSetTestPass);
        unorderedSetTestPass = UnorderedSetTest();
        Debug.Log("Unordered Set Test Passed: " + unorderedSetTestPass);


        testsPassed = testsPassed && orderedTestPass;
        testsPassed = testsPassed && unorderedTestPass;
        testsPassed = testsPassed && orderedSetTestPass;
        testsPassed = testsPassed && unorderedSetTestPass;

        Debug.Log("All Tests Passed: " + testsPassed);
    }


    private bool OrderedTest() {
        // objects are enqueued in order of priority and should be output in the same order
        bool isMonotonicTest = false;
        BucketPriority<string> bucket = new BucketPriority<string>(100, isMonotonicTest);
        List<string> expectedResults = new List<string>();
        List<string> results = new List<string>();

        System.Random rng = new System.Random();

        for (int i = 0; i < 100; i++) {
            int k = rng.Next();
            bucket.Enqueue(i, k.ToString());
            expectedResults.Add(k.ToString());
        }

        while(bucket.Peak() != null) {
            results.Add(bucket.Dequeue());
        }
        return results.SequenceEqual(expectedResults);
    }

    private bool UnorderedTest() {
        // objects are enqueued regardless of priority, but should be output in order of priority
        bool isMonotonicTest = false;
        BucketPriority<string> bucket = new BucketPriority<string>(100, isMonotonicTest);
        List<string> expectedResults = new List<string>();
        List<string> results = new List<string>();

        List<int> bucketPriorities = new List<int>();
        List<int> bucketValues = new List<int>();

        System.Random rng = new System.Random();

        // Generate two lists
        for (int i = 0; i < 100; i++) {
            bucketPriorities.Add(i);
            int k = rng.Next();
            bucketValues.Add(k);
            expectedResults.Add(k.ToString());
        }

        // Fisher-Yates shuffle the lists, keeping indices matching
        for(int i  = bucketPriorities.Count - 1; i > 0; i--) {
            int k = rng.Next(i + 1);
            int prioTemp = bucketPriorities[k];
            int valuTemp = bucketValues[k];
            bucketPriorities[k] = bucketPriorities[i];
            bucketValues[k] = bucketValues[i];
            bucketPriorities[i] = prioTemp;
            bucketValues[i] = valuTemp;
        }

        for (int i = 0; i < bucketPriorities.Count; i++) {
            bucket.Enqueue(bucketPriorities[i], bucketValues[i].ToString());
        }

        for (int i = 0; i < bucketPriorities.Count; i++) {
            results.Add(bucket.Dequeue());
        }

        return results.SequenceEqual(expectedResults);
    }

    private bool OrderedSetTest() {
        // objects are enqueued in order of priority (with more than one object allowed in each priority) and should be output in order of priority, with priorities behaving like a stack
        bool isMonotonicTest = false;
        BucketPriority<string> bucket = new BucketPriority<string>(100, isMonotonicTest);
        List<string> expectedResults = new List<string>();
        List<string> results = new List<string>();

        System.Random rng = new System.Random();

        for (int i = 0; i < 100; i++) {
            int k = rng.Next();
            bucket.Enqueue(i, k.ToString());
            expectedResults.Add(k.ToString());
            if(i % 3 == 0) {
                k = rng.Next();
                bucket.Enqueue(i, k.ToString());
                expectedResults.Add(k.ToString());
            }
        }

        while (bucket.Peak() != null) {
            results.Add(bucket.Dequeue());
        }
        return results.SequenceEqual(expectedResults);
    }

    private bool UnorderedSetTest() {
        // objects are enqueued regaredless of priority (with more than one object allowed in each priority), but should be output in order of priority, with priorities behaving like a stack
        bool isMonotonicTest = false;
        BucketPriority<string> bucket = new BucketPriority<string>(100, isMonotonicTest);
        List<string> expectedResults = new List<string>();
        List<string> results = new List<string>();

        List<int> bucketPriorities = new List<int>();
        List<int> bucketValues = new List<int>();

        // Generate two lists of 0 - 99 and expected results

        System.Random rng = new System.Random();

        for (int i = 0; i < 100; i++) {
            bucketPriorities.Add(i);
            int k = rng.Next();
            bucketValues.Add(k);
            expectedResults.Add(k.ToString());
            if(i % 3 == 0) {
                bucketPriorities.Add(i);
                k = rng.Next();
                bucketValues.Add(k);
                expectedResults.Add(k.ToString());
            }
        }

        // Fisher-Yates shuffle the lists, keeping indices matching
        for (int i = bucketPriorities.Count - 1; i > 0; i--) {
            int k = rng.Next(i + 1);
            int prioTemp = bucketPriorities[k];
            int valuTemp = bucketValues[k];
            bucketPriorities[k] = bucketPriorities[i];
            bucketValues[k] = bucketValues[i];
            bucketPriorities[i] = prioTemp;
            bucketValues[i] = valuTemp;
        }

        for (int i = 0; i < bucketPriorities.Count; i++) {
            bucket.Enqueue(bucketPriorities[i], bucketValues[i].ToString());
        }

        for (int i = 0; i < bucketPriorities.Count; i++) {
            results.Add(bucket.Dequeue());
        }

        return results.SequenceEqual(expectedResults);
    }
}

