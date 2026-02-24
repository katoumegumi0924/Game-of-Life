using System;
using UnityEngine;

public interface IPoolElement
{
    int ID { get; set; }
    void Reset();

    void Export(System.IO.BinaryWriter w);
    void Import(System.IO.BinaryReader r);
}

public class DataPool<T> where T : struct, IPoolElement
{
    public T[] buffer;
    public int cursor = 1;
    public int capacity = 0;
    public int[] recycleIds;
    public int recycleCursor = 0;

    public int count { get { return cursor - recycleCursor - 1; } }

    public void Reset()
    {
        buffer = new T[8];
        cursor = 1;
        capacity = 8;
        recycleIds = new int[8];
        recycleCursor = 0;
    }

    public void Free()
    {
        buffer = null;
        recycleIds = null;
        cursor = 1;
        recycleCursor = 0;
        capacity = 0;
    }

    public void SetCapacity(int newCap)
    {
        if (newCap < 0)
            capacity = 0;
        if (capacity == newCap)
            return;
        if (newCap > 0 && newCap < cursor)
            return;
        T[] oldArr = buffer;
        int[] oldRcy = recycleIds;
        if (newCap > 0)
        {
            buffer = new T[newCap];
            recycleIds = new int[newCap];
        }
        else
        {
            buffer = null;
            recycleIds = null;
        }
        if (oldArr != null && buffer != null)
        {
            Array.Copy(oldArr, buffer, newCap > capacity ? capacity : newCap);
            if (recycleCursor > 0)
                Array.Copy(oldRcy, recycleIds, newCap > capacity ? capacity : newCap);
        }
        capacity = newCap;
    }

    public ref T Add(out int id)
    {
        if (recycleCursor > 0)
        {
            id = recycleIds[--recycleCursor];
            if (id == 0)
                throw new Exception("Pool recycle array was corrupted!");
        }
        else
        {
            id = cursor++;
            if (id > 0x0fffffff)
                throw new Exception("Pool max size reached");
            int _cap = capacity;
            while (id >= _cap)
            {
                if (_cap < 8)
                    _cap = 8;
                else
                    _cap = _cap * 2;
            }
            if (_cap != capacity)
                SetCapacity(_cap);
        }
        buffer[id].Reset();
        buffer[id].ID = id;
        return ref buffer[id];
    }

    public void Remove(int id)
    {
        if (id <= 0 || id >= cursor)
            return;

        if (buffer[id].ID != 0)
        {
            recycleIds[recycleCursor++] = id;
            buffer[id].Reset();
            buffer[id].ID = 0;
        }

        if (count <= 0)
        {
            cursor = 1;
            recycleCursor = 0;
        }
    }

    public void Flush()
    {
        if (buffer != null)
        {
            int maxId = 0;
            for (int i = cursor - 1; i >= 1; --i)
            {
                if (buffer[i].ID == i)
                {
                    maxId = i;
                    break;
                }
            }
            if (maxId == 0)
            {
                Reset();
                return;
            }
            cursor = maxId + 1;

            int slow = 0;
            for (int fast = 0; fast < recycleCursor; ++fast)
            {
                if (recycleIds[fast] < cursor)
                {
                    recycleIds[slow] = recycleIds[fast];
                    ++slow;
                }
            }
            recycleCursor = slow;
            Array.Clear(recycleIds, recycleCursor, capacity - recycleCursor);

            int _cap = 0;
            while (maxId >= _cap)
            {
                if (_cap < 8)
                    _cap = 8;
                else
                    _cap = _cap * 2;
            }
            if (_cap != capacity)
                SetCapacity(_cap);
        }
    }

    // Ë÷ÒýÆ÷
    public ref T this[int index]
    {
        get
        {
            return ref buffer[index];
        }
    }

    public void Export(System.IO.BinaryWriter w)
    {
        w.Write(capacity);
        w.Write(cursor);
        w.Write(recycleCursor);
        for (int i = 1; i < cursor; ++i)
            buffer[i].Export(w);
        for (int i = 0; i < recycleCursor; ++i)
            w.Write(recycleIds[i]);
    }

    public void Import(System.IO.BinaryReader r)
    {
        SetCapacity(r.ReadInt32());
        cursor = r.ReadInt32();
        recycleCursor = r.ReadInt32();
        for (int i = 1; i < cursor; ++i)
            buffer[i].Import(r);
        for (int i = 0; i < recycleCursor; ++i)
            recycleIds[i] = r.ReadInt32();
    }
}
