namespace Lochs.Challenges;

public class DoublyLinkedList
{
    private readonly string _value;
    private DoublyLinkedList _previous;
    private DoublyLinkedList _next;

    public DoublyLinkedList(string value, DoublyLinkedList previous, DoublyLinkedList next)
    {
        _value = value;
        _previous = previous;
        _next = next;
        _previous.SetNext(this);
        _next.SetPrevious(this);
    }
    
    public DoublyLinkedList Previous() 
        => _previous;
    
    public void SetPrevious(DoublyLinkedList previous)
        => _previous = previous;
    
    public DoublyLinkedList Next() 
        => _next;

    public void SetNext(DoublyLinkedList next)
        => _next = next;

    public bool MatchesValue(string value)
        => _value == value;
}

public class ListHelper
{
    private DoublyLinkedList _head;

    public ListHelper()
    {
        _head = new DoublyLinkedList(null, null, null);
    }
    
    public DoublyLinkedList Insert(string value, string after)
    {
        throw new NotImplementedException();
    }

    private DoublyLinkedList Find(string value)
    {
        if (_head.MatchesValue(value))
        {
            return _head;
        }

        DoublyLinkedList next = _head.Next();

        while (next != null)
        {
            if (next.MatchesValue(value))
            {
                return next;
            }

            next = next.Next();
        }

        return null;
    }
}