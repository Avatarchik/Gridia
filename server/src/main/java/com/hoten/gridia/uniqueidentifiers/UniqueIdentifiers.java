package com.hoten.gridia.uniqueidentifiers;

import java.util.Queue;
import java.util.concurrent.LinkedBlockingQueue;

public class UniqueIdentifiers {

    protected int _nextNewId = 1;
    protected int _expandAmount;
    protected final Queue<Integer> _available = new LinkedBlockingQueue();

    public UniqueIdentifiers(int expandAmount) {
        _expandAmount = expandAmount;
    }

    public synchronized int next() {
        if (_available.isEmpty()) {
            expand();
        }
        return _available.remove();
    }

    public synchronized void retire(int id) {
        _available.add(id);
    }

    private void expand() {
        for (int i = 0; i < _expandAmount; i++) {
            _available.add(_nextNewId++);
        }
    }
}
