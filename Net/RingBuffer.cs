using System;
using System.Collections.Generic;

public sealed class RingBuffer {

    private int _head;
    private int _tail;
    private int _size;
    private int _sizeUntilCut;
    private byte[] _buffer;

    public int Count {
        get {
            return _size;
        }
    }

    public int FreeCount {
        get {
            return _buffer.Length - _size;
        }
    }

    public int Capacity {
        get {
            return _buffer.Length;
        }
        set {
            SetCapacity( value );
        }
    }

    public RingBuffer( int capacity ) {
        _buffer = new byte[capacity];
    }

    public void Clear() {
        _head = 0;
        _tail = 0;
        _size = 0;
        _sizeUntilCut = _buffer.Length;
    }

    public void Clear( int size ) {
        if ( size > _size ) {
            size = _size;
        }
        if ( size == 0 ) {
            return;
        }
        _head = ( _head + size ) % _buffer.Length;
        _size -= size;
        if ( _size == 0 ) {
            _head = 0;
            _tail = 0;
        }
        _sizeUntilCut = _buffer.Length - _head;
    }

    void SetCapacity( int capacity ) {
        if ( capacity != _buffer.Length ) {
            byte[] newBuffer = new byte[capacity];
            if ( _size > 0 ) {
                if ( _head < _tail ) {
                    Buffer.BlockCopy( _buffer, _head, newBuffer, 0, _size );
                } else {
                    Buffer.BlockCopy( _buffer, _head, newBuffer, 0, _buffer.Length - _head );
                    Buffer.BlockCopy( _buffer, 0, newBuffer, _buffer.Length - _head, _tail );
                }
            }
            _head = 0;
            _tail = _size;
            _buffer = newBuffer;
        }
    }

    public void Write( byte[] buffer, int size ) {
        if ( size == 0 ) {
            return;
        }
        size = Math.Min( size, buffer.Length );
        if ( ( _size + size ) > _buffer.Length ) {
            var newSize = _size + size;
            newSize = ( ( newSize + 3 ) & ~3 ); // 4 bytes aligned
            SetCapacity( newSize );
        }

        if ( _head < _tail ) {
            int rightLength = ( _buffer.Length - _tail );
            if ( rightLength >= size ) {
                Buffer.BlockCopy( buffer, 0, _buffer, _tail, size );
            } else {
                Buffer.BlockCopy( buffer, 0, _buffer, _tail, rightLength );
                Buffer.BlockCopy( buffer, rightLength, _buffer, 0, size - rightLength );
            }
        } else {
            Buffer.BlockCopy( buffer, 0, _buffer, _tail, size );
        }

        _tail = ( _tail + size ) % _buffer.Length;
        _size += size;
        _sizeUntilCut = _buffer.Length - _head;
    }

    public int Read( byte[] buffer, int size ) {
        if ( size > _size ) {
            size = _size;
        }
        if ( size == 0 ) {
            return 0;
        }
        size = Math.Min( size, buffer.Length );
        if ( _head < _tail ) {
            Buffer.BlockCopy( _buffer, _head, buffer, 0, size );
        } else {
            int rightLength = ( _buffer.Length - _head );
            if ( rightLength >= size ) {
                Buffer.BlockCopy( _buffer, _head, buffer, 0, size );
            } else {
                Buffer.BlockCopy( _buffer, _head, buffer, 0, rightLength );
                Buffer.BlockCopy( _buffer, 0, buffer, rightLength, size - rightLength );
            }
        }
        _head = ( _head + size ) % _buffer.Length;
        _size -= size;
        if ( _size == 0 ) {
            _head = 0;
            _tail = 0;
        }
        _sizeUntilCut = _buffer.Length - _head;
        return size;
    }

    public byte this[int index] {
        get {
            return index >= _sizeUntilCut
                ? _buffer[index - _sizeUntilCut]
                : _buffer[_head + index];
        }
    }
}
