using System;
using UnityEngine;


public struct RunningAverage
{
    private readonly Vector3[] _buffer;
    private int _count;
    private int _next;


    private Vector3 average(Vector3[] buffer, int count)
    {
        Vector3 sum = default;

        for (var i = 0; i < count; i++)
        {
            sum += buffer[i];
        }

        return sum / count;
    }


    public RunningAverage(int frames)
    {
        _buffer = new Vector3[frames];
        _count = 0;
        _next = 0;
    }


    public void Add(Vector3 val)
    {
        _buffer[_next] = val;
        _count = Math.Min(_count + 1, _buffer.Length);
        _next = (_next + 1) % _buffer.Length;
    }


    public Vector3 Get()
    {
        return average(_buffer, _count);
    }
}