using System.Collections.Generic;

public interface IDeckManager
{
    void ShuffleDeck();
    List<int> DealCards();
}
